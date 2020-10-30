using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
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

    private const float MinimumMass = 0.1f;

    public Creature()
    {
        _seed = (uint)Mathf.Floor(Random.value * uint.MaxValue);
        _sequence = Convert.ToString(_seed, 2);
        _scale = ReadAsNormalizedFloat(13);
        _bodyShape = ReadAsEnum<BodyShape>(22);
        _isFlying = ReadAsBool(30);
        _mass = ReadAsNormalizedFloat(5) * 2.0f * _scale + MinimumMass;
        if (_isFlying)
        {
            _mass /= 3.0f;
        }
        _speed = ReadAsNormalizedFloat(16) / (_scale * _mass);

        _root = new GameObject();
        var body = RenderBody(_bodyShape);
        body.transform.parent = _root.transform;
        _root.transform.localScale = Vector3.one * _scale;
        var position = Random.insideUnitSphere * 50.0f;
        position.y =  Mathf.Clamp(position.y, 2.0f, 50.0f);
        _root.transform.position = position;
        _rigidBody = body.AddComponent<Rigidbody>();
        _rigidBody.mass = _mass;

        Renderer bodyRenderer = body.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Standard"));
        material.color = ReadAsRgbColor(5);
        material.SetFloat("_Metallic", ReadAsNormalizedFloat(7));
        material.SetFloat("_Glossiness", ReadAsNormalizedFloat(19));
        bodyRenderer.material = material;
    }

    public Vector3 Position => _rigidBody.transform.position;

    public GameObject Root => _root;

    public void Move()
    {
        if (Random.value < 0.6)
        {
            return;
        }

        if (_lastMove == Vector3.zero)
        {
            _lastMove = Random.insideUnitSphere;
        }

        var current = _lastMove + Vector3.one * Random.value;
        current = current.normalized;

        if (!_isFlying)
        {
            current.y = 0;
        }

        _rigidBody.AddForce(current * _speed);

        _lastMove = current;
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
    private readonly List<Creature> _creatures = new List<Creature>();
    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < 100; i++)
        {
            _creatures.Add(new Creature());
        }
    }

    // Update is called once per frame
    void Update()
    {
        var delete = new Queue<Creature>();
        foreach (var creature in _creatures)
        {
            if (creature.Position.magnitude > 500.0f)
            {
                delete.Enqueue(creature);
                continue;
            }
            creature.Move();
        }
        while (delete.Count > 0)
        {
            var deleted = delete.Dequeue();
            _creatures.Remove(deleted);
            Destroy(deleted.Root);
        }
    }
}
