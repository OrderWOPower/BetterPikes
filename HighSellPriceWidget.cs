using HarmonyLib;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Nameplate;

namespace HighSellPrice
{
    [HarmonyPatch(typeof(SettlementNameplateEventVisualBrushWidget), "UpdateVisual")]
    public class HighSellPriceWidget
    {
        private static void Prefix(SettlementNameplateEventVisualBrushWidget __instance, int type)
        {
            if (type == HighSellPriceVM.NumOfEventTypes)
            {
                // Set the icon widget to a red coin.
                __instance.SetState("HighSellingPrice");

                return;
            }
        }
    }
}
