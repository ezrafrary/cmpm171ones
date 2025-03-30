using UnityEngine;

public class PermenantEscapeMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("UI")]
    public GameObject escapeMenuRivePannel;
    public bool disableEscapeMenuAtTheStart = true;

    [Header("Controls")]
    public bool menuOpen;


    [Header("Mouse Settings")]
    public MouseLook mouseLook;
    public bool disableEscapeButton;

    private bool buttonWasPressedLastFrame = false;


    void Start()
    {
        // if(disableEscapeMenuAtTheStart){
        //     closeMenu();
        // }
        setMouselook();
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
}
