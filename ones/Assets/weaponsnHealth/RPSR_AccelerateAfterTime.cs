using UnityEngine;

public class RPSR_AccelerateAfterTime : MonoBehaviour
{
    public float timeAlive = 0;
    public float timeWhenSpeedIncreaseHappens = 20;
    public float speedAfterTimer = 0;
    public int accelerationSFXIndex = -1;

    public Bullet ParentBullet;

    //private bool accelratedLastframe = false;


    void FixedUpdate(){
        timeAlive++;
        if(timeAlive >= timeWhenSpeedIncreaseHappens){
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 direction = rb.linearVelocity.normalized;
            rb.linearVelocity = direction * speedAfterTimer;
            // if(!accelratedLastframe){
            //     accelratedLastframe = true;
            //     Debug.Log("test");
            //     if(ParentBullet.playerPhotonSoundManager){
            //         if(accelerationSFXIndex != -1){
            //             //ParentBullet.playerPhotonSoundManager.playImpactSound(transform.position, accelerationSFXIndex);

            //             ParentBullet.playerPhotonSoundManager.playFollowingSound(gameObject, accelerationSFXIndex);
            //         }
            //     }else{
            //         Debug.Log("shits fucked");
            //     }
            // }
        }
    }
}
