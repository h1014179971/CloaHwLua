using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;

namespace Delivery.Idle
{
    public class IdleStatisticsPostSiteItem : ScrollRectItem
    {
        private IdleStatisticsLogic idleStatisticsLogic;

        private RectTransform rectTrans;
        private MyText postSiteName;
        private MyText postSiteLv;
        private MyText deliverSpeed;
        private MyText income;
        private MyText rest;
        private Image postSiteIcon;

        private Transform situationNode;

        private void Awake()
        {
            idleStatisticsLogic = IdleStatisticsLogic.Instance;

            rectTrans = GetComponent<RectTransform>();
            postSiteName = this.GetComponentByPath<MyText>("postSiteNode/postSiteName");
            postSiteLv = this.GetComponentByPath<MyText>("postSiteNode/icon/postSiteLv");
            deliverSpeed = this.GetComponentByPath<MyText>("postSiteNode/deliverSpeed");
            income = this.GetComponentByPath<MyText>("postSiteNode/income");
            rest = this.GetComponentByPath<MyText>("postSiteNode/rest");
            situationNode = this.GetComponentByPath<Transform>("postSiteNode/incomeSituation");
            postSiteIcon = this.GetComponentByPath<Image>("postSiteNode/icon/siteIcon");

            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_ChangeRest, UpdateRest);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_ChangeRest, UpdateRest);
        }


        private void InitData()
        {
            PostSiteStatistics statistics = idleStatisticsLogic.GetPostSiteStatisticsByIndex(Index);
            postSiteName.text = statistics.name;
            postSiteLv.text = "Lv." + statistics.level.ToString();
            deliverSpeed.text = UnitConvertMgr.Instance.GetFloatValue(statistics.deliverSpeed,2) + "/分钟";
            income.text = UnitConvertMgr.Instance.GetFloatValue(statistics.income,2) + "/分钟";
            rest.text = statistics.rest.ToString();
            postSiteIcon.sprite = idleStatisticsLogic.GetPostSiteSprite(Index);

            int badIndex = idleStatisticsLogic.GetBadIndex();
            int bestIndex = idleStatisticsLogic.GetBestIndex();
            if(badIndex==bestIndex)
            {
                situationNode.gameObject.SetActive(false);
            }
            else
            {
                situationNode.gameObject.SetActive(true);
                if(Index==badIndex)
                {
                    situationNode.Find("bad").gameObject.SetActive(true);
                    situationNode.Find("good").gameObject.SetActive(false);
                }
                else if(Index==bestIndex)
                {
                    situationNode.Find("bad").gameObject.SetActive(false);
                    situationNode.Find("good").gameObject.SetActive(true);
                }
                else
                {
                    situationNode.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateRest(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> argsOne = (EventArgsOne<int>)baseEventArgs;
            PostSiteStatistics statistics = idleStatisticsLogic.GetPostSiteStatisticsByIndex(Index);
            if (statistics.siteId != argsOne.param1) return;
            rest.text = statistics.rest.ToString();
        }

        public override int Index
        {
            get
            {
                return base.Index; ;
            }
            set
            {
                _index = value;
                rectTrans.anchoredPosition = _scroller.GetPosition(_index);
                InitData();
            }
        }

        public override void CreateItem()
        {
            base.CreateItem();
        }

        public override void Init()
        {
            base.Init();
            InitData();
        }
    }
}


