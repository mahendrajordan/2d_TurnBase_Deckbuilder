#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CardData data = (CardData)target;

        // update serialized object
        serializedObject.Update();

        EditorGUILayout.LabelField("Basic Info", EditorStyles.boldLabel);
        data.id = EditorGUILayout.IntField("Id", data.id);
        data.name = EditorGUILayout.TextField("name", data.name);
        data.cost = EditorGUILayout.IntField("cost", data.cost);
        data.description = EditorGUILayout.TextArea(data.description,GUILayout.MinHeight(100));

        EditorGUILayout.Space(25);
        data.cardType = (CardType) EditorGUILayout.EnumPopup("Card Type", data.cardType);

        if(data.cardType == CardType.Damage || data.cardType == CardType.DamageAndDebuff || data.cardType == CardType.DamageAndBuff)
        {
            EditorGUILayout.LabelField("Damage Info", EditorStyles.boldLabel);
            data.attackCount = EditorGUILayout.IntSlider("Attack Count", data.attackCount, 1, 8);
            data.diceAmount = EditorGUILayout.IntField("Dice Amount", data.diceAmount);
            data.dicePoint = EditorGUILayout.IntField("Dice Point", data.dicePoint);
            data.bonusAttackRoll = EditorGUILayout.IntField("Bonus Attack Roll", data.bonusAttackRoll);
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