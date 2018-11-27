using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class WebGLBuilder : MonoBehaviour {
	
	[MenuItem("Build/Build WebGL")]
    public static void build()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] 
		
		{ 
			"Assets/Scenes/Login_Menu.unity",
			"Assets/Scenes/Main_Menu.unity",
			"Assets/Scenes/Map1.unity",
            "Assets/Scenes/Control_Screen.unity"
        };

        buildPlayerOptions.locationPathName = "build";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}
