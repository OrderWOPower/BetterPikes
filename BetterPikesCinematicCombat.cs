using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
	public class BetterPikesCinematicCombat
	{
		public static bool Prefix1(MissionWeapon affectorWeapon)
		{
			if (BetterPikesHelper.IsPike(affectorWeapon))
			{
				// Disable cinematic combat for pikemen.
				return false;
			}

			return true;
		}

		public static bool Prefix2()
		{
			if (Agent.Main != null && BetterPikesHelper.IsWieldingPike(Agent.Main))
			{
				// Disable cinematic combat for pikemen.
				return false;
			}

			return true;
		}
	}
}
