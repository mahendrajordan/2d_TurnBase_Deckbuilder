using UnityEngine;

public class CardSpecialEffect : MonoBehaviour
{
    protected DeckBuilderMaster deckBuilderMaster;

    public virtual void Setup(DeckBuilderMaster _deckBuilderMaster)
    {
        deckBuilderMaster = _deckBuilderMaster;
    }

    public virtual void ActiveEffect()
    {
        
    }
}
