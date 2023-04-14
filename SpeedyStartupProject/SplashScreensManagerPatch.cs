using HarmonyLib;
using KSP.Game;
using KSP.Game.StartupFlow;

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

        if (LandingHUDPatch.ShouldShowLegalTexts)
        {
            var legal = GameManager.Instance.Game.UI.Get<LandingHUD>().LegalMenu;
            legal.gameObject.SetActive(true);
            legal.StartupFlow();
        }
        
        return false;
    }
}