using Foundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Foundation;

namespace Delivery.Idle
{
    public class CitySiteTouch : BaseTouch
    {
        private Vector3 beginTouchPos;
        private RaycastHit result;
        private bool isAnyCollider = false;//是否碰撞到物体
        private CameraAotoMove autoMove;
        private CameraAotoMove AutoMove
        {
            get
            {
                if(autoMove==null)
                {
                    autoMove = GameObject.FindObjectOfType<CameraAotoMove>();

                }
                return autoMove;
            }
        }

        public override void OnTouchBegan(Vector3 touchPos)
        {
            base.OnTouchBegan(touchPos);
            beginTouchPos = touchPos;
            _eventData = new PointerEventData(EventSystem.current);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPos), out result) && !PointerOverUI(touchPos))
            {
                isAnyCollider = true;
            }
            else
                isAnyCollider = false;
            if (isAnyCollider)
            {
                string colliderTag = result.collider.gameObject.tag;
                if (colliderTag == Tags.Npc)
                {
                    IdleNpcTalkCtrl.Instance.CreateNpcTalk(result.collider.gameObject);
                }
                else if(colliderTag==Tags.RewardNpc)
                {
                    IdleNpcRewardModel rewardModel = result.collider.gameObject.GetComponent<IdleNpcRewardModel>();
                    if (rewardModel != null)
                        rewardModel.GetReward();
                }
                #region 居民楼逻辑
                IdleBuildModel buildModel = result.collider.gameObject.GetComponent<IdleBuildModel>();
                if (buildModel)
                    buildModel.OnTouchClick();
                #endregion
                #region 飞机逻辑
                IdleFeijiModel idleFeijiModel = result.collider.gameObject.GetComponent<IdleFeijiModel>();
                if (idleFeijiModel)
                    idleFeijiModel.OnTouchClick();
                #endregion
            }

        }

        public override void OnTouchEnded(Vector3 touchPos)
        {
            base.OnTouchEnded(touchPos);
            if (!isAnyCollider) return;
            float distance = (beginTouchPos - touchPos).magnitude;
            if (distance > 10.0f) return;
            string colliderTag = result.collider.gameObject.tag;
            if (colliderTag == Tags.CitySite)
            {
                Transform parent = result.collider.transform.parent;
                if (parent.tag != Tags.CitySite || AutoMove.IsMoving) return;
                IdleSiteModel siteModel = parent.GetComponent<IdleSiteModel>();
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Window_ShowPostSite, siteModel));
            }
            else if(colliderTag==Tags.DeliverySite)
            {
                if (AutoMove.IsMoving) return;
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowSite));
            }
            else if(colliderTag==Tags.TruckSite)
            {
                if (AutoMove.IsMoving) return;
                EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Window_ShowTruck));
            }
            else if(colliderTag==Tags.SiteLock)
            {
                IdleSiteModel siteModel = result.collider.transform.parent.GetComponent<IdleSiteModel>();
                if (siteModel == null) return;
                IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSite(siteModel.SiteId);
                PlayerSite playerSite = PlayerMgr.Instance.GetPlayerSite(siteModel.SiteId);
                Long2 cost = new Long2(idleSite.unlockPrice);
                if (PlayerMgr.Instance.PlayerCity.money < cost)
                {
                    IdleTipCtrl.Instance.ShowTip("金额不足");
                    return;
                }

                PlayerMgr.Instance.CutMoney(cost);
                playerSite.isLock = false;
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<IdleSiteModel>(EnumEventType.Event_Site_UnLock, siteModel));
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<TaskType>(EnumEventType.Event_Task_ProcessChange, TaskType.Task_UnlockPostSite));

                Vector3 pos = siteModel.transform.position;
                pos.z = 1;
                float cameraSize = -1;
                EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Vector3, float>(EnumEventType.Event_Camera_MoveToTarget, pos, cameraSize));

            }
            else if(colliderTag==Tags.LockShelfArea)
            {
                IdleLockShelfAreaModel lockShelfArea = result.collider.transform.parent.GetComponent<IdleLockShelfAreaModel>();
                if (lockShelfArea == null) return;
                int itemId = lockShelfArea.itemId;
                IdleSite idleSite = IdleSiteMgr.Instance.GetIdleSiteByItemId(itemId);
                if (idleSite == null) return;
                string tips = $"请先解锁{idleSite.name}";
                IdleTipCtrl.Instance.ShowTip(tips);
            }
            
        }

        private PointerEventData _eventData;
        private bool PointerOverUI(Vector2 mousePosition)
        {
            _eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(_eventData, results);
            if (results.Count > 0)
            {
                return true;
            }
            return false;
        }


    }
}

