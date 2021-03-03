using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;
using System;

namespace Delivery.Idle
{
    public class IdlePostSiteLogic : Singleton<IdlePostSiteLogic>
    {
        private PlayerMgr playerMgr;
        private IdleSiteMgr idleSiteMgr;

        private IdleSiteModel siteModel;

        private PlayerSite playerSite;
        private IdleSite site;
        private IdleSiteBase siteBase;
        private IdleSiteGrade siteGrade;
        private IdleSiteTime siteTime;
        private IdleSiteVolume siteVolume;
        private int maxSiteTimeLv;

        public bool isAnyLevelUp;

        public IdlePostSiteLogic()
        {
            playerMgr = PlayerMgr.Instance;
            idleSiteMgr = IdleSiteMgr.Instance;
        }

        #region 游戏数据相关
        //初始化
        public void Init(IdleSiteModel _siteModel)
        {
            siteModel = _siteModel;
            int siteId = siteModel.SiteId;
            playerSite = playerMgr.GetPlayerSite(siteId);
            site = idleSiteMgr.GetIdleSite(siteId, playerSite.cityId);
            siteBase = idleSiteMgr.GetSiteBase(siteId, playerSite.siteBaseLv);
            siteGrade = idleSiteMgr.GetSiteGradeBySiteId(siteId, playerSite.siteBaseLv);
            siteTime = idleSiteMgr.GetSiteTime(siteId, playerSite.siteTimeLv);
            siteVolume = idleSiteMgr.GetSiteVolume(siteId, playerSite.siteVolumeLv);
            maxSiteTimeLv = idleSiteMgr.GetSiteTimeMaxLv(siteId);

            isAnyLevelUp = false;
        }

        public Vector3 GetPostSitePos()
        {
            return siteModel.transform.position;
        }

        public string GetPostSiteName()
        {
            return site.name;
        }

        public string GetPostSiteDesc()
        {
            return site.desc;
        }

        public int GetSiteId()
        {
            return siteModel.SiteId;
        }
    
        /// <summary>
        /// 获取配送速度
        /// </summary>
        public Long2 GetDeliverSpeed()
        {
            return playerMgr.GetSiteDeliverSpeed(playerSite);
        }
        /// <summary>
        /// 获取收益
        /// </summary>
        public string GetIncome()
        {
            Long2 income = playerMgr.GetSiteIncome(playerSite);
            //return UnitConvertMgr.Instance.GetValue(income);
            return UnitConvertMgr.Instance.GetFloatValue(income,2);
        }

        /// <summary>
        /// 获取堆积货物量
        /// </summary>
        public string GetRest()
        {
            if (playerSite == null) return "";
            return playerSite.loadItemNum.ToString();
        }
        //获取驿站总容量
        public string GetVolume()
        {
            IdleSiteVolume siteVolume = idleSiteMgr.GetSiteVolume(playerSite.Id, playerSite.siteVolumeLv);
            Long2 volume = new Long2(siteVolume.volume);
            return UnitConvertMgr.Instance.GetFloatValue(volume, 2);
        }
        
        /// <summary>
        /// 获取下一阶段收益倍数
        /// </summary>
        public int GetMultiple()
        {
            IdleSiteGrade nextSiteModel = idleSiteMgr.GetNextSiteGrade(playerSite.Id, playerSite.siteBaseLv, out bool max);
            return nextSiteModel.multiple;
        }
        
        public IdleSiteGrade GetIdleSiteGrade()
        {
            return siteGrade;
        }

        public int GetSiteLv()
        {
            return siteBase.Lv;
        }

        /// <summary>
        /// 获取配送时间
        /// </summary>
        public float GetDeliverTime()
        {
            float time= (8 / siteTime.speed) * 2 + siteTime.qtime;
            return time;
        }
        //获取当前配送时间等级进度
        public float GetDeliverTimeLvProcess()
        {
            int currentLv = siteTime.Lv;
            if (maxSiteTimeLv <= 0) return 1;
            return (float)currentLv / maxSiteTimeLv;
        }
        /// <summary>
        /// 升级站点花费
        /// </summary>
        public Long2 GetUpgradeSiteCost()
        {
            return new Long2(siteBase.price);
        }
        /// <summary>
        /// 升级配送时间花费
        /// </summary>
        public Long2 GetUpgradeSiteTimeCost()
        {
            return new Long2(siteTime.price);
        }

        public Long2 GetUpgradeSiteVolumeCost()
        {
            return new Long2(siteVolume.price);
        }

        public Vector2 GetCameraPos()
        {
            Vector2 pos = new Vector2(site.posX, site.posY);
            return pos;
        }
        public float GetCameraSize()
        {
            return site.size;
        }

        /// <summary>
        /// 升级站点
        /// </summary>
        public bool UpgradeSite()
        {
            Dictionary<int, IdleSiteBase> siteBases = idleSiteMgr.GetSiteBaseBySiteId(playerSite.Id);
            if (!siteBases.ContainsKey(playerSite.siteBaseLv+1)) return false;
            Long2 cost = new Long2(siteBase.price);
            if (!playerMgr.IsEnoughMoney(cost))
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return false;
            }
            playerSite.siteBaseLv += 1;
            siteBase = idleSiteMgr.GetSiteBase(playerSite.Id, playerSite.siteBaseLv);

            int lastSiteGradeId = siteGrade.Id;
            siteGrade = idleSiteMgr.GetSiteGradeBySiteId(playerSite.Id, playerSite.siteBaseLv);

            if(siteGrade!=null && lastSiteGradeId !=siteGrade.Id)
            {
                Dictionary<string, string> siteProertie = new Dictionary<string, string>();
                siteProertie["post_ID"] = playerSite.Id.ToString();
                siteProertie["post_lev"] = playerSite.siteBaseLv.ToString();
                siteProertie["siteVolumeLv"] = playerSite.siteVolumeLv.ToString();
                siteProertie["siteTimeLv"] = playerSite.siteTimeLv.ToString();
                PlatformFactory.Instance.TAEventPropertie("gt_postupgrade", siteProertie);
            }



            playerMgr.CutMoney(cost);
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_TotalIncomeChange));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Site_BaseGrade, siteModel));
            //EventDispatcher.Instance.TriggerEvent(new EventArgsOne<TaskType>(EnumEventType.Event_Task_ProcessChange, TaskType.Task_LevelUpPostSite));
            isAnyLevelUp = true;
            return true;
        }

      

        /// <summary>
        /// 员工升级
        /// </summary>
        public bool UpgradeStaff()
        {
            Dictionary<int, IdleSiteTime> sitetimes = idleSiteMgr.GetSiteTimeBySiteId(playerSite.Id);
            if (!sitetimes.ContainsKey(playerSite.siteTimeLv + 1)) return false;
            Long2 cost = new Long2(siteTime.price);
            if (playerMgr.PlayerCity.money < cost)
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return false;
            }
            playerSite.siteTimeLv += 1;
            siteTime = idleSiteMgr.GetSiteTime(playerSite.Id, playerSite.siteTimeLv);
            playerMgr.CutMoney(cost);
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_TotalIncomeChange));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Site_TimeGrade, siteModel));
            isAnyLevelUp = true;
            return true;
        }
        //升级驿站容量
        public bool UpgradeSiteVolume()
        {
            if (IsMaxSiteVolumeLv()) return false;
            Long2 cost = new Long2(siteVolume.price);
            if (playerMgr.PlayerCity.money < cost)
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return false;
            }
            playerSite.siteVolumeLv += 1;
            siteVolume = idleSiteMgr.GetSiteVolume(playerSite.Id, playerSite.siteVolumeLv);
            playerMgr.CutMoney(cost);
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Site_VolumeGrade, siteModel));
            isAnyLevelUp = true;
            return true;
        }
        public int GetSiteVolumeLv()
        {
            return playerSite.siteVolumeLv;
        }

        public bool PlayerCanPay(Long2 cost)
        {
            return cost <= playerMgr.PlayerCity.money;
        }

        public bool IsMaxPostSiteLv()
        {
            Dictionary<int, IdleSiteBase> siteBases = idleSiteMgr.GetSiteBaseBySiteId(playerSite.Id);
            return !siteBases.ContainsKey(playerSite.siteBaseLv + 1);
        }


        public bool IsMaxStaffLv()
        {
            Dictionary<int, IdleSiteTime> sitetimes = idleSiteMgr.GetSiteTimeBySiteId(playerSite.Id);
            return !sitetimes.ContainsKey(playerSite.siteTimeLv + 1);
        }

        public bool IsMaxSiteVolumeLv()
        {
            Dictionary<int, IdleSiteVolume> siteVolumes = idleSiteMgr.GetSiteVolumeBySiteId(playerSite.Id);
            return !siteVolumes.ContainsKey(playerSite.siteVolumeLv + 1);
        }
        

        public int GetLastPostSiteId()
        {
            return playerMgr.GetLastUnlockSiteId(site.Id);
        }
        public int GetNextPostSiteId()
        {
            return playerMgr.GetNextUnlockSiteId(site.Id);
        }

        #endregion

        public void OnCloseBtnClick()
        {
            //if (isAnyLevelUp)
            //{
            //    Dictionary<string, string> siteProertie = new Dictionary<string, string>();
            //    siteProertie["post_ID"] = playerSite.Id.ToString();
            //    siteProertie["post_lev"] = playerSite.siteBaseLv.ToString();
            //    siteProertie["siteVolumeLv"] = playerSite.siteVolumeLv.ToString();
            //    siteProertie["siteTimeLv"] = playerSite.siteTimeLv.ToString();
            //    PlatformFactory.Instance.TAEventPropertie("gt_postupgrade", siteProertie);
            //}
            UIController.Instance.CloseCurrentWindow();
        }

        public bool OnUpgradeSiteBtnClick()
        {
            return UpgradeSite();
        }
        

        public bool OnUpgradeStaffBtnClick()
        {
            return UpgradeStaff();
        }
        public bool OnUpgradeSiteVolumeBtnClick()
        {
            return UpgradeSiteVolume();
        }
    }
}


