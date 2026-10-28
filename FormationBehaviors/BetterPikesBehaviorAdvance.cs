using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.FormationBehaviors
{
	[HarmonyPatch(typeof(BehaviorAdvance), "TickOccasionally")]
	public class BetterPikesBehaviorAdvance
	{
		public static void Postfix(BehaviorAdvance __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);

				if (formation.ArrangementOrder != ArrangementOrder.ArrangementOrderCircle)
				{
					float deviationOfPositions = formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents, underRangedAttackRatio = formation.QuerySystem.UnderRangedAttackRatio, formationWidth = formation.Width;
					Vec2 formationPosition = formation.CachedAveragePosition, orderPosition = formation.OrderPosition;

					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						Vec2 agentPosition = agent.Position.AsVec2, currentGlobalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);

						if (((deviationOfPositions >= 1 && underRangedAttackRatio < 0.25f) || !BetterPikesHelper.IsNearToFormation(agentPosition, formationPosition, formationWidth)) && agent.IsAIControlled && agent.CanMoveDirectlyToPosition(currentGlobalPositionOfUnit) && agent.CanMoveDirectlyToPosition(orderPosition) && !agent.InteractingWithAnyGameObject())
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
