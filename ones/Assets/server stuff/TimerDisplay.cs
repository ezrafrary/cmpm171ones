using UnityEngine;
using UnityEngine.UI; // for UI elements
using Photon.Pun;
using TMPro;

public class TimerDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText; // Reference to the UI Text component
    private PhotonTimer photonTimer; // Reference to the PhotonTimer script

    void Start()
    {
        photonTimer = FindObjectOfType<PhotonTimer>(); // Find the PhotonTimer script in the scene
        if (timerText == null) 
        {
            Debug.LogError("TimerText UI element not assigned!");
        }
    }


    public static string ConvertSecondsToMinutes(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        
        // Format the string as "minutes:seconds"
        return minutes + ":" + seconds;
    }

    void Update()
    {
        // Display the timer value
        if (photonTimer != null)
        {
            // Show the timer value with a 1 decimal place precision
            timerText.text = ConvertSecondsToMinutes(Mathf.CeilToInt(photonTimer.timer));
        }
    }
}
