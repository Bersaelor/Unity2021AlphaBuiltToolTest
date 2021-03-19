using System;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using LooC.Meta;

public class BuildTools : Editor, IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get { return 0; }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log(
            $"[PlotBuildTools] OnPreprocessBuild for target {report.summary.platform} at path {report.summary.outputPath}");

        #region Set Build Properties LastBuildTime - Reference: https: //answers.unity.com/questions/1425758/how-can-i-find-all-instances-of-a-scriptable-objec.html
        //FindAssets uses tags check documentation for more info
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(BuildSettings)}");
        if (guids.Length > 1)
            Debug.LogErrorFormat("[PlotBuildTools] Found more than 1 Build Properties: {0}. Using first one!",
                guids.Length);

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            BuildSettings buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettings>(path);
            buildSettings.LastBuildTime = DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss"); // case sensitive
            buildSettings.AndroidBundleVersionCode = BuildSettings.GetBundleVersionCode();
            EditorUtility.SetDirty(buildSettings);
            Debug.LogFormat("[PlotBuildTools] Updated settings LastBuildDate to \"{0}\", bundle to \"{1}\". Settings Path: {2}",
                buildSettings.LastBuildTime, buildSettings.AndroidBundleVersionCode, path);
        }
        else
        {
            // TODO: AUTO-CREATE ONE!
            Debug.LogWarning("[PlotBuildTools] Couldn't find Build Settings, please create one!");
        }
        #endregion
    }
}

public class PostBuildTools: Editor, IPostprocessBuildWithReport {
    public int callbackOrder { get { return 100; } }
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("MyCustomBuildProcessor.OnPostprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);

        // Directory.Delete(streamingAssetsPath, true);
    }
}