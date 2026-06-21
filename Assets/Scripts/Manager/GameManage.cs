using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManage : MonoBehaviour
{
    public static GameManage Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private TMP_Text timerText;

    [Header("Lose Screen")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TMP_Text resultTimeText;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private float elapsedTime;
    private bool isGameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (losePanel != null) losePanel.SetActive(false);
    }

    private void Update()
    {
        if (isGameOver) return;

        elapsedTime += Time.deltaTime;
        if (timerText != null) timerText.text = FormatTime(elapsedTime);
    }

    public void ShowLoseScreen()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (resultTimeText != null) resultTimeText.text = "You was spin for\n" + FormatTime(elapsedTime);
        if (losePanel != null) losePanel.SetActive(true);
    }

    // Hooked to the "Try Again" button's OnClick in the Inspector.
    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Hooked to the "Home" button's OnClick in the Inspector.
    public void Home()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private static string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;
        return $"{minutes:00}:{secs:00}";
    }
}
