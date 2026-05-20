// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Expresses the relative precedence assigned to a <see cref="Collaborator"/> when several are eligible to service the same Interface request.
    /// </summary>
    public class Priority
    {
        /// <summary>
        /// The numeric precedence used to rank this collaborator against others competing to service the same request.
        /// </summary>
        public int Value { get; set; }
    }
}
