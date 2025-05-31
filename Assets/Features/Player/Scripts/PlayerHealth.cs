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
        Debug.Log($"Taking damage: {amount}");
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
