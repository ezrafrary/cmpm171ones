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


        // Access specific event properties
        if (evt.Name.StartsWith("rating"))
        {

            // Access properties directly and cast them
            if (evt.Properties.TryGetValue("rating", out object rating))
            {
                float ratingValue = (float)rating;
                Debug.Log($"Rating: {ratingValue}");
            }

            if (evt.Properties.TryGetValue("message", out object message))
            {
                string messageValue = message as string;
                Debug.Log($"Message: {messageValue}");
            }

        }
    }

    private void OnDisable()
    {
        m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
    }
}
