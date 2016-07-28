using KSP.UI.Screens;

namespace StageDesigner
{
    public class AppLauncher
    {
        public static AppLauncher instance;

        static ApplicationLauncherButton button;

        const string iconPath = "StageDesigner/Textures/AppLauncherIcon";
        const ApplicationLauncher.AppScenes visibleScenes = ApplicationLauncher.AppScenes.VAB;

        public AppLauncher()
        {
            if (instance == null)
            {
                instance = this;

                addButton();
            }
        }

        public void addButton()
        {
            if (ApplicationLauncher.Ready)
            {
                _addButton();
            }
            else
            {
                GameEvents.onGUIApplicationLauncherReady.Add(onAppLauncherReadyAddButton);
            }
        }

        public void removeButton()
        {
            if (ApplicationLauncher.Ready)
            {
                _removeButton();
            }
            else
            {
                GameEvents.onGUIApplicationLauncherReady.Add(onAppLauncherReadyRemoveButton);
            }
        }

        void onAppLauncherReadyAddButton()
        {
            _addButton();
            GameEvents.onGUIApplicationLauncherReady.Remove(onAppLauncherReadyAddButton);
        }

        void onAppLauncherReadyRemoveButton()
        {
            _removeButton();
            GameEvents.onGUIApplicationLauncherReady.Remove(onAppLauncherReadyRemoveButton);
        }

        void _addButton()
        {
            if (button != null)
            {
                return;
            }
            button = ApplicationLauncher.Instance.AddModApplication(onTrue, onFalse, null, null,
                null, null, visibleScenes, GameDatabase.Instance.GetTexture(iconPath, false));
            if (StageDesigner.Enabled)
            {
                /* this doesn't seem to work */
                button.SetTrue(false);
                //button.toggleButton.star = true; FIXME?
            }
            Events.PluginEnabled += onPluginEnable;
            Events.PluginDisabled += onPluginDisable;
        }

        void _removeButton()
        {
            if (button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(button);
                button = null;
                Events.PluginEnabled -= onPluginEnable;
                Events.PluginDisabled -= onPluginDisable;
            }
        }

        void onTrue()
        {
            StageDesigner.SetActive(true);
        }

        void onFalse()
        {
            StageDesigner.SetActive(false);
        }

        void onPluginEnable(bool byUser)
        {
            if (byUser)
            {
                button.SetTrue(false);
            }
        }

        void onPluginDisable(bool byUser)
        {
            if (byUser)
            {
                button.SetFalse(false);
            }
        }
    }
}

