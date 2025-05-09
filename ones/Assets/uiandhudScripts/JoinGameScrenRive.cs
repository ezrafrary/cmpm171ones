using UnityEngine;
using Rive;
using Rive.Components;

public class JoinGameScrenRive : MonoBehaviour
{
    [SerializeField] private RiveWidget m_riveWidget;

    public GameObject createRoomScreen;
    public GameObject joinGameScreen;
    public GameObject lobby;
    public GameObject MainMenu;
    public RoomList roomList;


    private void OnEnable()
    {
        m_riveWidget.OnRiveEventReported += OnRiveEventReported;
    }


    private void OnRiveEventReported(ReportedEvent evt)
    {
        //Debug.Log($"Event received, name: \"{evt.Name}\", secondsDelay: {evt.SecondsDelay}");



        


        if(evt.Name.StartsWith("CreateRoom")){
            CreateRoomButtonPressed();
        }


        

        if(evt.Name.StartsWith("Back")){
            BackButtonPressed();
        }
        if(evt.Name.StartsWith("Refresh")){
            roomList.refreshRoomList();
        }

    }


    private void CreateRoomButtonPressed(){
        createRoomScreen.SetActive(true);
        joinGameScreen.SetActive(false);
    }

    private void BackButtonPressed(){
        lobby.SetActive(false);
        MainMenu.SetActive(true);
    }

    


    private void OnDisable()
    {
        m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
    }

}
