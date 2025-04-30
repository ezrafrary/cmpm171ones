using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class loadoutScreen : MonoBehaviour
{

    public Transform[] equipmentPannels;



    void Start(){
        setupLoadoutScreen();
    }

    public void setupLoadoutScreen(){
        int i = 0;
        foreach(Transform _equipmentPannel in equipmentPannels){
            
            _equipmentPannel.gameObject.SetActive(false);

            if(i == PlayerPrefs.GetInt("Slot1_weapon", 4)){
                _equipmentPannel.gameObject.SetActive(true);
            }
            if(i == PlayerPrefs.GetInt("Slot2_weapon")){
                _equipmentPannel.gameObject.SetActive(true);
            }
            if(i == PlayerPrefs.GetInt("Slot3_weapon")){
                _equipmentPannel.gameObject.SetActive(true);
            }

            i++;
        }
    }

    public void saveSlot1(int _weaponIndex){
        PlayerPrefs.SetInt("Slot1_weapon", _weaponIndex);
    }
    public void saveSlot2(int _weaponIndex){
        PlayerPrefs.SetInt("Slot2_weapon", _weaponIndex);
    }
    public void saveSlot3(int _weaponIndex){
        PlayerPrefs.SetInt("Slot3_weapon", _weaponIndex);
    }
}
