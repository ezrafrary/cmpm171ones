using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;

    [Header("next 2 should be the same object")]
    public new GameObject camera;
    public Camera cameraObj; //this is so we can set camerafov without having to jank it

    public string nickname;
    public GameObject playerModel;


    public TextMeshPro nicknameText;
    public GameObject nicknameParent;

    public Transform TPweaponHolder;
    public WeaponSwitcher weaponSwitcher;

    public DamageIndicator hitIndicatorPrefab;
    public GameObject playerUI;


    public OptionsMenu playerOptions;

    [Header("outline")]
    public Outline playerOutline;
    public ColorPicker playerOutlineColorPicker;

    [Header("ignorelayer")]
    public GameObject playerHitbox;
    public GameObject[] playerHitboxes;

    int defaultFov = 60;
    private float refreshSettingsTimer = 3;





    void Start(){
        playerOptions.loadSettings();
        SetCameraFov();
        setupOutlineColor();
    }
    
    void Update(){
        if(refreshSettingsTimer > 0){
            refreshSettingsTimer -= Time.deltaTime;
        }else{
            refreshSettingsTimer = 3;
            playerOptions.loadSettings();
            SetCameraFov();
            setupOutlineColor();
        }
    }




    public void Setup_SetIgnoreHitboxes(){
        playerHitbox.layer = LayerMask.NameToLayer("clientSidePlayerHitbox");
        foreach(GameObject obj in playerHitboxes){
            obj.layer = LayerMask.NameToLayer("clientSidePlayerHitbox");
        }
    }


    public void setupOutlineColor(){
        Color _outlineColor = new Color(playerOutlineColorPicker.getRed()/255f, playerOutlineColorPicker.getGreen()/255f, playerOutlineColorPicker.getBlue()/255f, 1.0f);
        playerOutline.OutlineColor = _outlineColor;
    }

    [PunRPC]
    public void createHitIndicator(Vector3 _damagePosition){
        hitIndicatorPrefab.DamageLocation = _damagePosition;

        GameObject go = Instantiate(hitIndicatorPrefab.gameObject, hitIndicatorPrefab.transform.position, hitIndicatorPrefab.transform.rotation); 
        go.transform.SetParent(playerUI.transform);
        go.SetActive(true);
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

    public void SetCameraFov(){
        cameraObj.fieldOfView = PlayerPrefs.GetInt("FOV", defaultFov);
    }


    public void SetPlayerSens(float _sensX, float _sensY){
        camera.GetComponent<MouseLook>().setSensitivity(_sensX,_sensY);
    }


    //this function is vital, do not change it unless you know what you are doing, and if you change it, note that you did so in the design doc
    public void IsLocalPlayer(){
        TPweaponHolder.gameObject.SetActive(false);
        nicknameParent.SetActive(false);

        movement.enabled = true;
        camera.SetActive(true);
        playerModel.GetComponent<SkinnedMeshRenderer>().enabled = false;
        Setup_SetIgnoreHitboxes();
    }

    [PunRPC]
    public void SetNickname(string _name){
        nickname = _name;

        nicknameText.text = nickname;
    }
}
