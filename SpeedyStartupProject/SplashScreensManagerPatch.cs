using HarmonyLib;
using KSP.Game;
using KSP.Game.StartupFlow;

namespace SpeedyStartup;

[HarmonyPatch(typeof(SplashScreensManager))]
public class SplashScreensManagerPatch
{
    // ReSharper disable once InconsistentNaming
    [HarmonyPatch("StartAnimations")]
    [HarmonyPrefix]
    private static void StartAnimationsPrefix(SplashScreensManager __instance)
    {
        __instance._resolvedCalled = true;
        __instance._resolveAction();
    }

    // ReSharper disable once InconsistentNaming
    [HarmonyPatch("ResolveSplashScreens")]
    [HarmonyPrefix]
    private static void ResolveSplashScreensPrefix(SplashScreensManager __instance)
    {
        if (GameManagerPatch.LoadingScreenShouldBeVisible)
            GameManager.Instance.Game.UI.SetLoadingBarVisibility(true);

        if (LandingHUDPatch.ShouldShowLegalTexts)
        {
            var legal = GameManager.Instance.Game.UI.Get<LandingHUD>().LegalMenu;
            legal.gameObject.SetActive(true);
            legal.StartupFlow();
        }
    }
}