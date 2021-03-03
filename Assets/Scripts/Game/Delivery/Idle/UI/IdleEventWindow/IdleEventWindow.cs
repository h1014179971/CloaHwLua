using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;

namespace Delivery.Idle
{
    public class IdleEventWindow : UIWindow
    {
        private IdleEventWindowLogic idleEventWindowLogic;

        private MyButton closeBtn;
        private MyButton attendBtn;
        private GameObject loadingImage;
        private MyText desc;
        private Image icon;

        private IdleUIAnimation anim;
        private void Awake()
        {
            idleEventWindowLogic = IdleEventWindowLogic.Instance;

            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
            attendBtn = this.GetComponentByPath<MyButton>("bg/btn-attend");
            loadingImage = attendBtn.GetComponentByPath<Transform>("loading").gameObject;
            desc = this.GetComponentByPath<MyText>("bg/desc");
            icon = this.GetComponentByPath<Image>("bg/icon");

            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);

          
            AddAllListener();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
            PlatformFactory.Instance.onLoadRewardResult -= SetLoadingImage;
        }

        private void AddAllListener()
        {
            closeBtn.onClick.AddListener(CloseWindow);
            attendBtn.onClick.AddListener(OnAttendBtnClick);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);

            PlatformFactory.Instance.onLoadRewardResult += SetLoadingImage;
        }

        private void SetLoadingImage(bool isHide)
        {
            loadingImage.SetActive(!isHide);
        }

        private void OnExitFinish()
        {
            idleEventWindowLogic.OnCloseBtnClick();
        }

        private void CloseWindow()
        {
            anim.Exit();
            idleEventWindowLogic.EndSpecialEvent();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }

        private void CloseWindow(BaseEventArgs baseEventArgs)
        {
            if (!IsActive) return;
            anim.Exit();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }

        public void OnAttendBtnClick()
        {
            if (PlatformFactory.Instance.isRewardLoaded())
            {
                AudioCtrl.Instance.PauseBackgroundMusic(false);
                anim.ExitImmediatelty();
                Time.timeScale = 0;
                PlatformFactory.Instance.showRewardedVideo("IdleEventAd", OnFinishPlayAd);
            }

        }

        private void OnFinishPlayAd(bool finish)
        {
            if(finish)
            {
                idleEventWindowLogic.StartSpecialEvent();
                Dictionary<string, string> proertie = new Dictionary<string, string>();
                proertie["ad_pos"] = "events";
                PlatformFactory.Instance.TAEventPropertie("gt_ad_show", proertie);
            }
            else
                idleEventWindowLogic.EndSpecialEvent();
            AudioCtrl.Instance.UnPauseBackgroundMusic(false);
            Time.timeScale = 1.0f;
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
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
            desc.text = idleEventWindowLogic.GetCurrentEvent().desc;
            loadingImage.SetActive(!PlatformFactory.Instance.isRewardLoaded());
            string iconName = idleEventWindowLogic.GetCurrentEvent().spriteRes;
            icon.sprite = AssetHelper.GetSpecialEventBtnSprite(iconName);
        }
    }
}

