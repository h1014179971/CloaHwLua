using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Foundation;
using Newtonsoft.Json.Serialization;
using libx;

namespace UIFramework
{
    public class CurrentLanguageChage : UnityEvent { };
    public class LanguageCtrl : MonoSingleton<LanguageCtrl>
    {
        [SerializeField]private Language _language;
        private Dictionary<int, LanguageData> _languageDic = new Dictionary<int, LanguageData>();
        public CurrentLanguageChage CurrentLanguageChageEvent = new CurrentLanguageChage();
        public Language Language
        {
            set {
                _language = value;
                CurrentLanguageChageEvent.Invoke();
            }
            get { return _language; }
        }
        protected override void Awake()
        {
            base.Awake();

        }
        public void Init()
        {
            ReadFile();
#if !UNITY_EDITOR
      SetLanguage();
#endif
        }
        private void ReadFile()
        {
            Assets.LoadAssetAsync("LanguageData.json", typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<LanguageData> dt = FullSerializerAPI.Deserialize(typeof(List<LanguageData>), jsonStr) as List<LanguageData>;
                    _languageDic = dt.ToDictionary(key => key.Id, value => value);
                    Language = _language;
                }
                else
                    LogUtility.LogError($"LanguageData.json读取失败,error={request.error}");
            };
            //StreamFile.ReaderFile("JsonData/LanguageData.json", this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
                    
            //        List<LanguageData> dt = FullSerializerAPI.Deserialize(typeof(List<LanguageData>), jsonStr) as List<LanguageData>;
            //        _languageDic = dt.ToDictionary(key => key.Id, value => value);
            //        Language = _language;                                                  
            //    } 
            //});
        }
        private void SetLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                    Language = Language.Chinese;
                    break;
                case SystemLanguage.ChineseSimplified:
                    Language = Language.ChineseSimplified;
                    break;
                case SystemLanguage.ChineseTraditional:
                    Language = Language.ChineseTraditional;
                    break;
                case SystemLanguage.English:
                    Language = Language.English;
                    break;
                case SystemLanguage.Japanese:
                    Language = Language.Japanese;
                    break;
                case SystemLanguage.German:
                    Language = Language.German;
                    break;
                case SystemLanguage.Swedish:
                    Language = Language.Swedish;
                    break;
                case SystemLanguage.Korean:
                    Language = Language.Korean;
                    break;
                case SystemLanguage.Danish:
                    Language = Language.Danish;
                    break;
                case SystemLanguage.Thai:
                    Language = Language.Thai;
                    break;
                case SystemLanguage.Arabic:
                    Language = Language.Arabic;
                    break;
                case SystemLanguage.Portuguese:
                    Language = Language.Portuguese;
                    break;
                case SystemLanguage.French:
                    Language = Language.French;
                    break;
                case SystemLanguage.Russian:
                    Language = Language.Russian;
                    break;
                default:
                    Language = Language.English;
                    break;
            } 
        }

        public string GetLanguageById(int id)
        {                                                              
            if (!_languageDic.ContainsKey(id))
            {
                Debug.LogError($"语言表没有{id}编号");
                return "";
            }  
            LanguageData dt = _languageDic[id];
            Debug.Log($"---{_language.ToString()}");
            switch (Language)
            {
                case Language.Chinese:
                    return dt.Chinese;
                case Language.ChineseSimplified:
                    return dt.ChineseSimplified;
                case Language.ChineseTraditional:
                    return dt.ChineseTraditional;
                case Language.English:
                    return dt.English;
                case Language.Japanese:
                    return dt.Japanese;
                default:
                    return dt.English;
            }   
        } 
        public string GetLanguageById(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                LogUtility.LogError($"多语言id为空");
                return null;
            }
            return GetLanguageById(int.Parse(id));
        }


    }
}

