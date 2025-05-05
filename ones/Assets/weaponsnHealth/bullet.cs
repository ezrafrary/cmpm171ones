using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;


public class Bullet : MonoBehaviour
{
    [Header("killerinfo")]
    public string weaponName;
    public string playerName;
    public int killerHealthLeft;
    public ReplaySystem replaySystem;

    public int parentPhotonViewID;

    [Space]



    public int damage = 1;
    public int explosiveDamage = 0;
    public float explosiveRadius = 0;

    public int scoreGainedForKill = 1;


    public GameObject ignoreHitbox;
    public GameObject[] ignoreHitboxes;

    private float projectilelifetime = 500f;
    private bool dealtDamage = false;


    [Header("VFX")]
    public GameObject hitVFX;
    public GameObject missVFX;
    public float timeUntilBulletIsVisible = 3; //makes the bullet invisible so u dont see it when its close to you
    public GameObject hitmarker;
    public GameObject headshotHitmarker;

    public Vector3 startLocation = new Vector3(0,0,0);
    public float distanceUntilDamageFalloffStarts = 10.0f;
    public float damageLostPerMeter = 1;
    public int minimumDamage = 0;




    [Header("SFX")]
    public PlayerPhotonSoundManager playerPhotonSoundManager;
    public int bulletImpactSoundIndex = -1;


    public void setStartLocation(Vector3 _startLocation){
        startLocation = _startLocation;
    }

    public void setHitmarker(GameObject _hitmarker){
        hitmarker = _hitmarker;
    }
    public void setHeadshotHitmarker(GameObject _hitmarker){
        headshotHitmarker = _hitmarker;
    }


    public void setIgnoreHitbox(GameObject _ignoreHitbox){
        ignoreHitbox = _ignoreHitbox;
    }
    public void setIgnoreHitboxes(GameObject[] _ignoreHitboxes){
        ignoreHitboxes = _ignoreHitboxes;
    }


    private PhotonView pv;


    [Header("hitreg")]
    public GameObject oldPos;
    public Transform startPos;

    void Start(){
        pv = GetComponent<PhotonView>();
        if(timeUntilBulletIsVisible > 0){
            //Debug.Log("bulletmadeinvisible");
            GetComponent<MeshRenderer>().enabled = false;
        }
        
        oldPos = new GameObject();
        //there is no startPos if it is someone else shooting
        if(startPos){
            oldPos.transform.position = startPos.position;
        }

        if(pv.IsMine){
            pv.RPC("SetupReplaySystem",RpcTarget.AllBuffered);
        }
    }
    void FixedUpdate(){
        projectilelifetime--;
        if (projectilelifetime < 0){
            if(pv){
                if (pv.IsMine){
                    PhotonNetwork.Destroy(gameObject);

                    Destroy(oldPos);
                }
            }
        }
        if(timeUntilBulletIsVisible > 0){
            timeUntilBulletIsVisible--;
        }else{
            GetComponent<MeshRenderer>().enabled = true;
        }

        //Debug.Log("Linecasting between: " + transform.position + " and " + oldPos.transform.position);
        
        Debug.DrawLine(oldPos.transform.position, transform.position, Color.red, 1f);

        if(Physics.Linecast(oldPos.transform.position, transform.position, out RaycastHit hit, LayerMask.NameToLayer("clientSidePlayerHitbox"))){
            //Debug.Log("hit: " + hit + " hit.collider: " + hit.collider);
            if(hit.collider.CompareTag("ignoreBullets")){
                //Debug.Log("hit something that ignores bullets");
            }else if(hit.collider.CompareTag("projectile")){
                //Debug.Log("hit projecitle");
            }else{
                if(hit.collider){
                        
                    bulletHitSomething(hit.collider);
                }
            }
        }


        oldPos.transform.position = transform.position;
    }


    //ihatecolisiondetectionihatecolisiondetectionihatecolisiondetectionihatecolisiondetectionihatecolisiondetectionihatecolisiondetectionihatecolisiondetection
    void OnTriggerEnter(Collider other){

        if(other.CompareTag("ignoreBullets")){
            return;
        }
        if(other.CompareTag("projectile")){
            return;
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("clientSidePlayerHitbox")){
            return;
        }
        if(other){
            bulletHitSomething(other);
        }
    }



    public int calculateDamageWithFalloff(){

        float distanceTraveled = Vector3.Distance(startPos.position, transform.position);
        int distanceTraveledInt = (int)distanceTraveled;
        
        int damageWithFalloff = damage;
        if(distanceTraveledInt > distanceUntilDamageFalloffStarts){
            damageWithFalloff = (int)damage - (int)((distanceTraveledInt + (int)distanceUntilDamageFalloffStarts) * damageLostPerMeter);
        }

        if(damageWithFalloff < minimumDamage){
            damageWithFalloff = minimumDamage;
        }

        return damageWithFalloff;
        
    }



    public static GameObject GetObjectByPhotonID(int photonViewID)
    {
        PhotonView targetView = PhotonView.Find(photonViewID);
        if (targetView != null)
        {
            return targetView.gameObject;
        }
        else
        {
            Debug.LogWarning($"No GameObject found with PhotonView ID {photonViewID}");
            return null;
        }
    }

    [PunRPC]
    public void syncParentPVID(int viewID){
        parentPhotonViewID = viewID;
    }

    [PunRPC]
    public void clipThatRPC(){
        replaySystem.clipThatWait2();
    }

    [PunRPC]
    public void SetupReplaySystem() {
        if(GetObjectByPhotonID(parentPhotonViewID)){
            if(GetObjectByPhotonID(parentPhotonViewID).GetComponent<PlayerSetup>().replaySystem){
                replaySystem = GetObjectByPhotonID(parentPhotonViewID).GetComponent<PlayerSetup>().replaySystem;
                //Debug.Log(GetObjectByPhotonID(parentPhotonViewID).GetComponent<PlayerSetup>().nickname); 
            }
        }
        
    }


    void bulletHitSomething(Collider other){



        if(dealtDamage){
            return;
        }
        
        if(!pv){
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
            //Debug.Log("hitignrorehitbox");
            return;
        }   

        foreach (GameObject ihb in ignoreHitboxes){
            if(other.transform.gameObject == ihb){
                return;
            }
        }

        
        ExplosionDamage(gameObject.transform.position, explosiveRadius);

        bool bulletHitPlayer = false;//used to decide which hitvfx to dispaly

        if (other.transform.gameObject.GetComponent<Health>()){
            int replayID = -1;
            playerPhotonSoundManager.playHitSound();
            if(hitVFX){
                PhotonNetwork.Instantiate(hitVFX.name, oldPos.transform.position, Quaternion.identity);
                bulletHitPlayer = true;
            }
            //PhotonNetwork.LocalPlayer.AddScore(damage); add score for damage
            if (calculateDamageWithFalloff() >= other.transform.gameObject.GetComponent<Health>().health && other.transform.gameObject.GetComponent<Health>().hasDied == false){
                //kill
                RoomManager.instance.kills++;
                RoomManager.instance.score += scoreGainedForKill;
                RoomManager.instance.SetHashes();
                playerPhotonSoundManager.playKillSound();
                GetComponent<PhotonView>().RPC("clipThatRPC", RpcTarget.All);
                replayID = 1;
            }
            
            // if(other.transform.gameObject.GetComponent<Health>().hasDied == true){
            //     playerPhotonSoundManager.playKillSound();
            // }

            other.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, calculateDamageWithFalloff(), playerName, weaponName, "body", killerHealthLeft, replayID, GetComponent<Rigidbody>().linearVelocity);
            other.transform.gameObject.GetComponent<PhotonView>().RPC("createHitIndicator", RpcTarget.All, startLocation);
            //Debug.Log("dealt damage");
            //hitmarker
            hitmarker.GetComponent<Hitmarker>().createHitmarker();
            dealtDamage = true;
        }


        if (other.transform.gameObject.GetComponent<damageModifierHitbox>()){
            int replayID = -1;
            if(hitVFX){
                PhotonNetwork.Instantiate(hitVFX.name, oldPos.transform.position, Quaternion.identity);
                bulletHitPlayer = true;
            }
            bool playerDead = false;
            int modifiedDamage = (int)other.transform.gameObject.GetComponent<damageModifierHitbox>().damageMultiplier * (int)calculateDamageWithFalloff();
            if(modifiedDamage >= other.transform.gameObject.GetComponent<damageModifierHitbox>().healthHolder.GetComponent<Health>().health && other.transform.gameObject.GetComponent<damageModifierHitbox>().healthHolder.GetComponent<Health>().hasDied == false){
                playerDead = true;
                RoomManager.instance.kills++;
                RoomManager.instance.score += scoreGainedForKill;
                RoomManager.instance.SetHashes();
                if(playerPhotonSoundManager){
                    playerPhotonSoundManager.playKillSound();
                }
                replayID = 1;
                GetComponent<PhotonView>().RPC("clipThatRPC", RpcTarget.All);
            }

            // if(other.transform.gameObject.GetComponent<damageModifierHitbox>().healthHolder.GetComponent<Health>().hasDied == true){
            //     playerPhotonSoundManager.playKillSound();
            // }

            other.transform.gameObject.GetComponent<damageModifierHitbox>().Modified_TakeDamage(calculateDamageWithFalloff(), playerName, weaponName, null, killerHealthLeft, replayID, GetComponent<Rigidbody>().linearVelocity);
            if (other.transform.gameObject.GetComponent<damageModifierHitbox>().hitboxId == "head"){   
                headshotHitmarker.GetComponent<Hitmarker>().createHitmarker();
                if(!playerDead){
                    if(playerPhotonSoundManager){
                        playerPhotonSoundManager.playHeadshotSOund();
                    }
                }
            }else{
                hitmarker.GetComponent<Hitmarker>().createHitmarker();
            }
            
            dealtDamage = true;
        }
        

        if(bulletImpactSoundIndex != -1){
            if(playerPhotonSoundManager){
                playerPhotonSoundManager.playImpactSound(transform.position, bulletImpactSoundIndex);
            }
        }


        if(!bulletHitPlayer && missVFX){
            PhotonNetwork.Instantiate(missVFX.name, oldPos.transform.position, Quaternion.identity);
        }
            
        if(pv){
            if (pv.IsMine){
                PhotonNetwork.Destroy(gameObject);
                Destroy(oldPos);
            }
        }
    }


    void ExplosionDamage(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int replayID = -1;
        bool _playerDead = false;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.gameObject.GetComponent<Health>()){
                if(!hitCollider.transform.gameObject.GetComponent<Health>().hasTakenExplosiveDamageThisTick){
                    hitCollider.transform.gameObject.GetComponent<Health>().hasTakenExplosiveDamageThisTick = true;

                    if (explosiveDamage >= hitCollider.transform.gameObject.GetComponent<Health>().health && hitCollider.transform.gameObject.GetComponent<Health>().health > 0 && hitCollider.transform.gameObject.GetComponent<Health>().hasDied == false){
                        //kill
                        
                        if(playerPhotonSoundManager){
                            playerPhotonSoundManager.playKillSound();
                        }
                        if(!hitCollider.transform.gameObject.GetComponent<Health>().IsLocalPlayer){
                            replayID = 1;
                            RoomManager.instance.kills++;
                            RoomManager.instance.score += scoreGainedForKill;
                            RoomManager.instance.SetHashes();
                            
                            GetComponent<PhotonView>().RPC("clipThatRPC", RpcTarget.All);
                        }
                        _playerDead = true;
                    }
                    hitCollider.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, explosiveDamage, playerName, weaponName, "explosion", killerHealthLeft, replayID, GetComponent<Rigidbody>().linearVelocity);
                }
                if(!_playerDead){//playhitsound will overwrite playekillsound
                    if(playerPhotonSoundManager){
                        playerPhotonSoundManager.playHitSound();
                    }
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
