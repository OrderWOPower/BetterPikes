using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public class BetterPikesAgentAi
	{
		// Exclude pikemen from panicking due to cavalry charges.
		private static bool Prefix(Agent victim) => !BetterPikesHelper.IsPikeFormation(victim.Formation);
	}
}
