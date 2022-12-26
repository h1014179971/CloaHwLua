using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEditor.Callbacks;

namespace PSDImporter
{
    public class PSDCreateor /*:AssetModificationProcessor*/ 
    {
        static (int width, int height) canvasWH; //画布宽高
        static List<PngData> listpngDatas;
        static string selectionAssetFolder;

        [MenuItem("Assets/PSDTools/PSD2Scene", priority = -100, validate = false)]
        static void PSDToScene()
        {
            listpngDatas = PSDReadJson.ReadJson(ref selectionAssetFolder, ref canvasWH);
            if (listpngDatas.Count > 0)
                CreateScene();
        }
        [MenuItem("Assets/PSDTools/PSD2UGUI", priority = -99, validate = false)]
        static void PSDToUGUI()
        {
            listpngDatas = PSDReadJson.ReadJson(ref selectionAssetFolder, ref canvasWH);
            if (listpngDatas.Count > 0)
                CreateUGUI();
        }
        [MenuItem("Assets/PSDTools/PSD2Prefab",priority = - 98,validate = false)]
        static void PSDToUIView()
        {
            listpngDatas = PSDReadJson.ReadJson(ref selectionAssetFolder, ref canvasWH);
            if (listpngDatas.Count > 0)
                CreateUGUI(true);
        }

        /// <summary>
        /// 创建场景
        /// </summary>
        private static void CreateScene()
        {
            //父对象
            var rootTrans = CreateGo<Transform>(new DirectoryInfo(selectionAssetFolder).Name, null);
            rootTrans.transform.position = new Vector3(canvasWH.width * 0.01f * 0.5f, canvasWH.height * 0.01f * 0.5f, 0f);
            int count = listpngDatas.Count - 1;
            foreach (var item in listpngDatas)
            {
                string group = item.groupName == "root" ? string.Empty : item.groupName; //有没有图层组
                string pngpath = $"{selectionAssetFolder}{group}/{item.pngName}.png";  //图片路径
                if (item.pngType.Contains("image"))
                {
                    SpriteRenderer sr = null;
                    if (!string.IsNullOrEmpty(group))
                    {
                        if (rootTrans.transform.Find(group) == null)
                        {
                            var go = CreateGo<Transform>(group, rootTrans);
                            go.transform.localPosition = Vector3.zero;
                            go.SetAsFirstSibling();
                        }
                        sr = CreateGo<SpriteRenderer>(item.pngName, rootTrans.transform.Find(group));
                    }
                    else
                    {
                        sr = CreateGo<SpriteRenderer>(item.pngName, rootTrans);
                    }
                    sr.transform.SetAsFirstSibling();
                    sr.transform.position = new Vector3(item.x * 0.01f, item.y * 0.01f, 0);
                    //倒序排列
                    sr.sortingOrder = count - item.index;

                    var sp = (Sprite)AssetDatabase.LoadAssetAtPath(pngpath, typeof(Sprite));
                    if (sp != null)
                        sr.sprite = sp;
                    else
                        Debug.LogError($"not found sprite at: {pngpath}");
                }
                
            }
            rootTrans.transform.position = Vector3.zero;  //归0
            CreatePrefab(rootTrans.gameObject, rootTrans.gameObject.name);
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        private static void CreateUGUI(bool toPrefab = false)
        {
            Font txtFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameAssets/fonts/Alibaba.ttf");
            Transform canvasTrans = null;
            if (GameObject.FindObjectOfType<Canvas>() == null)
            {
                //创建canvas
                bool menuitem = EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
                if (menuitem == false)
                    canvasTrans = CreateGo<Canvas>("Canvas", null).transform;
            }
            if (canvasTrans == null)
                canvasTrans = GameObject.FindObjectOfType<Canvas>().transform;
            var rootRectTrans = CreateGo<RectTransform>(new DirectoryInfo(selectionAssetFolder).Name, canvasTrans);
            rootRectTrans.position = new Vector3(canvasWH.width * 0.5f, canvasWH.height * 0.5f, 0);
            rootRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasWH.width);
            rootRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasWH.height);

            RectTransform parentRectTrans = rootRectTrans;
            //UGUI 排序 谁在排序靠下 优先显示
            for (int i = listpngDatas.Count - 1; i >= 0; i--)
            {
                parentRectTrans = rootRectTrans;
                PngData item = listpngDatas[i];
                string group = item.groupName == "root" ? string.Empty : item.groupName; //组
                if (!string.IsNullOrEmpty(group))
                {
                    if (rootRectTrans.transform.Find(group) == null)
                    {
                        var go = CreateGo<RectTransform>(group, rootRectTrans);
                        go.localPosition = Vector3.zero;
                    }
                    parentRectTrans = (RectTransform)rootRectTrans.transform.Find(group);
                }
                if (item.pngType.Contains("image"))
                {
                    #region  create image
                    string pngpath = $"{selectionAssetFolder}{group}/{item.pngName}.png";
                    //Image img = null;
                    //if (item.pngName.Contains("btn"))
                    //{
                    //    Button btn = CreateGo<Button>(item.pngName, parentRectTrans);
                    //    img = btn.image;
                    //} 
                    //else
                    //    img = CreateGo<Image>(item.pngName, parentRectTrans);//TODO:可以根据item.pngName 自行添加其他UI组件
                    Image img = CreateGo<Image>(item.pngName, parentRectTrans);
                    var sp = (Sprite)AssetDatabase.LoadAssetAtPath(pngpath, typeof(Sprite));
                    if (sp != null)
                    {
                        img.sprite = sp;
                        img.SetNativeSize();
                    }
                    else
                        Debug.LogError($"not found sprite at: {pngpath}");
                    img.rectTransform.position = new Vector3(item.x, item.y, 0);
                    if (item.pngName.Contains("btn"))
                    {
                        img.gameObject.AddComponent<Button>();
                    }
                    #endregion
                }
                else if (item.pngType.Contains("text"))
                {
                    #region create text
                    Text txt = CreateGo<Text>(item.pngName, parentRectTrans);//TODO:可以根据item.pngName 自行添加其他UI组件
                    Color txtColor = HexToColor(item.rgb);
                    txtColor.a = item.opacity;
                    txt.text = item.contents;
                    txt.rectTransform.sizeDelta = new Vector2(item.width,item.height);
                    txt.rectTransform.position = new Vector3(item.x, item.y, 0);
                    txt.color = txtColor;
                    if (txtFont != null)
                        txt.font = txtFont;
                    txt.fontSize = item.size;
                    txt.alignment = TextAnchor.MiddleCenter;
                    txt.horizontalOverflow = HorizontalWrapMode.Overflow;
                    txt.verticalOverflow = VerticalWrapMode.Overflow;
                    txt.raycastTarget = false;
                    #endregion
                }

            }
            rootRectTrans.transform.localPosition = Vector3.zero;
            rootRectTrans.transform.localScale = Vector3.one;
            if (toPrefab)
                CreatePrefab(rootRectTrans.gameObject, rootRectTrans.gameObject.name + "_UI");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private static Color HexToColor(string hex)
        {
            if (hex.Length <= 6)
                hex += "FF";
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }

        private static T CreateGo<T>(string goName, Transform parent) where T : Component
        {
            GameObject go = new GameObject(goName);
            go.transform.SetParent(parent);

            if (typeof(T) != typeof(Transform))
            {
                T t = go.AddComponent<T>();
                return t;
            }
            else
                return go.transform as T;
        }

        private static void CreatePrefab(GameObject prefab, string goname)
        {
            var go = PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, selectionAssetFolder + goname + ".prefab", InteractionMode.AutomatedAction);
            EditorGUIUtility.PingObject(go);
        }

        //资源保存时(包括代码)会调用此函数,必须是静态函数
        //public static void OnWillSaveAssets(string[] names)
        //{
        //    if (_rootRectTrans != null)
        //    {
        //        UIViewController ViewController = _rootRectTrans.gameObject.GetComponent<UIViewController>();
        //        if (string.IsNullOrEmpty(ViewController.ScriptsFolder))
        //        {
        //            var setting = UICodeGenKitSetting.Load();
        //            ViewController.ScriptsFolder = setting.ScriptDir;
        //        }

        //        if (string.IsNullOrEmpty(ViewController.PrefabFolder))
        //        {
        //            var setting = UICodeGenKitSetting.Load();
        //            ViewController.PrefabFolder = setting.PrefabDir;
        //        }

        //        if (string.IsNullOrEmpty(ViewController.ScriptName))
        //        {
        //            ViewController.ScriptName = ViewController.name;
        //        }

        //        if (string.IsNullOrEmpty(ViewController.Namespace))
        //        {
        //            var setting = UICodeGenKitSetting.Load();
        //            ViewController.Namespace = setting.Namespace;
        //        }
        //        _rootRectTrans = null;
        //        UICodeGenKit.Generate(ViewController);
        //    }
        //    else
        //        UICodeGenKitPipeline.Default.OnCompile();
        //}


    }
}
