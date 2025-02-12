using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class scoreZone : MonoBehaviour
{
    public int scoreGainedPerTick = 1;

    public int fixedUpdatesBetweenTicks = 20;

    private bool playerInField;
    private int fixedUpdatesBetweenTicksTimer = 0;


    void FixedUpdate(){
        if(fixedUpdatesBetweenTicksTimer > 0){
            fixedUpdatesBetweenTicksTimer--;
        }else{
            fixedUpdatesBetweenTicksTimer = fixedUpdatesBetweenTicks;
            if(playerInField){
                PhotonNetwork.LocalPlayer.AddScore(scoreGainedPerTick);

            }
        }
        playerInField = false;
    }
   
    void OnTriggerStay(Collider other){
        if(other.gameObject.GetComponent<Health>()){//dont let projectiles trip this off
            if(other.gameObject.GetComponent<Health>().IsLocalPlayer){
                playerInField = true;
            }
        } 
    }
}
