using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string Name;
    public int healt;

    public int attackRoll;
    public int damageRoll;
    public int armorClass;
}
