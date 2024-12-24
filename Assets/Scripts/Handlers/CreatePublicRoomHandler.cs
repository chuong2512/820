using UnityEngine;
using UnityEngine.UI;

public class CreatePublicRoomHandler : MonoBehaviour
{
    public Toggle fiftyTogglePrivateRoom;
    public Toggle hundredTogglePrivateRoom;
    public Toggle oneFiftyTogglePrivateRoom;

    private void OnEnable()
    {
        SetWinningScore(PreferenceManager.WinningPoints);
    }
    public void OnSelectWinningScoreToggleClick(int winningScore)
    {
        SetWinningScore(winningScore);
        AudioManager.Instance.PlayButtonSound();
    }

    private void SetWinningScore(int winningPoints)
    {
        PreferenceManager.WinningPoints = winningPoints;

        fiftyTogglePrivateRoom.isOn = false;
        hundredTogglePrivateRoom.isOn = false;
        oneFiftyTogglePrivateRoom.isOn = false;

        if (winningPoints == 50)
            fiftyTogglePrivateRoom.isOn = true;
        else if (winningPoints == 100)
            hundredTogglePrivateRoom.isOn = true;
        else if (winningPoints == 150)
            oneFiftyTogglePrivateRoom.isOn = true;
    }

    public void OnCreatePublicRoomButtonClick()
    {
        NetworkManager.Instance.CreatePublicRoom(GenerateAutoKey(), 2);
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().HideMessage();
        AdmobManager.Instance.DisplayBannerAd();
        AudioManager.Instance.PlayButtonSound();
    }

    private string GenerateAutoKey()
    {
        string[] alphabets = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        string randomAlphabet1 = alphabets[Random.Range(0, alphabets.Length)];
        string randomAlphabet2 = alphabets[Random.Range(0, alphabets.Length)];
        int randomDigitNumber1 = Random.Range(10, 1000);
        int randomDigitNumber2 = Random.Range(10, 1000);

        string roomKey = randomDigitNumber1 + randomAlphabet1 + randomAlphabet2 + "room" + randomDigitNumber2;
        return roomKey;
    }
}
