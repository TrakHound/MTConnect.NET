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
    /// cppagent-style JSON representation of the EVENT-category observations in a
    /// stream. Every MTConnect event type is exposed as three typed collections —
    /// one each for the VALUE, DATA_SET, and TABLE representations — so the
    /// serialized object shape matches the C++ reference agent's output.
    /// </summary>
    public class JsonEvents
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
                if (!ActivationCount.IsNullOrEmpty()) foreach (var x in ActivationCount) l.Add(x.ToObservation(ActivationCountDataItem.TypeId));
                if (!ActivationCountDataSet.IsNullOrEmpty()) foreach (var x in ActivationCountDataSet) l.Add(x.ToObservation(ActivationCountDataItem.TypeId));
                if (!ActivationCountTable.IsNullOrEmpty()) foreach (var x in ActivationCountTable) l.Add(x.ToObservation(ActivationCountDataItem.TypeId));

                if (!ActiveAxes.IsNullOrEmpty()) foreach (var x in ActiveAxes) l.Add(x.ToObservation(ActiveAxesDataItem.TypeId));
                if (!ActiveAxesDataSet.IsNullOrEmpty()) foreach (var x in ActiveAxesDataSet) l.Add(x.ToObservation(ActiveAxesDataItem.TypeId));
                if (!ActiveAxesTable.IsNullOrEmpty()) foreach (var x in ActiveAxesTable) l.Add(x.ToObservation(ActiveAxesDataItem.TypeId));

                if (!ActivePowerSource.IsNullOrEmpty()) foreach (var x in ActivePowerSource) l.Add(x.ToObservation(ActivePowerSourceDataItem.TypeId));
                if (!ActivePowerSourceDataSet.IsNullOrEmpty()) foreach (var x in ActivePowerSourceDataSet) l.Add(x.ToObservation(ActivePowerSourceDataItem.TypeId));
                if (!ActivePowerSourceTable.IsNullOrEmpty()) foreach (var x in ActivePowerSourceTable) l.Add(x.ToObservation(ActivePowerSourceDataItem.TypeId));

                if (!ActuatorState.IsNullOrEmpty()) foreach (var x in ActuatorState) l.Add(x.ToObservation(ActuatorStateDataItem.TypeId));
                if (!ActuatorStateDataSet.IsNullOrEmpty()) foreach (var x in ActuatorStateDataSet) l.Add(x.ToObservation(ActuatorStateDataItem.TypeId));
                if (!ActuatorStateTable.IsNullOrEmpty()) foreach (var x in ActuatorStateTable) l.Add(x.ToObservation(ActuatorStateDataItem.TypeId));

                if (!AdapterSoftwareVersion.IsNullOrEmpty()) foreach (var x in AdapterSoftwareVersion) l.Add(x.ToObservation(AdapterSoftwareVersionDataItem.TypeId));
                if (!AdapterSoftwareVersionDataSet.IsNullOrEmpty()) foreach (var x in AdapterSoftwareVersionDataSet) l.Add(x.ToObservation(AdapterSoftwareVersionDataItem.TypeId));
                if (!AdapterSoftwareVersionTable.IsNullOrEmpty()) foreach (var x in AdapterSoftwareVersionTable) l.Add(x.ToObservation(AdapterSoftwareVersionDataItem.TypeId));

                if (!AdapterUri.IsNullOrEmpty()) foreach (var x in AdapterUri) l.Add(x.ToObservation(AdapterUriDataItem.TypeId));
                if (!AdapterUriDataSet.IsNullOrEmpty()) foreach (var x in AdapterUriDataSet) l.Add(x.ToObservation(AdapterUriDataItem.TypeId));
                if (!AdapterUriTable.IsNullOrEmpty()) foreach (var x in AdapterUriTable) l.Add(x.ToObservation(AdapterUriDataItem.TypeId));

                if (!Alarm.IsNullOrEmpty()) foreach (var x in Alarm) l.Add(x.ToObservation(AlarmDataItem.TypeId));
                if (!AlarmDataSet.IsNullOrEmpty()) foreach (var x in AlarmDataSet) l.Add(x.ToObservation(AlarmDataItem.TypeId));
                if (!AlarmTable.IsNullOrEmpty()) foreach (var x in AlarmTable) l.Add(x.ToObservation(AlarmDataItem.TypeId));

                if (!AlarmLimit.IsNullOrEmpty()) foreach (var x in AlarmLimit) l.Add(x.ToObservation(AlarmLimitDataItem.TypeId));
                if (!AlarmLimitDataSet.IsNullOrEmpty()) foreach (var x in AlarmLimitDataSet) l.Add(x.ToObservation(AlarmLimitDataItem.TypeId));
                if (!AlarmLimitTable.IsNullOrEmpty()) foreach (var x in AlarmLimitTable) l.Add(x.ToObservation(AlarmLimitDataItem.TypeId));

                if (!AlarmLimits.IsNullOrEmpty()) foreach (var x in AlarmLimits) l.Add(x.ToObservation(AlarmLimitsDataItem.TypeId));
                if (!AlarmLimitsDataSet.IsNullOrEmpty()) foreach (var x in AlarmLimitsDataSet) l.Add(x.ToObservation(AlarmLimitsDataItem.TypeId));
                if (!AlarmLimitsTable.IsNullOrEmpty()) foreach (var x in AlarmLimitsTable) l.Add(x.ToObservation(AlarmLimitsDataItem.TypeId));

                if (!Application.IsNullOrEmpty()) foreach (var x in Application) l.Add(x.ToObservation(ApplicationDataItem.TypeId));
                if (!ApplicationDataSet.IsNullOrEmpty()) foreach (var x in ApplicationDataSet) l.Add(x.ToObservation(ApplicationDataItem.TypeId));
                if (!ApplicationTable.IsNullOrEmpty()) foreach (var x in ApplicationTable) l.Add(x.ToObservation(ApplicationDataItem.TypeId));

                if (!AssetAdded.IsNullOrEmpty()) foreach (var x in AssetAdded) l.Add(x.ToObservation(AssetAddedDataItem.TypeId));
                if (!AssetAddedDataSet.IsNullOrEmpty()) foreach (var x in AssetAddedDataSet) l.Add(x.ToObservation(AssetAddedDataItem.TypeId));
                if (!AssetAddedTable.IsNullOrEmpty()) foreach (var x in AssetAddedTable) l.Add(x.ToObservation(AssetAddedDataItem.TypeId));

                if (!AssetChanged.IsNullOrEmpty()) foreach (var x in AssetChanged) l.Add(x.ToObservation(AssetChangedDataItem.TypeId));
                if (!AssetChangedDataSet.IsNullOrEmpty()) foreach (var x in AssetChangedDataSet) l.Add(x.ToObservation(AssetChangedDataItem.TypeId));
                if (!AssetChangedTable.IsNullOrEmpty()) foreach (var x in AssetChangedTable) l.Add(x.ToObservation(AssetChangedDataItem.TypeId));

                if (!AssetCount.IsNullOrEmpty()) foreach (var x in AssetCount) l.Add(x.ToObservation(AssetCountDataItem.TypeId));
                if (!AssetCountDataSet.IsNullOrEmpty()) foreach (var x in AssetCountDataSet) l.Add(x.ToObservation(AssetCountDataItem.TypeId));
                if (!AssetCountTable.IsNullOrEmpty()) foreach (var x in AssetCountTable) l.Add(x.ToObservation(AssetCountDataItem.TypeId));

                if (!AssetRemoved.IsNullOrEmpty()) foreach (var x in AssetRemoved) l.Add(x.ToObservation(AssetRemovedDataItem.TypeId));
                if (!AssetRemovedDataSet.IsNullOrEmpty()) foreach (var x in AssetRemovedDataSet) l.Add(x.ToObservation(AssetRemovedDataItem.TypeId));
                if (!AssetRemovedTable.IsNullOrEmpty()) foreach (var x in AssetRemovedTable) l.Add(x.ToObservation(AssetRemovedDataItem.TypeId));

                if (!AssociatedAssetId.IsNullOrEmpty()) foreach (var x in AssociatedAssetId) l.Add(x.ToObservation(AssociatedAssetIdDataItem.TypeId));
                if (!AssociatedAssetIdDataSet.IsNullOrEmpty()) foreach (var x in AssociatedAssetIdDataSet) l.Add(x.ToObservation(AssociatedAssetIdDataItem.TypeId));
                if (!AssociatedAssetIdTable.IsNullOrEmpty()) foreach (var x in AssociatedAssetIdTable) l.Add(x.ToObservation(AssociatedAssetIdDataItem.TypeId));

                if (!Availability.IsNullOrEmpty()) foreach (var x in Availability) l.Add(x.ToObservation(AvailabilityDataItem.TypeId));
                if (!AvailabilityDataSet.IsNullOrEmpty()) foreach (var x in AvailabilityDataSet) l.Add(x.ToObservation(AvailabilityDataItem.TypeId));
                if (!AvailabilityTable.IsNullOrEmpty()) foreach (var x in AvailabilityTable) l.Add(x.ToObservation(AvailabilityDataItem.TypeId));

                if (!AxisCoupling.IsNullOrEmpty()) foreach (var x in AxisCoupling) l.Add(x.ToObservation(AxisCouplingDataItem.TypeId));
                if (!AxisCouplingDataSet.IsNullOrEmpty()) foreach (var x in AxisCouplingDataSet) l.Add(x.ToObservation(AxisCouplingDataItem.TypeId));
                if (!AxisCouplingTable.IsNullOrEmpty()) foreach (var x in AxisCouplingTable) l.Add(x.ToObservation(AxisCouplingDataItem.TypeId));

                if (!AxisFeedrateOverride.IsNullOrEmpty()) foreach (var x in AxisFeedrateOverride) l.Add(x.ToObservation(AxisFeedrateOverrideDataItem.TypeId));
                if (!AxisFeedrateOverrideDataSet.IsNullOrEmpty()) foreach (var x in AxisFeedrateOverrideDataSet) l.Add(x.ToObservation(AxisFeedrateOverrideDataItem.TypeId));
                if (!AxisFeedrateOverrideTable.IsNullOrEmpty()) foreach (var x in AxisFeedrateOverrideTable) l.Add(x.ToObservation(AxisFeedrateOverrideDataItem.TypeId));

                if (!AxisInterlock.IsNullOrEmpty()) foreach (var x in AxisInterlock) l.Add(x.ToObservation(AxisInterlockDataItem.TypeId));
                if (!AxisInterlockDataSet.IsNullOrEmpty()) foreach (var x in AxisInterlockDataSet) l.Add(x.ToObservation(AxisInterlockDataItem.TypeId));
                if (!AxisInterlockTable.IsNullOrEmpty()) foreach (var x in AxisInterlockTable) l.Add(x.ToObservation(AxisInterlockDataItem.TypeId));

                if (!AxisState.IsNullOrEmpty()) foreach (var x in AxisState) l.Add(x.ToObservation(AxisStateDataItem.TypeId));
                if (!AxisStateDataSet.IsNullOrEmpty()) foreach (var x in AxisStateDataSet) l.Add(x.ToObservation(AxisStateDataItem.TypeId));
                if (!AxisStateTable.IsNullOrEmpty()) foreach (var x in AxisStateTable) l.Add(x.ToObservation(AxisStateDataItem.TypeId));

                if (!BatteryState.IsNullOrEmpty()) foreach (var x in BatteryState) l.Add(x.ToObservation(BatteryStateDataItem.TypeId));
                if (!BatteryStateDataSet.IsNullOrEmpty()) foreach (var x in BatteryStateDataSet) l.Add(x.ToObservation(BatteryStateDataItem.TypeId));
                if (!BatteryStateTable.IsNullOrEmpty()) foreach (var x in BatteryStateTable) l.Add(x.ToObservation(BatteryStateDataItem.TypeId));

                if (!BindingState.IsNullOrEmpty()) foreach (var x in BindingState) l.Add(x.ToObservation(BindingStateDataItem.TypeId));
                if (!BindingStateDataSet.IsNullOrEmpty()) foreach (var x in BindingStateDataSet) l.Add(x.ToObservation(BindingStateDataItem.TypeId));
                if (!BindingStateTable.IsNullOrEmpty()) foreach (var x in BindingStateTable) l.Add(x.ToObservation(BindingStateDataItem.TypeId));

                if (!Block.IsNullOrEmpty()) foreach (var x in Block) l.Add(x.ToObservation(BlockDataItem.TypeId));
                if (!BlockDataSet.IsNullOrEmpty()) foreach (var x in BlockDataSet) l.Add(x.ToObservation(BlockDataItem.TypeId));
                if (!BlockTable.IsNullOrEmpty()) foreach (var x in BlockTable) l.Add(x.ToObservation(BlockDataItem.TypeId));

                if (!BlockCount.IsNullOrEmpty()) foreach (var x in BlockCount) l.Add(x.ToObservation(BlockCountDataItem.TypeId));
                if (!BlockCountDataSet.IsNullOrEmpty()) foreach (var x in BlockCountDataSet) l.Add(x.ToObservation(BlockCountDataItem.TypeId));
                if (!BlockCountTable.IsNullOrEmpty()) foreach (var x in BlockCountTable) l.Add(x.ToObservation(BlockCountDataItem.TypeId));

                if (!CharacteristicPersistentId.IsNullOrEmpty()) foreach (var x in CharacteristicPersistentId) l.Add(x.ToObservation(CharacteristicPersistentIdDataItem.TypeId));
                if (!CharacteristicPersistentIdDataSet.IsNullOrEmpty()) foreach (var x in CharacteristicPersistentIdDataSet) l.Add(x.ToObservation(CharacteristicPersistentIdDataItem.TypeId));
                if (!CharacteristicPersistentIdTable.IsNullOrEmpty()) foreach (var x in CharacteristicPersistentIdTable) l.Add(x.ToObservation(CharacteristicPersistentIdDataItem.TypeId));

                if (!CharacteristicStatus.IsNullOrEmpty()) foreach (var x in CharacteristicStatus) l.Add(x.ToObservation(CharacteristicStatusDataItem.TypeId));
                if (!CharacteristicStatusDataSet.IsNullOrEmpty()) foreach (var x in CharacteristicStatusDataSet) l.Add(x.ToObservation(CharacteristicStatusDataItem.TypeId));
                if (!CharacteristicStatusTable.IsNullOrEmpty()) foreach (var x in CharacteristicStatusTable) l.Add(x.ToObservation(CharacteristicStatusDataItem.TypeId));

                if (!ChuckInterlock.IsNullOrEmpty()) foreach (var x in ChuckInterlock) l.Add(x.ToObservation(ChuckInterlockDataItem.TypeId));
                if (!ChuckInterlockDataSet.IsNullOrEmpty()) foreach (var x in ChuckInterlockDataSet) l.Add(x.ToObservation(ChuckInterlockDataItem.TypeId));
                if (!ChuckInterlockTable.IsNullOrEmpty()) foreach (var x in ChuckInterlockTable) l.Add(x.ToObservation(ChuckInterlockDataItem.TypeId));

                if (!ChuckState.IsNullOrEmpty()) foreach (var x in ChuckState) l.Add(x.ToObservation(ChuckStateDataItem.TypeId));
                if (!ChuckStateDataSet.IsNullOrEmpty()) foreach (var x in ChuckStateDataSet) l.Add(x.ToObservation(ChuckStateDataItem.TypeId));
                if (!ChuckStateTable.IsNullOrEmpty()) foreach (var x in ChuckStateTable) l.Add(x.ToObservation(ChuckStateDataItem.TypeId));

                if (!ClockTime.IsNullOrEmpty()) foreach (var x in ClockTime) l.Add(x.ToObservation(ClockTimeDataItem.TypeId));
                if (!ClockTimeDataSet.IsNullOrEmpty()) foreach (var x in ClockTimeDataSet) l.Add(x.ToObservation(ClockTimeDataItem.TypeId));
                if (!ClockTimeTable.IsNullOrEmpty()) foreach (var x in ClockTimeTable) l.Add(x.ToObservation(ClockTimeDataItem.TypeId));

                if (!Code.IsNullOrEmpty()) foreach (var x in Code) l.Add(x.ToObservation(CodeDataItem.TypeId));
                if (!CodeDataSet.IsNullOrEmpty()) foreach (var x in CodeDataSet) l.Add(x.ToObservation(CodeDataItem.TypeId));
                if (!CodeTable.IsNullOrEmpty()) foreach (var x in CodeTable) l.Add(x.ToObservation(CodeDataItem.TypeId));

                if (!ComponentData.IsNullOrEmpty()) foreach (var x in ComponentData) l.Add(x.ToObservation(ComponentDataDataItem.TypeId));
                if (!ComponentDataDataSet.IsNullOrEmpty()) foreach (var x in ComponentDataDataSet) l.Add(x.ToObservation(ComponentDataDataItem.TypeId));
                if (!ComponentDataTable.IsNullOrEmpty()) foreach (var x in ComponentDataTable) l.Add(x.ToObservation(ComponentDataDataItem.TypeId));

                if (!CompositionState.IsNullOrEmpty()) foreach (var x in CompositionState) l.Add(x.ToObservation(CompositionStateDataItem.TypeId));
                if (!CompositionStateDataSet.IsNullOrEmpty()) foreach (var x in CompositionStateDataSet) l.Add(x.ToObservation(CompositionStateDataItem.TypeId));
                if (!CompositionStateTable.IsNullOrEmpty()) foreach (var x in CompositionStateTable) l.Add(x.ToObservation(CompositionStateDataItem.TypeId));

                if (!ConnectionStatus.IsNullOrEmpty()) foreach (var x in ConnectionStatus) l.Add(x.ToObservation(ConnectionStatusDataItem.TypeId));
                if (!ConnectionStatusDataSet.IsNullOrEmpty()) foreach (var x in ConnectionStatusDataSet) l.Add(x.ToObservation(ConnectionStatusDataItem.TypeId));
                if (!ConnectionStatusTable.IsNullOrEmpty()) foreach (var x in ConnectionStatusTable) l.Add(x.ToObservation(ConnectionStatusDataItem.TypeId));

                if (!ControlLimit.IsNullOrEmpty()) foreach (var x in ControlLimit) l.Add(x.ToObservation(ControlLimitDataItem.TypeId));
                if (!ControlLimitDataSet.IsNullOrEmpty()) foreach (var x in ControlLimitDataSet) l.Add(x.ToObservation(ControlLimitDataItem.TypeId));
                if (!ControlLimitTable.IsNullOrEmpty()) foreach (var x in ControlLimitTable) l.Add(x.ToObservation(ControlLimitDataItem.TypeId));

                if (!ControlLimits.IsNullOrEmpty()) foreach (var x in ControlLimits) l.Add(x.ToObservation(ControlLimitsDataItem.TypeId));
                if (!ControlLimitsDataSet.IsNullOrEmpty()) foreach (var x in ControlLimitsDataSet) l.Add(x.ToObservation(ControlLimitsDataItem.TypeId));
                if (!ControlLimitsTable.IsNullOrEmpty()) foreach (var x in ControlLimitsTable) l.Add(x.ToObservation(ControlLimitsDataItem.TypeId));

                if (!ControllerMode.IsNullOrEmpty()) foreach (var x in ControllerMode) l.Add(x.ToObservation(ControllerModeDataItem.TypeId));
                if (!ControllerModeDataSet.IsNullOrEmpty()) foreach (var x in ControllerModeDataSet) l.Add(x.ToObservation(ControllerModeDataItem.TypeId));
                if (!ControllerModeTable.IsNullOrEmpty()) foreach (var x in ControllerModeTable) l.Add(x.ToObservation(ControllerModeDataItem.TypeId));

                if (!ControllerModeOverride.IsNullOrEmpty()) foreach (var x in ControllerModeOverride) l.Add(x.ToObservation(ControllerModeOverrideDataItem.TypeId));
                if (!ControllerModeOverrideDataSet.IsNullOrEmpty()) foreach (var x in ControllerModeOverrideDataSet) l.Add(x.ToObservation(ControllerModeOverrideDataItem.TypeId));
                if (!ControllerModeOverrideTable.IsNullOrEmpty()) foreach (var x in ControllerModeOverrideTable) l.Add(x.ToObservation(ControllerModeOverrideDataItem.TypeId));

                if (!CoupledAxes.IsNullOrEmpty()) foreach (var x in CoupledAxes) l.Add(x.ToObservation(CoupledAxesDataItem.TypeId));
                if (!CoupledAxesDataSet.IsNullOrEmpty()) foreach (var x in CoupledAxesDataSet) l.Add(x.ToObservation(CoupledAxesDataItem.TypeId));
                if (!CoupledAxesTable.IsNullOrEmpty()) foreach (var x in CoupledAxesTable) l.Add(x.ToObservation(CoupledAxesDataItem.TypeId));

                if (!CycleCount.IsNullOrEmpty()) foreach (var x in CycleCount) l.Add(x.ToObservation(CycleCountDataItem.TypeId));
                if (!CycleCountDataSet.IsNullOrEmpty()) foreach (var x in CycleCountDataSet) l.Add(x.ToObservation(CycleCountDataItem.TypeId));
                if (!CycleCountTable.IsNullOrEmpty()) foreach (var x in CycleCountTable) l.Add(x.ToObservation(CycleCountDataItem.TypeId));

                if (!DateCode.IsNullOrEmpty()) foreach (var x in DateCode) l.Add(x.ToObservation(DateCodeDataItem.TypeId));
                if (!DateCodeDataSet.IsNullOrEmpty()) foreach (var x in DateCodeDataSet) l.Add(x.ToObservation(DateCodeDataItem.TypeId));
                if (!DateCodeTable.IsNullOrEmpty()) foreach (var x in DateCodeTable) l.Add(x.ToObservation(DateCodeDataItem.TypeId));

                if (!DeactivationCount.IsNullOrEmpty()) foreach (var x in DeactivationCount) l.Add(x.ToObservation(DeactivationCountDataItem.TypeId));
                if (!DeactivationCountDataSet.IsNullOrEmpty()) foreach (var x in DeactivationCountDataSet) l.Add(x.ToObservation(DeactivationCountDataItem.TypeId));
                if (!DeactivationCountTable.IsNullOrEmpty()) foreach (var x in DeactivationCountTable) l.Add(x.ToObservation(DeactivationCountDataItem.TypeId));

                if (!Depth.IsNullOrEmpty()) foreach (var x in Depth) l.Add(x.ToObservation(DepthDataItem.TypeId));
                if (!DepthDataSet.IsNullOrEmpty()) foreach (var x in DepthDataSet) l.Add(x.ToObservation(DepthDataItem.TypeId));
                if (!DepthTable.IsNullOrEmpty()) foreach (var x in DepthTable) l.Add(x.ToObservation(DepthDataItem.TypeId));

                if (!DeviceAdded.IsNullOrEmpty()) foreach (var x in DeviceAdded) l.Add(x.ToObservation(DeviceAddedDataItem.TypeId));
                if (!DeviceAddedDataSet.IsNullOrEmpty()) foreach (var x in DeviceAddedDataSet) l.Add(x.ToObservation(DeviceAddedDataItem.TypeId));
                if (!DeviceAddedTable.IsNullOrEmpty()) foreach (var x in DeviceAddedTable) l.Add(x.ToObservation(DeviceAddedDataItem.TypeId));

                if (!DeviceChanged.IsNullOrEmpty()) foreach (var x in DeviceChanged) l.Add(x.ToObservation(DeviceChangedDataItem.TypeId));
                if (!DeviceChangedDataSet.IsNullOrEmpty()) foreach (var x in DeviceChangedDataSet) l.Add(x.ToObservation(DeviceChangedDataItem.TypeId));
                if (!DeviceChangedTable.IsNullOrEmpty()) foreach (var x in DeviceChangedTable) l.Add(x.ToObservation(DeviceChangedDataItem.TypeId));

                if (!DeviceRemoved.IsNullOrEmpty()) foreach (var x in DeviceRemoved) l.Add(x.ToObservation(DeviceRemovedDataItem.TypeId));
                if (!DeviceRemovedDataSet.IsNullOrEmpty()) foreach (var x in DeviceRemovedDataSet) l.Add(x.ToObservation(DeviceRemovedDataItem.TypeId));
                if (!DeviceRemovedTable.IsNullOrEmpty()) foreach (var x in DeviceRemovedTable) l.Add(x.ToObservation(DeviceRemovedDataItem.TypeId));

                if (!DeviceUuid.IsNullOrEmpty()) foreach (var x in DeviceUuid) l.Add(x.ToObservation(DeviceUuidDataItem.TypeId));
                if (!DeviceUuidDataSet.IsNullOrEmpty()) foreach (var x in DeviceUuidDataSet) l.Add(x.ToObservation(DeviceUuidDataItem.TypeId));
                if (!DeviceUuidTable.IsNullOrEmpty()) foreach (var x in DeviceUuidTable) l.Add(x.ToObservation(DeviceUuidDataItem.TypeId));

                if (!Direction.IsNullOrEmpty()) foreach (var x in Direction) l.Add(x.ToObservation(DirectionDataItem.TypeId));
                if (!DirectionDataSet.IsNullOrEmpty()) foreach (var x in DirectionDataSet) l.Add(x.ToObservation(DirectionDataItem.TypeId));
                if (!DirectionTable.IsNullOrEmpty()) foreach (var x in DirectionTable) l.Add(x.ToObservation(DirectionDataItem.TypeId));

                if (!DoorState.IsNullOrEmpty()) foreach (var x in DoorState) l.Add(x.ToObservation(DoorStateDataItem.TypeId));
                if (!DoorStateDataSet.IsNullOrEmpty()) foreach (var x in DoorStateDataSet) l.Add(x.ToObservation(DoorStateDataItem.TypeId));
                if (!DoorStateTable.IsNullOrEmpty()) foreach (var x in DoorStateTable) l.Add(x.ToObservation(DoorStateDataItem.TypeId));

                if (!EmergencyStop.IsNullOrEmpty()) foreach (var x in EmergencyStop) l.Add(x.ToObservation(EmergencyStopDataItem.TypeId));
                if (!EmergencyStopDataSet.IsNullOrEmpty()) foreach (var x in EmergencyStopDataSet) l.Add(x.ToObservation(EmergencyStopDataItem.TypeId));
                if (!EmergencyStopTable.IsNullOrEmpty()) foreach (var x in EmergencyStopTable) l.Add(x.ToObservation(EmergencyStopDataItem.TypeId));

                if (!EndOfBar.IsNullOrEmpty()) foreach (var x in EndOfBar) l.Add(x.ToObservation(EndOfBarDataItem.TypeId));
                if (!EndOfBarDataSet.IsNullOrEmpty()) foreach (var x in EndOfBarDataSet) l.Add(x.ToObservation(EndOfBarDataItem.TypeId));
                if (!EndOfBarTable.IsNullOrEmpty()) foreach (var x in EndOfBarTable) l.Add(x.ToObservation(EndOfBarDataItem.TypeId));

                if (!EquipmentMode.IsNullOrEmpty()) foreach (var x in EquipmentMode) l.Add(x.ToObservation(EquipmentModeDataItem.TypeId));
                if (!EquipmentModeDataSet.IsNullOrEmpty()) foreach (var x in EquipmentModeDataSet) l.Add(x.ToObservation(EquipmentModeDataItem.TypeId));
                if (!EquipmentModeTable.IsNullOrEmpty()) foreach (var x in EquipmentModeTable) l.Add(x.ToObservation(EquipmentModeDataItem.TypeId));

                if (!Execution.IsNullOrEmpty()) foreach (var x in Execution) l.Add(x.ToObservation(ExecutionDataItem.TypeId));
                if (!ExecutionDataSet.IsNullOrEmpty()) foreach (var x in ExecutionDataSet) l.Add(x.ToObservation(ExecutionDataItem.TypeId));
                if (!ExecutionTable.IsNullOrEmpty()) foreach (var x in ExecutionTable) l.Add(x.ToObservation(ExecutionDataItem.TypeId));

                if (!FeatureMeasurement.IsNullOrEmpty()) foreach (var x in FeatureMeasurement) l.Add(x.ToObservation(FeatureMeasurementDataItem.TypeId));
                if (!FeatureMeasurementDataSet.IsNullOrEmpty()) foreach (var x in FeatureMeasurementDataSet) l.Add(x.ToObservation(FeatureMeasurementDataItem.TypeId));
                if (!FeatureMeasurementTable.IsNullOrEmpty()) foreach (var x in FeatureMeasurementTable) l.Add(x.ToObservation(FeatureMeasurementDataItem.TypeId));

                if (!Firmware.IsNullOrEmpty()) foreach (var x in Firmware) l.Add(x.ToObservation(FirmwareDataItem.TypeId));
                if (!FirmwareDataSet.IsNullOrEmpty()) foreach (var x in FirmwareDataSet) l.Add(x.ToObservation(FirmwareDataItem.TypeId));
                if (!FirmwareTable.IsNullOrEmpty()) foreach (var x in FirmwareTable) l.Add(x.ToObservation(FirmwareDataItem.TypeId));

                if (!FixtureAssetId.IsNullOrEmpty()) foreach (var x in FixtureAssetId) l.Add(x.ToObservation(FixtureAssetIdDataItem.TypeId));
                if (!FixtureAssetIdDataSet.IsNullOrEmpty()) foreach (var x in FixtureAssetIdDataSet) l.Add(x.ToObservation(FixtureAssetIdDataItem.TypeId));
                if (!FixtureAssetIdTable.IsNullOrEmpty()) foreach (var x in FixtureAssetIdTable) l.Add(x.ToObservation(FixtureAssetIdDataItem.TypeId));

                if (!FixtureId.IsNullOrEmpty()) foreach (var x in FixtureId) l.Add(x.ToObservation(FixtureIdDataItem.TypeId));
                if (!FixtureIdDataSet.IsNullOrEmpty()) foreach (var x in FixtureIdDataSet) l.Add(x.ToObservation(FixtureIdDataItem.TypeId));
                if (!FixtureIdTable.IsNullOrEmpty()) foreach (var x in FixtureIdTable) l.Add(x.ToObservation(FixtureIdDataItem.TypeId));

                if (!FunctionalMode.IsNullOrEmpty()) foreach (var x in FunctionalMode) l.Add(x.ToObservation(FunctionalModeDataItem.TypeId));
                if (!FunctionalModeDataSet.IsNullOrEmpty()) foreach (var x in FunctionalModeDataSet) l.Add(x.ToObservation(FunctionalModeDataItem.TypeId));
                if (!FunctionalModeTable.IsNullOrEmpty()) foreach (var x in FunctionalModeTable) l.Add(x.ToObservation(FunctionalModeDataItem.TypeId));

                if (!Hardness.IsNullOrEmpty()) foreach (var x in Hardness) l.Add(x.ToObservation(HardnessDataItem.TypeId));
                if (!HardnessDataSet.IsNullOrEmpty()) foreach (var x in HardnessDataSet) l.Add(x.ToObservation(HardnessDataItem.TypeId));
                if (!HardnessTable.IsNullOrEmpty()) foreach (var x in HardnessTable) l.Add(x.ToObservation(HardnessDataItem.TypeId));

                if (!Hardware.IsNullOrEmpty()) foreach (var x in Hardware) l.Add(x.ToObservation(HardwareDataItem.TypeId));
                if (!HardwareDataSet.IsNullOrEmpty()) foreach (var x in HardwareDataSet) l.Add(x.ToObservation(HardwareDataItem.TypeId));
                if (!HardwareTable.IsNullOrEmpty()) foreach (var x in HardwareTable) l.Add(x.ToObservation(HardwareDataItem.TypeId));

                if (!HostName.IsNullOrEmpty()) foreach (var x in HostName) l.Add(x.ToObservation(HostNameDataItem.TypeId));
                if (!HostNameDataSet.IsNullOrEmpty()) foreach (var x in HostNameDataSet) l.Add(x.ToObservation(HostNameDataItem.TypeId));
                if (!HostNameTable.IsNullOrEmpty()) foreach (var x in HostNameTable) l.Add(x.ToObservation(HostNameDataItem.TypeId));

                if (!LeakDetect.IsNullOrEmpty()) foreach (var x in LeakDetect) l.Add(x.ToObservation(LeakDetectDataItem.TypeId));
                if (!LeakDetectDataSet.IsNullOrEmpty()) foreach (var x in LeakDetectDataSet) l.Add(x.ToObservation(LeakDetectDataItem.TypeId));
                if (!LeakDetectTable.IsNullOrEmpty()) foreach (var x in LeakDetectTable) l.Add(x.ToObservation(LeakDetectDataItem.TypeId));

                if (!Library.IsNullOrEmpty()) foreach (var x in Library) l.Add(x.ToObservation(LibraryDataItem.TypeId));
                if (!LibraryDataSet.IsNullOrEmpty()) foreach (var x in LibraryDataSet) l.Add(x.ToObservation(LibraryDataItem.TypeId));
                if (!LibraryTable.IsNullOrEmpty()) foreach (var x in LibraryTable) l.Add(x.ToObservation(LibraryDataItem.TypeId));

                if (!Line.IsNullOrEmpty()) foreach (var x in Line) l.Add(x.ToObservation(LineDataItem.TypeId));
                if (!LineDataSet.IsNullOrEmpty()) foreach (var x in LineDataSet) l.Add(x.ToObservation(LineDataItem.TypeId));
                if (!LineTable.IsNullOrEmpty()) foreach (var x in LineTable) l.Add(x.ToObservation(LineDataItem.TypeId));

                if (!LineLabel.IsNullOrEmpty()) foreach (var x in LineLabel) l.Add(x.ToObservation(LineLabelDataItem.TypeId));
                if (!LineLabelDataSet.IsNullOrEmpty()) foreach (var x in LineLabelDataSet) l.Add(x.ToObservation(LineLabelDataItem.TypeId));
                if (!LineLabelTable.IsNullOrEmpty()) foreach (var x in LineLabelTable) l.Add(x.ToObservation(LineLabelDataItem.TypeId));

                if (!LineNumber.IsNullOrEmpty()) foreach (var x in LineNumber) l.Add(x.ToObservation(LineNumberDataItem.TypeId));
                if (!LineNumberDataSet.IsNullOrEmpty()) foreach (var x in LineNumberDataSet) l.Add(x.ToObservation(LineNumberDataItem.TypeId));
                if (!LineNumberTable.IsNullOrEmpty()) foreach (var x in LineNumberTable) l.Add(x.ToObservation(LineNumberDataItem.TypeId));

                if (!LoadCount.IsNullOrEmpty()) foreach (var x in LoadCount) l.Add(x.ToObservation(LoadCountDataItem.TypeId));
                if (!LoadCountDataSet.IsNullOrEmpty()) foreach (var x in LoadCountDataSet) l.Add(x.ToObservation(LoadCountDataItem.TypeId));
                if (!LoadCountTable.IsNullOrEmpty()) foreach (var x in LoadCountTable) l.Add(x.ToObservation(LoadCountDataItem.TypeId));

                if (!LocationAddress.IsNullOrEmpty()) foreach (var x in LocationAddress) l.Add(x.ToObservation(LocationAddressDataItem.TypeId));
                if (!LocationAddressDataSet.IsNullOrEmpty()) foreach (var x in LocationAddressDataSet) l.Add(x.ToObservation(LocationAddressDataItem.TypeId));
                if (!LocationAddressTable.IsNullOrEmpty()) foreach (var x in LocationAddressTable) l.Add(x.ToObservation(LocationAddressDataItem.TypeId));

                if (!LocationNarrative.IsNullOrEmpty()) foreach (var x in LocationNarrative) l.Add(x.ToObservation(LocationNarrativeDataItem.TypeId));
                if (!LocationNarrativeDataSet.IsNullOrEmpty()) foreach (var x in LocationNarrativeDataSet) l.Add(x.ToObservation(LocationNarrativeDataItem.TypeId));
                if (!LocationNarrativeTable.IsNullOrEmpty()) foreach (var x in LocationNarrativeTable) l.Add(x.ToObservation(LocationNarrativeDataItem.TypeId));

                if (!LocationSpatialGeographic.IsNullOrEmpty()) foreach (var x in LocationSpatialGeographic) l.Add(x.ToObservation(LocationSpatialGeographicDataItem.TypeId));
                if (!LocationSpatialGeographicDataSet.IsNullOrEmpty()) foreach (var x in LocationSpatialGeographicDataSet) l.Add(x.ToObservation(LocationSpatialGeographicDataItem.TypeId));
                if (!LocationSpatialGeographicTable.IsNullOrEmpty()) foreach (var x in LocationSpatialGeographicTable) l.Add(x.ToObservation(LocationSpatialGeographicDataItem.TypeId));

                if (!LockState.IsNullOrEmpty()) foreach (var x in LockState) l.Add(x.ToObservation(LockStateDataItem.TypeId));
                if (!LockStateDataSet.IsNullOrEmpty()) foreach (var x in LockStateDataSet) l.Add(x.ToObservation(LockStateDataItem.TypeId));
                if (!LockStateTable.IsNullOrEmpty()) foreach (var x in LockStateTable) l.Add(x.ToObservation(LockStateDataItem.TypeId));

                if (!MaintenanceList.IsNullOrEmpty()) foreach (var x in MaintenanceList) l.Add(x.ToObservation(MaintenanceListDataItem.TypeId));
                if (!MaintenanceListDataSet.IsNullOrEmpty()) foreach (var x in MaintenanceListDataSet) l.Add(x.ToObservation(MaintenanceListDataItem.TypeId));
                if (!MaintenanceListTable.IsNullOrEmpty()) foreach (var x in MaintenanceListTable) l.Add(x.ToObservation(MaintenanceListDataItem.TypeId));

                if (!Material.IsNullOrEmpty()) foreach (var x in Material) l.Add(x.ToObservation(MaterialDataItem.TypeId));
                if (!MaterialDataSet.IsNullOrEmpty()) foreach (var x in MaterialDataSet) l.Add(x.ToObservation(MaterialDataItem.TypeId));
                if (!MaterialTable.IsNullOrEmpty()) foreach (var x in MaterialTable) l.Add(x.ToObservation(MaterialDataItem.TypeId));

                if (!MaterialLayer.IsNullOrEmpty()) foreach (var x in MaterialLayer) l.Add(x.ToObservation(MaterialLayerDataItem.TypeId));
                if (!MaterialLayerDataSet.IsNullOrEmpty()) foreach (var x in MaterialLayerDataSet) l.Add(x.ToObservation(MaterialLayerDataItem.TypeId));
                if (!MaterialLayerTable.IsNullOrEmpty()) foreach (var x in MaterialLayerTable) l.Add(x.ToObservation(MaterialLayerDataItem.TypeId));

                if (!MeasurementType.IsNullOrEmpty()) foreach (var x in MeasurementType) l.Add(x.ToObservation(MeasurementTypeDataItem.TypeId));
                if (!MeasurementTypeDataSet.IsNullOrEmpty()) foreach (var x in MeasurementTypeDataSet) l.Add(x.ToObservation(MeasurementTypeDataItem.TypeId));
                if (!MeasurementTypeTable.IsNullOrEmpty()) foreach (var x in MeasurementTypeTable) l.Add(x.ToObservation(MeasurementTypeDataItem.TypeId));

                if (!MeasurementUnits.IsNullOrEmpty()) foreach (var x in MeasurementUnits) l.Add(x.ToObservation(MeasurementUnitsDataItem.TypeId));
                if (!MeasurementUnitsDataSet.IsNullOrEmpty()) foreach (var x in MeasurementUnitsDataSet) l.Add(x.ToObservation(MeasurementUnitsDataItem.TypeId));
                if (!MeasurementUnitsTable.IsNullOrEmpty()) foreach (var x in MeasurementUnitsTable) l.Add(x.ToObservation(MeasurementUnitsDataItem.TypeId));

                if (!MeasurementValue.IsNullOrEmpty()) foreach (var x in MeasurementValue) l.Add(x.ToObservation(MeasurementValueDataItem.TypeId));
                if (!MeasurementValueDataSet.IsNullOrEmpty()) foreach (var x in MeasurementValueDataSet) l.Add(x.ToObservation(MeasurementValueDataItem.TypeId));
                if (!MeasurementValueTable.IsNullOrEmpty()) foreach (var x in MeasurementValueTable) l.Add(x.ToObservation(MeasurementValueDataItem.TypeId));

                if (!Message.IsNullOrEmpty()) foreach (var x in Message) l.Add(x.ToObservation(MessageDataItem.TypeId));
                if (!MessageDataSet.IsNullOrEmpty()) foreach (var x in MessageDataSet) l.Add(x.ToObservation(MessageDataItem.TypeId));
                if (!MessageTable.IsNullOrEmpty()) foreach (var x in MessageTable) l.Add(x.ToObservation(MessageDataItem.TypeId));

                if (!MTConnectVersion.IsNullOrEmpty()) foreach (var x in MTConnectVersion) l.Add(x.ToObservation(MTConnectVersionDataItem.TypeId));
                if (!MTConnectVersionDataSet.IsNullOrEmpty()) foreach (var x in MTConnectVersionDataSet) l.Add(x.ToObservation(MTConnectVersionDataItem.TypeId));
                if (!MTConnectVersionTable.IsNullOrEmpty()) foreach (var x in MTConnectVersionTable) l.Add(x.ToObservation(MTConnectVersionDataItem.TypeId));

                if (!Network.IsNullOrEmpty()) foreach (var x in Network) l.Add(x.ToObservation(NetworkDataItem.TypeId));
                if (!NetworkDataSet.IsNullOrEmpty()) foreach (var x in NetworkDataSet) l.Add(x.ToObservation(NetworkDataItem.TypeId));
                if (!NetworkTable.IsNullOrEmpty()) foreach (var x in NetworkTable) l.Add(x.ToObservation(NetworkDataItem.TypeId));

                if (!NetworkPort.IsNullOrEmpty()) foreach (var x in NetworkPort) l.Add(x.ToObservation(NetworkPortDataItem.TypeId));
                if (!NetworkPortDataSet.IsNullOrEmpty()) foreach (var x in NetworkPortDataSet) l.Add(x.ToObservation(NetworkPortDataItem.TypeId));
                if (!NetworkPortTable.IsNullOrEmpty()) foreach (var x in NetworkPortTable) l.Add(x.ToObservation(NetworkPortDataItem.TypeId));

                if (!OperatingMode.IsNullOrEmpty()) foreach (var x in OperatingMode) l.Add(x.ToObservation(OperatingModeDataItem.TypeId));
                if (!OperatingModeDataSet.IsNullOrEmpty()) foreach (var x in OperatingModeDataSet) l.Add(x.ToObservation(OperatingModeDataItem.TypeId));
                if (!OperatingModeTable.IsNullOrEmpty()) foreach (var x in OperatingModeTable) l.Add(x.ToObservation(OperatingModeDataItem.TypeId));

                if (!OperatingSystem.IsNullOrEmpty()) foreach (var x in OperatingSystem) l.Add(x.ToObservation(OperatingSystemDataItem.TypeId));
                if (!OperatingSystemDataSet.IsNullOrEmpty()) foreach (var x in OperatingSystemDataSet) l.Add(x.ToObservation(OperatingSystemDataItem.TypeId));
                if (!OperatingSystemTable.IsNullOrEmpty()) foreach (var x in OperatingSystemTable) l.Add(x.ToObservation(OperatingSystemDataItem.TypeId));

                if (!OperatorId.IsNullOrEmpty()) foreach (var x in OperatorId) l.Add(x.ToObservation(OperatorIdDataItem.TypeId));
                if (!OperatorIdDataSet.IsNullOrEmpty()) foreach (var x in OperatorIdDataSet) l.Add(x.ToObservation(OperatorIdDataItem.TypeId));
                if (!OperatorIdTable.IsNullOrEmpty()) foreach (var x in OperatorIdTable) l.Add(x.ToObservation(OperatorIdDataItem.TypeId));

                if (!PalletId.IsNullOrEmpty()) foreach (var x in PalletId) l.Add(x.ToObservation(PalletIdDataItem.TypeId));
                if (!PalletIdDataSet.IsNullOrEmpty()) foreach (var x in PalletIdDataSet) l.Add(x.ToObservation(PalletIdDataItem.TypeId));
                if (!PalletIdTable.IsNullOrEmpty()) foreach (var x in PalletIdTable) l.Add(x.ToObservation(PalletIdDataItem.TypeId));

                if (!PartCount.IsNullOrEmpty()) foreach (var x in PartCount) l.Add(x.ToObservation(PartCountDataItem.TypeId));
                if (!PartCountDataSet.IsNullOrEmpty()) foreach (var x in PartCountDataSet) l.Add(x.ToObservation(PartCountDataItem.TypeId));
                if (!PartCountTable.IsNullOrEmpty()) foreach (var x in PartCountTable) l.Add(x.ToObservation(PartCountDataItem.TypeId));

                if (!PartCountType.IsNullOrEmpty()) foreach (var x in PartCountType) l.Add(x.ToObservation(PartCountTypeDataItem.TypeId));
                if (!PartCountTypeDataSet.IsNullOrEmpty()) foreach (var x in PartCountTypeDataSet) l.Add(x.ToObservation(PartCountTypeDataItem.TypeId));
                if (!PartCountTypeTable.IsNullOrEmpty()) foreach (var x in PartCountTypeTable) l.Add(x.ToObservation(PartCountTypeDataItem.TypeId));

                if (!PartDetect.IsNullOrEmpty()) foreach (var x in PartDetect) l.Add(x.ToObservation(PartDetectDataItem.TypeId));
                if (!PartDetectDataSet.IsNullOrEmpty()) foreach (var x in PartDetectDataSet) l.Add(x.ToObservation(PartDetectDataItem.TypeId));
                if (!PartDetectTable.IsNullOrEmpty()) foreach (var x in PartDetectTable) l.Add(x.ToObservation(PartDetectDataItem.TypeId));

                if (!PartGroupId.IsNullOrEmpty()) foreach (var x in PartGroupId) l.Add(x.ToObservation(PartGroupIdDataItem.TypeId));
                if (!PartGroupIdDataSet.IsNullOrEmpty()) foreach (var x in PartGroupIdDataSet) l.Add(x.ToObservation(PartGroupIdDataItem.TypeId));
                if (!PartGroupIdTable.IsNullOrEmpty()) foreach (var x in PartGroupIdTable) l.Add(x.ToObservation(PartGroupIdDataItem.TypeId));

                if (!PartId.IsNullOrEmpty()) foreach (var x in PartId) l.Add(x.ToObservation(PartIdDataItem.TypeId));
                if (!PartIdDataSet.IsNullOrEmpty()) foreach (var x in PartIdDataSet) l.Add(x.ToObservation(PartIdDataItem.TypeId));
                if (!PartIdTable.IsNullOrEmpty()) foreach (var x in PartIdTable) l.Add(x.ToObservation(PartIdDataItem.TypeId));

                if (!PartIndex.IsNullOrEmpty()) foreach (var x in PartIndex) l.Add(x.ToObservation(PartIndexDataItem.TypeId));
                if (!PartIndexDataSet.IsNullOrEmpty()) foreach (var x in PartIndexDataSet) l.Add(x.ToObservation(PartIndexDataItem.TypeId));
                if (!PartIndexTable.IsNullOrEmpty()) foreach (var x in PartIndexTable) l.Add(x.ToObservation(PartIndexDataItem.TypeId));

                if (!PartKindId.IsNullOrEmpty()) foreach (var x in PartKindId) l.Add(x.ToObservation(PartKindIdDataItem.TypeId));
                if (!PartKindIdDataSet.IsNullOrEmpty()) foreach (var x in PartKindIdDataSet) l.Add(x.ToObservation(PartKindIdDataItem.TypeId));
                if (!PartKindIdTable.IsNullOrEmpty()) foreach (var x in PartKindIdTable) l.Add(x.ToObservation(PartKindIdDataItem.TypeId));

                if (!PartNumber.IsNullOrEmpty()) foreach (var x in PartNumber) l.Add(x.ToObservation(PartNumberDataItem.TypeId));
                if (!PartNumberDataSet.IsNullOrEmpty()) foreach (var x in PartNumberDataSet) l.Add(x.ToObservation(PartNumberDataItem.TypeId));
                if (!PartNumberTable.IsNullOrEmpty()) foreach (var x in PartNumberTable) l.Add(x.ToObservation(PartNumberDataItem.TypeId));

                if (!PartProcessingState.IsNullOrEmpty()) foreach (var x in PartProcessingState) l.Add(x.ToObservation(PartProcessingStateDataItem.TypeId));
                if (!PartProcessingStateDataSet.IsNullOrEmpty()) foreach (var x in PartProcessingStateDataSet) l.Add(x.ToObservation(PartProcessingStateDataItem.TypeId));
                if (!PartProcessingStateTable.IsNullOrEmpty()) foreach (var x in PartProcessingStateTable) l.Add(x.ToObservation(PartProcessingStateDataItem.TypeId));

                if (!PartStatus.IsNullOrEmpty()) foreach (var x in PartStatus) l.Add(x.ToObservation(PartStatusDataItem.TypeId));
                if (!PartStatusDataSet.IsNullOrEmpty()) foreach (var x in PartStatusDataSet) l.Add(x.ToObservation(PartStatusDataItem.TypeId));
                if (!PartStatusTable.IsNullOrEmpty()) foreach (var x in PartStatusTable) l.Add(x.ToObservation(PartStatusDataItem.TypeId));

                if (!PartUniqueId.IsNullOrEmpty()) foreach (var x in PartUniqueId) l.Add(x.ToObservation(PartUniqueIdDataItem.TypeId));
                if (!PartUniqueIdDataSet.IsNullOrEmpty()) foreach (var x in PartUniqueIdDataSet) l.Add(x.ToObservation(PartUniqueIdDataItem.TypeId));
                if (!PartUniqueIdTable.IsNullOrEmpty()) foreach (var x in PartUniqueIdTable) l.Add(x.ToObservation(PartUniqueIdDataItem.TypeId));

                if (!PathFeedrateOverride.IsNullOrEmpty()) foreach (var x in PathFeedrateOverride) l.Add(x.ToObservation(PathFeedrateOverrideDataItem.TypeId));
                if (!PathFeedrateOverrideDataSet.IsNullOrEmpty()) foreach (var x in PathFeedrateOverrideDataSet) l.Add(x.ToObservation(PathFeedrateOverrideDataItem.TypeId));
                if (!PathFeedrateOverrideTable.IsNullOrEmpty()) foreach (var x in PathFeedrateOverrideTable) l.Add(x.ToObservation(PathFeedrateOverrideDataItem.TypeId));

                if (!PathMode.IsNullOrEmpty()) foreach (var x in PathMode) l.Add(x.ToObservation(PathModeDataItem.TypeId));
                if (!PathModeDataSet.IsNullOrEmpty()) foreach (var x in PathModeDataSet) l.Add(x.ToObservation(PathModeDataItem.TypeId));
                if (!PathModeTable.IsNullOrEmpty()) foreach (var x in PathModeTable) l.Add(x.ToObservation(PathModeDataItem.TypeId));

                if (!PowerState.IsNullOrEmpty()) foreach (var x in PowerState) l.Add(x.ToObservation(PowerStateDataItem.TypeId));
                if (!PowerStateDataSet.IsNullOrEmpty()) foreach (var x in PowerStateDataSet) l.Add(x.ToObservation(PowerStateDataItem.TypeId));
                if (!PowerStateTable.IsNullOrEmpty()) foreach (var x in PowerStateTable) l.Add(x.ToObservation(PowerStateDataItem.TypeId));

                if (!PowerStatus.IsNullOrEmpty()) foreach (var x in PowerStatus) l.Add(x.ToObservation(PowerStatusDataItem.TypeId));
                if (!PowerStatusDataSet.IsNullOrEmpty()) foreach (var x in PowerStatusDataSet) l.Add(x.ToObservation(PowerStatusDataItem.TypeId));
                if (!PowerStatusTable.IsNullOrEmpty()) foreach (var x in PowerStatusTable) l.Add(x.ToObservation(PowerStatusDataItem.TypeId));

                if (!ProcessAggregateId.IsNullOrEmpty()) foreach (var x in ProcessAggregateId) l.Add(x.ToObservation(ProcessAggregateIdDataItem.TypeId));
                if (!ProcessAggregateIdDataSet.IsNullOrEmpty()) foreach (var x in ProcessAggregateIdDataSet) l.Add(x.ToObservation(ProcessAggregateIdDataItem.TypeId));
                if (!ProcessAggregateIdTable.IsNullOrEmpty()) foreach (var x in ProcessAggregateIdTable) l.Add(x.ToObservation(ProcessAggregateIdDataItem.TypeId));

                if (!ProcessKindId.IsNullOrEmpty()) foreach (var x in ProcessKindId) l.Add(x.ToObservation(ProcessKindIdDataItem.TypeId));
                if (!ProcessKindIdDataSet.IsNullOrEmpty()) foreach (var x in ProcessKindIdDataSet) l.Add(x.ToObservation(ProcessKindIdDataItem.TypeId));
                if (!ProcessKindIdTable.IsNullOrEmpty()) foreach (var x in ProcessKindIdTable) l.Add(x.ToObservation(ProcessKindIdDataItem.TypeId));

                if (!ProcessOccurrenceId.IsNullOrEmpty()) foreach (var x in ProcessOccurrenceId) l.Add(x.ToObservation(ProcessOccurrenceIdDataItem.TypeId));
                if (!ProcessOccurrenceIdDataSet.IsNullOrEmpty()) foreach (var x in ProcessOccurrenceIdDataSet) l.Add(x.ToObservation(ProcessOccurrenceIdDataItem.TypeId));
                if (!ProcessOccurrenceIdTable.IsNullOrEmpty()) foreach (var x in ProcessOccurrenceIdTable) l.Add(x.ToObservation(ProcessOccurrenceIdDataItem.TypeId));

                if (!ProcessState.IsNullOrEmpty()) foreach (var x in ProcessState) l.Add(x.ToObservation(ProcessStateDataItem.TypeId));
                if (!ProcessStateDataSet.IsNullOrEmpty()) foreach (var x in ProcessStateDataSet) l.Add(x.ToObservation(ProcessStateDataItem.TypeId));
                if (!ProcessStateTable.IsNullOrEmpty()) foreach (var x in ProcessStateTable) l.Add(x.ToObservation(ProcessStateDataItem.TypeId));

                if (!ProcessTime.IsNullOrEmpty()) foreach (var x in ProcessTime) l.Add(x.ToObservation(ProcessTimeDataItem.TypeId));
                if (!ProcessTimeDataSet.IsNullOrEmpty()) foreach (var x in ProcessTimeDataSet) l.Add(x.ToObservation(ProcessTimeDataItem.TypeId));
                if (!ProcessTimeTable.IsNullOrEmpty()) foreach (var x in ProcessTimeTable) l.Add(x.ToObservation(ProcessTimeDataItem.TypeId));

                if (!Program.IsNullOrEmpty()) foreach (var x in Program) l.Add(x.ToObservation(ProgramDataItem.TypeId));
                if (!ProgramDataSet.IsNullOrEmpty()) foreach (var x in ProgramDataSet) l.Add(x.ToObservation(ProgramDataItem.TypeId));
                if (!ProgramTable.IsNullOrEmpty()) foreach (var x in ProgramTable) l.Add(x.ToObservation(ProgramDataItem.TypeId));

                if (!ProgramComment.IsNullOrEmpty()) foreach (var x in ProgramComment) l.Add(x.ToObservation(ProgramCommentDataItem.TypeId));
                if (!ProgramCommentDataSet.IsNullOrEmpty()) foreach (var x in ProgramCommentDataSet) l.Add(x.ToObservation(ProgramCommentDataItem.TypeId));
                if (!ProgramCommentTable.IsNullOrEmpty()) foreach (var x in ProgramCommentTable) l.Add(x.ToObservation(ProgramCommentDataItem.TypeId));

                if (!ProgramEdit.IsNullOrEmpty()) foreach (var x in ProgramEdit) l.Add(x.ToObservation(ProgramEditDataItem.TypeId));
                if (!ProgramEditDataSet.IsNullOrEmpty()) foreach (var x in ProgramEditDataSet) l.Add(x.ToObservation(ProgramEditDataItem.TypeId));
                if (!ProgramEditTable.IsNullOrEmpty()) foreach (var x in ProgramEditTable) l.Add(x.ToObservation(ProgramEditDataItem.TypeId));

                if (!ProgramEditName.IsNullOrEmpty()) foreach (var x in ProgramEditName) l.Add(x.ToObservation(ProgramEditNameDataItem.TypeId));
                if (!ProgramEditNameDataSet.IsNullOrEmpty()) foreach (var x in ProgramEditNameDataSet) l.Add(x.ToObservation(ProgramEditNameDataItem.TypeId));
                if (!ProgramEditNameTable.IsNullOrEmpty()) foreach (var x in ProgramEditNameTable) l.Add(x.ToObservation(ProgramEditNameDataItem.TypeId));

                if (!ProgramHeader.IsNullOrEmpty()) foreach (var x in ProgramHeader) l.Add(x.ToObservation(ProgramHeaderDataItem.TypeId));
                if (!ProgramHeaderDataSet.IsNullOrEmpty()) foreach (var x in ProgramHeaderDataSet) l.Add(x.ToObservation(ProgramHeaderDataItem.TypeId));
                if (!ProgramHeaderTable.IsNullOrEmpty()) foreach (var x in ProgramHeaderTable) l.Add(x.ToObservation(ProgramHeaderDataItem.TypeId));

                if (!ProgramLocation.IsNullOrEmpty()) foreach (var x in ProgramLocation) l.Add(x.ToObservation(ProgramLocationDataItem.TypeId));
                if (!ProgramLocationDataSet.IsNullOrEmpty()) foreach (var x in ProgramLocationDataSet) l.Add(x.ToObservation(ProgramLocationDataItem.TypeId));
                if (!ProgramLocationTable.IsNullOrEmpty()) foreach (var x in ProgramLocationTable) l.Add(x.ToObservation(ProgramLocationDataItem.TypeId));

                if (!ProgramLocationType.IsNullOrEmpty()) foreach (var x in ProgramLocationType) l.Add(x.ToObservation(ProgramLocationTypeDataItem.TypeId));
                if (!ProgramLocationTypeDataSet.IsNullOrEmpty()) foreach (var x in ProgramLocationTypeDataSet) l.Add(x.ToObservation(ProgramLocationTypeDataItem.TypeId));
                if (!ProgramLocationTypeTable.IsNullOrEmpty()) foreach (var x in ProgramLocationTypeTable) l.Add(x.ToObservation(ProgramLocationTypeDataItem.TypeId));

                if (!ProgramNestLevel.IsNullOrEmpty()) foreach (var x in ProgramNestLevel) l.Add(x.ToObservation(ProgramNestLevelDataItem.TypeId));
                if (!ProgramNestLevelDataSet.IsNullOrEmpty()) foreach (var x in ProgramNestLevelDataSet) l.Add(x.ToObservation(ProgramNestLevelDataItem.TypeId));
                if (!ProgramNestLevelTable.IsNullOrEmpty()) foreach (var x in ProgramNestLevelTable) l.Add(x.ToObservation(ProgramNestLevelDataItem.TypeId));

                if (!RotaryMode.IsNullOrEmpty()) foreach (var x in RotaryMode) l.Add(x.ToObservation(RotaryModeDataItem.TypeId));
                if (!RotaryModeDataSet.IsNullOrEmpty()) foreach (var x in RotaryModeDataSet) l.Add(x.ToObservation(RotaryModeDataItem.TypeId));
                if (!RotaryModeTable.IsNullOrEmpty()) foreach (var x in RotaryModeTable) l.Add(x.ToObservation(RotaryModeDataItem.TypeId));

                if (!RotaryVelocityOverride.IsNullOrEmpty()) foreach (var x in RotaryVelocityOverride) l.Add(x.ToObservation(RotaryVelocityOverrideDataItem.TypeId));
                if (!RotaryVelocityOverrideDataSet.IsNullOrEmpty()) foreach (var x in RotaryVelocityOverrideDataSet) l.Add(x.ToObservation(RotaryVelocityOverrideDataItem.TypeId));
                if (!RotaryVelocityOverrideTable.IsNullOrEmpty()) foreach (var x in RotaryVelocityOverrideTable) l.Add(x.ToObservation(RotaryVelocityOverrideDataItem.TypeId));

                if (!Rotation.IsNullOrEmpty()) foreach (var x in Rotation) l.Add(x.ToObservation(RotationDataItem.TypeId));
                if (!RotationDataSet.IsNullOrEmpty()) foreach (var x in RotationDataSet) l.Add(x.ToObservation(RotationDataItem.TypeId));
                if (!RotationTable.IsNullOrEmpty()) foreach (var x in RotationTable) l.Add(x.ToObservation(RotationDataItem.TypeId));

                if (!SensorAttachment.IsNullOrEmpty()) foreach (var x in SensorAttachment) l.Add(x.ToObservation(SensorAttachmentDataItem.TypeId));
                if (!SensorAttachmentDataSet.IsNullOrEmpty()) foreach (var x in SensorAttachmentDataSet) l.Add(x.ToObservation(SensorAttachmentDataItem.TypeId));
                if (!SensorAttachmentTable.IsNullOrEmpty()) foreach (var x in SensorAttachmentTable) l.Add(x.ToObservation(SensorAttachmentDataItem.TypeId));

                if (!SensorState.IsNullOrEmpty()) foreach (var x in SensorState) l.Add(x.ToObservation(SensorStateDataItem.TypeId));
                if (!SensorStateDataSet.IsNullOrEmpty()) foreach (var x in SensorStateDataSet) l.Add(x.ToObservation(SensorStateDataItem.TypeId));
                if (!SensorStateTable.IsNullOrEmpty()) foreach (var x in SensorStateTable) l.Add(x.ToObservation(SensorStateDataItem.TypeId));

                if (!SerialNumber.IsNullOrEmpty()) foreach (var x in SerialNumber) l.Add(x.ToObservation(SerialNumberDataItem.TypeId));
                if (!SerialNumberDataSet.IsNullOrEmpty()) foreach (var x in SerialNumberDataSet) l.Add(x.ToObservation(SerialNumberDataItem.TypeId));
                if (!SerialNumberTable.IsNullOrEmpty()) foreach (var x in SerialNumberTable) l.Add(x.ToObservation(SerialNumberDataItem.TypeId));

                if (!SpecificationLimit.IsNullOrEmpty()) foreach (var x in SpecificationLimit) l.Add(x.ToObservation(SpecificationLimitDataItem.TypeId));
                if (!SpecificationLimitDataSet.IsNullOrEmpty()) foreach (var x in SpecificationLimitDataSet) l.Add(x.ToObservation(SpecificationLimitDataItem.TypeId));
                if (!SpecificationLimitTable.IsNullOrEmpty()) foreach (var x in SpecificationLimitTable) l.Add(x.ToObservation(SpecificationLimitDataItem.TypeId));

                if (!SpecificationLimits.IsNullOrEmpty()) foreach (var x in SpecificationLimits) l.Add(x.ToObservation(SpecificationLimitsDataItem.TypeId));
                if (!SpecificationLimitsDataSet.IsNullOrEmpty()) foreach (var x in SpecificationLimitsDataSet) l.Add(x.ToObservation(SpecificationLimitsDataItem.TypeId));
                if (!SpecificationLimitsTable.IsNullOrEmpty()) foreach (var x in SpecificationLimitsTable) l.Add(x.ToObservation(SpecificationLimitsDataItem.TypeId));

                if (!SpindleInterlock.IsNullOrEmpty()) foreach (var x in SpindleInterlock) l.Add(x.ToObservation(SpindleInterlockDataItem.TypeId));
                if (!SpindleInterlockDataSet.IsNullOrEmpty()) foreach (var x in SpindleInterlockDataSet) l.Add(x.ToObservation(SpindleInterlockDataItem.TypeId));
                if (!SpindleInterlockTable.IsNullOrEmpty()) foreach (var x in SpindleInterlockTable) l.Add(x.ToObservation(SpindleInterlockDataItem.TypeId));

                if (!SwingAngle.IsNullOrEmpty()) foreach (var x in SwingAngle) l.Add(x.ToObservation(SwingAngleDataItem.TypeId));
                if (!SwingAngleDataSet.IsNullOrEmpty()) foreach (var x in SwingAngleDataSet) l.Add(x.ToObservation(SwingAngleDataItem.TypeId));
                if (!SwingAngleTable.IsNullOrEmpty()) foreach (var x in SwingAngleTable) l.Add(x.ToObservation(SwingAngleDataItem.TypeId));

                if (!SwingDiameter.IsNullOrEmpty()) foreach (var x in SwingDiameter) l.Add(x.ToObservation(SwingDiameterDataItem.TypeId));
                if (!SwingDiameterDataSet.IsNullOrEmpty()) foreach (var x in SwingDiameterDataSet) l.Add(x.ToObservation(SwingDiameterDataItem.TypeId));
                if (!SwingDiameterTable.IsNullOrEmpty()) foreach (var x in SwingDiameterTable) l.Add(x.ToObservation(SwingDiameterDataItem.TypeId));

                if (!SwingRadius.IsNullOrEmpty()) foreach (var x in SwingRadius) l.Add(x.ToObservation(SwingRadiusDataItem.TypeId));
                if (!SwingRadiusDataSet.IsNullOrEmpty()) foreach (var x in SwingRadiusDataSet) l.Add(x.ToObservation(SwingRadiusDataItem.TypeId));
                if (!SwingRadiusTable.IsNullOrEmpty()) foreach (var x in SwingRadiusTable) l.Add(x.ToObservation(SwingRadiusDataItem.TypeId));

                if (!TaskAssetId.IsNullOrEmpty()) foreach (var x in TaskAssetId) l.Add(x.ToObservation(TaskAssetIdDataItem.TypeId));
                if (!TaskAssetIdDataSet.IsNullOrEmpty()) foreach (var x in TaskAssetIdDataSet) l.Add(x.ToObservation(TaskAssetIdDataItem.TypeId));
                if (!TaskAssetIdTable.IsNullOrEmpty()) foreach (var x in TaskAssetIdTable) l.Add(x.ToObservation(TaskAssetIdDataItem.TypeId));

                if (!Thickness.IsNullOrEmpty()) foreach (var x in Thickness) l.Add(x.ToObservation(ThicknessDataItem.TypeId));
                if (!ThicknessDataSet.IsNullOrEmpty()) foreach (var x in ThicknessDataSet) l.Add(x.ToObservation(ThicknessDataItem.TypeId));
                if (!ThicknessTable.IsNullOrEmpty()) foreach (var x in ThicknessTable) l.Add(x.ToObservation(ThicknessDataItem.TypeId));

                if (!ToolAssetId.IsNullOrEmpty()) foreach (var x in ToolAssetId) l.Add(x.ToObservation(ToolAssetIdDataItem.TypeId));
                if (!ToolAssetIdDataSet.IsNullOrEmpty()) foreach (var x in ToolAssetIdDataSet) l.Add(x.ToObservation(ToolAssetIdDataItem.TypeId));
                if (!ToolAssetIdTable.IsNullOrEmpty()) foreach (var x in ToolAssetIdTable) l.Add(x.ToObservation(ToolAssetIdDataItem.TypeId));

                if (!ToolCuttingItem.IsNullOrEmpty()) foreach (var x in ToolCuttingItem) l.Add(x.ToObservation(ToolCuttingItemDataItem.TypeId));
                if (!ToolCuttingItemDataSet.IsNullOrEmpty()) foreach (var x in ToolCuttingItemDataSet) l.Add(x.ToObservation(ToolCuttingItemDataItem.TypeId));
                if (!ToolCuttingItemTable.IsNullOrEmpty()) foreach (var x in ToolCuttingItemTable) l.Add(x.ToObservation(ToolCuttingItemDataItem.TypeId));

                if (!ToolGroup.IsNullOrEmpty()) foreach (var x in ToolGroup) l.Add(x.ToObservation(ToolGroupDataItem.TypeId));
                if (!ToolGroupDataSet.IsNullOrEmpty()) foreach (var x in ToolGroupDataSet) l.Add(x.ToObservation(ToolGroupDataItem.TypeId));
                if (!ToolGroupTable.IsNullOrEmpty()) foreach (var x in ToolGroupTable) l.Add(x.ToObservation(ToolGroupDataItem.TypeId));

                if (!ToolId.IsNullOrEmpty()) foreach (var x in ToolId) l.Add(x.ToObservation(ToolIdDataItem.TypeId));
                if (!ToolIdDataSet.IsNullOrEmpty()) foreach (var x in ToolIdDataSet) l.Add(x.ToObservation(ToolIdDataItem.TypeId));
                if (!ToolIdTable.IsNullOrEmpty()) foreach (var x in ToolIdTable) l.Add(x.ToObservation(ToolIdDataItem.TypeId));

                if (!ToolNumber.IsNullOrEmpty()) foreach (var x in ToolNumber) l.Add(x.ToObservation(ToolNumberDataItem.TypeId));
                if (!ToolNumberDataSet.IsNullOrEmpty()) foreach (var x in ToolNumberDataSet) l.Add(x.ToObservation(ToolNumberDataItem.TypeId));
                if (!ToolNumberTable.IsNullOrEmpty()) foreach (var x in ToolNumberTable) l.Add(x.ToObservation(ToolNumberDataItem.TypeId));

                if (!ToolOffset.IsNullOrEmpty()) foreach (var x in ToolOffset) l.Add(x.ToObservation(ToolOffsetDataItem.TypeId));
                if (!ToolOffsetDataSet.IsNullOrEmpty()) foreach (var x in ToolOffsetDataSet) l.Add(x.ToObservation(ToolOffsetDataItem.TypeId));
                if (!ToolOffsetTable.IsNullOrEmpty()) foreach (var x in ToolOffsetTable) l.Add(x.ToObservation(ToolOffsetDataItem.TypeId));

                if (!ToolOffsets.IsNullOrEmpty()) foreach (var x in ToolOffsets) l.Add(x.ToObservation(ToolOffsetsDataItem.TypeId));
                if (!ToolOffsetsDataSet.IsNullOrEmpty()) foreach (var x in ToolOffsetsDataSet) l.Add(x.ToObservation(ToolOffsetsDataItem.TypeId));
                if (!ToolOffsetsTable.IsNullOrEmpty()) foreach (var x in ToolOffsetsTable) l.Add(x.ToObservation(ToolOffsetsDataItem.TypeId));

                if (!TransferCount.IsNullOrEmpty()) foreach (var x in TransferCount) l.Add(x.ToObservation(TransferCountDataItem.TypeId));
                if (!TransferCountDataSet.IsNullOrEmpty()) foreach (var x in TransferCountDataSet) l.Add(x.ToObservation(TransferCountDataItem.TypeId));
                if (!TransferCountTable.IsNullOrEmpty()) foreach (var x in TransferCountTable) l.Add(x.ToObservation(TransferCountDataItem.TypeId));

                if (!Translation.IsNullOrEmpty()) foreach (var x in Translation) l.Add(x.ToObservation(TranslationDataItem.TypeId));
                if (!TranslationDataSet.IsNullOrEmpty()) foreach (var x in TranslationDataSet) l.Add(x.ToObservation(TranslationDataItem.TypeId));
                if (!TranslationTable.IsNullOrEmpty()) foreach (var x in TranslationTable) l.Add(x.ToObservation(TranslationDataItem.TypeId));

                if (!Uncertainty.IsNullOrEmpty()) foreach (var x in Uncertainty) l.Add(x.ToObservation(UncertaintyDataItem.TypeId));
                if (!UncertaintyDataSet.IsNullOrEmpty()) foreach (var x in UncertaintyDataSet) l.Add(x.ToObservation(UncertaintyDataItem.TypeId));
                if (!UncertaintyTable.IsNullOrEmpty()) foreach (var x in UncertaintyTable) l.Add(x.ToObservation(UncertaintyDataItem.TypeId));

                if (!UncertaintyType.IsNullOrEmpty()) foreach (var x in UncertaintyType) l.Add(x.ToObservation(UncertaintyTypeDataItem.TypeId));
                if (!UncertaintyTypeDataSet.IsNullOrEmpty()) foreach (var x in UncertaintyTypeDataSet) l.Add(x.ToObservation(UncertaintyTypeDataItem.TypeId));
                if (!UncertaintyTypeTable.IsNullOrEmpty()) foreach (var x in UncertaintyTypeTable) l.Add(x.ToObservation(UncertaintyTypeDataItem.TypeId));

                if (!UnloadCount.IsNullOrEmpty()) foreach (var x in UnloadCount) l.Add(x.ToObservation(UnloadCountDataItem.TypeId));
                if (!UnloadCountDataSet.IsNullOrEmpty()) foreach (var x in UnloadCountDataSet) l.Add(x.ToObservation(UnloadCountDataItem.TypeId));
                if (!UnloadCountTable.IsNullOrEmpty()) foreach (var x in UnloadCountTable) l.Add(x.ToObservation(UnloadCountDataItem.TypeId));

                if (!User.IsNullOrEmpty()) foreach (var x in User) l.Add(x.ToObservation(UserDataItem.TypeId));
                if (!UserDataSet.IsNullOrEmpty()) foreach (var x in UserDataSet) l.Add(x.ToObservation(UserDataItem.TypeId));
                if (!UserTable.IsNullOrEmpty()) foreach (var x in UserTable) l.Add(x.ToObservation(UserDataItem.TypeId));

                if (!ValveState.IsNullOrEmpty()) foreach (var x in ValveState) l.Add(x.ToObservation(ValveStateDataItem.TypeId));
                if (!ValveStateDataSet.IsNullOrEmpty()) foreach (var x in ValveStateDataSet) l.Add(x.ToObservation(ValveStateDataItem.TypeId));
                if (!ValveStateTable.IsNullOrEmpty()) foreach (var x in ValveStateTable) l.Add(x.ToObservation(ValveStateDataItem.TypeId));

                if (!Variable.IsNullOrEmpty()) foreach (var x in Variable) l.Add(x.ToObservation(VariableDataItem.TypeId));
                if (!VariableDataSet.IsNullOrEmpty()) foreach (var x in VariableDataSet) l.Add(x.ToObservation(VariableDataItem.TypeId));
                if (!VariableTable.IsNullOrEmpty()) foreach (var x in VariableTable) l.Add(x.ToObservation(VariableDataItem.TypeId));

                if (!WaitState.IsNullOrEmpty()) foreach (var x in WaitState) l.Add(x.ToObservation(WaitStateDataItem.TypeId));
                if (!WaitStateDataSet.IsNullOrEmpty()) foreach (var x in WaitStateDataSet) l.Add(x.ToObservation(WaitStateDataItem.TypeId));
                if (!WaitStateTable.IsNullOrEmpty()) foreach (var x in WaitStateTable) l.Add(x.ToObservation(WaitStateDataItem.TypeId));

                if (!Wire.IsNullOrEmpty()) foreach (var x in Wire) l.Add(x.ToObservation(WireDataItem.TypeId));
                if (!WireDataSet.IsNullOrEmpty()) foreach (var x in WireDataSet) l.Add(x.ToObservation(WireDataItem.TypeId));
                if (!WireTable.IsNullOrEmpty()) foreach (var x in WireTable) l.Add(x.ToObservation(WireDataItem.TypeId));

                if (!WorkOffset.IsNullOrEmpty()) foreach (var x in WorkOffset) l.Add(x.ToObservation(WorkOffsetDataItem.TypeId));
                if (!WorkOffsetDataSet.IsNullOrEmpty()) foreach (var x in WorkOffsetDataSet) l.Add(x.ToObservation(WorkOffsetDataItem.TypeId));
                if (!WorkOffsetTable.IsNullOrEmpty()) foreach (var x in WorkOffsetTable) l.Add(x.ToObservation(WorkOffsetDataItem.TypeId));

                if (!WorkOffsets.IsNullOrEmpty()) foreach (var x in WorkOffsets) l.Add(x.ToObservation(WorkOffsetsDataItem.TypeId));
                if (!WorkOffsetsDataSet.IsNullOrEmpty()) foreach (var x in WorkOffsetsDataSet) l.Add(x.ToObservation(WorkOffsetsDataItem.TypeId));
                if (!WorkOffsetsTable.IsNullOrEmpty()) foreach (var x in WorkOffsetsTable) l.Add(x.ToObservation(WorkOffsetsDataItem.TypeId));

                if (!WorkholdingId.IsNullOrEmpty()) foreach (var x in WorkholdingId) l.Add(x.ToObservation(WorkholdingIdDataItem.TypeId));
                if (!WorkholdingIdDataSet.IsNullOrEmpty()) foreach (var x in WorkholdingIdDataSet) l.Add(x.ToObservation(WorkholdingIdDataItem.TypeId));
                if (!WorkholdingIdTable.IsNullOrEmpty()) foreach (var x in WorkholdingIdTable) l.Add(x.ToObservation(WorkholdingIdDataItem.TypeId));


                return l;
            }
        }
        /// <summary>
        /// The <c>ActivationCount</c> events reported with the scalar VALUE representation.
        /// Accumulation of the number of times a function has attempted to, or is planned to attempt to, activate or be performed.
        /// </summary>
        [JsonPropertyName("ActivationCount")]
        public IEnumerable<JsonEventValue> ActivationCount { get; set; }

        /// <summary>
        /// The <c>ActivationCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ActivationCountDataSet")]
        public IEnumerable<JsonEventDataSet> ActivationCountDataSet { get; set; }

        /// <summary>
        /// The <c>ActivationCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ActivationCountTable")]
        public IEnumerable<JsonEventTable> ActivationCountTable { get; set; }


        /// <summary>
        /// The <c>ActiveAxes</c> events reported with the scalar VALUE representation.
        /// Set of axes currently associated with a Path or Controller.
        /// </summary>
        [JsonPropertyName("ActiveAxes")]
        public IEnumerable<JsonEventValue> ActiveAxes { get; set; }

        /// <summary>
        /// The <c>ActiveAxes</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ActiveAxesDataSet")]
        public IEnumerable<JsonEventDataSet> ActiveAxesDataSet { get; set; }

        /// <summary>
        /// The <c>ActiveAxes</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ActiveAxesTable")]
        public IEnumerable<JsonEventTable> ActiveAxesTable { get; set; }


        /// <summary>
        /// The <c>ActivePowerSource</c> events reported with the scalar VALUE representation.
        /// Active energy source for the Component.
        /// </summary>
        [JsonPropertyName("ActivePowerSource")]
        public IEnumerable<JsonEventValue> ActivePowerSource { get; set; }

        /// <summary>
        /// The <c>ActivePowerSource</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ActivePowerSourceDataSet")]
        public IEnumerable<JsonEventDataSet> ActivePowerSourceDataSet { get; set; }

        /// <summary>
        /// The <c>ActivePowerSource</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ActivePowerSourceTable")]
        public IEnumerable<JsonEventTable> ActivePowerSourceTable { get; set; }


        /// <summary>
        /// The <c>ActuatorState</c> events reported with the scalar VALUE representation.
        /// Operational state of an apparatus for moving or controlling a mechanism or system.
        /// </summary>
        [JsonPropertyName("ActuatorState")]
        public IEnumerable<JsonEventValue> ActuatorState { get; set; }

        /// <summary>
        /// The <c>ActuatorState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ActuatorStateDataSet")]
        public IEnumerable<JsonEventDataSet> ActuatorStateDataSet { get; set; }

        /// <summary>
        /// The <c>ActuatorState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ActuatorStateTable")]
        public IEnumerable<JsonEventTable> ActuatorStateTable { get; set; }


        /// <summary>
        /// The <c>AdapterSoftwareVersion</c> events reported with the scalar VALUE representation.
        /// Originator’s software version of the adapter.
        /// </summary>
        [JsonPropertyName("AdapterSoftwareVersion")]
        public IEnumerable<JsonEventValue> AdapterSoftwareVersion { get; set; }

        /// <summary>
        /// The <c>AdapterSoftwareVersion</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AdapterSoftwareVersionDataSet")]
        public IEnumerable<JsonEventDataSet> AdapterSoftwareVersionDataSet { get; set; }

        /// <summary>
        /// The <c>AdapterSoftwareVersion</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AdapterSoftwareVersionTable")]
        public IEnumerable<JsonEventTable> AdapterSoftwareVersionTable { get; set; }


        /// <summary>
        /// The <c>AdapterUri</c> events reported with the scalar VALUE representation.
        /// URI of the adapter.
        /// </summary>
        [JsonPropertyName("AdapterUri")]
        public IEnumerable<JsonEventValue> AdapterUri { get; set; }

        /// <summary>
        /// The <c>AdapterUri</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AdapterUriDataSet")]
        public IEnumerable<JsonEventDataSet> AdapterUriDataSet { get; set; }

        /// <summary>
        /// The <c>AdapterUri</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AdapterUriTable")]
        public IEnumerable<JsonEventTable> AdapterUriTable { get; set; }


        /// <summary>
        /// The <c>Alarm</c> events reported with the scalar VALUE representation.
        /// **DEPRECATED:** Replaced with `CONDITION` category data items in Version 1.1.0.
        /// </summary>
        [JsonPropertyName("Alarm")]
        public IEnumerable<JsonEventValue> Alarm { get; set; }

        /// <summary>
        /// The <c>Alarm</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AlarmDataSet")]
        public IEnumerable<JsonEventDataSet> AlarmDataSet { get; set; }

        /// <summary>
        /// The <c>Alarm</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AlarmTable")]
        public IEnumerable<JsonEventTable> AlarmTable { get; set; }


        /// <summary>
        /// The <c>AlarmLimit</c> events reported with the scalar VALUE representation.
        /// Set of limits used to trigger warning or alarm indicators.**DEPRECATED** in *Version 2.5*. Replaced by  `ALARM_LIMITS`.
        /// </summary>
        [JsonPropertyName("AlarmLimit")]
        public IEnumerable<JsonEventValue> AlarmLimit { get; set; }

        /// <summary>
        /// The <c>AlarmLimit</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AlarmLimitDataSet")]
        public IEnumerable<JsonEventDataSet> AlarmLimitDataSet { get; set; }

        /// <summary>
        /// The <c>AlarmLimit</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AlarmLimitTable")]
        public IEnumerable<JsonEventTable> AlarmLimitTable { get; set; }


        /// <summary>
        /// The <c>AlarmLimits</c> events reported with the scalar VALUE representation.
        /// Set of limits used to trigger warning or alarm indicators.
        /// </summary>
        [JsonPropertyName("AlarmLimits")]
        public IEnumerable<JsonEventValue> AlarmLimits { get; set; }

        /// <summary>
        /// The <c>AlarmLimits</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AlarmLimitsDataSet")]
        public IEnumerable<JsonEventDataSet> AlarmLimitsDataSet { get; set; }

        /// <summary>
        /// The <c>AlarmLimits</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AlarmLimitsTable")]
        public IEnumerable<JsonEventTable> AlarmLimitsTable { get; set; }


        /// <summary>
        /// The <c>Application</c> events reported with the scalar VALUE representation.
        /// Application on a Component.
        /// </summary>
        [JsonPropertyName("Application")]
        public IEnumerable<JsonEventValue> Application { get; set; }

        /// <summary>
        /// The <c>Application</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ApplicationDataSet")]
        public IEnumerable<JsonEventDataSet> ApplicationDataSet { get; set; }

        /// <summary>
        /// The <c>Application</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ApplicationTable")]
        public IEnumerable<JsonEventTable> ApplicationTable { get; set; }


        /// <summary>
        /// The <c>AssetAdded</c> events reported with the scalar VALUE representation.
        /// AssetId of the Asset that has been added.
        /// </summary>
        [JsonPropertyName("AssetAdded")]
        public IEnumerable<JsonEventValue> AssetAdded { get; set; }

        /// <summary>
        /// The <c>AssetAdded</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AssetAddedDataSet")]
        public IEnumerable<JsonEventDataSet> AssetAddedDataSet { get; set; }

        /// <summary>
        /// The <c>AssetAdded</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AssetAddedTable")]
        public IEnumerable<JsonEventTable> AssetAddedTable { get; set; }


        /// <summary>
        /// The <c>AssetChanged</c> events reported with the scalar VALUE representation.
        /// AssetId of the Asset that has been changed.
        /// </summary>
        [JsonPropertyName("AssetChanged")]
        public IEnumerable<JsonEventValue> AssetChanged { get; set; }

        /// <summary>
        /// The <c>AssetChanged</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AssetChangedDataSet")]
        public IEnumerable<JsonEventDataSet> AssetChangedDataSet { get; set; }

        /// <summary>
        /// The <c>AssetChanged</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AssetChangedTable")]
        public IEnumerable<JsonEventTable> AssetChangedTable { get; set; }


        /// <summary>
        /// The <c>AssetCount</c> events reported with the scalar VALUE representation.
        /// Data set of the number of Asset of a given type for a Device.
        /// </summary>
        [JsonPropertyName("AssetCount")]
        public IEnumerable<JsonEventValue> AssetCount { get; set; }

        /// <summary>
        /// The <c>AssetCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AssetCountDataSet")]
        public IEnumerable<JsonEventDataSet> AssetCountDataSet { get; set; }

        /// <summary>
        /// The <c>AssetCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AssetCountTable")]
        public IEnumerable<JsonEventTable> AssetCountTable { get; set; }


        /// <summary>
        /// The <c>AssetRemoved</c> events reported with the scalar VALUE representation.
        /// AssetId of the Asset that has been removed.
        /// </summary>
        [JsonPropertyName("AssetRemoved")]
        public IEnumerable<JsonEventValue> AssetRemoved { get; set; }

        /// <summary>
        /// The <c>AssetRemoved</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AssetRemovedDataSet")]
        public IEnumerable<JsonEventDataSet> AssetRemovedDataSet { get; set; }

        /// <summary>
        /// The <c>AssetRemoved</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AssetRemovedTable")]
        public IEnumerable<JsonEventTable> AssetRemovedTable { get; set; }


        /// <summary>
        /// The <c>AssociatedAssetId</c> events reported with the scalar VALUE representation.
        /// AssetId of the Assets associated with a Component.
        /// </summary>
        [JsonPropertyName("AssociatedAssetId")]
        public IEnumerable<JsonEventValue> AssociatedAssetId { get; set; }

        /// <summary>
        /// The <c>AssociatedAssetId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AssociatedAssetIdDataSet")]
        public IEnumerable<JsonEventDataSet> AssociatedAssetIdDataSet { get; set; }

        /// <summary>
        /// The <c>AssociatedAssetId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AssociatedAssetIdTable")]
        public IEnumerable<JsonEventTable> AssociatedAssetIdTable { get; set; }


        /// <summary>
        /// The <c>Availability</c> events reported with the scalar VALUE representation.
        /// Agent's ability to communicate with the data source.
        /// </summary>
        [JsonPropertyName("Availability")]
        public IEnumerable<JsonEventValue> Availability { get; set; }

        /// <summary>
        /// The <c>Availability</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AvailabilityDataSet")]
        public IEnumerable<JsonEventDataSet> AvailabilityDataSet { get; set; }

        /// <summary>
        /// The <c>Availability</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AvailabilityTable")]
        public IEnumerable<JsonEventTable> AvailabilityTable { get; set; }


        /// <summary>
        /// The <c>AxisCoupling</c> events reported with the scalar VALUE representation.
        /// Describes the way the axes will be associated to each other.   This is used in conjunction with `COUPLED_AXES` to indicate the way they are interacting.
        /// </summary>
        [JsonPropertyName("AxisCoupling")]
        public IEnumerable<JsonEventValue> AxisCoupling { get; set; }

        /// <summary>
        /// The <c>AxisCoupling</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AxisCouplingDataSet")]
        public IEnumerable<JsonEventDataSet> AxisCouplingDataSet { get; set; }

        /// <summary>
        /// The <c>AxisCoupling</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AxisCouplingTable")]
        public IEnumerable<JsonEventTable> AxisCouplingTable { get; set; }


        /// <summary>
        /// The <c>AxisFeedrateOverride</c> events reported with the scalar VALUE representation.
        /// Value of a signal or calculation issued to adjust the feedrate of an individual linear type axis.
        /// </summary>
        [JsonPropertyName("AxisFeedrateOverride")]
        public IEnumerable<JsonEventValue> AxisFeedrateOverride { get; set; }

        /// <summary>
        /// The <c>AxisFeedrateOverride</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AxisFeedrateOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> AxisFeedrateOverrideDataSet { get; set; }

        /// <summary>
        /// The <c>AxisFeedrateOverride</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AxisFeedrateOverrideTable")]
        public IEnumerable<JsonEventTable> AxisFeedrateOverrideTable { get; set; }


        /// <summary>
        /// The <c>AxisInterlock</c> events reported with the scalar VALUE representation.
        /// State of the axis lockout function when power has been removed and the axis is allowed to move freely.
        /// </summary>
        [JsonPropertyName("AxisInterlock")]
        public IEnumerable<JsonEventValue> AxisInterlock { get; set; }

        /// <summary>
        /// The <c>AxisInterlock</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AxisInterlockDataSet")]
        public IEnumerable<JsonEventDataSet> AxisInterlockDataSet { get; set; }

        /// <summary>
        /// The <c>AxisInterlock</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AxisInterlockTable")]
        public IEnumerable<JsonEventTable> AxisInterlockTable { get; set; }


        /// <summary>
        /// The <c>AxisState</c> events reported with the scalar VALUE representation.
        /// State of a Linear or Rotary component representing an axis.
        /// </summary>
        [JsonPropertyName("AxisState")]
        public IEnumerable<JsonEventValue> AxisState { get; set; }

        /// <summary>
        /// The <c>AxisState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("AxisStateDataSet")]
        public IEnumerable<JsonEventDataSet> AxisStateDataSet { get; set; }

        /// <summary>
        /// The <c>AxisState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("AxisStateTable")]
        public IEnumerable<JsonEventTable> AxisStateTable { get; set; }


        /// <summary>
        /// The <c>BatteryState</c> events reported with the scalar VALUE representation.
        /// Present status of the battery.
        /// </summary>
        [JsonPropertyName("BatteryState")]
        public IEnumerable<JsonEventValue> BatteryState { get; set; }

        /// <summary>
        /// The <c>BatteryState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("BatteryStateDataSet")]
        public IEnumerable<JsonEventDataSet> BatteryStateDataSet { get; set; }

        /// <summary>
        /// The <c>BatteryState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("BatteryStateTable")]
        public IEnumerable<JsonEventTable> BatteryStateTable { get; set; }


        /// <summary>
        /// The <c>BindingState</c> events reported with the scalar VALUE representation.
        /// State of the binding process when Component participates in a task as a collaborator
        /// </summary>
        [JsonPropertyName("BindingState")]
        public IEnumerable<JsonEventValue> BindingState { get; set; }

        /// <summary>
        /// The <c>BindingState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("BindingStateDataSet")]
        public IEnumerable<JsonEventDataSet> BindingStateDataSet { get; set; }

        /// <summary>
        /// The <c>BindingState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("BindingStateTable")]
        public IEnumerable<JsonEventTable> BindingStateTable { get; set; }


        /// <summary>
        /// The <c>Block</c> events reported with the scalar VALUE representation.
        /// Line of code or command being executed by a Controller entity.
        /// </summary>
        [JsonPropertyName("Block")]
        public IEnumerable<JsonEventValue> Block { get; set; }

        /// <summary>
        /// The <c>Block</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("BlockDataSet")]
        public IEnumerable<JsonEventDataSet> BlockDataSet { get; set; }

        /// <summary>
        /// The <c>Block</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("BlockTable")]
        public IEnumerable<JsonEventTable> BlockTable { get; set; }


        /// <summary>
        /// The <c>BlockCount</c> events reported with the scalar VALUE representation.
        /// Total count of the number of blocks of program code that have been executed since execution started.
        /// </summary>
        [JsonPropertyName("BlockCount")]
        public IEnumerable<JsonEventValue> BlockCount { get; set; }

        /// <summary>
        /// The <c>BlockCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("BlockCountDataSet")]
        public IEnumerable<JsonEventDataSet> BlockCountDataSet { get; set; }

        /// <summary>
        /// The <c>BlockCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("BlockCountTable")]
        public IEnumerable<JsonEventTable> BlockCountTable { get; set; }


        /// <summary>
        /// The <c>CharacteristicPersistentId</c> events reported with the scalar VALUE representation.
        /// UUID of the characteristic.
        /// </summary>
        [JsonPropertyName("CharacteristicPersistentId")]
        public IEnumerable<JsonEventValue> CharacteristicPersistentId { get; set; }

        /// <summary>
        /// The <c>CharacteristicPersistentId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CharacteristicPersistentIdDataSet")]
        public IEnumerable<JsonEventDataSet> CharacteristicPersistentIdDataSet { get; set; }

        /// <summary>
        /// The <c>CharacteristicPersistentId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CharacteristicPersistentIdTable")]
        public IEnumerable<JsonEventTable> CharacteristicPersistentIdTable { get; set; }


        /// <summary>
        /// The <c>CharacteristicStatus</c> events reported with the scalar VALUE representation.
        /// Pass/fail result of the measurement.
        /// </summary>
        [JsonPropertyName("CharacteristicStatus")]
        public IEnumerable<JsonEventValue> CharacteristicStatus { get; set; }

        /// <summary>
        /// The <c>CharacteristicStatus</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CharacteristicStatusDataSet")]
        public IEnumerable<JsonEventDataSet> CharacteristicStatusDataSet { get; set; }

        /// <summary>
        /// The <c>CharacteristicStatus</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CharacteristicStatusTable")]
        public IEnumerable<JsonEventTable> CharacteristicStatusTable { get; set; }


        /// <summary>
        /// The <c>ChuckInterlock</c> events reported with the scalar VALUE representation.
        /// State of an interlock function or control logic state intended to prevent the associated Chuck component from being operated.
        /// </summary>
        [JsonPropertyName("ChuckInterlock")]
        public IEnumerable<JsonEventValue> ChuckInterlock { get; set; }

        /// <summary>
        /// The <c>ChuckInterlock</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ChuckInterlockDataSet")]
        public IEnumerable<JsonEventDataSet> ChuckInterlockDataSet { get; set; }

        /// <summary>
        /// The <c>ChuckInterlock</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ChuckInterlockTable")]
        public IEnumerable<JsonEventTable> ChuckInterlockTable { get; set; }


        /// <summary>
        /// The <c>ChuckState</c> events reported with the scalar VALUE representation.
        /// Operating state of a mechanism that holds a part or stock material during a manufacturing process. It may also represent a mechanism that holds any other mechanism in place within a piece of equipment.
        /// </summary>
        [JsonPropertyName("ChuckState")]
        public IEnumerable<JsonEventValue> ChuckState { get; set; }

        /// <summary>
        /// The <c>ChuckState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ChuckStateDataSet")]
        public IEnumerable<JsonEventDataSet> ChuckStateDataSet { get; set; }

        /// <summary>
        /// The <c>ChuckState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ChuckStateTable")]
        public IEnumerable<JsonEventTable> ChuckStateTable { get; set; }


        /// <summary>
        /// The <c>ClockTime</c> events reported with the scalar VALUE representation.
        /// Time provided by a timing device at a specific point in time.
        /// </summary>
        [JsonPropertyName("ClockTime")]
        public IEnumerable<JsonEventValue> ClockTime { get; set; }

        /// <summary>
        /// The <c>ClockTime</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ClockTimeDataSet")]
        public IEnumerable<JsonEventDataSet> ClockTimeDataSet { get; set; }

        /// <summary>
        /// The <c>ClockTime</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ClockTimeTable")]
        public IEnumerable<JsonEventTable> ClockTimeTable { get; set; }


        /// <summary>
        /// The <c>Code</c> events reported with the scalar VALUE representation.
        /// Programmatic code being executed.**DEPRECATED** in *Version 1.1*.
        /// </summary>
        [JsonPropertyName("Code")]
        public IEnumerable<JsonEventValue> Code { get; set; }

        /// <summary>
        /// The <c>Code</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CodeDataSet")]
        public IEnumerable<JsonEventDataSet> CodeDataSet { get; set; }

        /// <summary>
        /// The <c>Code</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CodeTable")]
        public IEnumerable<JsonEventTable> CodeTable { get; set; }


        /// <summary>
        /// The <c>ComponentData</c> events reported with the scalar VALUE representation.
        /// Event that represents a Component where the EntryDefinition identifies the Component and the CellDefinitions define the Component's observed DataItems.
        /// </summary>
        [JsonPropertyName("ComponentData")]
        public IEnumerable<JsonEventValue> ComponentData { get; set; }

        /// <summary>
        /// The <c>ComponentData</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ComponentDataDataSet")]
        public IEnumerable<JsonEventDataSet> ComponentDataDataSet { get; set; }

        /// <summary>
        /// The <c>ComponentData</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ComponentDataTable")]
        public IEnumerable<JsonEventTable> ComponentDataTable { get; set; }


        /// <summary>
        /// The <c>CompositionState</c> events reported with the scalar VALUE representation.
        /// Operating state of a mechanism represented by a Composition entity.
        /// </summary>
        [JsonPropertyName("CompositionState")]
        public IEnumerable<JsonEventValue> CompositionState { get; set; }

        /// <summary>
        /// The <c>CompositionState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CompositionStateDataSet")]
        public IEnumerable<JsonEventDataSet> CompositionStateDataSet { get; set; }

        /// <summary>
        /// The <c>CompositionState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CompositionStateTable")]
        public IEnumerable<JsonEventTable> CompositionStateTable { get; set; }


        /// <summary>
        /// The <c>ConnectionStatus</c> events reported with the scalar VALUE representation.
        /// Status of the connection between an adapter and an agent.
        /// </summary>
        [JsonPropertyName("ConnectionStatus")]
        public IEnumerable<JsonEventValue> ConnectionStatus { get; set; }

        /// <summary>
        /// The <c>ConnectionStatus</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ConnectionStatusDataSet")]
        public IEnumerable<JsonEventDataSet> ConnectionStatusDataSet { get; set; }

        /// <summary>
        /// The <c>ConnectionStatus</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ConnectionStatusTable")]
        public IEnumerable<JsonEventTable> ConnectionStatusTable { get; set; }


        /// <summary>
        /// The <c>ControlLimit</c> events reported with the scalar VALUE representation.
        /// Set of limits used to indicate whether a process variable is stable and in control.**DEPRECATED** in *Version 2.5*. Replaced by `CONTROL_LIMITS`.
        /// </summary>
        [JsonPropertyName("ControlLimit")]
        public IEnumerable<JsonEventValue> ControlLimit { get; set; }

        /// <summary>
        /// The <c>ControlLimit</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ControlLimitDataSet")]
        public IEnumerable<JsonEventDataSet> ControlLimitDataSet { get; set; }

        /// <summary>
        /// The <c>ControlLimit</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ControlLimitTable")]
        public IEnumerable<JsonEventTable> ControlLimitTable { get; set; }


        /// <summary>
        /// The <c>ControlLimits</c> events reported with the scalar VALUE representation.
        /// Set of limits used to indicate whether a process variable is stable and in control.
        /// </summary>
        [JsonPropertyName("ControlLimits")]
        public IEnumerable<JsonEventValue> ControlLimits { get; set; }

        /// <summary>
        /// The <c>ControlLimits</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ControlLimitsDataSet")]
        public IEnumerable<JsonEventDataSet> ControlLimitsDataSet { get; set; }

        /// <summary>
        /// The <c>ControlLimits</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ControlLimitsTable")]
        public IEnumerable<JsonEventTable> ControlLimitsTable { get; set; }


        /// <summary>
        /// The <c>ControllerMode</c> events reported with the scalar VALUE representation.
        /// Current mode of the Controller component.
        /// </summary>
        [JsonPropertyName("ControllerMode")]
        public IEnumerable<JsonEventValue> ControllerMode { get; set; }

        /// <summary>
        /// The <c>ControllerMode</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ControllerModeDataSet")]
        public IEnumerable<JsonEventDataSet> ControllerModeDataSet { get; set; }

        /// <summary>
        /// The <c>ControllerMode</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ControllerModeTable")]
        public IEnumerable<JsonEventTable> ControllerModeTable { get; set; }


        /// <summary>
        /// The <c>ControllerModeOverride</c> events reported with the scalar VALUE representation.
        /// Setting or operator selection that changes the behavior of a piece of equipment.
        /// </summary>
        [JsonPropertyName("ControllerModeOverride")]
        public IEnumerable<JsonEventValue> ControllerModeOverride { get; set; }

        /// <summary>
        /// The <c>ControllerModeOverride</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ControllerModeOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> ControllerModeOverrideDataSet { get; set; }

        /// <summary>
        /// The <c>ControllerModeOverride</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ControllerModeOverrideTable")]
        public IEnumerable<JsonEventTable> ControllerModeOverrideTable { get; set; }


        /// <summary>
        /// The <c>CoupledAxes</c> events reported with the scalar VALUE representation.
        /// Set of associated axes.
        /// </summary>
        [JsonPropertyName("CoupledAxes")]
        public IEnumerable<JsonEventValue> CoupledAxes { get; set; }

        /// <summary>
        /// The <c>CoupledAxes</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CoupledAxesDataSet")]
        public IEnumerable<JsonEventDataSet> CoupledAxesDataSet { get; set; }

        /// <summary>
        /// The <c>CoupledAxes</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CoupledAxesTable")]
        public IEnumerable<JsonEventTable> CoupledAxesTable { get; set; }


        /// <summary>
        /// The <c>CycleCount</c> events reported with the scalar VALUE representation.
        /// Accumulation of the number of times a cyclic function has attempted to, or is planned to attempt to execute.
        /// </summary>
        [JsonPropertyName("CycleCount")]
        public IEnumerable<JsonEventValue> CycleCount { get; set; }

        /// <summary>
        /// The <c>CycleCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("CycleCountDataSet")]
        public IEnumerable<JsonEventDataSet> CycleCountDataSet { get; set; }

        /// <summary>
        /// The <c>CycleCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("CycleCountTable")]
        public IEnumerable<JsonEventTable> CycleCountTable { get; set; }


        /// <summary>
        /// The <c>DateCode</c> events reported with the scalar VALUE representation.
        /// Time and date code associated with a material or other physical item.
        /// </summary>
        [JsonPropertyName("DateCode")]
        public IEnumerable<JsonEventValue> DateCode { get; set; }

        /// <summary>
        /// The <c>DateCode</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DateCodeDataSet")]
        public IEnumerable<JsonEventDataSet> DateCodeDataSet { get; set; }

        /// <summary>
        /// The <c>DateCode</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DateCodeTable")]
        public IEnumerable<JsonEventTable> DateCodeTable { get; set; }


        /// <summary>
        /// The <c>DeactivationCount</c> events reported with the scalar VALUE representation.
        /// Accumulation of the number of times a function has attempted to, or is planned to attempt to, deactivate or cease.
        /// </summary>
        [JsonPropertyName("DeactivationCount")]
        public IEnumerable<JsonEventValue> DeactivationCount { get; set; }

        /// <summary>
        /// The <c>DeactivationCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DeactivationCountDataSet")]
        public IEnumerable<JsonEventDataSet> DeactivationCountDataSet { get; set; }

        /// <summary>
        /// The <c>DeactivationCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DeactivationCountTable")]
        public IEnumerable<JsonEventTable> DeactivationCountTable { get; set; }


        /// <summary>
        /// The <c>Depth</c> events reported with the scalar VALUE representation.
        /// Dimension or distance as measured downwards from the top
        /// </summary>
        [JsonPropertyName("Depth")]
        public IEnumerable<JsonEventValue> Depth { get; set; }

        /// <summary>
        /// The <c>Depth</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DepthDataSet")]
        public IEnumerable<JsonEventDataSet> DepthDataSet { get; set; }

        /// <summary>
        /// The <c>Depth</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DepthTable")]
        public IEnumerable<JsonEventTable> DepthTable { get; set; }


        /// <summary>
        /// The <c>DeviceAdded</c> events reported with the scalar VALUE representation.
        /// UUID of new device added to an MTConnect Agent.
        /// </summary>
        [JsonPropertyName("DeviceAdded")]
        public IEnumerable<JsonEventValue> DeviceAdded { get; set; }

        /// <summary>
        /// The <c>DeviceAdded</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DeviceAddedDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceAddedDataSet { get; set; }

        /// <summary>
        /// The <c>DeviceAdded</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DeviceAddedTable")]
        public IEnumerable<JsonEventTable> DeviceAddedTable { get; set; }


        /// <summary>
        /// The <c>DeviceChanged</c> events reported with the scalar VALUE representation.
        /// UUID of the device whose metadata has changed.
        /// </summary>
        [JsonPropertyName("DeviceChanged")]
        public IEnumerable<JsonEventValue> DeviceChanged { get; set; }

        /// <summary>
        /// The <c>DeviceChanged</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DeviceChangedDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceChangedDataSet { get; set; }

        /// <summary>
        /// The <c>DeviceChanged</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DeviceChangedTable")]
        public IEnumerable<JsonEventTable> DeviceChangedTable { get; set; }


        /// <summary>
        /// The <c>DeviceRemoved</c> events reported with the scalar VALUE representation.
        /// UUID of a device removed from an MTConnect Agent.
        /// </summary>
        [JsonPropertyName("DeviceRemoved")]
        public IEnumerable<JsonEventValue> DeviceRemoved { get; set; }

        /// <summary>
        /// The <c>DeviceRemoved</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DeviceRemovedDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceRemovedDataSet { get; set; }

        /// <summary>
        /// The <c>DeviceRemoved</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DeviceRemovedTable")]
        public IEnumerable<JsonEventTable> DeviceRemovedTable { get; set; }


        /// <summary>
        /// The <c>DeviceUuid</c> events reported with the scalar VALUE representation.
        /// Identifier of another piece of equipment that is temporarily associated with a component of this piece of equipment to perform a particular function.
        /// </summary>
        [JsonPropertyName("DeviceUuid")]
        public IEnumerable<JsonEventValue> DeviceUuid { get; set; }

        /// <summary>
        /// The <c>DeviceUuid</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DeviceUuidDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceUuidDataSet { get; set; }

        /// <summary>
        /// The <c>DeviceUuid</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DeviceUuidTable")]
        public IEnumerable<JsonEventTable> DeviceUuidTable { get; set; }


        /// <summary>
        /// The <c>Direction</c> events reported with the scalar VALUE representation.
        /// Direction of motion.
        /// </summary>
        [JsonPropertyName("Direction")]
        public IEnumerable<JsonEventValue> Direction { get; set; }

        /// <summary>
        /// The <c>Direction</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DirectionDataSet")]
        public IEnumerable<JsonEventDataSet> DirectionDataSet { get; set; }

        /// <summary>
        /// The <c>Direction</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DirectionTable")]
        public IEnumerable<JsonEventTable> DirectionTable { get; set; }


        /// <summary>
        /// The <c>DoorState</c> events reported with the scalar VALUE representation.
        /// Operational state of a Door component or composition element.
        /// </summary>
        [JsonPropertyName("DoorState")]
        public IEnumerable<JsonEventValue> DoorState { get; set; }

        /// <summary>
        /// The <c>DoorState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("DoorStateDataSet")]
        public IEnumerable<JsonEventDataSet> DoorStateDataSet { get; set; }

        /// <summary>
        /// The <c>DoorState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("DoorStateTable")]
        public IEnumerable<JsonEventTable> DoorStateTable { get; set; }


        /// <summary>
        /// The <c>EmergencyStop</c> events reported with the scalar VALUE representation.
        /// State of the emergency stop signal for a piece of equipment, controller path, or any other component or subsystem of a piece of equipment.
        /// </summary>
        [JsonPropertyName("EmergencyStop")]
        public IEnumerable<JsonEventValue> EmergencyStop { get; set; }

        /// <summary>
        /// The <c>EmergencyStop</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("EmergencyStopDataSet")]
        public IEnumerable<JsonEventDataSet> EmergencyStopDataSet { get; set; }

        /// <summary>
        /// The <c>EmergencyStop</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("EmergencyStopTable")]
        public IEnumerable<JsonEventTable> EmergencyStopTable { get; set; }


        /// <summary>
        /// The <c>EndOfBar</c> events reported with the scalar VALUE representation.
        /// Indication of whether the end of a piece of bar stock being feed by a bar feeder has been reached.
        /// </summary>
        [JsonPropertyName("EndOfBar")]
        public IEnumerable<JsonEventValue> EndOfBar { get; set; }

        /// <summary>
        /// The <c>EndOfBar</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("EndOfBarDataSet")]
        public IEnumerable<JsonEventDataSet> EndOfBarDataSet { get; set; }

        /// <summary>
        /// The <c>EndOfBar</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("EndOfBarTable")]
        public IEnumerable<JsonEventTable> EndOfBarTable { get; set; }


        /// <summary>
        /// The <c>EquipmentMode</c> events reported with the scalar VALUE representation.
        /// Indication that a piece of equipment, or a sub-part of a piece of equipment, is performing specific types of activities.
        /// </summary>
        [JsonPropertyName("EquipmentMode")]
        public IEnumerable<JsonEventValue> EquipmentMode { get; set; }

        /// <summary>
        /// The <c>EquipmentMode</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("EquipmentModeDataSet")]
        public IEnumerable<JsonEventDataSet> EquipmentModeDataSet { get; set; }

        /// <summary>
        /// The <c>EquipmentMode</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("EquipmentModeTable")]
        public IEnumerable<JsonEventTable> EquipmentModeTable { get; set; }


        /// <summary>
        /// The <c>Execution</c> events reported with the scalar VALUE representation.
        /// Operating state of a Component.
        /// </summary>
        [JsonPropertyName("Execution")]
        public IEnumerable<JsonEventValue> Execution { get; set; }

        /// <summary>
        /// The <c>Execution</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ExecutionDataSet")]
        public IEnumerable<JsonEventDataSet> ExecutionDataSet { get; set; }

        /// <summary>
        /// The <c>Execution</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ExecutionTable")]
        public IEnumerable<JsonEventTable> ExecutionTable { get; set; }


        /// <summary>
        /// The <c>FeatureMeasurement</c> events reported with the scalar VALUE representation.
        /// Assessing elements of a feature.
        /// </summary>
        [JsonPropertyName("FeatureMeasurement")]
        public IEnumerable<JsonEventValue> FeatureMeasurement { get; set; }

        /// <summary>
        /// The <c>FeatureMeasurement</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FeatureMeasurementDataSet")]
        public IEnumerable<JsonEventDataSet> FeatureMeasurementDataSet { get; set; }

        /// <summary>
        /// The <c>FeatureMeasurement</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FeatureMeasurementTable")]
        public IEnumerable<JsonEventTable> FeatureMeasurementTable { get; set; }


        /// <summary>
        /// The <c>Firmware</c> events reported with the scalar VALUE representation.
        /// Embedded software of a Component.
        /// </summary>
        [JsonPropertyName("Firmware")]
        public IEnumerable<JsonEventValue> Firmware { get; set; }

        /// <summary>
        /// The <c>Firmware</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FirmwareDataSet")]
        public IEnumerable<JsonEventDataSet> FirmwareDataSet { get; set; }

        /// <summary>
        /// The <c>Firmware</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FirmwareTable")]
        public IEnumerable<JsonEventTable> FirmwareTable { get; set; }


        /// <summary>
        /// The <c>FixtureAssetId</c> events reported with the scalar VALUE representation.
        /// AssetId of the Fixture that is associated with a Component
        /// </summary>
        [JsonPropertyName("FixtureAssetId")]
        public IEnumerable<JsonEventValue> FixtureAssetId { get; set; }

        /// <summary>
        /// The <c>FixtureAssetId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FixtureAssetIdDataSet")]
        public IEnumerable<JsonEventDataSet> FixtureAssetIdDataSet { get; set; }

        /// <summary>
        /// The <c>FixtureAssetId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FixtureAssetIdTable")]
        public IEnumerable<JsonEventTable> FixtureAssetIdTable { get; set; }


        /// <summary>
        /// The <c>FixtureId</c> events reported with the scalar VALUE representation.
        /// Identifier for the current workholding or part clamp in use by a piece of equipment.
        /// </summary>
        [JsonPropertyName("FixtureId")]
        public IEnumerable<JsonEventValue> FixtureId { get; set; }

        /// <summary>
        /// The <c>FixtureId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FixtureIdDataSet")]
        public IEnumerable<JsonEventDataSet> FixtureIdDataSet { get; set; }

        /// <summary>
        /// The <c>FixtureId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FixtureIdTable")]
        public IEnumerable<JsonEventTable> FixtureIdTable { get; set; }


        /// <summary>
        /// The <c>FunctionalMode</c> events reported with the scalar VALUE representation.
        /// Current intended production status of the Component.
        /// </summary>
        [JsonPropertyName("FunctionalMode")]
        public IEnumerable<JsonEventValue> FunctionalMode { get; set; }

        /// <summary>
        /// The <c>FunctionalMode</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("FunctionalModeDataSet")]
        public IEnumerable<JsonEventDataSet> FunctionalModeDataSet { get; set; }

        /// <summary>
        /// The <c>FunctionalMode</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("FunctionalModeTable")]
        public IEnumerable<JsonEventTable> FunctionalModeTable { get; set; }


        /// <summary>
        /// The <c>Hardness</c> events reported with the scalar VALUE representation.
        /// Hardness of a material.
        /// </summary>
        [JsonPropertyName("Hardness")]
        public IEnumerable<JsonEventValue> Hardness { get; set; }

        /// <summary>
        /// The <c>Hardness</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("HardnessDataSet")]
        public IEnumerable<JsonEventDataSet> HardnessDataSet { get; set; }

        /// <summary>
        /// The <c>Hardness</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("HardnessTable")]
        public IEnumerable<JsonEventTable> HardnessTable { get; set; }


        /// <summary>
        /// The <c>Hardware</c> events reported with the scalar VALUE representation.
        /// Hardware of a Component.
        /// </summary>
        [JsonPropertyName("Hardware")]
        public IEnumerable<JsonEventValue> Hardware { get; set; }

        /// <summary>
        /// The <c>Hardware</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("HardwareDataSet")]
        public IEnumerable<JsonEventDataSet> HardwareDataSet { get; set; }

        /// <summary>
        /// The <c>Hardware</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("HardwareTable")]
        public IEnumerable<JsonEventTable> HardwareTable { get; set; }


        /// <summary>
        /// The <c>HostName</c> events reported with the scalar VALUE representation.
        /// Name of the host computer supplying data.
        /// </summary>
        [JsonPropertyName("HostName")]
        public IEnumerable<JsonEventValue> HostName { get; set; }

        /// <summary>
        /// The <c>HostName</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("HostNameDataSet")]
        public IEnumerable<JsonEventDataSet> HostNameDataSet { get; set; }

        /// <summary>
        /// The <c>HostName</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("HostNameTable")]
        public IEnumerable<JsonEventTable> HostNameTable { get; set; }


        /// <summary>
        /// The <c>LeakDetect</c> events reported with the scalar VALUE representation.
        /// Indication designating whether a leak has been detected.
        /// </summary>
        [JsonPropertyName("LeakDetect")]
        public IEnumerable<JsonEventValue> LeakDetect { get; set; }

        /// <summary>
        /// The <c>LeakDetect</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LeakDetectDataSet")]
        public IEnumerable<JsonEventDataSet> LeakDetectDataSet { get; set; }

        /// <summary>
        /// The <c>LeakDetect</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LeakDetectTable")]
        public IEnumerable<JsonEventTable> LeakDetectTable { get; set; }


        /// <summary>
        /// The <c>Library</c> events reported with the scalar VALUE representation.
        /// Software library on a Component
        /// </summary>
        [JsonPropertyName("Library")]
        public IEnumerable<JsonEventValue> Library { get; set; }

        /// <summary>
        /// The <c>Library</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LibraryDataSet")]
        public IEnumerable<JsonEventDataSet> LibraryDataSet { get; set; }

        /// <summary>
        /// The <c>Library</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LibraryTable")]
        public IEnumerable<JsonEventTable> LibraryTable { get; set; }


        /// <summary>
        /// The <c>Line</c> events reported with the scalar VALUE representation.
        /// Current line of code being executed.**DEPRECATED** in *Version 1.4.0*.
        /// </summary>
        [JsonPropertyName("Line")]
        public IEnumerable<JsonEventValue> Line { get; set; }

        /// <summary>
        /// The <c>Line</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LineDataSet")]
        public IEnumerable<JsonEventDataSet> LineDataSet { get; set; }

        /// <summary>
        /// The <c>Line</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LineTable")]
        public IEnumerable<JsonEventTable> LineTable { get; set; }


        /// <summary>
        /// The <c>LineLabel</c> events reported with the scalar VALUE representation.
        /// Identifier for a Block of code in a Program.
        /// </summary>
        [JsonPropertyName("LineLabel")]
        public IEnumerable<JsonEventValue> LineLabel { get; set; }

        /// <summary>
        /// The <c>LineLabel</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LineLabelDataSet")]
        public IEnumerable<JsonEventDataSet> LineLabelDataSet { get; set; }

        /// <summary>
        /// The <c>LineLabel</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LineLabelTable")]
        public IEnumerable<JsonEventTable> LineLabelTable { get; set; }


        /// <summary>
        /// The <c>LineNumber</c> events reported with the scalar VALUE representation.
        /// Position of a block of program code within a control program.
        /// </summary>
        [JsonPropertyName("LineNumber")]
        public IEnumerable<JsonEventValue> LineNumber { get; set; }

        /// <summary>
        /// The <c>LineNumber</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LineNumberDataSet")]
        public IEnumerable<JsonEventDataSet> LineNumberDataSet { get; set; }

        /// <summary>
        /// The <c>LineNumber</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LineNumberTable")]
        public IEnumerable<JsonEventTable> LineNumberTable { get; set; }


        /// <summary>
        /// The <c>LoadCount</c> events reported with the scalar VALUE representation.
        /// Accumulation of the number of times an operation has attempted to, or is planned to attempt to, load materials, parts, or other items.
        /// </summary>
        [JsonPropertyName("LoadCount")]
        public IEnumerable<JsonEventValue> LoadCount { get; set; }

        /// <summary>
        /// The <c>LoadCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LoadCountDataSet")]
        public IEnumerable<JsonEventDataSet> LoadCountDataSet { get; set; }

        /// <summary>
        /// The <c>LoadCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LoadCountTable")]
        public IEnumerable<JsonEventTable> LoadCountTable { get; set; }


        /// <summary>
        /// The <c>LocationAddress</c> events reported with the scalar VALUE representation.
        /// Structured information that allows the unambiguous determination of an object for purposes of identification and location. ISO 19160-4:2017
        /// </summary>
        [JsonPropertyName("LocationAddress")]
        public IEnumerable<JsonEventValue> LocationAddress { get; set; }

        /// <summary>
        /// The <c>LocationAddress</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LocationAddressDataSet")]
        public IEnumerable<JsonEventDataSet> LocationAddressDataSet { get; set; }

        /// <summary>
        /// The <c>LocationAddress</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LocationAddressTable")]
        public IEnumerable<JsonEventTable> LocationAddressTable { get; set; }


        /// <summary>
        /// The <c>LocationNarrative</c> events reported with the scalar VALUE representation.
        /// Textual description of the location of an object or activity.
        /// </summary>
        [JsonPropertyName("LocationNarrative")]
        public IEnumerable<JsonEventValue> LocationNarrative { get; set; }

        /// <summary>
        /// The <c>LocationNarrative</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LocationNarrativeDataSet")]
        public IEnumerable<JsonEventDataSet> LocationNarrativeDataSet { get; set; }

        /// <summary>
        /// The <c>LocationNarrative</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LocationNarrativeTable")]
        public IEnumerable<JsonEventTable> LocationNarrativeTable { get; set; }


        /// <summary>
        /// The <c>LocationSpatialGeographic</c> events reported with the scalar VALUE representation.
        /// Absolute geographic location defined by two coordinates, longitude and latitude and an elevation.
        /// </summary>
        [JsonPropertyName("LocationSpatialGeographic")]
        public IEnumerable<JsonEventValue> LocationSpatialGeographic { get; set; }

        /// <summary>
        /// The <c>LocationSpatialGeographic</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LocationSpatialGeographicDataSet")]
        public IEnumerable<JsonEventDataSet> LocationSpatialGeographicDataSet { get; set; }

        /// <summary>
        /// The <c>LocationSpatialGeographic</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LocationSpatialGeographicTable")]
        public IEnumerable<JsonEventTable> LocationSpatialGeographicTable { get; set; }


        /// <summary>
        /// The <c>LockState</c> events reported with the scalar VALUE representation.
        /// State or operating mode of a Lock.
        /// </summary>
        [JsonPropertyName("LockState")]
        public IEnumerable<JsonEventValue> LockState { get; set; }

        /// <summary>
        /// The <c>LockState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("LockStateDataSet")]
        public IEnumerable<JsonEventDataSet> LockStateDataSet { get; set; }

        /// <summary>
        /// The <c>LockState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("LockStateTable")]
        public IEnumerable<JsonEventTable> LockStateTable { get; set; }


        /// <summary>
        /// The <c>MaintenanceList</c> events reported with the scalar VALUE representation.
        /// Actions or activities to be performed in support of a piece of equipment.
        /// </summary>
        [JsonPropertyName("MaintenanceList")]
        public IEnumerable<JsonEventValue> MaintenanceList { get; set; }

        /// <summary>
        /// The <c>MaintenanceList</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MaintenanceListDataSet")]
        public IEnumerable<JsonEventDataSet> MaintenanceListDataSet { get; set; }

        /// <summary>
        /// The <c>MaintenanceList</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MaintenanceListTable")]
        public IEnumerable<JsonEventTable> MaintenanceListTable { get; set; }


        /// <summary>
        /// The <c>Material</c> events reported with the scalar VALUE representation.
        /// Identifier of a material used or consumed in the manufacturing process
        /// </summary>
        [JsonPropertyName("Material")]
        public IEnumerable<JsonEventValue> Material { get; set; }

        /// <summary>
        /// The <c>Material</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MaterialDataSet")]
        public IEnumerable<JsonEventDataSet> MaterialDataSet { get; set; }

        /// <summary>
        /// The <c>Material</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MaterialTable")]
        public IEnumerable<JsonEventTable> MaterialTable { get; set; }


        /// <summary>
        /// The <c>MaterialLayer</c> events reported with the scalar VALUE representation.
        /// Identifies the layers of material applied to a part or product as part of an additive manufacturing process.
        /// </summary>
        [JsonPropertyName("MaterialLayer")]
        public IEnumerable<JsonEventValue> MaterialLayer { get; set; }

        /// <summary>
        /// The <c>MaterialLayer</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MaterialLayerDataSet")]
        public IEnumerable<JsonEventDataSet> MaterialLayerDataSet { get; set; }

        /// <summary>
        /// The <c>MaterialLayer</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MaterialLayerTable")]
        public IEnumerable<JsonEventTable> MaterialLayerTable { get; set; }


        /// <summary>
        /// The <c>MeasurementType</c> events reported with the scalar VALUE representation.
        /// Class of measurement being performed. QIF 3:2018 Section 6.3
        /// </summary>
        [JsonPropertyName("MeasurementType")]
        public IEnumerable<JsonEventValue> MeasurementType { get; set; }

        /// <summary>
        /// The <c>MeasurementType</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MeasurementTypeDataSet")]
        public IEnumerable<JsonEventDataSet> MeasurementTypeDataSet { get; set; }

        /// <summary>
        /// The <c>MeasurementType</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MeasurementTypeTable")]
        public IEnumerable<JsonEventTable> MeasurementTypeTable { get; set; }


        /// <summary>
        /// The <c>MeasurementUnits</c> events reported with the scalar VALUE representation.
        /// Engineering units of the measurement.
        /// </summary>
        [JsonPropertyName("MeasurementUnits")]
        public IEnumerable<JsonEventValue> MeasurementUnits { get; set; }

        /// <summary>
        /// The <c>MeasurementUnits</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MeasurementUnitsDataSet")]
        public IEnumerable<JsonEventDataSet> MeasurementUnitsDataSet { get; set; }

        /// <summary>
        /// The <c>MeasurementUnits</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MeasurementUnitsTable")]
        public IEnumerable<JsonEventTable> MeasurementUnitsTable { get; set; }


        /// <summary>
        /// The <c>MeasurementValue</c> events reported with the scalar VALUE representation.
        /// Measurement based on the measurement type.
        /// </summary>
        [JsonPropertyName("MeasurementValue")]
        public IEnumerable<JsonEventValue> MeasurementValue { get; set; }

        /// <summary>
        /// The <c>MeasurementValue</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MeasurementValueDataSet")]
        public IEnumerable<JsonEventDataSet> MeasurementValueDataSet { get; set; }

        /// <summary>
        /// The <c>MeasurementValue</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MeasurementValueTable")]
        public IEnumerable<JsonEventTable> MeasurementValueTable { get; set; }


        /// <summary>
        /// The <c>Message</c> events reported with the scalar VALUE representation.
        /// Information to be transferred from a piece of equipment to a client software application.
        /// </summary>
        [JsonPropertyName("Message")]
        public IEnumerable<JsonEventValue> Message { get; set; }

        /// <summary>
        /// The <c>Message</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MessageDataSet")]
        public IEnumerable<JsonEventDataSet> MessageDataSet { get; set; }

        /// <summary>
        /// The <c>Message</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MessageTable")]
        public IEnumerable<JsonEventTable> MessageTable { get; set; }


        /// <summary>
        /// The <c>MTConnectVersion</c> events reported with the scalar VALUE representation.
        /// Reference version of the MTConnect Standard supported by the adapter.
        /// </summary>
        [JsonPropertyName("MTConnectVersion")]
        public IEnumerable<JsonEventValue> MTConnectVersion { get; set; }

        /// <summary>
        /// The <c>MTConnectVersion</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("MTConnectVersionDataSet")]
        public IEnumerable<JsonEventDataSet> MTConnectVersionDataSet { get; set; }

        /// <summary>
        /// The <c>MTConnectVersion</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("MTConnectVersionTable")]
        public IEnumerable<JsonEventTable> MTConnectVersionTable { get; set; }


        /// <summary>
        /// The <c>Network</c> events reported with the scalar VALUE representation.
        /// Network details of a Component.
        /// </summary>
        [JsonPropertyName("Network")]
        public IEnumerable<JsonEventValue> Network { get; set; }

        /// <summary>
        /// The <c>Network</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("NetworkDataSet")]
        public IEnumerable<JsonEventDataSet> NetworkDataSet { get; set; }

        /// <summary>
        /// The <c>Network</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("NetworkTable")]
        public IEnumerable<JsonEventTable> NetworkTable { get; set; }


        /// <summary>
        /// The <c>NetworkPort</c> events reported with the scalar VALUE representation.
        /// Number of the TCP/IP or UDP/IP port for the connection endpoint.
        /// </summary>
        [JsonPropertyName("NetworkPort")]
        public IEnumerable<JsonEventValue> NetworkPort { get; set; }

        /// <summary>
        /// The <c>NetworkPort</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("NetworkPortDataSet")]
        public IEnumerable<JsonEventDataSet> NetworkPortDataSet { get; set; }

        /// <summary>
        /// The <c>NetworkPort</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("NetworkPortTable")]
        public IEnumerable<JsonEventTable> NetworkPortTable { get; set; }


        /// <summary>
        /// The <c>OperatingMode</c> events reported with the scalar VALUE representation.
        /// State of Component or Composition that describes the automatic or manual operation of the entity.
        /// </summary>
        [JsonPropertyName("OperatingMode")]
        public IEnumerable<JsonEventValue> OperatingMode { get; set; }

        /// <summary>
        /// The <c>OperatingMode</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("OperatingModeDataSet")]
        public IEnumerable<JsonEventDataSet> OperatingModeDataSet { get; set; }

        /// <summary>
        /// The <c>OperatingMode</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("OperatingModeTable")]
        public IEnumerable<JsonEventTable> OperatingModeTable { get; set; }


        /// <summary>
        /// The <c>OperatingSystem</c> events reported with the scalar VALUE representation.
        /// Operating System (OS) of a Component.
        /// </summary>
        [JsonPropertyName("OperatingSystem")]
        public IEnumerable<JsonEventValue> OperatingSystem { get; set; }

        /// <summary>
        /// The <c>OperatingSystem</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("OperatingSystemDataSet")]
        public IEnumerable<JsonEventDataSet> OperatingSystemDataSet { get; set; }

        /// <summary>
        /// The <c>OperatingSystem</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("OperatingSystemTable")]
        public IEnumerable<JsonEventTable> OperatingSystemTable { get; set; }


        /// <summary>
        /// The <c>OperatorId</c> events reported with the scalar VALUE representation.
        /// Identifier of the person currently responsible for operating the piece of equipment.
        /// </summary>
        [JsonPropertyName("OperatorId")]
        public IEnumerable<JsonEventValue> OperatorId { get; set; }

        /// <summary>
        /// The <c>OperatorId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("OperatorIdDataSet")]
        public IEnumerable<JsonEventDataSet> OperatorIdDataSet { get; set; }

        /// <summary>
        /// The <c>OperatorId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("OperatorIdTable")]
        public IEnumerable<JsonEventTable> OperatorIdTable { get; set; }


        /// <summary>
        /// The <c>PalletId</c> events reported with the scalar VALUE representation.
        /// Identifier for a pallet.
        /// </summary>
        [JsonPropertyName("PalletId")]
        public IEnumerable<JsonEventValue> PalletId { get; set; }

        /// <summary>
        /// The <c>PalletId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PalletIdDataSet")]
        public IEnumerable<JsonEventDataSet> PalletIdDataSet { get; set; }

        /// <summary>
        /// The <c>PalletId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PalletIdTable")]
        public IEnumerable<JsonEventTable> PalletIdTable { get; set; }


        /// <summary>
        /// The <c>PartCount</c> events reported with the scalar VALUE representation.
        /// Aggregate count of parts.
        /// </summary>
        [JsonPropertyName("PartCount")]
        public IEnumerable<JsonEventValue> PartCount { get; set; }

        /// <summary>
        /// The <c>PartCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartCountDataSet")]
        public IEnumerable<JsonEventDataSet> PartCountDataSet { get; set; }

        /// <summary>
        /// The <c>PartCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartCountTable")]
        public IEnumerable<JsonEventTable> PartCountTable { get; set; }


        /// <summary>
        /// The <c>PartCountType</c> events reported with the scalar VALUE representation.
        /// Interpretation of `PART_COUNT`.
        /// </summary>
        [JsonPropertyName("PartCountType")]
        public IEnumerable<JsonEventValue> PartCountType { get; set; }

        /// <summary>
        /// The <c>PartCountType</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartCountTypeDataSet")]
        public IEnumerable<JsonEventDataSet> PartCountTypeDataSet { get; set; }

        /// <summary>
        /// The <c>PartCountType</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartCountTypeTable")]
        public IEnumerable<JsonEventTable> PartCountTypeTable { get; set; }


        /// <summary>
        /// The <c>PartDetect</c> events reported with the scalar VALUE representation.
        /// Indication designating whether a part or work piece has been detected or is present.
        /// </summary>
        [JsonPropertyName("PartDetect")]
        public IEnumerable<JsonEventValue> PartDetect { get; set; }

        /// <summary>
        /// The <c>PartDetect</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartDetectDataSet")]
        public IEnumerable<JsonEventDataSet> PartDetectDataSet { get; set; }

        /// <summary>
        /// The <c>PartDetect</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartDetectTable")]
        public IEnumerable<JsonEventTable> PartDetectTable { get; set; }


        /// <summary>
        /// The <c>PartGroupId</c> events reported with the scalar VALUE representation.
        /// Identifier given to a collection of individual parts.
        /// </summary>
        [JsonPropertyName("PartGroupId")]
        public IEnumerable<JsonEventValue> PartGroupId { get; set; }

        /// <summary>
        /// The <c>PartGroupId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartGroupIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartGroupIdDataSet { get; set; }

        /// <summary>
        /// The <c>PartGroupId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartGroupIdTable")]
        public IEnumerable<JsonEventTable> PartGroupIdTable { get; set; }


        /// <summary>
        /// The <c>PartId</c> events reported with the scalar VALUE representation.
        /// Identifier of a part in a manufacturing operation.
        /// </summary>
        [JsonPropertyName("PartId")]
        public IEnumerable<JsonEventValue> PartId { get; set; }

        /// <summary>
        /// The <c>PartId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartIdDataSet { get; set; }

        /// <summary>
        /// The <c>PartId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartIdTable")]
        public IEnumerable<JsonEventTable> PartIdTable { get; set; }


        /// <summary>
        /// The <c>PartIndex</c> events reported with the scalar VALUE representation.
        /// Sequence of a part in a group of parts.
        /// </summary>
        [JsonPropertyName("PartIndex")]
        public IEnumerable<JsonEventValue> PartIndex { get; set; }

        /// <summary>
        /// The <c>PartIndex</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartIndexDataSet")]
        public IEnumerable<JsonEventDataSet> PartIndexDataSet { get; set; }

        /// <summary>
        /// The <c>PartIndex</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartIndexTable")]
        public IEnumerable<JsonEventTable> PartIndexTable { get; set; }


        /// <summary>
        /// The <c>PartKindId</c> events reported with the scalar VALUE representation.
        /// Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.
        /// </summary>
        [JsonPropertyName("PartKindId")]
        public IEnumerable<JsonEventValue> PartKindId { get; set; }

        /// <summary>
        /// The <c>PartKindId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartKindIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartKindIdDataSet { get; set; }

        /// <summary>
        /// The <c>PartKindId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartKindIdTable")]
        public IEnumerable<JsonEventTable> PartKindIdTable { get; set; }


        /// <summary>
        /// The <c>PartNumber</c> events reported with the scalar VALUE representation.
        /// Identifier of a part or product moving through the manufacturing process.**DEPRECATED** in *Version 1.7*. `PART_NUMBER` is now a `subType` of `PART_KIND_ID`.
        /// </summary>
        [JsonPropertyName("PartNumber")]
        public IEnumerable<JsonEventValue> PartNumber { get; set; }

        /// <summary>
        /// The <c>PartNumber</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartNumberDataSet")]
        public IEnumerable<JsonEventDataSet> PartNumberDataSet { get; set; }

        /// <summary>
        /// The <c>PartNumber</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartNumberTable")]
        public IEnumerable<JsonEventTable> PartNumberTable { get; set; }


        /// <summary>
        /// The <c>PartProcessingState</c> events reported with the scalar VALUE representation.
        /// Particular condition of the part occurrence at a specific time.
        /// </summary>
        [JsonPropertyName("PartProcessingState")]
        public IEnumerable<JsonEventValue> PartProcessingState { get; set; }

        /// <summary>
        /// The <c>PartProcessingState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartProcessingStateDataSet")]
        public IEnumerable<JsonEventDataSet> PartProcessingStateDataSet { get; set; }

        /// <summary>
        /// The <c>PartProcessingState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartProcessingStateTable")]
        public IEnumerable<JsonEventTable> PartProcessingStateTable { get; set; }


        /// <summary>
        /// The <c>PartStatus</c> events reported with the scalar VALUE representation.
        /// State or condition of a part.
        /// </summary>
        [JsonPropertyName("PartStatus")]
        public IEnumerable<JsonEventValue> PartStatus { get; set; }

        /// <summary>
        /// The <c>PartStatus</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartStatusDataSet")]
        public IEnumerable<JsonEventDataSet> PartStatusDataSet { get; set; }

        /// <summary>
        /// The <c>PartStatus</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartStatusTable")]
        public IEnumerable<JsonEventTable> PartStatusTable { get; set; }


        /// <summary>
        /// The <c>PartUniqueId</c> events reported with the scalar VALUE representation.
        /// Identifier given to a distinguishable, individual part.
        /// </summary>
        [JsonPropertyName("PartUniqueId")]
        public IEnumerable<JsonEventValue> PartUniqueId { get; set; }

        /// <summary>
        /// The <c>PartUniqueId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PartUniqueIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartUniqueIdDataSet { get; set; }

        /// <summary>
        /// The <c>PartUniqueId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PartUniqueIdTable")]
        public IEnumerable<JsonEventTable> PartUniqueIdTable { get; set; }


        /// <summary>
        /// The <c>PathFeedrateOverride</c> events reported with the scalar VALUE representation.
        /// Value of a signal or calculation issued to adjust the feedrate for the axes associated with a Path component that may represent a single axis or the coordinated movement of multiple axes.
        /// </summary>
        [JsonPropertyName("PathFeedrateOverride")]
        public IEnumerable<JsonEventValue> PathFeedrateOverride { get; set; }

        /// <summary>
        /// The <c>PathFeedrateOverride</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PathFeedrateOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> PathFeedrateOverrideDataSet { get; set; }

        /// <summary>
        /// The <c>PathFeedrateOverride</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PathFeedrateOverrideTable")]
        public IEnumerable<JsonEventTable> PathFeedrateOverrideTable { get; set; }


        /// <summary>
        /// The <c>PathMode</c> events reported with the scalar VALUE representation.
        /// Describes the operational relationship between a Path entity and another Path entity for pieces of equipment comprised of multiple logical groupings of controlled axes or other logical operations.
        /// </summary>
        [JsonPropertyName("PathMode")]
        public IEnumerable<JsonEventValue> PathMode { get; set; }

        /// <summary>
        /// The <c>PathMode</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PathModeDataSet")]
        public IEnumerable<JsonEventDataSet> PathModeDataSet { get; set; }

        /// <summary>
        /// The <c>PathMode</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PathModeTable")]
        public IEnumerable<JsonEventTable> PathModeTable { get; set; }


        /// <summary>
        /// The <c>PowerState</c> events reported with the scalar VALUE representation.
        /// Indication of the status of the source of energy for an entity to allow it to perform its intended function or the state of an enabling signal providing permission for the entity to perform its functions.
        /// </summary>
        [JsonPropertyName("PowerState")]
        public IEnumerable<JsonEventValue> PowerState { get; set; }

        /// <summary>
        /// The <c>PowerState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PowerStateDataSet")]
        public IEnumerable<JsonEventDataSet> PowerStateDataSet { get; set; }

        /// <summary>
        /// The <c>PowerState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PowerStateTable")]
        public IEnumerable<JsonEventTable> PowerStateTable { get; set; }


        /// <summary>
        /// The <c>PowerStatus</c> events reported with the scalar VALUE representation.
        /// Status of the Component.**DEPRECATED** in *Version 1.1.0*.
        /// </summary>
        [JsonPropertyName("PowerStatus")]
        public IEnumerable<JsonEventValue> PowerStatus { get; set; }

        /// <summary>
        /// The <c>PowerStatus</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("PowerStatusDataSet")]
        public IEnumerable<JsonEventDataSet> PowerStatusDataSet { get; set; }

        /// <summary>
        /// The <c>PowerStatus</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("PowerStatusTable")]
        public IEnumerable<JsonEventTable> PowerStatusTable { get; set; }


        /// <summary>
        /// The <c>ProcessAggregateId</c> events reported with the scalar VALUE representation.
        /// Identifier given to link the individual occurrence to a group of related occurrences, such as a process step in a process plan.
        /// </summary>
        [JsonPropertyName("ProcessAggregateId")]
        public IEnumerable<JsonEventValue> ProcessAggregateId { get; set; }

        /// <summary>
        /// The <c>ProcessAggregateId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProcessAggregateIdDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessAggregateIdDataSet { get; set; }

        /// <summary>
        /// The <c>ProcessAggregateId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProcessAggregateIdTable")]
        public IEnumerable<JsonEventTable> ProcessAggregateIdTable { get; set; }


        /// <summary>
        /// The <c>ProcessKindId</c> events reported with the scalar VALUE representation.
        /// Identifier given to link the individual occurrence to a class of processes or process definition.
        /// </summary>
        [JsonPropertyName("ProcessKindId")]
        public IEnumerable<JsonEventValue> ProcessKindId { get; set; }

        /// <summary>
        /// The <c>ProcessKindId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProcessKindIdDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessKindIdDataSet { get; set; }

        /// <summary>
        /// The <c>ProcessKindId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProcessKindIdTable")]
        public IEnumerable<JsonEventTable> ProcessKindIdTable { get; set; }


        /// <summary>
        /// The <c>ProcessOccurrenceId</c> events reported with the scalar VALUE representation.
        /// Identifier of a process being executed by the device.
        /// </summary>
        [JsonPropertyName("ProcessOccurrenceId")]
        public IEnumerable<JsonEventValue> ProcessOccurrenceId { get; set; }

        /// <summary>
        /// The <c>ProcessOccurrenceId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProcessOccurrenceIdDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessOccurrenceIdDataSet { get; set; }

        /// <summary>
        /// The <c>ProcessOccurrenceId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProcessOccurrenceIdTable")]
        public IEnumerable<JsonEventTable> ProcessOccurrenceIdTable { get; set; }


        /// <summary>
        /// The <c>ProcessState</c> events reported with the scalar VALUE representation.
        /// Particular condition of the process occurrence at a specific time.
        /// </summary>
        [JsonPropertyName("ProcessState")]
        public IEnumerable<JsonEventValue> ProcessState { get; set; }

        /// <summary>
        /// The <c>ProcessState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProcessStateDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessStateDataSet { get; set; }

        /// <summary>
        /// The <c>ProcessState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProcessStateTable")]
        public IEnumerable<JsonEventTable> ProcessStateTable { get; set; }


        /// <summary>
        /// The <c>ProcessTime</c> events reported with the scalar VALUE representation.
        /// Time and date associated with an activity or event.
        /// </summary>
        [JsonPropertyName("ProcessTime")]
        public IEnumerable<JsonEventValue> ProcessTime { get; set; }

        /// <summary>
        /// The <c>ProcessTime</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProcessTimeDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessTimeDataSet { get; set; }

        /// <summary>
        /// The <c>ProcessTime</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProcessTimeTable")]
        public IEnumerable<JsonEventTable> ProcessTimeTable { get; set; }


        /// <summary>
        /// The <c>Program</c> events reported with the scalar VALUE representation.
        /// Name of the logic or motion program being executed by the Controller component.
        /// </summary>
        [JsonPropertyName("Program")]
        public IEnumerable<JsonEventValue> Program { get; set; }

        /// <summary>
        /// The <c>Program</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramDataSet { get; set; }

        /// <summary>
        /// The <c>Program</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramTable")]
        public IEnumerable<JsonEventTable> ProgramTable { get; set; }


        /// <summary>
        /// The <c>ProgramComment</c> events reported with the scalar VALUE representation.
        /// Comment or non-executable statement in the control program.
        /// </summary>
        [JsonPropertyName("ProgramComment")]
        public IEnumerable<JsonEventValue> ProgramComment { get; set; }

        /// <summary>
        /// The <c>ProgramComment</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramCommentDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramCommentDataSet { get; set; }

        /// <summary>
        /// The <c>ProgramComment</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramCommentTable")]
        public IEnumerable<JsonEventTable> ProgramCommentTable { get; set; }


        /// <summary>
        /// The <c>ProgramEdit</c> events reported with the scalar VALUE representation.
        /// Indication of the status of the Controller components program editing mode.A program may be edited while another is executed.
        /// </summary>
        [JsonPropertyName("ProgramEdit")]
        public IEnumerable<JsonEventValue> ProgramEdit { get; set; }

        /// <summary>
        /// The <c>ProgramEdit</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramEditDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramEditDataSet { get; set; }

        /// <summary>
        /// The <c>ProgramEdit</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramEditTable")]
        public IEnumerable<JsonEventTable> ProgramEditTable { get; set; }


        /// <summary>
        /// The <c>ProgramEditName</c> events reported with the scalar VALUE representation.
        /// Name of the program being edited. This is used in conjunction with ProgramEdit when in `ACTIVE` state.
        /// </summary>
        [JsonPropertyName("ProgramEditName")]
        public IEnumerable<JsonEventValue> ProgramEditName { get; set; }

        /// <summary>
        /// The <c>ProgramEditName</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramEditNameDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramEditNameDataSet { get; set; }

        /// <summary>
        /// The <c>ProgramEditName</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramEditNameTable")]
        public IEnumerable<JsonEventTable> ProgramEditNameTable { get; set; }


        /// <summary>
        /// The <c>ProgramHeader</c> events reported with the scalar VALUE representation.
        /// Non-executable header section of the control program.
        /// </summary>
        [JsonPropertyName("ProgramHeader")]
        public IEnumerable<JsonEventValue> ProgramHeader { get; set; }

        /// <summary>
        /// The <c>ProgramHeader</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramHeaderDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramHeaderDataSet { get; set; }

        /// <summary>
        /// The <c>ProgramHeader</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramHeaderTable")]
        public IEnumerable<JsonEventTable> ProgramHeaderTable { get; set; }


        /// <summary>
        /// The <c>ProgramLocation</c> events reported with the scalar VALUE representation.
        /// URI for the source file associated with Program.
        /// </summary>
        [JsonPropertyName("ProgramLocation")]
        public IEnumerable<JsonEventValue> ProgramLocation { get; set; }

        /// <summary>
        /// The <c>ProgramLocation</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramLocationDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramLocationDataSet { get; set; }

        /// <summary>
        /// The <c>ProgramLocation</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramLocationTable")]
        public IEnumerable<JsonEventTable> ProgramLocationTable { get; set; }


        /// <summary>
        /// The <c>ProgramLocationType</c> events reported with the scalar VALUE representation.
        /// Defines whether the logic or motion program defined by Program is being executed from the local memory of the controller or from an outside source.
        /// </summary>
        [JsonPropertyName("ProgramLocationType")]
        public IEnumerable<JsonEventValue> ProgramLocationType { get; set; }

        /// <summary>
        /// The <c>ProgramLocationType</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramLocationTypeDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramLocationTypeDataSet { get; set; }

        /// <summary>
        /// The <c>ProgramLocationType</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramLocationTypeTable")]
        public IEnumerable<JsonEventTable> ProgramLocationTypeTable { get; set; }


        /// <summary>
        /// The <c>ProgramNestLevel</c> events reported with the scalar VALUE representation.
        /// Indication of the nesting level within a control program that is associated with the code or instructions that is currently being executed.
        /// </summary>
        [JsonPropertyName("ProgramNestLevel")]
        public IEnumerable<JsonEventValue> ProgramNestLevel { get; set; }

        /// <summary>
        /// The <c>ProgramNestLevel</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ProgramNestLevelDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramNestLevelDataSet { get; set; }

        /// <summary>
        /// The <c>ProgramNestLevel</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ProgramNestLevelTable")]
        public IEnumerable<JsonEventTable> ProgramNestLevelTable { get; set; }


        /// <summary>
        /// The <c>RotaryMode</c> events reported with the scalar VALUE representation.
        /// Current operating mode for a Rotary type axis.
        /// </summary>
        [JsonPropertyName("RotaryMode")]
        public IEnumerable<JsonEventValue> RotaryMode { get; set; }

        /// <summary>
        /// The <c>RotaryMode</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("RotaryModeDataSet")]
        public IEnumerable<JsonEventDataSet> RotaryModeDataSet { get; set; }

        /// <summary>
        /// The <c>RotaryMode</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("RotaryModeTable")]
        public IEnumerable<JsonEventTable> RotaryModeTable { get; set; }


        /// <summary>
        /// The <c>RotaryVelocityOverride</c> events reported with the scalar VALUE representation.
        /// Percentage change to the velocity of the programmed velocity for a Rotary axis.
        /// </summary>
        [JsonPropertyName("RotaryVelocityOverride")]
        public IEnumerable<JsonEventValue> RotaryVelocityOverride { get; set; }

        /// <summary>
        /// The <c>RotaryVelocityOverride</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("RotaryVelocityOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> RotaryVelocityOverrideDataSet { get; set; }

        /// <summary>
        /// The <c>RotaryVelocityOverride</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("RotaryVelocityOverrideTable")]
        public IEnumerable<JsonEventTable> RotaryVelocityOverrideTable { get; set; }


        /// <summary>
        /// The <c>Rotation</c> events reported with the scalar VALUE representation.
        /// Three space angular displacement of an object or coordinate system relative to a cartesian coordinate system.
        /// </summary>
        [JsonPropertyName("Rotation")]
        public IEnumerable<JsonEventValue> Rotation { get; set; }

        /// <summary>
        /// The <c>Rotation</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("RotationDataSet")]
        public IEnumerable<JsonEventDataSet> RotationDataSet { get; set; }

        /// <summary>
        /// The <c>Rotation</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("RotationTable")]
        public IEnumerable<JsonEventTable> RotationTable { get; set; }


        /// <summary>
        /// The <c>SensorAttachment</c> events reported with the scalar VALUE representation.
        /// Attachment between a sensor and an entity.
        /// </summary>
        [JsonPropertyName("SensorAttachment")]
        public IEnumerable<JsonEventValue> SensorAttachment { get; set; }

        /// <summary>
        /// The <c>SensorAttachment</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SensorAttachmentDataSet")]
        public IEnumerable<JsonEventDataSet> SensorAttachmentDataSet { get; set; }

        /// <summary>
        /// The <c>SensorAttachment</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SensorAttachmentTable")]
        public IEnumerable<JsonEventTable> SensorAttachmentTable { get; set; }


        /// <summary>
        /// The <c>SensorState</c> events reported with the scalar VALUE representation.
        /// Detection result of a sensor.
        /// </summary>
        [JsonPropertyName("SensorState")]
        public IEnumerable<JsonEventValue> SensorState { get; set; }

        /// <summary>
        /// The <c>SensorState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SensorStateDataSet")]
        public IEnumerable<JsonEventDataSet> SensorStateDataSet { get; set; }

        /// <summary>
        /// The <c>SensorState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SensorStateTable")]
        public IEnumerable<JsonEventTable> SensorStateTable { get; set; }


        /// <summary>
        /// The <c>SerialNumber</c> events reported with the scalar VALUE representation.
        /// Serial number associated with a Component, Asset, or Device.
        /// </summary>
        [JsonPropertyName("SerialNumber")]
        public IEnumerable<JsonEventValue> SerialNumber { get; set; }

        /// <summary>
        /// The <c>SerialNumber</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SerialNumberDataSet")]
        public IEnumerable<JsonEventDataSet> SerialNumberDataSet { get; set; }

        /// <summary>
        /// The <c>SerialNumber</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SerialNumberTable")]
        public IEnumerable<JsonEventTable> SerialNumberTable { get; set; }


        /// <summary>
        /// The <c>SpecificationLimit</c> events reported with the scalar VALUE representation.
        /// Set of limits defining a range of values designating acceptable performance for a variable.**DEPRECATED** in *Version 2.5*. Replaced by  `SPECIFICATION_LIMITS`.
        /// </summary>
        [JsonPropertyName("SpecificationLimit")]
        public IEnumerable<JsonEventValue> SpecificationLimit { get; set; }

        /// <summary>
        /// The <c>SpecificationLimit</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SpecificationLimitDataSet")]
        public IEnumerable<JsonEventDataSet> SpecificationLimitDataSet { get; set; }

        /// <summary>
        /// The <c>SpecificationLimit</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SpecificationLimitTable")]
        public IEnumerable<JsonEventTable> SpecificationLimitTable { get; set; }


        /// <summary>
        /// The <c>SpecificationLimits</c> events reported with the scalar VALUE representation.
        /// Set of limits defining a range of values designating acceptable performance for a variable.
        /// </summary>
        [JsonPropertyName("SpecificationLimits")]
        public IEnumerable<JsonEventValue> SpecificationLimits { get; set; }

        /// <summary>
        /// The <c>SpecificationLimits</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SpecificationLimitsDataSet")]
        public IEnumerable<JsonEventDataSet> SpecificationLimitsDataSet { get; set; }

        /// <summary>
        /// The <c>SpecificationLimits</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SpecificationLimitsTable")]
        public IEnumerable<JsonEventTable> SpecificationLimitsTable { get; set; }


        /// <summary>
        /// The <c>SpindleInterlock</c> events reported with the scalar VALUE representation.
        /// Indication of the status of the spindle for a piece of equipment when power has been removed and it is free to rotate.
        /// </summary>
        [JsonPropertyName("SpindleInterlock")]
        public IEnumerable<JsonEventValue> SpindleInterlock { get; set; }

        /// <summary>
        /// The <c>SpindleInterlock</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SpindleInterlockDataSet")]
        public IEnumerable<JsonEventDataSet> SpindleInterlockDataSet { get; set; }

        /// <summary>
        /// The <c>SpindleInterlock</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SpindleInterlockTable")]
        public IEnumerable<JsonEventTable> SpindleInterlockTable { get; set; }


        /// <summary>
        /// The <c>SwingAngle</c> events reported with the scalar VALUE representation.
        /// Angular range over which the object is designed to move about a fixed axis or pivot
        /// </summary>
        [JsonPropertyName("SwingAngle")]
        public IEnumerable<JsonEventValue> SwingAngle { get; set; }

        /// <summary>
        /// The <c>SwingAngle</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SwingAngleDataSet")]
        public IEnumerable<JsonEventDataSet> SwingAngleDataSet { get; set; }

        /// <summary>
        /// The <c>SwingAngle</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SwingAngleTable")]
        public IEnumerable<JsonEventTable> SwingAngleTable { get; set; }


        /// <summary>
        /// The <c>SwingDiameter</c> events reported with the scalar VALUE representation.
        /// Maximal linear width (diameter) of the area described by the object’s movement about an axis
        /// </summary>
        [JsonPropertyName("SwingDiameter")]
        public IEnumerable<JsonEventValue> SwingDiameter { get; set; }

        /// <summary>
        /// The <c>SwingDiameter</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SwingDiameterDataSet")]
        public IEnumerable<JsonEventDataSet> SwingDiameterDataSet { get; set; }

        /// <summary>
        /// The <c>SwingDiameter</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SwingDiameterTable")]
        public IEnumerable<JsonEventTable> SwingDiameterTable { get; set; }


        /// <summary>
        /// The <c>SwingRadius</c> events reported with the scalar VALUE representation.
        /// Maximal linear distance from the pivot or axis to the furthest point reached by the object’s swing
        /// </summary>
        [JsonPropertyName("SwingRadius")]
        public IEnumerable<JsonEventValue> SwingRadius { get; set; }

        /// <summary>
        /// The <c>SwingRadius</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("SwingRadiusDataSet")]
        public IEnumerable<JsonEventDataSet> SwingRadiusDataSet { get; set; }

        /// <summary>
        /// The <c>SwingRadius</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("SwingRadiusTable")]
        public IEnumerable<JsonEventTable> SwingRadiusTable { get; set; }


        /// <summary>
        /// The <c>TaskAssetId</c> events reported with the scalar VALUE representation.
        /// AssetId of the Task that the Component binds to
        /// </summary>
        [JsonPropertyName("TaskAssetId")]
        public IEnumerable<JsonEventValue> TaskAssetId { get; set; }

        /// <summary>
        /// The <c>TaskAssetId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("TaskAssetIdDataSet")]
        public IEnumerable<JsonEventDataSet> TaskAssetIdDataSet { get; set; }

        /// <summary>
        /// The <c>TaskAssetId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("TaskAssetIdTable")]
        public IEnumerable<JsonEventTable> TaskAssetIdTable { get; set; }


        /// <summary>
        /// The <c>Thickness</c> events reported with the scalar VALUE representation.
        /// Dimension between two surfaces of an object, usually the dimension of smallest measure, for example an additive layer, or a depth of cut.
        /// </summary>
        [JsonPropertyName("Thickness")]
        public IEnumerable<JsonEventValue> Thickness { get; set; }

        /// <summary>
        /// The <c>Thickness</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ThicknessDataSet")]
        public IEnumerable<JsonEventDataSet> ThicknessDataSet { get; set; }

        /// <summary>
        /// The <c>Thickness</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ThicknessTable")]
        public IEnumerable<JsonEventTable> ThicknessTable { get; set; }


        /// <summary>
        /// The <c>ToolAssetId</c> events reported with the scalar VALUE representation.
        /// Identifier of an individual tool asset.
        /// </summary>
        [JsonPropertyName("ToolAssetId")]
        public IEnumerable<JsonEventValue> ToolAssetId { get; set; }

        /// <summary>
        /// The <c>ToolAssetId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ToolAssetIdDataSet")]
        public IEnumerable<JsonEventDataSet> ToolAssetIdDataSet { get; set; }

        /// <summary>
        /// The <c>ToolAssetId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ToolAssetIdTable")]
        public IEnumerable<JsonEventTable> ToolAssetIdTable { get; set; }


        /// <summary>
        /// The <c>ToolCuttingItem</c> events reported with the scalar VALUE representation.
        /// Indices of the currently active cutting tool edge.
        /// </summary>
        [JsonPropertyName("ToolCuttingItem")]
        public IEnumerable<JsonEventValue> ToolCuttingItem { get; set; }

        /// <summary>
        /// The <c>ToolCuttingItem</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ToolCuttingItemDataSet")]
        public IEnumerable<JsonEventDataSet> ToolCuttingItemDataSet { get; set; }

        /// <summary>
        /// The <c>ToolCuttingItem</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ToolCuttingItemTable")]
        public IEnumerable<JsonEventTable> ToolCuttingItemTable { get; set; }


        /// <summary>
        /// The <c>ToolGroup</c> events reported with the scalar VALUE representation.
        /// Identifier for the tool group associated with a specific tool. Commonly used to designate spare tools.
        /// </summary>
        [JsonPropertyName("ToolGroup")]
        public IEnumerable<JsonEventValue> ToolGroup { get; set; }

        /// <summary>
        /// The <c>ToolGroup</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ToolGroupDataSet")]
        public IEnumerable<JsonEventDataSet> ToolGroupDataSet { get; set; }

        /// <summary>
        /// The <c>ToolGroup</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ToolGroupTable")]
        public IEnumerable<JsonEventTable> ToolGroupTable { get; set; }


        /// <summary>
        /// The <c>ToolId</c> events reported with the scalar VALUE representation.
        /// Identifier of the tool currently in use for a given `Path`.**DEPRECATED** in *Version 1.2.0*.   See `TOOL_NUMBER`.
        /// </summary>
        [JsonPropertyName("ToolId")]
        public IEnumerable<JsonEventValue> ToolId { get; set; }

        /// <summary>
        /// The <c>ToolId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ToolIdDataSet")]
        public IEnumerable<JsonEventDataSet> ToolIdDataSet { get; set; }

        /// <summary>
        /// The <c>ToolId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ToolIdTable")]
        public IEnumerable<JsonEventTable> ToolIdTable { get; set; }


        /// <summary>
        /// The <c>ToolNumber</c> events reported with the scalar VALUE representation.
        /// Identifier assigned by the Controller component to a cutting tool when in use by a piece of equipment.
        /// </summary>
        [JsonPropertyName("ToolNumber")]
        public IEnumerable<JsonEventValue> ToolNumber { get; set; }

        /// <summary>
        /// The <c>ToolNumber</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ToolNumberDataSet")]
        public IEnumerable<JsonEventDataSet> ToolNumberDataSet { get; set; }

        /// <summary>
        /// The <c>ToolNumber</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ToolNumberTable")]
        public IEnumerable<JsonEventTable> ToolNumberTable { get; set; }


        /// <summary>
        /// The <c>ToolOffset</c> events reported with the scalar VALUE representation.
        /// Reference to the tool offset variables applied to the active cutting tool.
        /// </summary>
        [JsonPropertyName("ToolOffset")]
        public IEnumerable<JsonEventValue> ToolOffset { get; set; }

        /// <summary>
        /// The <c>ToolOffset</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ToolOffsetDataSet")]
        public IEnumerable<JsonEventDataSet> ToolOffsetDataSet { get; set; }

        /// <summary>
        /// The <c>ToolOffset</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ToolOffsetTable")]
        public IEnumerable<JsonEventTable> ToolOffsetTable { get; set; }


        /// <summary>
        /// The <c>ToolOffsets</c> events reported with the scalar VALUE representation.
        /// Properties of each addressable tool offset.
        /// </summary>
        [JsonPropertyName("ToolOffsets")]
        public IEnumerable<JsonEventValue> ToolOffsets { get; set; }

        /// <summary>
        /// The <c>ToolOffsets</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ToolOffsetsDataSet")]
        public IEnumerable<JsonEventDataSet> ToolOffsetsDataSet { get; set; }

        /// <summary>
        /// The <c>ToolOffsets</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ToolOffsetsTable")]
        public IEnumerable<JsonEventTable> ToolOffsetsTable { get; set; }


        /// <summary>
        /// The <c>TransferCount</c> events reported with the scalar VALUE representation.
        /// Accumulation of the number of times an operation has attempted to, or is planned to attempt to, transfer materials, parts, or other items from one location to another.
        /// </summary>
        [JsonPropertyName("TransferCount")]
        public IEnumerable<JsonEventValue> TransferCount { get; set; }

        /// <summary>
        /// The <c>TransferCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("TransferCountDataSet")]
        public IEnumerable<JsonEventDataSet> TransferCountDataSet { get; set; }

        /// <summary>
        /// The <c>TransferCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("TransferCountTable")]
        public IEnumerable<JsonEventTable> TransferCountTable { get; set; }


        /// <summary>
        /// The <c>Translation</c> events reported with the scalar VALUE representation.
        /// Three space linear displacement of an object or coordinate system relative to a cartesian coordinate system.
        /// </summary>
        [JsonPropertyName("Translation")]
        public IEnumerable<JsonEventValue> Translation { get; set; }

        /// <summary>
        /// The <c>Translation</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("TranslationDataSet")]
        public IEnumerable<JsonEventDataSet> TranslationDataSet { get; set; }

        /// <summary>
        /// The <c>Translation</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("TranslationTable")]
        public IEnumerable<JsonEventTable> TranslationTable { get; set; }


        /// <summary>
        /// The <c>Uncertainty</c> events reported with the scalar VALUE representation.
        /// Uncertainty specified by UncertaintyType.
        /// </summary>
        [JsonPropertyName("Uncertainty")]
        public IEnumerable<JsonEventValue> Uncertainty { get; set; }

        /// <summary>
        /// The <c>Uncertainty</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("UncertaintyDataSet")]
        public IEnumerable<JsonEventDataSet> UncertaintyDataSet { get; set; }

        /// <summary>
        /// The <c>Uncertainty</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("UncertaintyTable")]
        public IEnumerable<JsonEventTable> UncertaintyTable { get; set; }


        /// <summary>
        /// The <c>UncertaintyType</c> events reported with the scalar VALUE representation.
        /// Method used to compute standard uncertainty.
        /// </summary>
        [JsonPropertyName("UncertaintyType")]
        public IEnumerable<JsonEventValue> UncertaintyType { get; set; }

        /// <summary>
        /// The <c>UncertaintyType</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("UncertaintyTypeDataSet")]
        public IEnumerable<JsonEventDataSet> UncertaintyTypeDataSet { get; set; }

        /// <summary>
        /// The <c>UncertaintyType</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("UncertaintyTypeTable")]
        public IEnumerable<JsonEventTable> UncertaintyTypeTable { get; set; }


        /// <summary>
        /// The <c>UnloadCount</c> events reported with the scalar VALUE representation.
        /// Accumulation of the number of times an operation has attempted to, or is planned to attempt to, unload materials, parts, or other items.
        /// </summary>
        [JsonPropertyName("UnloadCount")]
        public IEnumerable<JsonEventValue> UnloadCount { get; set; }

        /// <summary>
        /// The <c>UnloadCount</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("UnloadCountDataSet")]
        public IEnumerable<JsonEventDataSet> UnloadCountDataSet { get; set; }

        /// <summary>
        /// The <c>UnloadCount</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("UnloadCountTable")]
        public IEnumerable<JsonEventTable> UnloadCountTable { get; set; }


        /// <summary>
        /// The <c>User</c> events reported with the scalar VALUE representation.
        /// Identifier of the person currently responsible for operating the piece of equipment.
        /// </summary>
        [JsonPropertyName("User")]
        public IEnumerable<JsonEventValue> User { get; set; }

        /// <summary>
        /// The <c>User</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("UserDataSet")]
        public IEnumerable<JsonEventDataSet> UserDataSet { get; set; }

        /// <summary>
        /// The <c>User</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("UserTable")]
        public IEnumerable<JsonEventTable> UserTable { get; set; }


        /// <summary>
        /// The <c>ValveState</c> events reported with the scalar VALUE representation.
        /// State of a valve is one of open, closed, or transitioning between the states.
        /// </summary>
        [JsonPropertyName("ValveState")]
        public IEnumerable<JsonEventValue> ValveState { get; set; }

        /// <summary>
        /// The <c>ValveState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("ValveStateDataSet")]
        public IEnumerable<JsonEventDataSet> ValveStateDataSet { get; set; }

        /// <summary>
        /// The <c>ValveState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("ValveStateTable")]
        public IEnumerable<JsonEventTable> ValveStateTable { get; set; }


        /// <summary>
        /// The <c>Variable</c> events reported with the scalar VALUE representation.
        /// Data whose meaning may change over time due to changes in the operation of a piece of equipment or the process being executed on that piece of equipment.
        /// </summary>
        [JsonPropertyName("Variable")]
        public IEnumerable<JsonEventValue> Variable { get; set; }

        /// <summary>
        /// The <c>Variable</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("VariableDataSet")]
        public IEnumerable<JsonEventDataSet> VariableDataSet { get; set; }

        /// <summary>
        /// The <c>Variable</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("VariableTable")]
        public IEnumerable<JsonEventTable> VariableTable { get; set; }


        /// <summary>
        /// The <c>WaitState</c> events reported with the scalar VALUE representation.
        /// Indication of the reason that Execution is reporting a value of `WAIT`.
        /// </summary>
        [JsonPropertyName("WaitState")]
        public IEnumerable<JsonEventValue> WaitState { get; set; }

        /// <summary>
        /// The <c>WaitState</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("WaitStateDataSet")]
        public IEnumerable<JsonEventDataSet> WaitStateDataSet { get; set; }

        /// <summary>
        /// The <c>WaitState</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("WaitStateTable")]
        public IEnumerable<JsonEventTable> WaitStateTable { get; set; }


        /// <summary>
        /// The <c>Wire</c> events reported with the scalar VALUE representation.
        /// Identifier for the type of wire used as the cutting mechanism in Electrical Discharge Machining or similar processes.
        /// </summary>
        [JsonPropertyName("Wire")]
        public IEnumerable<JsonEventValue> Wire { get; set; }

        /// <summary>
        /// The <c>Wire</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("WireDataSet")]
        public IEnumerable<JsonEventDataSet> WireDataSet { get; set; }

        /// <summary>
        /// The <c>Wire</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("WireTable")]
        public IEnumerable<JsonEventTable> WireTable { get; set; }


        /// <summary>
        /// The <c>WorkOffset</c> events reported with the scalar VALUE representation.
        /// Reference to offset variables for a work piece or part.
        /// </summary>
        [JsonPropertyName("WorkOffset")]
        public IEnumerable<JsonEventValue> WorkOffset { get; set; }

        /// <summary>
        /// The <c>WorkOffset</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("WorkOffsetDataSet")]
        public IEnumerable<JsonEventDataSet> WorkOffsetDataSet { get; set; }

        /// <summary>
        /// The <c>WorkOffset</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("WorkOffsetTable")]
        public IEnumerable<JsonEventTable> WorkOffsetTable { get; set; }


        /// <summary>
        /// The <c>WorkOffsets</c> events reported with the scalar VALUE representation.
        /// Properties of each addressable work offset.
        /// </summary>
        [JsonPropertyName("WorkOffsets")]
        public IEnumerable<JsonEventValue> WorkOffsets { get; set; }

        /// <summary>
        /// The <c>WorkOffsets</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("WorkOffsetsDataSet")]
        public IEnumerable<JsonEventDataSet> WorkOffsetsDataSet { get; set; }

        /// <summary>
        /// The <c>WorkOffsets</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("WorkOffsetsTable")]
        public IEnumerable<JsonEventTable> WorkOffsetsTable { get; set; }


        /// <summary>
        /// The <c>WorkholdingId</c> events reported with the scalar VALUE representation.
        /// Identifier for the current workholding or part clamp in use by a piece of equipment.**DEPRECATION WARNING**: Recommend using `FIXTURE_ID` instead.
        /// </summary>
        [JsonPropertyName("WorkholdingId")]
        public IEnumerable<JsonEventValue> WorkholdingId { get; set; }

        /// <summary>
        /// The <c>WorkholdingId</c> events reported with the DATA_SET representation
        /// (a set of key/value entries).
        /// </summary>
        [JsonPropertyName("WorkholdingIdDataSet")]
        public IEnumerable<JsonEventDataSet> WorkholdingIdDataSet { get; set; }

        /// <summary>
        /// The <c>WorkholdingId</c> events reported with the TABLE representation
        /// (a set of keyed rows of cells).
        /// </summary>
        [JsonPropertyName("WorkholdingIdTable")]
        public IEnumerable<JsonEventTable> WorkholdingIdTable { get; set; }



        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonEvents() { }

        /// <summary>
        /// Initializes a new instance from a flat sequence of <paramref name="observations"/>,
        /// routing each one into the typed collection matching its MTConnect type and
        /// representation.
        /// </summary>
        public JsonEvents(IEnumerable<IObservationOutput> observations)
        {
            if (observations != null)
            {
                if (!observations.IsNullOrEmpty())
                {
                    IEnumerable<IObservationOutput> typeObservations;
                    // Add ActivationCount
                    typeObservations = observations.Where(o => o.Type == ActivationCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ActivationCount = jsonObservations;
                    }

                    // Add ActivationCountDataSet
                    typeObservations = observations.Where(o => o.Type == ActivationCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ActivationCountDataSet = jsonObservations;
                    }

                    // Add ActivationCountTable
                    typeObservations = observations.Where(o => o.Type == ActivationCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ActivationCountTable = jsonObservations;
                    }


                    // Add ActiveAxes
                    typeObservations = observations.Where(o => o.Type == ActiveAxesDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ActiveAxes = jsonObservations;
                    }

                    // Add ActiveAxesDataSet
                    typeObservations = observations.Where(o => o.Type == ActiveAxesDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ActiveAxesDataSet = jsonObservations;
                    }

                    // Add ActiveAxesTable
                    typeObservations = observations.Where(o => o.Type == ActiveAxesDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ActiveAxesTable = jsonObservations;
                    }


                    // Add ActivePowerSource
                    typeObservations = observations.Where(o => o.Type == ActivePowerSourceDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ActivePowerSource = jsonObservations;
                    }

                    // Add ActivePowerSourceDataSet
                    typeObservations = observations.Where(o => o.Type == ActivePowerSourceDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ActivePowerSourceDataSet = jsonObservations;
                    }

                    // Add ActivePowerSourceTable
                    typeObservations = observations.Where(o => o.Type == ActivePowerSourceDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ActivePowerSourceTable = jsonObservations;
                    }


                    // Add ActuatorState
                    typeObservations = observations.Where(o => o.Type == ActuatorStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ActuatorState = jsonObservations;
                    }

                    // Add ActuatorStateDataSet
                    typeObservations = observations.Where(o => o.Type == ActuatorStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ActuatorStateDataSet = jsonObservations;
                    }

                    // Add ActuatorStateTable
                    typeObservations = observations.Where(o => o.Type == ActuatorStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ActuatorStateTable = jsonObservations;
                    }


                    // Add AdapterSoftwareVersion
                    typeObservations = observations.Where(o => o.Type == AdapterSoftwareVersionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AdapterSoftwareVersion = jsonObservations;
                    }

                    // Add AdapterSoftwareVersionDataSet
                    typeObservations = observations.Where(o => o.Type == AdapterSoftwareVersionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AdapterSoftwareVersionDataSet = jsonObservations;
                    }

                    // Add AdapterSoftwareVersionTable
                    typeObservations = observations.Where(o => o.Type == AdapterSoftwareVersionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AdapterSoftwareVersionTable = jsonObservations;
                    }


                    // Add AdapterUri
                    typeObservations = observations.Where(o => o.Type == AdapterUriDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AdapterUri = jsonObservations;
                    }

                    // Add AdapterUriDataSet
                    typeObservations = observations.Where(o => o.Type == AdapterUriDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AdapterUriDataSet = jsonObservations;
                    }

                    // Add AdapterUriTable
                    typeObservations = observations.Where(o => o.Type == AdapterUriDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AdapterUriTable = jsonObservations;
                    }


                    // Add Alarm
                    typeObservations = observations.Where(o => o.Type == AlarmDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Alarm = jsonObservations;
                    }

                    // Add AlarmDataSet
                    typeObservations = observations.Where(o => o.Type == AlarmDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AlarmDataSet = jsonObservations;
                    }

                    // Add AlarmTable
                    typeObservations = observations.Where(o => o.Type == AlarmDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AlarmTable = jsonObservations;
                    }


                    // Add AlarmLimit
                    typeObservations = observations.Where(o => o.Type == AlarmLimitDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AlarmLimit = jsonObservations;
                    }

                    // Add AlarmLimitDataSet
                    typeObservations = observations.Where(o => o.Type == AlarmLimitDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AlarmLimitDataSet = jsonObservations;
                    }

                    // Add AlarmLimitTable
                    typeObservations = observations.Where(o => o.Type == AlarmLimitDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AlarmLimitTable = jsonObservations;
                    }


                    // Add AlarmLimits
                    typeObservations = observations.Where(o => o.Type == AlarmLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AlarmLimits = jsonObservations;
                    }

                    // Add AlarmLimitsDataSet
                    typeObservations = observations.Where(o => o.Type == AlarmLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AlarmLimitsDataSet = jsonObservations;
                    }

                    // Add AlarmLimitsTable
                    typeObservations = observations.Where(o => o.Type == AlarmLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AlarmLimitsTable = jsonObservations;
                    }


                    // Add Application
                    typeObservations = observations.Where(o => o.Type == ApplicationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Application = jsonObservations;
                    }

                    // Add ApplicationDataSet
                    typeObservations = observations.Where(o => o.Type == ApplicationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ApplicationDataSet = jsonObservations;
                    }

                    // Add ApplicationTable
                    typeObservations = observations.Where(o => o.Type == ApplicationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ApplicationTable = jsonObservations;
                    }


                    // Add AssetAdded
                    typeObservations = observations.Where(o => o.Type == AssetAddedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AssetAdded = jsonObservations;
                    }

                    // Add AssetAddedDataSet
                    typeObservations = observations.Where(o => o.Type == AssetAddedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AssetAddedDataSet = jsonObservations;
                    }

                    // Add AssetAddedTable
                    typeObservations = observations.Where(o => o.Type == AssetAddedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AssetAddedTable = jsonObservations;
                    }


                    // Add AssetChanged
                    typeObservations = observations.Where(o => o.Type == AssetChangedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AssetChanged = jsonObservations;
                    }

                    // Add AssetChangedDataSet
                    typeObservations = observations.Where(o => o.Type == AssetChangedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AssetChangedDataSet = jsonObservations;
                    }

                    // Add AssetChangedTable
                    typeObservations = observations.Where(o => o.Type == AssetChangedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AssetChangedTable = jsonObservations;
                    }


                    // Add AssetCount
                    typeObservations = observations.Where(o => o.Type == AssetCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AssetCount = jsonObservations;
                    }

                    // Add AssetCountDataSet
                    typeObservations = observations.Where(o => o.Type == AssetCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AssetCountDataSet = jsonObservations;
                    }

                    // Add AssetCountTable
                    typeObservations = observations.Where(o => o.Type == AssetCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AssetCountTable = jsonObservations;
                    }


                    // Add AssetRemoved
                    typeObservations = observations.Where(o => o.Type == AssetRemovedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AssetRemoved = jsonObservations;
                    }

                    // Add AssetRemovedDataSet
                    typeObservations = observations.Where(o => o.Type == AssetRemovedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AssetRemovedDataSet = jsonObservations;
                    }

                    // Add AssetRemovedTable
                    typeObservations = observations.Where(o => o.Type == AssetRemovedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AssetRemovedTable = jsonObservations;
                    }


                    // Add AssociatedAssetId
                    typeObservations = observations.Where(o => o.Type == AssociatedAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AssociatedAssetId = jsonObservations;
                    }

                    // Add AssociatedAssetIdDataSet
                    typeObservations = observations.Where(o => o.Type == AssociatedAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AssociatedAssetIdDataSet = jsonObservations;
                    }

                    // Add AssociatedAssetIdTable
                    typeObservations = observations.Where(o => o.Type == AssociatedAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AssociatedAssetIdTable = jsonObservations;
                    }


                    // Add Availability
                    typeObservations = observations.Where(o => o.Type == AvailabilityDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Availability = jsonObservations;
                    }

                    // Add AvailabilityDataSet
                    typeObservations = observations.Where(o => o.Type == AvailabilityDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AvailabilityDataSet = jsonObservations;
                    }

                    // Add AvailabilityTable
                    typeObservations = observations.Where(o => o.Type == AvailabilityDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AvailabilityTable = jsonObservations;
                    }


                    // Add AxisCoupling
                    typeObservations = observations.Where(o => o.Type == AxisCouplingDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AxisCoupling = jsonObservations;
                    }

                    // Add AxisCouplingDataSet
                    typeObservations = observations.Where(o => o.Type == AxisCouplingDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AxisCouplingDataSet = jsonObservations;
                    }

                    // Add AxisCouplingTable
                    typeObservations = observations.Where(o => o.Type == AxisCouplingDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AxisCouplingTable = jsonObservations;
                    }


                    // Add AxisFeedrateOverride
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AxisFeedrateOverride = jsonObservations;
                    }

                    // Add AxisFeedrateOverrideDataSet
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AxisFeedrateOverrideDataSet = jsonObservations;
                    }

                    // Add AxisFeedrateOverrideTable
                    typeObservations = observations.Where(o => o.Type == AxisFeedrateOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AxisFeedrateOverrideTable = jsonObservations;
                    }


                    // Add AxisInterlock
                    typeObservations = observations.Where(o => o.Type == AxisInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AxisInterlock = jsonObservations;
                    }

                    // Add AxisInterlockDataSet
                    typeObservations = observations.Where(o => o.Type == AxisInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AxisInterlockDataSet = jsonObservations;
                    }

                    // Add AxisInterlockTable
                    typeObservations = observations.Where(o => o.Type == AxisInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AxisInterlockTable = jsonObservations;
                    }


                    // Add AxisState
                    typeObservations = observations.Where(o => o.Type == AxisStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        AxisState = jsonObservations;
                    }

                    // Add AxisStateDataSet
                    typeObservations = observations.Where(o => o.Type == AxisStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        AxisStateDataSet = jsonObservations;
                    }

                    // Add AxisStateTable
                    typeObservations = observations.Where(o => o.Type == AxisStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        AxisStateTable = jsonObservations;
                    }


                    // Add BatteryState
                    typeObservations = observations.Where(o => o.Type == BatteryStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        BatteryState = jsonObservations;
                    }

                    // Add BatteryStateDataSet
                    typeObservations = observations.Where(o => o.Type == BatteryStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        BatteryStateDataSet = jsonObservations;
                    }

                    // Add BatteryStateTable
                    typeObservations = observations.Where(o => o.Type == BatteryStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        BatteryStateTable = jsonObservations;
                    }


                    // Add BindingState
                    typeObservations = observations.Where(o => o.Type == BindingStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        BindingState = jsonObservations;
                    }

                    // Add BindingStateDataSet
                    typeObservations = observations.Where(o => o.Type == BindingStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        BindingStateDataSet = jsonObservations;
                    }

                    // Add BindingStateTable
                    typeObservations = observations.Where(o => o.Type == BindingStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        BindingStateTable = jsonObservations;
                    }


                    // Add Block
                    typeObservations = observations.Where(o => o.Type == BlockDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Block = jsonObservations;
                    }

                    // Add BlockDataSet
                    typeObservations = observations.Where(o => o.Type == BlockDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        BlockDataSet = jsonObservations;
                    }

                    // Add BlockTable
                    typeObservations = observations.Where(o => o.Type == BlockDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        BlockTable = jsonObservations;
                    }


                    // Add BlockCount
                    typeObservations = observations.Where(o => o.Type == BlockCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        BlockCount = jsonObservations;
                    }

                    // Add BlockCountDataSet
                    typeObservations = observations.Where(o => o.Type == BlockCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        BlockCountDataSet = jsonObservations;
                    }

                    // Add BlockCountTable
                    typeObservations = observations.Where(o => o.Type == BlockCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        BlockCountTable = jsonObservations;
                    }


                    // Add CharacteristicPersistentId
                    typeObservations = observations.Where(o => o.Type == CharacteristicPersistentIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        CharacteristicPersistentId = jsonObservations;
                    }

                    // Add CharacteristicPersistentIdDataSet
                    typeObservations = observations.Where(o => o.Type == CharacteristicPersistentIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        CharacteristicPersistentIdDataSet = jsonObservations;
                    }

                    // Add CharacteristicPersistentIdTable
                    typeObservations = observations.Where(o => o.Type == CharacteristicPersistentIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        CharacteristicPersistentIdTable = jsonObservations;
                    }


                    // Add CharacteristicStatus
                    typeObservations = observations.Where(o => o.Type == CharacteristicStatusDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        CharacteristicStatus = jsonObservations;
                    }

                    // Add CharacteristicStatusDataSet
                    typeObservations = observations.Where(o => o.Type == CharacteristicStatusDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        CharacteristicStatusDataSet = jsonObservations;
                    }

                    // Add CharacteristicStatusTable
                    typeObservations = observations.Where(o => o.Type == CharacteristicStatusDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        CharacteristicStatusTable = jsonObservations;
                    }


                    // Add ChuckInterlock
                    typeObservations = observations.Where(o => o.Type == ChuckInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ChuckInterlock = jsonObservations;
                    }

                    // Add ChuckInterlockDataSet
                    typeObservations = observations.Where(o => o.Type == ChuckInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ChuckInterlockDataSet = jsonObservations;
                    }

                    // Add ChuckInterlockTable
                    typeObservations = observations.Where(o => o.Type == ChuckInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ChuckInterlockTable = jsonObservations;
                    }


                    // Add ChuckState
                    typeObservations = observations.Where(o => o.Type == ChuckStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ChuckState = jsonObservations;
                    }

                    // Add ChuckStateDataSet
                    typeObservations = observations.Where(o => o.Type == ChuckStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ChuckStateDataSet = jsonObservations;
                    }

                    // Add ChuckStateTable
                    typeObservations = observations.Where(o => o.Type == ChuckStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ChuckStateTable = jsonObservations;
                    }


                    // Add ClockTime
                    typeObservations = observations.Where(o => o.Type == ClockTimeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ClockTime = jsonObservations;
                    }

                    // Add ClockTimeDataSet
                    typeObservations = observations.Where(o => o.Type == ClockTimeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ClockTimeDataSet = jsonObservations;
                    }

                    // Add ClockTimeTable
                    typeObservations = observations.Where(o => o.Type == ClockTimeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ClockTimeTable = jsonObservations;
                    }


                    // Add Code
                    typeObservations = observations.Where(o => o.Type == CodeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Code = jsonObservations;
                    }

                    // Add CodeDataSet
                    typeObservations = observations.Where(o => o.Type == CodeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        CodeDataSet = jsonObservations;
                    }

                    // Add CodeTable
                    typeObservations = observations.Where(o => o.Type == CodeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        CodeTable = jsonObservations;
                    }


                    // Add ComponentData
                    typeObservations = observations.Where(o => o.Type == ComponentDataDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ComponentData = jsonObservations;
                    }

                    // Add ComponentDataDataSet
                    typeObservations = observations.Where(o => o.Type == ComponentDataDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ComponentDataDataSet = jsonObservations;
                    }

                    // Add ComponentDataTable
                    typeObservations = observations.Where(o => o.Type == ComponentDataDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ComponentDataTable = jsonObservations;
                    }


                    // Add CompositionState
                    typeObservations = observations.Where(o => o.Type == CompositionStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        CompositionState = jsonObservations;
                    }

                    // Add CompositionStateDataSet
                    typeObservations = observations.Where(o => o.Type == CompositionStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        CompositionStateDataSet = jsonObservations;
                    }

                    // Add CompositionStateTable
                    typeObservations = observations.Where(o => o.Type == CompositionStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        CompositionStateTable = jsonObservations;
                    }


                    // Add ConnectionStatus
                    typeObservations = observations.Where(o => o.Type == ConnectionStatusDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ConnectionStatus = jsonObservations;
                    }

                    // Add ConnectionStatusDataSet
                    typeObservations = observations.Where(o => o.Type == ConnectionStatusDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ConnectionStatusDataSet = jsonObservations;
                    }

                    // Add ConnectionStatusTable
                    typeObservations = observations.Where(o => o.Type == ConnectionStatusDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ConnectionStatusTable = jsonObservations;
                    }


                    // Add ControlLimit
                    typeObservations = observations.Where(o => o.Type == ControlLimitDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ControlLimit = jsonObservations;
                    }

                    // Add ControlLimitDataSet
                    typeObservations = observations.Where(o => o.Type == ControlLimitDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ControlLimitDataSet = jsonObservations;
                    }

                    // Add ControlLimitTable
                    typeObservations = observations.Where(o => o.Type == ControlLimitDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ControlLimitTable = jsonObservations;
                    }


                    // Add ControlLimits
                    typeObservations = observations.Where(o => o.Type == ControlLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ControlLimits = jsonObservations;
                    }

                    // Add ControlLimitsDataSet
                    typeObservations = observations.Where(o => o.Type == ControlLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ControlLimitsDataSet = jsonObservations;
                    }

                    // Add ControlLimitsTable
                    typeObservations = observations.Where(o => o.Type == ControlLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ControlLimitsTable = jsonObservations;
                    }


                    // Add ControllerMode
                    typeObservations = observations.Where(o => o.Type == ControllerModeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ControllerMode = jsonObservations;
                    }

                    // Add ControllerModeDataSet
                    typeObservations = observations.Where(o => o.Type == ControllerModeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ControllerModeDataSet = jsonObservations;
                    }

                    // Add ControllerModeTable
                    typeObservations = observations.Where(o => o.Type == ControllerModeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ControllerModeTable = jsonObservations;
                    }


                    // Add ControllerModeOverride
                    typeObservations = observations.Where(o => o.Type == ControllerModeOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ControllerModeOverride = jsonObservations;
                    }

                    // Add ControllerModeOverrideDataSet
                    typeObservations = observations.Where(o => o.Type == ControllerModeOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ControllerModeOverrideDataSet = jsonObservations;
                    }

                    // Add ControllerModeOverrideTable
                    typeObservations = observations.Where(o => o.Type == ControllerModeOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ControllerModeOverrideTable = jsonObservations;
                    }


                    // Add CoupledAxes
                    typeObservations = observations.Where(o => o.Type == CoupledAxesDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        CoupledAxes = jsonObservations;
                    }

                    // Add CoupledAxesDataSet
                    typeObservations = observations.Where(o => o.Type == CoupledAxesDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        CoupledAxesDataSet = jsonObservations;
                    }

                    // Add CoupledAxesTable
                    typeObservations = observations.Where(o => o.Type == CoupledAxesDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        CoupledAxesTable = jsonObservations;
                    }


                    // Add CycleCount
                    typeObservations = observations.Where(o => o.Type == CycleCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        CycleCount = jsonObservations;
                    }

                    // Add CycleCountDataSet
                    typeObservations = observations.Where(o => o.Type == CycleCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        CycleCountDataSet = jsonObservations;
                    }

                    // Add CycleCountTable
                    typeObservations = observations.Where(o => o.Type == CycleCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        CycleCountTable = jsonObservations;
                    }


                    // Add DateCode
                    typeObservations = observations.Where(o => o.Type == DateCodeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        DateCode = jsonObservations;
                    }

                    // Add DateCodeDataSet
                    typeObservations = observations.Where(o => o.Type == DateCodeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DateCodeDataSet = jsonObservations;
                    }

                    // Add DateCodeTable
                    typeObservations = observations.Where(o => o.Type == DateCodeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DateCodeTable = jsonObservations;
                    }


                    // Add DeactivationCount
                    typeObservations = observations.Where(o => o.Type == DeactivationCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        DeactivationCount = jsonObservations;
                    }

                    // Add DeactivationCountDataSet
                    typeObservations = observations.Where(o => o.Type == DeactivationCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DeactivationCountDataSet = jsonObservations;
                    }

                    // Add DeactivationCountTable
                    typeObservations = observations.Where(o => o.Type == DeactivationCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DeactivationCountTable = jsonObservations;
                    }


                    // Add Depth
                    typeObservations = observations.Where(o => o.Type == DepthDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Depth = jsonObservations;
                    }

                    // Add DepthDataSet
                    typeObservations = observations.Where(o => o.Type == DepthDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DepthDataSet = jsonObservations;
                    }

                    // Add DepthTable
                    typeObservations = observations.Where(o => o.Type == DepthDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DepthTable = jsonObservations;
                    }


                    // Add DeviceAdded
                    typeObservations = observations.Where(o => o.Type == DeviceAddedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        DeviceAdded = jsonObservations;
                    }

                    // Add DeviceAddedDataSet
                    typeObservations = observations.Where(o => o.Type == DeviceAddedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DeviceAddedDataSet = jsonObservations;
                    }

                    // Add DeviceAddedTable
                    typeObservations = observations.Where(o => o.Type == DeviceAddedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DeviceAddedTable = jsonObservations;
                    }


                    // Add DeviceChanged
                    typeObservations = observations.Where(o => o.Type == DeviceChangedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        DeviceChanged = jsonObservations;
                    }

                    // Add DeviceChangedDataSet
                    typeObservations = observations.Where(o => o.Type == DeviceChangedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DeviceChangedDataSet = jsonObservations;
                    }

                    // Add DeviceChangedTable
                    typeObservations = observations.Where(o => o.Type == DeviceChangedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DeviceChangedTable = jsonObservations;
                    }


                    // Add DeviceRemoved
                    typeObservations = observations.Where(o => o.Type == DeviceRemovedDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        DeviceRemoved = jsonObservations;
                    }

                    // Add DeviceRemovedDataSet
                    typeObservations = observations.Where(o => o.Type == DeviceRemovedDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DeviceRemovedDataSet = jsonObservations;
                    }

                    // Add DeviceRemovedTable
                    typeObservations = observations.Where(o => o.Type == DeviceRemovedDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DeviceRemovedTable = jsonObservations;
                    }


                    // Add DeviceUuid
                    typeObservations = observations.Where(o => o.Type == DeviceUuidDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        DeviceUuid = jsonObservations;
                    }

                    // Add DeviceUuidDataSet
                    typeObservations = observations.Where(o => o.Type == DeviceUuidDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DeviceUuidDataSet = jsonObservations;
                    }

                    // Add DeviceUuidTable
                    typeObservations = observations.Where(o => o.Type == DeviceUuidDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DeviceUuidTable = jsonObservations;
                    }


                    // Add Direction
                    typeObservations = observations.Where(o => o.Type == DirectionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Direction = jsonObservations;
                    }

                    // Add DirectionDataSet
                    typeObservations = observations.Where(o => o.Type == DirectionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DirectionDataSet = jsonObservations;
                    }

                    // Add DirectionTable
                    typeObservations = observations.Where(o => o.Type == DirectionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DirectionTable = jsonObservations;
                    }


                    // Add DoorState
                    typeObservations = observations.Where(o => o.Type == DoorStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        DoorState = jsonObservations;
                    }

                    // Add DoorStateDataSet
                    typeObservations = observations.Where(o => o.Type == DoorStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        DoorStateDataSet = jsonObservations;
                    }

                    // Add DoorStateTable
                    typeObservations = observations.Where(o => o.Type == DoorStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        DoorStateTable = jsonObservations;
                    }


                    // Add EmergencyStop
                    typeObservations = observations.Where(o => o.Type == EmergencyStopDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        EmergencyStop = jsonObservations;
                    }

                    // Add EmergencyStopDataSet
                    typeObservations = observations.Where(o => o.Type == EmergencyStopDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        EmergencyStopDataSet = jsonObservations;
                    }

                    // Add EmergencyStopTable
                    typeObservations = observations.Where(o => o.Type == EmergencyStopDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        EmergencyStopTable = jsonObservations;
                    }


                    // Add EndOfBar
                    typeObservations = observations.Where(o => o.Type == EndOfBarDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        EndOfBar = jsonObservations;
                    }

                    // Add EndOfBarDataSet
                    typeObservations = observations.Where(o => o.Type == EndOfBarDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        EndOfBarDataSet = jsonObservations;
                    }

                    // Add EndOfBarTable
                    typeObservations = observations.Where(o => o.Type == EndOfBarDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        EndOfBarTable = jsonObservations;
                    }


                    // Add EquipmentMode
                    typeObservations = observations.Where(o => o.Type == EquipmentModeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        EquipmentMode = jsonObservations;
                    }

                    // Add EquipmentModeDataSet
                    typeObservations = observations.Where(o => o.Type == EquipmentModeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        EquipmentModeDataSet = jsonObservations;
                    }

                    // Add EquipmentModeTable
                    typeObservations = observations.Where(o => o.Type == EquipmentModeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        EquipmentModeTable = jsonObservations;
                    }


                    // Add Execution
                    typeObservations = observations.Where(o => o.Type == ExecutionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Execution = jsonObservations;
                    }

                    // Add ExecutionDataSet
                    typeObservations = observations.Where(o => o.Type == ExecutionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ExecutionDataSet = jsonObservations;
                    }

                    // Add ExecutionTable
                    typeObservations = observations.Where(o => o.Type == ExecutionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ExecutionTable = jsonObservations;
                    }


                    // Add FeatureMeasurement
                    typeObservations = observations.Where(o => o.Type == FeatureMeasurementDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        FeatureMeasurement = jsonObservations;
                    }

                    // Add FeatureMeasurementDataSet
                    typeObservations = observations.Where(o => o.Type == FeatureMeasurementDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        FeatureMeasurementDataSet = jsonObservations;
                    }

                    // Add FeatureMeasurementTable
                    typeObservations = observations.Where(o => o.Type == FeatureMeasurementDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        FeatureMeasurementTable = jsonObservations;
                    }


                    // Add Firmware
                    typeObservations = observations.Where(o => o.Type == FirmwareDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Firmware = jsonObservations;
                    }

                    // Add FirmwareDataSet
                    typeObservations = observations.Where(o => o.Type == FirmwareDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        FirmwareDataSet = jsonObservations;
                    }

                    // Add FirmwareTable
                    typeObservations = observations.Where(o => o.Type == FirmwareDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        FirmwareTable = jsonObservations;
                    }


                    // Add FixtureAssetId
                    typeObservations = observations.Where(o => o.Type == FixtureAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        FixtureAssetId = jsonObservations;
                    }

                    // Add FixtureAssetIdDataSet
                    typeObservations = observations.Where(o => o.Type == FixtureAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        FixtureAssetIdDataSet = jsonObservations;
                    }

                    // Add FixtureAssetIdTable
                    typeObservations = observations.Where(o => o.Type == FixtureAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        FixtureAssetIdTable = jsonObservations;
                    }


                    // Add FixtureId
                    typeObservations = observations.Where(o => o.Type == FixtureIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        FixtureId = jsonObservations;
                    }

                    // Add FixtureIdDataSet
                    typeObservations = observations.Where(o => o.Type == FixtureIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        FixtureIdDataSet = jsonObservations;
                    }

                    // Add FixtureIdTable
                    typeObservations = observations.Where(o => o.Type == FixtureIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        FixtureIdTable = jsonObservations;
                    }


                    // Add FunctionalMode
                    typeObservations = observations.Where(o => o.Type == FunctionalModeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        FunctionalMode = jsonObservations;
                    }

                    // Add FunctionalModeDataSet
                    typeObservations = observations.Where(o => o.Type == FunctionalModeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        FunctionalModeDataSet = jsonObservations;
                    }

                    // Add FunctionalModeTable
                    typeObservations = observations.Where(o => o.Type == FunctionalModeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        FunctionalModeTable = jsonObservations;
                    }


                    // Add Hardness
                    typeObservations = observations.Where(o => o.Type == HardnessDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Hardness = jsonObservations;
                    }

                    // Add HardnessDataSet
                    typeObservations = observations.Where(o => o.Type == HardnessDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        HardnessDataSet = jsonObservations;
                    }

                    // Add HardnessTable
                    typeObservations = observations.Where(o => o.Type == HardnessDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        HardnessTable = jsonObservations;
                    }


                    // Add Hardware
                    typeObservations = observations.Where(o => o.Type == HardwareDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Hardware = jsonObservations;
                    }

                    // Add HardwareDataSet
                    typeObservations = observations.Where(o => o.Type == HardwareDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        HardwareDataSet = jsonObservations;
                    }

                    // Add HardwareTable
                    typeObservations = observations.Where(o => o.Type == HardwareDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        HardwareTable = jsonObservations;
                    }


                    // Add HostName
                    typeObservations = observations.Where(o => o.Type == HostNameDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        HostName = jsonObservations;
                    }

                    // Add HostNameDataSet
                    typeObservations = observations.Where(o => o.Type == HostNameDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        HostNameDataSet = jsonObservations;
                    }

                    // Add HostNameTable
                    typeObservations = observations.Where(o => o.Type == HostNameDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        HostNameTable = jsonObservations;
                    }


                    // Add LeakDetect
                    typeObservations = observations.Where(o => o.Type == LeakDetectDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LeakDetect = jsonObservations;
                    }

                    // Add LeakDetectDataSet
                    typeObservations = observations.Where(o => o.Type == LeakDetectDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LeakDetectDataSet = jsonObservations;
                    }

                    // Add LeakDetectTable
                    typeObservations = observations.Where(o => o.Type == LeakDetectDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LeakDetectTable = jsonObservations;
                    }


                    // Add Library
                    typeObservations = observations.Where(o => o.Type == LibraryDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Library = jsonObservations;
                    }

                    // Add LibraryDataSet
                    typeObservations = observations.Where(o => o.Type == LibraryDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LibraryDataSet = jsonObservations;
                    }

                    // Add LibraryTable
                    typeObservations = observations.Where(o => o.Type == LibraryDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LibraryTable = jsonObservations;
                    }


                    // Add Line
                    typeObservations = observations.Where(o => o.Type == LineDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Line = jsonObservations;
                    }

                    // Add LineDataSet
                    typeObservations = observations.Where(o => o.Type == LineDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LineDataSet = jsonObservations;
                    }

                    // Add LineTable
                    typeObservations = observations.Where(o => o.Type == LineDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LineTable = jsonObservations;
                    }


                    // Add LineLabel
                    typeObservations = observations.Where(o => o.Type == LineLabelDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LineLabel = jsonObservations;
                    }

                    // Add LineLabelDataSet
                    typeObservations = observations.Where(o => o.Type == LineLabelDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LineLabelDataSet = jsonObservations;
                    }

                    // Add LineLabelTable
                    typeObservations = observations.Where(o => o.Type == LineLabelDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LineLabelTable = jsonObservations;
                    }


                    // Add LineNumber
                    typeObservations = observations.Where(o => o.Type == LineNumberDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LineNumber = jsonObservations;
                    }

                    // Add LineNumberDataSet
                    typeObservations = observations.Where(o => o.Type == LineNumberDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LineNumberDataSet = jsonObservations;
                    }

                    // Add LineNumberTable
                    typeObservations = observations.Where(o => o.Type == LineNumberDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LineNumberTable = jsonObservations;
                    }


                    // Add LoadCount
                    typeObservations = observations.Where(o => o.Type == LoadCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LoadCount = jsonObservations;
                    }

                    // Add LoadCountDataSet
                    typeObservations = observations.Where(o => o.Type == LoadCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LoadCountDataSet = jsonObservations;
                    }

                    // Add LoadCountTable
                    typeObservations = observations.Where(o => o.Type == LoadCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LoadCountTable = jsonObservations;
                    }


                    // Add LocationAddress
                    typeObservations = observations.Where(o => o.Type == LocationAddressDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LocationAddress = jsonObservations;
                    }

                    // Add LocationAddressDataSet
                    typeObservations = observations.Where(o => o.Type == LocationAddressDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LocationAddressDataSet = jsonObservations;
                    }

                    // Add LocationAddressTable
                    typeObservations = observations.Where(o => o.Type == LocationAddressDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LocationAddressTable = jsonObservations;
                    }


                    // Add LocationNarrative
                    typeObservations = observations.Where(o => o.Type == LocationNarrativeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LocationNarrative = jsonObservations;
                    }

                    // Add LocationNarrativeDataSet
                    typeObservations = observations.Where(o => o.Type == LocationNarrativeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LocationNarrativeDataSet = jsonObservations;
                    }

                    // Add LocationNarrativeTable
                    typeObservations = observations.Where(o => o.Type == LocationNarrativeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LocationNarrativeTable = jsonObservations;
                    }


                    // Add LocationSpatialGeographic
                    typeObservations = observations.Where(o => o.Type == LocationSpatialGeographicDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LocationSpatialGeographic = jsonObservations;
                    }

                    // Add LocationSpatialGeographicDataSet
                    typeObservations = observations.Where(o => o.Type == LocationSpatialGeographicDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LocationSpatialGeographicDataSet = jsonObservations;
                    }

                    // Add LocationSpatialGeographicTable
                    typeObservations = observations.Where(o => o.Type == LocationSpatialGeographicDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LocationSpatialGeographicTable = jsonObservations;
                    }


                    // Add LockState
                    typeObservations = observations.Where(o => o.Type == LockStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        LockState = jsonObservations;
                    }

                    // Add LockStateDataSet
                    typeObservations = observations.Where(o => o.Type == LockStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        LockStateDataSet = jsonObservations;
                    }

                    // Add LockStateTable
                    typeObservations = observations.Where(o => o.Type == LockStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        LockStateTable = jsonObservations;
                    }


                    // Add MaintenanceList
                    typeObservations = observations.Where(o => o.Type == MaintenanceListDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        MaintenanceList = jsonObservations;
                    }

                    // Add MaintenanceListDataSet
                    typeObservations = observations.Where(o => o.Type == MaintenanceListDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MaintenanceListDataSet = jsonObservations;
                    }

                    // Add MaintenanceListTable
                    typeObservations = observations.Where(o => o.Type == MaintenanceListDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MaintenanceListTable = jsonObservations;
                    }


                    // Add Material
                    typeObservations = observations.Where(o => o.Type == MaterialDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Material = jsonObservations;
                    }

                    // Add MaterialDataSet
                    typeObservations = observations.Where(o => o.Type == MaterialDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MaterialDataSet = jsonObservations;
                    }

                    // Add MaterialTable
                    typeObservations = observations.Where(o => o.Type == MaterialDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MaterialTable = jsonObservations;
                    }


                    // Add MaterialLayer
                    typeObservations = observations.Where(o => o.Type == MaterialLayerDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        MaterialLayer = jsonObservations;
                    }

                    // Add MaterialLayerDataSet
                    typeObservations = observations.Where(o => o.Type == MaterialLayerDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MaterialLayerDataSet = jsonObservations;
                    }

                    // Add MaterialLayerTable
                    typeObservations = observations.Where(o => o.Type == MaterialLayerDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MaterialLayerTable = jsonObservations;
                    }


                    // Add MeasurementType
                    typeObservations = observations.Where(o => o.Type == MeasurementTypeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        MeasurementType = jsonObservations;
                    }

                    // Add MeasurementTypeDataSet
                    typeObservations = observations.Where(o => o.Type == MeasurementTypeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MeasurementTypeDataSet = jsonObservations;
                    }

                    // Add MeasurementTypeTable
                    typeObservations = observations.Where(o => o.Type == MeasurementTypeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MeasurementTypeTable = jsonObservations;
                    }


                    // Add MeasurementUnits
                    typeObservations = observations.Where(o => o.Type == MeasurementUnitsDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        MeasurementUnits = jsonObservations;
                    }

                    // Add MeasurementUnitsDataSet
                    typeObservations = observations.Where(o => o.Type == MeasurementUnitsDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MeasurementUnitsDataSet = jsonObservations;
                    }

                    // Add MeasurementUnitsTable
                    typeObservations = observations.Where(o => o.Type == MeasurementUnitsDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MeasurementUnitsTable = jsonObservations;
                    }


                    // Add MeasurementValue
                    typeObservations = observations.Where(o => o.Type == MeasurementValueDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        MeasurementValue = jsonObservations;
                    }

                    // Add MeasurementValueDataSet
                    typeObservations = observations.Where(o => o.Type == MeasurementValueDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MeasurementValueDataSet = jsonObservations;
                    }

                    // Add MeasurementValueTable
                    typeObservations = observations.Where(o => o.Type == MeasurementValueDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MeasurementValueTable = jsonObservations;
                    }


                    // Add Message
                    typeObservations = observations.Where(o => o.Type == MessageDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Message = jsonObservations;
                    }

                    // Add MessageDataSet
                    typeObservations = observations.Where(o => o.Type == MessageDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MessageDataSet = jsonObservations;
                    }

                    // Add MessageTable
                    typeObservations = observations.Where(o => o.Type == MessageDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MessageTable = jsonObservations;
                    }


                    // Add MTConnectVersion
                    typeObservations = observations.Where(o => o.Type == MTConnectVersionDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        MTConnectVersion = jsonObservations;
                    }

                    // Add MTConnectVersionDataSet
                    typeObservations = observations.Where(o => o.Type == MTConnectVersionDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        MTConnectVersionDataSet = jsonObservations;
                    }

                    // Add MTConnectVersionTable
                    typeObservations = observations.Where(o => o.Type == MTConnectVersionDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        MTConnectVersionTable = jsonObservations;
                    }


                    // Add Network
                    typeObservations = observations.Where(o => o.Type == NetworkDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Network = jsonObservations;
                    }

                    // Add NetworkDataSet
                    typeObservations = observations.Where(o => o.Type == NetworkDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        NetworkDataSet = jsonObservations;
                    }

                    // Add NetworkTable
                    typeObservations = observations.Where(o => o.Type == NetworkDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        NetworkTable = jsonObservations;
                    }


                    // Add NetworkPort
                    typeObservations = observations.Where(o => o.Type == NetworkPortDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        NetworkPort = jsonObservations;
                    }

                    // Add NetworkPortDataSet
                    typeObservations = observations.Where(o => o.Type == NetworkPortDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        NetworkPortDataSet = jsonObservations;
                    }

                    // Add NetworkPortTable
                    typeObservations = observations.Where(o => o.Type == NetworkPortDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        NetworkPortTable = jsonObservations;
                    }


                    // Add OperatingMode
                    typeObservations = observations.Where(o => o.Type == OperatingModeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        OperatingMode = jsonObservations;
                    }

                    // Add OperatingModeDataSet
                    typeObservations = observations.Where(o => o.Type == OperatingModeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        OperatingModeDataSet = jsonObservations;
                    }

                    // Add OperatingModeTable
                    typeObservations = observations.Where(o => o.Type == OperatingModeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        OperatingModeTable = jsonObservations;
                    }


                    // Add OperatingSystem
                    typeObservations = observations.Where(o => o.Type == OperatingSystemDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        OperatingSystem = jsonObservations;
                    }

                    // Add OperatingSystemDataSet
                    typeObservations = observations.Where(o => o.Type == OperatingSystemDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        OperatingSystemDataSet = jsonObservations;
                    }

                    // Add OperatingSystemTable
                    typeObservations = observations.Where(o => o.Type == OperatingSystemDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        OperatingSystemTable = jsonObservations;
                    }


                    // Add OperatorId
                    typeObservations = observations.Where(o => o.Type == OperatorIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        OperatorId = jsonObservations;
                    }

                    // Add OperatorIdDataSet
                    typeObservations = observations.Where(o => o.Type == OperatorIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        OperatorIdDataSet = jsonObservations;
                    }

                    // Add OperatorIdTable
                    typeObservations = observations.Where(o => o.Type == OperatorIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        OperatorIdTable = jsonObservations;
                    }


                    // Add PalletId
                    typeObservations = observations.Where(o => o.Type == PalletIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PalletId = jsonObservations;
                    }

                    // Add PalletIdDataSet
                    typeObservations = observations.Where(o => o.Type == PalletIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PalletIdDataSet = jsonObservations;
                    }

                    // Add PalletIdTable
                    typeObservations = observations.Where(o => o.Type == PalletIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PalletIdTable = jsonObservations;
                    }


                    // Add PartCount
                    typeObservations = observations.Where(o => o.Type == PartCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartCount = jsonObservations;
                    }

                    // Add PartCountDataSet
                    typeObservations = observations.Where(o => o.Type == PartCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartCountDataSet = jsonObservations;
                    }

                    // Add PartCountTable
                    typeObservations = observations.Where(o => o.Type == PartCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartCountTable = jsonObservations;
                    }


                    // Add PartCountType
                    typeObservations = observations.Where(o => o.Type == PartCountTypeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartCountType = jsonObservations;
                    }

                    // Add PartCountTypeDataSet
                    typeObservations = observations.Where(o => o.Type == PartCountTypeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartCountTypeDataSet = jsonObservations;
                    }

                    // Add PartCountTypeTable
                    typeObservations = observations.Where(o => o.Type == PartCountTypeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartCountTypeTable = jsonObservations;
                    }


                    // Add PartDetect
                    typeObservations = observations.Where(o => o.Type == PartDetectDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartDetect = jsonObservations;
                    }

                    // Add PartDetectDataSet
                    typeObservations = observations.Where(o => o.Type == PartDetectDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartDetectDataSet = jsonObservations;
                    }

                    // Add PartDetectTable
                    typeObservations = observations.Where(o => o.Type == PartDetectDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartDetectTable = jsonObservations;
                    }


                    // Add PartGroupId
                    typeObservations = observations.Where(o => o.Type == PartGroupIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartGroupId = jsonObservations;
                    }

                    // Add PartGroupIdDataSet
                    typeObservations = observations.Where(o => o.Type == PartGroupIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartGroupIdDataSet = jsonObservations;
                    }

                    // Add PartGroupIdTable
                    typeObservations = observations.Where(o => o.Type == PartGroupIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartGroupIdTable = jsonObservations;
                    }


                    // Add PartId
                    typeObservations = observations.Where(o => o.Type == PartIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartId = jsonObservations;
                    }

                    // Add PartIdDataSet
                    typeObservations = observations.Where(o => o.Type == PartIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartIdDataSet = jsonObservations;
                    }

                    // Add PartIdTable
                    typeObservations = observations.Where(o => o.Type == PartIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartIdTable = jsonObservations;
                    }


                    // Add PartIndex
                    typeObservations = observations.Where(o => o.Type == PartIndexDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartIndex = jsonObservations;
                    }

                    // Add PartIndexDataSet
                    typeObservations = observations.Where(o => o.Type == PartIndexDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartIndexDataSet = jsonObservations;
                    }

                    // Add PartIndexTable
                    typeObservations = observations.Where(o => o.Type == PartIndexDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartIndexTable = jsonObservations;
                    }


                    // Add PartKindId
                    typeObservations = observations.Where(o => o.Type == PartKindIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartKindId = jsonObservations;
                    }

                    // Add PartKindIdDataSet
                    typeObservations = observations.Where(o => o.Type == PartKindIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartKindIdDataSet = jsonObservations;
                    }

                    // Add PartKindIdTable
                    typeObservations = observations.Where(o => o.Type == PartKindIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartKindIdTable = jsonObservations;
                    }


                    // Add PartNumber
                    typeObservations = observations.Where(o => o.Type == PartNumberDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartNumber = jsonObservations;
                    }

                    // Add PartNumberDataSet
                    typeObservations = observations.Where(o => o.Type == PartNumberDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartNumberDataSet = jsonObservations;
                    }

                    // Add PartNumberTable
                    typeObservations = observations.Where(o => o.Type == PartNumberDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartNumberTable = jsonObservations;
                    }


                    // Add PartProcessingState
                    typeObservations = observations.Where(o => o.Type == PartProcessingStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartProcessingState = jsonObservations;
                    }

                    // Add PartProcessingStateDataSet
                    typeObservations = observations.Where(o => o.Type == PartProcessingStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartProcessingStateDataSet = jsonObservations;
                    }

                    // Add PartProcessingStateTable
                    typeObservations = observations.Where(o => o.Type == PartProcessingStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartProcessingStateTable = jsonObservations;
                    }


                    // Add PartStatus
                    typeObservations = observations.Where(o => o.Type == PartStatusDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartStatus = jsonObservations;
                    }

                    // Add PartStatusDataSet
                    typeObservations = observations.Where(o => o.Type == PartStatusDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartStatusDataSet = jsonObservations;
                    }

                    // Add PartStatusTable
                    typeObservations = observations.Where(o => o.Type == PartStatusDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartStatusTable = jsonObservations;
                    }


                    // Add PartUniqueId
                    typeObservations = observations.Where(o => o.Type == PartUniqueIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PartUniqueId = jsonObservations;
                    }

                    // Add PartUniqueIdDataSet
                    typeObservations = observations.Where(o => o.Type == PartUniqueIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PartUniqueIdDataSet = jsonObservations;
                    }

                    // Add PartUniqueIdTable
                    typeObservations = observations.Where(o => o.Type == PartUniqueIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PartUniqueIdTable = jsonObservations;
                    }


                    // Add PathFeedrateOverride
                    typeObservations = observations.Where(o => o.Type == PathFeedrateOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PathFeedrateOverride = jsonObservations;
                    }

                    // Add PathFeedrateOverrideDataSet
                    typeObservations = observations.Where(o => o.Type == PathFeedrateOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PathFeedrateOverrideDataSet = jsonObservations;
                    }

                    // Add PathFeedrateOverrideTable
                    typeObservations = observations.Where(o => o.Type == PathFeedrateOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PathFeedrateOverrideTable = jsonObservations;
                    }


                    // Add PathMode
                    typeObservations = observations.Where(o => o.Type == PathModeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PathMode = jsonObservations;
                    }

                    // Add PathModeDataSet
                    typeObservations = observations.Where(o => o.Type == PathModeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PathModeDataSet = jsonObservations;
                    }

                    // Add PathModeTable
                    typeObservations = observations.Where(o => o.Type == PathModeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PathModeTable = jsonObservations;
                    }


                    // Add PowerState
                    typeObservations = observations.Where(o => o.Type == PowerStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PowerState = jsonObservations;
                    }

                    // Add PowerStateDataSet
                    typeObservations = observations.Where(o => o.Type == PowerStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PowerStateDataSet = jsonObservations;
                    }

                    // Add PowerStateTable
                    typeObservations = observations.Where(o => o.Type == PowerStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PowerStateTable = jsonObservations;
                    }


                    // Add PowerStatus
                    typeObservations = observations.Where(o => o.Type == PowerStatusDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        PowerStatus = jsonObservations;
                    }

                    // Add PowerStatusDataSet
                    typeObservations = observations.Where(o => o.Type == PowerStatusDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        PowerStatusDataSet = jsonObservations;
                    }

                    // Add PowerStatusTable
                    typeObservations = observations.Where(o => o.Type == PowerStatusDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        PowerStatusTable = jsonObservations;
                    }


                    // Add ProcessAggregateId
                    typeObservations = observations.Where(o => o.Type == ProcessAggregateIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProcessAggregateId = jsonObservations;
                    }

                    // Add ProcessAggregateIdDataSet
                    typeObservations = observations.Where(o => o.Type == ProcessAggregateIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProcessAggregateIdDataSet = jsonObservations;
                    }

                    // Add ProcessAggregateIdTable
                    typeObservations = observations.Where(o => o.Type == ProcessAggregateIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProcessAggregateIdTable = jsonObservations;
                    }


                    // Add ProcessKindId
                    typeObservations = observations.Where(o => o.Type == ProcessKindIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProcessKindId = jsonObservations;
                    }

                    // Add ProcessKindIdDataSet
                    typeObservations = observations.Where(o => o.Type == ProcessKindIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProcessKindIdDataSet = jsonObservations;
                    }

                    // Add ProcessKindIdTable
                    typeObservations = observations.Where(o => o.Type == ProcessKindIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProcessKindIdTable = jsonObservations;
                    }


                    // Add ProcessOccurrenceId
                    typeObservations = observations.Where(o => o.Type == ProcessOccurrenceIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProcessOccurrenceId = jsonObservations;
                    }

                    // Add ProcessOccurrenceIdDataSet
                    typeObservations = observations.Where(o => o.Type == ProcessOccurrenceIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProcessOccurrenceIdDataSet = jsonObservations;
                    }

                    // Add ProcessOccurrenceIdTable
                    typeObservations = observations.Where(o => o.Type == ProcessOccurrenceIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProcessOccurrenceIdTable = jsonObservations;
                    }


                    // Add ProcessState
                    typeObservations = observations.Where(o => o.Type == ProcessStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProcessState = jsonObservations;
                    }

                    // Add ProcessStateDataSet
                    typeObservations = observations.Where(o => o.Type == ProcessStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProcessStateDataSet = jsonObservations;
                    }

                    // Add ProcessStateTable
                    typeObservations = observations.Where(o => o.Type == ProcessStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProcessStateTable = jsonObservations;
                    }


                    // Add ProcessTime
                    typeObservations = observations.Where(o => o.Type == ProcessTimeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProcessTime = jsonObservations;
                    }

                    // Add ProcessTimeDataSet
                    typeObservations = observations.Where(o => o.Type == ProcessTimeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProcessTimeDataSet = jsonObservations;
                    }

                    // Add ProcessTimeTable
                    typeObservations = observations.Where(o => o.Type == ProcessTimeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProcessTimeTable = jsonObservations;
                    }


                    // Add Program
                    typeObservations = observations.Where(o => o.Type == ProgramDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Program = jsonObservations;
                    }

                    // Add ProgramDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramDataSet = jsonObservations;
                    }

                    // Add ProgramTable
                    typeObservations = observations.Where(o => o.Type == ProgramDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramTable = jsonObservations;
                    }


                    // Add ProgramComment
                    typeObservations = observations.Where(o => o.Type == ProgramCommentDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProgramComment = jsonObservations;
                    }

                    // Add ProgramCommentDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramCommentDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramCommentDataSet = jsonObservations;
                    }

                    // Add ProgramCommentTable
                    typeObservations = observations.Where(o => o.Type == ProgramCommentDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramCommentTable = jsonObservations;
                    }


                    // Add ProgramEdit
                    typeObservations = observations.Where(o => o.Type == ProgramEditDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProgramEdit = jsonObservations;
                    }

                    // Add ProgramEditDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramEditDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramEditDataSet = jsonObservations;
                    }

                    // Add ProgramEditTable
                    typeObservations = observations.Where(o => o.Type == ProgramEditDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramEditTable = jsonObservations;
                    }


                    // Add ProgramEditName
                    typeObservations = observations.Where(o => o.Type == ProgramEditNameDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProgramEditName = jsonObservations;
                    }

                    // Add ProgramEditNameDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramEditNameDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramEditNameDataSet = jsonObservations;
                    }

                    // Add ProgramEditNameTable
                    typeObservations = observations.Where(o => o.Type == ProgramEditNameDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramEditNameTable = jsonObservations;
                    }


                    // Add ProgramHeader
                    typeObservations = observations.Where(o => o.Type == ProgramHeaderDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProgramHeader = jsonObservations;
                    }

                    // Add ProgramHeaderDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramHeaderDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramHeaderDataSet = jsonObservations;
                    }

                    // Add ProgramHeaderTable
                    typeObservations = observations.Where(o => o.Type == ProgramHeaderDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramHeaderTable = jsonObservations;
                    }


                    // Add ProgramLocation
                    typeObservations = observations.Where(o => o.Type == ProgramLocationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProgramLocation = jsonObservations;
                    }

                    // Add ProgramLocationDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramLocationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramLocationDataSet = jsonObservations;
                    }

                    // Add ProgramLocationTable
                    typeObservations = observations.Where(o => o.Type == ProgramLocationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramLocationTable = jsonObservations;
                    }


                    // Add ProgramLocationType
                    typeObservations = observations.Where(o => o.Type == ProgramLocationTypeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProgramLocationType = jsonObservations;
                    }

                    // Add ProgramLocationTypeDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramLocationTypeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramLocationTypeDataSet = jsonObservations;
                    }

                    // Add ProgramLocationTypeTable
                    typeObservations = observations.Where(o => o.Type == ProgramLocationTypeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramLocationTypeTable = jsonObservations;
                    }


                    // Add ProgramNestLevel
                    typeObservations = observations.Where(o => o.Type == ProgramNestLevelDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ProgramNestLevel = jsonObservations;
                    }

                    // Add ProgramNestLevelDataSet
                    typeObservations = observations.Where(o => o.Type == ProgramNestLevelDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ProgramNestLevelDataSet = jsonObservations;
                    }

                    // Add ProgramNestLevelTable
                    typeObservations = observations.Where(o => o.Type == ProgramNestLevelDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ProgramNestLevelTable = jsonObservations;
                    }


                    // Add RotaryMode
                    typeObservations = observations.Where(o => o.Type == RotaryModeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        RotaryMode = jsonObservations;
                    }

                    // Add RotaryModeDataSet
                    typeObservations = observations.Where(o => o.Type == RotaryModeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        RotaryModeDataSet = jsonObservations;
                    }

                    // Add RotaryModeTable
                    typeObservations = observations.Where(o => o.Type == RotaryModeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        RotaryModeTable = jsonObservations;
                    }


                    // Add RotaryVelocityOverride
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        RotaryVelocityOverride = jsonObservations;
                    }

                    // Add RotaryVelocityOverrideDataSet
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        RotaryVelocityOverrideDataSet = jsonObservations;
                    }

                    // Add RotaryVelocityOverrideTable
                    typeObservations = observations.Where(o => o.Type == RotaryVelocityOverrideDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        RotaryVelocityOverrideTable = jsonObservations;
                    }


                    // Add Rotation
                    typeObservations = observations.Where(o => o.Type == RotationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Rotation = jsonObservations;
                    }

                    // Add RotationDataSet
                    typeObservations = observations.Where(o => o.Type == RotationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        RotationDataSet = jsonObservations;
                    }

                    // Add RotationTable
                    typeObservations = observations.Where(o => o.Type == RotationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        RotationTable = jsonObservations;
                    }


                    // Add SensorAttachment
                    typeObservations = observations.Where(o => o.Type == SensorAttachmentDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SensorAttachment = jsonObservations;
                    }

                    // Add SensorAttachmentDataSet
                    typeObservations = observations.Where(o => o.Type == SensorAttachmentDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SensorAttachmentDataSet = jsonObservations;
                    }

                    // Add SensorAttachmentTable
                    typeObservations = observations.Where(o => o.Type == SensorAttachmentDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SensorAttachmentTable = jsonObservations;
                    }


                    // Add SensorState
                    typeObservations = observations.Where(o => o.Type == SensorStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SensorState = jsonObservations;
                    }

                    // Add SensorStateDataSet
                    typeObservations = observations.Where(o => o.Type == SensorStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SensorStateDataSet = jsonObservations;
                    }

                    // Add SensorStateTable
                    typeObservations = observations.Where(o => o.Type == SensorStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SensorStateTable = jsonObservations;
                    }


                    // Add SerialNumber
                    typeObservations = observations.Where(o => o.Type == SerialNumberDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SerialNumber = jsonObservations;
                    }

                    // Add SerialNumberDataSet
                    typeObservations = observations.Where(o => o.Type == SerialNumberDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SerialNumberDataSet = jsonObservations;
                    }

                    // Add SerialNumberTable
                    typeObservations = observations.Where(o => o.Type == SerialNumberDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SerialNumberTable = jsonObservations;
                    }


                    // Add SpecificationLimit
                    typeObservations = observations.Where(o => o.Type == SpecificationLimitDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SpecificationLimit = jsonObservations;
                    }

                    // Add SpecificationLimitDataSet
                    typeObservations = observations.Where(o => o.Type == SpecificationLimitDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SpecificationLimitDataSet = jsonObservations;
                    }

                    // Add SpecificationLimitTable
                    typeObservations = observations.Where(o => o.Type == SpecificationLimitDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SpecificationLimitTable = jsonObservations;
                    }


                    // Add SpecificationLimits
                    typeObservations = observations.Where(o => o.Type == SpecificationLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SpecificationLimits = jsonObservations;
                    }

                    // Add SpecificationLimitsDataSet
                    typeObservations = observations.Where(o => o.Type == SpecificationLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SpecificationLimitsDataSet = jsonObservations;
                    }

                    // Add SpecificationLimitsTable
                    typeObservations = observations.Where(o => o.Type == SpecificationLimitsDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SpecificationLimitsTable = jsonObservations;
                    }


                    // Add SpindleInterlock
                    typeObservations = observations.Where(o => o.Type == SpindleInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SpindleInterlock = jsonObservations;
                    }

                    // Add SpindleInterlockDataSet
                    typeObservations = observations.Where(o => o.Type == SpindleInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SpindleInterlockDataSet = jsonObservations;
                    }

                    // Add SpindleInterlockTable
                    typeObservations = observations.Where(o => o.Type == SpindleInterlockDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SpindleInterlockTable = jsonObservations;
                    }


                    // Add SwingAngle
                    typeObservations = observations.Where(o => o.Type == SwingAngleDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SwingAngle = jsonObservations;
                    }

                    // Add SwingAngleDataSet
                    typeObservations = observations.Where(o => o.Type == SwingAngleDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SwingAngleDataSet = jsonObservations;
                    }

                    // Add SwingAngleTable
                    typeObservations = observations.Where(o => o.Type == SwingAngleDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SwingAngleTable = jsonObservations;
                    }


                    // Add SwingDiameter
                    typeObservations = observations.Where(o => o.Type == SwingDiameterDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SwingDiameter = jsonObservations;
                    }

                    // Add SwingDiameterDataSet
                    typeObservations = observations.Where(o => o.Type == SwingDiameterDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SwingDiameterDataSet = jsonObservations;
                    }

                    // Add SwingDiameterTable
                    typeObservations = observations.Where(o => o.Type == SwingDiameterDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SwingDiameterTable = jsonObservations;
                    }


                    // Add SwingRadius
                    typeObservations = observations.Where(o => o.Type == SwingRadiusDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        SwingRadius = jsonObservations;
                    }

                    // Add SwingRadiusDataSet
                    typeObservations = observations.Where(o => o.Type == SwingRadiusDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        SwingRadiusDataSet = jsonObservations;
                    }

                    // Add SwingRadiusTable
                    typeObservations = observations.Where(o => o.Type == SwingRadiusDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        SwingRadiusTable = jsonObservations;
                    }


                    // Add TaskAssetId
                    typeObservations = observations.Where(o => o.Type == TaskAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        TaskAssetId = jsonObservations;
                    }

                    // Add TaskAssetIdDataSet
                    typeObservations = observations.Where(o => o.Type == TaskAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        TaskAssetIdDataSet = jsonObservations;
                    }

                    // Add TaskAssetIdTable
                    typeObservations = observations.Where(o => o.Type == TaskAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        TaskAssetIdTable = jsonObservations;
                    }


                    // Add Thickness
                    typeObservations = observations.Where(o => o.Type == ThicknessDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Thickness = jsonObservations;
                    }

                    // Add ThicknessDataSet
                    typeObservations = observations.Where(o => o.Type == ThicknessDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ThicknessDataSet = jsonObservations;
                    }

                    // Add ThicknessTable
                    typeObservations = observations.Where(o => o.Type == ThicknessDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ThicknessTable = jsonObservations;
                    }


                    // Add ToolAssetId
                    typeObservations = observations.Where(o => o.Type == ToolAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ToolAssetId = jsonObservations;
                    }

                    // Add ToolAssetIdDataSet
                    typeObservations = observations.Where(o => o.Type == ToolAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ToolAssetIdDataSet = jsonObservations;
                    }

                    // Add ToolAssetIdTable
                    typeObservations = observations.Where(o => o.Type == ToolAssetIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ToolAssetIdTable = jsonObservations;
                    }


                    // Add ToolCuttingItem
                    typeObservations = observations.Where(o => o.Type == ToolCuttingItemDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ToolCuttingItem = jsonObservations;
                    }

                    // Add ToolCuttingItemDataSet
                    typeObservations = observations.Where(o => o.Type == ToolCuttingItemDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ToolCuttingItemDataSet = jsonObservations;
                    }

                    // Add ToolCuttingItemTable
                    typeObservations = observations.Where(o => o.Type == ToolCuttingItemDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ToolCuttingItemTable = jsonObservations;
                    }


                    // Add ToolGroup
                    typeObservations = observations.Where(o => o.Type == ToolGroupDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ToolGroup = jsonObservations;
                    }

                    // Add ToolGroupDataSet
                    typeObservations = observations.Where(o => o.Type == ToolGroupDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ToolGroupDataSet = jsonObservations;
                    }

                    // Add ToolGroupTable
                    typeObservations = observations.Where(o => o.Type == ToolGroupDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ToolGroupTable = jsonObservations;
                    }


                    // Add ToolId
                    typeObservations = observations.Where(o => o.Type == ToolIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ToolId = jsonObservations;
                    }

                    // Add ToolIdDataSet
                    typeObservations = observations.Where(o => o.Type == ToolIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ToolIdDataSet = jsonObservations;
                    }

                    // Add ToolIdTable
                    typeObservations = observations.Where(o => o.Type == ToolIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ToolIdTable = jsonObservations;
                    }


                    // Add ToolNumber
                    typeObservations = observations.Where(o => o.Type == ToolNumberDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ToolNumber = jsonObservations;
                    }

                    // Add ToolNumberDataSet
                    typeObservations = observations.Where(o => o.Type == ToolNumberDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ToolNumberDataSet = jsonObservations;
                    }

                    // Add ToolNumberTable
                    typeObservations = observations.Where(o => o.Type == ToolNumberDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ToolNumberTable = jsonObservations;
                    }


                    // Add ToolOffset
                    typeObservations = observations.Where(o => o.Type == ToolOffsetDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ToolOffset = jsonObservations;
                    }

                    // Add ToolOffsetDataSet
                    typeObservations = observations.Where(o => o.Type == ToolOffsetDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ToolOffsetDataSet = jsonObservations;
                    }

                    // Add ToolOffsetTable
                    typeObservations = observations.Where(o => o.Type == ToolOffsetDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ToolOffsetTable = jsonObservations;
                    }


                    // Add ToolOffsets
                    typeObservations = observations.Where(o => o.Type == ToolOffsetsDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ToolOffsets = jsonObservations;
                    }

                    // Add ToolOffsetsDataSet
                    typeObservations = observations.Where(o => o.Type == ToolOffsetsDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ToolOffsetsDataSet = jsonObservations;
                    }

                    // Add ToolOffsetsTable
                    typeObservations = observations.Where(o => o.Type == ToolOffsetsDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ToolOffsetsTable = jsonObservations;
                    }


                    // Add TransferCount
                    typeObservations = observations.Where(o => o.Type == TransferCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        TransferCount = jsonObservations;
                    }

                    // Add TransferCountDataSet
                    typeObservations = observations.Where(o => o.Type == TransferCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        TransferCountDataSet = jsonObservations;
                    }

                    // Add TransferCountTable
                    typeObservations = observations.Where(o => o.Type == TransferCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        TransferCountTable = jsonObservations;
                    }


                    // Add Translation
                    typeObservations = observations.Where(o => o.Type == TranslationDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Translation = jsonObservations;
                    }

                    // Add TranslationDataSet
                    typeObservations = observations.Where(o => o.Type == TranslationDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        TranslationDataSet = jsonObservations;
                    }

                    // Add TranslationTable
                    typeObservations = observations.Where(o => o.Type == TranslationDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        TranslationTable = jsonObservations;
                    }


                    // Add Uncertainty
                    typeObservations = observations.Where(o => o.Type == UncertaintyDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Uncertainty = jsonObservations;
                    }

                    // Add UncertaintyDataSet
                    typeObservations = observations.Where(o => o.Type == UncertaintyDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        UncertaintyDataSet = jsonObservations;
                    }

                    // Add UncertaintyTable
                    typeObservations = observations.Where(o => o.Type == UncertaintyDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        UncertaintyTable = jsonObservations;
                    }


                    // Add UncertaintyType
                    typeObservations = observations.Where(o => o.Type == UncertaintyTypeDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        UncertaintyType = jsonObservations;
                    }

                    // Add UncertaintyTypeDataSet
                    typeObservations = observations.Where(o => o.Type == UncertaintyTypeDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        UncertaintyTypeDataSet = jsonObservations;
                    }

                    // Add UncertaintyTypeTable
                    typeObservations = observations.Where(o => o.Type == UncertaintyTypeDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        UncertaintyTypeTable = jsonObservations;
                    }


                    // Add UnloadCount
                    typeObservations = observations.Where(o => o.Type == UnloadCountDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        UnloadCount = jsonObservations;
                    }

                    // Add UnloadCountDataSet
                    typeObservations = observations.Where(o => o.Type == UnloadCountDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        UnloadCountDataSet = jsonObservations;
                    }

                    // Add UnloadCountTable
                    typeObservations = observations.Where(o => o.Type == UnloadCountDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        UnloadCountTable = jsonObservations;
                    }


                    // Add User
                    typeObservations = observations.Where(o => o.Type == UserDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        User = jsonObservations;
                    }

                    // Add UserDataSet
                    typeObservations = observations.Where(o => o.Type == UserDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        UserDataSet = jsonObservations;
                    }

                    // Add UserTable
                    typeObservations = observations.Where(o => o.Type == UserDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        UserTable = jsonObservations;
                    }


                    // Add ValveState
                    typeObservations = observations.Where(o => o.Type == ValveStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        ValveState = jsonObservations;
                    }

                    // Add ValveStateDataSet
                    typeObservations = observations.Where(o => o.Type == ValveStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        ValveStateDataSet = jsonObservations;
                    }

                    // Add ValveStateTable
                    typeObservations = observations.Where(o => o.Type == ValveStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        ValveStateTable = jsonObservations;
                    }


                    // Add Variable
                    typeObservations = observations.Where(o => o.Type == VariableDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Variable = jsonObservations;
                    }

                    // Add VariableDataSet
                    typeObservations = observations.Where(o => o.Type == VariableDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        VariableDataSet = jsonObservations;
                    }

                    // Add VariableTable
                    typeObservations = observations.Where(o => o.Type == VariableDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        VariableTable = jsonObservations;
                    }


                    // Add WaitState
                    typeObservations = observations.Where(o => o.Type == WaitStateDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        WaitState = jsonObservations;
                    }

                    // Add WaitStateDataSet
                    typeObservations = observations.Where(o => o.Type == WaitStateDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        WaitStateDataSet = jsonObservations;
                    }

                    // Add WaitStateTable
                    typeObservations = observations.Where(o => o.Type == WaitStateDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        WaitStateTable = jsonObservations;
                    }


                    // Add Wire
                    typeObservations = observations.Where(o => o.Type == WireDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        Wire = jsonObservations;
                    }

                    // Add WireDataSet
                    typeObservations = observations.Where(o => o.Type == WireDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        WireDataSet = jsonObservations;
                    }

                    // Add WireTable
                    typeObservations = observations.Where(o => o.Type == WireDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        WireTable = jsonObservations;
                    }


                    // Add WorkOffset
                    typeObservations = observations.Where(o => o.Type == WorkOffsetDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        WorkOffset = jsonObservations;
                    }

                    // Add WorkOffsetDataSet
                    typeObservations = observations.Where(o => o.Type == WorkOffsetDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        WorkOffsetDataSet = jsonObservations;
                    }

                    // Add WorkOffsetTable
                    typeObservations = observations.Where(o => o.Type == WorkOffsetDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        WorkOffsetTable = jsonObservations;
                    }


                    // Add WorkOffsets
                    typeObservations = observations.Where(o => o.Type == WorkOffsetsDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        WorkOffsets = jsonObservations;
                    }

                    // Add WorkOffsetsDataSet
                    typeObservations = observations.Where(o => o.Type == WorkOffsetsDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        WorkOffsetsDataSet = jsonObservations;
                    }

                    // Add WorkOffsetsTable
                    typeObservations = observations.Where(o => o.Type == WorkOffsetsDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        WorkOffsetsTable = jsonObservations;
                    }


                    // Add WorkholdingId
                    typeObservations = observations.Where(o => o.Type == WorkholdingIdDataItem.TypeId && o.Representation == DataItemRepresentation.VALUE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventValue>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventValue(observation));
                        }
                        WorkholdingId = jsonObservations;
                    }

                    // Add WorkholdingIdDataSet
                    typeObservations = observations.Where(o => o.Type == WorkholdingIdDataItem.TypeId && o.Representation == DataItemRepresentation.DATA_SET);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventDataSet>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventDataSet(observation));
                        }
                        WorkholdingIdDataSet = jsonObservations;
                    }

                    // Add WorkholdingIdTable
                    typeObservations = observations.Where(o => o.Type == WorkholdingIdDataItem.TypeId && o.Representation == DataItemRepresentation.TABLE);
                    if (!typeObservations.IsNullOrEmpty())
                    {
                        var jsonObservations = new List<JsonEventTable>();
                        foreach (var observation in typeObservations)
                        {
                            jsonObservations.Add(new JsonEventTable(observation));
                        }
                        WorkholdingIdTable = jsonObservations;
                    }


                }
            }
        }
    }
}
