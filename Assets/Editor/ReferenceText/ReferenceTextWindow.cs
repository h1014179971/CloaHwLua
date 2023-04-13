using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace WJYFramework.Editor
{
    public class ReferenceTextWindow : EditorWindow
    {
        private static UnityEngine.Object[] selectionObjs;
        private static UnityEngine.Object[] lastSelectionObjs;//上次选择的对象
        private bool initializedGUIStyle = false;
        //工具栏按钮样式
        private GUIStyle toolbarButtonGUIStyle;
        //工具栏样式
        private GUIStyle toolbarGUIStyle;
        private static ReferenceTextData data = new ReferenceTextData();
        //选中资源列表
        private List<string> selectedAssetGuid = new List<string>();
        private AssetTreeView m_AssetTreeView;
        private TreeViewState m_TreeViewState;
        //是否需要更新资源树
        private bool needUpdateAssetTree = true;
        //查找资源引用信息
        [MenuItem("Assets/UI工具/检查超框文本信息")]
        static void FindRef()
        {
            OnInit();
            OpenWindow();
            data.CollectInfo();
            ReferenceTextWindow window = GetWindow<ReferenceTextWindow>();
            window.UpdateSelectedAssets();
        }
        [MenuItem("★工具★/UI工具/检查超框文本信息")]
        static void OpenWindow()
        {
            ReferenceTextWindow window = GetWindow<ReferenceTextWindow>();
            window.wantsMouseMove = false;
            window.titleContent = new GUIContent("Check Text");
            window.Show();
            window.Focus();
        }
        static void OnInit()
        {
            selectionObjs = Selection.objects;
            if (selectionObjs == null || selectionObjs.Length <= 0)
            {
                Debug.LogError($"未选择对象");
                return;
            }
            lastSelectionObjs = selectionObjs;
        }
        static void OnLastInit()
        {
            selectionObjs = lastSelectionObjs;
            if (selectionObjs == null || selectionObjs.Length <= 0)
            {
                Debug.LogError($"未选择对象");
                return;
            }
            lastSelectionObjs = selectionObjs;
        }
        //更新选中资源列表
        private void UpdateSelectedAssets()
        {
            if (selectionObjs == null || selectionObjs.Length <= 0) return;
            selectedAssetGuid.Clear();
            foreach (var obj in selectionObjs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                //如果是文件夹
                if (Directory.Exists(path))
                {
                    string[] folder = new string[] { path };
                    //将文件夹下所有prefab资源作为选择资源
                    string[] guids = AssetDatabase.FindAssets("t:Prefab", folder);
                    foreach (var guid in guids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        if (!selectedAssetGuid.Contains(guid) && !Directory.Exists(assetPath))
                        {
                            selectedAssetGuid.Add(guid);
                            data.ImportAsset(guid);
                        }
                    }
                }
                //如果是文件资源
                else
                {
                    if (IsPrefabInstance(obj as GameObject))
                    {
                        string guid = AssetDatabase.AssetPathToGUID(path);
                        selectedAssetGuid.Add(guid);
                        data.ImportAsset(guid);
                    }
                    
                }
            }
        }
        private static bool IsPrefabInstance(GameObject obj)
        {
            var type = PrefabUtility.GetPrefabAssetType(obj);
            if (type == PrefabAssetType.NotAPrefab)
                return false;
            return true;
        }
        //通过选中资源列表更新TreeView
        private void UpdateAssetTree()
        {
            if (needUpdateAssetTree && selectedAssetGuid.Count != 0)
            {
                var root = SelectedAssetGuidToRootItem(selectedAssetGuid);
                if (m_AssetTreeView == null)
                {
                    //初始化TreeView
                    if (m_TreeViewState == null)
                        m_TreeViewState = new TreeViewState();
                    var headerState = AssetTreeView.CreateDefaultMultiColumnHeaderState(position.width);
                    var multiColumnHeader = new MultiColumnHeader(headerState);
                    m_AssetTreeView = new AssetTreeView(m_TreeViewState, multiColumnHeader);
                }
                m_AssetTreeView.assetRoot = root;
                
                m_AssetTreeView.CollapseAll();// 重叠
                m_AssetTreeView.Reload();
                needUpdateAssetTree = false;
            }
        }
        //生成root相关
        //private HashSet<string> updatedAssetSet = new HashSet<string>();
        //通过选择资源列表生成TreeView的根节点
        private AssetViewItem SelectedAssetGuidToRootItem(List<string> selectedAssetGuid)
        {
            //updatedAssetSet.Clear();
            int elementCount = 0;
            var root = new AssetViewItem { id = elementCount, depth = -1, displayName = "Root", data = null };
            int depth = 0;
            foreach (var childGuid in selectedAssetGuid)
            {
                root.AddChild(CreateTree(childGuid, ref elementCount, depth));
            }
            
            return root;
        }
        //通过每个节点的数据生成子节点
        private AssetViewItem CreateTree(string guid, ref int elementCount, int _depth)
        {
            //if (!updatedAssetSet.Contains(guid))
            //{
            //    data.UpdateAssetState(guid);
            //    updatedAssetSet.Add(guid);
            //}
            ++elementCount;
            var referenceData = data.assetDict[guid];
            
            var root = new AssetViewItem { id = elementCount, displayName = referenceData.name, data = referenceData, depth = _depth };

            int childCount = 0;
            foreach (var textDatas in referenceData.txtDescDict.Values)
            {
                childCount++;
                for (int i = 0; i < textDatas.Count; i++)
                {
                    int index = i;
                    var txtData = textDatas[i];
                    //root.AddChild(CreateTree1(guid, ref elementCount, _depth + childCount + index));
                    root.AddChild(CreateTextTree(ref elementCount, _depth + childCount + index, txtData));
                }
            }

            return root;
        }
        //通过每个节点的数据生成子节点
        private AssetViewItem CreateTree1(string guid, ref int elementCount, int _depth)
        {
            //if (!updatedAssetSet.Contains(guid))
            //{
            //    data.UpdateAssetState(guid);
            //    updatedAssetSet.Add(guid);
            //}
            ++elementCount;
            var referenceData = data.assetDict[guid];

            var root = new AssetViewItem { id = elementCount, displayName = referenceData.name, data = referenceData, depth = _depth };
            

            return root;
        }
        //生成子节点
        private AssetViewItem CreateTextTree(ref int elementCount, int _depth,ReferenceTextData.TextDescription txtDesc)
        {
            
            ++elementCount;
            var root = new AssetViewItem { id = elementCount, displayName = txtDesc.name, txtData = txtDesc, depth = _depth };
            
            return root;
        }
        
        private void OnGUI()
        {
            InitGUIStyleIfNeeded();
            DrawOptionBar();
            UpdateAssetTree();
            if (m_AssetTreeView != null)
            {
                //绘制Treeview
                m_AssetTreeView.OnGUI(new Rect(0, toolbarGUIStyle.fixedHeight, position.width, position.height - toolbarGUIStyle.fixedHeight));
            }
        }
        //初始化GUIStyle
        void InitGUIStyleIfNeeded()
        {
            if (!initializedGUIStyle)
            {
                toolbarButtonGUIStyle = new GUIStyle("ToolbarButton");
                toolbarGUIStyle = new GUIStyle("Toolbar");
                initializedGUIStyle = true;
            }
        }
        int count;
        
        //绘制上条
        public void DrawOptionBar()
        {
            EditorGUILayout.BeginHorizontal(toolbarGUIStyle);
            //GUILayout.Toolbar(1, new string[] { "21", "22" });
            //count = EditorGUILayout.Popup("语言:", count, new string[] { "zh", "tw" });
            //count = EditorGUILayout.MaskField("maskField:", count, new string[] { "zh", "tw" });
            GUILayout.Label("语言：",GUILayout.Width(120));
            data.langlibrary = (LocalELangLibrary)EditorGUILayout.EnumFlagsField(data.langlibrary, GUILayout.Width(120));
            if (GUILayout.Button("重新检测", toolbarButtonGUIStyle, GUILayout.Width(120)))
            {
                SaveData();
                OnLastInit();
                data.CollectInfo();
                UpdateSelectedAssets();
                needUpdateAssetTree = true;
            }
            if (GUILayout.Button("检测", toolbarButtonGUIStyle, GUILayout.Width(120)))
            {
                SaveData();
                OnInit();
                data.CollectInfo();
                UpdateSelectedAssets();
                needUpdateAssetTree = true;
            }
            
            //GUILayout.FlexibleSpace();

            //展开
            if (GUILayout.Button("展开", toolbarButtonGUIStyle, GUILayout.Width(120)))
            {
                if (m_AssetTreeView != null) m_AssetTreeView.ExpandAll();
            }
            //折叠
            if (GUILayout.Button("折叠", toolbarButtonGUIStyle, GUILayout.Width(120)))
            {
                if (m_AssetTreeView != null) m_AssetTreeView.CollapseAll();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void SaveData()
        {
            int langid = (int)data.langlibrary;
            PlayerPrefs.SetInt("langlibrary", (int)data.langlibrary);
        }
    }
}

