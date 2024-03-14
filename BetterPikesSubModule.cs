using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    // This mod gives pikes realistic lengths, fixes the pike bracing animation so that the longer pikes can hit enemy cavalry, and makes pikemen use pike formations that are as functional as the game allows.
    public class BetterPikesSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.betterpikes").PatchAll();

        public override void OnBeforeMissionBehaviorInitialize(Mission mission) => mission.AddMissionBehavior(new BetterPikesMissionBehavior());
    }
}
