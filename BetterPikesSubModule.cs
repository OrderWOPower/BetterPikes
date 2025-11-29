using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BetterPikes
{
    // This mod gives pikes realistic lengths, fixes the pike bracing animation so that the longer pikes can hit enemy cavalry, and makes pikemen use pike formations that are as functional as the game allows.
    public class BetterPikesSubModule : MBSubModuleBase
    {
        private Harmony _harmony;
        private Type _typeofPostureLogic;

        protected override void OnSubModuleLoad()
        {
            _harmony = new Harmony("mod.bannerlord.betterpikes");
            _harmony.PatchAll();
        }

        public override void OnBeforeMissionBehaviorInitialize(Mission mission) => mission.AddMissionBehavior(new BetterPikesMissionBehavior());

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            _typeofPostureLogic = AccessTools.TypeByName("RBMAI.PostureLogic+CreateMeleeBlowPatch");

            _harmony.Patch(AccessTools.Method(_typeofPostureLogic, "calculateDefenderPostureDamage"), postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesPostureLogic), "Postfix")));
            _harmony.Patch(AccessTools.Method(_typeofPostureLogic, "calculateAttackerPostureDamage"), postfix: new HarmonyMethod(AccessTools.Method(typeof(BetterPikesPostureLogic), "Postfix")));
        }

        public override void OnGameEnd(Game game)
        {
            _harmony.Unpatch(AccessTools.Method(_typeofPostureLogic, "calculateDefenderPostureDamage"), AccessTools.Method(typeof(BetterPikesPostureLogic), "Postfix"));
            _harmony.Unpatch(AccessTools.Method(_typeofPostureLogic, "calculateAttackerPostureDamage"), AccessTools.Method(typeof(BetterPikesPostureLogic), "Postfix"));
        }
    }
}
