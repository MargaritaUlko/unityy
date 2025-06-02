using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatEnemy : MonoBehaviour
{
    public float speed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public int damage = 1;

    private bool movingRight = true;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime * (movingRight ? 1 : -1));

        // Проверка на обрыв (или конец платформы)
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        if (!groundInfo.collider)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}