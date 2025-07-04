using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class GameOverController : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void NotifyGameEnded(string userId);
    #endif

    [Header("UI References")]
    public Text userIdText;
    public Text sessionIdText;
    public Text difficultyText;
    public Text gameIdText;
    public Text scoreText;
    public Text correctAnswersText;
    public Text wrongAnswersText;
    public Text timeTakenText;
    
    [Header("Buttons")]
    public Button retryButton;
    public Button exitButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gameOverClip;
    
    private void Start()
    {
        if (audioSource != null && gameOverClip != null)
        {
            audioSource.PlayOneShot(gameOverClip);
        }

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager no disponible.");
            return;
        }

        // Leer datos desde GameManager
        string userId = gm.UserId;
        string sessionId = gm.SessionId;
        string difficulty = gm.Difficulty;
        string gameId = gm.GameId;
        int score = gm.Score;
        int correct = gm.CorrectAnswers;
        int wrong = gm.WrongAnswers;
        float time = gm.TimeTaken;

        // Mostrar en UI
        SetUIText(userIdText, "Usuario", userId);
        SetUIText(sessionIdText, "Sesión", sessionId);
        SetUIText(difficultyText, "Dificultad", difficulty);
        SetUIText(gameIdText, "ID del Juego", gameId);

        SetUIText(scoreText, "Puntuación", score.ToString());
        SetUIText(correctAnswersText, "Aciertos", correct.ToString());
        SetUIText(wrongAnswersText, "Fallos", wrong.ToString());
        SetUIText(timeTakenText, "Tiempo", FormatTime(time));

        // Control de botón de Retry con delay
        if (gm.RetryCount < 3)
        {
            retryButton.gameObject.SetActive(true);
            retryButton.onClick.AddListener(() => StartCoroutine(DelayedRetryGame()));
        }
        else
        {
            retryButton.gameObject.SetActive(false);
        }

        // Botón Exit (envía datos) con delay
        // if (exitButton != null)
        // {
        //     exitButton.onClick.AddListener(() =>
        //     {
        //         StartCoroutine(DelayedSendScore(userId, sessionId, gameId, difficulty, score));
        //     });
        // }
    }
    
    private IEnumerator DelayedRetryGame()
    {
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.RetryCount++;
        GameManager.Instance.LoadGame();
    }

    private IEnumerator DelayedSendScore(string userId, string sessionId, string gameId, string difficulty, int score)
    {
        yield return new WaitForSeconds(0.2f);
        SendScoreAndExit(userId, sessionId, gameId, difficulty, score);
    }
    
    private void SetUIText(Text textComponent, string label, string value)
    {
        if (textComponent != null)
        {
            textComponent.text = $"{value}";
        }
    }

    // Formatea el tiempo en formato MM:SS
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    // Acción al hacer clic en "Retry"
    private void RetryGame()
    {
        GameManager.Instance.RetryCount++;
        GameManager.Instance.LoadGame();
    }
    
    private IEnumerator SendScoreToServer(string userId, string sessionId, string gameId, string difficulty, int score)
    {
        const string url = "https://memorix.es/wp-json/memorix/v1/submit-score";
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        WWWForm form = new WWWForm();
        form.AddField(nameof(userId), userId);
        form.AddField(nameof(sessionId), sessionId);
        form.AddField(nameof(gameId), gameId);
        form.AddField(nameof(difficulty), difficulty);
        form.AddField(nameof(score), score);
        form.AddField(nameof(timestamp), timestamp);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score enviado correctamente.");
            }
            else
            {
                Debug.LogError($"Error al enviar score: {www.error}");
            }
        }
    }
    
    // Enviar puntuación y cerrar juego (o volver al sistema)
    private void SendScoreAndExit(string userId, string sessionId, string gameId, string difficulty, int score)
    {
        StartCoroutine(SendScoreToServer(userId, sessionId, gameId, difficulty, score));

        #if UNITY_WEBGL && !UNITY_EDITOR
                    NotifyGameEnded(userId);
        #else
                Debug.Log("Simulación: NotifyGameEnded");
        #endif
    }
}
