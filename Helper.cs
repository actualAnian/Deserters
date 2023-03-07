using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace Deserters
{
    internal static class Helper
    {
        internal static bool IsDeserterParty(this MobileParty mobileParty)
        {
            return mobileParty != null && mobileParty.Party != null && mobileParty.Party.Id.Contains("Deserter");
        }
        internal static bool IsDeserterParty(this PartyBase party)
        {
            return (party != null) && party.Id.Contains("Deserter");
        }
        static public void Nuke()
        {
            bool successful = true;
            if(Campaign.Current == null)
            {
                MBInformationManager.AddQuickInformation(new TextObject("{=des_nuke_not_in_campaign}ERROR, couldn't complete the nuke, load a save game first"));
                return;
            }
            //Campaign.Current.GetCampaignBehavior<AiDesertersPatrollingBehavior>().hoursSincelastDecision = new();
            Queue<MobileParty> deserterParties = new Queue<MobileParty>(MobileParty.All.WhereQ((MobileParty m) => m.IsDeserterParty()));
            while (!deserterParties.IsEmpty())
            {
                MobileParty mobileParty = deserterParties.Dequeue();
                try
                {
                    if(mobileParty.MapEvent != null)
                        mobileParty.Party.MapEvent.DoSurrender(mobileParty.Party.Side);
                    mobileParty.IsActive = false;
                    DestroyPartyAction.Apply(null, mobileParty);
                }
                catch (Exception)
                { 
                    successful = false;
                }
            };
            DesertersRosterBehavior._desertedTroops = TroopRoster.CreateDummyTroopRoster();
            if (successful)
                MBInformationManager.AddQuickInformation(new TextObject("{=des_nuke_successful}Succesfully removed all deserter parties!"));
            else
                MBInformationManager.AddQuickInformation(new TextObject("{=des_nuke_failed}ERROR, couldn't remove all parties"));
        }
    }
}