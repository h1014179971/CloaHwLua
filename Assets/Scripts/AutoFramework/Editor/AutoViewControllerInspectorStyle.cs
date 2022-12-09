using System;
using UnityEngine;

namespace AutoCode
{
    public class AutoViewControllerInspectorStyle
    {
        public readonly Lazy<GUIStyle> BigTitleStyle = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 15
        });
    }
}

