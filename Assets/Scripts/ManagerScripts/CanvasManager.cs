using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Button")]
    public Button startButton;
    public Button settingsButton;
    public Button backButton;
    public Button quitButton;

    public Button resumeGame;
    public Button returnToMenu;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject pauseMenuPanel;

    [Header("Text Elements")]
    public TMP_Text livesText;

    private bool paused = false;

    void Awake()
    {
        
        Time.timeScale = 1f;

       
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);

       
        if (!IsMainMenuScene())
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false);
            if (settingsPanel) settingsPanel.SetActive(false);
        }
    }

    void Start()
    {
        
        if (startButton) startButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        if (settingsButton) settingsButton.onClick.AddListener(() => SetMenus(settingsPanel, mainMenuPanel));
        if (backButton) backButton.onClick.AddListener(() => SetMenus(mainMenuPanel, settingsPanel));
        if (quitButton) quitButton.onClick.AddListener(QuitGame);

        if (resumeGame) resumeGame.onClick.AddListener(TogglePause);
        if (returnToMenu) returnToMenu.onClick.AddListener(ReturnToMainMenu);

        if (livesText && GameManager.Instance)
        {
            livesText.text = $"Lives: {GameManager.Instance.lives}";
            GameManager.Instance.OnLivesChanged += (l) => livesText.text = $"Lives: {l}";
        }
    }

    void Update()
    {
        if (pauseMenuPanel && Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        paused = !paused;
        if (pauseMenuPanel) pauseMenuPanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void SetMenus(GameObject menuToActivate, GameObject menuToDeactivate)
    {
        if (menuToActivate) menuToActivate.SetActive(true);
        if (menuToDeactivate) menuToDeactivate.SetActive(false);
    }

    private bool IsMainMenuScene()
    {
        var s = SceneManager.GetActiveScene();
        return s.buildIndex == 0 || s.name == "MainMenu";
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
