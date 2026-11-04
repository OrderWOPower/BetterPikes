using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.InteractionWithOtherMods
{
	public class BetterPikesCinematicChargesInteraction
	{
		public static void Postfix(ref bool __result, Agent agent)
		{
			Formation formation = agent.Formation;

			if (!BetterPikesSettings.Instance.CanPikemenPerformCinematicCharges && BetterPikesHelper.IsPikeFormation(formation) && BetterPikesHelper.IsAheadOfPosition(agent.Position.AsVec2, formation.CachedAveragePosition, formation.CurrentDirection, 0))
			{
				// Disable sprinting for pikemen.
				__result = false;
			}
		}

		[HarmonyPatch]
		public class BetterPikesCinematicChargesInteractionGiving
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
		public class BetterPikesCinematicChargesInteractionReceiving
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
