using UnityEngine;

public class RPSR_AccelerateAfterTime : MonoBehaviour
{
    public float timeAlive = 0;
    public float timeWhenSpeedIncreaseHappens = 20;
    public float speedAfterTimer = 0;

    


    void FixedUpdate(){
        timeAlive++;
        if(timeAlive >= timeWhenSpeedIncreaseHappens){
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 direction = rb.linearVelocity.normalized;
            rb.linearVelocity = direction * speedAfterTimer;
        }
    }
}
