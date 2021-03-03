using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundation {
    public class TimerEvent
    {

        public string m_key = "";

        /// <summary>
        /// 重复调用次数,-1代表一直调用
        /// </summary>
        public int m_repeatCount = 0;
        public int m_currentRepeat = 0;

        /// <summary>
        /// 是否忽略时间缩放
        /// </summary>
        public bool m_isIgnoreTimeScale = false;
        public TimerCallBack m_callBack;
        public object[] m_objs;

        public float m_interval;
        public float m_currentTimer = 0;

        public bool m_isDone = false;
        public bool m_hasAutoDestroyOwner;
        public GameObject m_autoDestroyOwner;

        public TimerEvent AddTo(GameObject gameObject)
        {
            m_autoDestroyOwner = gameObject;
            m_hasAutoDestroyOwner = m_autoDestroyOwner != null;
            return this;
        }
        public void Update()
        {
            if (m_hasAutoDestroyOwner && m_autoDestroyOwner == null)
            {
                Timer.Instance.DestroyTimer(this);
                return;
            }
            if (m_isIgnoreTimeScale)
            {
                m_currentTimer += Time.unscaledDeltaTime;
            }
            else
            {
                m_currentTimer += Time.deltaTime;
            }

            if (m_currentTimer >= m_interval)
            {
                m_isDone = true;
            }
        }

        public void CompleteTimer()
        {
            CallBackTimer();

            if (m_repeatCount > 0)
            {
                m_currentRepeat++;
            }

            if (m_currentRepeat != m_repeatCount)
            {
                m_isDone = false;
                m_currentTimer = 0;
            }
        }

        public void CallBackTimer()
        {
            if (m_callBack != null)
            {
                try
                {
                    m_callBack(m_objs);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public void ResetTimer()
        {
            m_currentTimer = 0;
            m_currentRepeat = 0;
            m_isDone = false;
            m_hasAutoDestroyOwner = false;
            m_autoDestroyOwner = null;
        }
    }
    public delegate void TimerCallBack(params object[] objs);
}


