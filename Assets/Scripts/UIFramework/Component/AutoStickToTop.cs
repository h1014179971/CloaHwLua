using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIFramework
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class AutoStickToTop : MonoBehaviour
    {
        public UIPanel releatePanel;
        public RectTransform target;
        RectTransform _autoStickToTopPanel;
        Transform _originParent;
        float _topY;
        bool _bStickToTop;
        bool _bInit;

        private void Awake()
        {
            if (releatePanel != null)
            {
                releatePanel.AfterOpen.AddListener(() =>
                {
                    StartCoroutine(Init());
                });
            }
            else
            {
                StartCoroutine(Init());
            }
        }

        // Use this for initialization
        IEnumerator Init()
        {
            yield return null;
            _autoStickToTopPanel = GetComponent<RectTransform>();
            _originParent = target.parent;
            _topY = target.position.y - _originParent.GetComponent<RectTransform>().anchoredPosition.y - target.sizeDelta.y / 2;
            _bInit = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_bInit || target == null || _autoStickToTopPanel == null)
                return;

            bool bStick = (_originParent.position.y >= _topY);
            if (_bStickToTop != bStick)
            {
                _bStickToTop = bStick;
                if (_bStickToTop)
                {
                    target.SetParent(_autoStickToTopPanel, true);
                }
                else
                {
                    target.SetParent(_originParent, true);
                }
                target.anchoredPosition = Vector2.zero;
            }
        }
    }
}