using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers
{
    public class FirstPersonMouseLook : MonoBehaviour
    {
        [SerializeField] private bool isYInverted = true;
        [SerializeField] private bool isXInverted;
        [SerializeField] private float horizontalSensitivity = 20.0f;
        [SerializeField] private float verticalSensitivity = 20.0f;
        [Tooltip("Player object parent of the camera this component is attached to.")]
        [SerializeField] private Transform player;
        [Tooltip("The input actions asset where the input action " +
                 "mapping is configured. (e.g., MyProject.inputactions)")]
        [SerializeField] private InputActionAsset inputActions;
        [Tooltip("The name or ID of the mapped look action. " +
                 "This action is mapped to a value of type Vector2, " +
                 "and can be represented by a pointer delta, " +
                 "or a gamepad or joystick analog stick.")]
        [SerializeField] private string lookActionName = "Look";
        [Tooltip("To prevent the look camera from initializing at " +
                 "a large angle, which happens occasionally despite " +
                 "initialization at 0, the first n look reads are thrown out. " +
                 "This field controls how many initial reads to ignore.")]
        [SerializeField] private int ignoredInitialInputEventCount = 5;
        [Tooltip("Minimum input magnitude required to cause the controller to move the camera.")]
        [SerializeField] private float minimumInputMagnitude = 0.01f;

        [SerializeField] private bool isCursorLocked = true;

        private InputAction _look;
        private float _verticalAngle;
        private float _horizontalAngle;
        private int _ignoredDeltasCount;

        private const float VerticalClampAngle = 89.5f;

        private void Awake()
        {
            _look = inputActions.FindAction(lookActionName, true);
        }

        private void Start()
        {
            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void Update()
        {
            UpdateSettings();
            var deltas = _look.ReadValue<Vector2>();
            Look(deltas);
        }

        private void UpdateSettings()
        {
            if (isCursorLocked && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (!isCursorLocked && Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void Look(Vector2 deltas)
        {
            if (_ignoredDeltasCount++ < ignoredInitialInputEventCount)
            {
                return;
            }

            if (deltas.sqrMagnitude < minimumInputMagnitude)
            {
                return;
            }

            var vertical = verticalSensitivity * Time.deltaTime;
            var horizontal = horizontalSensitivity * Time.deltaTime;

            if (isXInverted)
            {
                horizontal = -horizontal;
            }

            if (isYInverted)
            {
                vertical = -vertical;
            }

            _verticalAngle += deltas.y * vertical;
            _verticalAngle = Mathf.Clamp(_verticalAngle, -VerticalClampAngle, VerticalClampAngle);

            _horizontalAngle = deltas.x * horizontal;

            transform.localRotation = Quaternion.Euler(_verticalAngle, 0.0f, 0.0f);
            player.Rotate(Vector3.up, _horizontalAngle);
        }
    }
}
