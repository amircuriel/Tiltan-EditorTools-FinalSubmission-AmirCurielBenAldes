using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

[Overlay(typeof(SceneView), "Weapon Designer/Weapon Quick Bar", "Weapon Quick Bar", defaultDockZone = DockZone.TopToolbar)]
/// Adds a tiny Scene View bar for quick weapon actions.
public class WeaponQuickBarOverlay : ToolbarOverlay
{
    public WeaponQuickBarOverlay() : base(
        OpenWeaponOverviewWindowButton.Id,
        PingBadWeaponButton.Id,
        ValidateWeaponsButton.Id)
    {
    }
}

[EditorToolbarElement(Id)]
internal class OpenWeaponOverviewWindowButton : EditorToolbarButton
{
    public const string Id = "Weapon Designer/Open Weapon Overview Window";

    public OpenWeaponOverviewWindowButton()
    {
        text = "OpenPanel";
        tooltip = "Open the Weapon Overview Window.";
        icon = EditorGUIUtility.IconContent("d_UnityEditor.InspectorWindow").image as Texture2D;
        clicked += WeaponOverviewWindow.Open;
    }
}

[EditorToolbarElement(Id)]
internal class PingBadWeaponButton : EditorToolbarButton
{
    public const string Id = "Weapon Designer/Ping Bad Weapon"; //I thought about using better termms than "bad weapon" but it's late at night so that's what you get Lior, sorry

    public PingBadWeaponButton()
    {
        text = "PingBad";
        tooltip = "Ping the first weapon asset with a validation issue.";
        icon = EditorGUIUtility.IconContent("console.warnicon.sml").image as Texture2D;
        clicked += PingFirstBadWeapon;
    }

    private static void PingFirstBadWeapon()
    {
        WeaponDesignerAssetUtility.WeaponInfo problem = WeaponDesignerAssetUtility.FirstProblem();
        if (problem.weapon == null)
        {
            EditorUtility.DisplayDialog("Weapon Check", "No broken weapon assets found.", "Nice");
            return;
        }

        WeaponDesignerAssetUtility.PingAndSelect(problem.weapon);
    }
}

[EditorToolbarElement(Id)]
internal class ValidateWeaponsButton : EditorToolbarButton
{
    public const string Id = "Weapon Designer/Validate Weapons";

    public ValidateWeaponsButton()
    {
        text = "Check";
        tooltip = "Run a quick weapon validation summary.";
        icon = EditorGUIUtility.IconContent("TestPassed").image as Texture2D;
        clicked += ShowValidation;
    }

    private static void ShowValidation()
    {
        var weapons = WeaponDesignerAssetUtility.FindWeapons();
        int problems = WeaponDesignerAssetUtility.CountProblems(weapons);
        EditorUtility.DisplayDialog("Weapon Check", $"{weapons.Count} weapon assets checked.\n{problems} need attention.", "OK");
    }
}
