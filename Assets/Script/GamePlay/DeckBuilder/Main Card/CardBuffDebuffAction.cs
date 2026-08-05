using System.Collections;
using UnityEngine;

public class CardBuffDebuffAction : IdCardAction
{
    public void Execute(CardContextInfo context)
    {
        Card card = context.card;
        MainBody user = context.user;
        MainBody target = context.target;
        CardData cardData = context.cardData;
        bool isTargetSelf = context.isTargetSelf;
        MainBody targetEffect = isTargetSelf ? user : target;

        bool isStackAble = cardData.buffDebuffData.stackAble;
        if(isStackAble)
            targetEffect.buffDebuffHandler.TakeEffect(cardData.buffDebuffData, cardData.buffDebuffAmount, cardData.buffDebuffRound);
        else
            targetEffect.buffDebuffHandler.TakeEffectUnStackAble(cardData.buffDebuffData, cardData.buffDebuffRound);

        card.BattleMaster.GetGamePlayHistory().CreateTakeEffectInformation(targetEffect, cardData.buffDebuffData);       
    }
}
