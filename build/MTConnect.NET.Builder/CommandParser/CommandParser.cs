// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace TrakHound.Builder
{
    internal static class CommandParser
    {
        private const string _regexPattern = "(\"[^\"]+\"|[^\\s\"]+)";
        private static readonly Regex _regex = new Regex(_regexPattern);


        public static async Task Run(string command)
        {
            var matches = _regex.Matches(command);
            if (matches != null)
            {
                var isHelp = false;

                // Process Args (use regex to capture "")
                var args = new List<string>();
                for (var i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var matchValue = match.Groups[1].Value;

                    if ((i == 0 && matchValue == "help") || (i == matches.Count - 1 && matchValue == "/?"))
                    {
                        // Set Help to true and skip the arg (don't send to Run() or Help())
                        isHelp = true;
                    }
                    else
                    {
                        args.Add(matchValue);
                    }
                }

                bool success;

                // Get all type from the current Assembly
                var types = Assembly.GetExecutingAssembly().GetTypes();
                foreach (var type in types)
                {
                    if (!isHelp)
                    {
                        success = await Run(type, args.ToArray());
                    }
                    else
                    {
                        success = Help(type, args.ToArray());
                    }

                    if (success) break;
                }
            }
        }

        public static async Task<bool> Run(Type type, string[] args)
        {
            var match = CommandMethodMatch.Match(type, args);
            if (match != null)
            {
                // Read Command Parameters & Options
                var commandParameters = GetCommandParameters(match.Method, match.ArgIndex, args);

                // Run Method
                if (match.Method.ReturnType == typeof(Task))
                {
                    await (Task)match.Method.Invoke(null, commandParameters.Values.ToArray());
                }
                else
                {
                    match.Method.Invoke(null, commandParameters.Values.ToArray());
                }
            }

            return false; // Need Error here
        }

        public static bool Help(Type type, string[] args)
        {
            if (type != null)
            {
                var methodMatch = CommandMethodMatch.Match(type, args);
                if (methodMatch != null)
                {
                    PrintHelp(methodMatch.Method);

                    return true;
                }
                else
                {
                    var groupMatch = CommandGroupMatch.Match(type, args);
                    if (groupMatch != null)
                    {
                        PrintHelp(groupMatch.Type);

                        return true;
                    }
                }
            }

            return false;
        }


        private static Dictionary<string, object> GetCommandParameters(MethodInfo method, int argumentIndex, string[] args)
        {
            var argIndex = argumentIndex;

            var commandParameters = new Dictionary<string, object>();

            var parameters = method.GetParameters();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var isOptional = parameter.DefaultValue != DBNull.Value;
                    var isSet = false;

                    commandParameters.Remove(parameter.Name);
                    commandParameters.Add(parameter.Name, parameter.DefaultValue);

                    // Process Parameters
                    var parameterAttribute = parameter.GetCustomAttribute<CommandParameterAttribute>();
                    if (parameterAttribute != null)
                    {
                        if (argIndex < args.Length)
                        {
                            var argValue = args[argIndex];
                            if (argValue != null && !argValue.StartsWith("--"))
                            {
                                var parameterValue = Convert.ChangeType(argValue, parameter.ParameterType);

                                commandParameters.Remove(parameter.Name);
                                commandParameters.Add(parameter.Name, parameterValue);

                                isSet = true;
                                argIndex++;
                            }
                        }
                    }

                    // Process Options ('--' prefix)
                    var optionAttribute = parameter.GetCustomAttribute<CommandOptionAttribute>();
                    if (optionAttribute != null)
                    {
                        if (argIndex < args.Length)
                        {
                            var parameterName = parameter.Name;

                            if (args[argIndex] == $"--{parameter.Name.ToCamelCase()}")
                            {
                                argIndex++;

                                if (parameter.ParameterType == typeof(bool))
                                {
                                    commandParameters.Remove(parameter.Name);
                                    commandParameters.Add(parameter.Name, true);
                                    isSet = true;
                                }
                                else
                                {
                                    if (argIndex < args.Length)
                                    {
                                        var parameterValue = Convert.ChangeType(args[argIndex], parameter.ParameterType);

                                        commandParameters.Remove(parameter.Name);
                                        commandParameters.Add(parameter.Name, parameterValue);

                                        isSet = true;
                                        argIndex++;
                                    }
                                }
                            }
                        }
                    }

                    if (!isSet && !isOptional) return null; // need error here
                }
            }

            return commandParameters;
        }


        private static void PrintHelp(Type type)
        {
            if (type != null)
            {
                var commandGroupAttribute = type.GetCustomAttribute<CommandGroupAttribute>();
                if (commandGroupAttribute != null)
                {
                    var name = commandGroupAttribute.Name;
                    var description = commandGroupAttribute.Description;

                    Console.WriteLine($"Command Group : {name}");
                    if (!string.IsNullOrEmpty(description)) Console.WriteLine(description);
                    Console.WriteLine();
                    Console.WriteLine("Commands");
                    Console.WriteLine("-------------------");
                    Console.WriteLine();


                    var methods = type.GetMethods();
                    if (methods != null)
                    {
                        foreach (var method in methods)
                        {
                            PrintHelp(method);
                            Console.WriteLine();
                        }
                    }
                }
            }
        }

        private static void PrintHelp(MethodInfo method)
        {
            if (method != null)
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute != null)
                {
                    var name = commandAttribute.Name;
                    var description = commandAttribute.Description;

                    Console.WriteLine($"Command : {name}");
                    if (!string.IsNullOrEmpty(description)) Console.WriteLine(description);
                    Console.WriteLine();

                    var shift = "    ";

                    var parameters = method.GetParameters();
                    if (parameters != null)
                    {
                        Console.WriteLine(shift + $"Parameters:");
                        Console.WriteLine();
                        Console.WriteLine(shift + string.Format("{0,-22} |{1,12} |{2,12} |   {3}", "Name", "Type", "Required", "Description"));
                        Console.WriteLine(shift + string.Format("{0,-22} {1,12} {2,12} {3}", "-----------------------", "-------------", "-------------", "---------------"));

                        foreach (var parameter in parameters)
                        {
                            string parameterType = null;
                            string parameterDescription = null;
                            var required = parameter.DefaultValue == DBNull.Value ? "Required" : "Optional";

                            // Parameter
                            var commandParameterAttribute = parameter.GetCustomAttribute<CommandParameterAttribute>();
                            if (commandParameterAttribute != null)
                            {
                                parameterType = "Parameter";
                                parameterDescription = commandParameterAttribute.Description;
                            }

                            // Option
                            var commandOptionAttribute = parameter.GetCustomAttribute<CommandOptionAttribute>();
                            if (commandOptionAttribute != null)
                            {
                                parameterType = "Option";
                                parameterDescription = commandOptionAttribute.Description;
                            }

                            if (parameterType != null)
                            {
                                Console.WriteLine(shift + string.Format("{0,-22} |{1,12} |{2,12} |   {3}", parameter.Name, parameterType, required, parameterDescription));
                            }
                        }
                    }
                }
            }
        }


        public static string ToCamelCase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                var parts = s.SplitOnWord();
                if (parts != null)
                {
                    var sb = new StringBuilder();
                    for (var i = 0; i <= parts.Count() - 1; i++)
                    {
                        if (i > 0) sb.Append(parts[i].UppercaseFirstCharacter());
                        else sb.Append(parts[i].ToLower());
                    }
                    return sb.ToString();
                }
            }

            return null;
        }

        public static string[] SplitOnWord(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] parts;

                if (s.Contains(' '))
                {
                    // Split string by empty space
                    parts = s.Split(' ');
                }
                else if (s.Contains('_'))
                {
                    // Split string by underscore
                    parts = s.Split('_');
                }
                else
                {
                    // Split string by Uppercase characters
                    parts = SplitOnUppercase(s);
                }

                return parts;
            }

            return null;
        }

        public static string[] SplitOnUppercase(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != s.ToUpper())
                {
                    var p = "";
                    var x = 0;
                    for (var i = 0; i < s.Length; i++)
                    {
                        if (i > 0 && char.IsUpper(s[i]))
                        {
                            p += s.Substring(x, i - x) + " ";
                            x = i;
                        }

                        if (i == s.Length - 1)
                        {
                            p += s.Substring(x);
                        }
                    }
                    return p.Split(' ');
                }
                else return new string[] { s };
            }

            return null;
        }

        public static string UppercaseFirstCharacter(this string s)
        {
            if (s == null) return null;

            if (s.Length > 1)
            {
                var sb = new StringBuilder(s.Length);
                for (var i = 0; i <= s.Length - 1; i++)
                {
                    if (i == 0) sb.Append(char.ToUpper(s[i]));
                    else sb.Append(char.ToLower(s[i]));
                }
                return sb.ToString();
            }

            return s.ToUpper();
        }
    }
}
