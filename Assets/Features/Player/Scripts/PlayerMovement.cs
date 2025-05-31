using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Range(0f, 1f)] private float acceleration = 0.3f;
    [SerializeField] private float speed = 2.0f;
    [SerializeField, Range(0f, 1f)] private float drag = 0.9f;
    [Space(5)]

    [Header("Jump")]
    [SerializeField] private float jumpForce = 60.0f;
    [Space(5)]

    [Header("Double Jump")]
    [SerializeField] private bool doubleJumpEnabled = false;
    [SerializeField] private float doubleJumpForce = 30.0f;
    [Space(5)]

    [Header("Dash")]
    [SerializeField] private bool dashEnabled = false;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [Space(5)]

    [Header("Wall Slide")]
    [SerializeField] private bool wallSlideEnabled = false;
    [SerializeField] private float wallSlideForce = 2f;
    [Space(5)]

    [Header("Wall Jump")]
    [SerializeField] private bool wallJumpEnabled = false;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(4f, 4f);
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private float wallJumpingTime = 0.2f;
    [Space(10)]

    [Header("Layers")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [Space(5)]

    [Header("Layer's Checkers")]
    [SerializeField] private BoxCollider2D groundCheck;
    [SerializeField] private BoxCollider2D wallCheck;

    private Rigidbody2D rb;
    private float xInput;

    private bool isDashing;
    private float dashTimeLeft;
    private float lastDashTime = -Mathf.Infinity;

    private bool canDoubleJump;

    private bool isWallSliding;

    private float wallJumpingCounter;
    private float wallJumpingDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

    }

    void Update()
    {
        xInput = Input.GetAxis("Horizontal");

        if (CanJump())
        {
            Jump();
        }

        if (CanDash())
        {
            Dash();
        }

        if (CanDoubleJump())
        {
            DoubleJump();
        }

        if (CanWallSliding())
        {
            isWallSliding = true;
            WallSlide();
        } 
        else
        {
            isWallSliding = false;
        }

        if (CanWallJump())
        {
            WallJump();
        }

        if (IsGrounded())
        {
            canDoubleJump = true;
        }
    }

    void FixedUpdate()
    {
        ApplyFriction();

        if (!isDashing)
        {
            Move();
        }

        UpdateDashTimer();
    }


    #region Movement
    private void ApplyFriction()
    {
        if (IsGrounded() && xInput == 0 && rb.velocity.y <= 0)
        {
            rb.velocity *= drag;
        }
    }
    private void Move()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(rb.velocity.x + increment, -speed, speed);
            rb.velocity = new Vector2(newSpeed, rb.velocity.y);

            float yRotation = xInput > 0 ? 0f : 180f;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
    #endregion

    #region Jump
    private bool CanJump()
    {
        return Input.GetButtonDown("Jump") && IsGrounded();
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    #endregion

    #region Dash
    private bool CanDash()
    {
        return dashEnabled && Input.GetButtonDown("Dash") && Time.time >= lastDashTime + dashCooldown && !isDashing;
    }

    private void Dash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;
        StartDash();
    }

    private void StartDash()
    {
        float dashDirection = transform.rotation.y >= 0 ? 1f : -1f;
        rb.velocity = new Vector2(dashForce * dashDirection, 0f);
    }

    private void UpdateDashTimer()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;

            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
            }
        }
    }
    #endregion

    #region Duble Jump
    private bool CanDoubleJump()
    {
        return doubleJumpEnabled && !IsGrounded();
    }

    private void DoubleJump()
    {
        if (!IsGrounded() && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
            canDoubleJump = false;
        }
    }
    #endregion

    #region Wall Slide
    private bool CanWallSliding()
    {
        return wallSlideEnabled && IsWalled() && !IsGrounded();
    }

    private void WallSlide()
    {
        float slideSpeed = Mathf.Clamp(rb.velocity.y, -wallSlideForce, float.MaxValue);
        rb.velocity = new Vector2(rb.velocity.x, slideSpeed);
    }
    #endregion

    #region Wall Jump
    private bool CanWallJump()
    {
        return wallJumpEnabled;
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            wallJumpingDirection = -transform.right.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            float desiredYRotation = wallJumpingDirection > 0 ? 0f : 180f;

            if (Mathf.Abs(transform.eulerAngles.y - desiredYRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Euler(0f, desiredYRotation, 0f);
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() { }
    #endregion

    private bool IsGrounded()
    {
        return Physics2D.OverlapAreaAll(
            groundCheck.bounds.min,
            groundCheck.bounds.max,
            groundMask
        ).Length > 0;
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapAreaAll(
            wallCheck.bounds.min,
            wallCheck.bounds.max,
            wallMask
        ).Length > 0;
    }
}

