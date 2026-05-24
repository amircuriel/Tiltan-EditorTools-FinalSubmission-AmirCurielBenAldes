using UnityEditor;
using UnityEngine;

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
}
