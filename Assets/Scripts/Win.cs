using Spine.Unity;
using UnityEngine;

public class Win : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioManager audioManager;
    public AdsManager adsManager;
    public ParticleSystem firework;
    public SkeletonGraphic animWin;
    public GameObject Bg;
    public MainGame mainGame;

    public Home home;
    public int CurrentMap;
    // private bool First = true;

    public ChallengeGame challengeGame;
    // public SkeletonDataAsset animationAsset;
    void Start()
    {
    }

    public void ResetAnim()
    {
        gameObject.SetActive(true);
        animWin.gameObject.SetActive(true);
        Invoke("EndReset", 0.5f);
    }

    public void EndReset()
    {
        gameObject.SetActive(false);
        animWin.gameObject.SetActive(false);
    }

    void ChangeFireWorkSize()
    {
        float scaleA = 1f / 15f * Camera.main.orthographicSize;
        firework.transform.localScale  = new Vector3(scaleA,scaleA,scaleA);
        for (int i = 0; i < 3; i++)
        {
            firework.transform.GetChild(i).localScale  = new Vector3(scaleA,scaleA,scaleA);
            if(i == 2)
            {
                for (int j = 0; j < 2; j++)
                {
                    firework.transform.GetChild(i).GetChild(j)
                        .localScale = new Vector3(scaleA,scaleA,scaleA);
                }
            }
        }

    }

    public void PlayAnimation(int mapNum)
    {
        CurrentMap = mapNum;
        ChangeFireWorkSize();

        gameObject.SetActive(true);
        firework.gameObject.SetActive(true);
        firework.Play();
        Bg.SetActive(true);

        Invoke("PlayAudioFireWork", 1);
        Invoke("PlayWinAnimation", 2f);
        Invoke("EndAnimation", 5.543f);
    }

    void PlayAudioFireWork()
    {
        audioManager.PlaySFX("fw");
    }

    void PlayWinAnimation()
    {
        // if(time == 2)
        // {
        // if(!First)
        // {
            // animWin.transform.SetParent(home.transform);
            // Invoke("ReturnAnim", 0.5f);
        // } else First = false;
        // animWin.skeletonDataAsset = animationAsset;
        animWin.AnimationState.SetAnimation(0, "animation", false);
        // }
        // else
        // {
        //     Debug.Log(time);
        //     animWin.AnimationState.SetAnimation(0, "animation", false);
        //     yield return new WaitForSeconds(time);
        // }

        // Debug.Log(transform.parent.parent.localScale.x * 100); //0.009851719
        // float scale = transform.parent.parent.localScale.x * 100 * 0.92f;
        // animWin.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);

        animWin.gameObject.SetActive(true);
    }

    void ReturnAnim()
    {
        animWin.transform.SetParent(transform);
    }

    void EndAnimation()
    {
        adsManager.ShowInterstitialAd(CurrentMap);
        Bg.SetActive(false);
        animWin.gameObject.SetActive(false);
        firework.gameObject.SetActive(false);
        firework.Stop();
        gameObject.SetActive(false);
        home.PlayChestAnimation(CurrentMap);

        if(mainGame.enabled) mainGame.Exit(CurrentMap);
        else if(challengeGame.enabled) challengeGame.Exit();
        // animWin.AnimationState.ClearTracks();
        // animWin.startingAnimation = "";
        // if(CurrentMap == 1) home.BtnMiniGame.interactable = true;
    }
}
