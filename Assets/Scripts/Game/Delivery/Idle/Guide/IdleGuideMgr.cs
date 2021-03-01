using Foundation;
using libx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdleGuideMgr : MonoSingleton<IdleGuideMgr>
    {
        private Dictionary<int, IdleGuideStep> idleGuideStepsDic = new Dictionary<int, IdleGuideStep>();
        private Dictionary<int, IdleGuide> idleGuidesDic = new Dictionary<int, IdleGuide>();
        private int _loadGuideData = 2;
        public void Init()
        {
            ReadFile();
        }
        private void ReadFile()
        {
            Assets.LoadAssetAsync(Files.idleGuideStep, typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<IdleGuideStep> idleGuideSteps = FullSerializerAPI.Deserialize(typeof(List<IdleGuideStep>), jsonStr) as List<IdleGuideStep>;
                    idleGuideStepsDic = idleGuideSteps.ToDictionary(key => key.Id, value => value);
                    _loadGuideData--;
                    if (_loadGuideData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleGuideStep}读取失败,error={request.error}");
            };
            Assets.LoadAssetAsync(Files.idleGuide, typeof(TextAsset)).completed += delegate (AssetRequest request)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    string jsonStr = (request.asset as TextAsset).text;
                    List<IdleGuide> idleGuides = FullSerializerAPI.Deserialize(typeof(List<IdleGuide>), jsonStr) as List<IdleGuide>;
                    idleGuidesDic = idleGuides.ToDictionary(key => key.Id, value => value);
                    _loadGuideData--;
                    if (_loadGuideData <= 0)
                        EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
                }
                else
                    LogUtility.LogError($"{Files.idleGuide}读取失败,error={request.error}");
            };
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleGuideStep), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleGuideStep> idleGuideSteps = FullSerializerAPI.Deserialize(typeof(List<IdleGuideStep>), jsonStr) as List<IdleGuideStep>;
            //        idleGuideStepsDic = idleGuideSteps.ToDictionary(key => key.Id, value => value);
            //        _loadGuideData--;
            //        if (_loadGuideData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
            //StreamFile.ReaderFile(StreamFile.Combine(Files.jsonFolder, Files.idleGuide), this, (jsonStr) => {
            //    if (!string.IsNullOrEmpty(jsonStr))
            //    {
            //        List<IdleGuide> idleGuides = FullSerializerAPI.Deserialize(typeof(List<IdleGuide>), jsonStr) as List<IdleGuide>;
            //        idleGuidesDic = idleGuides.ToDictionary(key => key.Id, value => value);
            //        _loadGuideData--;
            //        if (_loadGuideData <= 0)
            //            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_IdleData));
            //    }
            //});
        }


        public IdleGuideStep GetIdleGuideStep(int stepId)
        {
            if (!idleGuideStepsDic.ContainsKey(stepId)) return null;
            return idleGuideStepsDic[stepId];
        }

        public IdleGuide GetIdleGuide(int guideId)
        {
            if (!idleGuidesDic.ContainsKey(guideId)) return null;
            return idleGuidesDic[guideId];
        }
        //获取步骤索引
        public int IndexOfGuide(int guideId,int stepId)
        {
            IdleGuide idleGuide = GetIdleGuide(guideId);
            if (idleGuide == null) return -1;
            string allSteps = idleGuide.GuideStepIds;
            return allSteps.IndexOf(stepId.ToString());
        }

        public Dictionary<int, IdleGuide> GetAllIdleGuides()
        {
            return idleGuidesDic;
        }

        public List<IdleGuideStep> GetAllGuideSteps(IdleGuide idleGuide)
        {
            if (idleGuide == null) return new List<IdleGuideStep>();
            List<IdleGuideStep> idleGuideSteps = new List<IdleGuideStep>();
            string[] stepIdStr = idleGuide.GuideStepIds.Split(',');
            for (int i = 0; i < stepIdStr.Length; i++)
            {
                idleGuideSteps.Add(GetIdleGuideStep(int.Parse(stepIdStr[i])));
            }
            return idleGuideSteps;
        }
        public List<IdleGuideStep>GetAllGuideSteps(int guideId)
        {
            if(!idleGuidesDic.ContainsKey(guideId)) return new List<IdleGuideStep>();
            List<IdleGuideStep> idleGuideSteps = new List<IdleGuideStep>();
            IdleGuide guide = GetIdleGuide(guideId);
            return GetAllGuideSteps(guide);
        }

    }

    

}

