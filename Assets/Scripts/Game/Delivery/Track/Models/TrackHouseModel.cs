using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;

namespace Delivery.Track
{
    public class TrackHouseModel : TrackModel
    {
        private TrackHouse _trackHouse;
        private List<Vector2> _trackItemPos = new List<Vector2>();
        private List<TrackItemModel> _itemModels = new List<TrackItemModel>();
        TimerEvent _timer = null;
        public TrackHouse TrackHouse
        {
            get { return _trackHouse; }
            set { _trackHouse = value; }
        }
        public void Init(TrackHouse trackHouse)
        {
            _trackHouse = trackHouse;
            if (_trackHouse.isLock) return;
            CreatePos();
            CreateItem();
        }
        private void CreatePos()
        {   
            _trackItemPos.Clear();
            Vector2 pos = new Vector2(-(_trackHouse.col - 1) * 0.5f * _trackHouse.width, (_trackHouse.row - 1) * 0.5f * _trackHouse.height);
            for (int i = 0; i < _trackHouse.row; i++)
            {
                for (int j = 0; j < _trackHouse.col; j++)
                {
                    _trackItemPos.Add(pos + new Vector2(j * _trackHouse.width, -i * _trackHouse.height));
                }
            }
        }
        private void CreateItem()
        {
            _timer = Timer.Instance.Register(2, -1, (pare) => {
                if (_itemModels.Count >= _trackHouse.row * _trackHouse.col) return;
                int id = Random.Range(5001, 5007);
                TrackItem trackItem = TrackItemMgr.Instance.GetTrackItemById(id);
                string prefabPath = PrefabPath.itemLittlePath + "item_" + id;
                GameObject littleObj = SG.ResourceManager.Instance.GetObjectFromPool(prefabPath, true, 1);
                TrackItemModel trackItemModel = littleObj.GetOrAddComponent<TrackItemModel>();
                trackItemModel.Init(trackItem);
                trackItemModel.RectTrans.SetParent(RectTrans);
                trackItemModel.RectTrans.anchoredPosition3D = _trackItemPos[_itemModels.Count];
                trackItemModel.RectTrans.localEulerAngles = Vector3.zero;
                trackItemModel.RectTrans.localScale = Vector3.one;
                _itemModels.Add(trackItemModel);
            }).AddTo(gameObject);
        }

        public List<TrackItemModel> GetItemModels()
        {
            return _itemModels;
        }

        public void UnLoadItem(TrackItemModel trackItemModel)
        {
            if (_itemModels.Contains(trackItemModel))
                _itemModels.Remove(trackItemModel);
            if (_itemModels.Count > 0)
                ReLoadItem();
        }
        //重新排列
        private void ReLoadItem()
        {
            for (int i = 0; i < _itemModels.Count; i++)
            {
                _itemModels[i].RectTrans.anchoredPosition = _trackItemPos[i];
            }
        }
    }
}


