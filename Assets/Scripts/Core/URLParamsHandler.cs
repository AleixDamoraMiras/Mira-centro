using System.Collections.Generic;
using UnityEngine;

public class URLParamsHandler : MonoBehaviour
{
    public static URLParamsHandler Instance;

    public string UserId { get; private set; }
    public string SessionId { get; private set; }
    public string Difficulty { get; private set; }
    public string GameId { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ReadUrlParameters();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ReadUrlParameters()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            string url = Application.absoluteURL;
            Dictionary<string, string> parameters = ParseQueryString(url);

            string userId, sessionId, difficulty, gameId;
            parameters.TryGetValue("userId", out userId);
            parameters.TryGetValue("sessionId", out sessionId);
            parameters.TryGetValue("difficulty", out difficulty);
            parameters.TryGetValue("gameId", out gameId);

            UserId = userId;
            SessionId = sessionId;
            Difficulty = difficulty;
            GameId = gameId;
        #else
            // Para pruebas en el editor
            string testUrl = "https://memorix/mirar-al-centro/?userId=1&gameId=109&sessionId=254&difficulty=";
            Dictionary<string, string> parameters = ParseQueryString(testUrl);
            
            string userId, sessionId, difficulty, gameId;
            parameters.TryGetValue("userId", out userId);
            parameters.TryGetValue("sessionId", out sessionId);
            parameters.TryGetValue("difficulty", out difficulty);
            parameters.TryGetValue("gameId", out gameId);

            UserId = userId;
            SessionId = sessionId;
            Difficulty = difficulty;
            GameId = gameId;

            Debug.Log($"Params recibidos -> UserId: {UserId}, SessionId: {SessionId}, Difficulty: {Difficulty}, GameId: {GameId}");
        #endif
    }

    private Dictionary<string, string> ParseQueryString(string url)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        if (url.Contains("?"))
        {
            string query = url.Substring(url.IndexOf('?') + 1);
            string[] pairs = query.Split('&');

            foreach (string pair in pairs)
            {
                string[] kv = pair.Split('=');
                if (kv.Length == 2)
                {
                    string key = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(kv[0]);
                    string value = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(kv[1]);
                    result[key] = value;
                }
            }
        }

        return result;
    }
}

