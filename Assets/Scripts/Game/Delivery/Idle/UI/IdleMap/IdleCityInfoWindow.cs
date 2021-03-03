using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
namespace Delivery.Idle
{
    public class IdleCityInfoWindow : UIWindow
    {

        private IdleCityInfoLogic idleCityInfoLogic;
        private MyButton closeBtn;

        private void Awake()
        {
            idleCityInfoLogic = IdleCityInfoLogic.Instance;
            closeBtn = this.GetComponentByPath<MyButton>("bg/btn-close");
        }

        private void Start()
        {
            closeBtn.onClick.AddListener(idleCityInfoLogic.OnCloseBtnClick);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void Open()
        {
            base.Open();
        }

        public override void Show()
        {
            base.Show();
        }

        protected override void InitWindow(object arg = null)
        {
        }
    }
}


