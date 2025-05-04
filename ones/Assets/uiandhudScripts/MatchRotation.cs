using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    [Tooltip("The target object whose rotation will be copied.")]
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.rotation = target.rotation;
        }
    }
}