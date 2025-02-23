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

    private float defaultSens = 2f;
    private float defaultFov = 60;


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
    public void colorInputFieldChanged(){
        //int colornumber = (int)(float.Parse(colorInputField.text)*16777215);
        float colornumber = float.Parse(colorInputField.text)/360;
        Color newcolor = Color.HSVToRGB(colornumber, 1, 1);
        /*float red = colornumber>>16;
        float green = (colornumber>>8)&0b000000000000000011111111;
        float blue =(colornumber)&0b000000000000000011111111;*/
        //Debug.Log("red: " + red + " green: " + green + " blue: " + blue);
        crosshair.color = newcolor;
        testcrosshair.color = newcolor;
        //crosshair.color = new Color(int.Parse(colorInputField.text)>>16, (int.Parse(colorInputField.text)>>8)&000000000000000011111111, int.Parse(colorInputField.text)&000000000000000011111111);
    }
    public void fovinputFieldChanged(){
        try{
            fovSlider.value = float.Parse(fovInputField.text);
        }catch{
            fovInputField.text = "";
            fovSlider.value = defaultFov;
        }
    }
    public void updateFovSlider(){

    }
    public void updateSensSlider(){

    }
    public void updateColorSlider(){

    }

    public void saveSettings(){
        PlayerPrefs.SetFloat("SensXY", sensSlider.value);
        PlayerPrefs.SetInt("FOV", (int)fovSlider.value);
    }
    
    public void loadSettings(){
        sensSlider.value = PlayerPrefs.GetFloat("SensXY", defaultSens);
        fovSlider.value = PlayerPrefs.GetInt("FOV", (int)defaultFov);
    }


    public void saveSettingsButtonPressed(){
        saveSettings();
    }

    public void loadSettingsButtonPressed(){
        loadSettings();
    }
}
