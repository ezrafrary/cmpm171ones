using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponSwitcher : MonoBehaviour
{
    public Animation animation;
    public AnimationClip draw;

    private int selectedWeapon = 0;

    private bool lockWeaponSwitch = false;

    public PhotonView playerSetupView;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {


        if(isCurrentWeaponReloading()){
            lockWeaponSwitch = true;
        }else{
            lockWeaponSwitch = false;
        }

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
        
        if(selectedWeapon == 0){
            if(PlayerPrefs.GetInt("Slot1_weapon") != 0){
                selectedWeapon = PlayerPrefs.GetInt("Slot1_weapon");
            }
        }
        if(selectedWeapon == 1){
            if(PlayerPrefs.GetInt("Slot2_weapon") != 0){
                selectedWeapon = PlayerPrefs.GetInt("Slot2_weapon");
            }
        }
        if(selectedWeapon == 2){
            if(PlayerPrefs.GetInt("Slot3_weapon") != 0){
                selectedWeapon = PlayerPrefs.GetInt("Slot3_weapon");
            }
        }

        playerSetupView.RPC("SetTPWeapon", RpcTarget.All, selectedWeapon);

        if (selectedWeapon >= transform.childCount){
            selectedWeapon = transform.childCount - 1;
        }

        animation.Stop();
        animation.Play(draw.name);

        int i = 0;
        foreach(Transform _weapon in transform){
            if(i == selectedWeapon){
                _weapon.gameObject.SetActive(true);
                _weapon.gameObject.GetComponent<Weapon>().SetGunText();
            }else{
                _weapon.gameObject.SetActive(false);
            }

            i++;
        }
        
    }
}
