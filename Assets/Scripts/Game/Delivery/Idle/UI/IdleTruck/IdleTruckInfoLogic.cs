using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;

namespace Delivery.Idle
{
    public class IdleTruckInfoLogic : Singleton<IdleTruckInfoLogic>
    {
        private PlayerMgr playerMgr;
        private IdleTruckMgr idleTruckMgr;
        //private Dictionary<int, int> tempTruckCount;
        private Dictionary<int, Sprite> itemIcons;
        //private Dictionary<int, Sprite> truckIcons;
        public bool isAnyLevelUp;

        public IdleTruckInfoLogic()
        {
            playerMgr = PlayerMgr.Instance;
            idleTruckMgr = IdleTruckMgr.Instance;
            //tempTruckCount = new Dictionary<int, int>();
            itemIcons = new Dictionary<int, Sprite>();
            //truckIcons = new Dictionary<int, Sprite>();
        }

        public void InitData()
        {
            isAnyLevelUp = false;
            //tempTruckCount.Clear();
            //List<PlayerTruck> trucks = playerMgr.GetPlayerTrucks(playerMgr.CityId);
            //for(int i=0;i<trucks.Count;i++)
            //{
            //    tempTruckCount.Add(i, trucks[i].count);
            //}
        }

        #region 游戏数据相关
        /// <summary>
        /// 获取可用货车数量(车辆升级界面调用)
        /// </summary>
        public int GetTruckCount()
        {
            return playerMgr.GetTruckNum(playerMgr.PlayerCity.cityId);
        }
        
        /// <summary>
        /// 获取货车数量升级消耗
        /// </summary>
        public Long2 GetTruckCountLevelupCost()
        {
            string costStr= idleTruckMgr.GetTruckLevelUpCost(GetTruckCount());
            Long2 cost = new Long2(costStr);
            return cost;
        }

        /// <summary>
        /// 获取货车等级
        /// </summary>
        public int GetTruckLv()
        {
            return playerMgr.GetTruckLv(playerMgr.CityId); ;
        }

        /// <summary>
        /// 获取货车装货量
        /// </summary>
        public string GetTruckVolume()
        {
            int volumeStr= idleTruckMgr.GetIdleTruckLv(GetTruckLv()).volume;
            int volume = volumeStr;
            return volume.ToString();
        }
      
        /// <summary>
        /// 获取升级容量花费
        /// </summary>
        public Long2 GetTruckVolumeLevelupCost()
        {
            int level = GetTruckLv();
            Long2 cost = new Long2(idleTruckMgr.GetIdleTruckLv(level).price);
            return cost;
        }

        ///// <summary>
        ///// 通过索引获取货车对象
        ///// </summary>
        //public IdleTruck GetIdleTruckByIndex(int index)
        //{
        //    List<IdleTruck> trucks = playerMgr.GetIdleTrucks(playerMgr.CityId);
        //    if (index < 0 || index >= trucks.Count) return null;
        //    return trucks[index];
        //}
        ///// <summary>
        ///// 获取玩家已解锁的卡车种类
        ///// </summary>
        ///// <returns></returns>
        //public int GetTruckTypes()
        //{
        //    return playerMgr.GetPlayerTrucks().Count;
        //}
      
        public bool IsMaxTruckCount()
        {
            int truckCount = GetTruckCount();
            return idleTruckMgr.IsMaxTruckCount(truckCount+1);
        }

        /// <summary>
        /// 货车数量升级
        /// </summary>
        public void UpgradeTruckCountLv()
        {
            int truckCount = GetTruckCount();
            if (idleTruckMgr.IsMaxTruckCount(truckCount + 1))
                return;
            Long2 cost = GetTruckCountLevelupCost();
            if (cost > playerMgr.PlayerCity.money)
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return;
            }
            playerMgr.SetTruckCount(truckCount + 1, playerMgr.CityId);
            playerMgr.CutMoney(cost);
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Truck_ChangeMaxCount));
            //EventDispatcher.Instance.TriggerEvent(new EventArgsOne<TaskType>(EnumEventType.Event_Task_ProcessChange, TaskType.Task_AllocateTruck));
            isAnyLevelUp = true;
        }
        
        public bool PlayerCanPay(Long2 cost)
        {
            return cost <= playerMgr.PlayerCity.money;
        }

        public bool IsMaxTruckLv()
        {
            int truckLv = GetTruckLv();
            return idleTruckMgr.IsMaxTruckLv(truckLv + 1);
        }
        /// <summary>
        /// 货车容量升级
        /// </summary>
        public void UpgradeTruckVolumeLv()
        {
            int truckLv = GetTruckLv();
            if (idleTruckMgr.IsMaxTruckLv(truckLv + 1)) return;
            Long2 cost = GetTruckVolumeLevelupCost();
            if (cost > playerMgr.PlayerCity.money)
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return;
            }
            playerMgr.TruckLevelUp(1);
            playerMgr.CutMoney(cost);
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Truck_ChangeVolumeLv));
            isAnyLevelUp = true;
        }

        
        public Vector2 GetCameraPos()
        {
            IdleCity idleCity = IdleCityMgr.Instance.GetIdleCityById();
            Vector2 pos = new Vector2(idleCity.stationcameraX, idleCity.stationcameraY);
            return pos;
        }

        public float GetCameraSize()
        {
            IdleCity idleCity = IdleCityMgr.Instance.GetIdleCityById();
            return idleCity.stationcameraSize;
        }

        public void Destroy()
        {
            //if (tempTruckCount != null)
            //    tempTruckCount.Clear();
            //if (itemIcons != null)
            //    itemIcons.Clear();
            //if (truckIcons != null)
            //    truckIcons.Clear();
        }

        #endregion



        #region UI响应事件
        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        public void OnCloseBtnClick()
        {
            if(isAnyLevelUp)
            {
                Dictionary<string, string> siteProertie = new Dictionary<string, string>();
                siteProertie["trucknum"] = GetTruckCount().ToString();
                siteProertie["truckvolume"] = GetTruckLv().ToString();
                PlatformFactory.Instance.TAEventPropertie("gt_truckposition", siteProertie);
            }
            UIController.Instance.CloseCurrentWindow();
        }

        public void OnUpgradeTruckCountBtnClick()
        {
            UpgradeTruckCountLv();
        }

      
        public void OnUpgradeTruckLvBtnClick()
        {
            UpgradeTruckVolumeLv();
        }
        
        #endregion

       

    }
}




