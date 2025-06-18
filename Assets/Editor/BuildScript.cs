using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript
{
    private static string[] scenes = { "Assets/01.Scenes/InitScene.unity", "Assets/01.Scenes/LoginScene.unity", "Assets/01.Scenes/LobbyScene.unity", "Assets/01.Scenes/GameScene.unity" };
    private static readonly string[] BUILD_OUTPUT_PATH = { "../BuildOutput/Android", "../BuildOutput/iOS" };
    private static string ENV_STAGE = "__STAGE__";
    private static string ENV_PRODUCTION = "__PRODUCTION__";
    private static Dictionary<ArgType, string> dicArgValues = new Dictionary<ArgType, string>();

    [MenuItem("Build/JenkinsBuild")]
    public static void JenkinsBuild()
    {
        InitializeDictionaryArgs();

        var platform = GetArgs<PLATFORM>(ArgType.PLATFORM);
        var server = GetArgs<SERVER>(ArgType.SERVER);
        var extension = GetArgs<EXTENSION>(ArgType.EXTENSION);

        // AAB 파일 생성을 위한 설정
        EditorUserBuildSettings.buildAppBundle = extension == EXTENSION.AAB;

        var buildGroup = GetBuildTargetGroup(platform);
        var buildTarget = GetBuildTarget(platform);

        var buildOutputhPath = GetBuildOutputPath(platform);
        if (!Directory.Exists(buildOutputhPath))
        {
            Directory.CreateDirectory(buildOutputhPath);
        }

        SetApplicationIdentifier();

        PlayerSettings.Android.keystoreName = "Keystore/defengo.keystore";
        PlayerSettings.keystorePass = "defengo";
        PlayerSettings.Android.keyaliasName = "defengo";
        PlayerSettings.keyaliasPass = "defengo";

        PlayerSettings.bundleVersion = GetArgs(ArgType.VERSIONNAME);

        if (int.TryParse(GetArgs(ArgType.BUILDNUMBER), out var buildNumber))
        {
            PlayerSettings.Android.bundleVersionCode = buildNumber;
            PlayerSettings.iOS.buildNumber = buildNumber.ToString();
        }

        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.appleDeveloperTeamID = "T726HG3M98";
        PlayerSettings.iOS.iOSManualProvisioningProfileID = "c0ff3634-64b1-4282-a45b-a598c3525f75";
        PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;

        try
        {
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions()
            {
                scenes = scenes,
                locationPathName = GetOutputBuildPath(platform),
                target = buildTarget,
                targetGroup = buildGroup,
                extraScriptingDefines = new string[]
                { server == SERVER.STAGE ? ENV_STAGE : ENV_PRODUCTION },
            });
            BuildResultReport(report);
        }
        catch (Exception)
        {
            Debug.Log("Android Build Error");
        }
    }

    #region Util
    private static void InitializeDictionaryArgs()
    {
        var args = Environment.GetCommandLineArgs();

        dicArgValues.Clear();

        for (int i = 0; i < args.Length; i++)
        {
            if (false == int.TryParse(args[i], out _))
            {
                if (Enum.TryParse<ArgType>(args[i].Replace("-", ""), out var result))
                {
                    Debug.Log($"Key : {result} | Value : {args[i + 1]}");
                    dicArgValues.Add(result, args[i + 1]);
                }
            }
        }

        for (ArgType i = 0; i <= ArgType.PLATFORM; ++i)
        {
            if (false == dicArgValues.ContainsKey(i))
            {
                switch (i)
                {
                    case ArgType.SERVER:
                        dicArgValues.Add(i, SERVER.STAGE.ToString());
                        break;
                    case ArgType.EXTENSION:
                        dicArgValues.Add(i, EXTENSION.APK.ToString());
                        break;
                    case ArgType.VERSIONNAME:
                        dicArgValues.Add(i, "2.14.0");
                        break;
                    case ArgType.BUILDNUMBER:
                        dicArgValues.Add(i, "165");
                        break;
                    case ArgType.PLATFORM:
                        dicArgValues.Add(i, PLATFORM.Android.ToString()); 
                        break;
                    default:
                        dicArgValues.Add(i, string.Empty);
                        break;
                }
            }
        }
    }
    private static string GetArgs(ArgType eType)
    {
        return dicArgValues.TryGetValue(eType, out var val) ? val : string.Empty;
    }

    private static T GetArgs<T>(ArgType eType) where T : struct, Enum
    {
        var result = GetArgs(eType);

        if (string.IsNullOrEmpty(result))
        {
            return default;
        }

        if (Enum.TryParse<T>(result, out var ret))
        {
            return ret;
        }

        return default;
    }
    private static BuildTargetGroup GetBuildTargetGroup(PLATFORM ePlatform)
    {
        return ePlatform == PLATFORM.Android ? BuildTargetGroup.Android : BuildTargetGroup.iOS;
    }
    private static BuildTarget GetBuildTarget(PLATFORM ePlatform)
    {
        return ePlatform == PLATFORM.Android ? BuildTarget.Android : BuildTarget.iOS;
    }
    private static string GetBuildOutputPath(PLATFORM ePlatform)
    {
        return ePlatform == PLATFORM.Android ? BUILD_OUTPUT_PATH[0] : BUILD_OUTPUT_PATH[1];
    }
    private static bool IsLive()
    {
        return GetArgs<SERVER>(ArgType.SERVER) == SERVER.LIVE;
    }
    private static string GetOutputBuildPath(PLATFORM ePlatform)
    {
        var buildOutputhPath = GetBuildOutputPath(ePlatform);
        switch (ePlatform)
        {
            case PLATFORM.Android:
                return $"{buildOutputhPath}/Defengo_{GetArgs(ArgType.VERSIONNAME)}_{GetArgs(ArgType.BUILDNUMBER)}_{GetArgs(ArgType.SERVER).ToLower()}.{GetArgs(ArgType.EXTENSION).ToLower()}";
            case PLATFORM.iOS:
                return $"{buildOutputhPath}";
            default:
                return string.Empty;
        }
    }
    private static void SetApplicationIdentifier()
    {
        var identifier = "io.staika.game.defengo";

        var platform = GetArgs<PLATFORM>(ArgType.PLATFORM);

        if (platform == PLATFORM.Android)
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, $"{identifier}");
        }
        else
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, $"{identifier}");
        }
    }
    #endregion
    #region enum
    enum ArgType
    {
        SERVER,
        EXTENSION,
        VERSIONNAME,
        PLATFORM,
        BUILDNUMBER,
    }
    enum SERVER
    {
        STAGE,
        LIVE,
    }
    enum EXTENSION
    {
        APK,
        AAB,
    }
    enum PLATFORM
    {
        None,
        Android,
        iOS,
    }
    #endregion

    private static void BuildResultReport(BuildReport report)
    {
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {report.summary.outputPath}");
        }
        else
        {
            Debug.LogError("Build failed: " + report.summary.result);
            Debug.LogError("totalErrors: " + report.summary.totalErrors);
            Debug.LogError("totalWarnings: " + report.summary.totalWarnings);
            Debug.LogError("totalTime: " + report.summary.totalTime);
            Debug.LogError("outputPath: " + report.summary.outputPath);
            foreach (var step in report.steps)
            {
                Debug.LogError("      step: " + step.name);
                foreach (var message in step.messages)
                {
                    Debug.LogError("Message: " + message.content);
                }
            }
        }
    }
}