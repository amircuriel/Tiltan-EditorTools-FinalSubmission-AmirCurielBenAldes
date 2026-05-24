using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// Small helper stuff for finding and fixing weapon assets.
internal static class WeaponDesignerAssetUtility
{
    internal struct WeaponInfo
    {
        public WeaponDefinition weapon;
        public string path;
        public string issues;
        public bool hasIssues;
        public bool canAutoFix;
        public bool missingIcon;
    }

    internal static List<WeaponInfo> FindWeapons()
    {
        var guids = AssetDatabase.FindAssets("t:WeaponDefinition");
        List<WeaponInfo> weapons = new List<WeaponInfo>();

        
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var weapon = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(path);
            if (!weapon)
            {
                continue;
            }

            weapons.Add(CheckWeapon(weapon, path));
        }

        
        weapons.Sort((a, b) => string.Compare(a.weapon.weaponName, b.weapon.weaponName, System.StringComparison.OrdinalIgnoreCase));
        return weapons;
    }

    internal static WeaponInfo CheckWeapon(WeaponDefinition weapon)
    {
        var path = AssetDatabase.GetAssetPath(weapon);
        return CheckWeapon(weapon, path);
    }

    private static WeaponInfo CheckWeapon(WeaponDefinition weapon, string path)
    {
        var text = new StringBuilder();
        var fixable = false;
        var missingIcon = false;

        
        if (string.IsNullOrWhiteSpace(weapon.weaponName))
        {
            text.Append("Missing name. ");
            fixable = true;
        }

        if (!weapon.icon)
        {
            text.Append("Missing icon. ");
            missingIcon = true;
        }

        if (weapon.damage == null)
        {
            text.Append("Damage object is missing. ");
            fixable = true;
        }
        else if (weapon.damage.damage < 0f)
        {
            text.Append("Negative damage. ");
            fixable = true;
        }

        if (weapon.usesAmmo)
        {
            if (weapon.magazineSize <= 0)
            {
                text.Append("Bad magazine. ");
                fixable = true;
            }

            if (weapon.reloadTime < 0f)
            {
                //we can't have a weapon with negative reload time (unless of course, you go back in time when you reload, which is hella sick,
                //but then raises a time travel paradox of "is the weapon currently reloaded if I travelled back in time before it was reloaded???")
                text.Append("Negative reload. "); 
                fixable = true;
            }
        }

        
        bool hasIssues = text.Length > 0;
        return new WeaponInfo
        {
            weapon = weapon,
            path = path,
            issues = hasIssues ? text.ToString().Trim() : "OK",
            hasIssues = hasIssues,
            canAutoFix = fixable,
            missingIcon = missingIcon
        };
    }

    internal static int FixWeapon(WeaponDefinition weapon)
    {
        if (!weapon)
        {
            return 0;
        }

        var fixes = 0;
        Undo.RecordObject(weapon, "Quick Fix Weapon");

        if (string.IsNullOrWhiteSpace(weapon.weaponName))
        {
            weapon.weaponName = ObjectNames.NicifyVariableName(weapon.name);
            fixes++;
        }

        if (weapon.damage == null)
        {
            weapon.damage = new DamageStat();
            fixes++;
        }
        else if (weapon.damage.damage < 0f)
        {
            weapon.damage.damage = Mathf.Abs(weapon.damage.damage);
            fixes++;
        }

        if (weapon.usesAmmo && weapon.magazineSize <= 0)
        {
            weapon.magazineSize = 1;
            fixes++;
        }

        if (weapon.usesAmmo && weapon.reloadTime < 0f)
        {
            weapon.reloadTime = 0f;
            fixes++;
        }

        if (fixes > 0)
        {
            EditorUtility.SetDirty(weapon);
        }

        return fixes;
    }

    internal static void FixAllPossible(IEnumerable<WeaponInfo> weapons)
    {
        int count = weapons.Where(item => item.canAutoFix).Sum(item => FixWeapon(item.weapon));

        if (count > 0)
        {
            AssetDatabase.SaveAssets();
        }
    }

    internal static WeaponInfo FirstProblem()
    {
        foreach (var item in FindWeapons().Where(item => item.hasIssues))
        {
            return item;
        }

        return default;
    }

    internal static int CountProblems(List<WeaponInfo> weapons)
    {
        return weapons.Count(item => item.hasIssues);
    }

    internal static void PingAndSelect(Object target)
    {
        if (target == null)
        {
            return;
        }

        Selection.activeObject = target;
        EditorGUIUtility.PingObject(target);
    }
}
