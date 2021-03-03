using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.Track
{
    public class TrackUAVModel : MonoBehaviour
    {
        private Camera _uiCamera;
        private string _colorHex;
        private RectTransform _rectTrans;
        private TrackLineModel _trackLineModel; //线路 
        private bool _go = true;//顺时针移动   
        private bool _isMove = true;
        private float _speed = 200f;
        private float _sqrMinPixel = 64;
        private Vector2 _offset;//起始点与下一个点的向量
        private float _rotateAngle;//需要旋转的角度
        private float _rotateAngleSpeed = 10;//旋转速度
        private int _lineIndex = 0;  //下一个目标点的index
        private int _nowLineIndex = 0;//当前点的index

        private Vector2 targetPos;//下一个目标位置
        private bool deleteLineDir;//无人机在待删除线段上的运行方向


        private List<Vector2> _trackItemPos = new List<Vector2>();
        private List<TrackItemModel> _itemModels = new List<TrackItemModel>();
        private int _row = 2;//行数
        private int _col = 3;//列数  
        private float _width = 20;
        private float _height = 30;
        private float _loadTime = 0.5f;//装卸每个货物的时间
        private bool isPortItem;//是否正在装/卸货
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
        public void Init( TrackLineModel trackLineModel)
        {
            _uiCamera = RootCanvas.Instance.UICamera;
            _colorHex = trackLineModel.ColorHex;
            _trackLineModel = trackLineModel;
            Image image = this.GetComponent<Image>();
            image.color = Utils.HexToColor(_colorHex); 
            RectTrans.anchoredPosition3D = _trackLineModel.Points[0];
            _lineIndex = 1;
            _nowLineIndex = 0;
            //_offset = _trackLineModel.Points[_lineIndex] - RectTrans.anchoredPosition;
            //_rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, _trackLineModel.Points[_lineIndex]);
            targetPos = _trackLineModel.Points[_lineIndex];
            _offset = targetPos - RectTrans.anchoredPosition;
            _rotateAngle= Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
            CreatePos();
            LineTrack lineTrack = _trackLineModel.GetLineTrack(_nowLineIndex);
            PortItem(lineTrack);
        }
        private void CreatePos()
        {
            
            _trackItemPos.Clear();
            Vector2 pos = new Vector2((_col - 1) * 0.5f * _width, (_row - 1) * 0.5f * _height);
            for (int i = 0; i < _row; i++)
            {
                for (int j = 0; j < _col; j++)
                {
                    _trackItemPos.Add(pos - new Vector2(j * _width, i * _height));
                }
            }
        }


        private int currentIndexInDeleteLine = 0;
        /// <summary>
        /// 当线段添加中间站点时相应事件
        /// </summary>
        private void OnAddMidTrackSite(BaseEventArgs arg)
        {
            if (_trackLineModel == null || _trackLineModel.Line == null) return;
            EventArgsThree<string,int,int> argsThree = (EventArgsThree<string,int,int>)arg;
            string name = argsThree.param1;
            if (name != _trackLineModel.Line.name) return;
            int startIndex = argsThree.param2;
            int endIndex = argsThree.param3;
            
            //判断下一个目标索引是否在待删除线段上
            //当无人机向终点行进时，目标索引不包括待删除线段第一个点
            //当无人机向仓库行进时，目标索引不包括待删除线段最后一个点
            if(_lineIndex>=startIndex&&_lineIndex<=endIndex)
            {
                if(_go)
                {
                    if (_lineIndex == startIndex)
                    {
                        EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVOverDeleteLine, _trackLineModel.Line.name));
                        return;
                    }
                    //if (endIndex == _lineIndex)
                    //    currentIndexInDeleteLine = _trackLineModel.ToDeleteLine.points2.Count - 1;
                    //else
                    //    currentIndexInDeleteLine = 1;
                    currentIndexInDeleteLine = _trackLineModel.ToDeleteLine.points2.Count - 1 - (endIndex - _lineIndex);
                }
                else
                {
                    if (_lineIndex == endIndex)
                    {
                        EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVOverDeleteLine, _trackLineModel.Line.name));
                        return;
                    }
                    //if (_lineIndex == startIndex)
                    //    currentIndexInDeleteLine = 0;
                    //else
                    //    currentIndexInDeleteLine = _trackLineModel.ToDeleteLine.points2.Count - 2;
                    currentIndexInDeleteLine = _lineIndex - startIndex;
                }

                deleteLineDir = _go;
                targetPos = _trackLineModel.ToDeleteLine.points2[currentIndexInDeleteLine];
                _offset = targetPos - RectTrans.anchoredPosition;
                _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
                _trackLineModel.IsUseDeleteLine = true;
            }
            else
            {
                //当前无人机不在待移除线段上则删除待移除线段
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVOverDeleteLine, _trackLineModel.Line.name));
            }
        }

        /// <summary>
        /// 无人机开始移动
        /// </summary>
        private void OnUAVMove(BaseEventArgs args)
        {
            if (_isMove) return;
            EventArgsOne<string> argsOne = (EventArgsOne<string>)args;
            string name = argsOne.param1;
            if (name != _trackLineModel.Line.name) return;
            
            _go = true;
            _lineIndex = 1;
            _nowLineIndex = _lineIndex - 1;
            targetPos = _trackLineModel.Points[_lineIndex];
            _offset = targetPos - RectTrans.anchoredPosition;
            _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
            LineTrack lineTrack = _trackLineModel.GetLineTrack(_nowLineIndex);
            PortItem(lineTrack);
        }
       


        private void Start()
        {
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Track_AddMidTrackSite, OnAddMidTrackSite);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Track_UAVMove, OnUAVMove);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Track_AddMidTrackSite, OnAddMidTrackSite);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Track_UAVMove, OnUAVMove);
        }

        void Update()
        {
            if (!_isMove) return;
            Vector2 v2 = Vector2.Lerp(Vector2.zero, _offset.normalized * _speed, Time.deltaTime);
            if(v2.magnitude>(targetPos-RectTrans.anchoredPosition).magnitude)
                RectTrans.anchoredPosition = targetPos;
            else
                RectTrans.anchoredPosition += v2;

            RectTrans.rotation = Quaternion.Lerp(RectTrans.rotation, Quaternion.Euler(new Vector3(0, 0, _rotateAngle)), Time.deltaTime * _rotateAngleSpeed);
            //当初始值跟目标角度小于2，就将目标角度赋值给初始角度，让旋转角度是我们需要的角度
            if (Quaternion.Angle(RectTrans.rotation, Quaternion.Euler(new Vector3(0, 0, _rotateAngle))) < 2)
            {
                RectTrans.rotation = Quaternion.Euler(new Vector3(0, 0, _rotateAngle));
            }

            if (_go)
            {
                //当无人机处在已经删除的线段上
                if (_trackLineModel.IsUseDeleteLine)
                {
                    if ((targetPos - RectTrans.anchoredPosition).sqrMagnitude <  _sqrMinPixel)//长度平方小于一定值判断到达目标点
                    {
                        if (currentIndexInDeleteLine >= _trackLineModel.ToDeleteLine.points2.Count - 1)
                        {
                            _lineIndex = _trackLineModel.GetPointIndex(_trackLineModel.ToDeleteLine.points2[currentIndexInDeleteLine]);
                            //当前站点不在路线上
                            if(_lineIndex<=0)
                            {
                                //无人机行进到仓库，删除待删除线段
                                //if(_trackLineModel.Points.Count<=1 && _lineIndex==0)
                                if( _lineIndex==0)
                                {
                                    _trackLineModel.IsUseDeleteLine = false;
                                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVOverDeleteLine, _trackLineModel.Line.name));
                                    if (_trackLineModel.Points.Count <= 1) _isMove = false;//当前仓库没有站点则无人机停止运动
                                    return;
                                }
                                //当前站点不在路线上时，沿着待删除路线回程
                                _go = false;
                                Debug.Log($"#########################_lineIndex={_lineIndex}");
                                PortItem(_trackLineModel.toDeleteLineTracks[currentIndexInDeleteLine]);
                                targetPos = _trackLineModel.ToDeleteLine.points2[currentIndexInDeleteLine];
                                _offset = targetPos - RectTrans.anchoredPosition;
                                _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
                                return;
                            }
                            //当前站点为路线的最后一个站点，则回程
                            if (_lineIndex==_trackLineModel.Points.Count-1 || !deleteLineDir)
                            {
                                Debug.Log("#########################_lineIndex==_trackLineModel.Points.Count-1 || !deleteLineDir");
                                _go = false;
                            }
                            _nowLineIndex = _lineIndex - 1;
                            LineTrack lineTrack = _trackLineModel.GetLineTrack(_nowLineIndex);
                            PortItem(lineTrack);
                            _trackLineModel.IsUseDeleteLine = false;
                            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVOverDeleteLine, _trackLineModel.Line.name));
                            return;
                            
                        }
                        currentIndexInDeleteLine++;
                        targetPos = _trackLineModel.ToDeleteLine.points2[currentIndexInDeleteLine];

                        if (targetPos == _trackLineModel.ToDeleteLine.points2[0]) deleteLineDir = false;//如果经过的点与待删除点的起点重合，说明无人机处于返程阶段
                        PortItem(_trackLineModel.toDeleteLineTracks[currentIndexInDeleteLine-1]);
                        _offset = targetPos - RectTrans.anchoredPosition;
                        _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
                        
                        return;
                    }
                }

                //if ((_trackLineModel.Points[_lineIndex] - RectTrans.anchoredPosition).sqrMagnitude < _sqrMinPixel)//长度平方小于一定值判断到达目标点
                if ((targetPos - RectTrans.anchoredPosition).sqrMagnitude < _sqrMinPixel)//长度平方小于一定值判断到达目标点
                {
                    _lineIndex = _trackLineModel.GetPointIndex(targetPos);//重新获取当前到达点的索引（移动过程中索引可能会改变）
                    _lineIndex++;
                    _nowLineIndex = _lineIndex - 1;
                    if (_lineIndex >= _trackLineModel.Points.Count)
                    {
                        if (_trackLineModel.Points[0] == _trackLineModel.Points[_trackLineModel.Points.Count - 1])
                        {
                            _lineIndex = 1;
                            _nowLineIndex = _lineIndex - 1;
                        }
                        else
                        {
                            _go = false;
                            _lineIndex = _trackLineModel.Points.Count - 2;
                            _nowLineIndex = _lineIndex + 1;
                        }
                    }
                    //_offset = _trackLineModel.Points[_lineIndex] - RectTrans.anchoredPosition;
                    //_rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, _trackLineModel.Points[_lineIndex]);

                    targetPos = _trackLineModel.Points[_lineIndex];
                    _offset = targetPos - RectTrans.anchoredPosition;
                    _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);

                    LineTrack lineTrack = _trackLineModel.GetLineTrack(_nowLineIndex);
                    PortItem(lineTrack);
                }
            }
            else
            {
               
                //当无人机处在已经删除的线段上
                if (_trackLineModel.IsUseDeleteLine)
                {
                    if ((targetPos - RectTrans.anchoredPosition).sqrMagnitude < _sqrMinPixel)//长度平方小于一定值判断到达目标点
                    {
                     
                        if (currentIndexInDeleteLine <=0)
                        {
                            Vector2 startPos = _trackLineModel.ToDeleteLine.points2[0];
                            //在待删除线段中找到与当前点相同位置的点
                            for(int i=1; i< _trackLineModel.ToDeleteLine.points2.Count-1;i++)
                            {
                                if(startPos==_trackLineModel.ToDeleteLine.points2[i])
                                {
                                    currentIndexInDeleteLine = i;
                                    targetPos = _trackLineModel.ToDeleteLine.points2[currentIndexInDeleteLine];
                                    PortItem(_trackLineModel.toDeleteLineTracks[currentIndexInDeleteLine]);
                                    _offset = targetPos - RectTrans.anchoredPosition;
                                    _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
                                    _go = true;
                                    deleteLineDir = false;
                                    return;
                                }
                            }
                            _trackLineModel.IsUseDeleteLine = false;
                            
                            _lineIndex = _trackLineModel.GetPointIndex(_trackLineModel.ToDeleteLine.points2[currentIndexInDeleteLine]);
                            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVOverDeleteLine, _trackLineModel.Line.name));
                            if (_lineIndex <= 0)
                            {
                                _lineIndex = 1;
                                _go = true;
                                if (_trackLineModel.Points.Count<=1)
                                {
                                    _isMove = false;
                                    return;
                                }
                                targetPos = _trackLineModel.Points[_lineIndex];
                                _offset = targetPos - RectTrans.anchoredPosition;
                                _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
                                
                            }
                            _nowLineIndex = _lineIndex - 1;
                            LineTrack lineTrack = _trackLineModel.GetLineTrack(_nowLineIndex);
                            PortItem(lineTrack);

                            return;
                        }
                        currentIndexInDeleteLine--;
                        targetPos = _trackLineModel.ToDeleteLine.points2[currentIndexInDeleteLine];
                        PortItem(_trackLineModel.toDeleteLineTracks[currentIndexInDeleteLine + 1]);
                        _offset = targetPos - RectTrans.anchoredPosition;
                        _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);

                        return;
                    }
                }


                //if ((_trackLineModel.Points[_lineIndex] - RectTrans.anchoredPosition).sqrMagnitude < _sqrMinPixel)//长度平方小于一定值判断到达目标点
                if ((targetPos - RectTrans.anchoredPosition).sqrMagnitude < _sqrMinPixel)//长度平方小于一定值判断到达目标点
                {
                    _lineIndex = _trackLineModel.GetPointIndex(targetPos);//重新获取当前到达点的索引（移动过程中索引可能会改变）
                    _lineIndex--;
                    _nowLineIndex = _lineIndex + 1;
                    if (_lineIndex < 0)
                    {
                        _go = true;
                        _lineIndex = 1;
                        _nowLineIndex = _lineIndex - 1;
                    }
                    //_offset = _trackLineModel.Points[_lineIndex] - RectTrans.anchoredPosition;
                    //_rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, _trackLineModel.Points[_lineIndex]);

                    targetPos = _trackLineModel.Points[_lineIndex];
                    _offset = targetPos - RectTrans.anchoredPosition;
                    _rotateAngle = Utility.Angle_360(RectTrans.anchoredPosition, targetPos);
                    LineTrack lineTrack = _trackLineModel.GetLineTrack(_nowLineIndex);
                    PortItem(lineTrack);
                }
            }
        }

        //装货/卸货
        private void PortItem(LineTrack lineTrack)
        {
            
            if (lineTrack == null) return;              
            if (lineTrack.trackModel is TrackHouseModel)
            {
                StartCoroutine(LoadItem(lineTrack));
            }
            else if(lineTrack.trackModel is TrackSiteModel)
            {
                StartCoroutine(UnLoadItem(lineTrack));
            }
        }
        //装货
        private IEnumerator LoadItem(LineTrack lineTrack)
        {
            if (isPortItem) yield break;//如果正在装货，则不再进行装货操作
            isPortItem = true;
            _isMove = false;
            TrackHouseModel trackHouseModel = lineTrack.trackModel as TrackHouseModel;
            List<TrackItemModel> houseItems = trackHouseModel.GetItemModels();
            while (_itemModels.Count < _row * _col && houseItems.Count > 0)
            {
                yield return new WaitForSeconds(_loadTime);
                TrackItemModel itemModel = houseItems[0];
                itemModel.RectTrans.SetParent(RectTrans);
                itemModel.RectTrans.anchoredPosition = _trackItemPos[_itemModels.Count];
                _itemModels.Add(itemModel);
                trackHouseModel.UnLoadItem(itemModel);
            }
            _isMove = true;
            isPortItem = false;
        }
        //卸货
        private IEnumerator UnLoadItem(LineTrack lineTrack)
        {
            if (isPortItem) yield break;
            isPortItem = true;
            _isMove = false;
            TrackSiteModel trackSiteModel = lineTrack.trackModel as TrackSiteModel;
            while (_itemModels.Count > 0 && IsMateItem(trackSiteModel))
            {
               
                yield return new WaitForSeconds(_loadTime);
                TrackItemModel trackItemModel = null;
                for (int i = _itemModels.Count - 1; i >= 0; i--)
                {
                    if (_itemModels[i].TrackItem.Id == trackSiteModel.TrackSite.trackItemId)
                    {
                        trackItemModel = _itemModels[i];
                        break;
                    }
                }
                int index = _itemModels.IndexOf(trackItemModel);
                _itemModels.Remove(trackItemModel);
                ReLoadItem(index);
                SG.ResourceManager.Instance.ReturnObjectToPool(trackItemModel.gameObject);
            }
            _isMove = true;
            isPortItem = false;
        }
        //是否有匹配的item
        private bool IsMateItem(TrackSiteModel trackSiteModel)
        {
            for (int i = 0; i < _itemModels.Count; i++)
            {
                if (_itemModels[i].TrackItem.Id == trackSiteModel.TrackSite.trackItemId)
                    return true;
            }
            return false;
        }
        //重新排列
        private void ReLoadItem(int index = 0)
        {
            for (int i = index; i < _itemModels.Count; i++)
            {
                _itemModels[i].RectTrans.anchoredPosition = _trackItemPos[i];
            }
        }
    }
}

