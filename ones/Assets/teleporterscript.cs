using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class teleporterscript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject othertp;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other){
        Debug.Log("hiya");
        PhotonView pv = other.GetComponent<PhotonView>();
        if(pv){
            if(pv.IsMine){
                other.transform.position = othertp.transform.position;
            }
        }
    }
}
