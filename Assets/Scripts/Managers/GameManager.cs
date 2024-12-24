using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent DisplayRegularAd = new UnityEvent();

    [Header("Multiplayer")]
    public string serverName;
    public bool connectedToPhotonServer;
    [HideInInspector] public bool tryToConnentToPhotonServer;
    public bool joinedRoom;

    [Header("Game")]
    public string NumberOfPlayers;
    public string GameType;

    public static GameManager _instance;
    public static GameManager Instance
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

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}

public enum Switch
{
    ON,
    OFF
}

public enum NumberOfPlayers
{
    TwoPlayersGame,
    ThreePlayersGame,
    FourPlayersGame
}

public enum GameType
{
    OfflinePlayerVsPlayer,
    OfflinePlayerVsAI,
    OnlinePlayerVsPlayer
}