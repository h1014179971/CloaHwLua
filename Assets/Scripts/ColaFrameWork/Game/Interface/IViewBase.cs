//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColaFramework
{
    /// <summary>
    /// ColaFramework框架中自定义的组件
    /// </summary>
    public interface IComponent
    {

    }

    /// <summary>
    /// ColaFramework框架中自定义的控制类组件(有一些UI响应等高级功能)
    /// </summary>
    public interface IControl : IComponent
    {

    }

    /// <summary>
    /// UI管理器接口
    /// </summary>
    [Obsolete]
    public interface IViewManager
    {
        /// <summary>
        /// UI管理器，打开某种类型的UI,并返回界面是否打开成功
        /// </summary>
        /// <typeparam name="T"></typeparam>
        bool Open(string uiType);

        /// <summary>
        /// UI管理器，关闭某种类型的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Close(string uiType);

        /// <summary>
        /// UI管理器，销毁某种类型的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Destroy(string uiType);

        /// <summary>
        /// UI管理器，刷新某种类型的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventData"></param>
        void UpdateUI(string uiType, EventData eventData);

        /// <summary>
        /// UI管理器，获取某种类型的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        UIBase GetViewByType(string uiType);

        /// <summary>
        /// UI管理器，将一个UI加入到UI管理器中参与管理
        /// </summary>
        /// <param name="ui"></param>
        void AddView(string uiType, UIBase ui);

        /// <summary>
        /// UI管理器，从UI管理器中移除某个UI，使其不再参与管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemoveViewByType(string uiType);

        /// <summary>
        /// UI管理器，打开某种类型的UI,并返回界面是否打开成功,带有功能开启检查
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        bool OpenUIWithReturn(string uiType);
    }

    /// <summary>
    /// UI界面等级枚举
    /// </summary>
    public enum UILevel : byte
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Common = 4,
    }

    /// <summary>
    /// UI的创建形式
    /// </summary>
    public enum UICreateType : byte
    {
        /// <summary>
        /// 根据一个资源ID创建
        /// </summary>
        Res = 0,
        /// <summary>
        /// 根据一个传入的现有gameobjec
        /// </summary>
        Go = 1,
    }

    /// <summary>
    /// UI排序的接口
    /// </summary>
    public interface ISorter
    {
        /// <summary>
        /// 动态排序功能 对Panel下面的canvas的sortingOrder排序
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="sortIndex"></param>
        /// <returns></returns>
        int SortIndexSetter(GameObject panel, int sortIndex);

        /// <summary>
        /// 根据sortIndex设置SortTag的SetSiblingIndex 排序
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="sortIndex"></param>
        /// <returns></returns>
        int SortTagIndexSetter(GameObject panel, int sortIndex);

        /// <summary>
        /// 对持有3d模型的ui排序
        /// </summary>
        /// <param name="model"></param>
        /// <param name="postion"></param>
        /// <returns></returns>
        int SortTag3DSetter(GameObject model, int z, bool isHigher);

        /// <summary>
        /// 将Panel置于其当前层最上方
        /// </summary>
        /// <param name="ui"></param>
        [Obsolete]
        void MovePanelToTop(UIBase ui);

        /// <summary>
        /// 对UI进行重新排序
        /// </summary>
        void ReSortPanels();

        /// <summary>
        /// 增加Panel到当前层的当前最上方
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="uiLevel"></param>
        [Obsolete]
        void AddPanel(UIBase ui);

        /// <summary>
        /// 移除指定Panel
        /// </summary>
        /// <param name="ui"></param>
        [Obsolete]
        void RemovePanel(UIBase ui);
    }

    /// <summary>
    /// UI的深度层级标识
    /// </summary>
    public enum UIDepth : byte
    {
        Bottom = 1,
        Normal = 2,
        Top = 3,
    }
}
