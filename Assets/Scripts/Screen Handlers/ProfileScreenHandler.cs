
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreenHandler : MonoBehaviour
{
    [Header("Profile Panel")]
    public DynamicImage profileAvatarImage;
    public Texture2D defaultAvatarTexture;
    public Text usernameText;
    public Text emailText;
    public Text totalGamesPlayedText;
    public Text winsText;
    public Text lossesText;
    public Text winPercentageText;

    [Header("Avatar Panel")]
    public Animator avatarPictureAnimator;
    public DynamicImage avatarImage;

    private void OnEnable()
    {
        AdmobManager.Instance.HideBannerAd();
    }
}
