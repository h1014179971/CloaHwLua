using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;

namespace Delivery
{
    public class UIFxCtrl : MonoSingleton<UIFxCtrl>
    {
        private RectTransform _fxRoot;
        public void Init()
        {
            _fxRoot = UIController.Instance.UISceenRoot as RectTransform;
        }
        public UIFxModel PlayFx(string key, Vector3 pos,string str)
        {
            GameObject obj = CreateUIFx(key);
            UIFxModel model = obj.GetComponent<UIFxModel>();
            model.Init(_fxRoot,pos,str);
            return model;
        }
        private GameObject CreateUIFx(string fxPath)
        {
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(fxPath, true, 1);
            return obj;
        }
    }
}

