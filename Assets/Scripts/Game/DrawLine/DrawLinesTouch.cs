using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using UnityEngine.EventSystems;
using Foundation;
namespace Delivery
{
    public class DrawLinesTouch : BaseTouch
    {
        public Texture2D lineTex;
        public int maxPoints = 5000;
        public float lineWidth = 4.0f;
        public int minPixelMove = 5;
        private VectorLine line;
        private Vector3 previousPosition;
        private int sqrMinPixelMove;  
        PointerEventData eventData;  
        private LayerMask layerGround;
        private Camera uiCamera;
        private Vector2 v1;
        private Vector2 v2;
        int mulV1 = -10;
        ItemModel touchItemModel = null; //开始点击是触碰到的item 
        ItemModel touchRemoveItemModel = null;//删除线段时最后一个item
        private Transform lineParent;
        private Transform uavParent;
        private ColorType colorType;//当前选择的线的类型
        Dictionary<ColorType, List<LineModel>> lineDic = new Dictionary<ColorType, List<LineModel>>(); //存储线
        Dictionary<ColorType, LineModel> endLineDic = new Dictionary<ColorType, LineModel>();//每条线的最末尾线段
        Dictionary<ColorType, List<UAVModel>> uavDic = new Dictionary<ColorType, List<UAVModel>>();//存储无人机
        
        private bool isLastItem;//点击的item是否是这条线上最后的节点
        #region 线段点击
        private LineModel touchLineModel;//点击的line
        private VectorLine touchLine1;
        private VectorLine touchLine2;
        int mulTouchV1 = -10;
        int mulTouchV2 = -10;
        private Vector2 touchV1;
        private Vector2 touchV2;
        #endregion   
        public override void Start()
        {
            base.Start();
            lineParent = this.GetComponentByPath<Transform>("bg/line");
            uavParent = this.GetComponentByPath<Transform>("bg/uav");
            sqrMinPixelMove = minPixelMove * minPixelMove;     
            eventData = new PointerEventData(EventSystem.current);
            layerGround = 1 << LayerMask.NameToLayer("UI_Pass");
            uiCamera = RootCanvas.Instance.UICamera;
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();    
        }
        private VectorLine CreateLine()
        {
            VectorLine vLine = new VectorLine("touch", new List<Vector2>(), lineTex ?? null, lineWidth, LineType.Continuous, Joins.Fill);
            vLine.rectTransform.SetParent(lineParent);
            vLine.rectTransform.anchoredPosition3D = Vector3.zero;
            vLine.rectTransform.localScale = Vector3.one;
            vLine.endPointsUpdate = 2;   //只更新最后几个点的优化，其余的不重新计算
            return vLine;
            
        }
        public override void OnTouchBegan(Vector3 touchPos)
        {
            base.OnTouchBegan(touchPos);
            #region 线段点击
            RegmentLineTouchBegan(touchPos);  
            #endregion
            #region 射线点击
            RayLineTouchBegan(touchPos);
            #endregion

        }
        #region 线段点击    
        private void RegmentLineTouchBegan(Vector3 touchPos)
        {
            var enumerator = lineDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<LineModel> lines = enumerator.Current.Value;
                if (lines != null)
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        int index;
                        Vector2 lineTouchPos;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, RootCanvas.Instance.UICamera, out lineTouchPos);
                        if (lines[i].Line.Selected(lineTouchPos, 2, out index))
                        {
                            if (IsPointerOverPass(touchPos) != null) continue;
                            touchLineModel = lines[i];
                            LogUtility.LogInfo($"index===={index}");
                            Color color = touchLineModel.Line.color;
                            LogUtility.LogInfo($"color==={color}");
                            color.a = 0.5f;
                            touchLineModel.Line.color = color;
                            colorType = touchLineModel.colorType;
                            Vector3 screentouchV1 = uiCamera.WorldToScreenPoint(touchLineModel.ItemModels[0].transform.position);
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screentouchV1, RootCanvas.Instance.UICamera, out touchV1);
                            Vector3 screentouchV2 = uiCamera.WorldToScreenPoint(touchLineModel.ItemModels[1].transform.position);
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screentouchV2, RootCanvas.Instance.UICamera, out touchV2);
                            if (touchLine1 == null)
                                touchLine1 = CreateLine();
                            return;
                        }
                    }
                }
            }
        }
        #endregion
        #region 射线点击
        private void RayLineTouchBegan(Vector3 touchPos)
        {
            LineModel endLineModel = null;
            var endEnumerator = endLineDic.GetEnumerator();
            while (endEnumerator.MoveNext())
            {
                LineModel endLine = endEnumerator.Current.Value;
                if (endLine == null) continue;
                int index;
                Vector2 lineTouchPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, RootCanvas.Instance.UICamera, out lineTouchPos);
                if (endLine.Line.Selected(lineTouchPos, 2, out index))
                {
                    if (IsPointerOverPass(touchPos) != null) continue;
                    if (endLine.stageLineType != StageLineType.End) continue;
                    touchItemModel = endLine.ItemModels[0];
                    colorType = endLine.colorType;
                    endLineModel = endLine;
                    break;
                }
            }
            if (touchItemModel == null)
            {
                ItemModel item = IsPointerOverPass(touchPos);
                if (item != null && item.itemType == ItemType.WareHouse)
                    touchItemModel = item;
            }
            if (endLineModel != null)
            {
                endLineModel.Line.points2.Clear();
                endLineModel.Line.Draw();
            }
            if (endLineModel == null && touchItemModel != null && touchItemModel.itemType != ItemType.WareHouse)
            {
                touchItemModel = null;
                return;
            }
            if (touchItemModel != null && touchItemModel.itemType == ItemType.WareHouse)
            {
                if (endLineModel != null)
                {
                    touchItemModel = RemoveEndLine(touchItemModel, colorType);
                }
                else
                {
                    if (!touchItemModel.IsHouseDrawLine())
                    {
                        touchItemModel = null;
                        return;
                    }
                }
            }
            if (!touchItemModel) return;
            if (touchItemModel.itemType == ItemType.WareHouse)
                colorType = touchItemModel.colorType;
            if (touchItemModel.Lines(colorType).Count >= 2)
            {
                touchItemModel = null;
                return;
            }
            IsLastItem(touchItemModel, colorType);
            if (line == null)
                line = CreateLine();

            previousPosition = touchPos;
            Vector3 screenV1 = uiCamera.WorldToScreenPoint(touchItemModel.transform.position);
            LogUtility.LogInfo($"screenV1=={screenV1}");
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV1, RootCanvas.Instance.UICamera, out v1);
        }
        #endregion
        public override void OnTouchMoved(Vector3 touchPos)
        {
            base.OnTouchMoved(touchPos);
            #region 线段点击
            RegmentLineTouchMoved(touchPos);
            #endregion
            #region 射线点击Move
            RayLineTouchMoved(touchPos);
            #endregion

        }
        #region 线段点击Move
        private void RegmentLineTouchMoved(Vector3 touchPos)
        {
            if (touchLineModel != null)
            {
                Vector2 touchLocalPos;
                ItemModel touchEndItem = IsPointerMoveOverPass(touchPos, touchLineModel, colorType);
                if (touchEndItem)
                {
                    Vector3 screenTouchV2 = uiCamera.WorldToScreenPoint(touchEndItem.transform.position);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenTouchV2, RootCanvas.Instance.UICamera, out touchLocalPos);
                    SetupLine(touchV1, touchLocalPos, ref mulTouchV1, ref touchLine1);
                    SetupLine(touchLocalPos, touchV2, ref mulTouchV2, ref touchLine2);
                    AddTouchLine(touchLine1, touchLine2, touchEndItem, colorType);
                    touchLine1 = null;
                    touchLine2 = null;
                    touchLineModel = null;
                    return;
                }
                RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, RootCanvas.Instance.UICamera, out touchLocalPos);
                SetupLine(touchV1, touchLocalPos, ref mulTouchV1, ref touchLine1);
                SetupLine(touchLocalPos, touchV2, ref mulTouchV2, ref touchLine2);  
            }
        }
        #endregion
        #region 射线点击Move
        private void RayLineTouchMoved(Vector3 touchPos)
        {
            if (!touchItemModel) return;
            LogUtility.LogInfo($"touchItemModel==={touchItemModel}");
            if ((touchPos - previousPosition).sqrMagnitude >= sqrMinPixelMove)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, RootCanvas.Instance.UICamera, out v2);
                SetupLine(v1, v2, ref mulV1, ref line);
            }
            ItemModel touchEndItemModel = IsPointerOverPass(touchPos);
            if (touchRemoveItemModel != touchEndItemModel)
                touchRemoveItemModel = null;
            LogUtility.LogInfo($"touchEndItemModel==={touchEndItemModel}");
            if (touchRemoveItemModel == null && touchEndItemModel && touchItemModel != touchEndItemModel && touchEndItemModel.Lines(colorType).Count < 2)
            {
                if (touchEndItemModel.itemType == ItemType.WareHouse && touchEndItemModel.colorType != colorType) return;
                Vector3 screenV2 = uiCamera.WorldToScreenPoint(touchEndItemModel.transform.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV2, RootCanvas.Instance.UICamera, out v2);
                SetupLine(v1, v2, ref mulV1, ref line);
                LineModel lineModel = AddLine(line, touchEndItemModel, colorType);
                AddEndLine(lineModel, colorType);
                touchItemModel = null;
                line = null;
                //创建无人机
                CreateUAV();
                return;
            }
            if (touchEndItemModel == null && IsPointerSameItem(touchPos, touchItemModel))
            {
                if (touchRemoveItemModel == touchItemModel) return;
                touchRemoveItemModel = touchItemModel;
                if (touchItemModel.itemType != ItemType.WareHouse)
                    touchItemModel = RemoveEndLine(touchItemModel, colorType);
                Vector3 screenV1 = uiCamera.WorldToScreenPoint(touchItemModel.transform.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, screenV1, RootCanvas.Instance.UICamera, out v1);
            }
        }
        #endregion
        public override void OnTouchEnded(Vector3 touchPos)
        {
            base.OnTouchEnded(touchPos);
            if(touchItemModel)
            {
                if (line != null)
                {
                    line.points2.Clear();
                    line.Draw();
                }
                //LineModel lineModel = GetLastLine(colorType);
                //AddEndLine(lineModel, colorType);
                touchItemModel = null;
                touchRemoveItemModel = null;
                mulV1 = -10;
            }
            LineModel lineModel = GetLastLine(colorType);
            if(lineModel != null)
                AddEndLine(lineModel, colorType);
            if (touchLineModel != null)
            {
                mulTouchV1 = -10;
                mulTouchV2 = -10;
                Color color = touchLineModel.Line.color; 
                if(color.a != 1f)
                {
                    color.a = 1f;
                    touchLineModel.Line.color = color;
                } 
                touchLine1.points2.Clear();
                touchLine1.Draw();   
                touchLine2.points2.Clear();
                touchLine2.Draw();   
                touchLineModel = null;
            }
             
        }
        private LineModel AddLine(VectorLine vLine,ItemModel endItem, ColorType type)
        {
            LineModel lineModel = vLine.rectTransform.GetOrAddComponent<LineModel>();
            List<LineModel> lines = null;
            if (!lineDic.TryGetValue(type, out lines))
            {
                lines = new List<LineModel>();
                lineDic.Add(type, lines);
            }
            vLine.name = type.ToString() + lines.Count;
            LogUtility.LogInfo($"isLastItem==={isLastItem}");
            if(isLastItem)
            {
                lineModel.AddItemModel(touchItemModel, endItem);
                lines.Add(lineModel);
            }
                
            else
            {
                lineModel.AddItemModel(endItem, touchItemModel);
                List<Vector2> points = new List<Vector2>(vLine.points2);
                vLine.points2.Clear();
                for (int i =points.Count - 1; i >= 0; i--)
                {
                    vLine.points2.Add(points[i]);
                }
                vLine.Draw();
                lines.Insert(0, lineModel);
            }
            
            lineModel.Line = vLine;
            lineModel.colorType = colorType;
            touchItemModel.AddLine(lineModel);
            endItem.AddLine(lineModel);
            ItemCtrl.Instance.AddLineItem(touchItemModel, colorType);
            ItemCtrl.Instance.AddLineItem(endItem, colorType);
            return lineModel;
        }
        private void AddEndLine(LineModel lastLineModel, ColorType type)
        {
            List<LineModel> lines = null;
            if(!lineDic.TryGetValue(type,out lines))
            {
                LogUtility.LogError($"没有{type.ToString()}类型的线");
                return;
            }
            VectorLine vLine = null;
            LineModel lineModel = null;
            if (!endLineDic.ContainsKey(type))
            {
                vLine =  CreateLine();
                lineModel = vLine.rectTransform.GetOrAddComponent<LineModel>();
                lineModel.Line = vLine;
                endLineDic.Add(type, lineModel);
                vLine.name = type.ToString() + "end";
            }
            else
            {
                lineModel = endLineDic[type];
                vLine = lineModel.Line;
            }                      
            lineModel.AddItemModel(lastLineModel.ItemModels[lastLineModel.ItemModels.Count-1]);
            lineModel.colorType = type;
            lineModel.stageLineType = StageLineType.End;
            VectorLine lastLine = lastLineModel.Line;
            Vector2 lastEndPos = lastLine.points2[lastLine.points2.Count - 1];
            Vector2 offset = lastEndPos - lastLine.points2[lastLine.points2.Count - 2];
            vLine.points2.Clear();
            vLine.points2.Add(lastLine.points2[lastLine.points2.Count - 1]);
            vLine.points2.Add(lastEndPos + offset.normalized * 110);
            vLine.color = Utility.ConvertColor(colorType);
            vLine.Draw();  
        } 
        /// <summary>
        /// 移除最后一条线
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="type"></param>
        private ItemModel RemoveEndLine(ItemModel itemModel, ColorType type)
        {
            List<LineModel> lines = null;
            if (!lineDic.TryGetValue(type, out lines))
            {
                LogUtility.LogError($"没有{type.ToString()}类型的线");
                return null;
            }
            ItemModel newItemModel = null;
            LineModel endLineModel = lines[lines.Count - 1];
            if(endLineModel.ItemModels.Count >=2 && endLineModel.ItemModels[1] == itemModel)
            {
                Color color = endLineModel.Line.color;  
                color.a = 0.5f;
                endLineModel.Line.color = color;
                newItemModel = endLineModel.ItemModels[0];
                newItemModel.RemoveLine(endLineModel);
                itemModel.RemoveLine(endLineModel);
                ItemCtrl.Instance.RemoveLineItem(itemModel, colorType);
                lines.Remove(endLineModel);
                endLineModel.Destroy();
            }
            return newItemModel;
        }
        /// <summary>
        /// 获取最后一条线段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private LineModel GetLastLine( ColorType type)
        {
            List<LineModel> lines = null;
            if (!lineDic.TryGetValue(type, out lines))
            {
                LogUtility.LogError($"没有{type.ToString()}类型的线");
                return null;
            }
            if (lines == null || lines.Count <= 0)
                return null;
            return lines[lines.Count - 1];  
        }
        private void AddTouchLine(VectorLine vLine1,VectorLine vLine2,ItemModel endItem, ColorType type)
        {
            if (touchLineModel != null)
            {
                List<LineModel> lines = null;
                if (!lineDic.TryGetValue(type, out lines))
                {
                    lines = new List<LineModel>();
                    lineDic.Add(type, lines);
                }
                int index = lines.IndexOf(touchLineModel);
                lines.Remove(touchLineModel);
                LineModel lineModel1 = vLine1.rectTransform.GetOrAddComponent<LineModel>();
                lineModel1.name = type.ToString() + index + 1;
                lineModel1.AddItemModel(touchLineModel.ItemModels[0], endItem);
                lineModel1.colorType = colorType; 
                lines.Insert(index, lineModel1);
                lineModel1.Line = vLine1;
                touchLineModel.ItemModels[0].AddLine(lineModel1);
                endItem.AddLine(lineModel1);
                ItemCtrl.Instance.InsertLineItem(touchLineModel.ItemModels[0],endItem,colorType);

                LineModel lineModel2 = vLine2.rectTransform.GetOrAddComponent<LineModel>();
                lineModel2.name = type.ToString() + index + 2;
                lineModel2.AddItemModel(endItem, touchLineModel.ItemModels[1]);
                lineModel2.colorType = colorType;
                lines.Insert(index + 1, lineModel2);
                lineModel2.Line = vLine2;
                endItem.AddLine(lineModel2);
                touchLineModel.ItemModels[1].AddLine(lineModel2);


                touchLineModel.Destroy();
                //VectorLine touchModelLine = touchLineModel.Line;
                //VectorLine.Destroy(ref touchModelLine);
            }
        }
        private void SetupLine(Vector2 vv1,Vector2 vv2,ref int mulvv1,ref VectorLine vLine)
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
            MoveDrawLine(vv1, vv2, mulvv1, ref vLine);
        }
        private void MoveDrawLine(Vector2 vv1, Vector2 vv2, int mulvv1, ref VectorLine vLine)
        {
            if (vLine == null)
                vLine = CreateLine();
            vLine.points2.Clear();
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
            if (!vLine.points2.Contains(vv1))
                vLine.points2.Add(vv1);
            if (!vLine.points2.Contains(mid))
            {
                Vector2 offset1 = mid - vv1;
                Vector2 offset2 = vv2 - mid;
                if(offset2.magnitude >=2 && offset1.magnitude >=2)
                    vLine.points2.Add(mid);
            }   
            if (!vLine.points2.Contains(vv2))
                vLine.points2.Add(vv2);
            vLine.color = Utility.ConvertColor(colorType);
            vLine.Draw();

        }
        #region 画线
        private void MoveDrawLine()
        {
            line.points2.Clear();
            float angleV1 = Utility.Angle45(mulV1);          
            float angleV2 = 0;              
            object data1 = Utility.LineAB(v1, angleV1);   
            Vector2 mid = Vector2.zero;
            if(data1 is Vector2)
            {
                Vector2 vec1 = (Vector2)data1;                
                float yv2 = vec1.x * v2.x + vec1.y;         
                if(angleV1>=90 && angleV1 <= 270)
                {
                    if (v2.y >= yv2) 
                        angleV2 = angleV1 + 135; 
                    else
                        angleV2 = angleV1 + 45;
                }
                else
                {
                    if (v2.y >= yv2) 
                        angleV2 = angleV1 + 45;  
                    else
                        angleV2 = angleV1 + 135;
                }    
                
                
            }
            else if(data1 is float)
            {
                if(v2.y >= v1.y)
                {
                    if (v2.x >= v1.x)
                        angleV2 = angleV1 + 135;
                    else
                        angleV2 = angleV1 + 45;
                }
                else
                {
                    if (v2.x >= v1.x)
                        angleV2 = angleV1 + 45;
                    else
                        angleV2 = angleV1 + 135;
                }
            }                                                  
            object data2 = Utility.LineAB(v2, angleV2);
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
                float x = v1.x;
                float y = vec2.x * x + vec2.y;
                mid = new Vector2(x, y);                  
            }
            else if (data1 is Vector2 && data2 is float)
            {
                Vector2 vec1 = (Vector2)data1;
                float x = v2.x;
                float y = vec1.x * x + vec1.y;
                mid = new Vector2(x, y);                  

            }
            if(!line.points2.Contains(v1))
                line.points2.Add(v1);
            if (!line.points2.Contains(mid))
                line.points2.Add(mid);
            if (!line.points2.Contains(v2))
                line.points2.Add(v2);   
            line.color = Color.red;
            line.Draw();

        }
#endregion
        /// <summary>
        /// 检测是否点击UI
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private ItemModel IsPointerOverPass(Vector2 mousePosition)
        {                      
            eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count > 0 )
            {
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject resultObj = results[i].gameObject;
                    if (resultObj.tag != Tags.Item) continue;
                    ItemModel resultModule = resultObj.GetOrAddComponent<ItemModel>();
                    if(touchItemModel == null)
                    {
                        touchItemModel = resultModule;
                        return resultModule;
                    }
                    if (touchItemModel != null /*&& touchItemModel != resultModule*/)
                    {  
                        if (HavaSameLine(touchItemModel, resultModule,colorType)) break; 
                        return resultModule;
                    }
                }           
            }
            return null;
        }
        private bool IsPointerSameItem(Vector2 mousePosition,ItemModel itemModel)
        {
            eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count > 0)
            {                                             
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject resultObj = results[i].gameObject;
                    if (resultObj == itemModel.gameObject)
                        return true; 
                }
            }
            return false;
        }
        private ItemModel IsPointerMoveOverPass(Vector2 mousePosition,LineModel lineModel,ColorType colorType)
        {
            eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject resultObj = results[i].gameObject;
                    if (resultObj.tag != Tags.Item) continue;
                    ItemModel resultModule = resultObj.GetOrAddComponent<ItemModel>();
                    if(!lineModel.ItemModels.Contains(resultModule) && resultModule.Lines(colorType).Count==0)
                        return resultModule; 
                }
            }
            return null;
        }
        /// <summary>
        /// 判断两个站点是否有条共同的线
        /// </summary>
        private bool HavaSameLine(ItemModel item1,ItemModel item2,ColorType colorType)
        {
            List<LineModel> lines1 = item1.Lines(colorType);
            List<LineModel> lines2 = item2.Lines(colorType);
            for(int i = 0; i < lines1.Count; i++)
            {
                for(int j = 0; j < lines2.Count; j++)
                {
                    if (lines1[i] == lines2[j])
                        return true;
                }
            }
            return false;
        }
        private void IsLastItem(ItemModel itemModel, ColorType colorType)
        {
            List<LineModel> lines;
            if(!lineDic.TryGetValue(colorType,out lines))
            {
                isLastItem = true;
            }
            else
            {
                if (lines.Count <= 0)
                {
                    isLastItem = true;
                    return;
                }    
                LineModel lineModel = lines[lines.Count - 1];
                if (lineModel.ItemModels.Count >= 2 && lineModel.ItemModels[1] == itemModel)
                    isLastItem = true;
                else
                    isLastItem = false;
            }
        }
        
        //创建无人机
        private void CreateUAV()
        {
            List<UAVModel> uavs;
            UAVModel uavModel = null;
            if (!uavDic.TryGetValue(colorType, out uavs))
            {
                uavs = new List<UAVModel>();
                uavDic.Add(colorType, uavs);
            }
            else
            {
                uavModel = uavs[0];
                //uavModel.Init(lineDic[colorType]);
                return;
            } 
            string prefabPath = PrefabPath.uavPath + "uav";                     
            GameObject uavObj = SG.ResourceManager.Instance.GetObjectFromPool(prefabPath, true, 1);
            RectTransform uavTrans = uavObj.GetComponent<RectTransform>();
            uavTrans.SetParent(uavParent);
            uavTrans.localScale = Vector3.one;
            uavModel = uavTrans.GetOrAddComponent<UAVModel>();
            uavModel.Init(colorType, lineDic[colorType]);
            
            uavs.Add(uavModel);
        }
    }
}

