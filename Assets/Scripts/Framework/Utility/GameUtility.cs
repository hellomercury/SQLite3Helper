using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public static class GameUtility
{
    public static string GetStreamingAssetsPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return "jar:file://" + Application.dataPath + "!/assets/";

            case RuntimePlatform.IPhonePlayer:
                return Application.dataPath + "/Raw/";

            default:
                return Application.streamingAssetsPath;
        }
    }

    public static void CopyFileFromSAPathToPDPath(string InSAName, string InPDPName = null, Action InOnCopyFinishedAction = null)
    {
        Assert.IsFalse(string.IsNullOrEmpty(InSAName));
        if (string.IsNullOrEmpty(InPDPName)) InPDPName = InSAName;
        string streamingAssetPath = Path.Combine(GetStreamingAssetsPath(), InSAName);
        string persistentDataPath = Path.Combine(Application.persistentDataPath, InPDPName);

#if !UNITY_EDITOR && UNITY_ANDROID
            using (WWW www = new WWW(streamingAssetPath))
            {
                while (!www.isDone) { }

                if (www.isDone && string.IsNullOrEmpty(www.error))
                {
                    File.WriteAllBytes(persistentDataPath, www.bytes);
                    if(null != InOnCopyFinishedAction) InOnCopyFinishedAction.Invoke();
                }
                else Debug.LogError("[CopyFileFromSAPathToPDPath]    error : " + www.error);
            }
#else
        File.Copy(streamingAssetPath, persistentDataPath, true);
        if (null != InOnCopyFinishedAction) InOnCopyFinishedAction.Invoke();
#endif
    }
}