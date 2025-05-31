using UnityEngine;

public class EnemyChaseOnSight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform agroZone;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackForce = 5f;

    private Rigidbody2D rb;
    private bool isChasing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        DetectPlayer();

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            StopChase();
        }
    }

    private void DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapAreaAll(
            agroZone.GetComponent<Collider2D>().bounds.min,
            agroZone.GetComponent<Collider2D>().bounds.max,
            playerLayer
        );

        isChasing = hits.Length > 0;
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        float xDirection = Mathf.Sign(player.position.x - transform.position.x);
        float yDirection = Mathf.Sign(player.position.y - transform.position.y);
        rb.velocity = new Vector2(xDirection * moveSpeed, yDirection * moveSpeed);
    }

    private void StopChase()
    {
        rb.velocity = new Vector2(0f, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
            }
        }
    }
}
