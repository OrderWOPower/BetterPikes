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
				bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty();

				if (!formation.IsDeployment)
				{
					if (hasEnemy)
					{
						formation.SetMovementOrder(MovementOrder.MovementOrderAdvance);
					}

					formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
					formation.SetFormOrder(FormOrder.FormOrderWide);
				}

				if (formation.ArrangementOrder != ArrangementOrder.ArrangementOrderCircle)
				{
					float deviationOfPositions = formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents, underRangedAttackRatio = formation.QuerySystem.UnderRangedAttackRatio, formationWidth = formation.Width;
					Vec2 formationPosition = formation.CachedAveragePosition, formationDirection = formation.CurrentDirection, orderPosition = formation.OrderPosition;

					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						Vec2 agentPosition = agent.Position.AsVec2, currentGlobalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);
						bool isNearToCurrentGlobalPosition = BetterPikesHelper.IsNearToCurrentGlobalPosition(agentPosition, currentGlobalPositionOfUnit);
						bool isNearToFormation = BetterPikesHelper.IsNearToFormation(agentPosition, formationPosition, formationWidth);
						bool isAheadOfFormation = BetterPikesHelper.IsAheadOfFormation(agentPosition, formationPosition, formationDirection);

						if (hasEnemy && ((deviationOfPositions >= 1 && underRangedAttackRatio < 0.25f) || !isNearToFormation || (!isNearToCurrentGlobalPosition && isAheadOfFormation)) && agent.IsAIControlled && agent.CanMoveDirectlyToPosition(currentGlobalPositionOfUnit) && agent.CanMoveDirectlyToPosition(orderPosition) && !agent.InteractingWithAnyGameObject())
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
