#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuffDebuffData))]

public class BuffDebuffDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BuffDebuffData data = (BuffDebuffData)target;

        // update serialized object
        serializedObject.Update();

        EditorGUILayout.LabelField("Basic Info", EditorStyles.boldLabel);
        data.id = EditorGUILayout.IntField("Id", data.id);
        data.name = EditorGUILayout.TextField("name", data.name);
        data.icon = (Sprite) EditorGUILayout.ObjectField("Icon", data.icon, typeof(Sprite));
        data.description = EditorGUILayout.TextArea(data.description,GUILayout.MinHeight(100));
        data.stackAble = EditorGUILayout.Toggle("Stack Able", data.stackAble);
        data.buffDebuffType = (BuffDebuffType) EditorGUILayout.EnumPopup("Buff Debuff Type", data.buffDebuffType);

        if(data.buffDebuffType == BuffDebuffType.Stats || data.buffDebuffType == BuffDebuffType.StatsnDot)
        {
            EditorGUILayout.Space(25);
            EditorGUILayout.LabelField("Bonus Stats", EditorStyles.boldLabel);
            data.attackRollBonus = EditorGUILayout.IntField("Attack Roll Bonus", data.attackRollBonus);
            data.damageRollBonus = EditorGUILayout.IntField("Damage Roll Bonus", data.damageRollBonus);
            data.armorClassBonus = EditorGUILayout.IntField("Armor Class Bonus", data.armorClassBonus);
            
            EditorGUILayout.Space();
            data.damagePerDiceBonus = EditorGUILayout.IntField("Damage Per Dice Bonus", data.damagePerDiceBonus);
            data.damagePerDiceTake = EditorGUILayout.IntField("Damage Per Dice Take", data.damagePerDiceTake);
        }

        if(data.buffDebuffType == BuffDebuffType.Dot || data.buffDebuffType == BuffDebuffType.StatsnDot)
        {
            EditorGUILayout.Space(25);
            EditorGUILayout.LabelField("Dot", EditorStyles.boldLabel);
            data.diceAmount = EditorGUILayout.IntField("Dice Amount", data.diceAmount);
            data.dicePoint = EditorGUILayout.IntField("Dice Point", data.dicePoint);
        }

        // save perubahan
        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }

        serializedObject.ApplyModifiedProperties();

    }
}
#endif
