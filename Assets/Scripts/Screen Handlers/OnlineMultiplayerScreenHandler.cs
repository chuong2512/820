using UnityEngine;
using UnityEngine.UI;

public class OnlineMultiplayerScreenHandler : MonoBehaviour
{
    [Header("Online Multiplayer Screen")]
    public GameObject createPrivateRoomPanel;
    public GameObject createPublicRoomPanel;
    public GameObject joinPrivateRoomPanel;
    public Transform publicRoomsContent;
    public GameObject roomsMessageText;

    [Header("Message")]
    public Animator messageAnimatior;
    public Image messageIcon;
    public Sprite errorSprite;
    public Sprite successSprite;
    public Text messageText;

    private void OnEnable()
    {
        createPrivateRoomPanel.SetActive(false);
        createPublicRoomPanel.SetActive(false);
        joinPrivateRoomPanel.SetActive(false);
        AdmobManager.Instance.DisplayBannerAd();
    }

    private void Update()
    {
        if (RoomListingManager.Instance.roomsList.Count == 0)
            roomsMessageText.SetActive(true);
        else
            roomsMessageText.SetActive(false);
    }

    public void OnBackButtonClick()
    {
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnCreatePrivateRoomButtonClick()
    {
        createPrivateRoomPanel.SetActive(true);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnJoinPrivateRoomButtonClick()
    {
        joinPrivateRoomPanel.SetActive(true);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnCreatePublicRoomButtonClick()
    {
        createPublicRoomPanel.SetActive(true);
        AudioManager.Instance.PlayButtonSound();
    }




    #region Message

    public void DisplayErrorMessage(string message)
    {
        AdmobManager.Instance.HideBannerAd();
        messageAnimatior.ResetTrigger("hideMessage");
        messageAnimatior.SetTrigger("displayMessage");
        messageIcon.GetComponent<Image>().sprite = errorSprite;
        messageText.text = message;
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 4f);
    }

    public void DisplaySuccessMessage(string message)
    {
        AdmobManager.Instance.HideBannerAd();
        messageAnimatior.ResetTrigger("hideMessage");
        messageAnimatior.SetTrigger("displayMessage");
        messageIcon.GetComponent<Image>().sprite = successSprite;
        messageText.text = message;
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 4f);
    }

    public void HideMessage()
    {
        AdmobManager.Instance.DisplayBannerAd();
        messageAnimatior.ResetTrigger("displayMessage");
        messageAnimatior.SetTrigger("hideMessage");
    }

    #endregion
}


