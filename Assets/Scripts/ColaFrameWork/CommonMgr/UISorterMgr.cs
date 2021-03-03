//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


namespace ColaFramework
{
    /// <summary>
    /// 参与排序的元素
    /// </summary>
    [Obsolete]
    public class UISorter
    {
        public UIBase ui;
        public int moveTop;
        public int index;

        public UISorter(UIBase ui, int moveTop, int index)
        {
            this.ui = ui;
            this.moveTop = moveTop;
            this.index = index;
        }
    }

    /// <summary>
    /// UI排序管理器
    /// </summary>
    [Obsolete]
    public class UISorterMgr : ISorter
    {
        private int minSortIndex = 0;
        private int maxSortIndex = 0;
        private List<UISorter> uiSortList;
        private List<Canvas> canvasSortList;
        private bool is3DHigher = true;

        /// <summary>
        /// UI排序管理器构造器,序号越大，界面越靠上
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="sortIndex"></param>
        /// <returns></returns>
        public UISorterMgr(int minIndex, int maxIndex)
        {
            minSortIndex = minIndex;
            maxSortIndex = maxIndex;
            uiSortList = new List<UISorter>();
            canvasSortList = new List<Canvas>();
        }

        /// <summary>
        /// 对某个Gameobject下的Canvas进行动态排序功能,识别子canvas，根据sortingOrder对canvas的排序
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="sortIndex"></param>
        /// <returns></returns>
        public int SortIndexSetter(GameObject panel, int sortIndex)
        {
            if (null == panel)
            {
                Debug.LogWarning("参与排序的ui不能为空！");
                return 0;
            }
            canvasSortList.Clear();
            var canvasList = panel.GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvasList.Length; i++)
            {
                canvasSortList.Add(canvasList[i]);
            }
            canvasSortList.Sort((x, y) => { return x.sortingOrder.CompareTo(y.sortingOrder); });

            for (int i = 0; i < canvasSortList.Count; i++)
            {
                canvasSortList[i].sortingOrder = sortIndex;
                //DropDown组件关闭按钮的层级为DropDown层级减一，所以多加一个间隔,无间隔会与其他层级冲突导致关闭功能异常
                sortIndex += 2;
            }
            return sortIndex + 2;
        }

        /// <summary>
        /// 设置UI的SortTag,根据显示修改上下关系做到排序
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="sortIndex"></param>
        /// <returns></returns>
        public int SortTagIndexSetter(GameObject panel, int sortIndex)
        {
            if (null == panel)
            {
                Debug.LogWarning("参与排序的ui不能为空！");
                return 0;
            }

            var sortTag = panel.GetComponent<SorterTag>();
            if (null == sortTag)
            {
                return sortIndex;
            }
            sortTag.SetSorter(sortIndex + 1);
            sortIndex = sortTag.GetSorter();
            return sortIndex;
        }

        /// <summary>
        /// 设置带有3D模型UI的SortTag，带3d模型的ui需要排序设置
        /// </summary>
        /// <param name="model"></param>
        /// <param name="z"></param>
        /// <param name="isHigher"></param>
        /// <returns></returns>
        public int SortTag3DSetter(GameObject model, int z, bool isHigher)
        {
            if (null == model)
            {
                return 0;
            }
            if (!model.activeSelf)
            {
                return z;
            }
            var sortTag = model.GetComponent<SorterTag>();
            if (null == sortTag)
            {
                return z;
            }
            int space3D = sortTag.Space3D;

            if (isHigher)
            {
                if (space3D > 0 || z != 0)
                {
                    z -= space3D;
                    sortTag.SetSpace3D(z);
                }
            }
            else
            {
                sortTag.SetSpace3D(z);
                z += space3D;
            }
            return z;
        }

        /// <summary>
        /// 将指定的UI提升到当前UILEVEL的最上层
        /// </summary>
        /// <param name="ui"></param>
        public void MovePanelToTop(UIBase ui)
        {
            int index;
            if (IsContainSorter(ui, out index))
            {
                uiSortList[index].moveTop = 1;
                ReSortPanels();
            }
            else
            {
                Debug.LogWarning(string.Format("UISortMgr中不包含 {0},MoveToTop-UI面板失败！", ui.Name));
            }
        }

        /// <summary>
        /// 重排UI界面
        /// 根据UI的打开先后顺序先赋值index，然后根据uiDepthLayer\moveTop\index三者权重进行UI重排
        /// </summary>
        public void ReSortPanels()
        {
            for (int i = 0; i < uiSortList.Count; i++)
            {
                uiSortList[i].index = i;
            }
            uiSortList.Sort((x, y) =>
            {
                if (x.ui.uiDepthLayer != y.ui.uiDepthLayer)
                {
                    return x.ui.uiDepthLayer.CompareTo(y.ui.uiDepthLayer);
                }

                if (x.moveTop != y.moveTop)
                {
                    return x.moveTop.CompareTo(y.moveTop);
                }

                return x.index.CompareTo(y.index);
            });

            int _index = this.minSortIndex;
            int _sortIndex = -1;
            int space3D = 0;

            for (int i = 0; i < uiSortList.Count; i++)
            {
                uiSortList[i].moveTop = 0; //重置moveTop标志位
                if (_index > maxSortIndex)
                {
                    _index = maxSortIndex;
                }
                _index = SortIndexSetter(uiSortList[i].ui.Panel, _index);
                _sortIndex = SortTagIndexSetter(uiSortList[i].ui.Panel, _sortIndex);
                if (is3DHigher)
                {
                    space3D = SortTag3DSetter(uiSortList[i].ui.Panel, space3D, true);
                }
            }

            if (!is3DHigher)
            {
                for (int i = uiSortList.Count - 1; i >= 0; i--)
                {
                    space3D = SortTag3DSetter(uiSortList[i].ui.Panel, space3D, false);
                }
            }

        }

        /// <summary>
        /// 添加打开面板时调用，会重排UI
        /// </summary>
        /// <param name="ui"></param>
        public void AddPanel(UIBase ui)
        {
            if (null == ui)
            {
                Debug.LogWarning("添加到UISortMgr中的ui不能为空！");
                return;
            }

            int index;
            if (IsContainSorter(ui, out index))
            {
                Debug.LogWarning(string.Format("{0}已经在uiSortList中添加过了！", ui.Name));
                return;
            }
            uiSortList.Add(new UISorter(ui, 0, 0));
            ReSortPanels();
        }

        /// <summary>
        /// 移除关闭面板时调用，会重排UI
        /// </summary>
        /// <param name="ui"></param>
        public void RemovePanel(UIBase ui)
        {
            if (null == ui)
            {
                Debug.LogWarning("UISortMgr中待移除的ui不能为空！");
                return;
            }

            int index;
            if (!IsContainSorter(ui, out index))
            {
                Debug.LogWarning(string.Format("UISortMgr中不包含 {0},移除UI面板失败！", ui.Name));
                return;
            }
            uiSortList.RemoveAt(index);
            ReSortPanels();
        }

        /// <summary>
        /// 判断一个ui是否在uiSortList中，并返回索引
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        private bool IsContainSorter(UIBase ui, out int index)
        {
            if (null != uiSortList)
            {
                for (int i = 0; i < uiSortList.Count; i++)
                {
                    if (uiSortList[i].ui == ui)
                    {
                        index = i;
                        return true;
                    }
                }
            }
            index = -1;
            return false;
        }
    }
}

