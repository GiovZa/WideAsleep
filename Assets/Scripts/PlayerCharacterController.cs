using System;
using UnityEngine;
using UnityEngine.Events;
using SoundSystem;

namespace playerChar
{
    [RequireComponent(typeof(CharacterController), typeof(AudioSource))]
    public class PlayerCharacterController : MonoBehaviour
    {
        public bool isGamePaused = false;

        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Tooltip("Audio source for footsteps, jump, etc...")]
        public AudioSource audioSource;

        [Header("General")] [Tooltip("Force applied downward when in the air")]
        public float GravityDownForce = 20f;

        [Tooltip("Physic layers checked to consider the player grounded")]
        public LayerMask GroundCheckLayers = -1;

        [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
        public float GroundCheckDistance = 0.05f;

        [Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float MaxSpeedOnGround = 10f;

        [Tooltip(
            "Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 15;

        [Tooltip("Max movement speed when crouching")] [Range(0, 1)]
        public float MaxSpeedCrouchedRatio = 0.5f;

        [Tooltip("Max movement speed when not grounded")]
        public float MaxSpeedInAir = 10f;

        [Tooltip("Acceleration speed when in the air")]
        public float AccelerationSpeedInAir = 25f;

        [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
        public float SprintSpeedModifier = 2f;

        [Tooltip("Height at which the player dies instantly when falling off the map")]
        public float KillHeight = -50f;

        [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 2f;

        [Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
        public float AimingRotationMultiplier = 0.4f;

        [Tooltip("Inverts Mouse Y controls")]
        public bool isInverted = false;
        // public float MouseSensitivity = 0.01f;

        [Header("Jump")] [Tooltip("Force applied upward when jumping")]
        public float JumpForce = 9f;

        [Header("Stance")] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
        public float CameraHeightRatio = 0.9f;

        [Tooltip("Height of character when standing")]
        public float CapsuleHeightStanding = 1.8f;

        [Tooltip("Height of character when crouching")]
        public float CapsuleHeightCrouching = 0.9f;

        [Tooltip("Speed of crouching transitions")]
        public float CrouchingSharpness = 10f;

        [Header("Audio")] [Tooltip("Amount of footstep sounds played when moving one meter")]
        public float FootstepSfxFrequency = 1f;

        [Tooltip("Amount of footstep sounds played when moving one meter while sprinting")]
        public float FootstepSfxFrequencyWhileSprinting = 1f;

        [Tooltip("Sound played for footsteps")]
        public AudioClip FootstepSfx;

        public float noiseMeter = 10f;

        [Tooltip("Sound played when jumping")] public AudioClip JumpSfx;
        [Tooltip("Sound played when landing")] public AudioClip LandSfx;

        [Header("Headbob")]
        [Tooltip("How tall the bounce of head bobs are when walking")] public float HeadbobAmountWalk = 0.08f;
        [Tooltip("How tall the bounce of head bobs are when sprinting")] public float HeadbobAmountSprint = 0.05f;
        [Tooltip("How fast the bounce loops")] public float HeadbobSpeed = 1f;

        public UnityAction<bool> OnStanceChanged;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public bool HasJumpedThisFrame { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsCrouching { get; private set; }

        private bool CanMove = true;

        CharacterController m_Controller;
        Vector3 m_GroundNormal;
        Vector3 m_CharacterVelocity;
        Vector3 m_LatestImpactSpeed;
        float m_LastTimeJumped = 0f;
        float m_CameraVerticalAngle = 0f;
        float m_FootstepDistanceCounter;
        float m_TargetCharacterHeight;

        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        void Start()
        {
            // fetch components on the same gameObject
            m_Controller = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();

            m_Controller.enableOverlapRecovery = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // force the crouch state to false when starting
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);
        }

        void Update()
        {
            // check for Y kill
            if (!IsDead && transform.position.y < KillHeight)
            {
                // m_Health.Kill();
                Debug.LogWarning("Death by Kill Height Not Implemented, resetting Player position");
                transform.transform.position = new Vector3(10,0,-10);
            }

            if (!CanMove)
            {
                return;
            }

            HasJumpedThisFrame = false;

            bool wasGrounded = IsGrounded;
            GroundCheck();

            // landing
            if (IsGrounded && !wasGrounded)
            {
                audioSource.PlayOneShot(LandSfx);
                SoundEvents.EmitSound(transform.position, 0.8f * noiseMeter);
            }

            // crouching
            if (Input.GetButtonDown("Crouch"))
            {
                SetCrouchingState(!IsCrouching, false);
            }

            UpdateCharacterHeight(false);

            if (isGamePaused == false)
                HandleCharacterMovement();
        }

        void OnDie()
        {
            IsDead = true;
            Debug.LogWarning("Ya Died!");

            // EventManager.Broadcast(Events.PlayerDeathEvent);
        }

        void HandleHeadbob(float bobAmount, float bobSpeed)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            Vector3 targetPos = new Vector3(0f, (m_Controller.height * CameraHeightRatio) + bob, 0f);
            PlayerCamera.transform.localPosition = Vector3.Lerp(
                PlayerCamera.transform.localPosition,
                targetPos,
                Time.deltaTime * 10f  // <-- smoothing speed, tweak as needed
            );
        }

        void GroundCheck()
        {
            // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
            float chosenGroundCheckDistance =
                IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

            // reset values before the ground check
            IsGrounded = false;
            m_GroundNormal = Vector3.up;

            // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
            if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
            {
                // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
                    m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    // storing the upward direction for the surface found
                    m_GroundNormal = hit.normal;

                    // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                    // and if the slope angle is lower than the character controller's limit
                    if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(m_GroundNormal))
                    {
                        IsGrounded = true;

                        // handle snapping to the ground
                        if (hit.distance > m_Controller.skinWidth)
                        {
                            m_Controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        void HandleCharacterMovement()
        {
            // horizontal character rotation
            {
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(
                    new Vector3(0f, Input.GetAxis("Mouse X") * RotationSpeed, 0f), Space.Self);
            }

            // vertical camera rotation
            {
                // add vertical inputs to the camera's vertical angle
                m_CameraVerticalAngle += Input.GetAxis("Mouse Y") * RotationSpeed * (isInverted ? 1f : -1f);

                // limit the camera's vertical angle to min/max
                m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            }

            // character movement handling
            bool isSprinting = Input.GetButton("Sprint");
            {
                if (isSprinting)
                {
                    isSprinting = SetCrouchingState(false, false);
                }

                float speedModifier = isSprinting ? SprintSpeedModifier : 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

                // Constrain move input to max magnitude of 1 (prevents diagonal speed boost)
                moveInput = Vector3.ClampMagnitude(moveInput, 1);
                Vector3 worldspaceMoveInput = transform.TransformVector(moveInput);

                // handle grounded movement
                if (IsGrounded)
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetVelocity = worldspaceMoveInput * MaxSpeedOnGround * speedModifier;
                    // reduce speed if crouching by crouch speed ratio
                    if (IsCrouching)
                        targetVelocity *= MaxSpeedCrouchedRatio;
                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) *
                                     targetVelocity.magnitude;

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                        MovementSharpnessOnGround * Time.deltaTime);

                    // Debug.Log($"IsGrounded Block: {CharacterVelocity}");

                    // jumping
                    if (IsGrounded && Input.GetButtonDown("Jump"))
                    {
                        // force the crouch state to false
                        if (SetCrouchingState(false, false))
                        {
                            // start by canceling out the vertical component of our velocity
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                            // then, add the jumpSpeed value upwards
                            CharacterVelocity += Vector3.up * JumpForce;

                            // play sound
                            audioSource.PlayOneShot(JumpSfx);
                            SoundEvents.EmitSound(transform.position, 1.0f * noiseMeter);

                            // remember last time we jumped because we need to prevent snapping to ground for a short time
                            m_LastTimeJumped = Time.time;
                            HasJumpedThisFrame = true;

                            // Force grounding to false
                            IsGrounded = false;
                            m_GroundNormal = Vector3.up;
                            // Debug.Log($"Jump Frame Block: {CharacterVelocity}");
                        }
                    }

                    // Footsteps and headbob only when grounded and moving
                    if (IsGrounded && CharacterVelocity.magnitude > 0.1f)
                    {
                        float stepFreq = isSprinting ? FootstepSfxFrequencyWhileSprinting :
                                        (IsCrouching ? FootstepSfxFrequency * 1.5f : FootstepSfxFrequency);
                        m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;

                        if (m_FootstepDistanceCounter >= 1f / stepFreq)
                        {
                            m_FootstepDistanceCounter = 0f;
                            
                            if (FootstepSfx != null)
                            {
                                audioSource.pitch = isSprinting ? 1.2f : 1f;

                                if (IsCrouching)
                                    audioSource.volume = 0.2f;
                                else
                                    audioSource.volume = isSprinting ? 1f : 0.5f;

                                audioSource.volume *= 0.1f;
                                audioSource.PlayOneShot(FootstepSfx);
                                SoundEvents.EmitSound(transform.position, audioSource.volume * noiseMeter);
                            }

                            // HandleHeadbob(isSprinting ? HeadbobAmountWalk : HeadbobAmountSprint, HeadbobSpeed);
                        }
                        
                        // Headbob every frame while moving
                        float bobHeight = isSprinting ? HeadbobAmountSprint :
                                        (IsCrouching ? HeadbobAmountWalk * 0.25f : HeadbobAmountWalk);
                        float bobSpeed = isSprinting ? HeadbobSpeed * 1.5f :
                                        (IsCrouching ? HeadbobSpeed * 0.5f : HeadbobSpeed);
                        HandleHeadbob(bobHeight, bobSpeed);

                    }
                    else
                    {
                        m_FootstepDistanceCounter = 0f; // reset when not moving or in air
                    }

                    // keep track of distance traveled for footsteps sound
                    m_FootstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
                }
                // handle air movement
                else
                {
                    // add air acceleration
                    CharacterVelocity += worldspaceMoveInput * AccelerationSpeedInAir * Time.deltaTime;

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity = CharacterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxSpeedInAir * speedModifier);
                    CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    // apply the gravity to the velocity
                    CharacterVelocity += Vector3.down * GravityDownForce * Time.deltaTime;
                    // Debug.Log($"Air Movement Block: {CharacterVelocity}");
                }
                // Debug.Log($"World Space Move Input: {worldspaceMoveInput}");
            }

            // apply the final calculated velocity value as a character movement
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);
            m_Controller.Move(CharacterVelocity * Time.deltaTime);

            // detect obstructions to adjust velocity accordingly
            m_LatestImpactSpeed = Vector3.zero;
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
                CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore))
            {
                // We remember the last impact speed because the fall damage logic might need it
                m_LatestImpactSpeed = CharacterVelocity;

                CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
            }

            // Debug.Log($"Character Velocity: {CharacterVelocity}");
        }

        // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * m_Controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - m_Controller.radius));
        }

        // Gets a reoriented direction that is tangent to a given slope
        public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        void UpdateCharacterHeight(bool force)
        {
            // Update height instantly
            if (force)
            {
                m_Controller.height = m_TargetCharacterHeight;
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
            }
            // Update smooth height
            else if (m_Controller.height != m_TargetCharacterHeight)
            {
                // resize the capsule and adjust camera position
                m_Controller.height = Mathf.Lerp(m_Controller.height, m_TargetCharacterHeight,
                    CrouchingSharpness * Time.deltaTime);
                m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
                PlayerCamera.transform.localPosition = Vector3.Lerp(PlayerCamera.transform.localPosition,
                    Vector3.up * m_TargetCharacterHeight * CameraHeightRatio, CrouchingSharpness * Time.deltaTime);
            }
        }

        // returns false if there was an obstruction
        bool SetCrouchingState(bool crouched, bool ignoreObstructions)
        {
            // set appropriate heights
            if (crouched)
            {
                // Debug.Log("Crouched");
                m_TargetCharacterHeight = CapsuleHeightCrouching;
            }
            else
            {
                // Detect obstructions
                if (!ignoreObstructions)
                {
                    // Debug.Log("Colliders");
                    Collider[] standingOverlaps = Physics.OverlapCapsule(
                        GetCapsuleBottomHemisphere(),
                        GetCapsuleTopHemisphere(CapsuleHeightStanding),
                        m_Controller.radius,
                        -1,
                        QueryTriggerInteraction.Ignore);

                    // Debug.Log($"Standing Overlaps Count: {standingOverlaps.Length}");
                    // Debug.Log($"GetCapsuleBottomHemisphere: {GetCapsuleBottomHemisphere()}");
                    // Debug.Log($"GetCapsuleTopHemisphere(CapsuleHeightStanding): {GetCapsuleTopHemisphere(CapsuleHeightStanding)}");
                    // Debug.Log($"m_Controller.radius: {m_Controller.radius}");
                    foreach (Collider c in standingOverlaps)
                    {
                        // Debug.Log($"Collider detected: {c.name} (Layer: {LayerMask.LayerToName(c.gameObject.layer)})");
                        if (c != m_Controller)
                        {
                            // Debug.Log($"Valid obstruction found! Collider: {c.name}");
                            return false;
                        }
                    }
                }
                // Debug.Log("✅ No obstructions detected, can stand up!");
                
                m_TargetCharacterHeight = CapsuleHeightStanding;
                // Debug.Log($"TargetCharacterHeight: {m_TargetCharacterHeight}");
                // Debug.Log($"CapsuleHeightStanding: {CapsuleHeightStanding}");
            }

            if (OnStanceChanged != null)
            {
                // Debug.Log("OnStanceChanged");
                OnStanceChanged.Invoke(crouched);
            }

            IsCrouching = crouched;
            return true;
        }
    
        // Movement toggle functions when interacting with objects with a separate UI (Piano)
        public void DisableMovement()
        {
            CanMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }

        public void EnableMovement()
        {
            CanMove = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}