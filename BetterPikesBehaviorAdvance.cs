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
        protected static void Postfix1()
        {
            _hasFormedUp = false;
            _formUpTimer = new Timer(Mission.Current.CurrentTime, 20f, false);
        }

        [HarmonyPostfix]
        [HarmonyPatch("TickOccasionally")]
        public static void Postfix2(BehaviorAdvance __instance)
        {
            Formation formation = __instance.Formation;

            if (BetterPikesHelper.IsPikeFormation(formation))
            {
                bool isEnemyNearby = formation.CachedClosestEnemyFormation != null && formation.CachedAveragePosition.Distance(formation.CachedClosestEnemyFormation.Formation.CachedAveragePosition) <= 100;
                Vec2 orderPosition = formation.OrderPosition;

                // If the percentage of pikemen is above a certain limit, make the formation form a deep shield wall.
                formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
                formation.SetFormOrder(FormOrder.FormOrderDeep);

                if ((formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents < 1 && _formUpTimer.ElapsedTime() >= 1f) || _formUpTimer.Check(Mission.Current.CurrentTime))
                {
                    _hasFormedUp = true;
                }

                formation.ApplyActionOnEachUnit(delegate (Agent agent)
                {
                    Vec2 currentGlobalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);

                    if (!isEnemyNearby && agent.CanMoveDirectlyToPosition(orderPosition) && agent.CanMoveDirectlyToPosition(currentGlobalPositionOfUnit) && (!_hasFormedUp || agent.Position.AsVec2.Distance(currentGlobalPositionOfUnit) >= 2))
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
