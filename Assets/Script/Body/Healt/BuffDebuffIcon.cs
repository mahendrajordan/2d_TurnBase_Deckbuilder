using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffDebuffIcon : MonoBehaviour
{
    BuffDebuffData buffDebuffData;
    [SerializeField] Image effecIcon;
    string description;
    int effecAmount;

    [Header("Description Panek")]
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI descriptionTxt;


    void OnDisable()
    {
        descriptionPanel.SetActive(false);
    }

    public void Setup(BuffDebuffData _buffDebuffData, int amount)
    {
        buffDebuffData = _buffDebuffData;
        effecIcon.sprite = buffDebuffData.icon;
        effecAmount = amount;
        SetupDescription();

        titleTxt.text = buffDebuffData.name;
        descriptionTxt.text = description;
    }

    void SetupDescription()
    {
        int attackRollBonus = buffDebuffData.attackRollBonus * effecAmount;
        int damageRollBonus = buffDebuffData.damageRollBonus * effecAmount;
        int armorClassBonus = buffDebuffData.armorClassBonus * effecAmount;
        int damagePerDiceBonus = buffDebuffData.damagePerDiceBonus * effecAmount;
        int damagePerDiceTake = buffDebuffData.damagePerDiceTake * effecAmount;

        int diceAmount = buffDebuffData.diceAmount * effecAmount;
        int dicePoint = buffDebuffData.dicePoint * effecAmount;

        description = buffDebuffData.description;
        description = description.Replace("{AttackRoll}", GetReplaceText(attackRollBonus));
        description = description.Replace("{DamageRoll}", GetReplaceText(damageRollBonus));
        description = description.Replace("{ArmorClass}", GetReplaceText(armorClassBonus));
        description = description.Replace("{dpdb}", GetReplaceText(damagePerDiceBonus));
        description = description.Replace("{dpdt}", GetReplaceText(damagePerDiceTake));

        description = description.Replace("{DiceAmount}", diceAmount.ToString());
        description = description.Replace("{DicePoint}", dicePoint.ToString());

    }

    string GetReplaceText(int n)
    {
        string addText = n>=0 ? "+" : "";
        return addText + n.ToString(); 
    }

    public void ShowDescriptionPanel()
    {
        descriptionPanel.SetActive(true);
    }

    public void HideDescriptionPanel()
    {
        descriptionPanel.SetActive(false);
    }
}
