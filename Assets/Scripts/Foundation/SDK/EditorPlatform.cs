using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorPlatform : PlatformFactory
{
    private static System.Action<bool> rewardCallBack;
    private static string adTag;
    public override void initSDK()
    {
        Debug.Log("EditorPlatform initSDK");
    }
    public override bool isRewardLoaded()
    {
        Debug.Log("EditorPlatform isRewardLoaded");
        return true;
    }
    public override void showRewardedVideo(string tag)
    {
        Debug.Log("EditorPlatform showRewardedVideo");
        PlatformFactory.Instance.IsPlaying = true;
        adTag = tag;
        rewardCallBack = null;
        PlatformCallback_FinishRewardAd();
    }
    public override void showRewardedVideo(string tag, Action<bool> actionCallBack)
    {
        Debug.Log("EditorPlatform showRewardedVideo Action");
        PlatformFactory.Instance.IsPlaying = true;
        adTag = tag;
        rewardCallBack = actionCallBack;
        PlatformCallback_FinishRewardAd();
    }
    void PlatformCallback_FinishRewardAd()
    {
        Debug.Log("EditorPlatform PlatformCallback_FinishRewardAd:");
        PlatformFactory.Instance.IsPlaying = false;
        if (rewardCallBack != null)
            rewardCallBack(true);
        //Foundation.Timer.Instance.Register(10,true, (pare) =>
        //{
            
        //});
        
    }
    public override bool isInterLoaded()
    {
        Debug.Log("EditorPlatform isInterLoaded");
        return true;
    }
    public override void showInterAd()
    {
        Debug.Log("EditorPlatform showInterAd");
    }
    public override void GameQuit()
    {
        
    }
    public override void OnApplicationPause(bool pause)
    {
        
    }
    public override void TAEventPropertie(string key, Dictionary<string, string> dic)
    {
        string jsonStr = FullSerializerAPI.Serialize(typeof(Dictionary<string, string>), dic, false, false);
        TAEventPropertie(key, jsonStr);
    }
    public override void TAEventPropertie(string key, string jsonStr)
    {
        Debug.Log("EditorPlatform EventPropertie:" + jsonStr);
    }

}
