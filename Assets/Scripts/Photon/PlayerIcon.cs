using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    public string nickName;
    public DynamicImage avatarImage;
    public GameObject timerImage;
    public Text playerNameText;

    public void OnDisplayProfileButtonClick()
    {
        if (nickName == Nickname.PLAYER1.ToString())
        {
            if (!string.IsNullOrEmpty(GameSetup.Instance.Player1DataBaseID))
            {
                FirebaseManager.Instance.GetUser(GameSetup.Instance.Player1DataBaseID, OnGetMyProfileComplete);
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().SetProfileAvatarImage(avatarImage.GetImage());
                UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);
            }
            else
            {
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayNoRecordFoundMessage("No Stats Found");
            }
        }
        else if (nickName == Nickname.PLAYER2.ToString())
        {
            if (!string.IsNullOrEmpty(GameSetup.Instance.Player2DataBaseID))
            {
                FirebaseManager.Instance.GetUser(GameSetup.Instance.Player2DataBaseID, OnGetMyProfileComplete);
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().SetProfileAvatarImage(avatarImage.GetImage());
                UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);
            }
            else
            {
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayNoRecordFoundMessage("No Stats Found");
            }
        }
        else if (nickName == Nickname.PLAYER3.ToString())
        {
            if (!string.IsNullOrEmpty(GameSetup.Instance.Player3DataBaseID))
            {
                FirebaseManager.Instance.GetUser(GameSetup.Instance.Player3DataBaseID, OnGetMyProfileComplete);
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().SetProfileAvatarImage(avatarImage.GetImage());
                UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);
            }
            else
            {
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayNoRecordFoundMessage("No Stats Found");
            }
        }
        else if (nickName == Nickname.PLAYER4.ToString())
        {
            if (!string.IsNullOrEmpty(GameSetup.Instance.Player4DataBaseID))
            {
                FirebaseManager.Instance.GetUser(GameSetup.Instance.Player4DataBaseID, OnGetMyProfileComplete);
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().SetProfileAvatarImage(avatarImage.GetImage());
                UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);
            }
            else
            {
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayNoRecordFoundMessage("No Stats Found");
            }
        }
    }

    private void OnGetMyProfileComplete(User user, ReadUserResult result)
    {
        UIManager.Instance.DeactivateScreen(GameScreens.LoadingScreen);
        if (result == ReadUserResult.Successfull)
        {
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayUserProfile(user);
        }
    }
}
