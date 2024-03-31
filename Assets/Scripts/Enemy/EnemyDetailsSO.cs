using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    [Header("Base Enemy Setting")]
    public string enemyName;

    public GameObject enemyPrefab;

    public float chaseDistance = 50f;

    [Header("Enemy Material Setting")]
    public Material enemyStandardMaterial;

    public float enemyMaterializeTime;

    public Shader enemyMaterializeShader;
    [ColorUsage(true, true)]
    public Color enemyMaterializeColor;

    [Header("Enemy Weapon Setting")]
    public WeaponDetailsSO enemyWeapon;

    public float firingIntervalMin = 0.1f;

    public float firingIntervalMax = 1f;

    public float firingDurationMin = 1f;

    public float firingDurationMax = 2f;

    public bool firingLineOfSightRequired;

    [Header("Enemy Healyh Setting")]
    public EnemyHealthDetails[] enemyHealthDetailsArray;

    public bool isImmuneAfterHit = false;

    public float hitImmunityTime;

    public bool isHealthBarDisplayed = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelpUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelpUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        HelpUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelpUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        HelpUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelpUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }

#endif
}