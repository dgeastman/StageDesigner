using KSP.UI.Screens;
using KSPPluginFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

            // get all the parts that are available based on current R&D, whether they've been unlocked or not
            foreach (AvailablePart part in PartLoader.LoadedPartsList.Where(p => ResearchAndDevelopment.PartTechAvailable(p)))
            {
                if (!part.partPrefab.Modules.Contains("ModuleEngineConfigs"))
                { 
                    // we're only interested in the parts that have a module of type RealFuels.ModuleEngineConfig
                    continue;
                }

                // get a dictionary of configname-burn time by looking for TestFlight.TestFlightReliability_EngineCycle modules
                var burnTimes = GetTestFlightBurnTimesForEngine(part);

                // via reflection, get the list of engine config variations
                var engineConfig = part.partPrefab.Modules["ModuleEngineConfigs"];
                System.Type engineConfigType = engineConfig.GetType();
                var piConfigsList = engineConfigType.GetField("configs");
                if (piConfigsList == null)
                {
                    LogFormatted_DebugOnly("Unable to get ModuleEngineConfigs.configs for part {0}:{1}", part.name, part.title);
                    continue;
                }

                // this field will tell us if it's RCS or not
                var engineType = engineConfigType.GetField("type").GetValue(engineConfig) as string;

                // now iterate through the variations
                IEnumerable configsList = piConfigsList.GetValue(engineConfig) as IEnumerable;
                foreach (var altConfig in configsList)
                {
                    string name = (string)altConfig.GetType().InvokeMember("GetValue", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, altConfig, new object[] { "name" });

                    // see if we have a burn time for this config by name
                    float burnTime = 0;
                    if (burnTimes.ContainsKey(name))
                    {
                        burnTime = burnTimes[name];
                    }

                    // add the completed engine data to our master list
                    var engineStat = new EngineData { Name = part.title, Title = name, PartName = engineConfig.name, BurnTime = burnTime, isRcs = (engineType == "ModuleRCS") };
                    engineStats.Add(engineStat);
                }
            }
            LogFormatted(BuildEngineListing());
        }

        internal Dictionary<string, float> GetTestFlightBurnTimesForEngine(AvailablePart engine)
        {
            var result = new Dictionary<string, float>();

            foreach (var module in engine.partPrefab.Modules)
            {
                if (module.GetType().Name == "TestFlightReliability_EngineCycle")
                {
                    System.Type engineCycleType = module.GetType();
                    var config = module.GetType().GetField("engineConfig").GetValue(module) as string;
                    var burnTime = (float)module.GetType().GetField("ratedBurnTime").GetValue(module);
                    result.Add(config, burnTime);
                }
            }

            return result;
        }

        internal string BuildEngineListing()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PartName,Title,Config,BurnTime,hasBurnTime,isRcs");
            foreach (var engine in engineStats)
            {
                sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}", engine.PartName, engine.Name.Replace(',',';'), engine.Title, engine.BurnTime, (engine.BurnTime > 0), engine.isRcs));
            }
            return sb.ToString();
        }
    }
}
