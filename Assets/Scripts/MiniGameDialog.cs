using DG.Tweening;
using UnityEngine;

public class MiniGameDialog : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public AudioManager audioManager;

    public Home home;

    // void Start()
    // {
        // SaveDataJson.Map[] miniGameData = saveDataJson.TakeMiniGameData().map;
        // Debug.Log(miniGameData.Length);
        // for(int i = 0; i < miniGameData.Length; i++)
        // {

        // }
    // }
    public void OpenAnimation()
    {
        gameObject.SetActive(true);
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();
        board.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            
        });
    }

    public void CloseAnimation()
    {
        Transform board = gameObject.transform.GetChild(1);
        audioManager.PlaySFX("click");
        board.DOPause();
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    public void PlayMiniGame(GameObject map)
    {
        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
        home.EnterMiniGame(int.Parse(map.name));
    }
}
