using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using DG.Tweening;
using Foundation;

namespace Delivery.Idle
{
    public class OfflineWindow : UIBlockerBase
    {
        private OfflineWindowLogic offlineWindowLogic;

        private MyText offlineTime;
        private MyText offlineIncome;
        private Long2 income;//离线收益
        private MyButton closeBtn;//关闭按钮
        private MyButton doubleBtn;//双倍领取按钮
        private GameObject loadingImage;//正在加载图

        private Transform fxNode;
        private Vector3 fxEndPoint;

        private RectTransform rectTrans;
        private IdleUIAnimation anim;

        private int incomeTimes = 1;//收益倍数
        private IdleTopUI topUI;
        private void Awake()
        {
            offlineWindowLogic = OfflineWindowLogic.Instance;
            IdleGuideCtrl.Instance.isOfflineWindowActive = true;

            offlineTime = this.GetComponentByPath<MyText>("bg/offlineTime");
            offlineIncome = this.GetComponentByPath<MyText>("bg/offlineIncomBg/offlineIncome");
            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
            doubleBtn = this.GetComponentByPath<MyButton>("bg/btn-double");
            loadingImage = doubleBtn.GetComponentByPath<Transform>("loading").gameObject;

            fxNode = RootCanvas.Instance.GetComponentByPath<Transform>("UIFxRoot");
            topUI = FindObjectOfType<IdleTopUI>();
            fxEndPoint = topUI.GetMoneyIconPos();

            InitData();

            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);

            AddAllListener();

            anim.Enter();
        }


        private void OnDestroy()
        {
            PlatformFactory.Instance.onLoadRewardResult -= SetLoadingImage;
        }

        private void AddAllListener()
        {
            closeBtn.onClick.AddListener(OnCloseBtnClick);
            doubleBtn.onClick.AddListener(OnDoubleRewardBtnClick);
          
            PlatformFactory.Instance.onLoadRewardResult += SetLoadingImage;
        }

        private void SetLoadingImage(bool isHide)
        {
            loadingImage.SetActive(!isHide);
        }


        private void OnCloseBtnClick()
        {
            incomeTimes = 1;
            anim.Exit();
        }

        private void OnDoubleRewardBtnClick()
        {
           
            if (PlatformFactory.Instance.isRewardLoaded())
            {
                AudioCtrl.Instance.PauseBackgroundMusic(false);
                Time.timeScale = 0;
                PlatformFactory.Instance.showRewardedVideo("OfflineAd", OnFinishPlayAd);
            }
        }
        
        private void OnFinishPlayAd(bool finish)
        {
            Time.timeScale = 1.0f;
            AudioCtrl.Instance.UnPauseBackgroundMusic(false);
            if (finish)
            {
                AudioCtrl.Instance.PlayMultipleSound(GameAudio.vendingMoney);
                incomeTimes = 2;
            }
            else
            {
                incomeTimes = 1;
            }

            anim.Exit();
     
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }



        private void OnExitFinish()
        {
            CloseWindow();
        }

        private void InitData()
        {
            offlineTime.text = $"您离开了{offlineWindowLogic.GetLeaveTime()}，在离开公司期间您赚取了：";
            income = offlineWindowLogic.GetOfflineIncome();
            offlineIncome.text = UnitConvertMgr.Instance.GetFloatValue(income, 2);

            loadingImage.SetActive(!PlatformFactory.Instance.isRewardLoaded());
        }

        private void CloseWindow()
        {
            gameObject.SetActive(false);
            IdleFxHelper.PlayGetRewardFxUI(Vector3.zero, () =>
            {
                IdleGuideCtrl.Instance.isOfflineWindowActive = false;
                PlayerMgr.Instance.AddMoney(income * incomeTimes);
                Destroy(this.gameObject);
            });
           
        }

    }
}

