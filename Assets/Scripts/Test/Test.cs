using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using Delivery;
using UnityEngine.UI;
using UIFramework;
public class Test : MonoBehaviour
{
    
    //public RectTransform childTrans;              
    //void Start()
    //{
    //    RectTransform rectTrans = GetComponent<RectTransform>();
    //    LogUtility.LogInfo($"anchorMin==={rectTrans.anchorMin}");
    //    LogUtility.LogInfo($"anchorMax==={rectTrans.anchorMax}");
    //    LogUtility.LogInfo($"offsetMin==={rectTrans.offsetMin}");
    //    LogUtility.LogInfo($"offsetMax==={rectTrans.offsetMax}");
    //    LogUtility.LogInfo($"anchoredPosition3D==={rectTrans.anchoredPosition3D}");
    //    LogUtility.LogInfo($"sizeDelta==={rectTrans.sizeDelta}");
    //    float x = rectTrans.offsetMin.x + rectTrans.sizeDelta.x * rectTrans.pivot.x;
    //    float y = rectTrans.offsetMin.y + rectTrans.sizeDelta.y * rectTrans.pivot.y;
    //    LogUtility.LogInfo($"x==={x}  y==={y}");
    //    LogUtility.LogInfo($"childTrans.anchoredPosition3D === {childTrans.anchoredPosition3D}");
    //    LogUtility.LogInfo($"ConvertGLanchoredPosition3D === {Tool.ConvertGLPosition(rectTrans)}");  
    //}
    private void Awake()
    {
        Debug.Log("test============");
        //CanvasScaler _scaler = RootCanvas.Instance.UICanvas.GetComponent<CanvasScaler>();
        //if (_scaler == null) _scaler = GetComponentInParent<CanvasScaler>();

        //var resolution = _scaler.referenceResolution;
        //var rt = _scaler.transform as RectTransform;
        //if (rt == null) return;
        //var screenSize = rt.sizeDelta;
        //var factor = Mathf.Max(screenSize.x / resolution.x, screenSize.y / resolution.y);
        //var scale = Vector3.one * factor;
        //transform.localScale = scale;
        GameObject obj = transform.GetChild(0).gameObject;
        Debug.Log($"obj name==={obj.name}");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {
        Debug.Log($"show ad");
        PlatformFactory.Instance.showRewardedVideo("test");
    }
}
