using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SDKTest : MonoBehaviour
{
    public Text _text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnIsLoadClick()
    {
        PlatformFactory.Instance.onLoadRewardResult += OnLoadRewardResult;
    }
    public void OnClick()
    {
        Debug.Log($"show ad");
        PlatformFactory.Instance.showRewardedVideo("test");
    }
    public void OnLoadRewardResult(bool isLoad)
    {
        Debug.Log($"OnLoadRewardResult:{isLoad}");
        if (isLoad)
            _text.text = "true";
        else
            _text.text = "false";
    }

}
