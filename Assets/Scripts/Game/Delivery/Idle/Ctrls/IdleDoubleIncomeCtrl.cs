using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Delivery.Idle
{

    public class IdleDoubleIncomeCtrl : MonoSingleton<IdleDoubleIncomeCtrl>
    {
        private long _startTime;//双倍收益开始的时间
        private int _timePerAd=600;//每个广告加的时间
        //private int _totalTime;//双倍收益持续的总时间
        private int _maxTime=3600;//最大的持续时间
        //private int _stayTime;//剩余时间

        private TimerEvent updateTimer;//时间更新计时器

        private PlayerMgr playerMgr;
        public void Init()
        {
            playerMgr = PlayerMgr.Instance;
            _startTime = playerMgr.LastDoubleIncomeTime;
            //_totalTime = PlayerMgr.Instance.LastDoubleIncomeTotalTime;

            if (_startTime>0)
            {
                long nowTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);
                //_stayTime = Convert.ToInt32(nowTime - _startTime);
                int stayTime= Convert.ToInt32(nowTime - _startTime);
                if (stayTime < playerMgr.LastDoubleIncomeTotalTime)
                {
                    playerMgr.LastDoubleIncomeTotalTime -= stayTime;
                    //重新开始双倍收益
                    StartDoubleIncome();
                    return;
                }
            }
            _startTime = -1;
            playerMgr.LastDoubleIncomeTotalTime = 0;
            //_stayTime = 0;
            PlayerMgr.Instance.LastDoubleIncomeTime = _startTime;
            //PlayerMgr.Instance.LastDoubleIncomeTotalTime = _totalTime;
        }

        //持续时间
        //public int StayTime
        //{
        //    get
        //    {
        //        //long nowTime = TimeUtils.ConvertLongUtcDateTime(DateTime.UtcNow);
        //        //return Convert.ToInt32(nowTime - _startTime);
        //        return _stayTime;
        //    }
        //}
        //总时间
        //public int TotalTime
        //{
        //    get
        //    {
        //        return playerMgr.LastDoubleIncomeTotalTime;
        //    }
        //}
        //剩余时间
        public int RestTime
        {
            //get
            //{
            //    return TotalTime - StayTime;
            //}
            get
            {
                return playerMgr.LastDoubleIncomeTotalTime;
            }
        }

        public int MaxTime
        {
            get
            {
                return _maxTime;
            }
        }

        //看完广告后更新双倍收益
        public void UpdateDoubleIncome()
        {
            playerMgr.LastDoubleIncomeTotalTime += _timePerAd;
            if (playerMgr.LastDoubleIncomeTotalTime > _maxTime)
                playerMgr.LastDoubleIncomeTotalTime = _maxTime;
            //PlayerMgr.Instance.LastDoubleIncomeTotalTime = _totalTime;
            if (_startTime<=0)
            {
                StartDoubleIncome();
            }

        }
        //开启双倍收益
        private void StartDoubleIncome()
        {
            if(_startTime<=0)
            {
                DateTime time = DateTime.UtcNow;
                _startTime = TimeUtils.ConvertLongUtcDateTime(time);
                PlayerMgr.Instance.LastDoubleIncomeTime = _startTime;
            }

            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_DoubleIncome_Start));
            updateTimer = Timer.Instance.Register("doubleIncomeTimer",1, -1, false, UpdateTimer).AddTo(gameObject);
        }
        //更新计时器
        private void UpdateTimer(params object[] objs)
        {
            //if(StayTime>=playerMgr.LastDoubleIncomeTotalTime)
            //{
            //    EndDoubleIncome();
            //    return;
            //}
            //_stayTime++;

            playerMgr.LastDoubleIncomeTotalTime--;
            if(playerMgr.LastDoubleIncomeTotalTime<0)
            {
                EndDoubleIncome();
                return;
            }
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_DoubleIncome_Update));
        }

        //结束双倍收益
        private void EndDoubleIncome()
        {
            _startTime = -1;
            playerMgr.LastDoubleIncomeTotalTime = 0;
            //_stayTime = 0;
            PlayerMgr.Instance.LastDoubleIncomeTime = _startTime;
            //PlayerMgr.Instance.LastDoubleIncomeTotalTime = _totalTime;

            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_DoubleIncome_End));
            if(updateTimer!=null)
            {
                Timer.Instance.DestroyTimer(updateTimer);
                updateTimer = null;
            }
           
        }

    }

}

