using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;   

public class OptionsMenu : MonoBehaviour
{
    public Slider sensSlider;
    public TMP_InputField sensInputField;
    public TMP_InputField fovInputField;
    public Slider fovSlider;
    public Slider colorSlider;
    public TMP_InputField colorInputField;
    public Image crosshair;
    public Image testcrosshair;
    public ColorPicker enemyOutlineColorPicker;
    public ColorPicker crosshairColorPicker;
    public TMP_InputField masterVolumeInputField;
    public Slider masterVolumeSlider;


    [Header("places to load the settings")]
    public MouseLook mouseLook;
    public PlayerSetup playerSetup;

    private float defaultSens = 2f;
    private float defaultFov = 60;
    private float defaultColor = 255f;
    private int defaultVolume = 100;




    //it is awkward to change settings on player, use player prefs 

    void Start(){
        loadSettings();
        gameObject.SetActive(false);
    }


    public void toggleFPSCounter(){
        if(PlayerPrefs.GetInt("PlayerFPSCounterSettings", 0) == 0){
            PlayerPrefs.SetInt("PlayerFPSCounterSettings", 1);
        }else{
            PlayerPrefs.SetInt("PlayerFPSCounterSettings", 0);
        }
        PermenantEscapeMenu.Instance.setFPSCounter();
    }

    public void toggleTwitchChatVisibliity(){
        PermenantEscapeMenu.Instance.toggleTwitchChatVisibliity();
    }




    public void masterVolumeSliderChanged(){
        masterVolumeInputField.text = masterVolumeSlider.value.ToString();
    }

    public void masterVolumeInputFieldChanged(){
        try{
            masterVolumeSlider.value = float.Parse(masterVolumeInputField.text);
        }catch{
            masterVolumeInputField.text = "100";
            masterVolumeSlider.value = defaultVolume;
        }
    }

    public void sensSliderChanged(){
        sensInputField.text = sensSlider.value.ToString();
    }

    public void sensInputFieldChanged(){
        try{
            sensSlider.value = float.Parse(sensInputField.text);
        }catch{
            sensInputField.text = "";
            sensSlider.value = defaultSens;
        }
    }

    public void fovSliderChanged(){
        fovInputField.text = fovSlider.value.ToString();
    }


    // public void colorSliderChanged(){
    //     colorInputField.text = colorSlider.value.ToString();
    //     colorInputFieldChanged();
    // }





    private Color getSavedColor(){
        Color returnColor = new Color(PlayerPrefs.GetFloat("CrosshairColor_Red", defaultColor)/255f, PlayerPrefs.GetFloat("CrosshairColor_Green", defaultColor)/255f, PlayerPrefs.GetFloat("CrosshairColor_Blue", defaultColor)/255f);
        return returnColor;
    }


    public void updateCrosshair(){
        if(crosshair){
            crosshair.color = getSavedColor();
        }
    }

    // public void colorInputFieldChanged(){
    //     float colornumber = float.Parse(colorInputField.text)/360;
    //     Color newcolor = Color.HSVToRGB(colornumber, 1, 1);
        
    //     crosshair.color = newcolor;
    //     testcrosshair.color = newcolor;
    // }


    public void fovinputFieldChanged(){
        try{
            fovSlider.value = float.Parse(fovInputField.text);
        }catch{
            fovInputField.text = "";
            fovSlider.value = defaultFov;
        }
        if(playerSetup){
            playerSetup.SetCameraFov();
        }
    }
    

    public void saveSettings(){
        PlayerPrefs.SetFloat("SensXY", sensSlider.value);
        PlayerPrefs.SetInt("FOV", (int)fovSlider.value);


        PlayerPrefs.SetFloat("EnemyOutline_Red", enemyOutlineColorPicker.getRed());
        PlayerPrefs.SetFloat("EnemyOutline_Green", enemyOutlineColorPicker.getGreen());
        PlayerPrefs.SetFloat("EnemyOutline_Blue", enemyOutlineColorPicker.getBlue());
        
        PlayerPrefs.SetFloat("CrosshairColor_Red", crosshairColorPicker.getRed());
        PlayerPrefs.SetFloat("CrosshairColor_Green", crosshairColorPicker.getGreen());
        PlayerPrefs.SetFloat("CrosshairColor_Blue", crosshairColorPicker.getBlue());

        PlayerPrefs.SetInt("MasterVolume", (int)masterVolumeSlider.value);

        loadSettings();
    }
    
    public void loadSettings(){
        sensSlider.value = PlayerPrefs.GetFloat("SensXY", defaultSens);
        fovSlider.value = PlayerPrefs.GetInt("FOV", (int)defaultFov);
        
        masterVolumeSlider.value = PlayerPrefs.GetInt("MasterVolume", defaultVolume);

        if(crosshair){
            crosshair.color = getSavedColor();
        }
        if(mouseLook){
            mouseLook.loadSettings();
        }else{
            //Debug.Log("nomouselook");
        }
        if(playerSetup){
            playerSetup.SetCameraFov();
        }

        enemyOutlineColorPicker.setColor(PlayerPrefs.GetFloat("EnemyOutline_Red", defaultColor), PlayerPrefs.GetFloat("EnemyOutline_Green", defaultColor), PlayerPrefs.GetFloat("EnemyOutline_Blue", defaultColor));
        crosshairColorPicker.setColor(PlayerPrefs.GetFloat("CrosshairColor_Red", defaultColor), PlayerPrefs.GetFloat("CrosshairColor_Green", defaultColor), PlayerPrefs.GetFloat("CrosshairColor_Blue", defaultColor));
    }

    //---------------------------------------------------
    //WARNING OF DEATH: DO NOT CALL THIS INSIDE OF saveSettingsButtonPressed, recursive loop with no exit condition that will crash unity. you have been warned
    //----------------------------------------------------
    public void permenantEscapeCloseEscapeMenu(){
        PermenantEscapeMenu.Instance.closeOptionsMenu();
    }

    public void saveSettingsButtonPressed(){
        saveSettings();
    }

    public void loadSettingsButtonPressed(){
        loadSettings();
    }
}
