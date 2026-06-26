using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpinnerSelectManager : MonoBehaviour
{
    public static SpinnerSelectManager Instance { get; private set; }

    [SerializeField] private SpinnerSlot[] slots;
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private const float ClickDelay = 0.1f;

    private void Awake() => Instance = this;

    private void Start()
    {
        int selected = SpinnerSelection.SelectedIndex;
        foreach (var slot in slots)
            slot.SetSelected(slot.slotIndex == selected);
    }

    public void SelectSlot(int index)
    {
        foreach (var slot in slots)
            slot.SetSelected(slot.slotIndex == index);
    }

    public void StartGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(LoadSceneDelayed(gameSceneName));
    }

    public void GoBack()
    {
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(LoadSceneDelayed(mainMenuSceneName));
    }

    private IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return new WaitForSecondsRealtime(ClickDelay);
        SceneManager.LoadScene(sceneName);
    }
}
