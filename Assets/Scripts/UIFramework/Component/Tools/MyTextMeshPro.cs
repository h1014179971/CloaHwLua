using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UIFramework;
using TMPro;

namespace TMPro 
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(TextInfo))]
    [AddComponentMenu("3D Object/MyTextMeshPro")]
    [ExecuteAlways]
    public class MyTextMeshPro : TextMeshPro
    {
        private TextInfo _textInfo;
        private TextInfo textInfo
        {
            get
            {
                if (_textInfo == null)
                    _textInfo = GetComponent<TextInfo>();
                return _textInfo;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying && LanguageCtrl.Instance != null)
                LanguageCtrl.Instance.CurrentLanguageChageEvent.AddListener(SetFont);
        }
        [ContextMenu("3D Object/MyTextMeshPro")]
        protected override void Start()
        {
            base.Start();

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying) return;

            //if (!textInfo.isLockFont)
            //{
            //    SetFont();
            //}        
            SetFont();
        }
        void SetFont()
        {
            if (!textInfo.isLockFont)
                ShowText();
        }
        void ShowText()
        {
            string str = LanguageCtrl.Instance.GetLanguageById(textInfo.id);
            this.text = str;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Application.isPlaying && LanguageCtrl.Instance != null)
                LanguageCtrl.Instance.CurrentLanguageChageEvent.RemoveListener(SetFont);
        }
        #region  对外接口
        public void SetText(int _id)
        {
            Clear();
            textInfo.id = _id.ToString();
            ShowText();
        }
        public void SetText(string _arg0, string _id = null)
        {
            Clear();
            if (_id != null)
                textInfo.id = _id;
            textInfo.arg0 = _arg0;
            ShowText();
        }
        public void SetText(string _arg0, string _arg1, string _id = null)
        {
            Clear();
            if (_id != null)
                textInfo.id = _id;
            textInfo.arg0 = _arg0;
            textInfo.arg1 = _arg1;
            ShowText();
        }
        public void SetText(string _arg0, string _arg1, string _arg2, string _id = null)
        {
            Clear();
            if (_id != null)
                textInfo.id = _id;
            textInfo.arg0 = _arg0;
            textInfo.arg1 = _arg1;
            textInfo.arg2 = _arg2;
            ShowText();
        }
        public void SetFormatText(string _arg0, string _id = null)
        {
            Clear();
            if (_id != null)
                textInfo.id = _id;
            textInfo.arg0 = _arg0;

        }
        public void SetFormatText(string _arg0, string _arg1, string _id = null)
        {
            Clear();
            if (_id != null)
                textInfo.id = _id;
            textInfo.arg0 = _arg0;
            textInfo.arg1 = _arg1;

        }
        private void Clear()
        {
            textInfo.arg0 = "";
            textInfo.arg1 = "";
            textInfo.arg2 = "";
        }

        #endregion
    }
}


