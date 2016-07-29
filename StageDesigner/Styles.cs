using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StageDesigner
{
    internal static class Styles
    {
        internal static GUIStyle WindowStyle;
        internal static GUIStyle ScrollStyle;

        internal static void SetupGuiStyles()
        {
            if (WindowStyle != null) return;
            SetStyles();
        }

        internal static void SetStyles()
        {
            WindowStyle = new GUIStyle(GUI.skin.window);
            ScrollStyle = new GUIStyle(GUI.skin.box);
        }
    }
}
