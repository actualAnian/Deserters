using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;



namespace Deserters
{
    public class DesertersRosterBehavior : CampaignBehaviorBase
    {
        private int _currentNoDeserterParties = 0;

        private static TroopRoster _desertedTroops = TroopRoster.CreateDummyTroopRoster();
        private static int _nextRandomizedTroopNumber = MBRandom.RandomInt(
                                MathF.Min(Settings.Instance.MinDesertersPartySize, Settings.Instance.MaxDesertersPartySize),
                                MathF.Max(Settings.Instance.MinDesertersPartySize, Settings.Instance.MaxDesertersPartySize));
        public static int ChangeTotalSizeLimitIfDeserters(PartyBase party)
        {
            if (party.IsDeserterParty())
            {
                return MathF.Max(Settings.Instance.MinDesertersPartySize, Settings.Instance.MaxDesertersPartySize);
            }
            return 0;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnTroopsDesertedEvent.AddNonSerializedListener(this, new Action<MobileParty, TroopRoster>(OnTroopsDeserted));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, SpawnDesertersIfPossible);
            CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(DecrementDesertersCounter));
        }

        private void DecrementDesertersCounter(PartyBase party)
        {
            if (party.IsDeserterParty() && _currentNoDeserterParties > 0) --_currentNoDeserterParties;
        }

        public void OnTroopsDeserted(MobileParty mobileParty, TroopRoster newlyDesertedTroops)
        {
            if (_desertedTroops.TotalManCount >= _nextRandomizedTroopNumber) return;
            foreach (TroopRosterElement troopType in newlyDesertedTroops.GetTroopRoster())
            {
                int troopsToRemove = CalculateNoTroopsToRemove(troopType.Character, troopType.Number);
                newlyDesertedTroops.RemoveTroop(troopType.Character, troopsToRemove);
            }
            _desertedTroops.Add(newlyDesertedTroops);
        }
        public static void OnTroopsDeserted(CharacterObject character)
        {
            if (_desertedTroops.TotalManCount >= _nextRandomizedTroopNumber) return;
            int troopsToRemove = CalculateNoTroopsToRemove(character, 1);
            _desertedTroops.AddToCounts(character, 1 - troopsToRemove, false, 0, 0, true, -1);
        }
        public static int CalculateNoTroopsToRemove(CharacterObject troopType, int numberOfTroopsInArmy)
        {
            if (troopType == null) return 0;
            float chanceToKeepTroop = 0;
            int nOTroopsToRemove = 0;
            chanceToKeepTroop = troopType.Tier switch
            {
                0 => Settings.Instance.ChanceToKeepTier0Troop,
                1 => Settings.Instance.ChanceToKeepTier1Troop,
                2 => Settings.Instance.ChanceToKeepTier2Troop,
                3 => Settings.Instance.ChanceToKeepTier3Troop,
                4 => Settings.Instance.ChanceToKeepTier4Troop,
                5 => Settings.Instance.ChanceToKeepTier5Troop,
                6 => Settings.Instance.ChanceToKeepTier6Troop,
                _ => Settings.Instance.ChanceToKeepTroopOther,
            };
            Random random = new();
            for (int i = 0; i < numberOfTroopsInArmy; i++)
                if (random.NextDouble() > chanceToKeepTroop) nOTroopsToRemove++;
            return nOTroopsToRemove;
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("numer_of_deserter_parties", ref _currentNoDeserterParties);
            dataStore.SyncData("deserter_troops_global_pool", ref _desertedTroops);
        }
        public void SpawnDesertersIfPossible()
        {
            if (_currentNoDeserterParties >= Settings.Instance.MaxNumberOfDeserterParties) _desertedTroops = TroopRoster.CreateDummyTroopRoster();
            if (_desertedTroops.TotalManCount >= _nextRandomizedTroopNumber)
            {
                Clan _looterClan = Clan.All.WhereQ((Clan c) => c.StringId == "looters").Single();
                _nextRandomizedTroopNumber = MBRandom.RandomInt(
                    MathF.Min(Settings.Instance.MinDesertersPartySize, Settings.Instance.MaxDesertersPartySize),
                    MathF.Max(Settings.Instance.MinDesertersPartySize, Settings.Instance.MaxDesertersPartySize));

                IEnumerable<Hideout> infestedHideouts = Hideout.All.WhereQ((Hideout h) => h.IsInfested);
                if (!infestedHideouts.Any()) return;
                Hideout randomHideout = infestedHideouts.ElementAt(MBRandom.RandomInt(0, infestedHideouts.Count()));
                MobileParty desertersParty = MobileParty.CreateParty("Deserters", new DesertersBanditPartyComponent(randomHideout, false), delegate (MobileParty mobileParty)
            {
                mobileParty.ActualClan = _looterClan;
            });

                if (randomHideout != null)
                {
                    float num = 45f * 1.5f;
                    TroopRoster troops = TroopRoster.CreateDummyTroopRoster();
                    troops.Add(_desertedTroops);
                    _desertedTroops = TroopRoster.CreateDummyTroopRoster();
                    desertersParty.InitializeMobilePartyAtPosition(troops, TroopRoster.CreateDummyTroopRoster(), randomHideout.Settlement.GatePosition);
                    Vec2 vec = desertersParty.Position2D;
                    float radiusAroundPlayerPartySquared = 20;
                    for (int i = 0; i < 15; i++)
                    {
                        Vec2 vec2 = MobilePartyHelper.FindReachablePointAroundPosition(vec, num, 0f);
                        if (vec2.DistanceSquared(MobileParty.MainParty.Position2D) > radiusAroundPlayerPartySquared)
                        {
                            vec = vec2;
                            break;
                        }
                    }
                    if (vec != desertersParty.Position2D)
                    {
                        desertersParty.Position2D = vec;
                    }
                    desertersParty.Party.Visuals.SetMapIconAsDirty();
                    desertersParty.ActualClan = _looterClan;
                    int initialGold = (int)(10f * (float)desertersParty.Party.MemberRoster.TotalManCount * (0.5f + 1f * MBRandom.RandomFloat));
                    desertersParty.InitializePartyTrade(initialGold);
                    foreach (ItemObject itemObject in Items.All)
                    {
                        if (itemObject.IsFood)
                        {
                            int num3 = 8;
                            int num2 = MBRandom.RoundRandomized((float)desertersParty.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * (float)num3 * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                            if (num2 > 0)
                            {
                                desertersParty.ItemRoster.AddToCounts(itemObject, num2);
                            }
                        }
                    }
                    desertersParty.Aggressiveness = 1f - 0.2f * MBRandom.RandomFloat;
                    desertersParty.SetMovePatrolAroundPoint(randomHideout.Settlement.Position2D);
                    ++_currentNoDeserterParties;
                }
            }
        }
    }
}