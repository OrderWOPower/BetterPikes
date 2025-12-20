using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    [HarmonyPatch(typeof(BehaviorAdvance))]
    public class BetterPikesBehaviorAdvance
    {
        private static bool _hasFormedUp;
        private static Timer _formUpTimer;

        [HarmonyPostfix]
        [HarmonyPatch("OnBehaviorActivatedAux")]
        protected static void Postfix1(BehaviorAdvance __instance)
        {
            _hasFormedUp = false;
            _formUpTimer = new Timer(Mission.Current.CurrentTime, MathF.Log10(__instance.Formation.CountOfUnits) * 5f, false);
        }

        [HarmonyPostfix]
        [HarmonyPatch("TickOccasionally")]
        public static void Postfix2(BehaviorAdvance __instance)
        {
            Formation formation = __instance.Formation;

            if (formation.GetCountOfUnitsWithCondition(agent => agent.IsActive() && BetterPikesHelper.IsPike(agent.WieldedWeapon)) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation)
            {
                // If the percentage of pikemen is above a certain limit, make the formation form a deep shield wall.
                formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
                formation.SetFormOrder(FormOrder.FormOrderDeep);

                if (formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents < 0.5f || _formUpTimer.Check(Mission.Current.CurrentTime))
                {
                    _hasFormedUp = true;
                }

                if (formation.CachedAveragePosition.IsValid)
                {
                    bool isEnemyNearby = formation.CachedClosestEnemyFormation != null && formation.CachedAveragePosition.Distance(formation.CachedClosestEnemyFormation.Formation.CachedAveragePosition) <= 100;

                    formation.ApplyActionOnEachUnit(delegate (Agent agent)
                    {
                        Vec2 currentGlobalPositionOfUnit = !isEnemyNearby ? formation.GetCurrentGlobalPositionOfUnit(agent, true) : Vec2.Invalid;

                        if (!isEnemyNearby && (!_hasFormedUp || agent.Position.AsVec2.Distance(currentGlobalPositionOfUnit) >= 2))
                        {
                            agent.SetTargetPosition(currentGlobalPositionOfUnit);
                        }
                        else
                        {
                            agent.ClearTargetFrame();
                        }
                    });
                }
            }
        }
    }
}
