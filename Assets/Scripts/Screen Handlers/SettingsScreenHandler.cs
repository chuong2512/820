using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenHandler : MonoBehaviour
{
    public Sprite offToggleSprite;
    public Sprite onToggleSprite;
    public GameObject vibrationButton;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle easyModeToggle;
    public Toggle mediumModeToggle;
    public Toggle hardModeToggle;
    public Toggle fiftyPointsToggle;
    public Toggle hunderedPointsToggle;
    public Toggle oneFiftyPointsToggle;
    public Toggle dominoOneToggle;
    public Toggle dominoTwoToggle;

    private void OnEnable()
    {
        DisplayVibrationSprite();
        DisplayDifficultyModeToggle();
        DisplayWinningPointsToggle();
        DisplayDominoToggle();

        musicSlider.GetComponent<Slider>().value = PreferenceManager.Music;
        sfxSlider.GetComponent<Slider>().value = PreferenceManager.SFX;
        AdmobManager.Instance.DisplayBannerAd();
    }

    public void OnBackButtonClick()
    {
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnVibrationButtonClick()
    {
        if (PreferenceManager.Vibration == VibrationState.On.ToString())
        {
            PreferenceManager.Vibration = VibrationState.Off.ToString();
        }
        else if (PreferenceManager.Vibration == VibrationState.Off.ToString())
        {
            Handheld.Vibrate();
            PreferenceManager.Vibration = VibrationState.On.ToString();
        }

        DisplayVibrationSprite();
        AudioManager.Instance.PlayButtonSound();
    }

    private void DisplayVibrationSprite()
    {
        if (PreferenceManager.Vibration == VibrationState.Off.ToString())
            vibrationButton.GetComponent<Image>().sprite = offToggleSprite;
        else if (PreferenceManager.Vibration == VibrationState.On.ToString())
            vibrationButton.GetComponent<Image>().sprite = onToggleSprite;
    }

    public void OnMusicSliderValueChange()
    {
        SetMusicVolume(musicSlider.GetComponent<Slider>().value);
    }

    private void SetMusicVolume(float volume)
    {
        PreferenceManager.Music = volume;
        AudioManager.Instance.SetMusicVolume(PreferenceManager.Music);
    }

    public void OnSFXSliderValueChange()
    {
        SetSFXVolume(sfxSlider.GetComponent<Slider>().value);
    }

    private void SetSFXVolume(float volume)
    {
        PreferenceManager.SFX = volume;
        AudioManager.Instance.SetSFXVolume(PreferenceManager.SFX);
    }

    public void OnDifficultyModeToggleClick(string difficultyMode)
    {
        PreferenceManager.DifficultyMode = difficultyMode;
        DisplayDifficultyModeToggle();
        AudioManager.Instance.PlayButtonSound();
    }

    private void DisplayDifficultyModeToggle()
    {
        easyModeToggle.isOn = false;
        mediumModeToggle.isOn = false;
        hardModeToggle.isOn = false;

        if (PreferenceManager.DifficultyMode == DifficultyMode.Easy.ToString())
            easyModeToggle.isOn = true;
        else if (PreferenceManager.DifficultyMode == DifficultyMode.Medium.ToString())
            mediumModeToggle.isOn = true;
        else if (PreferenceManager.DifficultyMode == DifficultyMode.Hard.ToString())
            hardModeToggle.isOn = true;
    }

    public void OnWinningPointsToggleClick(int points)
    {
        PreferenceManager.WinningPoints = points;
        DisplayWinningPointsToggle();
        AudioManager.Instance.PlayButtonSound();
    }

    private void DisplayWinningPointsToggle()
    {
        fiftyPointsToggle.isOn = false;
        hunderedPointsToggle.isOn = false;
        oneFiftyPointsToggle.isOn = false;

        if (PreferenceManager.WinningPoints == 50)
            fiftyPointsToggle.isOn = true;
        else if (PreferenceManager.WinningPoints == 100)
            hunderedPointsToggle.isOn = true;
        else if (PreferenceManager.WinningPoints == 150)
            oneFiftyPointsToggle.isOn = true;
    }

    public void OnDominoToggleClick(int domino)
    {
        PreferenceManager.DominoSelected = domino;
        DisplayDominoToggle();
        AudioManager.Instance.PlayButtonSound();
    }

    private void DisplayDominoToggle()
    {
        dominoOneToggle.isOn = false;
        dominoTwoToggle.isOn = false;

        if (PreferenceManager.DominoSelected == 1)
            dominoOneToggle.isOn = true;
        else if (PreferenceManager.DominoSelected == 2)
            dominoTwoToggle.isOn = true;
    }
}

public enum VibrationState
{
    On,
    Off
}

public enum DifficultyMode
{
    Easy,
    Medium,
    Hard
}

