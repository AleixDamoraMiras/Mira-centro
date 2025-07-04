using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Clase que representa cada estímulo individual con su imagen y respuesta correcta
[System.Serializable]
public class Stimulus {
    public string image;
    public string correctAnswer;
}

// Clase que representa un nivel completo: nombre, duración, intentos y lista de estímulos
[System.Serializable]
public class Level {
    public string name;
    public float duration;
    public int maxAttempts;
    public List<Stimulus> stimuli;
}

// Contenedor que se usará para deserializar todo el JSON (lista de niveles)
[System.Serializable]
public class LevelCollection {
    public List<Level> levels;
}

// Logic
public class LevelLoader : MonoBehaviour {
    public static LevelLoader Instance;
    public Level CurrentLevel { get; private set; }
    public string LevelsFilePath = "Assets/Resources/Levels.json";
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Carga el JSON desde Resources y selecciona el nivel según dificultad
    public void LoadLevelByDifficulty(string difficulty)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Levels");

        if (jsonFile == null) {
            Debug.LogError("No se encontró el archivo Levels.json en Resources");
            return;
        }

        LevelCollection levelData = JsonUtility.FromJson<LevelCollection>(jsonFile.text);

        // Siempre actualiza
        CurrentLevel = levelData.levels.Find(l => l.name.ToLower() == difficulty.ToLower());

        if (CurrentLevel == null)
        {
            Debug.LogError($"No se encontró el nivel con dificultad: {difficulty}");
        }
        else
        {
            Debug.Log($"Nivel '{CurrentLevel.name}' cargado con {CurrentLevel.stimuli.Count} estímulos.");
        }
    }

}
