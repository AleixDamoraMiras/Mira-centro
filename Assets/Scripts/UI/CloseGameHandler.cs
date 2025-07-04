using UnityEngine;
using System.Collections;

public class CloseGameHandler : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void NotifyGameEnded(string userId);
    #endif

    public void OnCloseClicked()
    {
        StartCoroutine(DelayedClose());
    }

    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(0.2f);
        string userId = GameManager.Instance != null ? GameManager.Instance.UserId : string.Empty;

        #if UNITY_WEBGL && !UNITY_EDITOR
                NotifyGameEnded(userId);
        #else
                Debug.Log("NotifyGameEnded is only available in WebGL builds.");
        #endif
    }
}
