using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("HP Images")]
    [SerializeField] private Image[] hpImages;

    [Header("Bomb Text")]
    [SerializeField] private Text bombText;

    [Header("Upgrade Texts")]
    [SerializeField] private Text attackLevelText;
    [SerializeField] private Text fireRateLevelText;

    [Header("Sprites")]
    [SerializeField] private Sprite heartFullSprite;
    [SerializeField] private Sprite heartEmptySprite;

    private void Start()
    {
        if (heartFullSprite == null || heartEmptySprite == null)
        {
            Debug.LogWarning("Heart sprites not assigned. Using colored squares.");
        }
    }

    public void UpdateHP(int currentLives)
    {
        if (hpImages == null || hpImages.Length == 0) return;

        for (int i = 0; i < hpImages.Length; i++)
        {
            if (hpImages[i] != null)
            {
                hpImages[i].enabled = true;
                
                if (i < currentLives)
                {
                    if (heartFullSprite != null)
                    {
                        hpImages[i].sprite = heartFullSprite;
                    }
                    hpImages[i].color = Color.white;
                }
                else
                {
                    if (heartEmptySprite != null)
                    {
                        hpImages[i].sprite = heartEmptySprite;
                    }
                    hpImages[i].color = new Color(1, 1, 1, 0.3f);
                }
            }
        }
    }

    public void UpdateBomb(int currentBombs)
    {
        if (bombText != null)
        {
            bombText.text = $"필사기: {currentBombs}";
        }
    }

    public void UpdateUpgrade(int attackLevel, int fireRateLevel)
    {
        if (attackLevelText != null)
        {
            attackLevelText.text = $"공격력: {attackLevel}";
        }

        if (fireRateLevelText != null)
        {
            fireRateLevelText.text = $"연사속도: {fireRateLevel}";
        }
    }
}
