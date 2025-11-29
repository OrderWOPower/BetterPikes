using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    public static class BetterPikesHelper
    {
        public static bool IsPike(MissionWeapon weapon) => !weapon.IsEmpty && weapon.GetWeaponComponentDataForUsage(0).WeaponDescriptionId != null && weapon.GetWeaponComponentDataForUsage(0).WeaponDescriptionId.Contains("Pike");
    }
}
