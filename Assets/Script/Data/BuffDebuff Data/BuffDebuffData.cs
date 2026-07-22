using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/BuffDebuffData")]
public class BuffDebuffData : ScriptableObject
{
    public int id;
    public string name;
    public Sprite icon;
    public string description;
    public bool stackAble = false;
    public BuffDebuffType buffDebuffType;

    public int attackRollBonus;
    public int damageRollBonus;
    public int armorClassBonus;
    public int damagePerDiceBonus;
    public int damagePerDiceTake;

    //Dot
    public int diceAmount = 0;
    public int dicePoint = 0;
}
