using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class WallMovement : MonoBehaviour
{
    [Header("Detección de Paredes")]
    [SerializeField] private float wallDetectionDistance = 1.5f;
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float raycastHeightOffset = 1.2f; 

    [Header("Fuerzas de Salto")]
    [SerializeField] private float wallJumpSideForce = 8f;
    [SerializeField] private float wallJumpUpForce = 10f;
    
    [Header ("Wall Run")]
    [SerializeField] private float wallRunSpeed = 6f;
    
    private Transform lastWallJumped;
    private ThirdPersonController _controller;
    private Animator _animator;

    private RaycastHit hitRight;
    private RaycastHit hitLeft;
    private RaycastHit hitFront;
    private bool wallOnRight;
    private bool wallOnLeft;
    private bool wallOnFront;
    private float lastWallJumpTime;

    private void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_controller.Grounded)
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

        if (wallOnRight || wallOnLeft || wallOnFront)
        {
            WallRun();
            WallJump();
        }
        else
        {
            
            //_animator.SetBool("IsWallRunning", false);
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

    private void WallRun()
    {
        bool isAirborne = !_controller.Grounded;
        bool isKeyHeld = Keyboard.current.xKey.isPressed; 

        if (isAirborne && isKeyHeld && (wallOnRight || wallOnLeft))
        {
            Vector3 wallNormal = wallOnRight ? hitRight.normal : hitLeft.normal;
            Vector3 wallRunDirection = Vector3.Cross(wallNormal,Vector3.up);

            if (Vector3.Dot(wallRunDirection, transform.forward)< 0)
            {
                wallRunDirection = -wallRunDirection;
            }

            _controller.SetWallRun(true, wallRunDirection, wallRunSpeed);
            _animator.SetBool("IsWallRunning", true);
        }
        else
        {
            _controller.SetWallRun(false, Vector3.zero, 0f);
            _animator.SetBool("IsWallRunning", false);
        }
    }

    private void WallJump()
    {
        if (Keyboard.current.yKey.wasPressedThisFrame)
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

               
                Vector3 jumpDirection = (wallNormal * wallJumpSideForce) + (Vector3.up * wallJumpUpForce);
                _controller.ApplyWallJumpImpulse(jumpDirection);

                _animator.SetTrigger("WallJumpTrigger");
            }
        }
    }
}