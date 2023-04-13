using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace WJYFramework.Editor
{
    public class AssetViewItem : TreeViewItem
    {
        public ReferenceTextData.AssetDescription data;
        public ReferenceTextData.TextDescription txtData;
    }
    public class AssetTreeView : TreeView
    {
        //图标宽度
        const float kIconWidth = 18f;
        //列表高度
        const float kRowHeights = 20f;
        public AssetViewItem assetRoot;

        private GUIStyle stateGUIStyle = new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter };
        enum MyColumns
        {
            Name,
            Path,
            Type,
            Lang,
            Txt,
            Desc,
        }
        protected override TreeViewItem BuildRoot()
        {
            return assetRoot;
        }
        public AssetTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader) : base(state, multicolumnHeader)
        {
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = false;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kIconWidth;
        }
        //响应双击事件
        protected override void DoubleClickedItem(int id)
        {
            var item = (AssetViewItem)FindItem(id, rootItem);
            //在ProjectWindow中高亮双击资源
            if (item != null)
            {
                if(item.data != null)
                {
                     OpenPrefab(item.data.path);
                }
                else if(item.txtData != null)
                {
                    OpenPrefab(item.txtData.parentPath, item.txtData.path);
                    //SelectionChildByPath(item.txtData.path);
                }
            }
        }
        private Transform _activeTrans;
        private void SelectionChildByPath(string path,Transform openPrefabTrans)
        {
            _activeTrans = openPrefabTrans;
            //Transform activeTrans = Selection.activeTransform;
            Transform t = openPrefabTrans.Find(path);
            if (t == null)
            {
                Debug.LogError($"{path} 找不到对象");
                return;
            }
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            Selection.activeGameObject = t.gameObject;
            EditorGUIUtility.PingObject(t.gameObject);

        }

        private GameObject _lastPrefabObj;
        private void OpenPrefab(string path,string childPath = null)
        {
            var assetObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            //焦点在project窗口
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = assetObject;
            EditorGUIUtility.PingObject(assetObject);
            if(_lastPrefabObj != assetObject)
            {
                AssetDatabase.OpenAsset(assetObject);
                _lastPrefabObj = assetObject;
                if (!string.IsNullOrEmpty(childPath))
                    SelectionChildByPath(childPath, Selection.activeTransform);
            }
            else
            {
                if(!string.IsNullOrEmpty(childPath))
                    SelectionChildByPath(childPath, _activeTrans);
            }
            
        }
        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                //图标+名称
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("名字"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 140,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = false
                },
                //路径
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("路径"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 360,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = false
                },  
                //资源类型
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("类型"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 200,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true,
                    canSort = false
                },
                //语言
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("语言"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 60,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true,
                    canSort = false
                },
                //文本
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("文本"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 300,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true,
                    canSort = false
                },
                //备注
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("备注"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 300,
                    minWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true,
                    canSort = false
                },
            };
            var state = new MultiColumnHeaderState(columns);
            return state;
        }
        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (AssetViewItem)args.item;
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
            }
        }

        //绘制列表中的每项内容
        void CellGUI(Rect cellRect, AssetViewItem item, MyColumns column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case MyColumns.Name:
                    {
                        var iconRect = cellRect;
                        iconRect.x += GetContentIndent(item);
                        iconRect.width = kIconWidth;
                        if (item.data != null)
                        {
                            
                            if (iconRect.x < cellRect.xMax)
                            {
                                var icon = GetIcon(item.data.path);
                                if (icon != null)
                                    GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
                            }
                            base.RowGUI(args);
                        }
                        else if (item.txtData != null)
                        {
                            GUI.Label(cellRect, $"             {item.txtData.name}" );
                        }
                        args.rowRect = cellRect;
                        

                    }
                    break;
                case MyColumns.Path:
                    {
                        string path = "";
                        if (item.data != null)
                            path = item.data.path;
                        else if (item.txtData != null)
                            path = $"             {item.txtData.path}";
                        GUI.Label(cellRect, path);
                    }
                    break;
                case MyColumns.Type:
                    {
                        string type = "";
                        if (item.data != null)
                            type = item.data.type;
                        else if (item.txtData != null)
                            type = $"             {item.txtData.type}";
                        GUI.Label(cellRect, type);
                    }
                    break;
                case MyColumns.Lang:
                    {
                        if(item.txtData != null)
                            GUI.Label(cellRect, item.txtData.lang);
                    }
                    break;
                case MyColumns.Txt:
                    {
                        cellRect.width += 100;
                        if (item.txtData != null)
                            GUI.Label(cellRect, item.txtData.txt);
                    }
                    break;
                case MyColumns.Desc:
                    {
                        cellRect.width += 100;
                        if (item.txtData != null)
                            GUI.Label(cellRect, item.txtData.desc);
                    }
                    break;
                default:
                    break;
            }
        }

        //根据资源信息获取资源图标
        private Texture2D GetIcon(string path)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (obj != null)
            {
                Texture2D icon = AssetPreview.GetMiniThumbnail(obj);
                if (icon == null)
                    icon = AssetPreview.GetMiniTypeThumbnail(obj.GetType());
                return icon;
            }
            return null;
        }
    }
}

