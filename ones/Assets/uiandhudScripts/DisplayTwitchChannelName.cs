using UnityEngine;
using TMPro;

public class DisplayTwitchChannelName : MonoBehaviour
{
    public TextMeshProUGUI displayText;



    public void updateText(string newDisplayText){
        displayText.text = newDisplayText;
    }

}
