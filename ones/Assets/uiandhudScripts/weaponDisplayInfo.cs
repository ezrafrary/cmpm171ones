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


    public TextMeshProUGUI weaponInfoPannelDisplayNameText1;
    public TextMeshProUGUI weaponInfoPannelDisplayNameText2;
    public TextMeshProUGUI weaponInfoPannelDisplayNameText3;


    public Image weaponDisplayImage1;
    public Image weaponDisplayImage2;
    public Image weaponDisplayImage3;
    
    

    void Start(){
        updateAllSlotsText();
    }




    public void updateAllSlotsText(){
        setTextWithWeaponInfo(weaponInfoPannelDisplayNameText1, weaponDisplayText1, PlayerPrefs.GetInt("Slot1_weapon"));
        setTextWithWeaponInfo(weaponInfoPannelDisplayNameText2, weaponDisplayText2, PlayerPrefs.GetInt("Slot2_weapon"));
        setTextWithWeaponInfo(weaponInfoPannelDisplayNameText3, weaponDisplayText3, PlayerPrefs.GetInt("Slot3_weapon"));

        weaponDisplayNameText1.text = getWeaponInIndex(PlayerPrefs.GetInt("Slot1_weapon")).name;
        weaponDisplayNameText2.text = getWeaponInIndex(PlayerPrefs.GetInt("Slot2_weapon")).name;
        weaponDisplayNameText3.text = getWeaponInIndex(PlayerPrefs.GetInt("Slot3_weapon")).name;

        setWeaponImage(weaponDisplayImage1, PlayerPrefs.GetInt("Slot1_weapon"));
        setWeaponImage(weaponDisplayImage2, PlayerPrefs.GetInt("Slot2_weapon"));
        setWeaponImage(weaponDisplayImage3, PlayerPrefs.GetInt("Slot3_weapon"));
        
    }

    public void setTextWithWeaponInfo(TextMeshProUGUI _textmeshPannelName, TextMeshProUGUI _textmesh, int _slotIndex){
        Weapon tempWeapon = getWeaponInIndex(_slotIndex).GetComponent<Weapon>();
        _textmeshPannelName.text = tempWeapon.weaponFullName + "\n";

        _textmesh.text = "Damage: " + tempWeapon.damage + "\n";
        _textmesh.text += "Splash Damage: " +tempWeapon.splashDamage + "\n";
        _textmesh.text += "Fire Rate: " + tempWeapon.fireRate + "\n";
        _textmesh.text += "Velocity: " + tempWeapon.bulletSpeed + "\n";
        _textmesh.text += "Mag Capicity: " + tempWeapon.magAmmo + "\n";
        _textmesh.text += "Reload Time: " + tempWeapon.reloadTime + "\n";
        _textmesh.text += "Reserve Mags: " + tempWeapon.mag + "\n";
        _textmesh.text += "Pellets: " + tempWeapon.numPellets + "\n";
        _textmesh.text += "Spread: " + tempWeapon.spread + "\n";
    }

    public void setWeaponImage(Image image, int _slotIndex){
        Sprite imageToChangeTo = getWeaponInIndex(_slotIndex).GetComponent<Weapon>().weaponIconSprite;
        if(imageToChangeTo != null){
            image.sprite = imageToChangeTo;
        }
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
