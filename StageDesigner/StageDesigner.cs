using UnityEngine;
using KSP;
using System.Collections.Generic;
using System.Linq;

namespace StageDesigner
{
    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class StageDesigner : MonoBehaviour
    {
        public static StageDesigner instance { get; private set; }
        static bool userEnable; // master enable/disable switch for the user
        bool softEnable = true; // hide the plugin even if enabled under certain circumstances
        static Events events;

        private List<EngineData> engineData;

        public StageDesigner()
        {
            instance = this;
            new AppLauncher();
        }

        void Awake()
        {
            events = new Events();
            events.HookEvents();

            InitializeEngineData();

            gameObject.AddComponent<MainWindow>();

            Events.EditorScreenChanged += onEditorScreenChanged;
        }

        void OnDestroy()
        {
            events.UnhookEvents();
            Events.EditorScreenChanged -= onEditorScreenChanged;
        }

        public static bool Enabled
        {
            get
            {
                if (EditorLogic.fetch == null)
                {
                    return false;
                }

                switch (HighLogic.LoadedScene)
                {
                    case GameScenes.EDITOR:
                        break;
                    default:
                        /* disable during scene changes */
                        return false;
                }

                return userEnable && (instance != null && instance.softEnable);
            }
        }
        void onEditorScreenChanged(EditorScreen screen)
        {
            if (EditorScreen.Parts == screen)
            {
                setSoftActive(true);
            }
            else if (EditorScreen.Actions == screen)
            {
                // hide the plugin if we're editing action groups
                setSoftActive(false);
            }
        }
        public static void SetActive(bool value)
        {
            userEnable = value;

            if (value)
            {
                Events.OnPluginEnabled(true);
            }
            else
            {
                Events.OnPluginDisabled(true);
            }
            Events.OnPluginToggled(value, true);
        }

        void setSoftActive(bool value)
        {
            // disable the plugin for a specific screen, the plugin is still technically active and will re-appear when screen changes
            softEnable = value;
            bool pluginEnabled = Enabled;
            if (pluginEnabled)
            {
                Events.OnPluginEnabled(false);
            }
            else
            {
                Events.OnPluginDisabled(false);
            }
            Events.OnPluginToggled(value, false);
        }

        private void InitializeEngineData()
        {
            engineData = new List<EngineData>();
            foreach (AvailablePart part in PartLoader.LoadedPartsList.Where(p => ResearchAndDevelopment.PartTechAvailable(p)))
            { }
        }
    }
}
