using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery
{
    public class UIFxModel : MonoBehaviour
    {
        private RectTransform _rectTrans;
        private MyText _myText;
        private RectTransform _txtRectTrans;
        private RectTransform _parent;
        private Vector3 _woldPos;
        public void Init(RectTransform parent,Vector3 woldPos,string txt)
        {
            _parent = parent;
            _woldPos = woldPos;
            _rectTrans = GetComponent<RectTransform>();
            _rectTrans.SetParent(parent);
            _myText = this.GetComponentByPath<MyText>("txt");
            _txtRectTrans = _myText.GetComponent<RectTransform>();
            Vector2 v2 = Vector2.zero;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(woldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPos, RootCanvas.Instance.UICamera, out v2);
            _rectTrans.anchoredPosition3D = v2;
            _rectTrans.localScale = Vector3.one;
            _myText.text = txt;
            _txtRectTrans.localScale = Vector3.one * FixScreen.idleCameraSize / Camera.main.orthographicSize * 0.67f;
            int fontSize = _myText.fontSize;
            _myText.fontSize = (int)Math.Round(fontSize * FixScreen.idleCameraSize / Camera.main.orthographicSize);
            
            _txtRectTrans.anchoredPosition3D = Vector3.zero;
            float unit = Screen.height / (Camera.main.orthographicSize * 2);
            screenPos = Camera.main.WorldToScreenPoint(Vector3.up);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPos, RootCanvas.Instance.UICamera, out v2);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_myText.DOFade(1, 0.5f).SetEase(Ease.Linear));
            sequence.Join(_rectTrans.transform.DOLocalMoveY(0, 1f).SetEase(Ease.Linear));
            sequence.Append(_myText.DOFade(0, 0.5f).SetEase(Ease.Linear));
            sequence.Join(_txtRectTrans.DOLocalMoveY(unit * 2, 0.5f).SetEase(Ease.Linear));
            //sequence.Append(_txtRectTrans.DOLocalMoveY(unit * 2, 0.5f).SetEase(Ease.Linear));
            sequence.OnComplete(delegate
            {
                _myText.fontSize = fontSize;
                SG.ResourceManager.Instance.ReturnObjectToPool(gameObject);
            });
        }
        private void LateUpdate()
        {
            Vector2 v2 = Vector2.zero;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_woldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, screenPos, RootCanvas.Instance.UICamera, out v2);
            _rectTrans.anchoredPosition3D = new Vector3(v2.x, v2.y, _rectTrans.anchoredPosition3D.z);
        }
    }
}

