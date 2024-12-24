using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    public string username;
    public int totalGamesPlayed;
    public int wins;
    public string email;
    public string password;
}

[Serializable]
public class LeaderBoardUser
{
    public string userID;
    public string username;
    public int totalGamesPlayed;
    public int wins;
    public float winPercentage;
}

public class FirebaseManager : MonoBehaviour
{

    private const string usersTableName = "users";
    private const string avatarPicture = "avatarPicture";
    private const long maxDownloadSize = 52428800;

    public static FirebaseManager _instance;
    public static FirebaseManager Instance
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

    private void InitializeFirebase()
    {
        }

    public void LoginUser(string email, string password, Action<LoginResult> onCompleted)
    {
    }

    public void RegisterUser(User user, Action<RegisterResult, User> onCompleted)
    {
     }


    public void WriteUser(User user, Action<WriteUserResult, User> onCompleted)
    {
     }

    public void GetUser(string userId, Action<User, ReadUserResult> onCompleted)
    {
      }

    public void GetUsers(Action<List<LeaderBoardUser>, GetUsersResult> onCompleted)
    {
    }

    public void UpdateTotalGamesPlayed(int totalGamesPlayed, Action<UpdateData> onCompleted)
    {
     }

    public void UpdateWins(int wins, Action<UpdateData> onCompleted)
    {
    }

    public void UploadAvatarPicture(Texture2D avatar2DTexture, int width, int length, Action<AvatarResult, Texture2D, int, int> onCompleted)
    {
     }

    public void GetAvatarPicture(string userId, Action<Texture2D, AvatarResult> onCompleted)
    {
       
    }


}

public enum PermissionResult
{
    Denied,
    Confirmed
}

public enum LoginResult
{
    Error,
    Successfull
}

public enum RegisterResult
{
    Error,
    Successfull
}

public enum SearchResult
{
    Error,
    Successfull,
    Empty
}

public enum WriteUserResult
{
    Error,
    Successfull
}

public enum ReadUserResult
{
    Error,
    Successfull
}

public enum GetUsersResult
{
    Error,
    Successfull
}

public enum UpdateData
{
    Error,
    Successfull
}

public enum AvatarResult
{
    Error,
    Successfull
}