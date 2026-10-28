using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.FormationBehaviors
{
	[HarmonyPatch(typeof(BehaviorCharge), "TickOccasionally")]
	public class BetterPikesBehaviorCharge
	{
		public static void Postfix(BehaviorCharge __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);

				if (formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle)
				{
					// If the pikemen are in circle formation, make the circle as tight as possible.
					formation.SetPositioning(null, null, 0);
					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						agent.ClearTargetFrame();
					});
				}
			}
		}
	}
}
