using UnityEngine;

[System.Serializable]
public class CardContextInfo 
{
    public Card card;
    public MainBody user;
    public MainBody target;
    public CardData cardData;
    public bool isTargetSelf;
}
