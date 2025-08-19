using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public TextMeshProUGUI goldInHome;

    public GameObject MessageDialog;

    public GameObject ListToolItem;


    [Header("Text")]
    public TextMeshProUGUI MagnetTxt;
    public TextMeshProUGUI UndoTxt;
    public TextMeshProUGUI CompassTxt;
    public TextMeshProUGUI FreezeTimerTxt;

    public TextMeshProUGUI ThunderTxt1;
    public TextMeshProUGUI ThunderTxt2;
    public TextMeshProUGUI AddTimeTxt1;
    public TextMeshProUGUI AddTimeTxt2;

    public TextMeshProUGUI HintTxt;
    public TextMeshProUGUI CompassInMiniGameTxt;


    void Start()
    {
        goldInHome.text = $"{(int)saveDataJson.GetData("Gold")}";
        CheckTimeEndInfiniteEnergy();

        if (coinsAmount == 0) coinsAmount = 10;

        initialPos = new Vector2[coinsAmount];
        initialRotation = new Quaternion[coinsAmount];
        
        for (int i = 0; i < pileOfCoins.transform.childCount; i++)
        {
            initialPos[i] = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            initialRotation[i] = pileOfCoins.transform.GetChild(i).GetComponent<RectTransform>().rotation;
        }

        if (toolsAmount == 0) toolsAmount = 6;

        initialPosTool = new Vector2[toolsAmount];
        
        for (int i = 0; i < pileOfTools.transform.childCount; i++)
        {
            initialPosTool[i] = pileOfTools.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
        }
    }

    public void AddPackage(string txt, int nunber)
    {
        audioManager.PlaySFX("collect");
        switch (txt)
        {
            case "starter_bundle":
                CountCoins(5000 * nunber);
                AddMoreStuff(0, 5 * nunber, 5 * nunber, 5 * nunber, 5 * nunber, 5 * nunber, 5 * nunber);
                AnimationCollectItem();
                break;
            case "island_bundle":
                CountCoins(500 * nunber);
                AddMoreStuff(0, 1 * nunber, 1 * nunber, 1 * nunber, 1 * nunber, 1 * nunber, 1 * nunber);
                AnimationCollectItem();
                break;
            case "city_bundle":
                CountCoins(1000 * nunber);
                AddMoreStuff(0, 2 * nunber, 2 * nunber, 2 * nunber, 2 * nunber, 2 * nunber, 2 * nunber);
                AnimationCollectItem();
                break;
            case "sale_classic":
                CountCoins(500 * nunber);
                break;
            case "mushroom_bundle":
                CountCoins(500 * nunber);
                break;
            default:
                break;
        }
    }

    public void AddMoreStuff(int gold = 0, int magnet = 0, int undo = 0, int compass = 0, int freezeTimer = 0, int thunder = 0, int addTime = 0,
        int hint = 0, int compassMiniGame = 0)
    {
        int Stuff = 0;

        if(gold != 0)
        {
            Stuff = (int)saveDataJson.GetData("Gold") + gold;
            saveDataJson.SaveData("Gold", Stuff);
            goldInHome.text = $"{Stuff}";
        }

        if(magnet != 0)
        {
            Stuff = (int)saveDataJson.GetData("Magnet") + magnet;
            saveDataJson.SaveData("Magnet", Stuff);
            MagnetTxt.text = $"{Stuff}";
        }

        if(undo != 0)
        {
            Stuff = (int)saveDataJson.GetData("Undo") + undo;
            saveDataJson.SaveData("Undo", Stuff);
            UndoTxt.text = $"{Stuff}";
        }

        if(compass != 0)
        {
            Stuff = (int)saveDataJson.GetData("Compass") + compass;
            saveDataJson.SaveData("Compass", Stuff);
            CompassTxt.text = $"{Stuff}";
        }

        if(freezeTimer != 0)
        {
            Stuff = (int)saveDataJson.GetData("FreezeTimer") + freezeTimer;
            saveDataJson.SaveData("FreezeTimer", Stuff);
            FreezeTimerTxt.text = $"{Stuff}";
        }

        if(thunder != 0)
        {
            Stuff = (int)saveDataJson.GetData("Thunder") + thunder;
            saveDataJson.SaveData("Thunder", Stuff);
            ThunderTxt1.text = $"{Stuff}";
            ThunderTxt2.text = $"{Stuff}";
        }


        if(addTime != 0)
        {
            Stuff = (int)saveDataJson.GetData("AddTime") + addTime;
            saveDataJson.SaveData("AddTime", Stuff);
            AddTimeTxt1.text = $"{Stuff}";
            AddTimeTxt2.text = $"{Stuff}";
        }

        if(hint != 0)
        {
            Stuff = (int)saveDataJson.GetData("Hint") + hint;
            saveDataJson.SaveData("Hint", Stuff);
            HintTxt.text = $"{Stuff}";
        }

        if(compassMiniGame != 0)
        {
            Stuff = (int)saveDataJson.GetData("CompassMiniGame") + compassMiniGame;
            saveDataJson.SaveData("CompassMiniGame", Stuff);
            CompassInMiniGameTxt.text = $"{Stuff}";
        }
    }

    public void AddMoreStuffInMiniGame(int hint = 0, int compassMiniGame = 0)
    {
        if(hint != 0)
        {
            int Hint = (int)saveDataJson.GetData("Hint") + hint;
            saveDataJson.SaveData("Hint", Hint);
        }

        if(compassMiniGame != 0)
        {
            int CompassMiniGame = (int)saveDataJson.GetData("CompassMiniGame") + compassMiniGame;
            saveDataJson.SaveData("CompassMiniGame", CompassMiniGame);
        }
    }


#region Message Popup
    public void ShowDialogNoCoin()
    {
        MessageDialog.SetActive(true);
        Image AdsMessageImage = MessageDialog.GetComponent<Image>();
        TextMeshProUGUI AdsMessageText = MessageDialog.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        AdsMessageText.text = "Not enough mushrooms";

        AdsMessageImage.DOPause();
        AdsMessageText.DOPause();

        AdsMessageImage.DOFade(0.83f, 0.5f);
        AdsMessageText.DOFade(1f, 0.5f);
        AdsMessageImage.DOFade(0f, 0.5f).SetDelay(2f);
        AdsMessageText.DOFade(0f, 0.5f).SetDelay(2f).OnComplete(() => {MessageDialog.SetActive(false);});
    }
#endregion

#region Handle Time Infinite Energy
    private bool isCountingDown = false;
    // private DateTime deadLine;
    public TextMeshProUGUI TextEnergy;
    string format = "dd/MM/yyyy HH:mm:ss";

    void CheckTimeEndInfiniteEnergy()
    {
        DateTime currentTime = DateTime.Now;

        string InfiniteEnergyEndTime = (string)saveDataJson.GetData("InfiniteEnergy");
        if(InfiniteEnergyEndTime != null && InfiniteEnergyEndTime != "")
        {
            if(DateTime.ParseExact(InfiniteEnergyEndTime, format, null) > currentTime)
            {
                // deadLine = DateTime.ParseExact(InfiniteEnergyEndTime, format, null);
                UpdateCountdownDisplay();
                TextEnergy.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);
                isCountingDown = true;
            }
            else saveDataJson.SaveData("InfiniteEnergy", null);
        }
    }

    public void SetTimeEndInfiniteEnergy()
    {
        DateTime currentTime = DateTime.Now;
        // deadLine = currentTime.AddMinutes(20);
        // string deadLineFormatted = FormatDateTime(deadLine);
        string InfiniteEnergyEndTime = (string)saveDataJson.GetData("InfiniteEnergy");
        if(InfiniteEnergyEndTime == null || InfiniteEnergyEndTime == "")
        {
            saveDataJson.SaveData("InfiniteEnergy", FormatDateTime(currentTime.AddMinutes(20)));
        }
        else 
        {
            saveDataJson.SaveData("InfiniteEnergy", FormatDateTime(DateTime.ParseExact(InfiniteEnergyEndTime, format, null).AddMinutes(20)));
        }
        TextEnergy.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true);

        UpdateCountdownDisplay();
        isCountingDown = true;
    }

    string FormatDateTime(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss");

    void Update()
    {
        if (isCountingDown)
        {
            UpdateCountdownDisplay();
        }
    }

    void UpdateCountdownDisplay()
    {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeRemaining = DateTime.ParseExact((string)saveDataJson.GetData("InfiniteEnergy"), format, null) - currentTime;

        string minutes = timeRemaining.Minutes > 9 ? $"{timeRemaining.Minutes}" : $"0{timeRemaining.Minutes}";
        string seconds = timeRemaining.Seconds > 9 ? $"{timeRemaining.Seconds}" : $"0{timeRemaining.Seconds}";

        if(timeRemaining.Hours == 0) TextEnergy.text = $"{minutes}m{seconds}s";
        else TextEnergy.text = $"{timeRemaining.Hours}h{minutes}m{seconds}s";

        if (timeRemaining.Minutes <= 0 && timeRemaining.Seconds < 0)
        {
            PauseCountdown();
            FinishCountdown();
            TextEnergy.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }

    public void StartCountdown()
    {
        isCountingDown = true;
    }

    public void PauseCountdown()
    {
        isCountingDown = false;

    }

    void FinishCountdown()
    {
        TextEnergy.text = $"{(int)saveDataJson.GetData("Energy")}";
        saveDataJson.SaveData("InfiniteEnergy", null);
        // Thêm logic khi đếm ngược kết thúc ở đây
    }
#endregion

#region Coin Collecting Animation
    [SerializeField] private GameObject pileOfCoins;
    [SerializeField] private Vector2[] initialPos;
    [SerializeField] private Quaternion[] initialRotation;
    [SerializeField] private int coinsAmount;

   public void CountCoins(float coin)
    {
        if(coin == 0) return;
        pileOfCoins.SetActive(true);
        var delay = 0f;
        Transform target = goldInHome.transform.parent.GetChild(0);
        Canvas coinCanvas = target.GetComponent<Canvas>();
        // coinCanvas.overrideSorting = true;

        float gold = (int)saveDataJson.GetData("Gold");
        int currentGold = (int)gold + (int)coin;
        saveDataJson.SaveData("Gold",  currentGold);

        int pileOfCoinsLength = pileOfCoins.transform.childCount;
        for (int i = 0; i < pileOfCoinsLength; i++)
        {
            gold += coin / 10;
            double currentCoin = Math.Round(gold, 0);
            Transform child = pileOfCoins.transform.GetChild(i);
            child.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            child.DOMove(target.position, 0.8f).SetDelay(delay + 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => {
                    audioManager.PlaySFX("collect");
                    goldInHome.text = $"{currentCoin}";
                    coinCanvas.overrideSorting = true;
                    target.GetChild(child.GetSiblingIndex()).gameObject.SetActive(true);
                    target.GetChild(child.GetSiblingIndex()).GetComponent<ParticleSystem>().Play();
                });

            child.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            
            if(i == pileOfCoinsLength - 1) child.DOScale(0f, 0.3f).SetDelay(delay + 1.5f)
                .SetEase(Ease.OutBack).OnComplete(() => {
                    pileOfCoins.SetActive(false);
                    target.GetChild(child.GetSiblingIndex()).gameObject.SetActive(false);
                    ResetCoin(child);
                    coinCanvas.overrideSorting = false;
                });
            else  child.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack)
                .OnComplete(() => { target.GetChild(child.GetSiblingIndex()).gameObject.SetActive(false); ResetCoin(child); });
            delay += 0.1f;

            target.DOScale(1.2f, 0.1f).SetLoops(10,LoopType.Yoyo).SetEase(Ease.InOutSine)
                .SetDelay(1.2f).OnComplete(() => target.localScale = new Vector3(1,1,1));
        }
    }

    void ResetCoin(Transform item)
    {
        int i = item.GetSiblingIndex();
        item.GetComponent<RectTransform>().anchoredPosition = initialPos[i];
        item.GetComponent<RectTransform>().rotation = initialRotation[i];
    }
#endregion


#region AnimationCollectItem
    [SerializeField] private GameObject pileOfTools;
    [SerializeField] private Vector2[] initialPosTool;
    [SerializeField] private int toolsAmount;

    void AnimationCollectItem()
    {
        pileOfTools.SetActive(true);
        var delay = 0f;

        int pileOfToolsLength = pileOfTools.transform.childCount;
        for (int i = 0; i < pileOfToolsLength; i++)
        {
            Transform child = pileOfTools.transform.GetChild(i);
            child.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            // child.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack).OnComplete(() => { 
            //     audioManager.PlaySFX("collect");
            //     ResetTool(child);
            // });
            if(i == pileOfToolsLength - 1) child.DOScale(0f, 0.3f).SetDelay(delay + 1.5f)
                .SetEase(Ease.OutBack).OnComplete(() => {
                    pileOfTools.SetActive(false);
                    audioManager.PlaySFX("collect");
                    ResetTool(child);
                });
            else  child.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack)
                .OnComplete(() => {audioManager.PlaySFX("collect"); ResetTool(child);});
            delay += 0.1f;
        }
    }

    void ResetTool(Transform item)
    {
        int i = item.GetSiblingIndex();
        item.GetComponent<RectTransform>().anchoredPosition = initialPosTool[i];
    }
#endregion
}
