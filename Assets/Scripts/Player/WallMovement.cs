using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class WallMovement : MonoBehaviour
{
    private RaycastHit hitRight;
    private RaycastHit hitLeft;
    [SerializeField] private bool wallOnRight;
    [SerializeField] private bool wallOnLeft;
    [SerializeField] private float wallDetectionDistance;
    [SerializeField] private string wallTag;
    [SerializeField] private ThirdPersonController _controller;

    [SerializeField] private float  wallJumpSideForce = 8f;

    [SerializeField] private float wallJumpUpForce = 10f;

    private Animator _animator;

    private void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Detector();
    }

    private void Detector()
    {
        wallOnRight = CheckWall(transform.right, out hitRight);

        wallOnLeft = CheckWall(-transform.right, out hitLeft);

        if (wallOnRight || wallOnLeft)
        {
            WallRun();
            WallJump();
        }
    }

    private bool CheckWall(Vector3 direction, out RaycastHit hitInfo)
    {
                            //Origen            //Dirección //Hit       //Distancia
        if (Physics.Raycast(transform.position, direction, out hitInfo, wallDetectionDistance))
        {
            return hitInfo.transform.CompareTag(wallTag);
        }
        else
        {
            return false;
        }
    }

    private void WallRun()
    {
        bool isNearWall = wallOnRight || wallOnLeft;
        bool isAirborne = !_controller.Grounded;
        bool isKeyHeld = Keyboard.current.xKey.isPressed;

        Debug.Log("isNearWall: " + isNearWall + " | isAirborne: " + isAirborne + " | isKeyHeld: " + isKeyHeld);

        if (isNearWall && isAirborne && isKeyHeld)
        {
            _animator.SetBool("IsWallRunning", true);
        }
        else
        {
            _animator.SetBool("IsWallRunning", false);
        }
    }     
    
    private void WallJump()
    {
        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            Vector3 wallNormal = wallOnRight ? hitRight.normal : hitLeft.normal;

            Vector3 jumpDirection = wallNormal * wallJumpSideForce + Vector3.up * wallJumpUpForce;

            _controller.ApplyWallJumpImpulse(jumpDirection);

            _animator.SetTrigger("WallJumpTrigger");
        }    
    }

}
