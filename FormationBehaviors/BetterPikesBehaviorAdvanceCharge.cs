using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes.FormationBehaviors
{
	[HarmonyPatch]
	public class BetterPikesBehaviorAdvanceCharge
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(BehaviorAdvance), "TickOccasionally");
			yield return AccessTools.Method(typeof(BehaviorCharge), "TickOccasionally");
		}

		public static void Postfix(BehaviorComponent __instance)
		{
			Formation formation = __instance.Formation;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				bool hasEnemy = formation.HasAnyEnemyFormationsThatIsNotEmpty();

				if (hasEnemy)
				{
					formation.SetMovementOrder(MovementOrder.MovementOrderAdvance);
					formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
					formation.SetFormOrder(FormOrder.FormOrderDeep);
				}

				if (formation.ArrangementOrder != ArrangementOrder.ArrangementOrderCircle)
				{
					float formationDepth = formation.Depth;
					Vec2 formationPosition = formation.CachedAveragePosition, formationDirection = formation.CurrentDirection;

					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						Vec2 agentPosition = agent.Position.AsVec2, currentPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(agent, true);
						bool isAheadOfFirstRank = BetterPikesHelper.IsAheadOfPosition(agentPosition, formationPosition, formationDirection, (formationDepth / 2) + 1);
						bool isAheadOfLastRank = BetterPikesHelper.IsAheadOfPosition(agentPosition, formationPosition, formationDirection, (-formationDepth / 2) - 1);

						if (hasEnemy && (isAheadOfFirstRank || !isAheadOfLastRank) && agent.IsAIControlled && agent.CanMoveDirectlyToPosition(currentPositionOfUnit) && !agent.InteractingWithAnyGameObject())
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
