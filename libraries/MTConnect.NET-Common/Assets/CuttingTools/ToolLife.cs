// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_37DECE45_091E_4f0c_AD72_EB481C0C1919

using System.Collections.Generic;

namespace MTConnect.Assets.CuttingTools
{
    /// <summary>
    /// Cutting tool life as related to the assembly.
    /// </summary>
    public partial class ToolLife
    {
        public static string GenerateHash(IToolLife toolLife)
        {
            if (toolLife != null)
            {
                var ids = new List<string>();
                ids.Add(ObjectExtensions.GetHashPropertyString(toolLife).ToSHA1Hash());
                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }
    }
}