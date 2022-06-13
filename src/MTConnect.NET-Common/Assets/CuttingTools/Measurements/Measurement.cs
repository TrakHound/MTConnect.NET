// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Assets.CuttingTools.Measurements
{
    /// <summary>
    /// A Measurement MUST be a scalar floating-point value that MAY be constrained to a maximum and minimum value.
    /// </summary>
    public class Measurement
    {
        public const string DescriptionText = "A Measurement MUST be a scalar floating-point value that MAY be constrained to a maximum and minimum value.";


        [XmlIgnore]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [XmlText]
        [JsonPropertyName("value")]
        public double Value { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value. 
        /// This is used by applications to determine accuracy of values. This MAY be specified for all numeric values.
        /// </summary>
        [XmlAttribute("significantDigits")]
        [JsonPropertyName("significantDigits")]
        public int SignificantDigits { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool SignificantDigitsSpecified => SignificantDigits > 0;

        /// <summary>
        /// The units for the measurements.
        /// </summary>
        [XmlAttribute("units")]
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The units the measurement was originally recorded in.
        /// </summary>
        [XmlAttribute("nativeUnits")]
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// A shop specific code for this measurement. ISO 13399 codes MAY be used to for these codes as well.
        /// </summary>
        [XmlAttribute("code")]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// The maximum value for this measurement. Exceeding this value would indicate the tool is not usable.
        /// </summary>
        [XmlAttribute("maximum")]
        [JsonPropertyName("maximum")]
        public double Maximum { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MaximumSpecified => Maximum > 0;

        /// <summary>
        /// The minimum value for this measurement. Exceeding this value would indicate the tool is not usable.
        /// </summary>
        [XmlAttribute("minimum")]
        [JsonPropertyName("minimum")]
        public double Minimum { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool MinimumSpecified => Minimum > 0;

        /// <summary>
        /// The as advertised value for this measurement.
        /// </summary>
        [XmlAttribute("nominal")]
        [JsonPropertyName("nominal")]
        public double Nominal { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public bool NominalSpecified => Nominal > 0;

        [XmlIgnore]
        [JsonIgnore]
        public string TypeDescription { get; }


        public Measurement() { }

        public Measurement(Measurement measurement)
        {
            if (measurement != null)
            {
                Value = measurement.Value;
                Nominal = measurement.Nominal;
                Minimum = measurement.Minimum;
                Maximum = measurement.Maximum;
                SignificantDigits = measurement.SignificantDigits;
                NativeUnits = measurement.NativeUnits;
            }
        }

        public static Measurement Create(string type, Measurement measurement)
        {
            if (!string.IsNullOrEmpty(type))
            {
                switch (type)
                {
                    // Common
                    case FunctionalLengthMeasurement.TypeId: return new FunctionalLengthMeasurement(measurement);
                    case WeightMeasurement.TypeId: return new WeightMeasurement(measurement);

                    // Assembly
                    case BodyDiameterMaxMeasurement.TypeId: return new BodyDiameterMaxMeasurement(measurement);
                    case BodyLengthMaxMeasurement.TypeId: return new BodyLengthMaxMeasurement(measurement);
                    case DepthOfCutMaxMeasurement.TypeId: return new DepthOfCutMaxMeasurement(measurement);
                    case CuttingDiameterMaxMeasurement.TypeId: return new CuttingDiameterMaxMeasurement(measurement);
                    case FlangeDiameterMaxMeasurement.TypeId: return new FlangeDiameterMaxMeasurement(measurement);
                    case OverallToolLengthMeasurement.TypeId: return new OverallToolLengthMeasurement(measurement);
                    case ShankDiameterMeasurement.TypeId: return new ShankDiameterMeasurement(measurement);
                    case ShankHeightMeasurement.TypeId: return new ShankHeightMeasurement(measurement);
                    case ShankLengthMeasurement.TypeId: return new ShankLengthMeasurement(measurement);
                    case UsableLengthMaxMeasurement.TypeId: return new UsableLengthMaxMeasurement(measurement);
                    case ProtrudingLengthMeasurement.TypeId: return new ProtrudingLengthMeasurement(measurement);

                    // Cutting Item
                    case ChamferFlatLengthMeasurement.TypeId: return new ChamferFlatLengthMeasurement(measurement);
                    case ChamferWidthMeasurement.TypeId: return new ChamferWidthMeasurement(measurement);
                    case CornerRadiusMeasurement.TypeId: return new CornerRadiusMeasurement(measurement);
                    case CuttingDiameterMeasurement.TypeId: return new CuttingDiameterMeasurement(measurement);
                    case CuttingEdgeLengthMeasurement.TypeId: return new CuttingEdgeLengthMeasurement(measurement);
                    case CuttingHeightMeasurement.TypeId: return new CuttingHeightMeasurement(measurement);
                    case CuttingReferencePointMeasurement.TypeId: return new CuttingReferencePointMeasurement(measurement);
                    case DriveAngleMeasurement.TypeId: return new DriveAngleMeasurement(measurement);
                    case FlangeDiameterMeasurement.TypeId: return new FlangeDiameterMeasurement(measurement);               
                    case FunctionalWidthMeasurement.TypeId: return new FunctionalWidthMeasurement(measurement);
                    case InscribedCircleDiameterMeasurement.TypeId: return new InscribedCircleDiameterMeasurement(measurement);
                    case InsertWidthMeasurement.TypeId: return new InsertWidthMeasurement(measurement);
                    case PointAngleMeasurement.TypeId: return new PointAngleMeasurement(measurement);
                    case StepDiameterLengthMeasurement.TypeId: return new StepDiameterLengthMeasurement(measurement);
                    case StepIncludedAngleMeasurement.TypeId: return new StepIncludedAngleMeasurement(measurement);
                    case ToolCuttingEdgeAngleMeasurement.TypeId: return new ToolCuttingEdgeAngleMeasurement(measurement);
                    case ToolLeadAngleMeasurement.TypeId: return new ToolLeadAngleMeasurement(measurement);
                    case ToolOrientationMeasurement.TypeId: return new ToolOrientationMeasurement(measurement);
                    case WiperEdgeLengthMeasurement.TypeId: return new WiperEdgeLengthMeasurement(measurement);
                }
            }

            return null;
        }
    }
}
