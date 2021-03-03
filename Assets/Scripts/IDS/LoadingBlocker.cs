using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDS
{
    public sealed class LoadingBlocker : MonoBehaviour
    {
        [SerializeField] GameObject _blocker;

        private static LoadingBlocker _instance;
        private static int _flag;

        public static LoadingBlocker Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
            _blocker.SetActive(false);
        }

        //public void Show()
        //{
        //    _blocker.SetActive(true);
        //}

        //public void Hide(bool force = true)
        //{
        //    _blocker.SetActive(false);
        //}

        public void Show()
        {
            if (_blocker == null)
            {
                return;
            }
            _blocker.SetActive(++_flag > 0);
            Debug.Log("ShowBlocker:" + _flag);
        }

        public void Hide(bool force = false)
        {
            if (_blocker == null)
            {
                return;
            }
            if (force)
            {
                _flag = 0;
                _blocker.SetActive(false);
            }
            else
            {
                _flag = Mathf.Max(0, --_flag); // 最小值为0
                _blocker.SetActive(_flag > 0);
            }
            Debug.Log("HideBlocker:" + _flag);
        }
    }
}