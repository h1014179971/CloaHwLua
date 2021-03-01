using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery
{
    public class FxCtrl : MonoSingleton<FxCtrl>
    {
        public FxModel PlayFx(string key, Vector3 pos,float dunration = -1)
        {
            FxModel model = PlayFx(key, pos, null, false, dunration);
            return model;
        }


        public FxModel PlayFx(string key, Vector3 pos, Vector3 euler, float dunration = -1)
        {
            FxModel model = PlayFx(key, pos, null,false,dunration);
            if (model != null)
                model.transform.eulerAngles = euler;
            return model;
        }
        public FxModel PlayFx(string key, Vector3 pos, System.Action back = null)
        {
            GameObject obj = CreateFx(key);
            FxModel model = obj.GetOrAddComponent<FxModel>();
            model.transform.position = pos;
            model.Init(null, back, false);
            return model;
        }
        public FxModel PlayFx(string key, Vector3 pos, System.Action back = null,bool loop = false, float dunration = -1)
        {
            GameObject obj = CreateFx(key);
            FxModel model = obj.GetOrAddComponent<FxModel>();
            model.transform.position = pos;
            model.Init(null, back, loop,dunration);
            return model;
        }


        public FxModel PlayFx(string key, Transform trans, bool loop = false, float dunration = -1)
        {
            GameObject obj = CreateFx(key);
            FxModel model = obj.GetOrAddComponent<FxModel>();
            model.transform.position = trans.position;
            model.Init(trans, null, loop,dunration);
            return model;
        }
        public FxModel PlayUIFx(string key, Transform trans, float dunration = -1)
        {
            GameObject obj = CreateFx(key);
            FxModel model = obj.GetOrAddComponent<FxModel>();
            model.transform.position = trans.position;
            model.Init(trans);
            return model;
        }
        private GameObject CreateFx(string fxPath)
        {
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(fxPath, true, 1);
            return obj;
        }

    }
}

