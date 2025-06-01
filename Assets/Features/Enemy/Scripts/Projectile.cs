using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float bounceForce = 3f; // ���� �������
    [SerializeField] private string[] ignoreTags = { "Projectile", "Enemy" };
    [SerializeField] private LayerMask bounceLayers; // ����, �� ������� ����� �����������

    private Rigidbody2D rb;
    private Vector2 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        lastVelocity = rb.velocity; // ���������� �������� ��� ������� �������
    }

    public void Launch(Vector2 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������, ����� �� ��������� �� ����� �������
        if (ShouldBounce(collision.collider))
        {
            Bounce(collision);
            return;
        }

        // ��������� ������ (�����, ������������ ����)
        foreach (string tag in ignoreTags)
        {
            if (collision.collider.CompareTag(tag)) return;
        }

        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    private bool ShouldBounce(Collider2D collision)
    {
        // ���������, ���� �� ������ � ����� bounceLayers
        return bounceLayers == (bounceLayers | (1 << collision.gameObject.layer));
    }

    private void Bounce(Collision2D collision)
    {
        // ������������ ������� �����������
        Vector2 normal = collision.contacts[0].normal;

        // ������������ ����������� �������
        Vector2 reflectDirection = Vector2.Reflect(lastVelocity.normalized, normal);

        // ��������� ����� �������� � ����������� ����� �������
        rb.velocity = reflectDirection * speed * bounceForce;

        Debug.Log($"Bounced! New velocity: {rb.velocity}");
    }
}
