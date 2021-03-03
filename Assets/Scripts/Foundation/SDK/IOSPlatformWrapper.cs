#if UNITY_IOS 
using AOT;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class IOSPlatformWrapper:MonoBehaviour{

    public delegate void CallbackDelegate(string str);
    private static System.Action<bool> rewardCallBack;
    [DllImport("__Internal")]
    private static extern void initHwAds( string jsonStr, CallbackDelegate callBack = null);
    [DllImport("__Internal")]
    private static extern void showHwRewardAd(string tag, CallbackDelegate platformCallback_RewardAd);
    [DllImport("__Internal")]
    private static extern void showHwInterAd(CallbackDelegate platformCallback_InterAd);
    [DllImport("__Internal")]
    private static extern bool isHwRewardLoaded();
    [DllImport("__Internal")]
    private static extern bool isHwInterLoaded();
    /////////////// 统计////////////////
    [DllImport("__Internal")]
    private static extern void TAEventHwPropertie(string key,string jsonStr);
    static IOSPlatformWrapper _instance;
    public static IOSPlatformWrapper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("IOSPlatformWrapper");
                go.AddComponent<IOSPlatformWrapper>();
                _instance = go.GetComponent<IOSPlatformWrapper>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    public  void initSDK()
    {
        Debug.Log("initSDK");
        initHwAds("121212", initSDKCallback);
    }
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    public static void initSDKCallback(string str)
    {
        Debug.Log("initSDKCallback");
    }

    public static void showRewardedVideo(string tag)
    {
        Debug.Log("IOSPlatformWrapper showRewardedVideo");
        PlatformFactory.Instance.IsPlaying = true;
        showHwRewardAd(tag, PlatformCallback_RewardAd);
    }
    public static void showRewardedVideo(string tag,System.Action<bool> actionCallBack)
    {
        Debug.Log("IOSPlatformWrapper showRewardedVideo Action");
        PlatformFactory.Instance.IsPlaying = true;
        rewardCallBack = actionCallBack;
        showHwRewardAd(tag, PlatformCallback_RewardAd);
    }
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    public static void PlatformCallback_RewardAd(string str)
    {
        Debug.Log("IOSPlatformWrapper   PlatformCallback_RewardAd:" + str);
        PlatformFactory.Instance.IsPlaying = false;
        if (str.Equals("true"))
        {
            if (rewardCallBack != null)
                rewardCallBack(true);
            
        }
        else
        {
            if (rewardCallBack != null)
                rewardCallBack(false);
        }
    }
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    public void OnLoadRewardResult(string isLoad)
    {
        Debug.Log($"IOSPlatformWrapper OnLoadRewardResult Action:{isLoad}");
        if (isLoad.Equals("true"))
        {
            if (PlatformFactory.Instance.onLoadRewardResult != null)
                PlatformFactory.Instance.onLoadRewardResult(true);
        }
        else
        {
            if (PlatformFactory.Instance.onLoadRewardResult != null)
                PlatformFactory.Instance.onLoadRewardResult(false);
        }

    }
    public static bool isRewardLoaded()
    {
        return isHwRewardLoaded();
    }
    public static void showInterAd()
    {
        showHwInterAd(PlatformCallback_InterAd);
    }
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    public static void PlatformCallback_InterAd(string str)
    {

    }
    public static bool isInterLoaded()
    {
        return isHwInterLoaded();
    }

    public static void  TAEventPropertie(string key,string jsonStr)
    {
        Debug.Log("IOSPlatformWrapper EventPropertie:"+jsonStr);
        TAEventHwPropertie(key,jsonStr);
    }

}
#endif
