using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delivery.Idle
{
    public class IdlePostSiteShadow : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            IdlePostSiteItemModel[] HideModel = transform.GetComponentsInChildren<IdlePostSiteItemModel>();
            for(int i=0;i<HideModel.Length;i++)
            {
                HideModel[i].gameObject.SetActive(false);
            }
        }

      
    }
}
