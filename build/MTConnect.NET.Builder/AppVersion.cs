using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;

namespace TrakHound.Builder
{
    internal class AppVersion
    {
        public static string GetVersion(string configurationId)
        {
            var config = Configuration.Read(configurationId);

            var path = config.VersionInfoPath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, path);
            }

            if (File.Exists(path))
            {
                var fileContents = File.ReadAllText(path);

                var regex = new Regex("<VersionPrefix>(.*)<");
                var match = regex.Match(fileContents);
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        public static string GetVersionSuffix(string configurationId)
        {
            var config = Configuration.Read(configurationId);

            var path = config.VersionInfoPath;
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, path);
            }

            if (File.Exists(path))
            {
                var fileContents = File.ReadAllText(path);

                var regex = new Regex("<VersionSuffix>(.*)<");
                var match = regex.Match(fileContents);
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        public static string GenerateFullVersion(string versionPrefix, string versionSuffix)
        {
            if (!string.IsNullOrEmpty(versionPrefix) && !string.IsNullOrEmpty(versionSuffix))
            {
                var build = DateTime.Now.Ticks / 10000000;

                return $"{versionPrefix}-{versionSuffix}+{build}";
            }
            else
            {
                return versionPrefix;
            }
        }

        //public static string Get(string configurationId)
        //{
        //    var config = Configuration.Read(configurationId);

        //    var path = config.AssemblyInfoPath;
        //    if (!Path.IsPathRooted(path))
        //    {
        //        path = Path.Combine(Environment.CurrentDirectory, path);
        //    }

        //    if (File.Exists(path))
        //    {
        //        var fileContents = File.ReadAllText(path);

        //        var regex = new Regex("AssemblyVersion\\(\\\"(.*)\\\"\\)");
        //        var match = regex.Match(fileContents);
        //        {
        //            var version = match.Groups[1].Value;
        //            //if (!string.IsNullOrEmpty(configuration.VersionSuffix)) version = $"{version}-{configuration.VersionSuffix}";
        //            return version;
        //        }
        //    }

        //    return null;
        //}



        public static int GetRevision(string configurationId, string productType, string productKey, string version)
        {
            if (!string.IsNullOrEmpty(configurationId) && !string.IsNullOrEmpty(productType) && !string.IsNullOrEmpty(productKey) && !string.IsNullOrEmpty(version))
            {
                var dataSourcePath = Path.Combine(AppContext.BaseDirectory, "build_versions");
                var dataSource = $"Data Source={dataSourcePath};Mode=ReadWriteCreate";

                try
                {
                    using (var connection = new SqliteConnection(dataSource))
                    {
                        // Open the connection
                        connection.Open();

                        string existingVersion = null;
                        int nextRevision = 0;

                        // Read Existing Version
                        var query = $"select [version] from [products] where [configuration_id] = '{configurationId}' and [product_type] = '{productType}' and [product_key] = '{productKey}';";
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = query;
                            existingVersion = ReadDatabaseString(command);
                        }

                        if (existingVersion.ToVersion() == version.ToVersion())
                        {
                            // Read Previous Revision
                            query = $"select [revision] from [products] where [configuration_id] = '{configurationId}' and [product_type] = '{productType}' and [product_key] = '{productKey}';";
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = query;
                                return ReadDatabaseString(command).ToInt();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetRevision() : " + ex.Message);
                }
            }

            return 0;
        }

        public static bool SetRevision(string configurationId, string productType, string productKey, string version, int revision)
        {
            if (!string.IsNullOrEmpty(configurationId) && !string.IsNullOrEmpty(productType) && !string.IsNullOrEmpty(productKey) && !string.IsNullOrEmpty(version))
            {
                var dataSourcePath = Path.Combine(AppContext.BaseDirectory, "build_versions");
                var dataSource = $"Data Source={dataSourcePath};Mode=ReadWriteCreate";

                try
                {
                    using (var connection = new SqliteConnection(dataSource))
                    {
                        // Open the connection
                        connection.Open();

                        var queries = new string[3];
                        queries[0] = "create table if not exists [products] ([configuration_id] text, [product_type] text, [product_key] text, [version] text, [revision] integer, PRIMARY KEY([configuration_id], [product_key]));";
                        queries[1] = $"delete from [products] where [configuration_id] = '{configurationId}' and [product_type] = '{productType}' and [product_key] = '{productKey}';";
                        queries[2] = $"insert into [products] ([configuration_id], [product_type], [product_key], [version], [revision]) values ('{configurationId}', '{productType}', '{productKey}', '{version}', {revision});";

                        var query = string.Join(';', queries);
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SetRevision() : " + ex.Message);
                }
            }

            return false;
        }

        private static string ReadDatabaseString(SqliteCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                bool first = true;

                while (first || reader.NextResult())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var obj = reader.GetValue(0);
                            if (obj != null)
                            {
                                return obj.ToString();
                            }
                        }
                    }

                    first = false;
                }
            }

            return null;
        }
    }
}
