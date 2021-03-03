using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using libx;
using UIFramework;
using DG.Tweening;

namespace Delivery.Idle
{
    public class IdleTipCtrl : MonoSingleton<IdleTipCtrl>
    {
        #region UI提示相关
        private GameObject tipPrefab;
        private MyText contentText;
        private bool active;
        private Vector3 startPos;
        private Transform tipParent;
        private RectTransform tipRectTrans;
        CanvasGroup canvasGroup;
        private Sequence sequence;
        #endregion

        private GameObject doubleIncomeTipPrefab;


        protected override void Awake()
        {
            active = false;
            startPos = new Vector3(0, 100f, 0);
            tipParent = UIController.Instance.BlockerRoot;
        }

        #region UI提示
        public void ShowTip(string content)
        {
            if (active) return;
            active = true;
            if(tipPrefab == null)
            {
                tipPrefab = AssetLoader.Load<GameObject>(PrefabName.tipUI);
            }
            GameObject tip = GameObject.Instantiate(tipPrefab);
            contentText = tip.GetComponentByPath<MyText>("content");
            contentText.text = content;
            tip.transform.SetParent(tipParent);
            tip.transform.localScale = Vector3.one;
            tipRectTrans = tip.transform.GetComponent<RectTransform>();
            canvasGroup = tip.transform.GetOrAddComponent<CanvasGroup>();
            PlayTipAni();
        }

        public void HideTip()
        {
            sequence.Kill();
            StopCoroutine("PlayTipAni");
            GameObject.Destroy(tipRectTrans.gameObject);
            active = false;
        }


        private IEnumerator PlayOutAni()
        {
            yield return new WaitForSeconds(1f);
            sequence = DOTween.Sequence();
            sequence.Append(canvasGroup.DOFade(0, 0.4f));
            sequence.Join(tipRectTrans.DOAnchorPos(startPos, 0.4f).SetEase(Ease.OutBack)).OnComplete(() =>
            {
                GameObject.Destroy(tipRectTrans.gameObject);
                active = false;
            });
        }


        private void PlayTipAni()
        {
            if (sequence!=null && sequence.IsPlaying()) return;
            sequence = DOTween.Sequence();
            tipRectTrans.anchoredPosition = startPos;
            canvasGroup.alpha = 0.0f;
            sequence.Append(canvasGroup.DOFade(1, 0.4f));
            sequence.Join(tipRectTrans.DOAnchorPos(Vector3.zero, 0.4f).SetEase(Ease.OutBack)).OnComplete(() =>
            {
                StartCoroutine(PlayOutAni());
            });
        }

        #endregion

        public void ShowDoubleIncomeTip()
        {
            
            GameObject tip = UIController.Instance.ShowBlocker(PrefabName.doubleIncomeTip, true);

            RectTransform content = tip.GetComponentByPath<RectTransform>("bg/content");
            CanvasGroup canvasGroup = content.GetOrAddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            Vector2 midPos = content.anchoredPosition;
            Vector2 startPos = midPos - new Vector2(300, 0);
            Vector2 endPos = midPos + new Vector2(300, 0);
            content.anchoredPosition = startPos;


            AudioCtrl.Instance.PlaySingleSound(GameAudio.doubleIncome);
            Sequence inSequence = DOTween.Sequence();
            inSequence.Append(canvasGroup.DOFade(1, 0.2f));
            inSequence.Join(content.DOAnchorPos(midPos, 0.5f)).OnComplete(() => {
                Sequence outSequence = DOTween.Sequence();
                //outSequence.Append(canvasGroup.DOFade(0, 0.5f));
                outSequence.Join(content.DOAnchorPos(endPos, 0.5f)).SetDelay(1).OnComplete(()=> {
                    canvasGroup.alpha = 0;
                    GameObject.Destroy(tip);
                });
            });
            
        }





    }
}

