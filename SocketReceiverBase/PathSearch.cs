using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PathSearchClass
{
    public static class PathSearch
    {
        public static string NewestFile(string directoryPath, string searchPattern = "*.txt")
        {
            var newestDirectory = new DirectoryInfo(directoryPath)
                .EnumerateDirectories()
                .OrderByDescending(d => d.CreationTimeUtc)
                .FirstOrDefault();

            if (newestDirectory == null)
            {
                return "";
            }

            var newestFile = newestDirectory
                .EnumerateFiles(searchPattern)
                .OrderByDescending(f => f.CreationTimeUtc)
                .FirstOrDefault();

            if (newestFile == null)
            {
                return "";
            }
            else
            {
                return newestFile.FullName;
            }
        }

        public static string[] NewerFiles(string directoryPath, DateTime specifiedTime, string searchPattern = "*.*")
        {
            var allFiles = new DirectoryInfo(directoryPath).EnumerateFiles(searchPattern, SearchOption.AllDirectories);
            var newerFiles = allFiles.Where(f => f.CreationTimeUtc > specifiedTime).Select(f => f.FullName).ToArray();

            return newerFiles;
        }

        public static string[] NewerFiles(string[] directoryPaths, DateTime specifiedTime, string searchPattern = "*.*")
        {
            List<string> newerFiles = new List<string>();

            foreach (var directoryPath in directoryPaths)
            {
                newerFiles.AddRange(NewerFiles(directoryPath, specifiedTime, searchPattern));
            }
            return newerFiles.ToArray();
        }

        public static string[] NewerFilesLatestDays(string directoryPath, int days, string searchPattern = "*.*")
        {
            DateTime specifiedTime = DateTime.Now.AddDays(-days);
            return NewerFiles(directoryPath, specifiedTime, searchPattern);
        }

        public static string[] NewerFilesLatestDays(string[] directoryPaths, int days, string searchPattern = "*.*")
        {
            List<string> newerFiles = new List<string>();

            foreach (var directoryPath in directoryPaths)
            {
                newerFiles.AddRange(NewerFilesLatestDays(directoryPath, days, searchPattern));
            }
            return newerFiles.ToArray();
        }

        public static string[] NewerFilesLatestMonths(string directoryPath, int months, string searchPattern = "*.*")
        {
            DateTime specifiedTime = DateTime.Now.AddMonths(-months);
            return NewerFiles(directoryPath, specifiedTime, searchPattern);
        }

        public static string[] NewerFilesLatestMonths(string[] directoryPaths, int months, string searchPattern = "*.*")
        {
            List<string> newerFiles = new List<string>();

            foreach (var directoryPath in directoryPaths)
            {
                newerFiles.AddRange(NewerFilesLatestMonths(directoryPath, months, searchPattern));
            }
            return newerFiles.ToArray();
        }

        public static string[] NewerDirectorys(string directoryPath, DateTime specifiedTime, string searchPattern = "*")
        {
            var allDirectorys = new DirectoryInfo(directoryPath).EnumerateDirectories(searchPattern, SearchOption.AllDirectories);
            var newerDirectorys = allDirectorys.Where(f => f.CreationTimeUtc > specifiedTime).Select(f => f.FullName).ToArray();

            return newerDirectorys;
        }

        public static string[] NewerDirectorysLatestDays(string directoryPath, int days, string searchPattern = "*.*")
        {
            DateTime specifiedTime = DateTime.Now.AddDays(-days);
            return NewerDirectorys(directoryPath, specifiedTime, searchPattern);
        }

        public static string[] NewerDirectorysLatestMonths(string directoryPath, int months, string searchPattern = "*.*")
        {
            DateTime specifiedTime = DateTime.Now.AddMonths(-months);
            return NewerDirectorys(directoryPath, specifiedTime, searchPattern);
        }

        public static string[] NewerDirectorys(string[] directoryPaths, DateTime specifiedTime, string searchPattern = "*")
        {
            List<string> newerDirectorys = new List<string>();

            foreach (var directoryPath in directoryPaths)
            {
                newerDirectorys.AddRange(NewerDirectorys(directoryPath, specifiedTime, searchPattern));
            }
            return newerDirectorys.ToArray();
        }

        public static string[] NewerFilesInNewerDirectorys(string directoryPath, DateTime specifiedTime, string searchPattern = "*.*")
        {
            var newerDirectorys = NewerDirectorys(directoryPath, specifiedTime, searchPattern);
            List<string> newerFiles = new List<string>();

            foreach (var newerDirectory in newerDirectorys)
            {
                newerFiles.AddRange(NewerFiles(newerDirectory, specifiedTime, searchPattern));
            }
            return newerFiles.ToArray();
        }

        public static string CreateDateDir(string directoryPath, DateTime specifiedTime, int daysOffset = 0)
        {
            try
            {
                DateTime n = specifiedTime.AddDays(-daysOffset);
                string targetPath = Path.Combine(directoryPath, n.ToString("yyyy"), n.ToString("yyyyMM"), n.ToString("yyyyMMdd"));
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                return targetPath;
            }
            catch { return ""; }
        }

        public static string CreateDateDir(string directoryPath, int daysOffset = 0)
        {
            return CreateDateDir(directoryPath, DateTime.Now, daysOffset);
        }

        public static string CreateMonthDir(string directoryPath, DateTime specifiedTime, int monthsOffset = 0)
        {
            try
            {
                DateTime n = specifiedTime.AddMonths((int)(-monthsOffset));
                string targetPath = Path.Combine(directoryPath, n.ToString("yyyy"), n.ToString("yyyyMM"));
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                return targetPath;
            }
            catch { return ""; }
        }

        public static string CreateMonthDir(string directoryPath, int monthsOffset = 0)
        {
            return CreateMonthDir(directoryPath, DateTime.Now, monthsOffset);
        }

        public static string CreateYearDir(string directoryPath, DateTime specifiedTime, int yearsOffset = 0)
        {
            try
            {
                DateTime n = specifiedTime.AddYears((int)-yearsOffset);
                string targetPath = Path.Combine(directoryPath, n.ToString("yyyy"));
                if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
                return targetPath;
            }
            catch { return ""; }
        }

        public static string CreateYearDir(string directoryPath, int yearsOffset = 0)
        {
            return CreateYearDir(directoryPath, DateTime.Now, yearsOffset);
        }


        public static string[] NewerFilesFromDateDirectory(string directoryPath, DateTime specifiedTime, int daysOffset = 0, string searchPattern = "*.*")
        {
            List<string> targetDirectorys = new List<string>();
            List<string> targetFiles = new List<string>();

            string lastDir = "";
            for (int i = 0; i <= daysOffset; i++)
            {
                DateTime n = specifiedTime.AddDays((int)-i);
                lastDir = CreateDateDir(directoryPath, n);
                targetDirectorys.Add(lastDir);
            }

            foreach (string targetDirectory in targetDirectorys)
            {
                if (targetDirectory != lastDir)
                {
                    targetFiles.AddRange(Directory.EnumerateFiles(targetDirectory));
                }
                else
                {
                    var listFiles = Directory.EnumerateFiles(targetDirectory);

                    foreach (var targetFile in listFiles)
                    {
                        if (GetCreateTimeFromFilePath(targetFile) > specifiedTime) { targetFiles.Add(targetFile); }
                    }
                }
            }
            return targetFiles.ToArray();
        }

        public static DateTime GetCreateTimeFromFilePath(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            Match match = Regex.Match(fileName, @"\d{8}_\d{6}");
            if (match.Success)
            {
                DateTime createTime = DateTime.ParseExact(match.Value, "yyyyMMdd_HHmmss", null);
                return createTime;
            }

            match = Regex.Match(fileName, @"\d{14}");
            if (match.Success)
            {
                DateTime createTime = DateTime.ParseExact(match.Value, "yyyyMMddHHmmss", null);
                return createTime;
            }

            return DateTime.MinValue;
        }
    }
}
