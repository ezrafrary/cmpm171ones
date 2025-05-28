using UnityEngine;
using Rive;
using Rive.Components;
using UnityEngine.UI;
using TMPro;

public class JoinGameScrenRive : MonoBehaviour
{
    [SerializeField] private RiveWidget m_riveWidget;

    public GameObject createRoomScreen;
    public GameObject joinGameScreen;
    public GameObject lobby;
    public GameObject MainMenu;
    public RoomList roomList;

    public GameObject roomNameDoesNotExistPopup;
    public float roomNameDoesNotExistPopupCoolDown = 2f;
    private float _roomNameDoesNotExistPopupCoolDown = 0f;

    public TMP_InputField roomNameField;



    void Update(){
        if(_roomNameDoesNotExistPopupCoolDown > 0){
            _roomNameDoesNotExistPopupCoolDown -= Time.deltaTime;
            roomNameDoesNotExistPopup.SetActive(true);
        }else{
            roomNameDoesNotExistPopup.SetActive(false);
        }
    }


    private void OnEnable()
    {
        m_riveWidget.OnRiveEventReported += OnRiveEventReported;
    }


    private void OnRiveEventReported(ReportedEvent evt)
    {
        //Debug.Log($"Event received, name: \"{evt.Name}\", secondsDelay: {evt.SecondsDelay}");



        
        if(evt.Name.StartsWith("JoinRoom")){
            JoinRoomButtonPressed();
        }

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

    private void JoinRoomButtonPressed(){
        string inputText = roomNameField.text;
        inputText = inputText.ToUpper();
        //Debug.Log("trying to join room: " + inputText);


        


        int retrunCode = RoomList.Instance.JoinRoomByName(inputText, RoomList.Instance.getMapSceneIndex(inputText));
        if(retrunCode == -1){
            _roomNameDoesNotExistPopupCoolDown = roomNameDoesNotExistPopupCoolDown;
            //Debug.Log("roomDoesntExist");
        }
    }


    private void OnDisable()
    {
        m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
    }

}
