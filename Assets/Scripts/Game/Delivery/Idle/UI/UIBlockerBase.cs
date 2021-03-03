using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using libx;

namespace UIFramework
{
    public class UIBlockerBase : MonoBehaviour
    {
        private AssetRequest request;
        public virtual void Init(AssetRequest _request)
        {
            request = _request;
        }

        private void OnDestroy()
        {
            if (request != null)
            {
                request.Release();
            }
        }

    }
}


