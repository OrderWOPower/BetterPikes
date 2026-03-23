using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(Formation))]
	public class BetterPikesFormation
	{
		public static void Prefix1(Formation __instance, ref MovementOrder input)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled && __instance.ArrangementOrder != ArrangementOrder.ArrangementOrderCircle)
			{
				// Make the pikemen advance.
				input = MovementOrder.MovementOrderAdvance;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("SetArrangementOrder")]
		public static void Prefix2(Formation __instance, ref ArrangementOrder order)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled)
			{
				// If the closest enemy formation is cavalry, make the pikemen form a circle. Else, make the pikemen form a shield wall.
				order = __instance.CachedClosestEnemyFormation != null && __instance.CachedClosestEnemyFormation.IsCavalryFormation ? ArrangementOrder.ArrangementOrderCircle : ArrangementOrder.ArrangementOrderShieldWall;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("SetFormOrder")]
		public static void Prefix3(Formation __instance, ref FormOrder order)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled)
			{
				order = FormOrder.FormOrderDeep;
			}
		}
	}
}
