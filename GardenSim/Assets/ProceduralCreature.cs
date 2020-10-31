using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class Creature
{
    private MeshFilter m;
    private MeshRenderer r;
    private BoxCollider b;
    private SphereCollider s;

    private uint _seed;
    private string _sequence;
    private float _scale;
    private BodyShape _bodyShape;
    private bool _isFlying;
    private GameObject _root;
    private Color _color;
    private float _speed;
    private Vector3 _lastMove;
    private Rigidbody _rigidBody;
    private float _mass;

    private const float MinimumMass = 0.15f;

    public Creature(GameObject parent = null)
    {
        _seed = (uint)Mathf.Floor(Random.value * uint.MaxValue);
        _sequence = Convert.ToString(_seed, 2);
        _scale = Mathf.Clamp(ReadAsNormalizedFloat(13), 0.1f, 1.0f);
        _bodyShape = ReadAsEnum<BodyShape>(22);
        _isFlying = ReadAsBool(30);
        _mass = Mathf.Clamp(ReadAsNormalizedFloat(5) * 2.0f * _scale, MinimumMass, 10.0f);
        if (_isFlying)
        {
            _mass /= 3.0f;
        }
        _speed = ReadAsNormalizedFloat(16) / (_scale * _mass);

        _root = new GameObject();
        if (parent != null)
        {
            _root.transform.parent = parent.transform;
        }
        var body = RenderBody(_bodyShape);
        body.transform.parent = _root.transform;
        _root.transform.localScale = Vector3.one * _scale;
        var position = Random.insideUnitSphere * 50.0f;
        position.y =  Mathf.Clamp(position.y, 2.0f, 50.0f);
        _root.transform.position = position;
        _rigidBody = body.AddComponent<Rigidbody>();
        _rigidBody.mass = _mass;
        //_rigidBody.drag = ReadAsNormalizedFloat(6) * 1.5f + 1.5f;

        Renderer bodyRenderer = body.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Standard"));
        material.color = ReadAsRgbColor(5);
        material.SetFloat("_Metallic", ReadAsNormalizedFloat(7));
        material.SetFloat("_Glossiness", ReadAsNormalizedFloat(19));
        bodyRenderer.material = material;
    }

    public Vector3 Position
    {
        get
        {
            try
            {
                return _rigidBody.transform.position;
            }
            catch (MissingReferenceException e)
            {
                return Vector3.one;
            }
        }
    }

    public Vector3 Velocity
    {
        get
        {
            try
            {
                return _rigidBody.velocity;
            }
            catch (MissingReferenceException e)
            {
                return Vector3.zero;
            }
        }
    }

    public GameObject Root => _root;

    public void Move()
    {
        if (Random.value < 0.9)
        {
            return;
        }

        if (_lastMove == Vector3.zero)
        {
            _lastMove = Random.insideUnitSphere;
        }

        var current = new Vector3(
                          Random.Range(_lastMove.x - 8.0f, _lastMove.x + 8.0f),
                          Random.Range(_lastMove.y - 8.0f, _lastMove.y + 8.0f),
                          Random.Range(_lastMove.z - 8.0f, _lastMove.z + 8.0f));
        current = current.normalized;

        if (!_isFlying)
        {
            current.y = 0;
        }

        _rigidBody.AddForce(current * _speed);

        _lastMove = current;
    }

    public void SetVelocity(float velocity)
    {
        try
        {
            _rigidBody.velocity = _rigidBody.velocity.normalized * velocity;
        }
        catch (Exception e)
        {
        }
    }

    private GameObject RenderBody(BodyShape bodyShape)
    {
        switch (bodyShape)
        {
            case BodyShape.Capsule:
                return GameObject.CreatePrimitive(PrimitiveType.Capsule);
            case BodyShape.Cylinder:
                return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            case BodyShape.Cube:
                return GameObject.CreatePrimitive(PrimitiveType.Cube);
            case BodyShape.Sphere:
            default:
                return GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
    }

    private Color ReadAsRgbColor(int offset)
    {
        var red = ReadAsNormalizedFloat(offset) * 3.5f;
        offset += 8;
        var green = ReadAsNormalizedFloat(offset) * 3.5f;
        offset += 8;
        var blue = ReadAsNormalizedFloat(offset) * 3.5f;
        offset += 8;
        var alpha =  ReadAsNormalizedFloat(offset);
        return new Color(red, blue, green, alpha);
    }

    private bool ReadAsBool(int offset)
    {
        return ReadN(offset, 1) == "1";
    }

    private T ReadAsEnum<T>(int offset) where T : Enum
    {
        var values  = (T[])Enum.GetValues(typeof(T));
        var valueCount = values.Length;
        var requiredDigits = Mathf.CeilToInt(Mathf.Log(valueCount, 2));
        var source = ReadN(offset, requiredDigits);
        var index = Convert.ToInt32(source, 2);
        return values[index];
    }

    private float ReadAsNormalizedFloat(int offset)
    {
        var source = ReadN(offset, 32);
        var value = Convert.ToUInt64(source, 2);
        return value / (Mathf.Pow(2, 32) - 1);
    }

    private string ReadN(int start, int n)
    {
        if (start >= _sequence.Length)
        {
            start = _sequence.Length % start;
        }

        if (n == 0)
        {
            return string.Empty;
        }

        var endLength = Mathf.Min(n, _sequence.Length - start);
        var remainder = Mathf.Abs(n - endLength);
        var value = _sequence.Substring(start, endLength);
        value += ReadN(0, remainder);
        return value;
    }
}

public enum BodyShape
{
    Sphere,
    Capsule,
    Cylinder,
    Cube,
}

public class ProceduralCreature : MonoBehaviour
{
    public readonly List<Creature> Creatures = new List<Creature>();

    [SerializeField] private float MaximumVelocity = 15.0f;

    void Start()
    {
        for (var i = 0; i < 100; i++)
        {
            Creatures.Add(new Creature());
        }
    }

    void Update()
    {
        var delete = new Queue<Creature>();
        foreach (var creature in Creatures)
        {
            if (creature.Position.magnitude > 500.0f || creature.Position.y < 0)
            {
                delete.Enqueue(creature);
                continue;
            }
        }
        while (delete.Count > 0)
        {
            var deleted = delete.Dequeue();
            Creatures.Remove(deleted);
            Destroy(deleted.Root);
        }
    }

    private void FixedUpdate()
    {
        foreach (var creature in Creatures)
        {
            creature.Move();
            
            if (creature.Velocity.magnitude > MaximumVelocity)
            {
                creature.SetVelocity(MaximumVelocity);
            }
        }
    }
}
