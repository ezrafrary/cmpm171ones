using UnityEngine;
using Photon.Pun;
using TMPro;

public class timerScript : MonoBehaviourPunCallbacks
{
    public float timerDuration = 10f;

    public RoomManager roomManager;
    private float origionalTimerDuration;



    void Start(){
        origionalTimerDuration = timerDuration;
        Debug.Log("tiemr created");
    }

    void Update(){
        if(timerDuration > 0f){
            timerDuration = timerDuration - Time.deltaTime;

        }else{
            timerEnded();
        }
    }

    public void StartTimer(){
        Debug.Log("staring timer");
    }


    public void timerEnded(){
        Debug.Log("timer ended");
        roomManager.EndGame();
    }


}
