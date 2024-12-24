using UnityEngine;
using UnityEngine.UI;

public class SplashScreenHandler : MonoBehaviour
{
    public Text copyrightsText;

    private void Start()
    {
        copyrightsText.text = "© " + System.DateTime.Now.ToString("yyyy") + " Callisto 1947 PAK, Inc. All rights reserved.";
        AdmobManager.Instance.HideBannerAd();
        Invoke(nameof(DisplayMainScreen), 3f);
    }

    private void DisplayMainScreen()
    {
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
    }
}
