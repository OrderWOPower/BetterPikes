using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace HighSellPrice
{
    // This mod alerts the player when in range of a city if they have trade goods with high selling prices.
    public class HighSellPriceSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.highsellprice").PatchAll();
    }
}
