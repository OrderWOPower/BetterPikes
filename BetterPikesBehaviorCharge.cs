using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(BehaviorCharge))]
	public class BetterPikesBehaviorCharge
	{
		[HarmonyPostfix]
		[HarmonyPatch("OnBehaviorActivatedAux")]
		protected static void Postfix1(BehaviorCharge __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				formation.ApplyActionOnEachUnit(delegate (Agent agent)
				{
					agent.ClearTargetFrame();
				});

				if (formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle)
				{
					// If the pikemen are in circle formation, make the circle as tight as possible.
					formation.SetPositioning(formation.CachedMedianPosition, formation.Direction, 1);
				}
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch("TickOccasionally")]
		public static void Postfix2(BehaviorCharge __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation) && formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle)
			{
				Vec2 formationPosition = formation.CachedAveragePosition;

				formation.ApplyActionOnEachUnit(delegate (Agent agent)
				{
					if (agent.Position.AsVec2.Distance(formationPosition) >= (formation.Width / 2) + 1 && agent.CanMoveDirectlyToPosition(formationPosition))
					{
						// Ensure that the pikemen maintain their formation.
						agent.SetTargetPosition(formationPosition);
					}
					else
					{
						agent.ClearTargetFrame();
					}
				});
			}
		}
	}
}
