using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        private readonly ActionIndexCache _readyThrustActionIndex, _guardUpActionIndex;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public BetterPikesMissionBehavior()
        {
            _readyThrustActionIndex = ActionIndexCache.Create("act_ready_thrust_pike");
            _guardUpActionIndex = ActionIndexCache.Create("act_guard_up_pike");
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            if (agent.IsHuman)
            {
                agent.AddComponent(new BetterPikesAgentComponent(agent));
                agent.SetHasOnAiInputSetCallback(true);
            }
        }

        public override void OnAgentPanicked(Agent affectedAgent)
        {
            if (affectedAgent.IsHuman)
            {
                affectedAgent.SetScriptedFlags(affectedAgent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);

                if (affectedAgent.GetCurrentAction(1) == _readyThrustActionIndex || affectedAgent.GetCurrentAction(1) == _guardUpActionIndex)
                {
                    affectedAgent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true, blendInPeriod: 0.5f);
                }
            }
        }

        public override void OnMissionTick(float dt)
        {
            foreach (Formation formation in Mission.Teams.SelectMany(team => team.FormationsIncludingSpecialAndEmpty.Where(f => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation)))
            {
                bool isPikeFormation = formation.GetCountOfUnitsWithCondition(a => a.IsActive() && BetterPikesHelper.IsPike(a.WieldedWeapon)) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation && formation.FiringOrder != FiringOrder.FiringOrderHoldYourFire;
                bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty() && formation.CachedClosestEnemyFormation != null;
                Vec2 positionOfClosestEnemyFormation = hasEnemy ? formation.CachedClosestEnemyFormation.Formation.CachedAveragePosition : Vec2.Invalid;
                bool isEnemyNearby = hasEnemy && formation.CachedAveragePosition.Distance(positionOfClosestEnemyFormation) <= BetterPikesSettings.Instance.MinDistanceToReadyPikes, isLoose = formation.IsLoose, isInCircleArrangement = formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle;

                foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes().Where(a => a.IsHuman && a.IsActive()))
                {
                    agent.SetScriptedFlags(agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);

                    if (isPikeFormation)
                    {
                        float distanceFromCurrentGlobalPosition = agent.Position.AsVec2.Distance(formation.GetCurrentGlobalPositionOfUnit(agent, true)), distanceFromClosestEnemyFormation = agent.Position.AsVec2.Distance(positionOfClosestEnemyFormation);

                        agent.SetMaximumSpeedLimit(agent.GetMaximumForwardUnlimitedSpeed(), false);

                        if (hasEnemy && !isLoose && (distanceFromCurrentGlobalPosition < 2 || distanceFromClosestEnemyFormation <= 100))
                        {
                            // If the pikemen have enemies, make the pikemen walk.
                            agent.SetScriptedFlags(agent.GetScriptedFlags() | Agent.AIScriptedFrameFlags.DoNotRun);
                        }

                        if (MBRandom.RandomFloat < 0.2f)
                        {
                            ActionIndexCache currentAction = agent.GetCurrentAction(1);

                            if ((isEnemyNearby || isInCircleArrangement) && BetterPikesHelper.IsPike(agent.WieldedWeapon) && agent.IsAIControlled && !agent.IsDoingPassiveAttack && distanceFromCurrentGlobalPosition < 2)
                            {
                                if (currentAction != _readyThrustActionIndex && currentAction != _guardUpActionIndex)
                                {
                                    agent.GetFormationFileAndRankInfo(out _, out int rankIndex);

                                    // If the pikemen's enemies are nearby, make the pikemen ready their pikes in different positions.
                                    if (rankIndex < 5)
                                    {
                                        // Make the first five ranks ready their pikes for a thrust.
                                        agent.SetActionChannel(1, _readyThrustActionIndex, startProgress: MBRandom.RandomFloat);
                                    }
                                    else
                                    {
                                        // Make the sixth rank onwards ready their pikes at an angle.
                                        agent.SetActionChannel(1, _guardUpActionIndex, startProgress: MBRandom.RandomFloat);
                                    }
                                }
                            }
                            else
                            {
                                if (currentAction == _readyThrustActionIndex || currentAction == _guardUpActionIndex)
                                {
                                    agent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true, blendInPeriod: 0.5f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
