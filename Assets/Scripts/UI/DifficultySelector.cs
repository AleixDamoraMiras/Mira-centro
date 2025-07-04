using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DifficultySelector : MonoBehaviour
{
    public void loadTutorial()
    {
        StartCoroutine(DelayedLoadTutorial());
    }

    private IEnumerator DelayedLoadTutorial()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("DiffToInst");
    }
    
    public void SelectDifficulty(string difficulty)
    {
        StartCoroutine(DelayedSelectDifficulty(difficulty));
    }

    private IEnumerator DelayedSelectDifficulty(string difficulty)
    {
        yield return new WaitForSeconds(0.2f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(difficulty.ToLower());
            GameManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogError("GameManager no encontrado.");
        }
    }
}
