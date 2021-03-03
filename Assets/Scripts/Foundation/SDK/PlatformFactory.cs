using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFactory {

    protected static PlatformFactory _Instance = null;
    public bool IsPlaying { get; set; }//广告是否正在播放
    public static PlatformFactory Instance
    {
        get
        {
            if (null == _Instance)
            {

                _Instance = CreatePlatformInterface();
            }
            return _Instance;
        }
    }
    static PlatformFactory CreatePlatformInterface()
    {
#if UNITY_IOS && !UNITY_EDITOR
            _Instance = new IOSPlatform();
#elif UNITY_ANDROID && !UNITY_EDITOR
            _Instance = new AndroidPlatform();
#else
        _Instance = new EditorPlatform();
#endif
        return _Instance;
    }
    public System.Action<bool> onLoadRewardResult;
    public virtual void initSDK() { }
    public virtual void loadInterAd() { }
    public virtual void showInterAd() { }
    public virtual bool isInterLoaded() { return false; }
    public virtual void loadRewardedVideo() { }
    public virtual void showRewardedVideo(string tag) { }
    public virtual void showRewardedVideo(string tag, System.Action<bool> actionCallBack) { }
    public virtual bool isRewardLoaded() { return false; }

    public virtual void GameQuit() { }
    public virtual void OnApplicationPause(bool pause) { }


    public virtual void TAEventPropertie(string key, string jsonStr){}
    public virtual void TAEventPropertie(string key,Dictionary<string,string> dic) { }
}
