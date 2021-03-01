using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using DG.Tweening;
using Foundation;

namespace Delivery.Idle
{
    public class IdleSiteInfoWindow : UIWindow
    {
        private IdleSiteInfoLogic idleSiteInfoLogic;

        #region UI控件
        private MyText transportSpeed;//运输速度
        private MyText storeRest;//仓库当前存货量

        private MyText volume;//装货量
        private MyText volumeLv;//装货量等级
        private MyText upgradeVolumeCost;//升级装货量花费
        private ConsecutiveButton upgradeVolumeBtn;//升级装货量按钮


        private MyText storeLv;//仓库等级
        private MyText storeVolume;//仓库容量
        private MyText upgradeStoreCost;//升级仓库容量花费
        private ConsecutiveButton upgradeStoreBtn;//升级货车容量按钮

        private GameObject maxVolumeLvBtn;
        private GameObject volumeBtnGrayBg;
        private GameObject maxStoreLvBtn;
        private GameObject storeBtnGrayBg;

        #endregion

        private IdleUIAnimation anim;

        private CameraAotoMove aotoMove;

        private void Awake()
        {
            idleSiteInfoLogic = IdleSiteInfoLogic.Instance;

            #region UI控件初始化
            transportSpeed = this.GetComponentByPath<MyText>("bg/content/transbg/transportSpeed");
            storeRest = this.GetComponentByPath<MyText>("bg/content/restBg/rest");

            volume = this.GetComponentByPath<MyText>("bg/content/siteVolume/siteVolumeBg/siteVolume");
            volumeLv = this.GetComponentByPath<MyText>("bg/content/siteVolume/siteVolumeBg/icon/siteVolumeLv");
            upgradeVolumeBtn = this.GetComponentByPath<ConsecutiveButton>("bg/content/siteVolume/siteVolumeBg/btn-upgradeSiteVolume");
            maxVolumeLvBtn = this.GetComponentByPath<Transform>("bg/content/siteVolume/siteVolumeBg/btn-maxLv").gameObject;
            volumeBtnGrayBg = upgradeVolumeBtn.GetComponentByPath<Transform>("bg-gray").gameObject;
            upgradeVolumeCost = upgradeVolumeBtn.GetComponentByPath<MyText>("upgradeVolumeCost");
          

         
            storeLv = this.GetComponentByPath<MyText>("bg/content/storeVolume/siteSpeedBg/icon/storeLv");
            storeVolume = this.GetComponentByPath<MyText>("bg/content/storeVolume/siteSpeedBg/storeVolume");
            
            upgradeStoreBtn = this.GetComponentByPath<ConsecutiveButton>("bg/content/storeVolume/siteSpeedBg/btn-upgradeStoreSpeed");
            maxStoreLvBtn = this.GetComponentByPath<Transform>("bg/content/storeVolume/siteSpeedBg/btn-maxLv").gameObject;
            storeBtnGrayBg = upgradeStoreBtn.GetComponentByPath<Transform>("bg-gray").gameObject;
            upgradeStoreCost = upgradeStoreBtn.GetComponentByPath<MyText>("upgradeStoreCost");


            #endregion
            aotoMove = GameObject.FindObjectOfType<CameraAotoMove>();

            

            AddUIListener();


            //动画相关
            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, -200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, OnEnterFinish, OnExitFinish);

        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeVolume, InitData);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeCycle, InitData);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeStoreVolume, InitData);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeStoreRest, UpdateRestItem);
        }

        private void AddUIListener()
        {
            EventTriggerListener.Get(transform.Find("eventBg").gameObject).SetEventHandle(EnumTouchEventType.OnClick, CloseWindow);
            upgradeVolumeBtn.AddClickListener(idleSiteInfoLogic.OnUpgradeVolumeBtnClick);
            //upgradeSpeedBtn.AddClickListener(idleSiteInfoLogic.OnUpgradeSpeedBtnClick);
            upgradeStoreBtn.AddClickListener(idleSiteInfoLogic.OnUpgradeStoreBtnClick);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeVolume, InitData);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeCycle, InitData);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeStoreVolume, InitData);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeStoreRest, UpdateRestItem);
        }


        private void CameraMoveBack()
        {
            GameObject site = GameObject.FindGameObjectWithTag("DeliverySite");
            if (site == null) return;
            Vector3 targetPos = site.transform.position;
            targetPos.z = 1;
            //EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, targetPos, -1));
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3,float>(EnumEventType.Event_Camera_MoveBack, targetPos,-1));
        }

        private void CloseWindow(GameObject listener, object obj, params object[] objs)
        {
           
            if (aotoMove.IsMoving) return;
            CameraMoveBack();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }
        private void CloseWindow(BaseEventArgs baseEventArgs)
        {
            if (!IsActive) return;
            CameraMoveBack();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }

     



        private void UpdateBtnState(BaseEventArgs baseEventArgs)
        {
            Long2 volumeCost = idleSiteInfoLogic.GetUpgradeVolumeCost();
            Long2 storeCost = idleSiteInfoLogic.GetUpdateStoreCost();
            InitBtns(volumeCost, storeCost);
        }
        //更新仓库剩余货物量
        private void UpdateRestItem(BaseEventArgs baseEventArgs)
        {
            storeRest.text = idleSiteInfoLogic.GetStoreRest().ToString();
        }

        private void InitData(BaseEventArgs baseEventArgs)
        {
    
            transportSpeed.text = idleSiteInfoLogic.GetTranportSpeed() + "/分钟";
            storeRest.text = idleSiteInfoLogic.GetStoreRest().ToString();

            int lVolume = idleSiteInfoLogic.GetVolume();
            volume.text = lVolume.ToString();
            volumeLv.text = "Lv." + idleSiteInfoLogic.GetVolumeLv().ToString();
            Long2 volumeCost = idleSiteInfoLogic.GetUpgradeVolumeCost();
            upgradeVolumeCost.text = UnitConvertMgr.Instance.GetFloatValue(volumeCost, 2);

            storeLv.text = "Lv." + idleSiteInfoLogic.GetStoreLv().ToString();
            storeVolume.text = idleSiteInfoLogic.GetStoreVolume().ToString();
            Long2 storeCost = idleSiteInfoLogic.GetUpdateStoreCost();
            upgradeStoreCost.text = UnitConvertMgr.Instance.GetFloatValue(storeCost, 2);

            //InitBtns(volumeCost, speedCost);
            InitBtns(volumeCost, storeCost);

        }

        private void InitBtns(Long2 volumeCost,Long2 speedCost)
        {

            if (idleSiteInfoLogic.IsMaxStoreLv())
            {
                upgradeStoreBtn.gameObject.SetActive(false);
                maxStoreLvBtn.SetActive(true);
            }
            else
            {
                if (idleSiteInfoLogic.PlayerCanPay(speedCost))
                {
                    //upgradeStoreBtn.interactable = true;
                    storeBtnGrayBg.SetActive(false);
                }
                else
                {
                    //upgradeStoreBtn.interactable = false;
                    storeBtnGrayBg.SetActive(true);
                }

                upgradeStoreBtn.gameObject.SetActive(true);
                maxStoreLvBtn.SetActive(false);
            }

            if (idleSiteInfoLogic.IsMaxVolume())
            {
                upgradeVolumeBtn.gameObject.SetActive(false);
                maxVolumeLvBtn.SetActive(true);
            }
            else
            {
                if (idleSiteInfoLogic.PlayerCanPay(volumeCost))
                {
                    //upgradeVolumeBtn.interactable = true;
                    volumeBtnGrayBg.SetActive(false);
                }
                else
                {
                    //upgradeVolumeBtn.interactable = false;
                    volumeBtnGrayBg.SetActive(true);
                }
                    
                upgradeVolumeBtn.gameObject.SetActive(true);
                maxVolumeLvBtn.SetActive(false);
            }
        }
        

        private void OnEnterFinish()
        {
            Debug.Log("OnEnterFinish");
        }

        private void OnExitFinish()
        {
            idleSiteInfoLogic.OnCloseBtnClick();
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
            idleSiteInfoLogic.InitData();
            Vector2 pos = idleSiteInfoLogic.GetCameraPos();
            float size = idleSiteInfoLogic.GetCameraSize();
            Vector3 targetPos = new Vector3(pos.x, pos.y, -1);
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, targetPos, size));
            InitData(null);
        }
    }
}

