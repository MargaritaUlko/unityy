using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private string[] ignoreTags = { "Projectile", "Enemy" }; // ���� ��� �������������

    private Rigidbody2D rb;

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

    public void Launch(Vector2 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���������� �������� ������������
        if (collision.CompareTag("Untagged") || collision.CompareTag("Projectile"))
            return;

        Debug.Log($"Projectile hit: {collision.gameObject.name} (Tag: {collision.tag})");

        // ������������ ������ ������������ � �������
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // ���������� playerHealth ������ health
                Debug.Log($"Dealt {damage} damage to player");
            }
            else
            {
                Debug.LogError("PlayerHealth component not found on player!");
            }
        }

        Destroy(gameObject);
    }
}
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    // ��� ����� ���������� ������������ � ������� ��� ������
    //    Destroy(gameObject);
    //}

