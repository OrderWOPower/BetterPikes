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
					float deviationOfPositions = formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents, underRangedAttackRatio = formation.QuerySystem.UnderRangedAttackRatio, formationDepth = formation.Depth;
					Vec2 formationPosition = formation.CachedAveragePosition, formationDirection = formation.CurrentDirection, orderPosition = formation.OrderPosition;

					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						Vec2 agentPosition = agent.Position.AsVec2, currentPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);
						bool isAheadOfFirstRank = BetterPikesHelper.IsAheadOfPosition(agentPosition, formationPosition, formationDirection, (formationDepth / 2) + 1);
						bool isAheadOfLastRank = BetterPikesHelper.IsAheadOfPosition(agentPosition, formationPosition, formationDirection, (-formationDepth / 2) - 1);

						if (hasEnemy && ((deviationOfPositions >= 0.5f && underRangedAttackRatio < 0.25f) || isAheadOfFirstRank || !isAheadOfLastRank) && agent.IsAIControlled && agent.CanMoveDirectlyToPosition(currentPositionOfUnit) && agent.CanMoveDirectlyToPosition(orderPosition) && !agent.InteractingWithAnyGameObject())
						{
							// Ensure that the pikemen maintain their formation.
							agent.SetTargetPosition(currentPositionOfUnit);
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
