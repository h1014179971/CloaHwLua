using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

namespace Delivery.Idle
{
    public class IdleInvestorCtrl : MonoSingleton<IdleInvestorCtrl>
    {
        private float triggerTime = 30;//投资人触发时间
        private TimerEvent triggerTimer;
        private Long2 income;
        private int incomeTimes;//收益倍数
        private int minTimes = 8;//最小倍数
        private int maxTimes = 13;//最大倍数
        public bool isTriggerInvestor;
        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Investor_End, EndInvestor);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Investor_Restart, RestartInvestor);
        }
        public void Init()
        {
            triggerTimer = Timer.Instance.Register(triggerTime, PrepareInvestor).AddTo(gameObject);
            isTriggerInvestor = false;
            

        }
        public override void Dispose()
        {
            base.Dispose();
            if(triggerTimer != null)
            {
                Timer.Instance.DestroyTimer(triggerTimer);
                triggerTimer = null;
            }
            
            income = Long2.zero;
            isTriggerInvestor = false;
        }
        public override void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Investor_End, EndInvestor);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Investor_Restart, RestartInvestor);
        }

        private void PrepareInvestor(params object[] objs)
        {
            if(triggerTimer!=null)
            {
                Timer.Instance.DestroyTimer(triggerTimer);
                triggerTimer = null;
            }
            
            isTriggerInvestor = true;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Investor_Prepare));

        }
        //获取收益
        public Long2 GetIncome()
        {

            if(incomeTimes<=0)
            {
                incomeTimes = Random.Range(minTimes, maxTimes);
                income = PlayerMgr.Instance.GetCityIncome() * incomeTimes;
            }
            return income;
        }

        private void RestartInvestor(BaseEventArgs baseEventArgs)
        {
            isTriggerInvestor = false;
            income = Long2.zero;
            incomeTimes = 0;
            triggerTimer = Timer.Instance.Register(triggerTime, PrepareInvestor);
        }

        private void EndInvestor(BaseEventArgs baseEventArgs)
        {
            isTriggerInvestor = false;
            PlayerMgr.Instance.AddMoney(income);
            income = Long2.zero;
            incomeTimes = 0;
            triggerTimer = Timer.Instance.Register(triggerTime, PrepareInvestor);
        }
    }
}

