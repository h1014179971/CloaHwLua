using System;
using System.Collections.Generic;
using System.Linq;
using CYUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class FindNotUseImageNodePrefabTool : EditorWindow
{
    public static List<FindNode> findNodes = new List<FindNode>();
    public class FindNode
    {
        public GameObject orginObj;
        public GameObject insObj;
        public string PrefabName;
        public List<string> NodeName;
        public bool IsSpawn;
    }
    
    private static void OpenMenu()
    {
        FindNotUseImageNodePrefabTool windows = GetWindow<FindNotUseImageNodePrefabTool>();
        windows.title = "未使用的Image节点";
        windows.Show();
        windows.position = new Rect(Screen.width/4,Screen.height/4,450,650);
    }

    [MenuItem("Assets/检查未使用Image节点UI",false,4000)]
    private static void FindNotUseImageNode()
    {
        Caching.ClearCache();
        findNodes.Clear();
        string guid = Selection.assetGUIDs[0];
        string path =  AssetDatabase.GUIDToAssetPath(guid);
        List<string> fileLst = new List<string>();
        FileUtils.searchAllFiles(path, fileLst, new List<string> { ".prefab" });
        int index = 0;
        if (fileLst.Count>0)
        {
            foreach (string s in fileLst)
            {
                EditorUtility.DisplayProgressBar("查找未使用的Image节点UI",String.Format("进度：{0}/{1}",index,fileLst.Count),0);
                string file = s.Replace(Application.dataPath, "Assets");
                GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
                FindNode node = new FindNode()
                {
                    PrefabName = go.name,
                    NodeName = new List<string>(),
                    orginObj = go,
                    IsSpawn = false
                };
                CheckObjImageNodeIsUse(go,node.NodeName);
                if (node.NodeName.Count>0)
                {
                    findNodes.Add(node);
                }

                index++;
            }
            EditorUtility.ClearProgressBar();
            if (findNodes.Count>0)
            {
                OpenMenu();
            }
        }     
    }

    private void OnDisable()
    {
        for (int i = 0; i < findNodes.Count; i++)
        {
            if (findNodes[i].insObj)
            {
                DestroyImmediate(findNodes[i].insObj);
            }
        }
    }

    private static void CheckObjImageNodeIsUse(GameObject go,List<string> nodes)
    {
      
        Image[] images = go.GetComponents<Image>();
        Image temp = null;
        for (int i = 0; i < images.Length; i++)
        {
            temp = images[i];
            if (temp)
            {
                if (temp.GetComponent<Mask>())
                {
                    continue;
                }
                if (temp.sprite==null)
                {
                    nodes.Add(temp.name);
                }
            }
        }

        foreach (Transform trans in go.transform)
        {
            CheckObjImageNodeIsUse(trans.gameObject,nodes);
        }
    }

    private Vector2 curPos = Vector2.zero;
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        curPos = GUILayout.BeginScrollView(curPos,GUILayout.Width(400));
        for (int i = 0; i < findNodes.Count; i++)
        {
            ShowListNode(findNodes[i]);
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
    }

    private void ShowListNode(FindNode node)
    {
        GUI.contentColor = Color.black;
        EditorGUILayout.ObjectField(node.orginObj,typeof(GameObject));
        if (node.IsSpawn == false || node.insObj==null)
        {
            if (GUILayout.Button("生成预设"))
            {
                SpawnPrefab(node);
            }
        }
        else
        {
            if (GUILayout.Button("删除预设"))
            {
                DeletePrefab(node);
            }
        }

        if (node.IsSpawn && node.insObj!=null)
        {
            if (GUILayout.Button("保存预设"))
            {
                SavePrefab(node);
            }
        }
        GUI.contentColor = Color.red;
        GUILayout.Label(String.Format("预设名:{0}",node.PrefabName));      
        List<string> needDeleteNode = new List<string>();
        for (int i = 0; i < node.NodeName.Count; i++)
        {            
            GUI.contentColor = Color.green;
            GUILayout.BeginVertical();
            GUILayout.Label(String.Format("节点名:{0}",node.NodeName[i]));
            if (node.insObj)
            {
                GUI.contentColor = Color.white;
                if (GUILayout.Button("找到节点并复制节点名"))
                {
                    FindNodeAndSelect(node.insObj, node.NodeName[i]);
                    TextEditor te = new TextEditor();
                    te.content = new GUIContent(node.NodeName[i]);
                    te.OnFocus();
                    te.Copy();
                }
                GUI.contentColor = Color.red;
                if (GUILayout.Button("删除Image节点"))
                {
                    if (FindNodeAndDelete(node.insObj,node.NodeName[i]))
                    {
                        needDeleteNode.Add(node.NodeName[i]);
                        if (EditorUtility.DisplayDialog("删除成功", String.Format("删除节点{0}成功", node.NodeName[i]), "ok", ""))
                        {
                            EditorUtility.SetDirty(node.insObj);
                        }                    
                    }
                }
            }
            GUILayout.EndVertical();
        }

        node.NodeName = node.NodeName.Where(model => !needDeleteNode.Contains(model)).ToList();

        GUI.contentColor = Color.blue;
        GUILayout.Label("----------------------分---------------------");
        GUILayout.Label("----------------------隔---------------------");
    }

    private void SavePrefab(FindNode node)
    {
        PrefabUtility.ReplacePrefab(node.insObj, node.orginObj);
    }

    private void SpawnPrefab(FindNode node)
    {
        GameObject parent =GameObject.Find("Canvas");
        if (parent!=null)
        {
            node.insObj = PrefabUtility.InstantiatePrefab(node.orginObj) as  GameObject;  
            node.insObj.transform.SetParent(parent.transform,false);
            node.insObj.SetActive(true);
            node.IsSpawn = true;
        }
        else
        {
            Debug.LogError("需要Canvas节点");
        }
    }
    
    private void DeletePrefab(FindNode node)
    {
        GameObject.DestroyImmediate(node.insObj);
        node.insObj = null;
    }

    private void FindNodeAndSelect(GameObject go, string nodeName)
    {
        if (go == null)
        {
            return;
        }
        if (string.CompareOrdinal(go.name,nodeName)==0)
        {
            Image temp = go.GetComponent<Image>();
            if (temp)
            {
                UnityEditor.Selection.activeTransform = go.transform;
                return;
            }

            return;
        }

        foreach (Transform trans in go.transform)
        {
            FindNodeAndSelect(trans.gameObject, nodeName);
        }

    }

    private bool FindNodeAndDelete(GameObject go,string nodeName)
    {
        if (go == null)
        {
            return false;
        }
        if (string.CompareOrdinal(go.name,nodeName)==0)
        {
            Image temp = go.GetComponent<Image>();
            if (temp)
            {
                DestroyImmediate(temp);
                return true;
            }

            return false;
        }

        foreach (Transform trans in go.transform)
        {
           bool isFind = FindNodeAndDelete(trans.gameObject, nodeName);
            if (isFind)
            {
                return true;
            }
           
        }
        return false;
    }
}