using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WeaponAttack : MonoBehaviour
{
    public string enemyTag = "Enemy";
    public Animator animator;
    public float attackCooldown = 0.5f;

    private BoxCollider2D weaponCollider;
    private bool canAttack = true;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider2D>();
        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false; // Изначально выключен
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    public void EnableHitbox()
    {
        weaponCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (weaponCollider.enabled && other.CompareTag(enemyTag))
        {
            Destroy(other.gameObject); // Уничтожить врага
        }
    }
}
