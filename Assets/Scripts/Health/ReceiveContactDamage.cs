using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveContactDamage : MonoBehaviour
{
    [SerializeField] private int contactDamageAmount;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeContactDamage(int damageAmount = 0)
    {
        if (contactDamageAmount > 0)
            damageAmount = contactDamageAmount;

        health.TakeDamage(damageAmount);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif
}
