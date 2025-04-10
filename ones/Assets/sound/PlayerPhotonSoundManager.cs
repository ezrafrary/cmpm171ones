using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerPhotonSoundManager : MonoBehaviour
{
    public Movement playerMovement;

    public AudioSource footstepSource;
    public AudioClip footstepSFX;

    public AudioSource gunShootSource;
    public AudioClip[] allGunShootSFX;


    public AudioClip hitSound;
    public AudioClip headshotSound;
    public AudioClip killSound;
    public AudioSource hitFeedbackSource;
    public AudioSource headshotSoundOrigin;


    public AudioClip dashSound;
    public AudioSource dashSoundSource;

    public Health _health;

    public void PlayFootstepsSFX(){
        GetComponent<PhotonView>().RPC("PlayFootstepsSFX_RPC", RpcTarget.All);
    }


    public void playKillSound(){
        hitFeedbackSource.clip = killSound;
        hitFeedbackSource.volume = 0.5f;
        hitFeedbackSource.Play();
    }

    public void playHitSound(){
        hitFeedbackSource.clip = hitSound;
        hitFeedbackSource.volume = 0.5f;
        hitFeedbackSource.Play();
    }

    public void playHeadshotSOund(){
        hitFeedbackSource.clip = headshotSound;
        hitFeedbackSource.volume = 0.5f;
        hitFeedbackSource.Play();
    }

    public void playDashSound(){
        GetComponent<PhotonView>().RPC("playDashSound_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void playDashSound_RPC(){
        
        dashSoundSource.clip = dashSound;

        dashSoundSource.volume = 0.2f;
        dashSoundSource.Play();
        
    }



    [PunRPC]
    public void PlayFootstepsSFX_RPC(){
        if(!playerMovement.getGrounded()){
            return;
        }

        footstepSource.clip = footstepSFX;

        //pitch/volume
        footstepSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f);
        footstepSource.volume = UnityEngine.Random.Range(0.05f, 0.01f);
        footstepSource.Play();
    }

    public void PlayShootSFX(int index){
        GetComponent<PhotonView>().RPC("PlayShootSFX_RPC", RpcTarget.All, index);
    }

    [PunRPC]
    public void PlayShootSFX_RPC(int index){
        gunShootSource.clip = allGunShootSFX[index];

        //pitch/volume
        gunShootSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f);
        gunShootSource.volume = UnityEngine.Random.Range(0.1f, 0.3f);

        gunShootSource.Play();
    }
}
