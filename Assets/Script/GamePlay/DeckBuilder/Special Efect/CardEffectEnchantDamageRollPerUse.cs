using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardEffectEnchantDamageRollPerUse : CardSpecialEffect
{
    enum EnchantDamageRollType {DiceAmount, DiceValue}
    [SerializeField] EnchantDamageRollType echantDamageRollType;

    [SerializeField] int enchantDiceAmountPerStack;
    [SerializeField] int enchantDiceValuePerStack;

    int stack=0;

    public override void ActiveEffect()
    {
        base.ActiveEffect();

        List<Card> cardList = deckBuilderMaster.GetCardOnHand().ToList();
        foreach(Card card in cardList)
        {
            if(card.GetCardData() == currentCard.GetCardData())
            {
                card.cardSpecialEffect.GetComponent<CardEffectEnchantDamageRollPerUse>().AddStack();
            }
        }
    }

    public void AddStack()
    {
        stack++;
        switch(echantDamageRollType)
        {
            case EnchantDamageRollType.DiceAmount:
                currentCard.DiceAmount+= enchantDiceValuePerStack;
                break;
            case EnchantDamageRollType.DiceValue :
                currentCard.DicePoint+= enchantDiceValuePerStack;
                break;
        }
    }
}
