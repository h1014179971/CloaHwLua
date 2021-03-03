using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using libx;

public class AssetTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = AssetLoader.Load<GameObject>("truck");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
