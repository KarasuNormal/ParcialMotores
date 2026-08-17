using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Movimiento")]
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Salto")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -25f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1.0f;
    private bool isDashing = false;
    private float dashTimeCounter;
    private float dashCooldownCounter;
    private Vector3 dashDirection;

    private Vector2 movementInput;
    private bool jumpPressed;
    private bool dashPressed;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.deltaTime;
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimeCounter -= Time.deltaTime;

            if (dashTimeCounter <= 0)
            {
                isDashing = false;
            }
            return;
        }

        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        controller.Move(move * playerSpeed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (jumpPressed && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            jumpPressed = false; // Consumir el salto
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (dashPressed && dashCooldownCounter <= 0 && move != Vector3.zero)
        {
            isDashing = true;
            dashTimeCounter = dashTime;
            dashCooldownCounter = dashCooldown;
            dashDirection = move.normalized;
            dashPressed = false;
        }
    }
    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
        }
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed)
        {
            dashPressed = true;
        }
    }
}