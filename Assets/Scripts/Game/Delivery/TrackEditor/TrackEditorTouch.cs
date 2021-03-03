using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using Foundation;
using Delivery.Track;

namespace Delivery.TrackEditor
{
    public class TrackEditorTouch : TrackEditorBaseTouch
    {
        public RectTransform _siteRectTrans;
        public TrackEditorDrawGrid _drawGrid;
        private PointerEventData _eventData;
        private TrackLevel _trackLevel;
        private List<TrackEditorModel> _saveModels = new List<TrackEditorModel>();//需要保存的
        private List<TrackEditorModel> _poolModels = new List<TrackEditorModel>();//对象池 丢弃的model会放到这里
        private TrackEditorModel _touchModel;
        public override void Start()
        {
            base.Start();
            _eventData = new PointerEventData(EventSystem.current);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_TrackEditor_ModelInput, ChangeIndex);
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
        public override void OnTouchBegan(Vector3 touchPos)
        {
            base.OnTouchBegan(touchPos);
            _touchModel = PointerOverTrack(touchPos);
            if(_touchModel != null  )
            {
                if(!_saveModels.Contains(_touchModel))
                {
                    //点击的是样本
                    TrackEditorModel model = GetModelPool(_touchModel);
                    if (model == null)
                        _touchModel = CloneModel(_touchModel);
                    else
                        _touchModel = model;
                }
                else
                {
                    _drawGrid.RemoveUsedPos(_touchModel.RectTrans);
                }
                
            }
        }
        public override void OnTouchMoved(Vector3 touchPos)
        {
            base.OnTouchMoved(touchPos);
            if (_touchModel == null) return;
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RootCanvas.Instance.RectTrans, touchPos, RootCanvas.Instance.UICamera, out pos);
            _touchModel.RectTrans.anchoredPosition = pos;

        }
        public override void OnTouchEnded(Vector3 touchPos)
        {
            base.OnTouchEnded(touchPos);
            if (_touchModel == null) return;
            Vector2 pos = _drawGrid.GetTrackEditorModelPos(_touchModel.RectTrans);
            _touchModel.RectTrans.anchoredPosition = pos;
            if (pos != new Vector2(-10000, -10000))
            {
                if (_poolModels.Contains(_touchModel))
                    _poolModels.Remove(_touchModel);
                if (!_saveModels.Contains(_touchModel))
                {
                    _saveModels.Add(_touchModel);
                    _touchModel.SetText(_saveModels.Count);
                }
                
            }
            else
            {
                if (_saveModels.Contains(_touchModel))
                {
                    int index = _saveModels.IndexOf(_touchModel);
                    _saveModels.Remove(_touchModel);
                    ResetSaveModels(index);
                }  
                if(!_poolModels.Contains(_touchModel))
                    _poolModels.Add(_touchModel);
            }
            _touchModel = null;
        }
        private void ChangeIndex(BaseEventArgs args)
        {
            EventArgsTwo<TrackEditorModel, string> arg = args as EventArgsTwo<TrackEditorModel, string>;
            if (!Utils.IsInteger(arg.param2))
            {
                arg.param1.ResetText();
                return;
            }
            int index = int.Parse(arg.param2);
            if(index <=0 || index >_saveModels.Count)
            {
                arg.param1.ResetText();
                return;
            }
            ResetSaveModels(arg.param1,index);
        }
        private void ResetSaveModels(TrackEditorModel model,int index)
        {
            if (!_saveModels.Contains(model)) return;
            _saveModels.Remove(model);
            _saveModels.Insert(index - 1, model);
            ResetSaveModels(index);
        }
        private void ResetSaveModels(int index)
        {
            for (int i = 0; i < _saveModels.Count; i++)
            {
                int ind = i + 1;
                _saveModels[i].SetText(ind);
            }
        }
        private TrackEditorModel GetModelPool(TrackEditorModel model)
        {
            for(int i = 0; i < _poolModels.Count; i++)
            {
                if (_poolModels[i].Id == model.Id)
                    return _poolModels[i];
            }
            return null;
        }
        private TrackEditorModel CloneModel(TrackEditorModel model)
        {
            TrackEditorModel tModel = Instantiate(model.gameObject).GetComponent<TrackEditorModel>();
            tModel.RectTrans.SetParent(_siteRectTrans);
            tModel.RectTrans.localScale = Vector3.one;
            return tModel;
        }
        private TrackEditorModel PointerOverTrack(Vector2 mousePosition)
        {
            _eventData.position = mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            //向点击位置发射一条射线，检测是否点击UI
            EventSystem.current.RaycastAll(_eventData, results);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    GameObject resultObj = results[i].gameObject;
                    if (resultObj.tag != Tags.Item) continue;
                    TrackEditorModel resultModule = resultObj.GetComponent<TrackEditorModel>();
                    if (resultModule != null)
                    {
                        return resultModule;
                    }
                }
            }
            return null;
        }
        public void SaveData(string path,float cd,List<TrackLevelHouse> houses)
        {
            _trackLevel = new TrackLevel();
            _trackLevel.houses = houses;
            _trackLevel.sites = new List<TrackLevelSite>();
            _trackLevel.createCD = cd;
            for (int i = 0; i < _saveModels.Count; i++)
            {
                TrackEditorModel model = _saveModels[i];
                TrackLevelSite site = new TrackLevelSite();
                site.Id = model.Id;
                site.x = model.RectTrans.anchoredPosition.x;
                site.y = model.RectTrans.anchoredPosition.y;
                _trackLevel.sites.Add(site);
            }
            string jsonStr =  FullSerializerAPI.Serialize(typeof(TrackLevel), _trackLevel,false,false);
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream))
                {
                    textWriter.Write(jsonStr);
                }
            }
        }
        public void NextLevel()
        {
            for(int i = _saveModels.Count - 1; i >= 0; i--)
            {
                _saveModels[i].RectTrans.anchoredPosition = new Vector2(-10000, -10000);
                _poolModels.Add(_saveModels[i]); 
            }
            _saveModels.Clear();
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_TrackEditor_ModelInput, ChangeIndex);
        }

    }
}

