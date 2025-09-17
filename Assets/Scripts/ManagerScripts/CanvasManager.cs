using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Main / Pause Buttons")]
    public Button startButton;
    public Button quitButton;
    public Button resumeGame;
    public Button returnToMenu;
    public Button creditsButton;

    [Header("Panels")]
    public GameObject mainMenuPanel;   
    public GameObject pauseMenuPanel;  

    [Header("Credits UI")]
    public GameObject creditsPanel;       
    public Button creditsBackButton;     

    [Header("Text Elements")]
    public TMP_Text livesText;

    private bool paused = false;

    void Awake()
    {
        Time.timeScale = 1f;

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false); 
       
    }

    void Start()
    {
        if (startButton) startButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        if (quitButton) quitButton.onClick.AddListener(QuitGame);

        if (creditsButton) creditsButton.onClick.AddListener(OpenCredits);
        if (creditsBackButton) creditsBackButton.onClick.AddListener(CloseCredits);

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
            TogglePause();
    }

    private void OpenCredits()
    {
        SetMenus(creditsPanel, mainMenuPanel);
    }

    private void CloseCredits()
    {
        SetMenus(mainMenuPanel, creditsPanel);
    }

    private void SetMenus(GameObject show, GameObject hide)
    {
        if (show) show.SetActive(true);
        if (hide) hide.SetActive(false);
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