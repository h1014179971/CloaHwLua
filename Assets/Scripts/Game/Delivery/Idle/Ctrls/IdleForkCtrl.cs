using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using libx;

namespace Delivery.Idle 
{
    public class IdleForkCtrl : MonoSingleton<IdleForkCtrl>
    {
        private Transform _mid;
        private GameObject[] _stopPoints;
        private List<IdleForkModel> _idleForkModels = new List<IdleForkModel>();
        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }
        public void Init()
        {
            if (_mid == null)
                _mid = GameObject.Find("map/mid").transform;
            _stopPoints = GameObject.FindGameObjectsWithTag(Tags.ForkStopPoint);
            CreateFork();
        }
        private void LoadPrefab()
        {
            SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_down_left, true, 1);
            SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_down_right, true, 1);
            SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_up_left, true, 1);
            SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_up_right, true, 1);
        }
        public GameObject GetObjectFromPool(int index)
        {
            switch (index) 
            {
                case 1: //下_左
                    return SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_down_left, true, 1);
                case 2: //下_右
                    return SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_down_right, true, 1);
                case 3: //上_左
                    return SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_up_left, true, 1);
                case 4: //上_右
                    return SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_up_right, true, 1);
                default:
                    return SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.fork_down_left, true, 1);
            }
            
        }
        private void CreateFork()
        {
            List<IdleItem> unlockIdleItems = PlayerMgr.Instance.GetUnLockIdleItems();
            for(int i = 0; i < unlockIdleItems.Count; i++)
            {
                IdleItem idleItem = unlockIdleItems[i];
                CreateFork(idleItem.Id);
            }
        }
        private void CreateFork(int itemId)
        {

            for (int i = 0; i < _idleForkModels.Count; i++)
            {
                if (_idleForkModels[i].ItemId == itemId) return;
            }
            Transform point = GetStopPointByItemId(itemId);
            GameObject obj = new GameObject();
            obj.transform.SetParent(_mid);
            obj.name = "fork_"+ _idleForkModels.Count;
            IdleForkModel model = obj.GetOrAddComponent<IdleForkModel>();
            model.Init(itemId, point);
            _idleForkModels.Add(model);
        }
        private void OnUnlockSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSiteModel> args = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            int siteId = args.param1.SiteId;
            IdleSite site = IdleSiteMgr.Instance.GetIdleSite(siteId);
            for(int i = 0; i < _idleForkModels.Count; i++)
            {
                IdleForkModel model = _idleForkModels[i];
                if (model.ItemId == site.itemId) return;
            }
            CreateFork(site.itemId);
        }
        private Transform GetStopPointByItemId(int itemId)
        {
            for(int i =0;i< _stopPoints.Length; i++)
            {
                GameObject obj = _stopPoints[i];
                if (obj.name.Contains(itemId.ToString()))
                    return obj.transform;
            }
            LogUtility.LogError($"找不到叉车停靠点{itemId}");
            return null;
        }
        public override void Dispose()
        {
            base.Dispose();
            _mid = null;
            _stopPoints = null;
            _idleForkModels.Clear();
        }
        public override void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }
    }
}


