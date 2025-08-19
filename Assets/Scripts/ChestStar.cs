using System.Collections;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestStar : MonoBehaviour
{
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public Button ClaimBtn;
    public Slider slider;
    public TextMeshProUGUI TextStar;

    public GameObject ChestAnim;
    public GameObject TakeRewardBtn;
    public Shop shop;
    public LevelChest levelChest;
    private bool enabledToClick = true;

    public void OpenAnimation()
    {
        // if(!enabledToClick) return;
        gameObject.SetActive(true);
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();

        CheckStar();
        enabledToClick = false;
        board.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            enabledToClick = true;
        });
    }

    public void CheckRewardChestStart(string txt = "")
    {
        int starChest = (int)saveDataJson.GetData("StarChest");
        if(starChest < 20) 
        {
            if(txt == "first") levelChest.CheckLevelReward();
            return;
        };

        PlayRewardAnimation();
    }

    void CheckStar()
    {
        float starChest = (int)saveDataJson.GetData("StarChest");

        starChest = starChest > 20 ? 20 : starChest;
        slider.value = starChest / 20;
        TextStar.text = $"{starChest}/20";
        // int RewardStarChest = (int)saveDataJson.GetData("RewardStarChest");
        // if(RewardStarChest > 0) ClaimBtn.interactable = true;
        // else ClaimBtn.interactable = false;
    }

    public void CloseAnimation(string txt = "")
    {
        if(!enabledToClick) return;
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        enabledToClick = false;
        board.DOPause();
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            if(txt == "")
            {
                gameObject.SetActive(false);
                enabledToClick = true;
            }
            else
            {
                board.gameObject.SetActive(false);
                ChestAnim.SetActive(true);
                ChestAnim.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);

                StartCoroutine(PlayChestAnimation());
            }
        });
    }

    public int AddMoreStar()
    {
        int starChest = (int)saveDataJson.GetData("StarChest");
        starChest += 3;
        // starChest = starChest > 20 ? starChest - 20 : starChest;
        saveDataJson.SaveData("StarChest", starChest);
        return starChest;
        // levelChest.CheckLevelReward();
        // saveDataJson.SaveData("RewardStarChest", (int)saveDataJson.GetData("RewardStarChest") + 1);
    }

    public void TakeReward()
    {
        CloseAnimation("reward");
    }

    void PlayNextChestAnimation()
    {
        ChestAnim.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "a2", true);
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
        ChestAnim.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "a1", false);
        // Invoke("PlayNextChestAnimation", 1f);
        Invoke("PlayChestSound", 0.75f);
        StartCoroutine(PlayChestAnimation());
    }

    IEnumerator PlayChestAnimation()
    {
        yield return new WaitForSeconds(1f);
        ChestAnim.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "a2", true);

        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;
        float posX = -canvasWidth / 4;

        for(int i = 0; i < 3; i++)
        {
            Transform child = ChestAnim.transform.GetChild(i);
            child.gameObject.SetActive(true);
            child.position = Vector3.zero;
            child.localScale = Vector3.zero;

            child.GetComponent<RectTransform>().DOAnchorPos(new Vector3(posX - posX * i, 480, 0), 0.5f).SetDelay(0.2f * i);
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
        audioManager.PlaySFX("click");
        shop.AddMoreStuff(50, 1);
        shop.SetTimeEndInfiniteEnergy();

        enabledToClick = false;
        for(int i = 0; i < 3; i++)
        {
            Transform child = ChestAnim.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
        ChestAnim.SetActive(false);
        TakeRewardBtn.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        // saveDataJson.SaveData("RewardStarChest", (int)saveDataJson.GetData("RewardStarChest") - 1);
        saveDataJson.SaveData("StarChest", (int)saveDataJson.GetData("StarChest") - 20);

        // OpenAnimation();
        gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        ChestAnim.SetActive(false);
        TakeRewardBtn.SetActive(false);
        levelChest.CheckLevelReward();
    }
}
