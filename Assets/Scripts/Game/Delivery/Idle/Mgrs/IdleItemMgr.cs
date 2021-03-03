using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using System.Linq;
using libx;

namespace Delivery.Idle
{
    public class IdleItemMgr : Singleton<IdleItemMgr>
    {
        Dictionary<int, IdleItem> _idleItemDic = new Dictionary<int, IdleItem>();
        public override void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            AssetLoader.LoadAsync(Files.idleItem, typeof(TextAsset), delegate (Object obj)
            {
                if (obj as TextAsset)
                {
                    string jsonStr = (obj as TextAsset).text;
                    List<IdleItem> idleItems = FullSerializerAPI.Deserialize(typeof(List<IdleItem>), jsonStr) as List<IdleItem>;
                    _idleItemDic = idleItems.ToDictionary(key => key.Id, value => value);
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleCity}读取失败");
            });
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleItem), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleItem> idleItems = FullSerializerAPI.Deserialize(typeof(List<IdleItem>), jsonStr) as List<IdleItem>;
            //        _idleItemDic = idleItems.ToDictionary(key => key.Id, value => value);     
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //}); 
        }
        public IdleItem GetIdleItemById(int itemId)
        {
            if (_idleItemDic.ContainsKey(itemId))
            {
                return _idleItemDic[itemId];
            }
            else
            {
                LogUtility.LogError($"没有此物品{itemId}");
                return null;
            }
        }
    }
}

