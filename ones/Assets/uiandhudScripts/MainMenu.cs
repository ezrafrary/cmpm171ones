using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuObject;
    public OptionsMenu optionsMenuObj;
    public TMP_InputField nameInputField;
    private string defaultname = "test1";

    public GameObject test1;
    public InputActionAsset inputActionAsset;

   

    void Start(){
        //optionsMenuObj.loadSettings();
        loadName();
        SetDefaultWeapons();
        SceneManager.LoadScene(6, LoadSceneMode.Additive); //escapemenu index
        LoadAllBindingOverrides(inputActionAsset);
    }



    public static void LoadAllBindingOverrides(InputActionAsset inputActionAsset)
    {
        foreach (var map in inputActionAsset.actionMaps)
        {
            foreach (var action in map.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    string key = $"rebind_{map.name}_{action.name}_{i}";
                    if (PlayerPrefs.HasKey(key))
                    {
                        string overridePath = PlayerPrefs.GetString(key);
                        action.ApplyBindingOverride(i, overridePath);
                    }
                }
            }
        }
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
        if(PlayerPrefs.GetInt("Slot1_weapon") == 0){
            PlayerPrefs.SetInt("Slot1_weapon", 4); // m4
        }
        if(PlayerPrefs.GetInt("Slot2_weapon") == 0){
            PlayerPrefs.SetInt("Slot2_weapon", 1); // id for RPG
        }
        if(PlayerPrefs.GetInt("Slot3_weapon") == 0){
            PlayerPrefs.SetInt("Slot3_weapon", 2);// id for Pistol
        }
    }
}
