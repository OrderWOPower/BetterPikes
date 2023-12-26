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

        [SettingPropertyInteger("{=BPBSKLDLKc}Pike Damage", 1, 100, "0", Order = 0, RequireRestart = false, HintText = "{=BP2TOhK7BM}Multiplier for pike damage. Default is 10.")]
        [SettingPropertyGroup("{=BPrqKBOYqk}Multipliers", GroupOrder = 0)]
        public int PikeDamageMultiplier { get; set; } = 10;

        [SettingPropertyFloatingInteger("{=BPcC9MxnMc}Minimum Pikemen in Pike Formation", 0.5f, 1.0f, "#0%", Order = 0, RequireRestart = false, HintText = "{=BPka6D5dyC}Minimum percentage of pikemen in a formation to be treated as a pike formation. Default is 50%.")]
        [SettingPropertyGroup("{=BPHWax9o9M}Limits", GroupOrder = 1)]
        public float MinPikemenPercentInPikeFormation { get; set; } = 0.5f;

        [SettingPropertyDropdown("{=BP8UqKzzmC}Remove Pikes from Side", Order = 0, RequireRestart = false, HintText = "{=BPxKQet4oG}Which side to remove pikes from in siege battles. Default is Both.")]
        [SettingPropertyGroup("{=BPSWNvjeoU}Siege Battles", GroupOrder = 2)]
        public Dropdown<string> SidesToRemovePikes { get; set; } = new Dropdown<string>(new string[] { "{=BPyjaAG9cm}None", "{=BPtIuVae3C}Attacker", "{=BP4nvUyZLd}Defender", "{=BPEy4uJVvQ}Both" }, 3);
    }
}
