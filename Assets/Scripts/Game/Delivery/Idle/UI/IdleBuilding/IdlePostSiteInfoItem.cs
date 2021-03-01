using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;
using UnityEngine.UI;

namespace Delivery.Idle
{
    public class IdlePostSiteInfoItem : ScrollRectItem
    {
        private IdleBuildingLogic idleBuildingLogic;

        private RectTransform rectTrans;
        private MyText postSiteName;
        private MyText desc;
        private MyText buildCost;
        private MyButton buildBtn;
        private GameObject completeImage;
        private GameObject moneyIcon;
        private Image postSiteIcon;
        private GameObject grayBtnBg;

        private void Awake()
        {
            idleBuildingLogic = IdleBuildingLogic.Instance;

            rectTrans = GetComponent<RectTransform>();

            postSiteName = this.GetComponentByPath<MyText>("postSiteInfoNode/postSiteName");
            desc = this.GetComponentByPath<MyText>("postSiteInfoNode/description");
            buildCost = this.GetComponentByPath<MyText>("postSiteInfoNode/buildCost");
            buildBtn = this.GetComponentByPath<MyButton>("postSiteInfoNode/btn-build");
            grayBtnBg = buildBtn.GetComponentByPath<Transform>("grayBg").gameObject;
            completeImage = this.GetComponentByPath<Transform>("postSiteInfoNode/finish").gameObject;
            moneyIcon = this.GetComponentByPath<Transform>("postSiteInfoNode/moneyIcon").gameObject;
            postSiteIcon = this.GetComponentByPath<Image>("postSiteInfoNode/siteIconBg/postSiteIcon");
          

            AddUIListener();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SiteInfo_UnLock, OnUnLockSite);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
        }

        private void AddUIListener()
        {
            buildBtn.onClick.AddListener(OnBuildBtnClick);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SiteInfo_UnLock, OnUnLockSite);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
        }

        private void UpdateBtnState(BaseEventArgs baseEventArgs)
        {
            int firstLockSiteIndex = idleBuildingLogic.GetEnableBuildSiteIndex();
            if (firstLockSiteIndex == Index)
            {
                if (idleBuildingLogic.PlayerCanPay(Index))
                    //buildBtn.interactable = true;
                    grayBtnBg.SetActive(false);
                else
                    //buildBtn.interactable = false;
                    grayBtnBg.SetActive(true);
                //return;
            }
            //PlayerSite playerSite = idleBuildingLogic.GetPlayerSiteByIndex(Index);
            //if (playerSite.isLock) return;
            //if (idleBuildingLogic.PlayerCanPay(Index))
            //    buildBtn.interactable = true;
            //else
            //    buildBtn.interactable = false;
        }

        private void OnUnLockSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> argsOne = (EventArgsOne<int>)baseEventArgs;
            if(argsOne.param1==Index)
            {
                buildBtn.interactable = true;
            }
        }

        private void InitData()
        {
            PlayerSite playerSite = idleBuildingLogic.GetPlayerSiteByIndex(Index);
            IdleSite site = idleBuildingLogic.GetIdleSiteByIndex(Index);
            postSiteName.text = site.name;
            desc.text = site.desc;
            postSiteIcon.sprite = idleBuildingLogic.GetPostSiteSprite(Index);
            Long2 unlockCost = new Long2(site.unlockPrice);
            if(playerSite.isLock)
            {
                buildCost.text = UnitConvertMgr.Instance.GetFloatValue(unlockCost,2);
            }
            //ShowOrHideCompleteImage(!playerSite.isLock);
            ShowOrHideWidget(playerSite.isLock);
            UpdateBtnState(null);
        }

        private void ShowOrHideCompleteImage(bool isShow)
        {
            int firstLockSiteIndex = idleBuildingLogic.GetEnableBuildSiteIndex();
            if (firstLockSiteIndex == Index)
                buildBtn.interactable = true;
            else
                buildBtn.interactable = false;
            completeImage.SetActive(isShow);
            buildCost.gameObject.SetActive(!isShow);
            buildBtn.gameObject.SetActive(!isShow);
            moneyIcon.SetActive(!isShow);
        }

        private void ShowOrHideWidget(bool isLock)
        {
            int firstLockSiteIndex = idleBuildingLogic.GetEnableBuildSiteIndex();
            if (firstLockSiteIndex == Index)
            {
                completeImage.SetActive(false);
                buildCost.gameObject.SetActive(true);
                buildBtn.gameObject.SetActive(true);
                buildBtn.interactable = true;
                moneyIcon.SetActive(true);
                return;
            }
            if (isLock)
            {
                completeImage.SetActive(false);
            }
            else
            {
                completeImage.SetActive(true);
            }
            buildCost.gameObject.SetActive(false);
            buildBtn.gameObject.SetActive(false);
            moneyIcon.SetActive(false);
           
        }


        private void OnBuildBtnClick()
        {
            bool success = idleBuildingLogic.BuildPostSite(Index);
            if (success)
            {
                ShowOrHideCompleteImage(true);
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SiteInfo_UnLock, Index + 1));
            }
                
        }

        public override int Index
        {
            get
            {
                return base.Index;
            }
            set
            {
                _index = value;
                transform.GetComponent<RectTransform>().anchoredPosition = _scroller.GetPosition(_index);
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
        }
    }
}

