using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuObject;
    public OptionsMenu optionsMenuObj;
    public TMP_InputField nameInputField;
    private string defaultname = "test1";


    void Start(){
        optionsMenuObj.loadSettings();
        loadName();
        SetDefaultWeapons();
        SceneManager.LoadScene(6, LoadSceneMode.Additive); //escapemenu index
    }


    public void saveName(){
        PlayerPrefs.SetString("playerName", nameInputField.text);
    }

    public void loadName(){
        nameInputField.text = PlayerPrefs.GetString("playerName", defaultname);
    }

    public void PlayButtonClicked(){
        mainMenuObject.SetActive(false);
    }

    public void SetDefaultWeapons(){

        if(PlayerPrefs.GetInt("Slot2_weapon") == 0){
            PlayerPrefs.SetInt("Slot2_weapon", 1); // id for RPG
        }
        if(PlayerPrefs.GetInt("Slot3_weapon") == 0){
            PlayerPrefs.SetInt("Slot3_weapon", 2);// id for Pistol
        }
    }
}
