using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

[System.Serializable]
public class HintData
{
    public List<string> hints;
}

public class HintManager : MonoBehaviour
{
    public TextMeshProUGUI hintText; // Reference to your TMP UI text

    void Start()
    {
        // Option 1: If JSON is in StreamingAssets
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "hints.json");

        if (File.Exists(jsonPath))
        {
            string jsonContent = File.ReadAllText(jsonPath);
            HintData data = JsonUtility.FromJson<HintData>(jsonContent);

            if (data.hints != null && data.hints.Count > 0)
            {
                string randomHint = data.hints[Random.Range(0, data.hints.Count)];
                hintText.text = "Tip: \n" + randomHint;
            }
        }
        else
        {
            Debug.LogError("Hints JSON file not found at " + jsonPath);
        }

        // Option 2: If JSON is in Resources folder as a TextAsset
        /*
        TextAsset jsonFile = Resources.Load<TextAsset>("hints");
        if (jsonFile != null)
        {
            HintData data = JsonUtility.FromJson<HintData>(jsonFile.text);
            if (data.hints != null && data.hints.Count > 0)
            {
                string randomHint = data.hints[Random.Range(0, data.hints.Count)];
                hintText.text = randomHint;
            }
        }
        */
    }
}
