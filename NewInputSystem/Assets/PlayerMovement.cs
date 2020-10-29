using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float groundSpeed = 15.0f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private Camera eyes;

    private Controls _controls;
    private Vector3 _movementDirection = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private bool _isGrounded = false;

    private const float MinimumDownwardVelocity = 0.0f;

    void Awake()
    {
        _controls = new Controls();
        _controls.Player.Move.performed += OnMovePerformed;
        _controls.Player.Move.canceled += OnMoveCanceled;
        _controls.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext obj)
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(-2.0f * jumpHeight * Physics.gravity.y);
        }
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < MinimumDownwardVelocity)
        {
            _velocity.y = MinimumDownwardVelocity;
        }

        var horizontalVelocity =
            groundSpeed * Vector3.ProjectOnPlane(
                    eyes.transform.TransformVector(_movementDirection),
                    Vector3.up)
            .normalized;

        controller.Move((_velocity + horizontalVelocity) * Time.deltaTime);
    }

    void FixedUpdate()
    {
        _velocity += Physics.gravity * Time.fixedDeltaTime;
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        _movementDirection.x = 0.0f;
        _movementDirection.z = 0.0f;
    }

    private void OnMovePerformed(InputAction.CallbackContext obj)
    {
        var input = obj.ReadValue<Vector2>();
        _movementDirection.x = input.x;
        _movementDirection.z = input.y;
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
