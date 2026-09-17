using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(Formation))]
	public class BetterPikesFormation
	{
		private static readonly Dictionary<Formation, WorldPosition> _holdPositions = new Dictionary<Formation, WorldPosition>();

		public static void Prefix1(Formation __instance, ref MovementOrder input)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled && __instance.CachedClosestEnemyFormation != null)
			{
				if (__instance.CachedClosestEnemyFormation.Formation.GetCountOfUnitsWithCondition(agent => agent.HasMount) >= __instance.CountOfUnits * 0.25f && __instance.OrderPositionIsValid)
				{
					if (!_holdPositions.TryGetValue(__instance, out WorldPosition holdPosition))
					{
						_holdPositions.Add(__instance, __instance.CachedMedianPosition);
					}
					else
					{
						// If the number of cavalry in the closest enemy formation is greater than or equal to 25% the number of pikemen, make the pikemen hold their position.
						input = MovementOrder.MovementOrderMove(holdPosition);
					}
				}
				else
				{
					_holdPositions.Remove(__instance);

					// Else, make the pikemen advance.
					input = MovementOrder.MovementOrderAdvance;
				}
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("SetArrangementOrder")]
		public static void Prefix2(Formation __instance, ref ArrangementOrder order)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled && __instance.CachedClosestEnemyFormation != null)
			{
				// If the number of cavalry in the closest enemy formation is greater than or equal to 25% the number of pikemen, make the pikemen form a circle. Else, make the pikemen form a shield wall.
				order = __instance.CachedClosestEnemyFormation.Formation.GetCountOfUnitsWithCondition(agent => agent.HasMount) >= __instance.CountOfUnits * 0.25f ? ArrangementOrder.ArrangementOrderCircle : ArrangementOrder.ArrangementOrderShieldWall;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("SetFormOrder")]
		public static void Prefix3(Formation __instance, ref FormOrder order)
		{
			if (BetterPikesHelper.IsPikeFormation(__instance) && __instance.IsAIControlled && __instance.CachedClosestEnemyFormation != null)
			{
				// If the number of cavalry in the closest enemy formation is greater than or equal to 25% the number of pikemen, make the pikemen form a wide formation. Else, make the pikemen form a deep formation.
				order = __instance.CachedClosestEnemyFormation.Formation.GetCountOfUnitsWithCondition(agent => agent.HasMount) >= __instance.CountOfUnits * 0.25f ? FormOrder.FormOrderWide : FormOrder.FormOrderDeep;
			}
		}
	}
}
