using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

namespace Delivery
{
    public class UIStagePage : UIPage
    {
        MyButton bgBtn;
        protected override void InitPage(object args = null)
        {
            
            
        }
        public override void Open()
        {
            base.Open();
            Init(); 
        }
        public override void Hide()
        {
            base.Hide();
            
        }
    }
}

