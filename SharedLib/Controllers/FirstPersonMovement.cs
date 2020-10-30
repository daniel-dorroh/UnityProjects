using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Controllers
{
    public class FirstPersonMovement : MonoBehaviour
    {
        [Tooltip("The input actions asset where the input action " +
                 "mapping is configured. (e.g., MyProject.inputactions)")]
        [SerializeField] private InputActionAsset inputActions;
        [Tooltip("The name or ID of the mapped move action. " +
                 "This action is mapped to a value of type Vector2, " +
                 "and can be represented by a WASD mapping, " +
                 "a gamepad or joystick analog stick, etc.")]
        [SerializeField] private string moveActionName = "Move";
        [Tooltip("The name or ID of the mapped jump action. " +
                 "This action is mapped to a value of type button.")]
        [SerializeField] private string jumpActionName = "Jump";
        [Tooltip("The character controller attached to the player object")]
        [SerializeField] private CharacterController characterController;
        [Tooltip("The camera through which the player sees.")]
        [SerializeField] private Camera characterCamera;
        [Tooltip("A game object at the bottom of the player that this controller uses to determine player grounding.")]
        [SerializeField] private Transform groundCheck;
        [Tooltip("The layer containing objects the player object can be considered grounded to. (e.g., the layer containing the terrain collider)")]
        [SerializeField] private LayerMask groundMask;
        [Tooltip("The radius from the center of the groundCheck object this controller uses to check for grounding. This value should be greater than or equal to the characterController skin width.")]
        [SerializeField] private float groundDistance = 0.1f;
        [SerializeField] private float groundSpeed = 15.0f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravitationalAcceleration = Physics.gravity.y;
        [Tooltip("Minimum input magnitude required to cause the controller to move the camera.")]
        [SerializeField] private float minimumInputMagnitude = 0.01f;

        private InputAction _move;
        private InputAction _jump;
        private Vector3 _velocity;
        private bool _isGrounded;

        private const float MinimumDownwardVelocity = 0;

        private void Awake()
        {
            _move = inputActions.FindAction(moveActionName, true);
            _jump = inputActions.FindAction(jumpActionName, false);
            if (_jump != null)
            {
                _jump.performed += OnJump;
            }
            else
            {
                Debug.LogWarning("No Jump action configured");
            }
        }

        private void Start()
        {
            if (groundDistance < characterController.skinWidth)
            {
                groundDistance = characterController.skinWidth;
                Debug.LogWarning("groundDistance coerced to character controller skin width value");
            }
        }

        private void Update()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (_isGrounded && _velocity.y < MinimumDownwardVelocity)
            {
                _velocity.y = MinimumDownwardVelocity;
            }

            var direction = _move.ReadValue<Vector2>();
            Move(direction);
        }

        private void Move(Vector2 direction)
        {
            var displacement = 0f;

            if (direction.SqrMagnitude() >= minimumInputMagnitude)
            {
                displacement = groundSpeed * Time.deltaTime;
            }

            var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
            characterController.Move(move * displacement + _velocity * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _velocity += Physics.gravity * Time.fixedDeltaTime;
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            if (_isGrounded)
            {
                _velocity.y = Mathf.Sqrt(-2.0f * jumpHeight * gravitationalAcceleration);
            }
        }

        void OnEnable()
        {
            _move.Enable();
            if (_jump != null)
            {
                _jump.Enable();
            }
        }

        void OnDisable()
        {
            _move.Disable();
            if (_jump != null)
            {
                _jump.Disable();
            }
        }
    }
}
