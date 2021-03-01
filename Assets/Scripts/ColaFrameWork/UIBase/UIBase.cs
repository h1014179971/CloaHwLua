//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace ColaFramework
{
    /// <summary>
    /// UI基类(废弃，使用Lua的UIBase基类)
    /// </summary>
    [Obsolete("UI基类(废弃，使用Lua的UIBase基类)")]
    public class UIBase : IEventHandler, IUGUIEventHandler
    {
        /// <summary>
        /// 当前的UI界面GameObject
        /// </summary>
        public GameObject Panel { get; protected set; }

        /// <summary>
        /// 当前的UI界面GameObject对应的唯一资源ID
        /// </summary>
        public int ResId { get; protected set; }

        /// <summary>
        /// UI界面的名字
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// gameObject的layer
        /// </summary>
        public int Layer
        {
            get { return this.Panel.layer; }
            set { this.Panel.layer = value; }
        }

        /// <summary>
        ///  UI的显隐状态
        /// </summary>
        public bool IsShow { get { return Panel != null && Panel.activeSelf; } }

        /// <summary>
        /// UI的等级/类型
        /// </summary>
        public UILevel UILevel { get; set; }

        /// <summary>
        /// UI的创建方法
        /// </summary>
        protected UICreateType uiCreateType = UICreateType.Res;

        /// <summary>
        /// 消息-回调函数字典，接收到消息后调用字典中的回调方法
        /// </summary>
        protected Dictionary<string, MsgHandler> msgHanderDic;

        /// <summary>
        /// 消息传递过来的数据
        /// </summary>
        protected EventData eventData;

        /// <summary>
        /// 存储关联的子UI列表
        /// </summary>
        private List<UIBase> subUIList;

        /// <summary>
        /// UI排序组件
        /// </summary>
        private SorterTag sorterTag;
        /// <summary>
        /// UI深度层级标识
        /// </summary>
        public UIDepth uiDepthLayer = UIDepth.Normal;
        /// <summary>
        /// 挂在UI根节点上的Canvas组件
        /// </summary>
        private Canvas uiCanvas;
        /// <summary>
        /// UI是否参与排序
        /// </summary>
        protected bool sortEnable = true;

        public UIBase(int resId, UILevel uiLevel, UIDepth depth = UIDepth.Normal)
        {
            this.UILevel = uiLevel;
            ResId = resId;
            this.uiCreateType = UICreateType.Res;
            this.uiDepthLayer = depth;
            //this.Name = CommonHelper.GetResourceMgr().GetResNameById(resId);
            this.Name = "";
        }

        protected UIBase(UIDepth depth = UIDepth.Normal)
        {
            this.ResId = 0;
            this.uiDepthLayer = depth;
            this.uiCreateType = UICreateType.Go;
        }

        protected void CreateWithGO(GameObject panel, UILevel uiLevel)
        {
            this.UILevel = uiLevel;
            this.ResId = 0;
            this.Panel = panel;
            this.Name = panel.name;
            this.uiCreateType = UICreateType.Go;
        }

        /// <summary>
        /// 打开一个UI界面
        /// </summary>
        public virtual void Open()
        {
            this.Create();
        }

        /// <summary>
        /// 关闭一个UI界面
        /// </summary>
        public virtual void Close()
        {
            this.Destroy();
        }

        /// <summary>
        /// UI界面显隐的时候会调用该方法
        /// </summary>
        /// <param name="isShow"></param>UI的是否显示
        public virtual void OnShow(bool isShow)
        {

        }

        /// <summary>
        /// 创建UI界面
        /// </summary>
        public virtual void Create()
        {
            this.InitRegisterHandler();
            if (UICreateType.Res == uiCreateType)
            {
                if (null != this.Panel)
                {
                    GameObject.Destroy(Panel);
                }
                this.Panel = null;/*CommonHelper.InstantiateGoByID(ResId, GUIHelper.GetUIRootObj());*/
            }
            else if (UICreateType.Go == uiCreateType)
            {
                this.Panel.SetActive(true);
            }

            //UI参与排序
            if (sortEnable)
            {
                sorterTag = this.Panel.AddSingleComponent<SorterTag>();
                uiCanvas = this.Panel.AddSingleComponent<Canvas>();
                this.Panel.AddSingleComponent<GraphicRaycaster>();
                uiCanvas.overrideSorting = true;
                this.Panel.AddSingleComponent<ParticleOrderAutoSorter>();

                CommonHelper.GetUIMgr().GetUISorterMgr().AddPanel(this);
            }

            AttachListener(Panel);
            this.OnCreate();
            this.OnShow(IsShow);
        }

        /// <summary>
        /// UI结束后中会调用该方法
        /// </summary>
        public virtual void OnCreate()
        {
        }

        /// <summary>
        /// 销毁一个UI界面
        /// </summary>
        public virtual void Destroy()
        {
            if (sortEnable)
            {
                CommonHelper.GetUIMgr().GetUISorterMgr().RemovePanel(this);
            }
            DestroySubPanels();
            this.UnRegisterHandler();
            UnAttachListener(Panel);
            if (UILevel == UILevel.Level1)
            {
                GUIHelper.GetEffectCameraObj().GetComponent<ImageEffectUIBlur>().FinalTexture = null;
            }
            if (null != Panel)
            {
                if (uiCreateType == UICreateType.Res)
                {
                    GameObject.Destroy(Panel);
                    Panel = null;
                }
                else if (UICreateType.Go == uiCreateType)
                {
                    Show(false);
                }
            }
            this.OnDestroy();
        }

        /// <summary>
        /// 销毁UI界面后调用该方法
        /// </summary>
        public virtual void OnDestroy()
        {

        }

        /// <summary>
        /// 设置一个UI界面的显隐
        /// </summary>
        /// <param name="isActive"></param>UI界面是否显示
        public void Show(bool isActive)
        {
            this.Panel.SetActive(isActive);
            this.OnShow(isActive);
        }

        /// <summary>
        /// 刷新UI界面
        /// </summary>
        /// <param name="eventData"></param>消息数据
        public virtual void UpdateUI(EventData eventData)
        {
            if (null == eventData) return;
            this.eventData = eventData;
            if (false == IsShow) return;
        }

        /// <summary>
        /// 关联子UI，统一参与管理
        /// </summary>
        /// <param name="subPanelPath"></param>
        /// <param name="subUI"></param>
        public void AttachSubPanel(string subPanelPath, UIBase subUI, UILevel uiLevel)
        {
            if (null == subUIList)
            {
                subUIList = new List<UIBase>();
            }
            if (subUI == null) { return; }
            if (string.IsNullOrEmpty(subPanelPath)) { return; }
            GameObject subUIObj = Panel.FindChildByPath(subPanelPath);
            if (null != subUIObj)
            {
                subUI.CreateWithGO(subUIObj, uiLevel);
                subUIList.Add(subUI);
            }
        }

        /// <summary>
        /// 将一个UI界面注册为本UI的子界面，统一参与管理
        /// </summary>
        public void RegisterSubPanel(UIBase subUI)
        {
            if (null == subUIList)
            {
                subUIList = new List<UIBase>();
            }
            if (subUI == null) { return; }
            subUI.uiDepthLayer = this.uiDepthLayer;
            subUIList.Add(subUI);
        }

        /// <summary>
        /// 解除子UI关联
        /// </summary>
        /// <param name="subUI"></param>
        public void DetchSubPanel(UIBase subUI)
        {
            if (null != subUIList)
            {
                subUIList.Remove(subUI);
            }
        }

        /// <summary>
        /// 销毁关联的子面板
        /// </summary>
        private void DestroySubPanels()
        {
            if (null != subUIList)
            {
                for (int i = 0; i < subUIList.Count; i++)
                {
                    subUIList[i].Destroy();
                    subUIList[i].Panel = null;
                }
                subUIList.Clear();
            }
        }

        /// <summary>
        /// 将当前UI层级提高，展示在当前Level的最上层
        /// </summary>
        public void BringTop()
        {
            var uiSorterMgr = CommonHelper.GetUIMgr().GetUISorterMgr();
            uiSorterMgr.MovePanelToTop(this);
        }


        /// <summary>
        /// 处理消息的函数的实现
        /// </summary>
        /// <param name="gameEvent"></param>事件
        /// <returns></returns>是否处理成功
        public bool HandleMessage(GameEvent evt)
        {
            bool handled = false;
            if (EventType.UIMsg == evt.EventType)
            {
                if (null != msgHanderDic)
                {
                    EventData eventData = evt.Para as EventData;
                    if (null != eventData && msgHanderDic.ContainsKey(eventData.Cmd))
                    {
                        msgHanderDic[eventData.Cmd](eventData);
                        handled = true;
                    }
                }
            }
            return handled;
        }

        /// <summary>
        /// 是否处理了该消息的函数的实现
        /// </summary>
        /// <returns></returns>是否处理
        public bool IsHasHandler(GameEvent evt)
        {
            bool handled = false;
            if (EventType.UIMsg == evt.EventType)
            {
                if (null != msgHanderDic)
                {
                    EventData eventData = evt.Para as EventData;
                    if (null != eventData && msgHanderDic.ContainsKey(eventData.Cmd))
                    {
                        handled = true;
                    }
                }
            }
            return handled;
        }

        /// <summary>
        /// 初始化注册消息监听
        /// </summary>
        protected void InitRegisterHandler()
        {
            msgHanderDic = null;
            GameEventMgr.GetInstance().RegisterHandler(this, EventType.UIMsg);
            msgHanderDic = new Dictionary<string, MsgHandler>()
            {
            };
        }

        /// <summary>
        /// 取消注册该UI监听的所有消息
        /// </summary>
        protected void UnRegisterHandler()
        {
            GameEventMgr.GetInstance().UnRegisterHandler(this);

            if (null != msgHanderDic)
            {
                msgHanderDic.Clear();
                msgHanderDic = null;
            }
        }

        /// <summary>
        /// 注册一个UI界面上的消息
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="msgHandler"></param>
        public void RegisterEvent(string evt, MsgHandler msgHandler)
        {
            if (null != msgHandler && null != msgHanderDic)
            {
                if (!msgHanderDic.ContainsKey(evt))
                {
                    msgHanderDic.Add(Name + evt, msgHandler);
                }
                else
                {
                    Debug.LogWarning(string.Format("消息{0}重复注册！", evt));
                }
            }
        }

        /// <summary>
        /// 取消注册一个UI界面上的消息
        /// </summary>
        /// <param name="evt"></param>
        public void UnRegisterEvent(string evt)
        {
            if (null != msgHanderDic)
            {
                msgHanderDic.Remove(Name + evt);
            }
        }

        /// <summary>
        /// 显示UI背景模糊
        /// </summary>
        public void ShowUIBlur()
        {
            CommonHelper.GetUIMgr().ShowUIBlur(this);
        }

        /// <summary>
        /// 设置点击外部关闭(执行该方法以后，当点击其他UI的时候，会自动关闭本UI)
        /// </summary>
        public void SetOutTouchDisappear()
        {
            CommonHelper.GetUIMgr().SetOutTouchDisappear(this);
        }

        #region UI事件回调处理逻辑

        /// <summary>
        /// 注册UIEventListener
        /// </summary>
        /// <param name="obj"></param>
        public void AttachListener(GameObject obj)
        {

            ScrollRect[] rects = obj.GetComponentsInChildren<ScrollRect>(true);
            for (int i = 0; i < rects.Length; i++)
            {
                AddOtherEventListenner(rects[i]);
            }
            Selectable[] selectable = obj.GetComponentsInChildren<Selectable>(true);

            foreach (Selectable st in selectable)
            {
                AddEventaHandler(st);
            }
        }

        void AddEventaHandler(Selectable st)
        {
            UGUIEventListener listener = st.gameObject.GetComponent<UGUIEventListener>();

            if (listener == null) //防止多次AttachListener
            {
                if ((st is Scrollbar) || (st is InputField) || (st is Slider))
                {
                    listener = st.gameObject.AddComponent<UGUIDragEventListenner>();
                }
                else
                {
                    //此处正常button是可以响应拖拽事件但有ScrollRect作为父组件的情况下会存在冲突
                    bool useDrag = false;
                    if (st is Button)
                    {
                        ScrollRect[] rect = st.gameObject.GetComponentsInParent<ScrollRect>(true);
                        useDrag = (rect == null || rect.Length == 0);
                    }

                    if (useDrag)
                    {
                        listener = st.gameObject.AddComponent<UGUIDragEventListenner>();
                    }
                    else
                    {
                        listener = st.gameObject.AddComponent<UGUIEventListener>();
                    }

                }
                listener.uiHandler = this;
            }
            else
            {
                if (this == listener.uiHandler) //如果当前的和原来的一样 就不用再Attach一次
                {
                    listener.CurSelectable = st;
                    return;
                }
                else             //如果想Attach一个新的对象 先清除掉原来的
                {
                    IUGUIEventHandler prevHandler = listener.uiHandler;
                    if (null != prevHandler) prevHandler.RemoveEventHandler(listener.gameObject);
                    listener.uiHandler = this;
                }
            }
            //在listenner上面记录Selectable组件
            listener.CurSelectable = st;
            AddEventHandlerEx(listener);
        }

        void AddEventHandlerEx(UGUIEventListener listener)
        {
            listener.onClick += onClick;
            listener.onDown += onDown;
            listener.onUp += onUp;
            listener.onDownDetail += this.onDownDetail;
            listener.onUpDetail += this.onUpDetail;
            listener.onEnter += onEnter;
            listener.onExit += onExit;
            listener.onDrop += onDrop;
            listener.onBeginDrag += onBeginDrag;
            listener.onDrag += onDrag;
            listener.onEndDrag += onEndDrag;
            listener.onSelect += onSelect;
            listener.onDeSelect += onDeSelect;
            listener.onScroll += onScroll;
            listener.onCancel += onCancel;
            listener.onSubmit += onSubmit;
            listener.onMove += onMove;
            listener.onUpdateSelected += onUpdateSelected;
            listener.onInitializePotentialDrag += this.onInitializePotentialDrag;
            listener.onEvent += onEvent;
            AddOtherEventHandler(listener.gameObject);
        }

        void AddOtherEventHandler(GameObject go)
        {
            OtherEventListenner otherlistenner = go.GetComponent<OtherEventListenner>();
            if (otherlistenner == null)
                otherlistenner = go.AddComponent<OtherEventListenner>();
            otherlistenner.inputvalueChangeAction += onStrValueChange;
            otherlistenner.inputeditEndAction += onEditEnd;
            otherlistenner.togglevalueChangeAction += onBoolValueChange;
            otherlistenner.slidervalueChangeAction += onFloatValueChange;
            otherlistenner.scrollbarvalueChangeAction += onFloatValueChange;
            otherlistenner.onEvent += onEvent;
        }

        void AddOtherEventListenner(ScrollRect rect)
        {

            OtherEventListenner otherlistenner = rect.gameObject.GetComponent<OtherEventListenner>();
            if (otherlistenner == null)
                otherlistenner = rect.gameObject.AddComponent<OtherEventListenner>();
            otherlistenner.onEvent += onEvent;
        }

        /// <summary>
        /// 反注册UIEventListener
        /// </summary>
        /// <param name="obj"></param>
        public void UnAttachListener(GameObject obj)
        {
            Selectable[] selectable = obj.GetComponentsInChildren<Selectable>(true);

            foreach (Selectable st in selectable)
            {
                RemoveEventHandler(st.gameObject);
            }
        }

        public void RemoveEventHandler(GameObject obj)
        {
            UGUIEventListener listener = obj.GetComponent<UGUIEventListener>();
            if (listener == null) return;
            if (listener.uiHandler == null || listener.uiHandler != this)        //必须在touch过同一个 MsgHandler的情况下才能用这个MsgHandler进行untouch
                return;

            listener.onClick -= onClick;
            listener.onDown -= onDown;
            listener.onUp -= onUp;
            listener.onEnter -= onEnter;
            listener.onExit -= onExit;
            listener.onDrop -= onDrop;
            listener.onBeginDrag -= onBeginDrag;
            listener.onDrag -= onDrag;
            listener.onEndDrag -= onEndDrag;
            listener.onSelect -= onSelect;
            listener.onDeSelect -= onDeSelect;
            listener.onScroll -= onScroll;
            listener.onCancel -= onCancel;
            listener.onSubmit -= onSubmit;
            listener.onMove -= onMove;
            listener.onUpdateSelected -= onUpdateSelected;
            listener.onInitializePotentialDrag -= onInitializePotentialDragHandle;
            listener.onEvent -= onEvent;

            OtherEventListenner otherlistenner = listener.gameObject.GetComponent<OtherEventListenner>();
            if (otherlistenner != null)
            {
                otherlistenner.inputvalueChangeAction -= onStrValueChange;
                otherlistenner.inputeditEndAction -= onEditEnd;
                otherlistenner.togglevalueChangeAction -= onBoolValueChange;
                otherlistenner.slidervalueChangeAction -= onFloatValueChange;
                otherlistenner.scrollbarvalueChangeAction -= onFloatValueChange;
                otherlistenner.onEvent -= onEvent;
            }
        }
        #endregion

        #region UI回调事件

        protected virtual void onClick(string name)
        {
        }

        protected virtual void onDown(string name)
        {
        }

        protected virtual void onUp(string name)
        {
        }

        protected virtual void onEnter(string name)
        {
        }

        protected virtual void onInitializePotentialDragHandle(string name)
        {
        }

        protected virtual void onUpdateSelected(string name)
        {
        }

        protected virtual void onMove(string name)
        {
        }

        protected virtual void onSubmit(string name)
        {
        }

        protected virtual void onCancel(string name)
        {
        }

        protected virtual void onScroll(string name)
        {
        }

        protected virtual void onDeSelect(string name)
        {
        }

        protected virtual void onSelect(string name)
        {
        }

        protected virtual void onEndDrag(string name, Vector2 deltaPos, Vector2 curToucPosition)
        {
        }

        protected virtual void onDrag(string name, Vector2 deltaPos, Vector2 curToucPosition)
        {
        }

        protected virtual void onBeginDrag(string name, Vector2 deltaPos, Vector2 curToucPosition)
        {
        }

        protected virtual void onDrop(string name)
        {
        }

        protected virtual void onExit(string name)
        {
        }

        protected virtual void onStrValueChange(string name, string text)
        {
        }

        protected virtual void onIntValueChange(string name, int value)
        {
        }

        protected virtual void onRectValueChange(string name, Vector2 rect)
        {
        }

        protected virtual void onFloatValueChange(string name, float value)
        {
        }

        protected virtual void onBoolValueChange(string name, bool isSelect)
        {
        }

        protected virtual void onEditEnd(string name, string text)
        {
        }

        protected virtual void onInitializePotentialDrag(string name)
        {
        }

        protected virtual void onUpDetail(string name, Vector2 deltapos, Vector2 curtoucposition)
        {
        }

        protected virtual void onDownDetail(string name, Vector2 deltaPos, Vector2 curToucPosition)
        {
        }

        /// <summary>
        /// 触发UI事件时会触发onEvent方法(在需要的事件里面添加即可)
        /// </summary>
        /// <param name="eventName"></param>触发的事件名称
        protected virtual void onEvent(string eventName)
        {
            if (eventName == "onClick")
            {
                CommonHelper.GetUIMgr().NotifyDisappear(Name);
            }
        }

        #endregion
    }
}


