using UnityEngine;

public class AdmobManager : MonoBehaviour
{

    [SerializeField] private bool displayAd;

    [SerializeField] private string androidBannerAdId;
    [SerializeField] private string androidRegularAdId;

    [SerializeField] private string iOSBannerAdId;
    [SerializeField] private string iOSRegularAdId;

    private string bannerAdId;
    private string regularAdId;

    public static AdmobManager _instance;
    public static AdmobManager Instance
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
        if (Application.platform == RuntimePlatform.Android)
        {
            bannerAdId = androidBannerAdId;
            regularAdId = androidRegularAdId;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            bannerAdId = iOSBannerAdId;
            regularAdId = iOSRegularAdId;
        }

        RequestBannerAD();
        RequestRegularAD();
    }

    private void RequestBannerAD()
    {
        if (displayAd)
        {
           
        }
    }

    private void RequestRegularAD()
    {
        if (displayAd)
        {
           
        }
    }

    public void DisplayBannerAd()
    {
        if (displayAd)
        {
          
        }
    }

    public void HideBannerAd()
    {
        if (displayAd)
        {
          
        }
    }

    public void DisplayRegularAd()
    {
        if (displayAd)
        {
            Invoke(nameof(DisplayRegularAdDelay), 0.5f);
        }
    }

    private void DisplayRegularAdDelay()
    {
   
    }
}
