using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardInterface : MonoBehaviour
{    
    CardData cardData;

    int id;
    string name;
    int cost;
    string description; 
    CardType cardType;
    int attackCount;
    int diceAmount;
    int dicePoint;
    int bonusAttackRoll;

    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI costTxt;
    [SerializeField] TextMeshProUGUI descriptionTxt;
    [SerializeField] GameObject bonusCardIcon;

    Button btn;
    int clickIndex = 0;

    UnityAction actionOnClick;
    UnityAction unActionOnClick;

    public void Setup(CardData _cardData, UnityAction _actionOnClick, UnityAction _unActionOnClik)
    {
        cardData = _cardData;

        id = cardData.id;
        name = cardData.name;
        cost = cardData.cost;
        description = cardData.description;
        cardType = cardData.cardType;
        attackCount = cardData.attackCount;
        diceAmount = cardData.diceAmount;
        dicePoint = cardData.dicePoint;
        bonusAttackRoll = cardData.bonusAttackRoll;

        this.name = name;
        nameTxt.text = name;
        costTxt.text = cost.ToString();
        descriptionTxt.text = description.Replace("{dmg}", "Dmg");

        actionOnClick = _actionOnClick;
        unActionOnClick = _unActionOnClik;

        btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(()=> PlayAction());
    }

    public void ActiveBonusCardIcon()=>bonusCardIcon.SetActive(true);

    void PlayAction()
    {
        if(clickIndex == 0) actionOnClick.Invoke();
        else if(clickIndex == 1) unActionOnClick.Invoke();
        clickIndex = clickIndex == 0 ? 1 : 0;
    }

    public CardData GetCardData()=> cardData;
}
