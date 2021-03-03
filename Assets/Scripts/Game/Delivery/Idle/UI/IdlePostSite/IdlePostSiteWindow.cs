using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using Foundation;
using System;
using DG.Tweening;

namespace Delivery.Idle
{
    public class IdlePostSiteWindow : UIWindow
    {
        private IdlePostSiteLogic idlePostSiteLogic;
        #region UI控件

        private MyText siteName;//驿站名
        private MyText desc;//驿站描述
        private MyText income;//收益
        private MyText deliverSpeed;//配送速度
        private MyText rest;//货物堆积
        private MyText multiple;//收益倍数
        private Scrollbar lvScrollbar;//等级进度条
        private MyText lvMax;//等级最大值
        private MyText postSiteLv;//驿站等级
        
        private MyText upgradePostSiteCost;//升级驿站花费
        
        private MyText upgradeStaffCost;//升级员工花费
        private MyText deliveryTime;//配送时间
        private Image timeProcessBg;//配送时间等级进度

        private ConsecutiveButton upgradePostSiteBtn;//驿站升级按钮
        private ConsecutiveButton upgradeStaffBtn;//员工升级按钮

        private ConsecutiveButton upgradeSiteVolumeBtn;//驿站容量升级按钮
        private MyText upgradeSiteVolumeCost;//驿站容量升级花费
        private GameObject siteVolumeMoney;
        private Transform siteVolumeLvRoot;//驿站容量等级表现父节点

        private GameObject maxPostSiteLvBtn;
        private GameObject postSiteBtnGrayBg;
        private GameObject maxSiteVolumeLvBtn;
        private GameObject siteVoluemBtnGrayBg;
        private GameObject maxStaffLvBtn;
        private GameObject staffBtnGrayBg;

        private MyButton lastBtn;//上一个驿站按钮
        private MyButton nextBtn;//下一个驿站按钮
 

        #endregion
        private int currentSiteVolumeIndex;
        private int maxSiteVolumeLv = 10;
        private IdleUIAnimation anim;

        private CameraAotoMove aotoMove;
        Tweener incomeTweener1;//收益变化动画
        Tweener incomeTweener2;//收益变化动画

        float cameraSize = -1;
        private bool hasBtnClick = false;
        private void Awake()
        {
            idlePostSiteLogic = IdlePostSiteLogic.Instance;
           
            #region 初始化UI控件
            siteName = this.GetComponentByPath<MyText>("bg/title/MyText");
            desc = this.GetComponentByPath<MyText>("bg/descriptionBg/description");
            income = this.GetComponentByPath<MyText>("bg/content/incomeBg/income");
            deliverSpeed = this.GetComponentByPath<MyText>("bg/content/deliverSpeedBg/deliverSpeed");
            rest = this.GetComponentByPath<MyText>("bg/content/restBg/rest");
            multiple = this.GetComponentByPath<MyText>("bg/content/updagradePostSiteBg/multiple");
            lvScrollbar = this.GetComponentByPath<Scrollbar>("bg/content/updagradePostSiteBg/postSiteLvScrollBar");
            lvMax = lvScrollbar.GetComponentByPath<MyText>("lvMax");
            postSiteLv = lvScrollbar.GetComponentByPath<MyText>("postSiteLv");


            

            upgradePostSiteBtn = this.GetComponentByPath<ConsecutiveButton>("bg/content/updagradePostSiteBg/btn-upgradePostSite");
            upgradePostSiteCost = upgradePostSiteBtn.GetComponentByPath<MyText>("cost");
            maxPostSiteLvBtn = this.GetComponentByPath<Transform>("bg/content/updagradePostSiteBg/btn-maxLv").gameObject;
            postSiteBtnGrayBg = upgradePostSiteBtn.GetComponentByPath<Transform>("bg-gray").gameObject;
            deliveryTime = this.GetComponentByPath<MyText>("bg/content/staffTrainingBg/timeBg/deliveryTime");
            timeProcessBg = this.GetComponentByPath<Image>("bg/content/staffTrainingBg/timeBg/process");

            upgradeStaffBtn = this.GetComponentByPath<ConsecutiveButton>("bg/content/staffTrainingBg/btn-upgradeStaff");
            upgradeStaffCost = upgradeStaffBtn.GetComponentByPath<MyText>("cost");
            maxStaffLvBtn = this.GetComponentByPath<Transform>("bg/content/staffTrainingBg/btn-maxLv").gameObject;
            staffBtnGrayBg = upgradeStaffBtn.GetComponentByPath<Transform>("bg-gray").gameObject;

            upgradeSiteVolumeBtn = this.GetComponentByPath<ConsecutiveButton>("bg/content/upgradeVolumeBg/btn-upgradeVolume");
            upgradeSiteVolumeCost = upgradeSiteVolumeBtn.GetComponentByPath<MyText>("cost");
            maxSiteVolumeLvBtn = this.GetComponentByPath<Transform>("bg/content/upgradeVolumeBg/btn-maxLv").gameObject;
            siteVoluemBtnGrayBg = upgradeSiteVolumeBtn.GetComponentByPath<Transform>("bg-gray").gameObject;
            siteVolumeMoney = upgradeSiteVolumeBtn.GetComponentByPath<Transform>("moneyIcon").gameObject;
            siteVolumeLvRoot = this.GetComponentByPath<Transform>("bg/content/upgradeVolumeBg/volumeRoot");
            currentSiteVolumeIndex = 0;

            lastBtn = this.GetComponentByPath<MyButton>("bg/btn-last");
            nextBtn = this.GetComponentByPath<MyButton>("bg/btn-next");

            #endregion

            //初始化UI动画
            RectTransform rectTrans = this.GetComponentByPath<RectTransform>("bg");
            Vector2 startPos = new Vector2(0, rectTrans.anchoredPosition .y- 200);
            anim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnExitFinish);

            aotoMove = GameObject.FindObjectOfType<CameraAotoMove>();
            AddUIListener();

        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_ChangeRest, UpdateRest);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Camera_SimpleMoveToTarget, OnCameraFocusMove);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Camera_SimpleMoveArrive, OnCameraFocusArrive);
        }

        private void AddUIListener()
        {
            EventTriggerListener.Get(transform.Find("eventBg").gameObject).SetEventHandle(EnumTouchEventType.OnClick, CloseWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_CloseCurrentWindow, CloseWindow);


            upgradePostSiteBtn.AddClickListener(OnUpgradePostSiteBtnClick);
            upgradeStaffBtn.AddClickListener(OnUpgradeStaff);
            upgradeSiteVolumeBtn.AddClickListener(OnUpgradeSiteVolume);

            lastBtn.onClick.AddListener(OnLastBtnClick);
            nextBtn.onClick.AddListener(OnNextBtnClick);

            EventDispatcher.Instance.AddListener(EnumEventType.Event_Idle_MoneyChange, UpdateBtnState);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_ChangeRest, UpdateRest);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Camera_SimpleMoveToTarget, OnCameraFocusMove);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Camera_SimpleMoveArrive, OnCameraFocusArrive);
        }

     

      
        private void OnExitFinish()
        {
            idlePostSiteLogic.OnCloseBtnClick();
        }

        private void CameraMoveBack()
        {
            Vector3 targetPos = idlePostSiteLogic.GetPostSitePos();
            targetPos.z = 1;
            //EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, targetPos, -1));
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3,float>(EnumEventType.Event_Camera_MoveBack, targetPos,cameraSize));
        }

        private void CloseWindow(GameObject listener, object obj, params object[] objs)
        {
            if (aotoMove != null && aotoMove.IsMoving) return;
            CameraMoveBack();
            //EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Camera_MoveBack));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }

        private void CloseWindow(BaseEventArgs baseEventArgs)
        {
            if (!IsActive) return;
            CameraMoveBack();
            //EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Camera_MoveBack));
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
            anim.Exit();
        }
       
        private void OnUpgradePostSiteBtnClick()
        {
            hasBtnClick = true;
            if (aotoMove.IsMoving)
            {
                upgradePostSiteBtn.ForbiddenBtn();
                return;
            }
            OnUpgradepPostSite();
            Long2 postSiteCost = idlePostSiteLogic.GetUpgradeSiteCost();
            Long2 staffCost = idlePostSiteLogic.GetUpgradeSiteTimeCost();
            //InitBtns(postSiteCost, staffCost);
        }
        //升级驿站按钮点击事件
        private void OnUpgradepPostSite()
        {
            if (!idlePostSiteLogic.OnUpgradeSiteBtnClick()) return;

            deliverSpeed.text =UnitConvertMgr.Instance.GetFloatValue(idlePostSiteLogic.GetDeliverSpeed(),2) + "/分钟";
            income.text = idlePostSiteLogic.GetIncome() + "/分钟";
            PlayIncomeAni();


            multiple.text = "x" + idlePostSiteLogic.GetMultiple().ToString();
            IdleSiteGrade siteGrade = idlePostSiteLogic.GetIdleSiteGrade();
            int siteLvMin = siteGrade.lvmin;
            int siteLvMax = siteGrade.lvmax + 1;
            int siteLv = idlePostSiteLogic.GetSiteLv();
            lvMax.text = "Lv." + siteLvMax.ToString();
            postSiteLv.text = "Lv." + siteLv;
            lvScrollbar.size = (float)(siteLv - siteLvMin) / (siteLvMax - siteLvMin);
            upgradePostSiteCost.text = UnitConvertMgr.Instance.GetFloatValue(idlePostSiteLogic.GetUpgradeSiteCost(),2);
        }
      
        private void PlayIncomeAni()
        {
            if (incomeTweener1 != null && incomeTweener1.IsPlaying()) return;
            if (incomeTweener2 != null && incomeTweener2.IsPlaying()) return;
            RectTransform incomeTrans = income.GetComponent<RectTransform>();
            Color originalColor = income.color;
            Color tempColor = originalColor;
            ColorUtility.TryParseHtmlString("#5100FF", out tempColor);
            income.color = tempColor;
            incomeTweener1 = incomeTrans.DOScale(1.2f, 0.2f).OnComplete(() =>
            {
                incomeTweener2= incomeTrans.DOScale(1.0f, 0.1f);
                income.color = originalColor;
            });
               
        }
        

        /// <summary>
        /// 更新堆积货物数量
        /// </summary>
        private void UpdateRest(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> argsOne = (EventArgsOne<int>)baseEventArgs;
            int id = argsOne.param1;
            if (id != idlePostSiteLogic.GetSiteId()) return;
            rest.text = $"{idlePostSiteLogic.GetRest()}/{idlePostSiteLogic.GetVolume()}";
        }
        //升级员工按钮点击事件
        private void OnUpgradeStaff()
        {
            if (!idlePostSiteLogic.OnUpgradeStaffBtnClick()) return;

            deliverSpeed.text = UnitConvertMgr.Instance.GetFloatValue(idlePostSiteLogic.GetDeliverSpeed(),2) + "/分钟";
            income.text = idlePostSiteLogic.GetIncome() + "/分钟";
            PlayIncomeAni();
            deliveryTime.text = idlePostSiteLogic.GetDeliverTime().ToString("0.#") + "s";
            timeProcessBg.fillAmount = idlePostSiteLogic.GetDeliverTimeLvProcess();
            Long2 staffCost = idlePostSiteLogic.GetUpgradeSiteTimeCost();
            upgradeStaffCost.text = UnitConvertMgr.Instance.GetFloatValue(staffCost, 2);

            Long2 postSiteCost = idlePostSiteLogic.GetUpgradeSiteCost();
            //InitBtns(postSiteCost, staffCost);

        }

        //升级驿站容量按钮点击事件
        private void OnUpgradeSiteVolume()
        {
            if (!idlePostSiteLogic.OnUpgradeSiteVolumeBtnClick()) return;
            upgradeSiteVolumeCost.text = UnitConvertMgr.Instance.GetFloatValue(idlePostSiteLogic.GetUpgradeSiteVolumeCost(), 2);
            currentSiteVolumeIndex++;
            rest.text = $"{idlePostSiteLogic.GetRest()}/{idlePostSiteLogic.GetVolume()}";
            if (currentSiteVolumeIndex<maxSiteVolumeLv)
            {
                string childname = "volume" + currentSiteVolumeIndex.ToString();
                Transform volumeChild = siteVolumeLvRoot.Find(childname);
                volumeChild.Find("active").gameObject.SetActive(true);
                volumeChild.Find("disable").gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData(IdleSiteModel siteModel)
        {
          
            idlePostSiteLogic.Init(siteModel);

            Vector2 pos = idlePostSiteLogic.GetCameraPos();
            float size = idlePostSiteLogic.GetCameraSize();
            Vector3 targetPos = new Vector3(pos.x, pos.y, -1);
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, targetPos, size));

            siteName.text = idlePostSiteLogic.GetPostSiteName();
            desc.text = idlePostSiteLogic.GetPostSiteDesc();
            deliverSpeed.text = UnitConvertMgr.Instance.GetFloatValue(idlePostSiteLogic.GetDeliverSpeed(),2) + "/分钟";
            income.text = idlePostSiteLogic.GetIncome() + "/分钟";
            rest.text = $"{idlePostSiteLogic.GetRest()}/{idlePostSiteLogic.GetVolume()}";
            multiple.text = "x" + idlePostSiteLogic.GetMultiple().ToString();
            IdleSiteGrade siteGrade = idlePostSiteLogic.GetIdleSiteGrade();
            int siteLvMin = siteGrade.lvmin;
            int siteLvMax = siteGrade.lvmax + 1;
            int siteLv = idlePostSiteLogic.GetSiteLv();
            lvMax.text = "Lv." + siteLvMax.ToString();
            postSiteLv.text = "Lv." + siteLv;
            lvScrollbar.size = (float)(siteLv - siteLvMin) / (siteLvMax - siteLvMin);

            Long2 postSiteCost = idlePostSiteLogic.GetUpgradeSiteCost();
            Long2 staffCost = idlePostSiteLogic.GetUpgradeSiteTimeCost();
            upgradePostSiteCost.text = UnitConvertMgr.Instance.GetFloatValue(postSiteCost, 2);
            upgradeStaffCost.text = UnitConvertMgr.Instance.GetFloatValue(staffCost, 2);

            Long2 siteVolumeCost = idlePostSiteLogic.GetUpgradeSiteVolumeCost();
            upgradeSiteVolumeCost.text = UnitConvertMgr.Instance.GetFloatValue(siteVolumeCost, 2);

            deliveryTime.text = idlePostSiteLogic.GetDeliverTime().ToString("0.#") + "s";
            timeProcessBg.fillAmount = idlePostSiteLogic.GetDeliverTimeLvProcess();

            int siteVolumeLv = idlePostSiteLogic.GetSiteVolumeLv();
            for(int i=0;i<maxSiteVolumeLv;i++)
            {
                string childname = "volume" + i.ToString();
                Transform volumeChild = siteVolumeLvRoot.Find(childname);
                if (i < siteVolumeLv)
                {
                    volumeChild.Find("active").gameObject.SetActive(true);
                    volumeChild.Find("disable").gameObject.SetActive(false);
                    currentSiteVolumeIndex = i;
                }
                else
                {
                    volumeChild.Find("active").gameObject.SetActive(false);
                    volumeChild.Find("disable").gameObject.SetActive(true);
                }
                
            }

            int lastSiteId = idlePostSiteLogic.GetLastPostSiteId();
            int nextSiteId = idlePostSiteLogic.GetNextPostSiteId();
            if (lastSiteId < 0)
            {
                lastBtn.gameObject.SetActive(false);
            }
            else
            {
                lastBtn.gameObject.SetActive(true);
            }
            if (nextSiteId < 0)
            {
                nextBtn.gameObject.SetActive(false);
            }
            else
            {
                nextBtn.gameObject.SetActive(true);
            }

            InitBtns(postSiteCost, staffCost, siteVolumeCost);

        }

        private void UpdateBtnState(BaseEventArgs baseEventArgs)
        {
            Long2 postSiteCost = idlePostSiteLogic.GetUpgradeSiteCost();
            Long2 staffCost = idlePostSiteLogic.GetUpgradeSiteTimeCost();
            Long2 volumeCost = idlePostSiteLogic.GetUpgradeSiteVolumeCost();
            InitBtns(postSiteCost, staffCost, volumeCost);
        }
        private void InitBtns(Long2 postSiteCost,Long2 staffCost,Long2 volumeCost)
        {
            UpdatePostSiteLvBtn(postSiteCost);
            UpdateStaffLvBtn(staffCost);
            UpdatePostsiteVolumeLvBtn(volumeCost);
        }
        //升级时移动镜头调用
        private void UpdatePostSiteLvBtn(BaseEventArgs baseEventArgs)
        {
            if (idlePostSiteLogic.IsMaxPostSiteLv())
            {
                upgradePostSiteBtn.gameObject.SetActive(false);
                maxPostSiteLvBtn.SetActive(true);
            }
            else
            {
                if (!aotoMove.IsMoving)
                {
                    postSiteBtnGrayBg.SetActive(false);
                }
                else
                {
                    postSiteBtnGrayBg.SetActive(true);
                }
                upgradePostSiteBtn.gameObject.SetActive(true);
                maxPostSiteLvBtn.SetActive(false);
            }
        }

        private void OnCameraFocusMove(BaseEventArgs baseEventArgs)
        {
            postSiteBtnGrayBg.SetActive(true);
        }
        private void OnCameraFocusArrive(BaseEventArgs baseEventArgs)
        {
            postSiteBtnGrayBg.SetActive(false);
        }

        //更新驿站升级按钮状态
        private void UpdatePostSiteLvBtn(Long2 postsiteCost)
        {
            if (idlePostSiteLogic.IsMaxPostSiteLv())
            {
                upgradePostSiteBtn.gameObject.SetActive(false);
                maxPostSiteLvBtn.SetActive(true);
            }
            else
            {
                
                if (idlePostSiteLogic.PlayerCanPay(postsiteCost))
                {
                    postSiteBtnGrayBg.SetActive(false);
                }
                else
                {
                    postSiteBtnGrayBg.SetActive(true);
                }
                //升级时镜头移动
                if(hasBtnClick&&aotoMove.IsMoving)
                    postSiteBtnGrayBg.SetActive(true);

                upgradePostSiteBtn.gameObject.SetActive(true);
                maxPostSiteLvBtn.SetActive(false);
            }
        }
        //更新快递员升级按钮
        private void UpdateStaffLvBtn(Long2 staffCost)
        {
            if (idlePostSiteLogic.IsMaxStaffLv())
            {
                upgradeStaffBtn.gameObject.SetActive(false);
                maxStaffLvBtn.SetActive(true);
            }
            else
            {
               
                if (idlePostSiteLogic.PlayerCanPay(staffCost))
                {
                    //upgradeStaffBtn.interactable = true;
                    staffBtnGrayBg.SetActive(false);
                }
                else
                {
                    //upgradeStaffBtn.interactable = false;
                    staffBtnGrayBg.SetActive(true);
                }
                upgradeStaffBtn.gameObject.SetActive(true);
                maxStaffLvBtn.SetActive(false);
            }
        }

        //更新驿站容量升级按钮
        private void UpdatePostsiteVolumeLvBtn(Long2 volumeCost)
        {
            if(idlePostSiteLogic.IsMaxSiteVolumeLv())
            {
                upgradeSiteVolumeBtn.gameObject.SetActive(false);
                maxSiteVolumeLvBtn.SetActive(true);
            }
            else
            {
               
                if (idlePostSiteLogic.PlayerCanPay(volumeCost))
                {
                    //upgradeSiteVolumeBtn.interactable = true;
                    siteVoluemBtnGrayBg.SetActive(false);
                }
                else
                {
                    //upgradeSiteVolumeBtn.interactable = false;
                    siteVoluemBtnGrayBg.SetActive(true);
                }
                upgradeSiteVolumeBtn.gameObject.SetActive(true);
                maxSiteVolumeLvBtn.SetActive(false);
            }

        }


        private void OnLastBtnClick()
        {

            if (aotoMove.IsMoving) return;
            hasBtnClick = false;
            int lastSiteId = idlePostSiteLogic.GetLastPostSiteId();
            IdleSiteModel siteModel = IdleSiteCtrl.Instance.GetIdleSiteModelById(lastSiteId);
            if (siteModel == null) return;
            InitData(siteModel);
        }

        private void OnNextBtnClick()
        {
            if (aotoMove.IsMoving) return;
            hasBtnClick = false;
            int nextSiteId = idlePostSiteLogic.GetNextPostSiteId();
            IdleSiteModel siteModel = IdleSiteCtrl.Instance.GetIdleSiteModelById(nextSiteId);
            if (siteModel == null) return;
            InitData(siteModel);
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
            cameraSize = Camera.main.orthographicSize;
        }
    }
}


