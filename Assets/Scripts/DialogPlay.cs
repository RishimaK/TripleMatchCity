using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogPlay : MonoBehaviour
{
    public AdsManager adsManager;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;
    public SupportTools supportTools;
    public Home home;
    public Image Tool1;
    public Image Tool2;
    public Sprite LockSprite;
    public Sprite TimeSprite;
    public Sprite ThunderSprite;
    public Sprite ChooseTimeSprite;
    public Sprite ChooseThunderSprite;
    public GameObject Arrow;
    public GameObject Exit;
    public GameObject PlayBtn;
    public GameObject PlayWithToolBtn;
    public ShopTool shopTool;

    private bool isAddTime = false;
    private bool isThunder = false;
    private int ChallengeMap;

    void Start()
    {

    }

    public void OpenAnimation(int map, int challengeMap = 0)
    {
        PlayBtn.GetComponent<Transform>().localPosition = new Vector3(0, -351, 0);
        ChallengeMap = challengeMap;
        gameObject.SetActive(true);
        audioManager.PlaySFX("click");
        isAddTime = false;
        isThunder = false;

        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();
        home.CheckSptriteLevel(
            map, 
            PlayBtn.GetComponent<Image>(), 
            gameObject.transform.GetChild(1).GetComponent<Image>(), 
            challengeMap == 0 ? false : true
        );
        SetTools(map);
        board.DOScale(new Vector3(1f,1f,1f), 0.2f).SetEase(Ease.OutBack).OnComplete(() => {
            
        });
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

    void SetTools(int map)
    {
        if(map >= 6)
        {
            Tool1.sprite = ThunderSprite;
            Tool1.GetComponent<Button>().enabled = true;
            Tool1.transform.GetChild(0).gameObject.SetActive(true);
            Tool1.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{(int)saveDataJson.GetData("Thunder")}";
            if(map >= 10)
            {
                Tool2.sprite = TimeSprite;
                Tool2.GetComponent<Button>().enabled = true;
                Tool2.transform.GetChild(0).gameObject.SetActive(true);
                Tool2.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{(int)saveDataJson.GetData("AddTime")}";
                if(!(bool)saveDataJson.GetData("PlayAgain") && map == 10)
                {
                    saveDataJson.SaveData("AddTime", 1);

                    PlayTutorial(Tool2.gameObject);
                    // Tool2.transform.GetChild(0).gameObject.SetActive(false);
                }
                if(map >= 11) 
                {
                    PlayBtn.GetComponent<Transform>().localPosition = new Vector3(-200, -351, 0);
                    PlayWithToolBtn.SetActive(true);    
                }

                return;
            }
            if(!(bool)saveDataJson.GetData("PlayAgain") && map == 6)
            {
                saveDataJson.SaveData("Thunder", 1);
                PlayTutorial(Tool1.gameObject);
                // Tool1.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    void PlayTutorial(GameObject obj)
    {
        if(ChallengeMap != 0) return;
        obj.transform.GetChild(0).gameObject.SetActive(false);
        if(obj.name == Tool2.name) Tool1.GetComponent<Button>().enabled = false;
        Exit.GetComponent<Button>().enabled = false;
        PlayBtn.GetComponent<Button>().enabled = false;

        Arrow.GetComponent<RectTransform>().localPosition = obj.GetComponent<RectTransform>().localPosition + new Vector3(0, 250,0);
        Arrow.SetActive(true);
    }

    public void PlayGame()
    {
        gameObject.SetActive(false);
        if(ChallengeMap == 0) home.EnterGame(isThunder, isAddTime);
        else home.EnterChallengeGame(ChallengeMap, isThunder, isAddTime);
        audioManager.PlaySFX("click");

        if(Arrow.activeSelf)
        {
            Arrow.SetActive(false);
            Exit.GetComponent<Button>().enabled = true;
        }
        else
        {
            if(isThunder)
            {
                int thunder = (int)saveDataJson.GetData("Thunder");
                saveDataJson.SaveData("Thunder", thunder - 1);
            }
            if(isAddTime)
            {
                int addTime = (int)saveDataJson.GetData("AddTime");
                saveDataJson.SaveData("AddTime", addTime - 1);
            }
        }
    }

    public void PlayGameWithFullSupport()
    {
        gameObject.SetActive(false);
        if(ChallengeMap == 0) home.EnterGame(true, true);
        else home.EnterChallengeGame(ChallengeMap, true, true);
    }

    public void AddTime()
    {
        if(Tool2.sprite == ChooseTimeSprite)
        {
            audioManager.PlaySFX("click");
            Tool2.sprite = TimeSprite;
            Tool2.transform.GetChild(0).gameObject.SetActive(true);

            isAddTime = false;
            return;
        }

        int addTime = (int)saveDataJson.GetData("AddTime");
        if(addTime <= 0) 
        {
            shopTool.OpenAnimation("AddTime");
            return;
        }
        Tool2.sprite = ChooseTimeSprite;
        Tool2.transform.GetChild(0).gameObject.SetActive(false);
        audioManager.PlaySFX("click");
        isAddTime = true;

        if(Arrow.activeSelf)
        {
            Arrow.GetComponent<RectTransform>().localPosition = PlayBtn.GetComponent<RectTransform>().localPosition + new Vector3(0, 250,0);

            Tool2.GetComponent<Button>().enabled = false;
            PlayBtn.GetComponent<Button>().enabled = true;
        }
    }

    public void UseThunder ()
    {
        if(Tool1.sprite == ChooseThunderSprite)
        {
            audioManager.PlaySFX("click");
            Tool1.sprite = ThunderSprite;
            Tool1.transform.GetChild(0).gameObject.SetActive(true);

            isThunder = false;
            return;
        }

        int thunder = (int)saveDataJson.GetData("Thunder");
        if(thunder <= 0) 
        {
            shopTool.OpenAnimation("Thunder");
            return;
        }
        audioManager.PlaySFX("click");
        Tool1.sprite = ChooseThunderSprite;
        Tool1.transform.GetChild(0).gameObject.SetActive(false);

        isThunder = true;

        if(Arrow.activeSelf)
        {
            Arrow.GetComponent<RectTransform>().localPosition = PlayBtn.GetComponent<RectTransform>().localPosition + new Vector3(0, 250,0);

            PlayBtn.GetComponent<Button>().enabled = true;
            Tool1.GetComponent<Button>().enabled = false;
        }
    }
}
