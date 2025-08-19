using System;
using UnityEngine;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System.Threading.Tasks;

[Serializable]
public class ConfigData {
    public int Banner;
    public Interstitial Interstitial;
    public int OpenAds;
}

[Serializable]
public class Interstitial
{
    public int start;
    public int show_position;
    public int time_wait_next_ad;
}

public class RemoteConfigAds : MonoBehaviour
{
    public ConfigData allConfigData;
    public AdsManager adsManager;
    private void Awake()
    {
        FetchDataAsync();
    }

    // void Start()
    // {
    //   Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener
    //     += ConfigUpdateListenerEventHandler;
    // }

    // // Handle real-time Remote Config events.
    // void ConfigUpdateListenerEventHandler(
    //     object sender, Firebase.RemoteConfig.ConfigUpdateEventArgs args) {
    //     if (args.Error != Firebase.RemoteConfig.RemoteConfigError.None) {
    //         Debug.Log(String.Format("Error occurred while listening: {0}", args.Error));
    //         return;
    //     }

    //     Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));
    //     // Activate all fetched values and then display a welcome message.
    //     remoteConfig.ActivateAsync().ContinueWithOnMainThread(
    //       task => {
    //           DisplayWelcomeMessage();
    //       });
    // }

    // void OnDestroy() {
    //     Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener
    //       -= ConfigUpdateListenerEventHandler;
    // }

    public Task FetchDataAsync()
    {
        // Debug.LogWarning("Fetching data...");
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
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
                string configData = remoteConfig.GetValue("AdMod_Config").StringValue;
                allConfigData = JsonUtility.FromJson<ConfigData>(configData);

                adsManager.DoneLoadFireBase();
            });
    }
}