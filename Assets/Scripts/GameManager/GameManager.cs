using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[DisallowMultipleComponent]

public class GameManager : SingleTon<GameManager>
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [Header("Dungeon Levels")]
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentDungeonLevel=0;
    private Room currentRoom;
    private Room previousRoom;
    private InstantiatedRoom bossRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;
    public GameState gameState;
    public GameState previousGameState;
    private long gameScore;
    private int scoreMultiplier;
    private bool isFading = false;
    protected override void Awake()
    {
        base.Awake();
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        InstantiatePlayer();
    }
    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
        gameScore = 0;
        scoreMultiplier = 1;
    }
    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointScored;
        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;
        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }
    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointScored;
        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }
    private void Update()
    {
        HandleGameState();
    }
    private void StaticEventHandler_OnRoomChanged(Room room)
    {
        SetCurrentRoom(room);
    }
    private void StaticEventHandler_OnRoomEnemiesDefeated(Room room)
    {
        RoomEnemiesDefeated();
    }
    private void StaticEventHandler_OnPointScored(int points)
    {
        gameScore += points*scoreMultiplier;
        StaticEventHandler.CallScoreChangedEvent(gameScore,scoreMultiplier);
    }
    private void StaticEventHandler_OnMultiplier(bool multiplier)
    {
        if (multiplier) scoreMultiplier++;
        else scoreMultiplier--;
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }
    private void Player_OnDestroyed(DestroyedEvent destroyedEvent,DestroyedEventArgs destroyedEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }
    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.characterPrefab);
        player = playerGameObject.GetComponent<Player>();
        player.Initialize(playerDetails);
    }
    private void HandleGameState()
    {
        switch(gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevel);
                gameState = GameState.playingLevel;
                RoomEnemiesDefeated();
                break;
            case GameState.playingLevel:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }
                break;
            case GameState.engagingEnemies:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
            case GameState.dungeonOverviewMap:
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    DungeonMap.Instance.ClearDungeonOverViewMap();
                }
                break;
            case GameState.bossStage:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    DisplayDungeonOverviewMap();
                }
                break;
            case GameState.engagingBoss:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
            case GameState.levelCompleted:
                StartCoroutine(LevelCompleted());
                break;
            case GameState.gameWon:
                if (previousGameState != GameState.gameWon)
                    StartCoroutine(GameWon());
                break;
            case GameState.gameLost:
                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines(); // Prevent messages if you clear the level just as you get killed
                    StartCoroutine(GameLost());
                }
                break;
            case GameState.restartGame:
                RestartGame();
                break;
            case GameState.gamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
        }
    }
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }
    private void RoomEnemiesDefeated()
    {
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }
            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }
        // If dungeon level completly cleared (dungeon cleared apart from boss and there is no boss room OR dungeon cleared apart from boss and boss room is also cleared)
        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            if (currentDungeonLevel < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }
    }
    private IEnumerator BossStage()
    {
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE  " + GameResources.Instance.currentPlayer.playerName 
            + "!  YOU'VE SURVIVED ....SO FAR\n\nNOW FIND AND DEFEAT THE BOSS....GOOD LUCK!", Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

    }
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        isFading = true;

        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }

        isFading = false;

    }
    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        // Display the message for the given time
        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        // else display the message until the return button is pressed
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        messageTextTMP.SetText("");
    }
    private void DisplayDungeonOverviewMap()
    {
        if (isFading) return;
        DungeonMap.Instance.DisplayDungeonOverViewMap();
    }
    public void PauseGameMenu()
    {
        if (gameState != GameState.gamePaused)
        {
            pauseMenu.SetActive(true);
            GetPlayer().playerControl.DisablePlayer();

            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused)
        {
            pauseMenu.SetActive(false);
            GetPlayer().playerControl.EnablePlayer();

            gameState = previousGameState;
            previousGameState = GameState.gamePaused;

        }
    }
    private void PlayDungeonLevel(int dungeonLevelIndex)
    {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[currentDungeonLevel]);
        if(!dungeonBuiltSuccessfully)
        {
            Debug.LogError("Dungeon Built Error");
        }
        StaticEventHandler.CallRoomChangedEvent(currentRoom);
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0);
        player.gameObject.transform.position=HelpUtilities.GetSpawnPositionNearestToPlayer(player.transform.position);
        StartCoroutine(DisplayDungeonLevelText());
    }
    private IEnumerator DisplayDungeonLevelText()
    {
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevel + 1).ToString() + "\n\n" + dungeonLevelList[currentDungeonLevel].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }
    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! \n\nYOU'VE SURVIVED THIS DUNGEON LEVEL", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("COLLECT ANY LOOT ....THEN PRESS RETURN\n\nTO DESCEND FURTHER INTO THE DUNGEON", Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        // When player presses the return key proceed to the next level
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; // to avoid enter being detected twice

        currentDungeonLevel++;

        PlayDungeonLevel(currentDungeonLevel);
    }
    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);

        string rankText;
        
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.characterName.ToUpper();
            }

            HighScoreManager.Instance.AddScore(new Score() { playerName = name, levelDescription = "LEVEL " + (currentDungeonLevel + 1).ToString() + " - " + GetCurrentDungeonLevel().levelName.ToUpper(), score = gameScore }, rank);
        }
        else
        {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }
        
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE DEFEATED THE DUNGEON", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        gameState = GameState.restartGame;
    }
    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        GetPlayer().playerControl.DisablePlayer();

        int rank = HighScoreManager.Instance.GetRank(gameScore);
        string rankText;

        
        // Test if the score is in the rankings
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");

            string name = GameResources.Instance.currentPlayer.playerName;

            if (name == "")
            {
                name = playerDetails.characterName.ToUpper();
            }

            // Update scores
            HighScoreManager.Instance.AddScore(new Score() { playerName = name, levelDescription = "LEVEL " + (currentDungeonLevel + 1).ToString() + " - " + GetCurrentDungeonLevel().levelName.ToUpper(), score = gameScore }, rank);
        }
        else
        {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }
        
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        // Disable enemies (FindObjectsOfType is resource hungry - but ok to use in this end of game situation)
        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        yield return StartCoroutine(DisplayMessageRoutine("BAD LUCK " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE SUCCUMBED TO THE DUNGEON", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public Player GetPlayer()
    {
        return player;
    }
    public Sprite GetPlayerMinimapIcon()
    {
        return playerDetails.playerMinimapIcon;
    }
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevel];
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
        HelpUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelpUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelpUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}
