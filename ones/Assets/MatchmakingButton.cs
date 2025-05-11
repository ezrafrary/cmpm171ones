using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class MatchmakingButton : MonoBehaviourPunCallbacks
{
    private bool isTryingToJoin = false;
    public RoomList roomList;

    public void OnMatchmakingButtonPressed()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Photon is not connected!");
            return;
        }

        isTryingToJoin = true;
        PhotonNetwork.JoinLobby(); // Triggers OnRoomListUpdate
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!isTryingToJoin) return;

        foreach (RoomInfo room in roomList)
        {
            if (!room.RemovedFromList && room.Name.Length > 16 && room.PlayerCount == 1)
            {
                
                int roomSceneIndex = 1;

                object sceneIndexObject;
                if(room.CustomProperties.TryGetValue("mapSceneIndex", out sceneIndexObject)){
                    roomSceneIndex = (int)sceneIndexObject;
                }


                RoomList.Instance.JoinRoomByName(room.Name, roomSceneIndex);

                isTryingToJoin = false;
                return;
            }
        }
        
        // If no suitable room found, create one
        string newRoomName = "MMLobby_________" + System.Guid.NewGuid().ToString("N").Substring(0, 17);
        
        RoomList.Instance.JoinRoomByName(newRoomName, 4);


        Debug.Log("Created new room: " + newRoomName);
        isTryingToJoin = false;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        // Load gameplay scene or handle room UI here
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to join room: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to create room: " + message);
    }
}
