using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;
using libx;

namespace Delivery.Idle
{
    public class IdleBuildingLogic : Singleton<IdleBuildingLogic>
    {
        private PlayerMgr playerMgr;
        private List<PlayerSite> playerSites;
        //private Dictionary<int, Sprite> postSiteIcons;
        private Dictionary<int, AssetRequest> postSiteIcons;
        public IdleBuildingLogic()
        {
            playerMgr = PlayerMgr.Instance;
            playerSites = playerMgr.PlayerCity.playerSites;
            //postSiteIcons = new Dictionary<int, Sprite>();
            postSiteIcons = new Dictionary<int, AssetRequest>();
        }

        /// <summary>
        /// 根据索引获取驿站信息
        /// </summary>
        public PlayerSite GetPlayerSiteByIndex(int index)
        {
            if (index < 0 || index >= playerSites.Count)
                return null;
            return playerSites[index];
        }

        public IdleSite GetIdleSiteByIndex(int index)
        {
            PlayerSite playerSite = GetPlayerSiteByIndex(index);
            if (playerSite == null) return null;
            return IdleSiteMgr.Instance.GetIdleSite(playerSite.Id, playerSite.cityId);
        }

        /// <summary>
        /// 获取驿站数量
        /// </summary>
        public int GetIdleSitesCount()
        {
            return playerSites.Count;
        }

        /// <summary>
        /// 获取当前未解锁的第一个驿站
        /// </summary>
        /// <returns></returns>
        public int GetEnableBuildSiteIndex()
        {
            int index;
            for (index = 0; index < playerSites.Count; index++)
            {
                if (playerSites[index].isLock)
                    return index;
            }
            return index;
        }

        public Sprite GetPostSiteSprite(int index)
        {
            IdleSite idleSite = GetIdleSiteByIndex(index);
            if (idleSite == null) return null;
            if (!postSiteIcons.ContainsKey(idleSite.Id))
            {
                
                string iconName = idleSite.image;
                //string path = "Textures/Idle/SiteIcons/" + iconName;
                //Sprite icon = Resources.Load(path, typeof(Sprite)) as Sprite;
                AssetRequest request = Assets.LoadAsset(iconName + ".png", typeof(Sprite));
                //postSiteIcons.Add(idleSite.Id, icon);
                postSiteIcons.Add(idleSite.Id, request);
            }
            return postSiteIcons[idleSite.Id].asset as Sprite;
        }

        public void Destroy()
        {
            if(postSiteIcons!=null)
            {
                var enumerator = postSiteIcons.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    AssetRequest request = enumerator.Current.Value;
                    if (request != null)
                        request.Release();
                }
                postSiteIcons.Clear();
            }
        }
        

        /// <summary>
        /// 玩家是否可以支付升级花费
        /// </summary>
        public bool PlayerCanPay(int index)
        {
            PlayerSite playerSite = GetPlayerSiteByIndex(index);
            IdleSite site = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id, playerSite.cityId);
            if (playerSite == null || site == null || !playerSite.isLock) return false;
            Long2 cost = new Long2(site.unlockPrice);
            if (playerMgr.PlayerCity.money < cost) return false;
            return true;
        }

        /// <summary>
        /// 建造驿站
        /// </summary>
        public bool BuildPostSite(int index)
        {
            PlayerSite playerSite = GetPlayerSiteByIndex(index);
            IdleSite site = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id,playerSite.cityId);
            if (playerSite == null || site == null || !playerSite.isLock) return false;
            Long2 cost = new Long2(site.unlockPrice);
            if (playerMgr.PlayerCity.money < cost)
            {
                IdleTipCtrl.Instance.ShowTip("金额不足");
                return false;
            }

            //bool isContainedSiteModel = false;
            List<IdleSiteModel> idleSiteModels = IdleSiteCtrl.Instance.GetIdleSiteModels();
            for(int i=0;i<idleSiteModels.Count;i++)
            {
                if(idleSiteModels[i].SiteId== playerSite.Id)
                {
                    //playerMgr.PlayerCity.money -= cost;
                    playerMgr.CutMoney(cost);
                    playerSite.isLock = false;
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Site_UnLock, idleSiteModels[i]));
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Idle_MoneyChange));
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<TaskType>(EnumEventType.Event_Task_ProcessChange, TaskType.Task_UnlockPostSite));
                    Dictionary<string, string> proertie = new Dictionary<string, string>();
                    proertie["post_ID"] = playerSite.Id.ToString();
                    PlatformFactory.Instance.TAEventPropertie("gt_postbuild", proertie);
                    break;
                }
            }
            //if (!isContainedSiteModel) return false;//如果场景不存在当前驿站
            //List<PlayerTruck> playerTrucks = playerMgr.GetPlayerTrucks();
            //for(int i = 0; i < playerTrucks.Count; i++)
            //{
            //    PlayerTruck playerTruck = playerTrucks[i];
            //    IdleTruckType truckType = IdleTruckMgr.Instance.GetIdleTruckTypeById(playerTruck.Id);
            //    string[] items = truckType.itemId.Split(',');
            //    for (int j = 0; j < items.Length; j++)
            //    {
            //        if (items[j] == site.Id.ToString())
            //            return true;
            //    }
            //}
            //List<IdleTruckType> truckTypes = IdleTruckMgr.Instance.GetIdleTruckTypeByItemId(site.itemId.ToString());
            //for(int i = 0; i < truckTypes.Count; i++)
            //{
            //    PlayerTruck pTruck = new PlayerTruck();
            //    pTruck.Id = truckTypes[i].Id;
            //    pTruck.count = 0;
            //    playerTrucks.Add(pTruck);
            //}
            return true;
        }
        
        public void OnCloseBtnClick()
        {
            UIController.Instance.CloseCurrentWindow();
        }
    }
}

