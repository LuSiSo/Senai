using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ===== GROUND CHECK =====
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // ===== SPEED (Idle / Walk / Run) =====
        float speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", speed);

        // ===== VERTICAL SPEED (Jump / Fall) =====
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);

        // ===== GROUND STATE =====
        animator.SetBool("IsGrounded", isGrounded);
    }
}
