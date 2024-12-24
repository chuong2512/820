
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardScreenHandler : MonoBehaviour
{
    [Header("Leaderboard")]
    public GameObject loadingImage;
    public LeaderboardElement myInfo;
    public Transform Content;
    public LeaderboardElement leaderboardElement;
    private List<LeaderboardElement> tempLeaderboardElementList =  new List<LeaderboardElement>();

    [Header("Profile")]
    public GameObject ProfilePanel;
    public DynamicImage profileAvatarImage;
    public Texture2D defaultAvatarTexture;
    public Text usernameText;
    public Text emailText;
    public Text totalGamesPlayedText;
    public Text winsText;
    public Text lossesText;
    public Text winPercentageText;

    [Header("Avatar")]
    public Animator avatarPictureAnimator;
    public DynamicImage avatarImage;

    [Header("Message")]
    public Animator messageAnimatior;
    public Image messageIcon;
    public Sprite errorSprite;
    public Sprite successSprite;
    public Text messageText;

    private void OnEnable()
    {
        myInfo.gameObject.SetActive(false);
        ProfilePanel.SetActive(false);

        for (int i = 0; i < tempLeaderboardElementList.Count; i++)
        {
            Destroy(tempLeaderboardElementList[i].gameObject);
        }

        if (GameManager.Instance.connectedToPhotonServer)
        {
            loadingImage.SetActive(true);
            FirebaseManager.Instance.GetUsers(OnGetUsersComplete);
            AdmobManager.Instance.DisplayBannerAd();
        }
        else
        {
            loadingImage.SetActive(false);
            DisplayErrorMessage("Internet connection interrupted");
        }
    }

    private void OnGetUsersComplete(List<LeaderBoardUser> userList, GetUsersResult _result)
    {
        if (_result == GetUsersResult.Successfull)
        {
            loadingImage.SetActive(false);
            RectTransform rt = Content.gameObject.GetComponent<RectTransform>();

            if (userList.Count <= 50)
                rt.sizeDelta = new Vector2(0, userList.Count * 90);
            else
                rt.sizeDelta = new Vector2(0, 50 * 90);


            foreach (LeaderBoardUser user in userList)
            {
                if (user.totalGamesPlayed != 0)
                {
                    float winPercentage = (float)user.wins / (float)user.totalGamesPlayed;
                    winPercentage *= 100;
                    user.winPercentage = System.Convert.ToInt32(winPercentage);
                }
                else
                {
                    user.winPercentage = 0;
                }
            }

            userList = userList.OrderByDescending(x => x.winPercentage).ToList();
            tempLeaderboardElementList = new List<LeaderboardElement>(userList.Count);

            foreach (LeaderBoardUser user in userList)
            {
                if (userList.IndexOf(user) < 50)
                {
                    LeaderboardElement tempLeaderboardElement = Instantiate(leaderboardElement);
                    tempLeaderboardElement.transform.SetParent(Content);
                    tempLeaderboardElement.transform.LeanScale(new Vector3(1, 1, 1), 0f);
                    tempLeaderboardElementList.Add(tempLeaderboardElement);

                    tempLeaderboardElement.LeaderboardElementSetup(user, (userList.IndexOf(user) + 1));
                }
            }
        }
        else
        {
            FirebaseManager.Instance.GetUsers(OnGetUsersComplete);
        }
    }

    public void OnBackButtonClick()
    {
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
        AudioManager.Instance.PlayButtonSound();
    }





    #region Profile

    public void FetchUserProfile(string userID)
    {
        UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);
        profileAvatarImage.GetComponent<DynamicImage>().SetImage(defaultAvatarTexture);
        avatarImage.GetComponent<DynamicImage>().SetImage(defaultAvatarTexture);
        FirebaseManager.Instance.GetUser(userID, OnGetProfileComplete);
        FirebaseManager.Instance.GetAvatarPicture(userID, OnGetAvatarPictureComplete);
    }

    private void OnGetProfileComplete(User user, ReadUserResult result)
    {
        if (result == ReadUserResult.Successfull)
        {
            UIManager.Instance.DeactivateScreen(GameScreens.LoadingScreen);
            ProfilePanel.SetActive(true);
            SetUserProfile(user);
        }
        else
        {
            DisplayErrorMessage("Error, try again");
        }
    }

    private void SetUserProfile(User user)
    {
        usernameText.text = user.username.ToUpper();
        emailText.text = user.email.ToString();
        totalGamesPlayedText.text = user.totalGamesPlayed.ToString();
        winsText.text = user.wins.ToString();

        int losses = user.totalGamesPlayed - user.wins;
        lossesText.text = losses.ToString();

        if (user.totalGamesPlayed != 0)
        {
            float winPercentage = (float)user.wins / (float)user.totalGamesPlayed;
            winPercentage *= 100;
            winPercentageText.text = System.Convert.ToInt32(winPercentage).ToString() + "%";
        }
        else
        {
            winPercentageText.text = "0%";
        }
    }

    public void OnGetAvatarPictureComplete(Texture2D texture, AvatarResult result)
    {
        if (result == AvatarResult.Successfull)
        {
            Debug.Log("Avatar picture fetched Successfully!");
            profileAvatarImage.GetComponent<DynamicImage>().SetImage(texture);
            avatarImage.GetComponent<DynamicImage>().SetImage(texture);
        }
    }

    public void OnCloseButtonClick()
    {
        ProfilePanel.SetActive(false);
        AudioManager.Instance.PlayButtonSound();
    }

    #endregion





    #region Avatar

    public void OnAvatarButtonClick()
    {
        avatarPictureAnimator.ResetTrigger("backward");
        avatarPictureAnimator.SetTrigger("forward");
    }

    public void OnCloseAvatarPanelButtonClick()
    {
        avatarPictureAnimator.ResetTrigger("forward");
        avatarPictureAnimator.SetTrigger("backward");
    }

    #endregion





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
