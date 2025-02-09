using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class weaponDisplayInfo : MonoBehaviour
{
    public GameObject PlayerObj;
    public TextMeshProUGUI weaponDisplayText1;
    public TextMeshProUGUI weaponDisplayText2;
    public TextMeshProUGUI weaponDisplayText3;

    public TextMeshProUGUI weaponDisplayNameText1;
    public TextMeshProUGUI weaponDisplayNameText2;
    public TextMeshProUGUI weaponDisplayNameText3;
    

    void Start(){
        updateAllSlotsText();
    }




    public void updateAllSlotsText(){
        setTextWithWeaponInfo(weaponDisplayText1, PlayerPrefs.GetInt("Slot1_weapon"));
        setTextWithWeaponInfo(weaponDisplayText2, PlayerPrefs.GetInt("Slot2_weapon"));
        setTextWithWeaponInfo(weaponDisplayText3, PlayerPrefs.GetInt("Slot3_weapon"));

        weaponDisplayNameText1.text = getWeaponInIndex(PlayerPrefs.GetInt("Slot1_weapon")).name;
        weaponDisplayNameText2.text = getWeaponInIndex(PlayerPrefs.GetInt("Slot2_weapon")).name;
        weaponDisplayNameText3.text = getWeaponInIndex(PlayerPrefs.GetInt("Slot3_weapon")).name;

    }

    public void setTextWithWeaponInfo(TextMeshProUGUI _textmesh, int _slotIndex){
        Weapon tempWeapon = getWeaponInIndex(_slotIndex).GetComponent<Weapon>();
        _textmesh.text = "Damage: " + tempWeapon.damage + "\n";
        _textmesh.text += "Fire Rate: " + tempWeapon.fireRate + "\n";
        _textmesh.text += "Velocity: " + tempWeapon.bulletSpeed + "\n";
        _textmesh.text += "Mag Capicity: " + tempWeapon.magAmmo + "\n";
        _textmesh.text += "Reserve Mags: " + tempWeapon.mag + "\n";
    }

    public GameObject getWeaponInIndex(int slotIndex){
        //Debug.Log(PlayerObj.GetComponent<PlayerSetup>().weaponSwitcher);
        int i = 0;
        foreach(Transform _weapon in PlayerObj.GetComponent<PlayerSetup>().weaponSwitcher.transform){
            if(i == slotIndex){
                return _weapon.gameObject;
            }
            i++;
        }
        return null;

    }

}
