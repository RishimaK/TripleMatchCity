using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Sale : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;

    public GameObject DialogChestLv; 
    public GameObject DialogChestStart; 
    public GameObject DialogRate; 
    private bool allowToOpen = true;
    public void OpenAnimation()
    {
        if((bool)saveDataJson.GetData("RemoveAds")) return;

        gameObject.SetActive(true);
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();

        board.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
        });
    }

    public void CheckTimeToOpenDiaLog ()
    {
        if((bool)saveDataJson.GetData("RemoveAds")) return;
        if (!allowToOpen) return;

        ShowThisObj();
        StartCoroutine(CheckDialog());
    } 

    IEnumerator CheckDialog ()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!DialogChestLv.activeSelf && !DialogChestStart.activeSelf && !DialogRate.activeSelf)
            {
                OpenAnimation();
                ShowThisObjChild();
                allowToOpen = false;
                Invoke("AllowToOpen", 180f);
                break;
            }
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

    private void AllowToOpen()
    {
        allowToOpen = true;
    }

    public void CloseAnimation()
    {
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        board.DOPause();
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
        });
    } 
}
