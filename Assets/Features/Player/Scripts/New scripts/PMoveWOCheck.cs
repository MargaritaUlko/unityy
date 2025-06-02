using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMoveWOCheck : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 15f;
    public float dashCooldown = 1f;

    [Header("Wall Check")]
    public Transform wallCheckPoint;         // Точка перед игроком для проверки стены
    public float wallCheckRadius = 0.1f;     // Радиус проверки
    public LayerMask wallLayer;              // Что считать стеной

    private Rigidbody2D rb;
    private bool canDash = true;
    private bool isFacingRight = true;

    private int maxJumps = 2;
    private int jumpCount = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;

        if (isGrounded)
        {
            jumpCount = 0;
        }

        // Проверка стены (можно использовать в будущем)
        bool isTouchingWall = IsTouchingWall();

        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if (move > 0 && !isFacingRight) Flip();
        if (move < 0 && isFacingRight) Flip();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        float direction = isFacingRight ? 1f : -1f;

        rb.velocity = new Vector2(direction * dashForce, 0f);

        yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheckPoint.position, wallCheckRadius, wallLayer);
    }
}
