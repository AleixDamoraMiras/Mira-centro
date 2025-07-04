using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Navegation : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject instructionsUI;
    public CountdownTimer countdownTimer;

    #if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern void NotifyGameEnded(string userId);

            [DllImport("__Internal")]
            private static extern void SkipGame();
    #endif

    private void Start()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (instructionsUI != null) instructionsUI.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        StartCoroutine(DelayedAction(() =>
        {
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            if (countdownTimer != null) countdownTimer.SetPaused(false);
        }));
    }

    public void OpenInstructions()
    {
        if (instructionsUI != null) instructionsUI.SetActive(true);
        if (countdownTimer != null) countdownTimer.SetPaused(true);
    }

    public void CloseInstructions()
    {
        StartCoroutine(DelayedAction(() =>
        {
            if (instructionsUI != null) instructionsUI.SetActive(false);
            if (countdownTimer != null) countdownTimer.SetPaused(false);
        }));
    }

    public void CallSkipGame()
    {
        StartCoroutine(DelayedAction(() =>
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                            SkipGame();
            #else
                        Debug.Log("SkipGame llamado (modo editor)");
            #endif
        }));
    }

    public void ExitGame()
    {
        StartCoroutine(DelayedAction(HandleGameExit));
    }

    private IEnumerator DelayedAction(System.Action action)
    {
        yield return new WaitForSeconds(0.2f);
        action?.Invoke();
    }

    private void HandleGameExit()
    {
        string userId = GameManager.Instance != null ? GameManager.Instance.UserId : "";

        #if UNITY_WEBGL && !UNITY_EDITOR
                    NotifyGameEnded(userId);
        #else
                Debug.Log("NotifyGameEnded (Editor): " + userId);
        #endif
    }
}
