using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealtHandler : MonoBehaviour
{
    MainBody mainBody;

    [SerializeField] int baseHealt;
    [SerializeField] int currentHealt;
    [SerializeField] int baseArmorClass;

    [Header("Ui")]
    [SerializeField] Image healtBar;
    [SerializeField] TextMeshProUGUI healtTxt;

    [Header("Damage Info")]
    [SerializeField] InfoSpawner textObj;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Vector2 speed;
    [SerializeField] Color damageColor = Color.red;
    [SerializeField] Color healColor = Color.green;

    public void Setup(MainBody _mainBody)
    {
        mainBody = _mainBody;
        baseHealt = mainBody.characterBaseHealt;
        currentHealt = baseHealt;
        baseArmorClass = mainBody.characterBaseArmorClass;
        UpdateUi();
    }

    public void TakeDamage(int dmg, int diceAmount)
    {
        int totalDmg = dmg + (diceAmount * mainBody.CharacterDamagePerDiceTake);

        SpawnDamage(dmg.ToString(), true);
        currentHealt -= dmg;

        if(currentHealt <= 0)
        {
            currentHealt = 0;
            mainBody.Isdead();
        }
        UpdateUi();
    }

    public bool IsGetHit(int attackRoll)
    {
        int totalArmorClass = baseArmorClass + mainBody.CharacterArmorClassBonus;
        if(attackRoll< totalArmorClass)
        {
            SpawnDamage("miss", true);
            return false;
        }
        return true;
    }

    public void TakeDotDamage(int dmg)
    {
        SpawnDamage(dmg.ToString(), true);
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

    void SpawnDamage(string info, bool isDamage)
    {
        InfoSpawner obj = mainBody.poolingMaster.GetPoolObject(textObj.gameObject).GetComponent<InfoSpawner>();
        obj.transform.position = spawnPoint.position;
        Color color = isDamage ? damageColor : healColor;
        obj.Setup(info, speed, color);
    }

}
