using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(BehaviorCharge), "TickOccasionally")]
	public class BetterPikesBehaviorCharge
	{
		public static void Postfix(BehaviorCharge __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);

				if (formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle)
				{
					// If the pikemen are in circle formation, make the circle as tight as possible.
					formation.SetPositioning(formation.CachedMedianPosition, formation.Direction, 0);
				}
			}
		}
	}
}
