using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Delivery
{

    /// <summary>
    /// 自定义UI适配（某些UI的适配需求特殊）
    /// </summary>
    public class UIAutoFitScreen : Singleton<UIAutoFitScreen>
    {
        private float defaultWidth = FixScreen.width;
        private float defaultHeight = FixScreen.height;
        public void SetWarehouseAutoFit(RectTransform warehouseBg,RectTransform warehouseParent)
        {
            
            float defaultAspect = defaultWidth / defaultHeight;
            float currentAspect = (float)Screen.width / Screen.height;
            if (currentAspect < defaultAspect)
            {
                float newWidth = currentAspect / defaultAspect * warehouseBg.rect.width;
                float newHeight = warehouseBg.rect.height;
                warehouseBg.sizeDelta = new Vector2(newWidth, newHeight);

                //List<RectTransform> warehouses = new List<RectTransform>();
                //warehouseParent.GetComponentsInChildren<RectTransform>(warehouses);
                //warehouses.Remove(warehouseParent);
                
                float padding = warehouseBg.rect.width / warehouseParent.childCount;
                float startPosX = padding / 2 - newWidth / 2;
                for (int i=0;i<warehouseParent.childCount;i++)
                {
                    float posX= startPosX + padding * i;
                    //warehouses[i].anchoredPosition = new Vector3(posX, warehouses[i].anchoredPosition.y);
                    warehouseParent.GetChild(i).localPosition= new Vector3(posX, warehouseParent.GetChild(i).localPosition.y);
                }


            }
        }
    }


}

