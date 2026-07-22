using UnityEngine;

public class MainBody : MonoBehaviour
{
    protected BattleMaster battleMaster;
    protected TurnBaseSystem turnBaseSystem;

    public PoolingMaster poolingMaster  { get; private set; }

    [SerializeField] protected ConditonType conditonType;
    public CharacterData CharacterData;
    public HealtHandler healtHandler;
    public BuffDebuffHandler buffDebuffHandler;

    [Space]
    [SerializeField] protected GameObject seletObj;
    public bool isCanSelect = false;

    public string characterName  { get; private set; }
    public int characterBaseHealt  { get; private set; }
    public int characterBaseAttackRoll  { get; private set; }
    public int characterBaseDamageRoll  { get; private set; }
    public int characterBaseArmorClass  { get; private set; }

    protected int characterAttackRollBonus;
    protected int characterDamageRollBonus;
    protected int characterArmorClassBonus;
    protected int characterDamagePerDiceBonus;
    protected int characterDamagePerDiceTake;

    void Awake()
    {
        Setup();
    }

#region  Setup
    protected virtual void Setup()
    {
        battleMaster = FindAnyObjectByType<BattleMaster>();
        turnBaseSystem = FindAnyObjectByType<TurnBaseSystem>();
        poolingMaster = PoolingMaster.ins;

        characterName = CharacterData.name;
        characterBaseHealt = CharacterData.healt;
        characterBaseAttackRoll = CharacterData.attackRoll;
        characterBaseDamageRoll = CharacterData.damageRoll;
        characterBaseArmorClass = CharacterData.armorClass;

        characterAttackRollBonus = 0;
        characterDamageRollBonus = 0;
        characterArmorClassBonus = 0;
    }
#endregion    

    public virtual void Isdead()
    {
        conditonType = ConditonType.Dead;
    }

    public void ActiveSelect(bool n)
    {
        isCanSelect = n;
        seletObj.SetActive(isCanSelect);
    }

#region GetStats
    public int CharacterAttckRollBonus { get{ return characterAttackRollBonus;} set{characterAttackRollBonus = value;} }
    public int CharacterDamageRollBonus { get{ return characterDamageRollBonus;} set{characterDamageRollBonus = value;} }
    public int CharacterArmorClassBonus { get{ return characterArmorClassBonus;} set{characterArmorClassBonus = value;} }
    public int CharacterDamagePerDiceBonus { get{ return characterDamagePerDiceBonus;} set{characterDamagePerDiceBonus = value;} }
    public int CharacterDamagePerDiceTake { get{ return characterDamagePerDiceTake;} set{characterDamagePerDiceTake = value;} }
#endregion

}
