using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.Events;

public class MessageBox : UIFramework.UIPopup {
    
    private Text _MsgText;
    private Button _ConfirmButton;
    private Button _RefuseButton;
    private Button _CancelButton;

    protected override void InitPopup()
    {
        RectTransform panel = this.transform.Find("Panel").GetComponent<RectTransform>();
        _MsgText = panel.Find("Message").GetComponent<Text>();
        _ConfirmButton = panel.Find("ButtonPanel/ConfirmButton").GetComponent<Button>();
        _RefuseButton = panel.Find("ButtonPanel/RefuseButton").GetComponent<Button>();
        _CancelButton = panel.Find("CancelButton").GetComponent<Button>();
        
        OpenAnimation.Add(new UIFramework.MoveUpFromDownAnimation(panel, Ease.OutCubic));
        
        CloseAnimation.Add(new UIFramework.MoveDownAnimaiton(panel, 1, Ease.InCubic));
        //ShowAnimation.Add(showAnim);
        //HideAnimation.Add(downAnim);

        //AfterClose.AddListener(() => {
        //    Destroy(this.gameObject);
        //});

        _CancelButton.onClick.AddListener(OnCancel);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 792f);                        
    }

    public void SetMessageBox(string msg, string confirmBtnText, string refuseBtnText)
    {
        //if (string.IsNullOrEmpty(confirmBtnText) && string.IsNullOrEmpty(refuseBtnText))
        //{
        //    confirmBtnText = "好的";
        //}
        SetMessage(msg);
        SetConfirmButton(confirmBtnText);
        SetRefuseButton(refuseBtnText);
    }

    public void SetMessage(string msg)
    {
        _MsgText.text = msg;
    }

    private void SetConfirmButton(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _ConfirmButton.gameObject.SetActive(false);
            return;
        }

        Text t = _ConfirmButton.transform.Find("Text").GetComponent<Text>();
        t.text = text;

        float textWidth = text.Length * t.fontSize;
        float w = textWidth + 130 * 2;
        _ConfirmButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);


        _ConfirmButton.onClick.AddListener(OnConfirm);


    }

    private void SetRefuseButton(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _RefuseButton.gameObject.SetActive(false);
            return;
        }
        //_RefuseButton.transform.Find("Text").GetComponent<Text>().text = text;

        Text t = _RefuseButton.transform.Find("Text").GetComponent<Text>();
        t.text = text;

        float textWidth = text.Length * t.fontSize;
        float w = textWidth + 130 * 2;
        _RefuseButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);



        _RefuseButton.onClick.AddListener(OnRefuse);
    }

    //private void SetCancelButton(string text)
    //{
    //    if (string.IsNullOrEmpty(text))
    //    {
    //        _CancelButton.gameObject.SetActive(false);
    //        return;
    //    }
    //    _CancelButton.transform.Find("Text").GetComponent<Text>().text = text;
    //    _CancelButton.onClick.AddListener(OnCancel);
    //}

    public void ClickConfirmBtn()
    {
        OnConfirm();
    }

    public void ClickRefuseBtn()
    {
        OnRefuse();
    }

    public void ClickCancelBtn()
    {
        OnCancel();
    }
}
