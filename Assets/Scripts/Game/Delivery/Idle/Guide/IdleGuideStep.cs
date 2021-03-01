using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdleGuideStep
    {
        public int Id;
        public int ActionType;
        public int EndCondition;
        public int ClickCount;
        public float Delay;
        public bool CloseWindow;
        public bool IsUI;
        public bool NeedFocus;
        public bool NeedFollowTarget;
        public string ParentTransName;
        public string TargetTransName;
        public int UIItemIndex;
        public string TipKey;
        public bool PauseGame;
        public int Finger;
    }
}
