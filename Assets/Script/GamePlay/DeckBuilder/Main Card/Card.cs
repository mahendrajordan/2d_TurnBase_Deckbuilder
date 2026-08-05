using System.Collections;
using System.Collections.Generic;
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

    public CardSpecialEffect cardSpecialEffect {get; private set;}

    List<IdCardAction> idCardActionList = new List<IdCardAction>();
    CardContextInfo cardContextInfo;
    CardAttackAction cardAttackAction;
    CardBuffDebuffAction cardBuffDebuffAction;
    CardSpecialEffectAction cardSpecialEffectAction;

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
        int dmg = (mainBody.characterBaseDamageRoll + mainBody.CharacterDamageRollBonus) * cardData.bonusDamageRollMultiple;
        descriptionTxt.text = description.Replace("{dmg}", dmg.ToString());

        btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(()=> SelectCard());

        SetupActionCard();
    }

    public void GetMainInfo(DeckBuilderMaster _deckBuilderMaster, BattleMaster _battleMaster)
    {        
        deckBuilderMaster = _deckBuilderMaster;
        battleMaster = _battleMaster;

        SetupSpecialEffec();
    }

    public void SetCardHandIndex(int n)
    {
        handIndex = n;
    }
#endregion

#region SelectCard
    void SelectCard()
    {
        if(!deckBuilderMaster.CanUseCard(cardData.cost)) return;

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
        offset.y += 50f;
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
        if(cardType == CardType.Damage || cardType == CardType.DamageAndBuff ||cardType == CardType.DamageAndDebuff ||cardType == CardType.Debuff || cardType == CardType.DamageAndSkill )
        {
            battleMaster.ActiveSelectAllEnemy(isSelect);
        }

        if(cardType == CardType.Buff || cardType == CardType.Skill)
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
    public void ActionCardNew(MainBody target)
    {
        battleMaster.GetGamePlayHistory().CreateCardInformation(mainBody, cardData);

        cardContextInfo.target = target;
        StartCoroutine(StartActiomCardCard());

        deckBuilderMaster.SetCurrentCardSelect(null);
        if(mainBody.GetComponent<PlayerBody>())deckBuilderMaster.UseThisChard(cardData.cost);

        battleMaster.ActiveSelectAllEnemy(false);
        battleMaster.ActiveSelectPlayer(false);
    }

    IEnumerator StartActiomCardCard()
    {
        for(int i = 0; i< attackCount; i++)
        {
            //card bertipe dmg harus hit target baru bisa memberi debuff atau skill
            if(cardType == CardType.Damage || cardType == CardType.DamageAndDebuff || cardType == CardType.DamageAndSkill)
                if(!cardContextInfo.target.healtHandler.IsGetHit(GetAttackRoll(),mainBody) ) continue;

            //start action
            foreach(IdCardAction idCardAction in idCardActionList)
            {
                idCardAction.Execute(cardContextInfo);
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    void SetupActionCard()
    {
        idCardActionList.Clear();
        cardContextInfo = new CardContextInfo();
        cardContextInfo.card = this;
        cardContextInfo.cardData = cardData;
        cardContextInfo.user = mainBody;
        cardContextInfo.isTargetSelf = false;

        cardAttackAction = new CardAttackAction();
        cardBuffDebuffAction = new CardBuffDebuffAction();
        cardSpecialEffectAction = new CardSpecialEffectAction();

        switch(cardType)
        {
            case CardType.Damage :
                idCardActionList.Add(cardAttackAction);
                break;
            case CardType.DamageAndDebuff :
                idCardActionList.Add(cardAttackAction);
                idCardActionList.Add(cardBuffDebuffAction);
                break;
            case CardType.DamageAndBuff :
                cardContextInfo.isTargetSelf = true;
                idCardActionList.Add(cardAttackAction);
                idCardActionList.Add(cardBuffDebuffAction);
                break;
            case CardType.DamageAndSkill :                
                idCardActionList.Add(cardAttackAction);
                idCardActionList.Add(cardSpecialEffectAction);
                break;
            case CardType.Buff :
                idCardActionList.Add(cardBuffDebuffAction);
                break;
            case CardType.Debuff :
                idCardActionList.Add(cardBuffDebuffAction);
                break;
            case CardType.Skill :
                idCardActionList.Add(cardSpecialEffectAction);
                break;
        }
            
    }
#endregion

#region Attack    

    int GetAttackRoll()
    {
        int attackRoll = Random.Range(1, 21);
        attackRoll += mainBody.characterBaseAttackRoll + mainBody.CharacterAttckRollBonus + bonusAttackRoll;
        return attackRoll;
    }
#endregion

#region Special Effect
    void SetupSpecialEffec()
    {
        if(cardSpecialEffect != null) Destroy(cardSpecialEffect.gameObject);

        if(cardData.cardEffect == null) return;
        CardSpecialEffect newCardSpecialEffect = Instantiate(cardData.cardEffect, this.transform);
        newCardSpecialEffect.Setup(deckBuilderMaster, this);
        cardSpecialEffect = newCardSpecialEffect;
    }
#endregion 

    public int GetId() => id;
    public int GetHandIndex() => handIndex;
    public CardData GetCardData() => cardData;

    public int AttackCount {get{return attackCount;} set{attackCount = value;}}
    public int DicePoint {get{return dicePoint;} set{dicePoint = value;}}
    public int DiceAmount {get{return diceAmount;} set{diceAmount = value;}}
    public int BonusAttackRoll {get{return BonusAttackRoll;} set{BonusAttackRoll = value;}}
    public BattleMaster BattleMaster {get{return battleMaster;} set{battleMaster = value;}}
}
