using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
namespace Delivery.Idle
{
    public class IdleTaskWindow : UIWindow
    {
        private IdleTaskWindowLogic idleTaskWindowLogic;

        private MyButton closeBtn;
        private IdleTaskItemCtrl taskItemCtrl;

        private IdleUIAnimation anim;
        private void Awake()
        {
            idleTaskWindowLogic = IdleTaskWindowLogic.Instance;

            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
            taskItemCtrl = this.GetComponentByPath<IdleTaskItemCtrl>("bg/tasksBg");

            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);

            AddAllListener();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseTask, CloseWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, OnCloseCurrentWindow);
        }

        private void OnExitFinish()
        {
            idleTaskWindowLogic.OnCloseBtnClick();
        }

        private void CloseWindow(BaseEventArgs baseEventArgs)
        {
            if (baseEventArgs == null)
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            EventArgsOne<IdleTaskBase> argsOne = (EventArgsOne<IdleTaskBase>)baseEventArgs;
            anim.SetExitFinishEvent(() =>
            {
                idleTaskWindowLogic.OnCloseBtnClick();
                if (argsOne != null && argsOne.param1 != null)
                    idleTaskWindowLogic.ShowOtherWindow(argsOne.param1);
            });
            anim.Exit();
        }
        
        private void OnCloseCurrentWindow(BaseEventArgs baseEventArgs)
        {
            if (!IsActive) return;
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.SetExitFinishEvent(() =>
            {
                idleTaskWindowLogic.OnCloseBtnClick();
            });
            anim.Exit();
        }


        private void AddAllListener()
        {
            closeBtn.onClick.AddListener(() => { CloseWindow(null); });
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseTask, CloseWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, OnCloseCurrentWindow);
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
            idleTaskWindowLogic.InitData();
            taskItemCtrl.InitAllTaskItem();
        }
    }
}

