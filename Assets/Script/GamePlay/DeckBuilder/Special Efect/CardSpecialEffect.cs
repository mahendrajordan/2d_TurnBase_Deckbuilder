using UnityEngine;

public class CardSpecialEffect : MonoBehaviour
{
    protected DeckBuilderMaster deckBuilderMaster;
    protected Card currentCard;

    public virtual void Setup(DeckBuilderMaster _deckBuilderMaster, Card _card)
    {
        deckBuilderMaster = _deckBuilderMaster;
        currentCard = _card;
    }

    public virtual void ActiveEffect()
    {
        
    }
}
