using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(BehaviorCharge), "TickOccasionally")]
    public class BetterPikesBehaviorCharge
    {
        public static void Postfix(BehaviorCharge __instance)
        {
            Formation formation = __instance.Formation;
            FormationQuerySystem closestEnemyFormation = formation.QuerySystem.ClosestEnemyFormation;

            if (formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem?.ItemUsage == "polearm_pike") >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation && closestEnemyFormation != null && !closestEnemyFormation.IsCavalryFormation && !closestEnemyFormation.IsRangedCavalryFormation)
            {
                // If the percentage of pikemen is above a certain limit, make the formation form a deep shield wall.
                formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                formation.FormOrder = FormOrder.FormOrderDeep;
                // If the formation's closest enemy formation is not cavalry, make the formation advance.
                AccessTools.PropertySetter(typeof(BehaviorComponent), "CurrentOrder").Invoke(__instance, new object[] { MovementOrder.MovementOrderAdvance });

                __instance.Formation.SetMovementOrder(__instance.CurrentOrder);
            }
        }
    }
}
