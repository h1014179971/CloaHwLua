using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
using DG.Tweening;

namespace Delivery.Idle
{
    public class IdleSiteBuildModel : MonoBehaviour
    {
        [SerializeField] private int _id;
        private RectTransform _blockerRoot;
        private RectTransform _facedRectTrans;
        private Image _img;
        private float _offsetY = 0;
        public int BuildId { get { return _id; } }
        
        public void LoadItem()
        {
            //int showRand = Random.Range(0, 10);
            //if (showRand > 0) return;
            if (_facedRectTrans != null) return;
            if(_blockerRoot == null)
                _blockerRoot = UIController.Instance.GameRoot as RectTransform;
            GameObject face = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.faced, true, 1);
            _facedRectTrans = face.GetComponent<RectTransform>();
            _facedRectTrans.SetParent(_blockerRoot);
            _facedRectTrans.localScale = Vector3.one;
            _img = _facedRectTrans.GetComponent<Image>();
            _offsetY = 0;
            UpdatePosition();
            _img.DOFade(1, 0);
            _img.DOFade(0, 1).SetDelay(1).OnComplete(() => {
                Hide();
            });
            //Sequence sequence = DOTween.Sequence();
            //sequence.Append(_img.DOFade(0, 3f).SetEase(Ease.Linear));
            //sequence.OnComplete(delegate
            //{
                
            //});
        }
        void Update()
        {
            if (_facedRectTrans == null) return;
            _offsetY += (Time.deltaTime  * 100);
            UpdatePosition();
            //if (_t < _showTime)
            //    _t += Time.deltaTime;
            //if (_t >= _showTime)
            //    Hide();
        }
        void UpdatePosition()
        {
            Vector2 v2 = Vector2.zero;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_blockerRoot, screenPos, RootCanvas.Instance.UICamera, out v2);
            _facedRectTrans.anchoredPosition3D = new Vector3(v2.x,v2.y + _offsetY,0);
        }
        void Hide()
        {
            SG.ResourceManager.Instance.ReturnTransformToPool(_facedRectTrans);
            _facedRectTrans = null;
        }
    }
}

