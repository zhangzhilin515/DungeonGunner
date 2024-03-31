using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sounds/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    public string musicName;

    public AudioClip musicClip;

    [Range(0, 1)]
    public float musicVolume = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckEmptyString(this, nameof(musicName), musicName);
        HelpUtilities.ValidateCheckNullValue(this, nameof(musicClip), musicClip);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
    }
#endif
}
