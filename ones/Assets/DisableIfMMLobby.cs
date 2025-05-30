using UnityEngine;

public class DisableIfMMLobby : MonoBehaviour
{
    public bool IsMMLobby = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        updateIsMMLobby();
        if(IsMMLobby){
            gameObject.SetActive(false);
        }
    }


    public void updateIsMMLobby(){
        if(RoomManager.instance){
            
            string tempText = RoomManager.instance.findRoomName();
            if(tempText.Length > 15){ //matchmaking lobby, dont display roomname
                IsMMLobby = true;
                Debug.Log("ismatchmakingLobby");
                return;
            }
            Debug.Log("notAmatchmakingLobby");
            IsMMLobby = false;
            if(tempText == "defaultStatement"){ //returned if player isnt in a room
                Debug.Log("playernotinroom");
            }
        }else{
            Debug.Log("noRoommanagerInstance");
        }
    }
}
