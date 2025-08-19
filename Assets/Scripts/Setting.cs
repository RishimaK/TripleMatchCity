using DG.Tweening;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;

    public GameObject AnotherSetting;
    public GameObject AnotherPause;
    private GameObject Another;

    public GameObject Sound;
    public GameObject Music;
    public GameObject Vibration;
    public MainGame mainGame;
    private ChallengeGame challengeGame;
    public QuitGame quitGame;
    public GameObject ExitBtn;
    public MiniGame miniGame;

    public void Awake()
    {
    }

    public void CheckValue()
    {
        PlayerData data = saveDataJson.GetData();
        if(!data.Sound) ChangeState(Sound);
        if(!data.Music) ChangeState(Music);
        if(!data.Vibration) ChangeState(Vibration);
    }

    public void ChangeSound()
    {
        audioManager.PlaySFX("click");
        bool status = ChangeState(Sound);
        saveDataJson.SaveData("Sound", status);
        audioManager.ChangeStatusOfSound(status);
    }

    public void ChangeMusic()
    {
        audioManager.PlaySFX("click");
        bool status = ChangeState(Music);
        saveDataJson.SaveData("Music", status);
        audioManager.ChangeStatusOfMusic(status);
    }

    public void ChangeVibration()
    {
        audioManager.PlaySFX("click");
        bool status = ChangeState(Vibration);
        saveDataJson.SaveData("Vibration", status);
        // adsManager.ChangeStatusOfVibration(status);
    }

    // IEnumerator WaitFunction(float time)
    // {
    //     enabledToClick = false;
    //     yield return new WaitForSeconds(time);
    //     enabledToClick = true;
    // }

    bool ChangeState(GameObject target)
    {
        // StartCoroutine(WaitFunction(0.2f));

        GameObject on = target.transform.GetChild(0).gameObject;
        GameObject off = target.transform.GetChild(1).gameObject;
        // GameObject btn = target.transform.GetChild(2).gameObject;

        // int i = off.activeSelf ? 1: -1;

        // RectTransform btnRect = btn.GetComponent<RectTransform>();
        // btnRect.DOAnchorPos(new Vector2(i * 27, btnRect.anchoredPosition.y), 0.2f).OnComplete(() => {
            off.SetActive(!off.activeSelf);
            on.SetActive(!on.activeSelf);
        // });

        return !target.transform.GetChild(1).gameObject.activeSelf;
    }

    public void SetAnotherSetting()
    {
        Another = AnotherSetting;
        AnimationOpen();
    }

    public void SetAnotherPause()
    {
        challengeGame = mainGame.GetComponent<ChallengeGame>();

        if(mainGame.enabled) mainGame.PauseGameAction();
        else if(challengeGame.enabled) challengeGame.PauseGameAction();
        else if(miniGame.enabled) miniGame.PauseGameAction();
        Another = AnotherPause;
        AnimationOpen();
    }

    public void Exit()
    {
        audioManager.PlaySFX("click");
        CloseAnimation();

        if(Another == AnotherPause)
        {
            if(mainGame.enabled) mainGame.ContinueGameAction();
            else if(challengeGame.enabled) challengeGame.ContinueGameAction();
            else if(miniGame.enabled) miniGame.ContinueGameAction();
        }
    }

    public void Quit()
    {
        Another.SetActive(false);

        CloseAnimation("quit");
    }

    void AnimationOpen()
    {
        Another.SetActive(true);
        gameObject.SetActive(true);
        audioManager.PlaySFX("click");
        OpenAnimation();
    }

    void OpenAnimation()
    {
        Transform board = gameObject.transform.GetChild(1);
        Image boardImage = board.GetComponent<Image>();
        board.localScale = new Vector3(0.6f,0.6f,1f);
        // boardImage.color = new Color(1,1,1,0);
        board.DOPause();
        // boardImage.DOFade(1f, 0.2f).SetEase(Ease.OutCubic);
            PlayBtnAnimation();
        board.DOScale(new Vector3(1f,1f,1f), 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
        });
    }

    public void CloseAnimation(string txt = "")
    {
        Transform board = gameObject.transform.GetChild(1);
        board.DOPause();
        // board.GetComponent<Image>().DOFade(0f, 0.2f).SetEase(Ease.OutCubic);
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            Another.SetActive(false);
            gameObject.SetActive(false);
            if(txt == "quit")
            {
                if(mainGame.enabled) quitGame.OpenAnimation();
                else if(challengeGame.enabled) challengeGame.Exit();
                else miniGame.PlayCloundAnimation("ComeBackHome");
            }

            SetScaleBtn();
        });
    }

    public void PlayBtnAnimation()
    {
        int num = Sound.transform.parent.childCount;
        float delay = 0;
        Transform child = null;
        for(int i = 0; i < num; i++)
        {
            child = Sound.transform.parent.GetChild(i);
            child.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.1f;
        }

        
        for(int i = 0; i < 2; i++)
        {
            child = Another.transform.GetChild(i);
            child.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
            delay += 0.1f;
        }
        ExitBtn.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
    }

    public void SetScaleBtn()
    {
        int num = Sound.transform.parent.childCount;
        for(int i = 0; i < num; i++) { Sound.transform.parent.GetChild(i).localScale = Vector3.zero; }
        
        for(int i = 0; i < 2; i++) { Another.transform.GetChild(i).localScale = Vector3.zero; }
        ExitBtn.transform.localScale = Vector3.zero;
    }
}
