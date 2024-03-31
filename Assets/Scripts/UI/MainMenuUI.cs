using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject instructionButton;
    [SerializeField] private GameObject highScoreButton;
    [SerializeField] private GameObject returnToMainMenuButton;
    private bool isInstructionSceneLoaded = false;
    private bool isHighScoreSceneLoaded = false;
    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
        returnToMainMenuButton.SetActive(false);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }
    public void LoadHighScores()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoreButton.SetActive(false);
        instructionButton.SetActive(false);
        isHighScoreSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }
    public void LoadCharacterSelector()
    {
        returnToMainMenuButton.SetActive(false);
        if(isHighScoreSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoreSceneLoaded = false;
        }
        else if(isInstructionSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionScene");
            isInstructionSceneLoaded = false;
        }

        playButton.SetActive(true);
        quitButton.SetActive(true);
        highScoreButton.SetActive(true);
        instructionButton.SetActive(true);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }
    public void LoadInstruction()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoreButton.SetActive(false);
        instructionButton.SetActive(false);
        isInstructionSceneLoaded=true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("InstructionScene", LoadSceneMode.Additive);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
