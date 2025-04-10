using UnityEngine;
using Rive;
using Rive.Components;
using System;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class PermenantEscapeMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Rive")]
    [SerializeField] private RiveWidget m_riveWidget;



    [Header("UI")]
    public GameObject escapeMenuRivePannel;
    public bool disableEscapeMenuAtTheStart = true;

    [Header("Controls")]
    public bool menuOpen;


    [Header("Mouse Settings")]
    public MouseLook mouseLook;
    public bool disableEscapeButton;
    
    
    [Header("OptionsMenu")]
    public GameObject optionsMenuCanvas;
    public OptionsMenu escapeMenu_optionsMenu;


    [Header("LoadoutMenu")]
    public GameObject loadoutMenu;

    //escape button
    private bool buttonWasPressedLastFrame = false;
    private bool optionsMenuOpen = false;
    private bool loadoutMenuOpen = false;
    

    void Start()
    {
        setMouselook();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Esc")){
            if (!buttonWasPressedLastFrame){
                if(!disableEscapeButton){
                    if(!optionsMenuOpen){
                        if(!loadoutMenuOpen){
                            if(menuOpen){
                                closeMenu();
                            }else{
                                openMenu();
                            }
                        }else{
                            closeLoadoutMenu();
                        }
                    }else{
                        closeOptionsMenu();
                    }
                }
            }

            buttonWasPressedLastFrame = true;
        }else{
            buttonWasPressedLastFrame = false;
        }
    }



    public void testButton(){
        Debug.Log("ButtonPressed");
    }


    public void setMouselook(){
        MouseLook tempmouseLook = FindFirstObjectByType<MouseLook>();
        mouseLook = tempmouseLook;
    }


    public void openMenu(){
        setMouselook();
        escapeMenuRivePannel.SetActive(true);
        menuOpen = true;
        if(mouseLook){
            mouseLook.UnlockCursor();
            mouseLook.LockPlayerMouseMovementRotation();
        }
    }


    public void closeMenu(){
        setMouselook();
        menuOpen = false;
        escapeMenuRivePannel.SetActive(false);
        if(mouseLook){
            mouseLook.LockCursor();
            mouseLook.UnlockPlayerMouseMovementRotation();
        }
    }




    public void openLoadoutMenu(){
        closeMenu();
        if(mouseLook){
            mouseLook.UnlockCursor();
            mouseLook.LockPlayerMouseMovementRotation();
        }
        loadoutMenu.SetActive(true);
        loadoutMenuOpen = true;
    }

    public void closeLoadoutMenu(){
        loadoutMenu.SetActive(false);
        openMenu();
        loadoutMenuOpen = false;
    }


    public void openOptionsMenu(){
        closeMenu();
        optionsMenuOpen = true;
        if(mouseLook){
            mouseLook.UnlockCursor();
            mouseLook.LockPlayerMouseMovementRotation();
        }
        optionsMenuCanvas.SetActive(true);
    }




    public void closeOptionsMenu(){
        escapeMenu_optionsMenu.saveSettingsButtonPressed();
        optionsMenuCanvas.SetActive(false);
        openMenu();
        optionsMenuOpen = false;
    }



    private void OnEnable()
    {
        m_riveWidget.OnRiveEventReported += OnRiveEventReported;
    }

    private void OnRiveEventReported(ReportedEvent evt)
    {
        //Debug.Log($"Event received, name: \"{evt.Name}\", secondsDelay: {evt.SecondsDelay}");

        if(evt.Name.StartsWith("BackToGameClicked")){
            closeMenu();
        }


        if(evt.Name.StartsWith("QuitClicked")){
            Application.Quit();
        }

        if(evt.Name.StartsWith("OptionsClicked")){
            openOptionsMenu();
        }

        if(evt.Name.StartsWith("MenuClicked")){
            if(PhotonNetwork.InRoom){
                PhotonNetwork.LeaveRoom();
            }
            SceneManager.LoadScene(0);
        }

        if(evt.Name.StartsWith("LoadoutClicked")){
            openLoadoutMenu();
        }

    }

    private void OnDisable()
    {
        m_riveWidget.OnRiveEventReported -= OnRiveEventReported;
    }





}
