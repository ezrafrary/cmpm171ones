using UnityEngine;
using TMPro;
using Photon.Pun;

public class FPScounter : MonoBehaviour
{
    public float timer, refresh, avgFramerate;
    string display = "{0} FPS";
    public TextMeshProUGUI m_Text;
    public TextMeshProUGUI m_Ping;

    


    private void Update()
    {
        //Change smoothDeltaTime to deltaTime or fixedDeltaTime to see the difference
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if(timer <= 0){
            avgFramerate = (int) (1f / timelapse);
            m_Ping.text = PhotonNetwork.GetPing() + " ms";
        } 
        m_Text.text = string.Format(display,avgFramerate.ToString());
    }
}
