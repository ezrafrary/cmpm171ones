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

    [Header("popup")]
    public TextMeshProUGUI roomTakenPopup;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    private int roomJoinSceneIndex = 4; //default room to join


    private string cachedRoomNameToCreate = "Unnamed Room";



    private float autoPressRefresh = 1;

    void Update(){
        if(autoPressRefresh > 0){
            autoPressRefresh = autoPressRefresh - Time.deltaTime;
        }else{
            autoPressRefresh = 5;
            refreshRoomList();
            //Debug.Log("refreshing");
        }
    }



    public void changeRoomJoinIndex(int newIndex){
        roomJoinSceneIndex = newIndex;
    }

    public void ChangeRoomToCreateName(string _roomName){
        cachedRoomNameToCreate = _roomName;
    }

    public void CreateRoomByIndex(){
        if(isRoomNameTaken(cachedRoomNameToCreate)){
            roomTakenPopup.gameObject.SetActive(true);
            roomTakenPopup.text = "Room Name " + cachedRoomNameToCreate + " is taken right now!";
            Debug.Log("Room name taken!");
        }else{
            AnalyticsManager.Instance.GameStarted(roomJoinSceneIndex);
            Debug.Log("creating room with name " + cachedRoomNameToCreate);
            JoinRoomByName(cachedRoomNameToCreate, roomJoinSceneIndex);

        }
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

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster(){

        base.OnConnectedToMaster(); 
        if (!PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    //if a player disconnects, or gets booted out of a room, we need to call this
    public void refreshRoomList(){
        // Make sure the player is not in a room before refreshing the list
        if (PhotonNetwork.InRoom) {
            PhotonNetwork.LeaveRoom();
        }

        // Make sure we're connected to the master server
        if (PhotonNetwork.IsConnected && PhotonNetwork.NetworkClientState != ClientState.JoiningLobby) {
            // Leave the current room (if any) and join the lobby to get an updated list of rooms
            PhotonNetwork.JoinLobby();
        }
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


    public bool isRoomNameTaken(string _newRoomName){
        foreach (var room in cachedRoomList){
            if(_newRoomName == room.Name){
                return true;
            }
        }
        return false;
    }



    public void printAllRoomNames(){
        Debug.Log("regular rooms:");
        foreach (var room in cachedRoomList){
            if(room.Name.Length <= 16){
                Debug.Log(room.Name);
            }
        }
        Debug.Log("Matchmaking Rooms:");
        foreach (var room in cachedRoomList){
            if(room.Name.Length > 16){
                Debug.Log(room.Name);
            }
        }
    }

    void UpdateUI(){
        foreach (Transform roomItem in roomListParent){
            Destroy(roomItem.gameObject); 
        }


        foreach (var room in cachedRoomList){

            if(room.Name.Length > 16){ //any roomname longer than 16 is a matchmaking room
                 
            }else{
                if(room.PlayerCount > 0){
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
            
            
        }

    }

    public int JoinRoomByName(string _name, int _sceneIndex){
        if(_sceneIndex < 0){
            return -1;
        }
        PlayerPrefs.SetString("RoomNameToJoin", _name);
        gameObject.SetActive(false);
        SceneManager.LoadScene(_sceneIndex);//change to _sceneIndex
        return 1;
    }

    public void JoinOfflineTutorialRoom()
    {
        // Ensure Photon is not connected online
        //gameObject.SetActive(true);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            StartCoroutine(JoinOfflineAfterDisconnect());
        }
        else
        {
            StartOfflineMode();
        }
    }

    private IEnumerator JoinOfflineAfterDisconnect()
    {
        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);
        StartOfflineMode();
    }
    private void StartOfflineMode()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Tried to enter offline mode while still connected. Wait for disconnection.");
            return;
        }

        PhotonNetwork.OfflineMode = true;
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = 1 };
        PhotonNetwork.JoinOrCreateRoom("OfflineTutorial", roomOptions, TypedLobby.Default);
        SceneManager.LoadScene(5);
    }

    public int getMapSceneIndex(string roomname)
    {
        foreach (var room in cachedRoomList)
        {
            if (room.Name == roomname)
            {
                object sceneIndexObject;
                if (room.CustomProperties.TryGetValue("mapSceneIndex", out sceneIndexObject))
                {
                    return (int)sceneIndexObject;
                }
                else
                {
                    Debug.LogWarning("Room found but has no 'mapSceneIndex' custom property. Returning default index.");
                    return roomJoinSceneIndex; // fallback if mapSceneIndex isn't set
                }
            }
        }

        //Debug.LogWarning("Room with name " + roomname + " not found in cachedRoomList.");
        return -1; // return -1 to indicate room not found
    }


}
