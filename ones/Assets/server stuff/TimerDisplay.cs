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


    public static string ConvertSecondsToMinutes(float totalSeconds)
    {
        int minutes = (int) (totalSeconds / 60);


        float seconds = (  (float) ((int)((totalSeconds%60)*10))  )/10;
        
        
        
        string stringseconds = seconds.ToString();
        if(seconds%1 == 0){
            stringseconds = stringseconds + ".0";
        }
        // Format the string as "minutes:seconds"


        if(seconds < 10 && seconds >= 0){
            return minutes + ":0" + stringseconds; 
        }
        return minutes + ":" + stringseconds;
    }




    public static string ConvertSecondsToMinutesNoDecimal(float totalSeconds){
        int minutes = (int) (totalSeconds / 60);

        int seconds = (int)(totalSeconds%60);

        if(seconds < 10 && seconds >= 0){
            return minutes + ":0" + seconds;
        }

        return minutes + ":" + seconds;

    }



    public static string combinedTimerLogic(float totalSeconds){
        if(totalSeconds > 10){
            return ConvertSecondsToMinutesNoDecimal(totalSeconds);
        }else{
            return ConvertSecondsToMinutes(totalSeconds);
        }
    }




    void Update()
    {
        // Display the timer value
        if (photonTimer != null)
        {
            // Show the timer value with a 1 decimal place precision
            //timerText.text = ConvertSecondsToMinutes(/*Mathf.CeilToInt*/(photonTimer.timer));
            timerText.text = combinedTimerLogic((photonTimer.timer));
        }
    }
}
