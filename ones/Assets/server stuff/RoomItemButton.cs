using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    public string RoomName;
    public int SceneIndex = 1;

    public void OnButtonPressed(){
        RoomList.Instance.JoinRoomByName(RoomName, SceneIndex);
    }
}
