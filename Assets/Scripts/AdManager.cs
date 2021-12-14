using UnityEngine;
using GoogleMobileAds.Api;
using System;

public enum BannerSize
{
    Banner,
    MediumRectangle,
    IABBanner,
    Leaderboard,
    SmartBanner,
}

public class AdManager : MonoBehaviour
{
    [Header("Banner")]
    [SerializeField] private string adUnitIdAndroidBanner;
    [SerializeField] private string adUnitIdiOSBanner;
    [SerializeField] private AdPosition bannerPosition;
    [SerializeField] private BannerSize bannerSize;
    private BannerView bannerView;


    [Header("Interstitial")]
    [SerializeField] private string adUnitIdAndroidInterstitial;
    [SerializeField] private string adUnitIdiOSInterstitial;
    private InterstitialAd interstitial;

    [Header("Reward")]
    [SerializeField] private string adUnitIdAndroidReward;
    [SerializeField] private string adUnitIdiOSReward;
    private RewardedAd rewardedAd;


    #region UNITY_METHODS

    void Start()
    {
        RequestInterstitial();
        RequestRewarded();
    }


    #endregion

    #region BANNER_ADS

    private AdSize GetAdSize()
    {
        switch (bannerSize)
        {
            case BannerSize.Banner:
                return AdSize.Banner;
                break;
            case BannerSize.IABBanner:
                return AdSize.IABBanner;
                break;
            case BannerSize.Leaderboard:
                return AdSize.Leaderboard;
                break;
            case BannerSize.MediumRectangle:
                return AdSize.MediumRectangle;
                break;
            case BannerSize.SmartBanner:
                return AdSize.SmartBanner;
                break;
            default:
                return AdSize.Banner;
                break;
        }
    }
    private void RequestBanner()
    {

#if UNITY_ANDROID
        string adUnitId = adUnitIdAndroidBanner;
#elif UNITY_IPHONE
            string adUnitId = adUnitIdiOSBanner;
#else
            string adUnitId = "unexpected_platform";
#endif


        this.bannerView = new BannerView(adUnitId, GetAdSize(), bannerPosition);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    #endregion

    #region Interstitial_ADS
    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = adUnitIdAndroidInterstitial;
#elif UNITY_IPHONE
        string adUnitId = adUnitIdiOSInterstitial;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }
    #endregion

    #region RewardedAd

    private void RequestRewarded()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = adUnitIdAndroidReward;
#elif UNITY_IPHONE
            adUnitId = adUnitIdiOSReward;
#else
            adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);
    }



    #endregion

    #region PUBLIC_METHODS
    public void ShowBanner()
    {
        RequestBanner();
    }

    public void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        else
        {
            RequestInterstitial();
        }
    }

    public void ShowReward()
    {
        if (rewardedAd.IsLoaded())
        {
            rewardedAd.Show();
        }
        else
        {
            RequestRewarded();
        }
    }
    #endregion

    #region Events
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleFailedToReceiveAd event received with message: "
              + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        if (sender.Equals(interstitial))
        {
            RequestInterstitial();
            Debug.Log("Close Interstitial");
        }
        print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        print("HandleAdLeavingApplication event received");
    }
    #endregion

    #region Reward_Event
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        print("HandleRewardedAdLoaded event received");
    }

    private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs e)
    {
        print(
            "HandleRewardedAdFailedToLoad event received with message: "
            + e.Message);
        RequestRewarded();
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        print(
              "HandleRewardedAdFailedToShow event received with message: "
              + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        print("HandleRewardedAdClosed event received");
        RequestRewarded();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        print(
              "HandleRewardedAdRewarded event received for "
              + amount.ToString() + " " + type);
    }


    #endregion
}
