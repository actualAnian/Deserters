using TaleWorlds.SaveSystem;

namespace Deserters
{
    public class DesertersSaveableTypeDefiner : SaveableTypeDefiner
    {
        public DesertersSaveableTypeDefiner() : base(65300) {}
        protected override void DefineClassTypes()
        {
            base.AddClassDefinition(typeof(DesertersBanditPartyComponent), 1, null);
        }
    }
}