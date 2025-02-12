using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPickup : MonoBehaviour
{
    public int healthGained = 1;
    public GameObject itemAttachedTo;

    public bool hasBeenUsed = false;

    void OnTriggerEnter(Collider other){
        
        if(other.transform.gameObject.GetComponent<Health>()){
            if(!hasBeenUsed){    
                other.transform.gameObject.GetComponent<PhotonView>().RPC("Heal", RpcTarget.All, healthGained);
                hasBeenUsed = true;
                itemHasBeenPickedUp();
                PhotonNetwork.Destroy(gameObject);
            }
        }
        
    }

    public void itemHasBeenPickedUp(){
        if(itemAttachedTo){
            itemAttachedTo.GetComponent<ItemSpawnPedestal>().itemPickedUp();
        }
    }
}
