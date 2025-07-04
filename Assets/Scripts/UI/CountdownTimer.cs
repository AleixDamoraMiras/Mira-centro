using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [Header("Configuración (fallback)")]
    public float fallbackTimeInMinutes = 2f;

    [Header("UI")]
    public Text timeText;
    public Button pauseButton;
    public Text pauseButtonText;

    private float timeRemaining;
    private bool isRunning = true;

    private void Start()
    {
        if (LevelLoader.Instance != null && LevelLoader.Instance.CurrentLevel != null) {
            timeRemaining = LevelLoader.Instance.CurrentLevel.duration;
        } else {
            timeRemaining = fallbackTimeInMinutes * 60f;
        }

        if (pauseButton != null) pauseButton.onClick.AddListener(TogglePause);
        UpdateTimeUI();
    }

    private void Update()
    {
        if (isRunning)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                isRunning = false;
                OnTimerEnd();
            }
            UpdateTimeUI();
        }
    }

    private void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60F);
            int seconds = Mathf.FloorToInt(timeRemaining % 60F);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
        
        if (pauseButtonText != null)
        {
            pauseButtonText.text = isRunning ? "||" : "▶";
        }
    }

    private void TogglePause()
    {
        isRunning = !isRunning;
        UpdateTimeUI();
    }

    // Aquí puedes manejar lo que sucede cuando el temporizador llega a cero
    private void OnTimerEnd()
    {
        Debug.Log("El temporizador ha terminado.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameOver();
        }
    }

    public void SetPaused(bool pause)
    {
        isRunning = !pause;
    }

}
