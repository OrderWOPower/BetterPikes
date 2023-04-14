using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(BehaviorAdvance), "TickOccasionally")]
    public class BetterPikesBehaviorAdvance
    {
        // If the percentage of pikemen is above a certain limit, make the formation into a deep shield wall.
        public static void Postfix(BehaviorAdvance __instance)
        {
            Formation formation = __instance.Formation;
            bool isPikeFormation = formation.CountOfUnits > 1 && formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem?.WeaponLength >= 400) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation;
            if (isPikeFormation)
            {
                formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                formation.FormOrder = FormOrder.FormOrderDeep;
            }
        }
    }
}
