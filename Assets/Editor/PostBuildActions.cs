using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Proccess data after build
/// Folder for libs in root, Frameworks
/// </summary>
public class PostBuildActions
{
    /// <summary>
    /// Run after project build
    /// Save version.txt file for build shell script
    /// </summary>
    /// <param name="buildTarget">Platform</param>
    /// <param name="path">Path to folder</param>
    [PostProcessBuild]
    public static void PostProcess(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS) return;

        ModifyXcodeProject(pathToBuiltProject);
        ModifyEntitlements(pathToBuiltProject);
        ModifyInfoPlist(pathToBuiltProject);
    }

    private static void ModifyXcodeProject(string pathToBuiltProject)
    {
        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

        string mainTarget = proj.GetUnityMainTargetGuid();
        string frameworkTarget = proj.GetUnityFrameworkTargetGuid();

        // Build Settings: ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES = NO 설정
        proj.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

        // In-App Purchase 활성화
        proj.AddFrameworkToProject(mainTarget, "StoreKit.framework", false);

        // .entitlements 파일 추가
        string entitlementsFileName = "Unity-iPhone.entitlements";
        string entitlementsPath = pathToBuiltProject + "/" + entitlementsFileName;
        proj.AddFile(entitlementsFileName, entitlementsFileName);
        proj.AddBuildProperty(mainTarget, "CODE_SIGN_ENTITLEMENTS", entitlementsFileName);

        proj.WriteToFile(projPath);
        UnityEngine.Debug.Log("Xcode 프로젝트 설정 완료!");
    }

    private static void ModifyEntitlements(string pathToBuiltProject)
    {
        string entitlementsPath = pathToBuiltProject + "/Unity-iPhone.entitlements";
        PlistDocument entitlements = new PlistDocument();

        // 기존 파일이 있다면 읽어오기
        if (File.Exists(entitlementsPath))
        {
            entitlements.ReadFromFile(entitlementsPath);
        }

        // Capabilities 추가
        entitlements.root.CreateArray("com.apple.developer.applesignin").AddString("Default");
        entitlements.root.SetString("aps-environment", "production"); // Push Notifications

        // .entitlements 파일 저장
        entitlements.WriteToFile(entitlementsPath);
        UnityEngine.Debug.Log(".entitlements 파일 설정 완료!");
    }

    private static void ModifyInfoPlist(string pathToBuiltProject)
    {
        // Info.plist 경로 설정
        string plistPath = pathToBuiltProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // NSUserTrackingUsageDescription 추가
        plist.root.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");

        // URL Types 추가
        PlistElementArray urlTypes;
        if (plist.root["CFBundleURLTypes"] == null)
        {
            urlTypes = plist.root.CreateArray("CFBundleURLTypes");
        }
        else
        {
            urlTypes = plist.root["CFBundleURLTypes"].AsArray();
        }

        PlistElementDict urlDict = urlTypes.AddDict();
        urlDict.SetString("CFBundleURLName", "io.staika.game.defengo"); // 앱의 번들 ID
        PlistElementArray urlSchemes = urlDict.CreateArray("CFBundleURLSchemes");
        urlSchemes.AddString("com.googleusercontent.apps.701023223608-9scd334cpgr6bnuanc7h8prbi6gg3ol5"); // 원하는 URL 스키마 추가 (ex: myapp://)

        // Info.plist 저장
        plist.WriteToFile(plistPath);
        UnityEngine.Debug.Log("Info.plist 수정 완료!");
    }
}