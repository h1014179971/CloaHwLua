using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Delivery.Idle
{
    [RequireComponent(typeof(BoxCollider))]
    public class IdleBuildModel : MonoBehaviour
    {
        public List<BehaviorDesigner.Runtime.Behavior> _behaviors;
        public void OnTouchClick()
        {
            
            //transform.DOBlendableLocalMoveBy(Vector3.up, 3f);
            //transform.DOBlendableScaleBy(Vector3.up, 3f);
            if (IsPlay())
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(transform.DOBlendableLocalMoveBy(Vector3.up * 0.2f, 0.07f));
                sequence.Join(transform.DOBlendableScaleBy(Vector3.up * 0.1f, 0.07f));
                sequence.Append(transform.DOBlendableLocalMoveBy(-Vector3.up * 0.2f, 0.07f));
                sequence.Join(transform.DOBlendableScaleBy(-Vector3.up * 0.1f, 0.07f));
                for (int i = 0; i < _behaviors.Count; i++)
                {
                    BehaviorDesigner.Runtime.Behavior behavior = _behaviors[i];
                    if (behavior.ExecutionStatus == BehaviorDesigner.Runtime.Tasks.TaskStatus.Running) continue;
                    behavior.OnBehaviorEnd += BehaviorEnded;
                    behavior.EnableBehavior();

                }

            }
             
        }
        private bool IsPlay()
        {
            for (int i = 0; i < _behaviors.Count; i++)
            {
                BehaviorDesigner.Runtime.Behavior behavior = _behaviors[i];
                if (behavior.ExecutionStatus == BehaviorDesigner.Runtime.Tasks.TaskStatus.Running) return false;
            }
            return true;
        }
        private void BehaviorEnded(BehaviorDesigner.Runtime.Behavior behavior)
        {
            behavior.DisableBehavior();
            behavior.OnBehaviorEnd -= BehaviorEnded;
        }
    }
}


