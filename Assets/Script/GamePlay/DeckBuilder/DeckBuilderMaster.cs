using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using UnityEngine.WSA;
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

    List<Card> cardOnHandkList = new List<Card>();
    List<Card> cardOnTrashkList = new List<Card>();

    Card CurrentCardSelect;

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
    public void DrawCardOnHand(int cardAmount = 5)
    {
        int rand = 0;
        for(int i = 0; i<cardAmount; i++)
        {
            do
            {
                rand = UnityEngine.Random.Range(0, cardDatas.Length);
                
            }while(TotalThisCardOnHand(rand) >= limitSameCard);

            //Card card = Instantiate(cardPrefab,cardDeckParent.transform.position,quaternion.identity ,cardDeckParent);            
            Card card = poolingMaster.GetPoolObject(cardPrefab.gameObject).GetComponent<Card>();
            card.transform.position = cardDeckParent.transform.position;
            card.transform.parent = cardDeckParent;

            card.Setup(cardDatas[rand], playerBody, cardHandParent, cardTrashParent, cardOffParent);
            card.GetMainInfo(this, battleMaster);
            cardOnHandkList.Add(card);
            
            card.gameObject.SetActive(true);
            card.SetCardHandIndex(i);
            StartCoroutine(MoveCardTo(card.transform, cardHandParent, i));
        }
        ResetUseCard();
    }

    //melimit kartu yg sama di tangan maksimal 3
    int TotalThisCardOnHand(int index)
    {
        int n = 0;
        for(int i=0; i<cardOnHandkList.Count; i++)
        {
            if(cardOnHandkList[i].GetId() == cardDatas[index].id)
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
        foreach(Card card in cardOnHandkList)
        {
            StartCoroutine(MoveCardTo(card.transform, cardTrashParent, 0, 1, 0));                
            cardOnTrashkList.Add(card);
        }
        cardOnHandkList.Clear();

        Invoke("DestoryAllCardOnTrash", .5f);
    }

    void DestoryAllCardOnTrash()
    {
        foreach(Card card in cardOnTrashkList)
        {
            poolingMaster.ReturnPoolObject(card.gameObject);
        }
        cardOnHandkList.Clear();
    }
#endregion

#region Select card
    public void SetCurrentCardSelect( Card card)
    {
        CurrentCardSelect = card;
    }

    public void UnSelectAllCard()
    {
        for(int i = 0; i< cardOnHandkList.Count; i++)
        {
            cardOnHandkList[i].UnSelectThisCard();
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
        RemoveAllCardOnHand();
        battleMaster.GetTurnBaseSystem().PlayNextTurn();
    }
#endregion


    // mengaktifkan action card 
    public void ActiveCard(MainBody mainBody)
    {
        if(CurrentCardSelect == null) return;

        StartCoroutine(MoveCardTo(CurrentCardSelect.transform, cardTrashParent, 0, 1, 0));
        cardOnHandkList.Remove(CurrentCardSelect);
        cardOnTrashkList.Add(CurrentCardSelect);
        CurrentCardSelect.ActionCard(mainBody);
    }
}
