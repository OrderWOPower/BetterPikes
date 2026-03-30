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
			if (affectedAgent.IsHuman && affectedAgent.IsActive() && (affectedAgent.GetCurrentAction(1) == _readyThrustActionIndex || affectedAgent.GetCurrentAction(1) == _guardUpActionIndex))
			{
				affectedAgent.SetActionChannel(1, ActionIndexCache.act_none, ignorePriority: true, blendInPeriod: 0.5f);
			}
		}

		public override void OnMissionTick(float dt)
		{
			foreach (Agent agent in Mission.Agents.Where(a => a.IsHuman && a.IsActive()))
			{
				if (BetterPikesHelper.IsWieldingPike(agent) && (agent.GetCurrentAction(1).GetName().Contains("ready") || agent.GetCurrentAction(1).GetName().Contains("release")))
				{
					float handleLength = BetterPikesHelper.GetWieldedWeapon(agent).Item.WeaponDesign.UsedPieces[2].ScaledLength, handleOffset = BetterPikesHelper.GetWieldedWeapon(agent).Item.WeaponDesign.UsedPieces[2].ScaledPieceOffset;
					// Find the frame of the agent's main hand.
					MatrixFrame mainHandFrame = agent.AgentVisuals.GetGlobalFrame().TransformToParent(agent.AgentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(agent.Monster.MainHandItemBoneIndex));
					MatrixFrame handleFrontFrame = new MatrixFrame(mainHandFrame.rotation, mainHandFrame.origin).Elevate((handleLength * 0.4f) + handleOffset);
					MatrixFrame handleBackFrame = new MatrixFrame(mainHandFrame.rotation, mainHandFrame.origin).Elevate((handleLength * 0.3f) + handleOffset);
					// Get the closest agent colliding with the pike handle.
					Agent collidedAgent = Mission.RayCastForClosestAgent(handleFrontFrame.origin, handleBackFrame.origin, -1, 0.04f, out _);

					if (collidedAgent != null && collidedAgent.IsHuman && collidedAgent.IsActive() && collidedAgent.IsEnemyOf(agent) && !collidedAgent.HasMount && !collidedAgent.IsInBeingStruckAction && collidedAgent.GetCurrentActionType(1) != Agent.ActionCodeType.Fall)
					{
						Blow blow = new Blow(agent.Index);
						AttackCollisionData attackCollisionDataForDebugPurpose;

						blow.DamageType = DamageTypes.Blunt;
						blow.BoneIndex = collidedAgent.Monster.HeadLookDirectionBoneIndex;
						blow.GlobalPosition = collidedAgent.Position;
						blow.GlobalPosition.z += collidedAgent.GetEyeGlobalHeight();
						blow.BaseMagnitude = 200f;
						blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
						blow.InflictedDamage = 0;
						blow.SwingDirection = agent.LookDirection;
						blow.Direction = blow.SwingDirection;
						blow.DamageCalculated = false;
						blow.BlowFlag |= BlowFlags.KnockBack;
						attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Chest, agent.Monster.MainHandItemBoneIndex, Agent.UsageDirection.AttackDown, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, collidedAgent.Velocity, Vec3.Up);
						// Knock back the collided agent.
						collidedAgent.RegisterBlow(blow, attackCollisionDataForDebugPurpose);
					}
				}

				if (Mission.Mode == MissionMode.Battle)
				{
					agent.SetScriptedFlags(agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
				}
			}

			foreach (Formation formation in Mission.Teams.SelectMany(team => team.FormationsIncludingSpecialAndEmpty.Where(f => BetterPikesHelper.IsPikeFormation(f))))
			{
				float closestEnemyFormationDistanceSquared = formation.CachedClosestEnemyFormationDistanceSquared, formationWidth = formation.Width;
				bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty() && formation.CachedClosestEnemyFormation != null, isLoose = formation.IsLoose;
				bool isEnemyNearby = closestEnemyFormationDistanceSquared <= MathF.Pow(BetterPikesSettings.Instance.MaxDistanceToReadyPikes, 2);
				bool isInCircleArrangement = formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle, isInSquareArrangement = formation.ArrangementOrder == ArrangementOrder.ArrangementOrderSquare;
				Vec2 formationPosition = formation.CachedAveragePosition, closestEnemyFormationPosition = hasEnemy ? formation.CachedClosestEnemyFormation.Formation.CachedAveragePosition : Vec2.Invalid;

				foreach (Agent agent in formation.GetUnitsWithoutDetachedOnes().Where(a => a.IsHuman && a.IsActive()))
				{
					Vec2 agentPosition = agent.Position.AsVec2;

					agent.SetMaximumSpeedLimit(agent.GetMaximumForwardUnlimitedSpeed(), false);

					if (Mission.Mode == MissionMode.Battle && hasEnemy && !isLoose && (agentPosition.DistanceSquared(formation.GetCurrentGlobalPositionOfUnit(agent, true)) < 1 || agentPosition.DistanceSquared(closestEnemyFormationPosition) <= closestEnemyFormationDistanceSquared))
					{
						// Make the pikemen walk.
						agent.SetScriptedFlags(agent.GetScriptedFlags() | Agent.AIScriptedFrameFlags.DoNotRun);
					}

					if (MBRandom.RandomFloat < 0.2f)
					{
						ActionIndexCache currentAction = agent.GetCurrentAction(1);

						if (((isEnemyNearby && !isLoose) || isInCircleArrangement || isInSquareArrangement) && BetterPikesHelper.IsWieldingPike(agent) && agent.IsAIControlled && !agent.IsDoingPassiveAttack && agentPosition.DistanceSquared(formationPosition) < MathF.Pow((formationWidth / 2) + 1, 2))
						{
							if (currentAction != _readyThrustActionIndex && currentAction != _guardUpActionIndex)
							{
								agent.GetFormationFileAndRankInfo(out _, out int rankIndex);

								// Make the pikemen ready their pikes in different positions.
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
