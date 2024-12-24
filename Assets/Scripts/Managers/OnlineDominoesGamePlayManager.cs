using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Version3;

public class OnlineDominoesGamePlayManager : MonoBehaviour
{
    private PhotonView PV;

    [Header("Heads Up Display")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;

    [Header("Game Play Manager")]
    private const int DominoWidth = 65;
    private const int DominoHeight = 130;
    private int WinningPoints;
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
    [SerializeField] private int movedForward = -1;
    [SerializeField] private string turn = null;
    [SerializeField] private string winner = null;

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

    public GameObject player1GamePlayButtons;
    public GameObject player2GamePlayButtons;
    public GameObject player3GamePlayButtons;
    public GameObject player4GamePlayButtons;

    public Transform dominoDeckPosition;
    public Transform[] player1DominoesPositions;
    public Transform[] player2DominoesPositions;
    public Transform[] player3DominoesPositions;
    public Transform[] player4DominoesPositions;

    [Header("Player Icons")]
    public PlayerIcon player1Icon;
    public PlayerIcon player2Icon;
    public PlayerIcon player3Icon;
    public PlayerIcon player4Icon;

    [Header("Camera")]
    public Button cameraButton;
    public Sprite manualCameraSprite;
    public Sprite autoCameraSprite;
    private Camera MainCamera;
    private bool cameraFlag = false;

    [Header("Positions")]
    public GameObject player1Domino;
    public GameObject player2Domino;
    public GameObject player3Domino;
    public GameObject player4Domino;
    public Transform player1DominoesInPosition;
    public Transform player1DominoesOutPosition;
    public Transform player2DominoesInPosition;
    public Transform player2DominoesOutPosition;
    public Transform player3DominoesInPosition;
    public Transform player3DominoesOutPosition;
    public Transform player4DominoesInPosition;
    public Transform player4DominoesOutPosition;
    public Transform player1IconUpPosition;
    public Transform player1IconDownPosition;
    public Transform player2IconUpPosition;
    public Transform player2IconDownPosition;
    public Transform player3IconUpPosition;
    public Transform player3IconDownPosition;
    public Transform player4IconUpPosition;
    public Transform player4IconDownPosition;
    public Color blueColor;

    public static OnlineDominoesGamePlayManager _instance;
    public static OnlineDominoesGamePlayManager Instance
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
        PV = GetComponent<PhotonView>();

        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //SetCamera();
        MainCamera.gameObject.transform.position = new Vector3(360, 540, 0);
        dominoGlobalParent = GameObject.FindGameObjectWithTag("Global Parent").transform;
        dominoLocalParent = GameObject.FindGameObjectWithTag("Local Parent").transform;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.CurrentRoom.IsOpen)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                gameObject.GetComponent<OnlineGamePlayScreenHandler>().RoomKeyPanel.SetActive(false);
                Invoke(nameof(StartGame), 2f);
            }
        }

        if (PhotonNetwork.PlayerList.Length != PhotonNetwork.CurrentRoom.MaxPlayers && !PhotonNetwork.CurrentRoom.IsOpen && !gameObject.GetComponent<OnlineGamePlayScreenHandler>().ResultPanel.activeInHierarchy)
        {
            StopAllCoroutines();
            winner = PhotonNetwork.NickName;
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayResult(winner);

            if(PhotonNetwork.NickName == Nickname.PLAYER1.ToString())
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayOpponentLeftTheGameMessage(GameSetup.Instance.Player2Name + " left the game");
            else if (PhotonNetwork.NickName == Nickname.PLAYER2.ToString())
                UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayOpponentLeftTheGameMessage(GameSetup.Instance.Player1Name + " left the game");
        }
    }

    public void SetPosition(string nickName)
    {
        if (nickName == Nickname.PLAYER1.ToString())
        {
            player1Domino.transform.parent.position = player1DominoesOutPosition.position;
            player2Domino.transform.parent.position = player2DominoesInPosition.position;
            player3Domino.transform.parent.position = player3DominoesInPosition.position;
            player4Domino.transform.parent.position = player4DominoesInPosition.position;

            player1Icon.transform.position = player1IconUpPosition.position;
            player2Icon.transform.position = player2IconUpPosition.position;
            player3Icon.transform.position = player3IconUpPosition.position;
            player4Icon.transform.position = player4IconUpPosition.position;
        }
        else if (nickName == Nickname.PLAYER2.ToString())
        {
            player1Domino.transform.parent.position = player1DominoesInPosition.position;
            player2Domino.transform.parent.position = player2DominoesOutPosition.position;
            player3Domino.transform.parent.position = player3DominoesInPosition.position;
            player4Domino.transform.parent.position = player4DominoesInPosition.position;

            player1Icon.transform.position = player1IconDownPosition.position;
            player2Icon.transform.position = player2IconDownPosition.position;
            player3Icon.transform.position = player3IconDownPosition.position;
            player4Icon.transform.position = player4IconDownPosition.position;
        }
        else if (nickName == Nickname.PLAYER3.ToString())
        {
            player1Domino.transform.parent.position = player1DominoesInPosition.position;
            player2Domino.transform.parent.position = player2DominoesInPosition.position;
            player3Domino.transform.parent.position = player3DominoesOutPosition.position;
            player4Domino.transform.parent.position = player4DominoesInPosition.position;

            player1Icon.transform.position = player1IconDownPosition.position;
            player2Icon.transform.position = player2IconUpPosition.position;
            player3Icon.transform.position = player3IconUpPosition.position;
            player4Icon.transform.position = player4IconDownPosition.position;
        }
        else if (nickName == Nickname.PLAYER4.ToString())
        {
            player1Domino.transform.parent.position = player1DominoesInPosition.position;
            player2Domino.transform.parent.position = player2DominoesInPosition.position;
            player3Domino.transform.parent.position = player3DominoesInPosition.position;
            player4Domino.transform.parent.position = player4DominoesOutPosition.position;

            player1Icon.transform.position = player1IconDownPosition.position;
            player2Icon.transform.position = player2IconUpPosition.position;
            player3Icon.transform.position = player3IconUpPosition.position;
            player4Icon.transform.position = player4IconDownPosition.position;
        }
    }

    private void StartGame()
    {
        PV.RPC(nameof(RPC_StartGame), RpcTarget.All, GameManager.Instance.NumberOfPlayers, PreferenceManager.WinningPoints);
    }

    [PunRPC]
    private void RPC_StartGame(string gameType, int winningPoints)
    {
        WinningPoints = winningPoints;

        if (gameType == NumberOfPlayers.TwoPlayersGame.ToString())
            numberOfPlayers = 2;
        else if (gameType == NumberOfPlayers.ThreePlayersGame.ToString())
            numberOfPlayers = 3;
        else if (gameType == NumberOfPlayers.FourPlayersGame.ToString())
            numberOfPlayers = 4;

        SetDominoesTexture();

        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(GenerateRandomDominoesDeck), 2f);
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

    private void GenerateRandomDominoesDeck()
    {
        int j = 0;
        while (j != 28)
        {
            int randomNumber = Random.Range(0, 28);
            if (!Dominoes[randomNumber].isAddedToDominoesDeck)
            {
                PV.RPC(nameof(RPC_SetAllPlayerDominoDeck), RpcTarget.Others, randomNumber);
                Dominoes[randomNumber].isAddedToDominoesDeck = true;
                DominoesDeck.Add(Dominoes[randomNumber]);
                j++;
            }
        }

        PV.RPC(nameof(RPC_DistributeDominoesAndStartGame), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_SetAllPlayerDominoDeck(int randomNumber)
    {
        Dominoes[randomNumber].isAddedToDominoesDeck = true;
        DominoesDeck.Add(Dominoes[randomNumber]);
    }

    [PunRPC]
    private void RPC_DistributeDominoesAndStartGame()
    {
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
    }

    private void DistributeDominoesToPlayer1(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player1Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);

            if (PhotonNetwork.NickName == Nickname.PLAYER1.ToString())
            {
                player1Dominoes[player1Dominoes.Count - 1].domino.LeanMove(player1DominoesPositions[player1Dominoes.Count - 1].position, dominoAnimationSpeed);
                player1Dominoes[player1Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
            else
            {
                player1Dominoes[player1Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                player1Dominoes[player1Dominoes.Count - 1].domino.LeanMove(player1DominoesPositions[player1Dominoes.Count - 1].position, dominoAnimationSpeed);
                player1Dominoes[player1Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
        }
    }

    private void DistributeDominoesToPlayer2(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player2Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);

            if (PhotonNetwork.NickName == Nickname.PLAYER2.ToString())
            {
                player2Dominoes[player2Dominoes.Count - 1].domino.LeanMove(player2DominoesPositions[player2Dominoes.Count - 1].position, dominoAnimationSpeed);
                player2Dominoes[player2Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
            else
            {
                player2Dominoes[player2Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                player2Dominoes[player2Dominoes.Count - 1].domino.LeanMove(player2DominoesPositions[player2Dominoes.Count - 1].position, dominoAnimationSpeed);
                player2Dominoes[player2Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
        }
    }

    private void DistributeDominoesToPlayer3(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player3Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);

            if (PhotonNetwork.NickName == Nickname.PLAYER3.ToString())
            {
                player3Dominoes[player3Dominoes.Count - 1].domino.LeanMove(player3DominoesPositions[player3Dominoes.Count - 1].position, dominoAnimationSpeed);
                player3Dominoes[player3Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                player3Dominoes[player3Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
            else
            {
                player3Dominoes[player3Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                player3Dominoes[player3Dominoes.Count - 1].domino.LeanMove(player3DominoesPositions[player3Dominoes.Count - 1].position, dominoAnimationSpeed);
                player3Dominoes[player3Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                player3Dominoes[player3Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
        }
    }

    private void DistributeDominoesToPlayer4(int numberOfDominoes)
    {
        for (int i = 0; i < numberOfDominoes; i++)
        {
            player4Dominoes.Add(DominoesDeck[0]);
            DominoesDeck.Remove(DominoesDeck[0]);

            if (PhotonNetwork.NickName == Nickname.PLAYER4.ToString())
            {
                player4Dominoes[player4Dominoes.Count - 1].domino.LeanMove(player4DominoesPositions[player4Dominoes.Count - 1].position, dominoAnimationSpeed);
                player4Dominoes[player4Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                player4Dominoes[player4Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
            else
            {
                player4Dominoes[player4Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                player4Dominoes[player4Dominoes.Count - 1].domino.LeanMove(player4DominoesPositions[player4Dominoes.Count - 1].position, dominoAnimationSpeed);
                player4Dominoes[player4Dominoes.Count - 1].domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
                player4Dominoes[player4Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
            }
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
                        player1 .transform.GetChild(1).GetComponent<Text>().text = player1Points.ToString();
                        AudioManager.Instance.PlayPointsSound();
                    }

                    turn = Nickname.PLAYER1.ToString();
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

                    turn = Nickname.PLAYER2.ToString();
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

                        turn = Nickname.PLAYER3.ToString();
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

                        turn = Nickname.PLAYER4.ToString();
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
            if (turn == Nickname.PLAYER2.ToString())
                turn = Nickname.PLAYER1.ToString();
            else if (turn == Nickname.PLAYER1.ToString())
                turn = Nickname.PLAYER2.ToString();
        }
        else if (numberOfPlayers == 3)
        {
            if (turn == Nickname.PLAYER3.ToString())
                turn = Nickname.PLAYER1.ToString();
            else if (turn == Nickname.PLAYER1.ToString())
                turn = Nickname.PLAYER2.ToString();
            else if (turn == Nickname.PLAYER2.ToString())
                turn = Nickname.PLAYER3.ToString();
        }
        else if (numberOfPlayers == 4)
        {
            if (turn == Nickname.PLAYER3.ToString())
                turn = Nickname.PLAYER1.ToString();
            else if (turn == Nickname.PLAYER1.ToString())
                turn = Nickname.PLAYER4.ToString();
            else if (turn == Nickname.PLAYER4.ToString())
                turn = Nickname.PLAYER2.ToString();
            else if (turn == Nickname.PLAYER2.ToString())
                turn = Nickname.PLAYER3.ToString();
        }

        if (PhotonNetwork.NickName == turn && PreferenceManager.Vibration == VibrationState.On.ToString())
            Handheld.Vibrate();

        DisplayTurnImage();
        player1GamePlayButtons.SetActive(false);
        player2GamePlayButtons.SetActive(false);
        player3GamePlayButtons.SetActive(false);
        player4GamePlayButtons.SetActive(false);

        if (turn == Nickname.PLAYER1.ToString())
            CheckIfPlayer1NeedsDominoe();
        else if (turn == Nickname.PLAYER2.ToString())
            CheckIfPlayer2NeedsDominoe();
        else if (turn == Nickname.PLAYER3.ToString())
            CheckIfPlayer3NeedsDominoe();
        else if (turn == Nickname.PLAYER4.ToString())
            CheckIfPlayer4NeedsDominoe();
    }

    private void DisplayTurnImage()
    {
        player1Icon.timerImage.SetActive(false);
        player2Icon.timerImage.SetActive(false);
        player3Icon.timerImage.SetActive(false);
        player4Icon.timerImage.SetActive(false);

        if (turn == Nickname.PLAYER1.ToString())
            player1Icon.timerImage.SetActive(true);
        else if (turn == Nickname.PLAYER2.ToString())
            player2Icon.timerImage.SetActive(true);
        else if (turn == Nickname.PLAYER3.ToString())
            player3Icon.timerImage.SetActive(true);
        else if (turn == Nickname.PLAYER4.ToString())
            player4Icon.timerImage.SetActive(true);

        if (turn == PhotonNetwork.NickName)
            StartCoroutine(PlayerTurnCountDownCoroutine());
    }

    private IEnumerator PlayerTurnCountDownCoroutine()
    {
        yield return new WaitForSeconds(20);
        player1GamePlayButtons.SetActive(false);
        player2GamePlayButtons.SetActive(false);
        player3GamePlayButtons.SetActive(false);
        player4GamePlayButtons.SetActive(false);
        CPUDecisionOnTurn();
    }

    private void StopCountDown()
    {
        StopAllCoroutines();
        player1GamePlayButtons.SetActive(false);
        player2GamePlayButtons.SetActive(false);
        player3GamePlayButtons.SetActive(false);
        player4GamePlayButtons.SetActive(false);
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

                if ((upDominoes.Count + downDominoes.Count) == 6 && MainCamera.gameObject.transform.position.z > -250)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -250f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 7 && MainCamera.gameObject.transform.position.z > -400)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -400f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 8 && MainCamera.gameObject.transform.position.z > -550)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -550f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 9 && MainCamera.gameObject.transform.position.z > -700)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -700f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 10 && MainCamera.gameObject.transform.position.z > -850)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -850f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 11 && MainCamera.gameObject.transform.position.z > -1000)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1000f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 12 && MainCamera.gameObject.transform.position.z > -1150)
                    MainCamera.gameObject.LeanMove(new Vector3(MainCamera.gameObject.transform.position.x, MainCamera.gameObject.transform.position.y, -1150f), dominoAnimationSpeed);
                else if ((upDominoes.Count + downDominoes.Count) == 13 && MainCamera.gameObject.transform.position.z > -1300)
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
                if (PhotonNetwork.NickName == turn)
                    player1GamePlayButtons.SetActive(true);

                return;
            }
        }

        if (DominoesDeck.Count != 0)
        {
            if (player1Dominoes.Count <= 8)
            {
                player1Dominoes.Add(DominoesDeck[0]);
                DominoesDeck.Remove(DominoesDeck[0]);

                if (PhotonNetwork.NickName == Nickname.PLAYER1.ToString())
                {
                    player1Dominoes[player1Dominoes.Count - 1].domino.LeanMove(player1DominoesPositions[player1Dominoes.Count - 1].position, dominoAnimationSpeed);
                }
                else
                {
                    player1Dominoes[player1Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                    player1Dominoes[player1Dominoes.Count - 1].domino.LeanMove(player1DominoesPositions[player1Dominoes.Count - 1].position, dominoAnimationSpeed);
                }

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

    private void CheckIfPlayer2NeedsDominoe()
    {
        for (int i = 0; i < player2Dominoes.Count; i++)
        {
            if ((player2Dominoes[i].leftDots == numberOnDown || player2Dominoes[i].rightDots == numberOnDown) ||
                (player2Dominoes[i].leftDots == numberOnLeft || player2Dominoes[i].rightDots == numberOnLeft) ||
                (player2Dominoes[i].leftDots == numberOnUp || player2Dominoes[i].rightDots == numberOnUp) ||
                (player2Dominoes[i].leftDots == numberOnRight || player2Dominoes[i].rightDots == numberOnRight))
            {
                if (PhotonNetwork.NickName == turn)
                    player2GamePlayButtons.SetActive(true);

                return;
            }
        }

        if (DominoesDeck.Count != 0)
        {
            if (player2Dominoes.Count <= 8)
            {
                player2Dominoes.Add(DominoesDeck[0]);
                DominoesDeck.Remove(DominoesDeck[0]);

                if (PhotonNetwork.NickName == Nickname.PLAYER2.ToString())
                {
                    player2Dominoes[player2Dominoes.Count - 1].domino.LeanMove(player2DominoesPositions[player2Dominoes.Count - 1].position, dominoAnimationSpeed);
                }
                else
                {
                    player2Dominoes[player2Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                    player2Dominoes[player2Dominoes.Count - 1].domino.LeanMove(player2DominoesPositions[player2Dominoes.Count - 1].position, dominoAnimationSpeed);
                }

                player2Dominoes[player2Dominoes.Count - 1].domino.LeanRotate(player2DominoesPositions[player2Dominoes.Count - 1].eulerAngles, dominoAnimationSpeed);
                player2Dominoes[player2Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
                Invoke(nameof(ArangeDominoes), 1f);
                Invoke(nameof(CheckIfPlayer2NeedsDominoe), 2f);
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

    private void CheckIfPlayer3NeedsDominoe()
    {
        for (int i = 0; i < player3Dominoes.Count; i++)
        {
            if ((player3Dominoes[i].leftDots == numberOnDown || player3Dominoes[i].rightDots == numberOnDown) ||
                (player3Dominoes[i].leftDots == numberOnLeft || player3Dominoes[i].rightDots == numberOnLeft) ||
                (player3Dominoes[i].leftDots == numberOnUp || player3Dominoes[i].rightDots == numberOnUp) ||
                (player3Dominoes[i].leftDots == numberOnRight || player3Dominoes[i].rightDots == numberOnRight))
            {
                if (PhotonNetwork.NickName == turn)
                    player3GamePlayButtons.SetActive(true);
                return;
            }
        }

        if (DominoesDeck.Count != 0)
        {
            if (player3Dominoes.Count <= 8)
            {
                player3Dominoes.Add(DominoesDeck[0]);
                DominoesDeck.Remove(DominoesDeck[0]);

                if (PhotonNetwork.NickName == Nickname.PLAYER3.ToString())
                {
                    player3Dominoes[player3Dominoes.Count - 1].domino.LeanMove(player3DominoesPositions[player3Dominoes.Count - 1].position, dominoAnimationSpeed);
                }
                else
                {
                    player3Dominoes[player3Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                    player3Dominoes[player3Dominoes.Count - 1].domino.LeanMove(player3DominoesPositions[player3Dominoes.Count - 1].position, dominoAnimationSpeed);
                }

                player3Dominoes[player3Dominoes.Count - 1].domino.LeanRotate(player3DominoesPositions[player3Dominoes.Count - 1].eulerAngles, dominoAnimationSpeed);
                player3Dominoes[player3Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
                Invoke(nameof(ArangeDominoes), 1f);
                Invoke(nameof(CheckIfPlayer3NeedsDominoe), 2f);
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

    private void CheckIfPlayer4NeedsDominoe()
    {
        for (int i = 0; i < player4Dominoes.Count; i++)
        {
            if ((player4Dominoes[i].leftDots == numberOnDown || player4Dominoes[i].rightDots == numberOnDown) ||
                (player4Dominoes[i].leftDots == numberOnLeft || player4Dominoes[i].rightDots == numberOnLeft) ||
                (player4Dominoes[i].leftDots == numberOnUp || player4Dominoes[i].rightDots == numberOnUp) ||
                (player4Dominoes[i].leftDots == numberOnRight || player4Dominoes[i].rightDots == numberOnRight))
            {
                if (PhotonNetwork.NickName == turn)
                    player4GamePlayButtons.SetActive(true);
                return;
            }
        }

        if (DominoesDeck.Count != 0)
        {
            if (player4Dominoes.Count <= 8)
            {
                player4Dominoes.Add(DominoesDeck[0]);
                DominoesDeck.Remove(DominoesDeck[0]);

                if (PhotonNetwork.NickName == Nickname.PLAYER4.ToString())
                {
                    player4Dominoes[player4Dominoes.Count - 1].domino.LeanMove(player4DominoesPositions[player4Dominoes.Count - 1].position, dominoAnimationSpeed);
                }
                else
                {
                    player4Dominoes[player4Dominoes.Count - 1].domino.transform.Find("Back Side").SetAsLastSibling();
                    player4Dominoes[player4Dominoes.Count - 1].domino.LeanMove(player4DominoesPositions[player4Dominoes.Count - 1].position, dominoAnimationSpeed);
                }

                player4Dominoes[player4Dominoes.Count - 1].domino.LeanRotate(player4DominoesPositions[player4Dominoes.Count - 1].eulerAngles, dominoAnimationSpeed);
                player4Dominoes[player4Dominoes.Count - 1].domino.LeanScale(new Vector3(1, 1, 1), 0f);
                Invoke(nameof(ArangeDominoes), 1f);
                Invoke(nameof(CheckIfPlayer4NeedsDominoe), 2f);
            }
            else if (player4Dominoes.Count >= 9)
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

    public void OnPlayer1DominoButtonClick(int dominoNumber)
    {
        if (dominoNumber <= player1Dominoes.Count && PhotonNetwork.NickName == turn)
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

    public void OnPlayer2DominoButtonClick(int dominoNumber)
    {
        if (dominoNumber <= player2Dominoes.Count && PhotonNetwork.NickName == turn)
        {
            if (movedForward == dominoNumber)                                                                              //If pressed the same domino again
            {
                movedForward = -1;
                foreach (Domino singleDomino in player2Dominoes)
                    singleDomino.domino.LeanMove(player2DominoesPositions[player2Dominoes.IndexOf(singleDomino)].position, 0.2f);

                DominoPositionsManager.Instance.ResetGame();
            }
            else                                                                                                           //Pressed some other domino
            {
                movedForward = dominoNumber;

                //Move back all the player dominoes to original position
                foreach (Domino singleDomino in player2Dominoes)
                {
                    singleDomino.domino.LeanMove(player2DominoesPositions[player2Dominoes.IndexOf(singleDomino)].position, 0.2f);
                }

                GameObject domino = player2Dominoes[dominoNumber - 1].domino;
                domino.LeanMove(new Vector3(domino.transform.position.x, domino.transform.position.y - 10f, domino.transform.position.z), 0.2f);
                DisplayConnectionTargets(dominoNumber);
            }
        }
    }

    public void OnPlayer3DominoButtonClick(int dominoNumber)
    {
        if (dominoNumber <= player3Dominoes.Count && PhotonNetwork.NickName == turn)
        {
            if (movedForward == dominoNumber)                                                                              //If pressed the same domino again
            {
                movedForward = -1;
                foreach (Domino singleDomino in player3Dominoes)
                    singleDomino.domino.LeanMove(player3DominoesPositions[player3Dominoes.IndexOf(singleDomino)].position, 0.2f);

                DominoPositionsManager.Instance.ResetGame();
            }
            else                                                                                                           //Pressed some other domino
            {
                movedForward = dominoNumber;

                //Move back all the player dominoes to original position
                foreach (Domino singleDomino in player3Dominoes)
                {
                    singleDomino.domino.LeanMove(player3DominoesPositions[player3Dominoes.IndexOf(singleDomino)].position, 0.2f);
                }

                GameObject domino = player3Dominoes[dominoNumber - 1].domino;
                domino.LeanMove(new Vector3(domino.transform.position.x, domino.transform.position.y - 10f, domino.transform.position.z), 0.2f);
                DisplayConnectionTargets(dominoNumber);
            }
        }
    }

    public void OnPlayer4DominoButtonClick(int dominoNumber)
    {
        if (dominoNumber <= player4Dominoes.Count && PhotonNetwork.NickName == turn)
        {
            if (movedForward == dominoNumber)                                                                              //If pressed the same domino again
            {
                movedForward = -1;
                foreach (Domino singleDomino in player4Dominoes)
                    singleDomino.domino.LeanMove(player4DominoesPositions[player4Dominoes.IndexOf(singleDomino)].position, 0.2f);

                DominoPositionsManager.Instance.ResetGame();
            }
            else                                                                                                           //Pressed some other domino
            {
                movedForward = dominoNumber;

                //Move back all the player dominoes to original position
                foreach (Domino singleDomino in player4Dominoes)
                {
                    singleDomino.domino.LeanMove(player4DominoesPositions[player4Dominoes.IndexOf(singleDomino)].position, 0.2f);
                }

                GameObject domino = player4Dominoes[dominoNumber - 1].domino;
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

        if (turn == Nickname.PLAYER1.ToString())
        {
            if (player1Dominoes[dominoNumber - 1].rightDots == numberOnUp || player1Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = blueColor;
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
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = blueColor;
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
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = blueColor;
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
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Image>().color = blueColor;
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
        else if (turn == Nickname.PLAYER2.ToString())
        {
            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnUp || player2Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.UpTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnDown || player2Dominoes[dominoNumber - 1].leftDots == numberOnDown)
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.DownTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnRight || player2Dominoes[dominoNumber - 1].leftDots == numberOnRight)
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.RightTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player2Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
            {
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Image>().color = blueColor;
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
        else if (turn == Nickname.PLAYER3.ToString())
        {
            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnUp || player3Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.UpTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnDown || player3Dominoes[dominoNumber - 1].leftDots == numberOnDown)
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.DownTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnRight || player3Dominoes[dominoNumber - 1].leftDots == numberOnRight)
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.RightTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player3Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
            {
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Image>().color = blueColor;
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
        else if (turn == Nickname.PLAYER4.ToString())
        {
            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnUp || player4Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.UpTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.UpTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.UpTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnDown || player4Dominoes[dominoNumber - 1].leftDots == numberOnDown)
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.DownTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.DownTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.DownTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnRight || player4Dominoes[dominoNumber - 1].leftDots == numberOnRight)
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = blueColor;
                DisplayPoints(dominoNumber);
            }
            else
            {
                DominoPositionsManager.Instance.RightTarget.GetComponent<Animator>().enabled = false;
                DominoPositionsManager.Instance.RightTarget.LeanScale(new Vector3(1, 1, 1), 0f);
                DominoPositionsManager.Instance.RightTarget.GetComponent<Image>().color = Color.white;
                DisplayPoints(dominoNumber);
            }

            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player4Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
            {
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Animator>().enabled = true;
                DominoPositionsManager.Instance.LeftTarget.GetComponent<Image>().color = blueColor;
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
    }

    private void DisplayPoints(int dominoNumber)
    {
        if (turn == Nickname.PLAYER1.ToString())
        {
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
        else if (turn == Nickname.PLAYER2.ToString())
        {
            //Up
            if (player2Dominoes[dominoNumber - 1].leftDots == numberOnUp || player2Dominoes[dominoNumber - 1].rightDots == numberOnUp)
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

                if (player2Dominoes[dominoNumber - 1].leftDots == numberOnUp && player2Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += (player2Dominoes[dominoNumber - 1].rightDots + player2Dominoes[dominoNumber - 1].leftDots);
                else if (player2Dominoes[dominoNumber - 1].leftDots == numberOnUp)
                    upPoints += player2Dominoes[dominoNumber - 1].rightDots;
                else if (player2Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += player2Dominoes[dominoNumber - 1].leftDots;

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
            if (player2Dominoes[dominoNumber - 1].leftDots == numberOnDown || player2Dominoes[dominoNumber - 1].rightDots == numberOnDown)
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

                if (player2Dominoes[dominoNumber - 1].leftDots == numberOnDown && player2Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += (player2Dominoes[dominoNumber - 1].rightDots + player2Dominoes[dominoNumber - 1].leftDots);
                else if (player2Dominoes[dominoNumber - 1].leftDots == numberOnDown)
                    downPoints += player2Dominoes[dominoNumber - 1].rightDots;
                else if (player2Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += player2Dominoes[dominoNumber - 1].leftDots;

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
            if (player2Dominoes[dominoNumber - 1].leftDots == numberOnRight || player2Dominoes[dominoNumber - 1].rightDots == numberOnRight)
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

                if (player2Dominoes[dominoNumber - 1].leftDots == numberOnRight && player2Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += (player2Dominoes[dominoNumber - 1].rightDots + player2Dominoes[dominoNumber - 1].leftDots);
                else if (player2Dominoes[dominoNumber - 1].leftDots == numberOnRight)
                    rightPoints += player2Dominoes[dominoNumber - 1].rightDots;
                else if (player2Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += player2Dominoes[dominoNumber - 1].leftDots;

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
            if (player2Dominoes[dominoNumber - 1].leftDots == numberOnLeft || player2Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
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

                if (player2Dominoes[dominoNumber - 1].leftDots == numberOnLeft && player2Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += (player2Dominoes[dominoNumber - 1].rightDots + player2Dominoes[dominoNumber - 1].leftDots);
                else if (player2Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
                    leftPoints += player2Dominoes[dominoNumber - 1].rightDots;
                else if (player2Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += player2Dominoes[dominoNumber - 1].leftDots;

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
        else if (turn == Nickname.PLAYER3.ToString())
        {
            //Up
            if (player3Dominoes[dominoNumber - 1].leftDots == numberOnUp || player3Dominoes[dominoNumber - 1].rightDots == numberOnUp)
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

                if (player3Dominoes[dominoNumber - 1].leftDots == numberOnUp && player3Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += (player3Dominoes[dominoNumber - 1].rightDots + player3Dominoes[dominoNumber - 1].leftDots);
                else if (player3Dominoes[dominoNumber - 1].leftDots == numberOnUp)
                    upPoints += player3Dominoes[dominoNumber - 1].rightDots;
                else if (player3Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += player3Dominoes[dominoNumber - 1].leftDots;

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
            if (player3Dominoes[dominoNumber - 1].leftDots == numberOnDown || player3Dominoes[dominoNumber - 1].rightDots == numberOnDown)
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

                if (player3Dominoes[dominoNumber - 1].leftDots == numberOnDown && player3Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += (player3Dominoes[dominoNumber - 1].rightDots + player3Dominoes[dominoNumber - 1].leftDots);
                else if (player3Dominoes[dominoNumber - 1].leftDots == numberOnDown)
                    downPoints += player3Dominoes[dominoNumber - 1].rightDots;
                else if (player3Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += player3Dominoes[dominoNumber - 1].leftDots;

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
            if (player3Dominoes[dominoNumber - 1].leftDots == numberOnRight || player3Dominoes[dominoNumber - 1].rightDots == numberOnRight)
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

                if (player3Dominoes[dominoNumber - 1].leftDots == numberOnRight && player3Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += (player3Dominoes[dominoNumber - 1].rightDots + player3Dominoes[dominoNumber - 1].leftDots);
                else if (player3Dominoes[dominoNumber - 1].leftDots == numberOnRight)
                    rightPoints += player3Dominoes[dominoNumber - 1].rightDots;
                else if (player3Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += player3Dominoes[dominoNumber - 1].leftDots;

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
            if (player3Dominoes[dominoNumber - 1].leftDots == numberOnLeft || player3Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
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

                if (player3Dominoes[dominoNumber - 1].leftDots == numberOnLeft && player3Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += (player3Dominoes[dominoNumber - 1].rightDots + player3Dominoes[dominoNumber - 1].leftDots);
                else if (player3Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
                    leftPoints += player3Dominoes[dominoNumber - 1].rightDots;
                else if (player3Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += player3Dominoes[dominoNumber - 1].leftDots;

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
        else if (turn == Nickname.PLAYER4.ToString())
        {
            //Up
            if (player4Dominoes[dominoNumber - 1].leftDots == numberOnUp || player4Dominoes[dominoNumber - 1].rightDots == numberOnUp)
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

                if (player4Dominoes[dominoNumber - 1].leftDots == numberOnUp && player4Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += (player4Dominoes[dominoNumber - 1].rightDots + player4Dominoes[dominoNumber - 1].leftDots);
                else if (player4Dominoes[dominoNumber - 1].leftDots == numberOnUp)
                    upPoints += player4Dominoes[dominoNumber - 1].rightDots;
                else if (player4Dominoes[dominoNumber - 1].rightDots == numberOnUp)
                    upPoints += player4Dominoes[dominoNumber - 1].leftDots;

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
            if (player4Dominoes[dominoNumber - 1].leftDots == numberOnDown || player4Dominoes[dominoNumber - 1].rightDots == numberOnDown)
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

                if (player4Dominoes[dominoNumber - 1].leftDots == numberOnDown && player4Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += (player4Dominoes[dominoNumber - 1].rightDots + player4Dominoes[dominoNumber - 1].leftDots);
                else if (player4Dominoes[dominoNumber - 1].leftDots == numberOnDown)
                    downPoints += player4Dominoes[dominoNumber - 1].rightDots;
                else if (player4Dominoes[dominoNumber - 1].rightDots == numberOnDown)
                    downPoints += player4Dominoes[dominoNumber - 1].leftDots;

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
            if (player4Dominoes[dominoNumber - 1].leftDots == numberOnRight || player4Dominoes[dominoNumber - 1].rightDots == numberOnRight)
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

                if (player4Dominoes[dominoNumber - 1].leftDots == numberOnRight && player4Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += (player4Dominoes[dominoNumber - 1].rightDots + player4Dominoes[dominoNumber - 1].leftDots);
                else if (player4Dominoes[dominoNumber - 1].leftDots == numberOnRight)
                    rightPoints += player4Dominoes[dominoNumber - 1].rightDots;
                else if (player4Dominoes[dominoNumber - 1].rightDots == numberOnRight)
                    rightPoints += player4Dominoes[dominoNumber - 1].leftDots;

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
            if (player4Dominoes[dominoNumber - 1].leftDots == numberOnLeft || player4Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
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

                if (player4Dominoes[dominoNumber - 1].leftDots == numberOnLeft && player4Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += (player4Dominoes[dominoNumber - 1].rightDots + player4Dominoes[dominoNumber - 1].leftDots);
                else if (player4Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
                    leftPoints += player4Dominoes[dominoNumber - 1].rightDots;
                else if (player4Dominoes[dominoNumber - 1].rightDots == numberOnLeft)
                    leftPoints += player4Dominoes[dominoNumber - 1].leftDots;

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
    }

    public void ConnectDominoRequest(string side)
    {
        if (PhotonNetwork.NickName == turn)
        {
            if(PhotonNetwork.NickName == Nickname.PLAYER1.ToString())
                PV.RPC(nameof(RPC_MovePayer1Dominoe), RpcTarget.All, side, movedForward);
            else if (PhotonNetwork.NickName == Nickname.PLAYER2.ToString())
                PV.RPC(nameof(RPC_MovePayer2Dominoe), RpcTarget.All, side, movedForward);
            else if (PhotonNetwork.NickName == Nickname.PLAYER3.ToString())
                PV.RPC(nameof(RPC_MovePayer3Dominoe), RpcTarget.All, side, movedForward);
            else if (PhotonNetwork.NickName == Nickname.PLAYER4.ToString())
                PV.RPC(nameof(RPC_MovePayer4Dominoe), RpcTarget.All, side, movedForward);
        }
    }

    [PunRPC]
    private void RPC_MovePayer1Dominoe(string side, int dominoNumber)
    {
        if (side == "UP")
        {
            if (player1Dominoes[dominoNumber - 1].rightDots == numberOnUp || player1Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                StopCountDown();
                upDominoes.Add(player1Dominoes[dominoNumber - 1]);
                player1Dominoes.Remove(player1Dominoes[dominoNumber - 1]);
                upDominoes[upDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player1Dominoes[dominoNumber - 1].rightDots == numberOnDown || player1Dominoes[dominoNumber - 1].leftDots == numberOnDown)
            {
                StopCountDown();
                downDominoes.Add(player1Dominoes[dominoNumber - 1]);
                player1Dominoes.Remove(player1Dominoes[dominoNumber - 1]);
                downDominoes[downDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player1Dominoes[dominoNumber - 1].rightDots == numberOnRight || player1Dominoes[dominoNumber - 1].leftDots == numberOnRight)
            {
                StopCountDown();
                rightDominoes.Add(player1Dominoes[dominoNumber - 1]);
                player1Dominoes.Remove(player1Dominoes[dominoNumber - 1]);
                rightDominoes[rightDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player1Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player1Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
            {
                StopCountDown();
                leftDominoes.Add(player1Dominoes[dominoNumber - 1]);
                player1Dominoes.Remove(player1Dominoes[dominoNumber - 1]);
                leftDominoes[leftDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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

    [PunRPC]
    private void RPC_MovePayer2Dominoe(string side, int dominoNumber)
    {
        if (side == "UP")
        {
            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnUp || player2Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                StopCountDown();
                upDominoes.Add(player2Dominoes[dominoNumber - 1]);
                player2Dominoes.Remove(player2Dominoes[dominoNumber - 1]);
                upDominoes[upDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnDown || player2Dominoes[dominoNumber - 1].leftDots == numberOnDown)
            {
                StopCountDown();
                downDominoes.Add(player2Dominoes[dominoNumber - 1]);
                player2Dominoes.Remove(player2Dominoes[dominoNumber - 1]);
                downDominoes[downDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnRight || player2Dominoes[dominoNumber - 1].leftDots == numberOnRight)
            {
                StopCountDown();
                rightDominoes.Add(player2Dominoes[dominoNumber - 1]);
                player2Dominoes.Remove(player2Dominoes[dominoNumber - 1]);
                rightDominoes[rightDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player2Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player2Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
            {
                StopCountDown();
                leftDominoes.Add(player2Dominoes[dominoNumber - 1]);
                player2Dominoes.Remove(player2Dominoes[dominoNumber - 1]);
                leftDominoes[leftDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
    
    [PunRPC]
    private void RPC_MovePayer3Dominoe(string side, int dominoNumber)
    {
        if (side == "UP")
        {
            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnUp || player3Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                StopCountDown();
                upDominoes.Add(player3Dominoes[dominoNumber - 1]);
                player3Dominoes.Remove(player3Dominoes[dominoNumber - 1]);
                upDominoes[upDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnDown || player3Dominoes[dominoNumber - 1].leftDots == numberOnDown)
            {
                StopCountDown();
                downDominoes.Add(player3Dominoes[dominoNumber - 1]);
                player3Dominoes.Remove(player3Dominoes[dominoNumber - 1]);
                downDominoes[downDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnRight || player3Dominoes[dominoNumber - 1].leftDots == numberOnRight)
            {
                StopCountDown();
                rightDominoes.Add(player3Dominoes[dominoNumber - 1]);
                player3Dominoes.Remove(player3Dominoes[dominoNumber - 1]);
                rightDominoes[rightDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player3Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player3Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
            {
                StopCountDown();
                leftDominoes.Add(player3Dominoes[dominoNumber - 1]);
                player3Dominoes.Remove(player3Dominoes[dominoNumber - 1]);
                leftDominoes[leftDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
    
    [PunRPC]
    private void RPC_MovePayer4Dominoe(string side, int dominoNumber)
    {
        if (side == "UP")
        {
            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnUp || player4Dominoes[dominoNumber - 1].leftDots == numberOnUp)
            {
                StopCountDown();
                upDominoes.Add(player4Dominoes[dominoNumber - 1]);
                player4Dominoes.Remove(player4Dominoes[dominoNumber - 1]);
                upDominoes[upDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnDown || player4Dominoes[dominoNumber - 1].leftDots == numberOnDown)
            {
                StopCountDown();
                downDominoes.Add(player4Dominoes[dominoNumber - 1]);
                player4Dominoes.Remove(player4Dominoes[dominoNumber - 1]);
                downDominoes[downDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnRight || player4Dominoes[dominoNumber - 1].leftDots == numberOnRight)
            {
                StopCountDown();
                rightDominoes.Add(player4Dominoes[dominoNumber - 1]);
                player4Dominoes.Remove(player4Dominoes[dominoNumber - 1]);
                rightDominoes[rightDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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
            if (player4Dominoes[dominoNumber - 1].rightDots == numberOnLeft || player4Dominoes[dominoNumber - 1].leftDots == numberOnLeft)
            {
                StopCountDown();
                leftDominoes.Add(player4Dominoes[dominoNumber - 1]);
                player4Dominoes.Remove(player4Dominoes[dominoNumber - 1]);
                leftDominoes[leftDominoes.Count - 1].domino.transform.Find("Back Side").SetAsFirstSibling();
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

        if (turn == Nickname.PLAYER1.ToString())
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
        else if (turn == Nickname.PLAYER2.ToString())
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
        else if (turn == Nickname.PLAYER3.ToString())
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
        else if (turn == Nickname.PLAYER4.ToString())
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
            winner = Nickname.PLAYER1.ToString();
            Invoke(nameof(GameCompleted), 2f);
            return false;
        }
        else if (player2Points >= WinningPoints)
        {
            winner = Nickname.PLAYER2.ToString();
            Invoke(nameof(GameCompleted), 2f);
            return false;
        }
        else if (player3Points >= WinningPoints)
        {
            winner = Nickname.PLAYER3.ToString();
            Invoke(nameof(GameCompleted), 2f);
            return false;
        }
        else if (player4Points >= WinningPoints)
        {
            winner = Nickname.PLAYER4.ToString();
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

            foreach (Domino domino in player1Dominoes)
                player1Dominoes[player1Dominoes.IndexOf(domino)].domino.LeanMove(player1DominoesPositions[player1Dominoes.IndexOf(domino)].position, 0.3f);
        }

        if (player2Dominoes.Count > 0)
        {
            player2Dominoes = player2Dominoes.OrderByDescending(x => x.rightDots).ThenByDescending(x => x.leftDots).ToList();

            foreach (Domino domino in player2Dominoes)
                player2Dominoes[player2Dominoes.IndexOf(domino)].domino.LeanMove(player2DominoesPositions[player2Dominoes.IndexOf(domino)].position, 0.3f);
        }

        if (player3Dominoes.Count > 0)
        {
            player3Dominoes = player3Dominoes.OrderByDescending(x => x.rightDots).ThenByDescending(x => x.leftDots).ToList();

            foreach (Domino domino in player3Dominoes)
                player3Dominoes[player3Dominoes.IndexOf(domino)].domino.LeanMove(player3DominoesPositions[player3Dominoes.IndexOf(domino)].position, 0.3f);
        }

        if (player4Dominoes.Count > 0)
        {
            player4Dominoes = player4Dominoes.OrderByDescending(x => x.rightDots).ThenByDescending(x => x.leftDots).ToList();

            foreach (Domino domino in player4Dominoes)
                player4Dominoes[player4Dominoes.IndexOf(domino)].domino.LeanMove(player4DominoesPositions[player4Dominoes.IndexOf(domino)].position, 0.3f);
        }
    }

    private void CPUDecisionOnTurn()
    {
        if (turn == Nickname.PLAYER1.ToString())
        {
            for (int i = 0; i < player1Dominoes.Count; i++)                                                                    //RIGHT
            {
                if (player1Dominoes[i].leftDots == numberOnRight || player1Dominoes[i].rightDots == numberOnRight)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer1Dominoe), RpcTarget.All, "RIGHT", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player1Dominoes.Count; i++)                                                                    //UP
            {
                if (player1Dominoes[i].leftDots == numberOnUp || player1Dominoes[i].rightDots == numberOnUp)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer1Dominoe), RpcTarget.All, "UP", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player1Dominoes.Count; i++)                                                                    //LEFT
            {
                if (player1Dominoes[i].leftDots == numberOnLeft || player1Dominoes[i].rightDots == numberOnLeft)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer1Dominoe), RpcTarget.All, "LEFT", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player1Dominoes.Count; i++)                                                                    //DOWN
            {
                if (player1Dominoes[i].leftDots == numberOnDown || player1Dominoes[i].rightDots == numberOnDown)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer1Dominoe), RpcTarget.All, "DOWN", movedForward);
                    return;
                }
            }
        }
        else if (turn == Nickname.PLAYER2.ToString())
        {
            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //DOWN
            {
                if (player2Dominoes[i].leftDots == numberOnDown || player2Dominoes[i].rightDots == numberOnDown)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer2Dominoe), RpcTarget.All, "DOWN", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //LEFT
            {
                if (player2Dominoes[i].leftDots == numberOnLeft || player2Dominoes[i].rightDots == numberOnLeft)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer2Dominoe), RpcTarget.All, "LEFT", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //UP
            {
                if (player2Dominoes[i].leftDots == numberOnUp || player2Dominoes[i].rightDots == numberOnUp)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer2Dominoe), RpcTarget.All, "UP", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player2Dominoes.Count; i++)                                                                    //RIGHT
            {
                if (player2Dominoes[i].leftDots == numberOnRight || player2Dominoes[i].rightDots == numberOnRight)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer2Dominoe), RpcTarget.All, "RIGHT", movedForward);
                    return;
                }
            }
        }
        else if (turn == Nickname.PLAYER3.ToString())
        {
            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //UP
            {
                if (player3Dominoes[i].leftDots == numberOnUp || player3Dominoes[i].rightDots == numberOnUp)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer3Dominoe), RpcTarget.All, "UP", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //RIGHT
            {
                if (player3Dominoes[i].leftDots == numberOnRight || player3Dominoes[i].rightDots == numberOnRight)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer3Dominoe), RpcTarget.All, "RIGHT", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //DOWN
            {
                if (player3Dominoes[i].leftDots == numberOnDown || player3Dominoes[i].rightDots == numberOnDown)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer3Dominoe), RpcTarget.All, "DOWN", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player3Dominoes.Count; i++)                                                                    //LEFT
            {
                if (player3Dominoes[i].leftDots == numberOnLeft || player3Dominoes[i].rightDots == numberOnLeft)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer3Dominoe), RpcTarget.All, "LEFT", movedForward);
                    return;
                }
            }
        }
        else if (turn == Nickname.PLAYER4.ToString())
        {
            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //LEFT
            {
                if (player4Dominoes[i].leftDots == numberOnLeft || player4Dominoes[i].rightDots == numberOnLeft)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer4Dominoe), RpcTarget.All, "LEFT", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //DOWN
            {
                if (player4Dominoes[i].leftDots == numberOnDown || player4Dominoes[i].rightDots == numberOnDown)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer4Dominoe), RpcTarget.All, "DOWN", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //RIGHT
            {
                if (player4Dominoes[i].leftDots == numberOnRight || player4Dominoes[i].rightDots == numberOnRight)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer4Dominoe), RpcTarget.All, "RIGHT", movedForward);
                    return;
                }
            }

            for (int i = 0; i < player4Dominoes.Count; i++)                                                                    //UP
            {
                if (player4Dominoes[i].leftDots == numberOnUp || player4Dominoes[i].rightDots == numberOnUp)
                {
                    movedForward = i + 1;
                    PV.RPC(nameof(RPC_MovePayer4Dominoe), RpcTarget.All, "UP", movedForward);
                    return;
                }
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
            targetDomino.domino.LeanRotate(new Vector3(0, 0, 90), dominoAnimationSpeed);
            targetDomino.domino.LeanScale(new Vector3(1, 1, 1), 0f);
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
        Invoke(nameof(RoundReset), 1f);
        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(GenerateRandomDominoesDeck), 2f);
    }

    private void GameCompleted()
    {
        player1Icon.timerImage.SetActive(false);
        player2Icon.timerImage.SetActive(false);
        player3Icon.timerImage.SetActive(false);
        player4Icon.timerImage.SetActive(false);

        UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().DisplayResult(winner);
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
        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
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

        player1Icon.gameObject.SetActive(false);
        player2Icon.gameObject.SetActive(false);
        player3Icon.gameObject.SetActive(false);
        player4Icon.gameObject.SetActive(false);

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


