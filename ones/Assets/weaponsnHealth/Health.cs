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

    private bool hasDied = false; //if the player takes 2 instances of damage in one frame, it duplicates client, this fixes that

    

    private void Start(){
        originalHealthBarSize = healthBar.sizeDelta.x;
        maxHealth = health;
    }



    [PunRPC]
    public void TakeDamage(int _damage){

        if (hasDied){ //making sure a player cant die twice in one frame. 
            return;
        }

        health -= _damage;
        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);
        healthText.text = health.ToString();
        if(health <= 0){
            hasDied = true;
            if(IsLocalPlayer){
                RoomManager.instance.PlayerDied(); //calls spawnplayer() in here
                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void Heal(int _heal){
        health += _heal;
        if(health > maxHealth){
            health = maxHealth;
        }
        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);
        healthText.text = health.ToString();
    }

    

    [PunRPC]
    public void KillPlayer(){
        if (health > 0){ //This seems pointless, but if you use OnTriggerEnter as a damage field, it gets called twice in one frame, duplicating a client 
            TakeDamage(health);
        }
    }
}
