using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using KSP.Game;

namespace SpeedyStartup;

[HarmonyPatch(typeof(GameManager))]
public static class GameManagerPatch
{
    public static bool LoadingScreenShouldBeVisible { get; private set; } = true;
    
    private static readonly MethodInfo SetLoadingBarVisibilityMethod =
        SymbolExtensions.GetMethodInfo(() => GameManager.Instance.Game.UI.SetLoadingBarVisibility(false, null));

    private static readonly MethodInfo SetLoadingBarVisibilityOnlyMethod =
        SymbolExtensions.GetMethodInfo(() => SetLoadingBarVisibilityOnly(null, false, null));
    
    [HarmonyPatch("CreateGameInstance")]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CreateGameInstanceTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            // Enable the loading bar but not the loading curtain while splashes are playing
            if (instruction.Calls(SetLoadingBarVisibilityMethod))
                yield return new CodeInstruction(OpCodes.Call, SetLoadingBarVisibilityOnlyMethod);
            // Don't disable the splashes
            else if (instruction.opcode == OpCodes.Ldc_I4_0)
                yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            else yield return instruction;
        }
    }

    [HarmonyPatch("OnLoadingFinished")]
    [HarmonyPostfix]
    private static void OnLoadingFinishedPostfix()
    {
        LoadingScreenShouldBeVisible = false;
    }
    
    private static void SetLoadingBarVisibilityOnly(this UIManager ui, bool visibility, Action resolve) => 
        ui.SetLoadingBarAndCurtainVisibility(false, visibility, resolve);
    
}