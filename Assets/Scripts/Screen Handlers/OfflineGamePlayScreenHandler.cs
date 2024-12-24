using UnityEngine;

public class OfflineGamePlayScreenHandler : MonoBehaviour
{
    [Header("Leave Game Panel")]
    public GameObject LeaveGamePanel;

    [Header("Result Panel")]
    public GameObject ResultPanel;
    public ResultPlayer player1;
    public ResultPlayer player2;
    public ResultPlayer player3;
    public ResultPlayer player4;

    private void OnEnable()
    {
        LeaveGamePanel.SetActive(false);
        ResultPanel.SetActive(false);

        if (!string.IsNullOrEmpty(PreferenceManager.Username))
            UpdateGamesPlayed();

        AdmobManager.Instance.HideBannerAd();
    }

    #region Offline Game Play Screen

    public void OnLeaveGameButtonClick()
    {
        Invoke(nameof(PauseGame), 0.5f);
        LeaveGamePanel.SetActive(true);
        AudioManager.Instance.PlayButtonSound();
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
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

    #endregion





    #region Leave Game Panel

    public void OnYesButtonClick()
    {
        Time.timeScale = 1;
        OfflineDominoesGamePlayManager.Instance.ResetGame();
        DominoPositionsManager.Instance.ResetGame();
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
        GameManager.Instance.DisplayRegularAd.Invoke();
        AudioManager.Instance.PlayButtonSound();
    }

    public void OnNoButtonClick()
    {
        Time.timeScale = 1;
        LeaveGamePanel.SetActive(false);
        AudioManager.Instance.PlayButtonSound();
    }

    #endregion





    #region Result Panel

    public void DisplayResult(string winner)
    {
        ResultPanel.SetActive(true);
        if (OfflineDominoesGamePlayManager.Instance.numberOfPlayers == 2)
        {
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(false);
            player4.gameObject.SetActive(false);

            player1.scoreText.text = OfflineDominoesGamePlayManager.Instance.player1Points.ToString();
            player2.scoreText.text = OfflineDominoesGamePlayManager.Instance.player2Points.ToString();
        }
        else if (OfflineDominoesGamePlayManager.Instance.numberOfPlayers == 3)
        {
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(true);
            player4.gameObject.SetActive(false);

            player1.scoreText.text = OfflineDominoesGamePlayManager.Instance.player1Points.ToString();
            player2.scoreText.text = OfflineDominoesGamePlayManager.Instance.player2Points.ToString();
            player3.scoreText.text = OfflineDominoesGamePlayManager.Instance.player3Points.ToString();
        }
        else if (OfflineDominoesGamePlayManager.Instance.numberOfPlayers == 4)
        {
            player1.gameObject.SetActive(true);
            player2.gameObject.SetActive(true);
            player3.gameObject.SetActive(true);
            player4.gameObject.SetActive(true);

            player1.scoreText.text = OfflineDominoesGamePlayManager.Instance.player1Points.ToString();
            player2.scoreText.text = OfflineDominoesGamePlayManager.Instance.player2Points.ToString();
            player3.scoreText.text = OfflineDominoesGamePlayManager.Instance.player3Points.ToString();
            player4.scoreText.text = OfflineDominoesGamePlayManager.Instance.player4Points.ToString();
        }

        player1.winnerImage.SetActive(false);
        player2.winnerImage.SetActive(false);
        player3.winnerImage.SetActive(false);
        player4.winnerImage.SetActive(false);

        player1.scoreText.color = Color.white;
        player2.scoreText.color = Color.white;
        player3.scoreText.color = Color.white;
        player4.scoreText.color = Color.white;

        if (winner == Player.Player1.ToString())
        {
            player1.winnerImage.SetActive(true);
            player1.scoreText.color = OfflineDominoesGamePlayManager.Instance.turnColor;
        }
        else if (winner == Player.Player2.ToString())
        {
            player2.winnerImage.SetActive(true);
            player2.scoreText.color = OfflineDominoesGamePlayManager.Instance.turnColor;
        }
        else if (winner == Player.Player3.ToString())
        {
            player3.winnerImage.SetActive(true);
            player3.scoreText.color = OfflineDominoesGamePlayManager.Instance.turnColor;
        }
        else if (winner == Player.Player4.ToString())
        {
            player4.winnerImage.SetActive(true);
            player4.scoreText.color = OfflineDominoesGamePlayManager.Instance.turnColor;
        }

        if (winner == Player.Player1.ToString() && !string.IsNullOrEmpty(PreferenceManager.Username))
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
        OfflineDominoesGamePlayManager.Instance.ResetGame();
        DominoPositionsManager.Instance.ResetGame();
        UIManager.Instance.ActivateSpecificScreen(GameScreens.MainScreen);
        AudioManager.Instance.PlayButtonSound();
    }

    #endregion
}
