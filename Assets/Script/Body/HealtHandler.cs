using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealtHandler : MonoBehaviour
{
    MainBody mainBody;

    [SerializeField] int baseHealt;
    [SerializeField] int currentHealt;
    [SerializeField] int baseArmorClass;
    [SerializeField] int armorClassBonus;

    [Header("Ui")]
    [SerializeField] Image healtBar;
    [SerializeField] TextMeshProUGUI healtTxt;

    public void Setup(MainBody _mainBody)
    {
        mainBody = _mainBody;
        baseHealt = mainBody.characterBaseHealt;
        currentHealt = baseHealt;
        baseArmorClass = mainBody.characterBaseArmorClass;
        UpdateUi();
    }

    public void TakeDamage(int dmg, int attackRoll)
    {
        int totalArmorClass = baseArmorClass + armorClassBonus;
        if(attackRoll< totalArmorClass)
        {
            //attack miss
            return;
        }

        currentHealt -= dmg;

        if(currentHealt <= 0)
        {
            currentHealt = 0;
            mainBody.Isdead();
        }
        UpdateUi();
    }

    void UpdateUi()
    {
        healtBar.fillAmount = (float)currentHealt/(float)baseHealt;
        healtTxt.text = $"{currentHealt}/{baseHealt}";
    }

    public int ArmorClassBonus {get{return armorClassBonus;} set{armorClassBonus = value;} }
}
