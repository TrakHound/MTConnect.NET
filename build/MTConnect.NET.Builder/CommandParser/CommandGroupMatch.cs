// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Reflection;

namespace TrakHound.Builder
{
    class CommandGroupMatch
    {
        public Type Type { get; set; }
        public int ArgIndex { get; set; }


        public CommandGroupMatch(Type type, int argIndex)
        {
            Type = type;
            ArgIndex = argIndex;
        }


        public static CommandGroupMatch Match(Type type, string[] args)
        {
            if (type != null && args != null && args.Length > 0)
            {
                var argIndex = 0;

                // Set Command Group Name (if specified)
                var commandGroupAttribute = type.GetCustomAttribute<CommandGroupAttribute>();
                if (commandGroupAttribute != null)
                {
                    var isMatch = false;

                    var matchNames = commandGroupAttribute.Name.Split(' ');
                    foreach (var matchName in matchNames)
                    {
                        if (argIndex > args.Length - 1) return null; // Need Error here

                        isMatch = args[argIndex] == matchName;
                        if (isMatch) argIndex++;
                        else break;
                    }

                    if (isMatch)
                    {
                        return new CommandGroupMatch(type, argIndex);
                    }
                }
            }

            return null;
        }
    }
}
