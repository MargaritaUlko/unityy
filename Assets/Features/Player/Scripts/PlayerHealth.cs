//using UnityEngine;

//public class PlayerHealth : MonoBehaviour
//{
//    [SerializeField] private HeartUI[] heartsUI;
//    [SerializeField] private int baseMaxHealth = 3;

//    void Start()
//    {
//        // Проверка на существование PlayerHealthManager
//        if (PlayerHealthManager.Instance == null)
//        {
//            Debug.LogError("PlayerHealthManager не инициализирован. Убедитесь, что он существует в сцене.");
//            return; // Прекращаем выполнение, если PlayerHealthManager не существует
//        }

//        // Инициализация здоровья
//        if (PlayerHealthManager.Instance.TotalMaxHealth == 0)
//        {
//            PlayerHealthManager.Instance.TotalMaxHealth = baseMaxHealth;
//            PlayerHealthManager.Instance.CurrentHealth = baseMaxHealth;
//        }
//        else
//        {
//            PlayerHealthManager.Instance.CurrentHealth = Mathf.Min(PlayerHealthManager.Instance.CurrentHealth, PlayerHealthManager.Instance.TotalMaxHealth);
//        }

//        UpdateHeartsUI();
//    }

//    public void TakeDamage(int amount = 1)
//    {
//        PlayerHealthManager.Instance.CurrentHealth = Mathf.Max(PlayerHealthManager.Instance.CurrentHealth - amount, 0);
//        UpdateHeartsUI();

//        if (PlayerHealthManager.Instance.CurrentHealth <= 0)
//        {
//            Debug.Log("Player Died");
//            gameObject.SetActive(false);
//        }
//    }

//    public void AddBonusHearts(int amount)
//    {
//        PlayerHealthManager.Instance.TotalMaxHealth = Mathf.Min(heartsUI.Length, PlayerHealthManager.Instance.TotalMaxHealth + amount);
//        PlayerHealthManager.Instance.CurrentHealth = Mathf.Min(PlayerHealthManager.Instance.CurrentHealth + amount, PlayerHealthManager.Instance.TotalMaxHealth);
//        UpdateHeartsUI();
//    }

//    public void ReceiveHealth(int amount = 1)
//    {
//        PlayerHealthManager.Instance.CurrentHealth = Mathf.Min(PlayerHealthManager.Instance.CurrentHealth + amount, PlayerHealthManager.Instance.TotalMaxHealth);
//        UpdateHeartsUI();
//    }

//    private void UpdateHeartsUI()
//    {
//        for (int i = 0; i < heartsUI.Length; i++)
//        {
//            if (i < PlayerHealthManager.Instance.CurrentHealth)
//                heartsUI[i].SetFull();
//            else if (i < PlayerHealthManager.Instance.TotalMaxHealth)
//                heartsUI[i].SetEmpty();
//            else
//                heartsUI[i].Hide();
//        }
//    }
//}




using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HeartUI[] heartsUI;
    [SerializeField] private int baseMaxHealth = 3;
    private int totalMaxHealth;
    private int currentHealth;

    void Start()
    {
        totalMaxHealth = baseMaxHealth;
        currentHealth = baseMaxHealth;
        UpdateHeartsUI();
    }

    public void TakeDamage(int amount = 1)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died");
            gameObject.SetActive(false);
        }
    }

    public void AddBonusHearts(int amount)
    {
        totalMaxHealth = Mathf.Min(heartsUI.Length, totalMaxHealth + amount);
        currentHealth = Mathf.Min(currentHealth + amount, totalMaxHealth);
        UpdateHeartsUI();
    }

    public void ReceiveHealth(int amount = 1)
    {
        currentHealth = Mathf.Min(currentHealth + amount, totalMaxHealth);
        UpdateHeartsUI();
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartsUI.Length; i++)
        {
            if (i < currentHealth)
                heartsUI[i].SetFull();
            else if (i < totalMaxHealth)
                heartsUI[i].SetEmpty();
            else
                heartsUI[i].Hide();
        }
    }
}
