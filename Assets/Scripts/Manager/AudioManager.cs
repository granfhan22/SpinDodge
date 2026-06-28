using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip bgMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip shieldSkillClip;
    [SerializeField] private AudioClip bulletDestroyClip;
    [SerializeField] private AudioClip blinkSkillClip;
    [SerializeField] private AudioClip bulletHitClip;
    [SerializeField] private AudioClip playerDamageClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip boxImpactClip;
    [SerializeField] private AudioClip ekeWarningClip;
    [SerializeField, Range(0f, 1f)] private float ekeWarningVolume = 0.3f;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip buttonClickClip;

    private const string KeyMusicVolume = "MusicVolume";
    private const string KeySFXVolume = "SFXVolume";
    private const string KeyMusicMute = "MusicMute";
    private const string KeySFXMute = "SFXMute";

    public float MusicVolume => musicSource != null ? musicSource.volume : 1f;
    public float SFXVolume => sfxSource != null ? sfxSource.volume : 1f;
    public bool IsMusicMuted => musicSource != null && musicSource.mute;
    public bool IsSFXMuted => sfxSource != null && sfxSource.mute;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadAudioSettings();
        PlayMusic(bgMusic);
    }

    private void LoadAudioSettings()
    {
        float musicVol = PlayerPrefs.GetFloat(KeyMusicVolume, 1f);
        float sfxVol = PlayerPrefs.GetFloat(KeySFXVolume, 1f);
        bool musicMute = PlayerPrefs.GetInt(KeyMusicMute, 0) == 1;
        bool sfxMute = PlayerPrefs.GetInt(KeySFXMute, 0) == 1;

        if (musicSource != null) { musicSource.volume = musicVol; musicSource.mute = musicMute; }
        if (sfxSource != null) { sfxSource.volume = sfxVol; sfxSource.mute = sfxMute; }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }
    public void ShieldSkill() => PlaySFX(shieldSkillClip);
    public void BlinkSkill() => PlaySFX(blinkSkillClip);
    public void PlayBulletDestroy() => PlaySFX(bulletDestroyClip);
    public void PlayBulletHit() => PlaySFX(bulletHitClip);
    public void PlayPlayerDamage() => PlaySFX(playerDamageClip);
    public void PlayDash() => PlaySFX(dashClip);
    public void PlayBoxImpact() => PlaySFX(boxImpactClip);
    public void PlayEkeWarning() => PlaySFX(ekeWarningClip, ekeWarningVolume);
    public void PlayGameOver() => PlaySFX(gameOverClip);
    public void PlayButtonClick() => PlaySFX(buttonClickClip);

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null) musicSource.volume = volume;
        PlayerPrefs.SetFloat(KeyMusicVolume, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
        PlayerPrefs.SetFloat(KeySFXVolume, volume);
        PlayerPrefs.Save();
    }

    public void SetMusicMute(bool muted)
    {
        if (musicSource != null) musicSource.mute = muted;
        PlayerPrefs.SetInt(KeyMusicMute, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFXMute(bool muted)
    {
        if (sfxSource != null) sfxSource.mute = muted;
        PlayerPrefs.SetInt(KeySFXMute, muted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
