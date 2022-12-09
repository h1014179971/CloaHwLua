using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace PSDImporter
{
    public struct PngData
    {
        public string groupName;
        public string pngName;
        public string pngType;//image(图片),text（文本）
        public string contents;//text 描叙
        public int id;
        public int index;
        public float x;
        public float y;
        public float width;
        public float height;
        public string rgb; //text
        public float opacity;//text 不透明度
        public int size;//text 大小
        public override string ToString()
        {
            return $"groupName:{groupName} \n pngName:{pngName} \n id:{id} \n index:{index} \n x:{x} \n y:{y} \n width:{width} \n height:{height}";
        }
    }
    public class PSDReadJson
    {
        public static List<PngData> ReadJson(ref string selectionAssetFolder, ref (int width, int height) canvasWH)
        {
            List<PngData> listpngDatas = new List<PngData>();
            var obj = Selection.activeObject;
            if (obj != null)
            {
                //string jsonpath =  Application.dataPath + AssetDatabase.GetAssetPath(obj).Replace("Assets", ""); //json路径
                string jsonpath = Path.Combine(System.Environment.CurrentDirectory, AssetDatabase.GetAssetPath(obj));
                selectionAssetFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj)) + "/"; //资源文件夹
                if (jsonpath.EndsWith(".ps.json"))
                {
                    #region 读取json
                    var jsonJo = JObject.Parse(File.ReadAllText(jsonpath));
                    JObject jopxdata = (JObject)jsonJo["data"]; //data 节点 得到宽高
                    canvasWH.width = (int)jopxdata["width"];
                    canvasWH.height = (int)jopxdata["height"];

                    JArray jasort = (JArray)jsonJo["sort"]; //找到sort 节点
                    List<(string pngName, int id, int index)> listSort = new List<(string pngName, int id, int index)>(); //存到集合中
                    foreach (JObject item in jasort)
                    {
                        string str = (string)item["name"];
                        int id = (int)item["id"];
                        int index = (int)item["index"];
                        listSort.Add((str, id, index));
                    }

                    JObject jp = (JObject)jsonJo["pngdata"]; //找到pngdata 节点
                    foreach (JProperty group in jp.Children())  //遍历图层
                    {
                        foreach (var pngdata in group.Value) //遍历所有图片
                        {
                            JObject jodata = (JObject)pngdata;
                            var data = new PngData();
                            data.groupName = group.Name;
                            data.pngName = (string)jodata["pngname"];
                            data.pngType = (string)jodata["pngType"];
                            data.contents = (string)jodata["contents"];
                            data.id = (int)jodata["id"];
                            data.index = listSort.Find((item) => (item.pngName == data.pngName && item.id == data.id)).index;
                            data.x = (float)jodata["x"];
                            data.y = (float)jodata["y"];
                            data.width = (float)jodata["width"];
                            data.height = (float)jodata["height"];
                            data.rgb = (string)jodata["rgb"];
                            data.opacity = (float)jodata["opacity"];
                            data.size = (int)jodata["size"];
                            listpngDatas.Add(data);
                        }
                    }
                    #endregion

                    //按照index 索引 顺序排序 0 1 2... 
                    listpngDatas.Sort((a, b) =>
                    {
                        if (a.index > b.index) return 1;
                        else if (a.index < b.index) return -1;
                        return 0;
                    });
                }
                else
                    Debug.LogError("该文件不是 .ps.json");
            }
            else 
                Debug.LogError("请选择 .ps.json 文件");
            return listpngDatas;
        }
    }
}
