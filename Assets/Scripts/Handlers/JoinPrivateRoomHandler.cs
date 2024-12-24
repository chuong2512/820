using UnityEngine;
using UnityEngine.UI;

public class JoinPrivateRoomHandler : MonoBehaviour
{
    public InputField roomKeyInputTextField;

    private void OnEnable()
    {
        roomKeyInputTextField.text = null;
    }

    public void OnInputFieldValueChanged()
    {
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().HideMessage();
    }

    public void OnJoinPrivateRoomButtonClick()
    {
        if (string.IsNullOrEmpty(roomKeyInputTextField.text))
        {
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage("Enter room key");
        }
        else
        {
            string tableNumber = roomKeyInputTextField.text;
            NetworkManager.Instance.JoinPrivateRoom(tableNumber);
        }
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().HideMessage();
        AdmobManager.Instance.DisplayBannerAd();
        AudioManager.Instance.PlayButtonSound();
    }
}
