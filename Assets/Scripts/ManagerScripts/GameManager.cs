using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;    

    public Action<int> OnLivesChanged;

    private int _lives = 3;

    public int lives
    {
        get => _lives;
        set
        {
            if (value < 0)
            {
                Debug.Log("Game Over! You died - AGAIN! And you have no lives left!");
                _lives = 0;
            }

            else if (value > maxLives)
            {
                _lives = maxLives;
            }

            else
            {
                _lives = value;
            }

            Debug.Log($"Lives Left: {_lives}");
            OnLivesChanged?.Invoke( _lives );
        }
    }

    public int maxLives = 9;

    [SerializeField] private string gameOverSceneName = "CIVGameOverMenu";

    private bool _loadingGameOver = false;

    public void LoadGameOverScene()
    {
        if (_loadingGameOver) return;
        _loadingGameOver = true;
        SceneManager.LoadScene(gameOverSceneName);
    }

    public bool TryAddLife(int amount = 1)
    {
        int before = lives;
        lives = before + amount;   
        return lives > before;     
    }

    #region Singleton Pattern
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    #endregion 

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                SceneManager.LoadScene(1);
            }

            else
            {
                SceneManager.LoadScene(0);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            lives++;
        }
    }

    public void StartGame() => SceneManager.LoadScene(1);

}


