using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ConditionalFieldAttribute conditional = (ConditionalFieldAttribute)attribute;
        SerializedProperty conditionProperty = FindConditionProperty(property, conditional.conditionFieldName);

        if (conditionProperty == null || conditionProperty.propertyType != SerializedPropertyType.Boolean)
        {
            return true;
        }

        bool visible = conditionProperty.boolValue == conditional.expectedBool;
        return conditional.inverse ? !visible : visible;
    }

    private static SerializedProperty FindConditionProperty(SerializedProperty property, string conditionFieldName)
    {
        string path = property.propertyPath;
        int lastDot = path.LastIndexOf('.');

        if (lastDot >= 0)
        {
            string siblingPath = path.Substring(0, lastDot + 1) + conditionFieldName;
            SerializedProperty sibling = property.serializedObject.FindProperty(siblingPath);
            if (sibling != null)
            {
                return sibling;
            }
        }

        return property.serializedObject.FindProperty(conditionFieldName);
    }
}
