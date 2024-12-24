using UnityEngine;

public class RulesScreenHandler : MonoBehaviour
{
    public void OnCloseButtonClick()
    {
        UIManager.Instance.DeactivateScreen(GameScreens.RulesScreen);
        AudioManager.Instance.PlayButtonSound();
    }
}
