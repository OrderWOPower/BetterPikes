using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesBehaviorCharge
    {
        protected static void Postfix(BehaviorCharge __instance, ref MovementOrder ____currentOrder)
        {
            Formation formation = __instance.Formation;
            FormationQuerySystem closestEnemyFormation = formation.QuerySystem.ClosestEnemyFormation;

            if (formation.CountOfUnits > 1 && formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem?.ItemUsage == "polearm_pike") >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation && closestEnemyFormation != null && !closestEnemyFormation.IsCavalryFormation && !closestEnemyFormation.IsRangedCavalryFormation)
            {
                // If the percentage of pikemen is above a certain limit, make the formation into a deep shield wall.
                formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                formation.FormOrder = FormOrder.FormOrderDeep;

                // If the pikemen's closest enemy formation is not cavalry, make the pikemen advance.
                ____currentOrder = MovementOrder.MovementOrderAdvance;
            }
        }
    }
}
