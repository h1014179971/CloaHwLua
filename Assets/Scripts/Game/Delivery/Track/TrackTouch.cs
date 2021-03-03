using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using Vectrosity;
using UnityEngine.EventSystems;
using Foundation;
using System.Linq;

namespace Delivery.Track
{
    public class TrackTouch : BaseTouch
    {
        public Texture2D _lineTex;
        public int _maxPoints = 5000;
        public float _lineWidth = 4.0f;
        public int _minPixelMove = 5;
        private VectorLine _line;
        private Vector3 _previousPosition;
        private int _sqrMinPixelMove;
        PointerEventData _eventData;    
        private Camera _uiCamera;
        private Transform _lineParent;
        private Transform _uavParent;
        
        private TrackLineModel _touchTrackLineModel;//点击的线
        private Vector2 _v1;
        private Vector2 _v2;
        private int _mulV1 = -10;
        private TrackModel _touchRemoveTrackModel = null;//删除线段时最后一个item
        private enum TouchState
        {
            None,
            Ray,
            Regment
        }                              
        private TouchState _touchState = TouchState.None;
        #region 线段点击               
        private LineTrack _lineLastLineTrack;
        private LineTrack _lineEndLineTrack;
        private Vector2 _touchV1;
        private Vector2 _touchV2;
        private int _mulTouchV1 = -10;
        private int _mulTouchV2 = -10;
        private VectorLine _touchRegmentLine;
        #endregion
        public override void Start()
        {
            base.Start();
            _lineParent = this.GetComponentByPath<Transform>("bg/line");
            _uavParent = this.GetComponentByPath<Transform>("bg/uav");
            _sqrMinPixelMove = _minPixelMove * _minPixelMove;
            _eventData = new PointerEventData(EventSystem.current); 
            _uiCamera = RootCanvas.Instance.UICamera;

       
        }
        public override void Update()
        {
            base.Update();
        }
        private VectorLine CreateLine(string colorHex = null)
        {                                                             
            TrackLineModel trackLineModel = _lineParent.GetComponentByPath<TrackLineModel>("line_" + colorHex);
            VectorLine vLine = trackLineModel?.Line;
            if (vLine == null)
            {
                vLine = new VectorLine("touch", new List<Vector2>(), _lineTex ?? null, _lineWidth, LineType.Continuous, Joins.Fill);
                vLine.rectTransform.SetParent(_lineParent);
                vLine.rectTransform.anchoredPosition3D = Vector3.zero;
                vLine.rectTransform.localScale = Vector3.one;
                //vLine.endPointsUpdate = 2;   //只更新最后几个点的优化，其余的不重新计算
            }
            return vLine;

        }
        public override void OnTouchBegan(Vector3 touchPos)
        {
            base.OnTouchBegan(touchPos);

            _touchTrackLineModel = null;//重置变量_touchTrackLineModel  否则影响后续判断
            for (int i = 0; i < TrackLineCtrl.Instance.TrackLineModels().Count; i++)
            {
                
                TrackLineModel trackLineModel = TrackLineCtrl.Instance.TrackLineModels()[i];
                int index;
                Vector2 lineTouchPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, RootCanvas.Instance.UICamera, out lineTouchPos);
                if (trackLineModel.Line.Selected(lineTouchPos, out index))
                {
                   
                    _touchTrackLineModel = trackLineModel;
                    if (trackLineModel.IsLineEnd(index))
                    {
                        RayLineTouchBegan(touchPos);
                    }
                    else
                    {
                        RegmentLineTouchBegan(touchPos,index);
                    }
                    break;
                }
            }     
            if(_touchTrackLineModel == null)
            {
                RayLineTouchBegan(touchPos);
            }
          
        }         
        #region 射线点击
        private void RayLineTouchBegan(Vector3 touchPos)
        {
            
            _touchState = TouchState.Ray;
            if (_touchTrackLineModel == null)
            {
                TrackModel track = PointerOverTrack(touchPos);

                if (track != null && track is TrackHouseModel)
                {
                    List<TrackLineModel> houseLines = track.TrackLineModels; 
                    if (houseLines == null || houseLines.Count <= 0)
                    {
                        TrackHouseModel houseModel = track as TrackHouseModel;
                        if (houseModel.TrackHouse.isLock) return;
                        VectorLine vLine = CreateLine(houseModel.TrackHouse.colorHex);
                        _touchTrackLineModel = vLine.rectTransform.GetOrAddComponent<TrackLineModel>();
                        _touchTrackLineModel.Line = vLine;
                        TrackLineCtrl.Instance.AddTrackLineModel(_touchTrackLineModel);
                        track.AddTrackLineModel(_touchTrackLineModel);
                        _touchTrackLineModel.TouchBeganDraw(track, houseModel.TrackHouse.colorHex);
                    }
                }
            }
            
            if (_touchTrackLineModel == null)
            {
                return;
            }
            _previousPosition = touchPos;
            TrackModel lastTrackModel = _touchTrackLineModel.GetLastTrackModel();
            ////------------------------------移除最后一个点占用的位置-----------------------------
            LineTrack lastLineTrack = _touchTrackLineModel.GetLastLineTrack();
            lastLineTrack.trackModel.OnLineExit(lastLineTrack.exitAxis, lastLineTrack.exitIndex);

            _touchRemoveTrackModel = lastTrackModel;
            Vector3 screenV1 = _uiCamera.WorldToScreenPoint(lastTrackModel.transform.position);
            LogUtility.LogInfo($"screenV1=={screenV1}");
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV1, RootCanvas.Instance.UICamera, out _v1);
        }
        #endregion
        #region 线段点击
        private void RegmentLineTouchBegan(Vector3 touchPos,int lineIndex)
        {
            
            if (_touchTrackLineModel != null)
            {
                _touchState = TouchState.Regment;
                _lineLastLineTrack = _touchTrackLineModel.RegmentLastLineTrack(lineIndex);
                _lineEndLineTrack = _touchTrackLineModel.RegmentEndLineTrack(lineIndex);
                //Vector2 screenV1 = _uiCamera.WorldToScreenPoint(_lineLastLineTrack.trackModel.transform.position);
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV1, _uiCamera, out _touchV1);
                //screenV1 = _uiCamera.WorldToScreenPoint(_lineEndLineTrack.trackModel.transform.position);
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV1, _uiCamera, out _touchV2);

                //------------------------------------------------------------------------------------------
                //_touchV1 = _touchTrackLineModel.Line.points2[_lineLastLineTrack.lineIndex];
                //_touchV2 = _touchTrackLineModel.Line.points2[_lineEndLineTrack.lineIndex];
                _touchV1 = _lineLastLineTrack.pos;
                _touchV2 = _lineEndLineTrack.pos;



                _touchRegmentLine = CreateLine();
                _touchRegmentLine.color = _touchTrackLineModel.Line.color; 
                _touchRegmentLine.Draw();
                Color color = _touchTrackLineModel.Line.color;
                color.a = 0.5f;
                _touchTrackLineModel.Line.SetColor(color, _lineLastLineTrack.lineIndex, _lineEndLineTrack.lineIndex-1);
                ////------------------------------移除最后一个点占用的位置-----------------------------
                LineTrack lastLineTrack = _touchTrackLineModel.GetLastLineTrack();
                lastLineTrack.trackModel.OnLineExit(lastLineTrack.exitAxis, lastLineTrack.exitIndex);
            }
        }
        #endregion
        public override void OnTouchMoved(Vector3 touchPos)
        {
            base.OnTouchMoved(touchPos);
            if (!_touchTrackLineModel) return;
            if ((touchPos - _previousPosition).sqrMagnitude < _sqrMinPixelMove) return;
            #region 射线点击Move
            if (_touchState == TouchState.Ray)
                RayLineTouchMoved(touchPos);
            else if(_touchState == TouchState.Regment)   
                RegmentLineTouchMoved(touchPos); 
            #endregion

        }
        #region 射线点击Move
        private void RayLineTouchMoved(Vector3 touchPos)
        {
            
            TrackModel endTrackModel = PointerOverTrack(touchPos);
            if (endTrackModel == null && _touchRemoveTrackModel != endTrackModel)
                _touchRemoveTrackModel = endTrackModel;
            
            if(endTrackModel != null)
            {         
                if ((endTrackModel is TrackHouseModel && _touchTrackLineModel.IsDrawInHouseModel(endTrackModel)) || (endTrackModel is TrackSiteModel && !endTrackModel.IsHaveLineModel(_touchTrackLineModel)))
                {
                    if (_touchRemoveTrackModel == null && _touchRemoveTrackModel != endTrackModel)
                    {
                        Vector3 screenV2 = _uiCamera.WorldToScreenPoint(endTrackModel.transform.position);
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV2, RootCanvas.Instance.UICamera, out _v2);
                        //------------------------------------------------------------------------------------------------------------------
                        //List<Vector2> points = SetupLine(_v1, _v2, ref _mulV1);
                        LineTrack lineTrack = new LineTrack();
                        lineTrack.trackModel = endTrackModel;
                        //List<Vector2> points = SetupLine(_touchTrackLineModel.GetLastLineTrack(), endTrackModel, lineTrack,ref _mulV1);
                        List<Vector2> points = SetupLine(_touchTrackLineModel.GetLastLineTrack(),lineTrack,ref _mulV1);
                        _touchTrackLineModel.TouchMoveDraw(points, lineTrack);
                        //_touchTrackLineModel.TouchMoveDraw(points, endTrackModel);
                        endTrackModel.AddTrackLineModel(_touchTrackLineModel);
                        if (_touchTrackLineModel.TrackUAVModel == null)
                            TrackUAVCtrl.Instance.CreateUAV(_touchTrackLineModel);
                        TrackModel lastTrackModel = _touchTrackLineModel.GetLastTrackModel();
                        Vector3 screenV1 = _uiCamera.WorldToScreenPoint(lastTrackModel.transform.position);
                        LogUtility.LogInfo($"screenV1=={screenV1}");
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV1, RootCanvas.Instance.UICamera, out _v1);
                        _touchRemoveTrackModel = lastTrackModel;
                        
                        return;
                    }
                    
                }
                if (_touchRemoveTrackModel == null && _touchTrackLineModel.GetLastTrackModel() == endTrackModel)
                {
                    //移除最后的站点
                    _touchTrackLineModel.RemoveLastTrackModel();
                    TrackModel lastTrackModel = _touchTrackLineModel.GetLastTrackModel();
                    if (lastTrackModel == null)
                    {
                        _touchTrackLineModel.TouchMoveDraw(new List<Vector2>());
                        _touchTrackLineModel = null;
                        _mulV1 = -10;
                    }
                    else
                    {
                        Vector3 screenV1 = _uiCamera.WorldToScreenPoint(lastTrackModel.transform.position);
                        LogUtility.LogInfo($"screenV1=={screenV1}");
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV1, RootCanvas.Instance.UICamera, out _v1);
                        _touchRemoveTrackModel = lastTrackModel;
                    }

                    return;
                }
            }
            if ((touchPos - _previousPosition).sqrMagnitude >= _sqrMinPixelMove)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, RootCanvas.Instance.UICamera, out _v2);
                //------------------------------------------------------------------------------------------------
                //List<Vector2> points = SetupLine(_v1, _v2, ref _mulV1);
                //每条线段进入站点的位置不一样,所以起始点要从开始点击时碰到的站点中获取
                List<Vector2> points = SetupLine(_touchTrackLineModel.GetLastLineTrack().pos, _v2, ref _mulV1);
                _touchTrackLineModel.TouchMoveDraw(points);
            }
        }
        #endregion
        #region 线段点击Move
        private void RegmentLineTouchMoved(Vector3 touchPos)
        {
            Vector2 touchLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, _uiCamera, out touchLocalPos);
            List<Vector2> points = new List<Vector2>();
            points.Clear();
            _touchRegmentLine.points2.Clear();
            List<Vector2> points1 = SetupLine(_touchV1, touchLocalPos, ref _mulTouchV1);
            List<Vector2> points2 = SetupLine(touchLocalPos, _touchV2, ref _mulTouchV2);
            points.Add(_touchV1);
     
            points=  points.Concat(points1).ToList<Vector2>();
            points = points.Concat(points2).ToList<Vector2>(); 
            _touchRegmentLine.points2 = points; 
            _touchRegmentLine.Draw();
            TrackModel endTrackModel = PointerOverTrack(touchPos);
            if (endTrackModel != null)
            {
                if ((endTrackModel is TrackHouseModel && _touchTrackLineModel.IsDrawInHouseModel(endTrackModel)) || (endTrackModel is TrackSiteModel && !endTrackModel.IsHaveLineModel(_touchTrackLineModel)))
                {     
                    Vector3 screenV2 = _uiCamera.WorldToScreenPoint(endTrackModel.transform.position);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV2, RootCanvas.Instance.UICamera, out touchLocalPos);
                    //---------------------------------------------------------------------------------------------------------
                    //points1 = SetupLine(_touchV1, touchLocalPos, ref _mulTouchV1);
                    //points2 = SetupLine(touchLocalPos, _touchV2, ref _mulTouchV2);
                    LineTrack lineTrack = new LineTrack();
                    lineTrack.trackModel = endTrackModel;
                    lineTrack.lineIndex = -1;
                    //points1 = SetupLine(_lineLastLineTrack, endTrackModel, lineTrack,ref _mulTouchV1);
                    //points2 = SetupLine(points1[points1.Count - 1], _lineEndLineTrack.pos, ref _mulTouchV2);
                    points1 = SetupLine(_lineLastLineTrack, lineTrack,ref _mulTouchV1);
                    points2 = SetupLine(lineTrack, _lineEndLineTrack, ref _mulTouchV2);
                    if(points1[points1.Count-1]==points2[0])
                    {
                        points2.RemoveAt(0);
                    }

                    points = _touchTrackLineModel.GetPoints(_lineLastLineTrack.lineIndex, _lineEndLineTrack.lineIndex);
                    //_touchRegmentLine.points2 = points;
                    //Color color = _touchRegmentLine.color;
                    //color.a = 0.5f;
                    //_touchRegmentLine.color = color;
                    //_touchRegmentLine.Draw();

                    //---------------------------------------------------------------------------------------------------
                    //_touchTrackLineModel.TouchRegmentMoveDraw(_lineLastLineTrack.lineIndex, points1, _lineEndLineTrack.lineIndex, points2, endTrackModel);
                    _touchTrackLineModel.TouchRegmentMoveDraw(_lineLastLineTrack.lineIndex, points1, _lineEndLineTrack.lineIndex, points2, lineTrack);


                    endTrackModel.AddTrackLineModel(_touchTrackLineModel);
                    RegmentLineTouchEnd();
                } 
            }
        }
        #endregion
        public override void OnTouchEnded(Vector3 touchPos)
        {
            base.OnTouchEnded(touchPos);
            if (_touchState == TouchState.Ray)
                RayLineTouchEnd();
            else if (_touchState == TouchState.Regment)
                RegmentLineTouchEnd();
           _touchState = TouchState.None;
            
        }
        #region 射线点击End
        private void RayLineTouchEnd()
        {
            if (_touchTrackLineModel)
            {
                if (_touchTrackLineModel.LineTracks.Count <= 1 && _touchTrackLineModel.GetLastTrackModel() is TrackHouseModel)
                    _touchTrackLineModel.RemoveLastTrackModel();
                _touchTrackLineModel.TouchEndDraw();
                _touchTrackLineModel = null;
                _mulV1 = -10;
            }
        }
        #endregion
        #region 线段点击End
        private void RegmentLineTouchEnd()
        {
            VectorLine.Destroy(ref _touchRegmentLine);
            Color color = _touchTrackLineModel.Line.color;
            color.a = 1f;
            _touchTrackLineModel.Line.color = color;
            _touchTrackLineModel.TouchEndDraw();
            _touchState = TouchState.None;
        }
        #endregion
        private TrackModel PointerOverTrack(Vector2 mousePosition)
        {
            _eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(_eventData, results);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject resultObj = results[i].gameObject;
                    if (resultObj.tag != Tags.Item) continue;
                    TrackModel resultModule = resultObj.GetOrAddComponent<TrackModel>();
                    if (resultModule != null )
                    {                                 
                        return resultModule;
                    } 
                }
            }
            return null;
        } 
        private List<Vector2> SetupLine(Vector2 vv1, Vector2 vv2, ref int mulvv1)
        {
            int m1 = Utility.AngleMul45(vv1, vv2, true);
            
            float angleV1 = Utility.Angle45(mulvv1);
            float angelAbs = Mathf.Abs(angleV1 - Utility.Angle_360(vv1, vv2));
            if (mulvv1 == 0 && (Utility.Angle_360(vv1, vv2) > 315 || Utility.Angle_360(vv1, vv2) < -45))
            {
                angelAbs = 360 - Utility.Angle_360(vv1, vv2);
            }
            else if (mulvv1 == 8 && (Utility.Angle_360(vv1, vv2) < 45 || Utility.Angle_360(vv1, vv2) > 315))
            {
                angelAbs = 45 - Utility.Angle_360(vv1, vv2);
            }
            if (angelAbs > 45)
            {
                mulvv1 = m1;
            }
            
            return MoveDrawLine(vv1, vv2, mulvv1);
        }
        private List<Vector2> MoveDrawLine(Vector2 vv1, Vector2 vv2, int mulvv1, bool isAddStartPos = false)
        {
            float angleV1 = Utility.Angle45(mulvv1);
            float angleV2 = 0;
            object data1 = Utility.LineAB(vv1, angleV1);
            Vector2 mid = Vector2.zero;
            if (data1 is Vector2)
            {
                Vector2 vec1 = (Vector2)data1;
                float yv2 = vec1.x * vv2.x + vec1.y;
                if (angleV1 >= 90 && angleV1 <= 270)
                {
                    if (vv2.y >= yv2)
                        angleV2 = angleV1 + 135;
                    else
                        angleV2 = angleV1 + 45;
                }
                else
                {
                    if (vv2.y >= yv2)
                        angleV2 = angleV1 + 45;
                    else
                        angleV2 = angleV1 + 135;
                }


            }
            else if (data1 is float)
            {
                if (vv2.y >= vv1.y)
                {
                    if (vv2.x >= vv1.x)
                        angleV2 = angleV1 + 135;
                    else
                        angleV2 = angleV1 + 45;
                }
                else
                {
                    if (vv2.x >= vv1.x)
                        angleV2 = angleV1 + 45;
                    else
                        angleV2 = angleV1 + 135;
                }
            }
            object data2 = Utility.LineAB(vv2, angleV2);
            if (data1 is Vector2 && data2 is Vector2)
            {
                Vector2 vec1 = (Vector2)data1;
                Vector2 vec2 = (Vector2)data2;
                float x = (vec2.y - vec1.y) / (vec1.x - vec2.x);
                float y = vec1.x * x + vec1.y;
                mid = new Vector2(x, y);
            }
            else if (data1 is float && data2 is Vector2)
            {
                Vector2 vec2 = (Vector2)data2;
                float x = vv1.x;
                float y = vec2.x * x + vec2.y;
                mid = new Vector2(x, y);
            }
            else if (data1 is Vector2 && data2 is float)
            {
                Vector2 vec1 = (Vector2)data1;
                float x = vv2.x;
                float y = vec1.x * x + vec1.y;
                mid = new Vector2(x, y);

            }
            List<Vector2> points = new List<Vector2>();

            //-----------------------------------------------------------------------------
            if (isAddStartPos)
            {
                points.Add(vv1);
            }

            Vector2 offset1 = mid - vv1;
            Vector2 offset2 = vv2 - mid;
            if (offset2.magnitude >= 2 && offset1.magnitude >= 2 && !points.Contains(mid) )
            {
                if(!points.Contains(mid))
                    points.Add(mid);
            }
            if (!points.Contains(vv2))
            {
                points.Add(vv2);
            }
            return points;  
        }

        //-------------------------------------设置路线（当能添加新站点时调用）--------------------------------------------------
        private List<Vector2> SetupLine(LineTrack lastLineTrack, LineTrack newLineTrack,ref int mulvv1)
        {
            Vector2 vv1;
            Vector3 vv1sScreenPos = RootCanvas.Instance.UICamera.WorldToScreenPoint(lastLineTrack.trackModel.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, vv1sScreenPos, RootCanvas.Instance.UICamera, out vv1);


            Vector2 vv2;
            Vector3 screenPos = RootCanvas.Instance.UICamera.WorldToScreenPoint(newLineTrack.trackModel.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenPos, RootCanvas.Instance.UICamera, out vv2);

            int m1 = Utility.AngleMul45(vv1, vv2, true);

            float angleV1 = Utility.Angle45(mulvv1);
            float angelAbs = Mathf.Abs(angleV1 - Utility.Angle_360(vv1, vv2));
            if (mulvv1 == 0 && (Utility.Angle_360(vv1, vv2) > 315 || Utility.Angle_360(vv1, vv2) < -45))
            {
                angelAbs = 360 - Utility.Angle_360(vv1, vv2);
            }
            else if (mulvv1 == 8 && (Utility.Angle_360(vv1, vv2) < 45 || Utility.Angle_360(vv1, vv2) > 315))
            {
                angelAbs = 45 - Utility.Angle_360(vv1, vv2);
            }
            if (angelAbs > 45)
            {
                mulvv1 = m1;
            }
            return MoveDrawLine(vv1, vv2, mulvv1, lastLineTrack, newLineTrack);
        }


        private List<Vector2> MoveDrawLine(Vector2 vv1, Vector2 vv2, int mulvv1, LineTrack lastLineTrack, LineTrack newLineTrack)
        {
            float angleV1 = Utility.Angle45(mulvv1);
            float angleV2 = 0;
            object data1 = Utility.LineAB(vv1, angleV1);
            Vector2 mid = Vector2.zero;
            if (data1 is Vector2)
            {
                Vector2 vec1 = (Vector2)data1;
                float yv2 = vec1.x * vv2.x + vec1.y;
                if (angleV1 >= 90 && angleV1 <= 270)
                {
                    if (vv2.y >= yv2)
                        angleV2 = angleV1 + 135;
                    else
                        angleV2 = angleV1 + 45;
                }
                else
                {
                    if (vv2.y >= yv2)
                        angleV2 = angleV1 + 45;
                    else
                        angleV2 = angleV1 + 135;
                }


            }
            else if (data1 is float)
            {
                if (vv2.y >= vv1.y)
                {
                    if (vv2.x >= vv1.x)
                        angleV2 = angleV1 + 135;
                    else
                        angleV2 = angleV1 + 45;
                }
                else
                {
                    if (vv2.x >= vv1.x)
                        angleV2 = angleV1 + 45;
                    else
                        angleV2 = angleV1 + 135;
                }
            }
            object data2 = Utility.LineAB(vv2, angleV2);
            if (data1 is Vector2 && data2 is Vector2)
            {
                Vector2 vec1 = (Vector2)data1;
                Vector2 vec2 = (Vector2)data2;
                float x = (vec2.y - vec1.y) / (vec1.x - vec2.x);
                float y = vec1.x * x + vec1.y;
                mid = new Vector2(x, y);
            }
            else if (data1 is float && data2 is Vector2)
            {
                Vector2 vec2 = (Vector2)data2;
                float x = vv1.x;
                float y = vec2.x * x + vec2.y;
                mid = new Vector2(x, y);
            }
            else if (data1 is Vector2 && data2 is float)
            {
                Vector2 vec1 = (Vector2)data1;
                float x = vv2.x;
                float y = vec1.x * x + vec1.y;
                mid = new Vector2(x, y);

            }
            List<Vector2> points = new List<Vector2>();

            
            LineDirection enterDir = GetTrackDirection(mid, vv2);
            if (mid == vv2)
            {
                enterDir = GetTrackDirection(vv1, vv2);
            }
            Vector2 enterPos = newLineTrack.trackModel.GetEnablePos(enterDir);
            //--------------------------------设置新站点位置属性--------------------------------
            newLineTrack.pos = enterPos;
            newLineTrack.trackModel.OnLineExit(newLineTrack.enterAxis, newLineTrack.enterIndex);
            newLineTrack.enterAxis = newLineTrack.trackModel.GetPosAxis(enterDir);
            newLineTrack.enterIndex = newLineTrack.trackModel.GetEnableIndex(newLineTrack.enterAxis);
            newLineTrack.trackModel.OnLineEnter(newLineTrack.enterAxis, newLineTrack.enterIndex);
            //设置上一个站点出站位置
            if (lastLineTrack != null && lastLineTrack.lineIndex != 0)
            {
                if((vv1-mid).magnitude>0.05f)
                {
                    vv1 = GetEnableExitPos(lastLineTrack, vv1, mid);
                }
                else
                {
                    vv1 = GetEnableExitPos(lastLineTrack, vv1, vv2);
                    //vv1 = GetEnableExitPos(lastLineTrack, vv1, enterPos);
                }
                return MoveDrawLine(vv1, enterPos, mulvv1, true);
                //if (vv1 != mid)
                //{

                //    vv1 = GetEnableExitPos(lastLineTrack, vv1, mid);
                //    return MoveDrawLine(vv1, enterPos, mulvv1, true);
                //}
            }
            //重新计算中点
            return MoveDrawLine(vv1, enterPos, mulvv1);



            //Vector2 offset1;
            //Vector2 offset2;

            //if (track != null)
            //{
            //    LineDirection enterDir = GetTrackDirection(mid, vv2);
            //    if(mid==vv2)
            //    {
            //        enterDir = GetTrackDirection(vv1, vv2);
            //    }
            //    Vector2 enterPos = track.GetEnablePos(enterDir);

            //    //--------------------------------设置新站点位置属性--------------------------------
            //    newLineTrack.trackModel = track;
            //    newLineTrack.enterAxis = track.GetPosAxis(enterDir);
            //    newLineTrack.enterIndex = track.GetEnableIndex(newLineTrack.enterAxis);
            //    track.OnLineEnter(newLineTrack.enterAxis, newLineTrack.enterIndex);
            //    newLineTrack.exitAxis = TrackAxis.None;
                
               
            //    //设置上一个站点出站位置
            //    if (lastLineTrack != null && lastLineTrack.lineIndex != 0)
            //    {
                
            //        if(vv1!=mid)
            //        {
            //            vv1 = GetEnableExitPos(lastLineTrack, vv1, mid);
            //            return MoveDrawLine(vv1, enterPos, mulvv1, true);
            //        }
                    
            //    }
            //    //重新计算中点
            //    return MoveDrawLine(vv1, enterPos, mulvv1);
            //}
            //offset1 = mid - vv1;
            //offset2 = vv2 - mid;

            //if (offset2.magnitude >= 2 && offset1.magnitude >= 2)
            //    points.Add(mid);

            //points.Add(vv2);
            //return points;
        }

        

        private Vector2 GetEnableExitPos(LineTrack lineTrack,Vector2 startPos,Vector2 midPos)
        {
            LineDirection dir = GetTrackDirection(midPos, startPos);
            TrackAxis axis = lineTrack.trackModel.GetPosAxis(dir);
            int index = lineTrack.enterIndex;
            int enableIndex= lineTrack.trackModel.GetEnableIndex(axis);
            if (index != 0 && enableIndex != 0)
            {
                if (index % 2 == 0)
                    index -= 1;
                else
                    index += 1;
            }
            else
                index = enableIndex;
            //if(!lineTrack.trackModel.IndexExit(axis,index))
            //{
            //    index = lineTrack.trackModel.GetEnableIndex(axis);
            //}
            
            lineTrack.trackModel.OnLineExit(lineTrack.exitAxis, lineTrack.exitIndex);
            lineTrack.exitIndex = index;
            lineTrack.exitAxis = axis;
            lineTrack.trackModel.OnLineEnter(axis, index);
            return lineTrack.trackModel.GetEnablePos(axis,index);
        }


        private LineDirection GetTrackDirection(Vector2 startPos, Vector2 endPos)
        {
            float angle = Utility.Angle_360(startPos, endPos);
            int times = Mathf.RoundToInt(angle) / 45;
            return (LineDirection)times;
        }

      
    }

    }

