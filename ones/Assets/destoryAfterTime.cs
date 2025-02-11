using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class destoryAfterTime : MonoBehaviour
{
    public int TimeUntilDestory = 10;
    private PhotonView pv;

    void Start(){
        pv = gameObject.GetComponent<PhotonView>();
    }

    void FixedUpdate(){
        if(TimeUntilDestory > 0){
            TimeUntilDestory--;
        }else{
            DestorySelf();
        }
        
    }

    public void DestorySelf(){
        if(pv){
            if (pv.IsMine){
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
