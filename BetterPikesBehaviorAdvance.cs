using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	[HarmonyPatch(typeof(BehaviorAdvance), "TickOccasionally")]
	public class BetterPikesBehaviorAdvance
	{
		public static void Postfix(BehaviorAdvance __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				float deviationOfPositions = formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents, formationWidth = formation.Width;
				bool isEnemyNearby = formation.CachedClosestEnemyFormationDistanceSquared <= 2500;
				Vec2 orderPosition = formation.OrderPosition, formationPosition = formation.CachedAveragePosition;

				formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);

				if (formation.ArrangementOrder != ArrangementOrder.ArrangementOrderCircle)
				{
					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						Vec2 currentGlobalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);

						if (deviationOfPositions >= 1 && agent.CanMoveDirectlyToPosition(currentGlobalPositionOfUnit) && agent.CanMoveDirectlyToPosition(orderPosition))
						{
							// Ensure that the pikemen maintain their formation.
							agent.SetTargetPosition(currentGlobalPositionOfUnit);
						}
						else
						{
							agent.ClearTargetFrame();
						}
					});
				}
				else
				{
					// If the pikemen are in circle formation, make the circle as tight as possible.
					formation.SetPositioning(formation.CachedMedianPosition, formation.Direction, 0);
					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						Vec2 currentGlobalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);

						if (isEnemyNearby && agent.Position.AsVec2.DistanceSquared(formationPosition) >= MathF.Pow(formationWidth / 2, 2) && agent.CanMoveDirectlyToPosition(currentGlobalPositionOfUnit))
						{
							// Ensure that the pikemen maintain their formation.
							agent.SetTargetPosition(currentGlobalPositionOfUnit);
						}
					});
				}
			}
		}
	}
}
