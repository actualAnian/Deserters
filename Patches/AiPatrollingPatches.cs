using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.LinQuick;

namespace Deserters.Patches
{
    [HarmonyDebug]
    [HarmonyPatch(typeof(AiBanditPatrollingBehavior), "AiHourlyTick")]
    public static class AiBanditPatrollingBehaviornAiHourlyTickPatch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgenerator)
        {
            List<CodeInstruction> codes = instructions.ToListQ<CodeInstruction>();

            Label jumpLabel = ilgenerator.DefineLabel();

            codes.Last().labels.Add(jumpLabel);


            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Helper), nameof(Helper.IsDeserterParty), new Type[] { typeof(MobileParty) }));
            yield return new CodeInstruction(OpCodes.Brtrue, jumpLabel);
            yield return new CodeInstruction(OpCodes.Ret);
            foreach (var instruction in instructions)
            {
                yield return instruction;
            }


        }
    }
}
