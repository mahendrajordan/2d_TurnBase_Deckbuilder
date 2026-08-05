using System.Collections;
using UnityEngine;

public class CardSpecialEffectAction : IdCardAction
{
    public void Execute(CardContextInfo context)
    {
        Card card = context.card;
        
        card.cardSpecialEffect.ActiveEffect();
    }
}
