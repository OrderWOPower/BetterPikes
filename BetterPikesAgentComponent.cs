using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public class BetterPikesAgentComponent : AgentComponent
	{
		private bool _shouldWalk, _shouldRun;

		public BetterPikesAgentComponent(Agent agent) : base(agent) { }

		public override void OnTick(float dt)
		{
			if (Agent.IsActive() && BetterPikesHelper.IsWieldingPike(Agent) && (Agent.GetCurrentActionType(1) == Agent.ActionCodeType.ReadyMelee || Agent.GetCurrentActionType(1) == Agent.ActionCodeType.ReleaseMelee) && BetterPikesSettings.Instance.DoPikesHaveCollision)
			{
				float handleLength = BetterPikesHelper.GetWieldedWeapon(Agent).Item.WeaponDesign.UsedPieces[2].ScaledLength, handleOffset = BetterPikesHelper.GetWieldedWeapon(Agent).Item.WeaponDesign.UsedPieces[2].ScaledPieceOffset;
				// Find the frame of the agent's main hand.
				MatrixFrame mainHandFrame = Agent.AgentVisuals.GetGlobalFrame().TransformToParent(Agent.AgentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(Agent.Monster.MainHandItemBoneIndex));
				MatrixFrame handleFrontFrame = new MatrixFrame(mainHandFrame.rotation, mainHandFrame.origin).Elevate((handleLength * 0.4f) + handleOffset);
				MatrixFrame handleBackFrame = new MatrixFrame(mainHandFrame.rotation, mainHandFrame.origin).Elevate((handleLength * 0.3f) + handleOffset);
				// Get the closest agent colliding with the pike handle.
				Agent collidedAgent = Mission.Current.RayCastForClosestAgent(handleFrontFrame.origin, handleBackFrame.origin, -1, 0.04f, out _);

				if (collidedAgent != null && collidedAgent.IsHuman && collidedAgent.IsActive() && collidedAgent.IsEnemyOf(Agent) && !collidedAgent.HasMount && !collidedAgent.IsInBeingStruckAction && collidedAgent.GetCurrentActionType(1) != Agent.ActionCodeType.Fall)
				{
					Blow blow = new Blow(Agent.Index);
					AttackCollisionData attackCollisionDataForDebugPurpose;

					blow.DamageType = DamageTypes.Blunt;
					blow.BoneIndex = collidedAgent.Monster.HeadLookDirectionBoneIndex;
					blow.GlobalPosition = collidedAgent.Position;
					blow.GlobalPosition.z += collidedAgent.GetEyeGlobalHeight();
					blow.BaseMagnitude = 0;
					blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
					blow.InflictedDamage = 0;
					blow.SwingDirection = Agent.LookDirection;
					blow.Direction = blow.SwingDirection;
					blow.DamageCalculated = true;
					blow.BlowFlag |= BlowFlags.KnockBack;
					attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Chest, Agent.Monster.MainHandItemBoneIndex, Agent.UsageDirection.AttackDown, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, collidedAgent.Velocity, Vec3.Up);
					// Knock back the collided agent.
					collidedAgent.RegisterBlow(blow, attackCollisionDataForDebugPurpose);
				}
			}
		}

		public override void OnAIInputSet(ref Agent.EventControlFlag eventFlag, ref Agent.MovementControlFlag movementFlag, ref Vec2 inputVector)
		{
			if (_shouldWalk)
			{
				// Make the pikemen walk.
				eventFlag &= ~Agent.EventControlFlag.Run;
				eventFlag |= Agent.EventControlFlag.Walk;
			}

			if (_shouldRun)
			{
				// Make the pikemen run.
				eventFlag &= ~Agent.EventControlFlag.Walk;
				eventFlag |= Agent.EventControlFlag.Run;
			}

			if (BetterPikesHelper.IsWieldingPike(Agent))
			{
				if (!BetterPikesSettings.Instance.CanPikemenBlock)
				{
					// Disable blocking for pikemen.
					movementFlag &= ~Agent.MovementControlFlag.DefendLeft & ~Agent.MovementControlFlag.DefendRight & ~Agent.MovementControlFlag.DefendUp & ~Agent.MovementControlFlag.DefendDown;
				}

				if (!BetterPikesSettings.Instance.CanPikemenAttackUp)
				{
					// Disable overhead attacks for pikemen.
					movementFlag &= ~Agent.MovementControlFlag.AttackUp;
				}
			}
		}

		public void ForceWalking(bool shouldWalk) => _shouldWalk = shouldWalk;

		public void ForceRunning(bool shouldRun) => _shouldRun = shouldRun;

		public bool IsOwnedBy(Agent agent) => Agent == agent;
	}
}
