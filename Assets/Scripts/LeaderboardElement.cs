using UnityEngine;
using UnityEngine.UI;

public class LeaderboardElement : MonoBehaviour
{
    private string userID;
    public Text rankText;
    public DynamicImage playerImage;
    public Text userNameText;
    public Text winPercentageText;

    public void LeaderboardElementSetup(LeaderBoardUser user, int rank)
    {
        //Debug.Log(user.winPercentage);
        userID = user.userID;
        FetchProfilePictureFromDataBase();

        rankText.text = rank.ToString();
        userNameText.text = user.username.ToString();
        winPercentageText.text = user.winPercentage.ToString() + " %";
    }

    private void FetchProfilePictureFromDataBase()
    {
        FirebaseManager.Instance.GetAvatarPicture(userID, OnGetAvatarPictureComplete);
    }

    public void OnGetAvatarPictureComplete(Texture2D texture, AvatarResult result)
    {
        if (result == AvatarResult.Successfull)
        {
            Debug.Log("Avatar picture fetched Successfully!");
            playerImage.SetImage(texture);
        }
        else
        {
            Debug.Log("Error! Avatar picture not fetched");
            FirebaseManager.Instance.GetAvatarPicture(userID, OnGetAvatarPictureComplete);
        }
    }

    public void OnProfileButtonClick()
    {
        UIManager.Instance.UIScreensReferences[GameScreens.LeaderboardScreen].GetComponent<LeaderboardScreenHandler>().FetchUserProfile(userID);   
    }
}
