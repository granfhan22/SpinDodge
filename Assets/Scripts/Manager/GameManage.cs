using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle; // tick = bật tiếng
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle; // tick = bật tiếng

    private float elapsedTime;
    private bool isGameOver;
    private bool isPaused;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (losePanel != null) losePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (homeButton != null) homeButton.onClick.AddListener(Home);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        if (musicToggle != null) musicToggle.onValueChanged.AddListener(OnMusicMuteChanged);
        if (sfxToggle != null) sfxToggle.onValueChanged.AddListener(OnSfxMuteChanged);
    }

    private void Update()
    {
        if (!isGameOver && Keyboard.current != null &&
            (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame))
            TogglePause();

        if (isGameOver || isPaused) return;

        elapsedTime += Time.deltaTime;
        if (timerText != null) timerText.text = FormatTime(elapsedTime);
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void OnMusicVolumeChanged(float value) => AudioManager.Instance?.SetMusicVolume(value);
    private void OnSfxVolumeChanged(float value) => AudioManager.Instance?.SetSFXVolume(value);
    private void OnMusicMuteChanged(bool soundOn) => AudioManager.Instance?.SetMusicMute(!soundOn);
    private void OnSfxMuteChanged(bool soundOn) => AudioManager.Instance?.SetSFXMute(!soundOn);

    public void ShowLoseScreen()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (resultTimeText != null) resultTimeText.text = "You was spin for\n" + FormatTime(elapsedTime);
        if (losePanel != null) losePanel.SetActive(true);
        AudioManager.Instance?.PlayGameOver();
        Time.timeScale = 0f;
    }

    // Hooked to the "Try Again" button's OnClick in the Inspector.
    public void TryAgain()
    {
        AudioManager.Instance?.PlayButtonClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Hooked to the "Home" button's OnClick in the Inspector.
    public void Home()
    {
        AudioManager.Instance?.PlayButtonClick();
        Time.timeScale = 1f;
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
