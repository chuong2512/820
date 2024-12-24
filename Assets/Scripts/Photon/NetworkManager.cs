using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public string roomType = RoomType.None.ToString();

    public static NetworkManager _instance;
    public static NetworkManager Instance
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

    private void Update()
    {
        if (!GameManager.Instance.tryToConnentToPhotonServer)
        {
            ConnectToServer();
            GameManager.Instance.tryToConnentToPhotonServer = true;
            Invoke(nameof(CheckIsUserIsConnectedToMasterServer), 3f);
        }
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void CheckIsUserIsConnectedToMasterServer()
    {
        if (!PhotonNetwork.IsConnected)
            GameManager.Instance.connectedToPhotonServer = false;

        if (!GameManager.Instance.connectedToPhotonServer)
            GameManager.Instance.tryToConnentToPhotonServer = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player is connected to " + PhotonNetwork.CloudRegion + " server");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        GameManager.Instance.connectedToPhotonServer = true;
        GameManager.Instance.serverName = PhotonNetwork.CloudRegion;
        UIManager.Instance.DeactivateScreen(GameScreens.LoadingScreen);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected! Photon Server");
        base.OnDisconnected(cause);
        GameManager.Instance.connectedToPhotonServer = false;
        GameManager.Instance.tryToConnentToPhotonServer = false;
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().OnDisconnected();
    }




    #region Public Room

    //create a public room
    public void CreatePublicRoom(string roomKey, int roomSize)
    {
        if (GameManager.Instance.connectedToPhotonServer)
        {
            UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);

            if (roomSize == 2)
                GameManager.Instance.NumberOfPlayers = NumberOfPlayers.TwoPlayersGame.ToString();
            else if (roomSize == 3)
                GameManager.Instance.NumberOfPlayers = NumberOfPlayers.ThreePlayersGame.ToString();
            else if (roomSize == 4)
                GameManager.Instance.NumberOfPlayers = NumberOfPlayers.FourPlayersGame.ToString();

            roomType = RoomType.Public.ToString();
            RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
            PhotonNetwork.CreateRoom(roomKey, roomOptions);
        }
        else
        {
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage("Internet connection interrupted");
        }
    }

    //join a public room
    public void JoinPublicRoom(string roomKey)
    {
        if (GameManager.Instance.connectedToPhotonServer)
        {
            UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);
            roomType = RoomType.Public.ToString();
            PhotonNetwork.JoinRoom(roomKey);
        }
        else
        {
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage("Internet connection interrupted");
        }
    }

    //join a random room
    public void JoinRandomRoom()
    {
        roomType = RoomType.Public.ToString();
        PhotonNetwork.JoinRandomRoom();
    }

    //no random room is available
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random room but failed. There must be no open games available.");
        UIManager.Instance.DeactivateScreen(GameScreens.LoadingScreen);
    }

    #endregion





    #region Private Room

    //create a private room
    public void CreatePrivateRoom(string roomKey, int roomSize)
    {
        if (GameManager.Instance.connectedToPhotonServer)
        {
            UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);

            if (roomSize == 2)
                GameManager.Instance.NumberOfPlayers = NumberOfPlayers.TwoPlayersGame.ToString();
            else if (roomSize == 3)
                GameManager.Instance.NumberOfPlayers = NumberOfPlayers.ThreePlayersGame.ToString();
            else if (roomSize == 4)
                GameManager.Instance.NumberOfPlayers = NumberOfPlayers.FourPlayersGame.ToString();

            roomType = RoomType.Private.ToString();
            PreferenceManager.RoomKey = roomKey;
            RoomOptions roomOptions = new RoomOptions() { IsVisible = false, IsOpen = true, MaxPlayers = (byte)roomSize };
            PhotonNetwork.CreateRoom(roomKey, roomOptions);
        }
        else
        {
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage("Internet connection interrupted");
        }
    }

    public void JoinPrivateRoom(string roomKey)
    {
        if (GameManager.Instance.connectedToPhotonServer)
        {
            UIManager.Instance.ActivateScreen(GameScreens.LoadingScreen);
            roomType = RoomType.Private.ToString();
            PhotonNetwork.JoinRoom(roomKey);
        }
        else
        {
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage("Internet connection interrupted");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage(message);
        UIManager.Instance.DeactivateScreen(GameScreens.LoadingScreen);
    }

    #endregion





    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().DisplayErrorMessage(message);
        UIManager.Instance.DeactivateScreen(GameScreens.LoadingScreen);
    }


    //room is created and joined 
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined: Number of players in Room: " + PhotonNetwork.PlayerList.Length);
        GameManager.Instance.joinedRoom = true;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        RoomListingManager.Instance.DestroyRoomFromList(PreferenceManager.RoomKey);
        GameManager.Instance.joinedRoom = false;
    }
}

public enum RoomType
{
    None,
    Private,
    Public
}