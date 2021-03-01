using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

namespace Delivery.Idle
{
    public class LoadModel
    {
        public IdleTruckModel truckModel;
        public int loadVolume;
        public bool isWaitForLoad;//是否正在等待送货（有快递员正在送货的途中）
        public Vector3 p2;
        public Vector3 v2;
    }
    public class IdleCityStaffCtrl : MonoSingleton<IdleCityStaffCtrl>
    {
        private Transform _mid;
        private GameObject[] _stopPoints;
        private Dictionary<int,Vector3> _staffWaitPointDic = new Dictionary<int, Vector3>();//快递中心送货员等待点key:itemid;
        private Dictionary<int,Vector3> _offsetDic = new Dictionary<int, Vector3>();//单位偏移量 key:itemid
        private Dictionary<int, Vector3> _waitPointDic = new Dictionary<int, Vector3>();//快递员返回时等待的点
        private float _staffDis = 0.5f;//等待快递员间隙
        private Dictionary<int,List<IdleCityStaffModel>> _idleCityStaffModelDic = new Dictionary<int, List<IdleCityStaffModel>>();
        private Dictionary<int,List<IdleCityStaffModel>> _waitStaffModelDic = new Dictionary<int, List<IdleCityStaffModel>>();//等待快递员
        private Dictionary<int, List<LoadModel>> _loadModelDic = new Dictionary<int, List<LoadModel>>();
        protected override void Awake()
        {
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_TruckLoad, TruckBackLoad);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeStoreVolume, OnStoreVolumeChange);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }
        public void Init()
        {
            if (_mid == null)
                _mid = GameObject.Find("map/mid").transform;
            _stopPoints = GameObject.FindGameObjectsWithTag(Tags.ForkStopPoint);
            LoadPoints();
            CreateStaff();
        }
        private void LoadPoints()
        {
            Transform staffWaitPoint = GameObject.FindGameObjectWithTag(Tags.CityStaffWaitPoint).transform;
            for (int i = 0; i < staffWaitPoint?.childCount; i++)
            {
                Transform point = staffWaitPoint.GetChild(i);
                Vector3 vec = point.position;
                vec.z = 0;
                string[] strs = point.name.Split('_');
                int itemId = int.Parse(strs[1]);
                _staffWaitPointDic[itemId] = vec;
                Transform child = point.Find("child");
                Vector3 offset = child.position - vec;
                _offsetDic[itemId] = offset;
                Transform wait = point.Find("wait");
                _waitPointDic[itemId] = wait.position;
            }
        }
        private void CreateStaff()
        {
            List<IdleItem> unlockIdleItems = PlayerMgr.Instance.GetUnLockIdleItems();
            int storeLv = PlayerMgr.Instance.PlayerCity.storeLv;
            IdleCityStoreVolume storeVolume = IdleCityMgr.Instance.GetIdleStoreVolume(storeLv);
            for (int i = 0; i < unlockIdleItems.Count; i++)
            {
                IdleItem idleItem = unlockIdleItems[i];
                if(!_idleCityStaffModelDic.ContainsKey(idleItem.Id))
                    CreateStaff(idleItem.Id, storeVolume.staffnum);
            }
        }
        private void CreateStaff(int itemId,int count)
        {
            List<IdleCityStaffModel> staffModels;
            if(!_idleCityStaffModelDic.TryGetValue(itemId,out staffModels))
            {
                staffModels = new List<IdleCityStaffModel>();
                _idleCityStaffModelDic[itemId] = staffModels;
            }
            for(int i = 0; i < count; i++)
            {
                string prefabPath = PrefabPath.idlePath + PrefabName.staff2;
                GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.staff2, true, 1);
                Transform point = GetStopPointByItemId(itemId);
                obj.transform.SetParent(_mid);
                obj.transform.position = GetWaitPoint(itemId);
                obj.name = $"citystaff_{itemId}_{staffModels.Count}";
                IdleCityStaffModel model = obj.GetOrAddComponent<IdleCityStaffModel>();
                model.Init(itemId, point);
                staffModels.Add(model);
            }
            
        }
        private Transform GetStopPointByItemId(int itemId)
        {
            for (int i = 0; i < _stopPoints.Length; i++)
            {
                GameObject obj = _stopPoints[i];
                if (obj.name.Contains(itemId.ToString()))
                    return obj.transform;
            }
            LogUtility.LogError($"找不到快递中心送货员停靠点{itemId}");
            return null;
        }
        //添加等到快递员
        public void AddIdleStaff(IdleCityStaffModel model)
        {
            List<IdleCityStaffModel> staffModels;
            if (!_waitStaffModelDic.TryGetValue(model.ItemId, out staffModels))
            {
                staffModels = new List<IdleCityStaffModel>();
                _waitStaffModelDic[model.ItemId] = staffModels;
            }  
            if (!staffModels.Contains(model))
            {
                staffModels.Add(model);
                StaffEndLine(model.ItemId);
            }

        }
        public void RemoveIdleStaff(IdleCityStaffModel model)
        {
            if (_waitStaffModelDic.ContainsKey(model.ItemId))
            {
                if(_waitStaffModelDic[model.ItemId].Contains(model))
                    _waitStaffModelDic[model.ItemId].Remove(model);
                StaffEndLine(model.ItemId);
            }
        }
        //重新排队 
        private void StaffEndLine(int itemId)
        {
            if (!_waitStaffModelDic.ContainsKey(itemId))
            {
                LogUtility.LogError($"快递中心快递员重新排队没有当前itemd{itemId}");
                return;
            }
            if(!_staffWaitPointDic.ContainsKey(itemId))
            {
                LogUtility.LogError($"快递中心快递员没有对应的等待点{itemId}");
                return;
            }
            List<IdleCityStaffModel> staffModels = _waitStaffModelDic[itemId];
            for (int i = 0; i < staffModels.Count; i++)
            {
                Vector3 target = _staffWaitPointDic[itemId] + _offsetDic[itemId] * _staffDis * i;
                staffModels[i].SetIdleTarget(target);
            }
        }
        public Vector3 WaitPoint(int itemId)
        {
            if(!_staffWaitPointDic.ContainsKey(itemId))
            {
                LogUtility.LogError($"快递中心没有对应的快递员等待点{itemId}");
                return Vector3.zero;
            }
            return _staffWaitPointDic[itemId];
        }
        public List<LoadModel> GetLoadModels(int itemId)
        {
            if (_loadModelDic.ContainsKey(itemId))
                return _loadModelDic[itemId];
            else
                return null;
        }
        public LoadModel GetLoadModel(int itemId)
        {
            if (!_loadModelDic.ContainsKey(itemId))
                return null;
            List<LoadModel> loadModels = _loadModelDic[itemId];
            for(int i = 0; i < loadModels.Count; i++)
            {
                LoadModel loadModel = loadModels[i];
                if (loadModel.isWaitForLoad == false)
                    return loadModel;
            }
            return null;
        }
        public Vector3 GetWaitPoint(int itemId)
        {
            if (_waitPointDic.ContainsKey(itemId))
                return _waitPointDic[itemId];
            else
            {
                LogUtility.LogError($"快递中心快递员{itemId}没有等待点");
                return Vector3.zero;
            }
        }
        private void OnUnlockSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSiteModel> args = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            int siteId = args.param1.SiteId;
            IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(siteId);
            int storeLv = PlayerMgr.Instance.PlayerCity.storeLv;
            IdleCityStoreVolume storeVolume = IdleCityMgr.Instance.GetIdleStoreVolume(storeLv);
            if(!_idleCityStaffModelDic.ContainsKey(idleSite.itemId))
                CreateStaff(idleSite.itemId,storeVolume.staffnum);
        }
        private void OnStoreVolumeChange(BaseEventArgs baseEventArgs)
        {
            int storeLv = PlayerMgr.Instance.PlayerCity.storeLv;
            IdleCityStoreVolume storeVolume = IdleCityMgr.Instance.GetIdleStoreVolume(storeLv);
            var enumerator = _idleCityStaffModelDic.GetEnumerator();
            while (enumerator.MoveNext())
            {

                if (storeVolume.staffnum <= enumerator.Current.Value.Count)
                    break;
                int itemId = enumerator.Current.Key;
                int count = storeVolume.staffnum - enumerator.Current.Value.Count;
                CreateStaff(itemId, count);
            }

        }
        private void TruckBackLoad(BaseEventArgs args)
        {
            EventArgsThree<IdleTruckModel, int, int> arg = args as EventArgsThree<IdleTruckModel, int, int>;
            int itemId = arg.param2;
            IdleTruckModel truckModel = arg.param1;
            IdleTruckStopModel stopModel = IdleCityCtrl.Instance.GetUseParkPoint(truckModel, itemId);
            LoadModel loadModel = new LoadModel();
            loadModel.truckModel = truckModel;
            loadModel.loadVolume = arg.param3;
            loadModel.p2 = stopModel.GetPoint();
            loadModel.v2 = stopModel.GetVector();
            if (_loadModelDic.ContainsKey(itemId))
                _loadModelDic[itemId].Add(loadModel);
            else
            {
                List<LoadModel> loadModels = new List<LoadModel>();
                loadModels.Add(loadModel);
                _loadModelDic[itemId] = loadModels;
            }
            StaffLoadItem(itemId);
        }
        /// <summary>
        ///  快递员装货
        /// </summary>
        /// <param name="itemId">货物id</param>
        public void StaffLoadItem(int itemId)
        {
            StaffEndLine(itemId);
        }
        public void RemoveIdleTruck(int itemId, LoadModel loadModel)
        {
            if (!_loadModelDic.ContainsKey(itemId))
            {
                LogUtility.LogError($"没有此类{itemId}卡车");
                return;
            }
            List<LoadModel> loadModels = _loadModelDic[itemId];
            
            EventDispatcher.Instance.TriggerEvent(new EventArgsThree<IdleTruckModel, int, int>(EnumEventType.Event_Idle_TruckMove, loadModel.truckModel, itemId, loadModel.loadVolume));
            loadModel.truckModel = null;
            loadModels.Remove(loadModel);
        }
        public override void Dispose()
        {
             _mid = null;
            _stopPoints = null;
            _staffWaitPointDic.Clear();
            _offsetDic.Clear();
            _waitPointDic.Clear();
            _idleCityStaffModelDic.Clear();
            _waitStaffModelDic.Clear();
            _loadModelDic.Clear();
        }
        public override void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_TruckLoad, TruckBackLoad);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeStoreVolume, OnStoreVolumeChange);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }
    }
}

