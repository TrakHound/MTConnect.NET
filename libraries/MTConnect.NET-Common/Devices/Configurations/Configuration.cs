// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml.Linq;

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Hand-authored partial extension of the generated <see cref="Configuration"/>
    /// concrete class that carries the vendor-extension collection declared on
    /// <see cref="IConfiguration.VendorExtensions"/>. See the interface's remarks
    /// for the MTConnect v2.7 XSD citation that underpins the semantics.
    /// </summary>
    public partial class Configuration
    {
        /// <inheritdoc cref="IConfiguration.VendorExtensions"/>
        public IEnumerable<XElement> VendorExtensions { get; set; }
    }
}
