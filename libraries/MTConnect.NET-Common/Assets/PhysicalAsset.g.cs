// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727791480187_194742_23509

namespace MTConnect.Assets
{
    /// <summary>
    /// Abstract physical Asset.
    /// </summary>
    public partial class PhysicalAsset : IPhysicalAsset
    {
        public const string DescriptionText = "Abstract physical Asset.";


        /// <summary>
        /// Date of calibration of the Asset.
        /// </summary>
        public System.DateTime CalibrationDate { get; set; }
        
        /// <summary>
        /// Date of last inspection of the Asset.
        /// </summary>
        public System.DateTime InspectionDate { get; set; }
        
        /// <summary>
        /// Date of creation or built of the Asset.
        /// </summary>
        public System.DateTime ManufactureDate { get; set; }
        
        /// <summary>
        /// Constrained scalar value associated with an Asset.
        /// </summary>
        public MTConnect.Assets.Pallet.IMeasurement Measurement { get; set; }
        
        /// <summary>
        /// Date of next inspection of the Asset.
        /// </summary>
        public System.DateTime NextInspectionDate { get; set; }
    }
}