using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using libx;

namespace Delivery.Idle
{
    public class IdleCityTruckCtrl : MonoSingleton<IdleCityTruckCtrl>
    {
        private struct ParkState
        {
            public bool isUnLock;//是否解锁 
            public bool beUsed;//是否有车停
            public int index;//停车位索引
        }

        private List<int> itemIds;//已解锁货物id
        private List<int> toAddItemIds;//新增货物id
        private List<Vector3> goPoints;//送货路径
        private List<Vector3> backPoints;//返回路径
        private List<Vector3> parkPoints;//停车点
        //private Dictionary<int, GameObject> lockLogoDic;//锁标志对象
        private Transform lockPointsNode;//停车位解锁点
        private Dictionary<int, Transform> lockPoints;//所有停车位解锁点
        
        private Transform goPointsNode;//送货路线点父节点
        private Transform backPointsNode;//返回路线点父节点
        private Transform parkPointsNode;//停车点父节点
        private Vector3 startPos;
        private Vector3 endPos;
        private Dictionary<int, ParkState> cityTruckParks;//卡车停车位<货物id，是否已被占用>
        private AssetRequest lockAsset;
        private GameObject lockLogoPrefab;

        private float diffTruckTime = 3;
        private float sameTruckTime = 5;
        private float truckLevelUpFxTime;//上次播放升级特效时间
        private float minFxTime=0.15f;//播放特效的时间间隔

        private bool initCreate; //是否创建完所有已存在的货车
        private bool isCreate;//是否正在创建货车
        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }
        public void Init()
        {
            lockPointsNode = GameObject.Find("map/cityTruckPoints/lockPoints").transform;
            itemIds = new List<int>();
            toAddItemIds = new List<int>();
            //lockLogoDic = new Dictionary<int, GameObject>();
            lockPoints = new Dictionary<int, Transform>();
            List<PlayerSite> sites = PlayerMgr.Instance.PlayerCity.playerSites;
            cityTruckParks = new Dictionary<int, ParkState>();
            for (int i=0;i<sites.Count;i++)
            {
                IdleSite site = IdleSiteMgr.Instance.GetIdleSite(sites[i].Id);
                //初始化停车位锁标志节点
                string childName = "point" + site.itemId.ToString();
                Transform child = lockPointsNode.Find(childName);
                if(!lockPoints.ContainsKey(site.itemId))
                    lockPoints.Add(site.itemId, child);

                if (!sites[i].isLock)
                {
                    if(!itemIds.Contains(site.itemId))
                    {
                        itemIds.Add(site.itemId);
                        //添加停车位信息
                        ParkState state = new ParkState();
                        state.isUnLock = true;
                        state.beUsed = false;
                        state.index = site.itemId-1;
                        //cityTruckParks.Add(i, state);
                        cityTruckParks.Add(site.itemId, state);
                    }   
                }
                else
                {
                    if (!itemIds.Contains(site.itemId))
                    {
                        if(child.childCount<=0)
                        {
                            GameObject lockLogo = CreateLockLogo();
                            lockLogo.name = "lock";
                            lockLogo.transform.SetParent(child, true);
                            Vector3 pos = Vector3.zero;
                            pos.z = 1;
                            lockLogo.transform.localPosition = pos;

                            SpriteRenderer sr = lockLogo.GetComponent<SpriteRenderer>();
                            sr.sortingOrder = -(int)(lockLogo.transform.position.y * 10);

                        }
                       
                        //lockLogoDic.Add(site.itemId, lockLogo);
                    }


                }
            }
            InitPoints();

            IdleCity city = IdleCityMgr.Instance.GetIdleCityById();
            diffTruckTime = city.diffTruckTime;
            sameTruckTime = city.sameTruckTime;
            truckLevelUpFxTime = Time.time;

            
            initCreate = false;
            StartCoroutine(CreateDiffTruck(itemIds,true));
        }
        
        private void InitPoints()
        {
            goPoints = new List<Vector3>();
            backPoints = new List<Vector3>();
            parkPoints = new List<Vector3>();
            goPointsNode = GameObject.Find("map/cityTruckPoints/goPoints").transform;
            backPointsNode = GameObject.Find("map/cityTruckPoints/backPoints").transform;
            parkPointsNode = GameObject.Find("map/cityTruckPoints/parkPoints").transform;
            for (int i=0;i<goPointsNode.childCount;i++)
            {
                goPoints.Add(goPointsNode.GetChild(i).position);
            }
            for(int i=0;i<backPointsNode.childCount;i++)
            {
                backPoints.Add(backPointsNode.GetChild(i).position);
            }
            for(int i=0;i<parkPointsNode.childCount;i++)
            {
                parkPoints.Add(parkPointsNode.GetChild(i).position);
            }
            startPos = GameObject.Find("map/cityTruckPoints/startPoint").transform.position;
            endPos = GameObject.Find("map/cityTruckPoints/endPoint").transform.position;
        }


        /// <summary>
        /// 创建装载不同货物的卡车
        /// </summary>
        private IEnumerator CreateDiffTruck(List<int> itemIdsList, bool isFirst = false)
        {
            isCreate = true;
            int index = 0;
            if(isFirst)
            {
                int firstItemId = itemIdsList[0];
                CreateTruck(firstItemId);
                //itemIdsList.RemoveAt(0);
                //itemIdsList.Add(firstItemId);
            }
            while (index< itemIdsList.Count)
            {
                if (!isFirst)
                    yield return new WaitForSeconds(diffTruckTime);
                //Timer.Instance.Register(sameTruckTime,-1, CreateTruck, itemIdsList[index]).AddTo(gameObject);
                Timer.Instance.Register(sameTruckTime,1, CreateTruck, itemIdsList[index]).AddTo(gameObject);
                index++;
               if(isFirst)
                    yield return new WaitForSeconds(diffTruckTime);
            }
            if (!initCreate)
                initCreate = true;
            else
            {
                for(int i=0;i<itemIdsList.Count;i++)
                {
                    if(!itemIds.Contains(itemIdsList[i]))
                    {
                        itemIds.Add(itemIdsList[i]);
                    }
                }
                
                itemIdsList.Clear();
            }
            isCreate = false;
        }

        //解锁驿站时添加对应的货物id
        private void OnUnlockSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSiteModel> args = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            int siteId = args.param1.SiteId;
            IdleSite site = IdleSiteMgr.Instance.GetIdleSite(siteId);
            if (site == null) return;
            if(!itemIds.Contains(site.itemId))
            {
                toAddItemIds.Add(site.itemId);
            }
            //UnlockTruckPark();
            UnlockTruckPark(site.itemId);
        }
        
        //解锁卡车停车位
        private void UnlockTruckPark(int itemId)
        {
            int index = itemIds.Count;
            ParkState state = new ParkState();
            state.isUnLock = true;
            state.beUsed = false;
            state.index = index;
            if(!cityTruckParks.ContainsKey(itemId))
            {
                cityTruckParks.Add(itemId, state);
            }
           
            if (lockPoints.ContainsKey(itemId))
            {
                for(int i=0;i<lockPoints[itemId].childCount;i++)
                {
                    Destroy(lockPoints[itemId].GetChild(i).gameObject);
                }
            }
        }

        //创建送货车
        private void CreateTruck(params object[] objs)
        {
            //当有新的驿站解锁时，创建新的货车
            if (!isCreate && toAddItemIds.Count > 0)
            {
                StartCoroutine(CreateDiffTruck(toAddItemIds));
            }
            int itemId = (int)objs[0];
            int currentParkIndex = GetEnableParkIndex(itemId);
            if (currentParkIndex < 0) return;
            Transform sky = GameObject.Find("map/sky").transform;
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool("idleCityTruck.prefab", true, 1);
            obj.transform.SetParent(sky);
            obj.transform.position = Vector3.zero;
            IdleSendTruckModel sendTruckModel = obj.GetOrAddComponent<IdleSendTruckModel>();

            sendTruckModel.Init(itemId, currentParkIndex);
            sendTruckModel.StartSendItem();
            //TruckInPark(currentParkIndex);
            TruckInPark(itemId);
        }

   
        public void TruckInPark(int itemId)
        {
            if (cityTruckParks.ContainsKey(itemId))
            {
                ParkState state = cityTruckParks[itemId];
                state.beUsed = true;
                cityTruckParks[itemId] = state;
            }
        }


        public void TruckOutPark(int itemId)
        {
            if (cityTruckParks.ContainsKey(itemId))
            {
                ParkState state = cityTruckParks[itemId];
                state.beUsed = false;
                cityTruckParks[itemId] = state;
                Timer.Instance.Register(sameTruckTime, 1, CreateTruck, itemId).AddTo(gameObject);
            }
        }


        ////获取可用停车位索引
        //private int GetEnableParkIndex()
        //{
        //    var enumerator = cityTruckParks.GetEnumerator();
        //    while(enumerator.MoveNext())
        //    {
        //        var keyvalue = enumerator.Current;
        //        if(!keyvalue.Value.beUsed)
        //        {
        //            return keyvalue.Key;
        //        }
        //    }
        //    return -1;
        //}

        //获取可用停车位索引
        private int GetEnableParkIndex(int itemId)
        {
            int index = -1;
            if(cityTruckParks.ContainsKey(itemId))
            {
                ParkState state = cityTruckParks[itemId];
                if (state.isUnLock && !state.beUsed)
                    index = state.index;
            }
            return index;
        }


        public Vector3 StartPos
        {
            get
            {
                return startPos;
            }
        }
        public Vector3 EndPos
        {
            get
            {
                return endPos;
            }
        }

        public Vector3 GetForwardPos(int index)
        {
            Vector3 pos = Vector3.zero;
            if (index < 0 || index > goPoints.Count - 1)
                return pos;
            pos = goPoints[index];
            return pos;
        }

        public Vector3 GetbackPos(int index)
        {
            Vector3 pos = Vector3.zero;
            if (index < 0 || index > backPoints.Count - 1)
                return pos;
            pos = backPoints[index];
            return pos;
        }
        public Vector3 GetParkPos(int index)
        {
            Vector3 pos = Vector3.zero;
            if (index < 0 || index > parkPoints.Count - 1)
                return pos;
            pos = parkPoints[index];
            return pos;
        }
        //创建停车位锁的标志
        private GameObject CreateLockLogo()
        {
            if(lockAsset == null)
            {
                lockAsset = Assets.LoadAsset("parkLock.prefab", typeof(GameObject));
                lockLogoPrefab = lockAsset.asset as GameObject;
            }
            return Instantiate(lockLogoPrefab);
        }

        ////送到快递中心的车辆容量升级
        //private void OnTruckVolumeLevelUp(BaseEventArgs baseEventArgs)
        //{
        //    if (Time.time - truckLevelUpFxTime <= minFxTime) return;
        //    truckLevelUpFxTime = Time.time;
        //   for(int i=0;i<itemIds.Count;i++)
        //    {
        //        if(lockPoints.ContainsKey(itemIds[i]))
        //        {
        //            FxCtrl.Instance.PlayFx(FxPrefabPath.idleSiteGrade, lockPoints[itemIds[i]], true, 0.5f);
        //        }
        //    }
        //}

        public override void Dispose()
        {
            base.Dispose();
            itemIds = null;//已解锁货物id
            toAddItemIds = null;//新增货物id
            goPoints = null;//送货路径
            backPoints = null;//返回路径
            parkPoints = null;//停车点
            lockPointsNode = null;//停车位解锁点
            lockPoints = null;//所有停车位解锁点
            goPointsNode = null;//送货路线点父节点
            backPointsNode = null;//返回路线点父节点
            parkPointsNode = null;//停车点父节点
            startPos = Vector3.zero;
            endPos = Vector3.zero;
            cityTruckParks = null;//卡车停车位<货物id，是否已被占用>
            lockLogoPrefab = null;
            truckLevelUpFxTime = 0;//上次播放升级特效时间
            initCreate = false; //是否创建完所有已存在的货车
            isCreate = false;//是否正在创建货车
            lockAsset = null;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnUnlockSite);
        }

    }
}

