using UnityEngine;
using Photon.Pun;

public class PhotonTimer : MonoBehaviourPunCallbacks
{
    private float startTime; // The time when the timer starts
    public float timer = 5; // The timer value (in seconds)
    public bool isTimerRunning = true; // Whether the timer is running or not
    public float timerDuration = 60f; // The total duration for the timer (in seconds)
    public RoomManager roomManager;
    private bool hasTimerRanBefore = false;
    public Leaderboard endgameLeaderboard;
    public float prevousSyncedTime = 300f;


    void Start()
    {
        //StartTimer();
    }

    void Update()
    {
        if(hasTimerRanBefore == true){
            
            if (isTimerRunning)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    // Update the timer for the MasterClient only
                    timer = timerDuration - (Time.time - startTime);
                    if (timer <= 0f)
                    {
                        isTimerRunning = false;
                        timer = 0f; // Prevent timer from going negative
                        OnTimerEnd(); // Call timer end action (e.g., end game, etc.)
                    }
                    
                    // Synchronize the timer value across the network
                    if(photonView && PhotonNetwork.InRoom){
                        photonView.RPC("SyncTimer", RpcTarget.Others, timer);
                    }
                }else{
                    //for players who are not hosting server
                    if (timer <= 0f)
                    {
                        isTimerRunning = false;
                        timer = 0f; // Prevent timer from going negative
                        OnTimerEnd(); // Call timer end action (e.g., end game, etc.)
                    }
                }
                if(timer <= 2f && timer >= 0f){
                    SyncEndgameLeaderboard();
                }
            }
        }
    }





    public void SyncEndgameLeaderboard(){
        endgameLeaderboard.Refresh();
        endgameLeaderboard.updateLeaderboard = false;
    }

    


    // This RPC is called on all clients to sync the timer
    [PunRPC]
    void SyncTimer(float syncedTime)
    {
        if (!PhotonNetwork.IsMasterClient) // Only update the timer for non-MasterClients
        {
            timer = syncedTime;
        }
    }

    // Action to perform when the timer reaches 0
    private void OnTimerEnd()
    {
        Debug.Log("Timer has ended!");
        if (PhotonNetwork.IsMasterClient)
        {
            
            AnalyticsManager.Instance.GameCompleted();
            photonView.RPC("EndGameRPC", RpcTarget.All);
        }
    }
    [PunRPC]
    void EndGameRPC()
    {
        roomManager.EndGame();
    }
        


    // Function to start the timer manually
    public void StartTimer()
    {
        hasTimerRanBefore = true;
        if (PhotonNetwork.IsMasterClient)
        {
            startTime = Time.time;
            isTimerRunning = true;
        }
    }
}