using UnityEngine;

public class DominoPositionsManager : MonoBehaviour
{
    public Transform centerReferencePoint;
    public Transform upTargetDomino;
    public Transform downTargetDomino;
    public Transform rightTargetDomino;
    public Transform leftTargetDomino;

    public GameObject UpTarget;
    public GameObject DownTarget;
    public GameObject RightTarget;
    public GameObject LeftTarget;

    public static DominoPositionsManager _instance;
    public static DominoPositionsManager Instance
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

    public void OnTargetButtonClick(string side)
    {
        if(GameManager.Instance.joinedRoom)
            OnlineDominoesGamePlayManager.Instance.ConnectDominoRequest(side);
        else
            OfflineDominoesGamePlayManager.Instance.ConnectDominoRequest(side);
    }

    public void ResetGame()
    {
        upTargetDomino.LeanMove(centerReferencePoint.position, 0f);
        downTargetDomino.LeanMove(centerReferencePoint.position, 0f);
        rightTargetDomino.LeanMove(centerReferencePoint.position, 0f);
        leftTargetDomino.LeanMove(centerReferencePoint.position, 0f);

        UpTarget.LeanMove(centerReferencePoint.position, 0f);
        DownTarget.LeanMove(centerReferencePoint.position, 0f);
        RightTarget.LeanMove(centerReferencePoint.position, 0f);
        LeftTarget.LeanMove(centerReferencePoint.position, 0f);

        UpTarget.SetActive(false);
        DownTarget.SetActive(false);
        RightTarget.SetActive(false);
        LeftTarget.SetActive(false);
    }
}
