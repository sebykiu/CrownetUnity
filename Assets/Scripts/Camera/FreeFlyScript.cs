//===========================================================================//
//                       FreeFlyCamera (Version 1.2)                         //
//                        (c) 2019 Sergey Stafeyev                           //
//                        (c) 2023 Sebastian Kiunke (performance updates)    //
//===========================================================================//

using UnityEngine;
using UnityEngine.Serialization;

namespace Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class FreeFlyScript : MonoBehaviour
    {
        #region UI

        [FormerlySerializedAs("_active")] [Space] [SerializeField] [Tooltip("The script is currently active")]
        private bool active = true;

        [FormerlySerializedAs("_enableRotation")] [Space] [SerializeField] [Tooltip("Camera rotation by mouse movement is active")]
        private bool enableRotation = true;

        [FormerlySerializedAs("_mouseSense")] [SerializeField] [Tooltip("Sensitivity of mouse rotation")]
        private float mouseSense = 1.8f;

        [FormerlySerializedAs("_enableTranslation")] [Space] [SerializeField] [Tooltip("Camera zooming in/out by 'Mouse Scroll Wheel' is active")]
        private bool enableTranslation = true;

        [FormerlySerializedAs("_translationSpeed")] [SerializeField] [Tooltip("Velocity of camera zooming in/out")]
        private float translationSpeed = 55f;

        [FormerlySerializedAs("_movementSpeed")] [SerializeField] [Tooltip("Camera movement speed")]
        private float movementSpeed = 10f;

        [FormerlySerializedAs("_boostedSpeed")] [SerializeField] [Tooltip("Speed of the quick camera movement when holding the 'Left Shift' key")]
        private float boostedSpeed = 50f;

        [FormerlySerializedAs("_boostSpeed")] [SerializeField] [Tooltip("Boost speed")]
        private KeyCode boostSpeed = KeyCode.LeftShift;

        [FormerlySerializedAs("_enableSpeedAcceleration")] [Space] [SerializeField] [Tooltip("Acceleration at camera movement is active")]
        private bool enableSpeedAcceleration = true;

        [FormerlySerializedAs("_speedAccelerationFactor")] [SerializeField] [Tooltip("Rate which is applied during camera movement")]
        private float speedAccelerationFactor = 1.5f;

        [FormerlySerializedAs("_initPositonButton")]
        [Space]
        [SerializeField]
        [Tooltip("This keypress will move the camera to initialization position")]
        private KeyCode initPositonButton = KeyCode.R;

        #endregion UI

        private CursorLockMode _wantedMode;

        private Transform _cameraTransform;

        private float _currentIncrease = 1;
        private float _currentIncreaseMem;

        private Vector3 _initPosition;
        private Vector3 _initRotation;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (boostedSpeed < movementSpeed)
                boostedSpeed = movementSpeed;
        }
#endif


        private void Start()
        {
            var trans = transform;
            _initPosition = trans.position;
            _initRotation = trans.eulerAngles;
        }

        private void OnEnable()
        {
            if (active)
                _wantedMode = CursorLockMode.Locked;
        }

        // Apply requested cursor state
        private void SetCursorState()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = _wantedMode = CursorLockMode.None;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _wantedMode = CursorLockMode.Locked;
            }

            // Apply cursor state
            Cursor.lockState = _wantedMode;
            // Hide cursor when locking
            Cursor.visible = (CursorLockMode.Locked != _wantedMode);
        }

        private void CalculateCurrentIncrease(bool moving)
        {
            _currentIncrease = Time.deltaTime;

            if (!enableSpeedAcceleration || enableSpeedAcceleration && !moving)
            {
                _currentIncreaseMem = 0;
                return;
            }

            _currentIncreaseMem += Time.deltaTime * (speedAccelerationFactor - 1);
            _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
        }

        private void Update()
        {
            if (!active || Cursor.visible) return;

            SetCursorState();
            HandleTranslation();
            HandleMovement();
            HandleRotation();
            ReturnToInitPosition();
        }

        private void HandleTranslation()
        {
            if (enableTranslation)
            {
                _cameraTransform.Translate(Vector3.forward * (Input.mouseScrollDelta.y * Time.deltaTime * translationSpeed));
            }
        }

        private void HandleMovement()
        {
            Vector3 deltaPosition = Vector3.zero;
            float currentSpeed = movementSpeed;

            if (Input.GetKey(boostSpeed)) currentSpeed = boostedSpeed;

            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            deltaPosition += _cameraTransform.forward * verticalInput;
            deltaPosition += _cameraTransform.right * horizontalInput;

            // Add other movement controls here...

            CalculateCurrentIncrease(deltaPosition != Vector3.zero);

            _cameraTransform.position += deltaPosition * (currentSpeed * _currentIncrease);
        }

        private void HandleRotation()
        {
            if (enableRotation)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSense;
                float mouseY = -Input.GetAxis("Mouse Y") * mouseSense; // Inverted for Pitch

                // Apply rotation
                _cameraTransform.Rotate(Vector3.up, mouseX, Space.World);
                _cameraTransform.Rotate(Vector3.right, mouseY, Space.Self);
            }
        }

        private void ReturnToInitPosition()
        {
            if (Input.GetKeyDown(initPositonButton))
            {
                _cameraTransform.position = _initPosition;
                _cameraTransform.eulerAngles = _initRotation;
            }
        }
    }
}