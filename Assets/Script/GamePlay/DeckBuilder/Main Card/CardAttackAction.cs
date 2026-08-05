using System.Collections;
using UnityEngine;

public class CardAttackAction : IdCardAction
{
    public void Execute(CardContextInfo context)
    {
        Card card = context.card;
        MainBody user = context.user;
        MainBody target = context.target;
        CardData cardData = context.cardData;

        int dmg = GetDmg(user, card, cardData);
        target.healtHandler.TakeDamage(dmg,  card.DiceAmount);
    }

    int GetDmg(MainBody mainBody, Card card, CardData cardData)
    {
        int diceAmount = card.DiceAmount;
        int dicePoint = card.DicePoint;

        int minDmg = diceAmount;
        int maxDmg = diceAmount * (mainBody.CharacterDamagePerDiceBonus + dicePoint); 
        int dmg = Random.Range(minDmg, maxDmg + 1);
        dmg += (mainBody.characterBaseDamageRoll + mainBody.CharacterDamageRollBonus) * cardData.bonusDamageRollMultiple;

        return dmg;
    }

}
