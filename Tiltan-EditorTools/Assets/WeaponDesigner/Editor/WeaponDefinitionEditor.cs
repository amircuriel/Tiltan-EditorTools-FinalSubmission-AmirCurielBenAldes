using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponDefinition))]
public class WeaponDefinitionEditor : Editor
{
    private static readonly Color InvalidSectionColor = new Color(1f, 0.35f, 0.35f, 0.28f);

    private SerializedProperty weaponName;
    private SerializedProperty icon;
    private SerializedProperty description;
    private SerializedProperty damage;
    private SerializedProperty usesAmmo;
    private SerializedProperty magazineSize;
    private SerializedProperty reloadTime;

    private GUIStyle titleStyle;

    private void OnEnable()
    {
        weaponName = serializedObject.FindProperty(nameof(WeaponDefinition.weaponName));
        icon = serializedObject.FindProperty(nameof(WeaponDefinition.icon));
        description = serializedObject.FindProperty(nameof(WeaponDefinition.description));
        damage = serializedObject.FindProperty(nameof(WeaponDefinition.damage));
        usesAmmo = serializedObject.FindProperty(nameof(WeaponDefinition.usesAmmo));
        magazineSize = serializedObject.FindProperty(nameof(WeaponDefinition.magazineSize));
        reloadTime = serializedObject.FindProperty(nameof(WeaponDefinition.reloadTime));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        WeaponDefinition weapon = (WeaponDefinition)target;
        DrawHeader(weapon);
        bool weaponInfoMissing = string.IsNullOrWhiteSpace(weapon.weaponName) || icon.objectReferenceValue == null;
        bool damageInvalid = weapon.damage != null && !weapon.damage.IsValid;
        bool ammoInvalid = usesAmmo.boolValue && (magazineSize.intValue <= 0 || reloadTime.floatValue < 0f);

        BeginSimpleSection("Weapon Info", weaponInfoMissing);
        EditorGUILayout.PropertyField(weaponName);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(description);
        if (string.IsNullOrWhiteSpace(weapon.weaponName))
        {
            EditorGUILayout.HelpBox("Weapon name is required.", MessageType.Error);
        }

        if (icon.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("No icon is assigned.", MessageType.Warning);
        }
        EndSimpleSection();

        BeginSimpleSection("Damage", damageInvalid);
        EditorGUILayout.PropertyField(damage);
        EndSimpleSection();

        BeginSimpleSection("Ammo", ammoInvalid);
        EditorGUILayout.PropertyField(usesAmmo);
        EditorGUILayout.PropertyField(magazineSize);
        EditorGUILayout.PropertyField(reloadTime);
        if (!usesAmmo.boolValue)
        {
            EditorGUILayout.HelpBox("Ammo fields are hidden because this weapon does not use ammo.", MessageType.Info);
        }
        else
        {
            if (magazineSize.intValue <= 0)
            {
                EditorGUILayout.HelpBox("Magazine size must be greater than 0.", MessageType.Error);
            }

            if (reloadTime.floatValue < 0f)
            {
                EditorGUILayout.HelpBox("Reload time cannot be negative.", MessageType.Error);
            }
        }
        EndSimpleSection();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(WeaponDefinition weapon)
    {
        titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            wordWrap = true
        };

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        DrawIcon(icon.objectReferenceValue as Sprite, 64f, "No Icon");

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField(string.IsNullOrWhiteSpace(weapon.weaponName) ? "Unnamed Weapon" : weapon.weaponName, titleStyle);
        EditorGUILayout.LabelField("Simple weapon data asset", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private static void BeginSimpleSection(string title, bool isInvalid)
    {
        EditorGUILayout.Space(6f);
        Rect sectionRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        if (isInvalid && Event.current.type == EventType.Repaint)
        {
            EditorGUI.DrawRect(sectionRect, InvalidSectionColor);
        }

        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
    }

    private static void EndSimpleSection()
    {
        EditorGUILayout.EndVertical();
    }

    private static void DrawIcon(Sprite sprite, float size, string placeholder)
    {
        Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
        if (sprite != null && TryDrawSprite(rect, sprite))
        {
            return;
        }

        GUI.Box(rect, placeholder, EditorStyles.helpBox);
    }

    private static bool TryDrawSprite(Rect rect, Sprite sprite)
    {
        try
        {
            Rect textureRect = sprite.textureRect;
            Texture2D texture = sprite.texture;

            if (texture == null)
            {
                return false;
            }

            Rect uv = new Rect(
                textureRect.x / texture.width,
                textureRect.y / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height);

            GUI.DrawTextureWithTexCoords(rect, texture, uv, true);
            return true;
        }
        catch (MissingReferenceException)
        {
            return false;
        }
    }
}
