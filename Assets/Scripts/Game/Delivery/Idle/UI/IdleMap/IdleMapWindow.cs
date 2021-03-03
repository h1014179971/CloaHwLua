using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using Foundation;
using System;

namespace Delivery.Idle
{
    public class IdleMapWindow : UIWindow
    {

        private IdleMapLogic idleMapLogic;

        private MyButton closeBtn;
        private ScrollRectEx cityBtnScroll;
        private int viewCount;
        private MyText nextCityName;
        private MyText unlockCost;
        private MyButton unlockBtn;

        private MyText unlockTimeText;
        private int restTime;
        private TimerEvent unlockTimer;//解锁倒计时

        private IdleUIAnimation anim;
        private void Awake()
        {
            idleMapLogic = IdleMapLogic.Instance;
            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
            cityBtnScroll = this.GetComponentByPath<ScrollRectEx>("bg/openedCityView");
            viewCount = cityBtnScroll.viewCount;
            nextCityName = this.GetComponentByPath<MyText>("bg/nextAreaBg/nextCityName");
            unlockCost = this.GetComponentByPath<MyText>("bg/nextAreaBg/costNode/cost");
            unlockBtn = this.GetComponentByPath<MyButton>("bg/nextAreaBg/btn-openCity");

            unlockTimeText = this.GetComponentByPath<MyText>("bg/nextAreaBg/unlockTime");

            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);


            AddAllListener();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, InitBtnState);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
        }

        private void AddAllListener()
        {
            closeBtn.onClick.AddListener(CloseWindow);
            unlockBtn.onClick.AddListener(idleMapLogic.OnUnlockBtnClick);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_MoneyChange, InitBtnState);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
        }
        private void OnExitFinish()
        {
            if(unlockTimer!=null)
            {
                Timer.Instance.DestroyTimer(unlockTimer);
                unlockTimer = null;
            }

            idleMapLogic.OnCloseBtnClick();
        }

        private void CloseWindow()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }
        private void CloseWindow(BaseEventArgs baseEventArgs)
        {
            if (!IsActive) return;
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }

       

        private void InitBtnState(BaseEventArgs baseEventArgs)
        {
            if(idleMapLogic.PlayerCanPay())
            {
                unlockBtn.interactable = true;
            }
            else
                unlockBtn.interactable = false;
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

        private void UpdateUnlockTime(params object[]objs)
        {
            restTime--;
            if(restTime<0)
            {
                unlockTimeText.gameObject.SetActive(false);
            }
            unlockTimeText.text = TimeUtils.secondsToString2(restTime) + "后解锁";
        }

        protected override void InitWindow(object arg = null)
        {
            idleMapLogic.InitData();
            int allCityCount = idleMapLogic.GetAllCityCount();
            int elementCount = allCityCount / 3;
            if (allCityCount % 3 != 0)
                elementCount++;
            if(elementCount < viewCount)
            {
                cityBtnScroll.viewCount = elementCount;
            }
            cityBtnScroll.UpdateContne(elementCount);

            IdleCity nextCity = idleMapLogic.GetNextCity();
            nextCityName.text = nextCity.name;
            Long2 cost = new Long2(nextCity.unlockPrice);
            unlockCost.text = UnitConvertMgr.Instance.GetFloatValue(cost,2);

            InitBtnState(null);



            #region 计时器相关
            DateTime time = DateTime.UtcNow;
            long nowTime = TimeUtils.ConvertLongUtcDateTime(time);
            long dTime = nowTime - PlayerMgr.Instance.Player.firstLoginTime;

            int totalTime = 24 * 3600;
            if(dTime-totalTime>=0)
            {
                unlockTimeText.gameObject.SetActive(false);
            }
            else
            {
                restTime = totalTime - (int)dTime;
                unlockTimeText.text = TimeUtils.secondsToString2(restTime) + "后解锁";
                unlockTimer = Timer.Instance.Register(1,restTime, UpdateUnlockTime);
            }
            #endregion


        }
    }
}

