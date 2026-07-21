using UnityEngine;

public class MainBody : MonoBehaviour
{
    [SerializeField] protected ConditonType conditonType;
    public CharacterData CharacterData;
    public HealtHandler healtHandler;

    [SerializeField] protected GameObject seletObj;
    public bool isCanSelect = false;

    public string characterName  { get; private set; }
    public int characterBaseHealt  { get; private set; }
    public int characterBaseAttackRoll  { get; private set; }
    public int characterBaseDamageRoll  { get; private set; }
    public int characterBaseArmorClass  { get; private set; }

    void Awake()
    {
        Setup();
    }

#region  Setup
    protected virtual void Setup()
    {
        characterName = CharacterData.name;
        characterBaseHealt = CharacterData.healt;
        characterBaseAttackRoll = CharacterData.attackRoll;
        characterBaseDamageRoll = CharacterData.damageRoll;
        characterBaseArmorClass = CharacterData.armorClass;
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

}
