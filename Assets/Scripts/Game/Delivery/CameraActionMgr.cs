using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Delivery
{
    public class CameraActionMgr : MonoBehaviour
    {
        private Transform audioTrigger;
        private Vector3 audioTriggerLocalPos;
        private static CameraActionMgr _instance;
        public static CameraActionMgr instance { get { return _instance; } }
        private PointerEventData pointerEventData;
        //灵敏度
        public float sensitivity = 0.2f;
        //移动速度
        public float speed = 200f;
        //摄像机距离
        public float distance = 10.0f;
        //摄像机正交尺寸
        public float size = 9.6f;
        //缩放系数
        public float scaleFactor = 1f;
        public float maxDistance = 30f;
        public float minDistance = 5f;
        public float maxSize = 16f;
        public float minSize = 6f;
        //记录上一次手机触摸位置判断用户是在左放大还是缩小手势
        private Vector2 oldPosition1;
        private Vector2 oldPosition2;
        private Vector2 lastSingleTouchPosition;
        private Vector3 m_CameraOffset;
        private Camera m_Camera;
        private Vector3 targetPos;
        private bool useMouse = false;
        //定义摄像机可以活动的范围
        public float xMin = -1000;
        public float xMax = 1000;
        public float zMin = -1000;
        public float zMax = 1000;
        //这个变量用来记录单指双指的变换
        private bool m_IsSingleFinger;
        private List<RaycastResult> results;
        private float cameraForwardDis;
        public bool isMove;
        public bool isFollowTarget;
        public GameObject targetGo;
        private bool isFocus;
        private float scaleActionDelay;
        private float preSize;
        public bool isTest;
        public Vector3 touchStart;
        private void Awake()
        {
            _instance = this;
            results = new List<RaycastResult>();
            isMove = false;
            isFocus = false;
            scaleActionDelay = 0;
        }
        #region 对外
        /// <summary>
        /// 摄像机跟随游戏物体
        /// </summary>
        /// <param name="target">目标</param>
        public void CameraFollowTarget(GameObject target, float _speed = 3)
        {
            isFollowTarget = true;
            targetGo = target;
            speed = _speed;
        }
        public void StopCameraFollow()
        {
            isFollowTarget = false;
            targetGo = null;
        }
        public void ResetAudioTriggerLocalPos()
        {
            if (audioTrigger != null)
            {
                audioTrigger.localPosition = audioTriggerLocalPos;
                size = preSize;
            }
        }
        /// <summary>
        /// 聚焦到一个位置
        /// </summary>
        /// <param name="focusPos">位置</param>
        /// <param name="delay">延迟</param>
        public void FocusCameraOnTarget(Vector3 focusPos, float delay = 0, bool isHalfScreen = false)
        {
        }
        public void FocusCameraOnTarget(string transName, float delay = 0)
        {
            GameObject focusGo = GameObject.Find(transName);
            if (focusGo != null)
            {
                Vector3 focusPos = focusGo.transform.position;
                FocusCameraOnTarget(focusPos, delay);
            }
        }
        public void FocusCameraOnTarget(GameObject focusGo, float delay = 0)
        {
            if (focusGo != null)
            {
                Vector3 focusPos = focusGo.transform.position;
                FocusCameraOnTarget(focusPos, delay);
            }
        }
        public void FocusNow(GameObject focusGo)
        {
            if (focusGo != null)
            {
                Vector3 focusPos = focusGo.transform.position;
                FocusCameraOnTarget(focusPos);
            }
        }
        #endregion
        void Start()
        {
            m_Camera = Camera.main;
            m_CameraOffset = m_Camera.transform.position;
            float angleX = m_Camera.transform.localRotation.eulerAngles.x;
            float sinF = angleX != 90 ? Mathf.Sin(angleX / 180 * Mathf.PI) : 1;
            cameraForwardDis = m_Camera.transform.position.y / sinF;
#if UNITY_EDITOR
            useMouse = true;
#endif
        }
        float inputTime = 0;
        bool tempPower;
        void Update()
        {
            //跟随中禁用交互
            if (isFollowTarget)
            {
                if (targetGo != null)
                {
                    Vector3 pos = targetGo.transform.position;
                    pos += -m_Camera.transform.forward * cameraForwardDis;
                    m_CameraOffset = new Vector3(Mathf.Clamp(pos.x, xMin, xMax), m_Camera.transform.position.y, Mathf.Clamp(pos.z, zMin, zMax));
                }
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (tempPower == true)
                {
                    touchStart = Input.mousePosition;
                    tempPower = false;
                    return;
                }
            }
            if (tempPower) return;
            //判断触摸数量为单点触摸
            if (Input.touchCount == 1)
            {
                pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.GetTouch(0).position;
                results.Clear();
                if (Input.GetTouch(0).phase == TouchPhase.Began || !m_IsSingleFinger)
                {
                    //在开始触摸或者从两字手指放开回来的时候记录一下触摸的位置
                    lastSingleTouchPosition = Input.GetTouch(0).position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    MoveCamera(Input.GetTouch(0).position);
                }
                m_IsSingleFinger = true;
            }
            else if (Input.touchCount > 1)
            {
                pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.GetTouch(0).position;
                results.Clear();
                //当从单指触摸进入多指触摸的时候,记录一下触摸的位置
                //保证计算缩放都是从两指手指触碰开始的
                if (m_IsSingleFinger)
                {
                    oldPosition1 = Input.GetTouch(0).position;
                    oldPosition2 = Input.GetTouch(1).position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    ScaleCamera();
                }
                m_IsSingleFinger = false;
            }
            //用鼠标的
            if (useMouse)
            {
                pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.mousePosition;
                results.Clear();
                if (Camera.main.orthographic)
                {
                    size -= Input.GetAxis("Mouse ScrollWheel") * scaleFactor;
                    size = Mathf.Clamp(size, minSize, maxSize);
                }
                else
                {
                    distance -= Input.GetAxis("Mouse ScrollWheel") * scaleFactor;
                    distance = Mathf.Clamp(distance, minDistance, maxDistance);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    lastSingleTouchPosition = Input.mousePosition;
                }
                if (Input.GetMouseButton(0))
                {
                    MoveCamera(Input.mousePosition);
                }
            }
        }
        /// <summary>
        /// 触摸缩放摄像头
        /// </summary>
        private void ScaleCamera()
        {
            //计算出当前两点触摸点的位置
            var tempPosition1 = Input.GetTouch(0).position;
            var tempPosition2 = Input.GetTouch(1).position;
            float currentTouchDistance = Vector3.Distance(tempPosition1, tempPosition2);
            float lastTouchDistance = Vector3.Distance(oldPosition1, oldPosition2);
            if (Camera.main.orthographic)
            {
                size -= (currentTouchDistance - lastTouchDistance) * scaleFactor * Time.deltaTime;
                size = Mathf.Clamp(size, minSize, maxSize);
            }
            else
            {
                //计算上次和这次双指触摸之间的距离差距
                //然后去更改摄像机的距离
                distance -= (currentTouchDistance - lastTouchDistance) * scaleFactor * Time.deltaTime;
                //把距离限制住在min和max之间
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }
            //备份上一次触摸点的位置，用于对比
            oldPosition1 = tempPosition1;
            oldPosition2 = tempPosition2;
            scaleActionDelay = 0.3f;
        }
        //Update方法一旦调用结束以后进入这里算出重置摄像机的位置
        private void LateUpdate()
        {
            if (Camera.main.orthographic)
            {
                targetPos = m_CameraOffset;
                if (Mathf.Abs(Camera.main.orthographicSize - size) <= 0.001f)
                {
                    Camera.main.orthographicSize = size;
                }
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, Time.deltaTime * speed);
            }
            else
            {
                targetPos = m_CameraOffset + m_Camera.transform.forward * -distance;
            }
            if (Vector3.SqrMagnitude(m_Camera.transform.position - targetPos) <= 0.00002f)
            {
                m_Camera.transform.position = targetPos;
                isMove = false;
                if (isFocus)
                {
                    isFocus = false;
                }
                if (Mathf.Abs(Camera.main.orthographicSize - size) <= 0.001f)
                {
                    Camera.main.orthographicSize = size;
                    if (scaleActionDelay > 0)
                    {
                        scaleActionDelay -= Time.deltaTime;
                        if (scaleActionDelay < 0)
                        {
                            isMove = false;
                        }
                    }
                }
                else
                {
                    isMove = true;
                }
            }
            else
            {
                isMove = true;
            }
            m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, targetPos, Time.deltaTime * speed);
        }
        private void MoveCamera(Vector3 scenePos)
        {
            if (m_IsSingleFinger || useMouse)
            {
                //Vector3 lastTouchPostion = m_Camera.ScreenToWorldPoint(new Vector3(lastSingleTouchPosition.x, lastSingleTouchPosition.y, -1));
                //Vector3 currentTouchPosition = m_Camera.ScreenToWorldPoint(new Vector3(scenePos.x, scenePos.y, -1));
                //Vector3 v = currentTouchPosition - lastTouchPostion;
                //m_CameraOffset += new Vector3(-v.x, 0, -v.z) * m_Camera.transform.position.y * sensitivity;
                //优化镜头
                Vector3 v = touchStart - scenePos;
                v = new Vector3(v.x, 0, v.y) * 0.003f;
                v = (Quaternion.Euler(transform.rotation.eulerAngles) * v);
                v.y = 0;
                m_CameraOffset += v;
                touchStart = Input.mousePosition;
                //把摄像机的位置控制在范围内
                if (isTest == false)
                {
                    m_CameraOffset = new Vector3(Mathf.Clamp(m_CameraOffset.x, xMin, xMax), m_CameraOffset.y, Mathf.Clamp(m_CameraOffset.z, zMin, zMax));
                }
                lastSingleTouchPosition = scenePos;
            }
        }
    }
}