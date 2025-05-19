using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class AutoImportSample
{
    static AutoImportSample()
    {
        // Go up from Assets/ to repo root
        string repoRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));

        // Build absolute paths
        string packageSamplePath = Path.Combine(repoRoot, "package", "Samples~", "BRT_Example");
        string projectSamplePath = Path.Combine(Application.dataPath, "Samples", "BRT_Example");

        if (!Directory.Exists(projectSamplePath) && Directory.Exists(packageSamplePath))
        {
            Debug.Log($"[AutoImportSample] Copying sample from {packageSamplePath} to {projectSamplePath}");

            Directory.CreateDirectory(Path.GetDirectoryName(projectSamplePath));
            CopyDirectory(packageSamplePath, projectSamplePath);

            AssetDatabase.Refresh();
            Debug.Log("[AutoImportSample] Sample copied successfully.");
        }
    }

    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourceDir, destinationDir));
        }

        foreach (string filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string destFile = filePath.Replace(sourceDir, destinationDir);
            Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            File.Copy(filePath, destFile, overwrite: true);
        }
    }
}
