using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIFramework
{
    public abstract class UIPopup : UIWindow
    {
        protected UnityAction ConfirmAction;// 确定
        protected UnityAction RefuseAction; // 拒绝
        protected UnityAction CancelAction; // 取消

        /// <summary>
        /// 主要用于初始化打开关闭动画和回调函数
        /// </summary>
        protected abstract void InitPopup();

        protected override void InitWindow(object arg = null)
        {
            InitPopup();
        }

        protected virtual void OnConfirm()
        {
            if (ConfirmAction != null)
            {
                AfterClose.AddListener(ConfirmAction);
            }

            UIController.Instance.BackToLastWindow();
            //Close();
        }

        protected virtual void OnRefuse()
        {
            if (RefuseAction != null)
            {
                AfterClose.AddListener(RefuseAction);
            }
            UIController.Instance.BackToLastWindow();
        }

        protected virtual void OnCancel()
        {
            if (CancelAction != null)
            {
                AfterClose.AddListener(CancelAction);   
            }
            UIController.Instance.BackToLastWindow();
        }

        public void SetCallback(UnityAction confirmCB, UnityAction refuseCB, UnityAction cancelCB)
        {
            ConfirmAction = confirmCB;
            RefuseAction = refuseCB;
            CancelAction = cancelCB;
        }
    }
}