using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DamageStat))]
/// Draws the damage field with a bit of validation UI.
public class DamageStatDrawer : PropertyDrawer
{
    private const float Padding = 6f;
    private const float HelpBoxHeight = 36f;
    private static readonly Color InvalidColor = new Color(1f, 0.35f, 0.35f, 0.28f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty damage = property.FindPropertyRelative(nameof(DamageStat.damage));
        bool isInvalid = damage.floatValue < 0f;

        Rect boxRect = EditorGUI.IndentedRect(position);
        if (isInvalid)
        {
            EditorGUI.DrawRect(boxRect, InvalidColor);
        }

        GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);

        Rect contentRect = new Rect(position.x + Padding, position.y + Padding, position.width - Padding * 2f, position.height - Padding * 2f);
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect line = new Rect(contentRect.x, contentRect.y, contentRect.width, lineHeight);
        EditorGUI.LabelField(line, label, EditorStyles.boldLabel);

        line.y += lineHeight + spacing;
        EditorGUI.PropertyField(line, damage, new GUIContent("Damage"));

        line.y += lineHeight + spacing;
        if (isInvalid)
        {
            EditorGUI.HelpBox(new Rect(contentRect.x, line.y, contentRect.width, HelpBoxHeight), "Damage cannot be negative.", MessageType.Error);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty damage = property.FindPropertyRelative(nameof(DamageStat.damage));
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float height = Padding * 2f + lineHeight * 2f + spacing;

        if (damage != null && damage.floatValue < 0f)
        {
            height += spacing + HelpBoxHeight;
        }

        return height;
    }
}
