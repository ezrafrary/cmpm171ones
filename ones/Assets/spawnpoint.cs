using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnpoint : MonoBehaviour
{
    public bool spawnable;
    public float blockSpawnRadius = 20f;

    void Update(){
        spawnable = CheckIfSpawnable();
    }



    

    public bool CheckIfSpawnable(){
        // Define the center of the area if not using the object's position
        Vector3 center = transform.position;

        // Get all colliders in the area defined by the sphere
        Collider[] collidersInArea = Physics.OverlapSphere(center, blockSpawnRadius);

        foreach (Collider collider in collidersInArea)
        {
            // Check if the collider belongs to a player (object with Player component)
            if (collider.GetComponent<Health>())
            {
                //Debug.Log("Player detected in the area: " + collider.name);
                // Do something, e.g., trigger an event, start a timer, etc.
                return false;
            }
        }
        return true;
    }




}
