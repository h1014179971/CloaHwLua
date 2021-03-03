using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Foundation
{
    //[RequireComponent(typeof(TimerEvent))]
    public class Timer : MonoSingleton<Timer> 
    {

        public  List<TimerEvent> m_timers = new List<TimerEvent>();
        public List<TimerEvent> m_returnTimers = new List<TimerEvent>(); //回收timerevent

        public  void Init()
        {

        }

         void Update()
        {
            //for (int i = 0; i < m_timers.Count; i++)
            for (int i = m_timers.Count-1; i >=0; i--)
            {
                TimerEvent e = m_timers[i];
                if (e == null)
                    continue;
                e.Update();

                if (e.m_isDone)
                {
                    e.CompleteTimer();

                    if (e.m_isDone)
                    {
                        m_timers.Remove(e);
                        ReturnTimer(e);
                    }
                }
            }
        }

        public  bool GetIsExistTimer(string key)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                var e = m_timers[i];
                if (e.m_key == key)
                {
                    return true;
                }
            }

            return false;
        }

        public  TimerEvent GetTimer(string key)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                var e = m_timers[i];
                if (e.m_key == key)
                {
                    return e;
                }
            }

            return null;
        }

        /// <summary>
        /// 延迟调用
        /// </summary>
        /// <param name="interval">开始延时</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent Register(float interval, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(null, interval, 0, false, callBack, objs);
        }

        /// <summary>
        /// 间隔一定时间重复调用
        /// </summary>
        /// <param name="interval">开始延时</param>
        /// <param name="repeatCount">重复调用的次数</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent Register(float interval, int repeatCount, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(null, interval, repeatCount, false, callBack, objs);
        }

        /// <summary>
        /// 延迟调用 忽略TimeScale
        /// </summary>
        /// <param name="interval">开始延时</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent Register(float interval, bool isIgnoreTimeScale, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(null, interval, 0, isIgnoreTimeScale, callBack, objs);
        }

        /// <summary>
        /// 延迟调用
        /// </summary>
        /// <param name="key">Timer的名字</param>
        /// <param name="interval">开始延时</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent Register(string key, float interval, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(key, interval, 0, false, callBack, objs);
        }

        /// <summary>
        /// 间隔一定时间重复调用
        /// </summary>
        /// <param name="key">Timer的名字</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="repeatCount">重复调用的次数</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent Register(string key, float interval, int repeatCount, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(key, interval, repeatCount, false, callBack, objs);
        }

        /// <summary>
        /// 延迟调用 忽略TimeScale
        /// </summary>
        /// <param name="key">Timer的名字</param>
        /// <param name="interval">开始延时</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent Register(string key, float interval, bool isIgnoreTimeScale, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(key, interval, 0, isIgnoreTimeScale, callBack, objs);
        }

        /// <summary>
        /// 间隔一定时间重复调用 忽略TimeScale
        /// </summary>
        /// <param name="key">Timer的名字</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="repeatCount">重复调用的次数</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent Register(string key, float interval, int repeatCount, bool isIgnoreTimeScale, TimerCallBack callBack, params object[] objs)
        {
            return AddTimer(key, interval, repeatCount, isIgnoreTimeScale, callBack, objs);
        }

        /// <summary>
        /// 添加一个Timer
        /// </summary>
        /// <param name="interval">间隔时间</param>
        /// <param name="repeatCount">重复调用的次数</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="key">Timer的名字</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="objs">回调函数的参数</param>
        /// <returns></returns>
        public  TimerEvent AddTimer(string key, float interval, int repeatCount, bool isIgnoreTimeScale, TimerCallBack callBack, params object[] objs)
        {
            TimerEvent te = ExtractReturnTimer();
            if (te == null)
                te = new TimerEvent();
            //TimerEvent te = new TimerEvent();
            te.m_key = key ?? te.GetHashCode().ToString();
            te.m_currentTimer = 0;
            te.m_interval = interval;
            te.m_repeatCount = repeatCount;
            te.m_currentRepeat = 0;
            te.m_isIgnoreTimeScale = isIgnoreTimeScale;

            te.m_callBack = callBack;
            te.m_objs = objs;

            te.m_isDone = false;
            te.m_hasAutoDestroyOwner = false;
            te.m_autoDestroyOwner = null;

            //m_timers.Add(te);
            m_timers.Insert(0, te);
            return te;
        }
        private TimerEvent ExtractReturnTimer()
        {
            if (m_returnTimers.Count > 0)
            {
                TimerEvent timerEvent = m_returnTimers[0];
                m_returnTimers.Remove(timerEvent);
                return timerEvent;
            }
            return null;
        }

        private void ReturnTimer(TimerEvent timerEvent)
        {
            timerEvent.ResetTimer();
            if (!m_returnTimers.Contains(timerEvent))
                m_returnTimers.Add(timerEvent);
        }

        public  void DestroyTimer(TimerEvent timer, bool isCallBack = false)
        {
            if (m_timers.Contains(timer))
            {
                if (isCallBack)
                {
                    timer.CallBackTimer();
                }

                m_timers.Remove(timer);
                ReturnTimer(timer);
            }
            else
            {
                Debug.LogError("Dont exist timer:" + timer);
            }
        }

        public  void UnRegister(string key, bool isCallBack = false)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                TimerEvent te = m_timers[i];
                if (te.m_key.Equals(key))
                {
                    DestroyTimer(te, isCallBack);
                }
            }
        }

        public  void UnRegister(bool isCallBack = false)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                if (isCallBack)
                {
                    m_timers[i].CallBackTimer();
                }
            }

            m_timers.Clear();
        }

        public  void ResetTimer(TimerEvent timer)
        {
            if (m_timers.Contains(timer))
            {
                timer.ResetTimer();
            }
            else
            {
                Debug.LogError("Dont exist timer:" + timer);
            }
        }

        public  void ResetTimer(string key)
        {
            for (int i = 0; i < m_timers.Count; i++)
            {
                var e = m_timers[i];

                if (e.m_key.Equals(key))
                {
                    ResetTimer(e);
                }
            }
        }
    }
}

