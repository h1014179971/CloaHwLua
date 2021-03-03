using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.Track
{

    public enum LineDirection
    {
        Right,
        UpRight,
        Up,
        UpLeft,
        Left,
        DownLeft,
        Down,
        DownRight
    }
    /// <summary>
    /// 站点位置所处的轴线
    /// </summary>
    public enum TrackAxis
    {
        None,
        UpHorizon,//上方水平线上
        LeftVertical,//左侧竖直线上
        DownHorizon,//下方水平线
        RightVertical,//右侧竖直线
        UpLeft,//左上
        UpRight,//右上
        DownLeft,//左下
        DownRight,//右下
    }

    //线上的站点
    public class LineTrack
    {
        public int lineIndex; //线点的位置
        public Vector2 pos;
        public TrackModel trackModel;

        public int enterIndex;//线段进入站点位置的索引
        public TrackAxis enterAxis;//线段进入站点位置所处的轴线
        public int exitIndex;//线段离开站点位置的索引
        public TrackAxis exitAxis;//线段离开站点位置所处的轴线
    }

    public class TrackModel : MonoBehaviour
    {
        private RectTransform _rectTrans;
        private List<TrackLineModel> _trackLineModels;

        private List<int> _upHorizontalIndexList;//处于上方的水平轴位置索引
        private List<int> _downHorizontalIndexList;//处于下方的水平轴位置索引
        private List<int> _leftVerticalIndexList;//处于左侧的竖直轴位置索引
        private List<int> _rightVerticalIndexList;//处于右侧的竖直轴位置索引
        private List<int> _upLeftIndexList;//左上轴位置索引
        private List<int> _upRightIndexList;//右上轴位置索引
        private List<int> _downLeftIndexList;//左下轴位置索引
        private List<int> _downRightIndexList;//右下轴位置索引

        private int _maxPointCount = 3;//每个轴上最大的位置个数
       
        
        private void Awake()
        {
           
            _upHorizontalIndexList = new List<int>();
            _downHorizontalIndexList = new List<int>();
            _leftVerticalIndexList = new List<int>();
            _rightVerticalIndexList = new List<int>();
            _upLeftIndexList = new List<int>();
            _upRightIndexList = new List<int>();
            _downLeftIndexList = new List<int>();
            _downRightIndexList = new List<int>();
            for (int i = 0; i < _maxPointCount; i++)
            {
                _upHorizontalIndexList.Add(i);
                _downHorizontalIndexList.Add(i);
                _leftVerticalIndexList.Add(i);
                _rightVerticalIndexList.Add(i);

                _upLeftIndexList.Add(i);
                _upRightIndexList.Add(i);
                _downLeftIndexList.Add(i);
                _downRightIndexList.Add(i);
            }
        }

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
        public List<TrackLineModel> TrackLineModels
        {
            get
            {
                if (_trackLineModels == null)
                    _trackLineModels = new List<TrackLineModel>();
                return _trackLineModels;
            }
        }
        public void AddTrackLineModel(TrackLineModel trackLineModel)
        {
            if (!TrackLineModels.Contains(trackLineModel))
                TrackLineModels.Add(trackLineModel);
        }
        public void RemoveTrackLineModel(TrackLineModel trackLineModel)
        {
            if (TrackLineModels.Contains(trackLineModel))
                TrackLineModels.Remove(trackLineModel);
        }
        public bool IsHaveLineModel(TrackLineModel trackLineModel)
        {
            if (TrackLineModels.Contains(trackLineModel))
                return true;
            return false;
        }

        


        public int GetEnableIndex(LineDirection dir)
        {
            TrackAxis axis = GetPosAxis(dir);
            return GetEnableIndex(axis);
        }
        public int GetEnableIndex(TrackAxis axis)
        {
            switch (axis)
            {
                case TrackAxis.LeftVertical:
                    return _leftVerticalIndexList[0];
                case TrackAxis.DownHorizon:
                    return _downHorizontalIndexList[0];
                case TrackAxis.RightVertical:
                    return _rightVerticalIndexList[0];
                case TrackAxis.UpHorizon:
                    return _upHorizontalIndexList[0];
                case TrackAxis.UpLeft:
                    return _upLeftIndexList[0];
                case TrackAxis.UpRight:
                    return _upRightIndexList[0];
                case TrackAxis.DownLeft:
                    return _downLeftIndexList[0];
                case TrackAxis.DownRight:
                    return _downRightIndexList[0];
            }
            return -1;
        }

        public Vector2 GetEnablePos(LineDirection dir)
        {
            TrackAxis axis = GetPosAxis(dir);
            int index = GetEnableIndex(axis);
            return GetEnablePos(axis, index);
        }

        public Vector2 GetEnablePos(TrackAxis axis,int index)
        {
            Vector3 screenPos = RootCanvas.Instance.UICamera.WorldToScreenPoint(_rectTrans.position);
            Vector2 pos;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenPos, RootCanvas.Instance.UICamera, out pos);
            if (index < 0) return pos;
            float lineWidth = 20.0f;
            float offset = 0;
            if (index % 2 == 0)
            {
                offset += lineWidth * (index / 2);
            }
            else
            {
                offset -= lineWidth * (index / 2 + 1);
            }
            
            switch (axis)
            {
                case TrackAxis.DownHorizon:
                    pos.y -= lineWidth;
                    pos.x -= offset;
                    break;
                case TrackAxis.LeftVertical:
                    pos.x -= lineWidth;
                    pos.y += offset;
                    break;
                case TrackAxis.RightVertical:
                    pos.x += lineWidth;
                    pos.y -= offset;
                    break;
                case TrackAxis.UpHorizon:
                    pos.y += lineWidth;
                    pos.x += offset;
                    break;
                case TrackAxis.UpLeft:
                    if(index==0)
                    {
                        pos.x -= lineWidth;
                        pos.y += lineWidth;
                    }
                    else if(index==1)
                        pos.x -= lineWidth;
                    else if(index==2)
                        pos.y += lineWidth;
                    break;
                case TrackAxis.UpRight:
                    if (index == 0)
                    {
                        pos.x += lineWidth;
                        pos.y += lineWidth;
                    }
                    else if (index == 1)
                        pos.y += lineWidth;
                    else if (index == 2)
                        pos.x += lineWidth;
                    break;
                case TrackAxis.DownRight:
                    if (index == 0)
                    {
                        pos.x += lineWidth;
                        pos.y -= lineWidth;
                    }
                    else if (index == 1)
                        pos.x += lineWidth;
                    else if (index == 2)
                        pos.y -= lineWidth;
                    break;
                case TrackAxis.DownLeft:
                    if (index == 0)
                    {
                        pos.x -= lineWidth;
                        pos.y -= lineWidth;
                    }
                    else if (index == 1)
                        pos.y -= lineWidth;
                    else if (index == 2)
                        pos.x -= lineWidth;
                    break;
            }
            return pos;
        }
       
        public bool IndexExit(TrackAxis axis,int index)
        {
            switch (axis)
            {
                case TrackAxis.DownHorizon:
                    return _downHorizontalIndexList.Contains(index);
                case TrackAxis.DownLeft:
                    return _downLeftIndexList.Contains(index);
                case TrackAxis.DownRight:
                    return _downRightIndexList.Contains(index);
                case TrackAxis.LeftVertical:
                    return _leftVerticalIndexList.Contains(index);
                case TrackAxis.RightVertical:
                    return _rightVerticalIndexList.Contains(index);
                case TrackAxis.UpHorizon:
                    return _upHorizontalIndexList.Contains(index);
                case TrackAxis.UpLeft:
                    return _upLeftIndexList.Contains(index);
                case TrackAxis.UpRight:
                    return _upRightIndexList.Contains(index);
            }
            return false;
        }

        


        public TrackAxis GetPosAxis(LineDirection dir)
        {
            switch (dir)
            {
                case LineDirection.Down:
                    if (_upHorizontalIndexList.Count > 0)
                        return TrackAxis.UpHorizon;
                    return TrackAxis.None;
                case LineDirection.DownLeft:
                    //if (_upHorizontalIndexList.Count > 0)
                    //    return TrackAxis.UpHorizon;
                    //else if (_rightVerticalIndexList.Count > 0)
                    //    return TrackAxis.RightVertical;
                    if (_upRightIndexList.Count > 0)
                        return TrackAxis.UpRight;
                    return TrackAxis.None;
                case LineDirection.DownRight:
                    //if (_upHorizontalIndexList.Count > 0)
                    //    return TrackAxis.UpHorizon;
                    //else if (_leftVerticalIndexList.Count > 0)
                    //    return TrackAxis.LeftVertical;
                    if (_upLeftIndexList.Count > 0)
                        return TrackAxis.UpLeft;
                    return TrackAxis.None;
                case LineDirection.Left:
                    if (_rightVerticalIndexList.Count > 0)
                        return TrackAxis.RightVertical;
                    return TrackAxis.None;
                case LineDirection.Right:
                    if (_leftVerticalIndexList.Count > 0)
                        return TrackAxis.LeftVertical;
                    return TrackAxis.None;
                case LineDirection.Up:
                    if (_downHorizontalIndexList.Count > 0)
                        return TrackAxis.DownHorizon;
                    return TrackAxis.None;
                case LineDirection.UpLeft:
                    //if (_downHorizontalIndexList.Count > 0)
                    //    return TrackAxis.DownHorizon;
                    //else if (_rightVerticalIndexList.Count > 0)
                    //    return TrackAxis.RightVertical;
                    if (_downRightIndexList.Count > 0)
                        return TrackAxis.DownRight;
                    return TrackAxis.None;
                case LineDirection.UpRight:
                    //if (_downHorizontalIndexList.Count > 0)
                    //    return TrackAxis.DownHorizon;
                    //else if (_leftVerticalIndexList.Count > 0)
                    //    return TrackAxis.LeftVertical;
                    if (_downLeftIndexList.Count > 0)
                        return TrackAxis.DownLeft;
                    return TrackAxis.None;
            }
            return TrackAxis.None;
        }
      
        public void OnLineEnter(TrackAxis axis,int index)
        {
            switch (axis)
            {
                case TrackAxis.DownHorizon:
                    _downHorizontalIndexList.Remove(index);
                    break;
                case TrackAxis.LeftVertical:
                    _leftVerticalIndexList.Remove(index);
                    break;
                case TrackAxis.RightVertical:
                    _rightVerticalIndexList.Remove(index);
                    break;
                case TrackAxis.UpHorizon:
                    _upHorizontalIndexList.Remove(index);
                    break;
                case TrackAxis.UpLeft:
                    _upLeftIndexList.Remove(index);
                    break;
                case TrackAxis.UpRight:
                    _upRightIndexList.Remove(index);
                    break;
                case TrackAxis.DownLeft:
                    _downLeftIndexList.Remove(index);
                    break;
                case TrackAxis.DownRight:
                    _downRightIndexList.Remove(index);
                    break;
            }
            
        }


        public void OnLineExit(TrackAxis axis, int index)
        {
            switch (axis)
            {
                case TrackAxis.DownHorizon:
                    if(!_downHorizontalIndexList.Contains(index))
                    {
                        _downHorizontalIndexList.Add(index);
                        _downHorizontalIndexList.Sort();
                    }
                    break;
                case TrackAxis.LeftVertical:
                    if (!_leftVerticalIndexList.Contains(index))
                    {
                        _leftVerticalIndexList.Add(index);
                        _leftVerticalIndexList.Sort();
                    }
                    break;
                case TrackAxis.RightVertical:
                    if (!_rightVerticalIndexList.Contains(index))
                    {
                        _rightVerticalIndexList.Add(index);
                        _rightVerticalIndexList.Sort();
                    }
                    break;
                case TrackAxis.UpHorizon:
                    if (!_upHorizontalIndexList.Contains(index))
                    {
                        _upHorizontalIndexList.Add(index);
                        _upHorizontalIndexList.Sort();
                    }
                    break;
                case TrackAxis.UpLeft:
                    if (!_upLeftIndexList.Contains(index))
                    {
                        _upLeftIndexList.Add(index);
                        _upLeftIndexList.Sort();
                    }
                    break;
                case TrackAxis.UpRight:
                    if (!_upRightIndexList.Contains(index))
                    {
                        _upRightIndexList.Add(index);
                        _upRightIndexList.Sort();
                    }
                    break;
                case TrackAxis.DownLeft:
                    if (!_downLeftIndexList.Contains(index))
                    {
                        _downLeftIndexList.Add(index);
                        _downLeftIndexList.Sort();
                    }
                    break;
                case TrackAxis.DownRight:
                    if (!_downRightIndexList.Contains(index))
                    {
                        _downRightIndexList.Add(index);
                        _downRightIndexList.Sort();
                    }
                    break;
            }

        }


    }
}

