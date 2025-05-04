using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetmove : MonoBehaviour
{
    float movespeed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
            transform.position = new Vector3((transform.position.x + movespeed), transform.position.y, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        movespeed = movespeed * -1;
    }
}
