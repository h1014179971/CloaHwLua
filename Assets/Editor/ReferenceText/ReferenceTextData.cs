using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace WJYFramework.Editor
{
    public class ReferenceTextData 
    {
        //资源引用信息字典（prefab）
        public Dictionary<string, AssetDescription> assetDict = new Dictionary<string, AssetDescription>();
        public LocalELangLibrary langlibrary = LocalELangLibrary.ZH_CN;
        private List<LocalELangLibrary> checkLangs = new List<LocalELangLibrary>();
        
        public void CollectInfo()
        {
            assetDict.Clear();
            int langid = PlayerPrefs.GetInt("langlibrary", 1);
            langlibrary = (LocalELangLibrary)langid;
            CheckAllLang();
        }

        //生成并加入引用信息
        public void ImportAsset(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            //获取该资源的最后修改时间，用于之后的修改判断
            Hash128 assetDependencyHash = AssetDatabase.GetAssetDependencyHash(path);
            //如果assetDict没包含该guid或包含了修改时间不一样则需要更新
            if (!assetDict.ContainsKey(guid))
            {
                //生成asset依赖信息，被引用需要在所有的asset依赖信息生成完后才能生成
                AssetDescription ad = new AssetDescription();
                ad.name = Path.GetFileNameWithoutExtension(path);
                ad.path = path;
                ad.type = AssetDatabase.GetMainAssetTypeAtPath(path).ToString();
                ad.assetDependencyHash = assetDependencyHash;
                if (assetDict.ContainsKey(guid))
                    assetDict[guid] = ad;
                else
                    assetDict.Add(guid, ad);
                GameObject selectObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                CheckOutText(path, selectObj,ad.txtDescDict);

                //Text[] txts = selectObj.GetComponentsInChildren<Text>();
                //ChangeLang(txts,path,selectObj,ad.txtDescDict);
            }
        }
        private void CheckAllLang()
        {
            checkLangs.Clear();
            foreach (LocalELangLibrary eLang in Enum.GetValues(typeof(LocalELangLibrary)))
            {
                if((langlibrary & eLang) != 0)
                {
                    checkLangs.Add(eLang);
                }
            }
        }
        /// <summary>
        /// 遍历对象中的Text
        /// </summary>
        /// <param name="obj"></param>
        private void CheckOutText(string parentPath, GameObject obj, Dictionary<string, List<TextDescription>> txtDescDict)
        {
            
            Text[] txts = obj.GetComponentsInChildren<Text>();
            for (int j = 0; j < checkLangs.Count; j++)
            {
                string langName = checkLangs[j].ToString();
                LangService.Instance.LangType = (ELangType)(Enum.Parse(typeof(ELangType), langName));
                for (int i = 0; i < txts.Length; i++)
                {

                    Text textComp = txts[i];
                    //textComp.OnRebuildRequested();
                    Selection.activeGameObject = textComp.gameObject;



                    if (textComp is WJYFramework.UI.LangText)
                    {
                        textComp.text = LangService.Instance.Get((textComp as WJYFramework.UI.LangText).langTextInfo.key);
                    }
                    else
                    {
                        UILangText uiLangTxt = textComp.GetComponent<UILangText>();
                        if (uiLangTxt != null)
                            textComp.text = LangService.Instance.Get(uiLangTxt.Key);
                    }
                    string textPath = textComp.transform.name;
                    GetObjPath(textComp.gameObject, obj, ref textPath);
                    int visibleLen = textComp.cachedTextGenerator.characterCountVisible; //在检测多语言切换的时候，characterCountVisible有概率显示上一个文本的数量，举例：中简-“关卡”,英文-“LEVEL” ,有概率显示2
                    //int visibleLen = textComp.cachedTextGenerator.characterCount;
                    int len = textComp.text.Length;
                    TextGenerationSettings settings = textComp.GetGenerationSettings(textComp.rectTransform.rect.size);
                    bool withError = textComp.cachedTextGenerator.PopulateWithErrors(textComp.text, settings, textComp.gameObject);
                    //if (visibleLen < len - 1)
                    //    Debug.LogError($"{textPath} 超框,实际显示字数：{visibleLen},原有字数：{len},文本:{textComp.text},withError:{withError}");
                    List<TextDescription> txtDescs;
                    if (!txtDescDict.TryGetValue(langName, out txtDescs))
                    {
                        txtDescs = new List<TextDescription>();
                        txtDescDict[langName] = txtDescs;
                    }

                    TextDescription td = new TextDescription();
                    td.name = textComp.transform.name;
                    td.parentPath = parentPath;
                    td.path = textPath;
                    if (textComp.GetComponent<UILangText>())
                        td.type = "UILangText";
                    else if (textComp is WJYFramework.UI.LangText)
                        td.type = "LangText";
                    else
                        td.type = "Text";
                    td.lang = langName;
                    td.txt = textComp.text;
                    if (visibleLen < len)
                    {
                        if (textComp.GetComponent<ContentSizeFitter>())
                            td.desc = $"超框:添加有“ContentSizeFitter”组件；实际显示字数：{visibleLen},原有字数：{len}";
                        else if (textComp.text.Contains("<color"))
                            td.desc = $"超框:富文本；实际显示字数：{visibleLen},原有字数：{len}";
                        else
                            td.desc = $"超框:实际显示字数：{visibleLen},原有字数：{len}";
                        txtDescs.Add(td);
                    }
                    if (string.IsNullOrEmpty(textComp.text) /*|| visibleLen < len - 1*/)
                    {


                        if (string.IsNullOrEmpty(textComp.text))
                            td.desc = "空";
                        //else
                        //    td.desc = $"超框-实际显示字数：{visibleLen+1},原有字数：{len}";
                        txtDescs.Add(td);
                        continue;
                    }



                    //RectTransform rectTransform = textComp.GetComponent<RectTransform>();
                    //Vector2 size = rectTransform.rect.size;
                    //UILineInfo[] lines = textComp.cachedTextGenerator.GetLinesArray();
                    ////最后一行其实文字索引
                    //if (lines.Length <= 0) continue;
                    //UILineInfo lineInfo = lines[lines.Length - 1];
                    //int lastLinestartCharIdx = lines[lines.Length - 1].startCharIdx;
                    //Font font = textComp.font;
                    //if (font != null)
                    //{
                    //    font.RequestCharactersInTexture(textComp.text, textComp.fontSize, FontStyle.Normal);
                    //    CharacterInfo characterInfo;
                    //    float width = 0f;
                    //    float height = 0;
                    //    for (int n = 0; n < textComp.text.Length; n++)
                    //    {
                    //        font.GetCharacterInfo(textComp.text[n], out characterInfo, textComp.fontSize);
                    //        width += characterInfo.advance;
                    //        if(width > size.x)
                    //        {
                    //            width = characterInfo.advance;
                    //            height += lineInfo.height;
                    //        }
                    //    }
                    //    if (height > size.y)
                    //    {
                    //        if (textComp.GetComponent<ContentSizeFitter>())
                    //            td.desc = $"超框:添加有“ContentSizeFitter”组件；文本框大小:{size},最后一行宽度：{width}";
                    //        else if (textComp.text.Contains("<color"))
                    //            td.desc = $"超框:富文本；文本框大小:{size},最后一行宽度：{width}";
                    //        else
                    //            td.desc = $"超框:文本框大小:{size},最后一行宽度：{width}";
                    //        txtDescs.Add(td);
                    //    }

                    //}
                    //else
                    //{
                    //    td.desc = "字体missing";
                    //    txtDescs.Add(td);
                    //}
                    


                }
            }  
        }
        #region 测试
        int langTypeIndex = -1;
        string langName;
        private void ChangeLang(Text[] txts, string parentPath, GameObject obj, Dictionary<string, List<TextDescription>> txtDescDict)
        {
            langTypeIndex++;
            if (langTypeIndex >= checkLangs.Count)
            {
                langTypeIndex = -1;
                return;
            }
            langName = checkLangs[langTypeIndex].ToString();
            LangService.Instance.LangType = (ELangType)(Enum.Parse(typeof(ELangType), langName));
            CheckTextAsync(txts,parentPath,obj,txtDescDict);
        }
        int txtIndex = -1;
        private async void CheckTextAsync(Text[] txts, string parentPath, GameObject obj, Dictionary<string, List<TextDescription>> txtDescDict)
        {
            txtIndex++;
            if(txtIndex >= txts.Length)
            {
                txtIndex = -1;
                ChangeLang(txts,parentPath,obj, txtDescDict);
                return;
            }
            Text textComp = txts[txtIndex];
            if (textComp is WJYFramework.UI.LangText)
            {
                textComp.text = LangService.Instance.Get((textComp as WJYFramework.UI.LangText).langTextInfo.key);
            }
            else
            {
                UILangText uiLangTxt = textComp.GetComponent<UILangText>();
                if (uiLangTxt != null)
                    textComp.text = LangService.Instance.Get(uiLangTxt.Key);
            }
            await Task.Delay(System.TimeSpan.FromSeconds(1f));
            Selection.activeGameObject = textComp.gameObject;
            
            string textPath = textComp.transform.name;
            GetObjPath(textComp.gameObject, obj, ref textPath);
            int visibleLen = textComp.cachedTextGenerator.characterCountVisible;
            //int visibleLen = textComp.cachedTextGenerator.characterCount;
            int len = textComp.text.Length;
            TextGenerationSettings settings = textComp.GetGenerationSettings(textComp.rectTransform.rect.size);
            bool withError = textComp.cachedTextGenerator.PopulateWithErrors(textComp.text, settings, textComp.gameObject);
            if (visibleLen < len - 1)
                Debug.LogError($"{textPath} 超框,实际显示字数：{visibleLen},原有字数：{len},文本:{textComp.text},withError:{withError}");
            List<TextDescription> txtDescs;
            if (!txtDescDict.TryGetValue(langName, out txtDescs))
            {
                txtDescs = new List<TextDescription>();
                txtDescDict[langName] = txtDescs;
            }

            TextDescription td = new TextDescription();
            td.name = textComp.transform.name;
            td.parentPath = parentPath;
            td.path = textPath;
            if (textComp.GetComponent<UILangText>())
                td.type = "UILangText";
            else if (textComp is WJYFramework.UI.LangText)
                td.type = "LangText";
            else
                td.type = "Text";
            td.lang = langName;
            td.txt = textComp.text;
            if (visibleLen < len)
            {
                if (textComp.GetComponent<ContentSizeFitter>())
                    td.desc = $"超框:添加有“ContentSizeFitter”组件；实际显示字数：{visibleLen},原有字数：{len}";
                else if (textComp.text.Contains("<color"))
                    td.desc = $"超框:富文本；实际显示字数：{visibleLen},原有字数：{len}";
                else
                    td.desc = $"超框:实际显示字数：{visibleLen},原有字数：{len}";
                txtDescs.Add(td);
            }



            RectTransform rectTransform = textComp.GetComponent<RectTransform>();
            Vector2 size = rectTransform.rect.size;
            UILineInfo[] lines = textComp.cachedTextGenerator.GetLinesArray();
            //最后一行其实文字索引
            int lastLinestartCharIdx = lines[lines.Length - 1].startCharIdx;
            UILineInfo lineInfo = lines[lines.Length - 1];
            Font font = textComp.font;
            if (font != null)
            {
                font.RequestCharactersInTexture(textComp.text, textComp.fontSize, FontStyle.Normal);
                CharacterInfo characterInfo;
                float width = 0f;
                float height = 0;
                for (int n = 0; n < textComp.text.Length; n++)
                {
                    font.GetCharacterInfo(textComp.text[n], out characterInfo, textComp.fontSize);
                    width += characterInfo.advance;
                    //Debug.LogError($"advance:{characterInfo.advance},glyphhe:{characterInfo.glyphHeight},w:{characterInfo.glyphWidth}");
                    if (width > size.x)
                    {
                        width = characterInfo.advance;
                        height += lineInfo.height;
                    }
                }
                //if (height > size.y)
                //{
                    Debug.LogError($"{textPath} 超框,size：{size},height：{height}");
                    if (textComp.GetComponent<ContentSizeFitter>())
                        td.desc = $"超框:添加有“ContentSizeFitter”组件；文本框大小:{size},最后一行宽度：{width}";
                    else if (textComp.text.Contains("<color"))
                        td.desc = $"超框:富文本；文本框大小:{size},最后一行宽度：{width}";
                    else
                        td.desc = $"超框:文本框大小:{size},最后一行宽度：{width}";
                    txtDescs.Add(td);
                //}

            }
            else
            {
                td.desc = "字体missing";
                txtDescs.Add(td);
            }

            if (string.IsNullOrEmpty(textComp.text) /*|| visibleLen < len - 1*/)
            {


                if (string.IsNullOrEmpty(textComp.text))
                    td.desc = "空";
                //else
                //    td.desc = $"超框-实际显示字数：{visibleLen+1},原有字数：{len}";
                txtDescs.Add(td);
            }
            CheckTextAsync(txts, parentPath, obj, txtDescDict);
        }
        #endregion
        private static void GetObjPath(GameObject curObj, GameObject selectionObj, ref string namePath)
        {
            if (string.IsNullOrEmpty(namePath))
                namePath = curObj.name;
            try
            {
                if (curObj.transform.parent != selectionObj.transform)
                {
                    namePath = $"{curObj.transform.parent.name}/{namePath}";
                    if (curObj.transform.parent != selectionObj.transform)
                    {
                        curObj = curObj.transform.parent.gameObject;
                        GetObjPath(curObj, selectionObj, ref namePath);
                    }
                }


            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{ex.Message}");
            }

        }

        //更新引用信息状态
        public void UpdateAssetState(string guid)
        {
            AssetDescription ad;

            //字典中没有该数据
            if (!assetDict.TryGetValue(guid, out ad))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ad = new AssetDescription();
                ad.name = Path.GetFileNameWithoutExtension(path);
                ad.path = path;
                ad.type = AssetDatabase.GetMainAssetTypeAtPath(path).ToString();
                assetDict.Add(guid, ad);
            }
        }
        /// <summary>
        /// prefab 信息
        /// </summary>
        public class AssetDescription
        {
            public string name = "";
            public string path = "";
            public string type = "";
            public Hash128 assetDependencyHash;
            //<语言，多种语言超框信息集合>
            public Dictionary<string, List<TextDescription>> txtDescDict = new Dictionary<string, List<TextDescription>>();
            
        }
        /// <summary>
        /// Text文本信息
        /// </summary>
        public class TextDescription
        {
            public string name = "";
            public string parentPath = "";
            public string path = "";
            public string type = "";
            public string lang = "";
            public string txt = "";
            public string desc = "";
        }
    }
}

