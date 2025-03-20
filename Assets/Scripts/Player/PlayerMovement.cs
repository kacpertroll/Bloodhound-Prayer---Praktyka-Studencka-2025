using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Movement Settings
    [Header("Movement Settings")] // Ustawienia ruchu
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public float slideSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    #endregion
    [Space]
    #region Crouch
    [Header("Crouch Settings")] // Ustawienia kucania
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchTransitionSpeed = 5f;
    #endregion
    [Space]
    #region Slide
    [Header("Slide Settings")] // Ustawienia �lizgu
    public float slideDuration = 0.75f;
    #endregion
    [Space]
    #region Dash
    [Header("Dash")] // Ustawienia dasha
    public Transform cameraHolder;
    public AnimationCurve dashSpeedCurve;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    #endregion
    [Space]
    #region Wall Jump
    [Header("Wall Jump/Run Settings")]
    public LayerMask wallLayer;
    public float wallJumpForce = 7f;
    public float wallRunGravity = -2f;

    private bool isWallJumping;
    private float wallJumpTimer;
    #endregion

    // Wa�ne zmienne!!!
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
        HandleWallJump();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = walkSpeed; // Aktualna pr�dko�� ruchu (do sterowania Head Bobbingiem)

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

        // Smooth zmiana wysoko�ci
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
    }

    private void HandleSlide()
    {
        // Warunek: Sprint + Kucni�cie = �lizg
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

        // Rozpocz�cie dasha
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
            float dashProgress = dashTimer / dashDuration; // Jak daleko w dasha jeste�my (0-1)
            float currentSpeed = dashSpeed * dashSpeedCurve.Evaluate(dashProgress); // Pr�dko�� wg krzywej

            controller.Move(dashDirection * currentSpeed * Time.deltaTime);

            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown; // Reset cooldownu
            }
        }

        if (dashCooldownTimer > 0) // Sprawdzanie cooldownu dasha
        {
            Debug.Log("Dash Cooldown: " + Mathf.Floor(dashCooldownTimer)); // Wy�wietlanie czasu cooldownu dasha do sprawdzania
        }
    }

    private void HandleWallJump()
    {
        // Sprawdzenie, czy gracz dotyka �ciany
        bool isTouchingWall = Physics.Raycast(transform.position, transform.right, 1f, wallLayer) ||
                              Physics.Raycast(transform.position, -transform.right, 1f, wallLayer);

        bool isMovingForward = Input.GetAxis("Vertical") > 0;

        // Je�li gracz dotyka �ciany, nie jest na ziemi i naciska skok � wykonujemy wall jump
        if (isTouchingWall && !isGrounded && Input.GetButtonDown("Jump") && !isWallJumping)
        {
            isWallJumping = true;
            wallJumpTimer = 1f;

            // Okre�lenie kierunku odbicia od �ciany
            Vector3 jumpDirection = (transform.forward * 0.5f + Vector3.up).normalized; // Mniej ruchu w bok, wi�cej do g�ry

            // Nadanie si�y skoku
            velocity.x = jumpDirection.x * wallJumpForce;
            velocity.y = jumpDirection.y * wallJumpForce;
            velocity.z = jumpDirection.z * wallJumpForce;
        }


        // Je�li wall jump trwa, licznik odlicza
        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;

            // Reset po zako�czeniu wall jumpa
            if (wallJumpTimer <= 0 || isGrounded)
            {
                isWallJumping = false;
                velocity.x = 0; // Reset poziomego ruchu po skoku
                velocity.z = 0;
            }
        }

        if (isTouchingWall && !isGrounded && isMovingForward && !isWallJumping)
        {
            velocity.y = wallRunGravity; // Spowolnienie opadania
        }
    }

    // Metody zwracaj�ce boola
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
