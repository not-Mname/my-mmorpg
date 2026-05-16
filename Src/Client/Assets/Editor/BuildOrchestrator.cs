using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using HybridCLR.Editor.Commands;
using UnityEditor.Build.Reporting;

public class BuildOrchestrator
{
    private static string SourceDir =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "../../AssetBundle/Windows"));
    private static string DestDir =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "../../TestResourceServer/Web/AssetBundle/Windows"));
    private static string BinDir =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "../bin"));

    [MenuItem("MMORPG/ResourceTools/ResourceBuild/Build And Deploy")]
    public static void BuildAndDeploy()
    {
        UnityEngine.Debug.Log("[BuildOrchestrator] Step 1/8: Run Excel2Json.cmd...");
        RunCmdFile(Path.Combine(Application.dataPath, "../../Data/Excel2Json.cmd"));

        UnityEngine.Debug.Log("[BuildOrchestrator] Step 2/8: Export Teleporters...");
        MapTools.ExportTeleporters();

        UnityEngine.Debug.Log("[BuildOrchestrator] Step 3/8: Export SpawnPoints...");
        MapTools.ExportSpawnPoints();

        UnityEngine.Debug.Log("[BuildOrchestrator] Step 4/8: Run CopyFromSrcData.cmd...");
        RunCmdFile(Path.Combine(Application.dataPath, "../Data/CopyFromSrcData.cmd"));

        UnityEngine.Debug.Log("[BuildOrchestrator] Step 5/8: HybridCLR GenerateAll (compile DLLs + AOT metadata)...");
        PrebuildCommand.GenerateAll();

        UnityEngine.Debug.Log("[BuildOrchestrator] Step 6/8: Build AssetBundles...");
        AssetBundleFramework.Builder.BuildWindows();

        UnityEngine.Debug.Log("[BuildOrchestrator] Step 7/8: Copy to web server...");
        CopyRecursive(SourceDir, DestDir);

        UnityEngine.Debug.Log("[BuildOrchestrator] Step 8/8: Build Windows Player to bin/...");
        Directory.CreateDirectory(BinDir);
        var report = BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
            Path.Combine(BinDir, "ExtremeWorld.exe"),
            BuildTarget.StandaloneWindows64,
            BuildOptions.None);
        if (report.summary.result == BuildResult.Succeeded)
            UnityEngine.Debug.Log($"[BuildOrchestrator] Player build succeeded: {report.summary.outputPath}");
        else
            UnityEngine.Debug.LogError($"[BuildOrchestrator] Player build failed: {report.summary}");

        UnityEngine.Debug.Log("[BuildOrchestrator] Done.");
        AssetDatabase.Refresh();
    }

    [MenuItem("MMORPG/ResourceTools/ResourceBuild/Build And Deploy (Lightweight)")]
    public static void BuildAndDeployLightweight()
    {
        HybridCLR.Editor.Commands.CompileDllCommand.CompileDllActiveBuildTargetDevelopment();

        AssetBundleFramework.Builder.BuildWindows();

        CopyRecursive(SourceDir, DestDir);

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 同步执行 .cmd 脚本，等待进程退出
    /// </summary>
    /// <param name="path">.cmd 文件的绝对路径</param>
    private static void RunCmdFile(string path)
    {
        path = Path.GetFullPath(path);
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError($"[BuildOrchestrator] 脚本文件不存在: {path}");
            return;
        }

        string workingDir = Path.GetDirectoryName(path);
        string fileName = Path.GetFileName(path);

        var psi = new ProcessStartInfo("cmd.exe", $"/c \"{fileName}\"")
        {
            WorkingDirectory = workingDir,
            UseShellExecute = true,
        };

        try
        {
            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                    UnityEngine.Debug.LogError($"[BuildOrchestrator] 脚本退出码 {process.ExitCode}: {path}");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
    }

    private static void CopyRecursive(string src, string dst)
    {
        Directory.CreateDirectory(dst);
        foreach (var f in Directory.GetFiles(src))
            File.Copy(f, Path.Combine(dst, Path.GetFileName(f)), true);
        foreach (var d in Directory.GetDirectories(src))
            CopyRecursive(d, Path.Combine(dst, Path.GetFileName(d)));
    }
}
