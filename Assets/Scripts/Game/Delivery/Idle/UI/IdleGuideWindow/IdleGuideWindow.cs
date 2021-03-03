using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Foundation;

namespace Delivery.Idle
{
    public class IdleGuideWindow : UIBlockerBase, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler
    {
        private RectTransform _tipsNode;
        private MyText _tips;
        private int _wordPerSecond = 40;//对话每秒出现的个数
        private Tweener _textOutTweener;
        private RectTransform _finger;
        private GameObject _targetFx;//目标特效
        private GameObject _tipFx;//提示点击区域特效

        private Image mask;//遮罩
        private Color maskColor;//遮罩的颜色
        private Material material;//遮罩的材质
        private Vector3[] corners = new Vector3[4];//遮罩的四个角
        private float diameter = 100;//直径
        private Canvas canvas;

        private IdleGuideStep _currentStep;//当前引导步骤
        private IdleUIAnimation _tipsAnim;

        private bool _isShowFinger;
        private bool _hasSetFinger;//是否已设置手指位置
        private bool _allowClick;//是否允许点击
        private void Awake()
        {
            _tipsNode = this.GetComponentByPath<RectTransform>("tipsNode");

            _tips = this.GetComponentByPath<MyText>("tipsNode/tipsBg/tips");
            _finger = this.GetComponentByPath<RectTransform>("finger");

            mask = this.GetComponentByPath<Image>("mask");
            maskColor = mask.color;
            canvas = RootCanvas.Instance.UICanvas;
            mask.rectTransform.GetWorldCorners(corners);
            material = mask.GetComponent<Image>().material;
            material.SetFloat("_Silder", diameter);


            Vector2 startPos = _tipsNode.anchoredPosition;
            startPos.y -= 100f;
            _tipsAnim = new IdleUIAnimation(_tipsNode, startPos, _tipsNode.anchoredPosition, OnTipsShow, OnTipsHide);

            _tipsNode.gameObject.SetActive(false);
            _finger.gameObject.SetActive(false);
            mask.gameObject.SetActive(false);
            _isShowFinger = false;
            _hasSetFinger = false;
            _allowClick = false;
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_StartGuideStep, OnStartGuideStep);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_GuideComplete, OnGuideComplete);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_StepComplete, OnStepComplete);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_TargetArrive, OnTargetArrive);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_StartGuideStep, OnStartGuideStep);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_StepComplete, OnStepComplete);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_GuideComplete, OnGuideComplete);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_TargetArrive, OnTargetArrive);
        }

        private void OnTipsHide()
        {
            if (_isShowFinger)
            {
                StopCoroutine("ShowFingerDelay");
                StartCoroutine("ShowFingerDelay");
            }
                //ShowFinger();
        }
        private bool finishShow = false;
        private void OnTipsShow()
        {
            finishShow = true;
        }

        private void OnStartGuideStep(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<IdleGuideStep> arg = (EventArgsOne<IdleGuideStep>)baseEventArgs;
            _currentStep = arg.param1;
            StartGuideStep();
        }
        //开始引导步骤
        private void StartGuideStep()
        {
            if (_currentStep == null) return;
            LogUtility.LogInfo($"window start guidStep={_currentStep.Id}");
            _hasSetFinger = false;
           
            HideTipFx();
            StopCoroutine("TriggerTimeEvent");
            int actionType = _currentStep.ActionType;
            switch (actionType)
            {
                case 1:
                    StartGuideStep_1();
                    break;
                case 2:
                    StartGuideStep_2();
                    break;
                case 3:
                    StartGuideStep_3();
                    break;
            }
        }

        private void OnStepComplete(BaseEventArgs baseEventArgs)
        {
            _allowClick = false;
            HideFinger();
            _isShowFinger = false;
            HideTipFx();
        }
        
        #region 开始步骤方法
        //开始操作类型为1的步骤
        private void StartGuideStep_1()
        {
            _allowClick = true;
            if (!string.IsNullOrEmpty(_currentStep.TipKey))
                ShowTips(_currentStep.TipKey);
        }
        //开始操作类型为2的步骤
        private void StartGuideStep_2()
        {
            _isShowFinger = true;
            _allowClick = false;
            HideTips();
        }
        private string _targetName;
        //开始操作类型为3的步骤
        private void StartGuideStep_3()
        {
            _allowClick = true;
            _isShowFinger = false;
            HideFinger();
            HideTips();
            //int index = _currentStep.ParentTransName.LastIndexOf('/');
            if(_currentStep.NeedFollowTarget)
            {
                int index = _currentStep.ParentTransName.LastIndexOf('/');
                _targetName = _currentStep.ParentTransName.Substring(index+1);
            }
            if (_currentStep.NeedFocus)
            {
                _targetName = "Camera";
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Guide_StartListenGuide));
            }
        }
        #endregion

        //显示对话
        private void ShowTips(string tips)
        {
            if (UIController.Instance.CurrentPage != null && UIController.Instance.CurrentPage.CurrentWindow == null)
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, false));//关闭菜单按钮
            finishShow = false;
            _tips.text = "";
            _tipsAnim.Enter();
            if (!_tipsNode.gameObject.activeSelf)
                _tipsNode.gameObject.SetActive(true);
            if (_targetFx != null)
            {
                _targetFx.SetActive(false);
            }
            _finger.gameObject.SetActive(false);
            int tipsLenth = tips.Length;
            float outTime = (float)tipsLenth / _wordPerSecond;
            _textOutTweener = _tips.DOText(tips, outTime).SetUpdate(true).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (_currentStep.Delay > 0)
                {
                    StartCoroutine("TriggerTimeEvent");
                }
            });
        }
        //隐藏对话
        private void HideTips()
        {
            if (UIController.Instance.CurrentPage != null && UIController.Instance.CurrentPage.CurrentWindow == null)
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));//打开菜单按钮
            if (!_tipsAnim.isActive)
                _tipsNode.gameObject.SetActive(false);
            else
                _tipsNode.gameObject.SetActive(true);
            _tipsAnim.Exit();
        }

        private IEnumerator ShowFingerDelay()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            ShowFinger();
            _allowClick = true;
        }

        //显示手指图片和特效
        private void ShowFinger()
        {
            string targetPath = $"{_currentStep.ParentTransName}/{_currentStep.TargetTransName}";

            if (_currentStep.IsUI)
            {
                RectTransform targetRectTrans = null;
                if (_currentStep.UIItemIndex >= 0)
                {
                    RectTransform contentRectTrans = RootCanvas.Instance.GetComponentByPath<RectTransform>(_currentStep.ParentTransName);
                    ScrollRectItem[] items = contentRectTrans.GetComponentsInChildren<ScrollRectItem>();
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].Index == _currentStep.UIItemIndex)
                        {
                            targetRectTrans = items[i].GetComponentByPath<RectTransform>(_currentStep.TargetTransName);
                        }
                    }
                }
                else
                {
                    targetRectTrans = RootCanvas.Instance.GetComponentByPath<RectTransform>(targetPath);
                }
                if (targetRectTrans == null) return;
                Vector2 targetPos = targetRectTrans.position;
                _finger.position = targetPos;
            }
            else
            {
                GameObject go = GameObject.Find(targetPath);
                if (go == null) return;
                Vector2 targetPos = Vector2.zero;
                Vector2 screenPos = Camera.main.WorldToScreenPoint(go.transform.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), screenPos, RootCanvas.Instance.UICamera, out targetPos);
                _finger.anchoredPosition3D = targetPos;
            }

            _finger.gameObject.SetActive(true);
            if (_targetFx == null)
                _targetFx = CreateTargetFx();
            else
                _targetFx.SetActive(true);
            _targetFx.transform.position = _finger.position;
            if (_tipFx != null)
                _tipFx.transform.position = _finger.position;
            _hasSetFinger = true;
        }
        

        private void HideFinger()
        {
            _finger.gameObject.SetActive(false);
            if (_targetFx != null)
                _targetFx.SetActive(false);
        }
        //创建指示目标点特效
        private GameObject CreateTargetFx()
        {
            string prefabPath = FxPrefabPath.idleGuideClickTarget;
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(prefabPath, true, 1);
            obj.transform.SetParent(transform);
            return obj;
        }

        private void ShowMask()
        {
            material.SetFloat("_IsCircle", 1);
            (canvas.transform as RectTransform).GetWorldCorners(corners);
            Vector3 center = _finger.GetComponent<RectTransform>().localPosition;
            center = new Vector4(center.x, center.y, 0f, 0f);
            material.SetVector("_Center", center);
            if(!mask.gameObject.activeSelf)
            {
                maskColor.a = 0.0f;
                mask.color = maskColor;
                mask.gameObject.SetActive(true);
                mask.DOFade(0.5f, 0.3f);
            }
        }
        private void HideMask()
        {
            mask.gameObject.SetActive(false);
        }

        private Vector2 WordToCanvasPos(Canvas canvas, Vector3 world)
        {
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out position);
            return position;
        }

        private void ShowTipFx()
        {
            if (!_hasSetFinger) return;
            if (_tipFx == null)
                _tipFx = CreateTipFx();
            _tipFx.transform.position = _finger.position;
            _tipFx.SetActive(true);
            ShowMask();
        }
        private void HideTipFx()
        {
            if (_tipFx != null)
                _tipFx.SetActive(false);
            HideMask();
        }
        //创建提示点击区域特效
        private GameObject CreateTipFx()
        {
            string prefabPath = FxPrefabPath.idleGuideTargetCircle;
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(prefabPath, true, 1);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one * 3;
            return obj;
        }


        //固定时间判断是否完成步骤
        private IEnumerator TriggerTimeEvent()
        {
            yield break;
            //yield return new WaitForSecondsRealtime(_currentStep.Delay);
            //if (!IdleGuideCtrl.Instance.JudgeStepComplete())
            //{
            //    //隔一秒再次判断，防止第一次判断失败
            //    yield return new WaitForSecondsRealtime(1.0f);
            //    IdleGuideCtrl.Instance.JudgeStepComplete();
            //}
           
        }
        //引导完成
        private void OnGuideComplete(BaseEventArgs baseEventArgs)
        {
            Destroy(gameObject);
        }

        private void OnTargetArrive(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<string> arg = (EventArgsOne<string>)baseEventArgs;
            LogUtility.LogInfo($"window target={arg.param1} arrive");
            if (string.IsNullOrEmpty(_targetName)) return;
            if (_targetName == arg.param1)
            {
                LogUtility.LogInfo("window camera arrive");
                _targetName = "";
                IdleGuideCtrl.Instance.JudgeStepComplete();
            }
        }



        public void OnPointerDown(PointerEventData eventData)
        {
            PassUIEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PassUIEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        //监听点击
        public void OnPointerClick(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        }
        //UI控件事件的渗透，不包括游戏逻辑
        private void PassUIEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            GameObject current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                if (current != results[i].gameObject)
                {
                    GameObject resultGo = results[i].gameObject;
                    if (string.IsNullOrEmpty(_currentStep.TargetTransName)) return;
                    if (_currentStep.TargetTransName.Contains(resultGo.name))
                    {
                        string parentName = resultGo.transform.parent.name;
                        if (_currentStep.UIItemIndex >= 0)
                        {
                            ScrollRectItem rectItem = resultGo.GetComponentInParent<ScrollRectItem>();
                            parentName = rectItem.transform.parent.name;
                            if (rectItem == null || rectItem.Index != _currentStep.UIItemIndex) return;
                        }
                        if (string.IsNullOrEmpty(_currentStep.ParentTransName)) return;
                        if (_currentStep.ParentTransName.Contains(parentName))
                        {
                            Selectable selectables = resultGo.GetComponent<Selectable>();
                            if (selectables != null)
                            {
                                ConsecutiveButton button = resultGo.GetComponent<ConsecutiveButton>();
                                if (button != null)
                                    button.isGuiding = true;
                                if (function.GetType() == ExecuteEvents.pointerDownHandler.GetType())
                                {
                                    selectables.image.color = selectables.colors.pressedColor;
                                }
                                else if (function.GetType() == ExecuteEvents.pointerUpHandler.GetType())
                                {
                                    selectables.image.color = selectables.colors.normalColor;
                                }
                            }
                            ExecuteEvents.Execute(resultGo, data, function);
                        }
                        return;
                    }
                }
            }
        }

        //事件渗透
        public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            if (!_allowClick) return;
            if (_currentStep == null) return;

            if (_textOutTweener != null && _textOutTweener.IsPlaying())
            {
                _textOutTweener.Kill();
                _tips.text = _currentStep.TipKey;
                if (_currentStep.Delay > 0)
                {
                    StopCoroutine("TriggerTimeEvent");
                    StartCoroutine("TriggerTimeEvent");
                }
                return;
            }

            if (!finishShow) return;

            if (_currentStep.ActionType == 3) return;

            if (_currentStep.ActionType == 1)
            {
                IdleGuideCtrl.Instance.JudgeStepComplete();
                return;
            }

            if (_currentStep.ActionType == 2 && !_currentStep.IsUI)
            {
                if (PointerClickGO(_currentStep))
                {
                    HideTipFx();
                    IdleGuideCtrl.Instance.JudgeStepComplete();
                }
                else
                {
                    ShowTipFx();
                }
                return;
            }

           
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            GameObject current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                if (current != results[i].gameObject)
                {
                    GameObject resultGo = results[i].gameObject;
                    string resultName = resultGo.name;
                    if (_currentStep.TargetTransName.Contains(resultName))
                    {
                        string parentName = resultGo.transform.parent.name;
                        if (_currentStep.UIItemIndex >= 0)
                        {
                            ScrollRectItem rectItem = resultGo.GetComponentInParent<ScrollRectItem>();
                            parentName = rectItem.transform.parent.name;
                            if (rectItem == null || rectItem.Index != _currentStep.UIItemIndex) return;
                        }

                        if (_currentStep.ParentTransName.Contains(parentName))
                        {
                            HideTipFx();
                            ExecuteEvents.Execute(resultGo, data, function);
                            IdleGuideCtrl.Instance.JudgeStepComplete();//点击到指定UI控件调用判断引导步骤是否完成
                        }
                        return;
                    }
                }
            }

            if (_currentStep.ActionType == 2 && _currentStep.IsUI)
            {
                ShowTipFx();
            }
        }


        //检测点击3d物体
        private bool PointerClickGO(IdleGuideStep guideStep)
        {
            RaycastHit result;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out result))
            {
                string resultName = result.collider.gameObject.name;
                string resultParentName = result.collider.transform.parent.name;
                int index = guideStep.ParentTransName.IndexOf(resultParentName);
                if (index < 0)
                {
                    return false;
                }

                string colliderTag = result.collider.gameObject.tag;
                if (colliderTag == Tags.CitySite)
                {
                    Transform parent = result.collider.transform.parent;
                    if (parent.tag != Tags.CitySite) return false;
                    IdleSiteModel siteModel = parent.GetComponent<IdleSiteModel>();
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Window_ShowPostSite, siteModel));
                    return true;
                }
                else if (colliderTag == Tags.DeliverySite)
                {
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowSite));
                    return true;
                }
                else if (colliderTag == Tags.TruckSite)
                {
                    EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowTruck));
                    return true;
                }
            }
            return false;
        }


    }
}

