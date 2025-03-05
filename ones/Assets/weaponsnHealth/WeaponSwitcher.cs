using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    public new Animation animation;
    public AnimationClip draw;

    public int selectedWeapon = 0;

    private bool lockWeaponSwitch = false;

    public PhotonView playerSetupView;


    [Header("playerequipmentui")]
    public TextMeshProUGUI equipmentSlot1Text;
    public RectTransform equipmentSlot1RectTransform;
    public TextMeshProUGUI equipmentSlot2Text;
    public RectTransform equipmentSlot2RectTransform;
    public TextMeshProUGUI equipmentSlot3Text;
    public RectTransform equipmentSlot3RectTransform;



    private Dictionary<int, Quaternion> weaponInitialRotations = new Dictionary<int, Quaternion>();

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {


        /*if(isCurrentWeaponReloading()){
            lockWeaponSwitch = true;
        }else{
            lockWeaponSwitch = false;
        }*/

        if(!lockWeaponSwitch){
            int previousSelectedWeapon = selectedWeapon;
            if (Input.GetKeyDown(KeyCode.Alpha1)){
                selectedWeapon = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)){
                selectedWeapon = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)){
                selectedWeapon = 2;
            }


            if (Input.GetAxis("Mouse ScrollWheel") > 0){
                if (selectedWeapon >= 2){
                    selectedWeapon = 0;
                }else {
                    selectedWeapon += 1;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0){
                if (selectedWeapon <= 0){
                    selectedWeapon = 2;
                }else {
                    selectedWeapon -= 1;
                }
            }

            if (previousSelectedWeapon != selectedWeapon){
                SelectWeapon();
            }
        }
    }

    public void setWeaponUiText(){

        int i = 0;
        foreach(Transform _weapon in transform){
            if(i == PlayerPrefs.GetInt("Slot1_weapon")){
                equipmentSlot1Text.text = _weapon.gameObject.name;
            }
            if(i == PlayerPrefs.GetInt("Slot2_weapon")){
                equipmentSlot2Text.text = _weapon.gameObject.name;
            }
            if(i == PlayerPrefs.GetInt("Slot3_weapon")){
                equipmentSlot3Text.text = _weapon.gameObject.name;
            }
            i++;
        }

    }

    public void setWeaponUiTextSize(){
        float unequippedFontSize = 20f;
        float equippedFontSize = 46f;

        float unequippedRectSize = 20f;
        float equippedRectSize = 40f;

        if(selectedWeapon == 0){
            
            equipmentSlot1Text.fontSize = equippedFontSize;
            equipmentSlot2Text.fontSize = unequippedFontSize;
            equipmentSlot3Text.fontSize = unequippedFontSize;

            equipmentSlot1RectTransform.sizeDelta = new Vector2(equipmentSlot1RectTransform.sizeDelta.x, equippedRectSize);
            equipmentSlot2RectTransform.sizeDelta = new Vector2(equipmentSlot2RectTransform.sizeDelta.x, unequippedRectSize);
            equipmentSlot3RectTransform.sizeDelta = new Vector2(equipmentSlot3RectTransform.sizeDelta.x, unequippedRectSize);
        }
        if(selectedWeapon == 1){
            equipmentSlot1Text.fontSize = unequippedFontSize;
            equipmentSlot2Text.fontSize = equippedFontSize;
            equipmentSlot3Text.fontSize = unequippedFontSize;
            
            equipmentSlot1RectTransform.sizeDelta = new Vector2(equipmentSlot1RectTransform.sizeDelta.x, unequippedRectSize);
            equipmentSlot2RectTransform.sizeDelta = new Vector2(equipmentSlot2RectTransform.sizeDelta.x, equippedRectSize);
            equipmentSlot3RectTransform.sizeDelta = new Vector2(equipmentSlot3RectTransform.sizeDelta.x, unequippedRectSize);
        }
        if(selectedWeapon == 2){
            equipmentSlot1Text.fontSize = unequippedFontSize;
            equipmentSlot2Text.fontSize = unequippedFontSize;
            equipmentSlot3Text.fontSize = equippedFontSize;

            equipmentSlot1RectTransform.sizeDelta = new Vector2(equipmentSlot1RectTransform.sizeDelta.x, unequippedRectSize);
            equipmentSlot2RectTransform.sizeDelta = new Vector2(equipmentSlot2RectTransform.sizeDelta.x, unequippedRectSize);
            equipmentSlot3RectTransform.sizeDelta = new Vector2(equipmentSlot3RectTransform.sizeDelta.x, equippedRectSize);
        }
    }

    public void refillSelectedWeapon(int _numMags){ 
        int i = 0;
        foreach(Transform _weapon in transform){
            if(i == selectedWeapon){
                _weapon.gameObject.GetComponent<Weapon>().refillMags(_numMags);
            }
            i++;
        }
    }

    public void preventFire(){
        int i = 0;
        foreach(Transform _weapon in transform){
            if(i == selectedWeapon){
                _weapon.gameObject.GetComponent<Weapon>().preventFire = true;
                
            }
            i++;
        }
    }

    public void allowFire(){
        int i = 0;
        foreach(Transform _weapon in transform){
            if(i == selectedWeapon){
                _weapon.gameObject.GetComponent<Weapon>().preventFire = false;
                
            }
            i++;
        }
    }

    bool isCurrentWeaponReloading(){
        int i = 0;
        foreach(Transform _weapon in transform){
            if(i == selectedWeapon){
                return _weapon.gameObject.GetComponent<Weapon>().IsReloading();
                
            }
            i++;
        }
        Debug.Log("no weapon found");
        return false;
    }

    void SelectWeapon(){
        setWeaponUiTextSize();
        setWeaponUiText();
        int _selectedWeapon = selectedWeapon;
        if(_selectedWeapon == 0){
            if(PlayerPrefs.GetInt("Slot1_weapon") != 0){
                _selectedWeapon = PlayerPrefs.GetInt("Slot1_weapon");
            }
        }
        if(_selectedWeapon == 1){
            if(PlayerPrefs.GetInt("Slot2_weapon") != 0){
                _selectedWeapon = PlayerPrefs.GetInt("Slot2_weapon");
            }
        }
        if(_selectedWeapon == 2){
            if(PlayerPrefs.GetInt("Slot3_weapon") != 0){
                _selectedWeapon = PlayerPrefs.GetInt("Slot3_weapon");
            }
        }

        playerSetupView.RPC("SetTPWeapon", RpcTarget.All, _selectedWeapon);

        if (_selectedWeapon >= transform.childCount){
            _selectedWeapon = transform.childCount - 1;
        }

        animation.Stop();
        animation.Play(draw.name);

        int i = 0;
        foreach(Transform _weapon in transform){
            if(i == _selectedWeapon){
                _weapon.gameObject.SetActive(true);
                _weapon.gameObject.GetComponent<Weapon>().SetGunText();
                

                if (!weaponInitialRotations.ContainsKey(i))
            {
                weaponInitialRotations[i] = _weapon.localRotation;  // Store the initial rotation
            }

            // Apply the stored initial rotation to prevent resetting to Quaternion.identity
            _weapon.localRotation = weaponInitialRotations[i];

            }else{
                _weapon.gameObject.SetActive(false);
            }

            i++;
        }
        
    }
}
