using System;
using System.Collections;
using System.Collections.Generic;
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

    public Creature()
    {
        _seed = (uint)Mathf.Floor(Random.value * uint.MaxValue);
        _sequence = Convert.ToString(_seed, 2);
        _scale = ReadAsNormalizedFloat(13);
        _bodyShape = ReadAsEnum<BodyShape>(22);
        _isFlying = ReadAsBool(30);

        var root = new GameObject();
        var body = RenderBody(_bodyShape);
        body.transform.parent = root.transform;
        root.transform.localScale = Vector3.one * _scale;
        var position = Random.insideUnitSphere * 50.0f;
        position.y =  Mathf.Clamp(position.y, 2.0f, 50.0f);
        root.transform.position = position;
        if (!_isFlying)
        {
            body.AddComponent<Rigidbody>();
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
    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < 100; i++)
        {
            new Creature();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
