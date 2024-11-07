using EquinoxsModUtils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EasyThreshing.Patches
{
    internal class ThresherInstancePatch
    {
        [HarmonyPatch(typeof(ThresherInstance), nameof(ThresherInstance.UpdateCrafting))]
        [HarmonyPrefix]
        static void ClearOutputsForCrafting(ThresherInstance __instance) {
            if (!EasyThreshingPlugin.voidExcessOutput.Value) return;

            Inventory inputInventory = __instance.GetInputInventory();
            if (inputInventory.myStacks[0].isEmpty) return;

            Inventory outputInventory = __instance.GetOutputInventory();
            SchematicsRecipeData recipe = EMU.Recipes.TryFindThresherRecipe(inputInventory.myStacks[0].id);

            if (IsSlotFull(__instance, 0) && IsSlotFull(__instance, 1) &&
                EasyThreshingPlugin.pauseIfBothFull.Value) return;

            for (int i = 0; i < outputInventory.myStacks.Count(); i++) {
                if (IsSlotFull(__instance, i)) {
                    outputInventory.RemoveResourcesFromSlot(i, recipe.outputQuantities[i]);
                }
            }
        }

        private static bool IsSlotFull(ThresherInstance __instance, int index) {
            Inventory inputInventory = __instance.GetInputInventory();
            Inventory outputInventory = __instance.GetOutputInventory();
            SchematicsRecipeData recipe = EMU.Recipes.TryFindThresherRecipe(inputInventory.myStacks[0].id);

            if (outputInventory.myStacks[index].isEmpty) return false;

            int toCraft = recipe.outputQuantities[index];
            int threshold = outputInventory.myStacks[index].maxStack - toCraft;
            return outputInventory.myStacks[index].count >= threshold;
        }
    }
}
