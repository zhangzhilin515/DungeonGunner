using UnityEngine;
[CreateAssetMenu(fileName ="AmmoDetails_",menuName ="Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    [Header("Basic Ammo Details")]
    public string ammoName;
    public bool isPlayerAmmo;
    [Header("Ammo Shoot Details")]
    public Sprite ammoSprite;
    public GameObject[] ammoPrefabArray;
    public Material ammoMaterial;
    public float ammoChargeTime = 0.1f;
    public Material ammoChargeMaterial;
    [Header("Ammo Hit Effect")]
    public AmmoHitEffectSO ammoHitEffect;
    [Header("Ammo Basic Details")]
    public int ammoDamage = 1;
    public float ammoSpeedMin = 20f;
    public float ammoSpeedMax = 20f;
    public float ammoRange = 20f;
    public float ammoRotationSpeed = 1f;
    [Header("Ammo Spread Details")]
    public float ammoSpreadMin = 0f;
    public float ammoSpreadMax = 0f;
    [Header("Ammo Spawn Details")]
    public int ammoSpawnAmountMin = 1;
    public int ammoSpawnAmountMax = 1;
    public float ammoSpawnIntervalMin = 0f;
    public float ammoSpawnIntervalMax = 0f;
    [Header("Ammo Trail Details")]
    public bool isAmmoTrail=false;
    public float ammoTrailTime = 3f;
    public Material ammoTrailMaterial;
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    [Range(0f, 1f)] public float ammoTrailEndWidth;
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelpUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelpUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelpUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0)
            HelpUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoChargeMaterial);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        if (isAmmoTrail)
        {
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelpUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }
    }

#endif
}
