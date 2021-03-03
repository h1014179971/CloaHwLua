#if UNITY_IOS 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOSPlatform : PlatformFactory
{

    public override void initSDK()
    {
        IOSPlatformWrapper.Instance.initSDK();
        
    }
    public override bool isRewardLoaded()
    {
        Debug.Log("IOSPlatform isRewardLoaded");
        return IOSPlatformWrapper.isRewardLoaded();
    }
    public override void showRewardedVideo(string tag)
    {
        Debug.Log("IOSPlatform showRewardedVideo");
        IOSPlatformWrapper.showRewardedVideo(tag);
    }
    public override void showRewardedVideo(string tag, Action<bool> actionCallBack)
    {
        Debug.Log("IOSPlatform showRewardedVideo Action");
        IOSPlatformWrapper.showRewardedVideo(tag,actionCallBack);
    }
    public override bool isInterLoaded()
    {
        Debug.Log("IOSPlatform isInterLoaded");
        return IOSPlatformWrapper.isInterLoaded();
    }
    public override void showInterAd()
    {
        Debug.Log("IOSPlatform showInterAd");
        IOSPlatformWrapper.showInterAd();
    }
    public override void GameQuit()
    {
        
    }
    public override void OnApplicationPause(bool pause)
    {
        
    }
    public override void TAEventPropertie(string key, Dictionary<string, string> dic)
    {
        string jsonStr = FullSerializerAPI.Serialize(typeof(Dictionary<string,string>), dic, false, false);
        TAEventPropertie(key,jsonStr);
    }
    public override void TAEventPropertie(string key,string jsonStr)
    {
        Debug.Log("IOSPlatform EventPropertie:"+jsonStr);
        IOSPlatformWrapper.TAEventPropertie(key,jsonStr);
    }


}
#endif
