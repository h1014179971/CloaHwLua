using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdleShelfModel
    {
        private int _shelfResId;//货架资源id
        private const string _itemName = "item";
        private Transform _shelfParentTrans;//父节点
        private Transform _shelfTrans;
        private Animation _anim;//升级动画
        private SpriteRenderer _shelfSprite;//货架图
        private Sprite _itemRes;//货物资源图
       
        private List<GameObject> _itemList;//货架上所有货物
        private int _currentItemCount;//当前货架上的货物数量
        private int _maxItemCount;//货架能放的最大货物数量

        private bool _IsEmpty;//是否装满

        private float _lastPlayFxTime;//上一次播放特效时间
        private float _dPlayerFxTime = 0.2f;//播放特效时间间隔

        public IdleShelfModel(int shelfResId,int itemId,Transform shelfParentTrans)
        {
           
            _itemList = new List<GameObject>();
            _itemRes = AssetHelper.GetItemBoxSprite(itemId);
            _currentItemCount = 0;
            _shelfParentTrans = shelfParentTrans;
            _shelfTrans = GameObject.Instantiate(AssetHelper.GetPrefab("shelf")).transform;
            _shelfTrans.SetParent(_shelfParentTrans);
            _shelfTrans.localPosition = Vector3.zero;
            _shelfTrans.localScale = Vector3.one;
            _shelfResId = shelfResId;
            _IsEmpty = false;
            Init();
            
        }
        //设置图片层级
        private void SetOrderLayer()
        {
            SpriteRenderer[] sprites = _shelfTrans.GetComponentsInChildren<SpriteRenderer>();
            for(int i=0;i<sprites.Length;i++)
            {
                sprites[i].sortingOrder= -(int)(_shelfTrans.position.y * 10);
            }
        }
        //初始化
        private void Init()
        {
            SetOrderLayer();

            IdleShelfRes shelfRes = IdleShelfResMgr.Instance.GetIdleShelRes(_shelfResId);
            int storeyCount = shelfRes.storey;
            _maxItemCount = storeyCount * 2;
            _anim = _shelfTrans.GetComponent<Animation>();
            _shelfSprite = _shelfTrans.GetComponent<SpriteRenderer>();
            _shelfSprite.sprite = AssetHelper.GetShelfSprite(_shelfResId);
            for (int i=0;i<_shelfTrans.childCount;i++)
            {
                GameObject item = _shelfTrans.Find(_itemName + i.ToString()).gameObject;
                item.SetActive(false);
                item.GetComponent<SpriteRenderer>().sprite = _itemRes;
                _itemList.Add(item);
            }
          
        }

        //设置显示的货物数量
        public void SetItemCount(int count,bool showFx=false)
        {
            int lastItemCount = _currentItemCount;
            _IsEmpty = false;
            if (count >= _maxItemCount)
            {
                count = _maxItemCount;
                _IsEmpty = true;
            }
            else if (count < 0)
                count = 0;
            _currentItemCount = count;
            if (lastItemCount == _currentItemCount) return;
            int dItemCount = _currentItemCount - lastItemCount;
            if (dItemCount > 0)
            {
                for (int i = 0; i < dItemCount; i++)
                {
                    GameObject item = _itemList[lastItemCount + i];
                    item.SetActive(true);
                    if (showFx)
                    {
                        FxModel fx = FxCtrl.Instance.PlayFx(FxPrefabPath.idleShelfItemShow, item.transform.position, -1);
                        fx.transform.GetChild(0).localScale = Vector3.one * 1.5f;
                    }
                }
            }
            else
            {
                for (int i = 0; i <-dItemCount; i++)
                {
                    _itemList[lastItemCount - i - 1].SetActive(false);
                }
            }
            if (showFx && IsInCamera())
                AudioCtrl.Instance.PlayMultipleSound(GameAudio.showItem);
        }
        //更新货架资源
        public void UpdateShelfRes(int shelfResId,float fxSize=-1)
        {
            if (_shelfResId == shelfResId) return;
            _shelfResId = shelfResId;
            IdleShelfRes shelfRes = IdleShelfResMgr.Instance.GetIdleShelRes(_shelfResId);
            int storeyCount = shelfRes.storey;
            _maxItemCount = storeyCount * 2;
            _anim = _shelfTrans.GetComponent<Animation>();
            _shelfSprite = _shelfTrans.GetComponent<SpriteRenderer>();
            _shelfSprite.sprite = AssetHelper.GetShelfSprite(_shelfResId);
            PlayShelfAni(true, fxSize);
        }

        //播放货架升级动画
        public void PlayShelfAni(bool playFx=true,float fxSize=-1)
        {
            if (Time.time - _lastPlayFxTime < _dPlayerFxTime) return;
            _lastPlayFxTime = Time.time;
            _anim.Play();
            if (playFx)
            {
                Vector3 pos = _shelfTrans.position;
                pos.y -= 0.36f;
                FxModel model = FxCtrl.Instance.PlayFx(FxPrefabPath.idleGradeFx, pos, 0.35f);
                if (fxSize <= 0)
                    fxSize = 3;
                model.transform.localScale = Vector3.one * fxSize;
            }

        }

        public int MaxItemCount
        {
            get
            {
                return _maxItemCount;
            }
        }

        private bool IsInCamera()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(_shelfTrans.position);
            if (screenPos.x > Screen.width || screenPos.x < 0 || screenPos.y > Screen.height || screenPos.y < 0)
                return false;
            return true;
        }


    }
}


