// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.IO.Compression;
using System.Text.RegularExpressions;

namespace TrakHound.Builder
{
    public static class Files
    {
        public static void Copy(string sourceDirectory, string destinationDirectory, IEnumerable<string> ignorePatterns = null)
        {
            if (!string.IsNullOrEmpty(sourceDirectory) && !string.IsNullOrEmpty(destinationDirectory))
            {
                try
                {
                    if (Directory.Exists(sourceDirectory))
                    {
                        // Create Directories
                        var directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
                        foreach (var directory in directories)
                        {
                            var valid = IsValid(directory, ignorePatterns);
                            if (valid)
                            {
                                var relativeDirectoryPath = Path.GetRelativePath(sourceDirectory, directory);
                                var destinationPath = Path.Combine(destinationDirectory, relativeDirectoryPath);

                                if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
                            }
                        }

                        // Copy Files
                        var files = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            var valid = IsValid(file, ignorePatterns);
                            if (valid)
                            {
                                var relativePath = Path.GetRelativePath(sourceDirectory, file);
                                var destinationPath = Path.Combine(destinationDirectory, relativePath);

                                File.Copy(file, destinationPath, true);
                            }
                        }
                    }
                    else
                    {
                        // DEBUG
                        Console.WriteLine($"Files.Copy() : {sourceDirectory} : Directory Does Not Exist");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Files.Copy() : {sourceDirectory} : {destinationDirectory} : {ex.Message}");
                }
                //catch { }
            }
        }

        public static void Move(string sourceDirectory, string destinationDirectory, IEnumerable<string> ignorePatterns = null)
        {
            if (!string.IsNullOrEmpty(sourceDirectory) && !string.IsNullOrEmpty(destinationDirectory))
            {
                try
                {
                    // Create Directories
                    var directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
                    foreach (var directory in directories)
                    {
                        var valid = IsValid(directory, ignorePatterns);
                        if (valid)
                        {
                            var relativeDirectoryPath = Path.GetRelativePath(sourceDirectory, directory);
                            var destinationPath = Path.Combine(destinationDirectory, relativeDirectoryPath);

                            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
                        }
                    }

                    // Move Files
                    var files = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var valid = IsValid(file, ignorePatterns);
                        if (valid)
                        {
                            var relativePath = Path.GetRelativePath(sourceDirectory, file);
                            var destinationPath = Path.Combine(destinationDirectory, relativePath);

                            File.Move(file, destinationPath, true);
                        }
                    }
                }
                catch { }
            }
        }

        public static byte[] Zip(string sourcePath)
        {
            if (!string.IsNullOrEmpty(sourcePath))
            {
                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                        {                      
                            // Add Source files from Directory
                            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                // Get path Relative to Archive root
                                var entry = Path.GetRelativePath(sourcePath, file);

                                // Add Entry to Archive
                                archive.CreateEntryFromFile(file, entry);
                            }
                        }

                        return outputStream.ToArray();
                    }
                }
                catch { }
            }

            return null;
        }

        public static void Zip(string sourcePath, string destinationPath)
        {
            var bytes = Zip(sourcePath);
            if (bytes != null)
            {
                try
                {
                    File.WriteAllBytes(destinationPath, bytes);
                }
                catch { }
            }
        }

		public static void Unzip(byte[] archivedBytes, string destinationPath)
		{
			if (archivedBytes != null && !string.IsNullOrEmpty(destinationPath))
			{
				try
				{
					if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

					using (var inputStream = new MemoryStream(archivedBytes))
					{
						using (var archive = new ZipArchive(inputStream, ZipArchiveMode.Read))
						{
							archive.ExtractToDirectory(destinationPath, true);
						}
					}
				}
				catch { }
			}
		}


		public static void Clear(string directory)
        {
            if (!string.IsNullOrEmpty(directory))
            {
                try
                {
                    if (Directory.Exists(directory))
                    {
                        var files = Directory.GetFiles(directory);
                        if (!files.IsNullOrEmpty())
                        {
                            foreach (var file in files)
                            {
                                try
                                {
                                    File.Delete(file);
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
        }



        private static bool IsValid(string path, IEnumerable<string> ignorePatterns = null)
        {
            var valid = true;
            if (!ignorePatterns.IsNullOrEmpty())
            {
                foreach (var pattern in ignorePatterns)
                {
                    try
                    {
                        var regex = new Regex(pattern);
                        if (regex.Match(path).Success) return false;
                    }
                    catch { }
                }
            }

            return valid;
        }
    }
}
