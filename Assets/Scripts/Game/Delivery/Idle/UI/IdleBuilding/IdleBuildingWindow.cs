using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using DG.Tweening;

namespace Delivery.Idle
{
    public class IdleBuildingWindow : UIWindow
    {
        private IdleBuildingLogic idleBuildingLogic;
        private MyButton closeBtn;
        private ScrollRectEx postSites;
        private int viewCount;

        private IdleUIAnimation anim;
        private void Awake()
        {
            idleBuildingLogic = IdleBuildingLogic.Instance;
            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
            postSites = this.GetComponentByPath<ScrollRectEx>("bg/sitesScrollView");
            viewCount = postSites.viewCount;


            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);

            closeBtn.onClick.AddListener(CloseWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_UnLock, OnSuccessBuild);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_UnLock, OnSuccessBuild);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
            idleBuildingLogic.Destroy();
        }

        private void OnExitFinish()
        {
            idleBuildingLogic.OnCloseBtnClick();
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

        private void OnSuccessBuild(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSiteModel> argsOne = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            Vector3 pos = argsOne.param1.transform.position;
            pos.z = 1;
            float cameraSize = -1;
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, pos, cameraSize));
            CloseWindow();
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
            int siteCount = idleBuildingLogic.GetIdleSitesCount();
            if (siteCount <= viewCount)
                postSites.viewCount = siteCount;
            postSites.UpdateContne(siteCount);

        }
    }
}

