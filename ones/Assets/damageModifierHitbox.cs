using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class damageModifierHitbox : MonoBehaviour
{
    public float damageMultiplier = 1;
    public string hitboxId = "body";

    public GameObject healthHolder;

    public void Modified_TakeDamage(int _damage){
        int modifiedDamage = (int)_damage * (int)damageMultiplier;
        healthHolder.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, modifiedDamage);
    }

}
