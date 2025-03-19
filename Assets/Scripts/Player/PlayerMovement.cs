using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] // Ustawienia ruchu
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public float slideSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    [Space]
    [Header("Crouch Settings")] // Ustawienia kucania
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchTransitionSpeed = 5f;
    [Space]
    [Header("Slide Settings")] // Ustawienia œlizgu
    public float slideDuration = 0.75f;
    [Space]
    [Header("Dash")] // Ustawienia dasha
    public Transform cameraHolder;
    public AnimationCurve dashSpeedCurve;

    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;
    [Space]
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    // Wa¿ne zmienne!!!
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSliding;
    private float slideTimer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = controller.isGrounded; // Fajnie bo Char Contrl ma wbudowane sprawdzanie czy jest na ziemi

        HandleMovement();
        HandleCrouch();
        HandleSlide();
        HandleGravity();
        HandleDash();

        if (dashCooldownTimer > 0)
        {
            Debug.Log("Dash Cooldown: " + Mathf.Floor(dashCooldownTimer)); // Wyœwietlanie czasu cooldownu dasha do sprawdzania
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = walkSpeed; // Aktualna prêdkoœæ ruchu (do sterowania Head Bobbingiem)

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isSliding)
            currentSpeed = sprintSpeed;
        if (isCrouching)
            currentSpeed = crouchSpeed;
        if (isSliding)
            currentSpeed = slideSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Grawitacja bo Char Contrl nie ma
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isSliding)
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            isCrouching = false;
        }

        // Smooth zmiana wysokoœci
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    private void HandleSlide()
    {
        // Warunek: Sprint + Kucniêcie = Œlizg
        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftShift) && !isSliding && isGrounded)
        {
            isSliding = true;
            slideTimer = slideDuration;
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
            }
        }
    }

    private void HandleDash()
    {
        // Cooldown odliczanie
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // Rozpoczêcie dasha
        if (Input.GetKeyDown(KeyCode.E) && dashCooldownTimer <= 0 && !isDashing)
        {
            isDashing = true;
            dashTimer = 0f; // Resetujemy timer dasha
            dashDirection = cameraHolder.forward.normalized; // Kierunek patrzenia kamery
        }

        // Wykonanie dasha
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            float dashProgress = dashTimer / dashDuration; // Jak daleko w dasha jesteœmy (0-1)
            float currentSpeed = dashSpeed * dashSpeedCurve.Evaluate(dashProgress); // Prêdkoœæ wg krzywej

            controller.Move(dashDirection * currentSpeed * Time.deltaTime);

            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown; // Reset cooldownu
            }
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsMoving()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    public bool IsSliding()
    {
        return isSliding;
    }
}
