using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{

    public class IdleLockShelfAreaModel : MonoBehaviour
    {
        public int itemId;
        private GameObject collider;
        private void Start()
        {
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Game_Began, InitModel);

        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Game_Began, InitModel);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_StartFirstStep, HideCollider);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_GuideComplete, ShowCollider);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_UnlockShelf, HideShelfLock);
        }

       private void InitModel(BaseEventArgs baseEventArgs)
        {
            collider = GameObject.FindGameObjectWithTag(Tags.LockShelfArea);

            if (PlayerMgr.Instance.IsItemUnlock(itemId))
                this.gameObject.SetActive(false);
            else
            {
                EventDispatcher.Instance.AddListener(EnumEventType.Event_City_UnlockShelf, HideShelfLock);
                EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_StartFirstStep, HideCollider);
                EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_GuideComplete, ShowCollider);
            }
        }


        private void HideCollider(BaseEventArgs baseEventArgs)
        {
            collider.SetActive(false);
        }
        private void ShowCollider(BaseEventArgs baseEventArgs)
        {
            collider.SetActive(true);
        }

        private void HideShelfLock(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> arg = baseEventArgs as EventArgsOne<int>;
            if (arg.param1 != itemId) return;
            this.gameObject.SetActive(false);
        }

    }

}


