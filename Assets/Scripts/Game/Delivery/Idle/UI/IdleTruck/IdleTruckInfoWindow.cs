using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using UnityEngine.Events;
using Foundation;

namespace Delivery.Idle
{
    public class IdleTruckInfoWindow : UIWindow
    {
        private IdleTruckInfoLogic idleTruckInfoLogic;
        #region UI控件
        private Transform upgradeNode;//升级相关的节点

        private MyText enableTruckMaxCount;//货车可用数
        private MyText upgradeCountCost;//升级数量花费
        private ConsecutiveButton upgradeCountBtn;//升级货车数量按钮
        private MyText volumeLv;//货车容量等级
        private MyText volume;//货车容量
        private MyText upgradeVolumeCost;//升级货车容量花费
        private ConsecutiveButton upgradeVolumeBtn;//升级货车容量按钮

        private GameObject maxCountLvBtn;
        private GameObject countBtnGrayBg;
        private GameObject maxVolumeLvBtn;
        private GameObject volumeBtnGrayBg;
        
        #endregion

        private IdleUIAnimation anim;

        private CameraAotoMove aotoMove;
        void Awake()
        {
            idleTruckInfoLogic = IdleTruckInfoLogic.Instance;

            #region  UI控件初始化
            
            upgradeNode = this.GetComponentByPath<Transform>("bg/content/truckUpgradeNode");

            enableTruckMaxCount = this.GetComponentByPath<MyText>("bg/content/truckUpgradeNode/upgradeCountbg/enableCount");
            upgradeCountBtn = this.GetComponentByPath<ConsecutiveButton>("bg/content/truckUpgradeNode/upgradeCountbg/btn-upgradeCount");
            maxCountLvBtn = this.GetComponentByPath<Transform>("bg/content/truckUpgradeNode/upgradeCountbg/btn-maxLv").gameObject;
            countBtnGrayBg = upgradeCountBtn.GetComponentByPath<Transform>("bg-gray").gameObject;
            upgradeCountCost = upgradeCountBtn.GetComponentByPath<MyText>("upgradeCountCost");


            volumeLv = this.GetComponentByPath<MyText>("bg/content/truckUpgradeNode/updradeVolumeBg/truckIcon/volumeLv");
            volume = this.GetComponentByPath<MyText>("bg/content/truckUpgradeNode/updradeVolumeBg/volume");
            upgradeVolumeBtn = this.GetComponentByPath<ConsecutiveButton>("bg/content/truckUpgradeNode/updradeVolumeBg/btn-upgradeVolume");
            maxVolumeLvBtn = this.GetComponentByPath<Transform>("bg/content/truckUpgradeNode/updradeVolumeBg/btn-maxLv").gameObject;
            volumeBtnGrayBg = upgradeVolumeBtn.GetComponentByPath<Transform>("bg-gray").gameObject;
            upgradeVolumeCost = upgradeVolumeBtn.GetComponentByPath<MyText>("upgradeVolumeCost");

     
            #endregion


            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, -200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);
            #region 添加事件监听
            aotoMove = GameObject.FindObjectOfType<CameraAotoMove>();
            AddAllUIListener();
            #endregion
            
        }

        private void OnExitFinish()
        {
            idleTruckInfoLogic.OnCloseBtnClick();
        }

        private void CameraMoveBack()
        {
            GameObject truckStation = GameObject.FindGameObjectWithTag("TruckSite");
            if (truckStation == null) return;
            Vector3 targetPos = truckStation.transform.position;
            targetPos.z = 1;
            //EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, targetPos, -1));
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3,float>(EnumEventType.Event_Camera_MoveBack,targetPos, -1));
        }

        private void CloseWindow(GameObject listener,object obj,params object[] objs)
        {
            //idleTruckInfoLogic.UpdateTruckCountData();
            if (aotoMove != null && aotoMove.IsMoving) return;
            //EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Camera_MoveBack));
            CameraMoveBack();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }

        private void CloseWindow(BaseEventArgs baseEventArgs)
        {
            if (!IsActive) return;
            //idleTruckInfoLogic.UpdateTruckCountData();
            //EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Camera_MoveBack));
            CameraMoveBack();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
            idleTruckInfoLogic.Destroy();
        }


        private void AddAllUIListener()
        {
            EventTriggerListener.Get(transform.Find("eventBg").gameObject).SetEventHandle(EnumTouchEventType.OnClick, CloseWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);

            upgradeCountBtn.AddClickListener(OnUpgradeTruckCount);

            upgradeVolumeBtn.AddClickListener(OnUpgradeTruckLv);
            
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
          
        }
        
        private void UpdateBtnState(BaseEventArgs baseEventArgs)
        {
            Long2 countCost = idleTruckInfoLogic.GetTruckCountLevelupCost();
            Long2 volumeCost = idleTruckInfoLogic.GetTruckVolumeLevelupCost();

            InitBtn(countCost, volumeCost);
            
        }

        private void OnUpgradeTruckCount()
        {
            idleTruckInfoLogic.OnUpgradeTruckCountBtnClick();
            InitUpgradeNode();
        }
        private void OnUpgradeTruckLv()
        {
            idleTruckInfoLogic.OnUpgradeTruckLvBtnClick();
            InitUpgradeNode();
        }

      
        /// <summary>
        /// 初始化升级相关控件数据
        /// </summary>
        private void InitUpgradeNode()
        {
            enableTruckMaxCount.text = idleTruckInfoLogic.GetTruckCount().ToString();
            Long2 countCost = idleTruckInfoLogic.GetTruckCountLevelupCost();
            upgradeCountCost.text = UnitConvertMgr.Instance.GetFloatValue(countCost, 2);
            volumeLv.text = "Lv." + idleTruckInfoLogic.GetTruckLv().ToString();
            volume.text = idleTruckInfoLogic.GetTruckVolume();
            Long2 volumeCost = idleTruckInfoLogic.GetTruckVolumeLevelupCost();
            upgradeVolumeCost.text = UnitConvertMgr.Instance.GetFloatValue(volumeCost, 2);

            InitBtn(countCost, volumeCost);
        }

        private void InitBtn(Long2 countCost,Long2 volumeCost)
        {
            if (idleTruckInfoLogic.IsMaxTruckCount())
            {
                upgradeCountBtn.gameObject.SetActive(false);
                maxCountLvBtn.SetActive(true);
            }
            else
            {
                //countMoneyIcon.SetActive(true);
                //upgradeCountCost.gameObject.SetActive(true);
                if (idleTruckInfoLogic.PlayerCanPay(countCost))
                {
                    //upgradeCountBtn.interactable = true;
                    countBtnGrayBg.SetActive(false);
                }
                else
                {
                    //upgradeCountBtn.interactable = false;
                    countBtnGrayBg.SetActive(true);
                }
                upgradeCountBtn.gameObject.SetActive(true);
                maxCountLvBtn.SetActive(false);
            }

            if (idleTruckInfoLogic.IsMaxTruckLv())
            {
                upgradeVolumeBtn.gameObject.SetActive(false);
                maxVolumeLvBtn.SetActive(true);
            }
            else
            {
                if (idleTruckInfoLogic.PlayerCanPay(volumeCost))
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
            idleTruckInfoLogic.InitData();
            InitUpgradeNode();

            Vector2 pos = idleTruckInfoLogic.GetCameraPos();
            float size = idleTruckInfoLogic.GetCameraSize();
            Vector3 targetPos = new Vector3(pos.x, pos.y, -1);
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, targetPos, size));
  
        }
    }
}


