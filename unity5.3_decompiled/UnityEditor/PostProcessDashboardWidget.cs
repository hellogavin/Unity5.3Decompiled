using System;
using UnityEditor;

internal class PostProcessDashboardWidget
{
    public static void PostProcess(BuildTarget target, string installPath, string stagingArea, string playerPackage, string companyName, string productName, int width, int height)
    {
        string path = stagingArea + "/DashboardBuild";
        FileUtil.DeleteFileOrDirectory(path);
        FileUtil.CopyFileOrDirectory(playerPackage, path);
        FileUtil.MoveFileOrDirectory("Temp/unitystream.unity3d", path + "/widget.unity3d");
        PostprocessBuildPlayer.InstallPlugins(path + "/Plugins", target);
        string str2 = PostprocessBuildPlayer.GenerateBundleIdentifier(companyName, productName) + ".widget";
        int num = width + 0x20;
        int num2 = height + 0x20;
        string[] input = new string[] { "UNITY_WIDTH_PLUS32", num.ToString(), "UNITY_HEIGHT_PLUS32", num2.ToString(), "UNITY_WIDTH", width.ToString(), "UNITY_HEIGHT", height.ToString(), "UNITY_BUNDLE_IDENTIFIER", str2, "UNITY_BUNDLE_NAME", productName };
        FileUtil.ReplaceText(path + "/UnityWidget.html", input);
        FileUtil.ReplaceText(path + "/Info.plist", input);
        FileUtil.DeleteFileOrDirectory(installPath);
        FileUtil.MoveFileOrDirectory(path, installPath);
    }
}

