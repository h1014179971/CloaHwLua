using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Track
{
    public class TrackLauncher : MonoBehaviour
    {
        [SerializeField]private TrackDrawGrid _trackDrawGrid;
        [SerializeField]private TrackModelCtrl _trackModelCtrl;
        private TrackGameState _trackGameState;
        private int _loadDataLength = 4;                                              
        void Awake()
        {
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Load_TrackData, LoadTrackData);
            TrackHouseMgr.Instance.Init();
            TrackSiteMgr.Instance.Init();
            TrackItemMgr.Instance.Init();
            TrackLevelMgr.Instance.Init(1001);
            RectTransform warehousebg = this.GetComponentByPath<RectTransform>("bg/warehousebg");
            RectTransform warehouseParent = this.GetComponentByPath<RectTransform>("bg/warehouse");
            UIAutoFitScreen.Instance.SetWarehouseAutoFit(warehousebg, warehouseParent);
        }
        private void LoadTrackData(BaseEventArgs args)
        {
            _loadDataLength--;
            if (_loadDataLength > 0) return;
            _trackGameState = TrackGameState.Began;
            _trackModelCtrl.Init(_trackDrawGrid);

        }
        public TrackGameState TrackGameState
        {
            get { return _trackGameState; }
            set { _trackGameState = value; }
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Load_TrackData, LoadTrackData);
        } 
    }
}


