using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Delivery.Track;
using System.IO;    

namespace Delivery.TrackEditor
{
    public class UITrackEditor : MonoBehaviour
    {
        public InputField _level;
        public InputField _path;
        public InputField _cd;
        public MyButton _saveBtn;
        public MyButton _closeBtn;
        public MyButton _nextBtn;
        public MyButton _tipBtn;
        public MyText _tipTxt;
        public List<Button> _houseBtns;
        public TrackEditorTouch _trackEditorTouch;
        private List<TrackLevelHouse> _trackLevelHouses;
        private string _trackPathKey = "TrackPath";
        private string _trackLevelKey = "TrackLevel";
        private string _trackCDKey = "TrackCD";
        void Start()
        {
            
            _saveBtn?.onClick.AddListener(OnSaveClick);
            _closeBtn?.onClick.AddListener(OnCloseClick);
            _nextBtn?.onClick.AddListener(OnNextClick);
            _tipBtn?.onClick.AddListener(OnTipClick);
            _tipBtn?.gameObject.SetActive(false);
            _cd.text = "0";
            _trackLevelHouses = new List<TrackLevelHouse>();
            for (int i = 0; i < _houseBtns.Count; i++)
            {
                int index = i;
                int id = 4001 + index;
                TrackLevelHouse house = new TrackLevelHouse();
                house.Id = id;
                house.isLock = true;
                _trackLevelHouses.Add(house);
                _houseBtns[index]?.onClick.AddListener(delegate { OnHouseClick(_houseBtns[index].transform,house); });
            }
            OnNextClick();
        }
        
        private void OnHouseClick(Transform trans, TrackLevelHouse house)
        {
            GameObject lockObj = trans.Find("lock").gameObject;
            house.isLock = !house.isLock;
            if (house.isLock)
                lockObj.SetActive(true);
            else
                lockObj.SetActive(false);
        }
        private bool _isInputCtrl;//是否按下ctrl键
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))                    
                _isInputCtrl = true;  
            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))  
                _isInputCtrl = false;  
            if (_isInputCtrl && Input.GetKeyDown(KeyCode.S))   
                OnSaveClick();   
            if (Input.GetKeyDown(KeyCode.Escape))
                OnCloseClick();
            //foreach(KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            //{
            //    Debug.Log($"kcode=={kcode.ToString()}");
            //}
        }
        void OnSaveClick()
        {
            int lv = int.Parse(_level.text);   
            Debug.Log($"lv==={lv}");
            if (!Utils.IsInteger(_level.text))
            {
                SetTipText("关卡ID填写有误，下次注意！！！！！！");
                return;
            }
            PlayerPrefs.SetString(_trackLevelKey, (lv + 1).ToString());
            string path = Path.Combine(_path.text, $"TrackLevel_{_level.text}.json" );
            if (string.IsNullOrEmpty(path))
            {
                SetTipText("文件保存路径为空，下次注意！！！！！！");
                return;
            }
            Debug.Log($"path==={path}");
            float cd = float.Parse(_cd.text);
            PlayerPrefs.SetString(_trackCDKey, _cd.text);
            PlayerPrefs.SetString(_trackPathKey, _path.text);
            _trackEditorTouch.SaveData(path,cd,_trackLevelHouses);
            SetTipText("保存成功，干得漂亮！！！，是否进行下一关卡的编辑？");
        }
        void OnCloseClick()
        {
            Application.Quit();
        }
        void OnNextClick()
        {
            if (PlayerPrefs.HasKey(_trackLevelKey))
                _level.text = PlayerPrefs.GetString(_trackLevelKey);
            if (PlayerPrefs.HasKey(_trackPathKey))
                _path.text = PlayerPrefs.GetString(_trackPathKey);
            if (PlayerPrefs.HasKey(_trackCDKey))
                _cd.text = PlayerPrefs.GetString(_trackCDKey);
            _trackEditorTouch.NextLevel();
        }
        public void SetTipText(string str)
        {
            _tipTxt.text = str;
            _tipBtn.gameObject.SetActive(true);
        }
        void OnTipClick()
        {
            _tipBtn.gameObject.SetActive(false);
        }
    }
}


