using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Collections;

public class Menus : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject InstructionsUI;
    public CountdownTimer countdownTimer;

    #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SkipGame();
    #endif
    
    #if UNITY_WEBGL && !UNITY_EDITOR
        // Función JS para redirigir al jugador fuera del juego
        [DllImport("__Internal")]
        private static extern void NotifyGameEnded(string userId);
    #endif

    // Inicializar: asegurarse de que el panel de pausa esté oculto al principio
    private void Start()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (InstructionsUI != null)
        {
            InstructionsUI.SetActive(false);
        }
    }

    // Abre el menú de pausa y detiene el cronómetro.
    public void OpenPauseMenu()
    {
        // if (countdownTimer != null) countdownTimer.SetPaused(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    //  Reanuda el juego: cierra el menú y continúa el tiempo.
    public void ClosePauseMenu()
    {
        StartCoroutine(DelayedClosePauseMenu());
    }
    
    private IEnumerator DelayedClosePauseMenu()
    {
        yield return new WaitForSeconds(0.2f);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (countdownTimer != null) countdownTimer.SetPaused(false);
    }
    
    public void OpenInstructions()
    {
        if (InstructionsUI != null) InstructionsUI.SetActive(true);
        if (countdownTimer != null) countdownTimer.SetPaused(true);
    }
    
    public void CloseInstructions()
    {
        StartCoroutine(DelayedCloseInstructions());
    }

    private IEnumerator DelayedCloseInstructions()
    {
        yield return new WaitForSeconds(0.2f);
        if (InstructionsUI != null) InstructionsUI.SetActive(false);
        if (countdownTimer != null) countdownTimer.SetPaused(false);
    }
    
    // Llama a la función JS para salir del juego, redirigiendo al usuario
    public void ExitGame()
    {
        StartCoroutine(DelayedExitGame());
    }

    private IEnumerator DelayedExitGame()
    {
        yield return new WaitForSeconds(0.2f);
        string userId = GameManager.Instance != null ? GameManager.Instance.UserId : "";

        #if UNITY_WEBGL && !UNITY_EDITOR
            NotifyGameEnded(userId);
        #else
            Debug.Log("Salir del juego (modo editor) con userId: " + userId);
        #endif
    }
    
    public void CallSkipGame()
    {
        StartCoroutine(DelayedSkipGame());
    }

    private IEnumerator DelayedSkipGame()
    {
        yield return new WaitForSeconds(0.2f);

        #if UNITY_WEBGL && !UNITY_EDITOR
            SkipGame();
        #else
            Debug.Log("SkipGame llamado (modo editor)");
        #endif
    }
}
