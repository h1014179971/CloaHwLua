
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using libx;
using System.Linq;

namespace Delivery.Idle
{
    public class ParkPoint
    {
        public IdleTruckStopModel stopModel;
        public IdleTruckModel truckModel;
        public SpriteRenderer lockSprite;
        public SpriteRenderer ganSprite;
        public SpriteRenderer xianSprite;
        public bool unLock;
        public bool isUse;
    }
    public class IdleCityCtrl : MonoSingleton<IdleCityCtrl>
    {
        

        private IdleCityModel _idleCityModel;
        private Dictionary<int,List<ParkPoint>> _parkPointDic = new Dictionary<int, List<ParkPoint>>();   //接收区停车点key:itemid

        private List<ParkPoint> _waitParkPoints;//配送区停车点
        private List<IdleTruckModel> _waitTrucks = new List<IdleTruckModel>();//配送区等待的车辆
        private List<SpriteRenderer> _floorSpriteList;//地板父节点
        private Vector3[,] _waitParkArea ;
        private int _waitAreaRowPointCount = 2; //开放式停车场每行两个边界点
        private GameObject truckStation;
        private float lastPlayFxTime;

        private GameObject TruckStation
        {
            get
            {
                if(truckStation == null)
                {
                    truckStation = GameObject.FindGameObjectWithTag(Tags.TruckSite);
                }
                return truckStation;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            lastPlayFxTime = Time.time;
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Truck_ChangeMaxCount, UnLockPark);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Truck_ChangeVolumeLv, OnTruckVolumeGrade);
        }
        public void Init()
        {
            CreateOutSite();
            CityOutPoint();
            WaitParkPoint();
            WaitParkArea();
            InitFloor();
            CreateFeiji();
        }
        private void CreateFeiji()
        {
            IdleFeijiModel[] models = Resources.FindObjectsOfTypeAll<IdleFeijiModel>();
            for(int i=0;i < models.Length; i++)
            {
                IdleFeijiModel idleFeijiModel = models[i];
                idleFeijiModel.Init();
            }
        }

        private void InitFloor()
        {
            _floorSpriteList = new List<SpriteRenderer>();
            Transform floorNode = GameObject.Find("map/group0/floor/dizuanNode")?.transform;
            if (floorNode == null) return;
            for(int i=0;i<floorNode.childCount;i++)
            {
                SpriteRenderer sr = floorNode.GetChild(i).GetComponent<SpriteRenderer>();
                if(sr!=null)
                {
                    _floorSpriteList.Add(sr);
                }
            }
            SetFloor();
        }
        //设置地板
        public void SetFloor()
        {
            int storeLv = PlayerMgr.Instance.PlayerCity.storeLv;
            if (_floorSpriteList == null || _floorSpriteList.Count <= 0) return;
            IdleCityStoreVolume idleStoreVolume = IdleCityMgr.Instance.GetIdleStoreVolume(storeLv);
            if (idleStoreVolume == null) return;
            string floorResName = idleStoreVolume.floorRes;
            if (floorResName == _floorSpriteList[0].sprite.name) return;
            floorResName += ".png";
            Sprite sprite = AssetLoader.Load<Sprite>(floorResName);
            if (sprite == null) return;
            for(int i=0;i< _floorSpriteList.Count;i++)
            {
                _floorSpriteList[i].sprite = sprite;
            }
        }


        private void CreateOutSite()
        {
            GameObject outSite = GameObject.FindGameObjectWithTag(Tags.CityOutSite);
            IdleCityModel model = outSite.GetOrAddComponent<IdleCityModel>();
            model.Init(PlayerMgr.Instance.PlayerCity);
            _idleCityModel = model;
        }
        //出货点
        private void CityOutPoint()
        {
            GameObject outPointTrans = GameObject.FindGameObjectWithTag(Tags.CityOutPoint);
            for (int i = 0; i < outPointTrans.transform.childCount; i++)
            {
                Transform trans = outPointTrans.transform.GetChild(i);
                ParkPoint parkPoint = new ParkPoint();
                parkPoint.stopModel = trans.GetOrAddComponent<IdleTruckStopModel>();
                parkPoint.lockSprite = trans.Find("lock")?.GetComponent<SpriteRenderer>();
                parkPoint.ganSprite = trans.Find("gan")?.GetComponent<SpriteRenderer>();
                parkPoint.xianSprite = trans.Find("xian")?.GetComponent<SpriteRenderer>();
                parkPoint.stopModel.Init();
                IdleTruckParkModel parkModel = parkPoint.stopModel.GetComponent<IdleTruckParkModel>();
                if (parkModel == null) continue;
                if (_parkPointDic.ContainsKey(parkModel.ItemId))
                {
                    _parkPointDic[parkModel.ItemId].Add(parkPoint);
                }
                else
                {
                    List<ParkPoint> parks = new List<ParkPoint>();
                    parks.Add(parkPoint);
                    _parkPointDic[parkModel.ItemId] = parks;
                }
            }
            List<PlayerSite> sites = PlayerMgr.Instance.PlayerCity.playerSites;
            for(int i = 0; i < sites.Count; i++)
            {
                PlayerSite playerSite = sites[i];
                IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(sites[i].Id);
                //if (playerSite.isLock) continue;
                if(_parkPointDic.ContainsKey(idleSite.itemId))
                {
                    List<ParkPoint> parkPoints = _parkPointDic[idleSite.itemId];
                    for(int j = 0; j < parkPoints.Count; j++)
                    {
                        ParkPoint parkPoint = parkPoints[j];
                        if (parkPoint.unLock) continue;
                        if (!playerSite.isLock)
                        {
                            parkPoint.unLock = true;
                            if (parkPoint.lockSprite)
                                parkPoint.lockSprite.enabled = false;
                            if (parkPoint.ganSprite)
                                parkPoint.ganSprite.enabled = true;
                            if (parkPoint.xianSprite)
                                parkPoint.xianSprite.enabled = true;
                        }
                        else
                        {
                            parkPoint.unLock = false;
                            if (parkPoint.lockSprite)
                                parkPoint.lockSprite.enabled = true;
                            if (parkPoint.ganSprite)
                                parkPoint.ganSprite.enabled = false;
                            if (parkPoint.xianSprite)
                                parkPoint.xianSprite.enabled = false;
                        }

                    }
                }
            }
            
        }
        private void WaitParkPoint()
        {
            int row = 0;
            GameObject waitParkPoint = GameObject.FindGameObjectWithTag(Tags.WaitParkPoint);
            _waitParkPoints = new List<ParkPoint>();
            for (int i = 0; i < waitParkPoint.transform.childCount; i++)
            {
                Transform trans = waitParkPoint.transform.GetChild(i);
                ParkPoint parkPoint = new ParkPoint();
                parkPoint.stopModel = trans.GetOrAddComponent<IdleTruckStopModel>();
                parkPoint.lockSprite = trans.Find("lock").GetComponent<SpriteRenderer>();
                if (i < PlayerMgr.Instance.GetTruckNum())
                {
                    parkPoint.unLock = true;
                    parkPoint.lockSprite.enabled = false;
                }
                parkPoint.stopModel.Init();
                _waitParkPoints.Add(parkPoint);
                if (parkPoint.stopModel.RowId > row)
                    row = parkPoint.stopModel.RowId;
            }
            _waitParkArea = new Vector3[row+1, _waitAreaRowPointCount];
        }
        private void WaitParkArea()
        {
            GameObject waitParkArea = GameObject.FindGameObjectWithTag(Tags.WaitParkArea);
            for (int i = 0; i < waitParkArea.transform.childCount; i++)
            {
                Transform trans = waitParkArea.transform.GetChild(i);
                int row = i / _waitAreaRowPointCount;
                int col = i % _waitAreaRowPointCount;
                _waitParkArea[row,col] = trans.position;
            }
        }
        private void OnUnlockSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSiteModel> args = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            int siteId = args.param1.SiteId;
            IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(siteId);
            if (_parkPointDic.ContainsKey(idleSite.itemId))
            {
                List<ParkPoint> parkPoints = _parkPointDic[idleSite.itemId];
                for (int j = 0; j < parkPoints.Count; j++)
                {
                    ParkPoint parkPoint = parkPoints[j];
                    if (parkPoint.unLock) continue;
                    parkPoint.unLock = true;
                    if(parkPoint.lockSprite)
                        parkPoint.lockSprite.enabled = false;
                    if (parkPoint.ganSprite)
                        parkPoint.ganSprite.enabled = true;
                    if (parkPoint.xianSprite)
                        parkPoint.xianSprite.enabled = true;
                }
            }
        }
        public IdleTruckStopModel UseParkPoint(IdleTruckModel truckModel)
        {
            for(int i = 0; i < _waitParkPoints.Count; i++)
            {
                ParkPoint parkPoint = _waitParkPoints[i];
                if (!parkPoint.unLock) continue;
                if (!parkPoint.isUse )
                {
                    parkPoint.isUse = true;
                    parkPoint.truckModel = truckModel;
                    return parkPoint.stopModel;
                }
                    
            }
            LogUtility.LogError($"停车场已停满");
            return null;
        }

       
        private void OnTruckVolumeGrade(BaseEventArgs baseEventArgs)
        {
            if (Time.time - lastPlayFxTime < 0.3f) return;
            lastPlayFxTime = Time.time;
            FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleSiteGrade, TruckStation.transform.position, 0.35f);
            model.transform.localScale = Vector3.one * 15;
        }

        private void UnLockPark(BaseEventArgs args)
        {
            PlayerTruck playerTruck = PlayerMgr.Instance.GetPlayerTruck();
            int count = Mathf.Min(playerTruck.truckNum, _waitParkPoints.Count);
            for (int i = 0; i < count; i++)
            {
                ParkPoint parkPoint = _waitParkPoints[i];
                if (parkPoint.unLock) continue;
                parkPoint.unLock = true;
                parkPoint.lockSprite.enabled = false;
                FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleGradeFx, parkPoint.stopModel.transform.position-Vector3.up*0.2f,0.35f);
                model.transform.localScale = Vector3.one * 4;
                parkPoint.isUse = true;
                IdleTruckModel truckModel = IdleTruckCtrl.Instance.CreateTruck(parkPoint);
                
                //parkPoint.truckModel = truckModel;
            }
        }
        public IdleTruckStopModel GetUseParkPoint(IdleTruckModel truckModel,int itemId)
        {
            if (_parkPointDic.ContainsKey(itemId))
            {
                List<ParkPoint> parkPoints = _parkPointDic[itemId];
                for (int i = 0; i < parkPoints.Count; i++)
                {
                    ParkPoint parkPoint = parkPoints[i];
                    if (parkPoint.unLock && parkPoint.isUse && parkPoint.truckModel == truckModel)
                    {
                        return parkPoint.stopModel;
                    }
                }
            }
            LogUtility.LogError($"停车场已停满");
            return null;
        }
        public void RemoveWaitPoint(IdleTruckModel truckModel)
        {
            if (_waitTrucks.Contains(truckModel))
                _waitTrucks.Remove(truckModel);
            for (int i = 0; i < _waitParkPoints.Count; i++)
            {
                ParkPoint parkPoint = _waitParkPoints[i];
                if (!parkPoint.unLock) continue;
                if(parkPoint.isUse && parkPoint.truckModel == truckModel)
                {
                    parkPoint.isUse = false;
                    parkPoint.truckModel = null;
                    break;
                }
            }
        }
        public void RemoveParkPoint(IdleTruckModel truckModel)
        {
            var enumrator = _parkPointDic.GetEnumerator();
            while (enumrator.MoveNext())
            {
                int itemId = enumrator.Current.Key;
                List<ParkPoint> parkPoints = enumrator.Current.Value;
                for (int i = 0; i < parkPoints.Count; i++)
                {
                    ParkPoint parkPoint = parkPoints[i];
                    if (parkPoint.truckModel == truckModel)
                    {
                        parkPoint.isUse = false;
                        parkPoint.truckModel = null;
                        break;
                    }
                }
            }
            ChoiceParkPoint(new List<int>());
        }
        //获取返回等待区的卡车
        public List<IdleTruckModel> GetBackTrucks()
        {
            return _waitTrucks;
        }
        //卡车回到等待区   
        public void TruckBack(IdleTruckModel truckModel)
        {
            if (!_waitTrucks.Contains(truckModel))
                _waitTrucks.Add(truckModel);
            ChoiceParkPoint(new List<int>(),truckModel);
            //TruckLoadItem();
        }
        //货车卸货到仓库
        public void TruckUnloadItem(int itemId, int loadItemCount)
        {
            _idleCityModel.LoadItem(itemId, loadItemCount);
        }
        public void ChoiceParkPoint(List<int> itemIds,IdleTruckModel truckModel)
        {
            int maxItemCount = 0;
            int itemId = 0;
            Dictionary<int, int> storeItems = PlayerMgr.Instance.PlayerCity.storeItems;
            var enumrator = storeItems.GetEnumerator();
            while (enumrator.MoveNext())
            {
                if (itemIds.Contains(enumrator.Current.Key)) continue;
                var keyValue = enumrator.Current;
                if (keyValue.Value > maxItemCount)
                {
                    itemId = keyValue.Key;
                    maxItemCount = keyValue.Value;
                }
            }
            List<ParkPoint> parkPoints = null;
            if (itemId != 0)
            {
                if(_parkPointDic.ContainsKey(itemId))
                {
                    parkPoints = _parkPointDic[itemId];
                    bool isEmpty = false;  //是否有空位
                    for(int i = 0; i < parkPoints.Count; i++)
                    {
                        ParkPoint parkPoint = parkPoints[i];
                        if(parkPoint.unLock && !parkPoint.isUse)
                        {
                            parkPoint.isUse = true;
                            parkPoint.truckModel = truckModel;
                            truckModel.CityMove(parkPoint.stopModel);
                            isEmpty = true;
                            break;
                        }
                    }
                    if(!isEmpty)
                    {
                        itemIds.Add(itemId);
                        ChoiceParkPoint(itemIds, truckModel);
                    }
                }
            }
            else
            {
                var entor = _parkPointDic.GetEnumerator();
                while (entor.MoveNext())
                {
                    parkPoints = entor.Current.Value;
                    bool isEmpty = false;  //是否有空位
                    for (int i = 0; i < parkPoints.Count; i++)
                    {
                        ParkPoint parkPoint = parkPoints[i];
                        if (parkPoint.unLock && !parkPoint.isUse)
                        {
                            parkPoint.isUse = true;
                            parkPoint.truckModel = truckModel;
                            truckModel.CityMove(parkPoint.stopModel);
                            isEmpty = true;
                            break;
                        }
                    }
                    if (isEmpty)
                    {
                        break;
                    }
                }
            }
        }
        private void ChoiceParkPoint(List<int> itemIds)
        {
            for(int i =0;i< _waitTrucks.Count; i++)
            {
                IdleTruckModel truckModel = _waitTrucks[i];
                ChoiceParkPoint(itemIds, truckModel);
            }
        }
        private List<int> itemIds = new List<int>();
        public void TruckLoadItem()
        {
            itemIds.Clear();
            var enumrator = _parkPointDic.GetEnumerator();
            while (enumrator.MoveNext())
            {
                int itemId = enumrator.Current.Key;
                List<ParkPoint> parkPoints = enumrator.Current.Value;
                Dictionary<int, int> storeItems = PlayerMgr.Instance.PlayerCity.storeItems;
                if (storeItems.ContainsKey(itemId))
                {
                    for(int i = 0; i < parkPoints.Count; i++)
                    {
                        ParkPoint parkPoint = parkPoints[i];
                        if(parkPoint.unLock && parkPoint.isUse && parkPoint.truckModel != null)
                        {
                            if (parkPoint.truckModel.GetTruckState() != 0) continue;
                            int truckVolume = parkPoint.truckModel.GetVolume();
                            truckVolume = truckVolume < storeItems[itemId] ? truckVolume : storeItems[itemId];
                            if (truckVolume <= 0) continue;
                            List<IdleSiteModel> idleSiteModels = IdleSiteCtrl.Instance.GetIdleSiteModelsByItemId(itemId);
                            int index = Random.Range(0, idleSiteModels.Count);
                            IdleSiteModel model = idleSiteModels[index];
                            parkPoint.truckModel.LoadItem(model, truckVolume);
                            parkPoint.truckModel.Move();
                            if (parkPoint.truckModel.IsMove)
                            {
                                EventDispatcher.Instance.TriggerEvent(new EventArgsThree<IdleTruckModel, int, int>(EnumEventType.Event_Idle_TruckLoad, parkPoint.truckModel, itemId, truckVolume));
                            }
                        }
                    }
                }
            }
            
        }
        public void UnLoadItem(int itemId,int truckVolume)
        {
            _idleCityModel.UnLoadItem(itemId, truckVolume);
        }
        //private void TruckLoadItem()
        //{                     
        //    for (int i = 0; i < _backTrucks.Count; i++)
        //    {
        //        IdleTruckModel truckModel = _backTrucks[i];
        //        string[] itemIds = truckModel.IdleTruck.itemId.Split(',');
        //        for(int j = 0; j < itemIds.Length; j++)
        //        {
        //            int itemId = int.Parse(itemIds[j]);
        //            Long2 num = _idleCityModel.GetItemNum(itemId);
        //            if (num <= Long2.zero) continue;
        //            Long2 truckVolume = truckModel.GetVolume();
        //            if (truckVolume <= Long2.zero) break;
        //            List<IdleSiteModel> idleSiteModels = IdleSiteCtrl.Instance.GetIdleSiteModelsByItemId(itemId);
        //            if (idleSiteModels.Count <= 0) continue;
        //            IdleSiteModel model = idleSiteModels[0];
        //            truckVolume = truckVolume < num ? truckVolume : num;
        //            truckModel.LoadItem(model, truckVolume);
        //            _idleCityModel.UnLoadItem(itemId, truckVolume); 
        //        }
        //        truckModel.Move();
        //        if (truckModel.IsMove)
        //        {
        //            _backTrucks.Remove(truckModel);
        //            i--;
        //        }


        //    }
        //}



        ////加载资源
        //public T LoadAsset<T>(string assetName) where T : Object
        //{
        //    if(!_resAssets.ContainsKey(assetName))
        //    {
        //        AssetRequest request = Assets.LoadAsset(assetName, typeof(T));
        //        if (request == null) return null;
        //        _resAssets.Add(assetName, request);
        //    }
        //    return _resAssets[assetName].asset as T;
        //}

        ////释放所有资源(跳转场景时调用)
        //public void ReleaseAllAssets()
        //{
        //    if (_resAssets == null || _resAssets.Count <= 0) return;
        //    List<AssetRequest> assetsList = _resAssets.Values.ToList();
        //    for(int i=0;i<assetsList.Count;i++)
        //    {
        //        assetsList[i].Release();
        //    }
        //    _resAssets.Clear();
        //    assetsList.Clear();
        //}
        /// <summary>
        /// 是否是等待区的点
        /// </summary>
        /// <param name="stopModel"></param>
        /// <returns></returns>
        public bool IsWaitParkPoint(IdleTruckStopModel stopModel) 
        {
            for(int i = _waitParkPoints.Count-1; i >=0; i--){
                ParkPoint parkPoint = _waitParkPoints[i];
                if (parkPoint.stopModel == stopModel)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取停车场左进入点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLeftEnterAreaPoint()
        {
            return _waitParkArea[0, 0];
        }
        /// <summary>
        /// 获取停车场右进入点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRightEnterAreaPoint()
        {
            return _waitParkArea[0, 1];
        }
        /// <summary>
        /// 进入停车场
        /// </summary>
        /// <param name="stopModel"></param>
        /// <param name="isLeft">是否是从左边进入</param>
        /// <returns></returns>

        public List<Vector3> EnterPark(IdleTruckStopModel stopModel,bool isLeft)
        {
            int rowId = stopModel.RowId;
            if (rowId == 0) return null;
            List<Vector3> points = new List<Vector3>();
            if (isLeft)
            {
                points.Add(_waitParkArea[0, 0]);
                points.Add(_waitParkArea[rowId, 0]);
            }
            else
            {
                points.Add(_waitParkArea[0, 1]);
                points.Add(_waitParkArea[rowId, 1]);
            }
            Vector3 p1 = _waitParkArea[rowId, 0];
            Vector3 v1 = _waitParkArea[rowId, 1] - p1;
            Vector3 p2 = stopModel.GetPoint();
            Vector3 v2 = stopModel.GetVector();
            Vector3 p = Vector3.zero;
            if (Utils.IsLineCross(out p, p1, v1, p2, v2))
                points.Add(p);
            return points;
        }
        public List<Vector3> OutPark(IdleTruckStopModel stopModel, bool isLeft)
        {
            int rowId = stopModel.RowId;
            if (rowId == 0) return null;
            List<Vector3> points = new List<Vector3>();
            Vector3 p1 = _waitParkArea[rowId, 0];
            Vector3 v1 = _waitParkArea[rowId, 1] - p1;
            Vector3 p2 = stopModel.GetPoint();
            Vector3 v2 = stopModel.GetVector();
            Vector3 p = Vector3.zero;
            if (Utils.IsLineCross(out p, p1, v1, p2, v2))
                points.Add(p);
            if (isLeft)
            {
                points.Add(_waitParkArea[rowId, 0]);
                points.Add(GetLeftEnterAreaPoint());
                
            }
            else
            {
                points.Add(_waitParkArea[rowId, 1]);
                points.Add(GetRightEnterAreaPoint());
            }
            return points;
        }
        public override void Dispose()
        {
            _idleCityModel = null;
            _parkPointDic.Clear();
            _waitParkPoints = null;
            _waitTrucks.Clear();
            _floorSpriteList = null;//地板父节点
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Truck_ChangeMaxCount, UnLockPark);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Truck_ChangeVolumeLv, OnTruckVolumeGrade);
        }

    }
}

