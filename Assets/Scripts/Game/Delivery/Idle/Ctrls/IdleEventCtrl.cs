using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdleEventCtrl : MonoSingleton<IdleEventCtrl>
    {
        private PlayerMgr playerMgr;
        //时间  作用对象  计算方式
        private float triggerTime = 60f;
        private TimerEvent startEventTimer;//开启特殊事件的计时器
        private TimerEvent countDownTimer;//倒计时
        private int restTime = 0;
        private bool onSpecialEvent;//正在进行特殊事件
        private IdleSpecialEvent currentSpecialEvent;


        public bool IsEventTrigger
        {
            get;
            set;
        }

        public bool OnSpecialEvent
        {
            get
            {
                return onSpecialEvent;
            }
        }
        public int RestTime
        {
            get
            {
                return restTime;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_Start, StartEvent);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ToEnd, EndEvent);
        }
        public void Init()
        {
            startEventTimer = Timer.Instance.Register(triggerTime, 1, PrepareEvent).AddTo(gameObject);//开始计时
            restTime = 0;
            onSpecialEvent = false;
            playerMgr = PlayerMgr.Instance;
        }
        public override void Dispose()
        {
            base.Dispose();
            playerMgr = null;
            startEventTimer = null;//开启特殊事件的计时器
            countDownTimer = null;//事件进行的计时器
            currentSpecialEvent = null;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_Start, StartEvent);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ToEnd, EndEvent);
        }

        private void PrepareEvent(params object[] objs)
        {
            if(startEventTimer!=null)
            {
                Timer.Instance.DestroyTimer(startEventTimer);
                startEventTimer = null;
            }
        
            InitSpecialEvent();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSpecialEvent>(EnumEventType.Event_SpecialEvent_Prepare,currentSpecialEvent));
            IsEventTrigger = true;
        }

        private void StartEvent(BaseEventArgs baseEventArgs)
        {
            //onEventTimer = Timer.Instance.Register(currentSpecialEvent.stayTime, 1, EndEvent).AddTo(gameObject);
            restTime = currentSpecialEvent.stayTime;
            onSpecialEvent = true;
            countDownTimer = Timer.Instance.Register(1, currentSpecialEvent.stayTime, CountDown).AddTo(gameObject);
            StartSpecialEvent();
        }
        private void CountDown(params object[] objs)
        {
            restTime--;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_SpecialEvent_Update));
            if (restTime <= 0)
            {
                restTime = 0;
                EndEvent();
            }

        }

        private void EndEvent(BaseEventArgs baseEventArgs)
        {
            EndEvent();
        }

        private void EndEvent()
        {
            onSpecialEvent = false;
            if (countDownTimer != null)
            {
                Timer.Instance.DestroyTimer(countDownTimer);
                countDownTimer = null;
            }
            EndSpecialEvent();
            startEventTimer = Timer.Instance.Register(triggerTime, 1, PrepareEvent).AddTo(gameObject);//开始计时
        }

        public IdleSpecialEvent GetCurrentEvent()
        {
            return currentSpecialEvent;
        }


        #region 初始化特殊事件
        private void InitSpecialEvent()
        {
            IdleSpecialEvent specialEvent = IdleSpecialEventMgr.Instance.GetRandomSpecialEvent();
            if (specialEvent == null) return;
            switch (specialEvent.id)
            {
                case 1:
                    InitSpecialEvent_1(specialEvent);
                    break;
                case 2:
                    InitSpecialEvent_2(specialEvent);
                    break;
            }

        }
        //id为1的特殊事件初始化方法
        private void InitSpecialEvent_1(IdleSpecialEvent idleSpecialEvent)
        {
            currentSpecialEvent = new IdleSpecialEvent(idleSpecialEvent);
        }
        //id为2的特殊事件初始化
        private void InitSpecialEvent_2(IdleSpecialEvent idleSpecialEvent)
        {
            currentSpecialEvent = new IdleSpecialEvent(idleSpecialEvent);

        }

        #endregion


        #region 开始特殊事件

        private void StartSpecialEvent()
        {
            switch (currentSpecialEvent.id)
            {
                case 1:
                    StartSpecialEvent_1();
                    break;
                case 2:
                    StartSpecialEvent_2();
                    break;
            }
        }

        //开始id为1的特殊事件
        private void StartSpecialEvent_1()
        {

            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeCityTruckVolume, 2));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeForkVolume, 2));
        }
        //开始id为2的特殊事件
        private void StartSpecialEvent_2()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeForkSpeed, 2));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangePostSiteStaffSpeed, 2));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeSiteStaffSpeed, 2));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeTruckSpeed, 2));
        }

        #endregion


        #region 结束特殊事件

        private void EndSpecialEvent()
        {
            switch (currentSpecialEvent.id)
            {
                case 1:
                    EndSpecialEvent_1();
                    break;
                case 2:
                    EndSpecalEvent_2();
                    break;
            }
            
        }

        private void EndSpecialEvent_1()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeCityTruckVolume, 1));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeForkVolume, 1));
        }

        private void EndSpecalEvent_2()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeForkSpeed, 1));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangePostSiteStaffSpeed, 1));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeSiteStaffSpeed, 1));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_SpecialEvent_ChangeTruckSpeed, 1));
        }

        #endregion

    }



}


