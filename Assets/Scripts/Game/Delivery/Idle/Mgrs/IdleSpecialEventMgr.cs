using Foundation;
using libx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdleSpecialEventMgr : Singleton<IdleSpecialEventMgr>
    {
        private Dictionary<int, IdleSpecialEvent> specialEventDic = new Dictionary<int, IdleSpecialEvent>();

        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            AssetLoader.LoadAsync(Files.idleSpecialEvent, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleSpecialEvent> events = FullSerializerAPI.Deserialize(typeof(List<IdleSpecialEvent>), jsonStr) as List<IdleSpecialEvent>;
                    specialEventDic = events.ToDictionary(key => key.id, value => value);
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleSpecialEvent}读取失败");
            });

            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleSpecialEvent), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleSpecialEvent> events = FullSerializerAPI.Deserialize(typeof(List<IdleSpecialEvent>), jsonStr) as List<IdleSpecialEvent>;
            //        specialEventDic = events.ToDictionary(key => key.id, value => value);
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
        }
        //根据id获取特殊事件
        public IdleSpecialEvent GetSpacialEvent(int id)
        {
            if(specialEventDic.ContainsKey(id))
            {
                return specialEventDic[id];
            }
            return null;
        }

        //获取随机特殊事件
        public IdleSpecialEvent GetRandomSpecialEvent()
        {
            int index = Random.Range(0, specialEventDic.Count);
            var enumerator = specialEventDic.GetEnumerator();
            int currentIndex = 0;
            while(enumerator.MoveNext())
            {
                if (currentIndex == index)
                {
                    return enumerator.Current.Value;
                }
                currentIndex++;
            }
            return null;
        }

    }
}


