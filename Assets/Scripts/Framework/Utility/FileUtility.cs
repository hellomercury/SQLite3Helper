using System.Collections.Generic;
using System.IO;

public static class FileUtility
{
    public static List<FileInfo> GetAllFileInfoFromTheDirectory(string InDirectory)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(InDirectory);

        List<FileInfo> fileInfos = new List<FileInfo>();
        fileInfos.AddRange(directoryInfo.GetFiles());

        DirectoryInfo[] subDirectoryInfos = directoryInfo.GetDirectories();
        int length = subDirectoryInfos.Length;
        for (int i = 0; i < length; i++)
        {
            fileInfos.AddRange(GetAllFileInfoFromTheDirectory(subDirectoryInfos[i]));
        }

        return fileInfos;
    }

    public static List<FileInfo> GetAllFileInfoFromTheDirectory(DirectoryInfo InDirectoryInfo)
    {
        if (null == InDirectoryInfo) return null;
        else
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            fileInfos.AddRange(InDirectoryInfo.GetFiles());

            DirectoryInfo[] subDirectoryInfos = InDirectoryInfo.GetDirectories();
            int length = subDirectoryInfos.Length;
            for (int i = 0; i < length; i++)
            {
                fileInfos.AddRange(GetAllFileInfoFromTheDirectory(subDirectoryInfos[i]));
            }

            return fileInfos;
        }
    }
}