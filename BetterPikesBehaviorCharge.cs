using HarmonyLib;
using TaleWorlds.Library;
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
				float formationWidth = formation.Width;
				Vec2 formationPosition = formation.CachedAveragePosition;

				if (formation.ArrangementOrder == ArrangementOrder.ArrangementOrderCircle)
				{
					// If the pikemen are in circle formation, make the circle as tight as possible.
					formation.SetPositioning(formation.CachedMedianPosition, formation.Direction, 0);
					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						if (agent.Position.AsVec2.Distance(formationPosition) >= (formationWidth / 2) + 1 && agent.CanMoveDirectlyToPosition(formationPosition))
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
}
