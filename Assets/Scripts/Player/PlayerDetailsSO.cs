using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="PlayerDetails_",menuName ="Scriptable Objects/Player/Player Details")]

public class PlayerDetailsSO : ScriptableObject
{
    [Header("Basic Player Parameter")]
    public string characterName;
    public GameObject characterPrefab;
    public RuntimeAnimatorController runtimeAnimatorController;
    public Sprite playerMinimapIcon;
    public Sprite playerHandSprite;
    [Header("Player Health Parameter")]
    public int playerHealthAmount;
    public bool isImmuneAfterHit = false;
    public float hitImmunityTime;
    [Header("Player Weapon Parameter")]
    public WeaponDetailsSO startingWeapon;
    public List<WeaponDetailsSO> startingWeaponList;
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckEmptyString(this, nameof(characterName), characterName);
        HelpUtilities.ValidateCheckNullValue(this, nameof(characterPrefab), characterPrefab);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount,false);
        HelpUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelpUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);
        HelpUtilities.ValidateCheckNullValue(this, nameof(playerMinimapIcon), playerMinimapIcon);
        HelpUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        if(isImmuneAfterHit)
        {
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false) ;
        }
    }
#endif
}
