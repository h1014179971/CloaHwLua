using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIFramework
{
    public abstract class UIWindow : UIPanel
    {
        /// <summary>
        /// 主要用于初始化打开关闭动画和回调函数
        /// </summary>
        protected abstract void InitWindow(object arg = null);

        public void Init(object arg = null)
        {
            InitWindow(arg);
        }
        public override void Hide()
        {
            AfterHide.AddListener(() => {
                gameObject.SetActive(false);
            });
            base.Hide();
        }
    }
}