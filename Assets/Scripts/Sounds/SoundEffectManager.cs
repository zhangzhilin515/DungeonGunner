using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class SoundEffectManager : SingleTon<SoundEffectManager>
{
    public int soundsVolume = 8;
    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }
        SetSoundsVolume(soundsVolume);
    }
    private void OnDisable()
    {
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }
    private void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;
        if(soundsVolume==0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelpUtilities.LinearToDecibels(soundsVolume));
        }
    }
    public void IncreaseSoundsVolume()
    {
        int maxSoundsVolume = 20;
        if (soundsVolume >= maxSoundsVolume) return;
        soundsVolume++;
        SetSoundsVolume(soundsVolume);
    }
    public void DecreaseSoundsVolume()
    {
        if (soundsVolume == 0) return;
        soundsVolume--;
        SetSoundsVolume(soundsVolume);
    }
    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }
    private IEnumerator DisableSound(SoundEffect sound,float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }
}