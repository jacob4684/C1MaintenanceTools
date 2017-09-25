using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using C1MaintenanceTools.FileSystemCleanup.Configuration;
using Composite.Core;
using C1UrlUtils = Composite.Core.WebClient.UrlUtils;

namespace C1MaintenanceTools.FileSystemCleanup
{
    public class CleanupFacade
    {
        private const string LogTitle = "File System Cleanup";

        public static void RunCleanup()
        {
            var sections = CleanupSection.GetSection();

            var files = sections.Files.Cast<CleanupFilesElement>();
            foreach (var f in files)
            {
                var physicalPath = HostingEnvironment.MapPath(C1UrlUtils.PublicRootPath + f.Path);

                if (!Directory.Exists(physicalPath))
                {
                    continue;
                }

                Func<FileInfo, bool> filter;

                var obsolescenceDate = DateTime.Now.Subtract(TimeSpan.FromDays(f.DbsolescenceDays));

                var extensions = f.Extensions
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => "." + e)
                    .ToList();

                var ignoreExtensions = f.IgnoreExtensions
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => "." + e)
                    .ToList();

                var desc = extensions.Any() ? $" Only extensions: {f.Extensions}." : ignoreExtensions.Any() ? $" Exclude extensions: {f.IgnoreExtensions}." : null;
                Log.LogInformation(LogTitle, $"Cleaning up files from {physicalPath} older than {obsolescenceDate.ToShortDateString()}.{desc}");

                if (extensions.Any())
                {
                    filter = fi => fi.LastAccessTime < obsolescenceDate && extensions.Contains(fi.Extension);
                }

                else if (ignoreExtensions.Any())
                {
                    filter = fi => fi.LastAccessTime < obsolescenceDate && !ignoreExtensions.Contains(fi.Extension);
                }
                else
                {
                    filter = fi => fi.LastAccessTime < obsolescenceDate;
                }

                DeleteFiles(physicalPath, filter);
            }

            var emptyfolders = sections.EmptyFolders.Cast<CleanupEmptyFoldersElement>();

            foreach (var f in emptyfolders)
            {
                var physicalPath = HostingEnvironment.MapPath(C1UrlUtils.PublicRootPath + f.Path);

                if (!Directory.Exists(physicalPath))
                {
                    continue;
                }

                DeleteEmptyFolders(physicalPath);
            }
        }

        public static void DeleteFiles(string dir, Func<FileInfo, bool> filter)
        {
            try
            {
                var fileInfos = Directory
                    .GetFiles(dir)
                    .Select(f => new FileInfo(f))
                    .ToList();

                if (fileInfos.Any())
                {
                    var filesToDelete = fileInfos
                        .Where(filter)
                        .ToList();

                    Log.LogInformation(LogTitle, $"Cleaning up {filesToDelete.Count} files");

                    filesToDelete.AsParallel().ForAll(f =>
                    {
                        try
                        {
                            f.Delete();
                        }
                        catch (Exception ex)
                        {
                            Log.LogWarning(LogTitle, $"Unable to delete file {f.FullName}. Exception: {ex}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.LogWarning(LogTitle, $"Unable to get files from folder {dir}. Exception: {ex}");
            }

            try
            {
                var directories = Directory.GetDirectories(dir);

                foreach (var directory in directories)
                {
                    DeleteFiles(directory, filter);
                }
            }
            catch (Exception ex)
            {
                Log.LogWarning(LogTitle, $"Unable to access subfolders below {dir}. Exception: {ex}");
            }
        }

        public static void DeleteEmptyFolders(string dir)
        {
            var directories = new string[] { };

            try
            {
                directories = Directory.GetDirectories(dir);
            }
            catch (Exception ex)
            {
                Log.LogWarning(LogTitle, $"Unable to access folder {dir}. Exception {ex}");
            }

            foreach (var directory in directories)
            {
                DeleteEmptyFolders(directory);

                try
                {
                    if (Directory.GetFiles(directory).Length != 0 || Directory.GetDirectories(directory).Length != 0)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Log.LogWarning(LogTitle, $"Unable to access folder {directory}. Exception {ex}");
                }

                try
                {
                    Directory.Delete(directory, false);
                }
                catch (Exception ex)
                {
                    Log.LogWarning(LogTitle, $"Unable to delete folder {directory}. Exception {ex}");
                }
            }
        }
    }
}