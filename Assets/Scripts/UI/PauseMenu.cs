using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResume);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuit);
        }
    }

    private void OnDisable()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(OnResume);
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuit);
        }
    }

    public void OnResume()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
        gameObject.SetActive(false);
    }

    public void OnQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
