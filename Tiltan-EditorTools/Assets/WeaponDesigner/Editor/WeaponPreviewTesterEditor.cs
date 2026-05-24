using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponPreviewTester))]
public class WeaponPreviewTesterEditor : Editor
{
    private SerializedProperty weapon;

    private GUIStyle titleStyle;

    private void OnEnable()
    {
        weapon = serializedObject.FindProperty(nameof(WeaponPreviewTester.weapon));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        WeaponPreviewTester tester = (WeaponPreviewTester)target;
        DrawHeader(tester.weapon);

        if (tester.weapon == null)
        {
            EditorGUILayout.HelpBox("No Weapon Definition assigned.", MessageType.Error);
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Weapon Asset", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(weapon);
        DrawButtons(tester);
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader(WeaponDefinition selectedWeapon)
    {
        titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            wordWrap = true
        };

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        DrawIcon(selectedWeapon != null ? selectedWeapon.icon : null, 56f, "No Weapon");

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField(selectedWeapon != null ? selectedWeapon.weaponName : "Weapon Preview Tester", titleStyle);

        if (selectedWeapon != null)
        {
            EditorGUILayout.LabelField($"Damage: {selectedWeapon.damage.damage:0}", EditorStyles.miniLabel);
        }
        else
        {
            EditorGUILayout.LabelField("Assign a weapon asset to preview it.", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawButtons(WeaponPreviewTester tester)
    {
        using (new EditorGUI.DisabledScope(tester.weapon == null))
        {
            if (GUILayout.Button("Ping Weapon Asset", EditorStyles.miniButton))
            {
                EditorGUIUtility.PingObject(tester.weapon);
                Selection.activeObject = tester.weapon;
            }
        }
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
