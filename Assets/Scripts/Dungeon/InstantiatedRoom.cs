using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]

public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty;// use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
    [HideInInspector] public int[,] aStarItemObstacles; // use to store position of moveable items that are obstacles
    [HideInInspector] public Bounds roomColliderBounds;
    [HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>();

    [SerializeField] private GameObject environmentGameObject;

    private BoxCollider2D boxCollider2D;
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider2D.bounds;
    }
    private void Start()
    {
        UpdateMoveableObstacles();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag==Settings.playerTag&&room!=GameManager.Instance.GetCurrentRoom())
        {
            room.isPreviousVisited = true;
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }
    public void Initialise(GameObject roomGameObject)
    {
        PopulateTilemapMemberVaribles(roomGameObject);
        BlockOffUnusedDoorways();
        AddObstaclesAndPreferredPath();
        CreateItemObstaclesArray();
        AddDoorsToRoom();
        DisableCollisionTilemapRenderer();
    }
    private void PopulateTilemapMemberVaribles(GameObject roomGameObject)
    {
        grid = GetComponentInChildren<Grid>();
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if(tilemap.gameObject.tag=="groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }
        }
    }
    private void BlockOffUnusedDoorways()
    {
        foreach (Doorway doorway in room.doorWayList)
        {
            if (doorway.isConnected)
                continue;
            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }
            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }
            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            case Orientation.none:
                break;
        }
    }
    private void BlockDoorwayHorizontally(Tilemap tilemap,Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        for(int xPos=0;xPos<doorway.doorwayCopyTileWidth;xPos++)
        {
            for(int yPos=0;yPos<doorway.doorwayCopyTileHeight;yPos++)
            {
                //Get the Rotation of The Tilemap Being Copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }

        }
    }
    private void AddObstaclesAndPreferredPath()
    {
        aStarMovementPenalty = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y+1];
        for(int x=0;x< room.templateUpperBounds.x - room.templateLowerBounds.x + 1;x++)
        {
            for(int y=0;y< room.templateUpperBounds.y - room.templateLowerBounds.y + 1;y++)
            {
                aStarMovementPenalty[x, y] = Settings.defaultAstarMovementPenalty;
                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));
                foreach(TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if(tile==collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }
                if (tile == GameResources.Instance.preferredEnemyPathTile) aStarMovementPenalty[x, y] = Settings.preferredPathAstarMovementPenalty;
            }
        }
    }
    private void AddDoorsToRoom()
    {
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS) return;
        foreach (Doorway doorway in room.doorWayList)
        {
            float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;
            GameObject door = null;
            if (doorway.doorPrefab!=null&&doorway.isConnected)
            {
                if(doorway.orientation==Orientation.north)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y , 0);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y+tileDistance*1.25f, 0);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x , doorway.position.y + tileDistance * 1.25f, 0);
                }
                Door doorComponent = door.GetComponent<Door>();
                if(room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;
                    doorComponent.LockDoor();

                    GameObject skullIcon = Instantiate(GameResources.Instance.minimapSkullPrefab,gameObject.transform);
                    skullIcon.transform.localPosition = door.transform.localPosition;
                }
            }
        }

    }
    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
    public void ActivateEnvironmentGameObject()
    {
        if (environmentGameObject != null) environmentGameObject.SetActive(true);
    }
    public void DeactivateEnvironmentGameObject()
    {
        if (environmentGameObject != null) environmentGameObject.SetActive(false);
    }
    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }
    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }
    public void LockDoors()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();
        foreach(Door door in doorArray)
        {
            door.LockDoor();
        }
        DisableRoomCollider();
    }
    public  void UnlockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }
    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay>0f) yield return new WaitForSeconds(doorUnlockDelay);
        Door[] doorArray = GetComponentsInChildren<Door>();
        foreach(Door door in doorArray)
        {
            door.UnlockDoor();
        }
        EnableRoomCollider();
    }
    private void CreateItemObstaclesArray()
    {
        aStarItemObstacles = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];
    }
    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.templateUpperBounds.x - room.templateLowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                aStarItemObstacles[x, y] = Settings.defaultAstarMovementPenalty;
            }
        }
    }
    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            Vector3Int colliderBoundsMin = grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            Vector3Int colliderBoundsMax = grid.WorldToCell(moveItem.boxCollider2D.bounds.max);

            for (int i = colliderBoundsMin.x; i <= colliderBoundsMax.x; i++)
            {
                for (int j = colliderBoundsMin.y; j <= colliderBoundsMax.y; j++)
                {
                    aStarItemObstacles[i - room.templateLowerBounds.x, j - room.templateLowerBounds.y] = 0;
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
    }
#endif
}
