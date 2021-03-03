using Foundation;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Delivery.Idle
{
    public class IdleRemoverModel : MonoBehaviour
    {
        private enum RemoverState
        {
            None,
            MoveToTruck,
            MoveToShelf,
            LoadItem,
            UnloadItem,
            FadeOut,
            MoveBack
        }
        private MeshRenderer _meshRenderer;
        private SkeletonAnimation _skeletonAnimation;
        private Spine.AnimationState _spineAnimationState;
        private Spine.Skeleton _skeleton;
        private SpriteRenderer _shadowSprite;
        private Color _shadowColor;
        private IdleSiteModel _idleSiteModel;
        private IdleTruckModel _idleTruckModel;
        private int _itemId;
        private int _itemCount;
        private Vector3 _speed;//移动方向速度
        private int _speedTimes = 1;//移动速度倍数
        private Vector3 _target;
        private RemoverState currentState;
        private List<Vector3> _moveToTruckPoints = new List<Vector3>();
        private List<Vector3> _moveToShelfPoints = new List<Vector3>();
        private List<Vector3> _backPoints = new List<Vector3>();
        
        private int currentIndex;

        private string withoutItemAniState = "walk";
        private string withItemAniState = "walks";
        private string loadAniState = "idle";


        private void SetSpeed(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> args = baseEventArgs as EventArgsOne<int>;
            _speedTimes = args.param1;
        }

        public void Init(IdleSiteModel siteModel,IdleTruckModel idleTruckModel,Transform pointRoot)
        {
            _idleSiteModel = siteModel;
            _idleTruckModel = idleTruckModel;
            if (_spineAnimationState == null)
            {
                //SkeletonAnimation skeletonAnimation = GetComponent<SkeletonAnimation>();
                _skeletonAnimation = GetComponent<SkeletonAnimation>();
                _spineAnimationState = _skeletonAnimation.AnimationState;
                _skeletonAnimation.loop = true;
                _skeleton = _skeletonAnimation.Skeleton;
            }
            _meshRenderer = GetComponent<MeshRenderer>();
            Transform goPointRoot = pointRoot.Find("moveToTruckPoints");
            Transform moveToShelfRoot = pointRoot.Find("moveToShelfPoints");
            Transform backPointRoot = pointRoot.Find("backPoints");

            _moveToTruckPoints.Clear();
            _moveToShelfPoints.Clear();
            _backPoints.Clear();
            for (int i = 0; i < goPointRoot?.childCount; i++)
            {
                Vector3 vec = goPointRoot.GetChild(i).position;
                vec.z = 0;
                _moveToTruckPoints.Add(vec);
            }
            for(int i=0;i<moveToShelfRoot?.childCount;i++)
            {
                Vector3 vec = moveToShelfRoot.GetChild(i).position;
                vec.z = 0;
                _moveToShelfPoints.Add(vec);
            }
            for (int i = 0; i < backPointRoot?.childCount; i++)
            {
                Vector3 vec = backPointRoot.GetChild(i).position;
                vec.z = 0;
                _backPoints.Add(vec);
            }
            _skeletonAnimation.enabled = true;
            _skeleton.A = 0;
            currentIndex = 0;
            Vector3 target = _moveToTruckPoints[currentIndex];
            SetTarget(target);
            _spineAnimationState.SetAnimation(0, withoutItemAniState, true);
            currentState = RemoverState.MoveToTruck;
            _shadowSprite = transform.Find("shadow").GetComponent<SpriteRenderer>();
            _shadowColor = _shadowSprite.color;
            _shadowColor.a = 0;
            _shadowSprite.color = _shadowColor;
            OrderLayer();

            EventDispatcher.Instance.AddListener(EnumEventType.Event_SpecialEvent_ChangePostSiteStaffSpeed, SetSpeed);
        }
     
        private float fadeSpeed = 2f;
        private void FadeIn()
        {
            float dAlpha = fadeSpeed * Time.deltaTime;
            float result = _skeleton.A + dAlpha;
            if (1.0f - result < 0.05f)
            {
                _skeleton.A = 1.0f;
            }
            else
                _skeleton.A = result;
            _shadowColor.a = _skeleton.A;
            _shadowSprite.color = _shadowColor;
        }

        private void FadeOut()
        {
            float dAlpha = fadeSpeed * Time.deltaTime;
            float result = _skeleton.A - dAlpha;
            if (result < 0.05f)
            {
                _skeleton.A = 0.0f;
                SG.ResourceManager.Instance.ReturnObjectToPool(gameObject);
                currentState = RemoverState.None;
            }
            else
                _skeleton.A = result;
            _shadowColor.a = _skeleton.A;
            _shadowSprite.color = _shadowColor;
        }

        //设置目标点
        private void SetTarget(Vector3 target)
        {
            _target = target;
            _speed = (_target - transform.position).normalized * _idleSiteModel.IdleSiteTime.speed * _speedTimes;
            if (_target.x < transform.position.x)
                _skeleton.ScaleX = 1;
            else
                _skeleton.ScaleX = -1;
        }

        //移动到货车
        private void MoveToTruck()
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if ((_target - transform.position).magnitude < v3.magnitude)
            {
                transform.position = _target;
                currentIndex++;
                if(currentIndex>_moveToTruckPoints.Count-1)
                {
                    StartCoroutine(LoadItem());
                    return;
                }
                else
                {
                    Vector3 target = _moveToTruckPoints[currentIndex];
                    SetTarget(target);
                }
            }
            else
                transform.position += v3;
        }
        //移动到货架
        private void MoveToShelf()
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if ((_target - transform.position).magnitude < v3.magnitude)
            {
                transform.position = _target;
                currentIndex++;
                if (currentIndex > _moveToShelfPoints.Count - 1)
                {
                    
                    StartCoroutine(UnloadItem());//卸货
                    return;
                }
                else
                {
                    Vector3 target = _moveToShelfPoints[currentIndex];
                    SetTarget(target);
                   
                }
            }
            else
                transform.position += v3;
        }
        //从货架移动到驿站
        private void MoveBack()
        {
            Vector3 v3 = Vector3.Lerp(Vector3.zero, _speed, Time.deltaTime);
            if ((_target - transform.position).magnitude < v3.magnitude)
            {
                transform.position = _target;
                currentIndex++;
                if (currentIndex > _backPoints.Count - 1)
                {
                    //消失逻辑
                    //SG.ResourceManager.Instance.ReturnObjectToPool(gameObject);
                    currentState = RemoverState.FadeOut;
                    return;
                }
                else
                {
                    Vector3 target = _backPoints[currentIndex];
                    SetTarget(target);
                  
                }
            }
            else
                transform.position += v3;
        }
        //装货
        private IEnumerator LoadItem()
        {
           
            currentState = RemoverState.LoadItem;
            _spineAnimationState.SetAnimation(0, loadAniState, true);
            yield return new WaitForSeconds(1.0f);
            _idleTruckModel.SetNextTarget(this);
            currentIndex = 0;
            Vector3 target = _moveToShelfPoints[currentIndex];
            SetTarget(target);
            currentState = RemoverState.MoveToShelf;
            _spineAnimationState.SetAnimation(0, withItemAniState, true);
        }
        //卸货
        private IEnumerator UnloadItem()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Guide_TargetArrive, gameObject.name));
            _skeletonAnimation.enabled = false;
            currentState = RemoverState.UnloadItem;
            yield return new WaitForSeconds(0.5f);
            //卸货逻辑
            _idleSiteModel.LoadItem(_itemId, _itemCount);
            _itemCount = 0;
            _itemId = -1;
            currentIndex = 0;
            Vector3 target = _backPoints[currentIndex];
            SetTarget(target);
            currentState = RemoverState.MoveBack;
            _skeletonAnimation.enabled = true;
            _spineAnimationState.SetAnimation(0, withoutItemAniState, true);
        }

        //IdleTruckModel中调用
        public void SetItem(int itemId,int itemCount)
        {
            _itemId = itemId;
            _itemCount = itemCount;
        }

        private void Update()
        {
            if(currentState==RemoverState.MoveToTruck)
            {
                if(_skeleton.A<1.0f)
                    FadeIn();
                MoveToTruck();
            }
            else if(currentState==RemoverState.MoveToShelf)
            {
                MoveToShelf();
            }
            else if(currentState==RemoverState.MoveBack)
            {
                MoveBack();
            }
            else if(currentState==RemoverState.FadeOut)
            {
                FadeOut();
            }
        }

       


        private void OrderLayer()
        {
            Timer.Instance.Register(1, -1, (pare) => {
                _meshRenderer.sortingOrder = -(int)(transform.position.y * 10);
                _shadowSprite.sortingOrder = _meshRenderer.sortingOrder - 1;
            }).AddTo(gameObject);
        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_SpecialEvent_ChangePostSiteStaffSpeed, SetSpeed);
        }

    }
}

