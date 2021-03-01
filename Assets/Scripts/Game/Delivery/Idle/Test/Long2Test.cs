using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

public class Long2Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        long a = 125469871156987468;
        double b = 578541124634534535247454d;
        string c = "124534563812347524204534";
        Long2 long1 = new Long2(a);
        Long2 long2 = new Long2(b);
        Long2 long3 = new Long2(c);
        Debug.Log($"long1==={long1},long2==={long2},long3==={long3}");
        Debug.Log($"+++{long1 + long2}");
        Debug.Log($"---{long1 - long2}");
        Debug.Log($"***{long1 * long2}");
        Debug.Log($"///{long1 / long2}");
        string str = FullSerializerAPI.Serialize(typeof(Long2), long1,false,false);
        Debug.Log($"str===={str}");
        //Long2 l3 = FullSerializerAPI.Deserialize(typeof(Long2), str) as Long2;
        //Debug.Log($"???????{l3}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
