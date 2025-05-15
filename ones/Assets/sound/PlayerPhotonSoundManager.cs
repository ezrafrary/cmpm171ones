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
    public AudioClip[] allExplosionSFX;


    public AudioClip hitSound;
    public AudioClip headshotSound;
    public AudioClip killSound;
    public AudioSource hitFeedbackSource;
    public AudioSource headshotSoundOrigin;


    public AudioClip dashSound;
    public AudioSource dashSoundSource;

    public Health _health;









    public static float GetMasterVolume(){
        return PlayerPrefs.GetInt("MasterVolume", 100)/100f;
    }



    public void PlayFootstepsSFX(){
        GetComponent<PhotonView>().RPC("PlayFootstepsSFX_RPC", RpcTarget.All);
    }


    public void playKillSound(){
        hitFeedbackSource.clip = killSound;
        hitFeedbackSource.volume = 0.5f * GetMasterVolume();
        hitFeedbackSource.Play();
    }

    public void playHitSound(){
        hitFeedbackSource.clip = hitSound;
        hitFeedbackSource.volume = 0.5f * GetMasterVolume();
        hitFeedbackSource.Play();
    }

    public void playHeadshotSOund(){
        hitFeedbackSource.clip = headshotSound;
        hitFeedbackSource.volume = 0.5f * GetMasterVolume();
        hitFeedbackSource.Play();
    }

    public void playDashSound(){
        GetComponent<PhotonView>().RPC("playDashSound_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void playDashSound_RPC(){
        
        dashSoundSource.clip = dashSound;

        dashSoundSource.volume = 0.2f * GetMasterVolume();
        dashSoundSource.Play();
        
    }




    public void playFollowingSound(GameObject target, int index)
    {
        PhotonView photonView = GetComponent<PhotonView>();
        int targetViewID = target.GetComponent<PhotonView>().ViewID;
        photonView.RPC("playFollowingSound_RPC", RpcTarget.All, targetViewID, index);
    }

    [PunRPC]
    public void playFollowingSound_RPC(int targetViewID, int index)
    {
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView != null)
        {
            GameObject targetObject = targetPhotonView.gameObject;

            // Create a new AudioSource attached to the target object
            AudioSource audioSource = targetObject.AddComponent<AudioSource>();
            audioSource.clip = allExplosionSFX[index];
            audioSource.pitch = 0.7f;
            audioSource.volume = 0.7f * GetMasterVolume();
            //audioSource.spatialBlend = 1.0f; // Make it 3D
            //audioSource.rolloffMode = AudioRolloffMode.Linear;
            //audioSource.maxDistance = 50f; // Set based on your needs
            audioSource.Play();

            // Optionally destroy the AudioSource after the clip finishes
            Destroy(audioSource, allExplosionSFX[index].length);
        }
    }



    public void playImpactSound(Vector3 locationToPlaySound, int index){
        GetComponent<PhotonView>().RPC("playImpactSound_RPC", RpcTarget.All,locationToPlaySound, index);
    }
    [PunRPC]
    public void playImpactSound_RPC(Vector3 locationToPlaySound, int index){
        AudioSource.PlayClipAtPoint(allExplosionSFX[index], locationToPlaySound, 1.0f * GetMasterVolume());
    }

    [PunRPC]
    public void PlayFootstepsSFX_RPC(){
        if(!playerMovement.getGrounded()){
            return;
        }

        footstepSource.clip = footstepSFX;

        //pitch/volume
        footstepSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f);
        footstepSource.volume = UnityEngine.Random.Range(0.05f, 0.01f) * GetMasterVolume();
        footstepSource.Play();
    }

    public void PlayShootSFX(int index, float lowpitch, float highpitch, float lowvol, float highvol, bool varyPitch, bool varyVol){
        GetComponent<PhotonView>().RPC("PlayShootSFX_RPC", RpcTarget.All, index, lowpitch, highpitch, lowvol, highvol, varyPitch, varyVol);
    }

    [PunRPC]
    public void PlayShootSFX_RPC(int index, float lowpitch, float hightpitch, float lowvol, float highvol, bool varyPitch, bool varyVol){
        gunShootSource.clip = allGunShootSFX[index];

        //pitch/volume
        if (varyPitch)
        {
            gunShootSource.pitch = UnityEngine.Random.Range(lowpitch, hightpitch);
        }
        if (varyVol)
        {
            gunShootSource.volume = UnityEngine.Random.Range(lowvol, highvol) * GetMasterVolume();
        }

        gunShootSource.Play();
    }
}
