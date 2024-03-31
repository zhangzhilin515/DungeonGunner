using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    [Header("Dungeon")]
    public RoomNodeTypeListSO roomNodeTypeList;
    [Header("Player")]
    public CurrentPlayerSO currentPlayer;
    public List<PlayerDetailsSO> playerDetailsList;
    [Header("Player Selection")]
    public GameObject playerSelectionPrefab;
    [Header("Sounds")]
    public AudioMixerGroup soundsMasterMixerGroup;
    public SoundEffectSO doorOpenCloseSoundEffect;
    public SoundEffectSO tableFlip;
    public SoundEffectSO chestOpen;
    public SoundEffectSO healthPickup;
    public SoundEffectSO ammoPickup;
    public SoundEffectSO weaponPickup;
    [Header("Music")]
    public AudioMixerGroup musicMasterMixerGroup;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;
    public MusicTrackSO mainMenuMusic;
    [Header("Shader")]
    public Material dimmedMaterial;
    public Material litMaterial;
    public Shader variableLitShader;
    public Shader materializeShader;
    [Header("Tile")]
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase preferredEnemyPathTile;
    [Header("UI")]
    public GameObject heartPrefab;
    public GameObject ammoIconPrefab;
    public GameObject scorePrefab;
    [Header("Chest")]
    public GameObject chestItemPrefab;
    public Sprite heartIcon;
    public Sprite bulletIcon;
    [Header("Minimap")]
    public GameObject minimapSkullPrefab;

    private static GameResources instance;
    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }
}
