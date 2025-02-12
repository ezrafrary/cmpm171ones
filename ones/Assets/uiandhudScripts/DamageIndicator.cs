using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DamageIndicator : MonoBehaviour
{
    public Vector3 DamageLocation;
    public Transform PlayerObject;
    public Transform DamageImagePivot;


    public CanvasGroup DamageImageCanvas;
    public float FadeStartTime;
    public float FadeTime;
    float maxFadeTime;

    void Start(){
        maxFadeTime = FadeTime;
    }


    void Update(){

        if(FadeStartTime > 0){
            FadeStartTime -= Time.deltaTime;
        }else{
            FadeTime -=Time.deltaTime;
            DamageImageCanvas.alpha = FadeTime / maxFadeTime;
            if(FadeTime <= 0){
                Destroy(this.gameObject);
            }
        }

        DamageLocation.y = PlayerObject.position.y;
        Vector3 Direction = (DamageLocation - PlayerObject.position).normalized;
        float angle = (Vector3.SignedAngle(Direction, PlayerObject.forward, Vector3.up));
        DamageImagePivot.transform.localEulerAngles = new Vector3(0,0,angle);
    }
}
