using UnityEngine;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
    }

#endif
}
