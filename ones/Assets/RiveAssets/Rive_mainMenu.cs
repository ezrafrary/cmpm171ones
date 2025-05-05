using System;
using Rive;
using Rive.Components;
using UnityEngine;


public class Rive_mainMenu : MonoBehaviour
{
    [SerializeField] private RiveWidget m_riveWidget;


    [Header("Play Button")]
    public GameObject MainMenu;
    public GameObject LobbyGameobject;

    [Header("LoadoutButton")]
    public GameObject MenuCanvas;
    public GameObject LoadoutMenuGameObject;

    [Header("SettingsIcon")]
    public GameObject SettingsMenuGameobject;

    [Header("roomList")]
    public RoomList roomList;



    private void OnEnable()
    {
        m_riveWidget.OnRiveEventReported += OnRiveEventReported;
    }

    private void OnRiveEventReported(ReportedEvent evt)
    {
        //Debug.Log($"Event received, name: \"{evt.Name}\", secondsDelay: {evt.SecondsDelay}");



        


        if(evt.Name.StartsWith("PlayButtonClicked")){
            MainMenu.SetActive(false);
            LobbyGameobject.SetActive(true);
        }


        if(evt.Name.StartsWith("LoadoutButtonClicked")){
            MenuCanvas.SetActive(false);
            LoadoutMenuGameObject.SetActive(true);
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
