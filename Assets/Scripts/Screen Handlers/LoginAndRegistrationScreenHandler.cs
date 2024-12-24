
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginAndRegistrationScreenHandler : MonoBehaviour
{
    [Header("Login")]
    public GameObject LoginPanel;
    public InputField emailLoginInputField;
    public InputField passwordLoginInputField;

    [Header("Registration")]
    public GameObject RegistrationPanel;
    public Texture2D defaultAvatarTexture;
    public InputField usernameRegistrationInputField;
    public InputField emailRegistrationInputField;
    public InputField passwordRegistrationInputField;
    public InputField confirmPasswordRegistrationInputField;

    [Header("Message")]
    public Animator messageAnimatior;
    public Image messageIcon;
    public Sprite errorSprite;
    public Sprite successSprite;
    public Text messageText;

    private void OnEnable()
    {
        LoginPanel.SetActive(true);
        RegistrationPanel.SetActive(false);
        HideMessage();
        AdmobManager.Instance.HideBannerAd();
    }

    public void OnCloseButtonClick()
    {
        UIManager.Instance.DeactivateScreen(GameScreens.LoginAndRegistrationScreen);
        AdmobManager.Instance.DisplayBannerAd();
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnDisplayLoginPanelButtonClick()
    {
        LoginPanel.SetActive(true);
        RegistrationPanel.SetActive(false);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnDisplayRegistrationPanelButtonClick()
    {
        LoginPanel.SetActive(false);
        RegistrationPanel.SetActive(true);
        AudioManager.Instance.PlayButtonSound();
    }

    public void InputFieldValueChange()
    {
        HideMessage();
    }


    





    #region Message

    public void DisplayErrorMessage(string message)
    {
        messageAnimatior.ResetTrigger("hideMessage");
        messageAnimatior.SetTrigger("displayMessage");
        messageIcon.GetComponent<Image>().sprite = errorSprite;
        messageText.text = message;
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 4f);
    }

    public void DisplaySuccessMessage(string message)
    {
        messageAnimatior.ResetTrigger("hideMessage");
        messageAnimatior.SetTrigger("displayMessage");
        messageIcon.GetComponent<Image>().sprite = successSprite;
        messageText.text = message;
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 4f);
    }

    public void HideMessage()
    {
        messageAnimatior.ResetTrigger("displayMessage");
        messageAnimatior.SetTrigger("hideMessage");
    }

    #endregion
}
