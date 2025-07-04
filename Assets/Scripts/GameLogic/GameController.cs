using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Config")]
    public bool isTutorial = false;
    public string levelName = "Instructions"; // En juego real se usará el valor desde GameManager
    public UnityEvent onTutorialComplete;

    [Header("UI References")]
    public Image stimulusImage;

    [Header("Botones de flechas")]
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    
    [Header("Feedback")]
    public GameObject feedbackContainer;
    public GameObject tickImage;
    public GameObject crossImage;
    public float feedbackDuration = 1.5f;

    [Header("Score (solo juego real)")]
    public Text scoreText;

    private StimulusLevel currentLevel;
    private int currentIndex = 0;
    private int remainingAttempts;
    private bool inputLocked = false;
    private int score = 0;
    private int correct = 0;
    private int wrong = 0;
    private float startTime;

    // Auido
    [Header("Score (solo juego real)")]
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip incorrectClip;
    
    private void Start()
    {
        if (StimulusDataLoader.AllLevels == null)
        {
            var loader = FindObjectOfType<StimulusDataLoader>();
            if (loader != null)
            {
                loader.LoadAllLevels();
            }
            else
            {
                Debug.LogError("StimulusDataLoader no encontrado en la escena.");
                return;
            }

            if (StimulusDataLoader.AllLevels == null)
            {
                Debug.LogError("StimulusDataLoader no ha podido cargar los niveles.");
                return;
            }
        }

        string lvl = isTutorial ? levelName : GameManager.Instance.Difficulty;
        currentLevel = StimulusDataLoader.GetLevelByName(lvl);

        if (currentLevel == null)
        {
            Debug.LogError("No se encontró el nivel: " + lvl);
            return;
        }
        
        if (!isTutorial) remainingAttempts = currentLevel.maxAttempts;

        startTime = Time.time;
        ShowStimulus();
        UpdateScoreUI();
    }

    private void Update()
    {
        if (inputLocked || currentIndex >= currentLevel.stimuli.Count) return;
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) HandleAnswer("left");
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) HandleAnswer("right");
        if (Keyboard.current.upArrowKey.wasPressedThisFrame) HandleAnswer("up");
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) HandleAnswer("down");
    }
    
    private void SetArrowButtonsInteractable(bool state)
    {
        if (upButton != null) upButton.interactable = state;
        if (downButton != null) downButton.interactable = state;
        if (leftButton != null) leftButton.interactable = state;
        if (rightButton != null) rightButton.interactable = state;
    }
    
    // Buttons Rows 
    public void upAnswerBtn()
    {
        HandleAnswer("up");
    }

    public void downAnswerBtn()
    {
        HandleAnswer("down");
    }

    public void rightAnswerBtn()
    {
        HandleAnswer("right");
    }

    public void leftAnswerBtn()
    {
        HandleAnswer("left");
    }
    
    // Punctuation
    private void HandleAnswer(string input)
    {
        inputLocked = true;
        SetArrowButtonsInteractable(false);
        

        var stimulus = currentLevel.stimuli[currentIndex];
        bool correctAnswer = input.ToLower() == stimulus.correctAnswer.ToLower();

        if (correctAnswer)
        {
            score++;
            correct++;
            PlaySound(correctClip);
            ShowFeedback(true);
        }
        else
        {
            wrong++;
            PlaySound(incorrectClip);
            ShowFeedback(false);
        }

        Invoke(nameof(ContinueAfterFeedback), feedbackDuration);
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score;
        }
    }

    
    // Feedback
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
        if (feedbackContainer != null) feedbackContainer.SetActive(false);
    }
    private void ContinueAfterFeedback()
    {
        currentIndex++;
        inputLocked = false;
        SetArrowButtonsInteractable(true);

        if (currentIndex >= currentLevel.stimuli.Count)
        {
            if (isTutorial)
            {
                onTutorialComplete?.Invoke();
            }
            else
            {
                EndGame();
            }
        }
        else
        {
            if (!isTutorial && GameManager.Instance.Difficulty.ToLower() == "dificil")
            {
                remainingAttempts--;
                if (remainingAttempts <= 0)
                {
                    EndGame();
                    return;
                }
            }

            ShowStimulus();
        }
    }
    
    
    // Other functions 
    public void Load() {
        SceneManager.LoadScene("Selector");
    }
    
    public void ReturnDiff() {
        SceneManager.LoadScene("DifficultySelection");
    }
    
    private void ShowStimulus()
    {
        var stimulus = currentLevel.stimuli[currentIndex];
        Sprite sprite = Resources.Load<Sprite>("Stimuli/" + stimulus.image);

        if (sprite != null && stimulusImage != null)
        {
            stimulusImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Imagen no encontrada: " + stimulus.image);
        }
    }

    private void EndGame()
    {
        float totalTime = Time.time - startTime;

        GameManager.Instance.Score = score;
        GameManager.Instance.CorrectAnswers = correct;
        GameManager.Instance.WrongAnswers = wrong;
        GameManager.Instance.TimeTaken = totalTime;

        GameManager.Instance.LoadGameOver();
    }
}
