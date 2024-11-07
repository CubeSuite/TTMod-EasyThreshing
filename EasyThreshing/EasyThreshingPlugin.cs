using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EasyThreshing.Patches;
using EquinoxsModUtils;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace EasyThreshing
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class EasyThreshingPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.equinox.EasyThreshing";
        private const string PluginName = "EasyThreshing";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        // Config Entries

        public static ConfigEntry<bool> duplicateSeeds;
        public static ConfigEntry<bool> voidExcessOutput;
        public static ConfigEntry<bool> pauseIfBothFull;

        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            ApplyPatches();
            CreateConfigEntries();

            EMU.Events.GameDefinesLoaded += OnGameDefinesLoaded;

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        // Events

        private void OnGameDefinesLoaded() {
            if (!duplicateSeeds.Value) return;

            int kindlevineResID = EMU.Resources.GetResourceIDByName(EMU.Names.Resources.Kindlevine);
            int shiverthornResID = EMU.Resources.GetResourceIDByName(EMU.Names.Resources.Shiverthorn);

            SchematicsRecipeData kindleRecipe = EMU.Recipes.TryFindThresherRecipe(kindlevineResID);
            SchematicsRecipeData shiverRecipe = EMU.Recipes.TryFindThresherRecipe(shiverthornResID);

            kindleRecipe.outputQuantities[0] = 2;
            shiverRecipe.outputQuantities[0] = 2;
        }

        // Private Functions

        private void ApplyPatches() {
            Harmony.CreateAndPatchAll(typeof(ThresherInstancePatch));
        }

        private void CreateConfigEntries() {
            duplicateSeeds = Config.Bind("General", "Duplicate Seeds", true, new ConfigDescription("When true, each threshed plant will produce two seeds."));
            voidExcessOutput = Config.Bind("General", "Void Excess Output", true, new ConfigDescription("When true, if one thresher output fills up, items are voided from it instead of the thresher pausing."));
            pauseIfBothFull = Config.Bind("General", "Pause If Both Full", true, new ConfigDescription("When true, threshers will pause crafting when both outputs are full instead of voiding both."));
        }
    }
}
