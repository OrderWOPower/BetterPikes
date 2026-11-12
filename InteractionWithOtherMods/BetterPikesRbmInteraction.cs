using TaleWorlds.MountAndBlade;

namespace BetterPikes.InteractionWithOtherMods
{
	public class BetterPikesRbmInteraction
	{
		private static bool Prefix(Agent victim)
		{
			if (BetterPikesHelper.IsPikeFormation(victim.Formation) && !BetterPikesSettings.Instance.CanPikemenPanicFromCharge)
			{
				// Exclude pikemen from panicking due to cavalry charges.
				return false;
			}

			return true;
		}

		private static void Postfix(ref bool __result, Agent ___Agent)
		{
			if (BetterPikesHelper.IsWieldingPike(___Agent) && !BetterPikesSettings.Instance.CanPikemenKickBash)
			{
				// Disable kicking and bashing for pikemen.
				__result = false;
			}
		}
	}
}
