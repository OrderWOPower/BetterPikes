using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace BetterPikes
{
    public class BetterPikesSettings : AttributeGlobalSettings<BetterPikesSettings>
    {
        public override string Id => "BetterPikes";

        public override string DisplayName => "Better Pikes";

        public override string FolderName => "BetterPikes";

        public override string FormatType => "json2";

        [SettingPropertyInteger("{=BetterPikes05}Pike Damage", 1, 100, "0", Order = 0, RequireRestart = false, HintText = "{=BetterPikes06}Multiplier for pike damage. Default is 10.")]
        [SettingPropertyGroup("{=BetterPikes01}Multipliers", GroupOrder = 0)]
        public int PikeDamageMultiplier { get; set; } = 10;

        [SettingPropertyFloatingInteger("{=BetterPikes07}Minimum Pikemen in Pike Formation", 0.5f, 1.0f, "#0%", Order = 0, RequireRestart = false, HintText = "{=BetterPikes08}Minimum percentage of pikemen in a formation to be treated as a pike formation. Default is 50%.")]
        [SettingPropertyGroup("{=BetterPikes02}Limits", GroupOrder = 1)]
        public float MinPikemenPercentInPikeFormation { get; set; } = 0.5f;

        [SettingPropertyInteger("{=BetterPikes09}Minimum Distance to Ready Pikes", 0, 1000, "0m", Order = 1, RequireRestart = false, HintText = "{=BetterPikes10}Minimum distance to nearby enemies for pikemen to ready their pikes. Default is 50m.")]
        [SettingPropertyGroup("{=BetterPikes02}Limits", GroupOrder = 1)]
        public int MinDistanceToReadyPikes { get; set; } = 50;

        [SettingPropertyBool("{=BetterPikes11}Pikemen Can Block", Order = 0, RequireRestart = false, HintText = "{=BetterPikes12}Pikemen can block with their pikes (unrealistic). Disabled by default.")]
        [SettingPropertyGroup("{=BetterPikes03}Blocking", GroupOrder = 2)]
        public bool CanPikemenBlock { get; set; } = false;

        [SettingPropertyDropdown("{=BetterPikes13}Remove Pikes from Side", Order = 0, RequireRestart = false, HintText = "{=BetterPikes14}Which side to remove pikes from in siege battles. Default is Both.")]
        [SettingPropertyGroup("{=BetterPikes04}Siege Battles", GroupOrder = 3)]
        public Dropdown<string> SidesToRemovePikes { get; set; } = new Dropdown<string>(new string[] { "{=BetterPikes15}None", "{=BetterPikes16}Attacker", "{=BetterPikes17}Defender", "{=BetterPikes18}Both" }, 3);
    }
}
