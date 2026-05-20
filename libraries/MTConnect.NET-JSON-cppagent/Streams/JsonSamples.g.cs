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
    /// <summary>
    /// cppagent-style JSON representation of the SAMPLE-category observations in a
    /// stream. Every MTConnect sample type is exposed as four typed collections —
    /// one each for the VALUE, DATA_SET, TABLE, and TIME_SERIES representations — so
    /// the serialized object shape matches the C++ reference agent's output.
    /// </summary>
    public class JsonSamples
    {
        /// <summary>
        /// Materializes every typed collection into a single flat list of
        /// <see cref="IObservation"/>, restoring each observation's MTConnect type.
        /// </summary>
        [JsonIgnore]
        public List<IObservation> Observations
        {
            get
            {
                var l = new List<IObservation>();
                if (!Acceleration.IsNullOrEmpty()) foreach (var x in Acceleration) l.Add(x.ToObservation(AccelerationDataItem.TypeId));
                if (!AccelerationDataSet.IsNullOrEmpty()) foreach (var x in AccelerationDataSet) l.Add(x.ToObservation(AccelerationDataItem.TypeId));
                if (!AccelerationTable.IsNullOrEmpty()) foreach (var x in AccelerationTable) l.Add(x.ToObservation(AccelerationDataItem.TypeId));
                if (!AccelerationTimeSeries.IsNullOrEmpty()) foreach (var x in AccelerationTimeSeries) l.Add(x.ToObservation(AccelerationDataItem.TypeId));

                if (!AccumulatedTime.IsNullOrEmpty()) foreach (var x in AccumulatedTime) l.Add(x.ToObservation(AccumulatedTimeDataItem.TypeId));
                if (!AccumulatedTimeDataSet.IsNullOrEmpty()) foreach (var x in AccumulatedTimeDataSet) l.Add(x.ToObservation(AccumulatedTimeDataItem.TypeId));
                if (!AccumulatedTimeTable.IsNullOrEmpty()) foreach (var x in AccumulatedTimeTable) l.Add(x.ToObservation(AccumulatedTimeDataItem.TypeId));
                if (!AccumulatedTimeTimeSeries.IsNullOrEmpty()) foreach (var x in AccumulatedTimeTimeSeries) l.Add(x.ToObservation(AccumulatedTimeDataItem.TypeId));

                if (!Amperage.IsNullOrEmpty()) foreach (var x in Amperage) l.Add(x.ToObservation(AmperageDataItem.TypeId));
                if (!AmperageDataSet.IsNullOrEmpty()) foreach (var x in AmperageDataSet) l.Add(x.ToObservation(AmperageDataItem.TypeId));
                if (!AmperageTable.IsNullOrEmpty()) foreach (var x in AmperageTable) l.Add(x.ToObservation(AmperageDataItem.TypeId));
                if (!AmperageTimeSeries.IsNullOrEmpty()) foreach (var x in AmperageTimeSeries) l.Add(x.ToObservation(AmperageDataItem.TypeId));

                if (!AmperageAC.IsNullOrEmpty()) foreach (var x in AmperageAC) l.Add(x.ToObservation(AmperageACDataItem.TypeId));
                if (!AmperageACDataSet.IsNullOrEmpty()) foreach (var x in AmperageACDataSet) l.Add(x.ToObservation(AmperageACDataItem.TypeId));
                if (!AmperageACTable.IsNullOrEmpty()) foreach (var x in AmperageACTable) l.Add(x.ToObservation(AmperageACDataItem.TypeId));
                if (!AmperageACTimeSeries.IsNullOrEmpty()) foreach (var x in AmperageACTimeSeries) l.Add(x.ToObservation(AmperageACDataItem.TypeId));

                if (!AmperageDC.IsNullOrEmpty()) foreach (var x in AmperageDC) l.Add(x.ToObservation(AmperageDCDataItem.TypeId));
                if (!AmperageDCDataSet.IsNullOrEmpty()) foreach (var x in AmperageDCDataSet) l.Add(x.ToObservation(AmperageDCDataItem.TypeId));
                if (!AmperageDCTable.IsNullOrEmpty()) foreach (var x in AmperageDCTable) l.Add(x.ToObservation(AmperageDCDataItem.TypeId));
                if (!AmperageDCTimeSeries.IsNullOrEmpty()) foreach (var x in AmperageDCTimeSeries) l.Add(x.ToObservation(AmperageDCDataItem.TypeId));

                if (!Angle.IsNullOrEmpty()) foreach (var x in Angle) l.Add(x.ToObservation(AngleDataItem.TypeId));
                if (!AngleDataSet.IsNullOrEmpty()) foreach (var x in AngleDataSet) l.Add(x.ToObservation(AngleDataItem.TypeId));
                if (!AngleTable.IsNullOrEmpty()) foreach (var x in AngleTable) l.Add(x.ToObservation(AngleDataItem.TypeId));
                if (!AngleTimeSeries.IsNullOrEmpty()) foreach (var x in AngleTimeSeries) l.Add(x.ToObservation(AngleDataItem.TypeId));

                if (!AngularAcceleration.IsNullOrEmpty()) foreach (var x in AngularAcceleration) l.Add(x.ToObservation(AngularAccelerationDataItem.TypeId));
                if (!AngularAccelerationDataSet.IsNullOrEmpty()) foreach (var x in AngularAccelerationDataSet) l.Add(x.ToObservation(AngularAccelerationDataItem.TypeId));
                if (!AngularAccelerationTable.IsNullOrEmpty()) foreach (var x in AngularAccelerationTable) l.Add(x.ToObservation(AngularAccelerationDataItem.TypeId));
                if (!AngularAccelerationTimeSeries.IsNullOrEmpty()) foreach (var x in AngularAccelerationTimeSeries) l.Add(x.ToObservation(AngularAccelerationDataItem.TypeId));

                if (!AngularDeceleration.IsNullOrEmpty()) foreach (var x in AngularDeceleration) l.Add(x.ToObservation(AngularDecelerationDataItem.TypeId));
                if (!AngularDecelerationDataSet.IsNullOrEmpty()) foreach (var x in AngularDecelerationDataSet) l.Add(x.ToObservation(AngularDecelerationDataItem.TypeId));
                if (!AngularDecelerationTable.IsNullOrEmpty()) foreach (var x in AngularDecelerationTable) l.Add(x.ToObservation(AngularDecelerationDataItem.TypeId));
                if (!AngularDecelerationTimeSeries.IsNullOrEmpty()) foreach (var x in AngularDecelerationTimeSeries) l.Add(x.ToObservation(AngularDecelerationDataItem.TypeId));

                if (!AngularVelocity.IsNullOrEmpty()) foreach (var x in AngularVelocity) l.Add(x.ToObservation(AngularVelocityDataItem.TypeId));
                if (!AngularVelocityDataSet.IsNullOrEmpty()) foreach (var x in AngularVelocityDataSet) l.Add(x.ToObservation(AngularVelocityDataItem.TypeId));
                if (!AngularVelocityTable.IsNullOrEmpty()) foreach (var x in AngularVelocityTable) l.Add(x.ToObservation(AngularVelocityDataItem.TypeId));
                if (!AngularVelocityTimeSeries.IsNullOrEmpty()) foreach (var x in AngularVelocityTimeSeries) l.Add(x.ToObservation(AngularVelocityDataItem.TypeId));

                if (!AssetUpdateRate.IsNullOrEmpty()) foreach (var x in AssetUpdateRate) l.Add(x.ToObservation(AssetUpdateRateDataItem.TypeId));
                if (!AssetUpdateRateDataSet.IsNullOrEmpty()) foreach (var x in AssetUpdateRateDataSet) l.Add(x.ToObservation(AssetUpdateRateDataItem.TypeId));
                if (!AssetUpdateRateTable.IsNullOrEmpty()) foreach (var x in AssetUpdateRateTable) l.Add(x.ToObservation(AssetUpdateRateDataItem.TypeId));
                if (!AssetUpdateRateTimeSeries.IsNullOrEmpty()) foreach (var x in AssetUpdateRateTimeSeries) l.Add(x.ToObservation(AssetUpdateRateDataItem.TypeId));

                if (!AxisFeedrate.IsNullOrEmpty()) foreach (var x in AxisFeedrate) l.Add(x.ToObservation(AxisFeedrateDataItem.TypeId));
                if (!AxisFeedrateDataSet.IsNullOrEmpty()) foreach (var x in AxisFeedrateDataSet) l.Add(x.ToObservation(AxisFeedrateDataItem.TypeId));
                if (!AxisFeedrateTable.IsNullOrEmpty()) foreach (var x in AxisFeedrateTable) l.Add(x.ToObservation(AxisFeedrateDataItem.TypeId));
                if (!AxisFeedrateTimeSeries.IsNullOrEmpty()) foreach (var x in AxisFeedrateTimeSeries) l.Add(x.ToObservation(AxisFeedrateDataItem.TypeId));

                if (!BatteryCapacity.IsNullOrEmpty()) foreach (var x in BatteryCapacity) l.Add(x.ToObservation(BatteryCapacityDataItem.TypeId));
                if (!BatteryCapacityDataSet.IsNullOrEmpty()) foreach (var x in BatteryCapacityDataSet) l.Add(x.ToObservation(BatteryCapacityDataItem.TypeId));
                if (!BatteryCapacityTable.IsNullOrEmpty()) foreach (var x in BatteryCapacityTable) l.Add(x.ToObservation(BatteryCapacityDataItem.TypeId));
                if (!BatteryCapacityTimeSeries.IsNullOrEmpty()) foreach (var x in BatteryCapacityTimeSeries) l.Add(x.ToObservation(BatteryCapacityDataItem.TypeId));

                if (!BatteryCharge.IsNullOrEmpty()) foreach (var x in BatteryCharge) l.Add(x.ToObservation(BatteryChargeDataItem.TypeId));
                if (!BatteryChargeDataSet.IsNullOrEmpty()) foreach (var x in BatteryChargeDataSet) l.Add(x.ToObservation(BatteryChargeDataItem.TypeId));
                if (!BatteryChargeTable.IsNullOrEmpty()) foreach (var x in BatteryChargeTable) l.Add(x.ToObservation(BatteryChargeDataItem.TypeId));
                if (!BatteryChargeTimeSeries.IsNullOrEmpty()) foreach (var x in BatteryChargeTimeSeries) l.Add(x.ToObservation(BatteryChargeDataItem.TypeId));

                if (!CapacityFluid.IsNullOrEmpty()) foreach (var x in CapacityFluid) l.Add(x.ToObservation(CapacityFluidDataItem.TypeId));
                if (!CapacityFluidDataSet.IsNullOrEmpty()) foreach (var x in CapacityFluidDataSet) l.Add(x.ToObservation(CapacityFluidDataItem.TypeId));
                if (!CapacityFluidTable.IsNullOrEmpty()) foreach (var x in CapacityFluidTable) l.Add(x.ToObservation(CapacityFluidDataItem.TypeId));
                if (!CapacityFluidTimeSeries.IsNullOrEmpty()) foreach (var x in CapacityFluidTimeSeries) l.Add(x.ToObservation(CapacityFluidDataItem.TypeId));

                if (!CapacitySpatial.IsNullOrEmpty()) foreach (var x in CapacitySpatial) l.Add(x.ToObservation(CapacitySpatialDataItem.TypeId));
                if (!CapacitySpatialDataSet.IsNullOrEmpty()) foreach (var x in CapacitySpatialDataSet) l.Add(x.ToObservation(CapacitySpatialDataItem.TypeId));
                if (!CapacitySpatialTable.IsNullOrEmpty()) foreach (var x in CapacitySpatialTable) l.Add(x.ToObservation(CapacitySpatialDataItem.TypeId));
                if (!CapacitySpatialTimeSeries.IsNullOrEmpty()) foreach (var x in CapacitySpatialTimeSeries) l.Add(x.ToObservation(CapacitySpatialDataItem.TypeId));

                if (!ChargeRate.IsNullOrEmpty()) foreach (var x in ChargeRate) l.Add(x.ToObservation(ChargeRateDataItem.TypeId));
                if (!ChargeRateDataSet.IsNullOrEmpty()) foreach (var x in ChargeRateDataSet) l.Add(x.ToObservation(ChargeRateDataItem.TypeId));
                if (!ChargeRateTable.IsNullOrEmpty()) foreach (var x in ChargeRateTable) l.Add(x.ToObservation(ChargeRateDataItem.TypeId));
                if (!ChargeRateTimeSeries.IsNullOrEmpty()) foreach (var x in ChargeRateTimeSeries) l.Add(x.ToObservation(ChargeRateDataItem.TypeId));

                if (!Concentration.IsNullOrEmpty()) foreach (var x in Concentration) l.Add(x.ToObservation(ConcentrationDataItem.TypeId));
                if (!ConcentrationDataSet.IsNullOrEmpty()) foreach (var x in ConcentrationDataSet) l.Add(x.ToObservation(ConcentrationDataItem.TypeId));
                if (!ConcentrationTable.IsNullOrEmpty()) foreach (var x in ConcentrationTable) l.Add(x.ToObservation(ConcentrationDataItem.TypeId));
                if (!ConcentrationTimeSeries.IsNullOrEmpty()) foreach (var x in ConcentrationTimeSeries) l.Add(x.ToObservation(ConcentrationDataItem.TypeId));

                if (!Conductivity.IsNullOrEmpty()) foreach (var x in Conductivity) l.Add(x.ToObservation(ConductivityDataItem.TypeId));
                if (!ConductivityDataSet.IsNullOrEmpty()) foreach (var x in ConductivityDataSet) l.Add(x.ToObservation(ConductivityDataItem.TypeId));
                if (!ConductivityTable.IsNullOrEmpty()) foreach (var x in ConductivityTable) l.Add(x.ToObservation(ConductivityDataItem.TypeId));
                if (!ConductivityTimeSeries.IsNullOrEmpty()) foreach (var x in ConductivityTimeSeries) l.Add(x.ToObservation(ConductivityDataItem.TypeId));

                if (!CuttingSpeed.IsNullOrEmpty()) foreach (var x in CuttingSpeed) l.Add(x.ToObservation(CuttingSpeedDataItem.TypeId));
                if (!CuttingSpeedDataSet.IsNullOrEmpty()) foreach (var x in CuttingSpeedDataSet) l.Add(x.ToObservation(CuttingSpeedDataItem.TypeId));
                if (!CuttingSpeedTable.IsNullOrEmpty()) foreach (var x in CuttingSpeedTable) l.Add(x.ToObservation(CuttingSpeedDataItem.TypeId));
                if (!CuttingSpeedTimeSeries.IsNullOrEmpty()) foreach (var x in CuttingSpeedTimeSeries) l.Add(x.ToObservation(CuttingSpeedDataItem.TypeId));

                if (!Deceleration.IsNullOrEmpty()) foreach (var x in Deceleration) l.Add(x.ToObservation(DecelerationDataItem.TypeId));
                if (!DecelerationDataSet.IsNullOrEmpty()) foreach (var x in DecelerationDataSet) l.Add(x.ToObservation(DecelerationDataItem.TypeId));
                if (!DecelerationTable.IsNullOrEmpty()) foreach (var x in DecelerationTable) l.Add(x.ToObservation(DecelerationDataItem.TypeId));
                if (!DecelerationTimeSeries.IsNullOrEmpty()) foreach (var x in DecelerationTimeSeries) l.Add(x.ToObservation(DecelerationDataItem.TypeId));

                if (!Density.IsNullOrEmpty()) foreach (var x in Density) l.Add(x.ToObservation(DensityDataItem.TypeId));
                if (!DensityDataSet.IsNullOrEmpty()) foreach (var x in DensityDataSet) l.Add(x.ToObservation(DensityDataItem.TypeId));
                if (!DensityTable.IsNullOrEmpty()) foreach (var x in DensityTable) l.Add(x.ToObservation(DensityDataItem.TypeId));
                if (!DensityTimeSeries.IsNullOrEmpty()) foreach (var x in DensityTimeSeries) l.Add(x.ToObservation(DensityDataItem.TypeId));

                if (!DepositionAccelerationVolumetric.IsNullOrEmpty()) foreach (var x in DepositionAccelerationVolumetric) l.Add(x.ToObservation(DepositionAccelerationVolumetricDataItem.TypeId));
                if (!DepositionAccelerationVolumetricDataSet.IsNullOrEmpty()) foreach (var x in DepositionAccelerationVolumetricDataSet) l.Add(x.ToObservation(DepositionAccelerationVolumetricDataItem.TypeId));
                if (!DepositionAccelerationVolumetricTable.IsNullOrEmpty()) foreach (var x in DepositionAccelerationVolumetricTable) l.Add(x.ToObservation(DepositionAccelerationVolumetricDataItem.TypeId));
                if (!DepositionAccelerationVolumetricTimeSeries.IsNullOrEmpty()) foreach (var x in DepositionAccelerationVolumetricTimeSeries) l.Add(x.ToObservation(DepositionAccelerationVolumetricDataItem.TypeId));

                if (!DepositionDensity.IsNullOrEmpty()) foreach (var x in DepositionDensity) l.Add(x.ToObservation(DepositionDensityDataItem.TypeId));
                if (!DepositionDensityDataSet.IsNullOrEmpty()) foreach (var x in DepositionDensityDataSet) l.Add(x.ToObservation(DepositionDensityDataItem.TypeId));
                if (!DepositionDensityTable.IsNullOrEmpty()) foreach (var x in DepositionDensityTable) l.Add(x.ToObservation(DepositionDensityDataItem.TypeId));
                if (!DepositionDensityTimeSeries.IsNullOrEmpty()) foreach (var x in DepositionDensityTimeSeries) l.Add(x.ToObservation(DepositionDensityDataItem.TypeId));

                if (!DepositionMass.IsNullOrEmpty()) foreach (var x in DepositionMass) l.Add(x.ToObservation(DepositionMassDataItem.TypeId));
                if (!DepositionMassDataSet.IsNullOrEmpty()) foreach (var x in DepositionMassDataSet) l.Add(x.ToObservation(DepositionMassDataItem.TypeId));
                if (!DepositionMassTable.IsNullOrEmpty()) foreach (var x in DepositionMassTable) l.Add(x.ToObservation(DepositionMassDataItem.TypeId));
                if (!DepositionMassTimeSeries.IsNullOrEmpty()) foreach (var x in DepositionMassTimeSeries) l.Add(x.ToObservation(DepositionMassDataItem.TypeId));

                if (!DepositionRateVolumetric.IsNullOrEmpty()) foreach (var x in DepositionRateVolumetric) l.Add(x.ToObservation(DepositionRateVolumetricDataItem.TypeId));
                if (!DepositionRateVolumetricDataSet.IsNullOrEmpty()) foreach (var x in DepositionRateVolumetricDataSet) l.Add(x.ToObservation(DepositionRateVolumetricDataItem.TypeId));
                if (!DepositionRateVolumetricTable.IsNullOrEmpty()) foreach (var x in DepositionRateVolumetricTable) l.Add(x.ToObservation(DepositionRateVolumetricDataItem.TypeId));
                if (!DepositionRateVolumetricTimeSeries.IsNullOrEmpty()) foreach (var x in DepositionRateVolumetricTimeSeries) l.Add(x.ToObservation(DepositionRateVolumetricDataItem.TypeId));

                if (!DepositionVolume.IsNullOrEmpty()) foreach (var x in DepositionVolume) l.Add(x.ToObservation(DepositionVolumeDataItem.TypeId));
                if (!DepositionVolumeDataSet.IsNullOrEmpty()) foreach (var x in DepositionVolumeDataSet) l.Add(x.ToObservation(DepositionVolumeDataItem.TypeId));
                if (!DepositionVolumeTable.IsNullOrEmpty()) foreach (var x in DepositionVolumeTable) l.Add(x.ToObservation(DepositionVolumeDataItem.TypeId));
                if (!DepositionVolumeTimeSeries.IsNullOrEmpty()) foreach (var x in DepositionVolumeTimeSeries) l.Add(x.ToObservation(DepositionVolumeDataItem.TypeId));

                if (!DewPoint.IsNullOrEmpty()) foreach (var x in DewPoint) l.Add(x.ToObservation(DewPointDataItem.TypeId));
                if (!DewPointDataSet.IsNullOrEmpty()) foreach (var x in DewPointDataSet) l.Add(x.ToObservation(DewPointDataItem.TypeId));
                if (!DewPointTable.IsNullOrEmpty()) foreach (var x in DewPointTable) l.Add(x.ToObservation(DewPointDataItem.TypeId));
                if (!DewPointTimeSeries.IsNullOrEmpty()) foreach (var x in DewPointTimeSeries) l.Add(x.ToObservation(DewPointDataItem.TypeId));

                if (!Diameter.IsNullOrEmpty()) foreach (var x in Diameter) l.Add(x.ToObservation(DiameterDataItem.TypeId));
                if (!DiameterDataSet.IsNullOrEmpty()) foreach (var x in DiameterDataSet) l.Add(x.ToObservation(DiameterDataItem.TypeId));
                if (!DiameterTable.IsNullOrEmpty()) foreach (var x in DiameterTable) l.Add(x.ToObservation(DiameterDataItem.TypeId));
                if (!DiameterTimeSeries.IsNullOrEmpty()) foreach (var x in DiameterTimeSeries) l.Add(x.ToObservation(DiameterDataItem.TypeId));

                if (!DischargeRate.IsNullOrEmpty()) foreach (var x in DischargeRate) l.Add(x.ToObservation(DischargeRateDataItem.TypeId));
                if (!DischargeRateDataSet.IsNullOrEmpty()) foreach (var x in DischargeRateDataSet) l.Add(x.ToObservation(DischargeRateDataItem.TypeId));
                if (!DischargeRateTable.IsNullOrEmpty()) foreach (var x in DischargeRateTable) l.Add(x.ToObservation(DischargeRateDataItem.TypeId));
                if (!DischargeRateTimeSeries.IsNullOrEmpty()) foreach (var x in DischargeRateTimeSeries) l.Add(x.ToObservation(DischargeRateDataItem.TypeId));

                if (!Displacement.IsNullOrEmpty()) foreach (var x in Displacement) l.Add(x.ToObservation(DisplacementDataItem.TypeId));
                if (!DisplacementDataSet.IsNullOrEmpty()) foreach (var x in DisplacementDataSet) l.Add(x.ToObservation(DisplacementDataItem.TypeId));
                if (!DisplacementTable.IsNullOrEmpty()) foreach (var x in DisplacementTable) l.Add(x.ToObservation(DisplacementDataItem.TypeId));
                if (!DisplacementTimeSeries.IsNullOrEmpty()) foreach (var x in DisplacementTimeSeries) l.Add(x.ToObservation(DisplacementDataItem.TypeId));

                if (!DisplacementAngular.IsNullOrEmpty()) foreach (var x in DisplacementAngular) l.Add(x.ToObservation(DisplacementAngularDataItem.TypeId));
                if (!DisplacementAngularDataSet.IsNullOrEmpty()) foreach (var x in DisplacementAngularDataSet) l.Add(x.ToObservation(DisplacementAngularDataItem.TypeId));
                if (!DisplacementAngularTable.IsNullOrEmpty()) foreach (var x in DisplacementAngularTable) l.Add(x.ToObservation(DisplacementAngularDataItem.TypeId));
                if (!DisplacementAngularTimeSeries.IsNullOrEmpty()) foreach (var x in DisplacementAngularTimeSeries) l.Add(x.ToObservation(DisplacementAngularDataItem.TypeId));

                if (!DisplacementLinear.IsNullOrEmpty()) foreach (var x in DisplacementLinear) l.Add(x.ToObservation(DisplacementLinearDataItem.TypeId));
                if (!DisplacementLinearDataSet.IsNullOrEmpty()) foreach (var x in DisplacementLinearDataSet) l.Add(x.ToObservation(DisplacementLinearDataItem.TypeId));
                if (!DisplacementLinearTable.IsNullOrEmpty()) foreach (var x in DisplacementLinearTable) l.Add(x.ToObservation(DisplacementLinearDataItem.TypeId));
                if (!DisplacementLinearTimeSeries.IsNullOrEmpty()) foreach (var x in DisplacementLinearTimeSeries) l.Add(x.ToObservation(DisplacementLinearDataItem.TypeId));

                if (!ElectricalEnergy.IsNullOrEmpty()) foreach (var x in ElectricalEnergy) l.Add(x.ToObservation(ElectricalEnergyDataItem.TypeId));
                if (!ElectricalEnergyDataSet.IsNullOrEmpty()) foreach (var x in ElectricalEnergyDataSet) l.Add(x.ToObservation(ElectricalEnergyDataItem.TypeId));
                if (!ElectricalEnergyTable.IsNullOrEmpty()) foreach (var x in ElectricalEnergyTable) l.Add(x.ToObservation(ElectricalEnergyDataItem.TypeId));
                if (!ElectricalEnergyTimeSeries.IsNullOrEmpty()) foreach (var x in ElectricalEnergyTimeSeries) l.Add(x.ToObservation(ElectricalEnergyDataItem.TypeId));

                if (!EquipmentTimer.IsNullOrEmpty()) foreach (var x in EquipmentTimer) l.Add(x.ToObservation(EquipmentTimerDataItem.TypeId));
                if (!EquipmentTimerDataSet.IsNullOrEmpty()) foreach (var x in EquipmentTimerDataSet) l.Add(x.ToObservation(EquipmentTimerDataItem.TypeId));
                if (!EquipmentTimerTable.IsNullOrEmpty()) foreach (var x in EquipmentTimerTable) l.Add(x.ToObservation(EquipmentTimerDataItem.TypeId));
                if (!EquipmentTimerTimeSeries.IsNullOrEmpty()) foreach (var x in EquipmentTimerTimeSeries) l.Add(x.ToObservation(EquipmentTimerDataItem.TypeId));

                if (!FillHeight.IsNullOrEmpty()) foreach (var x in FillHeight) l.Add(x.ToObservation(FillHeightDataItem.TypeId));
                if (!FillHeightDataSet.IsNullOrEmpty()) foreach (var x in FillHeightDataSet) l.Add(x.ToObservation(FillHeightDataItem.TypeId));
                if (!FillHeightTable.IsNullOrEmpty()) foreach (var x in FillHeightTable) l.Add(x.ToObservation(FillHeightDataItem.TypeId));
                if (!FillHeightTimeSeries.IsNullOrEmpty()) foreach (var x in FillHeightTimeSeries) l.Add(x.ToObservation(FillHeightDataItem.TypeId));

                if (!FillLevel.IsNullOrEmpty()) foreach (var x in FillLevel) l.Add(x.ToObservation(FillLevelDataItem.TypeId));
                if (!FillLevelDataSet.IsNullOrEmpty()) foreach (var x in FillLevelDataSet) l.Add(x.ToObservation(FillLevelDataItem.TypeId));
                if (!FillLevelTable.IsNullOrEmpty()) foreach (var x in FillLevelTable) l.Add(x.ToObservation(FillLevelDataItem.TypeId));
                if (!FillLevelTimeSeries.IsNullOrEmpty()) foreach (var x in FillLevelTimeSeries) l.Add(x.ToObservation(FillLevelDataItem.TypeId));

                if (!Flow.IsNullOrEmpty()) foreach (var x in Flow) l.Add(x.ToObservation(FlowDataItem.TypeId));
                if (!FlowDataSet.IsNullOrEmpty()) foreach (var x in FlowDataSet) l.Add(x.ToObservation(FlowDataItem.TypeId));
                if (!FlowTable.IsNullOrEmpty()) foreach (var x in FlowTable) l.Add(x.ToObservation(FlowDataItem.TypeId));
                if (!FlowTimeSeries.IsNullOrEmpty()) foreach (var x in FlowTimeSeries) l.Add(x.ToObservation(FlowDataItem.TypeId));

                if (!FollowingError.IsNullOrEmpty()) foreach (var x in FollowingError) l.Add(x.ToObservation(FollowingErrorDataItem.TypeId));
                if (!FollowingErrorDataSet.IsNullOrEmpty()) foreach (var x in FollowingErrorDataSet) l.Add(x.ToObservation(FollowingErrorDataItem.TypeId));
                if (!FollowingErrorTable.IsNullOrEmpty()) foreach (var x in FollowingErrorTable) l.Add(x.ToObservation(FollowingErrorDataItem.TypeId));
                if (!FollowingErrorTimeSeries.IsNullOrEmpty()) foreach (var x in FollowingErrorTimeSeries) l.Add(x.ToObservation(FollowingErrorDataItem.TypeId));

                if (!FollowingErrorAngular.IsNullOrEmpty()) foreach (var x in FollowingErrorAngular) l.Add(x.ToObservation(FollowingErrorAngularDataItem.TypeId));
                if (!FollowingErrorAngularDataSet.IsNullOrEmpty()) foreach (var x in FollowingErrorAngularDataSet) l.Add(x.ToObservation(FollowingErrorAngularDataItem.TypeId));
                if (!FollowingErrorAngularTable.IsNullOrEmpty()) foreach (var x in FollowingErrorAngularTable) l.Add(x.ToObservation(FollowingErrorAngularDataItem.TypeId));
                if (!FollowingErrorAngularTimeSeries.IsNullOrEmpty()) foreach (var x in FollowingErrorAngularTimeSeries) l.Add(x.ToObservation(FollowingErrorAngularDataItem.TypeId));

                if (!FollowingErrorLinear.IsNullOrEmpty()) foreach (var x in FollowingErrorLinear) l.Add(x.ToObservation(FollowingErrorLinearDataItem.TypeId));
                if (!FollowingErrorLinearDataSet.IsNullOrEmpty()) foreach (var x in FollowingErrorLinearDataSet) l.Add(x.ToObservation(FollowingErrorLinearDataItem.TypeId));
                if (!FollowingErrorLinearTable.IsNullOrEmpty()) foreach (var x in FollowingErrorLinearTable) l.Add(x.ToObservation(FollowingErrorLinearDataItem.TypeId));
                if (!FollowingErrorLinearTimeSeries.IsNullOrEmpty()) foreach (var x in FollowingErrorLinearTimeSeries) l.Add(x.ToObservation(FollowingErrorLinearDataItem.TypeId));

                if (!Frequency.IsNullOrEmpty()) foreach (var x in Frequency) l.Add(x.ToObservation(FrequencyDataItem.TypeId));
                if (!FrequencyDataSet.IsNullOrEmpty()) foreach (var x in FrequencyDataSet) l.Add(x.ToObservation(FrequencyDataItem.TypeId));
                if (!FrequencyTable.IsNullOrEmpty()) foreach (var x in FrequencyTable) l.Add(x.ToObservation(FrequencyDataItem.TypeId));
                if (!FrequencyTimeSeries.IsNullOrEmpty()) foreach (var x in FrequencyTimeSeries) l.Add(x.ToObservation(FrequencyDataItem.TypeId));

                if (!GlobalPosition.IsNullOrEmpty()) foreach (var x in GlobalPosition) l.Add(x.ToObservation(GlobalPositionDataItem.TypeId));
                if (!GlobalPositionDataSet.IsNullOrEmpty()) foreach (var x in GlobalPositionDataSet) l.Add(x.ToObservation(GlobalPositionDataItem.TypeId));
                if (!GlobalPositionTable.IsNullOrEmpty()) foreach (var x in GlobalPositionTable) l.Add(x.ToObservation(GlobalPositionDataItem.TypeId));
                if (!GlobalPositionTimeSeries.IsNullOrEmpty()) foreach (var x in GlobalPositionTimeSeries) l.Add(x.ToObservation(GlobalPositionDataItem.TypeId));

                if (!GravitationalAcceleration.IsNullOrEmpty()) foreach (var x in GravitationalAcceleration) l.Add(x.ToObservation(GravitationalAccelerationDataItem.TypeId));
                if (!GravitationalAccelerationDataSet.IsNullOrEmpty()) foreach (var x in GravitationalAccelerationDataSet) l.Add(x.ToObservation(GravitationalAccelerationDataItem.TypeId));
                if (!GravitationalAccelerationTable.IsNullOrEmpty()) foreach (var x in GravitationalAccelerationTable) l.Add(x.ToObservation(GravitationalAccelerationDataItem.TypeId));
                if (!GravitationalAccelerationTimeSeries.IsNullOrEmpty()) foreach (var x in GravitationalAccelerationTimeSeries) l.Add(x.ToObservation(GravitationalAccelerationDataItem.TypeId));

                if (!GravitationalForce.IsNullOrEmpty()) foreach (var x in GravitationalForce) l.Add(x.ToObservation(GravitationalForceDataItem.TypeId));
                if (!GravitationalForceDataSet.IsNullOrEmpty()) foreach (var x in GravitationalForceDataSet) l.Add(x.ToObservation(GravitationalForceDataItem.TypeId));
                if (!GravitationalForceTable.IsNullOrEmpty()) foreach (var x in GravitationalForceTable) l.Add(x.ToObservation(GravitationalForceDataItem.TypeId));
                if (!GravitationalForceTimeSeries.IsNullOrEmpty()) foreach (var x in GravitationalForceTimeSeries) l.Add(x.ToObservation(GravitationalForceDataItem.TypeId));

                if (!HumidityAbsolute.IsNullOrEmpty()) foreach (var x in HumidityAbsolute) l.Add(x.ToObservation(HumidityAbsoluteDataItem.TypeId));
                if (!HumidityAbsoluteDataSet.IsNullOrEmpty()) foreach (var x in HumidityAbsoluteDataSet) l.Add(x.ToObservation(HumidityAbsoluteDataItem.TypeId));
                if (!HumidityAbsoluteTable.IsNullOrEmpty()) foreach (var x in HumidityAbsoluteTable) l.Add(x.ToObservation(HumidityAbsoluteDataItem.TypeId));
                if (!HumidityAbsoluteTimeSeries.IsNullOrEmpty()) foreach (var x in HumidityAbsoluteTimeSeries) l.Add(x.ToObservation(HumidityAbsoluteDataItem.TypeId));

                if (!HumidityRelative.IsNullOrEmpty()) foreach (var x in HumidityRelative) l.Add(x.ToObservation(HumidityRelativeDataItem.TypeId));
                if (!HumidityRelativeDataSet.IsNullOrEmpty()) foreach (var x in HumidityRelativeDataSet) l.Add(x.ToObservation(HumidityRelativeDataItem.TypeId));
                if (!HumidityRelativeTable.IsNullOrEmpty()) foreach (var x in HumidityRelativeTable) l.Add(x.ToObservation(HumidityRelativeDataItem.TypeId));
                if (!HumidityRelativeTimeSeries.IsNullOrEmpty()) foreach (var x in HumidityRelativeTimeSeries) l.Add(x.ToObservation(HumidityRelativeDataItem.TypeId));

                if (!HumiditySpecific.IsNullOrEmpty()) foreach (var x in HumiditySpecific) l.Add(x.ToObservation(HumiditySpecificDataItem.TypeId));
                if (!HumiditySpecificDataSet.IsNullOrEmpty()) foreach (var x in HumiditySpecificDataSet) l.Add(x.ToObservation(HumiditySpecificDataItem.TypeId));
                if (!HumiditySpecificTable.IsNullOrEmpty()) foreach (var x in HumiditySpecificTable) l.Add(x.ToObservation(HumiditySpecificDataItem.TypeId));
                if (!HumiditySpecificTimeSeries.IsNullOrEmpty()) foreach (var x in HumiditySpecificTimeSeries) l.Add(x.ToObservation(HumiditySpecificDataItem.TypeId));

                if (!Length.IsNullOrEmpty()) foreach (var x in Length) l.Add(x.ToObservation(LengthDataItem.TypeId));
                if (!LengthDataSet.IsNullOrEmpty()) foreach (var x in LengthDataSet) l.Add(x.ToObservation(LengthDataItem.TypeId));
                if (!LengthTable.IsNullOrEmpty()) foreach (var x in LengthTable) l.Add(x.ToObservation(LengthDataItem.TypeId));
                if (!LengthTimeSeries.IsNullOrEmpty()) foreach (var x in LengthTimeSeries) l.Add(x.ToObservation(LengthDataItem.TypeId));

                if (!Level.IsNullOrEmpty()) foreach (var x in Level) l.Add(x.ToObservation(LevelDataItem.TypeId));
                if (!LevelDataSet.IsNullOrEmpty()) foreach (var x in LevelDataSet) l.Add(x.ToObservation(LevelDataItem.TypeId));
                if (!LevelTable.IsNullOrEmpty()) foreach (var x in LevelTable) l.Add(x.ToObservation(LevelDataItem.TypeId));
                if (!LevelTimeSeries.IsNullOrEmpty()) foreach (var x in LevelTimeSeries) l.Add(x.ToObservation(LevelDataItem.TypeId));

                if (!LinearForce.IsNullOrEmpty()) foreach (var x in LinearForce) l.Add(x.ToObservation(LinearForceDataItem.TypeId));
                if (!LinearForceDataSet.IsNullOrEmpty()) foreach (var x in LinearForceDataSet) l.Add(x.ToObservation(LinearForceDataItem.TypeId));
                if (!LinearForceTable.IsNullOrEmpty()) foreach (var x in LinearForceTable) l.Add(x.ToObservation(LinearForceDataItem.TypeId));
                if (!LinearForceTimeSeries.IsNullOrEmpty()) foreach (var x in LinearForceTimeSeries) l.Add(x.ToObservation(LinearForceDataItem.TypeId));

                if (!Load.IsNullOrEmpty()) foreach (var x in Load) l.Add(x.ToObservation(LoadDataItem.TypeId));
                if (!LoadDataSet.IsNullOrEmpty()) foreach (var x in LoadDataSet) l.Add(x.ToObservation(LoadDataItem.TypeId));
                if (!LoadTable.IsNullOrEmpty()) foreach (var x in LoadTable) l.Add(x.ToObservation(LoadDataItem.TypeId));
                if (!LoadTimeSeries.IsNullOrEmpty()) foreach (var x in LoadTimeSeries) l.Add(x.ToObservation(LoadDataItem.TypeId));

                if (!Mass.IsNullOrEmpty()) foreach (var x in Mass) l.Add(x.ToObservation(MassDataItem.TypeId));
                if (!MassDataSet.IsNullOrEmpty()) foreach (var x in MassDataSet) l.Add(x.ToObservation(MassDataItem.TypeId));
                if (!MassTable.IsNullOrEmpty()) foreach (var x in MassTable) l.Add(x.ToObservation(MassDataItem.TypeId));
                if (!MassTimeSeries.IsNullOrEmpty()) foreach (var x in MassTimeSeries) l.Add(x.ToObservation(MassDataItem.TypeId));

                if (!ObservationUpdateRate.IsNullOrEmpty()) foreach (var x in ObservationUpdateRate) l.Add(x.ToObservation(ObservationUpdateRateDataItem.TypeId));
                if (!ObservationUpdateRateDataSet.IsNullOrEmpty()) foreach (var x in ObservationUpdateRateDataSet) l.Add(x.ToObservation(ObservationUpdateRateDataItem.TypeId));
                if (!ObservationUpdateRateTable.IsNullOrEmpty()) foreach (var x in ObservationUpdateRateTable) l.Add(x.ToObservation(ObservationUpdateRateDataItem.TypeId));
                if (!ObservationUpdateRateTimeSeries.IsNullOrEmpty()) foreach (var x in ObservationUpdateRateTimeSeries) l.Add(x.ToObservation(ObservationUpdateRateDataItem.TypeId));

                if (!Openness.IsNullOrEmpty()) foreach (var x in Openness) l.Add(x.ToObservation(OpennessDataItem.TypeId));
                if (!OpennessDataSet.IsNullOrEmpty()) foreach (var x in OpennessDataSet) l.Add(x.ToObservation(OpennessDataItem.TypeId));
                if (!OpennessTable.IsNullOrEmpty()) foreach (var x in OpennessTable) l.Add(x.ToObservation(OpennessDataItem.TypeId));
                if (!OpennessTimeSeries.IsNullOrEmpty()) foreach (var x in OpennessTimeSeries) l.Add(x.ToObservation(OpennessDataItem.TypeId));

                if (!Orientation.IsNullOrEmpty()) foreach (var x in Orientation) l.Add(x.ToObservation(OrientationDataItem.TypeId));
                if (!OrientationDataSet.IsNullOrEmpty()) foreach (var x in OrientationDataSet) l.Add(x.ToObservation(OrientationDataItem.TypeId));
                if (!OrientationTable.IsNullOrEmpty()) foreach (var x in OrientationTable) l.Add(x.ToObservation(OrientationDataItem.TypeId));
                if (!OrientationTimeSeries.IsNullOrEmpty()) foreach (var x in OrientationTimeSeries) l.Add(x.ToObservation(OrientationDataItem.TypeId));

                if (!ParticleCount.IsNullOrEmpty()) foreach (var x in ParticleCount) l.Add(x.ToObservation(ParticleCountDataItem.TypeId));
                if (!ParticleCountDataSet.IsNullOrEmpty()) foreach (var x in ParticleCountDataSet) l.Add(x.ToObservation(ParticleCountDataItem.TypeId));
                if (!ParticleCountTable.IsNullOrEmpty()) foreach (var x in ParticleCountTable) l.Add(x.ToObservation(ParticleCountDataItem.TypeId));
                if (!ParticleCountTimeSeries.IsNullOrEmpty()) foreach (var x in ParticleCountTimeSeries) l.Add(x.ToObservation(ParticleCountDataItem.TypeId));

                if (!ParticleSize.IsNullOrEmpty()) foreach (var x in ParticleSize) l.Add(x.ToObservation(ParticleSizeDataItem.TypeId));
                if (!ParticleSizeDataSet.IsNullOrEmpty()) foreach (var x in ParticleSizeDataSet) l.Add(x.ToObservation(ParticleSizeDataItem.TypeId));
                if (!ParticleSizeTable.IsNullOrEmpty()) foreach (var x in ParticleSizeTable) l.Add(x.ToObservation(ParticleSizeDataItem.TypeId));
                if (!ParticleSizeTimeSeries.IsNullOrEmpty()) foreach (var x in ParticleSizeTimeSeries) l.Add(x.ToObservation(ParticleSizeDataItem.TypeId));

                if (!PathFeedrate.IsNullOrEmpty()) foreach (var x in PathFeedrate) l.Add(x.ToObservation(PathFeedrateDataItem.TypeId));
                if (!PathFeedrateDataSet.IsNullOrEmpty()) foreach (var x in PathFeedrateDataSet) l.Add(x.ToObservation(PathFeedrateDataItem.TypeId));
                if (!PathFeedrateTable.IsNullOrEmpty()) foreach (var x in PathFeedrateTable) l.Add(x.ToObservation(PathFeedrateDataItem.TypeId));
                if (!PathFeedrateTimeSeries.IsNullOrEmpty()) foreach (var x in PathFeedrateTimeSeries) l.Add(x.ToObservation(PathFeedrateDataItem.TypeId));

                if (!PathFeedratePerRevolution.IsNullOrEmpty()) foreach (var x in PathFeedratePerRevolution) l.Add(x.ToObservation(PathFeedratePerRevolutionDataItem.TypeId));
                if (!PathFeedratePerRevolutionDataSet.IsNullOrEmpty()) foreach (var x in PathFeedratePerRevolutionDataSet) l.Add(x.ToObservation(PathFeedratePerRevolutionDataItem.TypeId));
                if (!PathFeedratePerRevolutionTable.IsNullOrEmpty()) foreach (var x in PathFeedratePerRevolutionTable) l.Add(x.ToObservation(PathFeedratePerRevolutionDataItem.TypeId));
                if (!PathFeedratePerRevolutionTimeSeries.IsNullOrEmpty()) foreach (var x in PathFeedratePerRevolutionTimeSeries) l.Add(x.ToObservation(PathFeedratePerRevolutionDataItem.TypeId));

                if (!PathPosition.IsNullOrEmpty()) foreach (var x in PathPosition) l.Add(x.ToObservation(PathPositionDataItem.TypeId));
                if (!PathPositionDataSet.IsNullOrEmpty()) foreach (var x in PathPositionDataSet) l.Add(x.ToObservation(PathPositionDataItem.TypeId));
                if (!PathPositionTable.IsNullOrEmpty()) foreach (var x in PathPositionTable) l.Add(x.ToObservation(PathPositionDataItem.TypeId));
                if (!PathPositionTimeSeries.IsNullOrEmpty()) foreach (var x in PathPositionTimeSeries) l.Add(x.ToObservation(PathPositionDataItem.TypeId));

                if (!PH.IsNullOrEmpty()) foreach (var x in PH) l.Add(x.ToObservation(PHDataItem.TypeId));
                if (!PHDataSet.IsNullOrEmpty()) foreach (var x in PHDataSet) l.Add(x.ToObservation(PHDataItem.TypeId));
                if (!PHTable.IsNullOrEmpty()) foreach (var x in PHTable) l.Add(x.ToObservation(PHDataItem.TypeId));
                if (!PHTimeSeries.IsNullOrEmpty()) foreach (var x in PHTimeSeries) l.Add(x.ToObservation(PHDataItem.TypeId));

                if (!Position.IsNullOrEmpty()) foreach (var x in Position) l.Add(x.ToObservation(PositionDataItem.TypeId));
                if (!PositionDataSet.IsNullOrEmpty()) foreach (var x in PositionDataSet) l.Add(x.ToObservation(PositionDataItem.TypeId));
                if (!PositionTable.IsNullOrEmpty()) foreach (var x in PositionTable) l.Add(x.ToObservation(PositionDataItem.TypeId));
                if (!PositionTimeSeries.IsNullOrEmpty()) foreach (var x in PositionTimeSeries) l.Add(x.ToObservation(PositionDataItem.TypeId));

                if (!PositionCartesian.IsNullOrEmpty()) foreach (var x in PositionCartesian) l.Add(x.ToObservation(PositionCartesianDataItem.TypeId));
                if (!PositionCartesianDataSet.IsNullOrEmpty()) foreach (var x in PositionCartesianDataSet) l.Add(x.ToObservation(PositionCartesianDataItem.TypeId));
                if (!PositionCartesianTable.IsNullOrEmpty()) foreach (var x in PositionCartesianTable) l.Add(x.ToObservation(PositionCartesianDataItem.TypeId));
                if (!PositionCartesianTimeSeries.IsNullOrEmpty()) foreach (var x in PositionCartesianTimeSeries) l.Add(x.ToObservation(PositionCartesianDataItem.TypeId));

                if (!PowerFactor.IsNullOrEmpty()) foreach (var x in PowerFactor) l.Add(x.ToObservation(PowerFactorDataItem.TypeId));
                if (!PowerFactorDataSet.IsNullOrEmpty()) foreach (var x in PowerFactorDataSet) l.Add(x.ToObservation(PowerFactorDataItem.TypeId));
                if (!PowerFactorTable.IsNullOrEmpty()) foreach (var x in PowerFactorTable) l.Add(x.ToObservation(PowerFactorDataItem.TypeId));
                if (!PowerFactorTimeSeries.IsNullOrEmpty()) foreach (var x in PowerFactorTimeSeries) l.Add(x.ToObservation(PowerFactorDataItem.TypeId));

                if (!Pressure.IsNullOrEmpty()) foreach (var x in Pressure) l.Add(x.ToObservation(PressureDataItem.TypeId));
                if (!PressureDataSet.IsNullOrEmpty()) foreach (var x in PressureDataSet) l.Add(x.ToObservation(PressureDataItem.TypeId));
                if (!PressureTable.IsNullOrEmpty()) foreach (var x in PressureTable) l.Add(x.ToObservation(PressureDataItem.TypeId));
                if (!PressureTimeSeries.IsNullOrEmpty()) foreach (var x in PressureTimeSeries) l.Add(x.ToObservation(PressureDataItem.TypeId));

                if (!PressureAbsolute.IsNullOrEmpty()) foreach (var x in PressureAbsolute) l.Add(x.ToObservation(PressureAbsoluteDataItem.TypeId));
                if (!PressureAbsoluteDataSet.IsNullOrEmpty()) foreach (var x in PressureAbsoluteDataSet) l.Add(x.ToObservation(PressureAbsoluteDataItem.TypeId));
                if (!PressureAbsoluteTable.IsNullOrEmpty()) foreach (var x in PressureAbsoluteTable) l.Add(x.ToObservation(PressureAbsoluteDataItem.TypeId));
                if (!PressureAbsoluteTimeSeries.IsNullOrEmpty()) foreach (var x in PressureAbsoluteTimeSeries) l.Add(x.ToObservation(PressureAbsoluteDataItem.TypeId));

                if (!PressurizationRate.IsNullOrEmpty()) foreach (var x in PressurizationRate) l.Add(x.ToObservation(PressurizationRateDataItem.TypeId));
                if (!PressurizationRateDataSet.IsNullOrEmpty()) foreach (var x in PressurizationRateDataSet) l.Add(x.ToObservation(PressurizationRateDataItem.TypeId));
                if (!PressurizationRateTable.IsNullOrEmpty()) foreach (var x in PressurizationRateTable) l.Add(x.ToObservation(PressurizationRateDataItem.TypeId));
                if (!PressurizationRateTimeSeries.IsNullOrEmpty()) foreach (var x in PressurizationRateTimeSeries) l.Add(x.ToObservation(PressurizationRateDataItem.TypeId));

                if (!ProcessTimer.IsNullOrEmpty()) foreach (var x in ProcessTimer) l.Add(x.ToObservation(ProcessTimerDataItem.TypeId));
                if (!ProcessTimerDataSet.IsNullOrEmpty()) foreach (var x in ProcessTimerDataSet) l.Add(x.ToObservation(ProcessTimerDataItem.TypeId));
                if (!ProcessTimerTable.IsNullOrEmpty()) foreach (var x in ProcessTimerTable) l.Add(x.ToObservation(ProcessTimerDataItem.TypeId));
                if (!ProcessTimerTimeSeries.IsNullOrEmpty()) foreach (var x in ProcessTimerTimeSeries) l.Add(x.ToObservation(ProcessTimerDataItem.TypeId));

                if (!Resistance.IsNullOrEmpty()) foreach (var x in Resistance) l.Add(x.ToObservation(ResistanceDataItem.TypeId));
                if (!ResistanceDataSet.IsNullOrEmpty()) foreach (var x in ResistanceDataSet) l.Add(x.ToObservation(ResistanceDataItem.TypeId));
                if (!ResistanceTable.IsNullOrEmpty()) foreach (var x in ResistanceTable) l.Add(x.ToObservation(ResistanceDataItem.TypeId));
                if (!ResistanceTimeSeries.IsNullOrEmpty()) foreach (var x in ResistanceTimeSeries) l.Add(x.ToObservation(ResistanceDataItem.TypeId));

                if (!Resistivity.IsNullOrEmpty()) foreach (var x in Resistivity) l.Add(x.ToObservation(ResistivityDataItem.TypeId));
                if (!ResistivityDataSet.IsNullOrEmpty()) foreach (var x in ResistivityDataSet) l.Add(x.ToObservation(ResistivityDataItem.TypeId));
                if (!ResistivityTable.IsNullOrEmpty()) foreach (var x in ResistivityTable) l.Add(x.ToObservation(ResistivityDataItem.TypeId));
                if (!ResistivityTimeSeries.IsNullOrEmpty()) foreach (var x in ResistivityTimeSeries) l.Add(x.ToObservation(ResistivityDataItem.TypeId));

                if (!RotaryVelocity.IsNullOrEmpty()) foreach (var x in RotaryVelocity) l.Add(x.ToObservation(RotaryVelocityDataItem.TypeId));
                if (!RotaryVelocityDataSet.IsNullOrEmpty()) foreach (var x in RotaryVelocityDataSet) l.Add(x.ToObservation(RotaryVelocityDataItem.TypeId));
                if (!RotaryVelocityTable.IsNullOrEmpty()) foreach (var x in RotaryVelocityTable) l.Add(x.ToObservation(RotaryVelocityDataItem.TypeId));
                if (!RotaryVelocityTimeSeries.IsNullOrEmpty()) foreach (var x in RotaryVelocityTimeSeries) l.Add(x.ToObservation(RotaryVelocityDataItem.TypeId));

                if (!SettlingError.IsNullOrEmpty()) foreach (var x in SettlingError) l.Add(x.ToObservation(SettlingErrorDataItem.TypeId));
                if (!SettlingErrorDataSet.IsNullOrEmpty()) foreach (var x in SettlingErrorDataSet) l.Add(x.ToObservation(SettlingErrorDataItem.TypeId));
                if (!SettlingErrorTable.IsNullOrEmpty()) foreach (var x in SettlingErrorTable) l.Add(x.ToObservation(SettlingErrorDataItem.TypeId));
                if (!SettlingErrorTimeSeries.IsNullOrEmpty()) foreach (var x in SettlingErrorTimeSeries) l.Add(x.ToObservation(SettlingErrorDataItem.TypeId));

                if (!SettlingErrorAngular.IsNullOrEmpty()) foreach (var x in SettlingErrorAngular) l.Add(x.ToObservation(SettlingErrorAngularDataItem.TypeId));
                if (!SettlingErrorAngularDataSet.IsNullOrEmpty()) foreach (var x in SettlingErrorAngularDataSet) l.Add(x.ToObservation(SettlingErrorAngularDataItem.TypeId));
                if (!SettlingErrorAngularTable.IsNullOrEmpty()) foreach (var x in SettlingErrorAngularTable) l.Add(x.ToObservation(SettlingErrorAngularDataItem.TypeId));
                if (!SettlingErrorAngularTimeSeries.IsNullOrEmpty()) foreach (var x in SettlingErrorAngularTimeSeries) l.Add(x.ToObservation(SettlingErrorAngularDataItem.TypeId));

                if (!SettlingErrorLinear.IsNullOrEmpty()) foreach (var x in SettlingErrorLinear) l.Add(x.ToObservation(SettlingErrorLinearDataItem.TypeId));
                if (!SettlingErrorLinearDataSet.IsNullOrEmpty()) foreach (var x in SettlingErrorLinearDataSet) l.Add(x.ToObservation(SettlingErrorLinearDataItem.TypeId));
                if (!SettlingErrorLinearTable.IsNullOrEmpty()) foreach (var x in SettlingErrorLinearTable) l.Add(x.ToObservation(SettlingErrorLinearDataItem.TypeId));
                if (!SettlingErrorLinearTimeSeries.IsNullOrEmpty()) foreach (var x in SettlingErrorLinearTimeSeries) l.Add(x.ToObservation(SettlingErrorLinearDataItem.TypeId));

                if (!SoundLevel.IsNullOrEmpty()) foreach (var x in SoundLevel) l.Add(x.ToObservation(SoundLevelDataItem.TypeId));
                if (!SoundLevelDataSet.IsNullOrEmpty()) foreach (var x in SoundLevelDataSet) l.Add(x.ToObservation(SoundLevelDataItem.TypeId));
                if (!SoundLevelTable.IsNullOrEmpty()) foreach (var x in SoundLevelTable) l.Add(x.ToObservation(SoundLevelDataItem.TypeId));
                if (!SoundLevelTimeSeries.IsNullOrEmpty()) foreach (var x in SoundLevelTimeSeries) l.Add(x.ToObservation(SoundLevelDataItem.TypeId));

                if (!SpindleSpeed.IsNullOrEmpty()) foreach (var x in SpindleSpeed) l.Add(x.ToObservation(SpindleSpeedDataItem.TypeId));
                if (!SpindleSpeedDataSet.IsNullOrEmpty()) foreach (var x in SpindleSpeedDataSet) l.Add(x.ToObservation(SpindleSpeedDataItem.TypeId));
                if (!SpindleSpeedTable.IsNullOrEmpty()) foreach (var x in SpindleSpeedTable) l.Add(x.ToObservation(SpindleSpeedDataItem.TypeId));
                if (!SpindleSpeedTimeSeries.IsNullOrEmpty()) foreach (var x in SpindleSpeedTimeSeries) l.Add(x.ToObservation(SpindleSpeedDataItem.TypeId));

                if (!Strain.IsNullOrEmpty()) foreach (var x in Strain) l.Add(x.ToObservation(StrainDataItem.TypeId));
                if (!StrainDataSet.IsNullOrEmpty()) foreach (var x in StrainDataSet) l.Add(x.ToObservation(StrainDataItem.TypeId));
                if (!StrainTable.IsNullOrEmpty()) foreach (var x in StrainTable) l.Add(x.ToObservation(StrainDataItem.TypeId));
                if (!StrainTimeSeries.IsNullOrEmpty()) foreach (var x in StrainTimeSeries) l.Add(x.ToObservation(StrainDataItem.TypeId));

                if (!Temperature.IsNullOrEmpty()) foreach (var x in Temperature) l.Add(x.ToObservation(TemperatureDataItem.TypeId));
                if (!TemperatureDataSet.IsNullOrEmpty()) foreach (var x in TemperatureDataSet) l.Add(x.ToObservation(TemperatureDataItem.TypeId));
                if (!TemperatureTable.IsNullOrEmpty()) foreach (var x in TemperatureTable) l.Add(x.ToObservation(TemperatureDataItem.TypeId));
                if (!TemperatureTimeSeries.IsNullOrEmpty()) foreach (var x in TemperatureTimeSeries) l.Add(x.ToObservation(TemperatureDataItem.TypeId));

                if (!Tension.IsNullOrEmpty()) foreach (var x in Tension) l.Add(x.ToObservation(TensionDataItem.TypeId));
                if (!TensionDataSet.IsNullOrEmpty()) foreach (var x in TensionDataSet) l.Add(x.ToObservation(TensionDataItem.TypeId));
                if (!TensionTable.IsNullOrEmpty()) foreach (var x in TensionTable) l.Add(x.ToObservation(TensionDataItem.TypeId));
                if (!TensionTimeSeries.IsNullOrEmpty()) foreach (var x in TensionTimeSeries) l.Add(x.ToObservation(TensionDataItem.TypeId));

                if (!Tilt.IsNullOrEmpty()) foreach (var x in Tilt) l.Add(x.ToObservation(TiltDataItem.TypeId));
                if (!TiltDataSet.IsNullOrEmpty()) foreach (var x in TiltDataSet) l.Add(x.ToObservation(TiltDataItem.TypeId));
                if (!TiltTable.IsNullOrEmpty()) foreach (var x in TiltTable) l.Add(x.ToObservation(TiltDataItem.TypeId));
                if (!TiltTimeSeries.IsNullOrEmpty()) foreach (var x in TiltTimeSeries) l.Add(x.ToObservation(TiltDataItem.TypeId));

                if (!Torque.IsNullOrEmpty()) foreach (var x in Torque) l.Add(x.ToObservation(TorqueDataItem.TypeId));
                if (!TorqueDataSet.IsNullOrEmpty()) foreach (var x in TorqueDataSet) l.Add(x.ToObservation(TorqueDataItem.TypeId));
                if (!TorqueTable.IsNullOrEmpty()) foreach (var x in TorqueTable) l.Add(x.ToObservation(TorqueDataItem.TypeId));
                if (!TorqueTimeSeries.IsNullOrEmpty()) foreach (var x in TorqueTimeSeries) l.Add(x.ToObservation(TorqueDataItem.TypeId));

                if (!Velocity.IsNullOrEmpty()) foreach (var x in Velocity) l.Add(x.ToObservation(VelocityDataItem.TypeId));
                if (!VelocityDataSet.IsNullOrEmpty()) foreach (var x in VelocityDataSet) l.Add(x.ToObservation(VelocityDataItem.TypeId));
                if (!VelocityTable.IsNullOrEmpty()) foreach (var x in VelocityTable) l.Add(x.ToObservation(VelocityDataItem.TypeId));
                if (!VelocityTimeSeries.IsNullOrEmpty()) foreach (var x in VelocityTimeSeries) l.Add(x.ToObservation(VelocityDataItem.TypeId));

                if (!Viscosity.IsNullOrEmpty()) foreach (var x in Viscosity) l.Add(x.ToObservation(ViscosityDataItem.TypeId));
                if (!ViscosityDataSet.IsNullOrEmpty()) foreach (var x in ViscosityDataSet) l.Add(x.ToObservation(ViscosityDataItem.TypeId));
                if (!ViscosityTable.IsNullOrEmpty()) foreach (var x in ViscosityTable) l.Add(x.ToObservation(ViscosityDataItem.TypeId));
                if (!ViscosityTimeSeries.IsNullOrEmpty()) foreach (var x in ViscosityTimeSeries) l.Add(x.ToObservation(ViscosityDataItem.TypeId));

                if (!VoltAmpere.IsNullOrEmpty()) foreach (var x in VoltAmpere) l.Add(x.ToObservation(VoltAmpereDataItem.TypeId));
                if (!VoltAmpereDataSet.IsNullOrEmpty()) foreach (var x in VoltAmpereDataSet) l.Add(x.ToObservation(VoltAmpereDataItem.TypeId));
                if (!VoltAmpereTable.IsNullOrEmpty()) foreach (var x in VoltAmpereTable) l.Add(x.ToObservation(VoltAmpereDataItem.TypeId));
                if (!VoltAmpereTimeSeries.IsNullOrEmpty()) foreach (var x in VoltAmpereTimeSeries) l.Add(x.ToObservation(VoltAmpereDataItem.TypeId));

                if (!VoltAmpereReactive.IsNullOrEmpty()) foreach (var x in VoltAmpereReactive) l.Add(x.ToObservation(VoltAmpereReactiveDataItem.TypeId));
                if (!VoltAmpereReactiveDataSet.IsNullOrEmpty()) foreach (var x in VoltAmpereReactiveDataSet) l.Add(x.ToObservation(VoltAmpereReactiveDataItem.TypeId));
                if (!VoltAmpereReactiveTable.IsNullOrEmpty()) foreach (var x in VoltAmpereReactiveTable) l.Add(x.ToObservation(VoltAmpereReactiveDataItem.TypeId));
                if (!VoltAmpereReactiveTimeSeries.IsNullOrEmpty()) foreach (var x in VoltAmpereReactiveTimeSeries) l.Add(x.ToObservation(VoltAmpereReactiveDataItem.TypeId));

                if (!Voltage.IsNullOrEmpty()) foreach (var x in Voltage) l.Add(x.ToObservation(VoltageDataItem.TypeId));
                if (!VoltageDataSet.IsNullOrEmpty()) foreach (var x in VoltageDataSet) l.Add(x.ToObservation(VoltageDataItem.TypeId));
                if (!VoltageTable.IsNullOrEmpty()) foreach (var x in VoltageTable) l.Add(x.ToObservation(VoltageDataItem.TypeId));
                if (!VoltageTimeSeries.IsNullOrEmpty()) foreach (var x in VoltageTimeSeries) l.Add(x.ToObservation(VoltageDataItem.TypeId));

                if (!VoltageAC.IsNullOrEmpty()) foreach (var x in VoltageAC) l.Add(x.ToObservation(VoltageACDataItem.TypeId));
                if (!VoltageACDataSet.IsNullOrEmpty()) foreach (var x in VoltageACDataSet) l.Add(x.ToObservation(VoltageACDataItem.TypeId));
                if (!VoltageACTable.IsNullOrEmpty()) foreach (var x in VoltageACTable) l.Add(x.ToObservation(VoltageACDataItem.TypeId));
                if (!VoltageACTimeSeries.IsNullOrEmpty()) foreach (var x in VoltageACTimeSeries) l.Add(x.ToObservation(VoltageACDataItem.TypeId));

                if (!VoltageDC.IsNullOrEmpty()) foreach (var x in VoltageDC) l.Add(x.ToObservation(VoltageDCDataItem.TypeId));
                if (!VoltageDCDataSet.IsNullOrEmpty()) foreach (var x in VoltageDCDataSet) l.Add(x.ToObservation(VoltageDCDataItem.TypeId));
                if (!VoltageDCTable.IsNullOrEmpty()) foreach (var x in VoltageDCTable) l.Add(x.ToObservation(VoltageDCDataItem.TypeId));
                if (!VoltageDCTimeSeries.IsNullOrEmpty()) foreach (var x in VoltageDCTimeSeries) l.Add(x.ToObservation(VoltageDCDataItem.TypeId));

                if (!VolumeFluid.IsNullOrEmpty()) foreach (var x in VolumeFluid) l.Add(x.ToObservation(VolumeFluidDataItem.TypeId));
                if (!VolumeFluidDataSet.IsNullOrEmpty()) foreach (var x in VolumeFluidDataSet) l.Add(x.ToObservation(VolumeFluidDataItem.TypeId));
                if (!VolumeFluidTable.IsNullOrEmpty()) foreach (var x in VolumeFluidTable) l.Add(x.ToObservation(VolumeFluidDataItem.TypeId));
                if (!VolumeFluidTimeSeries.IsNullOrEmpty()) foreach (var x in VolumeFluidTimeSeries) l.Add(x.ToObservation(VolumeFluidDataItem.TypeId));

                if (!VolumeSpatial.IsNullOrEmpty()) foreach (var x in VolumeSpatial) l.Add(x.ToObservation(VolumeSpatialDataItem.TypeId));
                if (!VolumeSpatialDataSet.IsNullOrEmpty()) foreach (var x in VolumeSpatialDataSet) l.Add(x.ToObservation(VolumeSpatialDataItem.TypeId));
                if (!VolumeSpatialTable.IsNullOrEmpty()) foreach (var x in VolumeSpatialTable) l.Add(x.ToObservation(VolumeSpatialDataItem.TypeId));
                if (!VolumeSpatialTimeSeries.IsNullOrEmpty()) foreach (var x in VolumeSpatialTimeSeries) l.Add(x.ToObservation(VolumeSpatialDataItem.TypeId));

                if (!WaterHardness.IsNullOrEmpty()) foreach (var x in WaterHardness) l.Add(x.ToObservation(WaterHardnessDataItem.TypeId));
                if (!WaterHardnessDataSet.IsNullOrEmpty()) foreach (var x in WaterHardnessDataSet) l.Add(x.ToObservation(WaterHardnessDataItem.TypeId));
                if (!WaterHardnessTable.IsNullOrEmpty()) foreach (var x in WaterHardnessTable) l.Add(x.ToObservation(WaterHardnessDataItem.TypeId));
                if (!WaterHardnessTimeSeries.IsNullOrEmpty()) foreach (var x in WaterHardnessTimeSeries) l.Add(x.ToObservation(WaterHardnessDataItem.TypeId));

                if (!Wattage.IsNullOrEmpty()) foreach (var x in Wattage) l.Add(x.ToObservation(WattageDataItem.TypeId));
                if (!WattageDataSet.IsNullOrEmpty()) foreach (var x in WattageDataSet) l.Add(x.ToObservation(WattageDataItem.TypeId));
                if (!WattageTable.IsNullOrEmpty()) foreach (var x in WattageTable) l.Add(x.ToObservation(WattageDataItem.TypeId));
                if (!WattageTimeSeries.IsNullOrEmpty()) foreach (var x in WattageTimeSeries) l.Add(x.ToObservation(WattageDataItem.TypeId));

                if (!XDimension.IsNullOrEmpty()) foreach (var x in XDimension) l.Add(x.ToObservation(XDimensionDataItem.TypeId));
                if (!XDimensionDataSet.IsNullOrEmpty()) foreach (var x in XDimensionDataSet) l.Add(x.ToObservation(XDimensionDataItem.TypeId));
                if (!XDimensionTable.IsNullOrEmpty()) foreach (var x in XDimensionTable) l.Add(x.ToObservation(XDimensionDataItem.TypeId));
                if (!XDimensionTimeSeries.IsNullOrEmpty()) foreach (var x in XDimensionTimeSeries) l.Add(x.ToObservation(XDimensionDataItem.TypeId));

                if (!YDimension.IsNullOrEmpty()) foreach (var x in YDimension) l.Add(x.ToObservation(YDimensionDataItem.TypeId));
                if (!YDimensionDataSet.IsNullOrEmpty()) foreach (var x in YDimensionDataSet) l.Add(x.ToObservation(YDimensionDataItem.TypeId));
                if (!YDimensionTable.IsNullOrEmpty()) foreach (var x in YDimensionTable) l.Add(x.ToObservation(YDimensionDataItem.TypeId));
                if (!YDimensionTimeSeries.IsNullOrEmpty()) foreach (var x in YDimensionTimeSeries) l.Add(x.ToObservation(YDimensionDataItem.TypeId));

                if (!ZDimension.IsNullOrEmpty()) foreach (var x in ZDimension) l.Add(x.ToObservation(ZDimensionDataItem.TypeId));
                if (!ZDimensionDataSet.IsNullOrEmpty()) foreach (var x in ZDimensionDataSet) l.Add(x.ToObservation(ZDimensionDataItem.TypeId));
                if (!ZDimensionTable.IsNullOrEmpty()) foreach (var x in ZDimensionTable) l.Add(x.ToObservation(ZDimensionDataItem.TypeId));
                if (!ZDimensionTimeSeries.IsNullOrEmpty()) foreach (var x in ZDimensionTimeSeries) l.Add(x.ToObservation(ZDimensionDataItem.TypeId));


                return l;
            }
        }
        /// <summary>
        /// The <c>Acceleration</c> samples reported with the scalar VALUE representation.
        /// Positive rate of change of velocity.
        /// </summary>
        [JsonPropertyName("Acceleration")]
        public IEnumerable<JsonSampleValue> Acceleration { get; set; }

        /// <summary>
        /// The <c>Acceleration</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AccelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> AccelerationDataSet { get; set; }

        /// <summary>
        /// The <c>Acceleration</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AccelerationTable")]
        public IEnumerable<JsonSampleTable> AccelerationTable { get; set; }

        /// <summary>
        /// The <c>Acceleration</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AccelerationTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AccelerationTimeSeries { get; set; }


        /// <summary>
        /// The <c>AccumulatedTime</c> samples reported with the scalar VALUE representation.
        /// Accumulated time for an activity or event.
        /// </summary>
        [JsonPropertyName("AccumulatedTime")]
        public IEnumerable<JsonSampleValue> AccumulatedTime { get; set; }

        /// <summary>
        /// The <c>AccumulatedTime</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AccumulatedTimeDataSet")]
        public IEnumerable<JsonSampleDataSet> AccumulatedTimeDataSet { get; set; }

        /// <summary>
        /// The <c>AccumulatedTime</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AccumulatedTimeTable")]
        public IEnumerable<JsonSampleTable> AccumulatedTimeTable { get; set; }

        /// <summary>
        /// The <c>AccumulatedTime</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AccumulatedTimeTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AccumulatedTimeTimeSeries { get; set; }


        /// <summary>
        /// The <c>Amperage</c> samples reported with the scalar VALUE representation.
        /// Strength of electrical current.**DEPRECATED** in *Version 1.6*. Replaced by `AMPERAGE_AC` and `AMPERAGE_DC`.
        /// </summary>
        [JsonPropertyName("Amperage")]
        public IEnumerable<JsonSampleValue> Amperage { get; set; }

        /// <summary>
        /// The <c>Amperage</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AmperageDataSet")]
        public IEnumerable<JsonSampleDataSet> AmperageDataSet { get; set; }

        /// <summary>
        /// The <c>Amperage</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AmperageTable")]
        public IEnumerable<JsonSampleTable> AmperageTable { get; set; }

        /// <summary>
        /// The <c>Amperage</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AmperageTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AmperageTimeSeries { get; set; }


        /// <summary>
        /// The <c>AmperageAC</c> samples reported with the scalar VALUE representation.
        /// Electrical current that reverses direction at regular short intervals.
        /// </summary>
        [JsonPropertyName("AmperageAC")]
        public IEnumerable<JsonSampleValue> AmperageAC { get; set; }

        /// <summary>
        /// The <c>AmperageAC</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AmperageACDataSet")]
        public IEnumerable<JsonSampleDataSet> AmperageACDataSet { get; set; }

        /// <summary>
        /// The <c>AmperageAC</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AmperageACTable")]
        public IEnumerable<JsonSampleTable> AmperageACTable { get; set; }

        /// <summary>
        /// The <c>AmperageAC</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AmperageACTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AmperageACTimeSeries { get; set; }


        /// <summary>
        /// The <c>AmperageDC</c> samples reported with the scalar VALUE representation.
        /// Electric current flowing in one direction only.
        /// </summary>
        [JsonPropertyName("AmperageDC")]
        public IEnumerable<JsonSampleValue> AmperageDC { get; set; }

        /// <summary>
        /// The <c>AmperageDC</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AmperageDCDataSet")]
        public IEnumerable<JsonSampleDataSet> AmperageDCDataSet { get; set; }

        /// <summary>
        /// The <c>AmperageDC</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AmperageDCTable")]
        public IEnumerable<JsonSampleTable> AmperageDCTable { get; set; }

        /// <summary>
        /// The <c>AmperageDC</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AmperageDCTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AmperageDCTimeSeries { get; set; }


        /// <summary>
        /// The <c>Angle</c> samples reported with the scalar VALUE representation.
        /// Angular position.
        /// </summary>
        [JsonPropertyName("Angle")]
        public IEnumerable<JsonSampleValue> Angle { get; set; }

        /// <summary>
        /// The <c>Angle</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AngleDataSet")]
        public IEnumerable<JsonSampleDataSet> AngleDataSet { get; set; }

        /// <summary>
        /// The <c>Angle</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AngleTable")]
        public IEnumerable<JsonSampleTable> AngleTable { get; set; }

        /// <summary>
        /// The <c>Angle</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AngleTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AngleTimeSeries { get; set; }


        /// <summary>
        /// The <c>AngularAcceleration</c> samples reported with the scalar VALUE representation.
        /// Positive rate of change of angular velocity.
        /// </summary>
        [JsonPropertyName("AngularAcceleration")]
        public IEnumerable<JsonSampleValue> AngularAcceleration { get; set; }

        /// <summary>
        /// The <c>AngularAcceleration</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AngularAccelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> AngularAccelerationDataSet { get; set; }

        /// <summary>
        /// The <c>AngularAcceleration</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AngularAccelerationTable")]
        public IEnumerable<JsonSampleTable> AngularAccelerationTable { get; set; }

        /// <summary>
        /// The <c>AngularAcceleration</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AngularAccelerationTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AngularAccelerationTimeSeries { get; set; }


        /// <summary>
        /// The <c>AngularDeceleration</c> samples reported with the scalar VALUE representation.
        /// Negative rate of change of angular velocity.
        /// </summary>
        [JsonPropertyName("AngularDeceleration")]
        public IEnumerable<JsonSampleValue> AngularDeceleration { get; set; }

        /// <summary>
        /// The <c>AngularDeceleration</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AngularDecelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> AngularDecelerationDataSet { get; set; }

        /// <summary>
        /// The <c>AngularDeceleration</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AngularDecelerationTable")]
        public IEnumerable<JsonSampleTable> AngularDecelerationTable { get; set; }

        /// <summary>
        /// The <c>AngularDeceleration</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AngularDecelerationTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AngularDecelerationTimeSeries { get; set; }


        /// <summary>
        /// The <c>AngularVelocity</c> samples reported with the scalar VALUE representation.
        /// Rate of change of angular position.
        /// </summary>
        [JsonPropertyName("AngularVelocity")]
        public IEnumerable<JsonSampleValue> AngularVelocity { get; set; }

        /// <summary>
        /// The <c>AngularVelocity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AngularVelocityDataSet")]
        public IEnumerable<JsonSampleDataSet> AngularVelocityDataSet { get; set; }

        /// <summary>
        /// The <c>AngularVelocity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AngularVelocityTable")]
        public IEnumerable<JsonSampleTable> AngularVelocityTable { get; set; }

        /// <summary>
        /// The <c>AngularVelocity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AngularVelocityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AngularVelocityTimeSeries { get; set; }


        /// <summary>
        /// The <c>AssetUpdateRate</c> samples reported with the scalar VALUE representation.
        /// Average rate of change of values for assets in the MTConnect streams. The average is computed over a rolling window defined by the implementation.
        /// </summary>
        [JsonPropertyName("AssetUpdateRate")]
        public IEnumerable<JsonSampleValue> AssetUpdateRate { get; set; }

        /// <summary>
        /// The <c>AssetUpdateRate</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AssetUpdateRateDataSet")]
        public IEnumerable<JsonSampleDataSet> AssetUpdateRateDataSet { get; set; }

        /// <summary>
        /// The <c>AssetUpdateRate</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AssetUpdateRateTable")]
        public IEnumerable<JsonSampleTable> AssetUpdateRateTable { get; set; }

        /// <summary>
        /// The <c>AssetUpdateRate</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AssetUpdateRateTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AssetUpdateRateTimeSeries { get; set; }


        /// <summary>
        /// The <c>AxisFeedrate</c> samples reported with the scalar VALUE representation.
        /// Feedrate of a linear axis.
        /// </summary>
        [JsonPropertyName("AxisFeedrate")]
        public IEnumerable<JsonSampleValue> AxisFeedrate { get; set; }

        /// <summary>
        /// The <c>AxisFeedrate</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AxisFeedrateDataSet")]
        public IEnumerable<JsonSampleDataSet> AxisFeedrateDataSet { get; set; }

        /// <summary>
        /// The <c>AxisFeedrate</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AxisFeedrateTable")]
        public IEnumerable<JsonSampleTable> AxisFeedrateTable { get; set; }

        /// <summary>
        /// The <c>AxisFeedrate</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("AxisFeedrateTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> AxisFeedrateTimeSeries { get; set; }


        /// <summary>
        /// The <c>BatteryCapacity</c> samples reported with the scalar VALUE representation.
        /// Maximum rated charge a battery is capable of maintaining based on the battery discharging at a specified current over a specified time period.
        /// </summary>
        [JsonPropertyName("BatteryCapacity")]
        public IEnumerable<JsonSampleValue> BatteryCapacity { get; set; }

        /// <summary>
        /// The <c>BatteryCapacity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("BatteryCapacityDataSet")]
        public IEnumerable<JsonSampleDataSet> BatteryCapacityDataSet { get; set; }

        /// <summary>
        /// The <c>BatteryCapacity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("BatteryCapacityTable")]
        public IEnumerable<JsonSampleTable> BatteryCapacityTable { get; set; }

        /// <summary>
        /// The <c>BatteryCapacity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("BatteryCapacityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> BatteryCapacityTimeSeries { get; set; }


        /// <summary>
        /// The <c>BatteryCharge</c> samples reported with the scalar VALUE representation.
        /// Value of the battery's present capacity expressed as a percentage of the battery's maximum rated capacity.
        /// </summary>
        [JsonPropertyName("BatteryCharge")]
        public IEnumerable<JsonSampleValue> BatteryCharge { get; set; }

        /// <summary>
        /// The <c>BatteryCharge</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("BatteryChargeDataSet")]
        public IEnumerable<JsonSampleDataSet> BatteryChargeDataSet { get; set; }

        /// <summary>
        /// The <c>BatteryCharge</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("BatteryChargeTable")]
        public IEnumerable<JsonSampleTable> BatteryChargeTable { get; set; }

        /// <summary>
        /// The <c>BatteryCharge</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("BatteryChargeTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> BatteryChargeTimeSeries { get; set; }


        /// <summary>
        /// The <c>CapacityFluid</c> samples reported with the scalar VALUE representation.
        /// Maximum amount of fluid that can be held by a container.
        /// </summary>
        [JsonPropertyName("CapacityFluid")]
        public IEnumerable<JsonSampleValue> CapacityFluid { get; set; }

        /// <summary>
        /// The <c>CapacityFluid</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CapacityFluidDataSet")]
        public IEnumerable<JsonSampleDataSet> CapacityFluidDataSet { get; set; }

        /// <summary>
        /// The <c>CapacityFluid</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CapacityFluidTable")]
        public IEnumerable<JsonSampleTable> CapacityFluidTable { get; set; }

        /// <summary>
        /// The <c>CapacityFluid</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("CapacityFluidTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> CapacityFluidTimeSeries { get; set; }


        /// <summary>
        /// The <c>CapacitySpatial</c> samples reported with the scalar VALUE representation.
        /// Maximum amount of material that can be held by a container.
        /// </summary>
        [JsonPropertyName("CapacitySpatial")]
        public IEnumerable<JsonSampleValue> CapacitySpatial { get; set; }

        /// <summary>
        /// The <c>CapacitySpatial</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CapacitySpatialDataSet")]
        public IEnumerable<JsonSampleDataSet> CapacitySpatialDataSet { get; set; }

        /// <summary>
        /// The <c>CapacitySpatial</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CapacitySpatialTable")]
        public IEnumerable<JsonSampleTable> CapacitySpatialTable { get; set; }

        /// <summary>
        /// The <c>CapacitySpatial</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("CapacitySpatialTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> CapacitySpatialTimeSeries { get; set; }


        /// <summary>
        /// The <c>ChargeRate</c> samples reported with the scalar VALUE representation.
        /// Value of the current being supplied to the Component for the purpose of charging.
        /// </summary>
        [JsonPropertyName("ChargeRate")]
        public IEnumerable<JsonSampleValue> ChargeRate { get; set; }

        /// <summary>
        /// The <c>ChargeRate</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ChargeRateDataSet")]
        public IEnumerable<JsonSampleDataSet> ChargeRateDataSet { get; set; }

        /// <summary>
        /// The <c>ChargeRate</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ChargeRateTable")]
        public IEnumerable<JsonSampleTable> ChargeRateTable { get; set; }

        /// <summary>
        /// The <c>ChargeRate</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ChargeRateTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ChargeRateTimeSeries { get; set; }


        /// <summary>
        /// The <c>Concentration</c> samples reported with the scalar VALUE representation.
        /// Percentage of one component within a mixture of components.
        /// </summary>
        [JsonPropertyName("Concentration")]
        public IEnumerable<JsonSampleValue> Concentration { get; set; }

        /// <summary>
        /// The <c>Concentration</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ConcentrationDataSet")]
        public IEnumerable<JsonSampleDataSet> ConcentrationDataSet { get; set; }

        /// <summary>
        /// The <c>Concentration</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ConcentrationTable")]
        public IEnumerable<JsonSampleTable> ConcentrationTable { get; set; }

        /// <summary>
        /// The <c>Concentration</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ConcentrationTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ConcentrationTimeSeries { get; set; }


        /// <summary>
        /// The <c>Conductivity</c> samples reported with the scalar VALUE representation.
        /// Ability of a material to conduct electricity.
        /// </summary>
        [JsonPropertyName("Conductivity")]
        public IEnumerable<JsonSampleValue> Conductivity { get; set; }

        /// <summary>
        /// The <c>Conductivity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ConductivityDataSet")]
        public IEnumerable<JsonSampleDataSet> ConductivityDataSet { get; set; }

        /// <summary>
        /// The <c>Conductivity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ConductivityTable")]
        public IEnumerable<JsonSampleTable> ConductivityTable { get; set; }

        /// <summary>
        /// The <c>Conductivity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ConductivityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ConductivityTimeSeries { get; set; }


        /// <summary>
        /// The <c>CuttingSpeed</c> samples reported with the scalar VALUE representation.
        /// Speed difference (relative velocity) between the cutting mechanism and the surface of the workpiece it is operating on.
        /// </summary>
        [JsonPropertyName("CuttingSpeed")]
        public IEnumerable<JsonSampleValue> CuttingSpeed { get; set; }

        /// <summary>
        /// The <c>CuttingSpeed</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CuttingSpeedDataSet")]
        public IEnumerable<JsonSampleDataSet> CuttingSpeedDataSet { get; set; }

        /// <summary>
        /// The <c>CuttingSpeed</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CuttingSpeedTable")]
        public IEnumerable<JsonSampleTable> CuttingSpeedTable { get; set; }

        /// <summary>
        /// The <c>CuttingSpeed</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("CuttingSpeedTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> CuttingSpeedTimeSeries { get; set; }


        /// <summary>
        /// The <c>Deceleration</c> samples reported with the scalar VALUE representation.
        /// Negative rate of change of velocity.
        /// </summary>
        [JsonPropertyName("Deceleration")]
        public IEnumerable<JsonSampleValue> Deceleration { get; set; }

        /// <summary>
        /// The <c>Deceleration</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DecelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> DecelerationDataSet { get; set; }

        /// <summary>
        /// The <c>Deceleration</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DecelerationTable")]
        public IEnumerable<JsonSampleTable> DecelerationTable { get; set; }

        /// <summary>
        /// The <c>Deceleration</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DecelerationTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DecelerationTimeSeries { get; set; }


        /// <summary>
        /// The <c>Density</c> samples reported with the scalar VALUE representation.
        /// Volumetric mass of a material per unit volume of that material.
        /// </summary>
        [JsonPropertyName("Density")]
        public IEnumerable<JsonSampleValue> Density { get; set; }

        /// <summary>
        /// The <c>Density</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DensityDataSet")]
        public IEnumerable<JsonSampleDataSet> DensityDataSet { get; set; }

        /// <summary>
        /// The <c>Density</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DensityTable")]
        public IEnumerable<JsonSampleTable> DensityTable { get; set; }

        /// <summary>
        /// The <c>Density</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DensityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DensityTimeSeries { get; set; }


        /// <summary>
        /// The <c>DepositionAccelerationVolumetric</c> samples reported with the scalar VALUE representation.
        /// Rate of change in spatial volume of material deposited in an additive manufacturing process.
        /// </summary>
        [JsonPropertyName("DepositionAccelerationVolumetric")]
        public IEnumerable<JsonSampleValue> DepositionAccelerationVolumetric { get; set; }

        /// <summary>
        /// The <c>DepositionAccelerationVolumetric</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DepositionAccelerationVolumetricDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionAccelerationVolumetricDataSet { get; set; }

        /// <summary>
        /// The <c>DepositionAccelerationVolumetric</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DepositionAccelerationVolumetricTable")]
        public IEnumerable<JsonSampleTable> DepositionAccelerationVolumetricTable { get; set; }

        /// <summary>
        /// The <c>DepositionAccelerationVolumetric</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DepositionAccelerationVolumetricTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DepositionAccelerationVolumetricTimeSeries { get; set; }


        /// <summary>
        /// The <c>DepositionDensity</c> samples reported with the scalar VALUE representation.
        /// Density of the material deposited in an additive manufacturing process per unit of volume.
        /// </summary>
        [JsonPropertyName("DepositionDensity")]
        public IEnumerable<JsonSampleValue> DepositionDensity { get; set; }

        /// <summary>
        /// The <c>DepositionDensity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DepositionDensityDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionDensityDataSet { get; set; }

        /// <summary>
        /// The <c>DepositionDensity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DepositionDensityTable")]
        public IEnumerable<JsonSampleTable> DepositionDensityTable { get; set; }

        /// <summary>
        /// The <c>DepositionDensity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DepositionDensityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DepositionDensityTimeSeries { get; set; }


        /// <summary>
        /// The <c>DepositionMass</c> samples reported with the scalar VALUE representation.
        /// Mass of the material deposited in an additive manufacturing process.
        /// </summary>
        [JsonPropertyName("DepositionMass")]
        public IEnumerable<JsonSampleValue> DepositionMass { get; set; }

        /// <summary>
        /// The <c>DepositionMass</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DepositionMassDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionMassDataSet { get; set; }

        /// <summary>
        /// The <c>DepositionMass</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DepositionMassTable")]
        public IEnumerable<JsonSampleTable> DepositionMassTable { get; set; }

        /// <summary>
        /// The <c>DepositionMass</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DepositionMassTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DepositionMassTimeSeries { get; set; }


        /// <summary>
        /// The <c>DepositionRateVolumetric</c> samples reported with the scalar VALUE representation.
        /// Rate at which a spatial volume of material is deposited in an additive manufacturing process.
        /// </summary>
        [JsonPropertyName("DepositionRateVolumetric")]
        public IEnumerable<JsonSampleValue> DepositionRateVolumetric { get; set; }

        /// <summary>
        /// The <c>DepositionRateVolumetric</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DepositionRateVolumetricDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionRateVolumetricDataSet { get; set; }

        /// <summary>
        /// The <c>DepositionRateVolumetric</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DepositionRateVolumetricTable")]
        public IEnumerable<JsonSampleTable> DepositionRateVolumetricTable { get; set; }

        /// <summary>
        /// The <c>DepositionRateVolumetric</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DepositionRateVolumetricTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DepositionRateVolumetricTimeSeries { get; set; }


        /// <summary>
        /// The <c>DepositionVolume</c> samples reported with the scalar VALUE representation.
        /// Spatial volume of material to be deposited in an additive manufacturing process.
        /// </summary>
        [JsonPropertyName("DepositionVolume")]
        public IEnumerable<JsonSampleValue> DepositionVolume { get; set; }

        /// <summary>
        /// The <c>DepositionVolume</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DepositionVolumeDataSet")]
        public IEnumerable<JsonSampleDataSet> DepositionVolumeDataSet { get; set; }

        /// <summary>
        /// The <c>DepositionVolume</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DepositionVolumeTable")]
        public IEnumerable<JsonSampleTable> DepositionVolumeTable { get; set; }

        /// <summary>
        /// The <c>DepositionVolume</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DepositionVolumeTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DepositionVolumeTimeSeries { get; set; }


        /// <summary>
        /// The <c>DewPoint</c> samples reported with the scalar VALUE representation.
        /// Temperature at which moisture begins to condense, corresponding to saturation for a given absolute humidity.
        /// </summary>
        [JsonPropertyName("DewPoint")]
        public IEnumerable<JsonSampleValue> DewPoint { get; set; }

        /// <summary>
        /// The <c>DewPoint</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DewPointDataSet")]
        public IEnumerable<JsonSampleDataSet> DewPointDataSet { get; set; }

        /// <summary>
        /// The <c>DewPoint</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DewPointTable")]
        public IEnumerable<JsonSampleTable> DewPointTable { get; set; }

        /// <summary>
        /// The <c>DewPoint</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DewPointTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DewPointTimeSeries { get; set; }


        /// <summary>
        /// The <c>Diameter</c> samples reported with the scalar VALUE representation.
        /// Dimension of a diameter.
        /// </summary>
        [JsonPropertyName("Diameter")]
        public IEnumerable<JsonSampleValue> Diameter { get; set; }

        /// <summary>
        /// The <c>Diameter</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DiameterDataSet")]
        public IEnumerable<JsonSampleDataSet> DiameterDataSet { get; set; }

        /// <summary>
        /// The <c>Diameter</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DiameterTable")]
        public IEnumerable<JsonSampleTable> DiameterTable { get; set; }

        /// <summary>
        /// The <c>Diameter</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DiameterTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DiameterTimeSeries { get; set; }


        /// <summary>
        /// The <c>DischargeRate</c> samples reported with the scalar VALUE representation.
        /// Value of current being drawn from the Component.
        /// </summary>
        [JsonPropertyName("DischargeRate")]
        public IEnumerable<JsonSampleValue> DischargeRate { get; set; }

        /// <summary>
        /// The <c>DischargeRate</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DischargeRateDataSet")]
        public IEnumerable<JsonSampleDataSet> DischargeRateDataSet { get; set; }

        /// <summary>
        /// The <c>DischargeRate</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DischargeRateTable")]
        public IEnumerable<JsonSampleTable> DischargeRateTable { get; set; }

        /// <summary>
        /// The <c>DischargeRate</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DischargeRateTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DischargeRateTimeSeries { get; set; }


        /// <summary>
        /// The <c>Displacement</c> samples reported with the scalar VALUE representation.
        /// Change in position of an object.
        /// </summary>
        [JsonPropertyName("Displacement")]
        public IEnumerable<JsonSampleValue> Displacement { get; set; }

        /// <summary>
        /// The <c>Displacement</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DisplacementDataSet")]
        public IEnumerable<JsonSampleDataSet> DisplacementDataSet { get; set; }

        /// <summary>
        /// The <c>Displacement</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DisplacementTable")]
        public IEnumerable<JsonSampleTable> DisplacementTable { get; set; }

        /// <summary>
        /// The <c>Displacement</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DisplacementTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DisplacementTimeSeries { get; set; }


        /// <summary>
        /// The <c>DisplacementAngular</c> samples reported with the scalar VALUE representation.
        /// Absolute value of the change in angular position around a vector
        /// </summary>
        [JsonPropertyName("DisplacementAngular")]
        public IEnumerable<JsonSampleValue> DisplacementAngular { get; set; }

        /// <summary>
        /// The <c>DisplacementAngular</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DisplacementAngularDataSet")]
        public IEnumerable<JsonSampleDataSet> DisplacementAngularDataSet { get; set; }

        /// <summary>
        /// The <c>DisplacementAngular</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DisplacementAngularTable")]
        public IEnumerable<JsonSampleTable> DisplacementAngularTable { get; set; }

        /// <summary>
        /// The <c>DisplacementAngular</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DisplacementAngularTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DisplacementAngularTimeSeries { get; set; }


        /// <summary>
        /// The <c>DisplacementLinear</c> samples reported with the scalar VALUE representation.
        /// Absolute value of the change in position along a vector.
        /// </summary>
        [JsonPropertyName("DisplacementLinear")]
        public IEnumerable<JsonSampleValue> DisplacementLinear { get; set; }

        /// <summary>
        /// The <c>DisplacementLinear</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DisplacementLinearDataSet")]
        public IEnumerable<JsonSampleDataSet> DisplacementLinearDataSet { get; set; }

        /// <summary>
        /// The <c>DisplacementLinear</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DisplacementLinearTable")]
        public IEnumerable<JsonSampleTable> DisplacementLinearTable { get; set; }

        /// <summary>
        /// The <c>DisplacementLinear</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("DisplacementLinearTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> DisplacementLinearTimeSeries { get; set; }


        /// <summary>
        /// The <c>ElectricalEnergy</c> samples reported with the scalar VALUE representation.
        /// Wattage used or generated by a component over an interval of time.
        /// </summary>
        [JsonPropertyName("ElectricalEnergy")]
        public IEnumerable<JsonSampleValue> ElectricalEnergy { get; set; }

        /// <summary>
        /// The <c>ElectricalEnergy</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ElectricalEnergyDataSet")]
        public IEnumerable<JsonSampleDataSet> ElectricalEnergyDataSet { get; set; }

        /// <summary>
        /// The <c>ElectricalEnergy</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ElectricalEnergyTable")]
        public IEnumerable<JsonSampleTable> ElectricalEnergyTable { get; set; }

        /// <summary>
        /// The <c>ElectricalEnergy</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ElectricalEnergyTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ElectricalEnergyTimeSeries { get; set; }


        /// <summary>
        /// The <c>EquipmentTimer</c> samples reported with the scalar VALUE representation.
        /// Amount of time a piece of equipment or a sub-part of a piece of equipment has performed specific activities.
        /// </summary>
        [JsonPropertyName("EquipmentTimer")]
        public IEnumerable<JsonSampleValue> EquipmentTimer { get; set; }

        /// <summary>
        /// The <c>EquipmentTimer</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("EquipmentTimerDataSet")]
        public IEnumerable<JsonSampleDataSet> EquipmentTimerDataSet { get; set; }

        /// <summary>
        /// The <c>EquipmentTimer</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("EquipmentTimerTable")]
        public IEnumerable<JsonSampleTable> EquipmentTimerTable { get; set; }

        /// <summary>
        /// The <c>EquipmentTimer</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("EquipmentTimerTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> EquipmentTimerTimeSeries { get; set; }


        /// <summary>
        /// The <c>FillHeight</c> samples reported with the scalar VALUE representation.
        /// Amount of a substance in a container.
        /// </summary>
        [JsonPropertyName("FillHeight")]
        public IEnumerable<JsonSampleValue> FillHeight { get; set; }

        /// <summary>
        /// The <c>FillHeight</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FillHeightDataSet")]
        public IEnumerable<JsonSampleDataSet> FillHeightDataSet { get; set; }

        /// <summary>
        /// The <c>FillHeight</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FillHeightTable")]
        public IEnumerable<JsonSampleTable> FillHeightTable { get; set; }

        /// <summary>
        /// The <c>FillHeight</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("FillHeightTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> FillHeightTimeSeries { get; set; }


        /// <summary>
        /// The <c>FillLevel</c> samples reported with the scalar VALUE representation.
        /// Amount of a substance remaining compared to the planned maximum amount of that substance.
        /// </summary>
        [JsonPropertyName("FillLevel")]
        public IEnumerable<JsonSampleValue> FillLevel { get; set; }

        /// <summary>
        /// The <c>FillLevel</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FillLevelDataSet")]
        public IEnumerable<JsonSampleDataSet> FillLevelDataSet { get; set; }

        /// <summary>
        /// The <c>FillLevel</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FillLevelTable")]
        public IEnumerable<JsonSampleTable> FillLevelTable { get; set; }

        /// <summary>
        /// The <c>FillLevel</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("FillLevelTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> FillLevelTimeSeries { get; set; }


        /// <summary>
        /// The <c>Flow</c> samples reported with the scalar VALUE representation.
        /// Rate of flow of a fluid.
        /// </summary>
        [JsonPropertyName("Flow")]
        public IEnumerable<JsonSampleValue> Flow { get; set; }

        /// <summary>
        /// The <c>Flow</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FlowDataSet")]
        public IEnumerable<JsonSampleDataSet> FlowDataSet { get; set; }

        /// <summary>
        /// The <c>Flow</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FlowTable")]
        public IEnumerable<JsonSampleTable> FlowTable { get; set; }

        /// <summary>
        /// The <c>Flow</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("FlowTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> FlowTimeSeries { get; set; }


        /// <summary>
        /// The <c>FollowingError</c> samples reported with the scalar VALUE representation.
        /// Difference between actual and commanded position at any specific point in time during a motion.
        /// </summary>
        [JsonPropertyName("FollowingError")]
        public IEnumerable<JsonSampleValue> FollowingError { get; set; }

        /// <summary>
        /// The <c>FollowingError</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FollowingErrorDataSet")]
        public IEnumerable<JsonSampleDataSet> FollowingErrorDataSet { get; set; }

        /// <summary>
        /// The <c>FollowingError</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FollowingErrorTable")]
        public IEnumerable<JsonSampleTable> FollowingErrorTable { get; set; }

        /// <summary>
        /// The <c>FollowingError</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("FollowingErrorTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> FollowingErrorTimeSeries { get; set; }


        /// <summary>
        /// The <c>FollowingErrorAngular</c> samples reported with the scalar VALUE representation.
        /// Angular difference between the commanded encoder/resolver position and the actual encoder/resolver position at any specified point in time during a motion.
        /// </summary>
        [JsonPropertyName("FollowingErrorAngular")]
        public IEnumerable<JsonSampleValue> FollowingErrorAngular { get; set; }

        /// <summary>
        /// The <c>FollowingErrorAngular</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FollowingErrorAngularDataSet")]
        public IEnumerable<JsonSampleDataSet> FollowingErrorAngularDataSet { get; set; }

        /// <summary>
        /// The <c>FollowingErrorAngular</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FollowingErrorAngularTable")]
        public IEnumerable<JsonSampleTable> FollowingErrorAngularTable { get; set; }

        /// <summary>
        /// The <c>FollowingErrorAngular</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("FollowingErrorAngularTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> FollowingErrorAngularTimeSeries { get; set; }


        /// <summary>
        /// The <c>FollowingErrorLinear</c> samples reported with the scalar VALUE representation.
        /// Difference between the commanded encoder/resolver position and the actual encoder/resolver position at any specified point in time during a motion.
        /// </summary>
        [JsonPropertyName("FollowingErrorLinear")]
        public IEnumerable<JsonSampleValue> FollowingErrorLinear { get; set; }

        /// <summary>
        /// The <c>FollowingErrorLinear</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FollowingErrorLinearDataSet")]
        public IEnumerable<JsonSampleDataSet> FollowingErrorLinearDataSet { get; set; }

        /// <summary>
        /// The <c>FollowingErrorLinear</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FollowingErrorLinearTable")]
        public IEnumerable<JsonSampleTable> FollowingErrorLinearTable { get; set; }

        /// <summary>
        /// The <c>FollowingErrorLinear</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("FollowingErrorLinearTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> FollowingErrorLinearTimeSeries { get; set; }


        /// <summary>
        /// The <c>Frequency</c> samples reported with the scalar VALUE representation.
        /// Number of occurrences of a repeating event per unit time.
        /// </summary>
        [JsonPropertyName("Frequency")]
        public IEnumerable<JsonSampleValue> Frequency { get; set; }

        /// <summary>
        /// The <c>Frequency</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FrequencyDataSet")]
        public IEnumerable<JsonSampleDataSet> FrequencyDataSet { get; set; }

        /// <summary>
        /// The <c>Frequency</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FrequencyTable")]
        public IEnumerable<JsonSampleTable> FrequencyTable { get; set; }

        /// <summary>
        /// The <c>Frequency</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("FrequencyTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> FrequencyTimeSeries { get; set; }


        /// <summary>
        /// The <c>GlobalPosition</c> samples reported with the scalar VALUE representation.
        /// Position in three-dimensional space.**DEPRECATED** in Version 1.1.
        /// </summary>
        [JsonPropertyName("GlobalPosition")]
        public IEnumerable<JsonSampleValue> GlobalPosition { get; set; }

        /// <summary>
        /// The <c>GlobalPosition</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("GlobalPositionDataSet")]
        public IEnumerable<JsonSampleDataSet> GlobalPositionDataSet { get; set; }

        /// <summary>
        /// The <c>GlobalPosition</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("GlobalPositionTable")]
        public IEnumerable<JsonSampleTable> GlobalPositionTable { get; set; }

        /// <summary>
        /// The <c>GlobalPosition</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("GlobalPositionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> GlobalPositionTimeSeries { get; set; }


        /// <summary>
        /// The <c>GravitationalAcceleration</c> samples reported with the scalar VALUE representation.
        /// Acceleration relative to Earth's gravity of 9.80665 `METER/SECOND^2`.
        /// </summary>
        [JsonPropertyName("GravitationalAcceleration")]
        public IEnumerable<JsonSampleValue> GravitationalAcceleration { get; set; }

        /// <summary>
        /// The <c>GravitationalAcceleration</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("GravitationalAccelerationDataSet")]
        public IEnumerable<JsonSampleDataSet> GravitationalAccelerationDataSet { get; set; }

        /// <summary>
        /// The <c>GravitationalAcceleration</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("GravitationalAccelerationTable")]
        public IEnumerable<JsonSampleTable> GravitationalAccelerationTable { get; set; }

        /// <summary>
        /// The <c>GravitationalAcceleration</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("GravitationalAccelerationTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> GravitationalAccelerationTimeSeries { get; set; }


        /// <summary>
        /// The <c>GravitationalForce</c> samples reported with the scalar VALUE representation.
        /// Force relative to earth's gravity.
        /// </summary>
        [JsonPropertyName("GravitationalForce")]
        public IEnumerable<JsonSampleValue> GravitationalForce { get; set; }

        /// <summary>
        /// The <c>GravitationalForce</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("GravitationalForceDataSet")]
        public IEnumerable<JsonSampleDataSet> GravitationalForceDataSet { get; set; }

        /// <summary>
        /// The <c>GravitationalForce</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("GravitationalForceTable")]
        public IEnumerable<JsonSampleTable> GravitationalForceTable { get; set; }

        /// <summary>
        /// The <c>GravitationalForce</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("GravitationalForceTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> GravitationalForceTimeSeries { get; set; }


        /// <summary>
        /// The <c>HumidityAbsolute</c> samples reported with the scalar VALUE representation.
        /// Amount of water vapor expressed in grams per cubic meter.
        /// </summary>
        [JsonPropertyName("HumidityAbsolute")]
        public IEnumerable<JsonSampleValue> HumidityAbsolute { get; set; }

        /// <summary>
        /// The <c>HumidityAbsolute</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("HumidityAbsoluteDataSet")]
        public IEnumerable<JsonSampleDataSet> HumidityAbsoluteDataSet { get; set; }

        /// <summary>
        /// The <c>HumidityAbsolute</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("HumidityAbsoluteTable")]
        public IEnumerable<JsonSampleTable> HumidityAbsoluteTable { get; set; }

        /// <summary>
        /// The <c>HumidityAbsolute</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("HumidityAbsoluteTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> HumidityAbsoluteTimeSeries { get; set; }


        /// <summary>
        /// The <c>HumidityRelative</c> samples reported with the scalar VALUE representation.
        /// Amount of water vapor present expressed as a percent to reach saturation at the same temperature.
        /// </summary>
        [JsonPropertyName("HumidityRelative")]
        public IEnumerable<JsonSampleValue> HumidityRelative { get; set; }

        /// <summary>
        /// The <c>HumidityRelative</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("HumidityRelativeDataSet")]
        public IEnumerable<JsonSampleDataSet> HumidityRelativeDataSet { get; set; }

        /// <summary>
        /// The <c>HumidityRelative</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("HumidityRelativeTable")]
        public IEnumerable<JsonSampleTable> HumidityRelativeTable { get; set; }

        /// <summary>
        /// The <c>HumidityRelative</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("HumidityRelativeTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> HumidityRelativeTimeSeries { get; set; }


        /// <summary>
        /// The <c>HumiditySpecific</c> samples reported with the scalar VALUE representation.
        /// Ratio of the water vapor present over the total weight of the water vapor and air present expressed as a percent.
        /// </summary>
        [JsonPropertyName("HumiditySpecific")]
        public IEnumerable<JsonSampleValue> HumiditySpecific { get; set; }

        /// <summary>
        /// The <c>HumiditySpecific</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("HumiditySpecificDataSet")]
        public IEnumerable<JsonSampleDataSet> HumiditySpecificDataSet { get; set; }

        /// <summary>
        /// The <c>HumiditySpecific</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("HumiditySpecificTable")]
        public IEnumerable<JsonSampleTable> HumiditySpecificTable { get; set; }

        /// <summary>
        /// The <c>HumiditySpecific</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("HumiditySpecificTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> HumiditySpecificTimeSeries { get; set; }


        /// <summary>
        /// The <c>Length</c> samples reported with the scalar VALUE representation.
        /// Length of an object.
        /// </summary>
        [JsonPropertyName("Length")]
        public IEnumerable<JsonSampleValue> Length { get; set; }

        /// <summary>
        /// The <c>Length</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LengthDataSet")]
        public IEnumerable<JsonSampleDataSet> LengthDataSet { get; set; }

        /// <summary>
        /// The <c>Length</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LengthTable")]
        public IEnumerable<JsonSampleTable> LengthTable { get; set; }

        /// <summary>
        /// The <c>Length</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("LengthTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> LengthTimeSeries { get; set; }


        /// <summary>
        /// The <c>Level</c> samples reported with the scalar VALUE representation.
        /// Level of a resource.**DEPRECATED** in *Version 1.2*.  See `FILL_LEVEL`.
        /// </summary>
        [JsonPropertyName("Level")]
        public IEnumerable<JsonSampleValue> Level { get; set; }

        /// <summary>
        /// The <c>Level</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LevelDataSet")]
        public IEnumerable<JsonSampleDataSet> LevelDataSet { get; set; }

        /// <summary>
        /// The <c>Level</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LevelTable")]
        public IEnumerable<JsonSampleTable> LevelTable { get; set; }

        /// <summary>
        /// The <c>Level</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("LevelTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> LevelTimeSeries { get; set; }


        /// <summary>
        /// The <c>LinearForce</c> samples reported with the scalar VALUE representation.
        /// Force applied to a mass in one direction only.
        /// </summary>
        [JsonPropertyName("LinearForce")]
        public IEnumerable<JsonSampleValue> LinearForce { get; set; }

        /// <summary>
        /// The <c>LinearForce</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LinearForceDataSet")]
        public IEnumerable<JsonSampleDataSet> LinearForceDataSet { get; set; }

        /// <summary>
        /// The <c>LinearForce</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LinearForceTable")]
        public IEnumerable<JsonSampleTable> LinearForceTable { get; set; }

        /// <summary>
        /// The <c>LinearForce</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("LinearForceTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> LinearForceTimeSeries { get; set; }


        /// <summary>
        /// The <c>Load</c> samples reported with the scalar VALUE representation.
        /// Actual versus the standard rating of a piece of equipment.
        /// </summary>
        [JsonPropertyName("Load")]
        public IEnumerable<JsonSampleValue> Load { get; set; }

        /// <summary>
        /// The <c>Load</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LoadDataSet")]
        public IEnumerable<JsonSampleDataSet> LoadDataSet { get; set; }

        /// <summary>
        /// The <c>Load</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LoadTable")]
        public IEnumerable<JsonSampleTable> LoadTable { get; set; }

        /// <summary>
        /// The <c>Load</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("LoadTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> LoadTimeSeries { get; set; }


        /// <summary>
        /// The <c>Mass</c> samples reported with the scalar VALUE representation.
        /// Mass of an object(s) or an amount of material.
        /// </summary>
        [JsonPropertyName("Mass")]
        public IEnumerable<JsonSampleValue> Mass { get; set; }

        /// <summary>
        /// The <c>Mass</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MassDataSet")]
        public IEnumerable<JsonSampleDataSet> MassDataSet { get; set; }

        /// <summary>
        /// The <c>Mass</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MassTable")]
        public IEnumerable<JsonSampleTable> MassTable { get; set; }

        /// <summary>
        /// The <c>Mass</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("MassTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> MassTimeSeries { get; set; }


        /// <summary>
        /// The <c>ObservationUpdateRate</c> samples reported with the scalar VALUE representation.
        /// Average rate of change of values for data items in the MTConnect streams. The average is computed over a rolling window defined by the implementation.
        /// </summary>
        [JsonPropertyName("ObservationUpdateRate")]
        public IEnumerable<JsonSampleValue> ObservationUpdateRate { get; set; }

        /// <summary>
        /// The <c>ObservationUpdateRate</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ObservationUpdateRateDataSet")]
        public IEnumerable<JsonSampleDataSet> ObservationUpdateRateDataSet { get; set; }

        /// <summary>
        /// The <c>ObservationUpdateRate</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ObservationUpdateRateTable")]
        public IEnumerable<JsonSampleTable> ObservationUpdateRateTable { get; set; }

        /// <summary>
        /// The <c>ObservationUpdateRate</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ObservationUpdateRateTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ObservationUpdateRateTimeSeries { get; set; }


        /// <summary>
        /// The <c>Openness</c> samples reported with the scalar VALUE representation.
        /// Percentage open where 100% is fully open and 0% is fully closed.
        /// </summary>
        [JsonPropertyName("Openness")]
        public IEnumerable<JsonSampleValue> Openness { get; set; }

        /// <summary>
        /// The <c>Openness</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("OpennessDataSet")]
        public IEnumerable<JsonSampleDataSet> OpennessDataSet { get; set; }

        /// <summary>
        /// The <c>Openness</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("OpennessTable")]
        public IEnumerable<JsonSampleTable> OpennessTable { get; set; }

        /// <summary>
        /// The <c>Openness</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("OpennessTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> OpennessTimeSeries { get; set; }


        /// <summary>
        /// The <c>Orientation</c> samples reported with the scalar VALUE representation.
        /// Angular position of a plane or vector relative to a cartesian coordinate system
        /// </summary>
        [JsonPropertyName("Orientation")]
        public IEnumerable<JsonSampleValue> Orientation { get; set; }

        /// <summary>
        /// The <c>Orientation</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("OrientationDataSet")]
        public IEnumerable<JsonSampleDataSet> OrientationDataSet { get; set; }

        /// <summary>
        /// The <c>Orientation</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("OrientationTable")]
        public IEnumerable<JsonSampleTable> OrientationTable { get; set; }

        /// <summary>
        /// The <c>Orientation</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("OrientationTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> OrientationTimeSeries { get; set; }


        /// <summary>
        /// The <c>ParticleCount</c> samples reported with the scalar VALUE representation.
        /// Number of particles counted by their size or other characteristics.
        /// </summary>
        [JsonPropertyName("ParticleCount")]
        public IEnumerable<JsonSampleValue> ParticleCount { get; set; }

        /// <summary>
        /// The <c>ParticleCount</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ParticleCountDataSet")]
        public IEnumerable<JsonSampleDataSet> ParticleCountDataSet { get; set; }

        /// <summary>
        /// The <c>ParticleCount</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ParticleCountTable")]
        public IEnumerable<JsonSampleTable> ParticleCountTable { get; set; }

        /// <summary>
        /// The <c>ParticleCount</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ParticleCountTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ParticleCountTimeSeries { get; set; }


        /// <summary>
        /// The <c>ParticleSize</c> samples reported with the scalar VALUE representation.
        /// Size of particles counted by their size or other characteristics.
        /// </summary>
        [JsonPropertyName("ParticleSize")]
        public IEnumerable<JsonSampleValue> ParticleSize { get; set; }

        /// <summary>
        /// The <c>ParticleSize</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ParticleSizeDataSet")]
        public IEnumerable<JsonSampleDataSet> ParticleSizeDataSet { get; set; }

        /// <summary>
        /// The <c>ParticleSize</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ParticleSizeTable")]
        public IEnumerable<JsonSampleTable> ParticleSizeTable { get; set; }

        /// <summary>
        /// The <c>ParticleSize</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ParticleSizeTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ParticleSizeTimeSeries { get; set; }


        /// <summary>
        /// The <c>PathFeedrate</c> samples reported with the scalar VALUE representation.
        /// Feedrate for the axes, or a single axis, associated with a Path component.
        /// </summary>
        [JsonPropertyName("PathFeedrate")]
        public IEnumerable<JsonSampleValue> PathFeedrate { get; set; }

        /// <summary>
        /// The <c>PathFeedrate</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PathFeedrateDataSet")]
        public IEnumerable<JsonSampleDataSet> PathFeedrateDataSet { get; set; }

        /// <summary>
        /// The <c>PathFeedrate</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PathFeedrateTable")]
        public IEnumerable<JsonSampleTable> PathFeedrateTable { get; set; }

        /// <summary>
        /// The <c>PathFeedrate</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PathFeedrateTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PathFeedrateTimeSeries { get; set; }


        /// <summary>
        /// The <c>PathFeedratePerRevolution</c> samples reported with the scalar VALUE representation.
        /// Feedrate for the axes, or a single axis.
        /// </summary>
        [JsonPropertyName("PathFeedratePerRevolution")]
        public IEnumerable<JsonSampleValue> PathFeedratePerRevolution { get; set; }

        /// <summary>
        /// The <c>PathFeedratePerRevolution</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PathFeedratePerRevolutionDataSet")]
        public IEnumerable<JsonSampleDataSet> PathFeedratePerRevolutionDataSet { get; set; }

        /// <summary>
        /// The <c>PathFeedratePerRevolution</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PathFeedratePerRevolutionTable")]
        public IEnumerable<JsonSampleTable> PathFeedratePerRevolutionTable { get; set; }

        /// <summary>
        /// The <c>PathFeedratePerRevolution</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PathFeedratePerRevolutionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PathFeedratePerRevolutionTimeSeries { get; set; }


        /// <summary>
        /// The <c>PathPosition</c> samples reported with the scalar VALUE representation.
        /// Position of a control point associated with a Controller or a Path.
        /// </summary>
        [JsonPropertyName("PathPosition")]
        public IEnumerable<JsonSampleValue> PathPosition { get; set; }

        /// <summary>
        /// The <c>PathPosition</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PathPositionDataSet")]
        public IEnumerable<JsonSampleDataSet> PathPositionDataSet { get; set; }

        /// <summary>
        /// The <c>PathPosition</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PathPositionTable")]
        public IEnumerable<JsonSampleTable> PathPositionTable { get; set; }

        /// <summary>
        /// The <c>PathPosition</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PathPositionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PathPositionTimeSeries { get; set; }


        /// <summary>
        /// The <c>PH</c> samples reported with the scalar VALUE representation.
        /// Acidity or alkalinity of a solution.
        /// </summary>
        [JsonPropertyName("PH")]
        public IEnumerable<JsonSampleValue> PH { get; set; }

        /// <summary>
        /// The <c>PH</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PHDataSet")]
        public IEnumerable<JsonSampleDataSet> PHDataSet { get; set; }

        /// <summary>
        /// The <c>PH</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PHTable")]
        public IEnumerable<JsonSampleTable> PHTable { get; set; }

        /// <summary>
        /// The <c>PH</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PHTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PHTimeSeries { get; set; }


        /// <summary>
        /// The <c>Position</c> samples reported with the scalar VALUE representation.
        /// Point along an axis in a cartesian coordinate system.
        /// </summary>
        [JsonPropertyName("Position")]
        public IEnumerable<JsonSampleValue> Position { get; set; }

        /// <summary>
        /// The <c>Position</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PositionDataSet")]
        public IEnumerable<JsonSampleDataSet> PositionDataSet { get; set; }

        /// <summary>
        /// The <c>Position</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PositionTable")]
        public IEnumerable<JsonSampleTable> PositionTable { get; set; }

        /// <summary>
        /// The <c>Position</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PositionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PositionTimeSeries { get; set; }


        /// <summary>
        /// The <c>PositionCartesian</c> samples reported with the scalar VALUE representation.
        /// Point in a cartesian coordinate system.
        /// </summary>
        [JsonPropertyName("PositionCartesian")]
        public IEnumerable<JsonSampleValue> PositionCartesian { get; set; }

        /// <summary>
        /// The <c>PositionCartesian</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PositionCartesianDataSet")]
        public IEnumerable<JsonSampleDataSet> PositionCartesianDataSet { get; set; }

        /// <summary>
        /// The <c>PositionCartesian</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PositionCartesianTable")]
        public IEnumerable<JsonSampleTable> PositionCartesianTable { get; set; }

        /// <summary>
        /// The <c>PositionCartesian</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PositionCartesianTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PositionCartesianTimeSeries { get; set; }


        /// <summary>
        /// The <c>PowerFactor</c> samples reported with the scalar VALUE representation.
        /// Ratio of real power flowing to a load to the apparent power in that AC circuit.
        /// </summary>
        [JsonPropertyName("PowerFactor")]
        public IEnumerable<JsonSampleValue> PowerFactor { get; set; }

        /// <summary>
        /// The <c>PowerFactor</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PowerFactorDataSet")]
        public IEnumerable<JsonSampleDataSet> PowerFactorDataSet { get; set; }

        /// <summary>
        /// The <c>PowerFactor</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PowerFactorTable")]
        public IEnumerable<JsonSampleTable> PowerFactorTable { get; set; }

        /// <summary>
        /// The <c>PowerFactor</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PowerFactorTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PowerFactorTimeSeries { get; set; }


        /// <summary>
        /// The <c>Pressure</c> samples reported with the scalar VALUE representation.
        /// Force per unit area measured relative to atmospheric pressure. Commonly referred to as gauge pressure.
        /// </summary>
        [JsonPropertyName("Pressure")]
        public IEnumerable<JsonSampleValue> Pressure { get; set; }

        /// <summary>
        /// The <c>Pressure</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PressureDataSet")]
        public IEnumerable<JsonSampleDataSet> PressureDataSet { get; set; }

        /// <summary>
        /// The <c>Pressure</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PressureTable")]
        public IEnumerable<JsonSampleTable> PressureTable { get; set; }

        /// <summary>
        /// The <c>Pressure</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PressureTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PressureTimeSeries { get; set; }


        /// <summary>
        /// The <c>PressureAbsolute</c> samples reported with the scalar VALUE representation.
        /// Force per unit area measured relative to a vacuum.
        /// </summary>
        [JsonPropertyName("PressureAbsolute")]
        public IEnumerable<JsonSampleValue> PressureAbsolute { get; set; }

        /// <summary>
        /// The <c>PressureAbsolute</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PressureAbsoluteDataSet")]
        public IEnumerable<JsonSampleDataSet> PressureAbsoluteDataSet { get; set; }

        /// <summary>
        /// The <c>PressureAbsolute</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PressureAbsoluteTable")]
        public IEnumerable<JsonSampleTable> PressureAbsoluteTable { get; set; }

        /// <summary>
        /// The <c>PressureAbsolute</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PressureAbsoluteTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PressureAbsoluteTimeSeries { get; set; }


        /// <summary>
        /// The <c>PressurizationRate</c> samples reported with the scalar VALUE representation.
        /// Change of pressure per unit time.
        /// </summary>
        [JsonPropertyName("PressurizationRate")]
        public IEnumerable<JsonSampleValue> PressurizationRate { get; set; }

        /// <summary>
        /// The <c>PressurizationRate</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PressurizationRateDataSet")]
        public IEnumerable<JsonSampleDataSet> PressurizationRateDataSet { get; set; }

        /// <summary>
        /// The <c>PressurizationRate</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PressurizationRateTable")]
        public IEnumerable<JsonSampleTable> PressurizationRateTable { get; set; }

        /// <summary>
        /// The <c>PressurizationRate</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("PressurizationRateTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> PressurizationRateTimeSeries { get; set; }


        /// <summary>
        /// The <c>ProcessTimer</c> samples reported with the scalar VALUE representation.
        /// Amount of time a piece of equipment has performed different types of activities associated with the process being performed at that piece of equipment.
        /// </summary>
        [JsonPropertyName("ProcessTimer")]
        public IEnumerable<JsonSampleValue> ProcessTimer { get; set; }

        /// <summary>
        /// The <c>ProcessTimer</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProcessTimerDataSet")]
        public IEnumerable<JsonSampleDataSet> ProcessTimerDataSet { get; set; }

        /// <summary>
        /// The <c>ProcessTimer</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProcessTimerTable")]
        public IEnumerable<JsonSampleTable> ProcessTimerTable { get; set; }

        /// <summary>
        /// The <c>ProcessTimer</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ProcessTimerTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ProcessTimerTimeSeries { get; set; }


        /// <summary>
        /// The <c>Resistance</c> samples reported with the scalar VALUE representation.
        /// Degree to which a substance opposes the passage of an electric current.
        /// </summary>
        [JsonPropertyName("Resistance")]
        public IEnumerable<JsonSampleValue> Resistance { get; set; }

        /// <summary>
        /// The <c>Resistance</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ResistanceDataSet")]
        public IEnumerable<JsonSampleDataSet> ResistanceDataSet { get; set; }

        /// <summary>
        /// The <c>Resistance</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ResistanceTable")]
        public IEnumerable<JsonSampleTable> ResistanceTable { get; set; }

        /// <summary>
        /// The <c>Resistance</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ResistanceTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ResistanceTimeSeries { get; set; }


        /// <summary>
        /// The <c>Resistivity</c> samples reported with the scalar VALUE representation.
        /// Inability of a material to conduct electricity.
        /// </summary>
        [JsonPropertyName("Resistivity")]
        public IEnumerable<JsonSampleValue> Resistivity { get; set; }

        /// <summary>
        /// The <c>Resistivity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ResistivityDataSet")]
        public IEnumerable<JsonSampleDataSet> ResistivityDataSet { get; set; }

        /// <summary>
        /// The <c>Resistivity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ResistivityTable")]
        public IEnumerable<JsonSampleTable> ResistivityTable { get; set; }

        /// <summary>
        /// The <c>Resistivity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ResistivityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ResistivityTimeSeries { get; set; }


        /// <summary>
        /// The <c>RotaryVelocity</c> samples reported with the scalar VALUE representation.
        /// Rotational speed of a rotary axis.
        /// </summary>
        [JsonPropertyName("RotaryVelocity")]
        public IEnumerable<JsonSampleValue> RotaryVelocity { get; set; }

        /// <summary>
        /// The <c>RotaryVelocity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("RotaryVelocityDataSet")]
        public IEnumerable<JsonSampleDataSet> RotaryVelocityDataSet { get; set; }

        /// <summary>
        /// The <c>RotaryVelocity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("RotaryVelocityTable")]
        public IEnumerable<JsonSampleTable> RotaryVelocityTable { get; set; }

        /// <summary>
        /// The <c>RotaryVelocity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("RotaryVelocityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> RotaryVelocityTimeSeries { get; set; }


        /// <summary>
        /// The <c>SettlingError</c> samples reported with the scalar VALUE representation.
        /// Difference between actual and commanded position at the end of a motion.
        /// </summary>
        [JsonPropertyName("SettlingError")]
        public IEnumerable<JsonSampleValue> SettlingError { get; set; }

        /// <summary>
        /// The <c>SettlingError</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SettlingErrorDataSet")]
        public IEnumerable<JsonSampleDataSet> SettlingErrorDataSet { get; set; }

        /// <summary>
        /// The <c>SettlingError</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SettlingErrorTable")]
        public IEnumerable<JsonSampleTable> SettlingErrorTable { get; set; }

        /// <summary>
        /// The <c>SettlingError</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("SettlingErrorTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> SettlingErrorTimeSeries { get; set; }


        /// <summary>
        /// The <c>SettlingErrorAngular</c> samples reported with the scalar VALUE representation.
        /// Angular difference between the commanded encoder/resolver position, and the actual encoder/resolver position when motion is complete.
        /// </summary>
        [JsonPropertyName("SettlingErrorAngular")]
        public IEnumerable<JsonSampleValue> SettlingErrorAngular { get; set; }

        /// <summary>
        /// The <c>SettlingErrorAngular</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SettlingErrorAngularDataSet")]
        public IEnumerable<JsonSampleDataSet> SettlingErrorAngularDataSet { get; set; }

        /// <summary>
        /// The <c>SettlingErrorAngular</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SettlingErrorAngularTable")]
        public IEnumerable<JsonSampleTable> SettlingErrorAngularTable { get; set; }

        /// <summary>
        /// The <c>SettlingErrorAngular</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("SettlingErrorAngularTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> SettlingErrorAngularTimeSeries { get; set; }


        /// <summary>
        /// The <c>SettlingErrorLinear</c> samples reported with the scalar VALUE representation.
        /// Difference between the commanded encoder/resolver position, and the actual encoder/resolver position when motion is complete.
        /// </summary>
        [JsonPropertyName("SettlingErrorLinear")]
        public IEnumerable<JsonSampleValue> SettlingErrorLinear { get; set; }

        /// <summary>
        /// The <c>SettlingErrorLinear</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SettlingErrorLinearDataSet")]
        public IEnumerable<JsonSampleDataSet> SettlingErrorLinearDataSet { get; set; }

        /// <summary>
        /// The <c>SettlingErrorLinear</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SettlingErrorLinearTable")]
        public IEnumerable<JsonSampleTable> SettlingErrorLinearTable { get; set; }

        /// <summary>
        /// The <c>SettlingErrorLinear</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("SettlingErrorLinearTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> SettlingErrorLinearTimeSeries { get; set; }


        /// <summary>
        /// The <c>SoundLevel</c> samples reported with the scalar VALUE representation.
        /// Sound level or sound pressure level relative to atmospheric pressure.
        /// </summary>
        [JsonPropertyName("SoundLevel")]
        public IEnumerable<JsonSampleValue> SoundLevel { get; set; }

        /// <summary>
        /// The <c>SoundLevel</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SoundLevelDataSet")]
        public IEnumerable<JsonSampleDataSet> SoundLevelDataSet { get; set; }

        /// <summary>
        /// The <c>SoundLevel</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SoundLevelTable")]
        public IEnumerable<JsonSampleTable> SoundLevelTable { get; set; }

        /// <summary>
        /// The <c>SoundLevel</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("SoundLevelTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> SoundLevelTimeSeries { get; set; }


        /// <summary>
        /// The <c>SpindleSpeed</c> samples reported with the scalar VALUE representation.
        /// Rotational speed of the rotary axis.**DEPRECATED** in *Version 1.2*.  Replaced by `ROTARY_VELOCITY`.
        /// </summary>
        [JsonPropertyName("SpindleSpeed")]
        public IEnumerable<JsonSampleValue> SpindleSpeed { get; set; }

        /// <summary>
        /// The <c>SpindleSpeed</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SpindleSpeedDataSet")]
        public IEnumerable<JsonSampleDataSet> SpindleSpeedDataSet { get; set; }

        /// <summary>
        /// The <c>SpindleSpeed</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SpindleSpeedTable")]
        public IEnumerable<JsonSampleTable> SpindleSpeedTable { get; set; }

        /// <summary>
        /// The <c>SpindleSpeed</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("SpindleSpeedTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> SpindleSpeedTimeSeries { get; set; }


        /// <summary>
        /// The <c>Strain</c> samples reported with the scalar VALUE representation.
        /// Amount of deformation per unit length of an object when a load is applied.
        /// </summary>
        [JsonPropertyName("Strain")]
        public IEnumerable<JsonSampleValue> Strain { get; set; }

        /// <summary>
        /// The <c>Strain</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("StrainDataSet")]
        public IEnumerable<JsonSampleDataSet> StrainDataSet { get; set; }

        /// <summary>
        /// The <c>Strain</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("StrainTable")]
        public IEnumerable<JsonSampleTable> StrainTable { get; set; }

        /// <summary>
        /// The <c>Strain</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("StrainTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> StrainTimeSeries { get; set; }


        /// <summary>
        /// The <c>Temperature</c> samples reported with the scalar VALUE representation.
        /// Degree of hotness or coldness measured on a definite scale.
        /// </summary>
        [JsonPropertyName("Temperature")]
        public IEnumerable<JsonSampleValue> Temperature { get; set; }

        /// <summary>
        /// The <c>Temperature</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("TemperatureDataSet")]
        public IEnumerable<JsonSampleDataSet> TemperatureDataSet { get; set; }

        /// <summary>
        /// The <c>Temperature</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("TemperatureTable")]
        public IEnumerable<JsonSampleTable> TemperatureTable { get; set; }

        /// <summary>
        /// The <c>Temperature</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("TemperatureTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> TemperatureTimeSeries { get; set; }


        /// <summary>
        /// The <c>Tension</c> samples reported with the scalar VALUE representation.
        /// Force that stretches or elongates an object.
        /// </summary>
        [JsonPropertyName("Tension")]
        public IEnumerable<JsonSampleValue> Tension { get; set; }

        /// <summary>
        /// The <c>Tension</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("TensionDataSet")]
        public IEnumerable<JsonSampleDataSet> TensionDataSet { get; set; }

        /// <summary>
        /// The <c>Tension</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("TensionTable")]
        public IEnumerable<JsonSampleTable> TensionTable { get; set; }

        /// <summary>
        /// The <c>Tension</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("TensionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> TensionTimeSeries { get; set; }


        /// <summary>
        /// The <c>Tilt</c> samples reported with the scalar VALUE representation.
        /// Angular displacement.
        /// </summary>
        [JsonPropertyName("Tilt")]
        public IEnumerable<JsonSampleValue> Tilt { get; set; }

        /// <summary>
        /// The <c>Tilt</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("TiltDataSet")]
        public IEnumerable<JsonSampleDataSet> TiltDataSet { get; set; }

        /// <summary>
        /// The <c>Tilt</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("TiltTable")]
        public IEnumerable<JsonSampleTable> TiltTable { get; set; }

        /// <summary>
        /// The <c>Tilt</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("TiltTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> TiltTimeSeries { get; set; }


        /// <summary>
        /// The <c>Torque</c> samples reported with the scalar VALUE representation.
        /// Turning force exerted on an object or by an object.
        /// </summary>
        [JsonPropertyName("Torque")]
        public IEnumerable<JsonSampleValue> Torque { get; set; }

        /// <summary>
        /// The <c>Torque</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("TorqueDataSet")]
        public IEnumerable<JsonSampleDataSet> TorqueDataSet { get; set; }

        /// <summary>
        /// The <c>Torque</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("TorqueTable")]
        public IEnumerable<JsonSampleTable> TorqueTable { get; set; }

        /// <summary>
        /// The <c>Torque</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("TorqueTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> TorqueTimeSeries { get; set; }


        /// <summary>
        /// The <c>Velocity</c> samples reported with the scalar VALUE representation.
        /// Rate of change of position of a Component.
        /// </summary>
        [JsonPropertyName("Velocity")]
        public IEnumerable<JsonSampleValue> Velocity { get; set; }

        /// <summary>
        /// The <c>Velocity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VelocityDataSet")]
        public IEnumerable<JsonSampleDataSet> VelocityDataSet { get; set; }

        /// <summary>
        /// The <c>Velocity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VelocityTable")]
        public IEnumerable<JsonSampleTable> VelocityTable { get; set; }

        /// <summary>
        /// The <c>Velocity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VelocityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VelocityTimeSeries { get; set; }


        /// <summary>
        /// The <c>Viscosity</c> samples reported with the scalar VALUE representation.
        /// Fluid's resistance to flow.
        /// </summary>
        [JsonPropertyName("Viscosity")]
        public IEnumerable<JsonSampleValue> Viscosity { get; set; }

        /// <summary>
        /// The <c>Viscosity</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ViscosityDataSet")]
        public IEnumerable<JsonSampleDataSet> ViscosityDataSet { get; set; }

        /// <summary>
        /// The <c>Viscosity</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ViscosityTable")]
        public IEnumerable<JsonSampleTable> ViscosityTable { get; set; }

        /// <summary>
        /// The <c>Viscosity</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ViscosityTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ViscosityTimeSeries { get; set; }


        /// <summary>
        /// The <c>VoltAmpere</c> samples reported with the scalar VALUE representation.
        /// Apparent power in an electrical circuit, equal to the product of root-mean-square (RMS) voltage and RMS current (commonly referred to as VA).
        /// </summary>
        [JsonPropertyName("VoltAmpere")]
        public IEnumerable<JsonSampleValue> VoltAmpere { get; set; }

        /// <summary>
        /// The <c>VoltAmpere</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VoltAmpereDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltAmpereDataSet { get; set; }

        /// <summary>
        /// The <c>VoltAmpere</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VoltAmpereTable")]
        public IEnumerable<JsonSampleTable> VoltAmpereTable { get; set; }

        /// <summary>
        /// The <c>VoltAmpere</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VoltAmpereTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VoltAmpereTimeSeries { get; set; }


        /// <summary>
        /// The <c>VoltAmpereReactive</c> samples reported with the scalar VALUE representation.
        /// Reactive power in an AC electrical circuit (commonly referred to as VAR).
        /// </summary>
        [JsonPropertyName("VoltAmpereReactive")]
        public IEnumerable<JsonSampleValue> VoltAmpereReactive { get; set; }

        /// <summary>
        /// The <c>VoltAmpereReactive</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VoltAmpereReactiveDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltAmpereReactiveDataSet { get; set; }

        /// <summary>
        /// The <c>VoltAmpereReactive</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VoltAmpereReactiveTable")]
        public IEnumerable<JsonSampleTable> VoltAmpereReactiveTable { get; set; }

        /// <summary>
        /// The <c>VoltAmpereReactive</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VoltAmpereReactiveTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VoltAmpereReactiveTimeSeries { get; set; }


        /// <summary>
        /// The <c>Voltage</c> samples reported with the scalar VALUE representation.
        /// Electrical potential between two points.**DEPRECATED** in *Version 1.6*. Replaced by `VOLTAGE_AC` and `VOLTAGE_DC`.
        /// </summary>
        [JsonPropertyName("Voltage")]
        public IEnumerable<JsonSampleValue> Voltage { get; set; }

        /// <summary>
        /// The <c>Voltage</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VoltageDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltageDataSet { get; set; }

        /// <summary>
        /// The <c>Voltage</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VoltageTable")]
        public IEnumerable<JsonSampleTable> VoltageTable { get; set; }

        /// <summary>
        /// The <c>Voltage</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VoltageTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VoltageTimeSeries { get; set; }


        /// <summary>
        /// The <c>VoltageAC</c> samples reported with the scalar VALUE representation.
        /// Electrical potential between two points in an electrical circuit in which the current periodically reverses direction.
        /// </summary>
        [JsonPropertyName("VoltageAC")]
        public IEnumerable<JsonSampleValue> VoltageAC { get; set; }

        /// <summary>
        /// The <c>VoltageAC</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VoltageACDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltageACDataSet { get; set; }

        /// <summary>
        /// The <c>VoltageAC</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VoltageACTable")]
        public IEnumerable<JsonSampleTable> VoltageACTable { get; set; }

        /// <summary>
        /// The <c>VoltageAC</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VoltageACTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VoltageACTimeSeries { get; set; }


        /// <summary>
        /// The <c>VoltageDC</c> samples reported with the scalar VALUE representation.
        /// Electrical potential between two points in an electrical circuit in which the current is unidirectional.
        /// </summary>
        [JsonPropertyName("VoltageDC")]
        public IEnumerable<JsonSampleValue> VoltageDC { get; set; }

        /// <summary>
        /// The <c>VoltageDC</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VoltageDCDataSet")]
        public IEnumerable<JsonSampleDataSet> VoltageDCDataSet { get; set; }

        /// <summary>
        /// The <c>VoltageDC</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VoltageDCTable")]
        public IEnumerable<JsonSampleTable> VoltageDCTable { get; set; }

        /// <summary>
        /// The <c>VoltageDC</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VoltageDCTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VoltageDCTimeSeries { get; set; }


        /// <summary>
        /// The <c>VolumeFluid</c> samples reported with the scalar VALUE representation.
        /// Fluid volume of an object or container.
        /// </summary>
        [JsonPropertyName("VolumeFluid")]
        public IEnumerable<JsonSampleValue> VolumeFluid { get; set; }

        /// <summary>
        /// The <c>VolumeFluid</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VolumeFluidDataSet")]
        public IEnumerable<JsonSampleDataSet> VolumeFluidDataSet { get; set; }

        /// <summary>
        /// The <c>VolumeFluid</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VolumeFluidTable")]
        public IEnumerable<JsonSampleTable> VolumeFluidTable { get; set; }

        /// <summary>
        /// The <c>VolumeFluid</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VolumeFluidTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VolumeFluidTimeSeries { get; set; }


        /// <summary>
        /// The <c>VolumeSpatial</c> samples reported with the scalar VALUE representation.
        /// Geometric volume of an object or container.
        /// </summary>
        [JsonPropertyName("VolumeSpatial")]
        public IEnumerable<JsonSampleValue> VolumeSpatial { get; set; }

        /// <summary>
        /// The <c>VolumeSpatial</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VolumeSpatialDataSet")]
        public IEnumerable<JsonSampleDataSet> VolumeSpatialDataSet { get; set; }

        /// <summary>
        /// The <c>VolumeSpatial</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VolumeSpatialTable")]
        public IEnumerable<JsonSampleTable> VolumeSpatialTable { get; set; }

        /// <summary>
        /// The <c>VolumeSpatial</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("VolumeSpatialTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> VolumeSpatialTimeSeries { get; set; }


        /// <summary>
        /// The <c>WaterHardness</c> samples reported with the scalar VALUE representation.
        /// Concentration of calcium carbonate (CaCO3) in water
        /// </summary>
        [JsonPropertyName("WaterHardness")]
        public IEnumerable<JsonSampleValue> WaterHardness { get; set; }

        /// <summary>
        /// The <c>WaterHardness</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("WaterHardnessDataSet")]
        public IEnumerable<JsonSampleDataSet> WaterHardnessDataSet { get; set; }

        /// <summary>
        /// The <c>WaterHardness</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("WaterHardnessTable")]
        public IEnumerable<JsonSampleTable> WaterHardnessTable { get; set; }

        /// <summary>
        /// The <c>WaterHardness</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("WaterHardnessTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> WaterHardnessTimeSeries { get; set; }


        /// <summary>
        /// The <c>Wattage</c> samples reported with the scalar VALUE representation.
        /// Power flowing through or dissipated by an electrical circuit or piece of equipment.
        /// </summary>
        [JsonPropertyName("Wattage")]
        public IEnumerable<JsonSampleValue> Wattage { get; set; }

        /// <summary>
        /// The <c>Wattage</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("WattageDataSet")]
        public IEnumerable<JsonSampleDataSet> WattageDataSet { get; set; }

        /// <summary>
        /// The <c>Wattage</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("WattageTable")]
        public IEnumerable<JsonSampleTable> WattageTable { get; set; }

        /// <summary>
        /// The <c>Wattage</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("WattageTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> WattageTimeSeries { get; set; }


        /// <summary>
        /// The <c>XDimension</c> samples reported with the scalar VALUE representation.
        /// Dimension of an entity relative to the X direction of the referenced coordinate system.
        /// </summary>
        [JsonPropertyName("XDimension")]
        public IEnumerable<JsonSampleValue> XDimension { get; set; }

        /// <summary>
        /// The <c>XDimension</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("XDimensionDataSet")]
        public IEnumerable<JsonSampleDataSet> XDimensionDataSet { get; set; }

        /// <summary>
        /// The <c>XDimension</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("XDimensionTable")]
        public IEnumerable<JsonSampleTable> XDimensionTable { get; set; }

        /// <summary>
        /// The <c>XDimension</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("XDimensionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> XDimensionTimeSeries { get; set; }


        /// <summary>
        /// The <c>YDimension</c> samples reported with the scalar VALUE representation.
        /// Dimension of an entity relative to the Y direction of the referenced coordinate system.
        /// </summary>
        [JsonPropertyName("YDimension")]
        public IEnumerable<JsonSampleValue> YDimension { get; set; }

        /// <summary>
        /// The <c>YDimension</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("YDimensionDataSet")]
        public IEnumerable<JsonSampleDataSet> YDimensionDataSet { get; set; }

        /// <summary>
        /// The <c>YDimension</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("YDimensionTable")]
        public IEnumerable<JsonSampleTable> YDimensionTable { get; set; }

        /// <summary>
        /// The <c>YDimension</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("YDimensionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> YDimensionTimeSeries { get; set; }


        /// <summary>
        /// The <c>ZDimension</c> samples reported with the scalar VALUE representation.
        /// Dimension of an entity relative to the Z direction of the referenced coordinate system.
        /// </summary>
        [JsonPropertyName("ZDimension")]
        public IEnumerable<JsonSampleValue> ZDimension { get; set; }

        /// <summary>
        /// The <c>ZDimension</c> samples reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ZDimensionDataSet")]
        public IEnumerable<JsonSampleDataSet> ZDimensionDataSet { get; set; }

        /// <summary>
        /// The <c>ZDimension</c> samples reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ZDimensionTable")]
        public IEnumerable<JsonSampleTable> ZDimensionTable { get; set; }

        /// <summary>
        /// The <c>ZDimension</c> samples reported with the TIME_SERIES representation
        /// (a sequence of values sampled at a fixed rate).
        /// </summary>
        [JsonPropertyName("ZDimensionTimeSeries")]
        public IEnumerable<JsonSampleTimeSeries> ZDimensionTimeSeries { get; set; }



        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonSamples() { }

        /// <summary>
        /// Initializes a new instance from a flat sequence of <paramref name="observations"/>,
        /// routing each one into the typed collection matching its MTConnect type and
        /// representation.
        /// </summary>
        public JsonSamples(IEnumerable<IObservationOutput> observations)
        {
            if (observations != null)
            {
                if (!observations.IsNullOrEmpty())
                {
                    IEnumerable<IObservationOutput> typeObservations;
                    // Add Acceleration
                    typeObservations = observations.Where(o => o.Type == AccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AccelerationTable
                    typeObservations = observations.Where(o => o.Type == AccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AccelerationTable = jsonObservations;
                    }

                    // Add AccelerationTimeSeries
                    typeObservations = observations.Where(o => o.Type == AccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AccelerationTimeSeries = jsonObservations;
                    }


                    // Add AccumulatedTime
                    typeObservations = observations.Where(o => o.Type == AccumulatedTimeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AccumulatedTimeTable
                    typeObservations = observations.Where(o => o.Type == AccumulatedTimeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AccumulatedTimeTable = jsonObservations;
                    }

                    // Add AccumulatedTimeTimeSeries
                    typeObservations = observations.Where(o => o.Type == AccumulatedTimeDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AccumulatedTimeTimeSeries = jsonObservations;
                    }


                    // Add Amperage
                    typeObservations = observations.Where(o => o.Type == AmperageDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AmperageTable
                    typeObservations = observations.Where(o => o.Type == AmperageDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AmperageTable = jsonObservations;
                    }

                    // Add AmperageTimeSeries
                    typeObservations = observations.Where(o => o.Type == AmperageDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AmperageTimeSeries = jsonObservations;
                    }


                    // Add AmperageAC
                    typeObservations = observations.Where(o => o.Type == AmperageACDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AmperageACTable
                    typeObservations = observations.Where(o => o.Type == AmperageACDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AmperageACTable = jsonObservations;
                    }

                    // Add AmperageACTimeSeries
                    typeObservations = observations.Where(o => o.Type == AmperageACDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AmperageACTimeSeries = jsonObservations;
                    }


                    // Add AmperageDC
                    typeObservations = observations.Where(o => o.Type == AmperageDCDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AmperageDCTable
                    typeObservations = observations.Where(o => o.Type == AmperageDCDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AmperageDCTable = jsonObservations;
                    }

                    // Add AmperageDCTimeSeries
                    typeObservations = observations.Where(o => o.Type == AmperageDCDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AmperageDCTimeSeries = jsonObservations;
                    }


                    // Add Angle
                    typeObservations = observations.Where(o => o.Type == AngleDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AngleTable
                    typeObservations = observations.Where(o => o.Type == AngleDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AngleTable = jsonObservations;
                    }

                    // Add AngleTimeSeries
                    typeObservations = observations.Where(o => o.Type == AngleDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AngleTimeSeries = jsonObservations;
                    }


                    // Add AngularAcceleration
                    typeObservations = observations.Where(o => o.Type == AngularAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AngularAccelerationTable
                    typeObservations = observations.Where(o => o.Type == AngularAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AngularAccelerationTable = jsonObservations;
                    }

                    // Add AngularAccelerationTimeSeries
                    typeObservations = observations.Where(o => o.Type == AngularAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AngularAccelerationTimeSeries = jsonObservations;
                    }


                    // Add AngularDeceleration
                    typeObservations = observations.Where(o => o.Type == AngularDecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AngularDecelerationTable
                    typeObservations = observations.Where(o => o.Type == AngularDecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AngularDecelerationTable = jsonObservations;
                    }

                    // Add AngularDecelerationTimeSeries
                    typeObservations = observations.Where(o => o.Type == AngularDecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AngularDecelerationTimeSeries = jsonObservations;
                    }


                    // Add AngularVelocity
                    typeObservations = observations.Where(o => o.Type == AngularVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AngularVelocityTable
                    typeObservations = observations.Where(o => o.Type == AngularVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AngularVelocityTable = jsonObservations;
                    }

                    // Add AngularVelocityTimeSeries
                    typeObservations = observations.Where(o => o.Type == AngularVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AngularVelocityTimeSeries = jsonObservations;
                    }


                    // Add AssetUpdateRate
                    typeObservations = observations.Where(o => o.Type == AssetUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AssetUpdateRateTable
                    typeObservations = observations.Where(o => o.Type == AssetUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AssetUpdateRateTable = jsonObservations;
                    }

                    // Add AssetUpdateRateTimeSeries
                    typeObservations = observations.Where(o => o.Type == AssetUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AssetUpdateRateTimeSeries = jsonObservations;
                    }


                    // Add AxisFeedrate
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add AxisFeedrateTable
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        AxisFeedrateTable = jsonObservations;
                    }

                    // Add AxisFeedrateTimeSeries
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        AxisFeedrateTimeSeries = jsonObservations;
                    }


                    // Add BatteryCapacity
                    typeObservations = observations.Where(o => o.Type == BatteryCapacityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add BatteryCapacityTable
                    typeObservations = observations.Where(o => o.Type == BatteryCapacityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        BatteryCapacityTable = jsonObservations;
                    }

                    // Add BatteryCapacityTimeSeries
                    typeObservations = observations.Where(o => o.Type == BatteryCapacityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        BatteryCapacityTimeSeries = jsonObservations;
                    }


                    // Add BatteryCharge
                    typeObservations = observations.Where(o => o.Type == BatteryChargeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add BatteryChargeTable
                    typeObservations = observations.Where(o => o.Type == BatteryChargeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        BatteryChargeTable = jsonObservations;
                    }

                    // Add BatteryChargeTimeSeries
                    typeObservations = observations.Where(o => o.Type == BatteryChargeDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        BatteryChargeTimeSeries = jsonObservations;
                    }


                    // Add CapacityFluid
                    typeObservations = observations.Where(o => o.Type == CapacityFluidDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add CapacityFluidTable
                    typeObservations = observations.Where(o => o.Type == CapacityFluidDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        CapacityFluidTable = jsonObservations;
                    }

                    // Add CapacityFluidTimeSeries
                    typeObservations = observations.Where(o => o.Type == CapacityFluidDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        CapacityFluidTimeSeries = jsonObservations;
                    }


                    // Add CapacitySpatial
                    typeObservations = observations.Where(o => o.Type == CapacitySpatialDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add CapacitySpatialTable
                    typeObservations = observations.Where(o => o.Type == CapacitySpatialDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        CapacitySpatialTable = jsonObservations;
                    }

                    // Add CapacitySpatialTimeSeries
                    typeObservations = observations.Where(o => o.Type == CapacitySpatialDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        CapacitySpatialTimeSeries = jsonObservations;
                    }


                    // Add ChargeRate
                    typeObservations = observations.Where(o => o.Type == ChargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ChargeRateTable
                    typeObservations = observations.Where(o => o.Type == ChargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ChargeRateTable = jsonObservations;
                    }

                    // Add ChargeRateTimeSeries
                    typeObservations = observations.Where(o => o.Type == ChargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ChargeRateTimeSeries = jsonObservations;
                    }


                    // Add Concentration
                    typeObservations = observations.Where(o => o.Type == ConcentrationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ConcentrationTable
                    typeObservations = observations.Where(o => o.Type == ConcentrationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ConcentrationTable = jsonObservations;
                    }

                    // Add ConcentrationTimeSeries
                    typeObservations = observations.Where(o => o.Type == ConcentrationDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ConcentrationTimeSeries = jsonObservations;
                    }


                    // Add Conductivity
                    typeObservations = observations.Where(o => o.Type == ConductivityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ConductivityTable
                    typeObservations = observations.Where(o => o.Type == ConductivityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ConductivityTable = jsonObservations;
                    }

                    // Add ConductivityTimeSeries
                    typeObservations = observations.Where(o => o.Type == ConductivityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ConductivityTimeSeries = jsonObservations;
                    }


                    // Add CuttingSpeed
                    typeObservations = observations.Where(o => o.Type == CuttingSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add CuttingSpeedTable
                    typeObservations = observations.Where(o => o.Type == CuttingSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        CuttingSpeedTable = jsonObservations;
                    }

                    // Add CuttingSpeedTimeSeries
                    typeObservations = observations.Where(o => o.Type == CuttingSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        CuttingSpeedTimeSeries = jsonObservations;
                    }


                    // Add Deceleration
                    typeObservations = observations.Where(o => o.Type == DecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DecelerationTable
                    typeObservations = observations.Where(o => o.Type == DecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DecelerationTable = jsonObservations;
                    }

                    // Add DecelerationTimeSeries
                    typeObservations = observations.Where(o => o.Type == DecelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DecelerationTimeSeries = jsonObservations;
                    }


                    // Add Density
                    typeObservations = observations.Where(o => o.Type == DensityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DensityTable
                    typeObservations = observations.Where(o => o.Type == DensityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DensityTable = jsonObservations;
                    }

                    // Add DensityTimeSeries
                    typeObservations = observations.Where(o => o.Type == DensityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DensityTimeSeries = jsonObservations;
                    }


                    // Add DepositionAccelerationVolumetric
                    typeObservations = observations.Where(o => o.Type == DepositionAccelerationVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DepositionAccelerationVolumetricTable
                    typeObservations = observations.Where(o => o.Type == DepositionAccelerationVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DepositionAccelerationVolumetricTable = jsonObservations;
                    }

                    // Add DepositionAccelerationVolumetricTimeSeries
                    typeObservations = observations.Where(o => o.Type == DepositionAccelerationVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DepositionAccelerationVolumetricTimeSeries = jsonObservations;
                    }


                    // Add DepositionDensity
                    typeObservations = observations.Where(o => o.Type == DepositionDensityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DepositionDensityTable
                    typeObservations = observations.Where(o => o.Type == DepositionDensityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DepositionDensityTable = jsonObservations;
                    }

                    // Add DepositionDensityTimeSeries
                    typeObservations = observations.Where(o => o.Type == DepositionDensityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DepositionDensityTimeSeries = jsonObservations;
                    }


                    // Add DepositionMass
                    typeObservations = observations.Where(o => o.Type == DepositionMassDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DepositionMassTable
                    typeObservations = observations.Where(o => o.Type == DepositionMassDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DepositionMassTable = jsonObservations;
                    }

                    // Add DepositionMassTimeSeries
                    typeObservations = observations.Where(o => o.Type == DepositionMassDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DepositionMassTimeSeries = jsonObservations;
                    }


                    // Add DepositionRateVolumetric
                    typeObservations = observations.Where(o => o.Type == DepositionRateVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DepositionRateVolumetricTable
                    typeObservations = observations.Where(o => o.Type == DepositionRateVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DepositionRateVolumetricTable = jsonObservations;
                    }

                    // Add DepositionRateVolumetricTimeSeries
                    typeObservations = observations.Where(o => o.Type == DepositionRateVolumetricDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DepositionRateVolumetricTimeSeries = jsonObservations;
                    }


                    // Add DepositionVolume
                    typeObservations = observations.Where(o => o.Type == DepositionVolumeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DepositionVolumeTable
                    typeObservations = observations.Where(o => o.Type == DepositionVolumeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DepositionVolumeTable = jsonObservations;
                    }

                    // Add DepositionVolumeTimeSeries
                    typeObservations = observations.Where(o => o.Type == DepositionVolumeDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DepositionVolumeTimeSeries = jsonObservations;
                    }


                    // Add DewPoint
                    typeObservations = observations.Where(o => o.Type == DewPointDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DewPointTable
                    typeObservations = observations.Where(o => o.Type == DewPointDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DewPointTable = jsonObservations;
                    }

                    // Add DewPointTimeSeries
                    typeObservations = observations.Where(o => o.Type == DewPointDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DewPointTimeSeries = jsonObservations;
                    }


                    // Add Diameter
                    typeObservations = observations.Where(o => o.Type == DiameterDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DiameterTable
                    typeObservations = observations.Where(o => o.Type == DiameterDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DiameterTable = jsonObservations;
                    }

                    // Add DiameterTimeSeries
                    typeObservations = observations.Where(o => o.Type == DiameterDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DiameterTimeSeries = jsonObservations;
                    }


                    // Add DischargeRate
                    typeObservations = observations.Where(o => o.Type == DischargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DischargeRateTable
                    typeObservations = observations.Where(o => o.Type == DischargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DischargeRateTable = jsonObservations;
                    }

                    // Add DischargeRateTimeSeries
                    typeObservations = observations.Where(o => o.Type == DischargeRateDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DischargeRateTimeSeries = jsonObservations;
                    }


                    // Add Displacement
                    typeObservations = observations.Where(o => o.Type == DisplacementDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DisplacementTable
                    typeObservations = observations.Where(o => o.Type == DisplacementDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DisplacementTable = jsonObservations;
                    }

                    // Add DisplacementTimeSeries
                    typeObservations = observations.Where(o => o.Type == DisplacementDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DisplacementTimeSeries = jsonObservations;
                    }


                    // Add DisplacementAngular
                    typeObservations = observations.Where(o => o.Type == DisplacementAngularDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DisplacementAngularTable
                    typeObservations = observations.Where(o => o.Type == DisplacementAngularDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DisplacementAngularTable = jsonObservations;
                    }

                    // Add DisplacementAngularTimeSeries
                    typeObservations = observations.Where(o => o.Type == DisplacementAngularDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DisplacementAngularTimeSeries = jsonObservations;
                    }


                    // Add DisplacementLinear
                    typeObservations = observations.Where(o => o.Type == DisplacementLinearDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add DisplacementLinearTable
                    typeObservations = observations.Where(o => o.Type == DisplacementLinearDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        DisplacementLinearTable = jsonObservations;
                    }

                    // Add DisplacementLinearTimeSeries
                    typeObservations = observations.Where(o => o.Type == DisplacementLinearDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        DisplacementLinearTimeSeries = jsonObservations;
                    }


                    // Add ElectricalEnergy
                    typeObservations = observations.Where(o => o.Type == ElectricalEnergyDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ElectricalEnergyTable
                    typeObservations = observations.Where(o => o.Type == ElectricalEnergyDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ElectricalEnergyTable = jsonObservations;
                    }

                    // Add ElectricalEnergyTimeSeries
                    typeObservations = observations.Where(o => o.Type == ElectricalEnergyDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ElectricalEnergyTimeSeries = jsonObservations;
                    }


                    // Add EquipmentTimer
                    typeObservations = observations.Where(o => o.Type == EquipmentTimerDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add EquipmentTimerTable
                    typeObservations = observations.Where(o => o.Type == EquipmentTimerDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        EquipmentTimerTable = jsonObservations;
                    }

                    // Add EquipmentTimerTimeSeries
                    typeObservations = observations.Where(o => o.Type == EquipmentTimerDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        EquipmentTimerTimeSeries = jsonObservations;
                    }


                    // Add FillHeight
                    typeObservations = observations.Where(o => o.Type == FillHeightDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        FillHeight = jsonObservations;
                    }

                    // Add FillHeightDataSet
                    typeObservations = observations.Where(o => o.Type == FillHeightDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        FillHeightDataSet = jsonObservations;
                    }

                    // Add FillHeightTable
                    typeObservations = observations.Where(o => o.Type == FillHeightDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        FillHeightTable = jsonObservations;
                    }

                    // Add FillHeightTimeSeries
                    typeObservations = observations.Where(o => o.Type == FillHeightDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        FillHeightTimeSeries = jsonObservations;
                    }


                    // Add FillLevel
                    typeObservations = observations.Where(o => o.Type == FillLevelDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add FillLevelTable
                    typeObservations = observations.Where(o => o.Type == FillLevelDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        FillLevelTable = jsonObservations;
                    }

                    // Add FillLevelTimeSeries
                    typeObservations = observations.Where(o => o.Type == FillLevelDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        FillLevelTimeSeries = jsonObservations;
                    }


                    // Add Flow
                    typeObservations = observations.Where(o => o.Type == FlowDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add FlowTable
                    typeObservations = observations.Where(o => o.Type == FlowDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        FlowTable = jsonObservations;
                    }

                    // Add FlowTimeSeries
                    typeObservations = observations.Where(o => o.Type == FlowDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        FlowTimeSeries = jsonObservations;
                    }


                    // Add FollowingError
                    typeObservations = observations.Where(o => o.Type == FollowingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add FollowingErrorTable
                    typeObservations = observations.Where(o => o.Type == FollowingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        FollowingErrorTable = jsonObservations;
                    }

                    // Add FollowingErrorTimeSeries
                    typeObservations = observations.Where(o => o.Type == FollowingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        FollowingErrorTimeSeries = jsonObservations;
                    }


                    // Add FollowingErrorAngular
                    typeObservations = observations.Where(o => o.Type == FollowingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add FollowingErrorAngularTable
                    typeObservations = observations.Where(o => o.Type == FollowingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        FollowingErrorAngularTable = jsonObservations;
                    }

                    // Add FollowingErrorAngularTimeSeries
                    typeObservations = observations.Where(o => o.Type == FollowingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        FollowingErrorAngularTimeSeries = jsonObservations;
                    }


                    // Add FollowingErrorLinear
                    typeObservations = observations.Where(o => o.Type == FollowingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add FollowingErrorLinearTable
                    typeObservations = observations.Where(o => o.Type == FollowingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        FollowingErrorLinearTable = jsonObservations;
                    }

                    // Add FollowingErrorLinearTimeSeries
                    typeObservations = observations.Where(o => o.Type == FollowingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        FollowingErrorLinearTimeSeries = jsonObservations;
                    }


                    // Add Frequency
                    typeObservations = observations.Where(o => o.Type == FrequencyDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add FrequencyTable
                    typeObservations = observations.Where(o => o.Type == FrequencyDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        FrequencyTable = jsonObservations;
                    }

                    // Add FrequencyTimeSeries
                    typeObservations = observations.Where(o => o.Type == FrequencyDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        FrequencyTimeSeries = jsonObservations;
                    }


                    // Add GlobalPosition
                    typeObservations = observations.Where(o => o.Type == GlobalPositionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add GlobalPositionTable
                    typeObservations = observations.Where(o => o.Type == GlobalPositionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        GlobalPositionTable = jsonObservations;
                    }

                    // Add GlobalPositionTimeSeries
                    typeObservations = observations.Where(o => o.Type == GlobalPositionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        GlobalPositionTimeSeries = jsonObservations;
                    }


                    // Add GravitationalAcceleration
                    typeObservations = observations.Where(o => o.Type == GravitationalAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add GravitationalAccelerationTable
                    typeObservations = observations.Where(o => o.Type == GravitationalAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        GravitationalAccelerationTable = jsonObservations;
                    }

                    // Add GravitationalAccelerationTimeSeries
                    typeObservations = observations.Where(o => o.Type == GravitationalAccelerationDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        GravitationalAccelerationTimeSeries = jsonObservations;
                    }


                    // Add GravitationalForce
                    typeObservations = observations.Where(o => o.Type == GravitationalForceDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add GravitationalForceTable
                    typeObservations = observations.Where(o => o.Type == GravitationalForceDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        GravitationalForceTable = jsonObservations;
                    }

                    // Add GravitationalForceTimeSeries
                    typeObservations = observations.Where(o => o.Type == GravitationalForceDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        GravitationalForceTimeSeries = jsonObservations;
                    }


                    // Add HumidityAbsolute
                    typeObservations = observations.Where(o => o.Type == HumidityAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add HumidityAbsoluteTable
                    typeObservations = observations.Where(o => o.Type == HumidityAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        HumidityAbsoluteTable = jsonObservations;
                    }

                    // Add HumidityAbsoluteTimeSeries
                    typeObservations = observations.Where(o => o.Type == HumidityAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        HumidityAbsoluteTimeSeries = jsonObservations;
                    }


                    // Add HumidityRelative
                    typeObservations = observations.Where(o => o.Type == HumidityRelativeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add HumidityRelativeTable
                    typeObservations = observations.Where(o => o.Type == HumidityRelativeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        HumidityRelativeTable = jsonObservations;
                    }

                    // Add HumidityRelativeTimeSeries
                    typeObservations = observations.Where(o => o.Type == HumidityRelativeDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        HumidityRelativeTimeSeries = jsonObservations;
                    }


                    // Add HumiditySpecific
                    typeObservations = observations.Where(o => o.Type == HumiditySpecificDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add HumiditySpecificTable
                    typeObservations = observations.Where(o => o.Type == HumiditySpecificDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        HumiditySpecificTable = jsonObservations;
                    }

                    // Add HumiditySpecificTimeSeries
                    typeObservations = observations.Where(o => o.Type == HumiditySpecificDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        HumiditySpecificTimeSeries = jsonObservations;
                    }


                    // Add Length
                    typeObservations = observations.Where(o => o.Type == LengthDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add LengthTable
                    typeObservations = observations.Where(o => o.Type == LengthDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        LengthTable = jsonObservations;
                    }

                    // Add LengthTimeSeries
                    typeObservations = observations.Where(o => o.Type == LengthDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        LengthTimeSeries = jsonObservations;
                    }


                    // Add Level
                    typeObservations = observations.Where(o => o.Type == LevelDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add LevelTable
                    typeObservations = observations.Where(o => o.Type == LevelDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        LevelTable = jsonObservations;
                    }

                    // Add LevelTimeSeries
                    typeObservations = observations.Where(o => o.Type == LevelDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        LevelTimeSeries = jsonObservations;
                    }


                    // Add LinearForce
                    typeObservations = observations.Where(o => o.Type == LinearForceDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add LinearForceTable
                    typeObservations = observations.Where(o => o.Type == LinearForceDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        LinearForceTable = jsonObservations;
                    }

                    // Add LinearForceTimeSeries
                    typeObservations = observations.Where(o => o.Type == LinearForceDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        LinearForceTimeSeries = jsonObservations;
                    }


                    // Add Load
                    typeObservations = observations.Where(o => o.Type == LoadDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add LoadTable
                    typeObservations = observations.Where(o => o.Type == LoadDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        LoadTable = jsonObservations;
                    }

                    // Add LoadTimeSeries
                    typeObservations = observations.Where(o => o.Type == LoadDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        LoadTimeSeries = jsonObservations;
                    }


                    // Add Mass
                    typeObservations = observations.Where(o => o.Type == MassDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add MassTable
                    typeObservations = observations.Where(o => o.Type == MassDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        MassTable = jsonObservations;
                    }

                    // Add MassTimeSeries
                    typeObservations = observations.Where(o => o.Type == MassDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        MassTimeSeries = jsonObservations;
                    }


                    // Add ObservationUpdateRate
                    typeObservations = observations.Where(o => o.Type == ObservationUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ObservationUpdateRateTable
                    typeObservations = observations.Where(o => o.Type == ObservationUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ObservationUpdateRateTable = jsonObservations;
                    }

                    // Add ObservationUpdateRateTimeSeries
                    typeObservations = observations.Where(o => o.Type == ObservationUpdateRateDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ObservationUpdateRateTimeSeries = jsonObservations;
                    }


                    // Add Openness
                    typeObservations = observations.Where(o => o.Type == OpennessDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add OpennessTable
                    typeObservations = observations.Where(o => o.Type == OpennessDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        OpennessTable = jsonObservations;
                    }

                    // Add OpennessTimeSeries
                    typeObservations = observations.Where(o => o.Type == OpennessDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        OpennessTimeSeries = jsonObservations;
                    }


                    // Add Orientation
                    typeObservations = observations.Where(o => o.Type == OrientationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add OrientationTable
                    typeObservations = observations.Where(o => o.Type == OrientationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        OrientationTable = jsonObservations;
                    }

                    // Add OrientationTimeSeries
                    typeObservations = observations.Where(o => o.Type == OrientationDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        OrientationTimeSeries = jsonObservations;
                    }


                    // Add ParticleCount
                    typeObservations = observations.Where(o => o.Type == ParticleCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        ParticleCount = jsonObservations;
                    }

                    // Add ParticleCountDataSet
                    typeObservations = observations.Where(o => o.Type == ParticleCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ParticleCountDataSet = jsonObservations;
                    }

                    // Add ParticleCountTable
                    typeObservations = observations.Where(o => o.Type == ParticleCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ParticleCountTable = jsonObservations;
                    }

                    // Add ParticleCountTimeSeries
                    typeObservations = observations.Where(o => o.Type == ParticleCountDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ParticleCountTimeSeries = jsonObservations;
                    }


                    // Add ParticleSize
                    typeObservations = observations.Where(o => o.Type == ParticleSizeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        ParticleSize = jsonObservations;
                    }

                    // Add ParticleSizeDataSet
                    typeObservations = observations.Where(o => o.Type == ParticleSizeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ParticleSizeDataSet = jsonObservations;
                    }

                    // Add ParticleSizeTable
                    typeObservations = observations.Where(o => o.Type == ParticleSizeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ParticleSizeTable = jsonObservations;
                    }

                    // Add ParticleSizeTimeSeries
                    typeObservations = observations.Where(o => o.Type == ParticleSizeDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ParticleSizeTimeSeries = jsonObservations;
                    }


                    // Add PathFeedrate
                    typeObservations = observations.Where(o => o.Type == PathFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PathFeedrateTable
                    typeObservations = observations.Where(o => o.Type == PathFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PathFeedrateTable = jsonObservations;
                    }

                    // Add PathFeedrateTimeSeries
                    typeObservations = observations.Where(o => o.Type == PathFeedrateDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PathFeedrateTimeSeries = jsonObservations;
                    }


                    // Add PathFeedratePerRevolution
                    typeObservations = observations.Where(o => o.Type == PathFeedratePerRevolutionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PathFeedratePerRevolutionTable
                    typeObservations = observations.Where(o => o.Type == PathFeedratePerRevolutionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PathFeedratePerRevolutionTable = jsonObservations;
                    }

                    // Add PathFeedratePerRevolutionTimeSeries
                    typeObservations = observations.Where(o => o.Type == PathFeedratePerRevolutionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PathFeedratePerRevolutionTimeSeries = jsonObservations;
                    }


                    // Add PathPosition
                    typeObservations = observations.Where(o => o.Type == PathPositionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PathPositionTable
                    typeObservations = observations.Where(o => o.Type == PathPositionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PathPositionTable = jsonObservations;
                    }

                    // Add PathPositionTimeSeries
                    typeObservations = observations.Where(o => o.Type == PathPositionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PathPositionTimeSeries = jsonObservations;
                    }


                    // Add PH
                    typeObservations = observations.Where(o => o.Type == PHDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PHTable
                    typeObservations = observations.Where(o => o.Type == PHDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PHTable = jsonObservations;
                    }

                    // Add PHTimeSeries
                    typeObservations = observations.Where(o => o.Type == PHDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PHTimeSeries = jsonObservations;
                    }


                    // Add Position
                    typeObservations = observations.Where(o => o.Type == PositionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PositionTable
                    typeObservations = observations.Where(o => o.Type == PositionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PositionTable = jsonObservations;
                    }

                    // Add PositionTimeSeries
                    typeObservations = observations.Where(o => o.Type == PositionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PositionTimeSeries = jsonObservations;
                    }


                    // Add PositionCartesian
                    typeObservations = observations.Where(o => o.Type == PositionCartesianDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PositionCartesianTable
                    typeObservations = observations.Where(o => o.Type == PositionCartesianDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PositionCartesianTable = jsonObservations;
                    }

                    // Add PositionCartesianTimeSeries
                    typeObservations = observations.Where(o => o.Type == PositionCartesianDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PositionCartesianTimeSeries = jsonObservations;
                    }


                    // Add PowerFactor
                    typeObservations = observations.Where(o => o.Type == PowerFactorDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PowerFactorTable
                    typeObservations = observations.Where(o => o.Type == PowerFactorDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PowerFactorTable = jsonObservations;
                    }

                    // Add PowerFactorTimeSeries
                    typeObservations = observations.Where(o => o.Type == PowerFactorDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PowerFactorTimeSeries = jsonObservations;
                    }


                    // Add Pressure
                    typeObservations = observations.Where(o => o.Type == PressureDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PressureTable
                    typeObservations = observations.Where(o => o.Type == PressureDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PressureTable = jsonObservations;
                    }

                    // Add PressureTimeSeries
                    typeObservations = observations.Where(o => o.Type == PressureDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PressureTimeSeries = jsonObservations;
                    }


                    // Add PressureAbsolute
                    typeObservations = observations.Where(o => o.Type == PressureAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PressureAbsoluteTable
                    typeObservations = observations.Where(o => o.Type == PressureAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PressureAbsoluteTable = jsonObservations;
                    }

                    // Add PressureAbsoluteTimeSeries
                    typeObservations = observations.Where(o => o.Type == PressureAbsoluteDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PressureAbsoluteTimeSeries = jsonObservations;
                    }


                    // Add PressurizationRate
                    typeObservations = observations.Where(o => o.Type == PressurizationRateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add PressurizationRateTable
                    typeObservations = observations.Where(o => o.Type == PressurizationRateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        PressurizationRateTable = jsonObservations;
                    }

                    // Add PressurizationRateTimeSeries
                    typeObservations = observations.Where(o => o.Type == PressurizationRateDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        PressurizationRateTimeSeries = jsonObservations;
                    }


                    // Add ProcessTimer
                    typeObservations = observations.Where(o => o.Type == ProcessTimerDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ProcessTimerTable
                    typeObservations = observations.Where(o => o.Type == ProcessTimerDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ProcessTimerTable = jsonObservations;
                    }

                    // Add ProcessTimerTimeSeries
                    typeObservations = observations.Where(o => o.Type == ProcessTimerDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ProcessTimerTimeSeries = jsonObservations;
                    }


                    // Add Resistance
                    typeObservations = observations.Where(o => o.Type == ResistanceDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ResistanceTable
                    typeObservations = observations.Where(o => o.Type == ResistanceDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ResistanceTable = jsonObservations;
                    }

                    // Add ResistanceTimeSeries
                    typeObservations = observations.Where(o => o.Type == ResistanceDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ResistanceTimeSeries = jsonObservations;
                    }


                    // Add Resistivity
                    typeObservations = observations.Where(o => o.Type == ResistivityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        Resistivity = jsonObservations;
                    }

                    // Add ResistivityDataSet
                    typeObservations = observations.Where(o => o.Type == ResistivityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        ResistivityDataSet = jsonObservations;
                    }

                    // Add ResistivityTable
                    typeObservations = observations.Where(o => o.Type == ResistivityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ResistivityTable = jsonObservations;
                    }

                    // Add ResistivityTimeSeries
                    typeObservations = observations.Where(o => o.Type == ResistivityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ResistivityTimeSeries = jsonObservations;
                    }


                    // Add RotaryVelocity
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add RotaryVelocityTable
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        RotaryVelocityTable = jsonObservations;
                    }

                    // Add RotaryVelocityTimeSeries
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        RotaryVelocityTimeSeries = jsonObservations;
                    }


                    // Add SettlingError
                    typeObservations = observations.Where(o => o.Type == SettlingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add SettlingErrorTable
                    typeObservations = observations.Where(o => o.Type == SettlingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        SettlingErrorTable = jsonObservations;
                    }

                    // Add SettlingErrorTimeSeries
                    typeObservations = observations.Where(o => o.Type == SettlingErrorDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        SettlingErrorTimeSeries = jsonObservations;
                    }


                    // Add SettlingErrorAngular
                    typeObservations = observations.Where(o => o.Type == SettlingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add SettlingErrorAngularTable
                    typeObservations = observations.Where(o => o.Type == SettlingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        SettlingErrorAngularTable = jsonObservations;
                    }

                    // Add SettlingErrorAngularTimeSeries
                    typeObservations = observations.Where(o => o.Type == SettlingErrorAngularDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        SettlingErrorAngularTimeSeries = jsonObservations;
                    }


                    // Add SettlingErrorLinear
                    typeObservations = observations.Where(o => o.Type == SettlingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add SettlingErrorLinearTable
                    typeObservations = observations.Where(o => o.Type == SettlingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        SettlingErrorLinearTable = jsonObservations;
                    }

                    // Add SettlingErrorLinearTimeSeries
                    typeObservations = observations.Where(o => o.Type == SettlingErrorLinearDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        SettlingErrorLinearTimeSeries = jsonObservations;
                    }


                    // Add SoundLevel
                    typeObservations = observations.Where(o => o.Type == SoundLevelDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add SoundLevelTable
                    typeObservations = observations.Where(o => o.Type == SoundLevelDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        SoundLevelTable = jsonObservations;
                    }

                    // Add SoundLevelTimeSeries
                    typeObservations = observations.Where(o => o.Type == SoundLevelDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        SoundLevelTimeSeries = jsonObservations;
                    }


                    // Add SpindleSpeed
                    typeObservations = observations.Where(o => o.Type == SpindleSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add SpindleSpeedTable
                    typeObservations = observations.Where(o => o.Type == SpindleSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        SpindleSpeedTable = jsonObservations;
                    }

                    // Add SpindleSpeedTimeSeries
                    typeObservations = observations.Where(o => o.Type == SpindleSpeedDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        SpindleSpeedTimeSeries = jsonObservations;
                    }


                    // Add Strain
                    typeObservations = observations.Where(o => o.Type == StrainDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add StrainTable
                    typeObservations = observations.Where(o => o.Type == StrainDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        StrainTable = jsonObservations;
                    }

                    // Add StrainTimeSeries
                    typeObservations = observations.Where(o => o.Type == StrainDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        StrainTimeSeries = jsonObservations;
                    }


                    // Add Temperature
                    typeObservations = observations.Where(o => o.Type == TemperatureDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add TemperatureTable
                    typeObservations = observations.Where(o => o.Type == TemperatureDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        TemperatureTable = jsonObservations;
                    }

                    // Add TemperatureTimeSeries
                    typeObservations = observations.Where(o => o.Type == TemperatureDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        TemperatureTimeSeries = jsonObservations;
                    }


                    // Add Tension
                    typeObservations = observations.Where(o => o.Type == TensionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add TensionTable
                    typeObservations = observations.Where(o => o.Type == TensionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        TensionTable = jsonObservations;
                    }

                    // Add TensionTimeSeries
                    typeObservations = observations.Where(o => o.Type == TensionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        TensionTimeSeries = jsonObservations;
                    }


                    // Add Tilt
                    typeObservations = observations.Where(o => o.Type == TiltDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add TiltTable
                    typeObservations = observations.Where(o => o.Type == TiltDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        TiltTable = jsonObservations;
                    }

                    // Add TiltTimeSeries
                    typeObservations = observations.Where(o => o.Type == TiltDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        TiltTimeSeries = jsonObservations;
                    }


                    // Add Torque
                    typeObservations = observations.Where(o => o.Type == TorqueDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add TorqueTable
                    typeObservations = observations.Where(o => o.Type == TorqueDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        TorqueTable = jsonObservations;
                    }

                    // Add TorqueTimeSeries
                    typeObservations = observations.Where(o => o.Type == TorqueDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        TorqueTimeSeries = jsonObservations;
                    }


                    // Add Velocity
                    typeObservations = observations.Where(o => o.Type == VelocityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VelocityTable
                    typeObservations = observations.Where(o => o.Type == VelocityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VelocityTable = jsonObservations;
                    }

                    // Add VelocityTimeSeries
                    typeObservations = observations.Where(o => o.Type == VelocityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VelocityTimeSeries = jsonObservations;
                    }


                    // Add Viscosity
                    typeObservations = observations.Where(o => o.Type == ViscosityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ViscosityTable
                    typeObservations = observations.Where(o => o.Type == ViscosityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ViscosityTable = jsonObservations;
                    }

                    // Add ViscosityTimeSeries
                    typeObservations = observations.Where(o => o.Type == ViscosityDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ViscosityTimeSeries = jsonObservations;
                    }


                    // Add VoltAmpere
                    typeObservations = observations.Where(o => o.Type == VoltAmpereDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VoltAmpereTable
                    typeObservations = observations.Where(o => o.Type == VoltAmpereDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VoltAmpereTable = jsonObservations;
                    }

                    // Add VoltAmpereTimeSeries
                    typeObservations = observations.Where(o => o.Type == VoltAmpereDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VoltAmpereTimeSeries = jsonObservations;
                    }


                    // Add VoltAmpereReactive
                    typeObservations = observations.Where(o => o.Type == VoltAmpereReactiveDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VoltAmpereReactiveTable
                    typeObservations = observations.Where(o => o.Type == VoltAmpereReactiveDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VoltAmpereReactiveTable = jsonObservations;
                    }

                    // Add VoltAmpereReactiveTimeSeries
                    typeObservations = observations.Where(o => o.Type == VoltAmpereReactiveDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VoltAmpereReactiveTimeSeries = jsonObservations;
                    }


                    // Add Voltage
                    typeObservations = observations.Where(o => o.Type == VoltageDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VoltageTable
                    typeObservations = observations.Where(o => o.Type == VoltageDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VoltageTable = jsonObservations;
                    }

                    // Add VoltageTimeSeries
                    typeObservations = observations.Where(o => o.Type == VoltageDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VoltageTimeSeries = jsonObservations;
                    }


                    // Add VoltageAC
                    typeObservations = observations.Where(o => o.Type == VoltageACDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VoltageACTable
                    typeObservations = observations.Where(o => o.Type == VoltageACDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VoltageACTable = jsonObservations;
                    }

                    // Add VoltageACTimeSeries
                    typeObservations = observations.Where(o => o.Type == VoltageACDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VoltageACTimeSeries = jsonObservations;
                    }


                    // Add VoltageDC
                    typeObservations = observations.Where(o => o.Type == VoltageDCDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VoltageDCTable
                    typeObservations = observations.Where(o => o.Type == VoltageDCDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VoltageDCTable = jsonObservations;
                    }

                    // Add VoltageDCTimeSeries
                    typeObservations = observations.Where(o => o.Type == VoltageDCDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VoltageDCTimeSeries = jsonObservations;
                    }


                    // Add VolumeFluid
                    typeObservations = observations.Where(o => o.Type == VolumeFluidDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VolumeFluidTable
                    typeObservations = observations.Where(o => o.Type == VolumeFluidDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VolumeFluidTable = jsonObservations;
                    }

                    // Add VolumeFluidTimeSeries
                    typeObservations = observations.Where(o => o.Type == VolumeFluidDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VolumeFluidTimeSeries = jsonObservations;
                    }


                    // Add VolumeSpatial
                    typeObservations = observations.Where(o => o.Type == VolumeSpatialDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add VolumeSpatialTable
                    typeObservations = observations.Where(o => o.Type == VolumeSpatialDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        VolumeSpatialTable = jsonObservations;
                    }

                    // Add VolumeSpatialTimeSeries
                    typeObservations = observations.Where(o => o.Type == VolumeSpatialDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        VolumeSpatialTimeSeries = jsonObservations;
                    }


                    // Add WaterHardness
                    typeObservations = observations.Where(o => o.Type == WaterHardnessDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleValue(observation));
                        }
                        WaterHardness = jsonObservations;
                    }

                    // Add WaterHardnessDataSet
                    typeObservations = observations.Where(o => o.Type == WaterHardnessDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleDataSet(observation));
                        }
                        WaterHardnessDataSet = jsonObservations;
                    }

                    // Add WaterHardnessTable
                    typeObservations = observations.Where(o => o.Type == WaterHardnessDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        WaterHardnessTable = jsonObservations;
                    }

                    // Add WaterHardnessTimeSeries
                    typeObservations = observations.Where(o => o.Type == WaterHardnessDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        WaterHardnessTimeSeries = jsonObservations;
                    }


                    // Add Wattage
                    typeObservations = observations.Where(o => o.Type == WattageDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add WattageTable
                    typeObservations = observations.Where(o => o.Type == WattageDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        WattageTable = jsonObservations;
                    }

                    // Add WattageTimeSeries
                    typeObservations = observations.Where(o => o.Type == WattageDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        WattageTimeSeries = jsonObservations;
                    }


                    // Add XDimension
                    typeObservations = observations.Where(o => o.Type == XDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add XDimensionTable
                    typeObservations = observations.Where(o => o.Type == XDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        XDimensionTable = jsonObservations;
                    }

                    // Add XDimensionTimeSeries
                    typeObservations = observations.Where(o => o.Type == XDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        XDimensionTimeSeries = jsonObservations;
                    }


                    // Add YDimension
                    typeObservations = observations.Where(o => o.Type == YDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add YDimensionTable
                    typeObservations = observations.Where(o => o.Type == YDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        YDimensionTable = jsonObservations;
                    }

                    // Add YDimensionTimeSeries
                    typeObservations = observations.Where(o => o.Type == YDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        YDimensionTimeSeries = jsonObservations;
                    }


                    // Add ZDimension
                    typeObservations = observations.Where(o => o.Type == ZDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
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

                    // Add ZDimensionTable
                    typeObservations = observations.Where(o => o.Type == ZDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTable(observation));
                        }
                        ZDimensionTable = jsonObservations;
                    }

                    // Add ZDimensionTimeSeries
                    typeObservations = observations.Where(o => o.Type == ZDimensionDataItem.TypeId && o.Representation == DataItemRepresentation.TIME_SERIES);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonSampleTimeSeries>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonSampleTimeSeries(observation));
                        }
                        ZDimensionTimeSeries = jsonObservations;
                    }


                }
            }
        }
    }
}
