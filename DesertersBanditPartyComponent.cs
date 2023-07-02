using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Deserters
{
    public class DesertersBanditPartyComponent : BanditPartyComponent
    {

        public override TextObject Name
        {
            get
            {
                TextObject textObject = new("{=des_deserters_party}Deserters");
                textObject.SetTextVariable("IS_BANDIT", 1);
                return textObject;
            }
        }

        protected internal DesertersBanditPartyComponent(Hideout hideout, bool isBossParty) : base(hideout, isBossParty) {}
    }
}