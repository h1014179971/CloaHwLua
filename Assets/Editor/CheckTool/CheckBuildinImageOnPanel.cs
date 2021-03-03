using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using CYUtils;
using UnityEngine.UI;

public class CheckBuildinImageOnPanel : MonoBehaviour {

    static private string[] buildInImageNames = new string[] { "Background", "Checkmark", "InputFieldBackground", "Knob", "UISprite" };

    [MenuItem("Assets/修改预设中Image设置内置图片", false, 4011)]
    public static void RemoveBuildInImage()
    {
        //递归获取项目中所有的材质球
        List<string> fileLst = new List<string>();
        string subPath = "XNRes/AssetBundle/Prefabs/UI/";
        string path = System.IO.Path.Combine(Application.dataPath, subPath);

        FileUtils.searchAllFiles(path, fileLst, new List<string> { ".prefab" });
        foreach (string s in fileLst)
        {
            string file = s.Replace(Application.dataPath, "Assets");
            GameObject go = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;
            List<Image> images = new List<Image>();
            GetImageComponent(go.transform, ref images);
            for (int i = 0; i < images.Count; i++)
            {
                Image image = images[i];
                if (image.sprite != null)
                {
                    bool isNeedChange = false;
                    foreach (string name in buildInImageNames)
                    {
                        if(image.sprite.name == name)
                        {
                            image.sprite = null;
                            isNeedChange = true;
                            Debug.Log("路径： "+ s + "\n节点名： "+ images[i].transform.name, AssetDatabase.LoadAssetAtPath(GetRelativeAssetsPath(s), typeof(Object)));
                            break;
                        }
                    }
                    if (isNeedChange) EditorUtility.SetDirty(go);
                }
            }
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        Debug.Log("修改完毕！");
    }

    private static void GetImageComponent(Transform trans, ref List<Image> images)
    {
        Image image = trans.GetComponent<Image>();
        if (image != null)
            images.Add(image);
        for (int i = 0; i < trans.childCount; i++)
        {
            GetImageComponent(trans.GetChild(i), ref images);
        }
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + System.IO.Path.GetFullPath(path).Replace(System.IO.Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
}
