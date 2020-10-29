using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float turnSpeed = 0.5f;
    [SerializeField] private float nodSpeed = 0.5f;

    private Controls _controls;
    private float _verticalAngle;
    private float _horizontalAngle;
    private int _ignoreCount = 0;

    private const float MaximumVerticalAngle = 89.5f;

    void Awake()
    {
        _controls = new Controls();
    }

    void Start()
    {
        if (player == null)
        {
            throw new InvalidOperationException("player must be initialized");
        }
        Cursor.lockState = CursorLockMode.Locked;
        _verticalAngle = 0;
        _horizontalAngle = 0;
    }

    void Update()
    {
        var look = _controls.Player.Look.ReadValue<Vector2>();
        Look(look);
    }

    private void Look(Vector2 mouseDeltas)
    {
        if (_ignoreCount < 10)
        {
            _ignoreCount++;
            return;
        }

        if (mouseDeltas.sqrMagnitude < 0.01)
        {
            return;
        }

        var nod = nodSpeed * Time.deltaTime;
        var turn = turnSpeed * Time.deltaTime;

        _verticalAngle += mouseDeltas.y * nod;
        _verticalAngle = Mathf.Clamp(_verticalAngle, -MaximumVerticalAngle, MaximumVerticalAngle);

        _horizontalAngle = mouseDeltas.x * turn;

        transform.localRotation = Quaternion.Euler(_verticalAngle, 0.0f, 0.0f);
        player.Rotate(Vector3.up, _horizontalAngle);
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }
}
