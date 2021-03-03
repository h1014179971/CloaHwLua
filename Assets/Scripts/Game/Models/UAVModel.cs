using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Foundation;
using Vectrosity;

namespace Delivery
{
    public class UAVModel : MonoBehaviour
    {
        private Camera uiCamera;
        private ColorType colorType;
        private RectTransform rectTrans; 
        private List<LineModel> followLines; //线路    
        private LineModel followLine;//当前跟随的线    
        private float speed = 200f;
        private bool go = true;//顺时针移动    
        private int linePointIndex = 0;
        private Vector2 offset;//起始点与下一个点的向量
        private float rotateAngle;//需要旋转的角度
        private float rotateAngleSpeed = 10;//旋转速度
        private bool isMove = true;
        private float loadTime = 0.5f;//装卸每个货物的时间
        //private float maxtime = 1f;//每个站点停留时间
        //private float time = 1f;

        private List<Vector2> littlePos = new List<Vector2>();
        private List<ItemLittleModel> itemLittles = new List<ItemLittleModel>();
        private int row = 2;//行数
        private int col = 3;//列数  
        private float width = 20;
        private float height = 30;

        public RectTransform RectTrans
        {
            get
            {
                if (rectTrans == null)
                {
                    rectTrans = GetComponent<RectTransform>();
                }
                return rectTrans;
            }
        }
        private void CreatePos()
        {
            littlePos.Clear();
            Vector2 pos = new Vector2((col - 1) * 0.5f * width, (row - 1) * 0.5f * height);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    littlePos.Add(pos - new Vector2(j * width, i * height));
                }
            }
        }
        public void Init(ColorType type, List<LineModel> lines)
        {
            uiCamera = RootCanvas.Instance.UICamera;
            colorType = type;
            Image image = this.GetComponent<Image>();
            image.color = Utility.ConvertColor(colorType);
            followLines = lines;
            followLine = followLines[0];
            followLine.AddUAV(this);
            rectTrans = this.GetComponent<RectTransform>();
            rectTrans.anchoredPosition3D = followLine.Line.points2[0];
            linePointIndex = 1;
            offset = followLine.Line.points2[linePointIndex] - rectTrans.anchoredPosition;
            rotateAngle = Utility.Angle_360(rectTrans.anchoredPosition, followLine.Line.points2[linePointIndex]);
            rectTrans.localEulerAngles = new Vector3(0, 0, rotateAngle);     
            CreatePos();
            PortItemLittle(followLine.ItemModels[0]);
        }
        void Update()
        {
            //if (followLines == null || followLines.Count <= 0) return;
            if (!isMove) return;
            if (followLine == null) return;
            if (go)
            {   
                if(followLine == null && followLines.Count >0)
                {
                    followLine = followLines[0];
                    followLine.AddUAV(this);
                }   
                Vector2 v2 = Vector2.Lerp(Vector2.zero, offset.normalized*speed, Time.deltaTime);
                rectTrans.anchoredPosition += v2; 
                rectTrans.rotation = Quaternion.Lerp(rectTrans.rotation,Quaternion.Euler(new Vector3(0, 0, rotateAngle)),Time.deltaTime * rotateAngleSpeed);
                //当初始值跟目标角度小于2，就将目标角度赋值给初始角度，让旋转角度是我们需要的角度
                if (Quaternion.Angle(rectTrans.rotation, Quaternion.Euler(new Vector3(0, 0, rotateAngle))) < 2)
                {
                    rectTrans.rotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
                }
                if ((followLine.Line.points2[linePointIndex] - rectTrans.anchoredPosition).magnitude < speed * Time.deltaTime *2)
                {
                    linePointIndex++;
                    if(linePointIndex >= followLine.Line.points2.Count)
                    {
                        ItemModel item = followLine.ItemModels[followLine.ItemModels.Count - 1];
                        if (item == null)
                            LogUtility.LogError("此线段没有终点站点");
                        LineModel nextFollowLine = NextFollowLine(item,true);
                        if(nextFollowLine == null)
                        {
                            if (IsRemoveLine(followLine, go))
                            {
                                followLine.RemoveUAV(this);
                                if (followLines.Count > 0)
                                {
                                    followLine = followLines[followLines.Count - 1]; 
                                    followLine.AddUAV(this);
                                    PortItemLittle(followLine.ItemModels[1]);
                                }
                                else
                                    followLine = null;
                            }
                            go = false;    
                            linePointIndex = followLine.Line.points2.Count - 1;   
                        } 
                        else
                        {
                            followLine.RemoveUAV(this);
                            followLine = nextFollowLine;  
                            followLine.AddUAV(this);
                            linePointIndex = 1;
                            PortItemLittle(followLine.ItemModels[0]);
                        }
                        
                    } 
                    if(followLine != null)
                    {
                        offset = followLine.Line.points2[linePointIndex] - rectTrans.anchoredPosition;
                        rotateAngle = Utility.Angle_360(Vector2.zero, offset);
                    }
                    
                }

            }
            else
            {
                Vector2 v2 = Vector2.Lerp(Vector2.zero, offset.normalized * speed, Time.deltaTime);
                rectTrans.anchoredPosition += v2;
                rectTrans.rotation = Quaternion.Lerp(rectTrans.rotation, Quaternion.Euler(new Vector3(0, 0, rotateAngle)), Time.deltaTime * rotateAngleSpeed);
                //当初始值跟目标角度小于2，就将目标角度赋值给初始角度，让旋转角度是我们需要的角度
                if (Quaternion.Angle(rectTrans.rotation, Quaternion.Euler(new Vector3(0, 0, rotateAngle))) < 2)
                {
                    rectTrans.rotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
                }
                if ((followLine.Line.points2[linePointIndex] - rectTrans.anchoredPosition).magnitude < speed * Time.deltaTime *2)
                {
                    linePointIndex--;
                    
                    if (linePointIndex < 0)
                    {
                        ItemModel item = followLine.ItemModels[0];
                        if (item == null)
                            LogUtility.LogError("此线段没有起始站点");
                        LineModel nextFollowLine = NextFollowLine(item,false);
                        if (nextFollowLine == null)
                        {
                            go = true;
                            followLine.RemoveUAV(this);
                            if(followLines.Count > 0)
                            {
                                followLine = followLines[0];  
                                followLine.AddUAV(this);
                                PortItemLittle(followLine.ItemModels[0]);
                            }
                            else
                                followLine = null;
                            linePointIndex = 1; 

                        }
                        else
                        {
                            followLine.RemoveUAV(this);
                            followLine = nextFollowLine;  
                            followLine.AddUAV(this);
                            linePointIndex = followLine.Line.points2.Count - 1;
                            PortItemLittle(followLine.ItemModels[1]);
                        }
                        
                    } 
                    if(followLine != null)
                    {
                        offset = followLine.Line.points2[linePointIndex] - rectTrans.anchoredPosition;
                        rotateAngle = Utility.Angle_360(Vector2.zero, offset);
                    }
                    
                }
            }

        }
        /// <summary>
        /// 下一段沿线移动的线段
        /// </summary>
        /// <param name="itemModel"></param>
        LineModel NextFollowLine(ItemModel itemModel,bool isNext)
        {
            for(int i =0;i< followLines.Count; i++)
            {
                LineModel lineModel = followLines[i];
                if (isNext && lineModel.ItemModels[0] == itemModel)
                    return lineModel;
                if (!isNext && lineModel.ItemModels[1] == itemModel)
                    return lineModel;
            }
            return null;
        }
        private bool IsRemoveLine(LineModel lineModel,bool isNext/*是否是顺着走*/)
        {
            int index = 1;
            if (!isNext)
                index = 0;
            if(lineModel.ItemModels.Count < index + 1)
            {
                LogUtility.LogInfo($"item 越界");
                return true;
            }
            ItemModel itemModel = lineModel.ItemModels[index];
            for(int i =0;i< followLines.Count; i++)
            {
                if (followLines[i].ItemModels.Contains(itemModel))
                    return true;
            }
            return false;
        }
        //装货/卸货
        private void PortItemLittle(ItemModel itemModel)
        {
            if (itemModel.itemType == ItemType.WareHouse)
            {
                StartCoroutine(LoadItemLittle(itemModel));
            }
            else
                StartCoroutine(UnLoadItemLittle(itemModel));
        }
        //装货
        private IEnumerator LoadItemLittle(ItemModel itemModel)
        {
            isMove = false;
            List<ItemLittleModel> houseItems = itemModel.GetHouseItemLittle();
            while(itemLittles.Count <row * col && houseItems.Count > 0)
            {
                yield return new WaitForSeconds(loadTime);
                ItemLittleModel itemLittleModel = houseItems[0];
                itemLittleModel.RectTrans.SetParent(RectTrans);
                itemLittleModel.RectTrans.anchoredPosition = littlePos[itemLittles.Count];
                itemLittles.Add(itemLittleModel);
                itemModel.UnLoadItemLittle(itemLittleModel);
            }
            isMove = true;
        }
        //卸货
        private IEnumerator UnLoadItemLittle(ItemModel itemModel)
        {
            isMove = false;
            while (itemLittles.Count >0 && IsMateItem(itemModel))
            {
                yield return new WaitForSeconds(loadTime);
                ItemLittleModel itemLittleModel = null;
                for (int i = itemLittles.Count - 1; i >= 0; i--)
                {
                    if (itemLittles[i].Id == itemModel.id)
                    {
                        itemLittleModel = itemLittles[i];
                        break;
                    }    
                }
                int index = itemLittles.IndexOf(itemLittleModel);
                itemLittles.Remove(itemLittleModel);
                ReLoadItemLittle(index);
                SG.ResourceManager.Instance.ReturnObjectToPool(itemLittleModel.gameObject);   
            }
            isMove = true;
        }
        //是否有匹配的item
        private bool IsMateItem(ItemModel itemModel)
        {
            for(int i = 0; i < itemLittles.Count; i++)
            {
                if (itemLittles[i].Id == itemModel.id)
                    return true;
            }
            return false;
        }
        //重新排列
        private void ReLoadItemLittle(int index = 0)
        { 
            for (int i = index; i < itemLittles.Count; i++)
            {
                itemLittles[i].RectTrans.anchoredPosition = littlePos[i];
            }
        }
    }
}

