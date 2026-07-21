using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using UnityEngine.WSA;

public class DeckBuilderMaster : MonoBehaviour
{
    PlayerBody playerBody;
    BattleMaster battleMaster;

    [Header("Card")]
    [SerializeField] CardData[] cardDatas;
    [SerializeField] Card cardPrefab;
    [SerializeField] int limitSameCard = 3;

    [Header("Deck")]
    [SerializeField] int totalUseCard = 2;
    [SerializeField] int totalDeckOnHand = 5;
    
    [SerializeField] Transform cardDeckParent;
    [SerializeField] Transform cardHandParent;
    [SerializeField] Transform cardTrashParent;
    [SerializeField] Transform cardOffParent;

    List<Card> cardOnHandkList = new List<Card>();

    Card CurrentCardSelect;

    void Start()
    {
        Setup();
    }

#region Setup
    void Setup()
    {
        battleMaster = FindAnyObjectByType<BattleMaster>();
        playerBody = battleMaster.playerBody;

        DrawCardOnHand();
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

            Card card = Instantiate(cardPrefab,cardDeckParent.transform.position,quaternion.identity ,cardDeckParent);            
            card.Setup(cardDatas[rand], playerBody, cardHandParent, cardTrashParent, cardOffParent);
            card.GetMainInfo(this, battleMaster);
            cardOnHandkList.Add(card);
            
            card.gameObject.SetActive(true);
            card.SetCardHandIndex(i);
            StartCoroutine(MoveCardTo(card.transform, cardHandParent, i));
        }
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

#endregion

    public void ActiveCard(MainBody mainBody)
    {
        if(CurrentCardSelect == null) return;

        StartCoroutine(MoveCardTo(CurrentCardSelect.transform, cardTrashParent, 0, 1, 0));
        CurrentCardSelect.ActionCard(mainBody);
    }
}
