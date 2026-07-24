using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CardData[] cardDatas;
    List<CardData> cardOnDeckList = new List<CardData>();
    [SerializeField] CardInterface cardInterfacePrefab;
    List<CardInterface> cardInterfaceList = new List<CardInterface>();
    
    [SerializeField] GameObject deckSetupPanel;
    [SerializeField] Transform cardListParent;
    [SerializeField] Transform deckListParent;
    [SerializeField] Transform moveAreaTransform;

    [SerializeField] Button openDeckBtn;
    [SerializeField] Button playBtn;

    int maxCardOndeck = 5;
    int currentCardOnDect = 0;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        CreateCardDataList();

        openDeckBtn.onClick.AddListener(ShowDeck);
        playBtn.onClick.AddListener(StartGame);
        playBtn.gameObject.SetActive(false);
    }

    void CreateCardDataList()
    {
        int rand =0;

        for(int i = 0; i<7; i++)
        {
            do
            {
                rand = Random.Range(0, cardDatas.Length);
            }while(HaveThisCard(cardDatas[rand]));

            CardData cardData = cardDatas[rand];
            CardInterface cardInterface = Instantiate(cardInterfacePrefab, cardListParent);
            cardInterface.Setup(cardData,()=> MoveCardToDeck(cardInterface.transform, cardData), ()=> MoveCardToMain(cardInterface.transform, cardData));
            cardInterfaceList.Add(cardInterface);
        }
    }

    bool HaveThisCard(CardData cardData)
    {
        return cardInterfaceList.Count(x=> x.GetCardData() == cardData) > 0;
    }

    
#region Move Card
    void MoveCardToDeck(Transform obj, CardData cardData)
    {
        if(currentCardOnDect>= maxCardOndeck) return;

        StartCoroutine(MoveCard(obj, deckListParent));
        cardOnDeckList.Add(cardData);
        currentCardOnDect++;

        playBtn.gameObject.SetActive(currentCardOnDect>=maxCardOndeck);
    }

    void MoveCardToMain(Transform obj, CardData cardData)
    {
        StartCoroutine(MoveCard(obj, cardListParent));
        cardOnDeckList.Remove(cardData);
        currentCardOnDect--;

        playBtn.gameObject.SetActive(currentCardOnDect>=maxCardOndeck);
    }

    IEnumerator MoveCard(Transform obj, Transform target)
    {
        Vector2 start = obj.transform.position;
        Vector2 end = target.childCount > 0 ? target.GetChild(target.childCount - 1).position : target.position;
        obj.SetParent(moveAreaTransform);

        float duration = .2f;
        float timer = 0;
        float lerpPoint = 0;

        do
        {
            timer += Time.deltaTime;
            lerpPoint = timer/duration;
            obj.transform.position = Vector2.Lerp(start,end,lerpPoint);

            yield return null;
        }while(lerpPoint<1);

        obj.transform.position = end;
        obj.SetParent(target);
    }
#endregion

    public void ShowDeck()
    {
        deckSetupPanel.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
        DeckSaver.ins.SetBaseDeck(cardOnDeckList);
    }
}
