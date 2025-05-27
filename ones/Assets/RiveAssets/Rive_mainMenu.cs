using System;
using Rive;
using Rive.Components;
using UnityEngine;


public class Rive_mainMenu : MonoBehaviour
{
    [SerializeField] private RiveWidget m_riveWidget;


    [Header("Host Button")]
    public GameObject MainMenu;
    public GameObject LobbyGameobject;
    public GameObject JoinGameScreenObject;
    public GameObject CreateRoomScreenObject;

    [Header("LoadoutButton")]
    public GameObject MenuCanvas;
    public GameObject LoadoutMenuGameObject;

    [Header("SettingsIcon")]
    public GameObject SettingsMenuGameobject;

    [Header("roomList")]
    public RoomList roomList;
    public MatchmakingButton mathcmakingButton;


    private void OnEnable()
    {
        m_riveWidget.OnRiveEventReported += OnRiveEventReported;
    }

    private void OnRiveEventReported(ReportedEvent evt)
    {
        //Debug.Log($"Event received, name: \"{evt.Name}\", secondsDelay: {evt.SecondsDelay}");



        if(evt.Name.StartsWith("JoinClicked")){
            MainMenu.SetActive(false);
            LobbyGameobject.SetActive(true);
            JoinGameScreenObject.SetActive(true);
            CreateRoomScreenObject.SetActive(false);
        }


        if(evt.Name.StartsWith("HostClicked")){
            MainMenu.SetActive(false);
            LobbyGameobject.SetActive(true);
            JoinGameScreenObject.SetActive(false);
            CreateRoomScreenObject.SetActive(true);
        }


        if(evt.Name.StartsWith("PlayButtonClicked")){
            mathcmakingButton.OnMatchmakingButtonPressed();
        }
        


        if (evt.Name.StartsWith("LoadoutButtonClicked"))
        {
            // MenuCanvas.SetActive(false);
            // LoadoutMenuGameObject.SetActive(true);
            PermenantEscapeMenu.Instance.openLoadoutMenu(true);
        }

        if(evt.Name.StartsWith("TutorialClicked")){
            Debug.Log("start tutorial");
            roomList.JoinOfflineTutorialRoom();
        }

        if(evt.Name.StartsWith("SettingsButtonClicked")){
            // MenuCanvas.SetActive(false);
            // SettingsMenuGameobject.SetActive(true);
            PermenantEscapeMenu.Instance.openOptionsMenu(true);
        }

    }

    private void OnDisable()
    {
        m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
    }
}
