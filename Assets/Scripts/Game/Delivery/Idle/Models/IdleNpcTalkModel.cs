using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Foundation;

namespace Delivery.Idle
{
    public class IdleNpcTalkModel : MonoBehaviour
    {
        private RectTransform _rectParent;
        private RectTransform _rectTransform;
        private Image _img;
        private MyText _txt;
        private ContentSizeFitter _imgSizeFitter;
        private ContentSizeFitter _txtSizeFitter;
        private float _fadeTime = 5f;
        private IdleNpcModel _npcModel;
        private Vector3 _offestVec = new Vector3(0.5f, 0.7f, 0);
        private Tweener _textOutTweener;
        private Tweener _imgFadeTweener;
        private Tweener _txtFadeTweener;
        private string _talk;
        public void Init(IdleNpcModel model)
        {
            if (_rectParent == null)
                _rectParent = transform.parent.GetComponent<RectTransform>();
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            if (_img == null)
                _img = transform.GetComponent<Image>();
            if (_txt == null)
                _txt = transform.GetComponentByPath<MyText>("talktxt");
            if (_imgSizeFitter == null)
                _imgSizeFitter = GetComponent<ContentSizeFitter>();
            if (_txtSizeFitter == null)
                _txtSizeFitter = _txt.GetComponent<ContentSizeFitter>();
            _npcModel = model;
            if (_textOutTweener != null && _textOutTweener.IsPlaying())
                _textOutTweener.Kill();
            Show();
        }

        private void Show()
        {
            AudioCtrl.Instance.PlayMultipleSound(GameAudio.clickNpcTalk);
            if (_textOutTweener != null && _textOutTweener.IsPlaying())
            {
                _textOutTweener.Kill();
                _textOutTweener = null;
            }  
            if (_imgFadeTweener != null && _imgFadeTweener.IsPlaying())
            {
                _imgFadeTweener.Kill();
                _imgFadeTweener = null;
            }
                
            if (_txtFadeTweener != null && _txtFadeTweener.IsPlaying())
            {
                _txtFadeTweener.Kill();
                _txtFadeTweener = null;
            }
            if(_npcModel.Talks.Count <=0)
            {
                LogUtility.LogError($"{_npcModel.name}没有talk");
            }
            int index = Random.Range(0, _npcModel.Talks.Count);
            string talk = _npcModel.Talks[index];
            _npcModel.Talks.RemoveAt(index);
            if (!string.IsNullOrEmpty(_talk) && !_npcModel.Talks.Contains(_talk))
                _npcModel.Talks.Add(_talk);
            _talk = talk;
            _txt.text = "";
            _img.DOFade(1, 0);
            _txt.DOFade(1, 0);
            _textOutTweener = _txt.DOText(_talk, _talk.Length / 12f).SetUpdate(true).SetEase(Ease.Linear).OnComplete(() =>
            {
                _textOutTweener = null;
                FadeOut();
            });
            UpdatePosition();
        }
        
        void Update()
        {
            if (_npcModel == null) return;
            UpdatePosition();
        }
        void UpdatePosition()
        {
            Vector2 v2 = Vector2.zero;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_npcModel.transform.position + _offestVec);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectParent, screenPos, RootCanvas.Instance.UICamera, out v2);
            _rectTransform.anchoredPosition3D = v2;
        }
        public void TweenKill()
        {
            if (_textOutTweener != null && _textOutTweener.IsPlaying())
            {
                _textOutTweener.Kill();
                _textOutTweener = null;
                _txt.text = _talk;
                if (_imgFadeTweener == null)
                    FadeOut();
                _txtSizeFitter.SetLayoutVertical();
                _imgSizeFitter.SetLayoutVertical();
            }
            else
            {
                Show();
            }
        }
        private void FadeOut()
        {
            _txtFadeTweener = _txt.DOFade(0, 5).SetDelay(_fadeTime).OnComplete(() =>
            {
                _txtFadeTweener = null;
            });
            _imgFadeTweener = _img.DOFade(0, 1).SetDelay(_fadeTime).OnComplete(() =>
            {
                _imgFadeTweener = null;
                IdleNpcTalkCtrl.Instance.RemoveNpcTalk(_npcModel);
                if (!string.IsNullOrEmpty(_talk) && !_npcModel.Talks.Contains(_talk))
                    _npcModel.Talks.Add(_talk);
                _talk = null;
                _npcModel = null;
                SG.ResourceManager.Instance.ReturnTransformToPool(transform);
            });
            
        }
    }
}

