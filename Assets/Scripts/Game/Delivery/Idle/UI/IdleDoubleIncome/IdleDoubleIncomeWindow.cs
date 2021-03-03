using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
using Foundation;

namespace Delivery.Idle
{

    public class IdleDoubleIncomeWindow : UIWindow
    {
        private MyButton closeBtn;
        private MyButton startBtn;
        private Slider processSlider;
        private MyText restTimeText;
        private GameObject loadingImage;

        private IdleDoubleIncomeCtrl idleDoubleIncomeCtrl;

        private IdleUIAnimation anim;

        private void Awake()
        {
            idleDoubleIncomeCtrl = IdleDoubleIncomeCtrl.Instance;

            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
            startBtn = this.GetComponentByPath<MyButton>("bg/btn-start");
            processSlider = this.GetComponentByPath<Slider>("bg/processSlider");
            restTimeText = processSlider.GetComponentByPath<MyText>("restTime");
            loadingImage = startBtn.GetComponentByPath<Transform>("loading").gameObject;

            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);

            AddAllListener();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_DoubleIncome_Update, OnTimerUpdate);
            PlatformFactory.Instance.onLoadRewardResult -= SetLoadingImage;
        }

        private void AddAllListener()
        {
            closeBtn.onClick.AddListener(OnCloseBtnClick);
            startBtn.onClick.AddListener(OnConfirmBtnClick);

            EventDispatcher.Instance.AddListener(EnumEventType.Event_DoubleIncome_Update, OnTimerUpdate);

            PlatformFactory.Instance.onLoadRewardResult += SetLoadingImage;
        }


        private void OnCloseBtnClick()
        {
            CloseWindow();
        }


        private void CloseWindow()
        {
            anim.Exit();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }

        private void OnExitFinish()
        {
            UIController.Instance.CloseCurrentWindow();
        }


        private void OnConfirmBtnClick()
        {
            if (PlatformFactory.Instance.isRewardLoaded())
            {
                AudioCtrl.Instance.PauseBackgroundMusic(false);
                anim.ExitImmediatelty();
                Time.timeScale = 0;
                PlatformFactory.Instance.showRewardedVideo("DoubleIncomeAd", OnFinishPlayAd);
            }
        }


        private void OnFinishPlayAd(bool finish)
        {
            if (finish)
            {
                idleDoubleIncomeCtrl.UpdateDoubleIncome();
            }
            Time.timeScale = 1.0f;
            IdleTipCtrl.Instance.ShowDoubleIncomeTip();
            AudioCtrl.Instance.UnPauseBackgroundMusic(false);
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }



        private void SetLoadingImage(bool isHide)
        {
            loadingImage.SetActive(!isHide);
        }


        private void InitData()
        {
           
            UpdateSliderProcess();

            loadingImage.SetActive(!PlatformFactory.Instance.isRewardLoaded());
        }

        private void OnTimerUpdate(BaseEventArgs baseEventArgs)
        {
            UpdateSliderProcess();
        }

        private void UpdateSliderProcess()
        {
            int restTime = idleDoubleIncomeCtrl.RestTime;
            if (restTime <= 0)
                restTimeText.text = "0";
            else
                restTimeText.text = TimeUtils.secondsToString1(restTime);
            
            int maxTime = idleDoubleIncomeCtrl.MaxTime;
            processSlider.value = (float)restTime / maxTime;

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
            InitData();
        }
    }

}

