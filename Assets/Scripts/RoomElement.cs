using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomElement : MonoBehaviour
{
    public RoomInfo roomInfo;
    public Text roomStatus;
    public Text playersStatus;
    public GameObject joinButton;

    public void SetUpRoomInfo(RoomInfo _roomInfo)
    {
        roomInfo = _roomInfo;
        playersStatus.text = "PLAYERS: " + roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();

        if (_roomInfo.IsOpen)
        {
            roomStatus.text = "WAITING";
            joinButton.SetActive(true);
        }
        else
        {
            if (_roomInfo.PlayerCount == 1)
            {
                roomStatus.text = "GAME COMPLETED";
            }
            else if (_roomInfo.PlayerCount == 2)
            {
                roomStatus.text = "PLAYING";
            }
            joinButton.SetActive(false);
        }
    }

    public void OnJoinRoomButtonClick()
    {
        NetworkManager.Instance.JoinPublicRoom(roomInfo.Name);
        AudioManager.Instance.PlayButtonSound();
    }
}
