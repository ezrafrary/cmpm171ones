using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class escapeMenu : MonoBehaviour
{

    public GameObject escapemenu;
    public MouseLook mouseLook;
    public WeaponSwitcher weaponSwitcher;
    public GameObject playerObj;

    private bool menuOpen = false;
    private bool buttonWasPressedLastFrame = false;


    private bool disableEscapeButton = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }




    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Esc")){
            if (!buttonWasPressedLastFrame){
                if(!disableEscapeButton){
                    if(menuOpen){
                        closeMenu();
                    }else{
                        openMenu();
                    }
                }
                
            }

            buttonWasPressedLastFrame = true;
        }else{
            buttonWasPressedLastFrame = false;
        }
        
    }


    public void SetDisableEscapeMenu(bool _input){
        disableEscapeButton = _input;
    }


    private void openMenu(){
        escapemenu.SetActive(true);
        weaponSwitcher.preventFire();
        menuOpen = true;
        mouseLook.UnlockCursor();
        mouseLook.LockPlayerMouseMovementRotation();
    }
    private void closeMenu(){
        escapemenu.SetActive(false);
        weaponSwitcher.allowFire();
        mouseLook.LockCursor();
        mouseLook.UnlockPlayerMouseMovementRotation();
        menuOpen = false;
    }


    public void quitButtonPressed(){
        Application.Quit();
    }
    
    public void backToMenuButtonPressed(){
        ResetScene();
    }
    public void ResetScene()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

}
