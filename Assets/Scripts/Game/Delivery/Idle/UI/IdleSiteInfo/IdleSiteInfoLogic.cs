using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;
namespace Delivery.Idle
{
    public class IdleSiteInfoLogic : Singleton<IdleSiteInfoLogic>
    {
        private PlayerMgr playerMgr;
        private IdleCityMgr idleCityMgr;
        //private List<int> itemIdList;
        private int storeCount;
        private Dictionary<int, int> storeItems;

        private bool isAnyLevelUp;
       
        public void InitData()
        {
            playerMgr = PlayerMgr.Instance;
            idleCityMgr = IdleCityMgr.Instance;
            storeItems = playerMgr.PlayerCity.storeItems;
            storeCount = 0;
            List<PlayerSite> sites = playerMgr.PlayerCity.playerSites;
            for(int i=0;i<sites.Count;i++)
            {
                if (!sites[i].isLock)
                    storeCount++;
            }
            isAnyLevelUp = false;
        }

        #region 游戏数据相关

        //获取运输速度
        public int GetTranportSpeed()
        {
            int volume = GetVolume();
            IdleCity idleCity = idleCityMgr.GetIdleCityById();
            float dTime = idleCity.sameTruckTime;
            return Mathf.RoundToInt(volume / dTime * 60);
        }
        //获取仓库剩余货物
        public int GetStoreRest()
        {
            int storeRest = 0;
            var enumerator = storeItems.GetEnumerator();
            while(enumerator.MoveNext())
            {
                storeRest += enumerator.Current.Value;
            }
            return storeRest;
        }

        /// <summary>
        /// 获取装货量等级
        /// </summary>
        public int GetVolumeLv()
        {
            return playerMgr.PlayerCity.cityTruckVolumeLv;
        }

        /// <summary>
        /// 获取装货量
        /// </summary>
        public int GetVolume()
        {
            return idleCityMgr.GetVolume(GetVolumeLv());
        }

        /// <summary>
        /// 获取装货量升级花费
        /// </summary>
        public Long2 GetUpgradeVolumeCost()
        {
            IdleCityVolume cityVolume = idleCityMgr.GetIdleCityVolume(GetVolumeLv());
            if (cityVolume == null) return new Long2();
            Long2 cost = new Long2(cityVolume.price);
            return cost;
        }

        ///// <summary>
        ///// 获取出车速度等级
        ///// </summary>
        //public int GetSpeedLv()
        //{
        //    return playerMgr.PlayerCity.cycleLv;
        //}
        ///// <summary>
        ///// 获取出车速度
        ///// </summary>
        //public float GetSpeed()
        //{
        //    return idleCityMgr.GetCycle(GetSpeedLv());
        //}
        ///// <summary>
        ///// 获取升级出车速度花费
        ///// </summary>
        //public Long2 GetUpgradeSpeedCost()
        //{
        //    IdleCityCycle cityCycle = idleCityMgr.GetIdleCityCycle(GetSpeedLv());
        //    if (cityCycle == null) return new Long2();
        //    Long2 cost = new Long2(cityCycle.price);
        //    return cost;
        //}

        /// <summary>
        /// 获取仓库等级
        /// </summary>
        public int GetStoreLv()
        {
            return playerMgr.PlayerCity.storeLv;
        }

        /// <summary>
        /// 获取仓库最大容量
        /// </summary>
        public int GetStoreVolume()
        {
            //return playerMgr.GetStoreMaxVolume() * storeCount;
            return playerMgr.GetStoreMaxVolume();
        }

        public Long2 GetUpdateStoreCost()
        {
            IdleCityStoreVolume storeVolume = idleCityMgr.GetIdleStoreVolume(GetStoreLv());
            return new Long2(storeVolume.price);
        }

        /// <summary>
        /// 升级站点容量
        /// </summary>
        public bool UpgradeSiteVolume()
        {
            Long2 cost= GetUpgradeVolumeCost();
            IdleCity idleCity = IdleCityMgr.Instance.GetIdleCityById();
            int lv = playerMgr.PlayerCity.cityTruckVolumeLv;
            if (idleCityMgr.IsMaxVolumeLv(lv + 1)) return false;
            if (playerMgr.PlayerCity.money < cost)
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return false;
            }
            playerMgr.CutMoney(cost);
            playerMgr.PlayerCity.cityTruckVolumeLv += 1;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_City_ChangeVolume));
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            //EventDispatcher.Instance.TriggerEvent(new EventArgsOne<TaskType>(EnumEventType.Event_Task_ProcessChange, TaskType.Task_LevelUpSite));
            isAnyLevelUp = true;
            return true;
        }
        
        public bool UpgradeStore()
        {
            Long2 cost = GetUpdateStoreCost();
            int lv = playerMgr.PlayerCity.storeLv;
            if (idleCityMgr.IsMaxStoreLv(lv + 1)) return false;
            if (playerMgr.PlayerCity.money < cost)
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return false;
            }
            playerMgr.CutMoney(cost);
            playerMgr.PlayerCity.storeLv += 1;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_City_ChangeStoreVolume));
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            isAnyLevelUp = true;
            return false;
        }


        public bool PlayerCanPay(Long2 cost)
        {
            return cost <= playerMgr.PlayerCity.money;
        }

        public bool IsMaxVolume()
        {
            int lv = playerMgr.PlayerCity.cityTruckVolumeLv;
            return idleCityMgr.IsMaxVolumeLv(lv + 1);
        }


        public bool IsMaxSpeed()
        {
            int lv = playerMgr.PlayerCity.cycleLv;
            return idleCityMgr.IsMaxCycleLv(lv + 1);
        }

        public bool IsMaxStoreLv()
        {
            int lv = GetStoreLv();
            return idleCityMgr.IsMaxStoreLv(lv + 1);
        }

        public Vector2 GetCameraPos()
        {
            IdleCity idleCity = idleCityMgr.GetIdleCityById();
            Vector2 pos = new Vector2(idleCity.basecameraX, idleCity.basecameraY);
            return pos;
        }

        public float GetCameraSize()
        {
            IdleCity idleCity = idleCityMgr.GetIdleCityById();
            return idleCity.basecameraSize;
        }

        #endregion


        #region UI响应事件
        public void OnCloseBtnClick()
        {
            if(isAnyLevelUp)
            {
                Dictionary<string, string> siteProertie = new Dictionary<string, string>();
                siteProertie["centervolume"] = GetStoreLv().ToString();
                siteProertie["bigtruckvolume"] = GetVolumeLv().ToString();
                PlatformFactory.Instance.TAEventPropertie("gt_deliverycenter", siteProertie);
            }
            UIController.Instance.CloseCurrentWindow();
        }

        /// <summary>
        /// 装货量升级按钮点击事件
        /// </summary>
        public void OnUpgradeVolumeBtnClick()
        {
            UpgradeSiteVolume();
        }

        ///// <summary>
        ///// 站点运货速度升级按钮点击事件
        ///// </summary>
        //public void OnUpgradeSpeedBtnClick()
        //{
        //    UpgradeSpeed();
        //}

        public void OnUpgradeStoreBtnClick()
        {
            UpgradeStore();
        }

        #endregion
    }
}

