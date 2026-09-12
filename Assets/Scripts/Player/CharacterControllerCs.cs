using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace CharacterScripts
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class CharacterControllerCs : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        [SerializeField] private float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        [SerializeField] private float sprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        [SerializeField] private float rotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        [SerializeField] private float speedChangeRate = 10.0f;

        [SerializeField] private AudioSource audioFootsteps;
        [SerializeField] private AudioSource landingAudio;
        [SerializeField] private AudioSource audioFoley;
        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [Range(0, 1)][SerializeField] private float footstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        [SerializeField] private float jumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        [SerializeField] private float gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        [SerializeField] private float jumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] private float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not.")]
        [SerializeField] private bool grounded = true;

        // Propiedad pública única para que WallMovement lea el estado de forma segura
        public bool IsGrounded => grounded;

        [Tooltip("Useful for rough ground")]
        [SerializeField] private float groundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [SerializeField] private float groundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        [SerializeField] private LayerMask groundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField] private GameObject cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        [SerializeField] private float topClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        [SerializeField] private float bottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        [SerializeField] private float cameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        [SerializeField] private bool lockCameraPosition = false;

        // Internal State Variables
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        [Header("Wall Run")]
        private Vector3 _externalImpulse = Vector3.zero;
        [SerializeField] private float wallRunGravityDamping = 5f;

        private bool _isWallRunning = false;
        private Vector3 _wallRunDirection = Vector3.zero;
        private float _wallRunSpeed = 0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;
        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies.");
#endif

            AssignAnimationIDs();

            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, grounded);
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !lockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (_isWallRunning)
            {
                _controller.Move(_wallRunDirection.normalized * (_wallRunSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
            else
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime + _externalImpulse * Time.deltaTime);
                _externalImpulse = Vector3.Lerp(_externalImpulse, Vector3.zero, Time.deltaTime * 5f);
            }

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        public void SetWallRun(bool isActive, Vector3 direction, float speed)
        {
            _isWallRunning = isActive;
            _wallRunDirection = direction;
            _wallRunSpeed = speed;
        }

        private void JumpAndGravity()
        {
            if (grounded)
            {
                _fallTimeoutDelta = fallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_isWallRunning)
            {
                _verticalVelocity = Mathf.Lerp(_verticalVelocity, 0f, Time.deltaTime * wallRunGravityDamping);
            }
            else if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        public void ApplyWallJumpImpulse(Vector3 impulse)
        {
            _externalImpulse = impulse;
            _verticalVelocity = impulse.y;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? transparentGreen : transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (audioFootsteps != null) audioFootsteps.Play();
                if (audioFoley != null) audioFoley.Play();
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (landingAudio != null) landingAudio.Play();
            }
        }
    }
}