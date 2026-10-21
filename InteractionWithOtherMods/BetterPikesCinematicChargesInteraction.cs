using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.InteractionWithOtherMods
{
	public class BetterPikesCinematicChargesInteraction
	{
		[HarmonyPatch]
		public class BetterPikesCinematicChargesInteractionGiver
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

		[HarmonyPatch]
		public class BetterPikesCinematicChargesInteractionReceiver
		{
			private static MethodBase TargetMethod() => AccessTools.Method(AccessTools.TypeByName("CinematicChargesAgentComponent"), "IsChargeable");

			// Check whether Artem's Cinematic Charges is loaded.
			private static bool Prepare() => TargetMethod() != null;

			private static void Postfix(ref bool __result, Agent agent)
			{
				if (!BetterPikesSettings.Instance.CanPikemenPerformCinematicCharges && BetterPikesHelper.IsWieldingPike(agent))
				{
					// Disable cinematic charges for pikemen.
					__result = false;
				}
			}
		}
	}
}
