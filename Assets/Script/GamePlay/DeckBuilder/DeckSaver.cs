using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckSaver : MonoBehaviour
{
    public static DeckSaver ins;

    [SerializeField] List<CardData> cardDataList = new List<CardData>();
    List<CardData> cardDataBase = new List<CardData>();

    void Awake()
    {
        if(ins == null)
        {
            ins = this;
            DontDestroyOnLoad(this.gameObject);
            cardDataBase = cardDataList.ToList();

            return;
        }
        Destroy(this.gameObject);
    }

    public void SetBaseDeck(List<CardData> newCardDataList)
    {
        cardDataList = newCardDataList.ToList();
    }

    public List<CardData> CardDataList {get{return cardDataList;} set{cardDataList = value;}}
}
