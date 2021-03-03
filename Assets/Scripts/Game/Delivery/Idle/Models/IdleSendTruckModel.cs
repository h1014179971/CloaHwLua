using Foundation;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.Idle
{
    public class IdleSendTruckModel : MonoBehaviour
    {
        private Sprite _resSprite;//货车资源

        private SpriteRenderer _sprite;
        private Animation _animation;
        private float _speed = 2;
       

        private bool isSend;//是否正在送货
        private bool isBack;//是否正在返回
        private bool isTruckBack;//是否正在倒车
        private bool showUnloadProcess;//是否显示卸货进度
        private int _itemId;//货物id
        private Vector3 _targetPos;//目标位置
        private Vector3 _startPos;//初始位置

        private List<Vector3> _goPoints;
        private List<Vector3> _backPoints;
        private List<Vector3> _truckBackPoints;
        private int _parkIndex;
        private int currentPointIndex;

        private Transform truckLevelUpFxNode;
        private float lastPlayFxTime;//上一次播放特效时间
        private float playFxTime=0.15f;//播放特效时间间隔

        private Transform processParentTrans;
        private RectTransform processParentRectTrans;
        private RectTransform processImageRectTrans;//卸货进度
        private Image processImage;//进度图
        private Transform processNode;//进度图位置节点
        private int totalVolume;//货车总的装货量
        private float currentProcess;//当前进度


        private Vector3 _dir;
        private Vector3 _parkPos;

        public SpriteRenderer Sprite
        {
            get
            {
                if (_sprite == null)
                    _sprite = this.GetComponentByPath<SpriteRenderer>("truck");
                return _sprite;
            }
        }
        public void Init(int itemId,int index)
        {
            lastPlayFxTime = Time.time;

            _parkIndex = index;
            _itemId = itemId;
            InitPoints();

            truckLevelUpFxNode = this.GetComponentByPath<Transform>("fxNode");
            _animation = this.GetComponentByPath<Animation>("truck");

            processNode = this.GetComponentByPath<Transform>("processNode");
            processParentTrans = UIController.Instance.GameRoot;
            processParentRectTrans = processParentTrans.GetComponent<RectTransform>();
            totalVolume = 0;

            transform.position = _startPos;
            _speed = 4.0f;
            InitSpriteRes();
            Sprite.sprite = _resSprite;
            OrderLayer();


            showUnloadProcess = false;
            currentProcess = 0;
             isSend = false;
            isBack = false;

            EventDispatcher.Instance.AddListener(EnumEventType.Event_CityTruck_UnLoad, UnloadItem);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_City_ChangeVolume, OnTruckVolumeLvUp);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_CityTruck_SetTotalVolume, OnSetTotalVolume);
        }

        private void InitPoints()
        {
            _goPoints = new List<Vector3>();
            _truckBackPoints = new List<Vector3>();
            _backPoints = new List<Vector3>();

            IdleCityTruckCtrl truckCtrl = IdleCityTruckCtrl.Instance;
            _startPos = truckCtrl.StartPos;
            Vector3 goPoint = truckCtrl.GetForwardPos(_parkIndex);
            Vector3 addPoint = truckCtrl.GetForwardPos(_parkIndex + 1);
            Vector3 parkPoint = truckCtrl.GetParkPos(_parkIndex);
            Vector3 backPoint = truckCtrl.GetbackPos(_parkIndex);
            _goPoints.Add(addPoint);
            _truckBackPoints.Add(goPoint);
            _truckBackPoints.Add(parkPoint);
            _backPoints.Add(backPoint);
            _backPoints.Add(truckCtrl.EndPos);

            _parkPos = parkPoint;
            _dir = backPoint - _parkPos;

        }


        void Update()
        {
            if(isSend)
            {
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed, Time.deltaTime);
                if (v3.magnitude > (_targetPos - transform.position).magnitude)
                {
                    transform.position = _targetPos;
                    currentPointIndex++;
                    if(currentPointIndex>_goPoints.Count-1)
                    {
                        isSend = false;
                        _animation.Stop();
                        //开始卸货
                        //StartCoroutine(UnloadItem());
                        StartCoroutine(StartTruckBack());
                    }
                    else
                    {
                        Vector3 target = _goPoints[currentPointIndex];
                        NextPoint(target);
                    }
                }
                else
                    transform.position += v3;
            }
            if(isTruckBack)
            {
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed, Time.deltaTime);
                if (v3.magnitude > (_targetPos - transform.position).magnitude)
                {
                    transform.position = _targetPos;

                    currentPointIndex++;
                    if (currentPointIndex > _truckBackPoints.Count - 1)
                    {
                        isTruckBack = false;
                        _animation.Stop();
                        //StartCoroutine(UnloadItem());
                        ArrivePark();
                    }
                    else
                    {
                        Vector3 target = _truckBackPoints[currentPointIndex];
                        NextPoint(target);
                    }
                }
                else
                    transform.position += v3;
            }

            if(isBack)
            {
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _offset.normalized * _speed, Time.deltaTime);
                if (v3.magnitude > (_targetPos - transform.position).magnitude)
                {
                    transform.position = _targetPos;

                    currentPointIndex++;
                    if(currentPointIndex>_backPoints.Count-1)
                    {
                        isBack = false;
                        _animation.Stop();

                        Hide();
                    }
                    else
                    {
                        Vector3 target = _backPoints[currentPointIndex];
                        NextPoint(target);
                    }
                }
                else
                    transform.position += v3;
            }

            if(showUnloadProcess)
            {
                UpdateProcessPosition();
                if(totalVolume>0)
                {
                    processImage.fillAmount += Time.deltaTime;
                    if (processImage.fillAmount >= currentProcess)
                    {
                        processImage.fillAmount = currentProcess;
                        if(currentProcess>=1.0f)
                        {
                            totalVolume = 0;
                            StartCoroutine(MoveBack());
                        }

                    }
                }
            }

        }

        //货车容量升级
        private void OnTruckVolumeLvUp(BaseEventArgs baseEventArgs)
        {
            if (Time.time - lastPlayFxTime > playFxTime)
            {
                lastPlayFxTime = Time.time;
                FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleGradeFx, truckLevelUpFxNode, true, 0.5f);
                model.transform.localScale = Vector3.one * 3;
            }

        }

        public void StartSendItem()
        {
            currentPointIndex = 0;
            
            Vector3 target = _goPoints[currentPointIndex];
            isSend = true;
            NextPoint(target);
            _animation.Play();
            AudioCtrl.Instance.PlayRolloffWorldSound(gameObject, GameAudio.cityTruckMove);
        }

        private IEnumerator StartTruckBack()
        {
            yield return new WaitForSeconds(1.0f);
            currentPointIndex = 0;
            Vector3 target = _truckBackPoints[currentPointIndex];
            isTruckBack = true;
            AudioCtrl.Instance.PlayRolloffWorldSound(gameObject, GameAudio.cityTruckStop);
            NextPoint(target);
            _animation.Play();
        }
      
        private void ArrivePark()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Guide_TargetArrive, gameObject.name));
            EventDispatcher.Instance.TriggerEvent(new EventArgsThree<int, Vector3, Vector3>(EnumEventType.Event_CityTruck_Arrive, _itemId, _parkPos, _dir));
            
        }

        private void UnloadItem(BaseEventArgs baseEventArgs)
        {
            if (totalVolume <= 0) return;
            EventArgsTwo<int, int> args = (EventArgsTwo<int, int>)baseEventArgs;
            if (_itemId != args.param1) return;

            if(!showUnloadProcess)
                ShowProcess();

            int restVolume = args.param2;
            //if (totalVolume <= 0)
            //{
            //    totalVolume = IdleCityMgr.Instance.GetVolume(PlayerMgr.Instance.PlayerCity.cityTruckVolumeLv);
            //}
            currentProcess= 1.0f - (float)restVolume / totalVolume;
            if (restVolume <= 0)
            {
                currentProcess = 1.0f;
            }
        }
        //设置大货车总容量
        private void OnSetTotalVolume(BaseEventArgs baseEventArgs)
        {
            EventArgsTwo<int, int> args = (EventArgsTwo<int, int>)baseEventArgs;
            if (_itemId != args.param1) return;
            totalVolume = args.param2;
        }

        private IEnumerator MoveBack()
        {
            yield return new WaitForSeconds(1.0f);
            HideProcess();
            currentPointIndex = 0;
            Vector3 target = _backPoints[currentPointIndex];
            isBack = true;
            NextPoint(target);
            _animation.Play();
            IdleCityTruckCtrl.Instance.TruckOutPark(_itemId);
        }


  
        private void InitSpriteRes()
        {
            int volumeLv = PlayerMgr.Instance.PlayerCity.cityTruckVolumeLv;
            _resSprite = AssetHelper.GetCityTruckSprite(_itemId, volumeLv);
        }


        private Vector3 _offset;
        private void NextPoint(Vector3 targetPos)
        {
            _targetPos = targetPos;
            _offset = _targetPos - transform.position;
            _offset.z = 0;
            if (_offset.y <= 0)
            {
              
                if (_offset.x > 0)
                    Sprite.flipX = true;
                else
                    Sprite.flipX = false;
            }
            else
            {
              
                if (_offset.x > 0)
                    Sprite.flipX = false;
                else
                    Sprite.flipX = true;
            }
        }

        private void ShowProcess()
        {
            GameObject process = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.truckProcess, true, 1);
            processImageRectTrans = process.GetComponent<RectTransform>();
            processImage = process.transform.GetChild(0).GetComponent<Image>();
            processImageRectTrans.SetParent(processParentTrans);
            processImageRectTrans.localScale = Vector3.one;
            processImage.fillAmount = 0;
            showUnloadProcess = true;
        }

        private void HideProcess()
        {
            showUnloadProcess = false;
            SG.ResourceManager.Instance.ReturnTransformToPool(processImageRectTrans.transform);
        }

        void UpdateProcessPosition()
        {
            if (processImageRectTrans == null) return;
            Vector2 v2 = Vector2.zero;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(processNode.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(processParentRectTrans, screenPos, RootCanvas.Instance.UICamera, out v2);
            processImageRectTrans.anchoredPosition3D = v2;
        }


        private void OrderLayer()
        {
            Timer.Instance.Register(0.1f, -1, (pare) => {
                _sprite.sortingOrder = -(int)(transform.position.y * 10);
            }).AddTo(gameObject);
        }
        private void Hide()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_CityTruck_UnLoad, UnloadItem);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeVolume, OnTruckVolumeLvUp);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_CityTruck_SetTotalVolume, OnSetTotalVolume);
            SG.ResourceManager.Instance.ReturnObjectToPool(this.gameObject);
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_CityTruck_UnLoad, UnloadItem);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_City_ChangeVolume, OnTruckVolumeLvUp);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_CityTruck_SetTotalVolume, OnSetTotalVolume);
        }
    }
}



