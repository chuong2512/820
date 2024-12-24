using UnityEngine;

public class SelectGameTypeScreenHandler : MonoBehaviour
{
    public void OnBackButtonClick()
    {
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnTwoPlayersButtonClick()
    {
        GameManager.Instance.NumberOfPlayers = NumberOfPlayers.TwoPlayersGame.ToString();
        UIManager.Instance.ActivateSpecificScreen(GameScreens.OfflineGamePlayScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnThreePlayersButtonClick()
    {
        GameManager.Instance.NumberOfPlayers = NumberOfPlayers.ThreePlayersGame.ToString();
        UIManager.Instance.ActivateSpecificScreen(GameScreens.OfflineGamePlayScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnFourPlayersButtonClick()
    {
        GameManager.Instance.NumberOfPlayers = NumberOfPlayers.FourPlayersGame.ToString();
        UIManager.Instance.ActivateSpecificScreen(GameScreens.OfflineGamePlayScreen);
        AudioManager.Instance.PlayButtonSound();
    }
}
