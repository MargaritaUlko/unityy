
using UnityEngine;
using System.Collections; // Для IEnumerator

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private BoxCollider2D wallCheck; // Для проверки стен/края платформы
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float groundCheckDistance = 0.1f; // Для проверки края платформы
    [SerializeField] private Transform groundCheckPoint; // Точка, откуда проверяем землю
    [SerializeField] private Animator animator; // Аниматор для врага

    [Header("Detection & Attack")]
    [SerializeField] private float detectionRange = 5f; // Расстояние, на котором враг видит игрока
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackCooldownTime = 1f; // Перезарядка атаки

    private Rigidbody2D rb;
    private bool movingRight = true;
    private bool isAttacking = false;
    private Transform playerTransform; // Ссылка на трансформ игрока для преследования
    private bool canAttack = true; // Флаг перезарядки атаки

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
        // Поиск игрока, если еще не найден
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player"); // Убедитесь, что у игрока есть тег "Player"
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking) return; // Если атакуем, не двигаемся

        // Проверка наличия игрока в радиусе обнаружения
        bool playerDetected = CheckForPlayer();

        if (playerDetected && playerTransform != null)
        {
            ChasePlayer(); // Преследуем игрока
        }
        else
        {
            Patrol(); // Патрулируем
            DetectWallAndLedge(); // Проверяем стены и края платформ
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
        rb.velocity = new Vector2(directionToPlayer * moveSpeed * 1.2f, rb.velocity.y); // Чуть быстрее при преследовании

        // Отражаем спрайт в сторону игрока
        if (directionToPlayer > 0 && !movingRight)
        {
            Flip();
        }
        else if (directionToPlayer < 0 && movingRight)
        {
            Flip();
        }

        animator.SetBool("IsChasing", true); // Анимация преследования
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Обновляем направление движения для Patrol
        if (direction > 0 && !movingRight)
        {
            Flip();
        }
        else if (direction < 0 && movingRight)
        {
            Flip();
        }
        else if (movingRight && direction < 0) // Если двигались вправо, но повернули налево
        {
            Flip();
        }
        else if (!movingRight && direction > 0) // Если двигались влево, но повернули направо
        {
            Flip();
        }

        animator.SetBool("IsChasing", false); // Отключаем анимацию преследования
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private void DetectWallAndLedge()
    {
        // Проверка стены (как раньше)
        bool wallAhead = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, wallLayer).Length > 0;

        // Проверка края платформы (новая функциональность)
        Vector2 groundCheckPosition = groundCheckPoint.position;
        // Смещаем точку проверки чуть в сторону текущего направления движения
        groundCheckPosition.x += (movingRight ? 0.5f : -0.5f);
        bool ledgeAhead = !Physics2D.OverlapCircle(groundCheckPosition, groundCheckDistance, wallLayer); // Используем wallLayer для земли

        if (wallAhead || ledgeAhead)
        {
            movingRight = !movingRight;
            Flip(); // Сразу переворачиваем спрайт при смене направления
        }
    }

    private void Flip()
    {
        movingRight = !movingRight; // Обновляем состояние движения
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canAttack) return; // Если на перезарядке, не атакуем

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

                StartCoroutine(AttackCooldown()); // Запускаем перезарядку
            }
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        isAttacking = true; // Враг может "замереть" на мгновение после атаки
        animator.SetTrigger("Attack"); // Запускаем анимацию атаки

        yield return new WaitForSeconds(attackCooldownTime);

        isAttacking = false;
        canAttack = true;
    }

    // Для визуализации радиуса обнаружения игрока в редакторе
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