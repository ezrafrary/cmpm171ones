using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{

    /*
        A lot going on here, watching the video https://www.youtube.com/watch?v=IQRx7ojL7r8&list=PL0iUgXtqnG2gPaXE1hHYoBjqTSWTNwR-6&index=3&ab_channel=bananadev2
        will help with understanding it, as well as the following 3 videos, but that only helps with understaindg weapon switching, the projectile weapons work
        differenlty from how the video does it. 
    */


    public GameObject playerShooting;

    public Image reloadCircle;

    public int damage;

    public new Camera camera;

    public float fireRate;

    public float cameraRecoil;
    public float horizontalCameraRecoil = 0;

    public bool isAutomatic = true;

    private float nextFire;


    public float reloadTime = 100f;
    private float reloadTimer = 0;

    [Header("VFX")]
    public GameObject hitVFX;

    [Header("Ammo")]
    public int mag = 5;
    public int maxMags = 10;
    public int ammo = 30;
    public int magAmmo = 30;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;
    public GameObject hitmarker;
    public GameObject headshotHitmarker;

    [Header("Animation")]
    public new Animation animation;
    public AnimationClip reload;

    [Header("SFX")]
    public int ShootSFXIndex = 0;
    public PlayerPhotonSoundManager playerPhotonSoundManager;


    [Header("Recoil Settings")]
    [Range(0,1)]
    // public float recoilPercent = 0.3f;
    // [Range(0,2)]
    public float recoverPercent = 0.7f;
    [Space]
    public float recoilUp = 1f;
    public float recoilBack = 0;

    [Header("projectile settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public Transform bulletStartPoint;
    public float bulletSpeed = 4f;
    public GameObject playerObjForIgnoreHitbox;
    public GameObject[] ignoreHitboxes;

    public bool isShotgun = false;
    public int numPellets = 1;
    public float spread = 0.0f;


    [HideInInspector]
    public bool preventFire = false;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    private bool recoiling;
    private bool recovering;
    private float recoilLength;
    private float recoverLength;
    private float reloadMaxTime;

    private bool wasReloadingLastFrame = false;

    public ReplaySystem replaySystem;


    public void SetReloadCircle(){
        bool currentlyReloading = false;
        // if(animation["reload"]){
        //     reloadCircle.fillAmount = animation["reload"].time/reloadMaxTime;
        //     if(animation["reload"].time/reloadMaxTime != 0){
        //         currentlyReloading = true;
        //     }
        // }
        // if(animation["reload norotationchange"]){
        //     reloadCircle.fillAmount = animation["reload norotationchange"].time/reloadMaxTime;
        //     if(animation["reload norotationchange"].time/reloadMaxTime != 0){
        //         currentlyReloading = true;
        //     }
        // }

        if(reloadTimer > 0){
            currentlyReloading = true;
            reloadCircle.fillAmount = 1 - (reloadTimer / reloadTime);
        }


        if(!currentlyReloading){
            reloadCircle.fillAmount = (float) ammo / magAmmo;
        }

    }



    void Start(){
        reloadMaxTime = reload.length;
        SetGunText();
        originalPosition = transform.localPosition;
        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    void Update()
    {
        SetReloadCircle();
        

        if(reloadTimer > 0){
            reloadTimer = reloadTimer - Time.deltaTime;
            preventFire = true;
        }else{
            preventFire = false;
        }
        //Debug.Log(reloadTimer);


        if (nextFire > 0){
            nextFire -= Time.deltaTime;
        }

        if (UserInput.instance.AttackHeld && nextFire <= 0 && ammo > 0 && animation.isPlaying == false && !bulletPrefab && !preventFire){
            nextFire = 1 / fireRate;
            ammo--;
            
            SetGunText();

            Fire();
        }

        if (UserInput.instance.AttackHeld && nextFire <= 0 && ammo > 0 && animation.isPlaying == false && bulletPrefab && !preventFire && isAutomatic){
            nextFire = 1 / fireRate;
            ammo--;
            
            SetGunText();

            FireProjectile();
        }


        bool justShot = false;
        if (UserInput.instance.AttackInput && nextFire <= 0 && ammo > 0 && animation.isPlaying == false && bulletPrefab && !preventFire && !isAutomatic){
            justShot = true;
            nextFire = 1 / fireRate;
            ammo--;
            
            SetGunText();

            FireProjectile();
        }
        

        if (UserInput.instance.ReloadJustPressed && reloadTimer <= 0 && mag > 0 && magAmmo > ammo){
            Reload();
        }else if (UserInput.instance.AttackInput && reloadTimer <= 0 && mag > 0 && ammo <= 0 && !justShot){
            Reload();
        } 



        if(!IsReloading() && wasReloadingLastFrame){
            doneReloading();
        }



        if(IsReloading()){
            wasReloadingLastFrame = true;
        }else{
            wasReloadingLastFrame = false;
        }



        if (recoiling){
            Recoil();
        }
        if (recovering){
            Recovering();
        }
    }

    
    public void refillMags(int numMags){
        if(mag + numMags <= maxMags){
            mag = mag + numMags;
        }else{
            mag = maxMags;
        }
        SetGunText();
    }

    public void SetGunText(){
        magText.text = mag.ToString() + "/" + maxMags;
        ammoText.text = ammo + "/" + magAmmo;
        SetReloadCircle();
    }

    void Reload(){
        animation.Play(reload.name);
        reloadTimer = reloadTime;
    }


    void doneReloading(){
        if (mag > 0){
            mag--;
            ammo = magAmmo;
        }
        SetGunText();
    }

    // public bool IsReloading(){
    //     //Debug.Log("isreloading called");
    //     if (animation.isPlaying){
    //         return true;
    //     }
    //     return false;
    // }

    public bool IsReloading(){
        if (reloadTimer > 0){
            return true;
        }
        return false;
    }


    void Fire(){

        //camera.gameObject.transform.position = new Vector3(camera.gameObject.transform.position.x,camera.gameObject.transform.position.y + 1, camera.gameObject.transform.position.z );//Camera recoil
        playerPhotonSoundManager.PlayShootSFX(ShootSFXIndex);

        recoiling = true;
        recovering = false;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f)){
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            if (hit.transform.gameObject.GetComponent<Health>()){
                //PhotonNetwork.LocalPlayer.AddScore(damage); add score for damage
                if (damage >= hit.transform.gameObject.GetComponent<Health>().health){
                    //kill

                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                    PhotonNetwork.LocalPlayer.AddScore(1);
                }
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }

    void FireProjectile(){
        //camera.gameObject.transform.rotation = Quaternion.Euler(0,camera.gameObject.transform.rotation.y + 1, 0);//Camera recoil
        Movement movescript = GetComponentInParent<Movement>();
        movescript.recoildegrees += cameraRecoil;
        movescript.horizontalRecoilDegrees += horizontalCameraRecoil;



        recoiling = true;
        recovering = false;
        playerPhotonSoundManager.PlayShootSFX(ShootSFXIndex);
        
        if(isShotgun){
            for(int i = 0; i < numPellets; i++){
                if(bulletPrefab){
                    var testBullet = spawnBullet(spread);
                }else{
                    Debug.Log("no bullet prefab");
                }
            }
        }else{
            if(bulletPrefab){
                var testBullet = spawnBullet(spread);
            }else{
                Debug.Log("no bullet prefab");
            }
        }
        



    }


    


    GameObject spawnBullet(float _spread){


        Vector3 randomAxis = UnityEngine.Random.onUnitSphere;
        float randomAngle = UnityEngine.Random.Range(-_spread, _spread);
        Quaternion spreadRotation = Quaternion.AngleAxis(randomAngle, randomAxis);

        // Apply random rotation to the camera's forward direction
        Quaternion finalRotation = spreadRotation * camera.transform.rotation;



        var bullet = PhotonNetwork.Instantiate(bulletPrefab.name, bulletSpawnPoint.position, finalRotation * bulletPrefab.transform.rotation);
        //Debug.Log("bullet type: " + bullet.GetType());
        bullet.gameObject.tag = "projectile";
        bullet.GetComponent<Rigidbody>().linearVelocity = finalRotation * Vector3.forward * bulletSpeed;
        //bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        
       

        Bullet bulletSctipt = bullet.GetComponent<Bullet>();

        bulletSctipt.parentPhotonViewID = playerShooting.GetComponent<PhotonView>().ViewID;
        bullet.GetComponent<PhotonView>().RPC("syncParentPVID", RpcTarget.AllBuffered, bulletSctipt.parentPhotonViewID);
        
        
        bulletSctipt.playerName = playerObjForIgnoreHitbox.GetComponent<PlayerSetup>().nickname;
        bulletSctipt.weaponName = gameObject.name;
        bulletSctipt.killerHealthLeft = playerObjForIgnoreHitbox.GetComponent<Health>().health;
        bulletSctipt.setIgnoreHitbox(playerObjForIgnoreHitbox);
        bulletSctipt.setIgnoreHitboxes(ignoreHitboxes);
        bulletSctipt.setStartLocation(bulletSpawnPoint.transform.position);
        bulletSctipt.playerPhotonSoundManager = playerPhotonSoundManager;
        bulletSctipt.damage = damage;
        bulletSctipt.startPos = bulletStartPoint;
        
        


        // if(bulletSctipt.scalingDirection == "z"){
        //     bullet.transform.localScale = Vector3.Scale(bullet.transform.localScale, new Vector3(1, 1, (bulletSpeed / 25 * bulletSctipt.scalingMultiplier)));
        // }
        // if(bulletSctipt.scalingDirection == "y"){
        //     bullet.transform.localScale = Vector3.Scale(bullet.transform.localScale, new Vector3(1, (bulletSpeed / 25 * bulletSctipt.scalingMultiplier), 1));
        // }
        // if(bulletSctipt.scalingDirection == "x"){
        //     bullet.transform.localScale = Vector3.Scale(bullet.transform.localScale, new Vector3((bulletSpeed / 25 * bulletSctipt.scalingMultiplier), 1, 1));
        // }


        if(headshotHitmarker){
            bulletSctipt.setHeadshotHitmarker(headshotHitmarker);
        }else{
            Debug.Log("noheadshothitmarker");
        }

        if(hitmarker){
            bulletSctipt.setHitmarker(hitmarker);
        }else{
            Debug.Log("nohitmarker");
        }

        return bullet;
    }

    void Recoil(){
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if(transform.localPosition == finalPosition){
            recoiling = false;
            recovering = true;
        }
    }
    
    void Recovering(){
        Vector3 finalPosition = originalPosition;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if(transform.localPosition == finalPosition){
            recoiling = false;
            recovering = false;
        }
    }
}
