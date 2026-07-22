using System.Collections;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    EnemyBody enemyBody;
    TurnBaseSystem turnBaseSystem;

    [SerializeField] int maxEnergy = 1;
    [SerializeField] int currentEnergy;

    [SerializeField] CardData[] cardDatas;
    [SerializeField] float thinkTime = .1f;

    [Header("Card Use Info")]
    [SerializeField] InfoSpawner textObj;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Vector2 speed;
    [SerializeField] Color colorText;

    public void SetupEnemyBrain(EnemyBody _enmyBody)
    {
        enemyBody = _enmyBody;
        turnBaseSystem = enemyBody.GetTurnBaseSystem();
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
                Action(cardData);
                cardOnHand--;
            }

        }while(canPlay);
        

        turnBaseSystem.PlayNextTurn();
    }
#endregion

#region Play Action
    public void Action(CardData cardData )
    {
        MainBody target;
        if(cardData.cardType == CardType.Damage || cardData.cardType == CardType.DamageAndBuff ||cardData.cardType == CardType.DamageAndDebuff)
        {            
            target = turnBaseSystem.GetPlayerBody();
            StartCoroutine(Attack(cardData, target));
        }        
        if(cardData.cardType == CardType.DamageAndDebuff || cardData.cardType == CardType.Debuff)
        {
            target = turnBaseSystem.GetPlayerBody();
            GiveBuffDebuff(cardData, target);
        }
        if(cardData.cardType == CardType.DamageAndBuff || cardData.cardType == CardType.Buff)
        {
            MainBody newTarget = enemyBody;
            GiveBuffDebuff(cardData, newTarget);
        }

        SpawnCardName(cardData.name);
    }    
#endregion

#region Attack
    IEnumerator Attack(CardData cardData, MainBody target)
    {
        int attackCount = cardData.attackCount;

        for(int i=0; i< attackCount; i++)
        {
            target.healtHandler.TakeDamage(GetDmg(cardData), GetAttackRoll(cardData), cardData.diceAmount);
            yield return new WaitForSeconds(.1f);
        }
    }

    int GetDmg(CardData cardData)
    {
        int diceAmount = cardData.diceAmount;
        int dicePoint = cardData.dicePoint;

        int minDmg = diceAmount;
        int maxDmg = diceAmount * dicePoint;
        int dmg = Random.Range(minDmg, maxDmg);
        dmg += enemyBody.characterBaseDamageRoll + enemyBody.CharacterDamageRollBonus;

        return dmg;
    }

    int GetAttackRoll(CardData cardData)
    {
        int bonusAttackRoll = cardData.bonusAttackRoll;
        int attackRoll = Random.Range(1, 20);
        attackRoll += enemyBody.characterBaseAttackRoll + enemyBody.CharacterAttckRollBonus + bonusAttackRoll;
        return attackRoll;
    }
#endregion

#region Buff Debuff
    void GiveBuffDebuff(CardData cardData, MainBody target)
    {
        bool isStackAble = cardData.buffDebuffData.stackAble;
        if(isStackAble)
            target.buffDebuffHandler.TakeEffect(cardData.buffDebuffData, cardData.buffDebuffAmount, cardData.buffDebuffRound);
        else
            target.buffDebuffHandler.TakeEffectUnStackAble(cardData.buffDebuffData, cardData.buffDebuffRound);
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
