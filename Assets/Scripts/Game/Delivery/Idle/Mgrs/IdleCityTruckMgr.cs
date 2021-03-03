using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System.Linq;
using libx;

namespace Delivery.Idle
{
    public class IdleCityTruckMgr : Singleton<IdleCityTruckMgr>
    {
        private List<IdleCityTruckRes> _idleCityTruckResList = new List<IdleCityTruckRes>();
        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {

            AssetLoader.LoadAsync(Files.idleCityTruckRes, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    _idleCityTruckResList = FullSerializerAPI.Deserialize(typeof(List<IdleCityTruckRes>), jsonStr) as List<IdleCityTruckRes>;
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));

                }
                else
                    LogUtility.LogError($"{Files.idleTruckRes}读取失败");
            });


          
        }

        public IdleCityTruckRes GetIdleCityTruckRes(int lv)
        {
            for(int i=0;i<_idleCityTruckResList.Count;i++)
            {
                IdleCityTruckRes truckRes = _idleCityTruckResList[i];
                if (lv >= truckRes.lvMin && lv <= truckRes.lvMax)
                    return truckRes;
            }
            return null;
        }

    }
}


