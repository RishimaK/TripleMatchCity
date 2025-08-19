using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Rate : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public GameObject ListStar;

    public GameObject DialogChestLv; 
    public GameObject DialogChestStart; 
    private bool IsPointExit = false;

    public void CheckRate(int mapnum)
    {
        if((bool)saveDataJson.GetData("Rate")) return;
        if(mapnum == 6 || mapnum == 10 || mapnum == 20 || mapnum == 30 || mapnum == 40 || mapnum == 50)
        {
            ShowThisObj();
            StartCoroutine(CheckDialog());
        }
    }

    void ShowThisObj ()
    {
        gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    void ShowThisObjChild ()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    IEnumerator CheckDialog ()
    {
        while (true)
        {
            yield return null;
            if (!DialogChestLv.activeSelf && !DialogChestStart.activeSelf)
            {
                ShowThisObjChild();
                gameObject.SetActive(true);
                break;
            }
        }
    }

    public void PointDown(Button btn)
    {
        IsPointExit = false;
        audioManager.PlaySFX("click");
        int index = btn.transform.GetSiblingIndex();
        for(int i = 0; i <= index; i++)
        {
            ListStar.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
    }

    public void PointUp(Button btn)
    {
        int index = btn.transform.GetSiblingIndex();

        if(!IsPointExit)
        {
            if(index >= 3)
            {
                adsManager.Rate();
                saveDataJson.SaveData("Rate", true);
            }
            gameObject.SetActive(false);
        }

        for(int i = 0; i <= index; i++)
        {
            ListStar.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }

    public void PointExit()
    {
        IsPointExit = true;
    }

    public void exit()
    {
        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
    }
}
