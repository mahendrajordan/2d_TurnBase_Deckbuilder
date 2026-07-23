using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseSystem : MonoBehaviour
{
    PoolingMaster poolingMaster;

    [SerializeField] CardData[] rewardCardData;
    List<CardData> rewardCardDataList = new List<CardData>();
    List<CardData> cardDataList = new List<CardData>();
    [SerializeField] CardInterface cardInterface;
    [SerializeField] Transform moveParent;
    [SerializeField] GameObject panel;
    int cardOnDeck;
    int cardOnReward;
    int currentCardOnReward;

    [Header("Win")]
    [SerializeField] GameObject winPanel;
    [SerializeField] Button nextStageBtn;
    [SerializeField] Transform rewardDeckParent;
    [SerializeField] Transform mainDeckParent;
    [SerializeField] Transform trashDeckParent;

    [Header("Lose")]
    [SerializeField] GameObject losePanel;
    [SerializeField] Button mainMenuBtn;

    [Header("Finish Game")]    
    [SerializeField] GameObject finishPanel;
    [SerializeField] Button finishBtn;

    void Start()
    {
        Setup();
    }

#region  Setup
    void Setup()
    {
        cardDataList = DeckSaver.ins.CardDataList.ToList();
        cardOnDeck = cardDataList.Count;
        cardOnReward = rewardCardData.Length;
        currentCardOnReward = cardOnReward;

        poolingMaster = PoolingMaster.ins;

        mainMenuBtn.onClick.AddListener(()=> SceneManager.LoadScene(0));
        nextStageBtn.onClick.AddListener(()=> GoNextScene());
        finishBtn.onClick.AddListener(()=> SceneManager.LoadScene(0));

        GetRewardCardCanUse();
    }

    void GetRewardCardCanUse()
    {
        foreach(CardData cardData in rewardCardData)
        {
            if(cardDataList.Contains(cardData)) continue;
            rewardCardDataList.Add(cardData);
        }
    }
#endregion

#region Showh Panel
    public void ShowWinPanel()
    {
        panel.SetActive(true);
        winPanel.SetActive(true);
        losePanel.SetActive(false);
        finishPanel.SetActive(false);

        CreateRewardDeck();
        CreateMainDeck();

        nextStageBtn.gameObject.SetActive(false);
    }

    public void ShowLosePanel()
    {
        panel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
        finishPanel.SetActive(false);
    }

    public void ShowFinishPanel()
    {
        panel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        finishPanel.SetActive(true);
    }
#endregion

#region RewardDeck
    void CreateRewardDeck()
    {
        for(int i=0; i<2; i++)
        {
            CardData cardData = GetRewardCardData(); 
            if(cardData==null) break;

            CardInterface  newCard = poolingMaster.GetPoolObject(cardInterface.gameObject).GetComponent<CardInterface>();
            newCard.transform.SetParent(rewardDeckParent);

            newCard.Setup(cardData,()=> MoveRewardCardToMainDeck(newCard), ()=> MoveRewardCardBackToRewardDeck(newCard) );
            newCard.ActiveBonusCardIcon();
        }
    }

    void MoveRewardCardToMainDeck(CardInterface card)
    {
        if(currentCardOnReward != cardOnReward) return;
        if(cardOnDeck >= 10) return;

        StartCoroutine(MoveCard(card.transform, mainDeckParent));
        cardDataList.Add(card.GetCardData());
        cardOnDeck++;
        currentCardOnReward--;

        nextStageBtn.gameObject.SetActive(true);
    }

    void MoveRewardCardBackToRewardDeck(CardInterface card)
    {
        StartCoroutine(MoveCard(card.transform, rewardDeckParent));
        cardDataList.Remove(card.GetCardData());
        cardOnDeck--;        
        currentCardOnReward++;

        nextStageBtn.gameObject.SetActive(false);
    }

    CardData GetRewardCardData()
    {
        CardData cardData = null;
        if(rewardCardDataList.Count<=0) cardData = null;
        else if(rewardCardDataList.Count == 1)
        { 
            cardData = rewardCardDataList[0];
            rewardCardDataList.Clear();
        }
        else
        {
            int rand = Random.Range(0, rewardCardDataList.Count);
            cardData = rewardCardDataList[rand];
        }

        return cardData;
    }
#endregion

#region MainDeck
    void CreateMainDeck()
    {
        foreach(CardData cardData in cardDataList)
        {
            CardInterface  newCard = poolingMaster.GetPoolObject(cardInterface.gameObject).GetComponent<CardInterface>();
            newCard.transform.SetParent(mainDeckParent);
            
            newCard.Setup(cardData,()=> MoveMainCardToTrashDeck(newCard), ()=> MoveMainCardToMainDeck(newCard) );
        }
    }

    void MoveMainCardToTrashDeck(CardInterface card)
    {
        if(cardOnDeck <10) return;
        StartCoroutine(MoveCard(card.transform, trashDeckParent));
        cardDataList.Remove(card.GetCardData());
        cardOnDeck--;
    }
    void MoveMainCardToMainDeck(CardInterface card)
    {
        if(cardOnDeck == 10) return;
        StartCoroutine(MoveCard(card.transform, mainDeckParent));
        cardDataList.Remove(card.GetCardData());
        cardOnDeck++;
    }
#endregion

    IEnumerator MoveCard(Transform objTransform, Transform target)
    {
        objTransform.SetParent(moveParent);
        Vector2 start = objTransform.position;
        Vector2 end = target.childCount > 0 ? target.GetChild(target.childCount - 1).position : target.position;

        float duration = .2f;
        float timer = 0;
        float lerpPoint = 0;

        do
        {
            timer += Time.deltaTime;
            lerpPoint =timer/duration;
            objTransform.position = Vector2.Lerp(start, end, lerpPoint);

           yield return null; 
        }while(lerpPoint<1);
        objTransform.position = end;
        objTransform.SetParent(target);
    }

    void GoNextScene()
    {
        int currenSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxSceneIndex = SceneManager.sceneCountInBuildSettings;
        if(currenSceneIndex < maxSceneIndex-1)
        {
            DeckSaver.ins.CardDataList = cardDataList.ToList();
            SceneManager.LoadScene(currenSceneIndex + 1);
        }
    }
}
