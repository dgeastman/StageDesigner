using System;
using UnityEngine;

namespace StageDesigner
{
    public class Events
    {
        public static event Action<bool> PluginEnabled;
        public static event Action<bool> PluginDisabled;
        public static event Action<bool, bool> PluginToggled;
        public static event Action ConfigSaving;
        public static event Action LeavingEditor;
        public static event Action PartChanged;
        public static event Action RootPartPicked;
        public static event Action RootPartDropped;
        public static event Action<EditorScreen> EditorScreenChanged;

        public static void OnPluginEnabled(bool byUser)
        {
            PluginEnabled?.Invoke(byUser);
        }

        public static void OnPluginDisabled(bool byUser)
        {
            PluginDisabled?.Invoke(byUser);
        }

        public static void OnPluginToggled(bool value, bool byUser)
        {
            PluginToggled?.Invoke(value, byUser);
        }

        public static void OnLeavingEditor()
        {
            LeavingEditor?.Invoke();
        }

        public static void OnPartChanged()
        {
            PartChanged?.Invoke();
        }

        public static void OnRootPartPicked()
        {
            RootPartPicked?.Invoke();
        }

        public static void OnRootPartDropped()
        {
            RootPartDropped?.Invoke();
        }

        public static void OnEditorScreenChanged(EditorScreen screen)
        {
            EditorScreenChanged?.Invoke(screen);
        }

        public void HookEvents()
        {
            /* don't add static methods, GameEvents doesn't like that. */
            GameEvents.onGameSceneLoadRequested.Add(onGameSceneChange);
            GameEvents.onEditorPartEvent.Add(onEditorPartEvent);
            GameEvents.onEditorRestart.Add(onEditorRestart);
            GameEvents.onEditorScreenChange.Add(onEditorScreenChange);
        }

        public void UnhookEvents()
        {
            GameEvents.onGameSceneLoadRequested.Remove(onGameSceneChange);
            GameEvents.onEditorPartEvent.Remove(onEditorPartEvent);
            GameEvents.onEditorRestart.Remove(onEditorRestart);
            GameEvents.onEditorScreenChange.Remove(onEditorScreenChange);
        }

        void onGameSceneChange(GameScenes scene)
        {
            OnLeavingEditor();
            ConfigSaving?.Invoke();
        }

        void onEditorRestart()
        {
            StageDesigner.SetActive(false);
        }

        void onEditorScreenChange(EditorScreen screen)
        {
            OnEditorScreenChanged(screen);
        }

        void onEditorPartEvent(ConstructionEventType evt, Part part)
        {
            //MonoBehaviour.print (evt.ToString ());
            OnPartChanged();
            switch (evt)
            {
                case ConstructionEventType.PartPicked:
                    if (part == EditorLogic.RootPart)
                    {
                        OnRootPartPicked();
                    }
                    break;
                case ConstructionEventType.PartDropped:
                    if (part == EditorLogic.RootPart)
                    {
                        OnRootPartDropped();
                    }
                    break;
                case ConstructionEventType.PartDeleted:
                    if (part == EditorLogic.RootPart)
                    {
                        StageDesigner.SetActive(false);
                    }
                    break;
            }
        }
    }
}

