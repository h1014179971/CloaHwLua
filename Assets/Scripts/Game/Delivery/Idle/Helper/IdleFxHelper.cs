using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using DG.Tweening;
using UnityEngine.Events;

namespace Delivery.Idle
{
    public class IdleFxHelper
    {
        private static IdleTopUI topUI = GameObject.FindObjectOfType<IdleTopUI>();

        public static void PlayGetRewardFxWorld(Transform startTrans,UnityAction callback,bool isClick=false)
        {
            Vector3 worldPos = startTrans.position;
            FxCtrl.Instance.PlayFx(FxPrefabPath.idleAddMoney, worldPos + Vector3.up * 2, null);

            AudioCtrl.Instance.PlaySingleSound(GameAudio.moneyFxShow);
            RectTransform fxNode = UIController.Instance.FxRoot.GetComponent<RectTransform>();
            FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleInvestorReward, fxNode);
            Vector3 v2 = Vector3.zero;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(fxNode, screenPos, RootCanvas.Instance.UICamera, out v2);
            model.transform.position = v2;

            if (isClick)
            {
                FxModel clickModel = FxCtrl.Instance.PlayFx(FxPrefabPath.idleClickFx, startTrans.position, null);
                clickModel.transform.position = v2;
                //clickModel.transform.localScale = Vector3.one * 5;
            }

            model.transform.DOMove(topUI.GetMoneyIconPos(), 1.0f).OnComplete(() => {
                callback?.Invoke();
                FxModel moneyFx = FxCtrl.Instance.PlayFx(FxPrefabPath.idleMoneyBoom, fxNode);
                moneyFx.transform.position = topUI.GetMoneyTextPos();
                topUI.PlayMoneyAnim();
                AudioCtrl.Instance.PlayMultipleSound(GameAudio.vendingMoney);
                SG.ResourceManager.Instance.ReturnTransformToPool(model.transform);
            });
        }

        public static void PlayGetRewardFxUI(Vector3 rectPos,UnityAction callback,bool isClick=false)
        {
            Transform fxNode = UIController.Instance.FxRoot;

            if(isClick)
            {
                FxModel clickModel = FxCtrl.Instance.PlayFx(FxPrefabPath.idleClickFx, fxNode);
                clickModel.transform.position = rectPos;
                clickModel.transform.localScale = Vector3.one * 5;
            }

            FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleInvestorReward, fxNode);
            model.transform.position = rectPos;

            AudioCtrl.Instance.PlaySingleSound(GameAudio.moneyFxShow);
            model.transform.DOMove(topUI.GetMoneyIconPos(), 1.0f).OnComplete(() => {
                callback?.Invoke();
                
                topUI.PlayMoneyAnim();
                AudioCtrl.Instance.PlayMultipleSound(GameAudio.vendingMoney);
                SG.ResourceManager.Instance.ReturnTransformToPool(model.transform);

                FxModel moneyFx = FxCtrl.Instance.PlayFx(FxPrefabPath.idleMoneyBoom, fxNode);
                moneyFx.transform.position = topUI.GetMoneyTextPos();
                moneyFx.transform.localScale = Vector3.one;
                moneyFx.transform.localEulerAngles = Vector3.zero;
            });
        }

    }


}

