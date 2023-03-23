using HarmonyLib;
using KSP.Game;

namespace SpeedyStartup;

[HarmonyPatch(typeof(SplashScreensManager))]
public class SplashScreensManagerPatch
{
    [HarmonyPatch("StartAnimations")]
    [HarmonyPrefix]
    private static void StartAnimationsPrefix(Action resolve) => resolve();

    // ReSharper disable once InconsistentNaming
    [HarmonyPatch("ResolveSplashScreens")]
    [HarmonyPrefix]
    private static bool ResolveSplashScreensPrefix(SplashScreensManager __instance)
    {
        __instance.gameObject.SetActive(false);
        
        if (GameManagerPatch.LoadingScreenShouldBeVisible)
            GameManager.Instance.Game.UI.SetLoadingBarVisibility(true);
        
        return false;
    }
}