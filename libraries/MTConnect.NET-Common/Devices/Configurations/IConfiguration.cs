// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Xml.Linq;

namespace MTConnect.Devices.Configurations
{
    /// <summary>
    /// Hand-authored partial extension of the generated <see cref="IConfiguration"/>
    /// contract that adds the vendor-extension surface. The MTConnect Standard
    /// itself defines vendor extension of a component's Configuration through the
    /// XSD substitution group <c>AbstractConfiguration</c> — see
    /// <c>MTConnectDevices_2.7.xsd</c> where <c>ComponentConfigurationType</c>
    /// declares <c>&lt;xs:element ref="AbstractConfiguration" minOccurs="0"
    /// maxOccurs="unbounded"/&gt;</c>, and every standard child (SensorConfiguration,
    /// Specifications, Relationships, CoordinateSystems, Motion, SolidModel,
    /// ImageFiles, PowerSources) is declared with
    /// <c>substitutionGroup='AbstractConfiguration'</c>. A vendor publishes their
    /// own XSD declaring a vendor-namespaced element that likewise substitutes for
    /// <c>AbstractConfiguration</c>, and the operator supplies fully-formed
    /// instances of that element through this collection.
    /// </summary>
    public partial interface IConfiguration
    {
        /// <summary>
        /// Vendor-extension elements carried inside the component's
        /// <c>&lt;Configuration&gt;</c>. Each entry MUST be a fully-formed,
        /// vendor-namespaced XML element that a vendor XSD declares as a
        /// substitution of the standard <c>AbstractConfiguration</c> abstract
        /// element (see class-level remarks for the MTConnect XSD citation).
        /// The MTConnect.NET XML formatter writes each element verbatim inside
        /// the <c>&lt;Configuration&gt;</c> sequence, alongside any standard
        /// children present on this instance; the deserialiser captures any
        /// child element it does not recognise as a standard configuration
        /// child and adds it here. Strict XSD validation of the emitted
        /// document is the caller's responsibility and requires the vendor XSD
        /// to be loaded alongside the MTConnect schemas.
        /// </summary>
        IEnumerable<XElement> VendorExtensions { get; }
    }
}
