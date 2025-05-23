using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;


//WARNING: if you call TakeDamage() on a trigger field (say with OnTriggerEnter()) it may get called twice, use special handling.

public class Health : MonoBehaviour
{
    public int health;
    public bool IsLocalPlayer;
    public int maxHealth;



    [Header("UI")]
    public TextMeshProUGUI healthText;
    public GameObject respawnUI;

    [Space]
    public bool hasTakenExplosiveDamageThisTick = false;

    public RectTransform healthBar;
    private float originalHealthBarSize;
    public RectTransform TPhealthBar;
    private float originalTPHealthBarSize;




    public bool hasDied = false; //if the player takes 2 instances of damage in one frame, it duplicates client, this fixes that


    private void Start(){
        originalHealthBarSize = healthBar.sizeDelta.x;
        maxHealth = health;
        originalTPHealthBarSize = TPhealthBar.sizeDelta.x;
    }



    [PunRPC]
    public void TakeDamage(int _damage, string damageDealer, string weaponName, string killMethod, int killerHealthLeft, int replayID, Vector3 projectileLinearVelocity){

        if (hasDied){ //making sure a player cant die twice in one frame. 
            return;
        }

        health -= _damage;
        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);
        TPhealthBar.sizeDelta = new Vector2(originalTPHealthBarSize * health / 100f, TPhealthBar.sizeDelta.y);
        healthText.text = health.ToString();
        if(health <= 0){
            hasDied = true;
            if(IsLocalPlayer){
                RoomManager.instance.PlayerDied(damageDealer, weaponName, killMethod, killerHealthLeft, replayID); //calls spawnplayer() in here
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }
            if(killMethod == "head"){
                RoomManager.instance.SpawnHeadlessRagdoll(transform.position, transform.rotation, (projectileLinearVelocity * 0.1f + gameObject.GetComponent<Rigidbody>().linearVelocity));
            }else{
                RoomManager.instance.SpawnRagDoll(transform.position, transform.rotation, (projectileLinearVelocity * 0.1f + gameObject.GetComponent<Rigidbody>().linearVelocity) );
            }
            
            if(GetComponent<PhotonView>()){
                if(GetComponent<PhotonView>().IsMine){
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    [PunRPC]
    public void Heal(int _heal){
        health += _heal;
        if(health > maxHealth){
            health = maxHealth;
        }
        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);
        TPhealthBar.sizeDelta = new Vector2(originalTPHealthBarSize * health / 100f, TPhealthBar.sizeDelta.y);

        healthText.text = health.ToString();
    }

    

    [PunRPC]
    public void KillPlayer(){
        if (health > 0){ //This seems pointless, but if you use OnTriggerEnter as a damage field, it gets called twice in one frame, duplicating a client 
            TakeDamage(health, null, null, null, 0, -1, Vector3.zero);
        }
    }
}
