using UnityEngine;

public class QuitScreenHandler : MonoBehaviour
{
    public void OnYesButtonClick()
    {
        Application.Quit();
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnNoButtonClick()
    {
        UIManager.Instance.DeactivateScreen(GameScreens.QuitScreen);
        AudioManager.Instance.PlayButtonSound();
    }
}
