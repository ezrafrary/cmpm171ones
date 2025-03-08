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



    [Header("places to load the settings")]
    public MouseLook mouseLook;
    public PlayerSetup playerSetup;

    private float defaultSens = 2f;
    private float defaultFov = 60;
    private float defaultColor = 200f;





    //it is awkward to change settings on player, use player prefs 

    void Start(){
        loadSettings();
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


    public void colorSliderChanged(){
        colorInputField.text = colorSlider.value.ToString();
        colorInputFieldChanged();
    }



    private Color getSavedColor(){
        float colornumber = PlayerPrefs.GetFloat("CrosshairColor", defaultColor)/360;
        Color returnColor = Color.HSVToRGB(colornumber, 1, 1);
        return returnColor;
    }

    public void colorInputFieldChanged(){
        float colornumber = float.Parse(colorInputField.text)/360;
        Color newcolor = Color.HSVToRGB(colornumber, 1, 1);
        
        crosshair.color = newcolor;
        testcrosshair.color = newcolor;
    }


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
        PlayerPrefs.SetFloat("CrosshairColor", colorSlider.value);


        PlayerPrefs.SetFloat("EnemyOutline_Red", enemyOutlineColorPicker.getRed());
        PlayerPrefs.SetFloat("EnemyOutline_Green", enemyOutlineColorPicker.getGreen());
        PlayerPrefs.SetFloat("EnemyOutline_Blue", enemyOutlineColorPicker.getBlue());
        

        loadSettings();
    }
    
    public void loadSettings(){
        sensSlider.value = PlayerPrefs.GetFloat("SensXY", defaultSens);
        fovSlider.value = PlayerPrefs.GetInt("FOV", (int)defaultFov);
        colorSlider.value = PlayerPrefs.GetFloat("CrosshairColor", defaultColor);
        crosshair.color = getSavedColor();
        if(mouseLook){
            mouseLook.loadSettings();
        }else{
            //Debug.Log("nomouselook");
        }
        if(playerSetup){
            playerSetup.SetCameraFov();
        }

        enemyOutlineColorPicker.setColor(PlayerPrefs.GetFloat("EnemyOutline_Red", defaultColor), PlayerPrefs.GetFloat("EnemyOutline_Green", defaultColor), PlayerPrefs.GetFloat("EnemyOutline_Blue", defaultColor));

    }


    public void saveSettingsButtonPressed(){
        saveSettings();
    }

    public void loadSettingsButtonPressed(){
        loadSettings();
    }
}
