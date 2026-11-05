using UnityEngine;
using UnityEngine.InputSystem;
using CharacterScripts;

public class WallMovement : MonoBehaviour
{
    [Header("Detección de Paredes")]
    [SerializeField] private float wallDetectionDistance = 0.8f;
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float raycastHeightOffset = 1.2f;

    [Header("Fuerzas de Salto")]
    [SerializeField] private float wallJumpSideForce = 8f;
    [SerializeField] private float wallJumpUpForce = 10f;

    [Header("Wall Run")]
    [SerializeField] private float wallRunSpeed = 6f;

    [Header("Audio - Bloque C")]
    [SerializeField] private AudioSource sfxWallJump;
    [SerializeField] private AudioSource sfxWallRun;

    private Transform lastWallJumped;
    private CharacterControllerCs _controller;
    private Animator _animator;

    private RaycastHit hitRight;
    private RaycastHit hitLeft;
    private RaycastHit hitFront;
    private bool wallOnRight;
    private bool wallOnLeft;
    private bool wallOnFront;

    private bool isWallRunningActive = false;

    private void Start()
    {
        _controller = GetComponent<CharacterControllerCs>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_controller.IsGrounded)
        {
            lastWallJumped = null;
        }
        Detector();
    }

    private void Detector()
    {
        Vector3 rayOrigin = transform.position + (Vector3.up * raycastHeightOffset);

        wallOnRight = CheckWall(rayOrigin, transform.right, out hitRight);
        wallOnLeft = CheckWall(rayOrigin, -transform.right, out hitLeft);
        wallOnFront = CheckWall(rayOrigin, transform.forward, out hitFront);

        EvaluatedWallRunLogic();

        if (wallOnRight || wallOnLeft || wallOnFront)
        {
            WallJump();
        }
    }

    private bool CheckWall(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
    {
        if (Physics.Raycast(origin, direction, out hitInfo, wallDetectionDistance))
        {
            return hitInfo.transform.CompareTag(wallTag);
        }
        return false;
    }

    private void EvaluatedWallRunLogic()
    {
        bool isAirborne = !_controller.IsGrounded;

        bool r2Pressed = false;
        if (Gamepad.current != null)
        {
            r2Pressed = Gamepad.current.rightTrigger.isPressed;
        }
        else if (Keyboard.current != null)
        {
            r2Pressed = Keyboard.current.leftShiftKey.isPressed;
        }

        bool canWallRun = isAirborne && r2Pressed && (wallOnRight || wallOnLeft);

        if (canWallRun)
        {
            Vector3 wallNormal = wallOnRight ? hitRight.normal : hitLeft.normal;
            Vector3 wallRunDirection = Vector3.Cross(wallNormal, Vector3.up);

            if (Vector3.Dot(wallRunDirection, transform.forward) < 0)
            {
                wallRunDirection = -wallRunDirection;
            }

            _controller.SetWallRun(true, wallRunDirection, wallRunSpeed);
            SetWallRunSound(true);

            if (!isWallRunningActive)
            {
                isWallRunningActive = true;
                if (_animator != null)
                {
                    _animator.SetBool("IsWallRunning", true);
                }
            }

            if (_animator != null)
            {
                _animator.SetBool("IsWallRight", wallOnRight);
            }
        }
        else
        {
            if (isWallRunningActive)
            {
                isWallRunningActive = false;
                _controller.SetWallRun(false, Vector3.zero, 0f);
                SetWallRunSound(false);

                if (_animator != null)
                {
                    _animator.SetBool("IsWallRunning", false);
                }
            }
        }
    }

    private void WallJump()
    {
        bool xPressed = false;
        if (Gamepad.current != null)
        {
            xPressed = Gamepad.current.buttonSouth.wasPressedThisFrame;
        }
        else if (Keyboard.current != null)
        {
            xPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        if (xPressed)
        {
            Transform currentWall = null;
            Vector3 wallNormal = Vector3.zero;

            if (wallOnRight)
            {
                currentWall = hitRight.transform;
                wallNormal = hitRight.normal;
            }
            else if (wallOnLeft)
            {
                currentWall = hitLeft.transform;
                wallNormal = hitLeft.normal;
            }
            else if (wallOnFront)
            {
                currentWall = hitFront.transform;
                wallNormal = hitFront.normal;
            }

            if (currentWall != null && currentWall != lastWallJumped)
            {
                lastWallJumped = currentWall;
                isWallRunningActive = false;

                Vector3 jumpDirection = (wallNormal * wallJumpSideForce) + (Vector3.up * wallJumpUpForce);
                _controller.ApplyWallJumpImpulse(jumpDirection);
                if (sfxWallJump != null) sfxWallJump.Play();

                if (_animator != null)
                {
                    _animator.SetBool("IsWallRunning", false);
                    _animator.SetTrigger("WallJumpTrigger");
                }
            }
        }
    }

    private void SetWallRunSound(bool active)
    {
        if (sfxWallRun == null) return;

        if (active && !sfxWallRun.isPlaying) sfxWallRun.Play();
        else if (!active && sfxWallRun.isPlaying) sfxWallRun.Stop();
    }
}