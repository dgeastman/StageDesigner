using UnityEngine;
using KSP;
using System.Collections.Generic;
using System.Linq;
using KSPPluginFramework;
using KSP.UI.Screens;
using System.Collections;

namespace StageDesigner
{
    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class StageDesignerWindow : MonoBehaviourWindow
    {
        private ApplicationLauncherButton appLauncherButton;

        internal override void Awake()
        {
            WindowCaption = "Stage Designer";
            WindowRect = new Rect(0, 0, 250, 50);
            base.Awake();
        }

        internal override void Start()
        {
            Visible = false;
            StartCoroutine("AddToToolbar");
            base.Start();
        }

        internal override void DrawWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("Window Contents", "Here is a reallly long tooltip to demonstrate the war and peace model of writing too much text in a tooltip\r\n\r\nIt even includes a couple of carriage returns to make stuff fun"));
            GUILayout.EndVertical();
        }

        IEnumerator AddToToolbar()
        {
            while (!ApplicationLauncher.Ready)
            {
                yield return null;
            }

            // Load the icon for the button
            Texture iconTexture = GameDatabase.Instance.GetTexture("StageDesigner/Textures/AppLauncherIcon", false);
            if (iconTexture == null)
            {
                yield return null;
            }
            appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                OpenWindow,
                CloseWindow,
                null,
                null,
                null,
                null,
                ApplicationLauncher.AppScenes.VAB,
                iconTexture);
        }

        void OpenWindow()
        {
            Visible = true;
        }

        void CloseWindow()
        {
            Visible = false;
        }

    }
}
