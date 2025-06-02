using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int healAmount = 1;
    [SerializeField] private bool isConsumable = true;

    [Header("Effects")]
    [SerializeField] private AudioClip healSound;
    [SerializeField] private ParticleSystem healEffect;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask playerLayer = ~0; // По умолчанию все слои
    [SerializeField] private bool useTagInstead = true; // Использовать тег вместо слоя

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with: {collision.gameObject.name}");

        bool isPlayer = false;

        if (useTagInstead)
        {
            isPlayer = collision.gameObject.CompareTag("Player");
            Debug.Log($"Tag check: {isPlayer} (Tag: {collision.gameObject.tag})");
        }
        else
        {
            isPlayer = ((1 << collision.gameObject.layer) & playerLayer) != 0;
            Debug.Log($"Layer check: {isPlayer} (Layer: {LayerMask.LayerToName(collision.gameObject.layer)})");
        }

        if (isPlayer)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            Debug.Log($"PlayerHealth component: {playerHealth != null}");

            if (playerHealth != null)
            {
                playerHealth.ReceiveHealth(healAmount);
                Debug.Log($"Healed player for {healAmount} HP");

                PlayHealEffects();

                if (isConsumable)
                {
                    Debug.Log("Destroying healing item");
                    Destroy(gameObject);
                }
            }
        }
    }

    private void PlayHealEffects()
    {
        if (healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position);
            Debug.Log("Played heal sound");
        }

        if (healEffect != null)
        {
            Instantiate(healEffect, transform.position, Quaternion.identity);
            Debug.Log("Played heal effect");
        }
    }
}