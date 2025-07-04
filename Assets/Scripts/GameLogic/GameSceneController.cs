using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameSceneController : MonoBehaviour
{
    [Header("UI References")]
    private Text userIdText;
    private Text sessionIdText;
    private Text difficultyText;
    private Text scoreText;
    public Image stimulusImage;

    [Header("Feedback Visual")]
    public GameObject feedbackContainer;
    public GameObject tickImage;
    public GameObject crossImage;
    public float feedbackDuration = 1.5f;

    private int score = 0;
    private int currentStimulusIndex = 0;
    private int remainingAttempts;
    private bool useAttempts = false;

    private int totalCorrectAnswers = 0;
    private int totalWrongAnswers = 0;
    private float startTime;
    private float endTime;

    private bool inputLocked = false; // Nueva variable para bloquear input

    private void Start()
    {
        startTime = Time.time;

        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.LoadLevelByDifficulty(GameManager.Instance.Difficulty);

            useAttempts = GameManager.Instance.Difficulty.ToLower() == "dificil";
            ResetAttemptsForCurrentStimulus();

            ShowCurrentStimulus();
            UpdateScoreText();
        }
    }

    private void Update()
    {
        if (inputLocked) return;

        if (LevelLoader.Instance?.CurrentLevel != null &&
            currentStimulusIndex < LevelLoader.Instance.CurrentLevel.stimuli.Count)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) HandleAnswer("left");
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) HandleAnswer("right");
            if (Keyboard.current.upArrowKey.wasPressedThisFrame) HandleAnswer("up");
            if (Keyboard.current.downArrowKey.wasPressedThisFrame) HandleAnswer("down");
        }
    }

    private void HandleAnswer(string answer)
    {
        inputLocked = true;

        var stimulus = LevelLoader.Instance.CurrentLevel.stimuli[currentStimulusIndex];

        if (answer.ToLower() == stimulus.correctAnswer.ToLower())
        {
            score++;
            totalCorrectAnswers++;
            ShowFeedback(true);
            UpdateScoreText();
            Invoke(nameof(NextStimulus), feedbackDuration);
        }
        else
        {
            totalWrongAnswers++;
            ShowFeedback(false);

            if (useAttempts)
            {
                remainingAttempts--;
                if (remainingAttempts <= 0)
                {
                    Invoke(nameof(EndGame), feedbackDuration);
                    return;
                }
            }

            Invoke(nameof(NextStimulus), feedbackDuration);
        }
    }

    private void ShowCurrentStimulus()
    {
        var stimulus = LevelLoader.Instance.CurrentLevel.stimuli[currentStimulusIndex];
        Sprite sprite = Resources.Load<Sprite>("Stimuli/" + stimulus.image);

        if (sprite != null && stimulusImage != null)
        {
            stimulusImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("No stimulus image found " + stimulus.image);
        }
    }

    
    private void NextStimulus()
    {
        currentStimulusIndex++;
        ResetAttemptsForCurrentStimulus();
        inputLocked = false;

        if (currentStimulusIndex < LevelLoader.Instance.CurrentLevel.stimuli.Count)
        {
            ShowCurrentStimulus();
        }
        else
        {
            EndGame();
        }
    }
    
    private void ResetAttemptsForCurrentStimulus()
    {
        if (useAttempts)
        {
            remainingAttempts = LevelLoader.Instance.CurrentLevel.maxAttempts;
        }
    }
    
    
    private void ShowFeedback(bool isCorrect)
    {
        if (feedbackContainer == null || tickImage == null || crossImage == null) return;

        feedbackContainer.SetActive(true);
        tickImage.SetActive(isCorrect);
        crossImage.SetActive(!isCorrect);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), feedbackDuration);
    }

    private void HideFeedback()
    {
        feedbackContainer?.SetActive(false);
    }
    
    
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score;
        }
    }

    private void EndGame()
    {
        endTime = Time.time;
        float totalTime = endTime - startTime;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Score = score;
            GameManager.Instance.CorrectAnswers = totalCorrectAnswers;
            GameManager.Instance.WrongAnswers = totalWrongAnswers;
            GameManager.Instance.TimeTaken = totalTime;

            GameManager.Instance.LoadGameOver();
        }
    }
}
