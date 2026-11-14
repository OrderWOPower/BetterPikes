using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public static class BetterPikesHelper
	{
		public static MissionWeapon GetWieldedWeapon(Agent agent)
		{
			try
			{
				return agent.WieldedWeapon;
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(ex.ToString()));

				return MissionWeapon.Invalid;
			}
		}

		public static bool IsPike(MissionWeapon weapon) => !weapon.IsEmpty && weapon.GetWeaponComponentDataForUsage(0).WeaponDescriptionId != null && weapon.GetWeaponComponentDataForUsage(0).WeaponDescriptionId.Contains("Pike");

		public static bool IsWieldingPike(Agent agent) => agent != null && IsPike(GetWieldedWeapon(agent));

		public static bool IsPikeFormation(Formation formation) => formation != null && formation.CountOfUnits > 1 && formation.GetCountOfUnitsWithCondition(agent => IsWieldingPike(agent)) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation && formation.FiringOrder != FiringOrder.FiringOrderHoldYourFire;

		public static bool IsEnemyNearby(Formation formation) => formation?.CachedClosestEnemyFormationDistanceSquared <= MathF.Pow(BetterPikesSettings.Instance.MaxDistanceToReadyPikes, 2);

		public static bool IsThreatenedByCavalry(Formation formation)
		{
			Formation closestEnemyFormation = formation?.CachedClosestEnemyFormation?.Formation;

			// The pikemen feel threatened by cavalry if the number of cavalry in the closest enemy formation is greater than or equal to 25% the number of pikemen, and that formation is charging.
			return closestEnemyFormation != null && closestEnemyFormation.GetCountOfUnitsWithCondition(agent => agent.HasMount) >= formation.CountOfUnits / 4f && (closestEnemyFormation.GetReadonlyMovementOrderReference().OrderType == OrderType.Charge || closestEnemyFormation.GetReadonlyMovementOrderReference().OrderType == OrderType.ChargeWithTarget);
		}

		public static bool IsAheadOfPosition(Vec2 targetPosition, Vec2 referencePosition, Vec2 direction, float distance) => Vec2.DotProduct(targetPosition - (referencePosition + (direction * distance)), direction) > 0;

		public static bool IsNearToPosition(Vec2 targetPosition, Vec2 referencePosition, float distance) => targetPosition.DistanceSquared(referencePosition) < MathF.Pow(distance, 2);
	}
}
