using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public class BetterPikesAgentComponent : AgentComponent
	{
		private bool _shouldWalk, _shouldRun;

		public BetterPikesAgentComponent(Agent agent) : base(agent) { }

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
	}
}
