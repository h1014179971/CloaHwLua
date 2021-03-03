using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UIFramework;

namespace Delivery.Idle
{
    public class HideUI : MonoBehaviour, IPointerClickHandler
    {
        private bool start;
        private bool hasSet = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick");
            CanvasGroup contentCanvasGroup = UIController.Instance.UIContent.transform.GetOrAddComponent<CanvasGroup>();
            IdleTopUI topUI = FindObjectOfType<IdleTopUI>();
            CanvasGroup topCanvasGroup = topUI.transform.GetOrAddComponent<CanvasGroup>();
            if (hasSet)
            {
                contentCanvasGroup.alpha = 1.0f;
                topCanvasGroup.alpha = 1.0f;
            }
            else
            {
                contentCanvasGroup.alpha = 0;
                topCanvasGroup.alpha = 0;
            }
            hasSet = !hasSet;
        }
    }
}

