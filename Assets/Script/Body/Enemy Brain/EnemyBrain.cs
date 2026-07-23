using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    EnemyBody enemyBody;
    TurnBaseSystem turnBaseSystem;

    [SerializeField] int maxEnergy = 1;
    [SerializeField] int currentEnergy;

    [SerializeField] CardData[] cardDatas;
    [SerializeField] Card cardPrefab;
    [SerializeField] Transform cardHandParent;
    [SerializeField] float thinkTime = .1f;
    List<Card> cardLists = new List<Card>();

    [Header("Card Use Info")]
    [SerializeField] InfoSpawner textObj;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Vector2 speed;
    [SerializeField] Color colorText;

    public void SetupEnemyBrain(EnemyBody _enmyBody)
    {
        enemyBody = _enmyBody;
        turnBaseSystem = enemyBody.GetTurnBaseSystem();
        CreateCard();
    }

    void CreateCard()
    {
        for(int i = 0; i < cardDatas.Length; i++)
        {
            Card newCard = enemyBody.poolingMaster.GetPoolObject(cardPrefab.gameObject).GetComponent<Card>();
            newCard.Setup(cardDatas[i], enemyBody, cardHandParent, cardHandParent, cardHandParent);
            newCard.GetMainInfo(enemyBody.GetDeckBuilderMaster(), enemyBody.GetBattleMaster());

            newCard.transform.position = cardHandParent.position;
            newCard.transform.SetParent(cardHandParent);
            cardLists.Add(newCard);
        }
    }

#region  Turn Base
    public void PlayThisTurn()
    {
        currentEnergy = maxEnergy;
        StartCoroutine(PlayTurn());
    }

    IEnumerator PlayTurn()
    {
        int cardOnHand = 5;
        bool canPlay = true;

        do
        {
            yield return new WaitForSeconds(thinkTime);

            int rand = Random.Range(0,cardDatas.Length);
            CardData cardData = cardDatas[rand];
            currentEnergy -= cardData.cost;
            
            canPlay = currentEnergy>=0 && cardOnHand >0; 
            if(canPlay)
            {
                cardLists[rand].ActionCard(enemyBody.GetBattleMaster().playerBody);
                SpawnCardName(cardData.name);
                cardOnHand--;
            }

        }while(canPlay);
        

        turnBaseSystem.PlayNextTurn();
    }
#endregion


    void SpawnCardName(string info)
    {
        InfoSpawner obj = enemyBody.poolingMaster.GetPoolObject(textObj.gameObject).GetComponent<InfoSpawner>();
        obj.transform.position = spawnPoint.position;
        Color color = colorText;
        obj.Setup(info, speed, color);
    }
}
