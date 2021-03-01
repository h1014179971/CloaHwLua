using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using libx;

namespace Delivery.Idle 
{
    public class AssetHelper
    {
        private static Dictionary<string, Sprite> _idleTruckSpriteDic = new Dictionary<string, Sprite>();//站点送到驿站卡车资源
        private static Dictionary<string, Sprite> _idleCityTruckSpriteDic = new Dictionary<string, Sprite>();//送到站点的卡车资源
        private static Dictionary<string, Sprite> _idleItemSpriteDic = new Dictionary<string, Sprite>();//货物图标资源
        private static Dictionary<string, Sprite> _idleItemBoxSpriteDic = new Dictionary<string, Sprite>();//货箱资源
        private static Dictionary<string, Sprite> _idleShelfSpriteDic = new Dictionary<string, Sprite>();//货架图标
        private static Dictionary<string, GameObject> _idlePrefabDic = new Dictionary<string, GameObject>();//场景中预设资源
        private static Dictionary<string, Sprite> _idleUISpecialEventBtnSpriteDic = new Dictionary<string, Sprite>();//特殊事件按钮图标
        public static Sprite GetTruckSprite(int truckLv, bool isEmpty, bool isUp)
        {
            IdleTruckRes idleTruckRes = IdleTruckMgr.Instance.GetIdleTruckRes(truckLv);
            //string spriteName = idleTruckRes.spriteUp + itemId.ToString() + ".png";
            //if (!isUp)
            //    spriteName = idleTruckRes.spriteDown + itemId.ToString() + ".png";
            string spriteName = idleTruckRes.spriteUp + ".png";
            if(isEmpty)
            {
                spriteName = idleTruckRes.emptySpriteUp + ".png";
                if(!isUp)
                    spriteName = idleTruckRes.emptySpriteDown + ".png";
            }
            else
            {
                spriteName = idleTruckRes.spriteUp + ".png";
                if(!isUp)
                    spriteName = idleTruckRes.spriteDown + ".png";
            }
            Sprite sprite = null;
            if (_idleTruckSpriteDic.TryGetValue(spriteName, out sprite))
                return sprite;
            else
            {
                sprite = AssetLoader.Load<Sprite>(spriteName);
                if (sprite == null)
                    LogUtility.LogError($"卡车没有对应资源{spriteName}");
                _idleTruckSpriteDic.Add(spriteName, sprite);
                return sprite;
            }
        }

       
        //读取送到站点货车的图片资源
        public static Sprite GetCityTruckSprite(int itemId,int truckLv)
        {
            IdleCityTruckRes truckRes = IdleCityTruckMgr.Instance.GetIdleCityTruckRes(truckLv);
            if (truckRes == null) return null;
            string spriteName = truckRes.sprite +itemId.ToString() + ".png";
          
            Sprite sprite = null;
            if (_idleCityTruckSpriteDic.TryGetValue(spriteName, out sprite))
                return sprite;
            else
            {
                sprite = AssetLoader.Load<Sprite>(spriteName);
                if (sprite == null)
                    LogUtility.LogError($"卡车没有对应资源{spriteName}");
                _idleCityTruckSpriteDic.Add(spriteName, sprite);
                return sprite;
            }
        }

        public static Sprite GetItemSprite(int itemId)
        {
            IdleItem item = IdleItemMgr.Instance.GetIdleItemById(itemId);
            string iconName = item.icon + ".png";
            Sprite sprite = null;
            if (_idleItemSpriteDic.TryGetValue(iconName, out sprite))
                return sprite;
            else
            {
                sprite = AssetLoader.Load<Sprite>(iconName);
                if(sprite==null)
                    LogUtility.LogError($"没有对应资源{iconName}");
                _idleItemSpriteDic.Add(iconName, sprite);
                return sprite;
            }
        }

        public static Sprite GetItemBoxSprite(int itemId)
        {
            IdleItem item = IdleItemMgr.Instance.GetIdleItemById(itemId);
            string iconName = item.itemRes + ".png";
            Sprite sprite = null;
            if (_idleItemBoxSpriteDic.TryGetValue(iconName, out sprite))
                return sprite;
            else
            {
                sprite = AssetLoader.Load<Sprite>(iconName);
                if (sprite == null)
                    LogUtility.LogError($"没有对应资源{iconName}");
                _idleItemBoxSpriteDic.Add(iconName, sprite);
                return sprite;
            }
        }

        public static Sprite GetShelfSprite(int shelfId)
        {
            IdleShelfRes shelfRes = IdleShelfResMgr.Instance.GetIdleShelRes(shelfId);
            if (shelfRes == null) return null;
            string spriteName = shelfRes.sprite + ".png";
            Sprite sprite = null;
            if (_idleShelfSpriteDic.TryGetValue(spriteName, out sprite))
                return sprite;
            else
            {
                sprite = AssetLoader.Load<Sprite>(spriteName);
                if (sprite == null)
                    LogUtility.LogError($"没有对应资源{spriteName}");
                _idleShelfSpriteDic.Add(spriteName, sprite);
                return sprite;
            }
        }

        public static GameObject GetPrefab(string prefabName)
        {
            prefabName += ".prefab";
            if(!_idlePrefabDic.ContainsKey(prefabName))
            {
                GameObject prefab = AssetLoader.Load<GameObject>(prefabName);
                if(prefab==null)
                    LogUtility.LogError($"没有对应资源{prefabName}");
                _idlePrefabDic.Add(prefabName, prefab);
            }
            return _idlePrefabDic[prefabName];
        }

        public static Sprite GetSpecialEventBtnSprite(string resName)
        {
            string spriteName = resName + ".png";
            Sprite sprite = null;
            if (_idleUISpecialEventBtnSpriteDic.TryGetValue(spriteName, out sprite))
                return sprite;
            else
            {
                sprite = AssetLoader.Load<Sprite>(spriteName);
                if (sprite == null)
                    LogUtility.LogError($"没有对应资源{spriteName}");
                _idleUISpecialEventBtnSpriteDic.Add(spriteName, sprite);
                return sprite;
            }
        }


    }
}


