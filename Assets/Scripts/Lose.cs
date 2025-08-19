using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lose : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public AdsManager adsManager;
    public MainGame mainGame;
    public ChallengeGame challengeGame;

    public GameObject BoardTimeOut;
    public GameObject BoardOutOfSlots;
    public GameObject BoardLevelFailed;
    public Shop shop;

    public GameObject FrCoin;
    public SupportTools supportTools;

    public EnergyTimer energyTimer;

    public Image Tool1;
    public Image Tool2;
    public DialogPlay dialogPlay;

    private GameObject CurrentBoard;
    private int coin = 0;
    private int CurentMap;

    private bool isAddTime = false;
    private bool isThunder = false;
    void Start()
    {
    }

    public void SetValue(string txt, int mapnum = 0)
    {
        CurentMap = mapnum == 0 ? (int)saveDataJson.GetData("OpenedMap") : mapnum;
        if(mainGame.enabled) 
        {
            saveDataJson.SaveData("PlayAgain", true);
            mainGame.PauseGameAction();
        }
        else if(challengeGame.enabled) challengeGame.PauseGameAction();

        gameObject.SetActive(true);

        if (txt == "Quit")
        {
            ShowBoardLevelFailed();
            EnergyReduction();
            return;
        }
        else if(txt == "TimeOut") CurrentBoard = BoardTimeOut;
        else if(txt == "OutOfSlots") CurrentBoard = BoardOutOfSlots;

        CurrentBoard.SetActive(true);
        FrCoin.SetActive(true);
        coin = (int)saveDataJson.GetData("Gold");
        FrCoin.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{coin}";
    }

    void ShowBoardLevelFailed()
    {
        if(mainGame.enabled) adsManager.LogEvent($"LoseMap_{CurentMap}");

        BoardLevelFailed.SetActive(true);
        isAddTime = false;
        isThunder = false;
        if(CurentMap >= 6)
        {
            Tool1.sprite = dialogPlay.ThunderSprite;
            Tool1.GetComponent<Button>().enabled = true;
            Tool1.transform.GetChild(0).gameObject.SetActive(true);
            Tool1.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{(int)saveDataJson.GetData("Thunder")}";

            if(CurentMap >= 10)
            {
                Tool2.sprite = dialogPlay.TimeSprite;
                Tool2.GetComponent<Button>().enabled = true;
                Tool2.transform.GetChild(0).gameObject.SetActive(true);
                Tool2.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{(int)saveDataJson.GetData("AddTime")}";
            }
        }
    }

    public void BuyMoreTime()
    {
        audioManager.PlaySFX("click");
        if(coin >= 100)
        {
            shop.AddMoreStuff(-100);
            coin -= 100;
            FrCoin.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{coin}";
            AddMoreTime();
        }else shop.ShowDialogNoCoin();
    }

    public void AdsBuyMoreTime()
    {
        audioManager.PlaySFX("click");
        adsManager.ShowRewardedAd("addMoreTime");
    }

    public void AddMoreTime()
    {
        adsManager.LogEvent($"Use_Tool_Add_Time_When_Lose");
        CurrentBoard.SetActive(false);
        gameObject.SetActive(false);
        if(mainGame.enabled) 
        {
            mainGame.AddMoreTime(60);
            mainGame.ContinueGameAction();
        }
        else if(challengeGame.enabled) 
        {
            challengeGame.AddMoreTime(60);
            challengeGame.ContinueGameAction();
        }
    }

    public void ClearSlots()
    {
        adsManager.LogEvent($"Use_Tool_Clear_Slots_When_Lose");
        audioManager.PlaySFX("click");
        if(coin >= 100)
        {
            shop.AddMoreStuff(-100);
            coin -= 100;
            FrCoin.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{coin}";
            ClearAllSlots();
        }
        else shop.ShowDialogNoCoin();
    }

    public void AdsClearAllSlots()
    {
        audioManager.PlaySFX("click");
        adsManager.ShowRewardedAd("clearAllSlots");
    }

    public void ClearAllSlots()
    {
        supportTools.ClearAllSlots();
        CurrentBoard.SetActive(false);
        gameObject.SetActive(false);
        if(mainGame.enabled) mainGame.ContinueGameAction();
        else if(challengeGame.enabled) challengeGame.ContinueGameAction();
    }

    public void EnergyReduction ()
    {
        string infiniteEnergy = (string)saveDataJson.GetData("InfiniteEnergy");

        if(infiniteEnergy == null || infiniteEnergy == "")
        {
            saveDataJson.SaveData("Energy", (int)saveDataJson.GetData("Energy") - 1);
            energyTimer.CheckEnergy();
        }
    }

    public void ExitCurrentBoard()
    {
        audioManager.PlaySFX("click");
        CurrentBoard.SetActive(false);
        EnergyReduction();

        ShowBoardLevelFailed();
        FrCoin.SetActive(false);
    }

    public void BackToHomeScreen()
    {
        audioManager.PlaySFX("click");
        adsManager.ShowInterstitialAd(CurentMap);
        gameObject.SetActive(false);
        BoardLevelFailed.SetActive(false);
        if(mainGame.enabled) mainGame.Exit();
        else if(challengeGame.enabled) challengeGame.Exit("deleteValue");
    }

    public void PlayAgain()
    {
        int energy = (int)saveDataJson.GetData("Energy");
        string infiniteEnergy = (string)saveDataJson.GetData("InfiniteEnergy");
        audioManager.PlaySFX("click");

        if(energy <= 0 && (infiniteEnergy == null || infiniteEnergy == "")) 
        {
            adsManager.ShowAdsMessage("Out of energy");
            return;
        };
        adsManager.ShowInterstitialAd(CurentMap);
        gameObject.SetActive(false);
        BoardLevelFailed.SetActive(false);
        if(mainGame.enabled)
        {
            mainGame.ResetData();
            mainGame.LoadGame(isThunder, isAddTime);
        }
        else if(challengeGame.enabled)
        {
            challengeGame.ResetData();
            challengeGame.DeleteValue();
            challengeGame.LoadGame(CurentMap, isThunder, isAddTime);
        }
    }

    public void AddTime()
    {
        if(Tool2.sprite == dialogPlay.ChooseTimeSprite)
        {
            audioManager.PlaySFX("click");
            Tool2.sprite = dialogPlay.TimeSprite;
            Tool2.transform.GetChild(0).gameObject.SetActive(true);
            isAddTime = false;
            return;
        }

        int addTime = (int)saveDataJson.GetData("AddTime");
        if(addTime <= 0) return;
        audioManager.PlaySFX("click");
        Tool2.transform.GetChild(0).gameObject.SetActive(false);
        Tool2.sprite = dialogPlay.ChooseTimeSprite;
        isAddTime = true;
    }

    public void UseThunder ()
    {
        if(Tool1.sprite == dialogPlay.ChooseThunderSprite)
        {
            audioManager.PlaySFX("click");
            Tool1.transform.GetChild(0).gameObject.SetActive(true);
            Tool1.sprite = dialogPlay.ThunderSprite;
            isThunder = false;
            return;
        }

        int thunder = (int)saveDataJson.GetData("Thunder");
        if(thunder <= 0) return;
        audioManager.PlaySFX("click");
        Tool1.transform.GetChild(0).gameObject.SetActive(false);
        Tool1.sprite = dialogPlay.ChooseThunderSprite;
        isThunder = true;
    }
}
