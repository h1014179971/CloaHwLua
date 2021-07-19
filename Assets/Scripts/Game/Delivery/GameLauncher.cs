using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delivery.Idle;
using UnityEngine.Events;
using Foundation;
using System;
using UIFramework;
using libx;
using UnityEngine.SceneManagement;

namespace Delivery
{
#if UNITY_EDITOR
    public class EditorGameLauncher 
    {
        public static List<string> _searchPath = new List<string>();
    }
#endif
    
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField] private Updater _updater;
        private int _loadDataLength = 11;//数据+场景
        //private bool _isLoadScene;
        private GameState _gameState = GameState.None;
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;//节电模式
            Application.targetFrameRate = 30;
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Load_PlayerData, LoadPlayerData);
            PlayerMgr.Instance.Init();
            AudioCtrl.Instance.Init();
        }
        IEnumerator AssetsInit()
        {
            var init = Assets.Initialize();
            yield return init;
            if (string.IsNullOrEmpty(init.error))
            {
#if UNITY_EDITOR
                for(int i = 0; i < EditorGameLauncher._searchPath.Count; i++)
                {
                    Assets.AddSearchPath(EditorGameLauncher._searchPath[i]);
                }
#else
                for(int i = 0; i < init._seachPath.Count; i++)
                {
                    string seachPath = init._seachPath[i];
                    Assets.AddSearchPath(seachPath);
                }
#endif

                init.Release();
                ColaFramework.GameManager.Instance.InitGameCore(gameObject);
                //yield break;
                _updater.Init();
                AudioCtrl.Instance.PlayBackgroundMusic(GameAudio.login);
                Debug.Log("start load data");
                LanguageCtrl.Instance.Init();
                PlatformFactory.Instance.initSDK();
                IdleCityMgr.Instance.Init();
                IdleSiteMgr.Instance.Init();
                IdleTruckMgr.Instance.Init();
                IdleItemMgr.Instance.Init();
				IdleCityTruckMgr.Instance.Init();
                IdleShelfResMgr.Instance.Init();
                IdleTaskMgr.Instance.Init();
                IdleGuideMgr.Instance.Init();
                IdleSpecialEventMgr.Instance.Init();
                UnitConvertMgr.Instance.Init();
                
            }
            else
            {
                init.Release();
                Debug.LogError($"Assets 初始化错误");
            }
        }
        private void LoadPlayerData(BaseEventArgs args)
        {
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Load_IdleData, LoadTrackData);
            EventDispatcher.Instance.AddListener(EnumEventType.Event_Game_Began, StartGame);
            StartCoroutine(AssetsInit());

        }
        private void LoadTrackData(BaseEventArgs args)
        {
            _loadDataLength--;
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_Load_ProcessChange, _loadDataLength));
            if (_loadDataLength > 0) return;
            //开始游戏 
            GameBegan();
            

        }
        private void LoadScene()
        {
            int cityId = PlayerMgr.Instance.CityId;
            string sceneName = "IdleScene_" + cityId;
            AsyncOperation async = null;
            UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode> onSceneLoadedAction = null;
            onSceneLoadedAction = (scene, mode) => {
                AudioCtrl.Instance.Dispose();
                if (_loadDataLength <= 0)
                    GameBegan();
                UnityEngine.SceneManagement.SceneManager.sceneLoaded -= onSceneLoadedAction;
            };
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoadedAction;
            async = SceneCtrl.Instance.LoadScene(sceneName);

            GameObject loadingPanel = UIController.Instance.ShowBlocker("UI/Loading/LoadingWindow");
            loadingPanel.GetComponent<LoadingWindow>().Init(_loadDataLength);
        }
        public void LoadGameScene()
        {
            StartCoroutine(AssetLoadScene());
        }
        IEnumerator AssetLoadScene()
        {

            //GameObject loadingPanel = UIController.Instance.ShowBlocker("UI/Loading/LoadingWindow");
            //loadingPanel.GetComponent<LoadingWindow>().Init( _loadDataLength);
            yield return null;
            int cityId = PlayerMgr.Instance.CityId;
            string sceneName = "IdleScene_" + cityId+".unity";
            var scene = Assets.LoadSceneAsync(sceneName, false);
           
            while (!scene.isDone)
            {
                Debug.Log($"progress==={scene.progress}");
                yield return null;
            }
            yield return scene.isDone;
            Time.timeScale = 1;
            Resources.UnloadUnusedAssets();
            GC.Collect();
            if (_isReset)
                Reset();
            
            _loadDataLength--;
            if (_loadDataLength <= 0)
                GameBegan();
            EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_Load_ProcessChange, _loadDataLength));
            
            
        }
        private void GameBegan()
        {
            PlayerMgr.Instance.CreatePlayer();
            PlayerMgr.Instance.AddOffLineIncome();
            
            EventDispatcher.Instance.TriggerEvent(new BaseEventArgs(EnumEventType.Event_Load_Complete));
            InitGame();
        }
        private void InitGame()
        {
            StartGame(null);
            UIController.Instance.OpenPageFromAssets<IdleWindow>("IdleWindow.prefab");
            UIController.Instance.ShowBlocker("IdleTopUI.prefab", true);
            if (PlayerMgr.Instance.IsNewPlayer)
            {
                EventDispatcher.Instance.TriggerEvent(new EventArgsOne<int>(EnumEventType.Event_Guide_StartGuide, 1));
            }
            else
            {
                PlayerMgr.Instance.ResetLastGuide();
            }
            int leaveSecond = (int)PlayerMgr.Instance.GetLeaveCityTime();
            if (leaveSecond / 60 >= 5)
            {
                UIController.Instance.ShowBlocker("OfflineWindow.prefab", true);
            }
        }

        //开始游戏逻辑
        private void StartGame(BaseEventArgs baseEventArgs)
        {
            AudioCtrl.Instance.Dispose();
            _gameState = GameState.Start;
            //ColaFramework.GameManager.Instance.InitGameCore(gameObject);
            IdleCityCtrl.Instance.Init();
            IdleForkCtrl.Instance.Init();
            IdleCityStaffCtrl.Instance.Init();
            IdleSiteCtrl.Instance.Init();
            IdleTruckCtrl.Instance.Init();
            IdleCityTruckCtrl.Instance.Init();
            
            IdleTaskCtrl.Instance.Init();
            IdleGuideCtrl.Instance.Init();
            IdleEventCtrl.Instance.Init();
            IdleInvestorCtrl.Instance.Init();
            IdleDoubleIncomeCtrl.Instance.Init();
            IdleActionTipCtrl.Instance.Init();
            UIFxCtrl.Instance.Init();
            PlayerMgr.Instance.TAEventPropertie();
            AudioCtrl.Instance.PlayBackgroundMusic(GameAudio.bgm);
        }
        public GameState GameState { get { return _gameState; } }
        private void Update()
        {
            AssetLoader.Update(Time.deltaTime);
            AudioCtrl.Instance.Update(Time.deltaTime);
            ColaFramework.AutoResGCMgr.Instance.Update(Time.deltaTime);
            ColaFramework.GameManager.Instance.Update(Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
        public void NextScene(int cityId)
        {
            Time.timeScale = 0;
            PlayerMgr.Instance.RecordPlayer();
            _gameState = GameState.None;
            PlayerMgr.Instance.CityId = cityId;
            _isReset = true;
            _loadDataLength = 2;
            IdleSiteMgr.Instance.LoadAssistFile();
            StartCoroutine(AssetLoadScene());
            
        }
        private bool _isReset = false;
        void Reset()
        {
            _isReset = false;
            Timer.Instance.UnRegister();
            IdleCityCtrl.Instance.Dispose();
            IdleForkCtrl.Instance.Dispose();
            IdleCityStaffCtrl.Instance.Dispose();
            IdleSiteCtrl.Instance.Dispose();
            IdleTruckCtrl.Instance.Dispose();
            IdleCityTruckCtrl.Instance.Dispose();
            IdleTaskCtrl.Instance.Dispose();
            IdleNpcTalkCtrl.Instance.Dispose();
            IdleGuideCtrl.Instance.Dispose();
            IdleEventCtrl.Instance.Dispose();
            IdleInvestorCtrl.Instance.Dispose(); 
            
        }
        private void OnApplicationFocus(bool focus)
        {
            Debug.Log($"OnApplicationFocus:{focus}");
        }

        private void OnApplicationPause(bool pause)
        {
            //ture 为切换到后台
            Debug.Log($"OnApplicationPause:{pause}");
            if (_gameState != GameState.Start) return;
            if (pause )
                PlayerMgr.Instance.RecordPlayer();
            else
            {
                if (!PlatformFactory.Instance.IsPlaying || !IdleGuideCtrl.Instance.IsGuiding)
                {
                    Time.timeScale = 1.0f;
                    AudioCtrl.Instance.UnPauseBackgroundMusic(false);
                    EventDispatcher.Instance.TriggerEvent(new EventArgsOne<bool>(EnumEventType.Event_IdleWindow_ShowMenuBtns, true));
                }
                
            }
            PlatformFactory.Instance.OnApplicationPause(pause);
        }

        private void OnApplicationQuit()
        {
            Debug.Log($"OnApplicationQuit:");
            PlayerMgr.Instance.RecordPlayer();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Load_IdleData, LoadTrackData);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Load_PlayerData, LoadTrackData);
            EventDispatcher.Instance.RemoveListener(EnumEventType.Event_Game_Began, StartGame);
        }


    }
}


