using System.Reflection;
using HarmonyLib;
using KSP.Game;
using KSP.Game.StartupFlow;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace SpeedyStartup;

[HarmonyPatch(typeof(LandingHUD), nameof(LandingHUD.Start))]
public class LandingHUDPatch
{
    private static readonly MethodInfo SetActiveMethod =
        SymbolExtensions.GetMethodInfo(() => ((GameObject)null).SetActive(false));

    private static readonly MethodInfo StartupFlowMethod =
        SymbolExtensions.GetMethodInfo(() => ((LegalMenu)null).StartupFlow());

    private static readonly MethodInfo SetShouldShowLegalTextsMethod =
        SymbolExtensions.GetMethodInfo(() => SetShouldShowLegalTexts(null));

    public static bool ShouldShowLegalTexts { get; private set; }
    
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.Calls(SetActiveMethod))
            {
                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Pop);
            }
            else if (instruction.Calls(StartupFlowMethod))
                yield return new CodeInstruction(OpCodes.Call, SetShouldShowLegalTextsMethod);
            else yield return instruction;
        }
    }

    private static void SetShouldShowLegalTexts(LegalMenu menu)
    {
        if (GameManager.Instance._splashScreens.gameObject.activeSelf)
            ShouldShowLegalTexts = true;
        else
        {
            menu.gameObject.SetActive(true);
            menu.StartupFlow();
        }
    }
}