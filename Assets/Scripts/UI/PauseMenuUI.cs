using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI musicLevelText;
    [SerializeField] private TextMeshProUGUI soundLevelText;
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private IEnumerator InitializeUI()
    {
        //�ȴ�1֡ʱ�����ֺ���Ч�������
        yield return null;

        soundLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    private void OnEnable()
    {
        Time.timeScale = 0f;
        StartCoroutine(InitializeUI());
    }
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    public void IncreaseSoundVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundsVolume();
        soundLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }
    public void DecreaseSoundVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundsVolume();
        soundLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }
}
