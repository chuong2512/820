
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    private PhotonView PV;

    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;

    [Space(20)]
    public bool Slot1Avaialble = true;
    public bool Slot2Avaialble = true;
    public bool Slot3Avaialble = true;
    public bool Slot4Avaialble = true;

    [Space(20)]
    public string Player1Name;
    public string Player2Name;
    public string Player3Name;
    public string Player4Name;

    [Space(20)]
    public string Player1DataBaseID;
    public string Player2DataBaseID;
    public string Player3DataBaseID;
    public string Player4DataBaseID;

    public static GameSetup _instance;
    public static GameSetup Instance
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
    }

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (Player1 == null)
        {

        }
    }

    public void SetPlayerIcon(string nickName, int photonViewID)
    {
        if (string.IsNullOrEmpty(PreferenceManager.Username))
        {
            if (nickName == Nickname.PLAYER1.ToString())
                PV.RPC(nameof(SetUpPlayerIcon), RpcTarget.AllBuffered, nickName, "Player 1", photonViewID, null);
            else if (nickName == Nickname.PLAYER2.ToString())
                PV.RPC(nameof(SetUpPlayerIcon), RpcTarget.AllBuffered, nickName, "Player 2", photonViewID, null);
            else if (nickName == Nickname.PLAYER3.ToString())
                PV.RPC(nameof(SetUpPlayerIcon), RpcTarget.AllBuffered, nickName, "Player 3", photonViewID, null);
            else if (nickName == Nickname.PLAYER4.ToString())
                PV.RPC(nameof(SetUpPlayerIcon), RpcTarget.AllBuffered, nickName, "Player 4", photonViewID, null);
        }
        else
        {
            PV.RPC(nameof(SetUpPlayerIcon), RpcTarget.AllBuffered, nickName, PreferenceManager.Username, photonViewID, 1);
        }

        if (PhotonNetwork.IsMasterClient)
            PV.RPC(nameof(RPC_ShareRoomKey), RpcTarget.OthersBuffered, PreferenceManager.RoomKey);
    }

    [PunRPC]
    private void SetUpPlayerIcon(string nickName, string playerName, int viewID, string dataBaseID)
    {
        if (nickName == Nickname.PLAYER1.ToString())
        {
            Player1Name = playerName;
            Player1DataBaseID = dataBaseID;
            PhotonView tempPhotonView = PhotonView.Find(viewID);
            Player1 = tempPhotonView.gameObject;
            Slot1Avaialble = false;

            OnlineDominoesGamePlayManager.Instance.player1Icon.gameObject.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player1Icon.nickName = nickName;
            OnlineDominoesGamePlayManager.Instance.player1Icon.playerNameText.text = playerName;
            OnlineDominoesGamePlayManager.Instance.player1.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player1.transform.GetChild(0).GetComponent<Text>().text = playerName + " :";
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player1.playerNameText.text = playerName;
            
            if(!string.IsNullOrEmpty(dataBaseID))
                FirebaseManager.Instance.GetAvatarPicture(dataBaseID, OnGetAvatarPictureOfPlayer1Complete);
        }
        else if (nickName == Nickname.PLAYER2.ToString())
        {
            Player2Name = playerName;
            Player2DataBaseID = dataBaseID;
            PhotonView tempPhotonView = PhotonView.Find(viewID);
            Player2 = tempPhotonView.gameObject;
            Slot2Avaialble = false;

            OnlineDominoesGamePlayManager.Instance.player2Icon.gameObject.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player2Icon.nickName = nickName;
            OnlineDominoesGamePlayManager.Instance.player2Icon.playerNameText.text = playerName;
            OnlineDominoesGamePlayManager.Instance.player2.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player2.transform.GetChild(0).GetComponent<Text>().text = playerName + " :";
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player2.playerNameText.text = playerName;

            if (!string.IsNullOrEmpty(dataBaseID))
                FirebaseManager.Instance.GetAvatarPicture(dataBaseID, OnGetAvatarPictureOfPlayer2Complete);
        }
        else if (nickName == Nickname.PLAYER3.ToString())
        {
            Player3Name = playerName;
            Player3DataBaseID = dataBaseID;
            PhotonView tempPhotonView = PhotonView.Find(viewID);
            Player3 = tempPhotonView.gameObject;
            Slot3Avaialble = false;

            OnlineDominoesGamePlayManager.Instance.player3Icon.gameObject.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player3Icon.nickName = nickName;
            OnlineDominoesGamePlayManager.Instance.player3Icon.playerNameText.text = playerName;
            OnlineDominoesGamePlayManager.Instance.player3.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player3.transform.GetChild(0).GetComponent<Text>().text = playerName + " :";
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player3.playerNameText.text = playerName;

            if (!string.IsNullOrEmpty(dataBaseID))
                FirebaseManager.Instance.GetAvatarPicture(dataBaseID, OnGetAvatarPictureOfPlayer3Complete);
        }
        else if (nickName == Nickname.PLAYER4.ToString())
        {
            Player4Name = playerName;
            Player4DataBaseID = dataBaseID;
            PhotonView tempPhotonView = PhotonView.Find(viewID);
            Player4 = tempPhotonView.gameObject;
            Slot4Avaialble = false;

            OnlineDominoesGamePlayManager.Instance.player4Icon.gameObject.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player4Icon.nickName = nickName;
            OnlineDominoesGamePlayManager.Instance.player4Icon.playerNameText.text = playerName;
            OnlineDominoesGamePlayManager.Instance.player4.SetActive(true);
            OnlineDominoesGamePlayManager.Instance.player4.transform.GetChild(0).GetComponent<Text>().text = playerName + " :";
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player4.playerNameText.text = playerName;

            if (!string.IsNullOrEmpty(dataBaseID))
                FirebaseManager.Instance.GetAvatarPicture(dataBaseID, OnGetAvatarPictureOfPlayer4Complete);
        }
    }

    [PunRPC]
    private void RPC_ShareRoomKey(string tableKey)
    {
        PreferenceManager.RoomKey = tableKey;
    }


    public void OnGetAvatarPictureOfPlayer1Complete(Texture2D texture, AvatarResult result)
    {
        if (result == AvatarResult.Successfull)
        {
            Debug.Log("Player 1 Avatar picture fetched Successfully!");
            OnlineDominoesGamePlayManager.Instance.player1Icon.avatarImage.SetImage(texture);
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player1.avatarImage.SetImage(texture);
        }
        else
        {
            Debug.Log("Error! Avatar picture not fetched");
        }
    }

    public void OnGetAvatarPictureOfPlayer2Complete(Texture2D texture, AvatarResult result)
    {
        if (result == AvatarResult.Successfull)
        {
            Debug.Log("Player 2 Avatar picture fetched Successfully!");
            OnlineDominoesGamePlayManager.Instance.player2Icon.avatarImage.SetImage(texture);
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player2.avatarImage.SetImage(texture);
        }
        else
        {
            Debug.Log("Error! Avatar picture not fetched");
        }
    }

    public void OnGetAvatarPictureOfPlayer3Complete(Texture2D texture, AvatarResult result)
    {
        if (result == AvatarResult.Successfull)
        {
            Debug.Log("Player 3 Avatar picture fetched Successfully!");
            OnlineDominoesGamePlayManager.Instance.player3Icon.avatarImage.SetImage(texture);
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player3.avatarImage.SetImage(texture);
        }
        else
        {
            Debug.Log("Error! Avatar picture not fetched");
        }
    }

    public void OnGetAvatarPictureOfPlayer4Complete(Texture2D texture, AvatarResult result)
    {
        if (result == AvatarResult.Successfull)
        {
            Debug.Log("Player 4 Avatar picture fetched Successfully!");
            OnlineDominoesGamePlayManager.Instance.player4Icon.avatarImage.SetImage(texture);
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().player4.avatarImage.SetImage(texture);
        }
        else
        {
            Debug.Log("Error! Avatar picture not fetched");
        }
    }
}

public enum Nickname
{
    PLAYER1,
    PLAYER2,
    PLAYER3,
    PLAYER4
}
