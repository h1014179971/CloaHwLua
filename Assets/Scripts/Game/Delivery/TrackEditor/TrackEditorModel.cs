using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery.TrackEditor
{
    public class TrackEditorModel : MonoBehaviour
    {
        public int Id;
        public InputField _inputField;
        private string _inputTxt;
        private RectTransform _rectTrans;
        public RectTransform RectTrans
        {
            get
            {
                if (_rectTrans == null)
                {
                    _rectTrans = GetComponent<RectTransform>();
                }
                return _rectTrans;
            }
        }
        private void Start()
        {
            _inputField.onEndEdit.AddListener((value) =>
            {     
                EventDispatcher.Instance.TriggerEvent(new EventArgsTwo<TrackEditorModel,string>( EnumEventType.Event_TrackEditor_ModelInput, this,value));
            });
        }
        public void SetText(int index)
        {
            SetText(index.ToString());
        }
        public void SetText(string index)
        {
            _inputField.text = index;
            _inputTxt = index;
        }
        public void ResetText()
        {
            _inputField.text = _inputTxt;
        }
    }
}

