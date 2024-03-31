using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class ActiveRooms : MonoBehaviour
{
    [SerializeField] private Camera miniMapCamera;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }
    private void EnableRooms()
    {
        if (GameManager.Instance.gameState == GameState.dungeonOverviewMap) return;
        HelpUtilities.CameraWorldPositionBounds(out Vector2Int minimapCameraWorldPositionLowerBounds, out Vector2Int minimapCameraWorldPositionUpperBounds, miniMapCamera);
        HelpUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds, out Vector2Int mainCameraWorldPositionUpperBounds, mainCamera);
        foreach (KeyValuePair<string,Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            if(room.lowerBounds.x<=minimapCameraWorldPositionUpperBounds.x&&room.lowerBounds.y<=minimapCameraWorldPositionUpperBounds.y
                &&room.upperBounds.x>=minimapCameraWorldPositionLowerBounds.x&&room.upperBounds.y>=minimapCameraWorldPositionLowerBounds.y)
            {
                room.instantiatedRoom.gameObject.SetActive(true);
                if (room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y
                && room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y)
                {
                    room.instantiatedRoom.ActivateEnvironmentGameObject();
                }
                else
                {
                    room.instantiatedRoom.DeactivateEnvironmentGameObject();
                }
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
    }
#endif
}
