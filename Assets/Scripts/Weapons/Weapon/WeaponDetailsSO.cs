using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;
    public Vector3 weaponShootPosition;
    public AmmoDetailsSO weaponCurrentAmmo;
    public WeaponShootEffectSO weaponShootEffect;
    public SoundEffectSO weaponFiringSoundEffect;
    public SoundEffectSO weaponReloadingSoundEffect;
    public bool hasInfiniteAmmo = false;
    public bool hasInfiniteClipCapacity = false;
    public int weaponClipAmmoCapacity = 6;
    public int weaponAmmoCapacity = 100;
    public float weaponFireRate = 0.2f;
    public float weaponPrechargeTime = 0f;
    public float weaponReloadTime = 0f;
    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelpUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelpUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }

#endif
    #endregion Validation
}
