using TMPro;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyTimer : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public TextMeshProUGUI energyTxt;

    public GameObject AddBtnImage;
    private Button AddTimeBtn;
    private Coroutine coroutineCountdown;

    void Start()
    {
        CheckEnergy("OpenGame");
    }
    public void CheckEnergy(string txt = "")
    {
        int energy = (int)saveDataJson.GetData("Energy");
        energyTxt.text = $"{energy}";

        if(txt == "OpenGame" && energy < 5) CheckEnergyTimer(energy);
        else if(energy < 5) SetEnergyTimer();
        else saveDataJson.SaveData("EnergyTimer", null);
    }
    private string format = "dd/MM/yyyy HH:mm:ss";
    void CheckEnergyTimer(int energy)
    {
        DateTime currentTime = DateTime.Now;
        string energyTimer = (string)saveDataJson.GetData("EnergyTimer");
        if(energyTimer == null || energyTimer == "") return;

        TimeSpan timeGap = currentTime - DateTime.ParseExact(energyTimer, format, null);
        double secondsGap = Math.Round(timeGap.TotalSeconds);
        int energyColleted = 0;

        for (int i = 5 - energy; i >= 0; i--)
        {
            if(secondsGap >= 1800 * i)
            {
                energyColleted = i;
                break;
            }
        }

        if(energyColleted > 0)
        {
            energy += energyColleted;
            energyTxt.text = $"{energy}";
            saveDataJson.SaveData("Energy", energy);
            saveDataJson.SaveData("EnergyTimer", FormatDateTime(currentTime));
        }

        if(energy < 5) coroutineCountdown = StartCoroutine(CountdownToAddTime());
    }

    void SetEnergyTimer()
    {
        Transform textEnergyTimer = energyTxt.transform.parent.GetChild(2);
        if(textEnergyTimer.gameObject.activeSelf) return;

        DateTime currentTime = DateTime.Now;

        saveDataJson.SaveData("EnergyTimer", FormatDateTime(currentTime));

        coroutineCountdown = StartCoroutine(CountdownToAddTime());
    }

    IEnumerator CountdownToAddTime()
    {
        TextMeshProUGUI textEnergyTimer = 
            energyTxt.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>();
        textEnergyTimer.gameObject.SetActive(true);

        int time = UpdateTimeDisplay(textEnergyTimer);

        while (time >= 0)
        {
            yield return new WaitForSeconds(1f);
            time = UpdateTimeDisplay(textEnergyTimer);
        }

        saveDataJson.SaveData("Energy", (int)saveDataJson.GetData("Energy") + 1);
        textEnergyTimer.gameObject.SetActive(false);
        CheckEnergy();
    }

    int UpdateTimeDisplay(TextMeshProUGUI textEnergyTimer)
    {
        DateTime currentTime = DateTime.Now;
        string energyTimer = (string)saveDataJson.GetData("EnergyTimer");
        TimeSpan timeGap = currentTime - DateTime.ParseExact(energyTimer, format, null);
        int secondsGap = (int)Math.Round(timeGap.TotalSeconds);
        int time = 1800 - secondsGap;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (time < 60) textEnergyTimer.text = string.Format("{0:00}s", seconds);
        else textEnergyTimer.text = string.Format("{0:00}m{1:00}s", minutes, seconds);

        return time;
    }

    string FormatDateTime(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy HH:mm:ss");

    public void ResetCountDown()
    {
        // AddTimeBtn.enabled = false;
        // AddBtnImage.SetActive(false);
        StopCoroutine(coroutineCountdown);
        energyTxt.transform.parent.GetChild(2).gameObject.SetActive(false);
        saveDataJson.SaveData("EnergyTimer", null);
    }
}
