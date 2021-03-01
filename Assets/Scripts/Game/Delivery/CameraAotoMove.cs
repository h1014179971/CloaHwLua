using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Foundation;
namespace Delivery.Idle
{

    public class CameraAotoMove : MonoBehaviour
    {
        private Vector3 startPos;
        private Vector3 targetPos;
        private float startSize;
        private float targetSize;
        private Camera sceneCamera;
        private CameraTouchMove touchMove;
        private bool moveToTarget;
        private bool moveBack;
        private bool focus;
        private bool followOther;
        private float moveTime =0.2f;
        private void Awake()
        {
            sceneCamera = GetComponent<Camera>();
            touchMove = GetComponent<CameraTouchMove>();
            startSize = sceneCamera.orthographicSize;
            startPos = transform.position;
            moveToTarget = false;
            moveBack = false;
            focus = false;
            followOther = false;
            isListening = false;
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Camera_MoveToTarget, OnMoveToTarget);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Camera_SimpleMoveToTarget, OnFocusToTarget);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Camera_MoveBack, OnMoveBack);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Camera_FollowOther, OnFollowOther);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Camera_FinishFollow, OnFinishFollow);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Guide_StartListenGuide, StartListenGuide);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Camera_MoveToTarget, OnMoveToTarget);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Camera_SimpleMoveToTarget, OnFocusToTarget);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Camera_MoveBack, OnMoveBack);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Camera_FollowOther, OnFollowOther);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Camera_FinishFollow, OnFinishFollow);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Guide_StartListenGuide, StartListenGuide);
        }

        public bool IsMoving
        {
            get
            {
                return moveToTarget || moveBack || focus;
            }
        }

        public bool Focus
        {
            get
            {
                return focus;
            }
            set
            {
                focus = value;
            }
        }

        private void OnMoveToTarget(BaseEventArgs baseEventArgs)
        {
            LogUtility.LogInfo("OnMoveToTarget");
            EventArgsTwo<Vector3, float> args = (EventArgsTwo<Vector3, float>)baseEventArgs;
            MoveToTarget(args.param1, args.param2);
        }

        private void OnSimpleMoveToTarget(BaseEventArgs baseEventArgs)
        {
            EventArgsTwo<Vector3, float> args = (EventArgsTwo<Vector3, float>)baseEventArgs;
            MoveToTarget(args.param1, args.param2, false);
        }


        private void MoveToTarget(Vector3 argPos, float size, bool saveStart=true)
        {
            LogUtility.LogInfo("MoveToTarget");
            if (IsMoving) return;
            LogUtility.LogInfo("IsMoving=true");
            if(saveStart)
            {
                startSize = sceneCamera.orthographicSize;
                startPos = transform.position;
            }
            if (size <= 0)
            {
                targetSize = sceneCamera.orthographicSize;
            }
            else
            {
                targetSize = size;
            }

            targetPos = new Vector3(argPos.x, argPos.y, startPos.z);
            if (argPos.z >= 0)
            {
                targetPos = touchMove.ResetPos(targetPos,targetSize);
            }
            LogUtility.LogInfo("set movetotarget");
            moveToTarget = true;
        }

        private void OnFocusToTarget(BaseEventArgs baseEventArgs)
        {
            EventArgsThree<Vector3, float,UnityAction> args = (EventArgsThree<Vector3, float, UnityAction>)baseEventArgs;
            StartCoroutine(FocusToTarget(args.param1, args.param2, args.param3));
        }
        
        private IEnumerator FocusToTarget(Vector3 pos,float size, UnityAction callback)
        {
            if (IsMoving) yield break;
            pos.z = transform.position.z;
            focus = true;
            Vector3 lastPos = transform.position;
            float lastSize = sceneCamera.orthographicSize;
            //transform.position = pos;
            while(sceneCamera.orthographicSize!=size)
            {
                sceneCamera.orthographicSize = Mathf.Lerp(sceneCamera.orthographicSize, size, moveTime);
                transform.position = Vector3.Lerp(transform.position, pos, moveTime);
                if (Mathf.Abs(sceneCamera.orthographicSize-size)<0.1f)
                {
                    transform.position = pos;
                    sceneCamera.orthographicSize = size;
                    if (callback != null)
                        callback.Invoke();
                }
                yield return null;
            }
            yield return new WaitForSecondsRealtime(1.5f);
            while (sceneCamera.orthographicSize != lastSize)
            {
                sceneCamera.orthographicSize = Mathf.Lerp(sceneCamera.orthographicSize, lastSize, moveTime);
                transform.position = Vector3.Lerp(transform.position, lastPos, moveTime);
                if (Mathf.Abs(sceneCamera.orthographicSize - lastSize) < 0.1f)
                {
                    transform.position = lastPos;
                    sceneCamera.orthographicSize = lastSize;
                }
                yield return null;
            }
            focus = false;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Camera_SimpleMoveArrive));
        }


        private void OnMoveBack(BaseEventArgs baseEventArgs)
        {
            //if (IsMoving) return;
            //EventArgsOne<Vector3> arg = (EventArgsOne<Vector3>)baseEventArgs;
            //Vector3 argPos = arg.param1;
            //Vector3 backPos = new Vector3(argPos.x, argPos.y, startPos.z);
            //if (argPos.z >= 0)
            //{
            //    backPos = touchMove.ResetPos(backPos,startSize);
            //}
            //startPos = backPos;
            //moveBack = true;

            if (IsMoving) return;
            EventArgsTwo<Vector3,float> arg = (EventArgsTwo<Vector3, float>)baseEventArgs;
            Vector3 argPos = arg.param1;
            Vector3 backPos = new Vector3(argPos.x, argPos.y, startPos.z);
            if (arg.param2 > 0)
            {
                startSize = arg.param2;
            }
            
            if (argPos.z >= 0)
            {
                backPos = touchMove.ResetPos(backPos, startSize);
            }
            startPos = backPos;
            moveBack = true;


        }

        private Transform targetTrans;
        private void OnFollowOther(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<Transform> argsOne = (EventArgsOne<Transform>)baseEventArgs;
            targetTrans = argsOne.param1;
            if (targetTrans == null) return;
            followOther = true;
        }

        private void OnFinishFollow(BaseEventArgs baseEventArgs)
        {
            //targetTrans = null;
            followOther = false;
        }

        private bool MoveTo(Vector3 _targetPos,float _targetSize)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, moveTime);
            sceneCamera.orthographicSize = Mathf.Lerp(sceneCamera.orthographicSize, _targetSize, moveTime);
            Vector3 offset = transform.position - _targetPos;
            if (offset.magnitude < 0.05f)
            {
                transform.position = _targetPos;
                sceneCamera.orthographicSize = _targetSize;
                return true;
            }
            return false;
        }

        private bool isListening;//是否正在监听引导
        private void StartListenGuide(BaseEventArgs baseEventArgs)
        {
            isListening = true;
        }


        private void Update()
        {
            if(moveToTarget)
            {
                if(MoveTo(targetPos,targetSize))
                {
                    moveToTarget = false;
                    if(isListening)
                    {
                        EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Guide_TargetArrive, "Camera"));
                        isListening = false;
                    }
                }
            }
            if(moveBack)
            {
                if(MoveTo(startPos,startSize))
                {
                    moveBack = false;
                }
            }

            if(followOther)
            {
                Vector3 pos = targetTrans.position;
                pos.z = transform.position.z;
                transform.position = Vector3.Lerp(transform.position, pos, moveTime);
            }
        }

    }


}

