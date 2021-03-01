using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

namespace Delivery.Track
{
    public class TrackUAVCtrl : MonoSingleton<TrackUAVCtrl>
    {
        private Transform _uavParent;
        private Dictionary<string, List<TrackUAVModel>> _trackUavs = new Dictionary<string, List<TrackUAVModel>>();
        public void CreateUAV(TrackLineModel trackLineModel)
        {
            List<TrackUAVModel> uavs;
            TrackUAVModel trackUavModel = null;
            if (!_trackUavs.TryGetValue(trackLineModel.ColorHex, out uavs))
            {
                uavs = new List<TrackUAVModel>();
                _trackUavs.Add(trackLineModel.ColorHex, uavs);
            }
            else
            {
                trackUavModel = uavs[0];            
                return;
            }
            if(_uavParent == null)
                _uavParent = this.GetComponentByPath<Transform>("bg/uav");
            string prefabPath = PrefabPath.uavPath + "uav";
            GameObject uavObj = SG.ResourceManager.Instance.GetObjectFromPool(prefabPath, true, 1);
            RectTransform uavTrans = uavObj.GetComponent<RectTransform>();
            uavTrans.SetParent(_uavParent);
            uavTrans.localScale = Vector3.one;
            trackUavModel = uavTrans.GetOrAddComponent<TrackUAVModel>();
            trackUavModel.Init(trackLineModel);  
            uavs.Add(trackUavModel);
            trackLineModel.TrackUAVModel = trackUavModel;
        }
    }
}

