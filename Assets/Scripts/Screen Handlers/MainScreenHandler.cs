
using UnityEngine;
using UnityEngine.UI;

public class MainScreenHandler : MonoBehaviour
{
    [Header("HUD")]
    public GameObject profileButton;
    public GameObject loginButton;
    public Button leaderboardButton;
    public DynamicImage avatarImage;

    [Header("Message")]
    public Animator messageAnimatior;
    public Image messageIcon;
    public Sprite errorSprite;
    public Sprite successSprite;
    public Text messageText;

    private void OnEnable()
    {
        GameManager.Instance.NumberOfPlayers = null;
        GameManager.Instance.GameType = null;
        SetHUD();
        AdmobManager.Instance.DisplayBannerAd();
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(PreferenceManager.Username))
            Invoke(nameof(FetchProfilePicture), 1f);
    }

    public void SetHUD()
    {
        if (string.IsNullOrEmpty(PreferenceManager.Username))
        {
            profileButton.SetActive(false);
            loginButton.SetActive(true);
            leaderboardButton.interactable = false;
        }
        else
        {
            //UIManager.Instance.UIScreensReferences[GameScreens.ProfileScreen].GetComponent<ProfileScreenHandler>().SetProfileFromCache();
            profileButton.transform.GetChild(1).GetComponent<Text>().text = PreferenceManager.Username.ToUpper();
            profileButton.SetActive(true);
            loginButton.SetActive(false);
            leaderboardButton.interactable = true;
        }
    }

    private void FetchProfilePicture()
    {
     }

    public void OnLoginButtonClick()
    {
        UIManager.Instance.ActivateScreen(GameScreens.LoginAndRegistrationScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnProfileButtonClick()
    {
        UIManager.Instance.ActivateScreen(GameScreens.ProfileScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnLeaderboardButtonClick()
    {
        UIManager.Instance.ActivateSpecificScreen(GameScreens.LeaderboardScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnHowToPlayButtonClick()
    {
        UIManager.Instance.ActivateScreen(GameScreens.RulesScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnPlayOfflineButtonClick()
    {
        GameManager.Instance.GameType = GameType.OfflinePlayerVsAI.ToString();
        UIManager.Instance.ActivateSpecificScreen(GameScreens.SelectGameTypeScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnPlayOnlineButtonClick()
    {
        if (GameManager.Instance.connectedToPhotonServer)
            UIManager.Instance.ActivateSpecificScreen(GameScreens.OnlineMultiplayerScreen);
        else
            DisplayErrorMessage("Internet connection interrupted");

        AudioManager.Instance.PlayButtonSound();
    }

    public void OnSettingsButtonClick()
    {
        UIManager.Instance.ActivateSpecificScreen(GameScreens.SettingsScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnQuitButtonClick()
    {
        UIManager.Instance.ActivateScreen(GameScreens.QuitScreen);
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
