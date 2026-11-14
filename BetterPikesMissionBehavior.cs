using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public class BetterPikesMissionBehavior : MissionBehavior
	{
		private readonly ActionIndexCache _readyThrustActionIndex, _guardUpActionIndex;
		private readonly List<BetterPikesAgentComponent> _agentComponents;

		public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

		public BetterPikesMissionBehavior()
		{
			_readyThrustActionIndex = ActionIndexCache.Create("act_ready_thrust_pike");
			_guardUpActionIndex = ActionIndexCache.Create("act_guard_up_pike");
			_agentComponents = new List<BetterPikesAgentComponent>();
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsHuman)
			{
				BetterPikesAgentComponent agentComponent = new BetterPikesAgentComponent(agent);

				agent.AddComponent(agentComponent);
				agent.SetHasOnAiInputSetCallback(true);

				_agentComponents.Add(agentComponent);
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow) => _agentComponents.RemoveAll(c => c.IsOwnedBy(affectedAgent));

		public override void OnAgentPanicked(Agent affectedAgent)
		{
			if (affectedAgent.IsHuman && affectedAgent.IsActive() && (affectedAgent.GetCurrentAction(1) == _readyThrustActionIndex || affectedAgent.GetCurrentAction(1) == _guardUpActionIndex))
			{
				affectedAgent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true, blendInPeriod: 0.5f);
			}
		}

		public override void OnMissionTick(float dt)
		{
			foreach (BetterPikesAgentComponent agentComponent in _agentComponents)
			{
				agentComponent.ForceWalking(false);
				agentComponent.ForceRunning(false);
			}

			foreach (Formation formation in Mission.Teams.SelectMany(team => team.FormationsIncludingSpecialAndEmpty))
			{
				if (BetterPikesHelper.IsPikeFormation(formation))
				{
					bool isLoose = formation.IsLoose, hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty(), isEnemyNearby = BetterPikesHelper.IsEnemyNearby(formation);
					bool shouldAlwaysReadyPikes = (formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle || formation.ArrangementOrder == ArrangementOrder.ArrangementOrderSquare) && formation.CachedCurrentVelocity.LengthSquared < 1;
					float formationWidth = formation.Width, formationDepth = formation.Depth;
					Vec2 formationPosition = formation.CachedAveragePosition, formationOrderPosition = formation.OrderPosition, formationDirection = formation.CurrentDirection;
					Formation.FormationIntegrityDataGroup cachedFormationIntegrityData = formation.CachedFormationIntegrityData;

					foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes().Where(a => a.IsHuman && a.IsActive()))
					{
						Vec2 agentPosition = agent.Position.AsVec2, currentPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);
						bool isAheadOfLastRank = BetterPikesHelper.IsAheadOfPosition(agentPosition, formationPosition, formationDirection, (-formationDepth / 2) - 1);
						bool isNearToFormation = BetterPikesHelper.IsNearToPosition(agentPosition, formationPosition, (MathF.Sqrt(MathF.Pow(formationWidth, 2) + MathF.Pow(formationDepth, 2)) / 2) + 1);
						bool isNearToCurrentPosition = BetterPikesHelper.IsNearToPosition(agentPosition, currentPositionOfUnit, 1);

						if (!isLoose)
						{
							BetterPikesAgentComponent agentComponent = _agentComponents.FirstOrDefault(c => c.IsOwnedBy(agent));

							agentComponent.ForceWalking(hasEnemy && isAheadOfLastRank && !agent.InteractingWithAnyGameObject());
							agentComponent.ForceRunning(!isAheadOfLastRank);
						}

						agent.SetMaximumSpeedLimit(agent.GetMaximumForwardUnlimitedSpeed(), false);

						if (agent.HumanAIComponent?.GetCurrentlyMovingGameObject() != null && agentPosition.DistanceSquared(agent.HumanAIComponent.GetCurrentlyMovingGameObject().GetUserFrameForAgent(agent).Origin.AsVec2) >= 25)
						{
							// Prevent the pikemen from picking up items that are further than 5m.
							agent.StopUsingGameObject();
						}

						if (MBRandom.RandomFloat < 0.2f)
						{
							ActionIndexCache currentAction = agent.GetCurrentAction(1);

							if (((isEnemyNearby && !isLoose && isAheadOfLastRank) || shouldAlwaysReadyPikes) && isNearToFormation && BetterPikesHelper.IsWieldingPike(agent) && agent.IsAIControlled && !agent.IsDoingPassiveAttack)
							{
								agent.GetFormationFileAndRankInfo(out _, out int rankIndex);

								// Make the pikemen ready their pikes in different positions.
								if (rankIndex < 5)
								{
									if (currentAction != _readyThrustActionIndex)
									{
										// Make the first five ranks ready their pikes for a thrust.
										agent.SetActionChannel(1, _readyThrustActionIndex, startProgress: MBRandom.RandomFloat);
									}
								}
								else
								{
									if (currentAction != _guardUpActionIndex)
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

						if (isEnemyNearby && BetterPikesHelper.IsWieldingPike(agent) && agent.IsAIControlled)
						{
							// Ensure that the pikemen maintain their formation.
							agent.SetFormationIntegrityData(currentPositionOfUnit, formationDirection, cachedFormationIntegrityData.AverageVelocityExcludeFarAgents, cachedFormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents, cachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents, true);

							if (!isLoose && isNearToCurrentPosition && !BetterPikesSettings.Instance.CanPikemenTurnSideways)
							{
								float angularDifference = (formationOrderPosition - formationPosition).RotationInRadians.ToDegrees() - agent.MovementDirectionAsAngle.ToDegrees();

								angularDifference = ((((angularDifference + 180) % 360) + 360) % 360) - 180;

								if (!MBMath.IsBetweenInclusive(angularDifference, -1, 1))
								{
									// Prevent the pikemen from turning sideways when in pike formation.
									agent.SetMovementDirection(Vec2.FromRotation(agent.MovementDirectionAsAngle + (angularDifference / 10).ToRadians()));
								}
							}
						}
					}
				}
				else
				{
					foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes().Where(a => a.IsHuman && a.IsActive()))
					{
						ActionIndexCache currentAction = agent.GetCurrentAction(1);

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
