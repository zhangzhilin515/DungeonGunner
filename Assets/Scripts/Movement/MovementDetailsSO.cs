using UnityEngine;
[CreateAssetMenu(fileName ="MovementDetailsSO_",menuName ="Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    public float minMoveSpeed = 8f;
    public float maxMoveSpeed = 8f;
    public float rollSpeed;
    public float rollDistance;
    public float rollCooldownTime;
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed) return minMoveSpeed;
        else return Random.Range(minMoveSpeed, maxMoveSpeed);
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);
        if(rollSpeed!=0f||rollDistance!=0f||rollCooldownTime!=0f)
        {
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed,false);
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif
}
