using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIFramework
{
    public abstract class UIPage : UIPanel
    {
        [NonSerialized]
        public UnityEvent OnWindowChanged = new UnityEvent();
        public UnityAction<int> OnWindowCount;
        private Transform _WindowRoot;
        public Transform WindowRoot
        {
            get
            {
                return _WindowRoot;
            }
        }

        //public UIWindow CurrentWindow
        //{
        //    get
        //    {
        //        if (_WindowTypeList.Count > 0)
        //        {
        //            return _WindowDict[_WindowTypeList[_WindowTypeList.Count - 1]];
        //        }
        //        return null;
        //    }
        //}

        public UIWindow CurrentWindow
        {
            get
            {
                int index = _WindowTypeList.Count - 1;
                while(index>=0)
                {
                    UIWindow window= _WindowDict[_WindowTypeList[index]];
                    if(window.IsActive)
                    {
                        return window;
                    }
                    index--;
                }
                return null;
            }
        }

        private Dictionary<Type, UIWindow> _WindowDict = new Dictionary<Type, UIWindow>();

        private List<Type> _WindowTypeList = new List<Type>();

        //private Stack<UIWindow> _WindowStack = new Stack<UIWindow>();
        //public Stack<UIWindow> WindowStack
        //{
        //    get
        //    {
        //        return _WindowStack;
        //    }
        //}


        public void Init(object args = null)
        {
            _WindowRoot = this.transform.Find("WindowContent");
            if (_WindowRoot == null)
            {
                _WindowRoot = this.transform;
            }

            InitPage(args);
        }
        public override void Open()
        {
            
            base.Open();                 
            gameObject.SetActive(true);  
        }

        public override void Hide()
        {
            AfterHide.AddListener(() => {
                gameObject.SetActive(false);
            });
            base.Hide();
            //gameObject.SetActive(false);
        }
        /// <summary>
        /// 主要用于初始化打开关闭动画和回调函数
        /// </summary>
        protected abstract void InitPage(object args = null);

        public UIWindow GetWindowByType(Type type)
        {
            if (_WindowTypeList.Contains(type))
            {
                return _WindowDict[type];
            }
            return null;
        }

        public void AddWindow(UIWindow window, bool closeCurrentWin, bool hideCurrentWin, bool clearFlag)
        {
            //Debug.Log("Add window: " + window.GetType());
            if (_WindowTypeList.Contains(window.GetType()))
            {
                Type t = window.GetType();
                _WindowTypeList.Remove(t);
                UIWindow win = _WindowDict[t];
                _WindowDict.Remove(t);
                if (win != null)
                {
                    Destroy(win.gameObject);
                }
                
                
            }
            
            //if (closeCurrentWin)
            //{
            //    if (CurrentWindow != null)
            //    {
            //        CloseCurrentWindow();
            //    }
            //}
            //else if (CurrentWindow != null && hideCurrentWin) 
            //{
            //    CurrentWindow.Hide();
            //}

            if (clearFlag)
            {
                ClearAllWindows();
            }
            
            window.AfterDestroy.AddListener(() =>
            {
                Type t = window.GetType();
                _WindowDict.Remove(t);
                _WindowTypeList.Remove(t);
                
                OnWindowChanged.Invoke();
                OnWindowCount?.Invoke(_WindowDict.Count);
            });

            _WindowTypeList.Add(window.GetType());
            _WindowDict.Add(window.GetType(), window);
            OnWindowChanged.Invoke();
            OnWindowCount?.Invoke(_WindowDict.Count);
        }

        public void CloseWindowByType(Type type)
        {
            if (!_WindowTypeList.Contains(type))
            {
                Debug.LogWarning("Cannot close nonexist window.");
                return;
            }
            UIWindow window = _WindowDict[type];
            if (window.IsActive)
                window.PlayCloseAniAndDestory(false);
            
            //_WindowDict.Remove(type);
            //_WindowTypeList.Remove(type);
            //window.PlayCloseAniAndDestory();
            //OnWindowCount?.Invoke(_WindowDict.Count);
        }

        public void CloseAllWindows()
        {
            if (_WindowTypeList.Count <= 0)
            {
                Debug.LogWarning("No window to be closed.");
                return;
            }

            //UIWindow window = _WindowDict[_WindowTypeList[_WindowTypeList.Count - 1]];
            //for (int i = 0; i < _WindowTypeList.Count - 1; i++)
            //{
            //    Type t = _WindowTypeList[i];
            //    UIWindow win = _WindowDict[t];
            //    Destroy(win.gameObject);
            //}
            //window.PlayCloseAniAndDestory();
           
            for (int i = 0; i < _WindowTypeList.Count - 1; i++)
            {
                Type t = _WindowTypeList[i];
                UIWindow win = _WindowDict[t];
                if(win.IsActive)
                    win.PlayCloseAniAndDestory();
                else
                    Destroy(win.gameObject);
            }
            _WindowTypeList.Clear();
            _WindowDict.Clear();
            OnWindowCount?.Invoke(_WindowDict.Count);
        }

        public void CloseCurrentWindow()
        {
            // Close current 
            if(CurrentWindow!=null)
            {
                CurrentWindow.PlayCloseAniAndDestory(false);
            }
            //if (_WindowTypeList.Count > 0)
            //{
            //    Type t = _WindowTypeList[_WindowTypeList.Count - 1];
            //    UIWindow window = _WindowDict[t];
            //    _WindowDict.Remove(t);
            //    _WindowTypeList.Remove(t);
            //    window.PlayCloseAniAndDestory();

            //    //_WindowStack.Pop().Close();
            //    OnWindowChanged.Invoke();
            //    OnWindowCount?.Invoke(_WindowDict.Count);
            //}
        }

        public void HideCurrentWindow()
        {
            // Hide current 
            if (_WindowTypeList.Count > 0)
            {

                Type t = _WindowTypeList[_WindowTypeList.Count - 1];
                UIWindow window = _WindowDict[t];
                window.Hide();
                //_WindowStack.Peek().Hide();
                //OnWindowChanged.Invoke();
            }
        }

        public void ShowCurrentWindow()
        {
            // Show current 
            if (_WindowTypeList.Count > 0)
            {
                Type t = _WindowTypeList[_WindowTypeList.Count - 1];
                UIWindow window = _WindowDict[t];
                window.Show();
                //_WindowStack.Peek().Show();
                //OnWindowChanged.Invoke();
            }
        }

        public void BackToLastWindow()
        {
            CloseCurrentWindow();
            if (CurrentWindow != null)
            {
                // Show previous one
                CurrentWindow.Show();
            }
        }

        public void BackToWindowByType(Type type)
        {
            if (!typeof(UIWindow).IsAssignableFrom(type))
            {
                DebugUtil.Error("Error, input paramater is not an UIPage.");
                return;
            }


            if (!_WindowTypeList.Contains(type))
            {
                Debug.LogWarning("No such window: " + type);
                return;
            }

            if (type == CurrentWindow.GetType())
            {
                Debug.LogWarning("Cannot back to current window.");
                return;
            }

            int index = _WindowTypeList.IndexOf(type);
            UIWindow windowToShow = _WindowDict[type];

            int count = 0;
            for (int i = index + 1; i < _WindowTypeList.Count - 1; i++)
            {
                _WindowDict[_WindowTypeList[i]].CloseAnimation.Clear();
                _WindowDict[_WindowTypeList[i]].PlayCloseAniAndDestory();
                _WindowDict.Remove(_WindowTypeList[i]);
                count++;
            }
            if (count > 0)
            {
                _WindowTypeList.RemoveRange(index + 1, count);
            }

            BackToLastWindow();
             //= pageToShow;
        }

        private void ClearAllWindows()
        {
            if (_WindowTypeList.Count < 1)
            {
                return;
            }

            // Destroy all windows
            for (int i = 0; i < _WindowTypeList.Count; i++)
            {
                Type t = _WindowTypeList[i];
                UIWindow window = _WindowDict[t];
                Destroy(window.gameObject);
            }

            _WindowTypeList.Clear();
            _WindowDict.Clear();
        }

        protected virtual void ClosePage()
        {
            UIController.Instance.BackToLastPage();
        }
        

    }
}