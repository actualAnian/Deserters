using Helpers;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace Deserters
{
        class AiDesertersPatrollingBehavior : CampaignBehaviorBase
        {
            private int HoursBeforeChoosingNewPatrolVillage { get; } = 30;

            private Dictionary<string, int> hoursSincelastDecision = new();
            public override void RegisterEvents()
            {
                CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
                CampaignEvents.OnPartyRemovedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.RemovePartyFromDict));
            }

        private void RemovePartyFromDict(PartyBase party)
        {
            if(party.IsDeserterParty() && hoursSincelastDecision.ContainsKey(party.Id))
                hoursSincelastDecision.Remove(party.Id);
        }

        private void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
            {
                if (mobileParty.Party == null || !mobileParty.IsDeserterParty()) return;

                if (mobileParty.MapFaction.Culture.CanHaveSettlement && (mobileParty.NeedTargetReset || (mobileParty.HomeSettlement.IsHideout && !mobileParty.HomeSettlement.Hideout.IsInfested)))
                {
                    Settlement settlement = SettlementHelper.FindNearestHideout(null, null);
                    if (settlement != null)
                    {
                        mobileParty.BanditPartyComponent.SetHomeHideout(settlement.Hideout);
                    }
                }

                AIBehaviorTuple item;
                //ValueTuple<AIBehaviorTuple, float> valueTuple;
            

            string DesertersID = mobileParty.Party.Id;
            if (!hoursSincelastDecision.ContainsKey(DesertersID))
                hoursSincelastDecision[DesertersID] = 0;

            if (hoursSincelastDecision[DesertersID] != 0 && hoursSincelastDecision[DesertersID] % HoursBeforeChoosingNewPatrolVillage == 0 && mobileParty.TargetSettlement != null)
                {
                    hoursSincelastDecision[DesertersID] = 0;
                    Settlement nextSettlementToPatrol = SettlementHelper.FindNearestVillage((Settlement s) =>
                    {
                        if (s.Id == mobileParty.TargetSettlement.Id) return false;
                        else return true;
                    }, mobileParty);

                    if(nextSettlementToPatrol != null)
                    {
                        item = new AIBehaviorTuple(nextSettlementToPatrol, AiBehavior.PatrolAroundPoint, false);
                        //valueTuple = new ValueTuple<AIBehaviorTuple, float>(item, 0.3f);
                        p.AIBehaviorScores.Add(item, 0.3f);
                        ++hoursSincelastDecision[DesertersID];
                        return;
                    }
                }
                Settlement patrolSettlement = mobileParty.TargetSettlement ?? mobileParty.HomeSettlement;
                item = new AIBehaviorTuple(patrolSettlement, AiBehavior.PatrolAroundPoint, false);
                //valueTuple = new ValueTuple<AIBehaviorTuple, float>(item, 0.3f);
                p.AIBehaviorScores.Add(item, 0.3f);
                ++hoursSincelastDecision[DesertersID];
            }
            public override void SyncData(IDataStore dataStore)
            {
            dataStore.SyncData("hours_since_decision_dict", ref hoursSincelastDecision);
            }
        }
}