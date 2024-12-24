using UnityEngine;

public static class PreferenceManager
{
    public static string Username
    {
        get { return PlayerPrefs.GetString("USER_NAME", null); }

        set { PlayerPrefs.SetString("USER_NAME", value); }
    }

    public static string Email
    {
        get { return PlayerPrefs.GetString("EMAIL", null); }

        set { PlayerPrefs.SetString("EMAIL", value); }
    }

    public static int TotalGamesPlayed
    {
        get { return PlayerPrefs.GetInt("TOTAL_GAMES_PLAYED", 0); }

        set { PlayerPrefs.SetInt("TOTAL_GAMES_PLAYED", value); }
    }

    public static int Wins
    {
        get { return PlayerPrefs.GetInt("WINS", 0); }

        set { PlayerPrefs.SetInt("WINS", value); }
    }

    public static string DifficultyMode
    {
        get { return PlayerPrefs.GetString("DIFFICULTY_MODE", global::DifficultyMode.Easy.ToString()); }

        set { PlayerPrefs.SetString("DIFFICULTY_MODE", value); }
    }

    public static int WinningPoints
    {
        get { return PlayerPrefs.GetInt("WINNING_POINTS", 50); }

        set { PlayerPrefs.SetInt("WINNING_POINTS", value); }
    }

    public static int DominoSelected
    {
        get { return PlayerPrefs.GetInt("DOMINO_SELECTED", 1); }

        set { PlayerPrefs.SetInt("DOMINO_SELECTED", value); }
    }

    public static string RoomKey
    {
        get { return PlayerPrefs.GetString("ROOM_KEY", null); }

        set { PlayerPrefs.SetString("ROOM_KEY", value); }
    }

    public static float Music
    {
        get { return PlayerPrefs.GetFloat("MUSIC", 0.8f); }

        set { PlayerPrefs.SetFloat("MUSIC", value); }
    }

    public static float SFX
    {
        get { return PlayerPrefs.GetFloat("SFX", 0.8f); }

        set { PlayerPrefs.SetFloat("SFX", value); }
    }

    public static string Vibration
    {
        get { return PlayerPrefs.GetString("VIBRATION", VibrationState.On.ToString()); }

        set { PlayerPrefs.SetString("VIBRATION", value); }
    }
}
