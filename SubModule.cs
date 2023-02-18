using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Deserters
{
    public partial class SubModule : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Harmony harmony = new("Deserters");
            harmony.PatchAll();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            MCM.Abstractions.FluentBuilder.ISettingsBuilder settingsBuilder = Settings.Instance.RegisterSettings();
            FluentGlobalSettings settings = settingsBuilder.BuildAsGlobal();
            settings.Register();

        }
        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            if (starterObject is CampaignGameStarter starter)
            {
                starter.AddBehavior(new DesertersRosterBehavior());
                starter.AddBehavior(new AiDesertersPatrollingBehavior());
            }
        }

    }
}