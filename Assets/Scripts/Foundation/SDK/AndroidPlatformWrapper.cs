#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;

public class AndroidPlatformWrapper : MonoBehaviour {

    static AndroidPlatformWrapper _instance;
    private static System.Action<bool> rewardCallBack;
    AndroidJavaObject jo = null;
    public delegate void CallbackDelegate(string str);
    public static AndroidPlatformWrapper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AndroidPlatformWrapper");
                go.AddComponent<AndroidPlatformWrapper>();
                _instance = go.GetComponent<AndroidPlatformWrapper>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void initSDK()
    {
        try
        {  
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"); 
            jo = new AndroidJavaObject("com.hw.GamePlayer");
            if(jo != null)
                jo.Call("initHwSDK", currentActivity, "");
        }
        catch (System.Exception e)
        {
            Debug.Log("call java :" + e.Message);
        }
        
    }
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    public void OnLoadRewardResult(string isLoad)
    {
        Debug.Log($"AndroidPlatformWrapper OnLoadRewardResult Action:{isLoad}");
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
    public bool isRewardLoaded()
    {
        if (jo != null)
            return jo.Call<bool>("isHwRewardLoaded");
        return false;
    }
    public void showRewardedVideo(string tag, System.Action<bool> actionCallBack)
    {
        Debug.Log("AndroidPlatformWrapper showRewardedVideo Action");
        
        rewardCallBack = actionCallBack;
        showRewardedVideo(tag);
    }
    
    /// <summary>
    /// 播放视频
    /// </summary>
    public void showRewardedVideo(string tag)
    {
        Debug.Log("AndroidPlatformWrapper showRewardedVideo");
        PlatformFactory.Instance.IsPlaying = true;
        object[] paramArray = new object[3];
        paramArray[0] = tag;
        paramArray[1] = "PlatformCallback_FinishRewardAd";
        paramArray[2] = "PlatformCallback_FailedRewardAd";
        if(jo != null)
            jo.Call("showHwRewardAd", paramArray); 
        
    }                             
    /// <summary>
    /// 视频播放成功
    /// </summary>
    /// <param name="jsonStr"></param>
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    void PlatformCallback_FinishRewardAd(string jsonStr)
    {
        Debug.Log("AndroidPlatformWrapper PlatformCallback_FinishRewardAd:"+jsonStr);
        PlatformFactory.Instance.IsPlaying = false;
        if (rewardCallBack != null)
            rewardCallBack(true);
    }
    /// <summary>
    /// 视频播放失败
    /// </summary>
    /// <param name="jsonStr"></param>
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    void PlatformCallback_FailedRewardAd(string jsonStr)
    {
        Debug.Log("AndroidPlatformWrapper PlatformCallback_FailedRewardAd:"+jsonStr);
        PlatformFactory.Instance.IsPlaying = false;
        if (rewardCallBack != null)
            rewardCallBack(false);
    }
    public bool isInterLoaded()
    {
        if (jo != null)
            return jo.Call<bool>("isHwInterLoaded");
        return false;
    }
    public void showInterAd()
    {
        Debug.Log("AndroidPlatformWrapper showInterAd");
        object[] paramArray = new object[3];
        paramArray[0] = "showInterAd";
        paramArray[1] = "PlatformCallback_FinishInterAd";
        paramArray[2] = "PlatformCallback_FailedInterAd";
        if(jo != null)
            jo.Call("showHwInterAd", paramArray); 
        
    }
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    void PlatformCallback_FinishInterAd(string jsonStr)
    {
        Debug.Log("AndroidPlatformWrapper PlatformCallback_FinishInterAd:"+jsonStr);
    }
    [MonoPInvokeCallback(typeof(CallbackDelegate))]
    void PlatformCallback_FailedInterAd(string jsonStr)
    {
        Debug.Log("AndroidPlatformWrapper PlatformCallback_FailedInterAd:" + jsonStr);  
    }
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void GameQuit()
    {
        if(jo != null)
            jo.Call("GameQuit"); 
        
    }
    public void OnApplicationPause(bool pause)
    {
        Debug.Log("AndroidPlatformWrapper OnApplicationPause:" + pause);
        object[] paramArray = new object[1];
        paramArray[0] = pause;
        if (jo != null)
            jo.Call("OnApplicationPause", paramArray);
    }
    public  void TAEventPropertie(string key, string jsonStr)
    {
        Debug.Log("AndroidPlatformWrapper EventPropertie:" + jsonStr);
        object[] paramArray = new object[2];
        paramArray[0] = key;
        paramArray[1] = jsonStr;  
        if (jo != null)
            jo.Call("TAEventPropertie", paramArray);   
    }
}
#endif
