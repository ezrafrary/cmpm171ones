using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;


public class Hitmarker : MonoBehaviour
{



    
    [Header("Hitmarkers")]
    public Image hitmarkerImage;
    public float hitmarkerLifetime;
    public float hitmarkerLifetimeTimer;
    
    void Update(){
        if(hitmarkerLifetimeTimer <= 0){
            hitmarkerImage.enabled = false;
        }else{
            hitmarkerLifetimeTimer = hitmarkerLifetimeTimer - Time.deltaTime;
        }
    }

    public void createHitmarker(){
        hitmarkerImage.enabled = true;
        hitmarkerLifetimeTimer = hitmarkerLifetime;
    }


}
