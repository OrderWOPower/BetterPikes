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

		public static bool IsNearToFormation(Vec2 agentPosition, Vec2 formationPosition, float distance) => agentPosition.DistanceSquared(formationPosition) < MathF.Pow(distance, 2);

		public static bool IsAheadOfFormation(Vec2 agentPosition, Vec2 formationPosition, Vec2 formationDirection, float distance) => Vec2.DotProduct(agentPosition - formationPosition, formationDirection) > distance;
	}
}
