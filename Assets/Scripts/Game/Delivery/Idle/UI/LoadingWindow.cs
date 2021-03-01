using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using DG.Tweening;
using Delivery.Idle;
using Delivery;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadingWindow : MonoBehaviour
{
    //private AsyncOperation async;
    private bool isHide = false;
    private bool isStart = false;
    private float currentProcess;
    private int maxLoadDataCount;
    private Slider processSlider;
    private MyText processText;
    private void Awake()
    {
        processSlider = this.GetComponentByPath<Slider>("process");
        processText = this.GetComponentByPath<MyText>("process/processText");
        processSlider.onValueChanged.AddListener(OnSliderValueChange);
        EventDispatcher.Instance.AddListener(EnumEventType.Event_Load_ProcessChange, OnProcessChange);
        EventDispatcher.Instance.AddListener(EnumEventType.Event_Load_Complete, OnGameBegan);
    
    }

    private void Update()
    {
        if (isStart)
        {
            processSlider.value = Mathf.Lerp(processSlider.value, currentProcess, 0.1f);
            if(1.0f- processSlider.value<=0.05f)
            {
                isStart = false;
                processSlider.value = 1.0f;
                StartCoroutine(LoadingEnd());
            }
        }

    }

    private void OnDestroy()
    {
       
        EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Load_ProcessChange, OnProcessChange);
        EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Load_Complete, OnGameBegan);
    }

    public void Init( int maxDataCount)
    {
        //async = _async;
        //isHide = false;
        //logoCanvasGroup.alpha = 0.0f;
        //logoCanvasGroup.DOFade(1.0f, 3.0f);
        currentProcess =0.5f;
        processSlider.value = 0;
        processText.text = "0%";
        maxLoadDataCount = maxDataCount;
        isStart = true;
    }

    private void OnSliderValueChange(float value)
    {
        value *= 100;
        processText.text = Mathf.CeilToInt(value).ToString() + "%";
    }

    private void OnProcessChange(BaseEventArgs baseEventArgs)
    {
        EventArgsOne<int> argsOne = (EventArgsOne<int>)baseEventArgs;
        int remainLoadData = argsOne.param1;
        currentProcess = 1.0f - (float)remainLoadData / maxLoadDataCount;
        if (currentProcess < 0.5f)
            currentProcess = 0.5f;
    }
    

    private void OnGameBegan(BaseEventArgs baseEventArgs)
    {
        isHide = true;
    }
    //延迟关闭loading页
    private IEnumerator LoadingEnd()
    {
        yield return new WaitForSeconds(0.5f);
        InitGame();
    }

    private void InitGame()
    {
       
        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Game_Began));
        UIController.Instance.OpenPageFromAssets<IdleWindow>("IdleWindow.prefab");
        UIController.Instance.ShowBlocker("IdleTopUI.prefab", true);
        if (PlayerMgr.Instance.IsNewPlayer)
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_Guide_StartGuide, 1));
        }
        else
        {
            PlayerMgr.Instance.ResetLastGuide();
        }
        int leaveSecond = (int)PlayerMgr.Instance.GetLeaveCityTime();
        if (leaveSecond / 60 >= 5)
        {
            UIController.Instance.ShowBlocker("OfflineWindow.prefab", true);
        }
        Destroy(gameObject);
    }

}
