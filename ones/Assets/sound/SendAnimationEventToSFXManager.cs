using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendAnimationEventToSFXManager : MonoBehaviour
{
    public PlayerPhotonSoundManager playerPhotonSoundManager;


    public void TriggerFootstepsSFX(){
        playerPhotonSoundManager.PlayFootstepsSFX();
    }
}
