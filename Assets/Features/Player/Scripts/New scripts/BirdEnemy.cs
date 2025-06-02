using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float speed = 4f;
    public int damage = 1;

    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isChasing)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= detectionRadius)
            {
                isChasing = true;
            }
        }
        else
        {
            // Летит к игроку
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
