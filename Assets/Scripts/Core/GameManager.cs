using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string UserId { get; private set; }
    public string SessionId { get; private set; }
    public string Difficulty { get; private set; }
    public string GameId { get; private set; }

    public int Score { get; set; }
    
    // Para guardar cada intento
    public int[] Scores { get; private set; } = new int[3]; 
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public float TimeTaken { get; set; }
    public int RetryCount { get; set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeFromUrlParams(); // Ahora se ejecuta después del Awake de otros
    }
    
    // Load Scenes
    
    public void LoadGame()
    {
        // Eliminar instancia antigua de LevelLoader si existe
        var existingLoader = FindFirstObjectByType<LevelLoader>();
        if (existingLoader != null)
        {
            Destroy(existingLoader.gameObject);
        }

        SceneManager.LoadScene("Game");
    }
    
    public void LoadDifficultySelection() {
        SceneManager.LoadScene("DifficultySelection");
    }

    public void LoadGameOver() {
        SceneManager.LoadScene("GameOver");
    }

    public void LoadInstructionsScene()
    {
        SceneManager.LoadScene("Instructions");
    }
    
    public void LoadSelector()
    {
        SceneManager.LoadScene("Selector");
    }
    
    public void SaveCurrentScore()
    {
        if (RetryCount >= 0 && RetryCount < 3)
        {
            Scores[RetryCount] = Score;
        }
    }

    private void InitializeFromUrlParams()
    {
        if (URLParamsHandler.Instance == null) {
            Debug.LogError("URLParamsHandler no está presente en la escena inicial.");
            return;
        }

        UserId = URLParamsHandler.Instance.UserId;
        SessionId = URLParamsHandler.Instance.SessionId;
        Difficulty = URLParamsHandler.Instance.Difficulty;
        GameId = URLParamsHandler.Instance.GameId;

        Debug.Log($"[GameManager] Configurado con -> UserId: {UserId}, SessionId: {SessionId}, Difficulty: {Difficulty}");
        DecideNextScene();
    }

    private void DecideNextScene(){
        if (!string.IsNullOrEmpty(Difficulty)) {
            // Si la dificultad ya viene por la URL, vamos directo al juego
            LoadGame();
        } else {
            // Si no, mostramos la selección de dificultad
            LoadDifficultySelection();
        }
    }

    
    public int GetBestScore()
    {
        return Mathf.Max(Scores);
    }
    
    public void SetDifficulty(string difficulty) {
        Difficulty = difficulty.ToLower();
    }
}
