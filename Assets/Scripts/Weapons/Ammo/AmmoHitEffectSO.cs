using UnityEngine;
[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapons/Ammo Hit Effect")]

public class AmmoHitEffectSO : ScriptableObject
{
    public Gradient colorGradient;

    public float duration = 0.50f;

    public float startParticleSize = 0.25f;

    public float startParticleSpeed = 3f;

    public float startLifetime = 0.5f;

    public int maxParticleNumber = 100;

    public int emissionRate = 100;

    public int burstParticleNumber = 20;

    public float effectGravity = -0.01f;

    public Sprite sprite;

    public Vector3 velocityOverLifetimeMin;

    public Vector3 velocityOverLifetimeMax;

    public GameObject ammoHitEffectPrefab;

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelpUtilities.ValidateCheckNullValue(this, nameof(ammoHitEffectPrefab), ammoHitEffectPrefab);
    }

#endif
}
