using UnityEngine;
using Rive;
using Rive.Components;


public class CreateRoomScreenRive : MonoBehaviour
{
    [SerializeField] private RiveWidget m_riveWidget;
    public GameObject MainMenu;
    public GameObject lobby;


    
    public RoomList roomList;

    private void OnEnable()
    {
        m_riveWidget.OnRiveEventReported += OnRiveEventReported;
    }


    private void OnRiveEventReported(ReportedEvent evt)
    {
        //Debug.Log($"Event received, name: \"{evt.Name}\", secondsDelay: {evt.SecondsDelay}");



        


        if(evt.Name.StartsWith("CreateGame")){
            roomList.CreateRoomByIndex();
        }


        

        if(evt.Name.StartsWith("CaveClicked")){
            roomList.changeRoomJoinIndex(3);
        }
        if(evt.Name.StartsWith("WarehouseClicked")){
            roomList.changeRoomJoinIndex(4);
        }
        if(evt.Name.StartsWith("AquaticClicked")){
            roomList.changeRoomJoinIndex(7);
        }


    }
    


    private void OnDisable()
    {
        m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
    }

    public void BackButtonPressed(){
        lobby.SetActive(false);
        MainMenu.SetActive(true);
    }


}
