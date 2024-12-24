using UnityEngine;
using UnityEngine.UI;

public class CreatePrivateRoomHandler : MonoBehaviour
{
    public InputField roomKeyInputTextField;
    public Toggle fiftyTogglePrivateRoom;
    public Toggle hundredTogglePrivateRoom;
    public Toggle oneFiftyTogglePrivateRoom;

    private void OnEnable()
    {
        roomKeyInputTextField.text = null;
        SetWinningScore(PreferenceManager.WinningPoints);
    }

    public void OnInputFieldValueChanged()
    {
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().HideMessage();
    }

    public void OnCreatePrivateRoomButtonClick()
    {
        if (string.IsNullOrEmpty(roomKeyInputTextField.text))
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage("Enter room key");
        else
            NetworkManager.Instance.CreatePrivateRoom(roomKeyInputTextField.text, 2);

        AudioManager.Instance.PlayButtonSound();
    }

    public void OnSelectWinningScoreToggleClick(int winningScore)
    {
        SetWinningScore(winningScore);
        AudioManager.Instance.PlayButtonSound();
    }

    private void SetWinningScore(int winningPoints)
    {
        PreferenceManager.WinningPoints = winningPoints;

        fiftyTogglePrivateRoom.isOn = false;
        hundredTogglePrivateRoom.isOn = false;
        oneFiftyTogglePrivateRoom.isOn = false;

        if (winningPoints == 50)
            fiftyTogglePrivateRoom.isOn = true;
        else if (winningPoints == 100)
            hundredTogglePrivateRoom.isOn = true;
        else if (winningPoints == 150)
            oneFiftyTogglePrivateRoom.isOn = true;
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().HideMessage();
        AdmobManager.Instance.DisplayBannerAd();
        AudioManager.Instance.PlayButtonSound();
    }
}
