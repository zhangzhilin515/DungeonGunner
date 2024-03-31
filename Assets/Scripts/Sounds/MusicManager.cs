using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingleTon<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        musicAudioSource = GetComponent<AudioSource>();

        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }

        SetMusicVolume(musicVolume);
    }
    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }
    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
    {
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }
    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }
    public void IncreaseMusicVolume()
    {
        int maxMusicVolume = 20;

        if (musicVolume >= maxMusicVolume) return;

        musicVolume ++;

        SetMusicVolume(musicVolume);
    }
    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;

        musicVolume --;

        SetMusicVolume(musicVolume);
    }
    public void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelpUtilities.LinearToDecibels(musicVolume));
        }
    }
}