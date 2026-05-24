using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// This is the little overview window for checking weapon assets.
public class WeaponOverviewWindow : EditorWindow
{
    private enum BoardFilter
    {
        All,
        BrokenOnly,
        MissingIconOrName,
        DamageAtLeast
    }

    private enum BoardSort
    {
        Name,
        Damage,
        UsesAmmo,
        MagazineSize,
        ReloadTime,
        Validation
    }

    private readonly List<WeaponDesignerAssetUtility.WeaponInfo> weapons = new List<WeaponDesignerAssetUtility.WeaponInfo>();
    private Vector2 scroll;
    private BoardFilter filter = BoardFilter.All;
    private BoardSort sort = BoardSort.Name;
    private bool descending;
    private float damageLimit = 20f;
    private GUIStyle titleStyle;

    [MenuItem("Weapon Designer/Weapon Overview Window", false, 10)]
    public static void Open()
    {
        WeaponOverviewWindow window = GetWindow<WeaponOverviewWindow>("Weapon Overview");
        window.minSize = new Vector2(430f, 310f);
        window.Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnGUI()
    {
        titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            wordWrap = true
        };

        EditorGUILayout.Space(6f);
        EditorGUILayout.LabelField("Weapon Overview Window", titleStyle);
        EditorGUILayout.LabelField("WeaponDefinition assets found with AssetDatabase.FindAssets.", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        filter = (BoardFilter)EditorGUILayout.EnumPopup(filter, GUILayout.MinWidth(150f));
        using (new EditorGUI.DisabledScope(filter != BoardFilter.DamageAtLeast))
        {
            damageLimit = EditorGUILayout.FloatField("Damage", damageLimit, GUILayout.MaxWidth(190f));
        }

        if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(70f)))
        {
            Refresh();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        sort = (BoardSort)EditorGUILayout.EnumPopup("Sort", sort);
        descending = GUILayout.Toggle(descending, descending ? "Descending" : "Ascending", EditorStyles.miniButton, GUILayout.Width(95f));
        EditorGUILayout.EndHorizontal();

        int problems = WeaponDesignerAssetUtility.CountProblems(weapons);
        EditorGUILayout.LabelField($"{weapons.Count} weapons / {problems} need attention", EditorStyles.boldLabel);

        SortWeapons();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        foreach (WeaponDesignerAssetUtility.WeaponInfo item in weapons)
        {
            if (!PassesFilter(item))
            {
                continue;
            }

            DrawWeaponRow(item);
        }
        EditorGUILayout.EndScrollView();
    }

    private void Refresh()
    {
        weapons.Clear();
        weapons.AddRange(WeaponDesignerAssetUtility.FindWeapons());
        Repaint();
    }

    private bool PassesFilter(WeaponDesignerAssetUtility.WeaponInfo item)
    {
        switch (filter)
        {
            case BoardFilter.BrokenOnly:
                return item.hasIssues;
            case BoardFilter.MissingIconOrName:
                return item.missingIcon || string.IsNullOrWhiteSpace(item.weapon.weaponName);
            case BoardFilter.DamageAtLeast:
                return item.weapon.damage != null && item.weapon.damage.damage >= damageLimit;
            default:
                return true;
        }
    }

    private void SortWeapons()
    {
        weapons.Sort(CompareWeapons);
    }

    private int CompareWeapons(WeaponDesignerAssetUtility.WeaponInfo left, WeaponDesignerAssetUtility.WeaponInfo right)
    {
        int result = sort switch
        {
            BoardSort.Damage => GetDamage(left).CompareTo(GetDamage(right)),
            BoardSort.UsesAmmo => left.weapon.usesAmmo.CompareTo(right.weapon.usesAmmo),
            BoardSort.MagazineSize => left.weapon.magazineSize.CompareTo(right.weapon.magazineSize),
            BoardSort.ReloadTime => left.weapon.reloadTime.CompareTo(right.weapon.reloadTime),
            BoardSort.Validation => left.hasIssues.CompareTo(right.hasIssues),
            _ => string.Compare(GetName(left), GetName(right), System.StringComparison.OrdinalIgnoreCase)
        };

        if (result == 0)
        {
            result = string.Compare(GetName(left), GetName(right), System.StringComparison.OrdinalIgnoreCase);
        }

        return descending ? -result : result;
    }

    private static string GetName(WeaponDesignerAssetUtility.WeaponInfo item)
    {
        return string.IsNullOrWhiteSpace(item.weapon.weaponName) ? item.weapon.name : item.weapon.weaponName;
    }

    private static float GetDamage(WeaponDesignerAssetUtility.WeaponInfo item)
    {
        return item.weapon.damage != null ? item.weapon.damage.damage : float.MinValue;
    }

    private void DrawWeaponRow(WeaponDesignerAssetUtility.WeaponInfo item)
    {
        Rect boxRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (item.hasIssues && Event.current.type == EventType.Repaint)
        {
            EditorGUI.DrawRect(boxRect, new Color(1f, 0.55f, 0.18f, 0.13f));
        }

        EditorGUILayout.BeginHorizontal();
        DrawIcon(item.weapon.icon, 45f);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(string.IsNullOrWhiteSpace(item.weapon.weaponName) ? "(unnamed weapon)" : item.weapon.weaponName, EditorStyles.boldLabel);
        string ammo = item.weapon.usesAmmo ? $"Ammo {item.weapon.magazineSize}, reload {item.weapon.reloadTime:0.##}" : "No ammo";
        float damage = item.weapon.damage?.damage ?? -1f;
        EditorGUILayout.LabelField($"Damage {damage:0.##}   {ammo}", EditorStyles.miniLabel);
        EditorGUILayout.LabelField(item.issues, item.hasIssues ? EditorStyles.wordWrappedMiniLabel : EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUILayout.Width(82f));
        if (GUILayout.Button("Ping", EditorStyles.miniButton))
        {
            WeaponDesignerAssetUtility.PingAndSelect(item.weapon);
        }

        if (GUILayout.Button("Select", EditorStyles.miniButton))
        {
            Selection.activeObject = item.weapon;
        }

        using (new EditorGUI.DisabledScope(!item.canAutoFix))
        {
            if (GUILayout.Button("Fix", EditorStyles.miniButton))
            {
                WeaponDesignerAssetUtility.FixWeapon(item.weapon);
                AssetDatabase.SaveAssets();
                Refresh();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private static void DrawIcon(Sprite sprite, float size)
    {
        Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
        if (!sprite || !sprite.texture)
        {
            GUI.Box(rect, "No icon", EditorStyles.helpBox);
            return;
        }

        Rect textureRect = sprite.textureRect;
        Rect uv = new Rect(textureRect.x / sprite.texture.width, textureRect.y / sprite.texture.height,
            textureRect.width / sprite.texture.width, textureRect.height / sprite.texture.height);
        GUI.DrawTextureWithTexCoords(rect, sprite.texture, uv, true);
    }
}
