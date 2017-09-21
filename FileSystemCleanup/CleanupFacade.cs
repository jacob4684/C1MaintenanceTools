using System;
using System.Collections.Generic;
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

                if (extensions.Any())
                {
                    filter = fi => fi.LastAccessTime < obsolescenceDate && extensions.Contains(fi.Extension);
                    DeleteFiles(physicalPath, filter);
                    continue;
                }

                var ignoreExtensions = f.IgnoreExtensions
                    .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => "." + e)
                    .ToList();

                if (ignoreExtensions.Any())
                {
                    filter = fi => fi.LastAccessTime < obsolescenceDate && !ignoreExtensions.Contains(fi.Extension);
                    DeleteFiles(physicalPath, filter);
                }
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
            var directories = new string[] { };

            try
            {
                directories = Directory.GetDirectories(dir);
            }
            catch (Exception)
            {
                Log.LogWarning("Automatic cleanup unable to access folder", "Unable to access folder " + dir);
            }

            foreach (var directory in directories)
            {
                DeleteFiles(directory, filter);

                List<FileInfo> fileInfos;

                try
                {
                    fileInfos = Directory
                        .GetFiles(directory)
                        .Select(f => new FileInfo(f))
                        .ToList();
                }
                catch (Exception)
                {
                    Log.LogWarning("Automatic cleanup unable to access folder", "Unable to access folder " + directory);
                    continue;
                }

                if (!fileInfos.Any())
                {
                    continue;
                }

                var filesToDelete = fileInfos
                    .Where(filter)
                    .ToList();

                filesToDelete.AsParallel().ForAll(f =>
                {
                    try
                    {
                        f.Delete();
                    }
                    catch (Exception)
                    {
                        Log.LogWarning("Automatic cleanup unable to delete file", "Unable to delete file " + f.FullName);
                    }
                });
            }
        }

        public static void DeleteEmptyFolders(string dir)
        {
            var directories = new string[] { };

            try
            {
                directories = Directory.GetDirectories(dir);
            }
            catch (Exception)
            {
                Log.LogWarning("Automatic cleanup unable to access folder", "Unable to access folder " + dir);
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
                catch (Exception)
                {
                    Log.LogWarning("Automatic cleanup unable to access folder", "Unable to access folder " + directory);
                }

                try
                {
                    Directory.Delete(directory, false);
                }
                catch (Exception)
                {
                    Log.LogWarning("Automatic cleanup unable to delete folder", "Unable to delete folder " + directory);
                }
            }
        }
    }
}