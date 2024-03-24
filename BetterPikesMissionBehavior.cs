using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public class BetterPikesMissionBehavior : MissionBehavior
    {
        private readonly ActionIndexCache _readyThrustActionIndex, _readyOverswingActionIndex, _guardUpActionIndex;
        private readonly Dictionary<Agent, int> _glitchTicks;

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public BetterPikesMissionBehavior()
        {
            _readyThrustActionIndex = ActionIndexCache.Create("act_ready_thrust_pike");
            _readyOverswingActionIndex = ActionIndexCache.Create("act_ready_overswing_pike");
            _guardUpActionIndex = ActionIndexCache.Create("act_guard_up_pike");
            _glitchTicks = new Dictionary<Agent, int>();
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            BetterPikesSettings settings = BetterPikesSettings.Instance;

            if (agent.IsHuman)
            {
                _glitchTicks.Add(agent, 0);

                // Determine which side to remove pikes from in siege battles.
                if (Mission.IsSiegeBattle && ((agent.Team.IsAttacker && settings.SidesToRemovePikes.SelectedIndex == 1) || (agent.Team.IsDefender && settings.SidesToRemovePikes.SelectedIndex == 2) || settings.SidesToRemovePikes.SelectedIndex == 3))
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
        }

        public override void OnAgentPanicked(Agent affectedAgent)
        {
            affectedAgent.SetScriptedFlags(affectedAgent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
            affectedAgent.SetActionChannel(1, ActionIndexCache.act_none, true);
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow) => _glitchTicks.Remove(affectedAgent);

        public override void OnMissionTick(float dt)
        {
            BetterPikesSettings settings = BetterPikesSettings.Instance;

            foreach (Formation formation in Mission.Teams.SelectMany(team => team.FormationsIncludingSpecialAndEmpty.Where(f => f.QuerySystem.IsInfantryFormation && f.FiringOrder != FiringOrder.FiringOrderHoldYourFire)))
            {
                bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty() && formation.GetCountOfUnitsWithCondition(a => IsPike(a.WieldedWeapon)) >= formation.CountOfUnits * settings.MinPikemenPercentInPikeFormation && !formation.IsLoose;
                bool isEnemyNearby = hasEnemy && formation.QuerySystem.AveragePosition.Distance(formation.QuerySystem.ClosestEnemyFormation.AveragePosition) <= settings.MinDistanceToReadyPikes;

                foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes())
                {
                    ActionIndexCache currentAction = agent.GetCurrentAction(1);

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

                    if (MBRandom.RandomFloat < 0.2f)
                    {
                        if (isEnemyNearby && IsPike(agent.WieldedWeapon) && !agent.IsMainAgent)
                        {
                            agent.GetFormationFileAndRankInfo(out _, out int rankIndex);

                            // If the pikemen's enemies are nearby, make the pikemen ready their pikes in different positions.
                            if (rankIndex < 4)
                            {
                                if (currentAction != _readyThrustActionIndex)
                                {
                                    // Make the first four ranks ready their pikes for an underarm thrust.
                                    agent.SetActionChannel(1, _readyThrustActionIndex);
                                }
                            }
                            else if (rankIndex == 4)
                            {
                                if (currentAction != _readyOverswingActionIndex)
                                {
                                    // Make the fifth rank ready their pikes for an overhead thrust.
                                    agent.SetActionChannel(1, _readyOverswingActionIndex);
                                }
                            }
                            else
                            {
                                if (currentAction != _guardUpActionIndex)
                                {
                                    // Make the sixth rank onwards ready their pikes at an angle.
                                    agent.SetActionChannel(1, _guardUpActionIndex);
                                }
                            }
                        }
                        else
                        {
                            if (currentAction == _readyThrustActionIndex || currentAction == _readyOverswingActionIndex || currentAction == _guardUpActionIndex)
                            {
                                agent.SetActionChannel(1, ActionIndexCache.act_none, true);
                            }
                        }
                    }
                }
            }

            foreach (Agent agent in Mission.Agents.Where(a => a.IsHuman))
            {
                agent.SetAgentFlags(IsPike(agent.WieldedWeapon) && !settings.CanPikemenBlock ? agent.GetAgentFlags() & ~AgentFlag.CanDefend : agent.GetAgentFlags() | AgentFlag.CanDefend);

                if (_glitchTicks.TryGetValue(agent, out int numOfTicks) && IsPike(agent.WieldedWeapon))
                {
                    Agent.ActionCodeType currentActionType = agent.GetCurrentActionType(1);

                    if (numOfTicks < 60 && agent.GetCurrentActionProgress(1) < 0.1f && (currentActionType == Agent.ActionCodeType.ReadyMelee || currentActionType == Agent.ActionCodeType.Guard))
                    {
                        _glitchTicks[agent]++;
                    }
                    else
                    {
                        if (numOfTicks >= 60)
                        {
                            agent.SetCurrentActionProgress(1, 1f);

                            if (currentActionType != Agent.ActionCodeType.EquipUnequip)
                            {
                                agent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.Instant);

                                continue;
                            }
                        }

                        _glitchTicks[agent] = 0;
                    }
                }
            }
        }

        private bool IsPike(MissionWeapon weapon) => weapon.CurrentUsageItem?.ItemUsage == "polearm_pike";
    }
}
