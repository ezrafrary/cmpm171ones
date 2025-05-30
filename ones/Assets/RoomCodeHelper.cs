using UnityEngine;
using TMPro;


public class RoomCodeHelper : MonoBehaviour
{
    public TextMeshProUGUI roomCodeText;
    public float updateTimer = 2;
    private float _updateTimer;



    

    void Start(){
        updateRoomCodeText();
    }

    void Update(){
        if(_updateTimer > 0){
            _updateTimer -= Time.deltaTime;
        }else{
            updateRoomCodeText();
            _updateTimer = updateTimer;
        }
    }


    public void updateRoomCodeText(){
        if(RoomManager.instance){
            roomCodeText.enabled = true;
            
            string tempText = RoomManager.instance.findRoomName();
            if(tempText.Length > 15){ //matchmaking lobby, dont display roomname
                return;
            }
            if(tempText != "defaultStatement"){ //returned if player isnt in a room
                roomCodeText.text = "Room Code: " + tempText;
            }else{
                roomCodeText.text = "roomNameNotFound";
            }
        }else{
            roomCodeText.enabled = false;
        }
    }
}
