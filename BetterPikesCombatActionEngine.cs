using HarmonyLib;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch]
	public class BetterPikesCombatActionEngine
	{
		private static MethodBase TargetMethod() => AccessTools.Method(AccessTools.TypeByName("CombatActionEngine"), "ShouldProcess");

		// Check whether AI Kick N Bash is loaded.
		private static bool Prepare() => TargetMethod() != null;

		private static void Postfix(ref bool __result, Agent agent)
		{
			if (agent != null && agent.IsHuman && BetterPikesHelper.IsWieldingPike(agent))
			{
				// Disable kicking and bashing for pikemen.
				__result = false;
			}
		}
	}
}
