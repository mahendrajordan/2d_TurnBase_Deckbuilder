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
}
