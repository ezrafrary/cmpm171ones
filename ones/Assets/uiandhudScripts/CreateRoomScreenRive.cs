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
            generateLobbyName();
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

    private void generateLobbyName(){
        roomList.ChangeRoomToCreateName(GenerateRandomString(5));
        
    }


    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] stringChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[Random.Range(0, chars.Length)];
        }

        return new string(stringChars);
    }

}
