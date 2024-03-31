using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
    public SpriteRenderer playerHandSpriteRenderer;
    public SpriteRenderer playerHandNoWeaponSpriteRenderer;
    public SpriteRenderer playerWeaponSpriteRenderer;
    public Animator animator;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(playerHandSpriteRenderer), playerHandSpriteRenderer);
        HelpUtilities.ValidateCheckNullValue(this, nameof(playerHandNoWeaponSpriteRenderer), playerHandNoWeaponSpriteRenderer);
        HelpUtilities.ValidateCheckNullValue(this, nameof(playerWeaponSpriteRenderer), playerWeaponSpriteRenderer);
        HelpUtilities.ValidateCheckNullValue(this, nameof(animator), animator);
    }
#endif
}
