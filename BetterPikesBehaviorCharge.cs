using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesBehaviorCharge
    {
        // Make a formation into a deep shield wall if the percentage of pikemen is above a certain limit.
        protected static void Postfix(BehaviorCharge __instance, ref MovementOrder ____currentOrder)
        {
            if (__instance.Formation.CountOfUnits > 1 && __instance.Formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem?.WeaponLength >= 400) >= __instance.Formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation)
            {
                FormationQuerySystem closestEnemyFormation = __instance.Formation.QuerySystem.ClosestEnemyFormation;
                __instance.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                __instance.Formation.FormOrder = FormOrder.FormOrderDeep;
                if (closestEnemyFormation != null && !closestEnemyFormation.IsCavalryFormation)
                {
                    ____currentOrder = MovementOrder.MovementOrderAdvance;
                }
            }
        }
    }
}
