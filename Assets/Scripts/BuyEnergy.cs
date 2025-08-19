using DG.Tweening;
using TMPro;
using UnityEngine;

public class BuyEnergy : MonoBehaviour
{
    private bool enabledToClick = true;
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;
    public AdsManager adsManager;
    public TextMeshProUGUI energyTxt;
    public EnergyTimer energyTimer;

    public Shop shop;

    public void OnpenDialog()
    {
        string infiniteEnergy = (string)saveDataJson.GetData("InfiniteEnergy");
        audioManager.PlaySFX("click");
        if((int)saveDataJson.GetData("Energy") == 5 || (infiniteEnergy != null && infiniteEnergy != ""))
        {
            adsManager.ShowAdsMessage("You have full energy");
            return;
        }
        enabledToClick = false;
        gameObject.SetActive(true);
        // gameObject.transform.GetChild(1).localScale = Vector3.zero;
        gameObject.transform.GetChild(1).DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            enabledToClick = true;
        });
    }

    public void CloseDialog()
    {
        audioManager.PlaySFX("click");
        enabledToClick = false;
        gameObject.transform.GetChild(1).DOScale(new Vector3(0f,0f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            gameObject.SetActive(false);
            enabledToClick = true;
        });
    }

    public void WatchAds()
    {
        if (!enabledToClick) return;
        adsManager.ShowRewardedAd("add 1 energy");
        audioManager.PlaySFX("click");

    }

    public void AddOneEnergy()
    {
        audioManager.PlaySFX("collect");
        int energy = (int)saveDataJson.GetData("Energy");
        energy++;
        saveDataJson.SaveData("Energy", energy);
        energyTxt.text = $"{energy}";

        if(energy == 5)
        {
            energyTimer.ResetCountDown();
            CloseDialog();
        }
    }

    public void RefillEnergy()
    {
        if (!enabledToClick) return;
        audioManager.PlaySFX("click");

        if((int)saveDataJson.GetData("Gold") < 100)
        {
            shop.ShowDialogNoCoin();
            return;
        }
        audioManager.PlaySFX("collect");

        shop.AddMoreStuff(-100);
        energyTxt.text = "5";
        saveDataJson.SaveData("Energy", 5);
        energyTimer.ResetCountDown();
        CloseDialog();
    }
}
