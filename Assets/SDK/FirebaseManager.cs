using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

public class FirebaseManager : MonoSingletonGlobal<FirebaseManager>
{
    private bool IsFirebaseInitialized = false;
    public bool IsNativeLanguage = true;
    public bool IsBannerCar = true;
    public bool IsBannerHome = true;
    public bool IsBannerPlay = true;
    public bool IsInterBack = true;
    public bool IsInterPlay = true;
    public bool IsInterTutorial = true;
    public bool IsRewardRefill = true;
    public CarReward[] carReward;

    public IEnumerator InitializedFirebase()
    {
        yield return null;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Initializer maybe
                var app = FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                // Finish initializer
                //Debug.Log("Firebase initialized");
                IsFirebaseInitialized = true;
            }
            else
            {
                IsFirebaseInitialized = true;
                Debug.LogError(string.Format("Dependency error: {0}", dependencyStatus)); // Firebase Unity SDK is not safe to use here.
            }

            InitializedFirebaseMessaging();
            InitializedFirebaseRemoteConfig();
        });
    }

    private void InitializedFirebaseMessaging()
    {
        //Debug.Log("Firebase Messaging initialized");
        Firebase.Messaging.FirebaseMessaging.TokenReceived += FirebaseMessaging_TokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += FirebaseMessaging_MessageReceived;
    }

    private void FirebaseMessaging_MessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }

    private void FirebaseMessaging_TokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + e.Token);
    }

    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    public Task InitializedFirebaseRemoteConfig()
    {
        //Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task => {
                string nativeLanguage = remoteConfig.GetValue("Native_language").StringValue;
                IsNativeLanguage = nativeLanguage == "0" ? false : true;
                string bannerCar = remoteConfig.GetValue("Banner_car").StringValue;
                IsBannerCar = bannerCar == "0" ? false : true;
                string bannerHome = remoteConfig.GetValue("Banner_home").StringValue;
                IsBannerHome = bannerHome == "0" ? false : true;
                string bannerPlay = remoteConfig.GetValue("Banner_play").StringValue;
                IsBannerPlay = bannerPlay == "0" ? false : true;
                string interBack = remoteConfig.GetValue("Inter_back").StringValue;
                IsInterBack = interBack == "0" ? false : true;
                string interPlay = remoteConfig.GetValue("Inter_play").StringValue;
                IsInterPlay = interPlay == "0" ? false : true;
                string interTutorial = remoteConfig.GetValue("Inter_tutorial").StringValue;
                IsInterTutorial = interTutorial == "0" ? false : true;
                string rewardRefill = remoteConfig.GetValue("Reward_refill").StringValue;
                IsRewardRefill = rewardRefill == "0" ? false : true;
                string rewardCar = remoteConfig.GetValue("Reward_car").StringValue;
                carReward = JsonHelper.getJsonArray<CarReward>(rewardCar);
                //Debug.Log(rewardCar);
                GameEvent.OnInitializedCarMethod();
                //Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            });
    }
    [System.Serializable]
    public class CarReward
    {
        public string id;
        public int open;
    }
}
