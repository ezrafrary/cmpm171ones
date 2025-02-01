using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomList : MonoBehaviourPunCallbacks
{

    //almost all of this code is from the bannadev video https://www.youtube.com/watch?v=_QilKZ1f5Vo&list=PL0iUgXtqnG2gPaXE1hHYoBjqTSWTNwR-6&index=13&ab_channel=bananadev2
    //if you are going to change anything in here, please watch the video first.
    
    public static RoomList Instance;

    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    private int roomJoinSceneIndex = 1;


    private string cachedRoomNameToCreate = "Unnamed Room";


    public void changeRoomJoinIndex(int newIndex){
        roomJoinSceneIndex = newIndex;
    }

    public void ChangeRoomToCreateName(string _roomName){
        cachedRoomNameToCreate = _roomName;
    }

    public void CreateRoomByIndex(){
        JoinRoomByName(cachedRoomNameToCreate, roomJoinSceneIndex);
    }


    private void Awake(){
        Instance = this;
    }

    IEnumerator Start()
    {
        if (PhotonNetwork.InRoom){
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }


        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster(){

        base.OnConnectedToMaster(); 
        PhotonNetwork.JoinLobby();
    }


    //called by photon, but only shows stuff that changed so we gotta deal with that
    public override void OnRoomListUpdate(List<RoomInfo> roomList){

        //if this is the first one
        if (cachedRoomList.Count <= 0){
            cachedRoomList = roomList;
        }else{
            foreach (var room in roomList){
                for (int i = 0; i < cachedRoomList.Count; i++){
                    if (cachedRoomList[i].Name == room.Name){
                        List<RoomInfo> newList = cachedRoomList;


                        if (room.RemovedFromList){
                            newList.Remove(newList[i]);
                        }else{
                            newList[i] = room;
                        }
                        cachedRoomList = newList;
                    }
                }
            }
        }
        UpdateUI();
    }


    void UpdateUI(){
        foreach (Transform roomItem in roomListParent){
            Destroy(roomItem.gameObject); 
        }


        foreach (var room in cachedRoomList){
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/16";
            
            roomItem.GetComponent<RoomItemButton>().RoomName = room.Name;

            int roomSceneIndex = 1;

            object sceneIndexObject;
            if(room.CustomProperties.TryGetValue("mapSceneIndex", out sceneIndexObject)){
                roomSceneIndex = (int)sceneIndexObject;
            }

            roomItem.GetComponent<RoomItemButton>().SceneIndex = roomSceneIndex;
        }

    }

    public void JoinRoomByName(string _name, int _sceneIndex){
        PlayerPrefs.SetString("RoomNameToJoin", _name);
        gameObject.SetActive(false);
        SceneManager.LoadScene(_sceneIndex);//change to _sceneIndex
    }

}
