using Photon.Pun;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            Invoke(nameof(SetupPlayer), 1f);
        }
    }

    private void SetupPlayer()
    {
        if (GameSetup.Instance.Slot1Avaialble == true)
            PhotonNetwork.NickName = Nickname.PLAYER1.ToString();
        else if (GameSetup.Instance.Slot2Avaialble == true)
            PhotonNetwork.NickName = Nickname.PLAYER2.ToString();
        else if (GameSetup.Instance.Slot3Avaialble == true)
            PhotonNetwork.NickName = Nickname.PLAYER3.ToString();
        else if (GameSetup.Instance.Slot4Avaialble == true)
            PhotonNetwork.NickName = Nickname.PLAYER4.ToString();

        if (PhotonNetwork.NickName == Nickname.PLAYER1.ToString() && NetworkManager.Instance.roomType == RoomType.Private.ToString())
        {
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().RoomKeyPanel.SetActive(true);
            UIManager.Instance.UIScreensReferences[GameScreens.OnlineGamePlayScreen].GetComponent<OnlineGamePlayScreenHandler>().roomKeyText.text = PreferenceManager.RoomKey; 
        }

        GameSetup.Instance.SetPlayerIcon(PhotonNetwork.NickName, gameObject.GetPhotonView().ViewID);
        OnlineDominoesGamePlayManager.Instance.SetPosition(PhotonNetwork.NickName);
    }
}
