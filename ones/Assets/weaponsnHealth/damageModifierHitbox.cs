using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class damageModifierHitbox : MonoBehaviour
{
    public float damageMultiplier = 1;
    public string hitboxId = "body";

    public GameObject healthHolder;

    public void Modified_TakeDamage(int _damage, string damageDealer, string damagerWeapon, string hitlocation, int killerHealthLeft, int replayID, Vector3 projectileLinearVelocity){
        int modifiedDamage = (int)_damage * (int)damageMultiplier;
        healthHolder.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, modifiedDamage, damageDealer, damagerWeapon, hitboxId, killerHealthLeft, replayID, projectileLinearVelocity);
    }   

}
