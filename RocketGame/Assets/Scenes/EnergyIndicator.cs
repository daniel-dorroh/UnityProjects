using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyIndicator : MonoBehaviour
{
    public Color LightColor { get; set; }

    private Light _light;

    void Start()
    {
        _light = GetComponentInChildren<Light>();
    }

    void Update()
    {
        if (_light.color != LightColor)
        {
            _light.color = LightColor;
        }
    }
}
