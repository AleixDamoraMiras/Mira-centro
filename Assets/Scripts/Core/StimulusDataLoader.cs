using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StimulusData
{
    public string image;
    public string correctAnswer;
}

[System.Serializable]
public class StimulusLevel
{
    public string name;
    public int duration;
    public int maxAttempts;
    public List<StimulusData> stimuli;
}

[System.Serializable]
public class StimulusLevelCollection
{
    public List<StimulusLevel> levels;
}

public class StimulusDataLoader : MonoBehaviour
{
    public static List<StimulusLevel> AllLevels { get; private set; }

    private void Awake()
    {
        LoadAllLevels();
    }

    public void LoadAllLevels()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Levels");
        if (jsonFile == null)
        {
            Debug.LogError("No se encontró el archivo Levels.json en Resources.");
            return;
        }

        StimulusLevelCollection data = JsonUtility.FromJson<StimulusLevelCollection>(jsonFile.text);
        if (data == null || data.levels == null)
        {
            Debug.LogError("Error al leer los niveles del JSON.");
            return;
        }

        AllLevels = data.levels;
    }

    public static StimulusLevel GetLevelByName(string name)
    {
        return AllLevels?.Find(l => l.name.ToLower() == name.ToLower());
    }
}
