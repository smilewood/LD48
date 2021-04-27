using UnityEditor;
using System.IO;
using System;

public class AddNameSpace : UnityEditor.AssetModificationProcessor
{
    public static void OnWillCreateAsset( string metaFilePath )
    {
        string fileName = Path.GetFileNameWithoutExtension(metaFilePath);

        if (!fileName.EndsWith(".cs"))
            return;

        string actualFile = $"{Path.GetDirectoryName(metaFilePath)}\\{fileName}";
        string[] segmentedPath = $"{Path.GetDirectoryName(metaFilePath)}".Split(new[] { '\\' }, StringSplitOptions.None);

        string generatedNamespace = "";
        string finalNamespace;

        // In case of placing the class at the root of a folder such as (Editor, Scripts, etc...)  
        if (segmentedPath.Length <= 2)
            finalNamespace = EditorSettings.projectGenerationRootNamespace;
        else
        {
            // Skipping the Assets folder and a single subfolder (i.e. Scripts, Editor, Plugins, etc...)
            for (var i = 2; i < 3/*segmentedPath.Length*/; ++i)
            {
                // Don't add '.' at the end of the namespace
                generatedNamespace += i == segmentedPath.Length - 1 ? segmentedPath[i] : segmentedPath[i] + ".";
                
            }

            finalNamespace = EditorSettings.projectGenerationRootNamespace + "." + generatedNamespace;
        }

        string content = File.ReadAllText(actualFile);
        string newContent = content.Replace("#NAMESPACE#", finalNamespace);

        if (content != newContent)
        {
            File.WriteAllText(actualFile, newContent);
            AssetDatabase.Refresh();
        }
    }
}