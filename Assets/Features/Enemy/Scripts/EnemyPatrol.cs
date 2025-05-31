
using UnityEngine;
using System.Collections; // ��� IEnumerator

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private BoxCollider2D wallCheck; // ��� �������� ����/���� ���������
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float groundCheckDistance = 0.1f; // ��� �������� ���� ���������
    [SerializeField] private Transform groundCheckPoint; // �����, ������ ��������� �����
    [SerializeField] private Animator animator; // �������� ��� �����

    [Header("Detection & Attack")]
    [SerializeField] private float detectionRange = 5f; // ����������, �� ������� ���� ����� ������
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackCooldownTime = 1f; // ����������� �����

    private Rigidbody2D rb;
    private bool movingRight = true;
    private bool isAttacking = false;
    private Transform playerTransform; // ������ �� ��������� ������ ��� �������������
    private bool canAttack = true; // ���� ����������� �����

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // ����� ������, ���� ��� �� ������
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player"); // ���������, ��� � ������ ���� ��� "Player"
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking) return; // ���� �������, �� ���������

        // �������� ������� ������ � ������� �����������
        bool playerDetected = CheckForPlayer();

        if (playerDetected && playerTransform != null)
        {
            ChasePlayer(); // ���������� ������
        }
        else
        {
            Patrol(); // �����������
            DetectWallAndLedge(); // ��������� ����� � ���� ��������
        }
    }

    private bool CheckForPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        return hitPlayer != null;
    }

    private void ChasePlayer()
    {
        float directionToPlayer = Mathf.Sign(playerTransform.position.x - transform.position.x);
        rb.velocity = new Vector2(directionToPlayer * moveSpeed * 1.2f, rb.velocity.y); // ���� ������� ��� �������������

        // �������� ������ � ������� ������
        if (directionToPlayer > 0 && !movingRight)
        {
            Flip();
        }
        else if (directionToPlayer < 0 && movingRight)
        {
            Flip();
        }

        animator.SetBool("IsChasing", true); // �������� �������������
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // ��������� ����������� �������� ��� Patrol
        if (direction > 0 && !movingRight)
        {
            Flip();
        }
        else if (direction < 0 && movingRight)
        {
            Flip();
        }
        else if (movingRight && direction < 0) // ���� ��������� ������, �� ��������� ������
        {
            Flip();
        }
        else if (!movingRight && direction > 0) // ���� ��������� �����, �� ��������� �������
        {
            Flip();
        }

        animator.SetBool("IsChasing", false); // ��������� �������� �������������
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private void DetectWallAndLedge()
    {
        // �������� ����� (��� ������)
        bool wallAhead = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, wallLayer).Length > 0;

        // �������� ���� ��������� (����� ����������������)
        Vector2 groundCheckPosition = groundCheckPoint.position;
        // ������� ����� �������� ���� � ������� �������� ����������� ��������
        groundCheckPosition.x += (movingRight ? 0.5f : -0.5f);
        bool ledgeAhead = !Physics2D.OverlapCircle(groundCheckPosition, groundCheckDistance, wallLayer); // ���������� wallLayer ��� �����

        if (wallAhead || ledgeAhead)
        {
            movingRight = !movingRight;
            Flip(); // ����� �������������� ������ ��� ����� �����������
        }
    }

    private void Flip()
    {
        movingRight = !movingRight; // ��������� ��������� ��������
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canAttack) return; // ���� �� �����������, �� �������

        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);

                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }

                StartCoroutine(AttackCooldown()); // ��������� �����������
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        isAttacking = true; // ���� ����� "��������" �� ��������� ����� �����
        animator.SetTrigger("Attack"); // ��������� �������� �����

        yield return new WaitForSeconds(attackCooldownTime);

        isAttacking = false;
        canAttack = true;
    }

    // ��� ������������ ������� ����������� ������ � ���������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Vector2 checkPos = groundCheckPoint.position;
            checkPos.x += (movingRight ? 0.5f : -0.5f);
            Gizmos.DrawWireSphere(checkPos, groundCheckDistance);
        }
    }
}