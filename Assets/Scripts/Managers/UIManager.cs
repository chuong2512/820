﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Screens
{
    public GameScreens ScreenName;
    public GameScreen GameScreen;
}

public class UIManager : MonoBehaviour
{
    public Camera MainCamera;
    public Canvas MyCanvas;
    public Screens[] UIScreens;
    public Dictionary<GameScreens, GameScreen> UIScreensReferences = new Dictionary<GameScreens, GameScreen>();

    public static UIManager _instance;
    public static UIManager Instance
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

    private void Start()
    {
        MyCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        MyCanvas.worldCamera = MainCamera;
        SpawnAllScreensAtStart();
    }

    private void SpawnAllScreensAtStart()
    {
        Canvas _canvas = Instantiate(MyCanvas);
        _canvas.transform.parent = gameObject.transform;
        foreach (var item in UIScreens)
        {
            GameScreen _obj = Instantiate(item.GameScreen, _canvas.gameObject.transform);
            _obj.MyName = item.ScreenName;
            UIScreensReferences.Add(item.ScreenName, _obj);
        }
        ActivateSpecificScreen(GameScreens.SplashScreen);
    }


    public void ActivateScreen(GameScreens screenName)
    {
        UIScreensReferences[screenName].gameObject.SetActive(true);
    }

    public void ActivateSpecificScreen(GameScreens screenName)
    {
        foreach (var item in UIScreens)
        {
            UIScreensReferences[item.ScreenName].gameObject.SetActive(false);
        }
        UIScreensReferences[screenName].gameObject.SetActive(true);
    }

    public void DeactivateScreen(GameScreens screenName)
    {
        UIScreensReferences[screenName].gameObject.SetActive(false);
    }

    public void HideAllScreens()
    {
        foreach (var item in UIScreens)
        {
            item.GameScreen.gameObject.SetActive(false);
        }
    }

    public void ShowAllScreens()
    {
        foreach (var item in UIScreens)
        {
            item.GameScreen.gameObject.SetActive(true);
        }
    }
}

public enum GameScreens
{
    SplashScreen,
    MainScreen,
    LoginAndRegistrationScreen,
    ProfileScreen,
    SettingsScreen,
    SelectGameTypeScreen,
    OnlineMultiplayerScreen,
    OfflineGamePlayScreen,
    OnlineGamePlayScreen,
    LeaderboardScreen,
    LoadingScreen,
    RulesScreen,
    QuitScreen,
}

