using Delivery;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonInfo))]
public class MyButton : Button
{
    private ButtonInfo _buttonInfo;
    private ButtonInfo buttonInfo
    {
        get
        {
            if (_buttonInfo == null)
                _buttonInfo = GetComponent<ButtonInfo>();
            return _buttonInfo;
        }
    }
    protected override void Start()
    {
        base.Start();
        this.onClick.AddListener(() => {
            if (string.IsNullOrEmpty(buttonInfo.clipName))
                LogUtility.LogError($"{name}没有音效");
            else
                AudioCtrl.Instance.PlaySingleSound2D(buttonInfo.clipName);
        });
    }
}  

