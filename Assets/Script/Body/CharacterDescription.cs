using TMPro;
using UnityEngine;

public class CharacterDescription : MonoBehaviour
{
    MainBody mainBody;

    [Header("Character Detail Ui")]
    [SerializeField] GameObject characterDetailPanel;
    [SerializeField] TextMeshProUGUI characterNameTxt;
    [SerializeField] TextMeshProUGUI characterAttackRollTxt;
    [SerializeField] TextMeshProUGUI characterDamageRollTxt;
    [SerializeField] TextMeshProUGUI characterArmorClassTxt;

    public void SetMainBody(MainBody _mainBody) => mainBody = _mainBody;

    public void UpdateDetailCharacter()
    {
        characterNameTxt.text = mainBody.characterName;
        characterAttackRollTxt.text = $"{mainBody.characterBaseAttackRoll + mainBody.CharacterAttckRollBonus}";
        characterDamageRollTxt.text = $"{mainBody.characterBaseDamageRoll + mainBody.CharacterDamageRollBonus}";
        characterArmorClassTxt.text = $"{mainBody.characterBaseArmorClass + mainBody.CharacterArmorClassBonus}";
    }

    public void ShowCharacterDetail(bool isShow)
    {
        characterDetailPanel.SetActive(isShow);
    }
}
