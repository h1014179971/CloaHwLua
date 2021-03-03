using System;
using UnityEngine;
using UnityEngine.UI;
using Foundation;
static public class MethodExtension
{
    
    #region GetOrAddComponent
    static public T GetOrAddComponent<T>(this GameObject go, string path = "") where T : Component
    {
        Transform t;
        if (string.IsNullOrEmpty(path))
            t = go.transform;
        else
            t = go.transform.Find(path);

        if (null == t)
        {
            //Foundation.LogUtility.LogError("GetOrAddComponent not Find GameObject at Path: " + path);
            Debug.LogError($"GetOrAddComponent not Find GameObject at Path:{path}");
            return null;
        }

        T ret = t.GetComponent<T>();
        if (null == ret)
            ret = t.gameObject.AddComponent<T>();
        return ret;
    }

    static public T GetOrAddComponent<T>(this Transform t, string path = "") where T : Component
    {
        return t.gameObject.GetOrAddComponent<T>(path);
    }

    static public T GetOrAddComponent<T>(this MonoBehaviour mono, string path = "") where T : Component
    {
        return mono.gameObject.GetOrAddComponent<T>(path);
    }

    #endregion

    #region GetComponentByPath

    static public T GetComponentByPath<T>(this Transform transform, string path) where T : Component
    {
        Transform t = transform.Find(path);
        if (null == t)
        {
            //Foundation.LogUtility.LogError("GetComponentByPath not Find GameObject at Path: " + path);
            Debug.LogError($"GetComponentByPath not Find GameObject at Path: {path}");
            return null;
        }
        T ret = t.GetComponent<T>();
        if (null == ret)
            //Foundation.LogUtility.LogError("GetComponentByPath not Find [ " + typeof(T).ToString() + " ] Component at Path: " + path);
            Debug.LogError($"GetComponentByPath not Find [ " + typeof(T).ToString() + " ] Component at Path:{path} ");
        return ret;
    }

    static public T GetComponentByPath<T>(this MonoBehaviour mono, string path) where T : Component
    {
        return mono.transform.GetComponentByPath<T>(path);
    }

    static public T GetComponentByPath<T>(this GameObject go, string path) where T : Component
    {
        return go.transform.GetComponentByPath<T>(path);
    }

    #endregion

}   



