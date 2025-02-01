using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;

    [Header("next 2 should be the same object")]
    public GameObject camera;
    public Camera cameraObj; //this is so we can set camerafov without having to jank it

    public string nickname;
    public GameObject playerModel;


    public TextMeshPro nicknameText;
    
    public Transform TPweaponHolder;
    public WeaponSwitcher weaponSwitcher;

    int defaultFov = 60;

    void Start(){
        SetCameraFov(PlayerPrefs.GetInt("FOV", defaultFov));
    }


    [PunRPC]
    public void refillCurrentWeapon(int _numMags){
        weaponSwitcher.refillSelectedWeapon(_numMags);
    }

    [PunRPC]
    public void SetTPWeapon(int _weaponIndex){
        foreach (Transform _weapon in TPweaponHolder){
            _weapon.gameObject.SetActive(false);
        }
        TPweaponHolder.GetChild(_weaponIndex).gameObject.SetActive(true);
    }

    public void SetCameraFov(float _cameraFov){
        cameraObj.fieldOfView = _cameraFov;
    }


    public void SetPlayerSens(float _sensX, float _sensY){
        camera.GetComponent<MouseLook>().setSensitivity(_sensX,_sensY);
    }


    //this function is vital, do not change it unless you know what you are doing, and if you change it, note that you did so in the design doc
    public void IsLocalPlayer(){
        TPweaponHolder.gameObject.SetActive(false);

        movement.enabled = true;
        camera.SetActive(true);
        playerModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }

    [PunRPC]
    public void SetNickname(string _name){
        nickname = _name;

        nicknameText.text = nickname;
    }
}
