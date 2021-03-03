using Foundation;
using libx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdleShelfResMgr : Singleton<IdleShelfResMgr>
    {
        private Dictionary<int, IdleShelfRes> _shelfResDic = new Dictionary<int, IdleShelfRes>();
        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            AssetLoader.LoadAsync(Files.idleShelfRes, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleShelfRes> idleShelfs = FullSerializerAPI.Deserialize(typeof(List<IdleShelfRes>), jsonStr) as List<IdleShelfRes>;
                    _shelfResDic = idleShelfs.ToDictionary(key => key.id, value => value);
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleCity}读取失败");
            });
           
        }


        public IdleShelfRes GetIdleShelRes(int id)
        {
            if (_shelfResDic.ContainsKey(id))
                return _shelfResDic[id];
            return null;
        }

        public int GetMaxResLv()
        {
            List<int> lvList = _shelfResDic.Keys.ToList();
            int maxLv = 1;
            for(int i=0;i<lvList.Count;i++)
            {
                if (lvList[i] > maxLv)
                    maxLv = lvList[i];
            }
            return maxLv;
        }

    }
}


