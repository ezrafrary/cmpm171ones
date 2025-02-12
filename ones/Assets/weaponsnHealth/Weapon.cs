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

    public Image reloadCircle;

    public int damage;

    public Camera camera;

    public float fireRate;

    private float nextFire;

    [Header("VFX")]
    public GameObject hitVFX;

    [Header("Ammo")]
    public int mag = 5;
    public int ammo = 30;
    public int magAmmo = 30;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;
    public GameObject hitmarker;
    public GameObject headshotHitmarker;

    [Header("Animation")]
    public Animation animation;
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
    public float bulletSpeed = 4f;
    public GameObject playerObjForIgnoreHitbox;
    public GameObject[] ignoreHitboxes;


    [HideInInspector]
    public bool preventFire = false;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    private bool recoiling;
    private bool recovering;
    private float recoilLength;
    private float recoverLength;
    private float reloadMaxTime;

    public void SetReloadCircle(){
        bool currentlyReloading = false;
        if(animation["reload"]){
            reloadCircle.fillAmount = animation["reload"].time/reloadMaxTime;
            if(animation["reload"].time/reloadMaxTime != 0){
                currentlyReloading = true;
            }
        }
        if(animation["reload norotationchange"]){
            reloadCircle.fillAmount = animation["reload norotationchange"].time/reloadMaxTime;
            if(animation["reload norotationchange"].time/reloadMaxTime != 0){
                currentlyReloading = true;
            }
        }
        if(!currentlyReloading){
            reloadCircle.fillAmount = (float) ammo / magAmmo;
        }

    }


    void Start(){
        reloadMaxTime = reload.length;
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetReloadCircle();
        originalPosition = transform.localPosition;
        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
        
    }

    void Update()
    {
        SetReloadCircle();
        if (nextFire > 0){
            nextFire -= Time.deltaTime;
        }

        if (UserInput.instance.AttackHeld && nextFire <= 0 && ammo > 0 && animation.isPlaying == false && !bulletPrefab && !preventFire){
            nextFire = 1 / fireRate;
            ammo--;
            
            SetGunText();

            Fire();
        }

        if (UserInput.instance.AttackHeld && nextFire <= 0 && ammo > 0 && animation.isPlaying == false && bulletPrefab && !preventFire){
            nextFire = 1 / fireRate;
            ammo--;
            
            SetGunText();

            FireProjectile();
        }
        


        if (UserInput.instance.ReloadJustPressed && animation.isPlaying == false && mag > 0 && magAmmo > ammo){
            Reload();
        }

        if (recoiling){
            Recoil();
        }
        if (recovering){
            Recovering();
        }
    }

    
    public void refillMags(int numMags){
        mag = mag + numMags;
        SetGunText();
    }

    public void SetGunText(){
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
        SetReloadCircle();
    }

    void Reload(){
        animation.Play(reload.name);

        if (mag > 0){
            mag--;
            ammo = magAmmo;
        }
        SetGunText();
    }

    public bool IsReloading(){
        //Debug.Log("isreloading called");
        if (animation.isPlaying){
            return true;
        }
        return false;
    }


    void Fire(){

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
        recoiling = true;
        recovering = false;
        playerPhotonSoundManager.PlayShootSFX(ShootSFXIndex);
        if(bulletPrefab){
            spawnBullet();

        }else{
            Debug.Log("no bullet prefab");
        }
    }


    void spawnBullet(){
        var bullet = PhotonNetwork.Instantiate(bulletPrefab.name, bulletSpawnPoint.position, Camera.main.transform.rotation * bulletPrefab.transform.rotation);
        bullet.gameObject.tag = "projectile";
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
        
       

        Bullet bulletSctipt = bullet.GetComponent<Bullet>();
        bulletSctipt.setIgnoreHitbox(playerObjForIgnoreHitbox);
        bulletSctipt.setIgnoreHitboxes(ignoreHitboxes);
        bulletSctipt.setStartLocation(bulletSpawnPoint.transform.position);
        bulletSctipt.playerPhotonSoundManager = playerPhotonSoundManager;
        bulletSctipt.damage = damage;


        if(bulletSctipt.scalingDirection == "z"){
            bullet.transform.localScale = Vector3.Scale(bullet.transform.localScale, new Vector3(1, 1, (bulletSpeed / 25 * bulletSctipt.scalingMultiplier)));
        }
        if(bulletSctipt.scalingDirection == "y"){
            bullet.transform.localScale = Vector3.Scale(bullet.transform.localScale, new Vector3(1, (bulletSpeed / 25 * bulletSctipt.scalingMultiplier), 1));
        }
        if(bulletSctipt.scalingDirection == "x"){
            bullet.transform.localScale = Vector3.Scale(bullet.transform.localScale, new Vector3((bulletSpeed / 25 * bulletSctipt.scalingMultiplier), 1, 1));
        }


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
