using Foundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.Idle
{
    public class IdleMainTaskItem : MonoBehaviour
    {

        private RectTransform rectTrans;
        private IdleTaskItemCtrl idleTaskItemCtrl;
        private IdleTaskWindowLogic idleTaskWindowLogic;

        private RectTransform content;
        private GameObject completeTips;
        private MyText reward;
        private MyText desc;
        private MyText introduce;
        private Scrollbar taskProcess;
        private MyText taskProcessText;
        private MyButton gotoBtn;//前往按钮
        private MyButton receiveBtn;//接受按钮


        private IdleUIAnimation inoutAnim;//移除显示动画

        private void Awake()
        {
            idleTaskWindowLogic = IdleTaskWindowLogic.Instance;
            idleTaskItemCtrl = transform.parent.GetComponent<IdleTaskItemCtrl>();
            rectTrans = GetComponent<RectTransform>();

            content = this.GetComponentByPath<RectTransform>("content");
            completeTips = this.GetComponentByPath<Transform>("completeTips").gameObject;
            reward = content.GetComponentByPath<MyText>("taskIcon/rewardBg/reward");
            introduce = content.GetComponentByPath<MyText>("description");
            desc = content.GetComponentByPath<MyText>("taskName");
            taskProcess = content.GetComponentByPath<Scrollbar>("taskProcess");
            taskProcessText = taskProcess.GetComponentByPath<MyText>("processText");
            gotoBtn = content.GetComponentByPath<MyButton>("btn-go");
            receiveBtn = content.GetComponentByPath<MyButton>("btn-receive");

            Vector2 startPos = new Vector2(rectTrans.anchoredPosition.x + 500, rectTrans.anchoredPosition.y);
            inoutAnim = new IdleUIAnimation(rectTrans, startPos, rectTrans.anchoredPosition, null, OnOutFinish);


        }

        private void Start()
        {
            AddAllListener();
        }


        private void AddAllListener()
        {
            receiveBtn.onClick.AddListener(OnReveiveBtnClick);
            gotoBtn.onClick.AddListener(OnGotoBtnClick);
        }

        private void OnOutFinish()
        {
            InitData();
            inoutAnim.Enter();
        }



        public void InitData()
        {
            PlayerTask playerTask = idleTaskWindowLogic.GetMainTask();
            if (playerTask == null)
            {
                //--------------------------------隐藏相关元素    显示主线任务已经做完
                //gameObject.SetActive(false);
                SetMainTaskComplete();
                return;
            }
            completeTips.SetActive(false);
            IdleMainTask idleTask = idleTaskWindowLogic.GetIdleMainTask();
            reward.text = "x" + UnitConvertMgr.Instance.GetFloatValue(new Long2(idleTask.Reward),2);
            desc.text = idleTask.Desc;
            introduce.text = idleTask.Introduce;

            TaskProcess process = idleTaskWindowLogic.GetCurrentProcessValue(idleTask);
            int currentProcess = process.currentProcess;
            int maxProcess = process.maxProcess;

            if (currentProcess >= maxProcess)
            {
                playerTask.IsFinish = true;
            }
            bool isFinish = playerTask.IsFinish;
            gotoBtn.gameObject.SetActive(!isFinish);
            receiveBtn.gameObject.SetActive(isFinish);

            if (isFinish)
            {
                currentProcess = process.maxProcess;
            }
            taskProcessText.text = $"{ currentProcess}/{maxProcess}";
            float processSize = 0;
            if (maxProcess != 0)
            {
                processSize = (float)process.currentProcess / process.maxProcess;
            }
            taskProcess.size = processSize;


        }


        private void SetMainTaskComplete()
        {
            content.gameObject.SetActive(false);
            completeTips.SetActive(true);
        }

        //领取奖励按钮点击事件
        private void OnReveiveBtnClick()
        {
            Vector3 receiveBtnPos = receiveBtn.GetComponent<RectTransform>().position;
            IdleFxHelper.PlayGetRewardFxUI(receiveBtnPos, null, true);

            idleTaskWindowLogic.OnReceiveMainTaskBtnClick();
            PlayerTask playerTask = idleTaskWindowLogic.GetMainTask();
            if (playerTask == null)
            {
                //隐藏相关UI
                SetMainTaskComplete();
            }
            else
                inoutAnim.Exit();
            
        }

        private void OnGotoBtnClick()
        {
            idleTaskWindowLogic.OnGotoBtnClick();
        }

    }
}


