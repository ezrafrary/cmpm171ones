using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;

public class backToMenuScript : MonoBehaviour
{
    public void backToMenuButtonPressed(){
        RoomManager.instance.kills = 0;
        RoomManager.instance.deaths = 0;
        RoomManager.instance.score = 0;
        RoomManager.instance.SetHashes();
        
        if(PhotonNetwork.InRoom){
            PhotonNetwork.LeaveRoom();
        }
        SceneManager.LoadScene(0);
    }
}
