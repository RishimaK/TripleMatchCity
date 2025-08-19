using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopTool : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public AudioManager audioManager;
    private Transform ToolImage;
    private bool enabledToClick = true;
    // private 
    public TextMeshProUGUI priceTxt;
    public Shop shop;
    public SaveDataJson saveDataJson;
    public AdsManager adsManager;
    public MainGame mainGame;
    public ChallengeGame challengeGame;
    public MiniGame miniGame;

    public void Start()
    {
    }
    public void OpenAnimation(string tool)
    {
        SetShopTool(tool);
        audioManager.PlaySFX("click");
        if(mainGame.enabled) mainGame.PauseGameAction();
        else if(challengeGame.enabled) challengeGame.PauseGameAction();
        else if(miniGame.enabled) miniGame.PauseGameAction();

        gameObject.SetActive(true);
        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        coinText.text = $"{(int)saveDataJson.GetData("Gold")}";
        // boardImage.color = new Color(1,1,1,0);
        board.DOPause();
        // boardImage.DOFade(1f, 0.2f).SetEase(Ease.OutCubic);
        enabledToClick = false;
        board.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            enabledToClick = true;
        });
    }

    public void CloseAnimation()
    {
        Transform board = gameObject.transform.GetChild(1);
        audioManager.PlaySFX("click");
        board.DOPause();
        // board.GetComponent<Image>().DOFade(0f, 0.2f).SetEase(Ease.OutCubic);
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
            enabledToClick = true;
            ToolImage.gameObject.SetActive(false);
            if(mainGame.enabled) mainGame.ContinueGameAction();
            else if(challengeGame.enabled) challengeGame.ContinueGameAction();
            else if(miniGame.enabled) miniGame.ContinueGameAction();
        });
    }

    void SetShopTool(string tool)
    {
        ToolImage = transform.Find($"Board/{tool}");
        ToolImage.gameObject.SetActive(true);
        if(tool == "Magnet") priceTxt.text = "200";
        else if(tool == "Undo") priceTxt.text = "100";
        else if(tool == "Compass") priceTxt.text = "150";
        else if(tool == "FreezeTimer") priceTxt.text = "150";
        else if(tool == "Thunder") priceTxt.text = "125";
        else if(tool == "AddTime") priceTxt.text = "200";
        else if(tool == "Hint") priceTxt.text = "100";
        else if(tool == "CompassMiniGame") priceTxt.text = "250";
    }

    public void BuyTool()
    {
        if(!enabledToClick) return;
        int gold = (int)saveDataJson.GetData("Gold");
        int price = int.Parse(priceTxt.text);
        if(gold < price)
        {
            audioManager.PlaySFX("click");
            shop.ShowDialogNoCoin();
            return;
        }
        audioManager.PlaySFX("click");
        shop.AddMoreStuff(-int.Parse(priceTxt.text));
        coinText.text = $"{(int)saveDataJson.GetData("Gold")}";
        AddMoreTool();
    }

    public void AddMoreTool()
    {
        int manget = 0, undo = 0, compass = 0, freezeTimer = 0, thunder = 0, addTime = 0,
            hint = 0, compassMiniGame = 0;
        if(ToolImage.name == "Magnet") manget = 1;
        else if(ToolImage.name == "Undo") undo = 1;
        else if(ToolImage.name == "Compass") compass = 1;
        else if(ToolImage.name == "FreezeTimer") freezeTimer = 1;
        else if(ToolImage.name == "Thunder") thunder = 1;
        else if(ToolImage.name == "AddTime") addTime = 1;
        else if(ToolImage.name == "Hint") hint = 1;
        else if(ToolImage.name == "CompassMiniGame") compassMiniGame = 1;
        enabledToClick = false;

        shop.AddMoreStuff(0,manget, undo, compass,freezeTimer, thunder, addTime, hint, compassMiniGame);
        CloseAnimation();
    }

    public void WatchAds()
    {
        if(!enabledToClick) return;

        audioManager.PlaySFX("click");
        adsManager.ShowRewardedAd("BuyTool");
    }
}
