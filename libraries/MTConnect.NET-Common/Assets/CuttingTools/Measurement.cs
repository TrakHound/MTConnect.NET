// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = EAID_C09F377D_8946_421b_B746_E23C01D97EAC

using MTConnect.Assets.CuttingTools.Measurements;

namespace MTConnect.Assets.CuttingTools
{
    public partial class Measurement
    {
        public string Type { get; set; }


        public Measurement() { }

        public Measurement(IMeasurement measurement)
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

        public static Measurement Create(string type, IMeasurement measurement)
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
                    case IncribedCircleDiameterMeasurement.TypeId: return new IncribedCircleDiameterMeasurement(measurement);
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

            return new Measurement(measurement);
        }
    }
}