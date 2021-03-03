using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Track
{
    public class TrackItemModel : MonoBehaviour
    {
        private RectTransform _rectTrans;
        private TrackItem _trackItem;
        public RectTransform RectTrans
        {
            get
            {
                if (_rectTrans == null)
                {
                    _rectTrans = GetComponent<RectTransform>();
                }
                return _rectTrans;
            }
        }
        public TrackItem TrackItem { get { return _trackItem; } }
        public void Init(TrackItem trackItem)
        {
            _trackItem = trackItem;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

