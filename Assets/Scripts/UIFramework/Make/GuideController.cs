using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Foundation;

namespace UIFramework
{
    public class GuideController : MonoSingleton<GuideController>
    {
        private Transform target;
        private Transform maskTra;
        private int nowIndex;
        private string fileDir = "GuideFile/Guide";  
        private string[] nameArray;
        private List<GuideUI> guideList = new List<GuideUI>();
        private int _guideId;
        private int makeId;
        private Transform _InitParent = null;
        public void Init(Transform parent,int guideId,bool isMain = false,Transform InitParent = null)
        {
            target = parent;
            _guideId = guideId;
            makeId = guideId;
            if (isMain)
                makeId = 0;
            _InitParent = InitParent;
            nowIndex = 0;
            guideList.Clear();
            ReadGuide();
            CreateMakeGuide();                                                                             
            Next();
        } 
        void ReadGuide()
        {
            //读取进度
            //int maskId = PlayerInfo.Info.GetPlayer().playerMakeGuide;  
            string content = Resources.Load<TextAsset>(fileDir + makeId).ToString();
            nameArray = content.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string path in nameArray)
            {
                string[] str = path.Split('@');
                GameObject go = null;
                if (target != null && target.Find(str[0]) != null)
                    go = target.Find(str[0]).gameObject;
                GuideUI guideUI = new GuideUI(go, str[0]);
                guideUI.speak = str[1];
                guideUI.name = str[2];
                guideUI.nextNameUI = str[2];
                guideList.Add(guideUI);
            }
        } 
        void CreateMakeGuide()
        {            
            GameObject prefab = Resources.Load("UI/MakeGuide/MakeUI_"+ makeId, typeof(GameObject)) as GameObject;
            maskTra = GameObject.Instantiate(prefab).transform;
            if(_InitParent != null)
                maskTra.SetParent(_InitParent, true);
            else
                maskTra.SetParent(target, true);
            RectTransform rt = maskTra.GetComponent<RectTransform>();
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchoredPosition3D = Vector3.zero;
            maskTra.transform.localScale = Vector3.one; 
        }
        public GameObject Find(string name)
        {                
            Transform t = target.Find(name);
            if (t == null) return null;
            else return t.gameObject;
        }

        public void Next()
        {
            if (maskTra == null) return;
            maskTra.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            if (nowIndex < guideList.Count)
            {
                GuideUI guideUI = guideList[nowIndex];
                ShowHightLight(guideUI);
                nowIndex++;
            }
            else//加载下一个文件
            {
                //LogUtility.LogInfo("引导结束");              
                Destroy(maskTra.gameObject);
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_UIGuide));
            }
        }
        void ShowHightLight(GuideUI guide, bool checkIsClone = true)
        {   
            StartCoroutine(FindUI(guide));
        }

        void CancelHightLight(GameObject go)
        {
            Destroy(go.GetComponent<GraphicRaycaster>());
            Destroy(go.GetComponent<Canvas>());
            go.GetComponent<Button>().onClick.RemoveListener(delegate { CancelHightLight(go); });
            Next();
        } 

        IEnumerator FindUI(GuideUI guide)
        {
            //寻找目标 

            GameObject go = null;
            while (go == null)
            {
                yield return new WaitForSeconds(0.3f);
                Debug.Log("wait");
                go = guide.go;
            }
                
            //高亮
            maskTra.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            maskTra.SetAsLastSibling();
            //string parStr = str[0].Substring(0, str[0].IndexOf('/'));
            //GameObject parTra = Find(parStr);
            //int sortingOrder = parTra.GetComponent<Canvas>().sortingOrder;
            float t = 0;        
            StartCoroutine(DelaySibling(go, t));   
        }
        //延迟高亮
        IEnumerator DelaySibling(GameObject go, float t)
        {
            yield return new WaitForSeconds(t);
            go.AddComponent<Canvas>().overrideSorting = true;
            go.GetComponent<Canvas>().sortingOrder = 15;
            go.AddComponent<GraphicRaycaster>();
            go.GetComponent<Button>().onClick.RemoveListener(delegate { CancelHightLight(go); });
            go.GetComponent<Button>().onClick.AddListener(delegate { CancelHightLight(go); });
        } 
        public void GuideAnalytics(string guide)
        {
            bool isGuide = true; 
            if (PlayerPrefs.HasKey(guide))
                isGuide = false;  
            if(isGuide)
            {                                             
                PlayerPrefs.SetString(guide,"1");
            }
            
        }
    }
}


