using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.LinQuick;

namespace Deserters.RosterBehaviorPatches
{

    [HarmonyPatch(typeof(PrisonerReleaseCampaignBehavior), "ApplyEscapeChanceToExceededPrisoners")]
    internal class ApplyEscapeChanceToExceededPrisonersPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var method_prisonroster = AccessTools.Method(typeof(TroopRoster), nameof(TroopRoster.AddToCounts));
            foreach(var instruction in instructions)
            {
                if(instruction.opcode == OpCodes.Callvirt && instruction.operand == method_prisonroster)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DesertersRosterBehavior), nameof(DesertersRosterBehavior.OnTroopsDeserted), new Type[] { typeof(CharacterObject) }));
                }
                else
                {
                yield return instruction;
                }
            }
        }
    }
    [HarmonyPatch(typeof(PartyBase), "PartySizeLimit", MethodType.Getter)]
    internal class GetPartySizeLimitPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgenerator)
        {
            int insertion = 0;
            Label jumpLabel = ilgenerator.DefineLabel();

            var field_partyMemberSizeLastCheckVersion = AccessTools.Field(typeof(PartyBase), "_partyMemberSizeLastCheckVersion");
            var field_cachedPartyMemberSizeLimit = AccessTools.Field(typeof(PartyBase), "_cachedPartyMemberSizeLimit");
            List<CodeInstruction> codes = instructions.ToListQ<CodeInstruction>();

            codes[0].labels.Add(jumpLabel);
            List<CodeInstruction> instructionsToAdd = new()
            {
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DesertersRosterBehavior), nameof(DesertersRosterBehavior.ChangeTotalSizeLimitIfDeserters))),
                new CodeInstruction(OpCodes.Stfld, field_cachedPartyMemberSizeLimit),
                new CodeInstruction(OpCodes.Ldarg_0, null),
                new CodeInstruction(OpCodes.Ldfld, field_cachedPartyMemberSizeLimit),
                new CodeInstruction(OpCodes.Brtrue, jumpLabel),
                };

            for (int index = 0; index < codes.Count; index++)
            {
                if (codes[Math.Abs(index-1)].opcode == OpCodes.Stfld && codes[Math.Abs(index - 1)].operand == (object)field_partyMemberSizeLastCheckVersion)
                {
                    insertion = index;
                }

                if (codes[index].opcode == OpCodes.Conv_I4 && codes[index+1].opcode == OpCodes.Stfld && codes[index+1].operand == (object)field_cachedPartyMemberSizeLimit)
                {
                    codes[index].labels.Add(jumpLabel);
                }
            }
            codes.InsertRange(insertion, instructionsToAdd);
            return codes.AsEnumerable<CodeInstruction>();
        }
    }
}
