using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRay : MonoBehaviour
{
    public Transform start;
    public Transform end;
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit2D hit =  Physics2D.Linecast(start.position, end.position);
        if(hit.collider != null)
        {
            Debug.Log($"hit  ==={hit.collider.name}");
        }
        RaycastHit h ;
        if(Physics.Linecast(start.position, end.position ,out h))
        {
            Debug.Log($"h=={h.collider.name}");
        }
        RaycastHit hi;
        if(Physics.Raycast(start.position + Vector3.back*10,Vector3.forward,out hi,100))
        {
            Debug.Log($"hi====={hi.collider.name}");
        }
        Vector3 dir = end.position - start.position;
        for(float i = 0; i <= dir.magnitude; i+=0.1f)
        {
            if (Physics.Raycast(start.position + dir.normalized *i + Vector3.back * 10, Vector3.forward, out hi, 100,1<<LayerMask.NameToLayer("Road")))
            {
                Debug.Log($"i==={i}=== hi====={hi.collider.name}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
