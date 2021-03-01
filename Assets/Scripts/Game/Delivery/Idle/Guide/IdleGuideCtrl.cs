using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.Events;

namespace Delivery.Idle
{
    public class IdleGuideCtrl : MonoSingleton<IdleGuideCtrl>
    {

        private int _clickCount;//点击次数
        private float _startTime;//步骤开始时间
        private IdleGuideMgr _idleGuideMgr;
        private GameObject _guideWindow;
        private IdleGuide _currentGuide;//当前引导
        private IdleGuideStep _currentStep;//当前步骤
        private float _lastStepDelay;//上一个操作步骤的延迟（完成某一步骤后，延迟开始下一步骤）
        private bool _isGuiding;//是否正在进行引导
        private List<IdleGuideStep> _guideSteps = new List<IdleGuideStep>();//当前引导的所有步骤
        private Dictionary<int, PlayerGuide> _playerGuideDic = new Dictionary<int, PlayerGuide>();//所有引导步骤

        private Dictionary<int, Func<IdleGuide, bool>> _judgeGuideStartFuncs = new Dictionary<int, Func<IdleGuide, bool>>();//判断是否开始引导方法组
        private Dictionary<int, Func<IdleGuideStep, bool>> _judgeStepCompleteFuncs = new Dictionary<int, Func<IdleGuideStep, bool>>();//判断步骤是否完成方法组

        private CameraAotoMove _cameraAutoMove;

        public bool isOfflineWindowActive;//离线界面是否开启
        public bool IsGuiding
        {
            get
            {
                return _isGuiding;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_StartGuide, OnStartGuide);
        }
        public void Init()
        {
            _idleGuideMgr = IdleGuideMgr.Instance;
            _playerGuideDic = PlayerMgr.Instance.GetPlayerGuides();
            AddJudgeGuideStartFuncs();
            AddJudgeStepFuncs();
            _currentGuide = null;
            _currentStep = null;
            _lastStepDelay = -1;
            _isGuiding = false;
            _cameraAutoMove = FindObjectOfType<CameraAotoMove>();

            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_StartGuide, OnStartGuide);
        }


        private void OnStartGuide(BaseEventArgs baseEventArgs)
        {
            if (isOfflineWindowActive) return;
            if (_playerGuideDic.Count == 0) return;
            EventArgsOne<int> arg = (EventArgsOne<int>)baseEventArgs;
            int conditionKey = arg.param1;

            var enumerator = _playerGuideDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PlayerGuide playerGuide = enumerator.Current.Value;
                IdleGuide idleGuide = IdleGuideMgr.Instance.GetIdleGuide(playerGuide.id);
                if (idleGuide.ConditionKey == conditionKey)
                {
                    if (JudgeGuideStart(idleGuide))
                    {
                        //开始引导
                        StartGuide(idleGuide);
                        return;
                    }
                }
            }
        }



        #region 判断是否达到开始引导的条件
        //添加判断引导开始的判断方法
        private void AddJudgeGuideStartFuncs()
        {
            _judgeGuideStartFuncs.Clear();
            _judgeGuideStartFuncs.Add(1, JudgeGuideStart_1);
            _judgeGuideStartFuncs.Add(2, JudgeGuideStart_2);
        }
        //判断是否达到开始引导的条件
        private bool JudgeGuideStart(IdleGuide idleGuide)
        {
            int conditionKey = idleGuide.ConditionKey;
            if (_judgeGuideStartFuncs.ContainsKey(conditionKey))
            {
                return _judgeGuideStartFuncs[conditionKey].Invoke(idleGuide);
            }
            return false;
        }

        //条件值为1的判断方法
        private bool JudgeGuideStart_1(IdleGuide idleGuide)
        {
            return true;
        }
        //条件值为2的判断方法
        private bool JudgeGuideStart_2(IdleGuide idleGuide)
        {
            Long2 conditionValue = new Long2(idleGuide.ConditionValue);
            if (PlayerMgr.Instance.PlayerCity.money >= conditionValue)
            {
                return true;
            }
            return false;
        }
        #endregion;


        private IEnumerator StartGuideDelay(IdleGuide idleGuide,float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            StartGuide(idleGuide);
        }

        //开始引导
        private void StartGuide(IdleGuide idleGuide)
        {
            LogUtility.LogInfo("ctrl before start guide");
            if (idleGuide == null) return;
            if (_isGuiding) return;
            _isGuiding = true;
            PlayerGuide playerGuide = PlayerMgr.Instance.GetPlayerGuide(idleGuide.Id);
            playerGuide.hasStart = true;

            //初始化引导
            _currentGuide = idleGuide;
            //初始化步骤
            _guideSteps = _idleGuideMgr.GetAllGuideSteps(_currentGuide);
            if (_guideSteps.Count <= 0) return;
            LogUtility.LogInfo("ctrl before close window");

            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Guide_StartFirstStep));

            Dictionary<string, string> siteProertie = new Dictionary<string, string>();
            siteProertie["guideid"] = _currentGuide.Id.ToString();
            PlatformFactory.Instance.TAEventPropertie("gt_guide", siteProertie);

            //关闭当前窗口
            if (UIController.Instance.CurrentPage != null && UIController.Instance.CurrentPage.CurrentWindow != null)
            {
                StartCoroutine(WaitForCloseWindow());
                return;
            }

            if (_guideWindow == null)
            {
                _guideWindow = UIController.Instance.ShowBlocker("IdleGuideWindow.prefab", true);
            }
            _currentStep = _guideSteps[0];
            StartStep(_currentStep);

        }

        private IEnumerator WaitForCloseWindow()
        {
            LogUtility.LogInfo("ctrl wait close window");
            yield return new WaitForSeconds(0.1f);//等待0.1s关闭界面，避免某些效果没播完
            LogUtility.LogInfo("ctrl close window");
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_CloseCurrentWindow));
            //等待当前界面完全关闭
            while (UIController.Instance.CurrentPage.CurrentWindow != null || _cameraAutoMove.IsMoving)
            {
                LogUtility.LogInfo("ctrl in close window while");
                yield return null;
            }
            LogUtility.LogInfo("ctrl out close window while");
            if (_guideWindow == null)
            {
                _guideWindow = UIController.Instance.ShowBlocker("IdleGuideWindow.prefab", true);
            }

            //if(_lastStepDelay>0)
            //{
            //    yield return new WaitForSecondsRealtime(_lastStepDelay);
            //    _lastStepDelay = -1;
            //}

            _currentStep = _guideSteps[0];
            StartStep(_currentStep);
        }

        //开始引导步骤
        private void StartStep(IdleGuideStep guideStep)
        {
            LogUtility.LogInfo("ctrl before start step");
            if (guideStep == null) return;
            _clickCount = 0;
            _startTime = Time.unscaledTime;
            if (guideStep.PauseGame)
                Time.timeScale = 0;
            else
                Time.timeScale = 1.0f;

            if (guideStep.NeedFocus)
            {
                if (guideStep.ActionType == 3)
                    StartCoroutine(StartFocus(guideStep));
                else
                    StartFocusImmediately(guideStep);
                return;
            }
            if (guideStep.NeedFollowTarget)
            {
                string followPath = guideStep.ParentTransName;
                GameObject followGo = GameObject.Find(followPath);
                if (followGo != null)
                {
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<Transform>(EnumEventType.Event_Camera_FollowOther, followGo.transform));//镜头跟随
                }
            }
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleGuideStep>(EnumEventType.Event_Guide_StartGuideStep, guideStep));

        }
        private void StartFocusImmediately(IdleGuideStep guideStep)
        {
            LogUtility.LogInfo("ctrl immediately focus");
            string focusPath = guideStep.ParentTransName + "/" + guideStep.TargetTransName;
            GameObject focusGo = GameObject.Find(focusPath);
            if (focusGo != null)
            {
                Vector3 pos = focusGo.transform.position;
                pos.z = -1;
                float size = -1;
                EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, pos, size));//移动镜头
            }
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleGuideStep>(EnumEventType.Event_Guide_StartGuideStep, guideStep));
        }
        private IEnumerator StartFocus(IdleGuideStep guideStep)
        {
            LogUtility.LogInfo("ctrl delay start focus");
            yield return new WaitForSeconds(0.3f);
            string focusPath = guideStep.ParentTransName;
            GameObject focusGo = GameObject.Find(focusPath);
            if (focusGo != null)
            {
                Vector3 pos = focusGo.transform.position;
                pos.z = -1;
                float size = -1;
                LogUtility.LogInfo($"ctrl move camera");
                EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, pos, size));//移动镜头
            }
            else
            {
                LogUtility.LogError($"focusGo is Null;path={focusPath}");
            }
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleGuideStep>(EnumEventType.Event_Guide_StartGuideStep, guideStep));
        }

        private void OnStepComplete()
        {
            //if (_currentStep.NeedFollowTarget)
            //{
            //    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Camera_FinishFollow));
            //}
            //_lastStepDelay = _currentStep.Delay;
            //LogUtility.LogInfo($"finish guidStep={_currentStep.Id}");
            //_guideSteps.Remove(_currentStep);
            //_currentStep = null;
            //if (_guideSteps.Count <= 0)
            //{
            //    PlayerMgr.Instance.PlayerCompleteGuide(_currentGuide.Id);//从玩家数据中移除引导
            //    int nextGuideId = _currentGuide.NextGuide;
            //    if (nextGuideId < 0)
            //    {
            //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Guide_GuideComplete));
            //        Time.timeScale = 1.0f;
            //        _isGuiding = false;
            //        return;
            //    }
            //    IdleGuide idleGuide = _idleGuideMgr.GetIdleGuide(nextGuideId);
            //    _isGuiding = false;
            //    if(_lastStepDelay>0)
            //    {
            //        StartCoroutine(StartGuideDelay(idleGuide, _lastStepDelay));
            //        _lastStepDelay = -1;
            //    }
            //    else
            //    {
            //        StartGuide(idleGuide);
            //    }
            //    return;
            //}
            ////_currentStep = _guideSteps[0];
            ////StartStep(_currentStep);
            ////StartCoroutine(StartStepDelay(_guideSteps[0]));
            //StartCoroutine(StartStepDelay(_guideSteps[0], _lastStepDelay));

            StartCoroutine(FinishCurrentStep());
        }

        private IEnumerator FinishCurrentStep()
        {
            if (_currentStep.NeedFollowTarget)
            {
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Camera_FinishFollow));
            }

            if(_currentStep.ActionType==2&&_currentStep.IsUI)
            {
                while(_cameraAutoMove.IsMoving)
                {
                    yield return null;
                }
            }

            //等待当前窗口关闭
            if(_currentStep.CloseWindow)
            {
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_CloseCurrentWindow));
                while (UIController.Instance.CurrentPage.CurrentWindow != null || _cameraAutoMove.IsMoving)
                {
                    yield return null;
                }
            }
            _lastStepDelay = _currentStep.Delay;
            if(_lastStepDelay>0)
            {
                yield return new WaitForSecondsRealtime(_lastStepDelay);
                _lastStepDelay = -1;
            }
            _guideSteps.Remove(_currentStep);
            _currentStep = null;


            //开始下一步引导或者结束引导
            if (_guideSteps.Count <= 0)
            {
                PlayerMgr.Instance.PlayerCompleteGuide(_currentGuide.Id);//从玩家数据中移除引导
                int nextGuideId = _currentGuide.NextGuide;
                if (nextGuideId < 0)
                {
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Guide_GuideComplete));
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
                    Time.timeScale = 1.0f;
                    _isGuiding = false;
                    yield break;
                }
                IdleGuide idleGuide = _idleGuideMgr.GetIdleGuide(nextGuideId);
                _isGuiding = false;
                StartGuide(idleGuide);
                yield break;
            }
            _currentStep = _guideSteps[0];
            StartStep(_currentStep);

        }

        
        private IEnumerator StartStepDelay(IdleGuideStep guideStep,float delay)
        {
            if(delay>0)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            _lastStepDelay = -1;
            while (_cameraAutoMove.IsMoving)
            {
                yield return null;
            }
            _currentStep = guideStep;
            StartStep(_currentStep);
        }

        #region 判断引导步骤是否完成方法

        private void AddJudgeStepFuncs()
        {
            _judgeStepCompleteFuncs.Clear();
            _judgeStepCompleteFuncs.Add(1, IsClickStepComplete);
            _judgeStepCompleteFuncs.Add(2, IsClickStepComplete);
            _judgeStepCompleteFuncs.Add(3, IsWaitStepComplete);
            _judgeStepCompleteFuncs.Add(4, IsWaitStepComplete);
        }
        //判断引导步骤是否完成
        public bool JudgeStepComplete()
        {
            if (_judgeStepCompleteFuncs.ContainsKey(_currentStep.ActionType))
            {
                if (_judgeStepCompleteFuncs[_currentStep.ActionType].Invoke(_currentStep))
                {
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Guide_StepComplete));
                    //进行下一步
                    OnStepComplete();
                    return true;
                }
            }
            return false;
        }

        //步骤操作类型：1.点击任何地方2.点击目标按钮/游戏物体3.无操作等待下一步引导4.自动进入下一步
        // 点击指定目标或任何地方
        private bool IsClickStepComplete(IdleGuideStep guideStep)
        {
            float delay = guideStep.Delay;
            if (delay > 0 && Time.unscaledTime - _startTime >= delay)
            {
                return true;
            }
            _clickCount++;
            int target = guideStep.ClickCount;
            if (_clickCount >= target)
            {
                _clickCount = 0;
                return true;
            }
            return false;
        }
        //自动进入下一步
        private bool IsWaitStepComplete(IdleGuideStep guideStep)
        {
            //float targetTime = guideStep.Delay;
            //if (Time.unscaledTime - _startTime >= targetTime)
            //{
            //    return true;
            //}
            //return false;
            return true;
        }
        #endregion




        public override void Dispose()
        {
            base.Dispose();
            _clickCount = 0;//点击次数
            _startTime = 0;//步骤开始时间
            _idleGuideMgr = null;
            _guideWindow = null;
            _currentGuide = null;//当前引导
            _currentStep = null;//当前步骤
            _isGuiding = false;//是否正在进行引导
            _guideSteps.Clear();//当前引导的所有步骤
            _playerGuideDic.Clear();//所有引导步骤
            _judgeGuideStartFuncs.Clear();//判断是否开始引导方法组
            _judgeStepCompleteFuncs.Clear();//判断步骤是否完成方法组
            isOfflineWindowActive = false;//离线界面是否开启
        }

        public override void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_StartGuide, OnStartGuide);
        }

    }
}


