using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ItemSpawnPedestal : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform spawnPosition;
    public float respawnCooldown;
    public float repsawnCooldownTimer = 1;
    public bool isItemSpawned = false;

    public GameObject spawnedItem;

    void Start(){
        repsawnCooldownTimer = respawnCooldown;
    }

    void Update(){
        if(repsawnCooldownTimer >= 0 && !isItemSpawned){
            repsawnCooldownTimer = repsawnCooldownTimer - Time.deltaTime;
        }else if (!isItemSpawned && PhotonNetwork.IsMasterClient){
            if(!spawnedItem){
                isItemSpawned = true;
                SpawnItem();
            }

            
        }
    }


    public void itemPickedUp(){
        repsawnCooldownTimer = respawnCooldown;
        isItemSpawned = false;
    }

    public void SpawnItem(){
        var itemSpawned = PhotonNetwork.Instantiate(itemPrefab.name, spawnPosition.position, itemPrefab.transform.rotation);
        if(itemSpawned.GetComponent<HealthPickup>()){
            itemSpawned.GetComponent<HealthPickup>().itemAttachedTo = gameObject;
        }
        if(itemSpawned.GetComponent<AmmoPickup>()){
            itemSpawned.GetComponent<AmmoPickup>().itemAttachedTo = gameObject;
        }

        spawnedItem = itemSpawned;
    }

}
