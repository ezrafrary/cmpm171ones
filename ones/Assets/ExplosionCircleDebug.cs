using UnityEngine;

public class ExplosionDebugCircle : MonoBehaviour
{
    public float radius = 1f;
    public float duration = 1f;
    private float timer;

    public int colormode = 1;


    void Start()
    {
        timer = duration;
    }

    void Update()
    {
        DrawDebugCircle(transform.position, radius);
        
    }

    void DrawDebugCircle(Vector3 center, float r)
    {
        int segments = 20;
        float angle = 0f;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * r;

        for (int i = 1; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * r;
            if(colormode == 1){
                Debug.DrawLine(prevPoint, newPoint, Color.yellow, Time.deltaTime);
            }else{
                Debug.DrawLine(prevPoint, newPoint, Color.red, Time.deltaTime);
            }
            prevPoint = newPoint;
        }
    }
}
