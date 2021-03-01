using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using CYUtils;
public class CheckMissNode : Editor
{

    [MenuItem("Assets/删除场景中挂载的废弃脚本和丢失节点", false, 4003)]
    public static void CheckMissScene()
    {
        List<string> fileList = new List<string>();
        string[] strs = Selection.assetGUIDs;
        string subPath = AssetDatabase.GUIDToAssetPath(strs[0]);
        string path = System.IO.Path.Combine(Application.dataPath, subPath);
        string file = path.Replace(Application.dataPath, "Assets");
        EditorApplication.OpenScene(file);
        //object[] scene = AssetDatabase.LoadAllAssetsAtPath(file);
        foreach (Transform node in Resources.FindObjectsOfTypeAll(typeof(Transform)))
        {
            /*if (node == null)
            {
                continue;
            }*/
            FindMissionNodeAndScript(node.gameObject, file);
        }
        EditorApplication.SaveScene();

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        Debug.Log("删除废弃节点和查找丢失脚本的场景完成，请保存");
    }
    private static void FindMissionNodeAndScript(GameObject node, string path)
    {
        if (node == null)
            return;
        MonoBehaviour[] components = node.GetComponents<MonoBehaviour>();
        Transform[] tran = node.GetComponents<Transform>();
        MonoBehaviour c = null;
        /*var components = node.GetComponents<Component>();
        var serializedObject = new SerializedObject(node);
        var prop = serializedObject.FindProperty("m_Component");
        int r=0;*/
        if (PrefabUtility.GetPrefabType(node) == PrefabType.MissingPrefabInstance)
        {
            // Debug.Log("删除删除丢失预设的节点：" + node.name + "场景名称：" + path);
            DeleteNode(node, path);
        }
        if (node == null)
            return;
        for (int i = 0; i < components.Length; i++)
        {
            c = components[i];
            if (components[i] == null)
            {
                if (path != null)
                {
                    Debug.LogError("----------------------丢失组件的节点:" + node.name + "-->场景路径" + path);
                    continue;
                }
            }
        }
        //serializedObject.ApplyModifiedProperties()

       /* foreach (Transform t in node.transform)
        {
            FindMissionNodeAndScript(t.gameObject, path);
        }*/
    }
    private static void DeleteNode(GameObject obj, string path)
    {
        if (obj == null)
            return;
        PrefabUtility.DisconnectPrefabInstance(obj);
        Undo.DestroyObjectImmediate(obj);
    }
}
