using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public int id;
    public string name;
    public int cost = 1;
    public string description;

    public CardType cardType;

    public int attackCount = 1;
    public int diceAmount = 0;
    public int dicePoint = 0;
    public int bonusAttackRoll = 0;
    public int bonusDamageRollMultiple = 1;

    public BuffDebuffData buffDebuffData;
    public int buffDebuffAmount;
    public int buffDebuffRound;

    public CardSpecialEffect cardEffect;

    public bool isLimitUsePerTurn = false;
    public int maxUsePerTurn = 3;
}
