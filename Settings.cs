using MCM.Abstractions.Base;
using MCM.Abstractions.FluentBuilder;
using MCM.Abstractions.FluentBuilder.Models;
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

        public bool DisplayMessages { get; set; } = false;


        public ISettingsBuilder RegisterSettings()
        {
            int order = 0;
            var builder = BaseSettingsBuilder.Create("deserters", "Deserters")!
                .SetFormat("json2")

                .SetFolderName(nameof(Deserters))
                .SetSubFolder("Deserters")
                .CreateGroup("{=des_general_settings}General deserter settings", BuildGeneralDeserterSettings)
                .CreateGroup("{=des_troop_quality_settings}Deserter parties quality of troop settings", BuildDeserterTroopRank)
                .CreateGroup("{=des_miscellaneous_settings}Miscellaneous", BuildOtherSettings)

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
                    .SetPropertyValue("chance_keep_troop_other", Settings.Instance.ChanceToKeepTroopOther)
                    
                    .SetPropertyValue("display_messages", Settings.Instance.DisplayMessages);

            void BuildGeneralDeserterSettings(ISettingsPropertyGroupBuilder builder)
                => builder

                    .AddInteger("max_parties",
                             "{=des_max_parties}Max Parties",
                             1,
                             100,
                             new ProxyRef<int>(() => Settings.Instance.MaxNumberOfDeserterParties, value => Settings.Instance.MaxNumberOfDeserterParties = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_max_parties_desc}Maximum number of deserter parties that can roam the world")
                                .SetOrder(order++))
                            
                    .AddInteger("min_party_size",
                             "{=des_min_troop_count}Minimum Party Size",
                             1,
                             200,
                             new ProxyRef<int>(() => Settings.Instance.MinDesertersPartySize, value => Settings.Instance.MinDesertersPartySize = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_min_troop_count_desc}Minimum troop count of a deserter party")
                                .SetOrder(order++))

                    .AddInteger("max_party_size",
                             "{=des_max_troop_count}Maximum Party Size",
                             1,
                             200,
                             new ProxyRef<int>(() => Settings.Instance.MaxDesertersPartySize, value => Settings.Instance.MaxDesertersPartySize = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_max_troop_count_desc}Maximum troop count of a deserter party")
                                .SetOrder(order++))

                    .SetGroupOrder(1);

            void BuildDeserterTroopRank(ISettingsPropertyGroupBuilder builder)
                => builder
                    .AddFloatingInteger("chance_keep_tier0_troop",
                             "{=des_likelihood_tier0}Tier 0 Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier0Troop, value => Settings.Instance.ChanceToKeepTier0Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier0_desc}Chance that a tier 0 troop will end up in a deserter party")
                                .SetOrder(order++))

                    .AddFloatingInteger("chance_keep_tier1_troop",
                             "{=des_likelihood_tier1}Tier 1 Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier1Troop, value => Settings.Instance.ChanceToKeepTier1Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier1_desc}Chance that a tier 1 troop will end up in a deserter party")
                                .SetOrder(order++))

                    .AddFloatingInteger("chance_keep_tier2_troop",
                             "{=des_likelihood_tier2}Tier 2 Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier2Troop, value => Settings.Instance.ChanceToKeepTier2Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier2_desc}Chance that a tier 2 troop will end up in a deserter party")
                                .SetOrder(order++))

                    .AddFloatingInteger("chance_keep_tier3_troop",
                             "{=des_likelihood_tier3}Tier 3 Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier3Troop, value => Settings.Instance.ChanceToKeepTier3Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier3_desc}Chance that a tier 3 troop will end up in a deserter party")
                                .SetOrder(order++))

                    .AddFloatingInteger("chance_keep_tier4_troop",
                             "{=des_likelihood_tier4}Tier 4 Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier4Troop, value => Settings.Instance.ChanceToKeepTier4Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier4_desc}Chance that a tier 4 troop will end up in a deserter party")
                                .SetOrder(order++))

                    .AddFloatingInteger("chance_keep_tier5_troop",
                             "{=des_likelihood_tier5}Tier 5 Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier5Troop, value => Settings.Instance.ChanceToKeepTier5Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier5_desc}Chance that a tier 5 troop will end up in a deserter party")
                                .SetOrder(order++))

                    .AddFloatingInteger("chance_keep_tier6_troop",
                             "{=des_likelihood_tier6}Tier 6 Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTier6Troop, value => Settings.Instance.ChanceToKeepTier6Troop = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier6_desc}Chance that a tier 6 troop will end up in a deserter party")
                                .SetOrder(order++))

                    .AddFloatingInteger("chance_keep_troop_other",
                             "{=des_likelihood_tier_other}Other Tier Likelihood",
                             0,
                             1,
                             new ProxyRef<float>(() => Settings.Instance.ChanceToKeepTroopOther, value => Settings.Instance.ChanceToKeepTroopOther = value),
                             propBuilder => propBuilder.SetRequireRestart(false)
                                .SetHintText("{=des_likelihood_tier_other_desc}Chance that other tier troop will end up in a deserter party")
                                .SetOrder(order++))

                    .SetGroupOrder(2);
            void BuildOtherSettings(ISettingsPropertyGroupBuilder builder)
            => builder
                    .AddBool("display_messages",
                            "{=des_display_messages}display debug messages",
                            new ProxyRef<bool>(() => Settings.Instance.DisplayMessages, value => Settings.Instance.DisplayMessages = value),
                            propBuilder => propBuilder.SetRequireRestart(false)
                            .SetHintText("{=des_display_messages_desc}Will create a message disclosing the location of a newly created or destroyed deserters party")
                            .SetOrder(order++))


                    .AddButton("nuke",
                               "{=des_nuke_desc}Remove all deserter parties",
                               new StorageRef((Action)(() => { Helper.Nuke(); })),
                               "{=des_nuke}Nuke",
                                propBuilder => propBuilder.SetRequireRestart(false)
                                    .SetOrder(order++))
                    .SetGroupOrder(3);
        }
    }
}