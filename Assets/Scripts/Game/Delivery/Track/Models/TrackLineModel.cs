using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using System.Linq;
using System;

namespace Delivery.Track
{
    public class TrackLineModel : MonoBehaviour
    {    
        private string _colorHex;             
        private List<Vector2> _points;    
        private List<LineTrack> _lineTracks = new List<LineTrack>();
        public VectorLine Line { get; set; } 

      
        public VectorLine ToDeleteLine { get; private set; }//待移除线段
        public List<LineTrack> toDeleteLineTracks = new List<LineTrack>();//待移除站点
       
        public string ColorHex { get { return _colorHex; } }
        public TrackUAVModel TrackUAVModel { get; set; }
        public List<LineTrack> LineTracks { get { return _lineTracks; } } 
        public List<Vector2> Points { get { return _points; } }        
        public bool IsLineEnd(int index)
        {
            if (index == Line.points2.Count - 2)
                return true;
            return false; 
        }



        private void Start()
        {
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Track_UAVOverDeleteLine, OnUAVOverDeleteLine);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Track_UAVOverDeleteLine, OnUAVOverDeleteLine);
        }

     
        public bool IsUseDeleteLine {
            get;
            set;
        }

        /// <summary>
        /// 无人机走过待删除路线
        /// </summary>
        /// <param name="args"></param>
        private void OnUAVOverDeleteLine(BaseEventArgs args)
        {
            if (ToDeleteLine == null || Line == null) return;
            EventArgsOne<string> argsOne = (EventArgsOne<string>)args;
            if(argsOne.param1==Line.name)
            {
                ToDeleteLine.points2.Clear();
                toDeleteLineTracks.Clear();
                ToDeleteLine.Draw();
            }
        }
        

        public void TouchBeganDraw(TrackModel houseModel,string colorHex)
        {
            if (_points == null)
                _points = new List<Vector2>();
            _points.Clear();
            _colorHex = colorHex;
            _points.Add(houseModel.RectTrans.anchoredPosition);
            Line.name = "line_" + colorHex;
            Line.points2 = _points;
            Line.color = Utils.HexToColor(_colorHex);
            Line.Draw();
            LineTrack lineTrack = new LineTrack();
            lineTrack.lineIndex = 0;
            lineTrack.pos = _points[0];
            lineTrack.trackModel = houseModel;

            //---------------------------------------------------------------------------设置站点位置信息
            lineTrack.enterAxis = TrackAxis.DownHorizon;
            lineTrack.enterIndex = 0;

            _lineTracks.Add(lineTrack);            
        }
        //移动画线
        public void TouchMoveDraw(List<Vector2> points,TrackModel siteModel = null)
        {
            if(siteModel == null)
                Line.points2 = new List<Vector2>(_points).Concat(points).ToList<Vector2>();
            else
            {
                _points = _points.Concat(points).ToList<Vector2>();
                LineTrack lineTrack = new LineTrack();
                lineTrack.lineIndex = _points.Count - 1;
                lineTrack.pos = _points[_points.Count - 1];
                lineTrack.trackModel = siteModel;
                
                ////----------------------------------------------------------------------设置站点位置信息
                ////int pointCount = _points.Count;
                //int trackCount = _lineTracks.Count;
                
                ////if(pointCount>=2)
                //if (trackCount >= 1)
                //{
                //    //LineDirection dir = GetTrackDirection(_points[pointCount - 2], _points[pointCount - 1]);
                //    LineDirection dir = GetTrackDirection(_points[lineTrack.lineIndex - 1], _points[lineTrack.lineIndex]);
                //    lineTrack.enterAxis = lineTrack.trackModel.GetPosAxis(dir);
                //    lineTrack.enterIndex = lineTrack.trackModel.GetEnableIndex(lineTrack.enterAxis);
                //    lineTrack.trackModel.OnLineEnter(lineTrack.enterAxis, lineTrack.enterIndex);
                //    lineTrack.exitAxis = TrackAxis.None;
                //}


                _lineTracks.Add(lineTrack); 
                Line.points2 = _points;
                //当画第一段线时， 发布无人机可以移动的消息
                if (_lineTracks.Count == 2 || (ToDeleteLine != null && ToDeleteLine.points2.Count > 1))
                {
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVMove, Line.name));
                }
            }
           
            Line.Draw();
        }


        //移动画线（当需要添加新站点时调用）
        public void TouchMoveDraw(List<Vector2> points, LineTrack lineTrack)
        {
            //如果当前路线最后一个点与待添加的点集合的第一个点重合，则不添加
            if(_points[_points.Count-1]==points[0])
            {
                points.RemoveAt(0);
            }
            _points = _points.Concat(points).ToList<Vector2>();

            lineTrack.lineIndex = _points.Count - 1;
            lineTrack.pos = _points[_points.Count - 1];

            _lineTracks.Add(lineTrack);
            Line.points2 = _points;
            //当画第一段线时， 发布无人机可以移动的消息
            if (_lineTracks.Count == 2 || (ToDeleteLine != null && ToDeleteLine.points2.Count > 1))
            {
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<string>(EnumEventType.Event_Track_UAVMove, Line.name));
            }

            Line.Draw();
        }


        private LineDirection GetTrackDirection(Vector2 startPos, Vector2 endPos)
        {
            float angle = Utility.Angle_360(startPos, endPos);
            //int times = (int)angle / 45;
            int times = Mathf.RoundToInt(angle) / 45;
            return (LineDirection)times;
        }


        /// <summary>
        /// 获取点的索引
        /// </summary>
        public int GetPointIndex(Vector2 point)
        {
        
            return _points.IndexOf(point);
        }

        private void CreateToDeleteLine()
        {
            ToDeleteLine = new VectorLine("toDeleteLine_" + ColorHex, new List<Vector2>(), null, 20f, LineType.Continuous, Joins.Fill);
            ToDeleteLine.rectTransform.SetParent(transform.parent);
            ToDeleteLine.rectTransform.anchoredPosition3D = Vector3.zero;
            ToDeleteLine.rectTransform.localScale = Vector3.one;
            Color color = Utils.HexToColor(_colorHex);
            color.a = 0.5f;
            ToDeleteLine.color = color;
        }

        //移动线段画线
        public void TouchRegmentMoveDraw(int lastIndex,List<Vector2> points1,int endIndex,List<Vector2> points2, TrackModel siteModel)
        {

            //------------------设置待移除线段-------------------
            if (ToDeleteLine == null)
            {
                CreateToDeleteLine();
            }
            if(!IsUseDeleteLine)
            {
                ToDeleteLine.points2.Clear();
                toDeleteLineTracks.Clear();
                for (int i = lastIndex; i <= endIndex; i++)
                {
                    ToDeleteLine.points2.Add(_points[i]);
                }
                //获取起始站点和结束站点
                LineTrack startTrack = null;
                LineTrack endTrack = null;
                for(int i=0;i<_lineTracks.Count;i++)
                {
                    if(_lineTracks[i].lineIndex==lastIndex)
                    {
                        startTrack = _lineTracks[i];
                    }
                    else if(_lineTracks[i].lineIndex==endIndex)
                    {
                        endTrack = _lineTracks[i];
                    }
                }
                toDeleteLineTracks.Add(startTrack);
                for(int i=lastIndex+1;i<endIndex;i++)
                {
                     toDeleteLineTracks.Add(null);
                }
               
                toDeleteLineTracks.Add(endTrack);
                ToDeleteLine.Draw();
                EventDispatcher.Instance.TriggerEvent(new EventArgsThree<string, int, int>(EnumEventType.Event_Track_AddMidTrackSite, Line.name, lastIndex, endIndex));
            }
           
            //-----------------------------------------



            for (int i = endIndex - 1; i > lastIndex; i--)
            {
                _points.RemoveAt(i);
            }
                
            int index = lastIndex + 1;
            for (int i = 0; i < points1.Count; i++)
            {
                _points.Insert(index, points1[i]);
                if (i == points1.Count - 1)
                {
                    InsertLineTrack(lastIndex, index, points1[i], siteModel);
                }
                index++;
            }
            for (int i = 0; i < points2.Count - 1; i++)
            {
                _points.Insert(index, points2[i]);
                index++;
            }

            
            Line.points2 = new List<Vector2>(_points);
            Line.Draw();
            
            ReLoadLineTrack(lastIndex);
        }
        private void InsertLineTrack(int lastIndex,int lineIndex,Vector2 pos,TrackModel siteModel)
        {
            int index =-1;
             for(int i = 0; i < _lineTracks.Count; i++)
            {
                LineTrack lTrack = _lineTracks[i];
                if(lTrack.lineIndex == lastIndex)
                {
                    index = i;
                    break;
                }
            }
            LineTrack lineTrack = new LineTrack();
            lineTrack.lineIndex = lineIndex;
            lineTrack.pos = pos;
            lineTrack.trackModel = siteModel;

            //------------------------------设置站点位置信息-------------------------------
            LineDirection dir = GetTrackDirection(_points[lineIndex - 1], _points[lineIndex]);
            lineTrack.enterAxis = lineTrack.trackModel.GetPosAxis(dir);
            lineTrack.enterIndex = lineTrack.trackModel.GetEnableIndex(lineTrack.enterAxis);
            lineTrack.trackModel.OnLineEnter(lineTrack.enterAxis, lineTrack.enterIndex);

            _lineTracks.Insert(index+1, lineTrack);
            //-----------------------------------更新下一个站点位置信息-----------------------------------
            LineTrack nextTrack = _lineTracks[index + 2];
            nextTrack.trackModel.OnLineExit(nextTrack.enterAxis, nextTrack.enterIndex);
            nextTrack.trackModel.OnLineExit(nextTrack.exitAxis, nextTrack.exitIndex);
            LineDirection nextTrackEnterDir = GetTrackDirection(_points[nextTrack.lineIndex - 1], _points[nextTrack.lineIndex]);
            nextTrack.enterAxis = nextTrack.trackModel.GetPosAxis(nextTrackEnterDir);
            nextTrack.enterIndex = nextTrack.trackModel.GetEnableIndex(nextTrack.enterAxis);
            nextTrack.trackModel.OnLineEnter(nextTrack.enterAxis, nextTrack.enterIndex);

        }



        //移动线段画线
        public void TouchRegmentMoveDraw(int lastIndex, List<Vector2> points1, int endIndex, List<Vector2> points2, LineTrack newLineTrack)
        {
            if(_points[lastIndex]==points1[0])
            {
                points1.RemoveAt(0);
            }
            if(_points.Count > endIndex + 2&&points2[points2.Count-1]==_points[endIndex+1])
            {
                points2.RemoveAt(points2.Count - 1);
            }

            //------------------设置待移除线段-------------------
            if (ToDeleteLine == null)
            {
                CreateToDeleteLine();
            }
            if (!IsUseDeleteLine)
            {
                ToDeleteLine.points2.Clear();
                toDeleteLineTracks.Clear();
                for (int i = lastIndex; i <= endIndex; i++)
                {
                    ToDeleteLine.points2.Add(_points[i]);
                }
                //获取起始站点和结束站点
                LineTrack startTrack = null;
                LineTrack endTrack = null;
                for (int i = 0; i < _lineTracks.Count; i++)
                {
                    if (_lineTracks[i].lineIndex == lastIndex)
                    {
                        startTrack = _lineTracks[i];
                    }
                    else if (_lineTracks[i].lineIndex == endIndex)
                    {
                        endTrack = _lineTracks[i];
                    }
                }
                //因为最后一个节点的位置会改变，但是线段中的点没有更新，所以需要重新修改
                if (endTrack != null)
                    ToDeleteLine.points2[ToDeleteLine.points2.Count - 1] = endTrack.pos;

                toDeleteLineTracks.Add(startTrack);
                for (int i = lastIndex + 1; i < endIndex; i++)
                {
                    toDeleteLineTracks.Add(null);
                }

                toDeleteLineTracks.Add(endTrack);
                ToDeleteLine.Draw();
                EventDispatcher.Instance.TriggerEvent(new EventArgsThree<string, int, int>(EnumEventType.Event_Track_AddMidTrackSite, Line.name, lastIndex, endIndex));
            }

            //-----------------------------------------



            for (int i = endIndex - 1; i > lastIndex; i--)
            {
                _points.RemoveAt(i);
            }

            int index = lastIndex + 1;
            for (int i = 0; i < points1.Count; i++)
            {
                _points.Insert(index, points1[i]);
                if (i == points1.Count - 1)
                {
                    InsertLineTrack(lastIndex, index, points1[i], newLineTrack);
                }
                index++;
            }
            //for (int i = 0; i < points2.Count - 1; i++)
            //{
            //    _points.Insert(index, points2[i]);
            //    index++;
            //}
            for (int i = 0; i < points2.Count ; i++)
            {
                if(i==points2.Count-1)
                {
                    _points[index] = points2[i];
                }
                else
                {
                    _points.Insert(index, points2[i]);
                }
                
                index++;
            }



            Line.points2 = new List<Vector2>(_points);
            Line.Draw();

            ReLoadLineTrack(lastIndex);
        }
        private void InsertLineTrack(int lastIndex, int lineIndex, Vector2 pos, LineTrack newLineTrack)
        {
            int index = -1;
            for (int i = 0; i < _lineTracks.Count; i++)
            {
                LineTrack lTrack = _lineTracks[i];
                if (lTrack.lineIndex == lastIndex)
                {
                    index = i;
                    break;
                }
            }
            newLineTrack.lineIndex = lineIndex;
            newLineTrack.pos = pos;
            
            _lineTracks.Insert(index + 1, newLineTrack);
            
        }



        private void ReLoadLineTrack(int lastIndex)
        {
            int index = -1;
            for(int i = lastIndex+1; i < _points.Count; i++)
            {
                index = i;
                Vector2 pos = _points[index];
                for(int j = 0; j < _lineTracks.Count; j++)
                {
                    LineTrack lineTrack = _lineTracks[j];
                    if (lineTrack.lineIndex <= lastIndex) continue;
                    if (lineTrack.pos == pos)
                        lineTrack.lineIndex = index;
                }
            }
        }
        public List<Vector2> GetPoints(int lastIndex, int endIndex)
        {
            List<Vector2> points = new List<Vector2>();
            for(int i = lastIndex; i <= endIndex; i++)
            {
                points.Add(_points[i]);
            }
            return points;
        }
        public void TouchEndDraw()
        {
            Line.points2 = new List<Vector2>(_points);
            Line.Draw();
            EndLineDraw();
        }
        private void EndLineDraw()
        {
            //if (_points.Count >= 2)
            //{
            //    Vector2 offset = _points[_points.Count - 1] - _points[_points.Count - 2];
            //    Vector2 endPos = _points[_points.Count - 1] + offset.normalized * 80;
            //    Line.points2.Add(endPos);
            //    Line.Draw();
            //}

            if (_lineTracks.Count >= 2)
            {
                LineTrack endLineTrack = _lineTracks[_lineTracks.Count - 1];
                TrackModel endTrack = endLineTrack.trackModel;
                int trackIndex = endLineTrack.lineIndex;
                Vector2 enterPos = _points[trackIndex];
                Vector2 lastPos = _points[trackIndex - 1];
                TrackAxis axis = endTrack.GetPosAxis(GetTrackDirection(enterPos, lastPos));//进入站点位置的轴的对称轴
                int index = endLineTrack.enterIndex;
                if (index != 0)
                {
                    if (index % 2 == 0)
                        index -= 1;
                    else
                        index += 1;
                }
                if (!endTrack.IndexExit(axis, index))
                {
                    index = endTrack.GetEnableIndex(axis);
                }
                Vector2 exitPos = endTrack.GetEnablePos(axis, index);

                endTrack.OnLineExit(endLineTrack.exitAxis, endLineTrack.exitIndex);
                endLineTrack.exitAxis = axis;
                endLineTrack.exitIndex = index;

                endTrack.OnLineEnter(endLineTrack.exitAxis, endLineTrack.exitIndex);

                Vector2 offset = exitPos - enterPos;
                if (Utility.IsAngle_45(enterPos, exitPos))
                {
                    Line.points2.Add(exitPos);
                }
                else
                {
                    Vector2 trackPos;
                    Vector3 trackScreenPos = RootCanvas.Instance.UICamera.WorldToScreenPoint(endTrack.transform.position);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, trackScreenPos, RootCanvas.Instance.UICamera, out trackPos);
                    Line.points2.Add(trackPos);
                    Line.points2.Add(exitPos);
                    offset = exitPos - trackPos;
                }
                Vector2 endPos = exitPos + offset.normalized * 60;
                Line.points2.Add(endPos);
                Line.Draw();

            }
        }
        //获取本线路上最后一个站点
        public LineTrack GetLastLineTrack()
        {
            if (_lineTracks == null || _lineTracks.Count <= 0)
                return null;
            return _lineTracks[_lineTracks.Count - 1];
        }
        public TrackModel GetLastTrackModel()
        {
            if (_lineTracks == null || _lineTracks.Count <= 0)
                return null;
            return _lineTracks[_lineTracks.Count - 1].trackModel;  
        }

        //添加已移除的点到待移除线段中
        private void AddRemovePointToDelete(LineTrack lineTrack)
        {
            //---------------设置待移除线段-----------------------------------------------
            if (_lineTracks.Count < 2) return;
            if (ToDeleteLine == null) CreateToDeleteLine();
            LineTrack lastTrack = null;
            if (_lineTracks.Count >= 2)
            {
                lastTrack = _lineTracks[_lineTracks.Count - 2];//上一个站点
            }

            //当前没有使用待删除线段
            if (!IsUseDeleteLine)
            {
                ToDeleteLine.points2.Clear();
                toDeleteLineTracks.Clear();
                //LineTrack lastTrack = _lineTracks[_lineTracks.Count - 2];
                int startIndex = 0;
                if (lastTrack != null)
                {
                    startIndex = lastTrack.lineIndex;
                }
                int endIndex = startIndex;
                ToDeleteLine.points2.Add(_points[startIndex]);
                toDeleteLineTracks.Add(lastTrack);
                while (_points[endIndex + 1] != lineTrack.pos && endIndex <= _points.Count - 2)
                {
                    endIndex++;
                    ToDeleteLine.points2.Add(_points[endIndex]);
                    toDeleteLineTracks.Add(null);
                }
                //if (_points[endIndex + 1] != lineTrack.pos)
                //{
                //    endIndex++;
                //    ToDeleteLine.points2.Add(_points[endIndex]);
                //    toDeleteLineTracks.Add(null);
                //}
                ToDeleteLine.points2.Add(_points[++endIndex]);
                toDeleteLineTracks.Add(lineTrack);
                EventDispatcher.Instance.TriggerEvent(new EventArgsThree<string, int, int>(EnumEventType.Event_Track_AddMidTrackSite, Line.name, startIndex, endIndex));
            }
            else
            {
                //当前存在待删除线段
                Vector2 deleteEndPos = ToDeleteLine.points2[ToDeleteLine.points2.Count - 1];
                Vector2 deleteStartPos = ToDeleteLine.points2[0];
                //待删除点为待删除线段最后一个点
                if (lineTrack.pos == deleteEndPos)
                {
                    //LineTrack lastTrack = _lineTracks[_lineTracks.Count - 2];
                    int lineTrackIndex = lineTrack.lineIndex;
                    Vector2 endPos = _points[0];
                    if (lastTrack != null)
                    {
                        endPos = lastTrack.pos;
                    }

                    while (_points.Count > 0 && lineTrackIndex>0 && endPos != _points[lineTrackIndex - 1])
                    {
                        lineTrackIndex--;
                        ToDeleteLine.points2.Add(_points[lineTrackIndex]);
                        toDeleteLineTracks.Add(null);
                    }

                  
                    ToDeleteLine.points2.Add(endPos);
                    toDeleteLineTracks.Add(lineTrack);
                }
                //待删除点为待删除线段第一个点
                else if (lineTrack.pos == deleteStartPos)
                {
                    
                    //将待移除线段的点从后往前再次添加到集合中
                   for(int i=ToDeleteLine.points2.Count-2;i>=0;i--)
                    {
                        ToDeleteLine.points2.Add(ToDeleteLine.points2[i]);
                        toDeleteLineTracks.Add(null);
                    }

                   
                    int lineTrackIndex = lineTrack.lineIndex;
                    Vector2 endPos = _points[0];
                    if (lastTrack != null)
                    {
                        endPos = lastTrack.pos;
                    }
                    while (_points.Count > 0 && lineTrackIndex > 0 && endPos != _points[lineTrackIndex - 1])
                    {
                        lineTrackIndex--;
                        ToDeleteLine.points2.Add(_points[lineTrackIndex]);
                        toDeleteLineTracks.Add(null);
                    }
                    
                    ToDeleteLine.points2.Add(endPos);
                    toDeleteLineTracks.Add(lastTrack);
                }
            }
            ToDeleteLine.Draw();
            //-----------------------------------------------------------------------------------
        }


        public void RemoveLastTrackModel()
        {  
            LineTrack lineTrack = _lineTracks[_lineTracks.Count - 1];
            //--------------------------------------从站点中移除线段进入的位置索引-----------------------------------
            lineTrack.trackModel.OnLineExit(lineTrack.enterAxis, lineTrack.enterIndex);
            lineTrack.trackModel.OnLineExit(lineTrack.exitAxis, lineTrack.exitIndex);

            AddRemovePointToDelete(lineTrack);

            lineTrack.trackModel.RemoveTrackLineModel(this);
            _lineTracks.RemoveAt(_lineTracks.Count - 1);
            RemovePointPos();
        }
        //移除存储的点
        public void RemovePointPos()
        {
            LineTrack lineTrack = GetLastLineTrack();
            if(lineTrack != null)
            {
                for (int i = _points.Count-1; i >= 0; i--)
                {
                    if (_points[i] == lineTrack.pos)
                        break;
                    else
                    {
                        _points.RemoveAt(i);
                    }
                }
            }
        }
        public TrackModel GetLineTrack(Vector2 pos)
        {
            for(int i =_lineTracks.Count - 1; i >= 0; i--)
            {
                LineTrack lineTrack = _lineTracks[i];
                if (lineTrack.pos == pos)
                    return lineTrack.trackModel;
            }
            return null;
        }
        public LineTrack GetLineTrack(int lineIndex)
        {
            for (int i = _lineTracks.Count - 1; i >= 0; i--)
            {
                LineTrack lineTrack = _lineTracks[i];
                if (lineTrack.lineIndex == lineIndex)
                    return lineTrack;
            }
            return null;
        }
        //经过仓库的时候是否可以画线
        public bool IsDrawInHouseModel(TrackModel trackModel)
        {
            if (trackModel is TrackHouseModel)
                return IsDrawInHouseModel(trackModel as TrackHouseModel);
            return false;
        }
        public bool IsDrawInHouseModel(TrackHouseModel houseModel)
        {
            if (_lineTracks == null || _lineTracks.Count <= 0)
                return true;
            if (_lineTracks[0].trackModel == houseModel && _lineTracks.Count >1)
            {
                for (int i = _lineTracks.Count - 1; i > 0; i--)
                {
                    if (_lineTracks[i].trackModel == houseModel)
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            } 
        }
        public LineTrack RegmentLastLineTrack(int index)
        {
            if (index < 0) return null;
            for(int i = 0; i < _lineTracks.Count; i++)
            {
                LineTrack lineTrack = _lineTracks[i];
                if (lineTrack.lineIndex == index)
                    return lineTrack;
            }
            index--;
            return RegmentLastLineTrack(index);
        }
        public  LineTrack RegmentEndLineTrack(int index)
        {
            index++;
            if (index >= Line.points2.Count) return null;
            for (int i = 0; i < _lineTracks.Count; i++)
            {
                LineTrack lineTrack = _lineTracks[i];
                if (lineTrack.lineIndex == index)
                    return lineTrack;
            }
            return RegmentEndLineTrack(index);
        }


    }
}

