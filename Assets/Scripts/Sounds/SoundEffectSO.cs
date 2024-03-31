using UnityEngine;
[CreateAssetMenu(fileName ="SoundEffect_",menuName ="Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    public string soundEffectName;
    public GameObject soundPrefab;
    public AudioClip soundEffectClip;
    [Range(0.1f, 1.5f)] public float soundEffectPitchRandomVariationMin = 0.8f;
    [Range(0.1f, 1.5f)] public float soundEffectPitchRandomVariationMax = 1.2f;
    [Range(0f, 1f)] public float soundEffectVolume = 1f;
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelpUtilities.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelpUtilities.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(soundEffectPitchRandomVariationMin), soundEffectPitchRandomVariationMin, nameof(soundEffectPitchRandomVariationMax), soundEffectPitchRandomVariationMax, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
}
