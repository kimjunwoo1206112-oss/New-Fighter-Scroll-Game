using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Components")]
    [SerializeField] private PlayerHUD playerHUD;
    [SerializeField] private ScoreDisplay scoreDisplay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHUD()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            if (playerHUD != null)
            {
                playerHUD.UpdateHP(player.CurrentLives);
                playerHUD.UpdateBomb(player.CurrentBombs);
                playerHUD.UpdateUpgrade(player.AttackLevel, player.FireRateLevel);
            }
        }

        if (scoreDisplay != null && GameManager.Instance != null)
        {
            scoreDisplay.UpdateScore(GameManager.Instance.Score);
        }
    }
}
