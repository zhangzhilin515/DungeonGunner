using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]

public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1; // default value.  This is set by the enemy spawner.
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }
    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;
        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }
        if (!chasePlayer)
            return;

        // Only process A Star path rebuild on certain frames to spread the load between enemies
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        // if the movement cooldown timer reached or player has moved more than required distance
        // then rebuild the enemy path and move the enemy
        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath))
        {
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            CreatePath();

            // If a path has been found move the enemy
            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));

            }
        }
    }
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, (nextPosition - transform.position).normalized,moveSpeed);
                yield return waitForFixedUpdate;
            }
            yield return waitForFixedUpdate;
        }
        enemy.idleEvent.CallIdleEvent();

    }

    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        movementSteps = Astar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        // Take off first step on path - this is the grid square the enemy is already on
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            enemy.idleEvent.CallIdleEvent();
        }
    }
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPositon = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPositon.x, adjustedPlayerCellPositon.y]);

        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        // find a surounding cell that isn't an obstacle - required because with the 'half collision' tiles
        // and tables the player can be on a grid square that is marked as an obstacle
        else
        {
            surroundingPositionList.Clear();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) continue;

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }
            for (int l = 0; l < 8; l++)
            {
                // Generate a random index for the list
                int index = Random.Range(0, surroundingPositionList.Count);

                // See if there is an obstacle in the selected surrounding position
                try
                {
                    obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y], 
                        currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPositon.x + surroundingPositionList[index].x, adjustedPlayerCellPositon.y + surroundingPositionList[index].y]);
                    // If no obstacle return the cell position to navigate to
                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                    }

                }
                catch
                {

                }
                surroundingPositionList.RemoveAt(index);
            }
            // If no non-obstacle cells found surrounding the player - send the enemy in the direction of an enemy spawn position
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

        }
    }
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif
}