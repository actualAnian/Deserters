using TaleWorlds.CampaignSystem.Party;

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
    }
}