using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInfo : MonoBehaviour
{

    public string id;
    public bool isLockFont = false;//是否锁定字体
    [HideInInspector]
    public string arg0;
    [HideInInspector]
    public string arg1;
    [HideInInspector]
    public string arg2;

    public override string ToString()
    {
        return "id:" + id + "  arg0:" + arg0 + "  arg1:" + arg1 + "  arg2:" + arg2;
    }
}  

