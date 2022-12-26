using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoCode
{
    public class AutoViewController : MonoBehaviour
    {
        [HideInInspector] public string Namespace = string.Empty;

        [HideInInspector] public string ScriptName;

        [HideInInspector] public string ScriptsFolder = string.Empty;

        [HideInInspector] public bool GeneratePrefab = false;


        [HideInInspector] public string PrefabFolder = string.Empty;

        [HideInInspector] public Font TxtFont = null;

        public string TemplateName => nameof(AutoViewController);
    }
}

