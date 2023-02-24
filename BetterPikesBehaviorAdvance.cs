using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(BehaviorAdvance), "TickOccasionally")]
    public class BetterPikesBehaviorAdvance
    {
        // Make a formation into a deep shield wall if the percentage of pikemen is above a certain limit.
        public static void Postfix(BehaviorAdvance __instance)
        {
            if (__instance.Formation.CountOfUnits > 1 && __instance.Formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem?.WeaponLength >= 400) >= __instance.Formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation)
            {
                __instance.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                __instance.Formation.FormOrder = FormOrder.FormOrderDeep;
            }
        }
    }
}
