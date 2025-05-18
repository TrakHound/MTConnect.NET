// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets
{
    /// <summary>
    /// Abstract physical Asset.
    /// </summary>
    public partial interface IPhysicalAsset
    {
        /// <summary>
        /// Date of calibration of the Asset.
        /// </summary>
        System.DateTime CalibrationDate { get; }
        
        /// <summary>
        /// Date of last inspection of the Asset.
        /// </summary>
        System.DateTime InspectionDate { get; }
        
        /// <summary>
        /// Date of creation or built of the Asset.
        /// </summary>
        System.DateTime ManufactureDate { get; }
        
        /// <summary>
        /// Constrained scalar value associated with an Asset.
        /// </summary>
        MTConnect.Assets.Pallet.IMeasurement Measurement { get; }
        
        /// <summary>
        /// Date of next inspection of the Asset.
        /// </summary>
        System.DateTime NextInspectionDate { get; }
    }
}