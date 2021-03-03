using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Foundation;
using Delivery.Idle;
using Delivery.Track;
using System;

namespace Delivery
{
    public class PlayerMgr : MonoSingleton<PlayerMgr>
    {
        private Player _player;
        private IdleCity _idleCity;//当前城市
        private bool _isNewPlayer = true;

        public bool IsNewPlayer
        {
            get
            {
                return _isNewPlayer;
            }
        }

        public void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.hashPlayer), this, (jsonStr) => {
                if (!string.IsNullOrEmpty(jsonStr))
                {
                    _player = FullSerializerAPI.Deserialize(typeof(Player), jsonStr) as Player;
                    PlayerCity.loginTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);
                    if (PlayerCity.leaveTime == 0)
                        PlayerCity.leaveTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);
                    _isNewPlayer = false;
                }
                else
                {
                    CreateNewPlayer(); 
                }
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_PlayerData));
            });
        }
        private void CreateNewPlayer()
        {
            _player = new Player();
            _player.cityId = 1001;
            _player.playerCity = new Dictionary<int, PlayerCity>();
            _player.firstLoginTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);
        }
        //第一次进来创建玩家数据
        public void CreatePlayer()
        {
            if (PlayerCity == null)
            {
                PlayerCity playerCity = new PlayerCity();
                _player.playerCity.Add(_player.cityId, playerCity);
                playerCity.cityId = _player.cityId;
                IdleCity idleCity = IdleCityMgr.Instance.GetIdleCityById(_player.cityId);
                //playerCity.loadItemNum = new Long2(idleCity.initialItem);
                playerCity.cityTruckVolumeLv = idleCity.volumeLv;
                playerCity.cycleLv = idleCity.cycleLv;
                playerCity.money = new Long2(idleCity.money);

                playerCity.playerTruck = new PlayerTruck();
                playerCity.playerTruck.truckLv = 1;
                playerCity.playerTruck.truckNum = idleCity.truckNum;

                playerCity.storeLv = 1;
                playerCity.storeItems = new Dictionary<int, int>();//初始化仓库存储
                string[] initialItems = idleCity.initialItem.Split(';');
                for (int i = 0; i < initialItems.Length; i++)
                {
                    string initial = initialItems[i];
                    string[] tial = initial.Split(':');
                    int itemId = int.Parse(tial[0]);
                    int long2 = int.Parse(tial[1]);
                    playerCity.storeItems.Add(itemId, long2);
                }

                List<IdleSite> sites = IdleSiteMgr.Instance.GetIdleSites(_player.cityId);
                playerCity.playerSites = new List<PlayerSite>();
                for (int i = 0; i < sites.Count; i++)
                {
                    IdleSite idleSite = sites[i];
                    PlayerSite playerSite = new PlayerSite();
                    playerSite.Id = idleSite.Id;
                    playerSite.cityId = idleSite.cityId;
                    playerSite.isLock = idleSite.isLock;
                    playerSite.siteBaseLv = idleSite.siteBaseLv;
                    playerSite.siteTimeLv = idleSite.siteTimeLv;
                    playerSite.siteVolumeLv = idleSite.siteVolumeLv;
                    playerSite.loadItemNum = 0;
                    playerCity.playerSites.Add(playerSite);
                }

                //初始化普通任务
                playerCity.playerTasks = new List<PlayerTask>();
                List<IdleTask> tasks = IdleTaskMgr.Instance.GetIdleTasks(_player.cityId);
                for (int i = 0; i < tasks.Count; i++)
                {
                    IdleTask idletask = tasks[i];
                    PlayerTask playerTask = new PlayerTask();
                    playerTask.Id = idletask.Id;
                    playerTask.IsFinish = false;
                    playerCity.playerTasks.Add(playerTask);
                }
                //初始化主线任务
                playerCity.playerMainTasks = new List<PlayerTask>();
                List<IdleMainTask> mainTasks = IdleTaskMgr.Instance.GetIdleMainTasks(_player.cityId);
                for(int i=0;i<mainTasks.Count;i++)
                {
                    IdleMainTask idleMainTask = mainTasks[i];
                    PlayerTask playerTask = new PlayerTask();
                    playerTask.Id = idleMainTask.Id;
                    playerTask.IsFinish = false;
                    playerCity.playerMainTasks.Add(playerTask);
                }

                

                if(_player.playerGuide == null)
                {
                    Dictionary<int, IdleGuide> idleGuides = IdleGuideMgr.Instance.GetAllIdleGuides();
                    _player.playerGuide = new Dictionary<int, PlayerGuide>();
                    var enumerator = idleGuides.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        IdleGuide idleGuide = enumerator.Current.Value;
                        PlayerGuide playerGuide = new PlayerGuide();
                        playerGuide.id = idleGuide.Id;
                        string firstStep = idleGuide.GuideStepIds.Split(',')[0];
                        playerGuide.hasStart = false;
                        _player.playerGuide.Add(playerGuide.id, playerGuide);
                    }
                }
                
            }
            PlayerCity.loginTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);
            if (PlayerCity.leaveTime == 0)
                PlayerCity.leaveTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);

        }
        //初始化玩家数据上报
        public void TAEventPropertie()
        {
            Dictionary<string, string> proertie = new Dictionary<string, string>();
            proertie["cityId"] = PlayerCity.cityId.ToString();
            proertie["money"] = UnitConvertMgr.Instance.GetFloatValue(PlayerCity.money,2);
            proertie["cityTruckVolumeLv"] = PlayerCity.cityTruckVolumeLv.ToString();   //快递中心卡车等级
            proertie["storeLv"] = PlayerCity.storeLv.ToString();   //快递中心仓库容量等级
            proertie["siteTruckNum"] = PlayerCity.playerTruck.truckNum.ToString();   //小卡车（驿站卡车）数量
            proertie["siteTruckLv"] = PlayerCity.playerTruck.truckLv.ToString();   //小卡车（驿站卡车）等级
            PlatformFactory.Instance.TAEventPropertie("gt_init_info", proertie);
            
            for (int i = 0; i < PlayerCity.playerSites.Count; i++)
            {
                PlayerSite playerSite = PlayerCity.playerSites[i];
                if (!playerSite.isLock)
                {
                    Dictionary<string, string> siteProertie = new Dictionary<string, string>();
                    siteProertie["cityId"] = PlayerCity.cityId.ToString();
                    siteProertie["siteId"] = playerSite.Id.ToString();
                    siteProertie["siteBaseLv"] = playerSite.siteBaseLv.ToString();
                    siteProertie["siteTimeLv"] = playerSite.siteTimeLv.ToString();
                    siteProertie["siteVolumeLv"] = playerSite.siteVolumeLv.ToString();
                    PlatformFactory.Instance.TAEventPropertie("gt_init_info_site", siteProertie);
                }   
            }
                
            
            
            
        }


        public Player Player { get { return _player; } }
        public int CityId { 
            get { return _player.cityId; }
            set { if (_player != null) _player.cityId = value; }
        }
        public PlayerCity PlayerCity
        {
            get
            {
                if (_player == null || _player.playerCity == null) return null;
                if (_player.playerCity.ContainsKey(_player.cityId))
                    return _player.playerCity[_player.cityId];
                else
                    return null;    
            }
        }  

        public PlayerCity CreatePlayerCity(int cityId)
        {
            PlayerCity playerCity = new PlayerCity();
            _player.playerCity.Add(cityId, playerCity);
            playerCity.cityId = cityId;
            IdleCity idleCity = IdleCityMgr.Instance.GetIdleCityById(cityId);
            //playerCity.loadItemNum = new Long2(idleCity.initialItem);
            playerCity.cityTruckVolumeLv = idleCity.volumeLv;
            playerCity.cycleLv = idleCity.cycleLv;
            playerCity.money = new Long2(idleCity.money);

            playerCity.playerTruck = new PlayerTruck();
            playerCity.playerTruck.truckLv = 1;
            playerCity.playerTruck.truckNum = idleCity.truckNum;

            playerCity.storeLv = 1;
            playerCity.storeItems = new Dictionary<int, int>();//初始化仓库存储
            string[] initialItems = idleCity.initialItem.Split(';');
            for (int i = 0; i < initialItems.Length; i++)
            {
                string initial = initialItems[i];
                string[] tial = initial.Split(':');
                int itemId = int.Parse(tial[0]);
                int long2 = int.Parse(tial[1]);
                playerCity.storeItems.Add(itemId, long2);
            }

            //List<IdleSite> sites = IdleSiteMgr.Instance.GetIdleSites(cityId);
            //playerCity.playerSites = new List<PlayerSite>();
            //for (int i = 0; i < sites.Count; i++)
            //{
            //    IdleSite idleSite = sites[i];
            //    PlayerSite playerSite = new PlayerSite();
            //    playerSite.Id = idleSite.Id;
            //    playerSite.cityId = idleSite.cityId;
            //    playerSite.isLock = idleSite.isLock;
            //    playerSite.siteBaseLv = idleSite.siteBaseLv;
            //    playerSite.siteTimeLv = idleSite.siteTimeLv;
            //    playerSite.siteVolumeLv = idleSite.siteVolumeLv;
            //    playerSite.loadItemNum = 0;
            //    playerCity.playerSites.Add(playerSite);
            //}

            //初始化普通任务
            playerCity.playerTasks = new List<PlayerTask>();
            List<IdleTask> tasks = IdleTaskMgr.Instance.GetIdleTasks(cityId);
            for (int i = 0; i < tasks.Count; i++)
            {
                IdleTask idletask = tasks[i];
                PlayerTask playerTask = new PlayerTask();
                playerTask.Id = idletask.Id;
                playerTask.IsFinish = false;
                playerCity.playerTasks.Add(playerTask);
            }
            //初始化主线任务
            playerCity.playerMainTasks = new List<PlayerTask>();
            List<IdleMainTask> mainTasks = IdleTaskMgr.Instance.GetIdleMainTasks(cityId);
            for (int i = 0; i < mainTasks.Count; i++)
            {
                IdleMainTask idleMainTask = mainTasks[i];
                PlayerTask playerTask = new PlayerTask();
                playerTask.Id = idleMainTask.Id;
                playerTask.IsFinish = false;
                playerCity.playerMainTasks.Add(playerTask);
            }

            playerCity.loginTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);
            if (playerCity.leaveTime == 0)
                playerCity.leaveTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);

            return playerCity;
        }
        //转生后调用
        public void CreatePlayerSites(List<IdleSite>idleSites)
        {
            if (PlayerCity==null || PlayerCity.playerSites != null) return;
            PlayerCity.playerSites = new List<PlayerSite>();
            for (int i = 0; i < idleSites.Count; i++)
            {
                IdleSite idleSite = idleSites[i];
                PlayerSite playerSite = new PlayerSite();
                playerSite.Id = idleSite.Id;
                playerSite.cityId = idleSite.cityId;
                playerSite.isLock = idleSite.isLock;
                playerSite.siteBaseLv = idleSite.siteBaseLv;
                playerSite.siteTimeLv = idleSite.siteTimeLv;
                playerSite.siteVolumeLv = idleSite.siteVolumeLv;
                playerSite.loadItemNum = 0;
                PlayerCity.playerSites.Add(playerSite);
            }
        }


        public PlayerCity GetPlayerCity(int cityId)
        {
            if (cityId == -1)
                cityId = CityId;
            if (_player.playerCity.ContainsKey(cityId))
                return _player.playerCity[cityId];
            else
            {
                LogUtility.LogError($"没有当前城市{cityId}");
                return null;
            }
        }

        /// <summary>
        /// 获取驿站配送速度
        /// </summary>
        public Long2 GetSiteDeliverSpeed(PlayerSite playerSite)
        {
            IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id, playerSite.cityId);
            IdleSiteBase siteBase = IdleSiteMgr.Instance.GetSiteBase(playerSite.Id, playerSite.siteBaseLv);
            IdleSiteGrade siteGrade = IdleSiteMgr.Instance.GetSiteGradeBySiteId(playerSite.Id, playerSite.siteBaseLv);
            IdleSiteTime siteTime = IdleSiteMgr.Instance.GetSiteTime(playerSite.Id, playerSite.siteTimeLv);
            Long2 itemNum = new Long2(siteBase.itemnum);
            Long2 speed = itemNum * siteGrade.staffnum * 60 / ((8 / siteTime.speed) * 2 + siteTime.qtime);
            return speed;
        }

        //获取驿站收益(默认一分钟)
        public Long2 GetSiteIncome(PlayerSite playerSite,float t = 60)
        {
            PlayerCity playerCity = GetPlayerCity(playerSite.cityId);
            IdleCity idleCity = IdleCityMgr.Instance.GetIdleCityById(playerCity.cityId);
            IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id, playerSite.cityId);
            IdleSiteBase siteBase = IdleSiteMgr.Instance.GetSiteBase(playerSite.Id, playerSite.siteBaseLv);
            IdleSiteGrade siteGrade = IdleSiteMgr.Instance.GetSiteGradeBySiteId(playerSite.Id, playerSite.siteBaseLv);
            IdleSiteTime siteTime = IdleSiteMgr.Instance.GetSiteTime(playerSite.Id, playerSite.siteTimeLv);
            //Long2 itemNum = new Long2(siteBase.itemnum);
            int itemNum = siteBase.itemnum;
            Long2 price = new Long2(siteBase.value);
            Long2 multi = new Long2(siteGrade.totalmultiple);
            Long2 cityTotalMulti = new Long2(idleCity.totalmultiple);
            //Long2 unitPrice = itemNum * price * multi * cityTotalMulti;//单次送货价格
            Long2 unitPrice = price * itemNum * multi * cityTotalMulti;//单次送货价格
            float time = t / ((8 / siteTime.speed)*2 + siteTime.qtime);   //送货次数
            Long2 totalPrice = unitPrice * time * siteGrade.staffnum; //总送货价格
            return totalPrice;
        }

        //获取城市一段时间内收益
        public Long2 GetCityIncome(int cityId = -1,float time=60)
        {
            if (cityId < 0)
                cityId = CityId;
            PlayerCity playerCity = GetPlayerCity(cityId);
            Long2 totalIncome = Long2.zero;
            for (int i = 0; i < playerCity.playerSites.Count; i++)
            {
                PlayerSite playerSite = playerCity.playerSites[i];
                if (playerSite.isLock) continue;
                Long2 price = GetSiteIncome(playerSite, time);
                totalIncome += price;
            }
            return totalIncome;
        }

        public Long2 OffLineIncome(int cityId = -1)
        {
            if (cityId == -1)
                cityId = CityId;
            DateTime time = DateTime.UtcNow;
            long nowTime = TimeUtils.ConvertLongUtcDateTime(time);
            PlayerCity playerCity = GetPlayerCity(cityId);
            long leaveTime = playerCity.leaveTime;
            long t = nowTime - leaveTime;
            t = t < Config.leaveTime ? t : Config.leaveTime;
            t = Math.Max(0, t);
            Long2 totalIncome = Long2.zero;
            for (int i = 0; i < playerCity.playerSites.Count; i++)
            {
                PlayerSite playerSite = playerCity.playerSites[i];
                if (playerSite.isLock) continue;
                Long2 price = GetSiteIncome(playerSite, t);
                totalIncome += price;
            }
            return totalIncome;
        }
        public void AddOffLineIncome()
        {

            Long2 offLineIncome = OffLineIncome(PlayerCity.cityId);
            PlayerCity.money += offLineIncome;
        }
        public void AddMoney(Long2 addMoney)
        {
            if (PlayerCity == null) return;
            PlayerCity.money += addMoney;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_Guide_StartGuide,2));
        }
        public bool IsEnoughMoney(Long2 money)
        {
            if (money <= PlayerCity.money)
                return true;
            else
                return false;
        }
        public void CutMoney(Long2 cutMoney)
        {
            if( IsEnoughMoney(cutMoney))
            {
                PlayerCity.money -= cutMoney;
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyReduce));
            }
        }
        #region 卡车

        public PlayerTruck GetPlayerTruck(int cityId = -1)
        {
            return  GetPlayerCity(cityId).playerTruck;
        }
        public int GetTruckNum(int cityId = -1)
        {
            if (cityId == -1)
                cityId = CityId;
            return GetPlayerTruck(cityId).truckNum;
        }
        //public List<PlayerTruck> GetPlayerTrucks(int cityId = -1)
        //{
        //    if (cityId == -1)
        //        cityId = CityId;
        //    PlayerCity playerCity = GetPlayerCity(cityId);
        //    return playerCity.playerTrucks;
        //}

        //public List<IdleTruck> GetIdleTrucks(int cityId = -1)
        //{
        //    if (cityId == -1)
        //        cityId = _player.cityId;
        //    List<IdleTruck> idleTrucks = new List<IdleTruck>();
        //    List<PlayerTruck> playerTrucks = GetPlayerTrucks(cityId);
        //    for(int i = 0;i< playerTrucks.Count; i++)
        //    {
        //        PlayerTruck playerTruck = playerTrucks[i];
        //        IdleTruckType truckType = IdleTruckMgr.Instance.GetIdleTruckTypeById(playerTruck.Id);
        //        IdleTruckLv truckLv = IdleTruckMgr.Instance.GetIdleTruckLv(GetTruckLv(cityId));
        //        IdleTruckRes truckRes = IdleTruckMgr.Instance.GetIdleTruckRes(truckType.Id, GetTruckLv(cityId));
        //        IdleTruck truck = new IdleTruck(truckType, truckLv, truckRes);
        //        idleTrucks.Add(truck);
        //    }
        //    return idleTrucks;
        //}
        public IdleTruck GetIdleTruck()
        {
            return GetIdleTruck(GetTruckLv());
        }
        public IdleTruck GetIdleTruck(int truckLv, int cityId = -1)
        {
            if (cityId == -1)
                cityId = _player.cityId;
            IdleTruckLv idleTruckLv = IdleTruckMgr.Instance.GetIdleTruckLv(truckLv);
            IdleTruckRes truckRes = IdleTruckMgr.Instance.GetIdleTruckRes(truckLv);
            IdleTruck truck = new IdleTruck(idleTruckLv, truckRes);
            return truck;
        }

        /// <summary>
        /// 设置卡车数量
        /// </summary>
        public void SetTruckCount(int count,int cityId)
        {
            PlayerTruck playerTruck = GetPlayerTruck(cityId);
            if(playerTruck != null)
                playerTruck.truckNum = count;
        }

        //卡车升级
        public void TruckLevelUp(int level,int cityId)
        {
            PlayerTruck playerTruck = GetPlayerTruck(cityId);
            if (playerTruck != null)
                playerTruck.truckLv += level;
        }
        public void TruckLevelUp(int level)
        {
            int cityId = _player.cityId;
            TruckLevelUp(level,cityId);
        }

        //获取卡车等级
        public int GetTruckLv(int cityId = -1)
        {
            return GetPlayerTruck(cityId).truckLv;        
        }
        #endregion
        #region 驿站
        public PlayerSite GetPlayerSite(int siteId)
        {
            List<PlayerSite> playerSites = PlayerCity.playerSites;
            for(int i = 0; i < playerSites.Count; i++)
            {
                PlayerSite playerSite = playerSites[i];
                if (playerSite.Id == siteId)
                    return playerSite;
            }
            return null;
        }
        public PlayerSite GetPlayerSite(int siteId,int cityId = -1)
        {
            if (cityId == -1)
                return GetPlayerSite(siteId);
            List<PlayerSite> playerSites = GetPlayerCity(cityId).playerSites;
            for (int i = 0; i < playerSites.Count; i++)
            {
                PlayerSite playerSite = playerSites[i];
                if (playerSite.Id == siteId)
                    return playerSite;
            }
            return null;
        }
        public List<IdleSite> GetUnLockIdleSites()
        {
            List<IdleSite> idleSites = new List<IdleSite>();
            for(int i = 0; i < PlayerCity.playerSites.Count; i++)
            {
                PlayerSite playerSite = PlayerCity.playerSites[i];
                if(!playerSite.isLock)
                {
                    IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id);
                    idleSites.Add(idleSite);
                }
            }
            return idleSites;
        }
        public int GetUnlockSiteCount(int cityId=-1)
        {
            if(cityId<0)
            {
                cityId = CityId;
            }
            int count = 0;
            List<PlayerSite> playerSites = GetPlayerCity(cityId).playerSites;
            for (int i = 0; i < playerSites.Count; i++)
            {
                PlayerSite playerSite = playerSites[i];
                if (!playerSite.isLock)
                    count++;
            }
            return count;
        }
        //获取随机的已解锁的驿站id
        public int GetRandomSiteId()
        {
            List<PlayerSite> sites = PlayerCity.playerSites;
            int endValue = sites.Count;
            for(int i=0;i<sites.Count;i++)
            {
                if(sites[i].isLock)
                {
                    endValue = i;
                    break;
                }
            }
            int index= UnityEngine.Random.Range(0, endValue);
            return sites[index].Id;
        }
        //获取驿站索引
        public int GetSiteIndex(int siteId)
        {
            List<PlayerSite> sites = PlayerCity.playerSites;;
            for (int i = 0; i < sites.Count; i++)
            {
                if(sites[i].Id==siteId)
                {
                    return i;
                }
            }
            return -1;
        }
        public List<IdleItem> GetUnLockIdleItems()
        {
            List<IdleItem> idleItems = new List<IdleItem>();
            List<IdleSite> unlockIdleSites = GetUnLockIdleSites();
            for(int i =0;i< unlockIdleSites.Count; i++)
            {
                IdleSite idleSite = unlockIdleSites[i];
                IdleItem idleItem = IdleItemMgr.Instance.GetIdleItemById(idleSite.itemId);
                idleItems.Add(idleItem);
            }
            return idleItems;
        }

        //判断某个货物是否已解锁
        public bool IsItemUnlock(int itemId)
        {
            for (int i = 0; i < PlayerCity.playerSites.Count; i++)
            {
                PlayerSite playerSite = PlayerCity.playerSites[i];
                if (!playerSite.isLock)
                {
                    IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id);
                    if (idleSite.itemId == itemId)
                        return true;
                }
            }
            return false;
        }

        public int GetSiteMaxVolume(int siteId)
        {
            PlayerSite site = GetPlayerSite(siteId);
            if (site == null) return 0;
            IdleSiteVolume siteVolume = IdleSiteMgr.Instance.GetSiteVolume(siteId, site.siteVolumeLv);
            return siteVolume.volume;
        }

        public int GetNextLockSiteId()
        {
            int siteId = 0;
            List<PlayerSite> playerSites = PlayerCity.playerSites;
            for(int i=0;i<playerSites.Count;i++)
            {
                if(playerSites[i].isLock)
                {
                    siteId = playerSites[i].Id;
                    break;
                }
            }
            return siteId;
        }
       
        //获取上一个已解锁货物id
        public int GetLastUnlockSiteId(int siteId)
        {
            List<PlayerSite> playerSites = PlayerCity.playerSites;
            int lastIndex = -1;
            int firstIndex = -1;
            for (int i = 0; i < playerSites.Count; i++)
            {
                if(!playerSites[i].isLock)
                {
                    if (firstIndex < 0) firstIndex = i;
                    if(playerSites[i].Id == siteId)
                    {
                        lastIndex = i - 1;
                        break;
                    }
                }
              
            }
            if (lastIndex < firstIndex) return -1;
            return playerSites[lastIndex].Id;
        }

        //获取下一个已解锁货物id
        public int GetNextUnlockSiteId(int siteId)
        {
            List<PlayerSite> playerSites = PlayerCity.playerSites;
            int nextIndex = -1;
            int endIndex = -1;
            for(int i=0;i<playerSites.Count;i++)
            {
                if(!playerSites[i].isLock)
                {
                    endIndex = i;
                    if(playerSites[i].Id==siteId)
                    {
                        nextIndex = i + 1;
                    }
                }
            }
            if (nextIndex > endIndex) return -1;
            return playerSites[nextIndex].Id;
        }

        #endregion

        /// <summary>
        /// 获取离开城市的时间
        /// </summary>
        public long GetLeaveCityTime(int cityId = -1)
        {
            if (cityId < 0)
                cityId = CityId;
            DateTime time = DateTime.UtcNow;
            long nowTime = TimeUtils.ConvertLongUtcDateTime(time);
            PlayerCity playerCity = GetPlayerCity(cityId);
            long leaveTime = playerCity.leaveTime;
            long t = nowTime - leaveTime;
            return t;
        }

        #region 任务
        public PlayerTask GetPlayerTask(int id, bool isMainTask = false)
        {
            List<PlayerTask> tasks;
            if (isMainTask)
                tasks = PlayerCity.playerMainTasks;
            else
                tasks = PlayerCity.playerTasks;
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Id == id)
                    return tasks[i];
            }
            return null;
        }

        public PlayerTask GetCurrentMainTask()
        {
            if (PlayerCity.playerMainTasks.Count <= 0) return null;
            return PlayerCity.playerMainTasks[0];
        }

        //领取任务奖励
        public void ReveiveReward(IdleTask idleTask)
        {
            PlayerTask playerTask = GetPlayerTask(idleTask.Id);
            if (playerTask == null) return;
            if(playerTask.IsFinish)
            {
                PlayerCity.playerTasks.Remove(playerTask);
                if(idleTask.RewardType==1)
                {
                    AddMoney(new Long2(idleTask.Reward));
                }
            }
        }
        //领取任务奖励
        public void ReceiveTaskReward(IdleTaskBase task, bool isMainTask = false)
        {
            PlayerTask playerTask = GetPlayerTask(task.Id, isMainTask);
            if (playerTask == null) return;
            if(playerTask.IsFinish)
            {
                if (isMainTask)
                    PlayerCity.playerMainTasks.Remove(playerTask);
                else
                    PlayerCity.playerTasks.Remove(playerTask);
                if(task.RewardType==1)
                {
                    AddMoney(new Long2(task.Reward));
                }
            }
        }

        #endregion

        #region 新手引导
        public PlayerGuide GetPlayerGuide(int guideId)
        {
            if(Player.playerGuide.ContainsKey(guideId))
            {
                return Player.playerGuide[guideId];
            }
            return null;
        }

        public Dictionary<int, PlayerGuide> GetPlayerGuides()
        {
            return Player.playerGuide;
        }

        public bool ContainedPlayerGuide(int guideId)
        {
            return Player.playerGuide.ContainsKey(guideId);
        }

        /// <summary>
        /// 完成引导
        /// </summary>
        public void PlayerCompleteGuide(int guideId)
        {
           if(ContainedPlayerGuide(guideId))
            {
                Player.playerGuide.Remove(guideId);
            }
           
        }
        //重置上一次关闭游戏时的引导
        public void ResetLastGuide()
        {
            var enumerator = Player.playerGuide.GetEnumerator();
            List<int> toRemoveGuideIds = new List<int>();//所有待移除引导id
            while(enumerator.MoveNext())
            {
                PlayerGuide playerGuide = enumerator.Current.Value;
                if(playerGuide.hasStart)
                {
                    IdleGuide idleGuide = IdleGuideMgr.Instance.GetIdleGuide(playerGuide.id);
                    if (idleGuide == null) return;
                    if(!idleGuide.NeedComplete)
                    {
                        toRemoveGuideIds.Add(idleGuide.Id);
                        while(idleGuide.NextGuide>0)
                        {
                            idleGuide = IdleGuideMgr.Instance.GetIdleGuide(idleGuide.NextGuide);
                            toRemoveGuideIds.Add(idleGuide.Id);
                        }
                    }
                    else
                    {
                        playerGuide.hasStart = false;
                    }
                }
            }

        }
        #endregion

        //获取最大容量(单类货物)
        public int GetStoreMaxVolume()
        {
            return IdleCityMgr.Instance.GetStoreVolume(PlayerCity.storeLv);
        }

        //获取当前存储的容量
        public int GetCurrentTotalVolume(out int totalVolume)
        {
            int maxVolume = GetStoreMaxVolume();
            int unlockSiteCount = 0;
            int currentVolume = 0;

            var enumerator = PlayerCity.storeItems.GetEnumerator();
            while(enumerator.MoveNext())
            {
                unlockSiteCount++;
                currentVolume += enumerator.Current.Value;
            }
            totalVolume = maxVolume * unlockSiteCount;
            return currentVolume;
        }

        //获取货物当前的储存数量
        public int GetCurrentStoreVolume(int itemId)
        {
            if (!PlayerCity.storeItems.ContainsKey(itemId))
                PlayerCity.storeItems.Add(itemId, 0);
            return PlayerCity.storeItems[itemId]; 
        }
        //解锁某类货物仓库(将其容量设置为已解锁货物的最大值)
        public void UnlockItemStore(int itemId)
        {
            int maxVolume=0;
            if(!PlayerCity.storeItems.ContainsKey(itemId))
            {
                foreach(var volume in PlayerCity.storeItems.Values)
                {
                    if (volume > maxVolume)
                        maxVolume = volume;
                }
                PlayerCity.storeItems.Add(itemId, maxVolume);
            }
        }

        //上一次开启双倍收益的时间
        public long LastDoubleIncomeTime
        {
            get
            {
                return PlayerCity.lastDoubleIncomeTime;
            }
            set
            {
                PlayerCity.lastDoubleIncomeTime = value;
            }
        }
        //上一次开启双倍收益的总时间
        public int LastDoubleIncomeTotalTime
        {
            get
            {
                return PlayerCity.lastDoubleIncomeTotalTime;
            }
            set
            {
                PlayerCity.lastDoubleIncomeTotalTime = value;
            }
        }
       
        

        public void RecordPlayer()
        {
            if (_player == null || _player.playerCity == null) return;
            PlayerCity.leaveTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);

            if(PlayerCity.lastDoubleIncomeTotalTime>0)
            {
                PlayerCity.lastDoubleIncomeTime = PlayerCity.leaveTime;
            }
            string str = FullSerializerAPI.Serialize(typeof(Player), _player, false, false);
            StreamFile.RecordFile(str, StreamFile.Combine(Files.jsonFolder, Files.hashPlayer));
        }


        

    }
}

