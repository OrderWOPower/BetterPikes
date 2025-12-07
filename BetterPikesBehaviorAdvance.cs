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

            if (formation.GetCountOfUnitsWithCondition(agent => agent.GetPrimaryWieldedItemIndex() != EquipmentIndex.None && BetterPikesHelper.IsPike(agent.WieldedWeapon)) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation)
            {
                // If the percentage of pikemen is above a certain limit, make the formation form a deep shield wall.
                formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
                formation.SetFormOrder(FormOrder.FormOrderDeep);

                formation.ApplyActionOnEachAttachedUnit(delegate (Agent agent)
                {
                    if (agent.IsAIControlled)
                    {
                        if ((!_arrangementTimer.Check(Mission.Current.CurrentTime) || formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents > formation.CachedFormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents) && formation.CachedClosestEnemyFormation != null && formation.CachedAveragePosition.Distance(formation.CachedClosestEnemyFormation.Formation.CachedAveragePosition) > 100)
                        {
                            agent.SetTargetPosition(formation.GetCurrentGlobalPositionOfUnit(agent, true));
                        }
                        else
                        {
                            agent.ClearTargetFrame();
                        }
                    }
                });
            }
        }
    }
}
