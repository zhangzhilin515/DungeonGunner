using UnityEngine;
using TMPro;

public class ScorePrefab : MonoBehaviour
{
    public TextMeshProUGUI rankTMP;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI scoreTMP;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(rankTMP), rankTMP);
        HelpUtilities.ValidateCheckNullValue(this, nameof(nameTMP), nameTMP);
        HelpUtilities.ValidateCheckNullValue(this, nameof(levelTMP), levelTMP);
        HelpUtilities.ValidateCheckNullValue(this, nameof(scoreTMP), scoreTMP);
    }
#endif
}
