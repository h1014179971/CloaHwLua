using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using Foundation;

public class MainWindow : UIPage
{
    public int remainTime = 600;//剩余时间
    public int allGoodsCount = 100;//货物总量
    private int remainGoodsCount;//当前剩余货物量
    private int starCount;//获得的星数
    private Text timeText;//倒计时文本
    private Text goodsCountText;//货物数量文本
    private Transform starContent;//星星的父节点
    private GameObject starPrefab;//星星预设
    private Button pauseBtn;//暂停按钮
    private void Awake()
    {
        timeText = this.GetComponentByPath<Text>("bg/timeBox/time");
        pauseBtn = this.GetComponentByPath<Button>("bg/pauseBtn");
        goodsCountText = this.GetComponentByPath<Text>("bg/goodsIcon/goodsCount");
        starContent = this.GetComponentByPath<Transform>("bg/starBox/starContent");
    }

    // Start is called before the first frame update
    void Start()
    {
        //初始化倒计时文本
        timeText.text = TimeUtils.secondsToString(remainTime);
        int timerRepeatCount = remainTime;//倒计时每秒更新一次文本信息
        //初始化货物数量文本
        remainGoodsCount = allGoodsCount;
        goodsCountText.text = $"{remainGoodsCount}/{allGoodsCount}";
        //初始化星数
        starCount = 0;
        starPrefab = Resources.Load("Prefabs/Star/star") as GameObject;
        //控件相关事件绑定
        Timer.Instance.Register("CountDown", 1, timerRepeatCount, false, OnTimerCountDown).AddTo(gameObject);//倒计时
        pauseBtn.onClick.AddListener(OnPauseBtnClick);//暂停
        EventDispatcher.Instance.AddListener(EnumEventType.Event_UIMainWindow_Goods, ReduceGoods);//货物数量变化
        EventDispatcher.Instance.AddListener(EnumEventType.Event_UIMainWindow_Star, AddStar);//星数变化
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener(EnumEventType.Event_UIMainWindow_Goods, ReduceGoods);
        EventDispatcher.Instance.RemoveListener(EnumEventType.Event_UIMainWindow_Star, AddStar);
    }

    /// <summary>
    /// 倒计时回调
    /// </summary>
    private void OnTimerCountDown(params object[] objects)
    {
        remainTime--;
        timeText.text = TimeUtils.secondsToString(remainTime);
        if(remainTime<=0)
        {
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_UIMainWindow_TimeOut));
        }
    }


    /// <summary>
    /// 减少货物
    /// </summary>
    /// <param name="count">减少的数量</param>
    private void ReduceGoods(BaseEventArgs baseEventArgs)
    {
        EventArgsOne<int> eventArgs = (EventArgsOne<int>)baseEventArgs;
        //remainGoodsCount -= count;
        remainGoodsCount -= eventArgs.param1;
        if (remainGoodsCount <= 0)
        {
            remainGoodsCount = 0;
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_UIMainWindow_Complete));
        }
        goodsCountText.text = $"{remainGoodsCount}/{allGoodsCount}";
    }

    /// <summary>
    /// 增加星数
    /// </summary>
    private void AddStar(BaseEventArgs baseEventArgs)
    {
        starCount++;
        if(starCount>3)
        {
            starCount = 3;
            return;
        }
        Instantiate(starPrefab,starContent,false);
    }

    /// <summary>
    /// 暂停按钮点击事件
    /// </summary>
    private void OnPauseBtnClick()
    {
        if(Time.timeScale==0)
        {
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
    
    protected override void InitPage(object args = null)
    {

    }

}
