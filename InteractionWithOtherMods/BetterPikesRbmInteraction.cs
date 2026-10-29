using System.Linq;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.InteractionWithOtherMods
{
	public class BetterPikesRbmInteraction
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

		protected static void Postfix1(TacticComponent __instance, ref float __result)
		{
			if (__instance.Team.FormationsIncludingSpecialAndEmpty.Any(formation => BetterPikesHelper.IsPikeFormation(formation)))
			{
				// Prevent pike formations from being split.
				__result = 0;
			}
		}

		private static void Postfix2(ref bool __result, Agent ___Agent)
		{
			if (!BetterPikesSettings.Instance.CanPikemenKickBash && BetterPikesHelper.IsWieldingPike(___Agent))
			{
				// Disable kicking and bashing for pikemen.
				__result = false;
			}
		}
	}
}
