using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = new Color(1,1,1,0.5f);
        //color.a = 0.5f;
        sr.material.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
