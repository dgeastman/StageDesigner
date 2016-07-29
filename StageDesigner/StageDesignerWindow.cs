using KSP.UI.Screens;
using KSPPluginFramework;
using RealFuels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StageDesigner
{
    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class StageDesignerWindow : MonoBehaviourWindowPlus
    {
        private ApplicationLauncherButton appLauncherButton;
        private List<EngineData> engineStats;
        private static Vector2 _scrollViewerPosition = Vector2.zero;

        internal override void Awake()
        {
            WindowCaption = "Stage Designer";
            WindowRect = new Rect((Screen.width/2)-450, Screen.height*0.25f, 900, Screen.height*0.5f);
            base.Awake();
        }

        internal override void Start()
        {
            Visible = false;
            DragEnabled = true;
            ClampToScreen = true;
            TooltipsEnabled = true;
            TooltipMouseOffset = new Vector2d(10, -10);

            StartCoroutine("AddToToolbar");
            base.Start();
        }

        internal override void OnGUIOnceOnly()
        {
            Styles.SetupGuiStyles();
            base.OnGUIOnceOnly();
        }

        internal override void DrawWindow(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Payload", "The mass of the vessel above the stage being designed, in metric tons."), GUILayout.Width(100));
            string payloadMass = GUILayout.TextField("1.0", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            // table header
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Engine", "Source part"), GUILayout.Width(200));
            GUILayout.Label(new GUIContent("Variant", "Alternate version"), GUILayout.Width(150));
            GUILayout.Label(new GUIContent("Burn Time", "How long the engine will thrust"), GUILayout.Width(100));
            GUILayout.Label(new GUIContent("dV", "Meters per second of change in velocity of this configuration"), GUILayout.Width(100));
            GUILayout.Label(new GUIContent("MinTWR", "Starting Thrust to Weight Ratio of this configuration"), GUILayout.Width(100));
            GUILayout.Label(new GUIContent("MaxTWR", "Ending Thrust to Weight Ratio of this configuration"), GUILayout.Width(100));
            GUILayout.Label(new GUIContent("Wet Mass", "Mass of the configuration when loaded with fuel"), GUILayout.Width(100));
            GUILayout.Label(new GUIContent("Dry Mass", "Mass of the configuration only counting engine and tanks"), GUILayout.Width(100));
            GUILayout.EndHorizontal();

            // table rows
            _scrollViewerPosition = GUILayout.BeginScrollView(_scrollViewerPosition, Styles.ScrollStyle);
            foreach (var engine in engineStats)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent(engine.Name, engine.PartName), GUILayout.Width(200));
                GUILayout.Label(engine.Title, GUILayout.Width(150));
                GUILayout.Label(engine.BurnTime.ToString(), GUILayout.Width(150));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

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
            InitializeEngineStats();
        }

        void CloseWindow()
        {
            Visible = false;
        }

        void InitializeEngineStats()
        {
            engineStats = new List<EngineData>();
            foreach (AvailablePart part in PartLoader.LoadedPartsList.Where(p => ResearchAndDevelopment.PartTechAvailable(p)))
            {
                List<ModuleEngineConfigs> allConfigs = new List<ModuleEngineConfigs>();
                allConfigs.AddRange(part.partPrefab.Modules.GetModules<ModuleEngineConfigs>());
                if (allConfigs.Count <= 0)
                    continue;

                foreach (var engineConfig in allConfigs)
                {
                    foreach (var altConfig in engineConfig.configs)
                    {
                        string burnTime = "-";
                        if (altConfig.HasValue("description"))
                        {
                            var description = altConfig.GetValue("description");
                            var burnTimeIndex = description.LastIndexOf("Rated Burn Time ");
                            if (burnTimeIndex >= 0)
                            {
                                burnTime = description.Substring(burnTimeIndex + 16);
                            }
                        }
                        var engineStat = new EngineData { Name = part.title, Title = altConfig.GetValue("name"), PartName = engineConfig.name, BurnTime = burnTime };
                        engineStats.Add(engineStat);
                    }
                }
            }
        }
    }
}
