using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;
    [Header("Room Prefab")]
    public GameObject prefab;
    [HideInInspector] public GameObject previousPrefab; // this is used to regenerate the guid if the so is copied and the prefab is changed
    [Header("Room Music")]
    public MusicTrackSO ambientMusic;
    public MusicTrackSO battleMusic;
    [Header("Room Parameter")]
    public RoomNodeTypeSO roomNodeType;
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;
    [Header("Doorway")]
    [SerializeField] public List<Doorway> doorwayList;
    [Header("Spawn Position")]
    public Vector2Int[] spawnPositionArray;
    [Header("Room Enemy Parameter")]
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Set unique GUID if empty or the prefab changes
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelpUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);

        HelpUtilities.ValidateCheckNullValue(this, nameof(ambientMusic), ambientMusic);

        HelpUtilities.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);

        HelpUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);

        HelpUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        HelpUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);

        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {
            HelpUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelpUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);

            foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomEnemySpawnParametersList)
            {
                HelpUtilities.ValidateCheckNullValue(this, nameof(roomEnemySpawnParameters.dungeonLevel), roomEnemySpawnParameters.dungeonLevel);
                HelpUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minTotalEnemiesToSpawn), roomEnemySpawnParameters.minTotalEnemiesToSpawn, nameof(roomEnemySpawnParameters.maxTotalEnemiesToSpawn), roomEnemySpawnParameters.maxTotalEnemiesToSpawn, true);
                HelpUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minSpawnInterval), roomEnemySpawnParameters.minSpawnInterval, nameof(roomEnemySpawnParameters.maxSpawnInterval), roomEnemySpawnParameters.maxSpawnInterval, true);
                HelpUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minConcurrentEnemies), roomEnemySpawnParameters.minConcurrentEnemies, nameof(roomEnemySpawnParameters.maxConcurrentEnemies), roomEnemySpawnParameters.maxConcurrentEnemies, false);

                bool isEnemyTypesListForDungeonLevel = false;

                foreach (SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectsByLevel in enemiesByLevelList)
                {
                    if (dungeonObjectsByLevel.dungeonLevel == roomEnemySpawnParameters.dungeonLevel && dungeonObjectsByLevel.spawnableObjectRatioList.Count > 0)
                        isEnemyTypesListForDungeonLevel = true;

                    HelpUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectsByLevel.dungeonLevel), dungeonObjectsByLevel.dungeonLevel);

                    foreach (SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectsByLevel.spawnableObjectRatioList)
                    {
                        HelpUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.dungeonObject), dungeonObjectRatio.dungeonObject);

                        HelpUtilities.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.ratio), dungeonObjectRatio.ratio, false);
                    }

                }

                if (isEnemyTypesListForDungeonLevel == false && roomEnemySpawnParameters.dungeonLevel != null)
                {
                    Debug.Log("No enemy types specified in for dungeon level " + roomEnemySpawnParameters.dungeonLevel.levelName + " in gameobject " + this.name.ToString());
                }
            }
        }
    }
#endif
}