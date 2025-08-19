// using System;
using System.Collections;
// using System.Threading.Tasks;
using GoogleMobileAds.Api;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;
    public SkeletonGraphic AnimLoading;
    public GameObject home;
    public AdsManager adsManager;

    public GameObject UpdateVersion;
    public GameObject LegendarySUB;

    // private bool isStartGame = false;
    private bool loadedOpenAd = true;
    private bool checkUpdate = true;

    public GameObject Banner;

    void Start()
    {
        // StartCoroutine(ChekcStartGame(5f));
        Invoke("StartGame", 5.5f);
        PlayAnimLoading(5);
        // if(!Application.isEditor) StartCoroutine(GetVersion());

    }

    // async Task Task1 ()
    // {
    //     await Task.Delay(5000);

    //     // return null;
    // } 

    public void PlayAnimLoading(float time)
    {
        gameObject.SetActive(true);
        AnimLoading.AnimationState.ClearTracks();
        AnimLoading.startingAnimation = "";
        AnimLoading.AnimationState.SetAnimation(0, "animation", false);
        AnimLoading.timeScale = 1f * 6f / time;

        Invoke("ContinueGame", time + 0.5f);
    }

    void ContinueGame()
    {
        gameObject.SetActive(false);
    }

    IEnumerator GetVersion()
    {
        checkUpdate = false;
        UnityWebRequest www = UnityWebRequest.Get("https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects");
        yield return www.SendWebRequest();
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("web request failed");
            checkUpdate = true;
        }
        else
        {
            string responseText = www.downloadHandler.text;
            // if(www.downloadHandler.text == Application.version)
            if (responseText.Contains(Application.version))
            {
                Debug.Log("right version");
                checkUpdate = true;
            }
            else
            {
                Debug.Log("wrong version");
                UpdateVersion.SetActive(true);
            }
        }
    }

    public void UpdateVersionOfGame()
    {
        audioManager.PlaySFX("click");
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // private IEnumerator ChekcStartGame(float currentTime)
    // {
    //     while (currentTime > 0)
    //     {
    //         yield return new WaitForSeconds(1f); // Đợi 1 giây
    //         currentTime -= 1f;
    //         // Debug.Log(currentTime);
    //         if(isStartGame) break;
    //     }

    //     if(!isStartGame){
    //         StartGame();
    //     }
    // }

    public void SetOpenAdStatus(bool status)
    {
        loadedOpenAd = status;
    }

    public void ContinueLoading()
    {
        Invoke("StartGame", 1f);
    }

    void StartGame()
    {
        if(!checkUpdate){
            Invoke("StartGame", Time.deltaTime);
            return;
        }

        // adsManager.StopShowOpenAd();
        Banner.SetActive(true);
        adsManager.ChangeStatusStartGame();
        audioManager.PlayMusic();
        // isStartGame = true;
        // home.GetComponent<Home>().PlayMusic();
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            adsManager.LoadBannerAd();
        });
    }
}
