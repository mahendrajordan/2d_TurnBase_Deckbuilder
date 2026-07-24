using System.Collections;
using UnityEngine;

public class CardEffectDrawCard : CardSpecialEffect
{
    [SerializeField] int cardDrawAmount = 1;
    [SerializeField] bool isRemoveAllCardOnHand = false;

    public override void ActiveEffect()
    {
        base.ActiveEffect();
        StartCoroutine(DrawCard());
    }

    IEnumerator DrawCard()
    {
        transform.SetParent(null);
        if(isRemoveAllCardOnHand) 
        {
            deckBuilderMaster.RemoveAllCardOnHand();
            yield return new WaitForSeconds(.5f);
        }

        yield return null;
        deckBuilderMaster.DrawCardOnHand(cardDrawAmount, false);
        Destroy(this.gameObject);
    }
}
