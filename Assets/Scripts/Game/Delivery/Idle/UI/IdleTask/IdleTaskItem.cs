using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
using Foundation;

namespace Delivery.Idle
{
    public class IdleTaskItem : MonoBehaviour
    {
        private int _index;

        private RectTransform rectTrans;
        private IdleTaskItemCtrl idleTaskItemCtrl;
        private IdleTaskWindowLogic idleTaskWindowLogic;

        private MyText reward;
        private MyText desc;
        private MyText introduce;
        private Scrollbar taskProcess;
        private MyText taskProcessText;
        private MyButton gotoBtn;//前往按钮
        private MyButton receiveBtn;//接受按钮

        private IdleUIAnimation moveAnim;//前移动画
        private IdleUIAnimation inoutAnim;//移除显示动画

        private void Awake()
        {
            idleTaskWindowLogic = IdleTaskWindowLogic.Instance;
            idleTaskItemCtrl = transform.parent.GetComponent<IdleTaskItemCtrl>();
            rectTrans = GetComponent<RectTransform>();
            _index = int.Parse(gameObject.name.Split('_')[1]) ;

            reward = this.GetComponentByPath<MyText>("taskIcon/rewardBg/reward");
            introduce = this.GetComponentByPath<MyText>("description");
            desc = this.GetComponentByPath<MyText>("taskName");
            taskProcess = this.GetComponentByPath<Scrollbar>("taskProcess");
            taskProcessText = taskProcess.GetComponentByPath<MyText>("processText");
            gotoBtn = this.GetComponentByPath<MyButton>("btn-go");
            receiveBtn = this.GetComponentByPath<MyButton>("btn-receive");

            moveAnim = new IdleUIAnimation(rectTrans, rectTrans.anchoredPosition, rectTrans.anchoredPosition);
            inoutAnim = new IdleUIAnimation(rectTrans, rectTrans.anchoredPosition, rectTrans.anchoredPosition, null, OnOutFinish);

           
        }

        private void Start()
        {
            AddAllListener();
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_TaskItem_Out, OnTaskItemOut);
        }

        private void AddAllListener()
        {
            receiveBtn.onClick.AddListener(OnReveiveBtnClick);
            gotoBtn.onClick.AddListener(OnGotoBtnClick);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_TaskItem_Out, OnTaskItemOut);
        }
        
        private void OnOutFinish()
        {
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_TaskItem_Out, _index));
        }
        
        private void OnTaskItemOut(BaseEventArgs baseEventArgs)
        {
            EventArgsOne<int> argsOne = (EventArgsOne<int>)baseEventArgs;
            int index = argsOne.param1;
            if(_index==index)
            {
                _index = idleTaskItemCtrl.GetEndIndex();
                InitData();
                Vector2 endPos = idleTaskItemCtrl.GetEndPos();
                inoutAnim.SetStartPos(endPos + new Vector2(500, 0));
                inoutAnim.SetEndPos(endPos);
                inoutAnim.Enter();
            }
            else if(_index>index)
            {
                Vector2 newPos = idleTaskItemCtrl.GetPosByIndex(_index - 1);
                moveAnim.SetStartPos(rectTrans.anchoredPosition);
                moveAnim.SetEndPos(newPos);
                moveAnim.Enter(false);
                _index -= 1;
            }
        }

        public void InitData()
        {
            PlayerTask playerTask = idleTaskWindowLogic.GetPlayerTaskByIndex(_index);
            if (playerTask == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            IdleTask idleTask = idleTaskWindowLogic.GetIdleTaskByIndex(_index);
            reward.text = "x" + UnitConvertMgr.Instance.GetFloatValue(new Long2(idleTask.Reward),2);
            desc.text = idleTask.Desc;
            introduce.text = idleTask.Introduce;
         
            TaskProcess process = idleTaskWindowLogic.GetCurrentProcessValue(idleTask);
            int currentProcess = process.currentProcess;
            int maxProcess = process.maxProcess;
            
            if(currentProcess>=maxProcess)
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
            if(maxProcess!=0)
            {
                processSize= (float)process.currentProcess / process.maxProcess;
            }
            taskProcess.size = processSize;


        }

        //领取奖励按钮点击事件
        private void OnReveiveBtnClick()
        {
            Vector3 receiveBtnPos = receiveBtn.GetComponent<RectTransform>().position;
            IdleFxHelper.PlayGetRewardFxUI(receiveBtnPos, null,true);

            idleTaskWindowLogic.OnReveiveBtnClick(_index);
            Vector2 startPos = new Vector2(rectTrans.anchoredPosition.x + 500, rectTrans.anchoredPosition.y);
            inoutAnim.SetStartPos(startPos);
            inoutAnim.SetEndPos(rectTrans.anchoredPosition);
            inoutAnim.Exit();
        }

       private void OnGotoBtnClick()
        {
            idleTaskWindowLogic.OnGotoBtnClick(_index);
        }
        
    }

}
