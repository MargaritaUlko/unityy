using UnityEngine;

public class EnemyCrab : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float detectionRange = 10f;

    private float shootTimer;
    private Transform player;
    private bool playerDead = false;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("����� �� ������! ���������, ��� � ������ ���� ��� 'Player'.", this);
            }
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        shootTimer = Random.Range(0f, shootCooldown);

        // ��������� ����������� ������
        if (projectilePrefab == null)
            Debug.LogError("Projectile Prefab not assigned!", this);
        if (firePoint == null)
            Debug.LogError("Fire Point not assigned!", this);
    }

    private void Update()
    {
        if (playerDead) return;

        // ���� ����� ���������, �������� ����� �����
        //if (player == null)
        //{
        //    player = GameObject.FindGameObjectWithTag("Player")?.transform;
        //    if (player == null)
        //    {
        //        playerDead = true;
        //        return;
        //    }
        //}

        // ��������� ���������
        if (Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                TryShoot();
                shootTimer = shootCooldown;
            }
        }
    }

    private void TryShoot()
    {
        // ��������� ��� ����������� ������
        if (player == null | projectilePrefab == null | firePoint == null)
        {
            Debug.LogWarning("Cannot shoot: missing references. Player: " + player + ", Projectile Prefab: " + projectilePrefab + ", Fire Point: " + firePoint);
            return;
        }

        // ������� ����
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile proj = projectile.GetComponent<Projectile>();

        if (proj == null)
        {
            //Debug.LogError("Projectile component missing!", projectile);
            Destroy(projectile);
            return;
        }

        // ������������ �����������
        Vector2 direction = (player.position - firePoint.position).normalized;
        proj.Launch(direction);
    }
}
//using UnityEngine;

//public class EnemyCrab : MonoBehaviour
//{
//    [Header("Shooting")]
//    [SerializeField] private GameObject projectilePrefab;
//    [SerializeField] private Transform firePoint;
//    [SerializeField] private float shootCooldown = 2f;
//    [SerializeField] private float detectionRange = 10f;

//    private float shootTimer;

//    private Transform player;

//    private void Start()
//    {
//        player = GameObject.FindGameObjectWithTag("Player").transform;
//        shootTimer = 0f;
//    }

//    private void Update()
//    {
//        if (player == null) return;

//        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

//        if (distanceToPlayer <= detectionRange)
//        {
//            shootTimer -= Time.deltaTime;
//            if (shootTimer <= 0f)
//            {
//                ShootAtPlayer();
//                shootTimer = shootCooldown;
//            }
//        }
//    }

//    private void ShootAtPlayer()
//    {
//        Vector2 direction = (player.position - firePoint.position).normalized;
//        Debug.Log($"Shooting at player. Direction: {direction}");
//        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
//        var proj = projectile.GetComponent<Projectile>();
//        if (proj != null)
//        {
//            proj.Launch(direction);
//        }
//        else
//        {
//            Debug.LogError("Projectile script not found on prefab!");
//        }
//    }

//}
