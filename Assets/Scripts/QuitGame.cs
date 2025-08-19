using DG.Tweening;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public AudioManager audioManager;
    public MainGame mainGame;
    public Lose lose;

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
            mainGame.ContinueGameAction();
        });
    }

    public void BtnQuit()
    {
        audioManager.PlaySFX("click");
        gameObject.SetActive(false);
        lose.SetValue("Quit");
    }

    public void BtnStay()
    {
        CloseAnimation();
    }
}
