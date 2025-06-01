using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    public string enemyTag = "Enemy";
    public Animator animator;
    public float attackDuration = 0.5f;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private System.Collections.IEnumerator Attack()
    {
        isAttacking = true;

        // ������ �������� �����
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // �������� ��������� �� �������� �����
        Collider weaponCollider = GetComponent<Collider>();
        if (weaponCollider != null)
            weaponCollider.enabled = true;

        yield return new WaitForSeconds(attackDuration);

        if (weaponCollider != null)
            weaponCollider.enabled = false;

        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag(enemyTag))
        {
            Destroy(other.gameObject); // ����� �����
        }
    }
}
