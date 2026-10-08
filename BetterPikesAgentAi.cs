using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public class BetterPikesAgentAi
	{
		private static bool Prefix(Agent victim)
		{
			if (!BetterPikesSettings.Instance.CanPikemenPanicFromCharge && BetterPikesHelper.IsPikeFormation(victim.Formation))
			{
				// Exclude pikemen from panicking due to cavalry charges.
				return false;
			}

			return true;
		}

		private static void Postfix(ref bool __result, Agent ___Agent)
		{
			if (!BetterPikesSettings.Instance.CanPikemenKickBash && BetterPikesHelper.IsWieldingPike(___Agent))
			{
				// Disable kicking and bashing for pikemen.
				__result = false;
			}
		}
	}
}
