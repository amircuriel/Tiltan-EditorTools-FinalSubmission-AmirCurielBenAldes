using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// This does the build checks for the weapon assets.
public class WeaponDesignerBuildTools : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    private static bool buildStartedFromTool;

    public int callbackOrder => 0;

    [MenuItem("Weapon Designer/Build With Weapon Check...", false, 60)]
    public static void BuildWithWeaponCheck()
    {
        buildStartedFromTool = true;
        try
        {
            List<WeaponDesignerAssetUtility.WeaponInfo> weapons = WeaponDesignerAssetUtility.FindWeapons();
            int problems = WeaponDesignerAssetUtility.CountProblems(weapons);

            if (problems > 0)
            {
                int choice = EditorUtility.DisplayDialogComplex(
                    "Weapon Check Before Build",
                    $"{problems} weapon asset(s) need attention.\n\nFix the simple issues before building?",
                    "Fix And Build",
                    "Cancel",
                    "Build Anyway");

                if (choice == 1)
                {
                    Debug.Log("Weapon Designer build was cancelled by the user.");
                    return;
                }

                if (choice == 0)
                {
                    WeaponDesignerAssetUtility.FixAllPossible(weapons);
                    weapons = WeaponDesignerAssetUtility.FindWeapons();
                    problems = WeaponDesignerAssetUtility.CountProblems(weapons);
                }

                if (problems > 0)
                {
                    bool keepGoing = EditorUtility.DisplayDialog(
                        "Build Still Has Warnings",
                        "Some weapon issues could not be fixed automatically, usually missing icons.\nBuild anyway?",
                        "Build Anyway",
                        "Cancel");

                    if (!keepGoing)
                    {
                        Debug.Log("Weapon Designer build was cancelled because weapon issues remain.");
                        return;
                    }
                }
            }

            BuildPlayerOptions options = MakeBuildOptions();
            if (options.scenes == null || options.scenes.Length == 0)
            {
                EditorUtility.DisplayDialog("No Scenes To Build", "There are no enabled scenes in Build Settings.", "OK");
                return;
            }

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogWarning($"Weapon Designer custom build ended with {report.summary.result}.");
            }
        }
        finally
        {
            buildStartedFromTool = false;
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        List<WeaponDesignerAssetUtility.WeaponInfo> weapons = WeaponDesignerAssetUtility.FindWeapons();
        int problems = WeaponDesignerAssetUtility.CountProblems(weapons);
        if (problems == 0)
        {
            Debug.Log($"Weapon Designer pre-build check passed. Checked {weapons.Count} weapon assets.");
            return;
        }

        if (buildStartedFromTool)
        {
            Debug.LogWarning($"Weapon Designer custom build is continuing with {problems} weapon warning(s).");
            return;
        }

        bool continueBuild = EditorUtility.DisplayDialog(
            "Weapon Issues Found",
            $"{problems} weapon asset(s) have validation issues.\nContinue the normal Unity build anyway?",
            "Continue (please don't)",
            "Cancel Build (please do)");

        if (!continueBuild)
        {
            throw new BuildFailedException("Build cancelled by Weapon Designer pre-build validation. Good job not fucking things up.");
        }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        int checkedCount = WeaponDesignerAssetUtility.FindWeapons().Count;
        Debug.Log($"Weapon Designer post-build check: {checkedCount} weapon assets were present. Result: {report.summary.result}.");
    }

    private static BuildPlayerOptions MakeBuildOptions()
    {
        string[] scenes = FindEnabledScenes();
        string folder = EditorUtility.SaveFolderPanel("Choose Build Folder", "Builds", "WeaponDesignerBuild");
        if (string.IsNullOrEmpty(folder))
        {
            return new BuildPlayerOptions { scenes = new string[0] };
        }

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        string productName = string.IsNullOrWhiteSpace(PlayerSettings.productName) ? "WeaponDesignerBuild" : PlayerSettings.productName;
        string fileName = target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64 ? productName + ".exe" : productName;

        return new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(folder, fileName),
            target = target,
            options = BuildOptions.None
        };
    }

    private static string[] FindEnabledScenes()
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }

        return scenes.ToArray();
    }
}
