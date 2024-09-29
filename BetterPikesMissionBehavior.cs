using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        private readonly ActionIndexCache _readyThrustActionIndex, _guardUpActionIndex;
        private Timer _blockTimer;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public BetterPikesMissionBehavior()
        {
            _readyThrustActionIndex = ActionIndexCache.Create("act_ready_thrust_pike");
            _guardUpActionIndex = ActionIndexCache.Create("act_guard_up_pike");
        }

        public override void AfterStart() => _blockTimer = new Timer(Mission.CurrentTime, 1f, false);

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            BetterPikesSettings settings = BetterPikesSettings.Instance;

            // Determine which side to remove pikes from in siege battles.
            if (Mission.IsSiegeBattle && agent.IsHuman && ((agent.Team.IsAttacker && settings.SidesToRemovePikes.SelectedIndex == 1) || (agent.Team.IsDefender && settings.SidesToRemovePikes.SelectedIndex == 2) || settings.SidesToRemovePikes.SelectedIndex == 3))
            {
                for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.ExtraWeaponSlot; index++)
                {
                    if (IsPike(agent.Equipment[index]))
                    {
                        // Remove pikes in siege battles.
                        agent.RemoveEquippedWeapon(index);
                    }
                }
            }
        }

        public override void OnAgentPanicked(Agent affectedAgent)
        {
            if (affectedAgent.IsHuman)
            {
                affectedAgent.SetAgentFlags(affectedAgent.GetAgentFlags() | AgentFlag.CanDefend);
                affectedAgent.SetScriptedFlags(affectedAgent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
                affectedAgent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true, blendInPeriod: 0.5f);
            }
        }

        public override void OnMissionTick(float dt)
        {
            float currentTime = Mission.CurrentTime;
            BetterPikesSettings settings = BetterPikesSettings.Instance;

            if (_blockTimer.ElapsedTime() >= 1.001f)
            {
                _blockTimer.Reset(currentTime);
            }

            foreach (Formation formation in Mission.Teams.SelectMany(team => team.FormationsIncludingSpecialAndEmpty.Where(f => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation)))
            {
                FormationQuerySystem querySystem = formation.QuerySystem;
                bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty() && formation.GetCountOfUnitsWithCondition(a => IsPike(a.WieldedWeapon)) >= formation.CountOfUnits * settings.MinPikemenPercentInPikeFormation && formation.FiringOrder != FiringOrder.FiringOrderHoldYourFire;
                bool isEnemyNearby = hasEnemy && querySystem.AveragePosition.Distance(querySystem.ClosestEnemyFormation.AveragePosition) <= settings.MinDistanceToReadyPikes;
                bool isLoose = formation.IsLoose, isCircle = formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle;
                float averageMaxUnlimitedSpeed = querySystem.FormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents * 3f;

                foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes().Concat(formation.DetachedUnits).Where(a => a.IsHuman))
                {
                    float distanceFromCurrentGlobalPosition = agent.Position.AsVec2.Distance(formation.GetCurrentGlobalPositionOfUnit(agent, true));
                    ActionIndexCache currentAction = agent.GetCurrentAction(1);

                    if (hasEnemy && !isLoose && distanceFromCurrentGlobalPosition < averageMaxUnlimitedSpeed * 2f)
                    {
                        // If the pikemen have enemies, make the pikemen walk.
                        agent.SetScriptedFlags(agent.GetScriptedFlags() | Agent.AIScriptedFrameFlags.DoNotRun);
                        agent.SetMaximumSpeedLimit(agent.MaximumForwardUnlimitedSpeed, false);
                    }
                    else
                    {
                        agent.SetScriptedFlags(agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
                    }

                    if (MBRandom.RandomFloat < 0.2f)
                    {
                        if ((isEnemyNearby || isCircle) && IsPike(agent.WieldedWeapon) && !agent.IsMainAgent && distanceFromCurrentGlobalPosition < averageMaxUnlimitedSpeed)
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

                    // Disable blocking for pikemen.
                    agent.SetAgentFlags(!_blockTimer.Check(currentTime) && IsPike(agent.WieldedWeapon) && !agent.IsMainAgent && !settings.CanPikemenBlock ? agent.GetAgentFlags() & ~AgentFlag.CanDefend : agent.GetAgentFlags() | AgentFlag.CanDefend);

                    if (IsPike(agent.WieldedWeapon) && currentAction.Name.Contains("defend") && currentAction.Name.Contains("staff") && !agent.IsMainAgent && !settings.CanPikemenBlock)
                    {
                        agent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true);
                    }
                }
            }
        }

        private bool IsPike(MissionWeapon weapon) => weapon.CurrentUsageItem?.ItemUsage == "polearm_pike";
    }
}
