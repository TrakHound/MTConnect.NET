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
    public class JsonEvents
    {
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

                if (!Application.IsNullOrEmpty()) foreach (var x in Application) l.Add(x.ToObservation(ApplicationDataItem.TypeId));
                if (!ApplicationDataSet.IsNullOrEmpty()) foreach (var x in ApplicationDataSet) l.Add(x.ToObservation(ApplicationDataItem.TypeId));
                if (!ApplicationTable.IsNullOrEmpty()) foreach (var x in ApplicationTable) l.Add(x.ToObservation(ApplicationDataItem.TypeId));

                if (!AssetChanged.IsNullOrEmpty()) foreach (var x in AssetChanged) l.Add(x.ToObservation(AssetChangedDataItem.TypeId));
                if (!AssetChangedDataSet.IsNullOrEmpty()) foreach (var x in AssetChangedDataSet) l.Add(x.ToObservation(AssetChangedDataItem.TypeId));
                if (!AssetChangedTable.IsNullOrEmpty()) foreach (var x in AssetChangedTable) l.Add(x.ToObservation(AssetChangedDataItem.TypeId));

                if (!AssetCount.IsNullOrEmpty()) foreach (var x in AssetCount) l.Add(x.ToObservation(AssetCountDataItem.TypeId));
                if (!AssetCountDataSet.IsNullOrEmpty()) foreach (var x in AssetCountDataSet) l.Add(x.ToObservation(AssetCountDataItem.TypeId));
                if (!AssetCountTable.IsNullOrEmpty()) foreach (var x in AssetCountTable) l.Add(x.ToObservation(AssetCountDataItem.TypeId));

                if (!AssetRemoved.IsNullOrEmpty()) foreach (var x in AssetRemoved) l.Add(x.ToObservation(AssetRemovedDataItem.TypeId));
                if (!AssetRemovedDataSet.IsNullOrEmpty()) foreach (var x in AssetRemovedDataSet) l.Add(x.ToObservation(AssetRemovedDataItem.TypeId));
                if (!AssetRemovedTable.IsNullOrEmpty()) foreach (var x in AssetRemovedTable) l.Add(x.ToObservation(AssetRemovedDataItem.TypeId));

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

                if (!SpindleInterlock.IsNullOrEmpty()) foreach (var x in SpindleInterlock) l.Add(x.ToObservation(SpindleInterlockDataItem.TypeId));
                if (!SpindleInterlockDataSet.IsNullOrEmpty()) foreach (var x in SpindleInterlockDataSet) l.Add(x.ToObservation(SpindleInterlockDataItem.TypeId));
                if (!SpindleInterlockTable.IsNullOrEmpty()) foreach (var x in SpindleInterlockTable) l.Add(x.ToObservation(SpindleInterlockDataItem.TypeId));

                if (!ToolAssetId.IsNullOrEmpty()) foreach (var x in ToolAssetId) l.Add(x.ToObservation(ToolAssetIdDataItem.TypeId));
                if (!ToolAssetIdDataSet.IsNullOrEmpty()) foreach (var x in ToolAssetIdDataSet) l.Add(x.ToObservation(ToolAssetIdDataItem.TypeId));
                if (!ToolAssetIdTable.IsNullOrEmpty()) foreach (var x in ToolAssetIdTable) l.Add(x.ToObservation(ToolAssetIdDataItem.TypeId));

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
        [JsonPropertyName("ActivationCount")]
        public IEnumerable<JsonEventValue> ActivationCount { get; set; }

        [JsonPropertyName("ActivationCountDataSet")]
        public IEnumerable<JsonEventDataSet> ActivationCountDataSet { get; set; }

        [JsonPropertyName("ActivationCountTable")]
        public IEnumerable<JsonEventTable> ActivationCountTable { get; set; }


        [JsonPropertyName("ActiveAxes")]
        public IEnumerable<JsonEventValue> ActiveAxes { get; set; }

        [JsonPropertyName("ActiveAxesDataSet")]
        public IEnumerable<JsonEventDataSet> ActiveAxesDataSet { get; set; }

        [JsonPropertyName("ActiveAxesTable")]
        public IEnumerable<JsonEventTable> ActiveAxesTable { get; set; }


        [JsonPropertyName("ActuatorState")]
        public IEnumerable<JsonEventValue> ActuatorState { get; set; }

        [JsonPropertyName("ActuatorStateDataSet")]
        public IEnumerable<JsonEventDataSet> ActuatorStateDataSet { get; set; }

        [JsonPropertyName("ActuatorStateTable")]
        public IEnumerable<JsonEventTable> ActuatorStateTable { get; set; }


        [JsonPropertyName("AdapterSoftwareVersion")]
        public IEnumerable<JsonEventValue> AdapterSoftwareVersion { get; set; }

        [JsonPropertyName("AdapterSoftwareVersionDataSet")]
        public IEnumerable<JsonEventDataSet> AdapterSoftwareVersionDataSet { get; set; }

        [JsonPropertyName("AdapterSoftwareVersionTable")]
        public IEnumerable<JsonEventTable> AdapterSoftwareVersionTable { get; set; }


        [JsonPropertyName("AdapterUri")]
        public IEnumerable<JsonEventValue> AdapterUri { get; set; }

        [JsonPropertyName("AdapterUriDataSet")]
        public IEnumerable<JsonEventDataSet> AdapterUriDataSet { get; set; }

        [JsonPropertyName("AdapterUriTable")]
        public IEnumerable<JsonEventTable> AdapterUriTable { get; set; }


        [JsonPropertyName("Alarm")]
        public IEnumerable<JsonEventValue> Alarm { get; set; }

        [JsonPropertyName("AlarmDataSet")]
        public IEnumerable<JsonEventDataSet> AlarmDataSet { get; set; }

        [JsonPropertyName("AlarmTable")]
        public IEnumerable<JsonEventTable> AlarmTable { get; set; }


        [JsonPropertyName("AlarmLimit")]
        public IEnumerable<JsonEventValue> AlarmLimit { get; set; }

        [JsonPropertyName("AlarmLimitDataSet")]
        public IEnumerable<JsonEventDataSet> AlarmLimitDataSet { get; set; }

        [JsonPropertyName("AlarmLimitTable")]
        public IEnumerable<JsonEventTable> AlarmLimitTable { get; set; }


        [JsonPropertyName("Application")]
        public IEnumerable<JsonEventValue> Application { get; set; }

        [JsonPropertyName("ApplicationDataSet")]
        public IEnumerable<JsonEventDataSet> ApplicationDataSet { get; set; }

        [JsonPropertyName("ApplicationTable")]
        public IEnumerable<JsonEventTable> ApplicationTable { get; set; }


        [JsonPropertyName("AssetChanged")]
        public IEnumerable<JsonEventValue> AssetChanged { get; set; }

        [JsonPropertyName("AssetChangedDataSet")]
        public IEnumerable<JsonEventDataSet> AssetChangedDataSet { get; set; }

        [JsonPropertyName("AssetChangedTable")]
        public IEnumerable<JsonEventTable> AssetChangedTable { get; set; }


        [JsonPropertyName("AssetCount")]
        public IEnumerable<JsonEventValue> AssetCount { get; set; }

        [JsonPropertyName("AssetCountDataSet")]
        public IEnumerable<JsonEventDataSet> AssetCountDataSet { get; set; }

        [JsonPropertyName("AssetCountTable")]
        public IEnumerable<JsonEventTable> AssetCountTable { get; set; }


        [JsonPropertyName("AssetRemoved")]
        public IEnumerable<JsonEventValue> AssetRemoved { get; set; }

        [JsonPropertyName("AssetRemovedDataSet")]
        public IEnumerable<JsonEventDataSet> AssetRemovedDataSet { get; set; }

        [JsonPropertyName("AssetRemovedTable")]
        public IEnumerable<JsonEventTable> AssetRemovedTable { get; set; }


        [JsonPropertyName("Availability")]
        public IEnumerable<JsonEventValue> Availability { get; set; }

        [JsonPropertyName("AvailabilityDataSet")]
        public IEnumerable<JsonEventDataSet> AvailabilityDataSet { get; set; }

        [JsonPropertyName("AvailabilityTable")]
        public IEnumerable<JsonEventTable> AvailabilityTable { get; set; }


        [JsonPropertyName("AxisCoupling")]
        public IEnumerable<JsonEventValue> AxisCoupling { get; set; }

        [JsonPropertyName("AxisCouplingDataSet")]
        public IEnumerable<JsonEventDataSet> AxisCouplingDataSet { get; set; }

        [JsonPropertyName("AxisCouplingTable")]
        public IEnumerable<JsonEventTable> AxisCouplingTable { get; set; }


        [JsonPropertyName("AxisFeedrateOverride")]
        public IEnumerable<JsonEventValue> AxisFeedrateOverride { get; set; }

        [JsonPropertyName("AxisFeedrateOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> AxisFeedrateOverrideDataSet { get; set; }

        [JsonPropertyName("AxisFeedrateOverrideTable")]
        public IEnumerable<JsonEventTable> AxisFeedrateOverrideTable { get; set; }


        [JsonPropertyName("AxisInterlock")]
        public IEnumerable<JsonEventValue> AxisInterlock { get; set; }

        [JsonPropertyName("AxisInterlockDataSet")]
        public IEnumerable<JsonEventDataSet> AxisInterlockDataSet { get; set; }

        [JsonPropertyName("AxisInterlockTable")]
        public IEnumerable<JsonEventTable> AxisInterlockTable { get; set; }


        [JsonPropertyName("AxisState")]
        public IEnumerable<JsonEventValue> AxisState { get; set; }

        [JsonPropertyName("AxisStateDataSet")]
        public IEnumerable<JsonEventDataSet> AxisStateDataSet { get; set; }

        [JsonPropertyName("AxisStateTable")]
        public IEnumerable<JsonEventTable> AxisStateTable { get; set; }


        [JsonPropertyName("BatteryState")]
        public IEnumerable<JsonEventValue> BatteryState { get; set; }

        [JsonPropertyName("BatteryStateDataSet")]
        public IEnumerable<JsonEventDataSet> BatteryStateDataSet { get; set; }

        [JsonPropertyName("BatteryStateTable")]
        public IEnumerable<JsonEventTable> BatteryStateTable { get; set; }


        [JsonPropertyName("Block")]
        public IEnumerable<JsonEventValue> Block { get; set; }

        [JsonPropertyName("BlockDataSet")]
        public IEnumerable<JsonEventDataSet> BlockDataSet { get; set; }

        [JsonPropertyName("BlockTable")]
        public IEnumerable<JsonEventTable> BlockTable { get; set; }


        [JsonPropertyName("BlockCount")]
        public IEnumerable<JsonEventValue> BlockCount { get; set; }

        [JsonPropertyName("BlockCountDataSet")]
        public IEnumerable<JsonEventDataSet> BlockCountDataSet { get; set; }

        [JsonPropertyName("BlockCountTable")]
        public IEnumerable<JsonEventTable> BlockCountTable { get; set; }


        [JsonPropertyName("CharacteristicPersistentId")]
        public IEnumerable<JsonEventValue> CharacteristicPersistentId { get; set; }

        [JsonPropertyName("CharacteristicPersistentIdDataSet")]
        public IEnumerable<JsonEventDataSet> CharacteristicPersistentIdDataSet { get; set; }

        [JsonPropertyName("CharacteristicPersistentIdTable")]
        public IEnumerable<JsonEventTable> CharacteristicPersistentIdTable { get; set; }


        [JsonPropertyName("CharacteristicStatus")]
        public IEnumerable<JsonEventValue> CharacteristicStatus { get; set; }

        [JsonPropertyName("CharacteristicStatusDataSet")]
        public IEnumerable<JsonEventDataSet> CharacteristicStatusDataSet { get; set; }

        [JsonPropertyName("CharacteristicStatusTable")]
        public IEnumerable<JsonEventTable> CharacteristicStatusTable { get; set; }


        [JsonPropertyName("ChuckInterlock")]
        public IEnumerable<JsonEventValue> ChuckInterlock { get; set; }

        [JsonPropertyName("ChuckInterlockDataSet")]
        public IEnumerable<JsonEventDataSet> ChuckInterlockDataSet { get; set; }

        [JsonPropertyName("ChuckInterlockTable")]
        public IEnumerable<JsonEventTable> ChuckInterlockTable { get; set; }


        [JsonPropertyName("ChuckState")]
        public IEnumerable<JsonEventValue> ChuckState { get; set; }

        [JsonPropertyName("ChuckStateDataSet")]
        public IEnumerable<JsonEventDataSet> ChuckStateDataSet { get; set; }

        [JsonPropertyName("ChuckStateTable")]
        public IEnumerable<JsonEventTable> ChuckStateTable { get; set; }


        [JsonPropertyName("ClockTime")]
        public IEnumerable<JsonEventValue> ClockTime { get; set; }

        [JsonPropertyName("ClockTimeDataSet")]
        public IEnumerable<JsonEventDataSet> ClockTimeDataSet { get; set; }

        [JsonPropertyName("ClockTimeTable")]
        public IEnumerable<JsonEventTable> ClockTimeTable { get; set; }


        [JsonPropertyName("Code")]
        public IEnumerable<JsonEventValue> Code { get; set; }

        [JsonPropertyName("CodeDataSet")]
        public IEnumerable<JsonEventDataSet> CodeDataSet { get; set; }

        [JsonPropertyName("CodeTable")]
        public IEnumerable<JsonEventTable> CodeTable { get; set; }


        [JsonPropertyName("ComponentData")]
        public IEnumerable<JsonEventValue> ComponentData { get; set; }

        [JsonPropertyName("ComponentDataDataSet")]
        public IEnumerable<JsonEventDataSet> ComponentDataDataSet { get; set; }

        [JsonPropertyName("ComponentDataTable")]
        public IEnumerable<JsonEventTable> ComponentDataTable { get; set; }


        [JsonPropertyName("CompositionState")]
        public IEnumerable<JsonEventValue> CompositionState { get; set; }

        [JsonPropertyName("CompositionStateDataSet")]
        public IEnumerable<JsonEventDataSet> CompositionStateDataSet { get; set; }

        [JsonPropertyName("CompositionStateTable")]
        public IEnumerable<JsonEventTable> CompositionStateTable { get; set; }


        [JsonPropertyName("ConnectionStatus")]
        public IEnumerable<JsonEventValue> ConnectionStatus { get; set; }

        [JsonPropertyName("ConnectionStatusDataSet")]
        public IEnumerable<JsonEventDataSet> ConnectionStatusDataSet { get; set; }

        [JsonPropertyName("ConnectionStatusTable")]
        public IEnumerable<JsonEventTable> ConnectionStatusTable { get; set; }


        [JsonPropertyName("ControlLimit")]
        public IEnumerable<JsonEventValue> ControlLimit { get; set; }

        [JsonPropertyName("ControlLimitDataSet")]
        public IEnumerable<JsonEventDataSet> ControlLimitDataSet { get; set; }

        [JsonPropertyName("ControlLimitTable")]
        public IEnumerable<JsonEventTable> ControlLimitTable { get; set; }


        [JsonPropertyName("ControllerMode")]
        public IEnumerable<JsonEventValue> ControllerMode { get; set; }

        [JsonPropertyName("ControllerModeDataSet")]
        public IEnumerable<JsonEventDataSet> ControllerModeDataSet { get; set; }

        [JsonPropertyName("ControllerModeTable")]
        public IEnumerable<JsonEventTable> ControllerModeTable { get; set; }


        [JsonPropertyName("ControllerModeOverride")]
        public IEnumerable<JsonEventValue> ControllerModeOverride { get; set; }

        [JsonPropertyName("ControllerModeOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> ControllerModeOverrideDataSet { get; set; }

        [JsonPropertyName("ControllerModeOverrideTable")]
        public IEnumerable<JsonEventTable> ControllerModeOverrideTable { get; set; }


        [JsonPropertyName("CoupledAxes")]
        public IEnumerable<JsonEventValue> CoupledAxes { get; set; }

        [JsonPropertyName("CoupledAxesDataSet")]
        public IEnumerable<JsonEventDataSet> CoupledAxesDataSet { get; set; }

        [JsonPropertyName("CoupledAxesTable")]
        public IEnumerable<JsonEventTable> CoupledAxesTable { get; set; }


        [JsonPropertyName("CycleCount")]
        public IEnumerable<JsonEventValue> CycleCount { get; set; }

        [JsonPropertyName("CycleCountDataSet")]
        public IEnumerable<JsonEventDataSet> CycleCountDataSet { get; set; }

        [JsonPropertyName("CycleCountTable")]
        public IEnumerable<JsonEventTable> CycleCountTable { get; set; }


        [JsonPropertyName("DateCode")]
        public IEnumerable<JsonEventValue> DateCode { get; set; }

        [JsonPropertyName("DateCodeDataSet")]
        public IEnumerable<JsonEventDataSet> DateCodeDataSet { get; set; }

        [JsonPropertyName("DateCodeTable")]
        public IEnumerable<JsonEventTable> DateCodeTable { get; set; }


        [JsonPropertyName("DeactivationCount")]
        public IEnumerable<JsonEventValue> DeactivationCount { get; set; }

        [JsonPropertyName("DeactivationCountDataSet")]
        public IEnumerable<JsonEventDataSet> DeactivationCountDataSet { get; set; }

        [JsonPropertyName("DeactivationCountTable")]
        public IEnumerable<JsonEventTable> DeactivationCountTable { get; set; }


        [JsonPropertyName("DeviceAdded")]
        public IEnumerable<JsonEventValue> DeviceAdded { get; set; }

        [JsonPropertyName("DeviceAddedDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceAddedDataSet { get; set; }

        [JsonPropertyName("DeviceAddedTable")]
        public IEnumerable<JsonEventTable> DeviceAddedTable { get; set; }


        [JsonPropertyName("DeviceChanged")]
        public IEnumerable<JsonEventValue> DeviceChanged { get; set; }

        [JsonPropertyName("DeviceChangedDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceChangedDataSet { get; set; }

        [JsonPropertyName("DeviceChangedTable")]
        public IEnumerable<JsonEventTable> DeviceChangedTable { get; set; }


        [JsonPropertyName("DeviceRemoved")]
        public IEnumerable<JsonEventValue> DeviceRemoved { get; set; }

        [JsonPropertyName("DeviceRemovedDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceRemovedDataSet { get; set; }

        [JsonPropertyName("DeviceRemovedTable")]
        public IEnumerable<JsonEventTable> DeviceRemovedTable { get; set; }


        [JsonPropertyName("DeviceUuid")]
        public IEnumerable<JsonEventValue> DeviceUuid { get; set; }

        [JsonPropertyName("DeviceUuidDataSet")]
        public IEnumerable<JsonEventDataSet> DeviceUuidDataSet { get; set; }

        [JsonPropertyName("DeviceUuidTable")]
        public IEnumerable<JsonEventTable> DeviceUuidTable { get; set; }


        [JsonPropertyName("Direction")]
        public IEnumerable<JsonEventValue> Direction { get; set; }

        [JsonPropertyName("DirectionDataSet")]
        public IEnumerable<JsonEventDataSet> DirectionDataSet { get; set; }

        [JsonPropertyName("DirectionTable")]
        public IEnumerable<JsonEventTable> DirectionTable { get; set; }


        [JsonPropertyName("DoorState")]
        public IEnumerable<JsonEventValue> DoorState { get; set; }

        [JsonPropertyName("DoorStateDataSet")]
        public IEnumerable<JsonEventDataSet> DoorStateDataSet { get; set; }

        [JsonPropertyName("DoorStateTable")]
        public IEnumerable<JsonEventTable> DoorStateTable { get; set; }


        [JsonPropertyName("EmergencyStop")]
        public IEnumerable<JsonEventValue> EmergencyStop { get; set; }

        [JsonPropertyName("EmergencyStopDataSet")]
        public IEnumerable<JsonEventDataSet> EmergencyStopDataSet { get; set; }

        [JsonPropertyName("EmergencyStopTable")]
        public IEnumerable<JsonEventTable> EmergencyStopTable { get; set; }


        [JsonPropertyName("EndOfBar")]
        public IEnumerable<JsonEventValue> EndOfBar { get; set; }

        [JsonPropertyName("EndOfBarDataSet")]
        public IEnumerable<JsonEventDataSet> EndOfBarDataSet { get; set; }

        [JsonPropertyName("EndOfBarTable")]
        public IEnumerable<JsonEventTable> EndOfBarTable { get; set; }


        [JsonPropertyName("EquipmentMode")]
        public IEnumerable<JsonEventValue> EquipmentMode { get; set; }

        [JsonPropertyName("EquipmentModeDataSet")]
        public IEnumerable<JsonEventDataSet> EquipmentModeDataSet { get; set; }

        [JsonPropertyName("EquipmentModeTable")]
        public IEnumerable<JsonEventTable> EquipmentModeTable { get; set; }


        [JsonPropertyName("Execution")]
        public IEnumerable<JsonEventValue> Execution { get; set; }

        [JsonPropertyName("ExecutionDataSet")]
        public IEnumerable<JsonEventDataSet> ExecutionDataSet { get; set; }

        [JsonPropertyName("ExecutionTable")]
        public IEnumerable<JsonEventTable> ExecutionTable { get; set; }


        [JsonPropertyName("FeatureMeasurement")]
        public IEnumerable<JsonEventValue> FeatureMeasurement { get; set; }

        [JsonPropertyName("FeatureMeasurementDataSet")]
        public IEnumerable<JsonEventDataSet> FeatureMeasurementDataSet { get; set; }

        [JsonPropertyName("FeatureMeasurementTable")]
        public IEnumerable<JsonEventTable> FeatureMeasurementTable { get; set; }


        [JsonPropertyName("Firmware")]
        public IEnumerable<JsonEventValue> Firmware { get; set; }

        [JsonPropertyName("FirmwareDataSet")]
        public IEnumerable<JsonEventDataSet> FirmwareDataSet { get; set; }

        [JsonPropertyName("FirmwareTable")]
        public IEnumerable<JsonEventTable> FirmwareTable { get; set; }


        [JsonPropertyName("FixtureId")]
        public IEnumerable<JsonEventValue> FixtureId { get; set; }

        [JsonPropertyName("FixtureIdDataSet")]
        public IEnumerable<JsonEventDataSet> FixtureIdDataSet { get; set; }

        [JsonPropertyName("FixtureIdTable")]
        public IEnumerable<JsonEventTable> FixtureIdTable { get; set; }


        [JsonPropertyName("FunctionalMode")]
        public IEnumerable<JsonEventValue> FunctionalMode { get; set; }

        [JsonPropertyName("FunctionalModeDataSet")]
        public IEnumerable<JsonEventDataSet> FunctionalModeDataSet { get; set; }

        [JsonPropertyName("FunctionalModeTable")]
        public IEnumerable<JsonEventTable> FunctionalModeTable { get; set; }


        [JsonPropertyName("Hardness")]
        public IEnumerable<JsonEventValue> Hardness { get; set; }

        [JsonPropertyName("HardnessDataSet")]
        public IEnumerable<JsonEventDataSet> HardnessDataSet { get; set; }

        [JsonPropertyName("HardnessTable")]
        public IEnumerable<JsonEventTable> HardnessTable { get; set; }


        [JsonPropertyName("Hardware")]
        public IEnumerable<JsonEventValue> Hardware { get; set; }

        [JsonPropertyName("HardwareDataSet")]
        public IEnumerable<JsonEventDataSet> HardwareDataSet { get; set; }

        [JsonPropertyName("HardwareTable")]
        public IEnumerable<JsonEventTable> HardwareTable { get; set; }


        [JsonPropertyName("HostName")]
        public IEnumerable<JsonEventValue> HostName { get; set; }

        [JsonPropertyName("HostNameDataSet")]
        public IEnumerable<JsonEventDataSet> HostNameDataSet { get; set; }

        [JsonPropertyName("HostNameTable")]
        public IEnumerable<JsonEventTable> HostNameTable { get; set; }


        [JsonPropertyName("LeakDetect")]
        public IEnumerable<JsonEventValue> LeakDetect { get; set; }

        [JsonPropertyName("LeakDetectDataSet")]
        public IEnumerable<JsonEventDataSet> LeakDetectDataSet { get; set; }

        [JsonPropertyName("LeakDetectTable")]
        public IEnumerable<JsonEventTable> LeakDetectTable { get; set; }


        [JsonPropertyName("Library")]
        public IEnumerable<JsonEventValue> Library { get; set; }

        [JsonPropertyName("LibraryDataSet")]
        public IEnumerable<JsonEventDataSet> LibraryDataSet { get; set; }

        [JsonPropertyName("LibraryTable")]
        public IEnumerable<JsonEventTable> LibraryTable { get; set; }


        [JsonPropertyName("Line")]
        public IEnumerable<JsonEventValue> Line { get; set; }

        [JsonPropertyName("LineDataSet")]
        public IEnumerable<JsonEventDataSet> LineDataSet { get; set; }

        [JsonPropertyName("LineTable")]
        public IEnumerable<JsonEventTable> LineTable { get; set; }


        [JsonPropertyName("LineLabel")]
        public IEnumerable<JsonEventValue> LineLabel { get; set; }

        [JsonPropertyName("LineLabelDataSet")]
        public IEnumerable<JsonEventDataSet> LineLabelDataSet { get; set; }

        [JsonPropertyName("LineLabelTable")]
        public IEnumerable<JsonEventTable> LineLabelTable { get; set; }


        [JsonPropertyName("LineNumber")]
        public IEnumerable<JsonEventValue> LineNumber { get; set; }

        [JsonPropertyName("LineNumberDataSet")]
        public IEnumerable<JsonEventDataSet> LineNumberDataSet { get; set; }

        [JsonPropertyName("LineNumberTable")]
        public IEnumerable<JsonEventTable> LineNumberTable { get; set; }


        [JsonPropertyName("LoadCount")]
        public IEnumerable<JsonEventValue> LoadCount { get; set; }

        [JsonPropertyName("LoadCountDataSet")]
        public IEnumerable<JsonEventDataSet> LoadCountDataSet { get; set; }

        [JsonPropertyName("LoadCountTable")]
        public IEnumerable<JsonEventTable> LoadCountTable { get; set; }


        [JsonPropertyName("LockState")]
        public IEnumerable<JsonEventValue> LockState { get; set; }

        [JsonPropertyName("LockStateDataSet")]
        public IEnumerable<JsonEventDataSet> LockStateDataSet { get; set; }

        [JsonPropertyName("LockStateTable")]
        public IEnumerable<JsonEventTable> LockStateTable { get; set; }


        [JsonPropertyName("MaintenanceList")]
        public IEnumerable<JsonEventValue> MaintenanceList { get; set; }

        [JsonPropertyName("MaintenanceListDataSet")]
        public IEnumerable<JsonEventDataSet> MaintenanceListDataSet { get; set; }

        [JsonPropertyName("MaintenanceListTable")]
        public IEnumerable<JsonEventTable> MaintenanceListTable { get; set; }


        [JsonPropertyName("Material")]
        public IEnumerable<JsonEventValue> Material { get; set; }

        [JsonPropertyName("MaterialDataSet")]
        public IEnumerable<JsonEventDataSet> MaterialDataSet { get; set; }

        [JsonPropertyName("MaterialTable")]
        public IEnumerable<JsonEventTable> MaterialTable { get; set; }


        [JsonPropertyName("MaterialLayer")]
        public IEnumerable<JsonEventValue> MaterialLayer { get; set; }

        [JsonPropertyName("MaterialLayerDataSet")]
        public IEnumerable<JsonEventDataSet> MaterialLayerDataSet { get; set; }

        [JsonPropertyName("MaterialLayerTable")]
        public IEnumerable<JsonEventTable> MaterialLayerTable { get; set; }


        [JsonPropertyName("MeasurementType")]
        public IEnumerable<JsonEventValue> MeasurementType { get; set; }

        [JsonPropertyName("MeasurementTypeDataSet")]
        public IEnumerable<JsonEventDataSet> MeasurementTypeDataSet { get; set; }

        [JsonPropertyName("MeasurementTypeTable")]
        public IEnumerable<JsonEventTable> MeasurementTypeTable { get; set; }


        [JsonPropertyName("MeasurementUnits")]
        public IEnumerable<JsonEventValue> MeasurementUnits { get; set; }

        [JsonPropertyName("MeasurementUnitsDataSet")]
        public IEnumerable<JsonEventDataSet> MeasurementUnitsDataSet { get; set; }

        [JsonPropertyName("MeasurementUnitsTable")]
        public IEnumerable<JsonEventTable> MeasurementUnitsTable { get; set; }


        [JsonPropertyName("MeasurementValue")]
        public IEnumerable<JsonEventValue> MeasurementValue { get; set; }

        [JsonPropertyName("MeasurementValueDataSet")]
        public IEnumerable<JsonEventDataSet> MeasurementValueDataSet { get; set; }

        [JsonPropertyName("MeasurementValueTable")]
        public IEnumerable<JsonEventTable> MeasurementValueTable { get; set; }


        [JsonPropertyName("Message")]
        public IEnumerable<JsonEventValue> Message { get; set; }

        [JsonPropertyName("MessageDataSet")]
        public IEnumerable<JsonEventDataSet> MessageDataSet { get; set; }

        [JsonPropertyName("MessageTable")]
        public IEnumerable<JsonEventTable> MessageTable { get; set; }


        [JsonPropertyName("MTConnectVersion")]
        public IEnumerable<JsonEventValue> MTConnectVersion { get; set; }

        [JsonPropertyName("MTConnectVersionDataSet")]
        public IEnumerable<JsonEventDataSet> MTConnectVersionDataSet { get; set; }

        [JsonPropertyName("MTConnectVersionTable")]
        public IEnumerable<JsonEventTable> MTConnectVersionTable { get; set; }


        [JsonPropertyName("Network")]
        public IEnumerable<JsonEventValue> Network { get; set; }

        [JsonPropertyName("NetworkDataSet")]
        public IEnumerable<JsonEventDataSet> NetworkDataSet { get; set; }

        [JsonPropertyName("NetworkTable")]
        public IEnumerable<JsonEventTable> NetworkTable { get; set; }


        [JsonPropertyName("NetworkPort")]
        public IEnumerable<JsonEventValue> NetworkPort { get; set; }

        [JsonPropertyName("NetworkPortDataSet")]
        public IEnumerable<JsonEventDataSet> NetworkPortDataSet { get; set; }

        [JsonPropertyName("NetworkPortTable")]
        public IEnumerable<JsonEventTable> NetworkPortTable { get; set; }


        [JsonPropertyName("OperatingMode")]
        public IEnumerable<JsonEventValue> OperatingMode { get; set; }

        [JsonPropertyName("OperatingModeDataSet")]
        public IEnumerable<JsonEventDataSet> OperatingModeDataSet { get; set; }

        [JsonPropertyName("OperatingModeTable")]
        public IEnumerable<JsonEventTable> OperatingModeTable { get; set; }


        [JsonPropertyName("OperatingSystem")]
        public IEnumerable<JsonEventValue> OperatingSystem { get; set; }

        [JsonPropertyName("OperatingSystemDataSet")]
        public IEnumerable<JsonEventDataSet> OperatingSystemDataSet { get; set; }

        [JsonPropertyName("OperatingSystemTable")]
        public IEnumerable<JsonEventTable> OperatingSystemTable { get; set; }


        [JsonPropertyName("OperatorId")]
        public IEnumerable<JsonEventValue> OperatorId { get; set; }

        [JsonPropertyName("OperatorIdDataSet")]
        public IEnumerable<JsonEventDataSet> OperatorIdDataSet { get; set; }

        [JsonPropertyName("OperatorIdTable")]
        public IEnumerable<JsonEventTable> OperatorIdTable { get; set; }


        [JsonPropertyName("PalletId")]
        public IEnumerable<JsonEventValue> PalletId { get; set; }

        [JsonPropertyName("PalletIdDataSet")]
        public IEnumerable<JsonEventDataSet> PalletIdDataSet { get; set; }

        [JsonPropertyName("PalletIdTable")]
        public IEnumerable<JsonEventTable> PalletIdTable { get; set; }


        [JsonPropertyName("PartCount")]
        public IEnumerable<JsonEventValue> PartCount { get; set; }

        [JsonPropertyName("PartCountDataSet")]
        public IEnumerable<JsonEventDataSet> PartCountDataSet { get; set; }

        [JsonPropertyName("PartCountTable")]
        public IEnumerable<JsonEventTable> PartCountTable { get; set; }


        [JsonPropertyName("PartCountType")]
        public IEnumerable<JsonEventValue> PartCountType { get; set; }

        [JsonPropertyName("PartCountTypeDataSet")]
        public IEnumerable<JsonEventDataSet> PartCountTypeDataSet { get; set; }

        [JsonPropertyName("PartCountTypeTable")]
        public IEnumerable<JsonEventTable> PartCountTypeTable { get; set; }


        [JsonPropertyName("PartDetect")]
        public IEnumerable<JsonEventValue> PartDetect { get; set; }

        [JsonPropertyName("PartDetectDataSet")]
        public IEnumerable<JsonEventDataSet> PartDetectDataSet { get; set; }

        [JsonPropertyName("PartDetectTable")]
        public IEnumerable<JsonEventTable> PartDetectTable { get; set; }


        [JsonPropertyName("PartGroupId")]
        public IEnumerable<JsonEventValue> PartGroupId { get; set; }

        [JsonPropertyName("PartGroupIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartGroupIdDataSet { get; set; }

        [JsonPropertyName("PartGroupIdTable")]
        public IEnumerable<JsonEventTable> PartGroupIdTable { get; set; }


        [JsonPropertyName("PartId")]
        public IEnumerable<JsonEventValue> PartId { get; set; }

        [JsonPropertyName("PartIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartIdDataSet { get; set; }

        [JsonPropertyName("PartIdTable")]
        public IEnumerable<JsonEventTable> PartIdTable { get; set; }


        [JsonPropertyName("PartKindId")]
        public IEnumerable<JsonEventValue> PartKindId { get; set; }

        [JsonPropertyName("PartKindIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartKindIdDataSet { get; set; }

        [JsonPropertyName("PartKindIdTable")]
        public IEnumerable<JsonEventTable> PartKindIdTable { get; set; }


        [JsonPropertyName("PartNumber")]
        public IEnumerable<JsonEventValue> PartNumber { get; set; }

        [JsonPropertyName("PartNumberDataSet")]
        public IEnumerable<JsonEventDataSet> PartNumberDataSet { get; set; }

        [JsonPropertyName("PartNumberTable")]
        public IEnumerable<JsonEventTable> PartNumberTable { get; set; }


        [JsonPropertyName("PartProcessingState")]
        public IEnumerable<JsonEventValue> PartProcessingState { get; set; }

        [JsonPropertyName("PartProcessingStateDataSet")]
        public IEnumerable<JsonEventDataSet> PartProcessingStateDataSet { get; set; }

        [JsonPropertyName("PartProcessingStateTable")]
        public IEnumerable<JsonEventTable> PartProcessingStateTable { get; set; }


        [JsonPropertyName("PartStatus")]
        public IEnumerable<JsonEventValue> PartStatus { get; set; }

        [JsonPropertyName("PartStatusDataSet")]
        public IEnumerable<JsonEventDataSet> PartStatusDataSet { get; set; }

        [JsonPropertyName("PartStatusTable")]
        public IEnumerable<JsonEventTable> PartStatusTable { get; set; }


        [JsonPropertyName("PartUniqueId")]
        public IEnumerable<JsonEventValue> PartUniqueId { get; set; }

        [JsonPropertyName("PartUniqueIdDataSet")]
        public IEnumerable<JsonEventDataSet> PartUniqueIdDataSet { get; set; }

        [JsonPropertyName("PartUniqueIdTable")]
        public IEnumerable<JsonEventTable> PartUniqueIdTable { get; set; }


        [JsonPropertyName("PathFeedrateOverride")]
        public IEnumerable<JsonEventValue> PathFeedrateOverride { get; set; }

        [JsonPropertyName("PathFeedrateOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> PathFeedrateOverrideDataSet { get; set; }

        [JsonPropertyName("PathFeedrateOverrideTable")]
        public IEnumerable<JsonEventTable> PathFeedrateOverrideTable { get; set; }


        [JsonPropertyName("PathMode")]
        public IEnumerable<JsonEventValue> PathMode { get; set; }

        [JsonPropertyName("PathModeDataSet")]
        public IEnumerable<JsonEventDataSet> PathModeDataSet { get; set; }

        [JsonPropertyName("PathModeTable")]
        public IEnumerable<JsonEventTable> PathModeTable { get; set; }


        [JsonPropertyName("PowerState")]
        public IEnumerable<JsonEventValue> PowerState { get; set; }

        [JsonPropertyName("PowerStateDataSet")]
        public IEnumerable<JsonEventDataSet> PowerStateDataSet { get; set; }

        [JsonPropertyName("PowerStateTable")]
        public IEnumerable<JsonEventTable> PowerStateTable { get; set; }


        [JsonPropertyName("PowerStatus")]
        public IEnumerable<JsonEventValue> PowerStatus { get; set; }

        [JsonPropertyName("PowerStatusDataSet")]
        public IEnumerable<JsonEventDataSet> PowerStatusDataSet { get; set; }

        [JsonPropertyName("PowerStatusTable")]
        public IEnumerable<JsonEventTable> PowerStatusTable { get; set; }


        [JsonPropertyName("ProcessAggregateId")]
        public IEnumerable<JsonEventValue> ProcessAggregateId { get; set; }

        [JsonPropertyName("ProcessAggregateIdDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessAggregateIdDataSet { get; set; }

        [JsonPropertyName("ProcessAggregateIdTable")]
        public IEnumerable<JsonEventTable> ProcessAggregateIdTable { get; set; }


        [JsonPropertyName("ProcessKindId")]
        public IEnumerable<JsonEventValue> ProcessKindId { get; set; }

        [JsonPropertyName("ProcessKindIdDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessKindIdDataSet { get; set; }

        [JsonPropertyName("ProcessKindIdTable")]
        public IEnumerable<JsonEventTable> ProcessKindIdTable { get; set; }


        [JsonPropertyName("ProcessOccurrenceId")]
        public IEnumerable<JsonEventValue> ProcessOccurrenceId { get; set; }

        [JsonPropertyName("ProcessOccurrenceIdDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessOccurrenceIdDataSet { get; set; }

        [JsonPropertyName("ProcessOccurrenceIdTable")]
        public IEnumerable<JsonEventTable> ProcessOccurrenceIdTable { get; set; }


        [JsonPropertyName("ProcessState")]
        public IEnumerable<JsonEventValue> ProcessState { get; set; }

        [JsonPropertyName("ProcessStateDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessStateDataSet { get; set; }

        [JsonPropertyName("ProcessStateTable")]
        public IEnumerable<JsonEventTable> ProcessStateTable { get; set; }


        [JsonPropertyName("ProcessTime")]
        public IEnumerable<JsonEventValue> ProcessTime { get; set; }

        [JsonPropertyName("ProcessTimeDataSet")]
        public IEnumerable<JsonEventDataSet> ProcessTimeDataSet { get; set; }

        [JsonPropertyName("ProcessTimeTable")]
        public IEnumerable<JsonEventTable> ProcessTimeTable { get; set; }


        [JsonPropertyName("Program")]
        public IEnumerable<JsonEventValue> Program { get; set; }

        [JsonPropertyName("ProgramDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramDataSet { get; set; }

        [JsonPropertyName("ProgramTable")]
        public IEnumerable<JsonEventTable> ProgramTable { get; set; }


        [JsonPropertyName("ProgramComment")]
        public IEnumerable<JsonEventValue> ProgramComment { get; set; }

        [JsonPropertyName("ProgramCommentDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramCommentDataSet { get; set; }

        [JsonPropertyName("ProgramCommentTable")]
        public IEnumerable<JsonEventTable> ProgramCommentTable { get; set; }


        [JsonPropertyName("ProgramEdit")]
        public IEnumerable<JsonEventValue> ProgramEdit { get; set; }

        [JsonPropertyName("ProgramEditDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramEditDataSet { get; set; }

        [JsonPropertyName("ProgramEditTable")]
        public IEnumerable<JsonEventTable> ProgramEditTable { get; set; }


        [JsonPropertyName("ProgramEditName")]
        public IEnumerable<JsonEventValue> ProgramEditName { get; set; }

        [JsonPropertyName("ProgramEditNameDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramEditNameDataSet { get; set; }

        [JsonPropertyName("ProgramEditNameTable")]
        public IEnumerable<JsonEventTable> ProgramEditNameTable { get; set; }


        [JsonPropertyName("ProgramHeader")]
        public IEnumerable<JsonEventValue> ProgramHeader { get; set; }

        [JsonPropertyName("ProgramHeaderDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramHeaderDataSet { get; set; }

        [JsonPropertyName("ProgramHeaderTable")]
        public IEnumerable<JsonEventTable> ProgramHeaderTable { get; set; }


        [JsonPropertyName("ProgramLocation")]
        public IEnumerable<JsonEventValue> ProgramLocation { get; set; }

        [JsonPropertyName("ProgramLocationDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramLocationDataSet { get; set; }

        [JsonPropertyName("ProgramLocationTable")]
        public IEnumerable<JsonEventTable> ProgramLocationTable { get; set; }


        [JsonPropertyName("ProgramLocationType")]
        public IEnumerable<JsonEventValue> ProgramLocationType { get; set; }

        [JsonPropertyName("ProgramLocationTypeDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramLocationTypeDataSet { get; set; }

        [JsonPropertyName("ProgramLocationTypeTable")]
        public IEnumerable<JsonEventTable> ProgramLocationTypeTable { get; set; }


        [JsonPropertyName("ProgramNestLevel")]
        public IEnumerable<JsonEventValue> ProgramNestLevel { get; set; }

        [JsonPropertyName("ProgramNestLevelDataSet")]
        public IEnumerable<JsonEventDataSet> ProgramNestLevelDataSet { get; set; }

        [JsonPropertyName("ProgramNestLevelTable")]
        public IEnumerable<JsonEventTable> ProgramNestLevelTable { get; set; }


        [JsonPropertyName("RotaryMode")]
        public IEnumerable<JsonEventValue> RotaryMode { get; set; }

        [JsonPropertyName("RotaryModeDataSet")]
        public IEnumerable<JsonEventDataSet> RotaryModeDataSet { get; set; }

        [JsonPropertyName("RotaryModeTable")]
        public IEnumerable<JsonEventTable> RotaryModeTable { get; set; }


        [JsonPropertyName("RotaryVelocityOverride")]
        public IEnumerable<JsonEventValue> RotaryVelocityOverride { get; set; }

        [JsonPropertyName("RotaryVelocityOverrideDataSet")]
        public IEnumerable<JsonEventDataSet> RotaryVelocityOverrideDataSet { get; set; }

        [JsonPropertyName("RotaryVelocityOverrideTable")]
        public IEnumerable<JsonEventTable> RotaryVelocityOverrideTable { get; set; }


        [JsonPropertyName("Rotation")]
        public IEnumerable<JsonEventValue> Rotation { get; set; }

        [JsonPropertyName("RotationDataSet")]
        public IEnumerable<JsonEventDataSet> RotationDataSet { get; set; }

        [JsonPropertyName("RotationTable")]
        public IEnumerable<JsonEventTable> RotationTable { get; set; }


        [JsonPropertyName("SensorAttachment")]
        public IEnumerable<JsonEventValue> SensorAttachment { get; set; }

        [JsonPropertyName("SensorAttachmentDataSet")]
        public IEnumerable<JsonEventDataSet> SensorAttachmentDataSet { get; set; }

        [JsonPropertyName("SensorAttachmentTable")]
        public IEnumerable<JsonEventTable> SensorAttachmentTable { get; set; }


        [JsonPropertyName("SensorState")]
        public IEnumerable<JsonEventValue> SensorState { get; set; }

        [JsonPropertyName("SensorStateDataSet")]
        public IEnumerable<JsonEventDataSet> SensorStateDataSet { get; set; }

        [JsonPropertyName("SensorStateTable")]
        public IEnumerable<JsonEventTable> SensorStateTable { get; set; }


        [JsonPropertyName("SerialNumber")]
        public IEnumerable<JsonEventValue> SerialNumber { get; set; }

        [JsonPropertyName("SerialNumberDataSet")]
        public IEnumerable<JsonEventDataSet> SerialNumberDataSet { get; set; }

        [JsonPropertyName("SerialNumberTable")]
        public IEnumerable<JsonEventTable> SerialNumberTable { get; set; }


        [JsonPropertyName("SpecificationLimit")]
        public IEnumerable<JsonEventValue> SpecificationLimit { get; set; }

        [JsonPropertyName("SpecificationLimitDataSet")]
        public IEnumerable<JsonEventDataSet> SpecificationLimitDataSet { get; set; }

        [JsonPropertyName("SpecificationLimitTable")]
        public IEnumerable<JsonEventTable> SpecificationLimitTable { get; set; }


        [JsonPropertyName("SpindleInterlock")]
        public IEnumerable<JsonEventValue> SpindleInterlock { get; set; }

        [JsonPropertyName("SpindleInterlockDataSet")]
        public IEnumerable<JsonEventDataSet> SpindleInterlockDataSet { get; set; }

        [JsonPropertyName("SpindleInterlockTable")]
        public IEnumerable<JsonEventTable> SpindleInterlockTable { get; set; }


        [JsonPropertyName("ToolAssetId")]
        public IEnumerable<JsonEventValue> ToolAssetId { get; set; }

        [JsonPropertyName("ToolAssetIdDataSet")]
        public IEnumerable<JsonEventDataSet> ToolAssetIdDataSet { get; set; }

        [JsonPropertyName("ToolAssetIdTable")]
        public IEnumerable<JsonEventTable> ToolAssetIdTable { get; set; }


        [JsonPropertyName("ToolGroup")]
        public IEnumerable<JsonEventValue> ToolGroup { get; set; }

        [JsonPropertyName("ToolGroupDataSet")]
        public IEnumerable<JsonEventDataSet> ToolGroupDataSet { get; set; }

        [JsonPropertyName("ToolGroupTable")]
        public IEnumerable<JsonEventTable> ToolGroupTable { get; set; }


        [JsonPropertyName("ToolId")]
        public IEnumerable<JsonEventValue> ToolId { get; set; }

        [JsonPropertyName("ToolIdDataSet")]
        public IEnumerable<JsonEventDataSet> ToolIdDataSet { get; set; }

        [JsonPropertyName("ToolIdTable")]
        public IEnumerable<JsonEventTable> ToolIdTable { get; set; }


        [JsonPropertyName("ToolNumber")]
        public IEnumerable<JsonEventValue> ToolNumber { get; set; }

        [JsonPropertyName("ToolNumberDataSet")]
        public IEnumerable<JsonEventDataSet> ToolNumberDataSet { get; set; }

        [JsonPropertyName("ToolNumberTable")]
        public IEnumerable<JsonEventTable> ToolNumberTable { get; set; }


        [JsonPropertyName("ToolOffset")]
        public IEnumerable<JsonEventValue> ToolOffset { get; set; }

        [JsonPropertyName("ToolOffsetDataSet")]
        public IEnumerable<JsonEventDataSet> ToolOffsetDataSet { get; set; }

        [JsonPropertyName("ToolOffsetTable")]
        public IEnumerable<JsonEventTable> ToolOffsetTable { get; set; }


        [JsonPropertyName("ToolOffsets")]
        public IEnumerable<JsonEventValue> ToolOffsets { get; set; }

        [JsonPropertyName("ToolOffsetsDataSet")]
        public IEnumerable<JsonEventDataSet> ToolOffsetsDataSet { get; set; }

        [JsonPropertyName("ToolOffsetsTable")]
        public IEnumerable<JsonEventTable> ToolOffsetsTable { get; set; }


        [JsonPropertyName("TransferCount")]
        public IEnumerable<JsonEventValue> TransferCount { get; set; }

        [JsonPropertyName("TransferCountDataSet")]
        public IEnumerable<JsonEventDataSet> TransferCountDataSet { get; set; }

        [JsonPropertyName("TransferCountTable")]
        public IEnumerable<JsonEventTable> TransferCountTable { get; set; }


        [JsonPropertyName("Translation")]
        public IEnumerable<JsonEventValue> Translation { get; set; }

        [JsonPropertyName("TranslationDataSet")]
        public IEnumerable<JsonEventDataSet> TranslationDataSet { get; set; }

        [JsonPropertyName("TranslationTable")]
        public IEnumerable<JsonEventTable> TranslationTable { get; set; }


        [JsonPropertyName("Uncertainty")]
        public IEnumerable<JsonEventValue> Uncertainty { get; set; }

        [JsonPropertyName("UncertaintyDataSet")]
        public IEnumerable<JsonEventDataSet> UncertaintyDataSet { get; set; }

        [JsonPropertyName("UncertaintyTable")]
        public IEnumerable<JsonEventTable> UncertaintyTable { get; set; }


        [JsonPropertyName("UncertaintyType")]
        public IEnumerable<JsonEventValue> UncertaintyType { get; set; }

        [JsonPropertyName("UncertaintyTypeDataSet")]
        public IEnumerable<JsonEventDataSet> UncertaintyTypeDataSet { get; set; }

        [JsonPropertyName("UncertaintyTypeTable")]
        public IEnumerable<JsonEventTable> UncertaintyTypeTable { get; set; }


        [JsonPropertyName("UnloadCount")]
        public IEnumerable<JsonEventValue> UnloadCount { get; set; }

        [JsonPropertyName("UnloadCountDataSet")]
        public IEnumerable<JsonEventDataSet> UnloadCountDataSet { get; set; }

        [JsonPropertyName("UnloadCountTable")]
        public IEnumerable<JsonEventTable> UnloadCountTable { get; set; }


        [JsonPropertyName("User")]
        public IEnumerable<JsonEventValue> User { get; set; }

        [JsonPropertyName("UserDataSet")]
        public IEnumerable<JsonEventDataSet> UserDataSet { get; set; }

        [JsonPropertyName("UserTable")]
        public IEnumerable<JsonEventTable> UserTable { get; set; }


        [JsonPropertyName("ValveState")]
        public IEnumerable<JsonEventValue> ValveState { get; set; }

        [JsonPropertyName("ValveStateDataSet")]
        public IEnumerable<JsonEventDataSet> ValveStateDataSet { get; set; }

        [JsonPropertyName("ValveStateTable")]
        public IEnumerable<JsonEventTable> ValveStateTable { get; set; }


        [JsonPropertyName("Variable")]
        public IEnumerable<JsonEventValue> Variable { get; set; }

        [JsonPropertyName("VariableDataSet")]
        public IEnumerable<JsonEventDataSet> VariableDataSet { get; set; }

        [JsonPropertyName("VariableTable")]
        public IEnumerable<JsonEventTable> VariableTable { get; set; }


        [JsonPropertyName("WaitState")]
        public IEnumerable<JsonEventValue> WaitState { get; set; }

        [JsonPropertyName("WaitStateDataSet")]
        public IEnumerable<JsonEventDataSet> WaitStateDataSet { get; set; }

        [JsonPropertyName("WaitStateTable")]
        public IEnumerable<JsonEventTable> WaitStateTable { get; set; }


        [JsonPropertyName("Wire")]
        public IEnumerable<JsonEventValue> Wire { get; set; }

        [JsonPropertyName("WireDataSet")]
        public IEnumerable<JsonEventDataSet> WireDataSet { get; set; }

        [JsonPropertyName("WireTable")]
        public IEnumerable<JsonEventTable> WireTable { get; set; }


        [JsonPropertyName("WorkOffset")]
        public IEnumerable<JsonEventValue> WorkOffset { get; set; }

        [JsonPropertyName("WorkOffsetDataSet")]
        public IEnumerable<JsonEventDataSet> WorkOffsetDataSet { get; set; }

        [JsonPropertyName("WorkOffsetTable")]
        public IEnumerable<JsonEventTable> WorkOffsetTable { get; set; }


        [JsonPropertyName("WorkOffsets")]
        public IEnumerable<JsonEventValue> WorkOffsets { get; set; }

        [JsonPropertyName("WorkOffsetsDataSet")]
        public IEnumerable<JsonEventDataSet> WorkOffsetsDataSet { get; set; }

        [JsonPropertyName("WorkOffsetsTable")]
        public IEnumerable<JsonEventTable> WorkOffsetsTable { get; set; }


        [JsonPropertyName("WorkholdingId")]
        public IEnumerable<JsonEventValue> WorkholdingId { get; set; }

        [JsonPropertyName("WorkholdingIdDataSet")]
        public IEnumerable<JsonEventDataSet> WorkholdingIdDataSet { get; set; }

        [JsonPropertyName("WorkholdingIdTable")]
        public IEnumerable<JsonEventTable> WorkholdingIdTable { get; set; }



        public JsonEvents() { }

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