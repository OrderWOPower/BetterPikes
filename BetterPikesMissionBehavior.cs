﻿using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        private readonly ActionIndexCache _readyThrustActionIndex, _readyOverswingActionIndex, _guardUpActionIndex;
        private readonly Timer _blockTimer;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public BetterPikesMissionBehavior()
        {
            _readyThrustActionIndex = ActionIndexCache.Create("act_ready_thrust_pike");
            _readyOverswingActionIndex = ActionIndexCache.Create("act_ready_overswing_pike");
            _guardUpActionIndex = ActionIndexCache.Create("act_guard_up_pike");
            _blockTimer = new Timer(0f, 1f, true);
        }

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
            BetterPikesSettings settings = BetterPikesSettings.Instance;

            foreach (Formation formation in Mission.Teams.SelectMany(team => team.FormationsIncludingSpecialAndEmpty.Where(f => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation)))
            {
                bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty() && formation.GetCountOfUnitsWithCondition(a => IsPike(a.WieldedWeapon)) >= formation.CountOfUnits * settings.MinPikemenPercentInPikeFormation && formation.FiringOrder != FiringOrder.FiringOrderHoldYourFire;
                bool isEnemyNearby = hasEnemy && formation.QuerySystem.AveragePosition.Distance(formation.QuerySystem.ClosestEnemyFormation.AveragePosition) <= settings.MinDistanceToReadyPikes, isLoose = formation.IsLoose;
                float num = formation.QuerySystem.FormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents * 3f;

                foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes().Concat(formation.DetachedUnits).Where(a => a.IsHuman))
                {
                    float num2 = agent.Position.AsVec2.Distance(formation.GetCurrentGlobalPositionOfUnit(agent, true));

                    // Disable blocking for pikemen.
                    agent.SetAgentFlags(!_blockTimer.Check(Mission.CurrentTime) && IsPike(agent.WieldedWeapon) && !agent.IsMainAgent && !settings.CanPikemenBlock ? agent.GetAgentFlags() & ~AgentFlag.CanDefend : agent.GetAgentFlags() | AgentFlag.CanDefend);

                    if (hasEnemy && !isLoose && num2 < num * 2f)
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
                        ActionIndexCache currentAction = agent.GetCurrentAction(1);

                        if (isEnemyNearby && IsPike(agent.WieldedWeapon) && !agent.IsMainAgent && num2 < num)
                        {
                            if (currentAction != _readyThrustActionIndex && currentAction != _readyOverswingActionIndex && currentAction != _guardUpActionIndex)
                            {
                                agent.GetFormationFileAndRankInfo(out _, out int rankIndex);

                                // If the pikemen's enemies are nearby, make the pikemen ready their pikes in different positions.
                                if (rankIndex < 4)
                                {
                                    // Make the first four ranks ready their pikes for an underarm thrust.
                                    agent.SetActionChannel(1, _readyThrustActionIndex, startProgress: MBRandom.RandomFloat);
                                }
                                else if (rankIndex == 4)
                                {
                                    // Make the fifth rank ready their pikes for an overhead thrust.
                                    agent.SetActionChannel(1, _readyOverswingActionIndex, startProgress: MBRandom.RandomFloat);
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
                            if (currentAction == _readyThrustActionIndex || currentAction == _readyOverswingActionIndex || currentAction == _guardUpActionIndex)
                            {
                                agent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true, blendInPeriod: 0.5f);
                            }
                        }
                    }
                }
            }
        }

        private bool IsPike(MissionWeapon weapon) => weapon.CurrentUsageItem?.ItemUsage == "polearm_pike";
    }
}
