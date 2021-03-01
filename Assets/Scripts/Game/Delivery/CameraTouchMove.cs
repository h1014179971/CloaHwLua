using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Foundation;

namespace Delivery.Idle
{
    [ExecuteInEditMode]
    public class CameraTouchMove : BaseTouch
    {
        private Camera _camera;
        private PointerEventData _eventData;
        private Vector3 _startTouchPos;
        private Vector3 _cameraTargetPos;
        private float _x;
        private float _y;
        private Vector3 _center;
        private float _width;
        private float _height;
        [SerializeField]private float _minSize = 6;
        [SerializeField]private float _maxSize = 16;
        private float _touchMag;
        //移动速度
        [SerializeField]private float _speed = 1.5f;
        [SerializeField] private Vector3 _movePos;
        [SerializeField]private Transform _trans1;
        [SerializeField]private Transform _trans2;
        private bool _isOneTouch;
        private bool _isTwoTouch;
        //这个变量用来记录单指双指的变换
        private bool _isSingleFinger;

        private CameraAotoMove _autoMove;
        public override void Start()
        {
            base.Start();
            _camera = this.GetComponent<Camera>();
            _eventData = new PointerEventData(EventSystem.current);
            float unit = FixScreen.height / (FixScreen.idleCameraSize * 2);//像素与单位1比为100:1
            float size = Screen.height / 2/ unit;
            _camera.orthographicSize = size;
            _width = Mathf.Abs(_trans1.position.x - _trans2.position.x);
            _height = Mathf.Abs(_trans1.position.y - _trans2.position.y);
            _x = _width / 2 - (Screen.width * size / Screen.height); 
            _y = _height / 2 - size;
            _center = (_trans1.position + _trans2.position) * 0.5f;

            _autoMove = FindObjectOfType<CameraAotoMove>();
        }
        public override void Update()
        {

        }
        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
                OnTouchBegan(Input.mousePosition);
            else if(Input.GetMouseButton(0))
                OnTouchMoved(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0))
                OnTouchEnded(Input.mousePosition);
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                ScaleCamera(Input.GetAxis("Mouse ScrollWheel"));
            }

#else
            if (Input.touchCount == 1)
            {
                
                if (!_isSingleFinger)
                    _startTouchPos = Input.mousePosition;
                _isSingleFinger  = true;
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                    OnTouchBegan(Input.mousePosition);
                else if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    OnTouchMoved(Input.mousePosition);
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    OnTouchEnded(Input.mousePosition);
            }
            else if(Input.touchCount > 1)
            {
                if(_isSingleFinger)
                    _touchMag = (Input.GetTouch(1).position - Input.GetTouch(0).position).magnitude;
                _isSingleFinger = false;
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
                    OnTwoTouchBegan(Input.GetTouch(0).position, Input.GetTouch(1).position);
                else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                    OnTwoTouchMoved(Input.GetTouch(0).position, Input.GetTouch(1).position);
                else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
                    OnTwoTouchEnd(Input.GetTouch(0).position, Input.GetTouch(1).position);
                
            }
#endif
        }
        public override void OnTouchBegan(Vector3 touchPos)
        {
            if (_autoMove.IsMoving) return;//如果镜头正在移动，则屏蔽玩家移动镜头操作
            if (PointerOverUI(touchPos))
                return;
            _isOneTouch = true;
            _startTouchPos = touchPos;
            _cameraTargetPos = _camera.transform.position;
        }
        public override void OnTouchMoved(Vector3 touchPos)
        {
            if (PointerOverUI(touchPos))
                return;
            if (!_isOneTouch) return;
            _movePos = -touchPos + _startTouchPos;
            if (_movePos.magnitude < 0.1f) return;
            float size = _camera.orthographicSize * 2 / Screen.height;
            _cameraTargetPos += (_movePos * size);
            float x = Mathf.Clamp(_cameraTargetPos.x, _center.x - _x, _center.x + _x);
            float y = Mathf.Clamp(_cameraTargetPos.y, _center.y - _y, _center.y + _y);
            _cameraTargetPos = new Vector3(x, y, _cameraTargetPos.z);
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _cameraTargetPos, 1);
            _startTouchPos = touchPos;
        }
        public override void OnTouchEnded(Vector3 touchPos)
        {
            _isOneTouch = false;
            _cameraTargetPos = _camera.transform.position;
        }
        private void OnTwoTouchBegan(Vector3 touchPos1, Vector3 touchPos2)
        {
            if (_autoMove.IsMoving) return;//如果镜头正在移动，则屏蔽玩家移动镜头操作
            if (PointerOverUI(touchPos1) || PointerOverUI(touchPos2))
                return;
            _isTwoTouch = true;
            _touchMag = (touchPos2 - touchPos1).magnitude;
        }
        private void OnTwoTouchMoved(Vector3 touchPos1,Vector3 touchPos2)
        {
            if (PointerOverUI(touchPos1) || PointerOverUI(touchPos2))
                return;
            if (!_isTwoTouch) return;
            if (_touchMag == 0)
            {
                _touchMag = (touchPos2 - touchPos1).magnitude;
                return;
            }
            float mag = (touchPos2 - touchPos1).magnitude;
            float scale = Mathf.Lerp(0,mag -  _touchMag, Time.deltaTime * 0.5f);
            ScaleCamera(scale);
            _touchMag = mag;
        }
        private void OnTwoTouchEnd(Vector3 touchPos1, Vector3 touchPos2)
        {
            _isTwoTouch = false;
            _touchMag = 0;
        }
        private  void ScaleCamera(float scale)
        {
            if (Mathf.Abs(scale) <= 0.01f) return;
            //float s = Mathf.Lerp(0, scale, 1);
            float s = Camera.main.orthographicSize;
            s -= scale;
            //_camera.orthographicSize -= scale;
            _camera.orthographicSize = Mathf.Clamp(s, _minSize, _maxSize);
            float size = _camera.orthographicSize;
            _x = _width / 2 - (Screen.width * size / Screen.height);
            _y = _height / 2 - size;
            float x = Mathf.Clamp(_camera.transform.position.x, _center.x - _x, _center.x + _x);
            float y = Mathf.Clamp(_camera.transform.position.y, _center.y - _y, _center.y + _y);
            //_camera.transform.position = Vector3.Lerp(_camera.transform.position, new Vector3(x, y, _camera.transform.position.z), Time.deltaTime * _speed);
            _camera.transform.position = new Vector3(x, y, _camera.transform.position.z);
        }
        private bool PointerOverUI(Vector2 mousePosition)
        {
            _eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(_eventData, results);
            if (results.Count > 0)
                return true;
            return false;
        }

        public Vector3 ResetPos(Vector3 pos,float size)
        {
            if(size<=0)
            {
                size = _camera.orthographicSize;
            }
            _x = _width / 2 - (Screen.width * size / Screen.height);
            _y = _height / 2 - size;
            pos.x = Mathf.Clamp(pos.x, _center.x - _x, _center.x + _x);
            pos.y = Mathf.Clamp(pos.y, _center.y - _y, _center.y + _y);
            return pos;
        }
    }
}


