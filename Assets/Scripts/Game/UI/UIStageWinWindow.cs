using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
using System;
using DG.Tweening;

namespace Delivery
{
    public class UIStageWinWindow : UIWindow
    {
        MyButton bgBtn;
        Transform bgTrans;
        Transform winbgTrans1;
        Transform winbgTrans2;
        protected override void InitWindow(object arg = null)
        {
            bgTrans = this.GetComponentByPath<Transform>("bgBtn/bg");
            bgTrans.localScale = Vector3.zero;
            bgTrans.DOScale(Vector3.one, 1f);    
            bgBtn = this.GetComponentByPath<MyButton>("bgBtn");
            bgBtn?.onClick.AddListener(OnClickClose);
            winbgTrans1 = this.GetComponentByPath<Transform>("bgBtn/bg/winbg_1");
            winbgTrans1.DORotate(new Vector3(0, 0, 360), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
            winbgTrans2 = this.GetComponentByPath<Transform>("bgBtn/bg/winbg_2");
            winbgTrans2.DORotate(new Vector3(0, 0, -360), 4f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }
        public override void Open()
        {
            base.Open();
        }
        public void OnClickClose()
        {
            Destroy(gameObject);
        }
    }
}

