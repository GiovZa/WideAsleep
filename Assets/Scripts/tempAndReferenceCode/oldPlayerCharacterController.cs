// using UnityEngine;

// [RequireComponent(typeof(CharacterController))]
// public class PlayerCharacterController : MonoBehaviour
// {
//     [Header("References")]
//     public Camera PlayerCamera;

//     [Header("Movement Settings")]
//     public float WalkSpeed = 6f;
//     public float SprintSpeed = 10f;
//     public float CrouchSpeed = 3f;
//     public float JumpForce = 7f;
//     public float Gravity = 9.81f;
//     public float MouseSensitivity = 2f;

//     [Header("Crouching Settings")]
//     public float StandHeight = 1.8f;
//     public float CrouchHeight = 1f;
//     public float CrouchTransitionSpeed = 10f;
    
//     private CharacterController _controller;
//     private Vector3 _velocity;
//     private float _cameraVerticalAngle = 0f;
//     private bool _isCrouching = false;
//     private float _targetHeight;

//     void Start()
//     {
//         _controller = GetComponent<CharacterController>();
//         _targetHeight = StandHeight;
//         Cursor.lockState = CursorLockMode.Locked;
//     }

//     void Update()
//     {
//         HandleMovement();
//         HandleRotation();
//         HandleCrouch();
//     }

//     void HandleMovement()
//     {
//         float moveX = Input.GetAxis("Horizontal");
//         float moveZ = Input.GetAxis("Vertical");

//         Vector3 move = transform.right * moveX + transform.forward * moveZ;
//         float speed = _isCrouching ? CrouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : WalkSpeed);

//         if (_controller.isGrounded)
//         {
//             _velocity.y = -2f;

//             if (Input.GetKeyDown(KeyCode.Space) && !_isCrouching)
//             {
//                 _velocity.y = JumpForce;
//             }
//         }

//         _controller.Move(move * speed * Time.deltaTime);
//         _velocity.y -= Gravity * Time.deltaTime;
//         _controller.Move(_velocity * Time.deltaTime);
//     }

//     void HandleRotation()
//     {
//         float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
//         float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

//         _cameraVerticalAngle -= mouseY;
//         _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);

//         PlayerCamera.transform.localRotation = Quaternion.Euler(_cameraVerticalAngle, 0f, 0f);
//         transform.Rotate(Vector3.up * mouseX);
//     }

//     void HandleCrouch()
//     {
//         if (Input.GetKeyDown(KeyCode.C))
//         {
//             if (!_isCrouching) 
//             {
//                 _targetHeight = CrouchHeight;
//                 _isCrouching = true;
//             }
//             else
//             {
//                 if (CanStandUp()) // Prevent standing if blocked
//                 {
//                     _targetHeight = StandHeight;
//                     _isCrouching = false;
//                 }
//             }
//         }

//         // Smooth height transition
//         _controller.height = Mathf.Lerp(_controller.height, _targetHeight, CrouchTransitionSpeed * Time.deltaTime);
//         _controller.center = new Vector3(0, _controller.height / 2, 0);
//         PlayerCamera.transform.localPosition = new Vector3(0, _controller.height * 0.9f, 0);
//     }

//     bool CanStandUp()
//     {
//         RaycastHit hit;
//         float checkHeight = StandHeight - CrouchHeight;
//         Vector3 origin = transform.position + Vector3.up * CrouchHeight;

//         return !Physics.Raycast(origin, Vector3.up, out hit, checkHeight);
//     }
// }