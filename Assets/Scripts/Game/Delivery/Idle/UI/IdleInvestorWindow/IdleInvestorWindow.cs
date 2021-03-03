using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using DG.Tweening;

namespace Delivery.Idle
{
    public class IdleInvestorWindow : UIWindow
    {
        private IdleInvestorLogic idleInvestorLogic;

        private MyText income;
        private MyButton comfirmBtn;
        private MyButton cancelBtn;
        private GameObject loadingImage;

        private IdleUIAnimation anim;

        private Transform fxNode;
        private Vector3 fxEndPoint;
        private IdleTopUI topUI;
        private void Awake()
        {
            idleInvestorLogic = IdleInvestorLogic.Instance;
            
            income = this.GetComponentByPath<MyText>("bg/incomeBg/income");
            cancelBtn = this.GetComponentByPath<MyButton>("bg/btn-cancel");
            comfirmBtn = this.GetComponentByPath<MyButton>("bg/btn-conform");
            loadingImage = comfirmBtn.GetComponentByPath<Transform>("loading").gameObject;

            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition.y - 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);

            fxNode= RootCanvas.Instance.GetComponentByPath<Transform>("UIFxRoot");
            topUI = FindObjectOfType<IdleTopUI>();
            fxEndPoint = topUI.GetMoneyIconPos();

            AddAllListener();
        }

        private void OnDestroy()
        {
            PlatformFactory.Instance.onLoadRewardResult -= SetLoadingImage;
        }

        private void AddAllListener()
        {
            cancelBtn.onClick.AddListener(CloseWindow);
            comfirmBtn.onClick.AddListener(OnConfirmBtnClick);
            PlatformFactory.Instance.onLoadRewardResult += SetLoadingImage;
        }

        private void OnExitFinish()
        {
            idleInvestorLogic.CloseCurrentWindow();
        }

        private void CloseWindow()
        {
            anim.Exit();
            idleInvestorLogic.RestartInvestor();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }


        private void OnConfirmBtnClick()
        {
            if (PlatformFactory.Instance.isRewardLoaded())
            {
                AudioCtrl.Instance.PauseBackgroundMusic(false);
                anim.ExitImmediatelty();
                Time.timeScale = 0;
                PlatformFactory.Instance.showRewardedVideo("IdleInvestorAd", OnFinishPlayAd);
            }
        }

        private void OnFinishPlayAd(bool finish)
        {
            Time.timeScale = 1.0f;
            AudioCtrl.Instance.UnPauseBackgroundMusic(false);
            if (finish)
            {
                IdleFxHelper.PlayGetRewardFxUI(Vector3.zero, () => {
                    AudioCtrl.Instance.PlayMultipleSound(GameAudio.vendingMoney);
                    idleInvestorLogic.EndInvestor();
                    topUI.PlayMoneyAnim();
                });

                //FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleInvestorReward, fxNode);
                //model.transform.DOMove(fxEndPoint, 1.0f).OnComplete(() => {
                //    idleInvestorLogic.EndInvestor();
                //    model.gameObject.SetActive(false);
                //    topUI.PlayMoneyAnim();
                //});
                Dictionary<string, string> proertie = new Dictionary<string, string>();
                proertie["ad_pos"] = "investors";
                PlatformFactory.Instance.TAEventPropertie("gt_ad_show", proertie);
            }
            else
            {
                idleInvestorLogic.RestartInvestor();
            }
        
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
        }

        

        private void SetLoadingImage(bool isHide)
        {
            loadingImage.SetActive(!isHide);
        }

        private void InitData()
        {
            IdleInvestorCtrl.Instance.isTriggerInvestor = false;
            income.text = UnitConvertMgr.Instance.GetFloatValue(idleInvestorLogic.GetIncome(), 2);
            loadingImage.SetActive(!PlatformFactory.Instance.isRewardLoaded());
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

