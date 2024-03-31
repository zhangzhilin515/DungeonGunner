using Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{

    [SerializeField] private GameObject miniMapPlayer;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        SpriteRenderer spriteRenderer = miniMapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMinimapIcon();
        }
    }

    private void Update()
    {
        if (playerTransform != null && miniMapPlayer != null)
        {
            miniMapPlayer.transform.position = playerTransform.position;
        }
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(miniMapPlayer), miniMapPlayer);
    }

#endif
}