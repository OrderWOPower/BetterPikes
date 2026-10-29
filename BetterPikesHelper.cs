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

		public static bool IsPikeFormation(Formation formation) => formation != null && formation.CountOfUnits > 0 && formation.GetCountOfUnitsWithCondition(agent => IsWieldingPike(agent)) >= formation.CountOfUnits * BetterPikesSettings.Instance.MinPikemenPercentInPikeFormation && formation.FiringOrder != FiringOrder.FiringOrderHoldYourFire;

		public static bool IsNearToCurrentGlobalPosition(Vec2 agentPosition, Vec2 currentGlobalPositionOfUnit) => agentPosition.DistanceSquared(currentGlobalPositionOfUnit) < 1;

		public static bool IsNearToFormation(Vec2 agentPosition, Vec2 formationPosition, float formationWidth) => agentPosition.DistanceSquared(formationPosition) < MathF.Pow((formationWidth / 2) + 1, 2);

		public static bool IsAheadOfFormation(Vec2 agentPosition, Vec2 formationPosition, Vec2 formationDirection) => Vec2.DotProduct(agentPosition - formationPosition, formationDirection) > 0;
	}
}
