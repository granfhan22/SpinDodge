using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private string selectSpinnerSceneName = "PickYourSpiner";

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle;

    [Header("Spinner Display")]
    [SerializeField] private Image spinnerDisplay;
    [SerializeField] private SpinnerData[] spinnerDataList;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float spinSpeed = 180f;

    private const float ClickDelay = 0.1f;
    private RectTransform targetButton;

    private void Awake() => Instance = this;

    private void Start()
    {
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        if (musicToggle != null) musicToggle.onValueChanged.AddListener(OnMusicMuteChanged);
        if (sfxToggle != null) sfxToggle.onValueChanged.AddListener(OnSfxMuteChanged);

        UpdateSpinnerDisplay();
    }

    private void Update()
    {
        if (spinnerDisplay == null) return;

        // Xoay spinner
        spinnerDisplay.rectTransform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

        // Di chuyển theo Y của button đang hover
        if (targetButton != null)
        {
            Vector3 target = spinnerDisplay.transform.position;
            target.y = targetButton.position.y;
            spinnerDisplay.transform.position = Vector3.Lerp(
                spinnerDisplay.transform.position, target, Time.deltaTime * moveSpeed);
        }
    }

    public void OnButtonHover(RectTransform button) => targetButton = button;

    private void UpdateSpinnerDisplay()
    {
        if (spinnerDisplay == null || spinnerDataList == null || spinnerDataList.Length == 0) return;
        int index = Mathf.Clamp(SpinnerSelection.SelectedIndex, 0, spinnerDataList.Length - 1);
        SpinnerData data = spinnerDataList[index];
        if (data?.icon != null) spinnerDisplay.sprite = data.icon;
    }

    public void PlayDirect()
    {
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(LoadSceneDelayed(gameSceneName));
    }

    public void GoToSelectSpinner()
    {
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(LoadSceneDelayed(selectSpinnerSceneName));
    }

    public void OpenSettings()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPanel != null) settingsPanel.SetActive(true);
        SyncAudioUI();
    }

    public void CloseSettings()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(QuitDelayed());
    }

    private void SyncAudioUI()
    {
        if (AudioManager.Instance == null) return;
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);
        if (musicToggle != null) musicToggle.SetIsOnWithoutNotify(!AudioManager.Instance.IsMusicMuted);
        if (sfxToggle != null) sfxToggle.SetIsOnWithoutNotify(!AudioManager.Instance.IsSFXMuted);
    }

    private void OnMusicVolumeChanged(float value) => AudioManager.Instance?.SetMusicVolume(value);
    private void OnSfxVolumeChanged(float value) => AudioManager.Instance?.SetSFXVolume(value);
    private void OnMusicMuteChanged(bool soundOn) => AudioManager.Instance?.SetMusicMute(!soundOn);
    private void OnSfxMuteChanged(bool soundOn) => AudioManager.Instance?.SetSFXMute(!soundOn);

    private IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return new WaitForSecondsRealtime(ClickDelay);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator QuitDelayed()
    {
        yield return new WaitForSecondsRealtime(ClickDelay);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
