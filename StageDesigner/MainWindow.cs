using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StageDesigner
{
    public class MainWindow : MonoBehaviour
    {
        int winID;
        Rect winRect;
        const string title = "Stage Designer";
        public const int main_window_width = 184;
        public const int main_window_height = 52;
        public GUIStyle style;

        void Awake()
        {
            winID = gameObject.GetInstanceID();
            winRect = new Rect(Settings.window_x, Settings.window_y, main_window_width, main_window_height);
            winRect.x = Mathf.Clamp(winRect.x, 0, Screen.width - main_window_width);
            winRect.y = Mathf.Clamp(winRect.y, 0, Screen.height - main_window_height);
            Events.ConfigSaving += save;

            style = new GUIStyle(GUI.skin.window);
            style.alignment = TextAnchor.UpperLeft;
            style.fixedWidth = main_window_width;
        }
        void OnDestroy()
        {
            Events.ConfigSaving -= save;
        }
        void save()
        {
            Settings.window_x = (int)winRect.x;
            Settings.window_y = (int)winRect.y;
        }
        void OnGUI()
        {
            winRect = GUILayout.Window(winID, winRect, drawWindow, title, style);
        }

        void drawWindow(int ID)
        {

        }
    }
}