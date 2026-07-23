using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    CardData cardData;
    MainBody mainBody;
    DeckBuilderMaster deckBuilderMaster;
    BattleMaster battleMaster;

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

    Button btn;
    Transform baseHandParent;
    Transform baseTrashParent;
    Transform baseOffParent;
    int handIndex;
    int selectIndex = 0; // 0 : sedang unSelect; 1 : sedang select

#region Setup
    public void Setup(CardData _cardData, MainBody _mainBody, Transform handTransform, Transform trashTransform, Transform offTransform)
    {
        mainBody = _mainBody;
        cardData = _cardData;

        baseHandParent = handTransform;
        baseTrashParent = trashTransform;
        baseOffParent = offTransform;
        selectIndex = 0;

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
        int dmg = mainBody.characterBaseDamageRoll + mainBody.CharacterDamageRollBonus;
        descriptionTxt.text = description.Replace("{dmg}", dmg.ToString());

        btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(()=> SelectCard());
    }

    public void GetMainInfo(DeckBuilderMaster _deckBuilderMaster, BattleMaster _battleMaster)
    {        
        deckBuilderMaster = _deckBuilderMaster;
        battleMaster = _battleMaster;
    }

    public void SetCardHandIndex(int n)
    {
        handIndex = n;
    }
#endregion

#region SelectCard
    void SelectCard()
    {
        if(selectIndex == 0)
            SelectThisCard();
        else
            UnSelectThisCard();
    }

    public void SelectThisCard()
    {
        if(selectIndex == 1) return;
        deckBuilderMaster.UnSelectAllCard();

        Vector2 offset = this.transform.position;
        offset.y += 100f;
        StartCoroutine(SetPosition(offset, 1.2f, baseOffParent));
        
        deckBuilderMaster.SetCurrentCardSelect(this);
        GetSelectType(true);        

        selectIndex = selectIndex == 1 ? 0 : 1;
    }

    public void UnSelectThisCard()
    {
        if(selectIndex == 0) return;
        Vector2 offset = this.transform.position;
        offset.y -= 100f;
        StartCoroutine(SetPosition(offset, 1f, baseHandParent));  

        deckBuilderMaster.SetCurrentCardSelect(null);        
        GetSelectType(false);
        
        selectIndex = selectIndex == 1 ? 0 : 1;
    }

    void GetSelectType(bool isSelect)
    {
        if(cardType == CardType.Damage || cardType == CardType.DamageAndBuff ||cardType == CardType.DamageAndDebuff ||cardType == CardType.Debuff)
        {
            battleMaster.ActiveSelectAllEnemy(isSelect);
        }

        if(cardType == CardType.Buff || cardType == CardType.skill)
        {
            battleMaster.ActiveSelectPlayer(isSelect);
        }
    }

    IEnumerator SetPosition(Vector2 endPos, float endScale , Transform parent)
    {
        Vector2 startPos = this.transform.position;
        float startScale = this.transform.localScale.x;

        float duration = .15f;
        float timer = 0;
        float lerpPoint = 0;

        do
        {
            timer += Time.deltaTime;
            lerpPoint = timer / duration;
            this.transform.position = Vector2.Lerp(startPos, endPos, lerpPoint);
            this.transform.localScale = Vector2.Lerp(Vector2.one * startScale, Vector2.one * endScale, lerpPoint);

            yield return null;
        }while(lerpPoint<1);

        this.transform.position = endPos;
        this.transform.localScale = Vector2.one * endScale;
        this.transform.parent = parent;
        this.transform.SetSiblingIndex(handIndex);
    }
#endregion

#region ActionCard
    public void ActionCard(MainBody target)
    {
        if(cardType == CardType.Damage || cardType == CardType.DamageAndBuff ||cardType == CardType.DamageAndDebuff)
        {
            StartCoroutine(Attack(target));
        }
        if(cardType == CardType.Debuff)
        {
            GiveBuffDebuff(target);
        }
        if(cardType == CardType.DamageAndBuff || cardType == CardType.Buff)
        {
            MainBody newTarget = mainBody;
            GiveBuffDebuff(newTarget);
        }
        
        deckBuilderMaster.SetCurrentCardSelect(null);

        battleMaster.ActiveSelectAllEnemy(false);
        battleMaster.ActiveSelectPlayer(false);
    }    
#endregion

#region Attack
    IEnumerator Attack(MainBody target)
    {
        for(int i=0; i< attackCount; i++)
        {
            if(!target.healtHandler.IsGetHit(GetAttackRoll()) ) continue;

            target.healtHandler.TakeDamage(GetDmg(),  diceAmount);
            //khusus "CardType.DamageAndDebuff" harus kena target
            if(cardType == CardType.DamageAndDebuff)
            {
                GiveBuffDebuff(target);
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    int GetDmg()
    {
        int minDmg = diceAmount;
        int maxDmg = diceAmount * (mainBody.CharacterDamagePerDiceBonus + dicePoint);
        int dmg = Random.Range(minDmg, maxDmg + 1);
        dmg += mainBody.characterBaseDamageRoll + mainBody.CharacterDamageRollBonus;

        return dmg;
    }

    int GetAttackRoll()
    {
        int attackRoll = Random.Range(1, 21);
        attackRoll += mainBody.characterBaseAttackRoll + mainBody.CharacterAttckRollBonus + bonusAttackRoll;
        return attackRoll;
    }
#endregion

#region Buff Debuff
    void GiveBuffDebuff(MainBody target)
    {
        bool isStackAble = cardData.buffDebuffData.stackAble;
        if(isStackAble)
            target.buffDebuffHandler.TakeEffect(cardData.buffDebuffData, cardData.buffDebuffAmount, cardData.buffDebuffRound);
        else
            target.buffDebuffHandler.TakeEffectUnStackAble(cardData.buffDebuffData, cardData.buffDebuffRound);
    }
#endregion

    public int GetId() => id;
    public int GetHandIndex() => handIndex;
}
