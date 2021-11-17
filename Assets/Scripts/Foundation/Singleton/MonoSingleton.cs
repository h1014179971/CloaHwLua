using System.Collections;
using System.Collections.Generic;
using UnityEngine;  

public abstract class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T>{
    protected static T _Instance = null;
    private static bool _applicationIsQuit = false;
    public static T Instance
    {
        get
        {
            if (_applicationIsQuit)
                return null;

            if (null != _Instance)
                return _Instance;
            GameObject gO = new GameObject(typeof(T).Name, typeof(T));
            _Instance = gO.GetComponent<T>();
            DontDestroyOnLoad(_Instance.gameObject);
            return _Instance;
        }
    }
    protected virtual void Awake()
    {
        if (null == _Instance)
            _Instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
    public static void ReleaseInstance()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
    }
    public virtual void Dispose()
    {

    }
    public virtual void OnDestroy()
    {
        _applicationIsQuit = true;
        _Instance = null;
    }
    private void OnApplicationQuit()
    {
        if(_Instance != null)
            Destroy(_Instance.gameObject);
    }
}
