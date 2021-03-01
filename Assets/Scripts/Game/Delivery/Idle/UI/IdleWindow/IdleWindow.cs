using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
using Foundation;
using DG.Tweening;

namespace Delivery.Idle
{
    public class IdleWindow : UIPage
    {
        private IdleWindowLogic idleWindowCtrl;

        public bool isActive;//是否处于显示状态

        #region UI控件
        private MyButton statisticsBtn;//统计按钮
        private MyButton mapBtn;//地图按钮

        private MyButton buildBtn;//建造按钮
        private GameObject buildNode;//建造按钮标志点
        private GameObject finger;//建造按钮手指提示

        private MyButton taskBtn;//任务按钮
        private MyButton eventBtn;//特殊事件按钮
        private GameObject taskNode;//任务完成标志点
        private GameObject taskCompleteImage;//任务完成按钮图
        private MyButton investorBtn;//投资人按钮
        private MyButton doubleIncomeBtn;//双倍收益按钮
        private GameObject doubleIncomeRestTimeBg;//双倍收益剩余时间背景
        private MyText doubleIncomeRestTimeText;//双倍收益剩余时间
        private GameObject doubleIncomeTip;//双倍收益提示动画
        private MyButton doubleIncomeAniBtn;//双倍收益spine动画按钮

        private GameObject mainTaskNode;//主线任务节点
        private Slider mainTaskSlider;//主线任务进度条
        private MyText taskContent;//主线任务描述
        private MyButton receiveBtn;//领取任务奖励按钮

        

        private GameObject specialEvenRestTimeBg;//特殊事件剩余时间背景
        private MyText specialEventRestTimeText;//特殊事件剩余时间

        

        private MyButton followPlaneBtn;//跟随飞机按钮
        private MyText planeRestTimeText;//飞机持续时间倒计时
        private Transform planeTrans;
        private bool inPlaneCountDown = false;//是否处于空投倒计时
        private int planeRestTime = 0;//空投剩余时间
        private TimerEvent planeTimer;

        //三个提示按钮父节点
        private GameObject levelupPostSiteBtnNode;
        private GameObject levelupSiteBtnNode;
        private GameObject levelupTruckBtnNode;
        //三个提示按钮
        private MyButton levelupPostSiteTipBtn;
        private MyButton levelupSiteTipBtn;
        private MyButton levelupTruckTipBtn;
        //三个提示按钮动画
        private Animation levelupPostSiteBtnAnim;
        private Animation levelupSiteTipBtnAnim;
        private Animation levelupTruckBtnAnim;

        private Animation actionTipAnim;
        private Animation buildBtnAnim;//建造按钮动画

        #endregion
        
        private IdleUIAnimation mainMenuAnim;//主菜单功能键动画
        private IdleUIAnimation bottomLeftAnim;//左下角按钮动画
        private IdleUIAnimation eventBtnAnim;//特殊事件按钮动画
        private IdleUIAnimation specialEventTimerAnim;//特殊事件计时器动画
        private IdleUIAnimation investorAnim;//投资人按钮动画
        private IdleUIAnimation postSiteActionTipAnim;//升级驿站操作提示动画
        private IdleUIAnimation siteActionTipAnim;//升级快递中心提示动画
        private IdleUIAnimation truckActionTipAnim;//升级车站操作提示动画

        private IdleUIAnimation mainTaskBtnAnim;//主线任务按钮动画
        private IdleUIAnimation followPlaneBtnAnim;//飞机跟随按钮动画


        private void Awake()
        {
            idleWindowCtrl = IdleWindowLogic.Instance;
            
            statisticsBtn = this.GetComponentByPath<MyButton>("menuBtns/btn-statistics");
            mapBtn = this.GetComponentByPath<MyButton>("menuBtns/btn-map");
            buildBtn = this.GetComponentByPath<MyButton>("menuBtns/btn-build");
            buildNode = buildBtn.GetComponentByPath<Transform>("node").gameObject;
            finger = buildBtn.GetComponentByPath<Transform>("finger").gameObject;
            buildNode.SetActive(false);
            buildBtnAnim = buildBtn.GetComponent<Animation>();
            InitBuildBtnNode();

            taskBtn = this.GetComponentByPath<MyButton>("menuBtns/btn-task");
            eventBtn = this.GetComponentByPath<MyButton>("btn-event");
            taskNode = taskBtn.GetComponentByPath<Transform>("node").gameObject;
            taskCompleteImage = taskBtn.GetComponentByPath<Transform>("complete").gameObject;
            investorBtn = this.GetComponentByPath<MyButton>("btn-investor");


            doubleIncomeBtn = this.GetComponentByPath<MyButton>("bottomLeftBtns/btn-doubleIncome");
            doubleIncomeRestTimeBg = doubleIncomeBtn.GetComponentByPath<Transform>("timeCountDownBg").gameObject;
            doubleIncomeRestTimeText = doubleIncomeRestTimeBg.GetComponentByPath<MyText>("restTime");
            doubleIncomeTip = this.GetComponentByPath<Transform>("bottomLeftBtns/doubleIncomeTip").gameObject;
            doubleIncomeAniBtn = doubleIncomeTip.GetComponentByPath<MyButton>("spineAniBtn");

            specialEvenRestTimeBg = this.GetComponentByPath<Transform>("timer-specialEvent").gameObject;
            specialEventRestTimeText = specialEvenRestTimeBg.GetComponentByPath<MyText>("restTime");

            levelupPostSiteBtnNode = this.GetComponentByPath<Transform>("midLeftBtns/levelupPostSite").gameObject;
            levelupPostSiteTipBtn = levelupPostSiteBtnNode.GetComponentByPath<MyButton>("btn-levelupPostSite");
            levelupPostSiteBtnAnim = levelupPostSiteTipBtn.GetComponent<Animation>();
            levelupSiteBtnNode = this.GetComponentByPath<Transform>("midLeftBtns/levelupSite").gameObject;
            levelupSiteTipBtn = levelupSiteBtnNode.GetComponentByPath<MyButton>("btn-levelupSite");
            levelupSiteTipBtnAnim = levelupSiteTipBtn.GetComponent<Animation>();
            levelupTruckBtnNode = this.GetComponentByPath<Transform>("midLeftBtns/levelupTruck").gameObject;
            levelupTruckTipBtn = levelupTruckBtnNode.GetComponentByPath<MyButton>("btn-levelupTruck");
            levelupTruckBtnAnim = levelupTruckTipBtn.GetComponent<Animation>();


            followPlaneBtn = this.GetComponentByPath<MyButton>("btn-followPlane");
            planeRestTimeText = followPlaneBtn.GetComponentByPath<MyText>("restTime");

            actionTipAnim = this.GetComponentByPath<Animation>("midLeftBtns");

            mainTaskNode = this.GetComponentByPath<Transform>("mainTaskNode").gameObject;
            mainTaskSlider = mainTaskNode.GetComponentByPath<Slider>("mainTaskSlider");
            taskContent = mainTaskSlider.GetComponentByPath<MyText>("taskContent");
            receiveBtn = mainTaskNode.GetComponentByPath<MyButton>("btn-receive");


            
            //主菜单按钮动画初始化
            RectTransform mainMenuRectTrans = this.GetComponentByPath<RectTransform>("menuBtns");
            mainMenuAnim = new IdleUIAnimation(mainMenuRectTrans, UIAnimationDir.Top, () => { isActive = true; }, () => { isActive = false; });

            //左下角按钮动画初始化
            RectTransform bottomLeftRectTrans = this.GetComponentByPath<RectTransform>("bottomLeftBtns");
            bottomLeftAnim = new IdleUIAnimation(bottomLeftRectTrans, UIAnimationDir.Left);
            
            //特殊事件按钮动画初始化
            RectTransform eventRectTrans = eventBtn.GetComponent<RectTransform>();
            eventBtnAnim = new IdleUIAnimation(eventRectTrans, UIAnimationDir.Right);
            eventBtnAnim.Exit();
            eventBtn.gameObject.SetActive(false);

            RectTransform specialEventTimerRect = specialEvenRestTimeBg.GetComponent<RectTransform>();
            specialEventTimerAnim = new IdleUIAnimation(specialEventTimerRect, UIAnimationDir.Left);
            specialEvenRestTimeBg.SetActive(false);

            //特殊事件按钮动画初始化
            RectTransform investorBtnRect = investorBtn.GetComponent<RectTransform>();
            investorAnim = new IdleUIAnimation(investorBtnRect, UIAnimationDir.Right);
            investorAnim.Exit();
            investorBtn.gameObject.SetActive(false);

            
            //升级驿站按钮动画初始化
            RectTransform postSiteActionTipRect = levelupPostSiteTipBtn.GetComponent<RectTransform>();
            postSiteActionTipAnim = new IdleUIAnimation(postSiteActionTipRect, UIAnimationDir.Left, OnActionTipBtnFinishEnter);

            RectTransform siteActionTipRect = levelupSiteTipBtn.GetComponent<RectTransform>();
            siteActionTipAnim = new IdleUIAnimation(siteActionTipRect, UIAnimationDir.Left, OnActionTipBtnFinishEnter);

            RectTransform truckActionTipRect = levelupTruckTipBtn.GetComponent<RectTransform>();
            truckActionTipAnim = new IdleUIAnimation(truckActionTipRect, UIAnimationDir.Left, OnActionTipBtnFinishEnter);
            //主线任务按钮动画
            RectTransform mainTaskRect = mainTaskNode.GetComponent<RectTransform>();
            mainTaskBtnAnim = new IdleUIAnimation(mainTaskRect, UIAnimationDir.Left);

            RectTransform followPlaneRest = followPlaneBtn.GetComponent<RectTransform>();
            followPlaneBtnAnim = new IdleUIAnimation(followPlaneRest, UIAnimationDir.Left);
            followPlaneBtnAnim.Exit();
            followPlaneBtn.gameObject.SetActive(false);

            AddAllUIListener();
        }
        

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_IdleWindow_ShowMenuBtns, ShowOrHideMenuBtns);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_IdleWindow_ForceHideMenuBtns, HideMenuBtnsImmediately);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_ShowPostSite, OpenPostSiteWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_ShowSite, OpenSiteWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_ShowTruck, OpenTruckSiteWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_ShowBuild, OpenBuildWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Window_ShowMap, OpenMapWindow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_TaskBtn_ShowNode, ShowTaskBtnNode);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Task_MainTaskComplete, ShowReceiveRewardBtn);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_Prepare, ShowEventBtn);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_End, OnSpecialEventEnd);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_Update, OnSpecialEventTimerUpdate);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Investor_Prepare, ShowInvestorBtn);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_DoubleIncome_Update, UpdateDoubleIncomeBtn);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_ActionTip_Trigger, OnAnyActionTipStart);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Site_CanUnlock, OnCanUnlockSite);

            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Feiji_Move, SetPlaneTrans);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Feiji_MoveComplete, OnPlaneFinishMove);

            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_StartFirstStep, OnGuideStart);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_GuideComplete, OnGuideEnd);
        }
        private void Start()
        {
            RectTransform rectTransform = this.GetComponent<RectTransform>();
            rectTransform.offsetMax = new Vector2(0, ((Screen.safeArea.height + Screen.safeArea.y) - Screen.height) * FixScreen.height / Screen.height);
        }

        private void OnCanUnlockSite(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<bool> arg = (EventArgsOne<bool>)baseEventArgs;
            if (arg.param1)
                PlayBuildBtnAnim();
            else
                StopBuildBtnAnim();
        }

        private void InitBuildBtnNode()
        {
            bool playAnim = IdleSiteCtrl.Instance.AnySiteCanLock;
            if (playAnim)
                PlayBuildBtnAnim();
            else
            {
                finger.SetActive(false);
            }
        }

        private void PlayBuildBtnAnim()
        {
            if (!buildBtnAnim.isPlaying)
            {
                buildNode.SetActive(true);
                buildBtnAnim.Play();

                finger.SetActive(!IdleGuideCtrl.Instance.IsGuiding);
            }
        }

        private void StopBuildBtnAnim()
        {
            if (buildBtnAnim.isPlaying)
            {
                buildNode.SetActive(false);
                buildBtnAnim.Stop();
                Image buildBtnImage = buildBtn.GetComponent<Image>();
                Color buildImageColor = buildBtnImage.color;
                buildImageColor.a = 1.0f;
                buildBtnImage.color = buildImageColor;
                finger.SetActive(false);
            }
        }

        private void OnGuideStart(BaseEventArgs baseEventArgs)
        {
            
            finger.gameObject.SetActive(false);

        }

        private void OnGuideEnd(BaseEventArgs baseEventArgs)
        {

            finger.gameObject.SetActive(buildBtnAnim.isPlaying);
        }

        #region 主线任务按钮相关
        
        private void ShowReceiveRewardBtn(BaseEventArgs baseEventArgs)
        {
            mainTaskSlider.gameObject.SetActive(false);
            receiveBtn.gameObject.SetActive(true);
        }

        private void InitMainTaskSlider()
        {
            PlayerTask playerTask = PlayerMgr.Instance.GetCurrentMainTask();
            if(playerTask==null)
            {
                mainTaskNode.gameObject.SetActive(false);
                return;
            }
            if(playerTask.IsFinish)
            {
                //切换到领取状态
                mainTaskSlider.gameObject.SetActive(false);
                receiveBtn.gameObject.SetActive(true);
            }
            else
            {
                receiveBtn.gameObject.SetActive(false);
                mainTaskSlider.gameObject.SetActive(true);
                IdleMainTask task = IdleTaskMgr.Instance.GetIdleMainTask(playerTask.Id);
                TaskProcess taskProcess = IdleTaskCtrl.Instance.GetTaskProcess(task);
                taskContent.text = task.Tips;
                mainTaskSlider.value = (float)taskProcess.currentProcess / taskProcess.maxProcess;
            }

        }

        private void OnMainTaskSliderClick(GameObject listener, object obj, params object[] objs)
        {
            PlayerTask playerTask = PlayerMgr.Instance.GetCurrentMainTask();
            if (playerTask == null) return;
            IdleMainTask idleTask = IdleTaskMgr.Instance.GetIdleMainTask(playerTask.Id);
            if (idleTask == null) return;
            IdleTaskCtrl.Instance.GotoOtherWindow(idleTask);
        }


        private void OnReceiveBtnClick()
        {
            PlayerTask playerTask = PlayerMgr.Instance.GetCurrentMainTask();
            if (playerTask.IsFinish)
            {
                IdleMainTask idleTask = IdleTaskMgr.Instance.GetIdleMainTask(playerTask.Id);
                Vector3 receiveBtnPos = receiveBtn.GetComponent<RectTransform>().position;
                PlayerMgr.Instance.PlayerCity.playerMainTasks.Remove(playerTask);
                IdleFxHelper.PlayGetRewardFxUI(receiveBtnPos, () =>
                {
                    if (idleTask.RewardType == 1)
                    {
                        PlayerMgr.Instance.AddMoney(new Long2(idleTask.Reward));
                    }
                },true);
                mainTaskBtnAnim.SetExitFinishEvent(() => {
                    IdleTaskCtrl.Instance.InitTaskProcess();
                    InitMainTaskSlider();
                    mainTaskBtnAnim.Enter();
                    mainTaskBtnAnim.SetExitFinishEvent(null);
                });
                mainTaskBtnAnim.Exit();
            }
        }

        #endregion





        /// <summary>
        /// 添加UI事件监听
        /// </summary>
        private void AddAllUIListener()
        {
            statisticsBtn.onClick.AddListener(idleWindowCtrl.OnStatisticsBtnClick);
            mapBtn.onClick.AddListener(idleWindowCtrl.OnMapBtnClick);
            buildBtn.onClick.AddListener(idleWindowCtrl.OnBuildBtnClick);
            taskBtn.onClick.AddListener(idleWindowCtrl.OnTaskBtnClick);
            eventBtn.onClick.AddListener(idleWindowCtrl.OnEventBtnClick);
            investorBtn.onClick.AddListener(idleWindowCtrl.OnInvestorBtnClick);
            doubleIncomeBtn.onClick.AddListener(idleWindowCtrl.OnDoubleIncomeBtnClick);
            doubleIncomeAniBtn.onClick.AddListener(idleWindowCtrl.OnDoubleIncomeBtnClick);
            levelupPostSiteTipBtn.onClick.AddListener(OnPostSiteActionTipClick);
            levelupSiteTipBtn.onClick.AddListener(OnSiteActionTipClick);
            levelupTruckTipBtn.onClick.AddListener(OnTruckActionTipClick);

            receiveBtn.onClick.AddListener(OnReceiveBtnClick);
            EventTriggerListener.Get(mainTaskSlider.gameObject).SetEventHandle(EnumTouchEventType.OnClick, OnMainTaskSliderClick);

            followPlaneBtn.onClick.AddListener(OnFollowPlaneBtnClick);

            EventDispatcher.Instance.AddListener(EnumEventType.Event_IdleWindow_ShowMenuBtns, ShowOrHideMenuBtns);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_IdleWindow_ForceHideMenuBtns, HideMenuBtnsImmediately);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_ShowPostSite, OpenPostSiteWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_ShowSite, OpenSiteWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_ShowTruck, OpenTruckSiteWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_ShowBuild, OpenBuildWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Window_ShowMap, OpenMapWindow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_TaskBtn_ShowNode, ShowTaskBtnNode);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Task_MainTaskComplete, ShowReceiveRewardBtn);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_Prepare, ShowEventBtn);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_Update, OnSpecialEventTimerUpdate);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_End, OnSpecialEventEnd);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Investor_Prepare, ShowInvestorBtn);

            EventDispatcher.Instance.AddListener(EnumEventType.Event_DoubleIncome_Update, UpdateDoubleIncomeBtn);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_ActionTip_Trigger, OnAnyActionTipStart);

            EventDispatcher.Instance.AddListener(EnumEventType.Event_Site_CanUnlock, OnCanUnlockSite);

            EventDispatcher.Instance.AddListener(EnumEventType.Event_Feiji_Move, SetPlaneTrans);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Feiji_MoveComplete, OnPlaneFinishMove);

            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_StartFirstStep, OnGuideStart);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_GuideComplete, OnGuideEnd);

           
        }

        #region 空投倒计时按钮

        private void SetPlaneTrans(BaseEventArgs baseEventArgs)
        {
            EventArgsTwo<Transform,float> args = baseEventArgs as EventArgsTwo<Transform,float>;
            planeTrans = args.param1;
            planeRestTime = (int)args.param2;
            StartPlaneCountDown();
        }

        private void OnPlaneFinishMove(BaseEventArgs baseEventArgs)
        {
            EndPlaneMove();
        }
      
        private void StartPlaneCountDown()
        {
            if (inPlaneCountDown) return;
            inPlaneCountDown = true;
            planeRestTimeText.text = TimeUtils.secondsToString(planeRestTime);
            if(IsActive)
            {
                followPlaneBtn.gameObject.SetActive(true);
                followPlaneBtnAnim.Enter();
            }
            planeTimer = Timer.Instance.Register(1, planeRestTime, UpdatePlaneRestTime);
        }

        private void EndPlaneMove()
        {
            planeRestTime = 0;
            inPlaneCountDown = false;
            if(planeTimer!=null)
            {
                Timer.Instance.DestroyTimer(planeTimer);
                planeTimer = null;
            }
            if (followPlaneBtnAnim.isActive)
            {
                followPlaneBtnAnim.Exit();
            }
        }

        private void UpdatePlaneRestTime(params object[] objs)
        {
            planeRestTime--;
            if(planeRestTime<=0)
            {
                //planeRestTime = 0;
                //inPlaneCountDown = false;
                //if(followPlaneBtnAnim.isActive)
                //{
                //    followPlaneBtnAnim.Exit();
                //}
                EndPlaneMove();
            }
            planeRestTimeText.text = TimeUtils.secondsToString(planeRestTime);
        }

        private void OnFollowPlaneBtnClick()
        {
            if (planeTrans == null) return;
            Vector3 targetPos = planeTrans.position;
            targetPos.z = 1;
           
            EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, targetPos, -1));
        }

        #endregion

        public void OnPostSiteActionTipClick()
        {
            ShowOrHidePostSiteActonTip(false);
            IdleActionTipCtrl.Instance.OpenTargetWindow(1);
        }

        public void OnSiteActionTipClick()
        {
            ShowOrHideSiteActonTip(false);
            IdleActionTipCtrl.Instance.OpenTargetWindow(2);
        }

        public void OnTruckActionTipClick()
        {
            ShowOrHideTruckActonTip(false);
            IdleActionTipCtrl.Instance.OpenTargetWindow(3);
        }

        //显示任务按钮完成的标志点
        private void ShowTaskBtnNode(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<bool> argsOne = (EventArgsOne<bool>)baseEventArgs;
            taskNode.SetActive(argsOne.param1);
            taskCompleteImage.SetActive(argsOne.param1);
        }
        //打开驿站窗口
        private void OpenPostSiteWindow(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSiteModel> argsOne = (EventArgsOne<IdleSiteModel>)baseEventArgs;
            IdleSiteModel sitemodel = argsOne.param1;
            PlayerSite site = PlayerMgr.Instance.GetPlayerSite(sitemodel.SiteId);
            if (site.isLock)
            {
                OpenBuildWindow(null);
                return;
            }
            idleWindowCtrl.OnPostBtnClick(baseEventArgs);
        }
        //打开站点窗口
        private void OpenSiteWindow(BaseEventArgs baseEventArgs)
        {
            idleWindowCtrl.OnSiteBtnClick();
        }
        //打开车站窗口
        private void OpenTruckSiteWindow(BaseEventArgs baseEventArgs)
        {
            idleWindowCtrl.OnTruckStationBtnClick();
        }
        //打开建造窗口
        private void OpenBuildWindow(BaseEventArgs baseEventArgs)
        {
            idleWindowCtrl.OnBuildBtnClick();
        }
        //打开地图窗口
        private void OpenMapWindow(BaseEventArgs baseEventArgs)
        {
            idleWindowCtrl.OnMapBtnClick();
        }
        //显示特殊事件按钮
        private void ShowEventBtn(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleSpecialEvent> args = baseEventArgs as EventArgsOne<IdleSpecialEvent>;
            eventBtn.GetComponent<Image>().sprite = AssetHelper.GetSpecialEventBtnSprite(args.param1.btnRes);
            if (CurrentWindow == null)
            {
                eventBtn.gameObject.SetActive(true);
                eventBtnAnim.Enter();
            }
        }
        //显示投资人按钮
        private void ShowInvestorBtn(BaseEventArgs baseEventArgs)
        {
            if (CurrentWindow == null)
            {
                investorBtn.gameObject.SetActive(true);
                investorAnim.Enter();
            }
        }

        //更新双倍收益按钮
        private void UpdateDoubleIncomeBtn(BaseEventArgs baseEventArgs)
        {
            UpdateDoubleIncomeBtn();
        }
        //更新双倍收益按钮状态
        private void UpdateDoubleIncomeBtn()
        {
            int restTime = IdleDoubleIncomeCtrl.Instance.RestTime;
            if (restTime > 0)
            {
                doubleIncomeRestTimeText.text = TimeUtils.secondsToString(restTime);
                doubleIncomeRestTimeBg.SetActive(true);
                doubleIncomeTip.SetActive(false);
            }
            else
            {
                doubleIncomeRestTimeBg.SetActive(false);
                doubleIncomeTip.SetActive(true);
            }
        }

        private void OnAnyActionTipStart(BaseEventArgs baseEventArgs)
        {
            if (!mainMenuAnim.isActive) return;
            EventArgsOne<int> args = (EventArgsOne<int>)baseEventArgs;
            switch(args.param1)
            {
                case 1:
                    ShowOrHidePostSiteActonTip(true);
                    break;
                case 2:
                    ShowOrHideSiteActonTip(true);
                    break;
                case 3:
                    ShowOrHideTruckActonTip(true);
                    break;
            }
        }

        private void OnAnyActionTipEnd(BaseEventArgs baseEventArgs)
        {
            if (!mainMenuAnim.isActive) return;
            EventArgsOne<int> args = (EventArgsOne<int>)baseEventArgs;
            switch (args.param1)
            {
                case 1:
                    ShowOrHidePostSiteActonTip(false);
                    break;
                case 2:
                    ShowOrHideSiteActonTip(false);
                    break;
                case 3:
                    ShowOrHideTruckActonTip(false);
                    break;
            }
        }

        //显示操作提示按钮
        private void ShowActionTipBtns()
        {
            ShowOrHidePostSiteActonTip(IdleActionTipCtrl.Instance.ToLevelUpPostSite);
            ShowOrHideSiteActonTip(IdleActionTipCtrl.Instance.ToLevelUpSite);
            ShowOrHideTruckActonTip(IdleActionTipCtrl.Instance.ToLevelUpTruckStation);
        }

        private void HideActionTipBtns()
        {
            actionTipAnim.Stop();
            ShowOrHidePostSiteActonTip(false);
            ShowOrHideSiteActonTip(false);
            ShowOrHideTruckActonTip(false);
        }

        private void ShowOrHidePostSiteActonTip(bool isShow)
        {
            if (isShow)
            {
                postSiteActionTipAnim.Enter();
                levelupPostSiteBtnNode.SetActive(isShow);
            }
            else
            {
                levelupPostSiteBtnNode.SetActive(isShow);
                postSiteActionTipAnim.Exit();
            }
           
        }

        private void ShowOrHideSiteActonTip(bool isShow)
        {
            if (isShow)
            {
                siteActionTipAnim.Enter();
                levelupSiteBtnNode.SetActive(isShow);
            }
            else
            {
                levelupSiteBtnNode.SetActive(isShow);
                siteActionTipAnim.Exit();
            }
           
        }

        private void ShowOrHideTruckActonTip(bool isShow)
        {
            if (isShow)
            {
                truckActionTipAnim.Enter();
                levelupTruckBtnNode.SetActive(isShow);
            }
            else
            {
                levelupTruckBtnNode.SetActive(isShow);
                truckActionTipAnim.Exit();
            }
           
        }

        private void OnActionTipBtnFinishEnter()
        {
            if(actionTipAnim!=null&&!actionTipAnim.isPlaying)
            {
                actionTipAnim.Play();
            }
        }
       
        private void OnSpecialEventEnd(BaseEventArgs baseEventArgs)
        {
            if (specialEventTimerAnim.isActive)
                specialEventTimerAnim.Exit();
        }

        private void OnSpecialEventTimerUpdate(BaseEventArgs baseEventArgs)
        {
            if (!specialEventTimerAnim.isActive) return;
            UpdateSpecialEventTime();
        }

        private void UpdateSpecialEventTime()
        {
            int restTime = IdleEventCtrl.Instance.RestTime;
            if (restTime <= 0)
                specialEventTimerAnim.Exit();
            specialEventRestTimeText.text = TimeUtils.secondsToString(restTime);
        }

        //显示或隐藏菜单按钮
        private void ShowOrHideMenuBtns(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<bool> argsOne = (EventArgsOne<bool>)baseEventArgs;
            if (isActive && argsOne.param1) return;
            if (argsOne.param1)
            {
                //leftAnim.Enter();
                //rightAnim.Enter();
                UpdateDoubleIncomeBtn();//更新双倍收益按钮
                mainMenuAnim.Enter();
                bottomLeftAnim.Enter();
                
                ShowActionTipBtns();
                IdleTaskCtrl.Instance.InitTaskProcess();
                InitMainTaskSlider();
                mainTaskBtnAnim.Enter();

                if (IdleEventCtrl.Instance.IsEventTrigger)
                {
                    eventBtn.gameObject.SetActive(true);
                    eventBtnAnim.Enter();
                }
                if(IdleEventCtrl.Instance.OnSpecialEvent)
                {
                    UpdateSpecialEventTime();
                    specialEvenRestTimeBg.SetActive(true);
                    specialEventTimerAnim.Enter();
                }
                else
                    specialEvenRestTimeBg.SetActive(false);
                if (IdleInvestorCtrl.Instance.isTriggerInvestor)
                {
                    investorBtn.gameObject.SetActive(true);
                    investorAnim.Enter();
                }
                if (inPlaneCountDown)
                {
                    followPlaneBtn.gameObject.SetActive(true);
                    followPlaneBtnAnim.Enter();
                }
                else
                    followPlaneBtn.gameObject.SetActive(false);
            }
            else
            {
                //leftAnim.Exit();
                //rightAnim.Exit();
                mainMenuAnim.Exit();
                mainTaskBtnAnim.Exit();
                bottomLeftAnim.Exit();
                specialEventTimerAnim.Exit();
                followPlaneBtnAnim.Exit();
                HideActionTipBtns();
                if (eventBtnAnim.isActive)
                    eventBtnAnim.Exit();
                if (investorAnim.isActive)
                    investorAnim.Exit();
            }
        }

        //立即关闭菜单按钮
        private void HideMenuBtnsImmediately(BaseEventArgs baseEventArgs)
        {
            mainMenuAnim.ExitImmediatelty();
            mainTaskBtnAnim.ExitImmediatelty();
            bottomLeftAnim.ExitImmediatelty();
            specialEventTimerAnim.ExitImmediatelty();
            followPlaneBtnAnim.ExitImmediatelty();
            HideActionTipBtns();
            if (eventBtnAnim.isActive)
                eventBtnAnim.ExitImmediatelty();
            if (investorAnim.isActive)
                investorAnim.ExitImmediatelty();
        }


        public override void Hide()
        {
            base.Hide();
        }

        public override void Open()
        {
            base.Open();
            IdleTaskCtrl.Instance.InitTaskProcess();

            mainMenuAnim.Enter();

         
            InitMainTaskSlider();
            mainTaskBtnAnim.Enter();

            UpdateDoubleIncomeBtn();
            bottomLeftAnim.Enter();
            ShowActionTipBtns();
            Init();
        }

        public override void Show()
        {
            base.Show();
        }

        protected override void ClosePage()
        {
            base.ClosePage();
        }

        protected override void InitPage(object args = null)
        {

        }


    }
}

