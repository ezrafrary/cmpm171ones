using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPickup : MonoBehaviour
{
    public int magsGained;
    public GameObject itemAttachedTo;
    public bool hasBeenUsed = false;


    void OnTriggerEnter(Collider other){

        if(other.transform.gameObject.GetComponent<Health>()){
            if(!hasBeenUsed && PhotonNetwork.IsMasterClient){    
                other.transform.gameObject.GetComponent<PhotonView>().RPC("refillCurrentWeapon",RpcTarget.All, magsGained);
                itemHasBeenPickedUp();
                if(PhotonNetwork.IsMasterClient){
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }


    public void itemHasBeenPickedUp(){
        hasBeenUsed = true;
        if(itemAttachedTo){
            
            itemAttachedTo.GetComponent<ItemSpawnPedestal>().itemPickedUp();
        }
    }
}
