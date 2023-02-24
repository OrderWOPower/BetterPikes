using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    // This mod gives pikes realistic lengths, fixes the pike bracing animation so that the longer pikes can hit enemy cavalry, and makes pikemen use pike formations that are as functional as the game allows.
    public class BetterPikesSubModule : MBSubModuleBase
    {
        public static Harmony HarmonyInstance { get; set; }

        protected override void OnSubModuleLoad()
        {
            HarmonyInstance = new Harmony("mod.bannerlord.betterpikes");
            HarmonyInstance.PatchAll();
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission) => mission.AddMissionBehavior(new BetterPikesMissionBehavior());
    }
}
