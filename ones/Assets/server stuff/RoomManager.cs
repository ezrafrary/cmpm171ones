using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{

    /*
        this class handles most of te serverside things. watch all of the following videos before editing it:
        https://www.youtube.com/watch?v=xRXOnuFji-Q&list=PL0iUgXtqnG2gPaXE1hHYoBjqTSWTNwR-6&index=1&ab_channel=bananadev2 
        https://www.youtube.com/watch?v=mZP_X1ejj8s&list=PL0iUgXtqnG2gPaXE1hHYoBjqTSWTNwR-6&index=5&ab_channel=bananadev2 
        https://www.youtube.com/watch?v=x8dfgCoe53w&list=PL0iUgXtqnG2gPaXE1hHYoBjqTSWTNwR-6&index=10&ab_channel=bananadev2 
        https://www.youtube.com/watch?v=TOP-huJ9duY&list=PL0iUgXtqnG2gPaXE1hHYoBjqTSWTNwR-6&index=12&ab_channel=bananadev2 
        https://www.youtube.com/watch?v=_QilKZ1f5Vo&list=PL0iUgXtqnG2gPaXE1hHYoBjqTSWTNwR-6&index=13&ab_channel=bananadev2
    */

    public static RoomManager instance;

    public GameObject player;
    [Space]
    public Transform[] spawnPoints;
    [Space]
    public GameObject roomCam;

    [Space]
    public GameObject nameUI;
    public GameObject connectingUI;
    public GameObject mainMenuUI;
    public GameObject youDiedUi;
    public GameObject gameOverUI;

    public PhotonTimer timer;

    private string defaultname = "unnamed";

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;
    [HideInInspector]
    public float playerSensX = 2;
    [HideInInspector]
    public float playerSensY = 2;
    [HideInInspector]
    public float playerFov = 60;

    public string roomNameToJoin = "test";


    
    private List<GameObject> players = new List<GameObject>();
    


    void Awake(){
        instance = this;
    }


    public void changeSens(float _sensX, float _sensY){
        playerSensX = _sensX;
        playerSensY = _sensY;
    }

    public void changeFov(int newFov){
        playerFov = newFov;
    }

    public void ChangeNicname(string _name){
        PlayerPrefs.SetString("playerName", _name);
    }
    
    public void JoinRoomButtonPressed(){
        Debug.Log("Connecting");

        RoomOptions ro = new RoomOptions();
        ro.CustomRoomProperties = new Hashtable(){
            {"mapSceneIndex", SceneManager.GetActiveScene().buildIndex}
        };
        
        ro.CustomRoomPropertiesForLobby = new []
        {
            "mapSceneIndex"
        };



        PhotonNetwork.JoinOrCreateRoom(PlayerPrefs.GetString("RoomNameToJoin"), ro, null);

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    
        
    }


    public override void OnJoinedRoom(){
        base.OnJoinedRoom();

        Debug.Log("We're connected and in a room!");
        connectingUI.SetActive(false);
        roomCam.SetActive(false);
        SpawnPlayer();
        timer.StartTimer();

    }

    public override void OnLeftRoom(){
        base.OnLeftRoom();
        Debug.Log("left room");
        roomCam.SetActive(true);
        try{
            mainMenuUI.SetActive(true);
        }catch{

        }
    }

    public void PlayerDied(){ //gets called by Health.cs when the player dies
        Debug.Log("player died");
        MouseLook.UnlockCursorStatic();
        roomCam.SetActive(true);
        youDiedUi.SetActive(true);
    }

    public void respawnPlayer(){
        roomCam.SetActive(false);
        youDiedUi.SetActive(false);
        SpawnPlayer();
    }

    public void EndGame(){
        try{
            KillAllPlayers(); //calls playerDied(), errors if player is already dead
            roomCam.SetActive(true);
            youDiedUi.SetActive(false);
            gameOverUI.SetActive(true);
            Debug.Log("game ended");
        }catch{
            roomCam.SetActive(true);
            youDiedUi.SetActive(false);
            gameOverUI.SetActive(true);
            Debug.Log("game ended while player was dead");
        }
    }


    public void KillAllPlayers(){
        foreach (GameObject i in players){
            Debug.Log(i);
            i.GetComponent<PhotonView>().RPC("KillPlayer",RpcTarget.All);
        }
    }

    public void SpawnPlayer(){

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer(); 
        _player.GetComponent<Health>().IsLocalPlayer = true; 
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString("playerName", defaultname));


        //set local settings using PlayerSetup
        _player.GetComponent<PlayerSetup>().SetPlayerSens(playerSensX, playerSensY);
        _player.GetComponent<PlayerSetup>().SetCameraFov(playerFov);

        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("playerName", defaultname);
        

        players.Add(_player);
    }

    
    public void SetHashes(){
        try{
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch{
            //nothing
        }
    }
}
