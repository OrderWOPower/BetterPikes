using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.InteractionWithOtherMods
{
	[HarmonyPatch]
	public class BetterPikesCinematicChargesInteraction
	{
		private static MethodBase TargetMethod() => AccessTools.Method(AccessTools.TypeByName("CinematicChargesAgentComponent"), "TryPerformCharge");

		// Check whether Artem's Cinematic Charges is loaded.
		private static bool Prepare() => TargetMethod() != null;

		private static bool Prefix(Agent agent)
		{
			if (!BetterPikesSettings.Instance.CanPikemenPerformCinematicCharges && BetterPikesHelper.IsWieldingPike(agent))
			{
				// Disable cinematic charges for pikemen.
				return false;
			}

			return true;
		}
	}
}
