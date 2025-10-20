using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using SoundSystem;
using UnityEngine.InputSystem;

namespace playerChar
{
    [RequireComponent(typeof(CharacterController), typeof(AudioSource))]
    public class PlayerCharacterController : MonoBehaviour
    {
        // public bool isGamePaused = false; // This is now handled by the GameStateManager

        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Tooltip("Audio source for footsteps, jump, etc...")]
        public AudioSource audioSource;
        
        [Tooltip("Animator for the player")]
        public Animator animator;

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

        [Header("Stamina")]
        [Tooltip("Maximum stamina for sprinting.")]
        public float MaxStamina = 100f;

        [Tooltip("The rate at which stamina drains while sprinting.")]
        public float StaminaDrainRate = 20f;

        [Tooltip("The rate at which stamina regenerates.")]
        public float StaminaRegenerationRate = 15f;
        
        [Tooltip("The delay in seconds before stamina begins to regenerate after sprinting.")]
        public float StaminaRegenerationDelay = 2f;

        [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 2f;

        [Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
        public float AimingRotationMultiplier = 0.4f;

        [Tooltip("Sensitivity for mouse input.")]
        public float MouseSensitivity = 1.0f;

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

        [Tooltip("Sounds played for footsteps")]
        public AudioClip[] FootstepSfx;

        [Tooltip("Random volume variation for footsteps")]
        public Vector2 FootstepVolumeRange = new Vector2(0.8f, 1.0f);

        [Tooltip("Random pitch variation for footsteps")]
        public Vector2 FootstepPitchRange = new Vector2(0.95f, 1.05f);

        public float noiseMeter = 10f;

        [Tooltip("Sounds played when jumping")] public AudioClip[] JumpSfx;
        [Tooltip("Sound played when landing")] public AudioClip[] LandSfx;

        [Header("Noisy Surfaces")]
        [Tooltip("Cooldown in seconds between playing noisy surface sounds.")]
        public float NoisySurfaceSfxCooldown = 0.5f;

        [Header("Headbob")]
        [Tooltip("How tall the bounce of head bobs are when walking")] public float HeadbobAmountWalk = 0.08f;
        [Tooltip("How tall the bounce of head bobs are when sprinting")] public float HeadbobAmountSprint = 0.05f;
        [Tooltip("How fast the bounce loops")] public float HeadbobSpeed = 1f;

        public UnityAction<bool> OnStanceChanged;

        public Vector3 CharacterVelocity { get; set; }
        public bool IsGrounded { get; private set; }
        public bool HasJumpedThisFrame { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsHiding { get; private set; }
        public float CurrentStamina { get; private set; }
        private bool CanMove = true;
        private bool isStunned = false;
        public Transform lookTarget;
        public Vector3 lookTargetOffset;

        CharacterController m_Controller;
        Vector3 m_GroundNormal;
        Vector3 m_CharacterVelocity;
        Vector3 m_LatestImpactSpeed;
        float m_LastTimeJumped = 0f;
        float m_LastTimeNoisySurfaceSfxPlayed = -1f;
        float m_CameraVerticalAngle = 0f;
        float m_FootstepDistanceCounter;
        float m_TargetCharacterHeight;
        float m_StaminaRegenerationTimer;

        private CustomInput m_Input;

        const float k_JumpGroundingPreventionTime = 0.2f;
        const float k_GroundCheckDistanceInAir = 0.07f;

        private void Awake()
        {
            m_Input = new CustomInput();
        }

        private void OnEnable()
        {
            m_Input.Player.Enable();
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            }
            // Subscribe to the sensitivity change event
            OptionsMenuController.OnSensitivityChanged += UpdateMouseSensitivity;
        }

        private void OnDisable()
        {
            m_Input.Player.Disable();
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
            // Unsubscribe to prevent memory leaks
            OptionsMenuController.OnSensitivityChanged -= UpdateMouseSensitivity;
        }

        public void Initialize()
        {
            // fetch components on the same gameObject
            m_Controller = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();

            if (GameStateManager.Instance == null)
            {
                Debug.LogError("GameStateManager instance not found.");
            }

            m_Controller.enableOverlapRecovery = true;

            // force the crouch state to false when starting
            SetCrouchingState(false, true);
            UpdateCharacterHeight(true);

            // Load saved sensitivity on initialize
            MouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");

            CurrentStamina = MaxStamina;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            // check for Y kill
            if (!Player.Instance.IsDead && transform.position.y < KillHeight)
            {
                Player.Instance.Die();
            }

            if (lookTarget != null)
            {
                HandleLookAtTarget();
            }

            HandleRotation();

            if (!CanMove)
            {
                return;
            }

            if (IsHiding)
            {
                RegenerateStamina();
                return;
            }

            HasJumpedThisFrame = false;

            bool wasGrounded = IsGrounded;
            GroundCheck();

            // landing
            if (IsGrounded && !wasGrounded)
            {
                if (LandSfx != null)
                {
                    AudioManager.Instance.PlayRandomSoundForPlayerOnly(LandSfx, transform.position, 1f, true, AudioManager.Instance.SFXMixerGroup);
                }
                SoundEvents.EmitSound(transform.position, 0.8f * noiseMeter);
            }

            // crouching
            if (m_Input.Player.Crouch.triggered)
            {
                SetCrouchingState(!IsCrouching, false);
            }

            UpdateCharacterHeight(false);

            HandleMovement();

            UpdateAnimator();
        }

        private void HandleGameStateChanged(GameState newState)
        {
            CanMove = newState == GameState.Gameplay;

            if (newState == GameState.InteractingWithUI || newState == GameState.Paused || newState == GameState.MainMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void HandleRotation()
        {
            if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
            {
                if (lookTarget != null) return;

                Vector2 look = m_Input.Player.Look.ReadValue<Vector2>();
                // horizontal character rotation
                // rotate the transform with the input speed around its local Y axis
                transform.Rotate(
                    new Vector3(0f, look.x * RotationSpeed * MouseSensitivity * Time.deltaTime, 0f), Space.Self);
            
                // vertical camera rotation
                // add vertical inputs to the camera's vertical angle
                m_CameraVerticalAngle += look.y * RotationSpeed * MouseSensitivity * Time.deltaTime * (isInverted ? 1f : -1f);

                // limit the camera's vertical angle to min/max
                m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

                // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
                PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            }
        }

        void HandleLookAtTarget()
        {
            Vector3 targetPosition = lookTarget.position + lookTargetOffset;
            Vector3 directionToTarget = (targetPosition - PlayerCamera.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            PlayerCamera.transform.rotation = Quaternion.Slerp(PlayerCamera.transform.rotation, targetRotation, Time.deltaTime * 2.0f);

            // Also rotate the player body
            Vector3 flatDirection = directionToTarget;
            flatDirection.y = 0;
            Quaternion bodyRotation = Quaternion.LookRotation(flatDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, bodyRotation, Time.deltaTime * 2.0f);
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

        void HandleMovement()
        {
            Vector2 move = m_Input.Player.Move.ReadValue<Vector2>();
            Vector3 moveInput = new Vector3(move.x, 0f, move.y);

            // --- Stamina Logic ---
            bool wantsToSprint = m_Input.Player.Sprint.IsPressed();
            bool isSprinting = wantsToSprint && CurrentStamina > 0f && IsGrounded && !IsCrouching;
            
            // If the player wants to sprint, but is crouching, try to stand up
            if (wantsToSprint && IsCrouching)
            {
                if (SetCrouchingState(false, false))
                {
                    isSprinting = CurrentStamina > 0f && IsGrounded;
                }
            }

            // Drain stamina if sprinting and moving
            if (isSprinting && moveInput.magnitude > 0.1f)
            {
                CurrentStamina -= StaminaDrainRate * Time.deltaTime;
                CurrentStamina = Mathf.Max(CurrentStamina, 0f);
                m_StaminaRegenerationTimer = StaminaRegenerationDelay;
            }
            else
            {
                RegenerateStamina();
            }
            // --- End Stamina Logic ---

            // character movement handling
            {
                if (isSprinting)
                {
                    // This check is now implicitly handled by the stamina logic, 
                    // but we keep it to ensure crouch state is correctly managed if stamina runs out mid-sprint.
                    isSprinting = SetCrouchingState(false, false);
                }

                float speedModifier = isSprinting ? SprintSpeedModifier : 1f;

                // converts move input to a worldspace vector based on our character's transform orientation
                Vector3 worldspaceMoveInput = transform.TransformVector(Vector3.ClampMagnitude(moveInput, 1));

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
                    if (IsGrounded && m_Input.Player.Jump.triggered)
                    {
                        // force the crouch state to false
                        if (SetCrouchingState(false, false))
                        {
                            // start by canceling out the vertical component of our velocity
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

                            // then, add the jumpSpeed value upwards
                            CharacterVelocity += Vector3.up * JumpForce;

                            // play sound
                            if (JumpSfx != null && JumpSfx.Length > 0)
                            {
                                // int index = UnityEngine.Random.Range(0, JumpSfx.Length);
                                // audioSource.PlayOneShot(JumpSfx[index]);
                                AudioManager.Instance.PlayRandomSoundForPlayerOnly(JumpSfx, transform.position, 0.5f, true, AudioManager.Instance.SFXMixerGroup);
                                SoundEvents.EmitSound(transform.position, 1.0f * noiseMeter);
                            }

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
                            
                            if (FootstepSfx != null && FootstepSfx.Length > 0)
                            {
                                int index = UnityEngine.Random.Range(0, FootstepSfx.Length);
                                AudioClip clip = FootstepSfx[index];

                                float basePitch = isSprinting ? 1.2f : 1f;
                                audioSource.pitch = basePitch * UnityEngine.Random.Range(FootstepPitchRange.x, FootstepPitchRange.y);

                                float baseVolume = IsCrouching ? 0.2f : (isSprinting ? 1f : 0.5f);
                                audioSource.volume = baseVolume * UnityEngine.Random.Range(FootstepVolumeRange.x, FootstepVolumeRange.y);
                                
                                //audioSource.volume *= 0.1f;

                                audioSource.PlayOneShot(clip);
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

        private void RegenerateStamina()
        {
            if (m_StaminaRegenerationTimer > 0)
            {
                m_StaminaRegenerationTimer -= Time.deltaTime;
            }
            else if (CurrentStamina < MaxStamina)
            {
                CurrentStamina += StaminaRegenerationRate * Time.deltaTime;
                CurrentStamina = Mathf.Min(CurrentStamina, MaxStamina);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // Make sure we are grounded and moving to trigger the sound
            if (IsGrounded && CharacterVelocity.magnitude > 0.2f)
            {
                // Check if the object has a NoisySurface component
                NoisySurface surface = other.gameObject.GetComponent<NoisySurface>();

                if (surface != null)
                {
                    // Check if the cooldown has passed
                    if (Time.time >= m_LastTimeNoisySurfaceSfxPlayed + NoisySurfaceSfxCooldown)
                    {
                        // Use AudioManager to play the sound at the collision point
                        if (AudioManager.Instance != null)
                        {
                            AudioManager.Instance.PlayNoisySurfaceSound(surface, transform.position);

                            // Reset cooldown timer
                            m_LastTimeNoisySurfaceSfxPlayed = Time.time;
                        }
                    }
                }
            }
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
                // Debug.Log("No obstructions detected, can stand up!");
                
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

            // --- New Animator Logic ---
            if (animator != null)
            {
                animator.SetBool("IsCrouching", IsCrouching);
            }
            // --- End New Animator Logic ---

            return true;
        }

        public void Freeze()
        {
            CanMove = false;
        }

        public void SetHidingState(bool hiding)
        {
            IsHiding = hiding;
            m_Controller.enabled = !hiding;
        }

        public void Stun(float duration)
        {
            if (isStunned) return;
            StartCoroutine(StunCoroutine(duration));
        }

        private IEnumerator StunCoroutine(float duration)
        {
            isStunned = true;
            CanMove = false;
            Debug.Log($"Player stunned for {duration} seconds.");

            // You can add visual/audio feedback for the stun effect here

            yield return new WaitForSeconds(duration);

            // Only re-enable movement if the game is in the Gameplay state
            if (GameStateManager.Instance.CurrentState == GameState.Gameplay)
            {
                CanMove = true;
            }
            isStunned = false;
            Debug.Log("Player stun ended.");
        }
    
        // Movement is now controlled by the GameStateManager.
        // The following methods are no longer needed.
        /*
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
        */

        public void Respawn(Vector3 position, Quaternion rotation)
        {
            // Reset state flags
            IsHiding = false;
            isStunned = false;

            // Reset physics, position, and orientation
            CharacterVelocity = Vector3.zero;
            m_CameraVerticalAngle = 0f;
            PlayerCamera.transform.localEulerAngles = Vector3.zero;

            // Use a temporary disable/enable of the CharacterController to properly teleport the player
            m_Controller.enabled = false;
            transform.position = position;
            transform.rotation = rotation;
            m_Controller.enabled = true;

            // Reset stats
            CurrentStamina = MaxStamina;

            // Re-sync with the current game state to ensure movement is correctly enabled/disabled
            HandleGameStateChanged(GameStateManager.Instance.CurrentState);

            Debug.Log("Player has respawned.");
        }

        // This method will be called by the event
        private void UpdateMouseSensitivity(float newSensitivity)
        {
            MouseSensitivity = newSensitivity;
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;

            // Transform the world-space velocity of the character controller to local space
            Vector3 localVelocity = transform.InverseTransformDirection(m_Controller.velocity);
            
            Vector3 horizontalVelocity = new Vector3(localVelocity.x, 0, localVelocity.z);
            float speed = horizontalVelocity.magnitude;
            
            // // Get the current animator values for smoothing
            // float currentVelocityX = animator.GetFloat("VelocityX");
            // float currentVelocityZ = animator.GetFloat("VelocityZ");

            // // Smooth the transitions
            // float smoothedVelocityX = Mathf.Lerp(currentVelocityX, localVelocity.x, Time.deltaTime * 10f);
            // float smoothedVelocityZ = Mathf.Lerp(currentVelocityZ, localVelocity.z, Time.deltaTime * 10f);

            // Set the animator parameters
            // animator.SetFloat("VelocityX", smoothedVelocityX);
            // animator.SetFloat("VelocityZ", smoothedVelocityZ);

            animator.SetFloat("Velocity", speed);
            animator.SetFloat("VelocityX", localVelocity.x);
            animator.SetFloat("VelocityZ", localVelocity.z);
        }
    }
}