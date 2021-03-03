//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace ColaFramework
{
    /// <summary>
    /// UI统一管理器
    /// </summary>
    [Obsolete]
    public class UIMgr : IViewManager, IEventHandler
    {
        private static UIMgr instance;

        /// <summary>
        /// 用于存储所有的UI的字典
        /// </summary>
        private Dictionary<string, UIBase> uiList;

        /// <summary>
        /// 用于存储参与点击其他地方关闭面板管理的UI的列表
        /// </summary>
        private List<UIBase> outTouchList;

        /// <summary>
        /// 存储要进行统一关闭面板的列表
        /// </summary>
        private List<UIBase> removeList;

        /// <summary>
        /// 存储统一隐藏/恢复显示的UI列表
        /// </summary>
        private List<UIBase> recordList;

        /// <summary>
        /// 消息-回调函数字典，接收到消息后调用字典中的回调方法
        /// </summary>
        private Dictionary<string, MsgHandler> msgHanderDic;

        /// <summary>
        /// UI界面排序管理器
        /// </summary>
        private UISorterMgr uiSorterMgr;

        /// <summary>
        /// 头顶字的缓存根节点
        /// </summary>
        private GameObject HUDTopBoradCache;

        /// <summary>
        /// 头顶字HUDBoard缓存列表
        /// </summary>
        private List<GameObject> HUDBoardList;

        /// <summary>
        /// 头顶字HUDBoard的展示根节点
        /// </summary>
        private GameObject HUDTopBoardRoot;

        /// <summary>
        /// 每个hudGroup下面限制挂载多少个pate,默认为25
        /// </summary>
        public int HUDLimitSize = 25;

        /// <summary>
        /// 主角的HUD根节点
        /// </summary>
        private GameObject hostHUDTopRoot;

        public UIMgr()
        {
            uiList = new Dictionary<string, UIBase>();
            outTouchList = new List<UIBase>();
            removeList = new List<UIBase>();
            recordList = new List<UIBase>();
            uiSorterMgr = new UISorterMgr(1, 8000);
            InitRegisterHandler();

            HUDTopBoardRoot = new GameObject("HUDTopBoardRoot");
            HUDTopBoardRoot.transform.SetParent(GUIHelper.GetUIRootObj().transform, false);
            HUDTopBoardRoot.layer = LayerMask.NameToLayer("UI");
            var canvas = HUDTopBoardRoot.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = GUIHelper.GetUIRoot().sortingOrder - 2;

            HUDTopBoradCache = new GameObject("HUDTopBoradCache");
            GameObject.DontDestroyOnLoad(HUDTopBoradCache);
            HUDTopBoradCache.SetActive(false);

            /*---------------UI界面控制脚本添加-------------------*/
            UIBase ui = new UILogin(100, UILevel.Level1);
            uiList.Add("UILogin", ui);
            ui = new UILoading(101, UILevel.Common);
            uiList.Add("UILoading", ui);
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
            {"OpenUIWithReturn",data=> { OpenUIWithReturn(data.ParaList[0] as string); }},
            {"CloseUI",data=>{Close(data.ParaList[0] as string);}},
        };
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
        /// 设置一个UI界面参与点击其他地方关闭面板管理
        /// </summary>
        /// <param name="ui"></param>
        public void SetOutTouchDisappear(UIBase ui)
        {
            if (!outTouchList.Contains(ui))
            {
                outTouchList.Add(ui);
            }
        }

        /// <summary>
        /// 分发处理点击其他地方关闭面板的
        /// </summary>
        /// <param name="uiName"></param>
        public void NotifyDisappear(string panelName)
        {
            removeList.Clear();
            for (int i = 0; i < outTouchList.Count; i++)
            {
                if (null != outTouchList[i] && outTouchList[i].Name != panelName)
                {
                    outTouchList[i].Close();
                    removeList.Add(outTouchList[i]);
                    break; //每次只关闭一个界面
                }
            }

            //从outTouch列表中移除已经关闭的UI界面
            for (int i = 0; i < removeList.Count; i++)
            {
                outTouchList.Remove(removeList[i]);
            }
        }

        public bool Open(string uiType)
        {
            if (null != uiList && uiList.ContainsKey(uiType))
            {
                uiList[uiType].Open();
                return uiList[uiType].IsShow;
            }
            return false;
        }

        public void Close(string uiType)
        {
            if (null != uiList && uiList.ContainsKey(uiType))
            {
                uiList[uiType].Close();
            }
        }

        public void Destroy(string uiType)
        {
            if (null != uiList && uiList.ContainsKey(uiType))
            {
                uiList[uiType].Destroy();
            }
        }

        public void UpdateUI(string uiType, EventData eventData)
        {
            if (null != uiList && uiList.ContainsKey(uiType))
            {
                uiList[uiType].UpdateUI(eventData);
            }
        }

        public UIBase GetViewByType(string uiType)
        {
            if (null != uiList && uiList.ContainsKey(uiType))
            {
                return uiList[uiType];
            }
            Debug.LogWarning(string.Format("UIMgr中不包含类型为{0}的UI", uiType));
            return null;
        }

        public void AddView(string uiType, UIBase ui)
        {
            if (null != uiList && !uiList.ContainsKey(uiType))
            {
                uiList.Add(uiType, ui);
            }
            Debug.LogWarning(string.Format("添加类型为{0}的UI到UIMgr中失败！", uiType));
        }

        public void RemoveViewByType(string uiType)
        {
            if (null != uiList && uiList.ContainsKey(uiType))
            {
                uiList.Remove(uiType);
            }
        }

        public bool OpenUIWithReturn(string uiType)
        {
            CheckFuncResult result = CommonHelper.CheckFuncOpen(uiType, true);
            if (CheckFuncResult.True == result)
            {
                return Open(uiType);
            }
            return false;
        }

        /// <summary>
        /// 记录并隐藏除了指定类型的当前显示的所有UI；
        /// </summary>
        public void StashAndHideAllUI(string uiType, params string[] extUITypes)
        {
            recordList.Clear();
            using (var enumator = uiList.GetEnumerator())
            {
                while (enumator.MoveNext())
                {
                    if (enumator.Current.Value.IsShow &&
                        !CommonHelper.IsArrayContainString(enumator.Current.Key, extUITypes))
                    {
                        recordList.Add(enumator.Current.Value);
                        enumator.Current.Value.Show(false);
                    }
                }
            }
        }

        /// <summary>
        /// 恢复显示之前记录下来的隐藏UI
        /// </summary>
        public void PopAndShowAllUI()
        {
            if (null == recordList || recordList.Count == 0)
            {
                return;
            }

            for (int i = 0; i < recordList.Count; i++)
            {
                recordList[i].Show(true);
            }
            recordList.Clear();
        }

        /// <summary>
        /// 统一关闭属于某一UI层
        /// </summary>
        /// <param name="level"></param>
        public void CloseUIByLevel(UILevel level)
        {
            if (null != uiList)
            {
                using (var enumator = uiList.GetEnumerator())
                {
                    while (enumator.MoveNext())
                    {
                        var ui = enumator.Current.Value;
                        if (level == ui.UILevel)
                        {
                            ui.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 显示UI背景模糊
        /// </summary>
        /// <param name="ui"></param>
        public void ShowUIBlur(UIBase ui)
        {
            string uiBlurName = string.Format("blur_{0}", ui.Name);
            GameObject uiBlurObj = ui.Panel.FindChildByPath(uiBlurName);
            if (null != uiBlurObj)
            {
                RawImage rawImage = uiBlurObj.GetComponent<RawImage>();
                SetBlurRawImage(rawImage);
            }
            else
            {
                CreateUIBlur(ui, uiBlurName);
            }
        }

        /// <summary>
        /// 创建UI背景模糊
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="blurName"></param>
        private void CreateUIBlur(UIBase ui, string blurName)
        {
            GameObject uiBlurObj = new GameObject(blurName);
            uiBlurObj.transform.SetParent(ui.Panel.transform, false);
            uiBlurObj.layer = ui.Layer;
            RawImage rawImage = uiBlurObj.AddComponent<RawImage>();
            Button button = uiBlurObj.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            RectTransform rectTransform = uiBlurObj.GetComponent<RectTransform>();
            if (null == rectTransform)
            {
                rectTransform = uiBlurObj.AddComponent<RectTransform>();
            }
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.SetAsFirstSibling();
            SetBlurRawImage(rawImage);
        }

        /// <summary>
        /// 设置背景模糊RawImage
        /// </summary>
        /// <param name="rawImage"></param>
        /// <param name="blurName"></param>
        /// <returns></returns>
        private void SetBlurRawImage(RawImage rawImage)
        {
            if (null != rawImage)
            {
                rawImage.gameObject.SetActive(false);
                RenderTexture texture = GUIHelper.GetEffectCameraObj().GetComponent<ImageEffectUIBlur>().FinalTexture;
                if (texture)
                {
                    rawImage.texture = texture;
                }
                rawImage.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 获取UI排序管理器
        /// </summary>
        /// <returns></returns>
        public UISorterMgr GetUISorterMgr()
        {
            return uiSorterMgr;
        }

        /// <summary>
        /// 创建头顶字HUDBoard组
        /// </summary>
        /// <returns></returns>
        public GameObject CreateHUDBoardTopGroup()
        {
            GameObject HUDTopGroup;
            if (HUDTopBoradCache.ChildCount() > 0)
            {
                HUDTopGroup = HUDTopBoradCache.GetChild(0); //每次取栈顶元素
                HUDTopGroup.transform.SetParent(HUDTopBoardRoot.transform, false);
                HUDTopGroup.transform.localScale = Vector3.one;
                HUDTopGroup.transform.SetAsFirstSibling();
            }
            else
            {
                HUDTopGroup = new GameObject("group");
                var rectTransform = HUDTopGroup.AddSingleComponent<RectTransform>();
                rectTransform.transform.SetParent(HUDTopBoardRoot.transform, false);
                rectTransform.anchoredPosition = Vector2.one;
                HUDTopGroup.transform.localScale = Vector3.one;
                HUDTopGroup.layer = LayerMask.NameToLayer("UI");
                rectTransform.SetAsFirstSibling();
            }

            var hostRoot = GetHostHUDRoot();
            hostRoot.transform.SetAsLastSibling(); //主角的HUD永远显示在最前面

            HUDBoardList.Add(HUDTopGroup);
            return HUDTopGroup;
        }

        /// <summary>
        /// 获取主角的HUDBoard根节点，主角永远显示在最前面
        /// </summary>
        /// <returns></returns>
        public GameObject GetHostHUDRoot()
        {
            if (null == hostHUDTopRoot)
            {
                hostHUDTopRoot = new GameObject("host_group");
                var rectTransform = hostHUDTopRoot.AddSingleComponent<RectTransform>();
                rectTransform.SetParent(HUDTopBoardRoot.transform, false);
                rectTransform.anchoredPosition = Vector2.zero;
                hostHUDTopRoot.transform.localScale = Vector3.zero;
                hostHUDTopRoot.layer = LayerMask.NameToLayer("UI");
            }
            return hostHUDTopRoot;
        }

        /// <summary>
        /// 获取当前负载均衡中比较空闲的hud节点
        /// </summary>
        /// <returns></returns>
        public GameObject GetHUDTopBoardRoot()
        {
            GameObject bestNode = null;
            for (int i = 0; i < HUDBoardList.Count; i++)
            {
                //找到所有HUD根节点缓存中比较空闲的
                if (HUDBoardList[i].ChildCount() < HUDLimitSize)
                {
                    bestNode = HUDBoardList[i];
                    break;
                }
            }
            if (null == bestNode)
            {
                //如果当前的负载均衡中都比较忙，就再新建一个hud节点出来
                bestNode = CreateHUDBoardTopGroup();
            }
            return bestNode;
        }


        public GameObject CreateHUD(string name)
        {
            var obj = new GameObject(name);
            var root = GetHUDTopBoardRoot();
            var rectTrans = obj.AddSingleComponent<RectTransform>();
            rectTrans.SetParent(root.transform, false);
            rectTrans.anchoredPosition = Vector2.zero;
            obj.transform.localScale = Vector3.one;
            return obj;
        }
    }
}
