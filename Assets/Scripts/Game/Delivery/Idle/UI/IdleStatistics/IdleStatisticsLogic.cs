using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;
using libx;

namespace Delivery.Idle
{
    //驿站的统计数据
    public struct PostSiteStatistics
    {
        public int siteId;//驿站id;
        public string name;//驿站名
        public int level;//等级
        public Long2 deliverSpeed;//配送速度
        public Long2 income;//收益
        public int rest;//货物堆积
    }
    public class IdleStatisticsLogic : Singleton<IdleStatisticsLogic>
    {
        private PlayerMgr playerMgr;
        private List<PlayerSite> playerSites;
        private List<PlayerSite> unlockSites;
        //private Dictionary<int, Sprite> postSiteIcons;
        private Dictionary<int, AssetRequest> postSiteIcons;
        private int bestIndex;
        private int badIndex;
        public IdleStatisticsLogic()
        {
            playerMgr = PlayerMgr.Instance;
            unlockSites = new List<PlayerSite>();
            //postSiteIcons = new Dictionary<int, Sprite>();
            postSiteIcons = new Dictionary<int, AssetRequest>();
        }

        public void InitData()
        {
            bestIndex = badIndex = 0;
            playerSites = PlayerMgr.Instance.PlayerCity.playerSites;
            unlockSites.Clear();
            Long2 bestIncome = new Long2();
            Long2 badIncome = new Long2();
            for (int i=0;i< playerSites.Count;i++)
            {
                PlayerSite site = playerSites[i];
                if (site.isLock) continue;
                unlockSites.Add(site);
                PostSiteStatistics statistics = InitStatistics(site);
                if(i==0)
                {
                    bestIncome = statistics.income;
                    badIncome = statistics.income;
                }
                else
                {
                    if(statistics.income>bestIncome)
                    {
                        bestIncome = statistics.income;
                        bestIndex = i;
                    }
                    else if(statistics.income<badIncome)
                    {
                        badIncome = statistics.income;
                        badIndex = i;
                    }
                }
            }
        }

        private PostSiteStatistics InitStatistics(PlayerSite playerSite)
        {
            IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(playerSite.Id,playerSite.cityId);
            PostSiteStatistics statistics = new PostSiteStatistics();
            statistics.siteId = playerSite.Id;
            statistics.name = idleSite.name;
            statistics.level = playerSite.siteBaseLv;
            statistics.deliverSpeed = playerMgr.GetSiteDeliverSpeed(playerSite);
            statistics.income = playerMgr.GetSiteIncome(playerSite);
            statistics.rest = playerSite.loadItemNum;
            return statistics;
        }

        public int GetBadIndex()
        {
            return badIndex;
        }
        public int GetBestIndex()
        {
            return bestIndex;
        }

        public int GetPostSiteCount()
        {
            return unlockSites.Count;
        }

        public PostSiteStatistics GetTotalStatistics()
        {
            PostSiteStatistics totalStatistics = new PostSiteStatistics();
            totalStatistics.deliverSpeed = new Long2();
            totalStatistics.income = new Long2();
            totalStatistics.rest = 0;
            for (int i = 0; i < unlockSites.Count; i++)
            {
                PlayerSite site = unlockSites[i];
                if (site.isLock) continue;
                PostSiteStatistics statistics = InitStatistics(site);
               
                totalStatistics.deliverSpeed += statistics.deliverSpeed;
                totalStatistics.income += statistics.income;
                totalStatistics.rest += statistics.rest;
            }

            return totalStatistics;
        }
        

        public PostSiteStatistics GetPostSiteStatisticsByIndex(int index)
        {
          
            if (index < 0 || index >= unlockSites.Count) return new PostSiteStatistics();
            return InitStatistics(unlockSites[index]);
        }

        public Sprite GetPostSiteSprite(int index)
        {
            if (index < 0 || index >= unlockSites.Count) return null;
            if(!postSiteIcons.ContainsKey(index))
            {
                PlayerSite playerSite = unlockSites[index];
                IdleSite idleSite= IdleSiteMgr.Instance.GetIdleSite(playerSite.Id, playerSite.cityId);
                
                string iconName = idleSite.image;
                AssetRequest request = Assets.LoadAsset(iconName + ".png", typeof(Sprite));
                postSiteIcons.Add(index, request);
                //string path = "Textures/Idle/SiteIcons/" + iconName;
                //Sprite siteSprite = Resources.Load(path, typeof(Sprite)) as Sprite;
                //postSiteIcons.Add(index, siteSprite);

            }
            return postSiteIcons[index].asset as Sprite;
        }
        
        public void Destroy()
        {
            if(postSiteIcons!=null)
            {
                var enumerator = postSiteIcons.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    AssetRequest request = enumerator.Current.Value;
                    if(request!=null)
                    {
                        request.Release();
                    }
                }
                postSiteIcons.Clear();
            }
        }

        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        public void OnCloseBtnClick()
        {
            UIController.Instance.CloseCurrentWindow();
        }
    }
}


