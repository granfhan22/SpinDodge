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
    [SerializeField] private AudioClip bulletHitClip;
    [SerializeField] private AudioClip playerDamageClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip boxImpactClip;
    [SerializeField] private AudioClip ekeWarningClip;
    [SerializeField, Range(0f, 1f)] private float ekeWarningVolume = 0.3f;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip buttonClickClip;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayMusic(bgMusic);
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
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }

    public void SetMusicMute(bool muted)
    {
        if (musicSource != null) musicSource.mute = muted;
    }

    public void SetSFXMute(bool muted)
    {
        if (sfxSource != null) sfxSource.mute = muted;
    }
}
