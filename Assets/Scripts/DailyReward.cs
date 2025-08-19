using System;
using DG.Tweening;
using UnityEngine;

public class DailyReward : MonoBehaviour
{
    private int dailyRewardStack;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;

    public GameObject ListReward;
    public Shop shop;

    public GameObject ExitBtn;
    public GameObject ClaimBtn;
    public ChestStar chestStar;
    private bool isOpenApp = true;

    string FormatDateTime(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy");

    public void CheckDailylyReward (string txt = "")
    {
        DateTime currentTime = DateTime.Now;

        string dailyTime = (string)saveDataJson.GetData("DailyReward");
        string format = "dd/MM/yyyy";
        dailyRewardStack = (int)saveDataJson.GetData("DailyRewardStack");

        ExitBtn.SetActive(false);
        ClaimBtn.SetActive(true);

        if(dailyTime == null || dailyTime == "")
        {
            dailyRewardStack = 1;
            gameObject.SetActive(true);
        }
        else
        {
            TimeSpan difference = currentTime - DateTime.ParseExact(dailyTime, format, null);
            if(difference.Days == 1)
            {
                // dailyRewardStack = (int)saveDataJson.GetData("DailyRewardStack");
                dailyRewardStack = dailyRewardStack == 7 ? 1 : dailyRewardStack + 1;

                gameObject.SetActive(true);
            }
            else if (difference.Days > 1)
            {
                dailyRewardStack = 1;
                gameObject.SetActive(true);
            }
            else
            {
                ExitBtn.SetActive(true);
                ClaimBtn.SetActive(false);
                isOpenApp = false;
                chestStar.CheckRewardChestStart("first");
                SetScaleBtn();
                ExitBtn.transform.localScale = Vector3.zero;
            }
        }

        SetDailyReward();
    }

    void SetDailyReward()
    {
        for(int i = 0; i < dailyRewardStack; i++)
        {
            Transform child = ListReward.transform.GetChild(i);
            Transform reward = child.GetChild(1);
            child.GetChild(0).gameObject.SetActive(false);
            reward.gameObject.SetActive(true);

            reward.GetChild(reward.childCount - 1).gameObject.SetActive(true);
            if(i == dailyRewardStack - 1 && ClaimBtn.activeSelf)
                reward.GetChild(reward.childCount - 1).gameObject.SetActive(false);
        }
    }

    public void ClaimReward()
    {
        audioManager.PlaySFX("click");
        saveDataJson.SaveData("DailyRewardStack", dailyRewardStack);
        saveDataJson.SaveData("DailyReward", FormatDateTime(DateTime.Now));
        Transform reward = ListReward.transform.GetChild(dailyRewardStack - 1).GetChild(1);
        int gold = 0;
        int magnet = 0;
        int undo = 0;
        int freezeTimer = 0;
        switch (dailyRewardStack)
        {
            case 1: gold = 100; break;
            case 2:
                shop.SetTimeEndInfiniteEnergy();
                break;
            case 3: magnet = 2; break;
            case 4: undo = 2; break;
            case 5: gold = 200; break;
            case 6: freezeTimer = 3; break;
            case 7:
                gold = 300;
                shop.SetTimeEndInfiniteEnergy();
                magnet = 4;
            break;
        }

        reward.GetChild(reward.childCount - 1).gameObject.SetActive(true);
        shop.CountCoins(gold);
        shop.AddMoreStuff(0, magnet, undo, 0, freezeTimer);
        ExitBtn.SetActive(true);
        ClaimBtn.SetActive(false);
    }

    public void Exit()
    {
        CloseAnimation();
    }

    public void OpenAnimation()
    {
        gameObject.SetActive(true);
        ClaimBtn.SetActive(false);
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();

        PlayBtnAnimation();
        board.DOScale(new Vector3(1f,1f,1f), 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
        });
    }

    public void CloseAnimation()
    {
        Transform board = gameObject.transform.GetChild(1);
        audioManager.PlaySFX("click");
        board.DOPause();
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
            SetScaleBtn();

            if(isOpenApp)
            {
                chestStar.CheckRewardChestStart("first");
                isOpenApp = false;
            } 
        });
    }

    public void PlayBtnAnimation()
    {
        Transform ListRewardTranForm = ListReward.transform;
        float delay = 0;

        ListRewardTranForm.GetChild(4).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(6).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(3).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        ListRewardTranForm.GetChild(5).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(0).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        ListRewardTranForm.GetChild(2).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(1).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ExitBtn.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
    }

    public void SetScaleBtn()
    {
        int num = ListReward.transform.childCount;
        for(int i = 0; i < num; i++) { ListReward.transform.GetChild(i).localScale = Vector3.zero; }
        ExitBtn.transform.localScale = Vector3.zero;
    }
}
