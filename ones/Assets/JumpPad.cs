using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class JumpPad : MonoBehaviour
{
    public float jumpforce;
    void OnTriggerEnter(Collider other){
        PhotonView pv = other.GetComponent<PhotonView>();
        if(pv){
            if (pv.IsMine){
            other.transform.gameObject.GetComponent<Rigidbody>().AddForce(0, jumpforce, 0, ForceMode.Impulse);
            }
        }
    }
}
