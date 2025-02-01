using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class killZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other){
        if(other.transform.gameObject.GetComponent<Health>()){
            other.transform.gameObject.GetComponent<PhotonView>().RPC("KillPlayer", RpcTarget.All);
        }
    }
}
