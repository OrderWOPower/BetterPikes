using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(BehaviorAdvance))]
    public class BetterPikesBehaviorAdvance
    {
        private static Timer _arrangementTimer;

        [HarmonyPostfix]
        [HarmonyPatch("OnBehaviorActivatedAux")]
        protected static void Postfix1(BehaviorAdvance __instance) => _arrangementTimer = new Timer(Mission.Current.CurrentTime, MathF.Log10(__instance.Formation.CountOfUnits) * 10f, false);

        [HarmonyPostfix]
        [HarmonyPatch("TickOccasionally")]
        public static void Postfix2(BehaviorAdvance __instance)
        {
            Formation formation = __instance.Formation;

            if (formation.GetCountOfUnitsWithCondition(agent => agent.WieldedWeapon.CurrentUsageItem?.WeaponDescriptionId != null && agent.WieldedWeapon.CurrentUsageItem.WeaponDescriptionId.Contains("Pike")) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation)
            {
                FormationQuerySystem querySystem = formation.QuerySystem;

                // If the percentage of pikemen is above a certain limit, make the formation form a deep shield wall.
                formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
                formation.FormOrder = FormOrder.FormOrderDeep;

                if ((!_arrangementTimer.Check(Mission.Current.CurrentTime) || querySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents > querySystem.FormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents) && querySystem.ClosestEnemyFormation != null && querySystem.AveragePosition.Distance(querySystem.ClosestEnemyFormation.AveragePosition) > 100)
                {
                    formation.ApplyActionOnEachUnit(delegate (Agent agent)
                    {
                        if (agent.IsAIControlled)
                        {
                            agent.SetTargetPosition(formation.GetCurrentGlobalPositionOfUnit(agent, true));
                        }
                    });
                }
                else
                {
                    formation.ApplyActionOnEachUnit(delegate (Agent agent)
                    {
                        if (agent.IsAIControlled)
                        {
                            agent.ClearTargetFrame();
                        }
                    });
                }
            }
        }
    }
}
