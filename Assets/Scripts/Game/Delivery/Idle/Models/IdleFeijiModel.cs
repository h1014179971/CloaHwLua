using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Foundation;
using UnityEngine.UI;
using UIFramework;

namespace Delivery.Idle 
{
    public class IdleFeijiModel : MonoBehaviour
    {
        [SerializeField]private Transform _feiji;
        [SerializeField] private Transform _bornPoint;
        [SerializeField] private SpriteRenderer _finger;
        private Transform _outPoint;
        private Vector3 _lastSpeedVec;//初始速度
        private Vector3 _speedVec;//速度
        private float _faultTime = 5;//容错时间（掐头去尾）
        private float _moveTime;//飞机飞完整个过程时间
        private bool _isMove;
        private bool _isGameStart;
        private float _t;
        private float _maxTime = 180;
        private RectTransform _processImageRectTrans;
        private Image _processImage;
        private int _clickCount;//点击次数
        private float _maxClickCount = 20;
        private bool _isMoveComplete;//飞机是否结束任务（包括点击完成）
        private IdleTopUI _idleTopUI;
        private float _speedTimes = 1.0f;//速度倍数
        public void Init()
        {
            
            _outPoint = _bornPoint.GetChild(0);
            _lastSpeedVec = (_outPoint.position - _bornPoint.position).normalized * 2;
            _speedVec = _lastSpeedVec;
            _speedTimes = 1.0f;
            _moveTime = (_outPoint.position - _bornPoint.position).magnitude / _speedVec.magnitude;
            GameObject process = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.truckProcess, true, 1);
            _processImageRectTrans = process.GetComponent<RectTransform>();
            _processImage = process.transform.GetChild(0).GetComponent<Image>();
            _processImageRectTrans.SetParent(UIController.Instance.GameRoot);
            _processImageRectTrans.localScale = Vector3.one;
            _processImage.fillAmount = 0;
            _isGameStart = true;
            _clickCount = 0;
        }

        void Update()
        {
            if (!_isGameStart) return;
            if(_isMove == true)
            {
                if (_speedTimes < 1.0f)
                    _speedTimes = Mathf.Lerp(_speedTimes, 1.0f, Time.deltaTime*1.5f);
                Vector3 v3 = Vector3.Lerp(Vector3.zero, _speedVec* _speedTimes, Time.deltaTime);
                if (v3.sqrMagnitude < (_outPoint.position - transform.position).sqrMagnitude)
                    transform.position += v3;
                else
                {
                    _isMove = false;
                    _feiji.position = _bornPoint.position;
                    MoveComplete();
                    _clickCount = 0;
                }
                Vector2 v2 = Vector2.zero;
                Vector2 screenPos = Camera.main.WorldToScreenPoint(_feiji.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(UIController.Instance.GameRoot as RectTransform, screenPos, RootCanvas.Instance.UICamera, out v2);
                _processImageRectTrans.anchoredPosition3D = v2+Vector2.up*100;
            }
            else
            {
                _t += Time.deltaTime;
                if(_t >=_maxTime)
                {
                    Timer.Instance.Register(_faultTime, 1, (para) =>
                    {
                        EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<Transform, float>(EnumEventType.Event_Feiji_Move, _feiji, _moveTime - _faultTime * 2));
                    }).AddTo(gameObject);
                    _isMoveComplete = false;
                    _isMove = true;
                    _t = 0;
                    _speedVec = _lastSpeedVec;
                    _processImage.fillAmount = 0;
                    if (!_finger.enabled)
                        _finger.enabled = true;
                    AudioCtrl.Instance.PlayMultipleSound(GameAudio.refreshFeiji);
                }
            }
        }

        public void OnTouchClick()
        {
            LogUtility.LogInfo($"OnTouchClick=={_isMove}");
            if (!_isMove) return;
            if (_finger.enabled)
                _finger.enabled = false;
            _clickCount++;
            if (_clickCount > _maxClickCount)
            {
                _speedVec *= 2;
                _speedTimes = 1.0f;
                MoveComplete();
                return;
            }
            _speedTimes = 0.2f;


            AudioCtrl.Instance.PlayMultipleSound(GameAudio.clickFeiji);
            _processImage.fillAmount = _clickCount / _maxClickCount;
            GameObject obj = SG.ResourceManager.Instance.GetObjectFromPool(PrefabName.xiangzi, true, 1);
            SpriteRenderer sprite = obj.transform.GetComponent<SpriteRenderer>();
            sprite.DOFade(1, 0);
            Vector3 position = _feiji.position;
            obj.transform.position = position;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(obj.transform.DOMove(position - Vector3.up * 3.5f, 1.1f).SetEase(Ease.Linear));
            sequence.Join(sprite.DOFade(0, 0.3f).SetDelay(0.8f).SetEase(Ease.Linear));
            sequence.OnComplete(delegate
            {
                //FxModel fx = FxCtrl.Instance.PlayFx(FxPrefabPath.idleShelfItemShow, obj.transform.position, -1);
                //AudioCtrl.Instance.PlayMultipleSound(GameAudio.xiangzidown);
                //fx.transform.GetChild(0).localScale = Vector3.one * 1.5f;
                //FxCtrl.Instance.PlayFx(FxPrefabPath.idleAddMoney, obj.transform.position, null);
                //FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleInvestorReward, UIController.Instance.FxRoot as RectTransform);
                //Vector3 v2 = Vector3.zero;
                //Vector2 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
                //RectTransformUtility.ScreenPointToWorldPointInRectangle(UIController.Instance.FxRoot as RectTransform, screenPos, RootCanvas.Instance.UICamera, out v2);
                //model.transform.position = v2;
                //model.transform.DOMove(FxEndPoint, 1.0f).OnComplete(() => {
                //    Long2 income = PlayerMgr.Instance.GetCityIncome();
                //    PlayerMgr.Instance.AddMoney(income);
                //    _idleTopUI.PlayMoneyAnim();
                //    AudioCtrl.Instance.PlayMultipleSound(GameAudio.vendingMoney);
                //    SG.ResourceManager.Instance.ReturnTransformToPool(model.transform);
                //});

                AudioCtrl.Instance.PlayMultipleSound(GameAudio.xiangzidown);
                IdleFxHelper.PlayGetRewardFxWorld(obj.transform, () =>
                {
                    Long2 income = PlayerMgr.Instance.GetCityIncome();
                    PlayerMgr.Instance.AddMoney(income);
                });

                SG.ResourceManager.Instance.ReturnObjectToPool(obj);
            });
        }
        private Vector3 FxEndPoint
        {
            get
            {
                if(_idleTopUI == null)
                    _idleTopUI = FindObjectOfType<IdleTopUI>();
                return _idleTopUI.GetMoneyIconPos(); ;
            }
        }
        private void MoveComplete()
        {
            if (!_isMoveComplete)
            {
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<Transform>(EnumEventType.Event_Feiji_MoveComplete, _feiji));
                _isMoveComplete = true;
            }
        }

}
}



