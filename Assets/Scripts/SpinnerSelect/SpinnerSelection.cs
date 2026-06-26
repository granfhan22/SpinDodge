using UnityEngine;

public static class SpinnerSelection
{
    private const string Key = "SelectedSpinnerIndex";

    public static int SelectedIndex
    {
        get => PlayerPrefs.GetInt(Key, 0);
        set { PlayerPrefs.SetInt(Key, value); PlayerPrefs.Save(); }
    }
}
