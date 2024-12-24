using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineGamePlayScreenHandler : MonoBehaviour
{
    private PhotonView PV;

    [Header("Leave Game Panel")]
    public GameObject LeaveGamePanel;

    [Header("Room Key Panel")]
    public GameObject RoomKeyPanel;
    public Text roomKeyText;

    [Header("Profile Panel")]
    public GameObject ProfilePanel;
    public DynamicImage profileAvatarImage;
    public Text usernameText;
    public Text emailText;
    public Text totalGamesPlayedText;
    public Text winsText;
    public Text lossesText;
    public Text winPercentageText;

    [Header("Avatar Panel")]
    public Animator avatarPictureAnimator;
    public DynamicImage avatarImage;

    [Header("Result Panel")]
    public GameObject ResultPanel;
    public ResultPlayer player1;
    public ResultPlayer player2;
    public ResultPlayer player3;
    public ResultPlayer player4;

    [Header("Message")]
    public Animator messageAnimatior;
    public Image messageIcon;
    public Sprite connectionLostSprite;
    public Sprite leftGameSprite;
    public Sprite noRecordSprite;
    public Text messageText;

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
        PV.ViewID = 2;

        UIManager.Instance.DeactivateScreen(GameScreens.LoadingScreen);
        ProfilePanel.SetActive(false);
        RoomKeyPanel.SetActive(false);
        LeaveGamePanel.SetActive(false);
        ResultPanel.SetActive(false);

        if (!string.IsNullOrEmpty(PreferenceManager.Username))
            UpdateGamesPlayed();

        AdmobManager.Instance.HideBannerAd();
    }

    #region Offline Game Play Screen

    public void OnLeaveGameButtonClick()
    {
        LeaveGamePanel.SetActive(true);
        AudioManager.Instance.PlayButtonSound();
    }

    private void UpdateGamesPlayed()
    {
        PreferenceManager.TotalGamesPlayed++;
        FirebaseManager.Instance.UpdateTotalGamesPlayed(PreferenceManager.TotalGamesPlayed, OnUpdateTotalGamesPlayedComplete);
    }

    public void OnUpdateTotalGamesPlayedComplete(UpdateData _result)
    {
        if (_result == UpdateData.Successfull)
        {
            Debug.Log("Successfully! Total games played updated");
        }
        else
        {
            Debug.Log("Error! Couldn't updated total games played");
            FirebaseManager.Instance.UpdateTotalGamesPlayed(PreferenceManager.TotalGamesPlayed, OnUpdateTotalGamesPlayedComplete);
        }
    }

    public void OnDisconnected()
    {
        if (!ResultPanel.activeInHierarchy)
        {
            if (PhotonNetwork.NickName == Nickname.PLAYER1.ToString())
                DisplayResult(Nickname.PLAYER2.ToString());
            else if (PhotonNetwork.NickName == Nickname.PLAYER2.ToString())
                DisplayResult(Nickname.PLAYER1.ToString());

            DisplayConnectionLostMessage("Internet connection interrupted");
        }
    }

    #endregion





    #region Room Key Panel

    public void OnShareKeyButtonClick()
    {
        new NativeShare().SetSubject("ROOM KEY: ").SetText(PreferenceManager.RoomKey).Share();
        AudioManager.Instance.PlayButtonSound();
    }

    #endregion





    #region Leave Game Panel

    public void OnYesButtonClick()
    {
        StartCoroutine(LeaveGameCouroutine());
        AudioManager.Instance.PlayButtonSound();
    }

    private IEnumerator LeaveGameCouroutine()
    {
        PhotonNetwork.LeaveRoom();
        yield return new WaitWhile(() => PhotonNetwork.InRoom);
        OnLeaveRoom();
    }

    private void OnLeaveRoom()
    {
        OnlineDominoesGamePlayManager.Instance.ResetGame();
        DominoPositionsManager.Instance.ResetGame();
        SceneManager.LoadScene(0);
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
        GameManager.Instance.DisplayRegularAd.Invoke();
    }

    public void OnNoButtonClick()
    {
        LeaveGamePanel.SetActive(false);
        AudioManager.Instance.PlayButtonSound();
    }

    #endregion





    #region Profile

    public void DisplayUserProfile(User user)
    {
        ProfilePanel.SetActive(true);
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

    public void SetProfileAvatarImage(Texture2D teture)
    {
        profileAvatarImage.SetImage(teture);
        avatarImage.SetImage(teture);
    }

    public void OnCloseProfilePanelButtonClick()
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





    #region Result Panel

    public void DisplayResult(string winner)
    {
        ResultPanel.SetActive(true);
        if (OnlineDominoesGamePlayManager.Instance.numberOfPlayers == 2)
        {
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(false);
            player4.gameObject.SetActive(false);

            player1.scoreText.text = OnlineDominoesGamePlayManager.Instance.player1Points.ToString();
            player2.scoreText.text = OnlineDominoesGamePlayManager.Instance.player2Points.ToString();
        }
        else if (OnlineDominoesGamePlayManager.Instance.numberOfPlayers == 3)
        {
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(true);
            player4.gameObject.SetActive(false);

            player1.scoreText.text = OnlineDominoesGamePlayManager.Instance.player1Points.ToString();
            player2.scoreText.text = OnlineDominoesGamePlayManager.Instance.player2Points.ToString();
            player3.scoreText.text = OnlineDominoesGamePlayManager.Instance.player3Points.ToString();
        }
        else if (OnlineDominoesGamePlayManager.Instance.numberOfPlayers == 4)
        {
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(true);
            player4.gameObject.SetActive(true);

            player1.scoreText.text = OnlineDominoesGamePlayManager.Instance.player1Points.ToString();
            player2.scoreText.text = OnlineDominoesGamePlayManager.Instance.player2Points.ToString();
            player3.scoreText.text = OnlineDominoesGamePlayManager.Instance.player3Points.ToString();
            player4.scoreText.text = OnlineDominoesGamePlayManager.Instance.player4Points.ToString();
        }

        player1.winnerImage.SetActive(false);
        player2.winnerImage.SetActive(false);
        player3.winnerImage.SetActive(false);
        player4.winnerImage.SetActive(false);

        player1.scoreText.color = Color.white;
        player2.scoreText.color = Color.white;
        player3.scoreText.color = Color.white;
        player4.scoreText.color = Color.white;

        if (winner == Nickname.PLAYER1.ToString())
        {
            player1.winnerImage.SetActive(true);
            player1.scoreText.color = OnlineDominoesGamePlayManager.Instance.blueColor;
        }
        else if (winner == Nickname.PLAYER2.ToString())
        {
            player2.winnerImage.SetActive(true);
            player2.scoreText.color = OnlineDominoesGamePlayManager.Instance.blueColor;
        }
        else if (winner == Nickname.PLAYER3.ToString())
        {
            player3.winnerImage.SetActive(true);
            player3.scoreText.color = OnlineDominoesGamePlayManager.Instance.blueColor;
        }
        else if (winner == Nickname.PLAYER4.ToString())
        {
            player4.winnerImage.SetActive(true);
            player4.scoreText.color = OnlineDominoesGamePlayManager.Instance.blueColor;
        }

        if (winner == PhotonNetwork.NickName && !string.IsNullOrEmpty(PreferenceManager.Username))
            UpdateWins();
    }

    private void UpdateWins()
    {
        PreferenceManager.Wins++;
        FirebaseManager.Instance.UpdateWins(PreferenceManager.Wins, OnUpdateWinsComplete);
    }

    public void OnUpdateWinsComplete(UpdateData _result)
    {
        if (_result == UpdateData.Successfull)
        {
            Debug.Log("Successfully! Wins updated");
        }
        else
        {
            Debug.Log("Error! Couldn't updated wins");
            FirebaseManager.Instance.UpdateWins(PreferenceManager.Wins, OnUpdateWinsComplete);
        }
    }

    public void OnHomeButtonClick()
    {
        StartCoroutine(LeaveGameCouroutine());
        AudioManager.Instance.PlayButtonSound();
    }

    #endregion





    #region Message

    public void DisplayConnectionLostMessage(string message)
    {
        messageAnimatior.ResetTrigger("hideMessage");
        messageAnimatior.SetTrigger("displayMessage");
        messageIcon.GetComponent<Image>().sprite = connectionLostSprite;
        messageText.text = message;
        Invoke(nameof(HideMessage), 5f);
    }

    public void DisplayOpponentLeftTheGameMessage(string message)
    {
        messageAnimatior.ResetTrigger("hideMessage");
        messageAnimatior.SetTrigger("displayMessage");
        messageIcon.GetComponent<Image>().sprite = leftGameSprite;
        messageText.text = message;
        Invoke(nameof(HideMessage), 5f);
    }

    public void DisplayNoRecordFoundMessage(string message)
    {
        messageAnimatior.ResetTrigger("hideMessage");
        messageAnimatior.SetTrigger("displayMessage");
        messageIcon.GetComponent<Image>().sprite = noRecordSprite;
        messageText.text = message;
        Invoke(nameof(HideMessage), 2f);
    }

    public void HideMessage()
    {
        messageAnimatior.ResetTrigger("displayMessage");
        messageAnimatior.SetTrigger("hideMessage");
    }

    #endregion
}
