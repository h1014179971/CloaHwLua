using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Foundation;
using libx;
namespace UIFramework
{
    public class UIController : MonoBehaviour
    {
        public string MainPage = UIPrefabPath.UI_MainPage;
        private static UIController _Instance = null;
        private UIPanel uiPanel;

        public static UIController Instance
        {
            get
            {
                if (_Instance == null)
                {
                    GameObject uiCanvas = GameObject.Find(UIName.UICanvas);
                    if (uiCanvas == null)
                    {
                        GameObject pageObj = Resources.Load<GameObject>(UIPrefabPath.UI_CANVAS);
                        if(pageObj == null)
                        {

                        }
                        else
                        {
                            uiCanvas = Instantiate(Resources.Load<GameObject>(UIPrefabPath.UI_CANVAS));
                            uiCanvas.name = UIName.UICanvas;
                            _Instance = uiCanvas.GetOrAddComponent<UIController>();
                            DontDestroyOnLoad(_Instance);
                        }
                        
                    }
                    else
                    {
                        Debug.Assert(uiCanvas != null, "UICanvas is NULL !!!");
                        _Instance = uiCanvas.GetOrAddComponent<UIController>();
                        DontDestroyOnLoad(_Instance);
                    }
                    
                }
                return _Instance;
            }
        }
        public static UIController getInstance()
        {
            return _Instance;
        }
        public static Transform UIRoot
        {
            get
            {
                return Instance.transform;
            }
        }

        private Canvas _uiCanvas;
        public Canvas UICanvas
        {
            get
            {
                if (_uiCanvas == null)
                {
                    _uiCanvas = GetComponent<Canvas>();
                }
                return _uiCanvas;
            }
        }

        private CanvasGroup _canvasGroup;
        //public CanvasGroup


        public void EnterMainPage<T>() where T : UIPage
        {
            MainPage = UIPrefabPath.UI_MainPage;
            OpenPage<T>(MainPage);
        }


        private UIPage _CurrentPage;
        public UIPage CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
        }

        private Dictionary<Type, UIPage> _PageDict = new Dictionary<Type, UIPage>();
        private Dictionary<Type, UIFx> _FxDict = new Dictionary<Type, UIFx>();
        public Dictionary<Type, UIPage> PageDict
        {
            get
            {
                return _PageDict;
            }
        }

        private List<Type> _PageTypeList = new List<Type>();
        public List<Type> PageTypeList
        {
            get
            {
                return _PageTypeList;
            }
        }
        private List<Type> _FxTypeList = new List<Type>();
        public List<Type> FxTypeList
        {
            get { return _FxTypeList; }
        }


        private Transform _UIContent;
        public Transform UIContent
        {
            get
            {
                return _UIContent;
            }
        }
        public Transform GameRoot { get { return _gameRoot; } }
        public Transform BlockerRoot { get { return _blockerRoot; } }
        public Transform FxRoot { get { return _fxRoot; } }

        public Transform UISceenRoot { get { return _uiSceenRoot; } }
        private Transform _uiSceenRoot;
        private Transform _gameRoot;
        private Transform _blockerRoot;
        private Transform _fxRoot;
        private GameObject _BlockerPrefab = null;
        private GameObject _Blocker = null;
        //private UITopPage _uiTopPage;
        private GameObject _LoadingPanelPrefab = null;
        private GameObject _LoadingPanel = null;
        private GameObject _LoadingWithBGPrefab = null;
        private GameObject _LoadingWithBG = null;

        

        private void Awake()
        {
            // Init ui content container
            GameObject go = GameObject.Find(UIName.UIContent);
            if (go == null)
            {
                go = new GameObject(UIName.UIContent);
                go.transform.SetParent(UIRoot);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
            _UIContent = go.transform;
            _uiSceenRoot = transform.Find("UISceenRoot");
            _gameRoot = transform.Find("UIGameRoot");
            _blockerRoot = transform.Find("UIBlockerRoot");
            _fxRoot = transform.Find("UIFxRoot");
            GameObject eventSystem = GameObject.Find("EventSystem");
            if (!eventSystem)
            {
                eventSystem = new GameObject();
                eventSystem.name = "EventSystem";
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
            DontDestroyOnLoad(eventSystem);
            
            DontDestroyOnLoad(gameObject); 
        }


        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ General func below ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ //
        public GameObject ShowBlocker(string path,bool fromBundle=false)
        {
            GameObject prefab = null;
            AssetRequest request = null;
            if (fromBundle)
            {
                request = Assets.LoadAsset(path, typeof(GameObject));
                prefab = request.asset as GameObject;
            }
            else
            {
                prefab = Resources.Load<GameObject>(path);
            }
            if (prefab == null) return null;

            GameObject obj = Instantiate(prefab, _blockerRoot);
            UIBlockerBase blocker = obj.GetComponent<UIBlockerBase>();
            if(blocker!=null)
            {
                blocker.Init(request);
            }
            obj.SetActive(true);
            return obj;
        }


        public void ShowBlocker()
        {                 
            if (_BlockerPrefab == null)
            {
                _BlockerPrefab = Resources.Load<GameObject>(UIPrefabPath.UI_Blocker); 
            }
            if (_Blocker == null)
            {
                _Blocker = Instantiate(_BlockerPrefab, _blockerRoot);
                //_uiTopPage = _Blocker.GetComponent<UITopPage>();
            }
            _Blocker.SetActive(true);
            ShowBlockerSetting(false);
        }
        public void ShowBlocker(bool isSet)
        {
            ShowBlocker();
            ShowBlockerSetting(isSet);
        }
        public void ShowBlockerSetting(bool isSet = false)
        {
            //_uiTopPage.ShowSetting(isSet);
        }
        public void HideBlocker()
        {                  
            if (_Blocker == null)
            {                   
                return;
            }  
            _Blocker.SetActive(false);   
        }

        public void HideBlocker(bool force)
        {
            if (force)
            {                  
                if (_Blocker != null)
                {
                    Destroy(_Blocker);
                }

                return;
            }

            HideBlocker();
        }

        public void ShowLoadingPanel()
        {
            if (_LoadingPanel != null || _LoadingPanelPrefab == null)
            {
                return;
            }
            _LoadingPanel = Instantiate(_LoadingPanelPrefab, _blockerRoot);
        }

        public void HideLoadingPanel()
        {
            if (_LoadingPanel == null)
            {
                return;
            }
            Destroy(_LoadingPanel);
        }

        public void ShowLoadingWithBG()
        {
            if (_LoadingWithBG != null || _LoadingPanelPrefab == null)
            {
                return;
            }
            _LoadingWithBG = Instantiate(_LoadingWithBGPrefab, _blockerRoot);
        }

        public void HideLoadingWithBG()
        {
            if (_LoadingWithBG == null)
            {
                return;
            }
            Destroy(_LoadingWithBG);
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ General func above ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ //


        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ UIPanel control func below ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ //
        private T GeneratePanel<T>(string path, Transform target = null) where T : UIPanel
        {
            GameObject panelObj = Resources.Load<GameObject>(path);
            return GeneratePanel<T>(panelObj, target);
        }

        private T GeneratePanel<T>(GameObject panelObj, Transform target = null) where T : UIPanel
        {
            if (target == null)
            {
                target = _UIContent.transform;
            }

            T p = Instantiate(panelObj, target).GetComponent<T>();
            if (p == null)
            {
                DebugUtil.Error(string.Format("Component [{0}] not find.", typeof(T).Name));
            }
            return p;
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ UIPanel control func above ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ //



        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Page control func below ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ //
       
        public T OpenPageFromAssets<T>(string assetName,object args=null) where T : UIPage
        {
            if (_PageTypeList.Contains(typeof(T)))
            {
                // Exists page, Show it, hide current page
                if (typeof(T) == _CurrentPage.GetType())
                {
                    // Trying to show current page  
                    DebugUtil.Warning("Trying to show current window.");
                    return _CurrentPage as T;
                }
                else
                {
                    // Accessing hided page, close current page and show hided page
                    //BackToPageBtType(typeof(T));
                    _PageTypeList.Remove(typeof(T));
                    _PageTypeList.Add(typeof(T));
                    _CurrentPage.Hide();
                    _CurrentPage = _PageDict[typeof(T)];
                    _CurrentPage.Open();
                    return _CurrentPage as T;
                }
            }
            else
            {
                AssetRequest request = Assets.LoadAsset(assetName,typeof(GameObject));
                GameObject pageObj = request.asset as GameObject;
                    
                if (pageObj)
                {
                    if (_CurrentPage)
                    {
                        _CurrentPage.Hide();
                    }
                    _CurrentPage = GeneratePanel<T>(pageObj, _UIContent);
                    _CurrentPage.request = request;
                    _PageTypeList.Add(typeof(T));
                    _PageDict.Add(typeof(T), _CurrentPage);
                    _CurrentPage.Open();
                    return _CurrentPage as T;
                }
                else
                {
                    DebugUtil.Error("Prefab NO FOUND! Path:" + assetName);
                    return null;
                }
            }
        }

        public T OpenPage<T>(string path, object args = null) where T : UIPage
        {
            if (_PageTypeList.Contains(typeof(T)))
            {
                // Exists page, Show it, hide current page
                if (typeof(T) == _CurrentPage.GetType())
                {
                    // Trying to show current page  
                    DebugUtil.Warning("Trying to show current window.");
                    return _CurrentPage as T;
                }
                else
                {
                    // Accessing hided page, close current page and show hided page
                    //BackToPageBtType(typeof(T));
                    _PageTypeList.Remove(typeof(T));
                    _PageTypeList.Add(typeof(T));
                    _CurrentPage.Hide();
                    _CurrentPage = _PageDict[typeof(T)];
                    _CurrentPage.Open();
                    return _CurrentPage as T;
                }
            }
            else
            {
                GameObject pageObj = Resources.Load<GameObject>(path);
                if (pageObj)
                {
                    return OpenPage<T>(pageObj, args);
                }
                else
                {
                    DebugUtil.Error("Prefab NO FOUND! Path:" + path);
                    return null;
                }
            }

                
        }

        public T OpenPage<T>(GameObject pageObj, object args = null) where T : UIPage
        {
            if (_PageTypeList.Contains(typeof(T)))
            {
                // Exists page, Show it, hide current page
                if (typeof(T) == _CurrentPage.GetType())
                {
                    // Trying to show current page  
                    DebugUtil.Warning("Trying to show current window.");
                    return _CurrentPage as T;
                }
                else
                {
                    // Accessing hided page, close current page and show hided page
                    //BackToPageBtType(typeof(T));
                    _PageTypeList.Remove(typeof(T));
                    _PageTypeList.Add(typeof(T));
                    _CurrentPage.Hide();
                    _CurrentPage = _PageDict[typeof(T)];
                    _CurrentPage.Open();
                    return _CurrentPage as T;
                }
            }
            else
            {
                if (_CurrentPage)
                {
                    _CurrentPage.Hide();
                }
                // Generate new page
                _CurrentPage = GeneratePanel<T>(pageObj, _UIContent);
                _PageTypeList.Add(typeof(T));
                _PageDict.Add(typeof(T), _CurrentPage);
                //_CurrentPage.Init(args);
                _CurrentPage.Open();
                return _CurrentPage as T;
            }
        }

        public void ClosePage<T>() where T : UIPage
        {
            Type t = typeof(T);
            if (_PageTypeList.Contains(t))
            {
                // Exists page, close it, show previous page
                //if (_CurrentPage.GetType() == t)
                if (t.IsAssignableFrom(_CurrentPage.GetType()))
                {
                    // Trying to close current page
                    _PageTypeList.Remove(t);
                    _PageDict.Remove(t);
                    _CurrentPage.PlayCloseAniAndDestory();

                    if (_PageTypeList.Count > 0)
                    {
                        // Show previous page
                        Type type = _PageTypeList[_PageTypeList.Count - 1];
                        _CurrentPage = _PageDict[type];
                        _CurrentPage.Show();
                    }
                    else
                    {
                        DebugUtil.Warning("There is no page.");
                    }
                }
                else
                {
                    // Closing hided page, directly remove it
                    _PageTypeList.Remove(t);
                    UIPage page = _PageDict[t];
                    _PageDict.Remove(t);
                    page.CloseAnimation.Clear();
                    page.PlayCloseAniAndDestory();
                }
            }
            else
            {
                // Trying to close nonexistent page
                DebugUtil.Error("Trying to close nonexistent page.");
            }
        }

        public void HidePage<T>() where T : UIPage
        {
            Type t = typeof(T);
            if (_PageTypeList.Contains(t))
            {
                if (t.IsAssignableFrom(_CurrentPage.GetType()))
                {
                    _CurrentPage.Hide();

                    if (_PageTypeList.Count > 1)
                    {
                        // Show previous page.
                        Type type = _PageTypeList[_PageTypeList.Count - 2];
                        _PageTypeList.Remove(type);
                        _PageTypeList.Add(type);
                        _CurrentPage = _PageDict[type];
                        _CurrentPage.Show();
                    }
                    else
                    {
                        DebugUtil.Warning("There is no page.");
                    }
                }
                else
                {
                    // Closing hided page, directly remove it
                    DebugUtil.Info("The page is hide now.");
                }
            }
            else
            {
                // Trying to close nonexistent page
                DebugUtil.Error("Trying to close nonexistent page.");
            }
        }

        public void CloseCurrentPage()
        {
            BackToLastPage();
        }

        public void BackToLastPage()
        {
            BackToPageByNum(1);
        }

        private void BackToPageByNum(int n)
        {
            if (n < 1)
            {
                DebugUtil.Warning("Error, you have to go back at least 1 panel.");
                return;
            }

            if (_PageDict.Count <= 0)
            {
                DebugUtil.Error("Error, there is no panel.");
                return;
            }
            int index = _PageTypeList.Count - 1 - n;
            if (index < 0)
            {
                DebugUtil.Error("Error, trying to go back to panel of index: " + index);
                return;
            }

            BackToPageByType(_PageTypeList[index]);

        }

        public void BackToPageByType(Type t)
        {
            if (!typeof(UIPage).IsAssignableFrom(t))
            {
                DebugUtil.Error("Error, input paramater is not an UIPage.");
                return;
            }

            if (_CurrentPage.GetType().Equals(t))
            {
                DebugUtil.Error("Error, you can not go back to current panel.");
                return;
            }

            if (_PageTypeList.Contains(t))
            {
                // Panel exists in list
                int index = _PageTypeList.IndexOf(t);
                UIPage pageToShow = _PageDict[t];
                int count = 0;
                for (int i = index + 1; i < _PageTypeList.Count - 1; i++)
                {
                    _PageDict[_PageTypeList[i]].CloseAnimation.Clear();
                    _PageDict[_PageTypeList[i]].PlayCloseAniAndDestory();
                    _PageDict.Remove(_PageTypeList[i]);
                    count++;
                }
                if (count > 0)
                {
                    _PageTypeList.RemoveRange(index + 1, count);
                }
                _PageDict.Remove(_CurrentPage.GetType());
                _PageTypeList.Remove(_CurrentPage.GetType());
                _CurrentPage.PlayCloseAniAndDestory();
                pageToShow.Show();
                _CurrentPage = pageToShow;
            }
            else
            {
                DebugUtil.Error("Trying to access to a nonexistent page.");
                return;
            }
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ Page control func above ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ //



        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Window control func below ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ //
        public T OpenWindowFromAsset<T>(string assetName,
                                object arg = null,
                                bool closeCurrentWin = false,
                                bool hideCurrentWin = true,
                                bool clearFlag = false) where T : UIWindow
        {
            if (_CurrentPage == null)
            {
                DebugUtil.Error("No opened page, cannot open a window.");
                return null;
            }
            if (closeCurrentWin || hideCurrentWin)
            {
                _CurrentPage.CloseCurrentWindow();
            }
            
            Transform target = _CurrentPage.WindowRoot;

            T window = _CurrentPage.GetWindowByType(typeof(T)) as T;
            if (window == null)
            {
                AssetRequest request = Assets.LoadAsset(assetName, typeof(GameObject));
                GameObject windowObj = request.asset as GameObject;
                window = GeneratePanel<T>(windowObj, target);
                window.request = request;
                _CurrentPage.AddWindow(window, closeCurrentWin, hideCurrentWin, clearFlag);
            }
            window.Init(arg);
            window.Open();
            ActiveUIPanel = window;

            return window;
        }


        public T OpenWindow<T>(string path, 
                               object arg = null,
                               bool closeCurrentWin = false,
                               bool hideCurrentWin = true,
                               bool clearFlag = false) where T : UIWindow
        {
            //LogUtility.LogInfo("openWindow=====" + typeof(T).Name);
            GameObject windowObj = Resources.Load<GameObject>(path);
            if (windowObj == null)
                return null;
            return OpenWindow<T>(windowObj, arg, closeCurrentWin, hideCurrentWin, clearFlag);
        }
       
        public T OpenWindow<T>( GameObject windowObj,
                                object arg = null,
                                bool closeCurrentWin = false,
                                bool hideCurrentWin = true,
                                bool clearFlag = false) where T : UIWindow
        {
            if (_CurrentPage == null)
            {
                DebugUtil.Error("No opened page, cannot open a window.");
                return null;
            }
            if(closeCurrentWin||hideCurrentWin)
            {
                //if (_CurrentPage.CurrentWindow != null)
                //    Debug.Log(" closeCurrentWin=" + _CurrentPage.CurrentWindow.IsActive);
                _CurrentPage.CloseCurrentWindow();
            }


            Transform target = _CurrentPage.WindowRoot;

            T window = _CurrentPage.GetWindowByType(typeof(T)) as T;
            if(window==null)
            {
                window = GeneratePanel<T>(windowObj, target);
                _CurrentPage.AddWindow(window, closeCurrentWin, hideCurrentWin, clearFlag);
            }
            //T window = GeneratePanel<T>(windowObj, target);
            //_CurrentPage.AddWindow(window, closeCurrentWin, hideCurrentWin, clearFlag);
            window.Init(arg);
            window.Open();
            //Debug.Log(" window.Open()="+window.IsActive);
            ActiveUIPanel = window;

            return window;
        }
        public UIPanel ActiveUIPanel
        {
            get { return uiPanel; }
            set { uiPanel = value; }
        }

        public void CloseWindowByType<T>() where T : UIWindow
        {
            Type t = typeof(T);
            if (_CurrentPage == null)
            {
                DebugUtil.Error("No opened page, cannot open a window.");
                return;
            }

            _CurrentPage.CloseWindowByType(t);
        }

        public void CloseCurrentWindow()
        {
            BackToLastWindow();
        }

        public void BackToLastWindow()
        {
            if (_CurrentPage == null)
            {
                DebugUtil.Error("No opened page, cannot close a window.");
                return;
            }

            _CurrentPage.BackToLastWindow();
        }

        public void BackToWindowByType(Type t)
        {
            if (_CurrentPage == null)
            {
                DebugUtil.Error("No opened page, cannot close a window.");
                return;
            }

            _CurrentPage.BackToWindowByType(t);
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ Window control func above ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ //


        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Popup ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ //

        public T OpenPopup<T>(string path,
                              UnityAction confirmCB = null,
                              UnityAction refuseCB = null,
                              UnityAction cancelCB = null) where T : UIPopup
        {
            GameObject popupObj = Resources.Load<GameObject>(path);
            if(popupObj == null)
            {
                return null;
            }
            return OpenPopup<T>(popupObj,confirmCB,refuseCB,cancelCB);
        }

        public T OpenPopup<T>(GameObject popupObj,
                              UnityAction confirmCB = null,
                              UnityAction refuseCB = null,
                              UnityAction cancelCB = null) where T : UIPopup
        {
            if(_CurrentPage == null)
            {
                DebugUtil.Error("No opened page, cannot open a popup.");
                return null;
            }

            Transform target = _CurrentPage.WindowRoot;
            T popup = GeneratePanel<T>(popupObj,target);
            popup.SetCallback(confirmCB,refuseCB,cancelCB);
            _CurrentPage.AddWindow(popup,false,false,false);
            popup.Init();
            popup.Open();

            return popup;
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ Popup ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ //

        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Fx control func below ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ //

        private T GenerateFx<T>(string path, Transform target = null) where T : UIFx
        {
            GameObject fxObj = Resources.Load<GameObject>(path);
            return GenerateFx<T>(fxObj, target);
        }

        private T GenerateFx<T>(GameObject fxObj, Transform target = null) where T : UIFx
        {
            if (target == null)
            {
                target = _fxRoot.transform;
            }

            T p = Instantiate(fxObj, target).GetComponent<T>();
            if (p == null)
            {
                DebugUtil.Error(string.Format("Component [{0}] not find.", typeof(T).Name));
            }
            return p;
        }


        public T OpenFx<T>(string path, object args = null) where T : UIFx
        {
            if (_FxTypeList.Contains(typeof(T)))
            {               
                UIFx fx  = _FxDict[typeof(T)];
                fx.Open(args);   
                return fx as T;
            }
            else
            {
                GameObject fxObj = Resources.Load<GameObject>(path);
                if (fxObj)
                {
                    return OpenFx<T>(fxObj, args);
                }
                else
                {
                    DebugUtil.Error("Prefab NO FOUND! Path:" + path);
                    return null;
                }
            }


        }

        public T OpenFx<T>(GameObject fxObj, object args = null) where T : UIFx
        {
            if (_PageTypeList.Contains(typeof(T)))
            {
                UIFx fx = _FxDict[typeof(T)];
                fx.Open(args);
                return fx as T;
            }
            else
            {   
                T fx = GenerateFx<T>(fxObj, _fxRoot);
                _FxTypeList.Add(typeof(T));
                _FxDict.Add(typeof(T), fx);
                fx.Init(args);
                fx.Open(args);
                return _CurrentPage as T;
            }
        }
        public void Show<T>() where T:UIFx
        {
            Type t = typeof(T);
            if (_FxTypeList.Contains(t))
            {                     
                UIFx fx = _FxDict[t];
                fx.Show();
            }
            else
            {
                DebugUtil.Error("Trying to close nonexistent fx.");
            }

        }
        public void CloseFx<T>() where T : UIFx
        {
            Type t = typeof(T);
            if (_FxTypeList.Contains(t))
            {    
                _FxTypeList.Remove(t);
                UIFx fx = _FxDict[t];
                _FxDict.Remove(t);
                fx.Close();                  
            }
            else
            {
                //LogUtility.LogError("Trying to close nonexistent fx.");
                Debug.LogError("Trying to close nonexistent fx.");
            }
        }

        public void HideFx<T>() where T : UIFx
        {
            Type t = typeof(T);
            if (_FxTypeList.Contains(t))
            {                      
                UIFx fx = _FxDict[t];
                fx.Hide();
            }
            else
            {                                      
                DebugUtil.Error("Trying to close nonexistent fx.");
            }
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ Fx control func below ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ //



        public void AddPageForTest(UIPage p)
        {
            p.Init();
            Type t = p.GetType();
            _PageTypeList.Add(t);
            _PageDict.Add(t, p);
            _CurrentPage = p;
            DebugUtil.Info("Add");
        }

        public Vector2 GetSizeDelta()
        {
            return transform.GetComponent<RectTransform>().sizeDelta;
        }
    }
}