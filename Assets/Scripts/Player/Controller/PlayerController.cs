using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Referências")]
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    [Header("Movimento")]
    public float speed = 6f;
    public float jumpForce = 6f;

    private float moveInput;
    private float facingDirection = 1f;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    private bool canDash = true;
    private bool isDashing;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    private bool isWallTouch;
    private bool isWallSliding;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce = new Vector2(7f, 10f);
    public float wallSlideSpeed = 2f;
    public float wallJumpLockTime = 0.15f;

    private bool isWallJumping;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // ===== GROUND =====
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // ===== WALL DETECT =====
        isWallTouch = Physics2D.OverlapCircle(
            wallCheck.position,
            0.2f,
            wallLayer
        );

        // ===== COYOTE =====
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // ===== WALL SLIDE =====
        isWallSliding = isWallTouch && !isGrounded && rb.linearVelocity.y < 0;

        // ===== DIREÇÃO REAL =====
        if (rb.linearVelocity.x > 0.1f)
            facingDirection = 1f;
        else if (rb.linearVelocity.x < -0.1f)
            facingDirection = -1f;

        // ===== FLIP (CORRETO E ESTÁVEL) =====
        if (spriteRenderer != null)
            spriteRenderer.flipX = facingDirection < 0f;

        // ===== WALLCHECK SEGUE DIREÇÃO =====
        if (wallCheck != null)
        {
            wallCheck.position = transform.position + new Vector3(0.5f * facingDirection, 0, 0);
        }

        // Animator
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void FixedUpdate()
    {
        if (isDashing || isWallJumping) return;

        rb.linearVelocity = new Vector2(
            moveInput * speed,
            rb.linearVelocity.y
        );

        // Wall slide limit
        if (isWallSliding)
        {
            if (rb.linearVelocity.y < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    -wallSlideSpeed
                );
            }
        }
    }

    // ===== INPUT MOVE =====
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
    }

    // ===== JUMP =====
    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        // normal jump
        if (coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            coyoteTimeCounter = 0f;
        }
        // wall jump
        else if (isWallSliding)
        {
            WallJump();
        }
    }

    void WallJump()
    {
        isWallJumping = true;

        float dir = -facingDirection;

        rb.linearVelocity = new Vector2(
            dir * wallJumpForce.x,
            wallJumpForce.y
        );

        StartCoroutine(WallJumpLock());
    }

    IEnumerator WallJumpLock()
    {
        yield return new WaitForSeconds(wallJumpLockTime);
        isWallJumping = false;
    }

    // ===== DASH =====
    public void OnSprint(InputValue value)
    {
        if (value.isPressed && canDash)
            StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        rb.linearVelocity = new Vector2(
            facingDirection * dashForce,
            0f
        );

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
}
