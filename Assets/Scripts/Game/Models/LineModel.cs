using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

namespace Delivery
{
    public class LineModel : MonoBehaviour
    {
        public ColorType colorType = ColorType.Red;
        public StageLineType stageLineType = StageLineType.None;
        private List<ItemModel> itemModels;
        private List<UAVModel> uavModes = new List<UAVModel>(); //正在词条线上行驶的无人机
        private bool isNeedDestroy = false;//是否需要删除(线路改变)
        public VectorLine Line { get; set; }
        public void AddItemModel(ItemModel beganItem,ItemModel endItem)
        {
            if (itemModels == null)
                itemModels = new List<ItemModel>();
            itemModels.Clear();
            itemModels.Add(beganItem);
            itemModels.Add(endItem);
            if (stageLineType != StageLineType.Middle)
                stageLineType = StageLineType.Middle;
        }
        /// <summary>
        /// 针对线尾只添加一个item
        /// </summary>
        /// <param name="endItem"></param>
        public void AddItemModel(ItemModel endItem)
        {
            if (itemModels == null)
                itemModels = new List<ItemModel>();
            itemModels.Clear();        
            itemModels.Add(endItem);
            if (stageLineType != StageLineType.End)
                stageLineType = StageLineType.End;
        }
        public List<ItemModel> ItemModels { get { return itemModels; } }
        public void AddUAV(UAVModel uav)
        {
            if (!uavModes.Contains(uav))
                uavModes.Add(uav);
        }
        public void RemoveUAV(UAVModel uav)
        {
            if (uavModes.Contains(uav))
                uavModes.Remove(uav);
            if(isNeedDestroy && uavModes.Count <= 0)
            {
                VectorLine line = Line;
                VectorLine.Destroy(ref line);
                Destroy(gameObject);
            }
        }
        public void ClearItem()
        {
            if (itemModels == null)
                itemModels = new List<ItemModel>();
            itemModels.Clear();
        }
        public void Destroy()
        {
            isNeedDestroy = true;
            for(int i = 0; i < itemModels.Count; i++)
            {
                itemModels[i].RemoveLine(this);
            }
            if(uavModes.Count <= 0)
            {
                VectorLine line = Line;
                VectorLine.Destroy(ref line);
                Destroy(gameObject);
            }
        }
    }
}

