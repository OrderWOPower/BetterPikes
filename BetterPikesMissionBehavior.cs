using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        private readonly ActionIndexCache _readyThrustActionIndex, _readyOverswingActionIndex, _guardUpActionIndex;
        private readonly List<Agent> _readyPikemen;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public BetterPikesMissionBehavior()
        {
            _readyThrustActionIndex = ActionIndexCache.Create("act_ready_thrust_pike");
            _readyOverswingActionIndex = ActionIndexCache.Create("act_ready_overswing_pike");
            _guardUpActionIndex = ActionIndexCache.Create("act_guard_up_pike");
            _readyPikemen = new List<Agent>();
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
            affectedAgent.SetScriptedFlags(affectedAgent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
            affectedAgent.SetActionChannel(0, ActionIndexCache.act_none, true);

            _readyPikemen.Remove(affectedAgent);
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow) => _readyPikemen.Remove(affectedAgent);

        public override void OnMissionTick(float dt)
        {
            BetterPikesSettings settings = BetterPikesSettings.Instance;

            foreach (Formation formation in Mission.Teams.SelectMany(team => team.FormationsIncludingSpecialAndEmpty.Where(f => f.QuerySystem.IsInfantryFormation)))
            {
                FormationQuerySystem querySystem = formation.QuerySystem;
                bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty() && formation.GetCountOfUnitsWithCondition(a => IsPike(a.WieldedWeapon)) >= formation.CountOfUnits * settings.MinPikemenPercentInPikeFormation && !formation.IsLoose;
                bool isEnemyNearby = hasEnemy && querySystem.AveragePosition.Distance(querySystem.ClosestEnemyFormation.AveragePosition) <= settings.MinDistanceToReadyPikes && !querySystem.ClosestEnemyFormation.IsCavalryFormation && !querySystem.ClosestEnemyFormation.IsRangedCavalryFormation;

                foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes())
                {
                    if (hasEnemy)
                    {
                        // If the pikemen have enemies, make the pikemen walk.
                        agent.SetScriptedFlags(agent.GetScriptedFlags() | Agent.AIScriptedFrameFlags.DoNotRun);
                        agent.SetMaximumSpeedLimit(agent.MaximumForwardUnlimitedSpeed, false);
                    }
                    else
                    {
                        agent.SetScriptedFlags(agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
                    }

                    if (isEnemyNearby && IsPike(agent.WieldedWeapon) && !agent.IsMainAgent)
                    {
                        if (agent.GetCurrentAction(1) == ActionIndexCache.act_none)
                        {
                            if (MBRandom.RandomFloat < 0.05f && !_readyPikemen.Contains(agent))
                            {
                                agent.GetFormationFileAndRankInfo(out _, out int rankIndex);

                                // If the pikemen's enemies are nearby, make the pikemen ready their pikes in different positions.
                                if (rankIndex < 4)
                                {
                                    // Make the first four ranks ready their pikes for an underarm thrust.
                                    agent.SetActionChannel(0, _readyThrustActionIndex, false, 0UL, 0f, 1f, 0.5f);
                                }
                                else if (rankIndex == 4)
                                {
                                    // Make the fifth rank ready their pikes for an overhead thrust.
                                    agent.SetActionChannel(0, _readyOverswingActionIndex, false, 0UL, 0f, 1f, 0.5f);
                                }
                                else
                                {
                                    // Make the sixth rank onwards ready their pikes at an angle.
                                    agent.SetActionChannel(0, _guardUpActionIndex, false, 2UL, 0f, 1f, 0.5f);
                                }

                                _readyPikemen.Add(agent);
                            }
                        }
                        else
                        {
                            agent.SetActionChannel(0, ActionIndexCache.act_none, true);
                        }
                    }
                    else
                    {
                        if (_readyPikemen.Contains(agent))
                        {
                            agent.SetActionChannel(0, ActionIndexCache.act_none, true);
                        }

                        _readyPikemen.Remove(agent);
                    }
                }
            }

            foreach (Agent agent in Mission.Agents.Where(a => a.IsHuman))
            {
                if (IsPike(agent.WieldedWeapon) && !settings.CanPikemenBlock)
                {
                    agent.SetAgentFlags(agent.GetAgentFlags() & ~AgentFlag.CanDefend);
                }
                else
                {
                    agent.SetAgentFlags(agent.GetAgentFlags() | AgentFlag.CanDefend);
                }
            }
        }

        private bool IsPike(MissionWeapon weapon) => weapon.CurrentUsageItem?.ItemUsage == "polearm_pike";
    }
}
