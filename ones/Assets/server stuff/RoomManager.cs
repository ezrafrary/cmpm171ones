using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
    public Transform allPlayers;
    [Space]
    public GameObject nameUI;
    public GameObject connectingUI;
    public GameObject mainMenuUI;
    public GameObject waitingForPlayerScreen;
    public TextMeshProUGUI playerCountText;



    [Header("game start screen")]
    public TextMeshProUGUI mapnameText;

    [Header("death screen")]

    public Image RespawnButtonBackground;
    public float AutoRespawnTimer;
    public float TimeUntilRespawnAvalible;
    public TextMeshProUGUI respawnTimer;
    public int deathcamIndex = 0;
    public ClipHolder clipHolder;
    public int currentReplayClipIndex = -1;



    private float _AutoRespawnTimer;
    private float _TimeUntilRespawnAvalible;
    private bool CanRespawn = false;



    public GameObject youDiedUi;
    public TextMeshProUGUI killerNameUi;
    public TextMeshProUGUI killerWeaponUi;
    public Image headshotIcon;
    public Image explosiveIcon;
    public Image bodyshotIcon;
    public TextMeshProUGUI killerHealthLeftUi;

    

    [Space]
    [Header("GameEndLogic")]
    public GameObject gameOverUI;
    public PhotonTimer timer;
    public Leaderboard endgameLeaderboard;

    private string defaultname = "unnamed";

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;
    [HideInInspector]
    public int score = 0;


    [HideInInspector]
    public float playerSensX = 2;
    [HideInInspector]
    public float playerSensY = 2;

    [Header("Roomname")]
    public string roomNameToJoin = "test";


    [Header("Ragdolls")]
    public GameObject ragdollPrefab;
    public GameObject headlessRagdollPrefab;

    public bool gameStarted = false;


    
    private List<GameObject> players = new List<GameObject>();
    


    void Awake(){
        instance = this;
    }

    void Start(){
        RespawnButtonBackground.color = new Color32(0,0,0,100);
        CanRespawn = false;
        if(mapnameText){
            mapnameText.text = SceneManager.GetActiveScene().name;
        }
        SceneManager.LoadScene(6, LoadSceneMode.Additive); //escapemenu index
        UpdatePlayerCount();
    }



    void Update(){

        if(!gameStarted){
            if(UpdatePlayerCount() > 1){
                startGame();
            }
        }

        if(_TimeUntilRespawnAvalible > 0){
            _TimeUntilRespawnAvalible = _TimeUntilRespawnAvalible - Time.deltaTime;
            CanRespawn = false;
        }else{
            CanRespawn = true;
            RespawnButtonBackground.color = new Color32(255,255,255,100);
        }


        if(_AutoRespawnTimer > 0){
            _AutoRespawnTimer = _AutoRespawnTimer - Time.deltaTime;
        }else{
            if(youDiedUi.activeSelf){
                Debug.Log("trying to respawnplayer");
                respawnPlayer();
            }
        }
        
        setTimerText();
    }



    public void openLoadoutMenu(){
        PermenantEscapeMenu.Instance.openLoadoutMenu();
    }
    

    public void changeSens(float _sensX, float _sensY){
        playerSensX = _sensX;
        playerSensY = _sensY;
    }

 

    public void ChangeNicname(string _name){
        PlayerPrefs.SetString("playerName", _name);
    }
    




    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting");

        if (PhotonNetwork.OfflineMode && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            StartCoroutine(JoinRoomAfterLeaving());
            return;
        }

        JoinRoomNow();
    }

    private IEnumerator JoinRoomAfterLeaving()
    {
        // Wait until the client has actually left the room
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }

        JoinRoomNow();
    }

    private void JoinRoomNow()
    {
        RoomOptions ro = new RoomOptions();
        ro.CustomRoomProperties = new Hashtable(){
            {"mapSceneIndex", SceneManager.GetActiveScene().buildIndex}
        };

        ro.CustomRoomPropertiesForLobby = new[] { "mapSceneIndex" };

        PhotonNetwork.JoinOrCreateRoom(PlayerPrefs.GetString("RoomNameToJoin"), ro, null);

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
        
    }



    public override void OnJoinedRoom(){
        base.OnJoinedRoom();

        Debug.Log("We're connected and in a room!");
        connectingUI.SetActive(false);
        waitingForPlayerScreen.SetActive(true);
        UpdatePlayerCount();
        //player has connected and is ready to start the game

        if(UpdatePlayerCount() > 1){
            startGame();
        }else if(SceneManager.GetActiveScene().name == "TUTORIAL"){
            startGame();
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer){
        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        UpdatePlayerCount();
    }


    int UpdatePlayerCount()
    {
        if (PhotonNetwork.InRoom)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            if(playerCountText){
                playerCountText.text = $"Players in room: {playerCount}";
            }
            return playerCount;
        }
        else
        {
            if(playerCountText){
                playerCountText.text = "Not in a room.";
            }
        }
        return 0;
    }

    public void startGame(){
        waitingForPlayerScreen.SetActive(false);
        roomCam.SetActive(false);
        SpawnPlayer();
        timer.StartTimer();
        gameStarted = true;
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

    IEnumerator WaitAndDoSomething()
    {
        yield return new WaitForSeconds(2.1f);
        try{
            clipHolder.playClip(clipHolder.savedReplays.Count - 1);
        }catch{
            
        }
    }

    public void SpawnRagDoll(Vector3 spawnPos, Quaternion inputQuat, Vector3 inputVelocity){
        var spawnedRagdoll = Instantiate(ragdollPrefab, spawnPos, inputQuat);
        spawnedRagdoll.GetComponent<DeadPlayerRigidBodyContainer>()._rigidbody.linearVelocity = inputVelocity;
    }

    public void SpawnHeadlessRagdoll(Vector3 spawnPos, Quaternion inputQuat, Vector3 inputVelocity){
        var spawnedRagdoll = Instantiate(headlessRagdollPrefab, spawnPos, inputQuat);
        spawnedRagdoll.GetComponent<DeadPlayerRigidBodyContainer>()._rigidbody.linearVelocity = inputVelocity;
    }

    public void PlayerDied(string _damageDealer, string _weaponName, string _killMethod, int killerHealthLeft, int replayID){ //gets called by Health.cs when the player dies
        //Debug.Log("player died");
        MouseLook.UnlockCursorStatic();
        roomCam.SetActive(true);
        youDiedUi.SetActive(true);
        SetDeathScreenUi(_damageDealer, _weaponName, _killMethod, killerHealthLeft);


        //Debug.Log("currentClipToPlay: " + clipHolder.savedReplays.Count);
        
        
        StartCoroutine(WaitAndDoSomething());
        

        // if(replayID != -1){
        //     Debug.Log("Play replay " + replayID);
        //     clipHolder.playClip(replayID);
        // }
    }



    public void SetDeathScreenUi(string _damageDealer, string _weaponName, string _killMethod, int killerHealthLeft){
        if(_damageDealer == ""){
            killerNameUi.text = "World";
        }else{
            killerNameUi.text = _damageDealer;
        }

        if(_weaponName == ""){
            killerWeaponUi.text = "World";
        }else{
            killerWeaponUi.text = _weaponName;
        }


        if(_killMethod == "head"){
            headshotIcon.enabled = true;
        }else{
            headshotIcon.enabled = false;
        }

        if(_killMethod == "explosion"){
            explosiveIcon.enabled = true;
        }else{
            explosiveIcon.enabled = false;
        }

        killerHealthLeftUi.text = killerHealthLeft.ToString();


        _AutoRespawnTimer = AutoRespawnTimer;
        _TimeUntilRespawnAvalible = TimeUntilRespawnAvalible;
        RespawnButtonBackground.color = new Color32(0,0,0,100);
    }

    public void setTimerText(){

        int AutoRespawnTimerToInt = (int)(_AutoRespawnTimer * 100);
        float dispalyRespawnTimer = (float)(AutoRespawnTimerToInt) / 100f;

        respawnTimer.text = dispalyRespawnTimer.ToString();
    }


    public void respawnPlayer(){
        if(CanRespawn){
            roomCam.SetActive(false);
            youDiedUi.SetActive(false);
            SpawnPlayer();
        }
    }

    public void EndGame(){ //this may be buggy, needs testing, called by PhotonTimer.cs
        

        
        
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
        
        if(PhotonNetwork.InRoom){
            Debug.Log("leaving room");
            PhotonNetwork.LeaveRoom();
        }else{
            Debug.Log("not in room");
        }
    }


    public void KillAllPlayers(){
        foreach(Transform player_transform in allPlayers){
            
            player_transform.GetComponent<PhotonView>().RPC("KillPlayer",RpcTarget.All);
            

        }
        
    }


    

    public Transform[] findAllSpawnableSpoints(){
        // Create a list to dynamically store spawnable points
        List<Transform> spawnableTransforms = new List<Transform>();

        // Loop through each spawn point
        foreach (Transform sp in spawnPoints)
        {
            // Check if the spawn point is spawnable
            
            if (sp.GetComponent<spawnpoint>()){
                if (sp.GetComponent<spawnpoint>().spawnable){
                    // Add the spawn point to the list
                    spawnableTransforms.Add(sp);
                }
            }else{
                spawnableTransforms.Add(sp);
            }
            
        }

        // Convert the list to an array and return it

        return spawnableTransforms.ToArray();
    }

    public void printAllAvalibleSpawnpoints(){
        Debug.Log("all avalible spawnpoints:");
        foreach (Transform sp in findAllSpawnableSpoints()){
            Debug.Log(sp);
        }
    }

    public void SpawnPlayer(){

        Transform spawnPoint = spawnPoints[0];

        if(findAllSpawnableSpoints().Length > 0){
            spawnPoint = findAllSpawnableSpoints()[UnityEngine.Random.Range(0, findAllSpawnableSpoints().Length)];
        }

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer(); 
        _player.GetComponent<Health>().IsLocalPlayer = true; 
        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PlayerPrefs.GetString("playerName", defaultname));


        //set local settings using PlayerSetup
        _player.GetComponent<PlayerSetup>().SetPlayerSens(playerSensX, playerSensY);
        _player.GetComponent<PlayerSetup>().SetCameraFov();

        _player.transform.SetParent(allPlayers);
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("playerName", defaultname);
        


        players.Add(_player);
    }

    
    public void SetHashes(){
        try{
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["kills"] = kills;
            hash["deaths"] = deaths;
            hash["score"] = score;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch{
            //nothing
        }
    }
}
