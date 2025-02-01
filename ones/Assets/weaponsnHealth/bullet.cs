using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;


public class Bullet : MonoBehaviour
{

    public int damage = 1;
    public int explosiveDamage = 0;
    public float explosiveRadius = 0;

    public int scoreGainedForKill = 1;


    public GameObject ignoreHitbox;
    private float projectilelifetime = 500f;
    private bool dealtDamage = false;


    [Header("VFX")]
    public GameObject hitVFX;
    public float timeUntilBulletIsVisible = 3; //makes the bullet invisible so u dont see it when its close to you
    

    public void setIgnoreHitbox(GameObject _ignoreHitbox){
        ignoreHitbox = _ignoreHitbox;
    }


    private PhotonView pv;

    void Start(){
        pv = GetComponent<PhotonView>();
        if(timeUntilBulletIsVisible > 0){
            
            GetComponent<MeshRenderer>().enabled = false;
        }

    }
    void FixedUpdate(){
        projectilelifetime--;
        if (projectilelifetime < 0){
            if(pv){
                if (pv.IsMine){
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
        if(timeUntilBulletIsVisible > 0){
            timeUntilBulletIsVisible--;
        }else{
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
    
    void OnTriggerEnter(Collider other){
        
        if(dealtDamage){
            return;
        }
        if(!pv.IsMine){ //This line makes projectile collision clientside rather than serverside, but is the only way i could fix the player hitting themself bug
            return;
        }
        if(other.transform.gameObject.GetComponent<Health>()){
            if(other.transform.gameObject.GetComponent<Health>().IsLocalPlayer){
                //Debug.Log("localplayerhit");
                return;
            }
        }
        if(other.transform.gameObject == ignoreHitbox){
            Debug.Log("hitignrorehitbox");
            return;
        }

        if(hitVFX){
            PhotonNetwork.Instantiate(hitVFX.name, transform.position, Quaternion.identity);
        }
        ExplosionDamage(gameObject.transform.position, explosiveRadius);

        if (other.transform.gameObject.GetComponent<Health>()){

            //PhotonNetwork.LocalPlayer.AddScore(damage); add score for damage
            if (damage >= other.transform.gameObject.GetComponent<Health>().health){
                //kill
                RoomManager.instance.kills++;
                RoomManager.instance.SetHashes();
                PhotonNetwork.LocalPlayer.AddScore(scoreGainedForKill);
            }
            other.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            Debug.Log("dealt damage");
            dealtDamage = true;
        }
            
            
        if(pv){
            if (pv.IsMine){
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }



    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.gameObject.GetComponent<Health>()){
                if(!hitCollider.transform.gameObject.GetComponent<Health>().hasTakenExplosiveDamageThisTick){
                    hitCollider.transform.gameObject.GetComponent<Health>().hasTakenExplosiveDamageThisTick = true;

                    if (explosiveDamage >= hitCollider.transform.gameObject.GetComponent<Health>().health && hitCollider.transform.gameObject.GetComponent<Health>().health > 0){
                        //kill

                        if(!hitCollider.transform.gameObject.GetComponent<Health>().IsLocalPlayer){
                            RoomManager.instance.kills++;
                            RoomManager.instance.SetHashes();
                            PhotonNetwork.LocalPlayer.AddScore(scoreGainedForKill);
                        }
                    }
                    hitCollider.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, explosiveDamage);
                }
            }
        }

        foreach (var hitCollider in hitColliders){
            if(hitCollider.transform.gameObject.GetComponent<Health>()){
                hitCollider.transform.gameObject.GetComponent<Health>().hasTakenExplosiveDamageThisTick = false;
            }
        }
    }
}
