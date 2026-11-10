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
			Formation formation = __instance;

			if (BetterPikesHelper.IsPikeFormation(formation) && formation.IsAIControlled && formation.CachedClosestEnemyFormation != null)
			{
				bool isThreatenedByCavalry = BetterPikesHelper.IsThreatenedByCavalry(formation);
				bool isDisorganized = formation.CachedFormationIntegrityData.DeviationOfPositionsExcludeFarAgents >= 0.3f, isEnemyNearby = BetterPikesHelper.IsEnemyNearby(formation);
				bool isUnderSignificantRangedAttack = formation.QuerySystem.IsUnderRangedAttack && formation.QuerySystem.UnderRangedAttackRatio >= 0.1f;
				bool hasMovingAgents = formation.HasUnitsWithCondition(agent => agent.MovementVelocity.LengthSquared > 0);
				bool hasObstructedAgents = formation.HasUnitsWithCondition(agent => !agent.CanMoveDirectlyToPosition(formation.GetOrderPositionOfUnit(agent).AsVec2));
				bool hasFarAgents = formation.HasUnitsWithCondition(agent => !BetterPikesHelper.IsNearToPosition(agent.Position.AsVec2, formation.CachedAveragePosition, (formation.Width / 2) + 1));
				bool shouldReorganize = isDisorganized && !isEnemyNearby && !isUnderSignificantRangedAttack && hasMovingAgents && !hasObstructedAgents;

				if ((isThreatenedByCavalry || shouldReorganize) && formation.OrderPositionIsValid)
				{
					if (!_holdPositions.TryGetValue(formation, out WorldPosition holdPosition))
					{
						WorldPosition formationPosition = formation.CachedMedianPosition;

						formationPosition.SetVec2(formationPosition.AsVec2 + (formation.CurrentDirection * (formation.Depth / 2)));

						_holdPositions.Add(formation, formationPosition);
					}
					else
					{
						if (isThreatenedByCavalry)
						{
							// If the formation is threatened by cavalry, make the pikemen hold their position.
							input = hasMovingAgents || hasFarAgents ? MovementOrder.MovementOrderMove(holdPosition) : MovementOrder.MovementOrderStop;
						}
						else if (shouldReorganize)
						{
							// If the formation is disorganized, make the pikemen hold their position.
							input = MovementOrder.MovementOrderMove(holdPosition);
						}
					}
				}
				else
				{
					_holdPositions.Remove(formation);

					// Else, make the pikemen advance.
					input = MovementOrder.MovementOrderAdvance;
				}
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("SetArrangementOrder")]
		public static void Prefix2(Formation __instance, ref ArrangementOrder order)
		{
			Formation formation = __instance;

			if (BetterPikesHelper.IsPikeFormation(formation) && formation.IsAIControlled && formation.CachedClosestEnemyFormation != null)
			{
				// If the formation is threatened by cavalry, make the pikemen form a circle. Else, make the pikemen form a shield wall.
				order = BetterPikesHelper.IsThreatenedByCavalry(formation) ? ArrangementOrder.ArrangementOrderCircle : ArrangementOrder.ArrangementOrderShieldWall;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch("SetFormOrder")]
		public static void Prefix3(Formation __instance, ref FormOrder order)
		{
			Formation formation = __instance;

			if (BetterPikesHelper.IsPikeFormation(formation) && formation.IsAIControlled && formation.CachedClosestEnemyFormation != null)
			{
				// If the formation is threatened by cavalry, make the pikemen form a wide formation. Else, make the pikemen form a deep formation.
				order = BetterPikesHelper.IsThreatenedByCavalry(formation) ? FormOrder.FormOrderWide : FormOrder.FormOrderDeep;
			}
		}

		[HarmonyPatch("SetControlledByAI")]
		public static void Postfix(Formation __instance, bool isControlledByAI)
		{
			Formation formation = __instance;

			if (BetterPikesHelper.IsPikeFormation(formation))
			{
				if (isControlledByAI)
				{
					formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
					formation.SetFormOrder(FormOrder.FormOrderDeep);
				}
				else
				{
					formation.ApplyActionOnEachUnit(delegate (Agent agent)
					{
						agent.ClearTargetFrame();
					});
				}
			}
		}
	}
}
