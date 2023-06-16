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

        [SettingPropertyInteger("Pike Damage", 1, 100, "0", Order = 0, RequireRestart = false, HintText = "Multiplier for pike damage. Default is 10.")]
        [SettingPropertyGroup("Multipliers", GroupOrder = 0)]
        public int PikeDamageMultiplier { get; set; } = 10;

        [SettingPropertyFloatingInteger("Minimum Pikemen in Pike Formation", 0.5f, 1.0f, "#0%", Order = 0, RequireRestart = false, HintText = "Minimum percentage of pikemen in a formation to be treated as a pike formation. Default is 50%.")]
        [SettingPropertyGroup("Limits", GroupOrder = 1)]
        public float MinPikemenPercentInPikeFormation { get; set; } = 0.5f;

        [SettingPropertyDropdown("Remove Pikes from Side", Order = 0, RequireRestart = false, HintText = "Which side to remove pikes from in siege battles. Default is Both.")]
        [SettingPropertyGroup("Siege Battles", GroupOrder = 2)]
        public Dropdown<string> SidesToRemovePikes { get; set; } = new Dropdown<string>(new string[] { "None", "Attacker", "Defender", "Both" }, 3);
    }
}
