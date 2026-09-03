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

    private void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
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
            Debug.Log("Wall run activo");
        }
    }     
    
    private void WallJump()
    {
        
    }

}
