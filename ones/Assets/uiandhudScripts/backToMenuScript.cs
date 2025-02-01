using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class backToMenuScript : MonoBehaviour
{
    public void backToMenuButtonPressed(){
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
