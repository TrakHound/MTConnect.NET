// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Output;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MTConnect.Streams.Json
{
    public class JsonSamples
    {
        [JsonIgnore]
        public List<IObservation> Observations
        {
            get
            {
                var l = new List<IObservation>();
                if (!Acceleration.IsNullOrEmpty()) foreach (var x in Acceleration) l.Add(x.ToObservation(AccelerationDataItem.TypeId));
                if (!AccelerationDataSet.IsNullOrEmpty()) foreach (var x in AccelerationDataSet) l.Add(x.ToObservation(AccelerationDataItem.TypeId));

                if (!AccumulatedTime.IsNullOrEmpty()) foreach (var x in AccumulatedTime) l.Add(x.ToObservation(AccumulatedTimeDataItem.TypeId));
                if (!AccumulatedTimeDataSet.IsNullOrEmpty()) foreach (var x in AccumulatedTimeDataSet) l.Add(x.ToObservation(AccumulatedTimeDataItem.TypeId));

                if (!Amperage.IsNullOrEmpty()) foreach (var x in Amperage) l.Add(x.ToObservation(AmperageDataItem.TypeId));
                if (!AmperageDataSet.IsNullOrEmpty()) foreach (var x in AmperageDataSet) l.Add(x.ToObservation(AmperageDataItem.TypeId));

                if (!AmperageAC.IsNullOrEmpty()) foreach (var x in AmperageAC) l.Add(x.ToObservation(AmperageACDataItem.TypeId));
                if (!AmperageACDataSet.IsNullOrEmpty()) foreach (var x in AmperageACDataSet) l.Add(x.ToObservation(AmperageACDataItem.TypeId));

                if (!AmperageDC.IsNullOrEmpty()) foreach (var x in AmperageDC) l.Add(x.ToObservation(AmperageDCDataItem.TypeId));
                if (!AmperageDCDataSet.IsNullOrEmpty()) foreach (var x in AmperageDCDataSet) l.Add(x.ToObservation(AmperageDCDataItem.TypeId));

                if (!Angle.IsNullOrEmpty()) foreach (var x in Angle) l.Add(x.ToObservation(AngleDataItem.TypeId));
                if (!AngleDataSet.IsNullOrEmpty()) foreach (var x in AngleDataSet) l.Add(x.ToObservation(AngleDataItem.TypeId));

                if (!AngularAcceleration.IsNullOrEmpty()) foreach (var x in AngularAcceleration) l.Add(x.ToObservation(AngularAccelerationDataItem.TypeId));
                if (!AngularAccelerationDataSet.IsNullOrEmpty()) foreach (var x in AngularAccelerationDataSet) l.Add(x.ToObservation(AngularAccelerationDataItem.TypeId));

                if (!AngularDeceleration.IsNullOrEmpty()) foreach (var x in AngularDeceleration) l.Add(x.ToObservation(AngularDecelerationDataItem.TypeId));
                if (!AngularDecelerationDataSet.IsNullOrEmpty()) foreach (var x in AngularDecelerationDataSet) l.Add(x.ToObservation(AngularDecelerationDataItem.TypeId));

                if (!AngularVelocity.IsNullOrEmpty()) foreach (var x in AngularVelocity) l.Add(x.ToObservation(AngularVelocityDataItem.TypeId));
                if (!AngularVelocityDataSet.IsNullOrEmpty()) foreach (var x in AngularVelocityDataSet) l.Add(x.ToObservation(AngularVelocityDataItem.TypeId));

                if (!AssetUpdateRate.IsNullOrEmpty()) foreach (var x in AssetUpdateRate) l.Add(x.ToObservation(AssetUpdateRateDataItem.TypeId));
                if (!AssetUpdateRateDataSet.IsNullOrEmpty()) foreach (var x in AssetUpdateRateDataSet) l.Add(x.ToObservation(AssetUpdateRateDataItem.TypeId));

                if (!AxisFeedrate.IsNullOrEmpty()) foreach (var x in AxisFeedrate) l.Add(x.ToObservation(AxisFeedrateDataItem.TypeId));
                if (!AxisFeedrateDataSet.IsNullOrEmpty()) foreach (var x in AxisFeedrateDataSet) l.Add(x.ToObservation(AxisFeedrateDataItem.TypeId));

                if (!BatteryCapacity.IsNullOrEmpty()) foreach (var x in BatteryCapacity) l.Add(x.ToObservation(BatteryCapacityDataItem.TypeId));
                if (!BatteryCapacityDataSet.IsNullOrEmpty()) foreach (var x in BatteryCapacityDataSet) l.Add(x.ToObservation(BatteryCapacityDataItem.TypeId));

                if (!BatteryCharge.IsNullOrEmpty()) foreach (var x in BatteryCharge) l.Add(x.ToObservation(BatteryChargeDataItem.TypeId));
                if (!BatteryChargeDataSet.IsNullOrEmpty()) foreach (var x in BatteryChargeDataSet) l.Add(x.ToObservation(BatteryChargeDataItem.TypeId));

                if (!CapacityFluid.IsNullOrEmpty()) foreach (var x in CapacityFluid) l.Add(x.ToObservation(CapacityFluidDataItem.TypeId));
                if (!CapacityFluidDataSet.IsNullOrEmpty()) foreach (var x in CapacityFluidDataSet) l.Add(x.ToObservation(CapacityFluidDataItem.TypeId));

                if (!CapacitySpatial.IsNullOrEmpty()) foreach (var x in CapacitySpatial) l.Add(x.ToObservation(CapacitySpatialDataItem.TypeId));
                if (!CapacitySpatialDataSet.IsNullOrEmpty()) foreach (var x in CapacitySpatialDataSet) l.Add(x.ToObservation(CapacitySpatialDataItem.TypeId));

                if (!ChargeRate.IsNullOrEmpty()) foreach (var x in ChargeRate) l.Add(x.ToObservation(ChargeRateDataItem.TypeId));
                if (!ChargeRateDataSet.IsNullOrEmpty()) foreach (var x in ChargeRateDataSet) l.Add(x.ToObservation(ChargeRateDataItem.TypeId));

                if (!Concentration.IsNullOrEmpty()) foreach (var x in Concentration) l.Add(x.ToObservation(ConcentrationDataItem.TypeId));
                if (!ConcentrationDataSet.IsNullOrEmpty()) foreach (var x in ConcentrationDataSet) l.Add(x.ToObservation(ConcentrationDataItem.TypeId));

                if (!Conductivity.IsNullOrEmpty()) foreach (var x in Conductivity) l.Add(x.ToObservation(ConductivityDataItem.TypeId));
                if (!ConductivityDataSet.IsNullOrEmpty()) foreach (var x in ConductivityDataSet) l.Add(x.ToObservation(ConductivityDataItem.TypeId));

                if (!CuttingSpeed.IsNullOrEmpty()) foreach (var x in CuttingSpeed) l.Add(x.ToObservation(CuttingSpeedDataItem.TypeId));
                if (!CuttingSpeedDataSet.IsNullOrEmpty()) foreach (var x in CuttingSpeedDataSet) l.Add(x.ToObservation(CuttingSpeedDataItem.TypeId));

                if (!Deceleration.IsNullOrEmpty()) foreach (var x in Deceleration) l.Add(x.ToObservation(DecelerationDataItem.TypeId));
                if (!DecelerationDataSet.IsNullOrEmpty()) foreach (var x in DecelerationDataSet) l.Add(x.ToObservation(DecelerationDataItem.TypeId));

                if (!Density.IsNullOrEmpty()) foreach (var x in Density) l.Add(x.ToObservation(DensityDataItem.TypeId));
                if (!DensityDataSet.IsNullOrEmpty()) foreach (var x in DensityDataSet) l.Add(x.ToObservation(DensityDataItem.TypeId));

                if (!DepositionAccelerationVolumetric.IsNullOrEmpty()) foreach (var x in DepositionAccelerationVolumetric) l.Add(x.ToObservation(DepositionAccelerationVolumetricDataItem.TypeId));
                if (!DepositionAccelerationVolumetricDataSet.IsNullOrEmpty()) foreach (var x in DepositionAccelerationVolumetricDataSet) l.Add(x.ToObservation(DepositionAccelerationVolumetricDataItem.TypeId));

                if (!DepositionDensity.IsNullOrEmpty()) foreach (var x in DepositionDensity) l.Add(x.ToObservation(DepositionDensityDataItem.TypeId));
                if (!DepositionDensityDataSet.IsNullOrEmpty()) foreach (var x in DepositionDensityDataSet) l.Add(x.ToObservation(DepositionDensityDataItem.TypeId));

                if (!DepositionMass.IsNullOrEmpty()) foreach (var x in DepositionMass) l.Add(x.ToObservation(DepositionMassDataItem.TypeId));
                if (!DepositionMassDataSet.IsNullOrEmpty()) foreach (var x in DepositionMassDataSet) l.Add(x.ToObservation(DepositionMassDataItem.TypeId));

                if (!DepositionRateVolumetric.IsNullOrEmpty()) foreach (var x in DepositionRateVolumetric) l.Add(x.ToObservation(DepositionRateVolumetricDataItem.TypeId));
                if (!DepositionRateVolumetricDataSet.IsNullOrEmpty()) foreach (var x in DepositionRateVolumetricDataSet) l.Add(x.ToObservation(DepositionRateVolumetricDataItem.TypeId));

                if (!DepositionVolume.IsNullOrEmpty()) foreach (var x in DepositionVolume) l.Add(x.ToObservation(DepositionVolumeDataItem.TypeId));
                if (!DepositionVolumeDataSet.IsNullOrEmpty()) foreach (var x in DepositionVolumeDataSet) l.Add(x.ToObservation(DepositionVolumeDataItem.TypeId));

                if (!DewPoint.IsNullOrEmpty()) foreach (var x in DewPoint) l.Add(x.ToObservation(DewPointDataItem.TypeId));
                if (!DewPointDataSet.IsNullOrEmpty()) foreach (var x in DewPointDataSet) l.Add(x.ToObservation(DewPointDataItem.TypeId));

                if (!Diameter.IsNullOrEmpty()) foreach (var x in Diameter) l.Add(x.ToObservation(DiameterDataItem.TypeId));
                if (!DiameterDataSet.IsNullOrEmpty()) foreach (var x in DiameterDataSet) l.Add(x.ToObservation(DiameterDataItem.TypeId));

                if (!DischargeRate.IsNullOrEmpty()) foreach (var x in DischargeRate) l.Add(x.ToObservation(DischargeRateDataItem.TypeId));
                if (!DischargeRateDataSet.IsNullOrEmpty()) foreach (var x in DischargeRateDataSet) l.Add(x.ToObservation(DischargeRateDataItem.TypeId));

                if (!Displacement.IsNullOrEmpty()) foreach (var x in Displacement) l.Add(x.ToObservation(DisplacementDataItem.TypeId));
                if (!DisplacementDataSet.IsNullOrEmpty()) foreach (var x in DisplacementDataSet) l.Add(x.ToObservation(DisplacementDataItem.TypeId));

                if (!DisplacementAngular.IsNullOrEmpty()) foreach (var x in DisplacementAngular) l.Add(x.ToObservation(DisplacementAngularDataItem.TypeId));
                if (!DisplacementAngularDataSet.IsNullOrEmpty()) foreach (var x in DisplacementAngularDataSet) l.Add(x.ToObservation(DisplacementAngularDataItem.TypeId));

                if (!DisplacementLinear.IsNullOrEmpty()) foreach (var x in DisplacementLinear) l.Add(x.ToObservation(DisplacementLinearDataItem.TypeId));
                if (!DisplacementLinearDataSet.IsNullOrEmpty()) foreach (var x in DisplacementLinearDataSet) l.Add(x.ToObservation(DisplacementLinearDataItem.TypeId));

                if (!ElectricalEnergy.IsNullOrEmpty()) foreach (var x in ElectricalEnergy) l.Add(x.ToObservation(ElectricalEnergyDataItem.TypeId));
                if (!ElectricalEnergyDataSet.IsNullOrEmpty()) foreach (var x in ElectricalEnergyDataSet) l.Add(x.ToObservation(ElectricalEnergyDataItem.TypeId));

                if (!EquipmentTimer.IsNullOrEmpty()) foreach (var x in EquipmentTimer) l.Add(x.ToObservation(EquipmentTimerDataItem.TypeId));
                if (!EquipmentTimerDataSet.IsNullOrEmpty()) foreach (var x in EquipmentTimerDataSet) l.Add(x.ToObservation(EquipmentTimerDataItem.TypeId));

                if (!FillLevel.IsNullOrEmpty()) foreach (var x in FillLevel) l.Add(x.ToObservation(FillLevelDataItem.TypeId));
                if (!FillLevelDataSet.IsNullOrEmpty()) foreach (var x in FillLevelDataSet) l.Add(x.ToObservation(FillLevelDataItem.TypeId));

                if (!Flow.IsNullOrEmpty()) foreach (var x in Flow) l.Add(x.ToObservation(FlowDataItem.TypeId));
                if (!FlowDataSet.IsNullOrEmpty()) foreach (var x in FlowDataSet) l.Add(x.ToObservation(FlowDataItem.TypeId));

                if (!FollowingError.IsNullOrEmpty()) foreach (var x in FollowingError) l.Add(x.ToObservation(FollowingErrorDataItem.TypeId));
                if (!FollowingErrorDataSet.IsNullOrEmpty()) foreach (var x in FollowingErrorDataSet) l.Add(x.ToObservation(FollowingErrorDataItem.TypeId));

                if (!FollowingErrorAngular.IsNullOrEmpty()) foreach (var x in FollowingErrorAngular) l.Add(x.ToObservation(FollowingErrorAngularDataItem.TypeId));
                if (!FollowingErrorAngularDataSet.IsNullOrEmpty()) foreach (var x in FollowingErrorAngularDataSet) l.Add(x.ToObservation(FollowingErrorAngularDataItem.TypeId));

                if (!FollowingErrorLinear.IsNullOrEmpty()) foreach (var x in FollowingErrorLinear) l.Add(x.ToObservation(FollowingErrorLinearDataItem.TypeId));
                if (!FollowingErrorLinearDataSet.IsNullOrEmpty()) foreach (var x in FollowingErrorLinearDataSet) l.Add(x.ToObservation(FollowingErrorLinearDataItem.TypeId));

                if (!Frequency.IsNullOrEmpty()) foreach (var x in Frequency) l.Add(x.ToObservation(FrequencyDataItem.TypeId));
                if (!FrequencyDataSet.IsNullOrEmpty()) foreach (var x in FrequencyDataSet) l.Add(x.ToObservation(FrequencyDataItem.TypeId));

                if (!GlobalPosition.IsNullOrEmpty()) foreach (var x in GlobalPosition) l.Add(x.ToObservation(GlobalPositionDataItem.TypeId));
                if (!GlobalPositionDataSet.IsNullOrEmpty()) foreach (var x in GlobalPositionDataSet) l.Add(x.ToObservation(GlobalPositionDataItem.TypeId));

                if (!GravitationalAcceleration.IsNullOrEmpty()) foreach (var x in GravitationalAcceleration) l.Add(x.ToObservation(GravitationalAccelerationDataItem.TypeId));
                if (!GravitationalAccelerationDataSet.IsNullOrEmpty()) foreach (var x in GravitationalAccelerationDataSet) l.Add(x.ToObservation(GravitationalAccelerationDataItem.TypeId));

                if (!GravitationalForce.IsNullOrEmpty()) foreach (var x in GravitationalForce) l.Add(x.ToObservation(GravitationalForceDataItem.TypeId));
                if (!GravitationalForceDataSet.IsNullOrEmpty()) foreach (var x in GravitationalForceDataSet) l.Add(x.ToObservation(GravitationalForceDataItem.TypeId));

                if (!HumidityAbsolute.IsNullOrEmpty()) foreach (var x in HumidityAbsolute) l.Add(x.ToObservation(HumidityAbsoluteDataItem.TypeId));
                if (!HumidityAbsoluteDataSet.IsNullOrEmpty()) foreach (var x in HumidityAbsoluteDataSet) l.Add(x.ToObservation(HumidityAbsoluteDataItem.TypeId));

                if (!HumidityRelative.IsNullOrEmpty()) foreach (var x in HumidityRelative) l.Add(x.ToObservation(HumidityRelativeDataItem.TypeId));
                if (!HumidityRelativeDataSet.IsNullOrEmpty()) foreach (var x in HumidityRelativeDataSet) l.Add(x.ToObservation(HumidityRelativeDataItem.TypeId));

                if (!HumiditySpecific.IsNullOrEmpty()) foreach (var x in HumiditySpecific) l.Add(x.ToObservation(HumiditySpecificDataItem.TypeId));
                if (!HumiditySpecificDataSet.IsNullOrEmpty()) foreach (var x in HumiditySpecificDataSet) l.Add(x.ToObservation(HumiditySpecificDataItem.TypeId));

                if (!Length.IsNullOrEmpty()) foreach (var x in Length) l.Add(x.ToObservation(LengthDataItem.TypeId));
                if (!LengthDataSet.IsNullOrEmpty()) foreach (var x in LengthDataSet) l.Add(x.ToObservation(LengthDataItem.TypeId));

                if (!Level.IsNullOrEmpty()) foreach (var x in Level) l.Add(x.ToObservation(LevelDataItem.TypeId));
                if (!LevelDataSet.IsNullOrEmpty()) foreach (var x in LevelDataSet) l.Add(x.ToObservation(LevelDataItem.TypeId));

                if (!LinearForce.IsNullOrEmpty()) foreach (var x in LinearForce) l.Add(x.ToObservation(LinearForceDataItem.TypeId));
                if (!LinearForceDataSet.IsNullOrEmpty()) foreach (var x in LinearForceDataSet) l.Add(x.ToObservation(LinearForceDataItem.TypeId));

                if (!Load.IsNullOrEmpty()) foreach (var x in Load) l.Add(x.ToObservation(LoadDataItem.TypeId));
                if (!LoadDataSet.IsNullOrEmpty()) foreach (var x in LoadDataSet) l.Add(x.ToObservation(LoadDataItem.TypeId));

                if (!Mass.IsNullOrEmpty()) foreach (var x in Mass) l.Add(x.ToObservation(MassDataItem.TypeId));
                if (!MassDataSet.IsNullOrEmpty()) foreach (var x in MassDataSet) l.Add(x.ToObservation(MassDataItem.TypeId));

                if (!ObservationUpdateRate.IsNullOrEmpty()) foreach (var x in ObservationUpdateRate) l.Add(x.ToObservation(ObservationUpdateRateDataItem.TypeId));
                if (!ObservationUpdateRateDataSet.IsNullOrEmpty()) foreach (var x in ObservationUpdateRateDataSet) l.Add(x.ToObservation(ObservationUpdateRateDataItem.TypeId));

                if (!Openness.IsNullOrEmpty()) foreach (var x in Openness) l.Add(x.ToObservation(OpennessDataItem.TypeId));
                if (!OpennessDataSet.IsNullOrEmpty()) foreach (var x in OpennessDataSet) l.Add(x.ToObservation(OpennessDataItem.TypeId));

                if (!Orientation.IsNullOrEmpty()) foreach (var x in Orientation) l.Add(x.ToObservation(OrientationDataItem.TypeId));
                if (!OrientationDataSet.IsNullOrEmpty()) foreach (var x in OrientationDataSet) l.Add(x.ToObservation(OrientationDataItem.TypeId));

                if (!PathFeedrate.IsNullOrEmpty()) foreach (var x in PathFeedrate) l.Add(x.ToObservation(PathFeedrateDataItem.TypeId));
                if (!PathFeedrateDataSet.IsNullOrEmpty()) foreach (var x in PathFeedrateDataSet) l.Add(x.ToObservation(PathFeedrateDataItem.TypeId));

                if (!PathFeedratePerRevolution.IsNullOrEmpty()) foreach (var x in PathFeedratePerRevolution) l.Add(x.ToObservation(PathFeedratePerRevolutionDataItem.TypeId));
                if (!PathFeedratePerRevolutionDataSet.IsNullOrEmpty()) foreach (var x in PathFeedratePerRevolutionDataSet) l.Add(x.ToObservation(PathFeedratePerRevolutionDataItem.TypeId));

                if (!PathPosition.IsNullOrEmpty()) foreach (var x in PathPosition) l.Add(x.ToObservation(PathPositionDataItem.TypeId));
                if (!PathPositionDataSet.IsNullOrEmpty()) foreach (var x in PathPositionDataSet) l.Add(x.ToObservation(PathPositionDataItem.TypeId));

                if (!PH.IsNullOrEmpty()) foreach (var x in PH) l.Add(x.ToObservation(PHDataItem.TypeId));
                if (!PHDataSet.IsNullOrEmpty()) foreach (var x in PHDataSet) l.Add(x.ToObservation(PHDataItem.TypeId));

                if (!Position.IsNullOrEmpty()) foreach (var x in Position) l.Add(x.ToObservation(PositionDataItem.TypeId));
                if (!PositionDataSet.IsNullOrEmpty()) foreach (var x in PositionDataSet) l.Add(x.ToObservation(PositionDataItem.TypeId));

                if (!PositionCartesian.IsNullOrEmpty()) foreach (var x in PositionCartesian) l.Add(x.ToObservation(PositionCartesianDataItem.TypeId));
                if (!PositionCartesianDataSet.IsNullOrEmpty()) foreach (var x in PositionCartesianDataSet) l.Add(x.ToObservation(PositionCartesianDataItem.TypeId));

                if (!PowerFactor.IsNullOrEmpty()) foreach (var x in PowerFactor) l.Add(x.ToObservation(PowerFactorDataItem.TypeId));
                if (!PowerFactorDataSet.IsNullOrEmpty()) foreach (var x in PowerFactorDataSet) l.Add(x.ToObservation(PowerFactorDataItem.TypeId));

                if (!Pressure.IsNullOrEmpty()) foreach (var x in Pressure) l.Add(x.ToObservation(PressureDataItem.TypeId));
                if (!PressureDataSet.IsNullOrEmpty()) foreach (var x in PressureDataSet) l.Add(x.ToObservation(PressureDataItem.TypeId));

                if (!PressureAbsolute.IsNullOrEmpty()) foreach (var x in PressureAbsolute) l.Add(x.ToObservation(PressureAbsoluteDataItem.TypeId));
                if (!PressureAbsoluteDataSet.IsNullOrEmpty()) foreach (var x in PressureAbsoluteDataSet) l.Add(x.ToObservation(PressureAbsoluteDataItem.TypeId));

                if (!PressurizationRate.IsNullOrEmpty()) foreach (var x in PressurizationRate) l.Add(x.ToObservation(PressurizationRateDataItem.TypeId));
                if (!PressurizationRateDataSet.IsNullOrEmpty()) foreach (var x in PressurizationRateDataSet) l.Add(x.ToObservation(PressurizationRateDataItem.TypeId));

                if (!ProcessTimer.IsNullOrEmpty()) foreach (var x in ProcessTimer) l.Add(x.ToObservation(ProcessTimerDataItem.TypeId));
                if (!ProcessTimerDataSet.IsNullOrEmpty()) foreach (var x in ProcessTimerDataSet) l.Add(x.ToObservation(ProcessTimerDataItem.TypeId));

                if (!Resistance.IsNullOrEmpty()) foreach (var x in Resistance) l.Add(x.ToObservation(ResistanceDataItem.TypeId));
                if (!ResistanceDataSet.IsNullOrEmpty()) foreach (var x in ResistanceDataSet) l.Add(x.ToObservation(ResistanceDataItem.TypeId));

                if (!RotaryVelocity.IsNullOrEmpty()) foreach (var x in RotaryVelocity) l.Add(x.ToObservation(RotaryVelocityDataItem.TypeId));
                if (!RotaryVelocityDataSet.IsNullOrEmpty()) foreach (var x in RotaryVelocityDataSet) l.Add(x.ToObservation(RotaryVelocityDataItem.TypeId));

                if (!SettlingError.IsNullOrEmpty()) foreach (var x in SettlingError) l.Add(x.ToObservation(SettlingErrorDataItem.TypeId));
                if (!SettlingErrorDataSet.IsNullOrEmpty()) foreach (var x in SettlingErrorDataSet) l.Add(x.ToObservation(SettlingErrorDataItem.TypeId));

                if (!SettlingErrorAngular.IsNullOrEmpty()) foreach (var x in SettlingErrorAngular) l.Add(x.ToObservation(SettlingErrorAngularDataItem.TypeId));
                if (!SettlingErrorAngularDataSet.IsNullOrEmpty()) foreach (var x in SettlingErrorAngularDataSet) l.Add(x.ToObservation(SettlingErrorAngularDataItem.TypeId));

                if (!SettlingErrorLinear.IsNullOrEmpty()) foreach (var x in SettlingErrorLinear) l.Add(x.ToObservation(SettlingErrorLinearDataItem.TypeId));
                if (!SettlingErrorLinearDataSet.IsNullOrEmpty()) foreach (var x in SettlingErrorLinearDataSet) l.Add(x.ToObservation(SettlingErrorLinearDataItem.TypeId));

                if (!SoundLevel.IsNullOrEmpty()) foreach (var x in SoundLevel) l.Add(x.ToObservation(SoundLevelDataItem.TypeId));
                if (!SoundLevelDataSet.IsNullOrEmpty()) foreach (var x in SoundLevelDataSet) l.Add(x.ToObservation(SoundLevelDataItem.TypeId));

                if (!SpindleSpeed.IsNullOrEmpty()) foreach (var x in SpindleSpeed) l.Add(x.ToObservation(SpindleSpeedDataItem.TypeId));
                if (!SpindleSpeedDataSet.IsNullOrEmpty()) foreach (var x in SpindleSpeedDataSet) l.Add(x.ToObservation(SpindleSpeedDataItem.TypeId));

                if (!Strain.IsNullOrEmpty()) foreach (var x in Strain) l.Add(x.ToObservation(StrainDataItem.TypeId));
                if (!StrainDataSet.IsNullOrEmpty()) foreach (var x in StrainDataSet) l.Add(x.ToObservation(StrainDataItem.TypeId));

                if (!Temperature.IsNullOrEmpty()) foreach (var x in Temperature) l.Add(x.ToObservation(TemperatureDataItem.TypeId));
                if (!TemperatureDataSet.IsNullOrEmpty()) foreach (var x in TemperatureDataSet) l.Add(x.ToObservation(TemperatureDataItem.TypeId));

                if (!Tension.IsNullOrEmpty()) foreach (var x in Tension) l.Add(x.ToObservation(TensionDataItem.TypeId));
                if (!TensionDataSet.IsNullOrEmpty()) foreach (var x in TensionDataSet) l.Add(x.ToObservation(TensionDataItem.TypeId));

                if (!Tilt.IsNullOrEmpty()) foreach (var x in Tilt) l.Add(x.ToObservation(TiltDataItem.TypeId));
                if (!TiltDataSet.IsNullOrEmpty()) foreach (var x in TiltDataSet) l.Add(x.ToObservation(TiltDataItem.TypeId));

                if (!Torque.IsNullOrEmpty()) foreach (var x in Torque) l.Add(x.ToObservation(TorqueDataItem.TypeId));
                if (!TorqueDataSet.IsNullOrEmpty()) foreach (var x in TorqueDataSet) l.Add(x.ToObservation(TorqueDataItem.TypeId));

                if (!Velocity.IsNullOrEmpty()) foreach (var x in Velocity) l.Add(x.ToObservation(VelocityDataItem.TypeId));
                if (!VelocityDataSet.IsNullOrEmpty()) foreach (var x in VelocityDataSet) l.Add(x.ToObservation(VelocityDataItem.TypeId));

                if (!Viscosity.IsNullOrEmpty()) foreach (var x in Viscosity) l.Add(x.ToObservation(ViscosityDataItem.TypeId));
                if (!ViscosityDataSet.IsNullOrEmpty()) foreach (var x in ViscosityDataSet) l.Add(x.ToObservation(ViscosityDataItem.TypeId));

                if (!VoltAmpere.IsNullOrEmpty()) foreach (var x in VoltAmpere) l.Add(x.ToObservation(VoltAmpereDataItem.TypeId));
                if (!VoltAmpereDataSet.IsNullOrEmpty()) foreach (var x in VoltAmpereDataSet) l.Add(x.ToObservation(VoltAmpereDataItem.TypeId));

                if (!VoltAmpereReactive.IsNullOrEmpty()) foreach (var x in VoltAmpereReactive) l.Add(x.ToObservation(VoltAmpereReactiveDataItem.TypeId));
                if (!VoltAmpereReactiveDataSet.IsNullOrEmpty()) foreach (var x in VoltAmpereReactiveDataSet) l.Add(x.ToObservation(VoltAmpereReactiveDataItem.TypeId));

                if (!Voltage.IsNullOrEmpty()) foreach (var x in Voltage) l.Add(x.ToObservation(VoltageDataItem.TypeId));
                if (!VoltageDataSet.IsNullOrEmpty()) foreach (var x in VoltageDataSet) l.Add(x.ToObservation(VoltageDataItem.TypeId));

                if (!VoltageAC.IsNullOrEmpty()) foreach (var x in VoltageAC) l.Add(x.ToObservation(VoltageACDataItem.TypeId));
                if (!VoltageACDataSet.IsNullOrEmpty()) foreach (var x in VoltageACDataSet) l.Add(x.ToObservation(VoltageACDataItem.TypeId));

                if (!VoltageDC.IsNullOrEmpty()) foreach (var x in VoltageDC) l.Add(x.ToObservation(VoltageDCDataItem.TypeId));
                if (!VoltageDCDataSet.IsNullOrEmpty()) foreach (var x in VoltageDCDataSet) l.Add(x.ToObservation(VoltageDCDataItem.TypeId));

                if (!VolumeFluid.IsNullOrEmpty()) foreach (var x in VolumeFluid) l.Add(x.ToObservation(VolumeFluidDataItem.TypeId));
                if (!VolumeFluidDataSet.IsNullOrEmpty()) foreach (var x in VolumeFluidDataSet) l.Add(x.ToObservation(VolumeFluidDataItem.TypeId));

                if (!VolumeSpatial.IsNullOrEmpty()) foreach (var x in VolumeSpatial) l.Add(x.ToObservation(VolumeSpatialDataItem.TypeId));
                if (!VolumeSpatialDataSet.IsNullOrEmpty()) foreach (var x in VolumeSpatialDataSet) l.Add(x.ToObservation(VolumeSpatialDataItem.TypeId));

                if (!Wattage.IsNullOrEmpty()) foreach (var x in Wattage) l.Add(x.ToObservation(WattageDataItem.TypeId));
                if (!WattageDataSet.IsNullOrEmpty()) foreach (var x in WattageDataSet) l.Add(x.ToObservation(WattageDataItem.TypeId));

                if (!XDimension.IsNullOrEmpty()) foreach (var x in XDimension) l.Add(x.ToObservation(XDimensionDataItem.TypeId));
                if (!XDimensionDataSet.IsNullOrEmpty()) foreach (var x in XDimensionDataSet) l.Add(x.ToObservation(XDimensionDataItem.TypeId));

                if (!YDimension.IsNullOrEmpty()) foreach (var x in YDimension) l.Add(x.ToObservation(YDimensionDataItem.TypeId));
                if (!YDimensionDataSet.IsNullOrEmpty()) foreach (var x in YDimensionDataSet) l.Add(x.ToObservation(YDimensionDataItem.TypeId));

                if (!ZDimension.IsNullOrEmpty()) foreach (var x in ZDimension) l.Add(x.ToObservation(ZDimensionDataItem.TypeId));
                if (!ZDimensionDataSet.IsNullOrEmpty()) foreach (var x in ZDimensionDataSet) l.Add(x.ToObservation(ZDimensionDataItem.TypeId));


                return l;
            }
        }
        [JsonPropertyName("Acceleration")]
        public IEnumerable<JsonSampleValue> Acceleration { get; set; }

        [JsonPropertyName("AccelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> AccelerationDataSet { get; set; }


        [JsonPropertyName("AccumulatedTime")]
        public IEnumerable<JsonSampleValue> AccumulatedTime { get; set; }

        [JsonPropertyName("AccumulatedTimeDataSet")]
        public IEnumerable<JsonSampleDataSet> AccumulatedTimeDataSet { get; set; }


        [JsonPropertyName("Amperage")]
        public IEnumerable<JsonSampleValue> Amperage { get; set; }

        [JsonPropertyName("AmperageDataSet")]
        public IEnumerable<JsonSampleDataSet> AmperageDataSet { get; set; }


        [JsonPropertyName("AmperageAC")]
        public IEnumerable<JsonSampleValue> AmperageAC { get; set; }

        [JsonPropertyName("AmperageACDataSet")]
        public IEnumerable<JsonSampleDataSet> AmperageACDataSet { get; set; }


        [JsonPropertyName("AmperageDC")]
        public IEnumerable<JsonSampleValue> AmperageDC { get; set; }

        [JsonPropertyName("AmperageDCDataSet")]
        public IEnumerable<JsonSampleDataSet> AmperageDCDataSet { get; set; }


        [JsonPropertyName("Angle")]
        public IEnumerable<JsonSampleValue> Angle { get; set; }

        [JsonPropertyName("AngleDataSet")]
        public IEnumerable<JsonSampleDataSet> AngleDataSet { get; set; }


        [JsonPropertyName("AngularAcceleration")]
        public IEnumerable<JsonSampleValue> AngularAcceleration { get; set; }

        [JsonPropertyName("AngularAccelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> AngularAccelerationDataSet { get; set; }


        [JsonPropertyName("AngularDeceleration")]
        public IEnumerable<JsonSampleValue> AngularDeceleration { get; set; }

        [JsonPropertyName("AngularDecelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> AngularDecelerationDataSet { get; set; }


        [JsonPropertyName("AngularVelocity")]
        public IEnumerable<JsonSampleValue> AngularVelocity { get; set; }

        [JsonPropertyName("AngularVelocityDataSet")]
        public IEnumerable<JsonSampleDataSet> AngularVelocityDataSet { get; set; }


        [JsonPropertyName("AssetUpdateRate")]
        public IEnumerable<JsonSampleValue> AssetUpdateRate { get; set; }

        [JsonPropertyName("AssetUpdateRateDataSet")]
        public IEnumerable<JsonSampleDataSet> AssetUpdateRateDataSet { get; set; }


        [JsonPropertyName("AxisFeedrate")]
        public IEnumerable<JsonSampleValue> AxisFeedrate { get; set; }

        [JsonPropertyName("AxisFeedrateDataSet")]
        public IEnumerable<JsonSampleDataSet> AxisFeedrateDataSet { get; set; }


        [JsonPropertyName("BatteryCapacity")]
        public IEnumerable<JsonSampleValue> BatteryCapacity { get; set; }

        [JsonPropertyName("BatteryCapacityDataSet")]
        public IEnumerable<JsonSampleDataSet> BatteryCapacityDataSet { get; set; }


        [JsonPropertyName("BatteryCharge")]
        public IEnumerable<JsonSampleValue> BatteryCharge { get; set; }

        [JsonPropertyName("BatteryChargeDataSet")]
        public IEnumerable<JsonSampleDataSet> BatteryChargeDataSet { get; set; }


        [JsonPropertyName("CapacityFluid")]
        public IEnumerable<JsonSampleValue> CapacityFluid { get; set; }

        [JsonPropertyName("CapacityFluidDataSet")]
        public IEnumerable<JsonSampleDataSet> CapacityFluidDataSet { get; set; }


        [JsonPropertyName("CapacitySpatial")]
        public IEnumerable<JsonSampleValue> CapacitySpatial { get; set; }

        [JsonPropertyName("CapacitySpatialDataSet")]
        public IEnumerable<JsonSampleDataSet> CapacitySpatialDataSet { get; set; }


        [JsonPropertyName("ChargeRate")]
        public IEnumerable<JsonSampleValue> ChargeRate { get; set; }

        [JsonPropertyName("ChargeRateDataSet")]
        public IEnumerable<JsonSampleDataSet> ChargeRateDataSet { get; set; }


        [JsonPropertyName("Concentration")]
        public IEnumerable<JsonSampleValue> Concentration { get; set; }

        [JsonPropertyName("ConcentrationDataSet")]
        public IEnumerable<JsonSampleDataSet> ConcentrationDataSet { get; set; }


        [JsonPropertyName("Conductivity")]
        public IEnumerable<JsonSampleValue> Conductivity { get; set; }

        [JsonPropertyName("ConductivityDataSet")]
        public IEnumerable<JsonSampleDataSet> ConductivityDataSet { get; set; }


        [JsonPropertyName("CuttingSpeed")]
        public IEnumerable<JsonSampleValue> CuttingSpeed { get; set; }

        [JsonPropertyName("CuttingSpeedDataSet")]
        public IEnumerable<JsonSampleDataSet> CuttingSpeedDataSet { get; set; }


        [JsonPropertyName("Deceleration")]
        public IEnumerable<JsonSampleValue> Deceleration { get; set; }

        [JsonPropertyName("DecelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> DecelerationDataSet { get; set; }


        [JsonPropertyName("Density")]
        public IEnumerable<JsonSampleValue> Density { get; set; }

        [JsonPropertyName("DensityDataSet")]
        public IEnumerable<JsonSampleDataSet> DensityDataSet { get; set; }


        [JsonPropertyName("DepositionAccelerationVolumetric")]
        public IEnumerable<JsonSampleValue> DepositionAccelerationVolumetric { get; set; }

        [JsonPropertyName("DepositionAccelerationVolumetricDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionAccelerationVolumetricDataSet { get; set; }


        [JsonPropertyName("DepositionDensity")]
        public IEnumerable<JsonSampleValue> DepositionDensity { get; set; }

        [JsonPropertyName("DepositionDensityDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionDensityDataSet { get; set; }


        [JsonPropertyName("DepositionMass")]
        public IEnumerable<JsonSampleValue> DepositionMass { get; set; }

        [JsonPropertyName("DepositionMassDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionMassDataSet { get; set; }


        [JsonPropertyName("DepositionRateVolumetric")]
        public IEnumerable<JsonSampleValue> DepositionRateVolumetric { get; set; }

        [JsonPropertyName("DepositionRateVolumetricDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionRateVolumetricDataSet { get; set; }


        [JsonPropertyName("DepositionVolume")]
        public IEnumerable<JsonSampleValue> DepositionVolume { get; set; }

        [JsonPropertyName("DepositionVolumeDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionVolumeDataSet { get; set; }


        [JsonPropertyName("DewPoint")]
        public IEnumerable<JsonSampleValue> DewPoint { get; set; }

        [JsonPropertyName("DewPointDataSet")]
        public IEnumerable<JsonSampleDataSet> DewPointDataSet { get; set; }


        [JsonPropertyName("Diameter")]
        public IEnumerable<JsonSampleValue> Diameter { get; set; }

        [JsonPropertyName("DiameterDataSet")]
        public IEnumerable<JsonSampleDataSet> DiameterDataSet { get; set; }


        [JsonPropertyName("DischargeRate")]
        public IEnumerable<JsonSampleValue> DischargeRate { get; set; }

        [JsonPropertyName("DischargeRateDataSet")]
        public IEnumerable<JsonSampleDataSet> DischargeRateDataSet { get; set; }


        [JsonPropertyName("Displacement")]
        public IEnumerable<JsonSampleValue> Displacement { get; set; }

        [JsonPropertyName("DisplacementDataSet")]
        public IEnumerable<JsonSampleDataSet> DisplacementDataSet { get; set; }


        [JsonPropertyName("DisplacementAngular")]
        public IEnumerable<JsonSampleValue> DisplacementAngular { get; set; }

        [JsonPropertyName("DisplacementAngularDataSet")]
        public IEnumerable<JsonSampleDataSet> DisplacementAngularDataSet { get; set; }


        [JsonPropertyName("DisplacementLinear")]
        public IEnumerable<JsonSampleValue> DisplacementLinear { get; set; }

        [JsonPropertyName("DisplacementLinearDataSet")]
        public IEnumerable<JsonSampleDataSet> DisplacementLinearDataSet { get; set; }


        [JsonPropertyName("ElectricalEnergy")]
        public IEnumerable<JsonSampleValue> ElectricalEnergy { get; set; }

        [JsonPropertyName("ElectricalEnergyDataSet")]
        public IEnumerable<JsonSampleDataSet> ElectricalEnergyDataSet { get; set; }


        [JsonPropertyName("EquipmentTimer")]
        public IEnumerable<JsonSampleValue> EquipmentTimer { get; set; }

        [JsonPropertyName("EquipmentTimerDataSet")]
        public IEnumerable<JsonSampleDataSet> EquipmentTimerDataSet { get; set; }


        [JsonPropertyName("FillLevel")]
        public IEnumerable<JsonSampleValue> FillLevel { get; set; }

        [JsonPropertyName("FillLevelDataSet")]
        public IEnumerable<JsonSampleDataSet> FillLevelDataSet { get; set; }


        [JsonPropertyName("Flow")]
        public IEnumerable<JsonSampleValue> Flow { get; set; }

        [JsonPropertyName("FlowDataSet")]
        public IEnumerable<JsonSampleDataSet> FlowDataSet { get; set; }


        [JsonPropertyName("FollowingError")]
        public IEnumerable<JsonSampleValue> FollowingError { get; set; }

        [JsonPropertyName("FollowingErrorDataSet")]
        public IEnumerable<JsonSampleDataSet> FollowingErrorDataSet { get; set; }


        [JsonPropertyName("FollowingErrorAngular")]
        public IEnumerable<JsonSampleValue> FollowingErrorAngular { get; set; }

        [JsonPropertyName("FollowingErrorAngularDataSet")]
        public IEnumerable<JsonSampleDataSet> FollowingErrorAngularDataSet { get; set; }


        [JsonPropertyName("FollowingErrorLinear")]
        public IEnumerable<JsonSampleValue> FollowingErrorLinear { get; set; }

        [JsonPropertyName("FollowingErrorLinearDataSet")]
        public IEnumerable<JsonSampleDataSet> FollowingErrorLinearDataSet { get; set; }


        [JsonPropertyName("Frequency")]
        public IEnumerable<JsonSampleValue> Frequency { get; set; }

        [JsonPropertyName("FrequencyDataSet")]
        public IEnumerable<JsonSampleDataSet> FrequencyDataSet { get; set; }


        [JsonPropertyName("GlobalPosition")]
        public IEnumerable<JsonSampleValue> GlobalPosition { get; set; }

        [JsonPropertyName("GlobalPositionDataSet")]
        public IEnumerable<JsonSampleDataSet> GlobalPositionDataSet { get; set; }


        [JsonPropertyName("GravitationalAcceleration")]
        public IEnumerable<JsonSampleValue> GravitationalAcceleration { get; set; }

        [JsonPropertyName("GravitationalAccelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> GravitationalAccelerationDataSet { get; set; }


        [JsonPropertyName("GravitationalForce")]
        public IEnumerable<JsonSampleValue> GravitationalForce { get; set; }

        [JsonPropertyName("GravitationalForceDataSet")]
        public IEnumerable<JsonSampleDataSet> GravitationalForceDataSet { get; set; }


        [JsonPropertyName("HumidityAbsolute")]
        public IEnumerable<JsonSampleValue> HumidityAbsolute { get; set; }

        [JsonPropertyName("HumidityAbsoluteDataSet")]
        public IEnumerable<JsonSampleDataSet> HumidityAbsoluteDataSet { get; set; }


        [JsonPropertyName("HumidityRelative")]
        public IEnumerable<JsonSampleValue> HumidityRelative { get; set; }

        [JsonPropertyName("HumidityRelativeDataSet")]
        public IEnumerable<JsonSampleDataSet> HumidityRelativeDataSet { get; set; }


        [JsonPropertyName("HumiditySpecific")]
        public IEnumerable<JsonSampleValue> HumiditySpecific { get; set; }

        [JsonPropertyName("HumiditySpecificDataSet")]
        public IEnumerable<JsonSampleDataSet> HumiditySpecificDataSet { get; set; }


        [JsonPropertyName("Length")]
        public IEnumerable<JsonSampleValue> Length { get; set; }

        [JsonPropertyName("LengthDataSet")]
        public IEnumerable<JsonSampleDataSet> LengthDataSet { get; set; }


        [JsonPropertyName("Level")]
        public IEnumerable<JsonSampleValue> Level { get; set; }

        [JsonPropertyName("LevelDataSet")]
        public IEnumerable<JsonSampleDataSet> LevelDataSet { get; set; }


        [JsonPropertyName("LinearForce")]
        public IEnumerable<JsonSampleValue> LinearForce { get; set; }

        [JsonPropertyName("LinearForceDataSet")]
        public IEnumerable<JsonSampleDataSet> LinearForceDataSet { get; set; }


        [JsonPropertyName("Load")]
        public IEnumerable<JsonSampleValue> Load { get; set; }

        [JsonPropertyName("LoadDataSet")]
        public IEnumerable<JsonSampleDataSet> LoadDataSet { get; set; }


        [JsonPropertyName("Mass")]
        public IEnumerable<JsonSampleValue> Mass { get; set; }

        [JsonPropertyName("MassDataSet")]
        public IEnumerable<JsonSampleDataSet> MassDataSet { get; set; }


        [JsonPropertyName("ObservationUpdateRate")]
        public IEnumerable<JsonSampleValue> ObservationUpdateRate { get; set; }

        [JsonPropertyName("ObservationUpdateRateDataSet")]
        public IEnumerable<JsonSampleDataSet> ObservationUpdateRateDataSet { get; set; }


        [JsonPropertyName("Openness")]
        public IEnumerable<JsonSampleValue> Openness { get; set; }

        [JsonPropertyName("OpennessDataSet")]
        public IEnumerable<JsonSampleDataSet> OpennessDataSet { get; set; }


        [JsonPropertyName("Orientation")]
        public IEnumerable<JsonSampleValue> Orientation { get; set; }

        [JsonPropertyName("OrientationDataSet")]
        public IEnumerable<JsonSampleDataSet> OrientationDataSet { get; set; }


        [JsonPropertyName("PathFeedrate")]
        public IEnumerable<JsonSampleValue> PathFeedrate { get; set; }

        [JsonPropertyName("PathFeedrateDataSet")]
        public IEnumerable<JsonSampleDataSet> PathFeedrateDataSet { get; set; }


        [JsonPropertyName("PathFeedratePerRevolution")]
        public IEnumerable<JsonSampleValue> PathFeedratePerRevolution { get; set; }

        [JsonPropertyName("PathFeedratePerRevolutionDataSet")]
        public IEnumerable<JsonSampleDataSet> PathFeedratePerRevolutionDataSet { get; set; }


        [JsonPropertyName("PathPosition")]
        public IEnumerable<JsonSampleValue> PathPosition { get; set; }

        [JsonPropertyName("PathPositionDataSet")]
        public IEnumerable<JsonSampleDataSet> PathPositionDataSet { get; set; }


        [JsonPropertyName("PH")]
        public IEnumerable<JsonSampleValue> PH { get; set; }

        [JsonPropertyName("PHDataSet")]
        public IEnumerable<JsonSampleDataSet> PHDataSet { get; set; }


        [JsonPropertyName("Position")]
        public IEnumerable<JsonSampleValue> Position { get; set; }

        [JsonPropertyName("PositionDataSet")]
        public IEnumerable<JsonSampleDataSet> PositionDataSet { get; set; }


        [JsonPropertyName("PositionCartesian")]
        public IEnumerable<JsonSampleValue> PositionCartesian { get; set; }

        [JsonPropertyName("PositionCartesianDataSet")]
        public IEnumerable<JsonSampleDataSet> PositionCartesianDataSet { get; set; }


        [JsonPropertyName("PowerFactor")]
        public IEnumerable<JsonSampleValue> PowerFactor { get; set; }

        [JsonPropertyName("PowerFactorDataSet")]
        public IEnumerable<JsonSampleDataSet> PowerFactorDataSet { get; set; }


        [JsonPropertyName("Pressure")]
        public IEnumerable<JsonSampleValue> Pressure { get; set; }

        [JsonPropertyName("PressureDataSet")]
        public IEnumerable<JsonSampleDataSet> PressureDataSet { get; set; }


        [JsonPropertyName("PressureAbsolute")]
        public IEnumerable<JsonSampleValue> PressureAbsolute { get; set; }

        [JsonPropertyName("PressureAbsoluteDataSet")]
        public IEnumerable<JsonSampleDataSet> PressureAbsoluteDataSet { get; set; }


        [JsonPropertyName("PressurizationRate")]
        public IEnumerable<JsonSampleValue> PressurizationRate { get; set; }

        [JsonPropertyName("PressurizationRateDataSet")]
        public IEnumerable<JsonSampleDataSet> PressurizationRateDataSet { get; set; }


        [JsonPropertyName("ProcessTimer")]
        public IEnumerable<JsonSampleValue> ProcessTimer { get; set; }

        [JsonPropertyName("ProcessTimerDataSet")]
        public IEnumerable<JsonSampleDataSet> ProcessTimerDataSet { get; set; }


        [JsonPropertyName("Resistance")]
        public IEnumerable<JsonSampleValue> Resistance { get; set; }

        [JsonPropertyName("ResistanceDataSet")]
        public IEnumerable<JsonSampleDataSet> ResistanceDataSet { get; set; }


        [JsonPropertyName("RotaryVelocity")]
        public IEnumerable<JsonSampleValue> RotaryVelocity { get; set; }

        [JsonPropertyName("RotaryVelocityDataSet")]
        public IEnumerable<JsonSampleDataSet> RotaryVelocityDataSet { get; set; }


        [JsonPropertyName("SettlingError")]
        public IEnumerable<JsonSampleValue> SettlingError { get; set; }

        [JsonPropertyName("SettlingErrorDataSet")]
        public IEnumerable<JsonSampleDataSet> SettlingErrorDataSet { get; set; }


        [JsonPropertyName("SettlingErrorAngular")]
        public IEnumerable<JsonSampleValue> SettlingErrorAngular { get; set; }

        [JsonPropertyName("SettlingErrorAngularDataSet")]
        public IEnumerable<JsonSampleDataSet> SettlingErrorAngularDataSet { get; set; }


        [JsonPropertyName("SettlingErrorLinear")]
        public IEnumerable<JsonSampleValue> SettlingErrorLinear { get; set; }

        [JsonPropertyName("SettlingErrorLinearDataSet")]
        public IEnumerable<JsonSampleDataSet> SettlingErrorLinearDataSet { get; set; }


        [JsonPropertyName("SoundLevel")]
        public IEnumerable<JsonSampleValue> SoundLevel { get; set; }

        [JsonPropertyName("SoundLevelDataSet")]
        public IEnumerable<JsonSampleDataSet> SoundLevelDataSet { get; set; }


        [JsonPropertyName("SpindleSpeed")]
        public IEnumerable<JsonSampleValue> SpindleSpeed { get; set; }

        [JsonPropertyName("SpindleSpeedDataSet")]
        public IEnumerable<JsonSampleDataSet> SpindleSpeedDataSet { get; set; }


        [JsonPropertyName("Strain")]
        public IEnumerable<JsonSampleValue> Strain { get; set; }

        [JsonPropertyName("StrainDataSet")]
        public IEnumerable<JsonSampleDataSet> StrainDataSet { get; set; }


        [JsonPropertyName("Temperature")]
        public IEnumerable<JsonSampleValue> Temperature { get; set; }

        [JsonPropertyName("TemperatureDataSet")]
        public IEnumerable<JsonSampleDataSet> TemperatureDataSet { get; set; }


        [JsonPropertyName("Tension")]
        public IEnumerable<JsonSampleValue> Tension { get; set; }

        [JsonPropertyName("TensionDataSet")]
        public IEnumerable<JsonSampleDataSet> TensionDataSet { get; set; }


        [JsonPropertyName("Tilt")]
        public IEnumerable<JsonSampleValue> Tilt { get; set; }

        [JsonPropertyName("TiltDataSet")]
        public IEnumerable<JsonSampleDataSet> TiltDataSet { get; set; }


        [JsonPropertyName("Torque")]
        public IEnumerable<JsonSampleValue> Torque { get; set; }

        [JsonPropertyName("TorqueDataSet")]
        public IEnumerable<JsonSampleDataSet> TorqueDataSet { get; set; }


        [JsonPropertyName("Velocity")]
        public IEnumerable<JsonSampleValue> Velocity { get; set; }

        [JsonPropertyName("VelocityDataSet")]
        public IEnumerable<JsonSampleDataSet> VelocityDataSet { get; set; }


        [JsonPropertyName("Viscosity")]
        public IEnumerable<JsonSampleValue> Viscosity { get; set; }

        [JsonPropertyName("ViscosityDataSet")]
        public IEnumerable<JsonSampleDataSet> ViscosityDataSet { get; set; }


        [JsonPropertyName("VoltAmpere")]
        public IEnumerable<JsonSampleValue> VoltAmpere { get; set; }

        [JsonPropertyName("VoltAmpereDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltAmpereDataSet { get; set; }


        [JsonPropertyName("VoltAmpereReactive")]
        public IEnumerable<JsonSampleValue> VoltAmpereReactive { get; set; }

        [JsonPropertyName("VoltAmpereReactiveDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltAmpereReactiveDataSet { get; set; }


        [JsonPropertyName("Voltage")]
        public IEnumerable<JsonSampleValue> Voltage { get; set; }

        [JsonPropertyName("VoltageDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltageDataSet { get; set; }


        [JsonPropertyName("VoltageAC")]
        public IEnumerable<JsonSampleValue> VoltageAC { get; set; }

        [JsonPropertyName("VoltageACDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltageACDataSet { get; set; }


        [JsonPropertyName("VoltageDC")]
        public IEnumerable<JsonSampleValue> VoltageDC { get; set; }

        [JsonPropertyName("VoltageDCDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltageDCDataSet { get; set; }


        [JsonPropertyName("VolumeFluid")]
        public IEnumerable<JsonSampleValue> VolumeFluid { get; set; }

        [JsonPropertyName("VolumeFluidDataSet")]
        public IEnumerable<JsonSampleDataSet> VolumeFluidDataSet { get; set; }


        [JsonPropertyName("VolumeSpatial")]
        public IEnumerable<JsonSampleValue> VolumeSpatial { get; set; }

        [JsonPropertyName("VolumeSpatialDataSet")]
        public IEnumerable<JsonSampleDataSet> VolumeSpatialDataSet { get; set; }


        [JsonPropertyName("Wattage")]
        public IEnumerable<JsonSampleValue> Wattage { get; set; }

        [JsonPropertyName("WattageDataSet")]
        public IEnumerable<JsonSampleDataSet> WattageDataSet { get; set; }


        [JsonPropertyName("XDimension")]
        public IEnumerable<JsonSampleValue> XDimension { get; set; }

        [JsonPropertyName("XDimensionDataSet")]
        public IEnumerable<JsonSampleDataSet> XDimensionDataSet { get; set; }


        [JsonPropertyName("YDimension")]
        public IEnumerable<JsonSampleValue> YDimension { get; set; }

        [JsonPropertyName("YDimensionDataSet")]
        public IEnumerable<JsonSampleDataSet> YDimensionDataSet { get; set; }


        [JsonPropertyName("ZDimension")]
        public IEnumerable<JsonSampleValue> ZDimension { get; set; }

        [JsonPropertyName("ZDimensionDataSet")]
        public IEnumerable<JsonSampleDataSet> ZDimensionDataSet { get; set; }



        public JsonSamples() { }

        public JsonSamples(IEnumerable<IObservationOutput> observations)
        {
            if (observations != null)
            {
                if (!observations.IsNullOrEmpty())
                {
                    IEnumerable<IObservationOutput> typeObservations;
                    // Add Acceleration
                    typeObservations = observations.Where(o => o.Type == AccelerationDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Acceleration = jsonObservations;
                    }

                    // Add AccelerationDataSet
                    typeObservations = observations.Where(o => o.Type == AccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AccelerationDataSet = jsonObservations;
                    }


                    // Add AccumulatedTime
                    typeObservations = observations.Where(o => o.Type == AccumulatedTimeDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AccumulatedTime = jsonObservations;
                    }

                    // Add AccumulatedTimeDataSet
                    typeObservations = observations.Where(o => o.Type == AccumulatedTimeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AccumulatedTimeDataSet = jsonObservations;
                    }


                    // Add Amperage
                    typeObservations = observations.Where(o => o.Type == AmperageDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Amperage = jsonObservations;
                    }

                    // Add AmperageDataSet
                    typeObservations = observations.Where(o => o.Type == AmperageDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AmperageDataSet = jsonObservations;
                    }


                    // Add AmperageAC
                    typeObservations = observations.Where(o => o.Type == AmperageACDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AmperageAC = jsonObservations;
                    }

                    // Add AmperageACDataSet
                    typeObservations = observations.Where(o => o.Type == AmperageACDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AmperageACDataSet = jsonObservations;
                    }


                    // Add AmperageDC
                    typeObservations = observations.Where(o => o.Type == AmperageDCDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AmperageDC = jsonObservations;
                    }

                    // Add AmperageDCDataSet
                    typeObservations = observations.Where(o => o.Type == AmperageDCDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AmperageDCDataSet = jsonObservations;
                    }


                    // Add Angle
                    typeObservations = observations.Where(o => o.Type == AngleDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Angle = jsonObservations;
                    }

                    // Add AngleDataSet
                    typeObservations = observations.Where(o => o.Type == AngleDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AngleDataSet = jsonObservations;
                    }


                    // Add AngularAcceleration
                    typeObservations = observations.Where(o => o.Type == AngularAccelerationDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AngularAcceleration = jsonObservations;
                    }

                    // Add AngularAccelerationDataSet
                    typeObservations = observations.Where(o => o.Type == AngularAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AngularAccelerationDataSet = jsonObservations;
                    }


                    // Add AngularDeceleration
                    typeObservations = observations.Where(o => o.Type == AngularDecelerationDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AngularDeceleration = jsonObservations;
                    }

                    // Add AngularDecelerationDataSet
                    typeObservations = observations.Where(o => o.Type == AngularDecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AngularDecelerationDataSet = jsonObservations;
                    }


                    // Add AngularVelocity
                    typeObservations = observations.Where(o => o.Type == AngularVelocityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AngularVelocity = jsonObservations;
                    }

                    // Add AngularVelocityDataSet
                    typeObservations = observations.Where(o => o.Type == AngularVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AngularVelocityDataSet = jsonObservations;
                    }


                    // Add AssetUpdateRate
                    typeObservations = observations.Where(o => o.Type == AssetUpdateRateDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AssetUpdateRate = jsonObservations;
                    }

                    // Add AssetUpdateRateDataSet
                    typeObservations = observations.Where(o => o.Type == AssetUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AssetUpdateRateDataSet = jsonObservations;
                    }


                    // Add AxisFeedrate
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        AxisFeedrate = jsonObservations;
                    }

                    // Add AxisFeedrateDataSet
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        AxisFeedrateDataSet = jsonObservations;
                    }


                    // Add BatteryCapacity
                    typeObservations = observations.Where(o => o.Type == BatteryCapacityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        BatteryCapacity = jsonObservations;
                    }

                    // Add BatteryCapacityDataSet
                    typeObservations = observations.Where(o => o.Type == BatteryCapacityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        BatteryCapacityDataSet = jsonObservations;
                    }


                    // Add BatteryCharge
                    typeObservations = observations.Where(o => o.Type == BatteryChargeDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        BatteryCharge = jsonObservations;
                    }

                    // Add BatteryChargeDataSet
                    typeObservations = observations.Where(o => o.Type == BatteryChargeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        BatteryChargeDataSet = jsonObservations;
                    }


                    // Add CapacityFluid
                    typeObservations = observations.Where(o => o.Type == CapacityFluidDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        CapacityFluid = jsonObservations;
                    }

                    // Add CapacityFluidDataSet
                    typeObservations = observations.Where(o => o.Type == CapacityFluidDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        CapacityFluidDataSet = jsonObservations;
                    }


                    // Add CapacitySpatial
                    typeObservations = observations.Where(o => o.Type == CapacitySpatialDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        CapacitySpatial = jsonObservations;
                    }

                    // Add CapacitySpatialDataSet
                    typeObservations = observations.Where(o => o.Type == CapacitySpatialDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        CapacitySpatialDataSet = jsonObservations;
                    }


                    // Add ChargeRate
                    typeObservations = observations.Where(o => o.Type == ChargeRateDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        ChargeRate = jsonObservations;
                    }

                    // Add ChargeRateDataSet
                    typeObservations = observations.Where(o => o.Type == ChargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ChargeRateDataSet = jsonObservations;
                    }


                    // Add Concentration
                    typeObservations = observations.Where(o => o.Type == ConcentrationDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Concentration = jsonObservations;
                    }

                    // Add ConcentrationDataSet
                    typeObservations = observations.Where(o => o.Type == ConcentrationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ConcentrationDataSet = jsonObservations;
                    }


                    // Add Conductivity
                    typeObservations = observations.Where(o => o.Type == ConductivityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Conductivity = jsonObservations;
                    }

                    // Add ConductivityDataSet
                    typeObservations = observations.Where(o => o.Type == ConductivityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ConductivityDataSet = jsonObservations;
                    }


                    // Add CuttingSpeed
                    typeObservations = observations.Where(o => o.Type == CuttingSpeedDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        CuttingSpeed = jsonObservations;
                    }

                    // Add CuttingSpeedDataSet
                    typeObservations = observations.Where(o => o.Type == CuttingSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        CuttingSpeedDataSet = jsonObservations;
                    }


                    // Add Deceleration
                    typeObservations = observations.Where(o => o.Type == DecelerationDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Deceleration = jsonObservations;
                    }

                    // Add DecelerationDataSet
                    typeObservations = observations.Where(o => o.Type == DecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DecelerationDataSet = jsonObservations;
                    }


                    // Add Density
                    typeObservations = observations.Where(o => o.Type == DensityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Density = jsonObservations;
                    }

                    // Add DensityDataSet
                    typeObservations = observations.Where(o => o.Type == DensityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DensityDataSet = jsonObservations;
                    }


                    // Add DepositionAccelerationVolumetric
                    typeObservations = observations.Where(o => o.Type == DepositionAccelerationVolumetricDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DepositionAccelerationVolumetric = jsonObservations;
                    }

                    // Add DepositionAccelerationVolumetricDataSet
                    typeObservations = observations.Where(o => o.Type == DepositionAccelerationVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DepositionAccelerationVolumetricDataSet = jsonObservations;
                    }


                    // Add DepositionDensity
                    typeObservations = observations.Where(o => o.Type == DepositionDensityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DepositionDensity = jsonObservations;
                    }

                    // Add DepositionDensityDataSet
                    typeObservations = observations.Where(o => o.Type == DepositionDensityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DepositionDensityDataSet = jsonObservations;
                    }


                    // Add DepositionMass
                    typeObservations = observations.Where(o => o.Type == DepositionMassDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DepositionMass = jsonObservations;
                    }

                    // Add DepositionMassDataSet
                    typeObservations = observations.Where(o => o.Type == DepositionMassDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DepositionMassDataSet = jsonObservations;
                    }


                    // Add DepositionRateVolumetric
                    typeObservations = observations.Where(o => o.Type == DepositionRateVolumetricDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DepositionRateVolumetric = jsonObservations;
                    }

                    // Add DepositionRateVolumetricDataSet
                    typeObservations = observations.Where(o => o.Type == DepositionRateVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DepositionRateVolumetricDataSet = jsonObservations;
                    }


                    // Add DepositionVolume
                    typeObservations = observations.Where(o => o.Type == DepositionVolumeDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DepositionVolume = jsonObservations;
                    }

                    // Add DepositionVolumeDataSet
                    typeObservations = observations.Where(o => o.Type == DepositionVolumeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DepositionVolumeDataSet = jsonObservations;
                    }


                    // Add DewPoint
                    typeObservations = observations.Where(o => o.Type == DewPointDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DewPoint = jsonObservations;
                    }

                    // Add DewPointDataSet
                    typeObservations = observations.Where(o => o.Type == DewPointDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DewPointDataSet = jsonObservations;
                    }


                    // Add Diameter
                    typeObservations = observations.Where(o => o.Type == DiameterDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Diameter = jsonObservations;
                    }

                    // Add DiameterDataSet
                    typeObservations = observations.Where(o => o.Type == DiameterDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DiameterDataSet = jsonObservations;
                    }


                    // Add DischargeRate
                    typeObservations = observations.Where(o => o.Type == DischargeRateDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DischargeRate = jsonObservations;
                    }

                    // Add DischargeRateDataSet
                    typeObservations = observations.Where(o => o.Type == DischargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DischargeRateDataSet = jsonObservations;
                    }


                    // Add Displacement
                    typeObservations = observations.Where(o => o.Type == DisplacementDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Displacement = jsonObservations;
                    }

                    // Add DisplacementDataSet
                    typeObservations = observations.Where(o => o.Type == DisplacementDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DisplacementDataSet = jsonObservations;
                    }


                    // Add DisplacementAngular
                    typeObservations = observations.Where(o => o.Type == DisplacementAngularDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DisplacementAngular = jsonObservations;
                    }

                    // Add DisplacementAngularDataSet
                    typeObservations = observations.Where(o => o.Type == DisplacementAngularDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DisplacementAngularDataSet = jsonObservations;
                    }


                    // Add DisplacementLinear
                    typeObservations = observations.Where(o => o.Type == DisplacementLinearDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        DisplacementLinear = jsonObservations;
                    }

                    // Add DisplacementLinearDataSet
                    typeObservations = observations.Where(o => o.Type == DisplacementLinearDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        DisplacementLinearDataSet = jsonObservations;
                    }


                    // Add ElectricalEnergy
                    typeObservations = observations.Where(o => o.Type == ElectricalEnergyDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        ElectricalEnergy = jsonObservations;
                    }

                    // Add ElectricalEnergyDataSet
                    typeObservations = observations.Where(o => o.Type == ElectricalEnergyDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ElectricalEnergyDataSet = jsonObservations;
                    }


                    // Add EquipmentTimer
                    typeObservations = observations.Where(o => o.Type == EquipmentTimerDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        EquipmentTimer = jsonObservations;
                    }

                    // Add EquipmentTimerDataSet
                    typeObservations = observations.Where(o => o.Type == EquipmentTimerDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        EquipmentTimerDataSet = jsonObservations;
                    }


                    // Add FillLevel
                    typeObservations = observations.Where(o => o.Type == FillLevelDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        FillLevel = jsonObservations;
                    }

                    // Add FillLevelDataSet
                    typeObservations = observations.Where(o => o.Type == FillLevelDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        FillLevelDataSet = jsonObservations;
                    }


                    // Add Flow
                    typeObservations = observations.Where(o => o.Type == FlowDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Flow = jsonObservations;
                    }

                    // Add FlowDataSet
                    typeObservations = observations.Where(o => o.Type == FlowDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        FlowDataSet = jsonObservations;
                    }


                    // Add FollowingError
                    typeObservations = observations.Where(o => o.Type == FollowingErrorDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        FollowingError = jsonObservations;
                    }

                    // Add FollowingErrorDataSet
                    typeObservations = observations.Where(o => o.Type == FollowingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        FollowingErrorDataSet = jsonObservations;
                    }


                    // Add FollowingErrorAngular
                    typeObservations = observations.Where(o => o.Type == FollowingErrorAngularDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        FollowingErrorAngular = jsonObservations;
                    }

                    // Add FollowingErrorAngularDataSet
                    typeObservations = observations.Where(o => o.Type == FollowingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        FollowingErrorAngularDataSet = jsonObservations;
                    }


                    // Add FollowingErrorLinear
                    typeObservations = observations.Where(o => o.Type == FollowingErrorLinearDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        FollowingErrorLinear = jsonObservations;
                    }

                    // Add FollowingErrorLinearDataSet
                    typeObservations = observations.Where(o => o.Type == FollowingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        FollowingErrorLinearDataSet = jsonObservations;
                    }


                    // Add Frequency
                    typeObservations = observations.Where(o => o.Type == FrequencyDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Frequency = jsonObservations;
                    }

                    // Add FrequencyDataSet
                    typeObservations = observations.Where(o => o.Type == FrequencyDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        FrequencyDataSet = jsonObservations;
                    }


                    // Add GlobalPosition
                    typeObservations = observations.Where(o => o.Type == GlobalPositionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        GlobalPosition = jsonObservations;
                    }

                    // Add GlobalPositionDataSet
                    typeObservations = observations.Where(o => o.Type == GlobalPositionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        GlobalPositionDataSet = jsonObservations;
                    }


                    // Add GravitationalAcceleration
                    typeObservations = observations.Where(o => o.Type == GravitationalAccelerationDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        GravitationalAcceleration = jsonObservations;
                    }

                    // Add GravitationalAccelerationDataSet
                    typeObservations = observations.Where(o => o.Type == GravitationalAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        GravitationalAccelerationDataSet = jsonObservations;
                    }


                    // Add GravitationalForce
                    typeObservations = observations.Where(o => o.Type == GravitationalForceDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        GravitationalForce = jsonObservations;
                    }

                    // Add GravitationalForceDataSet
                    typeObservations = observations.Where(o => o.Type == GravitationalForceDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        GravitationalForceDataSet = jsonObservations;
                    }


                    // Add HumidityAbsolute
                    typeObservations = observations.Where(o => o.Type == HumidityAbsoluteDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        HumidityAbsolute = jsonObservations;
                    }

                    // Add HumidityAbsoluteDataSet
                    typeObservations = observations.Where(o => o.Type == HumidityAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        HumidityAbsoluteDataSet = jsonObservations;
                    }


                    // Add HumidityRelative
                    typeObservations = observations.Where(o => o.Type == HumidityRelativeDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        HumidityRelative = jsonObservations;
                    }

                    // Add HumidityRelativeDataSet
                    typeObservations = observations.Where(o => o.Type == HumidityRelativeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        HumidityRelativeDataSet = jsonObservations;
                    }


                    // Add HumiditySpecific
                    typeObservations = observations.Where(o => o.Type == HumiditySpecificDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        HumiditySpecific = jsonObservations;
                    }

                    // Add HumiditySpecificDataSet
                    typeObservations = observations.Where(o => o.Type == HumiditySpecificDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        HumiditySpecificDataSet = jsonObservations;
                    }


                    // Add Length
                    typeObservations = observations.Where(o => o.Type == LengthDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Length = jsonObservations;
                    }

                    // Add LengthDataSet
                    typeObservations = observations.Where(o => o.Type == LengthDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        LengthDataSet = jsonObservations;
                    }


                    // Add Level
                    typeObservations = observations.Where(o => o.Type == LevelDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Level = jsonObservations;
                    }

                    // Add LevelDataSet
                    typeObservations = observations.Where(o => o.Type == LevelDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        LevelDataSet = jsonObservations;
                    }


                    // Add LinearForce
                    typeObservations = observations.Where(o => o.Type == LinearForceDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        LinearForce = jsonObservations;
                    }

                    // Add LinearForceDataSet
                    typeObservations = observations.Where(o => o.Type == LinearForceDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        LinearForceDataSet = jsonObservations;
                    }


                    // Add Load
                    typeObservations = observations.Where(o => o.Type == LoadDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Load = jsonObservations;
                    }

                    // Add LoadDataSet
                    typeObservations = observations.Where(o => o.Type == LoadDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        LoadDataSet = jsonObservations;
                    }


                    // Add Mass
                    typeObservations = observations.Where(o => o.Type == MassDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Mass = jsonObservations;
                    }

                    // Add MassDataSet
                    typeObservations = observations.Where(o => o.Type == MassDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        MassDataSet = jsonObservations;
                    }


                    // Add ObservationUpdateRate
                    typeObservations = observations.Where(o => o.Type == ObservationUpdateRateDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        ObservationUpdateRate = jsonObservations;
                    }

                    // Add ObservationUpdateRateDataSet
                    typeObservations = observations.Where(o => o.Type == ObservationUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ObservationUpdateRateDataSet = jsonObservations;
                    }


                    // Add Openness
                    typeObservations = observations.Where(o => o.Type == OpennessDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Openness = jsonObservations;
                    }

                    // Add OpennessDataSet
                    typeObservations = observations.Where(o => o.Type == OpennessDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        OpennessDataSet = jsonObservations;
                    }


                    // Add Orientation
                    typeObservations = observations.Where(o => o.Type == OrientationDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Orientation = jsonObservations;
                    }

                    // Add OrientationDataSet
                    typeObservations = observations.Where(o => o.Type == OrientationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        OrientationDataSet = jsonObservations;
                    }


                    // Add PathFeedrate
                    typeObservations = observations.Where(o => o.Type == PathFeedrateDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PathFeedrate = jsonObservations;
                    }

                    // Add PathFeedrateDataSet
                    typeObservations = observations.Where(o => o.Type == PathFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PathFeedrateDataSet = jsonObservations;
                    }


                    // Add PathFeedratePerRevolution
                    typeObservations = observations.Where(o => o.Type == PathFeedratePerRevolutionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PathFeedratePerRevolution = jsonObservations;
                    }

                    // Add PathFeedratePerRevolutionDataSet
                    typeObservations = observations.Where(o => o.Type == PathFeedratePerRevolutionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PathFeedratePerRevolutionDataSet = jsonObservations;
                    }


                    // Add PathPosition
                    typeObservations = observations.Where(o => o.Type == PathPositionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PathPosition = jsonObservations;
                    }

                    // Add PathPositionDataSet
                    typeObservations = observations.Where(o => o.Type == PathPositionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PathPositionDataSet = jsonObservations;
                    }


                    // Add PH
                    typeObservations = observations.Where(o => o.Type == PHDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PH = jsonObservations;
                    }

                    // Add PHDataSet
                    typeObservations = observations.Where(o => o.Type == PHDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PHDataSet = jsonObservations;
                    }


                    // Add Position
                    typeObservations = observations.Where(o => o.Type == PositionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Position = jsonObservations;
                    }

                    // Add PositionDataSet
                    typeObservations = observations.Where(o => o.Type == PositionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PositionDataSet = jsonObservations;
                    }


                    // Add PositionCartesian
                    typeObservations = observations.Where(o => o.Type == PositionCartesianDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PositionCartesian = jsonObservations;
                    }

                    // Add PositionCartesianDataSet
                    typeObservations = observations.Where(o => o.Type == PositionCartesianDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PositionCartesianDataSet = jsonObservations;
                    }


                    // Add PowerFactor
                    typeObservations = observations.Where(o => o.Type == PowerFactorDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PowerFactor = jsonObservations;
                    }

                    // Add PowerFactorDataSet
                    typeObservations = observations.Where(o => o.Type == PowerFactorDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PowerFactorDataSet = jsonObservations;
                    }


                    // Add Pressure
                    typeObservations = observations.Where(o => o.Type == PressureDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Pressure = jsonObservations;
                    }

                    // Add PressureDataSet
                    typeObservations = observations.Where(o => o.Type == PressureDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PressureDataSet = jsonObservations;
                    }


                    // Add PressureAbsolute
                    typeObservations = observations.Where(o => o.Type == PressureAbsoluteDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PressureAbsolute = jsonObservations;
                    }

                    // Add PressureAbsoluteDataSet
                    typeObservations = observations.Where(o => o.Type == PressureAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PressureAbsoluteDataSet = jsonObservations;
                    }


                    // Add PressurizationRate
                    typeObservations = observations.Where(o => o.Type == PressurizationRateDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        PressurizationRate = jsonObservations;
                    }

                    // Add PressurizationRateDataSet
                    typeObservations = observations.Where(o => o.Type == PressurizationRateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        PressurizationRateDataSet = jsonObservations;
                    }


                    // Add ProcessTimer
                    typeObservations = observations.Where(o => o.Type == ProcessTimerDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        ProcessTimer = jsonObservations;
                    }

                    // Add ProcessTimerDataSet
                    typeObservations = observations.Where(o => o.Type == ProcessTimerDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ProcessTimerDataSet = jsonObservations;
                    }


                    // Add Resistance
                    typeObservations = observations.Where(o => o.Type == ResistanceDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Resistance = jsonObservations;
                    }

                    // Add ResistanceDataSet
                    typeObservations = observations.Where(o => o.Type == ResistanceDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ResistanceDataSet = jsonObservations;
                    }


                    // Add RotaryVelocity
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        RotaryVelocity = jsonObservations;
                    }

                    // Add RotaryVelocityDataSet
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        RotaryVelocityDataSet = jsonObservations;
                    }


                    // Add SettlingError
                    typeObservations = observations.Where(o => o.Type == SettlingErrorDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        SettlingError = jsonObservations;
                    }

                    // Add SettlingErrorDataSet
                    typeObservations = observations.Where(o => o.Type == SettlingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        SettlingErrorDataSet = jsonObservations;
                    }


                    // Add SettlingErrorAngular
                    typeObservations = observations.Where(o => o.Type == SettlingErrorAngularDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        SettlingErrorAngular = jsonObservations;
                    }

                    // Add SettlingErrorAngularDataSet
                    typeObservations = observations.Where(o => o.Type == SettlingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        SettlingErrorAngularDataSet = jsonObservations;
                    }


                    // Add SettlingErrorLinear
                    typeObservations = observations.Where(o => o.Type == SettlingErrorLinearDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        SettlingErrorLinear = jsonObservations;
                    }

                    // Add SettlingErrorLinearDataSet
                    typeObservations = observations.Where(o => o.Type == SettlingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        SettlingErrorLinearDataSet = jsonObservations;
                    }


                    // Add SoundLevel
                    typeObservations = observations.Where(o => o.Type == SoundLevelDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        SoundLevel = jsonObservations;
                    }

                    // Add SoundLevelDataSet
                    typeObservations = observations.Where(o => o.Type == SoundLevelDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        SoundLevelDataSet = jsonObservations;
                    }


                    // Add SpindleSpeed
                    typeObservations = observations.Where(o => o.Type == SpindleSpeedDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        SpindleSpeed = jsonObservations;
                    }

                    // Add SpindleSpeedDataSet
                    typeObservations = observations.Where(o => o.Type == SpindleSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        SpindleSpeedDataSet = jsonObservations;
                    }


                    // Add Strain
                    typeObservations = observations.Where(o => o.Type == StrainDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Strain = jsonObservations;
                    }

                    // Add StrainDataSet
                    typeObservations = observations.Where(o => o.Type == StrainDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        StrainDataSet = jsonObservations;
                    }


                    // Add Temperature
                    typeObservations = observations.Where(o => o.Type == TemperatureDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Temperature = jsonObservations;
                    }

                    // Add TemperatureDataSet
                    typeObservations = observations.Where(o => o.Type == TemperatureDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        TemperatureDataSet = jsonObservations;
                    }


                    // Add Tension
                    typeObservations = observations.Where(o => o.Type == TensionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Tension = jsonObservations;
                    }

                    // Add TensionDataSet
                    typeObservations = observations.Where(o => o.Type == TensionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        TensionDataSet = jsonObservations;
                    }


                    // Add Tilt
                    typeObservations = observations.Where(o => o.Type == TiltDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Tilt = jsonObservations;
                    }

                    // Add TiltDataSet
                    typeObservations = observations.Where(o => o.Type == TiltDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        TiltDataSet = jsonObservations;
                    }


                    // Add Torque
                    typeObservations = observations.Where(o => o.Type == TorqueDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Torque = jsonObservations;
                    }

                    // Add TorqueDataSet
                    typeObservations = observations.Where(o => o.Type == TorqueDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        TorqueDataSet = jsonObservations;
                    }


                    // Add Velocity
                    typeObservations = observations.Where(o => o.Type == VelocityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Velocity = jsonObservations;
                    }

                    // Add VelocityDataSet
                    typeObservations = observations.Where(o => o.Type == VelocityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VelocityDataSet = jsonObservations;
                    }


                    // Add Viscosity
                    typeObservations = observations.Where(o => o.Type == ViscosityDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Viscosity = jsonObservations;
                    }

                    // Add ViscosityDataSet
                    typeObservations = observations.Where(o => o.Type == ViscosityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ViscosityDataSet = jsonObservations;
                    }


                    // Add VoltAmpere
                    typeObservations = observations.Where(o => o.Type == VoltAmpereDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        VoltAmpere = jsonObservations;
                    }

                    // Add VoltAmpereDataSet
                    typeObservations = observations.Where(o => o.Type == VoltAmpereDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VoltAmpereDataSet = jsonObservations;
                    }


                    // Add VoltAmpereReactive
                    typeObservations = observations.Where(o => o.Type == VoltAmpereReactiveDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        VoltAmpereReactive = jsonObservations;
                    }

                    // Add VoltAmpereReactiveDataSet
                    typeObservations = observations.Where(o => o.Type == VoltAmpereReactiveDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VoltAmpereReactiveDataSet = jsonObservations;
                    }


                    // Add Voltage
                    typeObservations = observations.Where(o => o.Type == VoltageDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Voltage = jsonObservations;
                    }

                    // Add VoltageDataSet
                    typeObservations = observations.Where(o => o.Type == VoltageDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VoltageDataSet = jsonObservations;
                    }


                    // Add VoltageAC
                    typeObservations = observations.Where(o => o.Type == VoltageACDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        VoltageAC = jsonObservations;
                    }

                    // Add VoltageACDataSet
                    typeObservations = observations.Where(o => o.Type == VoltageACDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VoltageACDataSet = jsonObservations;
                    }


                    // Add VoltageDC
                    typeObservations = observations.Where(o => o.Type == VoltageDCDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        VoltageDC = jsonObservations;
                    }

                    // Add VoltageDCDataSet
                    typeObservations = observations.Where(o => o.Type == VoltageDCDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VoltageDCDataSet = jsonObservations;
                    }


                    // Add VolumeFluid
                    typeObservations = observations.Where(o => o.Type == VolumeFluidDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        VolumeFluid = jsonObservations;
                    }

                    // Add VolumeFluidDataSet
                    typeObservations = observations.Where(o => o.Type == VolumeFluidDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VolumeFluidDataSet = jsonObservations;
                    }


                    // Add VolumeSpatial
                    typeObservations = observations.Where(o => o.Type == VolumeSpatialDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        VolumeSpatial = jsonObservations;
                    }

                    // Add VolumeSpatialDataSet
                    typeObservations = observations.Where(o => o.Type == VolumeSpatialDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        VolumeSpatialDataSet = jsonObservations;
                    }


                    // Add Wattage
                    typeObservations = observations.Where(o => o.Type == WattageDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Wattage = jsonObservations;
                    }

                    // Add WattageDataSet
                    typeObservations = observations.Where(o => o.Type == WattageDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        WattageDataSet = jsonObservations;
                    }


                    // Add XDimension
                    typeObservations = observations.Where(o => o.Type == XDimensionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        XDimension = jsonObservations;
                    }

                    // Add XDimensionDataSet
                    typeObservations = observations.Where(o => o.Type == XDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        XDimensionDataSet = jsonObservations;
                    }


                    // Add YDimension
                    typeObservations = observations.Where(o => o.Type == YDimensionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        YDimension = jsonObservations;
                    }

                    // Add YDimensionDataSet
                    typeObservations = observations.Where(o => o.Type == YDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        YDimensionDataSet = jsonObservations;
                    }


                    // Add ZDimension
                    typeObservations = observations.Where(o => o.Type == ZDimensionDataItem.TypeId);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        ZDimension = jsonObservations;
                    }

                    // Add ZDimensionDataSet
                    typeObservations = observations.Where(o => o.Type == ZDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ZDimensionDataSet = jsonObservations;
                    }


                }
            }
        }
    }  
}