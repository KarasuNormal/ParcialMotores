using UnityEngine;
using UnityEngine.InputSystem;

public class WallMovement : MonoBehaviour
{
    //Detector
    [SerializeField] private Animator animator;
    [SerializeField] private float wallDetectionDistance = 2f;  //A revisar
    [SerializeField] private string wallTag = "Wall";

    private void Update()
    {
        Detector();

    }

    private void Detector()
    {
        RaycastHit hit;    //En esta variable quedan almacenados los datos del hit

        //Origen            //Dirección         //Hit   //Distancia
        if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectionDistance))
        {
            if (hit.transform.CompareTag(wallTag))    //Si detecta x cosa con tag "Wall"...
            {
                WallRun();
                WallJump();
            }
        }
    }

    private void WallRun()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {

        }
    }

    void WallJump()
    {
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {

        }
    }
}
