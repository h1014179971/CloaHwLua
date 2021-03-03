using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Delivery.Idle
{
    public class IdleTruckInfoItem : ScrollRectItem
    {
        //    private RectTransform rectTrans;
        //    private IdleTruckInfoLogic idleTruckInfoLogic;//当前窗体的逻辑脚本

        //    #region UI控件

        //    private Image truckIcon;//货车图标
        //    private MyText truckVolumeLv;//货车容量等级
        //    private CustomSlider truckCountSlider;//货车数量滑动条
        //    private RectTransform enableBgRect;//滑动条可用背景
        //    private MyButton addBtn;//增加按钮
        //    private MyButton minusBtn;//减少按钮
        //    private MyText truckCount;//货车数量
        //    private MyText truckName;//货车名

        //    private Transform itemIcons;
        //    #endregion

        //    private float sliderBgWidth;

        //    private void Awake()
        //    {

        //        rectTrans = transform.GetComponent<RectTransform>();

        //        #region UI控件的初始化
        //        truckIcon = this.GetComponentByPath<Image>("truckInfoNode/truckIcon");
        //        truckVolumeLv = this.GetComponentByPath<MyText>("truckInfoNode/truckIcon/truckVolumeLv");
        //        truckCountSlider = this.GetComponentByPath<CustomSlider>("truckInfoNode/truckCountSlider");
        //        enableBgRect = truckCountSlider.GetComponentByPath<RectTransform>("enableBg");
        //        addBtn = this.GetComponentByPath<MyButton>("truckInfoNode/truckCountSlider/btn-add");
        //        minusBtn = this.GetComponentByPath<MyButton>("truckInfoNode/truckCountSlider/btn-minus");
        //        truckCount = this.GetComponentByPath<MyText>("truckInfoNode/truckCount");
        //        truckName = this.GetComponentByPath<MyText>("truckInfoNode/turckName");

        //        itemIcons = this.GetComponentByPath<Transform>("truckInfoNode/items");
        //        #endregion

        //        sliderBgWidth = truckCountSlider.GetComponentByPath<RectTransform>("Background").rect.width;

        //        #region 添加事件监听
        //        AddAllUIListener();

        //        #endregion
        //    }

        //    private void OnDestroy()
        //    {
        //        EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Truck_EnableCountChange, OnEnableTruckCountChange);
        //    }

        //    /// <summary>
        //    /// 添加所有UI事件监听
        //    /// </summary>
        //    private void AddAllUIListener()
        //    {
        //        truckCountSlider.onEnableMaxValueChange.AddListener(SetEnableBgWidth);
        //        truckCountSlider.onValueChanged.AddListener(OnSliderValueChange);
        //        addBtn.onClick.AddListener(OnAddBtnClick);
        //        minusBtn.onClick.AddListener(OnMinusBtnClick);

        //        EventDispatcher.Instance.AddListener(EnumEventType.Event_Truck_EnableCountChange, OnEnableTruckCountChange);
        //    }


        //    #region UI响应事件
        //    /// <summary>
        //    /// 设置可用背景宽度
        //    /// </summary>
        //    /// <param name="currentEnableValue"></param>
        //    private void SetEnableBgWidth(float currentEnableValue)
        //    {
        //        float enableRate = currentEnableValue / truckCountSlider.maxValue;
        //        float right = sliderBgWidth * (1 - enableRate);
        //        enableBgRect.offsetMax = new Vector2(-right, enableBgRect.offsetMax.y);

        //    }


        //    /// <summary>
        //    /// 滑动条数值变化响应
        //    /// </summary>
        //    /// <param name="value"></param>
        //    private void OnSliderValueChange(float value)
        //    {
        //        int count = Mathf.RoundToInt(value);
        //        truckCount.text = count.ToString();
        //        idleTruckInfoLogic.SetTruckCount(Index, count);
        //        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Truck_EnableCountChange));//通知其他item修改slider属性
        //    }

        //    private void OnEnableTruckCountChange(BaseEventArgs baseEventArgs)
        //    {
        //        InitData();
        //    }

        //    private void OnAddBtnClick()
        //    {
        //        truckCountSlider.value += 1;
        //    }

        //    private void OnMinusBtnClick()
        //    {
        //        truckCountSlider.value -= 1;
        //    }

        //    #endregion

        //    /// <summary>
        //    /// 初始化数据
        //    /// </summary>
        //    private void InitData()
        //    {
        //        if (idleTruckInfoLogic == null)
        //            idleTruckInfoLogic = IdleTruckInfoLogic.Instance;
        //        IdleTruck truck = idleTruckInfoLogic.GetIdleTruckByIndex(Index);

        //        int usedCount = idleTruckInfoLogic.GetTruckCountByIndex(Index);
        //        truckCount.text = usedCount.ToString();
        //        truckCountSlider.maxValue = idleTruckInfoLogic.GetTruckCount();
        //        truckCountSlider.EnableMaxValue = idleTruckInfoLogic.GetEnableTruckCount() + usedCount;
        //        truckCountSlider.value = usedCount;
        //        truckVolumeLv.text = "Lv." + idleTruckInfoLogic.GetTruckLv().ToString();
        //        truckName.text = truck.name;



        //        Sprite[] icons = idleTruckInfoLogic.GetItemSprite(Index);
        //        for(int i=0;i<3;i++)
        //        {
        //            Image icon = itemIcons.GetComponentByPath<Image>("item" + i.ToString());
        //            if(i>=icons.Length)
        //            {
        //                icon.gameObject.SetActive(false);
        //            }
        //            else
        //            {
        //                icon.sprite = icons[i];
        //                icon.gameObject.SetActive(true);
        //            }
        //        }

        //    }


        //    public override int Index {
        //        get
        //        {
        //            return base.Index;
        //        }
        //        set
        //        {
        //            _index = value;
        //            rectTrans.anchoredPosition = _scroller.GetPosition(_index);
        //            InitData();
        //        }
        //    }

        //    public override void CreateItem()
        //    {
        //        base.CreateItem();
        //    }

        //    public override void Init()
        //    {
        //        base.Init();
        //    }
    }

}


