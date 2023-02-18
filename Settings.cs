using MCM.Abstractions.Base;
using MCM.Abstractions.FluentBuilder;
using MCM.Common;
using System;

namespace Deserters
{
    public class Settings : IDisposable
    {
        public const string InstanceID = "Deserters";
        private static Settings? _instance;

        public void Dispose()
        {
        }
        public static Settings Instance
        {
            get
            {
                _instance ??= new Settings();
                return _instance;
            }
        }

        public int MaxNumberOfDeserterParties { get; set; } = 15;
        public int MinDesertersPartySize { get; set; } = 30;
        public int MaxDesertersPartySize { get; set; } = 60;

        public float ChanceToKeepTier0Troop { get; set; } = 0.1f;
        public float ChanceToKeepTier1Troop { get; set; } = 0.4f;
        public float ChanceToKeepTier2Troop { get; set; }= 0.7f;
        public float ChanceToKeepTier3Troop { get; set; } = 0.9f;
        public float ChanceToKeepTier4Troop { get; set; } = 1f;
        public float ChanceToKeepTier5Troop { get; set; } = 1f;
        public float ChanceToKeepTier6Troop { get; set; } = 1f;
        public float ChanceToKeepTroopOther { get; set; } = 1f;


        public ISettingsBuilder RegisterSettings()
        {
            var builder = BaseSettingsBuilder.Create("deserters", "Deserters")!
                .SetFormat("json2")

                .SetFolderName(nameof(Deserters))
                .SetSubFolder("Deserters")
                .CreateGroup("General deserter settings", BuildGeneralDeserterSettings)
                .CreateGroup("Deserter troop tiers likelihood", BuildDeserterTroopRank)

                .CreatePreset(BaseSettings.DefaultPresetId, BaseSettings.DefaultPresetName, builder => BuildDefaultPreset(builder, new()));

            return builder;

            static void BuildDefaultPreset(ISettingsPresetBuilder builder, object value)
                => builder
                    .SetPropertyValue("max_parties", Settings.Instance.MaxNumberOfDeserterParties)
                    .SetPropertyValue("min_party_size", Settings.Instance.MinDesertersPartySize)
                    .SetPropertyValue("max_party_size", Settings.Instance.MaxDesertersPartySize)

                    .SetPropertyValue("chance_keep_tier0_troop", Settings.Instance.ChanceToKeepTier0Troop)
                    .SetPropertyValue("chance_keep_tier1_troop", Settings.Instance.ChanceToKeepTier1Troop)
                    .SetPropertyValue("chance_keep_tier2_troop", Settings.Instance.ChanceToKeepTier2Troop)
                    .SetPropertyValue("chance_keep_tier3_troop", Settings.Instance.ChanceToKeepTier3Troop)
                    .SetPropertyValue("chance_keep_tier4_troop", Settings.Instance.ChanceToKeepTier4Troop)
                    .SetPropertyValue("chance_keep_tier5_troop", Settings.Instance.ChanceToKeepTier5Troop)
                    .SetPropertyValue("chance_keep_tier6_troop", Settings.Instance.ChanceToKeepTier6Troop)
                    .SetPropertyValue("chance_keep_troop_other", Settings.Instance.ChanceToKeepTroopOther);

            void BuildGeneralDeserterSettings(ISettingsPropertyGroupBuilder builder)
                => builder

                    .AddInteger("max_parties",
                             "Maximum number of deserter parties that can roam the world",
                             1,
                             100,
                             new ProxyRef<int>(() => Settings.Instance.MaxNumberOfDeserterParties, value => Settings.Instance.MaxNumberOfDeserterParties = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddInteger("min_party_size",
                             "Minimum troop count of a deserter party",
                             1,
                             200,
                             new ProxyRef<int>(() => Settings.Instance.MinDesertersPartySize, value => Settings.Instance.MinDesertersPartySize = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddInteger("max_party_size",
                             "Maximum troop count of a deserter party",
                             1,
                             200,
                             new ProxyRef<int>(() => Settings.Instance.MaxDesertersPartySize, value => Settings.Instance.MaxDesertersPartySize = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .SetGroupOrder(1);

            void BuildDeserterTroopRank(ISettingsPropertyGroupBuilder builder)
                => builder
                    .AddFloatingInteger("chance_keep_tier0_troop",
                             "Chance that a tier 0 troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier0Troop, value => Settings.Instance.ChanceToKeepTier0Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddFloatingInteger("chance_keep_tier1_troop",
                             "Chance that a tier 1 troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier1Troop, value => Settings.Instance.ChanceToKeepTier1Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddFloatingInteger("chance_keep_tier2_troop",
                             "Chance that a tier 2 troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier2Troop, value => Settings.Instance.ChanceToKeepTier2Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddFloatingInteger("chance_keep_tier3_troop",
                             "Chance that a tier 3 troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier3Troop, value => Settings.Instance.ChanceToKeepTier3Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddFloatingInteger("chance_keep_tier4_troop",
                             "Chance that a tier 4 troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier4Troop, value => Settings.Instance.ChanceToKeepTier4Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddFloatingInteger("chance_keep_tier5_troop",
                             "Chance that a tier 5 troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier5Troop, value => Settings.Instance.ChanceToKeepTier5Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddFloatingInteger("chance_keep_tier6_troop",
                             "Chance that a tier 6 troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier6Troop, value => Settings.Instance.ChanceToKeepTier6Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .AddFloatingInteger("chance_keep_troop_other",
                             "Chance that other tier troop will end up in a deserter party",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTroopOther, value => Settings.Instance.ChanceToKeepTroopOther = value),
                             propBuilder => propBuilder.SetRequireRestart(false))
                    .SetGroupOrder(2);
        }
    }
}