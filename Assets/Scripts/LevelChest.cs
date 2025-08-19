using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class LevelChest : MonoBehaviour
{
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public TextMeshProUGUI text;
    public GameObject ChestAnim;
    public GameObject TakeRewardBtn;
    public Shop shop;
    private bool enabledToClick = true;
    private int reward = 0;


    public void OpenAnimation()
    {
        audioManager.PlaySFX("click");
        gameObject.SetActive(true);
        enabledToClick = false;

        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();
        CheckLevel();
        board.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            enabledToClick = true;
        });
    }

    public void CheckLevel()
    {
        int level = (int)saveDataJson.GetData("OpenedMap");

        if(level < 10) level = 10;
        else if(level < 20) level = 20;
        else if(level < 30) level = 30;
        else if(level < 40) level = 40;
        else if(level < 50) level = 50;
        else if(level < 60) level = 60;
        text.text = $"Reach level {level} to open";
    }

    public void CloseAnimation()
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        enabledToClick = false;

        Transform board = gameObject.transform.GetChild(1);
        board.DOPause();
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

    public void CheckLevelReward()
    {
        int lv = (int)saveDataJson.GetData("OpenedMap");
        int saveReward = (int)saveDataJson.GetData("TakeLevelReward");

        if(lv == 10) reward = 1;
        else if(lv == 20) reward = 2;
        else if(lv == 30) reward = 3;
        else if(lv == 40) reward = 4;
        else if(lv == 50) reward = 5;
        else{
            saveDataJson.SaveData("TakeLevelReward", 0); 
            return;
        } 
        if(saveReward == -1) return;

        saveDataJson.SaveData("TakeLevelReward", reward);
        PlayRewardAnimation();
    }

    void PlayChestSound()
    {
        audioManager.PlaySFX("chest_open");
    }

    public void PlayRewardAnimation()
    {
        gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        ChestAnim.SetActive(true);
        ChestAnim.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
        Invoke("PlayChestSound", 0.75f);
        StartCoroutine(PlayChestAnimation());
    }

    IEnumerator PlayChestAnimation()
    {
        yield return new WaitForSeconds(1f);
        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;
        float posX = -canvasWidth / 5;
        List<int> rewards = new List<int> ();
        if(reward == 1) rewards = new List<int> {1,1,1,1};
        else if(reward == 2) rewards = new List<int> {2,1,1,2};
        else if(reward == 3) rewards = new List<int> {1,2,2,1};
        else if(reward == 4) rewards = new List<int> {1,1,2,2};
        else if(reward == 5) rewards = new List<int> {2,2,2,2};

        // List<int> rewards = new List<int> {1,1,2,2};


        for(int i = 0; i < 4; i++)
        {
            Transform child = ChestAnim.transform.GetChild(i);
            child.gameObject.SetActive(true);
            child.position = Vector3.zero;
            child.localScale = Vector3.zero;
            child.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"x{rewards[i]}";

            child.GetComponent<RectTransform>().DOAnchorPos(new Vector3(-canvasWidth / 2 - posX * (i + 1), 480, 0), 0.5f).SetDelay(0.2f * i);
            child.DOScale(new Vector3(1,1,1), 0.5f).SetDelay(0.2f * i);
        }

        yield return new WaitForSeconds(0.5f);

        TakeRewardBtn.transform.localScale = Vector3.zero;
        TakeRewardBtn.transform.DOScale(new Vector3(1,1,1), 0.5f).SetDelay(0.6f).SetEase(Ease.OutBack);
        TakeRewardBtn.SetActive(true);
        enabledToClick = true;
    }

    public void ClaimRewardBtn()
    {
        if(!enabledToClick) return;
        if(reward == 1) shop.AddMoreStuff(0,1,1,1,1);
        else if(reward == 2) shop.AddMoreStuff(0,2,1,1,2);
        else if(reward == 3) shop.AddMoreStuff(0,1,2,2,1);
        else if(reward == 4) shop.AddMoreStuff(0,1,1,2,2);
        else if(reward == 5) shop.AddMoreStuff(0,2,2,2,2);
        // shop.AddMoreStuff(0, reward, reward, reward, reward);
        // shop.SetTimeEndInfiniteEnergy();
        audioManager.PlaySFX("click");
        enabledToClick = false;
        for(int i = 0; i < 4; i++)
        {
            Transform child = ChestAnim.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
        ChestAnim.SetActive(false);
        TakeRewardBtn.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        // saveDataJson.SaveData("RewardStarChest", (int)saveDataJson.GetData("RewardStarChest") - 1);
        saveDataJson.SaveData("TakeLevelReward", -1);

        // OpenAnimation();
        gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        ChestAnim.SetActive(false);
        TakeRewardBtn.SetActive(false);
    }
}
