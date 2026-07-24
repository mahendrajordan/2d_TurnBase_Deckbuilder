using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffHandler : MonoBehaviour
{
    MainBody mainBody;
    HealtHandler healtHandler;

    Dictionary<BuffDebuffData, List<EffectData>> effectDatas = new Dictionary<BuffDebuffData, List<EffectData>>();
    Dictionary<BuffDebuffData, EffectData> effectDataUnStackAbles = new Dictionary<BuffDebuffData, EffectData>();
    Dictionary<BuffDebuffData, int> effectTotalAmount = new Dictionary<BuffDebuffData, int>();
    Dictionary<BuffDebuffData, BuffDebuffIcon> BuffDebuffIcons = new Dictionary<BuffDebuffData, BuffDebuffIcon>();

    List<EffectData> effectDataTemp = new List<EffectData>();

    [Header("Effec Icon")]
    [SerializeField] BuffDebuffIcon buffDebuffIconPrefab;
    [SerializeField] Transform spawnParent;

    public void Setup(MainBody _mainBody)
    {
        mainBody = _mainBody;
        healtHandler = mainBody.healtHandler;
    }

#region EffectCheck
    public void CheckAllEffect()
    {
        foreach(KeyValuePair<BuffDebuffData, List<EffectData>> item in effectDatas)
        {
            BuffDebuffData buffDebuffData = item.Key;
            List<EffectData> effectDataList = item.Value;           

            //dot
            TakeDotDamage(buffDebuffData);

            //check roundlife
            foreach(EffectData effectData in effectDataList)
            {
                effectData.roundLife--;
                if(effectData.roundLife <=0)
                {
                    ChangeStats(buffDebuffData, effectData.effectAmount, false);
                    AddEffectTotalAmount(buffDebuffData, -effectData.effectAmount, buffDebuffData.stackAble);
                    effectDataTemp.Add(effectData);

                    //check untuk merubah status berdasarkan total buff nya
                    IconCheck(buffDebuffData);
                }
            }

            //buang semua list yg sudah habis roundLife nya
            foreach(EffectData effectData in effectDataTemp)
            {
                if(effectDataList.Contains(effectData))
                {
                    effectDataList.Remove(effectData);
                }
            }
            effectDataTemp.Clear();
        }
    }
#endregion

#region Take Effect
    public void TakeEffect(BuffDebuffData buffDebuffData, int effectAmount, int roundLife)
    {
        List<EffectData> effectDataList = effectDatas.ContainsKey(buffDebuffData) ? effectDatas[buffDebuffData] : new List<EffectData>();
        EffectData effectData = new EffectData();
        effectData.buffDebuffData = buffDebuffData;
        effectData.roundLife = roundLife;
        effectData.effectAmount = effectAmount;
        effectDataList.Add(effectData);

        if(effectDatas.ContainsKey(buffDebuffData))
        {
            effectDatas[buffDebuffData] = effectDataList;
        }
        else
        {
            effectDatas.Add(buffDebuffData,effectDataList);
        }
        AddEffectTotalAmount(buffDebuffData, effectAmount);
        ChangeStats(buffDebuffData, effectData.effectAmount);
        
        SpawnBuffDebuffIcon(buffDebuffData);
    }

    public void TakeEffectUnStackAble(BuffDebuffData buffDebuffData, int roundLife)
    {
        EffectData effectData = new EffectData();
        effectData.buffDebuffData = buffDebuffData;
        effectData.roundLife = roundLife;
        effectData.effectAmount = 1;
        bool alreadyHaveEffect = false;

        if(effectDataUnStackAbles.ContainsKey(buffDebuffData))
        {            
            alreadyHaveEffect = effectDataUnStackAbles[buffDebuffData] != null;
            effectDataUnStackAbles[buffDebuffData] = effectData;
        }
        else
        {
            effectDataUnStackAbles.Add(buffDebuffData,effectData);
        }

        AddEffectTotalAmount(buffDebuffData, 1, false);

        if(!alreadyHaveEffect)
            ChangeStats(buffDebuffData,effectData.effectAmount);
        
        SpawnBuffDebuffIcon(buffDebuffData);
    }

    void AddEffectTotalAmount(BuffDebuffData buffDebuffData, int amount, bool canStack = true)
    {
        int totalAmount = effectTotalAmount.ContainsKey(buffDebuffData) ? effectTotalAmount[buffDebuffData] + amount : amount;
        int maxAmount = canStack ? 100 : 1;
        totalAmount = Mathf.Clamp(totalAmount, 0, maxAmount);
        
        if(effectTotalAmount.ContainsKey(buffDebuffData))
        {
            effectTotalAmount[buffDebuffData] = totalAmount;
        }
        else
        {
            effectTotalAmount.Add(buffDebuffData, totalAmount);
        }
    }
#endregion

#region ChangeStats
    void ChangeStats(BuffDebuffData buffDebuffData, int amount, bool isAddEffect = true)
    {
        mainBody.CharacterAttckRollBonus += isAddEffect ? (buffDebuffData.attackRollBonus * amount) : (-buffDebuffData.attackRollBonus  * amount);
        mainBody.CharacterDamageRollBonus += isAddEffect ? (buffDebuffData.damageRollBonus  * amount) : (-buffDebuffData.damageRollBonus * amount);
        mainBody.CharacterArmorClassBonus += isAddEffect ? (buffDebuffData.armorClassBonus * amount) : (-buffDebuffData.armorClassBonus * amount);
        mainBody.CharacterDamagePerDiceBonus += isAddEffect ? (buffDebuffData.damagePerDiceBonus * amount) : (-buffDebuffData.damagePerDiceBonus * amount);
        mainBody.CharacterDamagePerDiceTake += isAddEffect ? (buffDebuffData.damagePerDiceTake * amount) : (-buffDebuffData.damagePerDiceTake * amount);

        mainBody.UpdateDetailCharacter();
    }
#endregion

#region TakeDot
    void TakeDotDamage(BuffDebuffData buffDebuffData)
    {
        int diceAmount = buffDebuffData.diceAmount;
        int dicePoint = buffDebuffData.dicePoint;
        int dotAmount = effectTotalAmount[buffDebuffData];

        int minDmg = diceAmount * dotAmount;
        int maxDmg = dicePoint * dotAmount;
        int totalDmg = Random.Range(minDmg, maxDmg + 1);
        if(totalDmg==0) return;
        healtHandler.TakeDotDamage(totalDmg);
    }
#endregion

#region Icon
    void SpawnBuffDebuffIcon(BuffDebuffData buffDebuffData)
    {        
        int buffAmount = effectTotalAmount[buffDebuffData];
        if(BuffDebuffIcons.ContainsKey(buffDebuffData))
        {
            BuffDebuffIcons[buffDebuffData].gameObject.SetActive(true);
            BuffDebuffIcons[buffDebuffData].Setup(buffDebuffData, buffAmount);
        }
        else
        {
            BuffDebuffIcon buffDebuffIcon = mainBody.poolingMaster.GetPoolObject(buffDebuffIconPrefab.gameObject).GetComponent<BuffDebuffIcon>();
            buffDebuffIcon.transform.SetParent(spawnParent);
            buffDebuffIcon.Setup(buffDebuffData, buffAmount);
            BuffDebuffIcons.Add(buffDebuffData, buffDebuffIcon);  
        }
    }

    void IconCheck(BuffDebuffData buffDebuffData)
    {        
        if(effectTotalAmount[buffDebuffData]<=0)
            RemoveIcon(buffDebuffData);
        else
            UpdateIconInfo(buffDebuffData);
    }

    void RemoveIcon(BuffDebuffData buffDebuffData)
    {
        BuffDebuffIcons[buffDebuffData].gameObject.SetActive(false);
    }

    void UpdateIconInfo(BuffDebuffData buffDebuffData)
    {
        int buffAmount = effectTotalAmount[buffDebuffData];
        BuffDebuffIcons[buffDebuffData].Setup(buffDebuffData, buffAmount);
    }
#endregion

}

[System.Serializable]
public class EffectData
{
    public BuffDebuffData buffDebuffData;
    public int roundLife;
    public int effectAmount;
}
