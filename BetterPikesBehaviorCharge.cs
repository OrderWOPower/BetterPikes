using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesBehaviorCharge
    {
        // If the percentage of pikemen is above a certain limit, make the formation into a deep shield wall.
        // If the pikemen's enemies are not routing, make the pikemen walk.
        // If the pikemen's closest enemy formation is not cavalry, make the pikemen advance.
        protected static void Postfix(BehaviorCharge __instance, ref MovementOrder ____currentOrder)
        {
            Formation formation = __instance.Formation;
            FormationQuerySystem closestEnemyFormation = formation.QuerySystem.ClosestEnemyFormation;
            if (formation.CountOfUnits > 1 && formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem?.WeaponLength >= 400) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation)
            {
                formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                formation.FormOrder = FormOrder.FormOrderDeep;
                foreach (Agent agent in formation.UnitsWithoutLooseDetachedOnes)
                {
                    Agent targetAgent = agent.GetTargetAgent();
                    agent.SetScriptedFlags(targetAgent != null && !targetAgent.IsRunningAway && !targetAgent.IsMainAgent ? agent.GetScriptedFlags() | Agent.AIScriptedFrameFlags.DoNotRun : agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
                }
                if (closestEnemyFormation != null && !closestEnemyFormation.IsCavalryFormation)
                {
                    ____currentOrder = MovementOrder.MovementOrderAdvance;
                }
            }
        }
    }
}
