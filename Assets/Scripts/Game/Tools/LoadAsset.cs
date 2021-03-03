
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery
{
    public class LoadAsset
    {
        
        public static System.Object Load(string path)
        {
            return Resources.Load(path);
        }
        public static System.Object Load(string path,  System.Type systemTypeInstance)
        {
            return Resources.Load(path, systemTypeInstance);
        }
        public static T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
        public static T Instantiate<T>(string path) where T: Object
        {
            return GameObject.Instantiate<T>(Load<T>(path));
        }
    }
}

