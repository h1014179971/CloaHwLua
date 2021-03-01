using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;

namespace Delivery.Idle
{
    public class IdleStatisticsWindow : UIWindow
    {
        private IdleStatisticsLogic idleStatisticsLogic;
        
        #region UI控件
        private MyButton closeBtn;//关闭按钮
        private ScrollRectEx postSiteScroller;
        private MyText totalDeliverSpeed;
        private MyText totalIncome;
        private MyText totalRest;

        private int viewCount;
        #endregion

        private IdleUIAnimation anim;
        private void Awake()
        {
            idleStatisticsLogic = IdleStatisticsLogic.Instance;
            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
            postSiteScroller = this.GetComponentByPath<ScrollRectEx>("bg/postSiteStatisticsBg/postSites");
            viewCount = postSiteScroller.viewCount;
            totalDeliverSpeed = this.GetComponentByPath<MyText>("bg/speedBg/deliverSpeed");
            totalIncome = this.GetComponentByPath<MyText>("bg/incomeBg/income");
            totalRest = this.GetComponentByPath<MyText>("bg/restBg/rest");


            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);
            


            AddAllUIListener();

        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_ChangeRest, UpdateTotalRest);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
            idleStatisticsLogic.Destroy();
        }

        /// <summary>
        /// 添加所有UI时间监听
        /// </summary>
        private void AddAllUIListener()
        {
            closeBtn.onClick.AddListener(CloseWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_ChangeRest, UpdateTotalRest);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
        }

        private void OnExitFinish()
        {
            idleStatisticsLogic.OnCloseBtnClick();
        }

        private void CloseWindow()
        {
            anim.Exit();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }
        private void CloseWindow(BaseEventArgs baseEventArgs)
        {
            if (!IsActive) return;
            anim.Exit();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }


        private void InitData()
        {
            idleStatisticsLogic.InitData();
            int postSiteCount = idleStatisticsLogic.GetPostSiteCount();
            if (postSiteCount <= viewCount)
                postSiteScroller.viewCount = postSiteCount;
            postSiteScroller.UpdateContne(postSiteCount);

            PostSiteStatistics totalStatistics = idleStatisticsLogic.GetTotalStatistics();
            totalDeliverSpeed.text = UnitConvertMgr.Instance.GetFloatValue(totalStatistics.deliverSpeed,2) + "/分钟";
            totalIncome.text = UnitConvertMgr.Instance.GetFloatValue(totalStatistics.income,2) + "/分钟";
            totalRest.text = totalStatistics.rest.ToString();
        }

        private void UpdateTotalRest(BaseEventArgs baseEventArgs)
        {
            PostSiteStatistics totalStatistics = idleStatisticsLogic.GetTotalStatistics();
            totalRest.text = totalStatistics.rest.ToString();
        }
        
        public override void Hide()
        {
            base.Hide();
        }

        public override void Open()
        {
            base.Open();
            anim.Enter();
        }

        public override void Show()
        {
            base.Show();
        }

        protected override void InitWindow(object arg = null)
        {
            InitData();
        }


    }
}


