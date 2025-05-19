using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class AutoImportSample
{
    static AutoImportSample()
    {
        // Define paths
        string packageSamplePath = "../package/Samples~/BRT_Example";
        string projectSamplePath = "Assets/Samples/BRT_Example";

        // Only copy if the sample isn't already imported
        if (!Directory.Exists(projectSamplePath) && Directory.Exists(packageSamplePath))
        {
            Debug.Log("[AutoImportSample] Copying sample from package to project...");

            // Ensure destination directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(projectSamplePath));

            // Recursively copy sample folder
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
            File.Copy(filePath, filePath.Replace(sourceDir, destinationDir), overwrite: true);
        }
    }
}
