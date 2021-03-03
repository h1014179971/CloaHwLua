using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foundation;
using UnityEngine.Networking;

namespace Delivery
{
    public class SceneCtrl : MonoSingleton<SceneCtrl>
    {
        AsyncOperation _async;
        private void Update()
        {
            if (_async != null)
            {
                //LogUtility.LogInfo($"场景加载进度 _async==" + _async.progress);
                if(_async.progress >=1 && _async.isDone)
                {
                    _async = null;
                }
            }
        }
        public AsyncOperation LoadScene(string sceneName)
        {                       
            _async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName); 
            return _async;
        }
        public void LoadScene(int sceneId)
        {
            _async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneId); 
        }
        public void LoadSceneByPath(string path,string sceneName)
        {
            StartCoroutine(LoadSceneName(path, sceneName));
        }
        IEnumerator LoadSceneName(string path, string sceneName)
        {
            UnityWebRequest request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError(request.error);     
            }
            else
            {
                var bundle = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
                _async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
                bundle.Unload(false);
            }
            
        }
    }
}

