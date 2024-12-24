using UnityEngine;
using UnityEngine.UI;

public class ResultPlayer : MonoBehaviour
{
    public DynamicImage avatarImage;
    public Text playerNameText;
    public Text scoreText;
    public GameObject winnerImage;

    public void SetPlayerResult(Texture2D texture, string playerName, string score)
    {
        avatarImage.SetImage(texture);
        playerNameText.text = playerName;
        scoreText.text = score;
    }
}
