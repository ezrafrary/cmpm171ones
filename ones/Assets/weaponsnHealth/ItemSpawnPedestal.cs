using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ItemSpawnPedestal : MonoBehaviour
{
    
    [Header("ItemPrefabs")]
    public GameObject itemPrefab;
    public GameObject alternateItemPrefab;


    [Space]


    public Transform spawnPosition;
    public float respawnCooldown;
    public float repsawnCooldownTimer = 1;
    public bool isItemSpawned = false;

    public float chanceForAltItem = 0.5f;


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

       
        float randomFloat = Random.Range(0f,1f);

        GameObject itemSpawned;

        if(chanceForAltItem > randomFloat && alternateItemPrefab){
            itemSpawned = PhotonNetwork.Instantiate(alternateItemPrefab.name, spawnPosition.position, alternateItemPrefab.transform.rotation);
        }else{
            itemSpawned = PhotonNetwork.Instantiate(itemPrefab.name, spawnPosition.position, itemPrefab.transform.rotation);
        }


        if(itemSpawned.GetComponent<HealthPickup>()){
            itemSpawned.GetComponent<HealthPickup>().itemAttachedTo = gameObject;
        }
        if(itemSpawned.GetComponent<AmmoPickup>()){
            itemSpawned.GetComponent<AmmoPickup>().itemAttachedTo = gameObject;
        }

        spawnedItem = itemSpawned;
    }

}
