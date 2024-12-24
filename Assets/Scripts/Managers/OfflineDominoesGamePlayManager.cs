using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Version3;

[System.Serializable]
public class Domino
{
    public string name;
    public bool isAddedToDominoesDeck;
    public GameObject domino;
    public int rightDots;
    public int leftDots;
    public string connected = null;
}

public class OfflineDominoesGamePlayManager : MonoBehaviour
{
    [Header("Heads Up Display")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    [Header("Game Play Manager")]
    private const int DominoWidth = 65;
    private const int DominoHeight = 130;
    private int WinningPoints;
    public string player2Name;
    public string player3Name;
    public string player4Name;
    [Range(0.1f, 1f)]
    public float dominoAnimationSpeed;
    public int numberOfPlayers;
    public int numberOnUp;
    public int numberOnDown;
    public int numberOnRight;
    public int numberOnLeft;
    public int player1Points;
    public int player2Points;
    public int player3Points;
    public int player4Points;
    private int movedForward = -1;
    private string turn = null;
    private string winner = null;

    public Domino[] Dominoes;
    public List<Domino> DominoesDeck;
    public List<Domino> player1Dominoes;
    public List<Domino> player2Dominoes;
    public List<Domino> player3Dominoes;
    public List<Domino> player4Dominoes;
    public List<Sprite> darkModeDominoType1Textures;
    public List<Sprite> darkModeDominoType2Textures;
    private Transform dominoGlobalParent;
    private Transform dominoLocalParent;

    public Domino centerDomino;
    public List<Domino> upDominoes;
    public List<Domino> downDominoes;
    public List<Domino> rightDominoes;
    public List<Domino> leftDominoes;
    public GameObject gamePlayButtons;

    [Header("Player Icons")]
    public PlayerIcon player1Icon;
    public PlayerIcon player2Icon;
    public PlayerIcon player3Icon;
    public PlayerIcon player4Icon;
    public Texture2D defaultAvatarTexture;
    public Texture2D player2Image;
    public Texture2D player3Image;
    public Texture2D player4Image;

    [Header("Positions")]
    public Transform dominoDeckPosition;
    public Transform[] player1DominoesPositions;
    public Transform[] player2DominoesPositions;
    public Transform[] player3DominoesPositions;
    public Transform[] player4DominoesPositions;

    public Color turnColor;

    [Header("Camera")]
    public Button cameraButton;
    public Sprite manualCameraSprite;
    public Sprite autoCameraSprite;
    private Camera MainCamera;
    private bool cameraFlag = false;

    public static OfflineDominoesGamePlayManager _instance;
    public static OfflineDominoesGamePlayManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        StartGame();
    }

    private void StartGame()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //SetCamera();
        MainCamera.gameObject.transform.position = new Vector3(360, 540, 0);
        dominoGlobalParent = GameObject.FindGameObjectWithTag("Global Parent").transform;
        dominoLocalParent = GameObject.FindGameObjectWithTag("Local Parent").transform;
        WinningPoints = PreferenceManager.WinningPoints;

        if (GameManager.Instance.NumberOfPlayers == NumberOfPlayers.TwoPlayersGame.ToString())
            numberOfPlayers = 2;
        else if (GameManager.Instance.NumberOfPlayers == NumberOfPlayers.ThreePlayersGame.ToString())
            numberOfPlayers = 3;
        else if (GameManager.Instance.NumberOfPlayers == NumberOfPlayers.FourPlayersGame.ToString())
            numberOfPlayers = 4;

        SetDominoesTexture();
        SetupPlayerNamesPicture();
        GenerateRandomDominoesDeck();

        if (numberOfPlayers == 2)
        {
            player1.SetActive(true);
            player2.SetActive(true);
            player3.SetActive(false);
            player4.SetActive(false);

            player1Icon.gameObject.SetActive(true);
            player2Icon.gameObject.SetActive(true);
            player3Icon.gameObject.SetActive(false);
            player4Icon.gameObject.SetActive(false);

            DistributeDominoesToPlayer1(7);
            DistributeDominoesToPlayer2(7);
        }
        else if (numberOfPlayers == 3)
        {
            player1.SetActive(true);
            player2.SetActive(true);
            player3.SetActive(true);
            player4.SetActive(false);

            player1Icon.gameObject.SetActive(true);
            player2Icon.gameObject.SetActive(true);
            player3Icon.gameObject.SetActive(true);
            player4Icon.gameObject.SetActive(false);

            DistributeDominoesToPlayer1(7);
            DistributeDominoesToPlayer2(7);
            DistributeDominoesToPlayer3(7);
        }
        else if (numberOfPlayers == 4)
        {
            player1.SetActive(true);
            player2.SetActive(true);
            player3.SetActive(true);
            player4.SetActive(true);

            player1Icon.gameObject.SetActive(true);
            player2Icon.gameObject.SetActive(true);
            player3Icon.gameObject.SetActive(true);
            player4Icon.gameObject.SetActive(true);

            DistributeDominoesToPlayer1(5);
            DistributeDominoesToPlayer2(5);
            DistributeDominoesToPlayer3(5);
            DistributeDominoesToPlayer4(5);
        }

        Invoke(nameof(MoveFirstDominoe), 1f);
    }

    private void SetDominoesTexture()
    {
        for (int i = 0; i < Dominoes.Length; i++)
        {
            if (PreferenceManager.DominoSelected == 1)
            {
                Dominoes[i].domino.transform.GetChild(1).GetComponent<Image>().sprite = darkModeDominoType1Textures[i];
                Dominoes[i].domino.transform.GetChild(0).GetComponent<Image>().sprite = darkModeDominoType1Textures[0];
            }
            else if (PreferenceManager.DominoSelected == 2)
            {
                Dominoes[i].domino.transform.GetChild(1).GetComponent<Image>().sprite = darkModeDominoType2Textures[i];
                Dominoes[i].domino.transform.GetChild(0).GetComponent<Image>().sprite = darkModeDominoType2Textures[0];
            }
        }
    }

    private void SetupPlayerNamesPicture()
    {
        if (GameManager.Instance.GameType == GameType.OfflinePlayerVsPlayer.ToString())
        {
            player1.transform.GetChild(0).GetComponent<Text>().text = "Player 1 :";
            player1Icon.playerNameText.text = "Player 1";
            GetComponent<OfflineGamePlayScreenHandler>().player1.playerNameText.text = "Player 1";
            player1Icon.avatarImage.SetImage(defaultAvatarTexture);
            GetComponent<OfflineGamePlayScreenHandler>().player1.avatarImage.SetImage(defaultAvatarTexture);
        }
        else if (GameManager.Instance.GameType == GameType.OfflinePlayerVsAI.ToString())
        {
            if (string.IsNullOrEmpty(PreferenceManager.Username))
            {
                player1.transform.GetChild(0).GetComponent<Text>().text = "Player :";
                player1Icon.playerNameText.text = "Player";
                GetComponent<OfflineGamePlayScreenHandler>().player1.playerNameText.text = "Player";
                player1Icon.avatarImage.SetImage(defaultAvatarTexture);
                GetComponent<OfflineGamePlayScreenHandler>().player1.avatarImage.SetImage(defaultAvatarTexture);
            }
            else
            {
                player1.transform.GetChild(0).GetComponent<Text>().text = PreferenceManager.Username + " :";
                player1Icon.playerNameText.text = PreferenceManager.Username;
                GetComponent<OfflineGamePlayScreenHandler>().player1.playerNameText.text = PreferenceManager.Username;
                player1Icon.avatarImage.SetImage(UIManager.Instance.UIScreensReferences[GameScreens.MainScreen].GetComponent<MainScreenHandler>().avatarImage.GetImage());
                GetComponent<OfflineGamePlayScreenHandler>().player1.avatarImage.SetImage(UIManager.Instance.UIScreensReferences[GameScreens.MainScreen].GetComponent<MainScreenHandler>().avatarImage.GetImage());
            }
        }

        if (GameManager.Instance.GameType == GameType.OfflinePlayerVsPlayer.ToString())
        {
            player2.transform.GetChild(0).GetComponent<Text>().text = "Player 2 :";
            player3.transform.GetChild(0).GetComponent<Text>().text = "Player 3 :";
            player4.transform.GetChild(0).GetComponent<Text>().text = "Player 4 :";

            player2Icon.playerNameText.text = "Player 2";
            player3Icon.playerNameText.text = "Player 3";
            player4Icon.playerNameText.text = "Player 4";

            GetComponent<OfflineGamePlayScreenHandler>().player2.playerNameText.text = "Player 2";
            GetComponent<OfflineGamePlayScreenHandler>().player3.playerNameText.text = "Player 3";
            GetComponent<OfflineGamePlayScreenHandler>().player4.playerNameText.text = "Player 4";

            player2Icon.avatarImage.SetImage(defaultAvatarTexture);
            player3Icon.avatarImage.SetImage(defaultAvatarTexture);
            player4Icon.avatarImage.SetImage(defaultAvatarTexture);

            GetComponent<OfflineGamePlayScreenHandler>().player2.avatarImage.SetImage(defaultAvatarTexture);
            GetComponent<OfflineGamePlayScreenHandler>().player3.avatarImage.SetImage(defaultAvatarTexture);
            GetComponent<OfflineGamePlayScreenHandler>().player4.avatarImage.SetImage(defaultAvatarTexture);
        }
        else if (GameManager.Instance.GameType == GameType.OfflinePlayerVsAI.ToString())
        {
            player2.transform.GetChild(0).GetComponent<Text>().text = player2Name + " :";
            player3.transform.GetChild(0).GetComponent<Text>().text = player3Name + " :";
            player4.transform.GetChild(0).GetComponent<Text>().text = player4Name + " :";

            player2Icon.playerNameText.text = player2Name;
            player3Icon.playerNameText.text = player3Name;
            player4Icon.playerNameText.text = player4Name;

            GetComponent<OfflineGamePlayScreenHandler>().player2.playerNameText.text = player2Name;
            GetComponent<OfflineGamePlayScreenHandler>().player3.playerNameText.text = player3Name;
            GetComponent<OfflineGamePlayScreenHandler>().player4.playerNameText.text = player4Name;

            player2Icon.avatarImage.SetImage(player2Image);
            player3Icon.avatarImage.SetImage(player3Image);
            player4Icon.avatarImage.SetImage(player4Image);

            GetComponent<OfflineGamePlayScreenHandler>().player2.avatarImage.SetImage(player2Image);
            GetComponent<OfflineGamePlayScreenHandler>().player3.avatarImage.SetImage(player3Image);
            GetComponent<OfflineGamePlayScreenHandler>().player4.avatarImage.SetImage(player4Image);
        }
    }

    private void GenerateRandomDominoesDeck()
    {
        int j = 0;
        while (j != 28)
        {
            int randomNumber = Random.Range(0, 28);
            if (!Dominoes[randomNumber].isAddedToDominoesDeck)
            {
                Dominoes[randomNumber].isAddedToDominoesDeck = true;
                DominoesDeck.Add(Dominoes[randomNumber]);
                j++;
            }
        }
    }

    private void DistributeDominoesToPlayer1(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player1Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);
            player1Dominoes[player1Dominoes.Count - 1].domino.LeanMove(player1DominoesPositions[player1Dominoes.Count - 1].position, dominoAnimationSpeed);
            player1Dominoes[player1Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
        }
    }

    private void DistributeDominoesToPlayer2(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player2Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);
            player2Dominoes[player2Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
            player2Dominoes[player2Dominoes.Count - 1].domino.LeanMove(player2DominoesPositions[player2Dominoes.Count - 1].position, dominoAnimationSpeed);
            player2Dominoes[player2Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
        }

    }

    private void DistributeDominoesToPlayer3(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player3Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);
            player3Dominoes[player3Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
            player3Dominoes[player3Dominoes.Count - 1].domino.LeanMove(player3DominoesPositions[player3Dominoes.Count - 1].position, dominoAnimationSpeed);
            player3Dominoes[player3Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
            player3Dominoes[player3Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
        }
    }

    private void DistributeDominoesToPlayer4(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player4Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);
            player4Dominoes[player4Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
            player4Dominoes[player4Dominoes.Count - 1].domino.LeanMove(player4DominoesPositions[player4Dominoes.Count - 1].position, dominoAnimationSpeed);
            player4Dominoes[player4Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
            player4Dominoes[player4Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
        }
    }

    private void MoveFirstDominoe()
    {
        int numberOfDominoes = 7;
        if (numberOfPlayers == 4)
            numberOfDominoes = 5;

        for (int i = 6; i > 0; i--)
        {
            for (int j = 0; j < numberOfDominoes; j++)
            {
                if (player1Dominoes[j].rightDots == i && player1Dominoes[j].leftDots == i)
                {
                    centerDomino = player1Dominoes[j];
                    player1Dominoes.Remove(player1Dominoes[j]);

                    if (i == 5)
                    {
                        player1Points += 10;
                        player1.transform.GetChild(1).GetComponent<Text>().text = player1Points.ToString();
                        AudioManager.Instance.PlayPointsSound();
                    }

                    turn = Player.Player1.ToString();
                    Invoke(nameof(GiveTurn), 2f);
                    break;
                }

                if (player2Dominoes[j].rightDots == i && player2Dominoes[j].leftDots == i)
                {
                    centerDomino = player2Dominoes[j];
                    player2Dominoes.Remove(player2Dominoes[j]);

                    if (i == 5)
                    {
                        player2Points += 10;
                        player2.transform.GetChild(1).GetComponent<Text>().text = player2Points.ToString();
                        AudioManager.Instance.PlayPointsSound();
                    }

                    turn = Player.Player2.ToString();
                    Invoke(nameof(GiveTurn), 2f);
                    break;
                }

                if (numberOfPlayers >= 3)
                {
                    if (player3Dominoes[j].rightDots == i && player3Dominoes[j].leftDots == i)
                    {
                        centerDomino = player3Dominoes[j];
                        player3Dominoes.Remove(player3Dominoes[j]);

                        if (i == 5)
                        {
                            player3Points += 10;
                            player3.transform.GetChild(1).GetComponent<Text>().text = player3Points.ToString();
                            AudioManager.Instance.PlayPointsSound();
                        }

                        turn = Player.Player3.ToString();
                        Invoke(nameof(GiveTurn), 2f);
                        break;
                    }
                }

                if (numberOfPlayers == 4)
                {
                    if (player4Dominoes[j].rightDots == i && player4Dominoes[j].leftDots == i)
                    {
                        centerDomino = player4Dominoes[j];
                        player4Dominoes.Remove(player4Dominoes[j]);

                        if (i == 5)
                        {
                            player4Points += 10;
                            player4.transform.GetChild(1).GetComponent<Text>().text = player4Points.ToString();
                            AudioManager.Instance.PlayPointsSound();
                        }

                        turn = Player.Player4.ToString();
                        Invoke(nameof(GiveTurn), 2f);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(centerDomino.name))
                break;
        }

        if (string.IsNullOrEmpty(centerDomino.name))
        {
            RoundCompleted();
        }
        else
        {
            centerDomino.domino.transform.SetParent(dominoGlobalParent);
            centerDomino.domino.transform.Find("Back Side").SetAsFirstSibling();
            centerDomino.domino.transform.LeanMove(DominoPositionsManager.Instance.centerReferencePoint.position, dominoAnimationSpeed);
            centerDomino.domino.transform.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
            centerDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);

            numberOnUp = centerDomino.rightDots;
            numberOnDown = centerDomino.rightDots;
            numberOnRight = centerDomino.rightDots;
            numberOnLeft = centerDomino.leftDots;

            Invoke(nameof(ArangeDominoes), 0.5f);
        }
    }

    private void GiveTurn()
    {
        if (numberOfPlayers == 2)
        {
            if (turn == Player.Player2.ToString())
                turn = Player.Player1.ToString();
            else if (turn == Player.Player1.ToString())
                turn = Player.Player2.ToString();
        }
        else if (numberOfPlayers == 3)
        {
            if (turn == Player.Player3.ToString())
                turn = Player.Player1.ToString();
            else if (turn == Player.Player1.ToString())
                turn = Player.Player2.ToString();
            else if (turn == Player.Player2.ToString())
                turn = Player.Player3.ToString();
        }
        else if (numberOfPlayers == 4)
        {
            if (turn == Player.Player3.ToString())
                turn = Player.Player1.ToString();
            else if (turn == Player.Player1.ToString())
                turn = Player.Player4.ToString();
            else if (turn == Player.Player4.ToString())
                turn = Player.Player2.ToString();
            else if (turn == Player.Player2.ToString())
                turn = Player.Player3.ToString();
        }

        DisplayTurnImage();

        if (turn != Player.Player1.ToString())
        {
            gamePlayButtons.SetActive(false);
            Invoke(nameof(CPUDecisionOnTurn), 1f);
        }
        else
        {
            CheckIfPlayer1NeedsDominoe();
            if(PreferenceManager.Vibration == VibrationState.On.ToString())
                Handheld.Vibrate();
        }
    }

    private void DisplayTurnImage()
    {
        player1Icon.timerImage.SetActive(false);
        player2Icon.timerImage.SetActive(false);
        player3Icon.timerImage.SetActive(false);
        player4Icon.timerImage.SetActive(false);

        if (turn == Player.Player1.ToString())
            player1Icon.timerImage.SetActive(true);
        else if (turn == Player.Player2.ToString())
            player2Icon.timerImage.SetActive(true);
        else if (turn == Player.Player3.ToString())
            player3Icon.timerImage.SetActive(true);
        else if (turn == Player.Player4.ToString())
            player4Icon.timerImage.SetActive(true);
    }

    private IEnumerator CameraHandlerCoroutine(string allignCamera)
    {
        yield return new WaitForSeconds(dominoAnimationSpeed);

        if (!cameraFlag && !string.IsNullOrEmpty(centerDomino.name))
        {
            List<float> xAxisList = new List<float>(0);
            List<float> yAxisList = new List<float>(0);

            xAxisList.Add(centerDomino.domino.transform.position.x);
            yAxisList.Add(centerDomino.domino.transform.position.y);

            if (allignCamera == "VERTICAL")
            {
                for (int i = 0; i < upDominoes.Count; i++)
                {
                    xAxisList.Add(upDominoes[i].domino.transform.position.x);
                    yAxisList.Add(upDominoes[i].domino.transform.position.y);
                }

                for (int i = 0; i < downDominoes.Count; i++)
                {
                    xAxisList.Add(downDominoes[i].domino.transform.position.x);
                    yAxisList.Add(downDominoes[i].domino.transform.position.y);
                }

                Vector2 verticalCenter = new Vector2(MainCamera.gameObject.transform.position.x, yAxisList.Sum() / yAxisList.Count);
                MainCamera.gameObject.LeanMove(verticalCenter, dominoAnimationSpeed);
                
                StartCoroutine(CameraHandlerCoroutine(null));
            }
            else if (allignCamera == "HORIZONTAL")
            {
                for (int i = 0; i < rightDominoes.Count; i++)
                {
                    xAxisList.Add(rightDominoes[i].domino.transform.position.x);
                    yAxisList.Add(rightDominoes[i].domino.transform.position.y);
                }

                for (int i = 0; i < leftDominoes.Count; i++)
                {
                    xAxisList.Add(leftDominoes[i].domino.transform.position.x);
                    yAxisList.Add(leftDominoes[i].domino.transform.position.y);
                }

                Vector2 horizontalCenter = new Vector2(xAxisList.Sum() / xAxisList.Count, MainCamera.gameObject.transform.position.y);
                MainCamera.gameObject.LeanMove(horizontalCenter, dominoAnimationSpeed);

                StartCoroutine(CameraHandlerCoroutine(null));
            }
            else if (allignCamera == "BOTH")
            {
                for (int i = 0; i < upDominoes.Count; i++)
                {
                    xAxisList.Add(upDominoes[i].domino.transform.position.x);
                    yAxisList.Add(upDominoes[i].domino.transform.position.y);
                }

                for (int i = 0; i < downDominoes.Count; i++)
                {
                    xAxisList.Add(downDominoes[i].domino.transform.position.x);
                    yAxisList.Add(downDominoes[i].domino.transform.position.y);
                }

                Vector2 verticalCenter = new Vector2(MainCamera.gameObject.transform.position.x, yAxisList.Sum() / yAxisList.Count);
                MainCamera.gameObject.LeanMove(verticalCenter, dominoAnimationSpeed);

                StartCoroutine(CameraHandlerCoroutine("HORIZONTAL"));
            }
            else if (allignCamera == null)
            {
                if ((rightDominoes.Count + leftDominoes.Count) == 3 && MainCamera.gameObject.transform.position.z > -250)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -200f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 4 && MainCamera.gameObject.transform.position.z > -400)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -400f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 5 && MainCamera.gameObject.transform.position.z > -600)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -600f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 6 && MainCamera.gameObject.transform.position.z > -750)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -750f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 7 && MainCamera.gameObject.transform.position.z > -950)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -950f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 8 && MainCamera.gameObject.transform.position.z > -1100)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1100f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 9 && MainCamera.gameObject.transform.position.z > -1300)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1300f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 10 && MainCamera.gameObject.transform.position.z > -1450)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1450f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 11 && MainCamera.gameObject.transform.position.z > -1650)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1650f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 12 && MainCamera.gameObject.transform.position.z > -1800)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1800f), dominoAnimationSpeed);
                else if ((rightDominoes.Count + leftDominoes.Count) == 13 && MainCamera.gameObject.transform.position.z > -2000)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -2000f), dominoAnimationSpeed);

                if ((upDominoes.Count + downDominoes.Count) == 7 && MainCamera.gameObject.transform.position.z > -250)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -250f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 8 && MainCamera.gameObject.transform.position.z > -400)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -400f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 9 && MainCamera.gameObject.transform.position.z > -550)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -550f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 10 && MainCamera.gameObject.transform.position.z > -700)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -700f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 11 && MainCamera.gameObject.transform.position.z > -850)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -850f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 12 && MainCamera.gameObject.transform.position.z > -1000)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1000f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 13 && MainCamera.gameObject.transform.position.z > -1150)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1150f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 14 && MainCamera.gameObject.transform.position.z > -1300)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1300f), dominoAnimationSpeed);
            }
        }
    }

    private void CheckIfPlayer1NeedsDominoe()
    {
        for (int i = 0; i < player1Dominoes.Count; i++)
        {
            if ((player1Dominoes[i].leftDots == numberOnDown || player1Dominoes[i].rightDots == numberOnDown) ||
                (player1Dominoes[i].leftDots == numberOnLeft || player1Dominoes[i].rightDots == numberOnLeft) ||
                (player1Dominoes[i].leftDots == numberOnUp || player1Dominoes[i].rightDots == numberOnUp) ||
                (player1Dominoes[i].leftDots == numberOnRight || player1Dominoes[i].rightDots == numberOnRight))
            {
                gamePlayButtons.SetActive(true);
                return;
            }
        }

        if (DominoesDeck.Count != 0)
        {
            if (player1Dominoes.Count <= 8)
            {
                player1Dominoes.Add(DominoesDeck[0]);
                DominoesDeck.Remove(DominoesDeck[0]);
                player1Dominoes[player1Dominoes.Count - 1].domino.LeanMove(player1DominoesPositions[player1Dominoes.Count - 1].position, dominoAnimationSpeed);
                player1Dominoes[player1Dominoes.Count - 1].domino.LeanRotate(player1DominoesPositions[player1Dominoes.Count - 1].eulerAngles, dominoAnimationSpeed);
                player1Dominoes[player1Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
                Invoke(nameof(ArangeDominoes), 1f);
                Invoke(nameof(CheckIfPlayer1NeedsDominoe), 2f);
            }
            else if (player1Dominoes.Count >= 9)
            {
                Invoke(nameof(GiveTurn), 2f);
            }
        }
        else if (DominoesDeck.Count == 0)
        {
            if (CheckDeadEnd())
            {
                RoundCompleted();
            }
            else
            {
                Invoke(nameof(GiveTurn), 2f);
            }
        }
    }

    private bool CheckDeadEnd()
    {
        //If graveyard is empty and no other player can play a dominoe it's a dead end 

        for (int i = 0; i < player1Dominoes.Count; i++)
        {
            if ((player1Dominoes[i].leftDots == numberOnDown || player1Dominoes[i].rightDots == numberOnDown) ||
                (player1Dominoes[i].leftDots == numberOnLeft || player1Dominoes[i].rightDots == numberOnLeft) ||
                (player1Dominoes[i].leftDots == numberOnUp || player1Dominoes[i].rightDots == numberOnUp) ||
                (player1Dominoes[i].leftDots == numberOnRight || player1Dominoes[i].rightDots == numberOnRight))
            {
                return false;
            }
        }

        for (int i = 0; i < player2Dominoes.Count; i++)
        {
            if ((player2Dominoes[i].leftDots == numberOnDown || player2Dominoes[i].rightDots == numberOnDown) ||
                (player2Dominoes[i].leftDots == numberOnLeft || player2Dominoes[i].rightDots == numberOnLeft) ||
                (player2Dominoes[i].leftDots == numberOnUp || player2Dominoes[i].rightDots == numberOnUp) ||
                (player2Dominoes[i].leftDots == numberOnRight || player2Dominoes[i].rightDots == numberOnRight))
            {
                return false;
            }
        }

        for (int i = 0; i < player3Dominoes.Count; i++)
        {
            if ((player3Dominoes[i].leftDots == numberOnDown || player3Dominoes[i].rightDots == numberOnDown) ||
                (player3Dominoes[i].leftDots == numberOnLeft || player3Dominoes[i].rightDots == numberOnLeft) ||
                (player3Dominoes[i].leftDots == numberOnUp || player3Dominoes[i].rightDots == numberOnUp) ||
                (player3Dominoes[i].leftDots == numberOnRight || player3Dominoes[i].rightDots == numberOnRight))
            {
                return false;
            }
        }

        for (int i = 0; i < player4Dominoes.Count; i++)
        {
            if ((player4Dominoes[i].leftDots == numberOnDown || player4Dominoes[i].rightDots == numberOnDown) ||
                (player4Dominoes[i].leftDots == numberOnLeft || player4Dominoes[i].rightDots == numberOnLeft) ||
                (player4Dominoes[i].leftDots == numberOnUp || player4Dominoes[i].rightDots == numberOnUp) ||
                (player4Dominoes[i].leftDots == numberOnRight || player4Dominoes[i].rightDots == numberOnRight))
            {
                return false;
            }
        }

        Debug.Log("Dead End, Round Completed");
        return true;
    }

    public void OnDominoButtonClick(int dominoNumber)
    {
        if (dominoNumber <= player1Dominoes.Count)
        {
            if (movedForward == dominoNumber)                                                                              //If pressed the same domino again
            {
                movedForward = -1;
                foreach (Domino singleDomino in player1Dominoes)
                    singleDomino.domino.LeanMove(player1DominoesPositions[player1Dominoes.IndexOf(singleDomino)].position, 0.2f);

                DominoPositionsManager.Instance.ResetGame();
            }
            else                                                                                                           //Pressed some other domino
            {
                movedForward = dominoNumber;

                //Move back all the player dominoes to original position
                foreach (Domino singleDomino in player1Dominoes)
                {
                    singleDomino.domino.LeanMove(player1DominoesPositions[player1Dominoes.IndexOf(singleDomino)].position, 0.2f);
                }

                GameObject domino = player1Dominoes[dominoNumber - 1].domino;
                domino.LeanMove(new Vector3(domino.transform.position.x, domino.transform.position.y + 10f, domino.transform.position.z), 0.2f);
                DisplayConnectionTargets(dominoNumber);
            }
        }
    }

    private void DisplayConnectionTargets(int dominoNumber)
    {
        float yUpAxis = DominoPositionsManager.Instance.centerReferencePoint.position.y + 90f;
        for (int i = 0; i < upDominoes.Count; i++)
        {
            if (upDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                yUpAxis += DominoWidth;
            else if (upDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                yUpAxis += DominoHeight;
        }
        DominoPositionsManager.Instance.upTargetDomino.position = new Vector3(DominoPositionsManager.Instance.upTargetDomino.position.x, yUpAxis, DominoPositionsManager.Instance.upTargetDomino.position.z);
        DominoPositionsManager.Instance.UpTarget.LeanMove(DominoPositionsManager.Instance.upTargetDomino.position, 0f);
        DominoPositionsManager.Instance.UpTarget.SetActive(true);


        float yDownAxis = DominoPositionsManager.Instance.centerReferencePoint.position.y - 90f;
        for (int i = 0; i < downDominoes.Count; i++)
        {
            if (downDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                yDownAxis -= DominoWidth;
            else if (downDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                yDownAxis -= DominoHeight;
        }
        DominoPositionsManager.Instance.downTargetDomino.position = new Vector3(DominoPositionsManager.Instance.downTargetDomino.position.x, yDownAxis, DominoPositionsManager.Instance.upTargetDomino.position.z);
        DominoPositionsManager.Instance.DownTarget.LeanMove(DominoPositionsManager.Instance.downTargetDomino.position, 0f);
        DominoPositionsManager.Instance.DownTarget.SetActive(true);


        float xRightAxis = DominoPositionsManager.Instance.centerReferencePoint.position.x + 120f;
        for (int i = 0; i < rightDominoes.Count; i++)
        {
            if (rightDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                xRightAxis += DominoHeight;
            else if (rightDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                xRightAxis += DominoWidth;
        }
        DominoPositionsManager.Instance.rightTargetDomino.position = new Vector3(xRightAxis, DominoPositionsManager.Instance.rightTargetDomino.position.y, DominoPositionsManager.Instance.upTargetDomino.position.z);
        DominoPositionsManager.Instance.RightTarget.LeanMove(DominoPositionsManager.Instance.rightTargetDomino.position, 0f);
        DominoPositionsManager.Instance.RightTarget.SetActive(true);


        float xLeftAxis = DominoPositionsManager.Instance.centerReferencePoint.position.x - 120f;
        for (int i = 0; i < leftDominoes.Count; i++)
        {
            if (leftDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                xLeftAxis -= DominoHeight;
            else if (leftDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                xLeftAxis -= DominoWidth;
        }
        DominoPositionsManager.Instance.leftTargetDomino.position = new Vector3(xLeftAxis, DominoPositionsManager.Instance.leftTargetDomino.position.y, DominoPositionsManager.Instance.upTargetDomino.position.z);
        DominoPositionsManager.Instance.LeftTarget.LeanMove(DominoPositionsManager.Instance.leftTargetDomino.position, 0f);
        DominoPositionsManager.Instance.LeftTarget.SetActive(true);


        if (player1Dominoes[dominoNumber - 1].rightDots == numberOnUp || player1Dominoes[dominoNumber - 1].leftDots == numberOnUp)
        {
            if (PreferenceManager.DifficultyMode == DifficultyMode.Easy.ToString() || PreferenceManager.DifficultyMode == DifficultyMode.Medium.ToString())
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = turnColor;
            }
            else if (PreferenceManager.DifficultyMode == DifficultyMode.Hard.ToString())
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = false;
            }
            DisplayPoints(dominoNumber);
        }
        else
        {
            DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = false;
            DominoPositionsManager.Instance.UpTarget.LeanScale(new Vector3(1, 1, 1), 0f);
            DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = Color.white;
            DisplayPoints(dominoNumber);
        }

        if (player1Dominoes[dominoNumber - 1].rightDots == numberOnDown || player1Dominoes[dominoNumber - 1].leftDots == numberOnDown)
        {
            if (PreferenceManager.DifficultyMode == DifficultyMode.Easy.ToString() || PreferenceManager.DifficultyMode == DifficultyMode.Medium.ToString())
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = turnColor;
            }
            else if (PreferenceManager.DifficultyMode == DifficultyMode.Hard.ToString())
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = false;
            }
            DisplayPoints(dominoNumber);
        }
        else
        {
            DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = false;
            DominoPositionsManager.Instance.DownTarget.LeanScale(new Vector3(1, 1, 1), 0f);
            DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = Color.white;
            DisplayPoints(dominoNumber);
        }

        if (player1Dominoes[dominoNumber - 1].rightDots == numberOnRight || player1Dominoes[dominoNumber - 1].leftDots == numberOnRight)
        {
            if (PreferenceManager.DifficultyMode == DifficultyMode.Easy.ToString() || PreferenceManager.DifficultyMode == DifficultyMode.Medium.ToString())
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = turnColor;
            }
            else if (PreferenceManager.DifficultyMode == DifficultyMode.Hard.ToString())
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = false;
            }
            DisplayPoints(dominoNumber);
        }
        else
        {
            DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = false;
            DominoPositionsManager.Instance.RightTarget.LeanScale(new Vector3(1, 1, 1), 0f);
            DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = Color.white;
            DisplayPoints(dominoNumber);
        }

        if (player1Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player1Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
        {
            if (PreferenceManager.DifficultyMode == DifficultyMode.Easy.ToString() || PreferenceManager.DifficultyMode == DifficultyMode.Medium.ToString()) 
            {
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Image>().color = turnColor;
            }
            else if (PreferenceManager.DifficultyMode == DifficultyMode.Hard.ToString())
            {
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Animator>().enabled = false;
            }
            DisplayPoints(dominoNumber);
        }
        else
        {
            DominoPositionsManager.Instance.LeftTarget.GetComponent<Animator>().enabled = false;
            DominoPositionsManager.Instance.LeftTarget.LeanScale(new Vector3(1, 1, 1), 0f);
            DominoPositionsManager.Instance.LeftTarget.GetComponent<Image>().color = Color.white;
            DisplayPoints(dominoNumber);
        }
    }

    private void DisplayPoints(int dominoNumber)
    {
        if (PreferenceManager.DifficultyMode == DifficultyMode.Easy.ToString())
        {
            DominoPositionsManager.Instance.UpTarget.transform.GetChild(0).gameObject.SetActive(true);
            DominoPositionsManager.Instance.DownTarget.transform.GetChild(0).gameObject.SetActive(true);
            DominoPositionsManager.Instance.RightTarget.transform.GetChild(0).gameObject.SetActive(true);
            DominoPositionsManager.Instance.LeftTarget.transform.GetChild(0).gameObject.SetActive(true);

            //Up
            if (player1Dominoes[dominoNumber - 1].leftDots == numberOnUp || player1Dominoes[dominoNumber - 1].rightDots == numberOnUp)
            {
                int upPoints = 0;
                int sideEmpty = 0;

                if (downDominoes.Count == 0)
                    sideEmpty++;

                if (rightDominoes.Count == 0)
                    sideEmpty++;

                if (leftDominoes.Count == 0)
                    sideEmpty++;

                if (sideEmpty >= 3)
                    upPoints += (centerDomino.leftDots + centerDomino.rightDots);

                if (player1Dominoes[dominoNumber - 1].leftDots == numberOnUp && player1Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += (player1Dominoes[dominoNumber - 1].rightDots + player1Dominoes[dominoNumber - 1].leftDots);
                else if (player1Dominoes[dominoNumber - 1].leftDots == numberOnUp)
                    upPoints += player1Dominoes[dominoNumber - 1].rightDots;
                else if (player1Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += player1Dominoes[dominoNumber - 1].leftDots;

                if (downDominoes.Count != 0)
                {
                    if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        upPoints += (downDominoes[downDominoes.Count - 1].leftDots + downDominoes[downDominoes.Count - 1].rightDots);
                    else if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        upPoints += numberOnDown;
                }

                if (rightDominoes.Count != 0)
                {
                    if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        upPoints += (rightDominoes[rightDominoes.Count - 1].leftDots + rightDominoes[rightDominoes.Count - 1].rightDots);
                    else if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        upPoints += numberOnRight;
                }

                if (leftDominoes.Count != 0)
                {
                    if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        upPoints += (leftDominoes[leftDominoes.Count - 1].leftDots + leftDominoes[leftDominoes.Count - 1].rightDots);
                    else if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        upPoints += numberOnLeft;
                }

                DominoPositionsManager.Instance.UpTarget.transform.GetChild(0).GetComponent<Text>().text = upPoints.ToString();
            }
            else
            {
                DominoPositionsManager.Instance.UpTarget.transform.GetChild(0).GetComponent<Text>().text = "";
            }

            //Down
            if (player1Dominoes[dominoNumber - 1].leftDots == numberOnDown || player1Dominoes[dominoNumber - 1].rightDots == numberOnDown)
            {
                int downPoints = 0;
                int sideEmpty = 0;

                if (upDominoes.Count == 0)
                    sideEmpty++;

                if (rightDominoes.Count == 0)
                    sideEmpty++;

                if (leftDominoes.Count == 0)
                    sideEmpty++;

                if (sideEmpty >= 3)
                    downPoints += (centerDomino.leftDots + centerDomino.rightDots);

                if (player1Dominoes[dominoNumber - 1].leftDots == numberOnDown && player1Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += (player1Dominoes[dominoNumber - 1].rightDots + player1Dominoes[dominoNumber - 1].leftDots);
                else if (player1Dominoes[dominoNumber - 1].leftDots == numberOnDown)
                    downPoints += player1Dominoes[dominoNumber - 1].rightDots;
                else if (player1Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += player1Dominoes[dominoNumber - 1].leftDots;

                if (upDominoes.Count != 0)
                {
                    if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        downPoints += (upDominoes[upDominoes.Count - 1].leftDots + upDominoes[upDominoes.Count - 1].rightDots);
                    else if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        downPoints += numberOnUp;
                }

                if (rightDominoes.Count != 0)
                {
                    if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        downPoints += (rightDominoes[rightDominoes.Count - 1].leftDots + rightDominoes[rightDominoes.Count - 1].rightDots);
                    else if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        downPoints += numberOnRight;
                }

                if (leftDominoes.Count != 0)
                {
                    if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        downPoints += (leftDominoes[leftDominoes.Count - 1].leftDots + leftDominoes[leftDominoes.Count - 1].rightDots);
                    else if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        downPoints += numberOnLeft;
                }

                DominoPositionsManager.Instance.DownTarget.transform.GetChild(0).GetComponent<Text>().text = downPoints.ToString();
            }
            else
            {
                DominoPositionsManager.Instance.DownTarget.transform.GetChild(0).GetComponent<Text>().text = "";
            }

            //Right
            if (player1Dominoes[dominoNumber - 1].leftDots == numberOnRight || player1Dominoes[dominoNumber - 1].rightDots == numberOnRight)
            {
                int rightPoints = 0;
                int sideEmpty = 0;

                if (upDominoes.Count == 0)
                    sideEmpty++;

                if (downDominoes.Count == 0)
                    sideEmpty++;

                if (leftDominoes.Count == 0)
                    sideEmpty++;

                if (sideEmpty >= 3)
                    rightPoints += (centerDomino.leftDots + centerDomino.rightDots);

                if (player1Dominoes[dominoNumber - 1].leftDots == numberOnRight && player1Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += (player1Dominoes[dominoNumber - 1].rightDots + player1Dominoes[dominoNumber - 1].leftDots);
                else if (player1Dominoes[dominoNumber - 1].leftDots == numberOnRight)
                    rightPoints += player1Dominoes[dominoNumber - 1].rightDots;
                else if (player1Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += player1Dominoes[dominoNumber - 1].leftDots;

                if (upDominoes.Count != 0)
                {
                    if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        rightPoints += (upDominoes[upDominoes.Count - 1].leftDots + upDominoes[upDominoes.Count - 1].rightDots);
                    else if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        rightPoints += numberOnUp;
                }

                if (downDominoes.Count != 0)
                {
                    if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        rightPoints += (downDominoes[downDominoes.Count - 1].leftDots + downDominoes[downDominoes.Count - 1].rightDots);
                    else if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        rightPoints += numberOnDown;
                }

                if (leftDominoes.Count != 0)
                {
                    if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        rightPoints += (leftDominoes[leftDominoes.Count - 1].leftDots + leftDominoes[leftDominoes.Count - 1].rightDots);
                    else if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        rightPoints += numberOnLeft;
                }

                DominoPositionsManager.Instance.RightTarget.transform.GetChild(0).GetComponent<Text>().text = rightPoints.ToString();
            }
            else
            {
                DominoPositionsManager.Instance.RightTarget.transform.GetChild(0).GetComponent<Text>().text = "";
            }

            //Left
            if (player1Dominoes[dominoNumber - 1].leftDots == numberOnLeft || player1Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
            {
                int leftPoints = 0;
                int sideEmpty = 0;

                if (upDominoes.Count == 0)
                    sideEmpty++;

                if (downDominoes.Count == 0)
                    sideEmpty++;

                if (rightDominoes.Count == 0)
                    sideEmpty++;

                if (sideEmpty >= 3)
                    leftPoints += (centerDomino.leftDots + centerDomino.rightDots);

                if (player1Dominoes[dominoNumber - 1].leftDots == numberOnLeft && player1Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += (player1Dominoes[dominoNumber - 1].rightDots + player1Dominoes[dominoNumber - 1].leftDots);
                else if (player1Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
                    leftPoints += player1Dominoes[dominoNumber - 1].rightDots;
                else if (player1Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += player1Dominoes[dominoNumber - 1].leftDots;

                if (upDominoes.Count != 0)
                {
                    if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        leftPoints += (upDominoes[upDominoes.Count - 1].leftDots + upDominoes[upDominoes.Count - 1].rightDots);
                    else if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        leftPoints += numberOnUp;
                }

                if (downDominoes.Count != 0)
                {
                    if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        leftPoints += (downDominoes[downDominoes.Count - 1].leftDots + downDominoes[downDominoes.Count - 1].rightDots);
                    else if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        leftPoints += numberOnDown;
                }

                if (rightDominoes.Count != 0)
                {
                    if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                        leftPoints += (rightDominoes[rightDominoes.Count - 1].leftDots + rightDominoes[rightDominoes.Count - 1].rightDots);
                    else if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                        leftPoints += numberOnRight;
                }

                DominoPositionsManager.Instance.LeftTarget.transform.GetChild(0).GetComponent<Text>().text = leftPoints.ToString();
            }
            else
            {
                DominoPositionsManager.Instance.LeftTarget.transform.GetChild(0).GetComponent<Text>().text = "";
            }
        }
        else 
        {
            DominoPositionsManager.Instance.UpTarget.transform.GetChild(0).gameObject.SetActive(false);
            DominoPositionsManager.Instance.DownTarget.transform.GetChild(0).gameObject.SetActive(false);
            DominoPositionsManager.Instance.RightTarget.transform.GetChild(0).gameObject.SetActive(false);
            DominoPositionsManager.Instance.LeftTarget.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void ConnectDominoRequest(string side)
    {
        if (side == "UP")
        {
            if (player1Dominoes[movedForward - 1].rightDots == numberOnUp || player1Dominoes[movedForward - 1].leftDots == numberOnUp)
            {
                upDominoes.Add(player1Dominoes[movedForward - 1]);
                player1Dominoes.Remove(player1Dominoes[movedForward - 1]);
                FindUpTargetAndMoveDomino(upDominoes[upDominoes.Count - 1]);
                Invoke(nameof(ArangeDominoes), 1f);
                ResetGameAfterTurn();
                if (UpdatePointsAndCheckIfRoundIsCompleted(side))
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
        }
        else if (side == "DOWN")
        {
            if (player1Dominoes[movedForward - 1].rightDots == numberOnDown || player1Dominoes[movedForward - 1].leftDots == numberOnDown)
            {
                downDominoes.Add(player1Dominoes[movedForward - 1]);
                player1Dominoes.Remove(player1Dominoes[movedForward - 1]);
                FindDownTargetAndMoveDomino(downDominoes[downDominoes.Count - 1]);
                Invoke(nameof(ArangeDominoes), 1f);
                ResetGameAfterTurn();
                if (UpdatePointsAndCheckIfRoundIsCompleted(side))
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
        }
        else if (side == "RIGHT")
        {
            if (player1Dominoes[movedForward - 1].rightDots == numberOnRight || player1Dominoes[movedForward - 1].leftDots == numberOnRight)
            {
                rightDominoes.Add(player1Dominoes[movedForward - 1]);
                player1Dominoes.Remove(player1Dominoes[movedForward - 1]);
                FindRightTargetAndMoveDomino(rightDominoes[rightDominoes.Count - 1]);
                Invoke(nameof(ArangeDominoes), 1f);
                ResetGameAfterTurn();
                if (UpdatePointsAndCheckIfRoundIsCompleted(side))
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
        }
        else if (side == "LEFT")
        {
            if (player1Dominoes[movedForward - 1].rightDots == numberOnLeft || player1Dominoes[movedForward - 1].leftDots == numberOnLeft)
            {
                leftDominoes.Add(player1Dominoes[movedForward - 1]);
                player1Dominoes.Remove(player1Dominoes[movedForward - 1]);
                FindLeftTargetAndMoveDomino(leftDominoes[leftDominoes.Count - 1]);
                Invoke(nameof(ArangeDominoes), 1f);
                ResetGameAfterTurn();
                if (UpdatePointsAndCheckIfRoundIsCompleted(side))
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
        }
    }

    private bool UpdatePointsAndCheckIfRoundIsCompleted(string side)
    {
        int upPoints = 0;
        int downPoints = 0;
        int rightPoints = 0;
        int leftPoints = 0;

        if (side == "UP")
        {
            int sideEmpty = 0;

            if (downDominoes.Count == 0)
                sideEmpty++;

            if (rightDominoes.Count == 0)
                sideEmpty++;

            if (leftDominoes.Count == 0)
                sideEmpty++;

            if (sideEmpty >= 3)
                upPoints += (centerDomino.leftDots + centerDomino.rightDots);

            if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                upPoints += (numberOnUp + numberOnUp);
            else if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                upPoints += numberOnUp;

            if (downDominoes.Count != 0)
            {
                if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    upPoints += (numberOnDown + numberOnDown);
                else if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    upPoints += numberOnDown;
            }

            if (rightDominoes.Count != 0)
            {
                if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    upPoints += (numberOnRight + numberOnRight);
                else if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    upPoints += numberOnRight;
            }

            if (leftDominoes.Count != 0)
            {
                if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    upPoints += (numberOnLeft + numberOnLeft);
                else if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    upPoints += numberOnLeft;
            }
        }
        else if (side == "DOWN")
        {
            int sideEmpty = 0;
            if (upDominoes.Count == 0)
                sideEmpty++;

            if (rightDominoes.Count == 0)
                sideEmpty++;

            if (leftDominoes.Count == 0)
                sideEmpty++;

            if (sideEmpty >= 3)
                downPoints += (centerDomino.leftDots + centerDomino.rightDots);

            if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                downPoints += (numberOnDown + numberOnDown);
            else if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                downPoints += numberOnDown;

            if (upDominoes.Count != 0)
            {
                if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    downPoints += (numberOnUp + numberOnUp);
                else if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    downPoints += numberOnUp;
            }

            if (rightDominoes.Count != 0)
            {
                if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    downPoints += (numberOnRight + numberOnRight);
                else if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    downPoints += numberOnRight;
            }

            if (leftDominoes.Count != 0)
            {
                if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    downPoints += (numberOnLeft + numberOnLeft);
                else if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    downPoints += numberOnLeft;
            }
        }
        else if (side == "RIGHT")
        {
            int sideEmpty = 0;
            if (upDominoes.Count == 0)
                sideEmpty++;

            if (downDominoes.Count == 0)
                sideEmpty++;

            if (leftDominoes.Count == 0)
                sideEmpty++;

            if (sideEmpty >= 3)
                rightPoints += (centerDomino.leftDots + centerDomino.rightDots);

            if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                rightPoints += (numberOnRight + numberOnRight);
            else if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                rightPoints += numberOnRight;

            if (upDominoes.Count != 0)
            {
                if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    rightPoints += (numberOnUp + numberOnUp);
                else if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    rightPoints += numberOnUp;
            }

            if (downDominoes.Count != 0)
            {
                if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    rightPoints += (numberOnDown + numberOnDown);
                else if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    rightPoints += numberOnDown;
            }

            if (leftDominoes.Count != 0)
            {
                if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    rightPoints += (numberOnLeft + numberOnLeft);
                else if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    rightPoints += numberOnLeft;
            }
        }
        else if (side == "LEFT")
        {
            int sideEmpty = 0;
            if (upDominoes.Count == 0)
                sideEmpty++;

            if (downDominoes.Count == 0)
                sideEmpty++;

            if (rightDominoes.Count == 0)
                sideEmpty++;

            if (sideEmpty >= 3)
                leftPoints += (centerDomino.leftDots + centerDomino.rightDots);

            if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                leftPoints += (numberOnLeft + numberOnLeft);
            else if (leftDominoes[leftDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                leftPoints += numberOnLeft;

            if (upDominoes.Count != 0)
            {
                if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    leftPoints += (numberOnUp + numberOnUp);
                else if (upDominoes[upDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    leftPoints += numberOnUp;
            }

            if (downDominoes.Count != 0)
            {
                if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    leftPoints += (numberOnDown + numberOnDown);
                else if (downDominoes[downDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    leftPoints += numberOnDown;
            }

            if (rightDominoes.Count != 0)
            {
                if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Vertical.ToString())
                    leftPoints += (numberOnRight + numberOnRight);
                else if (rightDominoes[rightDominoes.Count - 1].connected == ConnectionAxis.Horizontal.ToString())
                    leftPoints += numberOnRight;
            }
        }

        if (turn == Player.Player1.ToString())
        {
            if (side == "UP" && (upPoints % 5 == 0))
            {
                player1Points += upPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "DOWN" && (downPoints % 5 == 0))
            {
                player1Points += downPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "RIGHT" && (rightPoints % 5 == 0))
            {
                player1Points += rightPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "LEFT" && (leftPoints % 5 == 0))
            {
                player1Points += leftPoints;
                AudioManager.Instance.PlayPointsSound();
            }

            player1.transform.GetChild(1).GetComponent<Text>().text = player1Points.ToString();
        }
        else if (turn == Player.Player2.ToString())
        {
            if (side == "UP" && (upPoints % 5 == 0))
            {
                player2Points += upPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "DOWN" && (downPoints % 5 == 0))
            {
                player2Points += downPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "RIGHT" && (rightPoints % 5 == 0))
            {
                player2Points += rightPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "LEFT" && (leftPoints % 5 == 0))
            {
                player2Points += leftPoints;
                AudioManager.Instance.PlayPointsSound();
            }

            player2.transform.GetChild(1).GetComponent<Text>().text = player2Points.ToString();
        }
        else if (turn == Player.Player3.ToString())
        {
            if (side == "UP" && (upPoints % 5 == 0))
            {
                player3Points += upPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "DOWN" && (downPoints % 5 == 0))
            {
                player3Points += downPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "RIGHT" && (rightPoints % 5 == 0))
            {
                player3Points += rightPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "LEFT" && (leftPoints % 5 == 0))
            {
                player3Points += leftPoints;
                AudioManager.Instance.PlayPointsSound();
            }

            player3.transform.GetChild(1).GetComponent<Text>().text = player3Points.ToString();
        }
        else if (turn == Player.Player4.ToString())
        {
            if (side == "UP" && (upPoints % 5 == 0))
            {
                player4Points += upPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "DOWN" && (downPoints % 5 == 0))
            {
                player4Points += downPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "RIGHT" && (rightPoints % 5 == 0))
            {
                player4Points += rightPoints;
                AudioManager.Instance.PlayPointsSound();
            }
            else if (side == "LEFT" && (leftPoints % 5 == 0))
            {
                player4Points += leftPoints;
                AudioManager.Instance.PlayPointsSound();
            }

            player4.transform.GetChild(1).GetComponent<Text>().text = player4Points.ToString();
        }

        if (numberOfPlayers == 2)
        {
            if (player1Dominoes.Count == 0 || player2Dominoes.Count == 0)
            {
                RoundCompleted();
                return false;
            }
        }
        else if (numberOfPlayers == 3)
        {
            if (player1Dominoes.Count == 0 || player2Dominoes.Count == 0 || player3Dominoes.Count == 0)
            {
                RoundCompleted();
                return false;
            }
        }
        else if (numberOfPlayers == 4)
        {
            if (player1Dominoes.Count == 0 || player2Dominoes.Count == 0 || player3Dominoes.Count == 0 || player4Dominoes.Count == 0)
            {
                RoundCompleted();
                return false;
            }
        }

        if (player1Points >= WinningPoints)
        {
            winner = Player.Player1.ToString();
            Invoke(nameof(GameCompleted), 2f);
            return false;
        }
        else if (player2Points >= WinningPoints)
        {
            winner = Player.Player2.ToString();
            Invoke(nameof(GameCompleted), 2f);
            return false;
        }
        else if (player3Points >= WinningPoints)
        {
            winner = Player.Player3.ToString();
            Invoke(nameof(GameCompleted), 2f);
            return false;
        }
        else if (player4Points >= WinningPoints)
        {
            winner = Player.Player4.ToString();
            Invoke(nameof(GameCompleted), 2f);
            return false;
        }
        else
        {
            return true;
        }
    }

    private void ResetGameAfterTurn()
    {
        movedForward = -1;
        DominoPositionsManager.Instance.ResetGame();
    }

    private void ArangeDominoes()
    {
        if (player1Dominoes.Count > 0)
        {
            player1Dominoes = player1Dominoes.OrderByDescending(x => x.rightDots).ThenByDescending(x => x.leftDots).ToList();

            foreach (Domino _domino in player1Dominoes)
                player1Dominoes[player1Dominoes.IndexOf(_domino)].domino.LeanMove(player1DominoesPositions[player1Dominoes.IndexOf(_domino)].position, 0.3f);
        }

        if (player2Dominoes.Count > 0)
        {
            player2Dominoes = player2Dominoes.OrderByDescending(x => x.rightDots).ThenByDescending(x => x.leftDots).ToList();

            foreach (Domino _domino in player2Dominoes)
                player2Dominoes[player2Dominoes.IndexOf(_domino)].domino.LeanMove(player2DominoesPositions[player2Dominoes.IndexOf(_domino)].position, 0.3f);
        }

        if (player3Dominoes.Count > 0)
        {
            player3Dominoes = player3Dominoes.OrderByDescending(x => x.rightDots).ThenByDescending(x => x.leftDots).ToList();

            foreach (Domino _domino in player3Dominoes)
                player3Dominoes[player3Dominoes.IndexOf(_domino)].domino.LeanMove(player3DominoesPositions[player3Dominoes.IndexOf(_domino)].position, 0.3f);
        }

        if (player4Dominoes.Count > 0)
        {
            player4Dominoes = player4Dominoes.OrderByDescending(x => x.rightDots).ThenByDescending(x => x.leftDots).ToList();

            foreach (Domino _domino in player4Dominoes)
                player4Dominoes[player4Dominoes.IndexOf(_domino)].domino.LeanMove(player4DominoesPositions[player4Dominoes.IndexOf(_domino)].position, 0.3f);
        }
    }

    private void CPUDecisionOnTurn()
    {
        if (turn == Player.Player2.ToString())
        {
            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //DOWN
            {
                if (player2Dominoes[i].leftDots == numberOnDown || player2Dominoes[i].rightDots == numberOnDown)
                {
                    movedForward = i + 1;
                    downDominoes.Add(player2Dominoes[i]);
                    player2Dominoes.Remove(player2Dominoes[i]);
                    downDominoes[downDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindDownTargetAndMoveDomino(downDominoes[downDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("DOWN"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //LEFT
            {
                if (player2Dominoes[i].leftDots == numberOnLeft || player2Dominoes[i].rightDots == numberOnLeft)
                {
                    movedForward = i + 1;
                    leftDominoes.Add(player2Dominoes[i]);
                    player2Dominoes.Remove(player2Dominoes[i]);
                    leftDominoes[leftDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindLeftTargetAndMoveDomino(leftDominoes[leftDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("LEFT"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //UP
            {
                if (player2Dominoes[i].leftDots == numberOnUp || player2Dominoes[i].rightDots == numberOnUp)
                {
                    movedForward = i + 1;
                    upDominoes.Add(player2Dominoes[i]);
                    player2Dominoes.Remove(player2Dominoes[i]);
                    upDominoes[upDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindUpTargetAndMoveDomino(upDominoes[upDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("UP"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //RIGHT
            {
                if (player2Dominoes[i].leftDots == numberOnRight || player2Dominoes[i].rightDots == numberOnRight)
                {
                    movedForward = i + 1;
                    rightDominoes.Add(player2Dominoes[i]);
                    player2Dominoes.Remove(player2Dominoes[i]);
                    rightDominoes[rightDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindRightTargetAndMoveDomino(rightDominoes[rightDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("RIGHT"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            if (DominoesDeck.Count != 0)
            {
                if (player2Dominoes.Count <= 8)
                {
                    player2Dominoes.Add(DominoesDeck[0]);
                    DominoesDeck.Remove(DominoesDeck[0]);
                    player2Dominoes[player2Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                    player2Dominoes[player2Dominoes.Count - 1].domino.LeanMove(player2DominoesPositions[player2Dominoes.Count - 1].position, dominoAnimationSpeed);
                    Invoke(nameof(ArangeDominoes), 1f);
                    Invoke(nameof(CPUDecisionOnTurn), 2f);
                }
                else if (player2Dominoes.Count >= 9)
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
            else if (DominoesDeck.Count == 0)
            {
                if (CheckDeadEnd())
                {
                    RoundCompleted();
                }
                else
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
        }
        else if (turn == Player.Player3.ToString())
        {
            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //UP
            {
                if (player3Dominoes[i].leftDots == numberOnUp || player3Dominoes[i].rightDots == numberOnUp)
                {
                    movedForward = i + 1;
                    upDominoes.Add(player3Dominoes[i]);
                    player3Dominoes.Remove(player3Dominoes[i]);
                    upDominoes[upDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindUpTargetAndMoveDomino(upDominoes[upDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("UP"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //RIGHT
            {
                if (player3Dominoes[i].leftDots == numberOnRight || player3Dominoes[i].rightDots == numberOnRight)
                {
                    movedForward = i + 1;
                    rightDominoes.Add(player3Dominoes[i]);
                    player3Dominoes.Remove(player3Dominoes[i]);
                    rightDominoes[rightDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindRightTargetAndMoveDomino(rightDominoes[rightDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("RIGHT"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //DOWN
            {
                if (player3Dominoes[i].leftDots == numberOnDown || player3Dominoes[i].rightDots == numberOnDown)
                {
                    movedForward = i + 1;
                    downDominoes.Add(player3Dominoes[i]);
                    player3Dominoes.Remove(player3Dominoes[i]);
                    downDominoes[downDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindDownTargetAndMoveDomino(downDominoes[downDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("DOWN"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //LEFT
            {
                if (player3Dominoes[i].leftDots == numberOnLeft || player3Dominoes[i].rightDots == numberOnLeft)
                {
                    movedForward = i + 1;
                    leftDominoes.Add(player3Dominoes[i]);
                    player3Dominoes.Remove(player3Dominoes[i]);
                    leftDominoes[leftDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindLeftTargetAndMoveDomino(leftDominoes[leftDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("LEFT"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            if (DominoesDeck.Count != 0)
            {
                if (player3Dominoes.Count <= 8)
                {
                    player3Dominoes.Add(DominoesDeck[0]);
                    DominoesDeck.Remove(DominoesDeck[0]);
                    player3Dominoes[player3Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                    player3Dominoes[player3Dominoes.Count - 1].domino.LeanMove(player3DominoesPositions[player3Dominoes.Count - 1].position, dominoAnimationSpeed);
                    player3Dominoes[player3Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                    Invoke(nameof(ArangeDominoes), 1f);
                    Invoke(nameof(CPUDecisionOnTurn), 2f);
                }
                else if (player3Dominoes.Count >= 9)
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
            else if (DominoesDeck.Count == 0)
            {
                if (CheckDeadEnd())
                {
                    RoundCompleted();
                }
                else
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
        }
        else if (turn == Player.Player4.ToString())
        {
            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //LEFT
            {
                if (player4Dominoes[i].leftDots == numberOnLeft || player4Dominoes[i].rightDots == numberOnLeft)
                {
                    movedForward = i + 1;
                    leftDominoes.Add(player4Dominoes[i]);
                    player4Dominoes.Remove(player4Dominoes[i]);
                    leftDominoes[leftDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindLeftTargetAndMoveDomino(leftDominoes[leftDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("LEFT"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //DOWN
            {
                if (player4Dominoes[i].leftDots == numberOnDown || player4Dominoes[i].rightDots == numberOnDown)
                {
                    movedForward = i + 1;
                    downDominoes.Add(player4Dominoes[i]);
                    player4Dominoes.Remove(player4Dominoes[i]);
                    downDominoes[downDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindDownTargetAndMoveDomino(downDominoes[downDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("DOWN"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //RIGHT
            {
                if (player4Dominoes[i].leftDots == numberOnRight || player4Dominoes[i].rightDots == numberOnRight)
                {
                    movedForward = i + 1;
                    rightDominoes.Add(player4Dominoes[i]);
                    player4Dominoes.Remove(player4Dominoes[i]);
                    rightDominoes[rightDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindRightTargetAndMoveDomino(rightDominoes[rightDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("RIGHT"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //UP
            {
                if (player4Dominoes[i].leftDots == numberOnUp || player4Dominoes[i].rightDots == numberOnUp)
                {
                    movedForward = i + 1;
                    upDominoes.Add(player4Dominoes[i]);
                    player4Dominoes.Remove(player4Dominoes[i]);
                    upDominoes[upDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
                    FindUpTargetAndMoveDomino(upDominoes[upDominoes.Count - 1]);
                    Invoke(nameof(ArangeDominoes), 1f);
                    ResetGameAfterTurn();
                    if (UpdatePointsAndCheckIfRoundIsCompleted("UP"))
                    {
                        Invoke(nameof(GiveTurn), 2f);
                    }
                    return;
                }
            }

            if (DominoesDeck.Count != 0)
            {
                if (player4Dominoes.Count <= 8)
                {
                    player4Dominoes.Add(DominoesDeck[0]);
                    DominoesDeck.Remove(DominoesDeck[0]);
                    player4Dominoes[player4Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                    player4Dominoes[player4Dominoes.Count - 1].domino.LeanMove(player4DominoesPositions[player4Dominoes.Count - 1].position, dominoAnimationSpeed);
                    player4Dominoes[player4Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                    Invoke(nameof(ArangeDominoes), 1f);
                    Invoke(nameof(CPUDecisionOnTurn), 2f);
                }
                else if (player4Dominoes.Count >= 9)
                {
                    Invoke(nameof(GiveTurn), 2f);
                }
            }
            else if (DominoesDeck.Count == 0)
            {
                if (CheckDeadEnd())
                    RoundCompleted();
                else
                    Invoke(nameof(GiveTurn), 2f);
            }
        }
    }

    private void FindUpTargetAndMoveDomino(Domino targetDomino)
    {
        float yAxis = DominoPositionsManager.Instance.centerReferencePoint.position.y + 32.5f;                     //32.5 (width/2)

        for (int i = 0; i < upDominoes.Count - 1; i++)
        {
            if (upDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                yAxis += DominoWidth;
            else if (upDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                yAxis += DominoHeight;
        }

        if (targetDomino.rightDots == targetDomino.leftDots)
        {
            yAxis += 32.5f;
            targetDomino.connected = ConnectionAxis.Horizontal.ToString();
            DominoPositionsManager.Instance.upTargetDomino.position = new Vector3(DominoPositionsManager.Instance.upTargetDomino.position.x, yAxis, DominoPositionsManager.Instance.upTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("VERTICAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.upTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);
        }
        else
        {
            yAxis += DominoWidth;
            targetDomino.connected = ConnectionAxis.Vertical.ToString();
            DominoPositionsManager.Instance.upTargetDomino.position = new Vector3(DominoPositionsManager.Instance.upTargetDomino.position.x, yAxis, DominoPositionsManager.Instance.upTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("VERTICAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.upTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);

            if (upDominoes[upDominoes.Count - 1].rightDots == numberOnUp)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, 0), dominoAnimationSpeed);
                numberOnUp = upDominoes[upDominoes.Count - 1].leftDots;
            }
            else if (upDominoes[upDominoes.Count - 1].leftDots == numberOnUp)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, 180), dominoAnimationSpeed);
                numberOnUp = upDominoes[upDominoes.Count - 1].rightDots;
            }
        }
    }

    private void FindDownTargetAndMoveDomino(Domino targetDomino)
    {
        float yAxis = DominoPositionsManager.Instance.centerReferencePoint.position.y - 32.5f;                     //32.5 (width/2)

        for (int i = 0; i < downDominoes.Count - 1; i++)
        {
            if (downDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                yAxis -= DominoWidth;
            else if (downDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                yAxis -= DominoHeight;
        }

        if (targetDomino.rightDots == targetDomino.leftDots)
        {
            yAxis -= 32.5f;
            targetDomino.connected = ConnectionAxis.Horizontal.ToString();
            DominoPositionsManager.Instance.downTargetDomino.position = new Vector3(DominoPositionsManager.Instance.downTargetDomino.position.x, yAxis, DominoPositionsManager.Instance.downTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("VERTICAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.downTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);
            targetDomino.domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
        }
        else
        {
            yAxis -= DominoWidth;
            targetDomino.connected = ConnectionAxis.Vertical.ToString();
            DominoPositionsManager.Instance.downTargetDomino.position = new Vector3(DominoPositionsManager.Instance.downTargetDomino.position.x, yAxis, DominoPositionsManager.Instance.downTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("VERTICAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.downTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);

            if (downDominoes[downDominoes.Count - 1].rightDots == numberOnDown)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, 180), dominoAnimationSpeed);
                numberOnDown = downDominoes[downDominoes.Count - 1].leftDots;
            }
            else if (downDominoes[downDominoes.Count - 1].leftDots == numberOnDown)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, 0), dominoAnimationSpeed);
                numberOnDown = downDominoes[downDominoes.Count - 1].rightDots;
            }
        }
    }

    private void FindRightTargetAndMoveDomino(Domino targetDomino)
    {
        float xAxis = DominoPositionsManager.Instance.centerReferencePoint.position.x + DominoWidth;             //DominoWidth (width)

        for (int i = 0; i < rightDominoes.Count - 1; i++)
        {
            if (rightDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                xAxis += DominoHeight;                                                                           //DominoHeight (height)
            else if (rightDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                xAxis += DominoWidth;                                                                            //DominoWidth (width)
        }

        if (targetDomino.rightDots == targetDomino.leftDots)
        {
            xAxis += 32.5f;                                                                             //32.5 (width/2)
            targetDomino.connected = ConnectionAxis.Vertical.ToString();
            DominoPositionsManager.Instance.rightTargetDomino.position = new Vector3(xAxis, DominoPositionsManager.Instance.rightTargetDomino.position.y, DominoPositionsManager.Instance.rightTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("HORIZONTAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.rightTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanRotate(new Vector3(0, 0, 0), dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);
        }
        else
        {
            xAxis += DominoWidth;                                                                               //DominoWidth (width)

            targetDomino.connected = ConnectionAxis.Horizontal.ToString();
            DominoPositionsManager.Instance.rightTargetDomino.position = new Vector3(xAxis, DominoPositionsManager.Instance.rightTargetDomino.position.y, DominoPositionsManager.Instance.rightTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("HORIZONTAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.rightTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);

            if (rightDominoes[rightDominoes.Count - 1].rightDots == numberOnRight)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, -90), dominoAnimationSpeed);
                numberOnRight = rightDominoes[rightDominoes.Count - 1].leftDots;
            }
            else if (rightDominoes[rightDominoes.Count - 1].leftDots == numberOnRight)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                numberOnRight = rightDominoes[rightDominoes.Count - 1].rightDots;
            }
        }
    }

    private void FindLeftTargetAndMoveDomino(Domino targetDomino)
    {
        float xAxis = DominoPositionsManager.Instance.centerReferencePoint.position.x - DominoWidth;  //DominoWidth (width)

        for (int i = 0; i < leftDominoes.Count - 1; i++)
        {
            if (leftDominoes[i].connected == ConnectionAxis.Horizontal.ToString())
                xAxis -= DominoHeight;                                                                //DominoHeight (height)
            else if (leftDominoes[i].connected == ConnectionAxis.Vertical.ToString())
                xAxis -= DominoWidth;                                                                 //DominoWidth (width)
        }

        if (targetDomino.rightDots == targetDomino.leftDots)
        {
            xAxis -= 32.5f;                                                                  //32.5 (width/2)
            targetDomino.connected = ConnectionAxis.Vertical.ToString();
            DominoPositionsManager.Instance.leftTargetDomino.position = new Vector3(xAxis, DominoPositionsManager.Instance.leftTargetDomino.position.y, DominoPositionsManager.Instance.leftTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("HORIZONTAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.leftTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanRotate(new Vector3(0, 0, 0), dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);
        }
        else
        {
            xAxis -= DominoWidth;                                                                    //DominoWidth (width)
            targetDomino.connected = ConnectionAxis.Horizontal.ToString();
            DominoPositionsManager.Instance.leftTargetDomino.position = new Vector3(xAxis, DominoPositionsManager.Instance.leftTargetDomino.position.y, DominoPositionsManager.Instance.leftTargetDomino.position.z);
            StartCoroutine(CameraHandlerCoroutine("HORIZONTAL"));
            targetDomino.domino.transform.SetParent(dominoGlobalParent);
            targetDomino.domino.LeanMove(DominoPositionsManager.Instance.leftTargetDomino.position, dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);

            if (leftDominoes[leftDominoes.Count - 1].rightDots == numberOnLeft)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                numberOnLeft = leftDominoes[leftDominoes.Count - 1].leftDots;
            }
            else if (leftDominoes[leftDominoes.Count - 1].leftDots == numberOnLeft)
            {
                targetDomino.domino.LeanRotate(new Vector3(0, 0, -90), dominoAnimationSpeed);
                numberOnLeft = leftDominoes[leftDominoes.Count - 1].rightDots;
            }
        }
    }

    private void RoundCompleted()
    {
        Invoke(nameof(RoundReset), 2f);
        Invoke(nameof(StartGameOnRoundComplete), 3f);
    }

    private void StartGameOnRoundComplete()
    {
        GenerateRandomDominoesDeck();

        if (numberOfPlayers == 2)
        {
            DistributeDominoesToPlayer1(7);
            DistributeDominoesToPlayer2(7);
        }
        else if (numberOfPlayers == 3)
        {
            DistributeDominoesToPlayer1(7);
            DistributeDominoesToPlayer2(7);
            DistributeDominoesToPlayer3(7);
        }
        else if (numberOfPlayers == 4)
        {
            DistributeDominoesToPlayer1(5);
            DistributeDominoesToPlayer2(5);
            DistributeDominoesToPlayer3(5);
            DistributeDominoesToPlayer4(5);
        }

        Invoke(nameof(MoveFirstDominoe), 1f);
        Invoke(nameof(ArangeDominoes), 2f);
    }

    private void GameCompleted()
    {
        GetComponent<OfflineGamePlayScreenHandler>().DisplayResult(winner);
    }

    private void RoundReset()
    {
        MainCamera.gameObject.transform.position = new Vector3(360, 540, 0);

        numberOnUp = 0;
        numberOnDown = 0;
        numberOnRight = 0;
        numberOnLeft = 0;
        turn = null;

        foreach (Domino singleDomino in Dominoes)
        {
            singleDomino.domino.transform.SetParent(dominoLocalParent);
            singleDomino.domino.LeanMove(dominoDeckPosition.position, dominoAnimationSpeed);
            singleDomino.domino.LeanRotate(new Vector3(0, 0, 0), dominoAnimationSpeed);
            singleDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);
            singleDomino.domino.transform.Find("Back Side").SetAsFirstSibling();

            singleDomino.isAddedToDominoesDeck = false;
            singleDomino.connected = null;
        }

        DominoesDeck = new List<Domino>();
        player1Dominoes = new List<Domino>();
        player2Dominoes = new List<Domino>();
        player3Dominoes = new List<Domino>();
        player4Dominoes = new List<Domino>();

        centerDomino = new Domino();
        upDominoes = new List<Domino>();
        downDominoes = new List<Domino>();
        rightDominoes = new List<Domino>();
        leftDominoes = new List<Domino>();

        player1Icon.timerImage.SetActive(false);
        player2Icon.timerImage.SetActive(false);
        player3Icon.timerImage.SetActive(false);
        player4Icon.timerImage.SetActive(false);
    }

    public void ResetGame()
    {
        StopAllCoroutines();
        CancelInvoke();
        MainCamera.gameObject.transform.position = new Vector3(360, 540, 0);
        cameraFlag = false;
        //SetCamera();

        numberOfPlayers = 0;
        numberOnUp = 0;
        numberOnDown = 0;
        numberOnRight = 0;
        numberOnLeft = 0;
        player1Points = 0;
        player2Points = 0;
        player3Points = 0;
        player4Points = 0;
        player1.transform.GetChild(1).GetComponent<Text>().text = player1Points.ToString();
        player2.transform.GetChild(1).GetComponent<Text>().text = player2Points.ToString();
        player3.transform.GetChild(1).GetComponent<Text>().text = player3Points.ToString();
        player4.transform.GetChild(1).GetComponent<Text>().text = player4Points.ToString();
        turn = null;
        winner = null;

        foreach (Domino singleDomino in Dominoes)
        {
            singleDomino.domino.transform.SetParent(dominoLocalParent);
            singleDomino.domino.LeanMove(dominoDeckPosition.position, dominoAnimationSpeed);
            singleDomino.domino.LeanRotate(new Vector3(0, 0, 0), dominoAnimationSpeed);
            singleDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);
            singleDomino.domino.transform.Find("Back Side").SetAsFirstSibling();

            singleDomino.isAddedToDominoesDeck = false;
            singleDomino.connected = null;
        }

        DominoesDeck = new List<Domino>();
        player1Dominoes = new List<Domino>();
        player2Dominoes = new List<Domino>();
        player3Dominoes = new List<Domino>();
        player4Dominoes = new List<Domino>();

        centerDomino = new Domino();
        upDominoes = new List<Domino>();
        downDominoes = new List<Domino>();
        rightDominoes = new List<Domino>();
        leftDominoes = new List<Domino>();

        player1Icon.timerImage.SetActive(false);
        player2Icon.timerImage.SetActive(false);
        player3Icon.timerImage.SetActive(false);
        player4Icon.timerImage.SetActive(false);

        player1.transform.GetChild(1).GetComponent<Text>().text = 0.ToString();
        player2.transform.GetChild(1).GetComponent<Text>().text = 0.ToString();
        player3.transform.GetChild(1).GetComponent<Text>().text = 0.ToString();
        player4.transform.GetChild(1).GetComponent<Text>().text = 0.ToString();
    }

    public void OnCameraButtonClick()
    {
        if (cameraFlag)
            cameraFlag = false;
        else if (!cameraFlag)
            cameraFlag = true;
        
        SetCamera();
    }

    private void SetCamera()
    {
        if (cameraFlag)          //Manual Camera 
        {
            cameraButton.GetComponent<Image>().sprite = manualCameraSprite;
            MainCamera.GetComponent<CameraMovement>().enabled = true;
            MainCamera.GetComponent<PinchZoom>().enabled = true;

            MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, 0f), dominoAnimationSpeed);
        }
        else if (!cameraFlag)    //Auto Camera 
        {
            cameraButton.GetComponent<Image>().sprite = autoCameraSprite;
            MainCamera.GetComponent<CameraMovement>().enabled = false;
            MainCamera.GetComponent<PinchZoom>().enabled = false;
            
            float difference = MainCamera.fieldOfView - 60;
            float tempDominoInvokingSpeed = dominoAnimationSpeed / difference;
            float dominoInvokingSpeed = dominoAnimationSpeed / difference;

            for (int i = 0; i < difference; i++)
            {
                Invoke(nameof(UpdateFieldOfView), tempDominoInvokingSpeed);
                tempDominoInvokingSpeed += dominoInvokingSpeed;
            }

            StartCoroutine(CameraHandlerCoroutine("BOTH"));
        }
    }

    private void UpdateFieldOfView()
    {
        MainCamera.fieldOfView -= 1;
    }

}

public enum Player
{
    Player1,
    Player2,
    Player3,
    Player4
}

public enum ConnectionSide
{
    Right,
    Left
}

public enum ConnectionAxis
{
    Vertical,
    Horizontal
}


