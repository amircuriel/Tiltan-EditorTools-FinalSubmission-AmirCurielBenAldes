using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// Holds the simple menu tools for the weapon designer.
public static class WeaponDesignerTools
{
    private const string PreviewTesterPath = "Assets/WeaponDesigner/Examples/WeaponPreviewTester.prefab";
    private const string DefaultWeaponPath = "Assets/WeaponDesigner/Examples/Example_Pistol.asset";

    [MenuItem("Weapon Designer/Create Preview Tester Prefab", false, 1)]
    public static void CreatePreviewTesterPrefab()
    {
        WeaponDefinition weapon = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(DefaultWeaponPath);

        GameObject previewObject = new GameObject("WeaponPreviewTester");
        WeaponPreviewTester tester = previewObject.AddComponent<WeaponPreviewTester>();
        tester.weapon = weapon;

        PrefabUtility.SaveAsPrefabAsset(previewObject, PreviewTesterPath);
        Object.DestroyImmediate(previewObject);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Object prefab = AssetDatabase.LoadAssetAtPath<Object>(PreviewTesterPath);
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }

    [MenuItem("Weapon Designer/Find Broken Weapons %#f", false, 20)]
    public static void FindBrokenWeapons()
    {
        List<WeaponDesignerAssetUtility.WeaponInfo> weapons = WeaponDesignerAssetUtility.FindWeapons();
        List<Object> broken = new List<Object>();

        foreach (WeaponDesignerAssetUtility.WeaponInfo item in weapons)
        {
            if (item.hasIssues)
            {
                broken.Add(item.weapon);
            }
        }

        if (broken.Count == 0)
        {
            EditorUtility.DisplayDialog("Weapon Search", "No broken weapon assets were found.", "OK");
            return;
        }

        Selection.objects = broken.ToArray();
        EditorGUIUtility.PingObject(broken[0]);
        EditorUtility.DisplayDialog("Weapon Search", $"Selected {broken.Count} weapon asset(s) with issues.", "OK");
    }

    [MenuItem("Weapon Designer/Select Strongest Weapon %&s", false, 21)]
    public static void SelectStrongestWeapon()
    {
        WeaponDefinition strongest = null;
        float bestDamage = float.MinValue;

        foreach (WeaponDesignerAssetUtility.WeaponInfo item in WeaponDesignerAssetUtility.FindWeapons())
        {
            if (item.weapon.damage != null && item.weapon.damage.damage > bestDamage)
            {
                strongest = item.weapon;
                bestDamage = item.weapon.damage.damage;
            }
        }

        if (strongest == null)
        {
            EditorUtility.DisplayDialog("Weapon Search", "No weapon assets were found.", "OK");
            return;
        }

        WeaponDesignerAssetUtility.PingAndSelect(strongest);
    }

    [MenuItem("Weapon Designer/Quick Fix Selected Weapon %&w", false, 22)]
    public static void QuickFixSelectedWeapon()
    {
        WeaponDefinition weapon = Selection.activeObject as WeaponDefinition;
        if (weapon == null)
        {
            EditorUtility.DisplayDialog("Weapon Fix", "Select a WeaponDefinition asset first.", "OK");
            return;
        }

        int fixedFields = WeaponDesignerAssetUtility.FixWeapon(weapon);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Weapon Fix", fixedFields == 0 ? "Nothing simple to fix." : $"Fixed {fixedFields} field(s).", "OK");
    }
}
