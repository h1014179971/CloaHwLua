using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AutoCode
{
    public static class AutoPrefabUtils 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetPrefabPath">Assets/xxx/yyy.prefab</param>
        /// <param name="gameObject"></param>
        public static Object SaveAndConnect(string assetPrefabPath, GameObject gameObject)
        {
            return PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject,
                assetPrefabPath,
                InteractionMode.AutomatedAction);
        }
    }
}

