using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class DeckBuilderMaster : MonoBehaviour
{
    PlayerBody playerBody;
    BattleMaster battleMaster;
    PoolingMaster poolingMaster;

    [Header("Card")]
    [SerializeField] CardData[] cardDatas;
    [SerializeField] Card cardPrefab;
    [SerializeField] int limitSameCard = 3;

    [Header("Deck")]
    [SerializeField] int totalUseCard = 2;
    int currentUsecard = 0;
    [SerializeField] TextMeshProUGUI totalUseCardTxt;
    [SerializeField] int totalDeckOnHand = 5;
    
    [SerializeField] Transform cardDeckParent;
    [SerializeField] Transform cardHandParent;
    [SerializeField] Transform cardTrashParent;
    [SerializeField] Transform cardOffParent;
    [SerializeField] Button endTurnBtn;

    List<Card> cardOnHandList = new List<Card>();
    List<Card> cardOnTrashList = new List<Card>();
    List<int> cardDataIdLimitUseList = new List<int>();

    Card CurrentCardSelect;

    bool isDrawCardOnHand = false;

    List<BasicCardData> basicCardDataCurrentRound = new List<BasicCardData>();

    void Awake()
    {
        cardDatas = DeckSaver.ins.CardDataList.ToArray();        
    }

    void Start()
    {
        Setup();
    }

#region Setup
    void Setup()
    {
        battleMaster = FindAnyObjectByType<BattleMaster>();
        playerBody = battleMaster.playerBody;
        poolingMaster = PoolingMaster.ins;

        endTurnBtn.onClick.AddListener(EndTurn);
    }
#endregion

#region drawCard
    public void DrawCardOnHand(int cardAmount = 5, bool isResetUseCard = true)
    {
        int rand = 0;
        bool canUseThisCard = true;
        CardData cardData = null;
        isDrawCardOnHand = true;

        for(int i = 0; i<cardAmount; i++)
        {
            do
            {
                rand = UnityEngine.Random.Range(0, cardDatas.Length);
                canUseThisCard = CanUseThisCard(cardDatas[rand]);
                
            }while(TotalThisCardOnHand(rand) >= limitSameCard || !canUseThisCard);

            //add limit use per turn  
            if(cardDatas[rand].isLimitUsePerTurn)
                AddLimitCardList(cardDatas[rand]);

            //Card card = Instantiate(cardPrefab,cardDeckParent.transform.position,quaternion.identity ,cardDeckParent);            
            Card card = poolingMaster.GetPoolObject(cardPrefab.gameObject).GetComponent<Card>();
            card.transform.position = cardDeckParent.transform.position;
            card.transform.parent = cardDeckParent;

            card.Setup(cardDatas[rand], playerBody, cardHandParent, cardTrashParent, cardOffParent);
            card.GetMainInfo(this, battleMaster);
            cardOnHandList.Add(card);

            BasicCardData basicCardData = basicCardDataCurrentRound.Find(x => x.cardId == cardDatas[rand].id);
            if(basicCardData != null)
                card.UpgradeStatsCard(basicCardData);
            
            card.gameObject.SetActive(true);
            card.SetCardHandIndex(i);
            StartCoroutine(MoveCardTo(card.transform, cardHandParent, i));
        }
        if(isResetUseCard) ResetUseCard();
    }

    //melimit kartu yg sama di tangan maksimal 3
    int TotalThisCardOnHand(int index)
    {
        int n = 0;
        for(int i=0; i<cardOnHandList.Count; i++)
        {
            if(cardOnHandList[i].GetId() == cardDatas[index].id)
                n++;
        }
        return n;
    }

    IEnumerator MoveCardTo(Transform obj, Transform parentTarget, int delayMultiple, float startScale = 0, float endScale = 1)
    {
        Vector2 startPos = obj.transform.position;
        Vector2 endPos = parentTarget.transform.position;

        obj.transform.localScale = Vector2.one * startScale;

        yield return new WaitForSeconds(.1f * delayMultiple);

        float duration = .5f;
        float timer = 0;
        float lerpPoint = 0;

        do
        {
            timer += Time.deltaTime;
            lerpPoint = timer/duration;

            obj.transform.position = Vector2.Lerp(startPos, endPos, lerpPoint);
            obj.transform.localScale = Vector2.Lerp(Vector2.one * startScale, Vector2.one * endScale, lerpPoint);
            
            yield return null;
            
        }while(lerpPoint<1);

        obj.transform.position = endPos;
        obj.transform.localScale = Vector2.one * endScale;
        obj.transform.parent = parentTarget;
    }

    public void RemoveAllCardOnHand()
    {
        foreach(Card card in cardOnHandList)
        {
            StartCoroutine(MoveCardTo(card.transform, cardTrashParent, 0, 1, 0));                
            cardOnTrashList.Add(card);
        }
        cardOnHandList.Clear();

        Invoke("DestoryAllCardOnTrash", .5f);
    }

    void DestoryAllCardOnTrash()
    {
        foreach(Card card in cardOnTrashList)
        {
            poolingMaster.ReturnPoolObject(card.gameObject);
        }
        cardOnTrashList.Clear();
    }
#endregion

#region Limit Card DrawCheck
    void AddLimitCardList(CardData cardData)
    {
        cardDataIdLimitUseList.Add(cardData.id);
    }

    bool CanUseThisCard(CardData cardData)
    {
        if(!cardData.isLimitUsePerTurn) return true;

        int amount = cardDataIdLimitUseList.Count(x=> x == cardData.id); 
        if(amount >= cardData.maxUsePerTurn) return false;

        return true;
    }
#endregion

#region Select card
    public void SetCurrentCardSelect( Card card)
    {
        CurrentCardSelect = card;
    }

    public void UnSelectAllCard()
    {
        for(int i = 0; i< cardOnHandList.Count; i++)
        {
            cardOnHandList[i].UnSelectThisCard();
        }
    }

    public void UseThisChard(int n)
    {
        currentUsecard += n;
        totalUseCardTxt.text = $"{totalUseCard-currentUsecard}";
    }
    void ResetUseCard()
    {
        currentUsecard = 0;
        totalUseCardTxt.text = $"{totalUseCard-currentUsecard}";
    }

    public bool CanUseCard(int cost)
    {
        int totalCost = currentUsecard + cost;
        return totalCost <= totalUseCard;
    }

#endregion

#region End Turn
    public void EndTurn()
    {
        if(battleMaster.GetTurnBaseSystem().GetWhoTurn() == WhoTurn.Enemy || !isDrawCardOnHand) return;

        RemoveAllCardOnHand();
        cardDataIdLimitUseList.Clear();
        battleMaster.GetTurnBaseSystem().PlayNextTurn();
        isDrawCardOnHand = false;

        basicCardDataCurrentRound.Clear();
    }
#endregion


    // mengaktifkan action card 
    public void ActiveCard(MainBody mainBody)
    {
        if(CurrentCardSelect == null) return;

        StartCoroutine(MoveCardTo(CurrentCardSelect.transform, cardTrashParent, 0, 1, 0));
        cardOnHandList.Remove(CurrentCardSelect);
        cardOnTrashList.Add(CurrentCardSelect);

        CurrentCardSelect.ActionCardNew(mainBody);
    }    

#region Bonus Stats Card
    public void AddBonusStatsCard(int id, int diceAmount, int dicePoint, int bonusAttackRoll)
    {
        Debug.Log($"AddBonusStatsCard");
        BasicCardData basicCardData = basicCardDataCurrentRound.Find(x => x.cardId == id) ;

        if (basicCardData == null)
        {
            basicCardData = new BasicCardData
            {
                cardId = id
            };
            basicCardDataCurrentRound.Add(basicCardData);
        }

        basicCardData.diceAmount += diceAmount;
        basicCardData.dicePoint += dicePoint; 
        basicCardData.bonusAttackRoll += bonusAttackRoll;   

        UpdateStatsCardOnHand(id, diceAmount, dicePoint, bonusAttackRoll);     
    }

    void UpdateStatsCardOnHand(int id, int diceAmount, int dicePoint, int bonusAttackRoll)
    {
        foreach(Card card in cardOnHandList)
        {
            if(card.GetCardData().id == id)
            {
                card.DiceAmount += diceAmount;
                card.DicePoint += dicePoint;
                card.BonusAttackRoll += bonusAttackRoll;
            }
        }
    }
#endregion

    public List<Card> GetCardOnHand()=> cardOnHandList;
}
[System.Serializable]
public class BasicCardData
{
    public int cardId;
    public int diceAmount;
    public int dicePoint;
    public int bonusAttackRoll;
}