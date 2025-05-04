using UnityEngine;
using Rive;
using Rive.Components;

public class OptionsMenuPageChanger : MonoBehaviour
{
    [SerializeField] private RiveWidget m_riveWidget;


    public GameObject gameplayOptionsMenu;
    public GameObject keybindsOptionsMenu;
    public GameObject videoOptionsMenu;
    public GameObject audioOptionsMenu;



    private void OnEnable()
    {
        m_riveWidget.OnRiveEventReported += OnRiveEventReported;
    }



    private void DisableAllMenus(){
        if(gameplayOptionsMenu){
            gameplayOptionsMenu.SetActive(false);
        }
        if(keybindsOptionsMenu){
            keybindsOptionsMenu.SetActive(false);
        }
        if(videoOptionsMenu){
            videoOptionsMenu.SetActive(false);
        }
        if(audioOptionsMenu){
            audioOptionsMenu.SetActive(false);
        }
    }

    private void OnRiveEventReported(ReportedEvent evt)
    {
        
        if(evt.Name.StartsWith("GameplayClicked")){
            DisableAllMenus();
            gameplayOptionsMenu.SetActive(true);
        }

        if(evt.Name.StartsWith("KeybindClicked")){
            DisableAllMenus();
            keybindsOptionsMenu.SetActive(true);
        }

        if(evt.Name.StartsWith("VideoClicked")){
            DisableAllMenus();
            videoOptionsMenu.SetActive(true);
        }

        if(evt.Name.StartsWith("AudioClicked")){
            DisableAllMenus();
            audioOptionsMenu.SetActive(true);
        }
    }

    private void OnDisable()
    {
        m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
    }


}
