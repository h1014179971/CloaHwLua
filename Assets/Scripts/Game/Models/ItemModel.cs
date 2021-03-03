using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery
{
    public partial class ItemModel : MonoBehaviour
    {
        public int id;
        public ItemType itemType;
        public ColorType colorType; 
        private RectTransform rectTrans;
        private Dictionary<ColorType, List<LineModel>> lineDic = new Dictionary<ColorType, List<LineModel>>();
        

        #region 存放货物的变量
        private List<Vector2> littlePos = new List<Vector2>();
        private List<ItemLittleModel> itemLittles = new List<ItemLittleModel>();
        private int row = 2;//行数
        private int col = 3;//列数  
        private float width = 20;
        private float height = 30;
        #endregion
        public RectTransform RectTrans {
            get {
                if (rectTrans == null)
                {
                    rectTrans = GetComponent<RectTransform>();
                }
                return rectTrans;
                }
        }
        private void Start()
        {
            if(itemType == ItemType.WareHouse)
                InitHouseItem();
        }
        public List<LineModel> Lines(ColorType colorType)
        {
            List<LineModel> lines;
            if (!lineDic.TryGetValue(colorType, out lines))
            {
                if (lines == null)
                    lines = new List<LineModel>();       
            }
            return lines;
        }
        public void AddLine(LineModel lineModel)
        {
            List<LineModel> lines;  
            if(!lineDic.TryGetValue(lineModel.colorType,out lines))
            {
                if (lines == null)
                    lines = new List<LineModel>();
                lineDic.Add(lineModel.colorType, lines);
            }                        
            lines.Add(lineModel);
        } 

        public bool IsHouseDrawLine()
        {
            if (itemType == ItemType.WareHouse && lineDic.Count <= 0)
                return true;
            return false;
        }

        public void RemoveLine(LineModel lineModel)
        {
            List<LineModel> lines;
            if (!lineDic.TryGetValue(lineModel.colorType, out lines))
            {
                //if (lines == null)
                //    lines = new List<LineModel>();
                //lineDic.Add(lineModel.colorType, lines);
                return;
            }
            if (lines.Contains(lineModel))
                lines.Remove(lineModel);
            if (lines.Count <= 0)
                lineDic.Remove(lineModel.colorType);
        }
        //获取通过此站点线路类型
        public List<ColorType> GetColorTypes()
        {                         
            var keys = lineDic.Select(q => q.Key);
            return keys as List<ColorType>;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"ItemModule OnTriggerEnter2D:{name}");
        }

    }
}

