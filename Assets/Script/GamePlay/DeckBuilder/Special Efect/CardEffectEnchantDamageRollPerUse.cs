using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardEffectEnchantDamageRollPerUse : CardSpecialEffect
{
    enum EnchantDamageRollType {DiceAmount, DiceValue}
    [SerializeField] EnchantDamageRollType echantDamageRollType;

    [SerializeField] int enchantDiceAmountPerStack;
    [SerializeField] int enchantDiceValuePerStack;

    public override void ActiveEffect()
    {
        base.ActiveEffect();

        List<Card> cardList = deckBuilderMaster.GetCardOnHand().ToList();
        AddStack();
    }

    public void AddStack()
    {
        switch(echantDamageRollType)
        {
            case EnchantDamageRollType.DiceAmount:
                deckBuilderMaster.AddBonusStatsCard(currentCard.GetCardData().id, enchantDiceValuePerStack, 0, 0);
                break;
            case EnchantDamageRollType.DiceValue :
                deckBuilderMaster.AddBonusStatsCard(currentCard.GetCardData().id, 0, enchantDiceValuePerStack, 0);
                break;
        }
    }
}
