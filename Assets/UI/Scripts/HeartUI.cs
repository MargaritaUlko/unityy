using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private Image heartImage;
    [SerializeField] private Sprite fullSprite;
    [SerializeField] private Sprite emptySprite;

    public void SetFull()
    {
        heartImage.sprite = fullSprite;
        gameObject.SetActive(true);
    }

    public void SetEmpty()
    {
        heartImage.sprite = emptySprite;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

