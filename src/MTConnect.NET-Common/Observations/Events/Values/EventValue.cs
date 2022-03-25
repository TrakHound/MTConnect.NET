// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Events;
using MTConnect.Observations;

namespace MTConnect.Observations.Events.Values
{
    public class EventValue : EventValueObservation
    {
        public object Value { get; set; }


        protected EventValue() { }

        public EventValue(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value?.ToString();
        }


        public static string GetDescriptionText(string type, string subType, string value)
        {
            switch (type.ToUnderscoreUpper())
            {
                case ActuatorStateDataItem.TypeId: return ActuatorStateDescriptions.Get(value.ConvertEnum<ActuatorState>());
                case AvailabilityDataItem.TypeId: return AvailabilityDescriptions.Get(value.ConvertEnum<Availability>());
                case AxisCouplingDataItem.TypeId: return AxisCouplingDescriptions.Get(value.ConvertEnum<AxisCoupling>());
                case AxisInterlockDataItem.TypeId: return AxisInterlockDescriptions.Get(value.ConvertEnum<AxisInterlock>());
                case AxisStateDataItem.TypeId: return AxisStateDescriptions.Get(value.ConvertEnum<AxisState>());
                case ChuckInterlockDataItem.TypeId: return ChuckInterlockDescriptions.Get(value.ConvertEnum<ChuckInterlock>());
                case ChuckStateDataItem.TypeId: return ChuckStateDescriptions.Get(value.ConvertEnum<ChuckState>());
                case CompositionStateDataItem.TypeId:

                    switch (subType.ToUnderscoreUpper().ConvertEnum<CompositionStateDataItem.SubTypes>())
                    {
                        case CompositionStateDataItem.SubTypes.ACTION: return CompositionActionStateDescriptions.Get(value.ConvertEnum<CompositionActionState>());
                        case CompositionStateDataItem.SubTypes.LATERAL: return CompositionLateralStateDescriptions.Get(value.ConvertEnum<CompositionLateralState>());
                        case CompositionStateDataItem.SubTypes.MOTION: return CompositionMotionStateDescriptions.Get(value.ConvertEnum<CompositionMotionState>());
                        case CompositionStateDataItem.SubTypes.SWITCHED: return CompositionSwitchedStateDescriptions.Get(value.ConvertEnum<CompositionSwitchedState>());
                        case CompositionStateDataItem.SubTypes.VERTICAL: return CompositionVerticalStateDescriptions.Get(value.ConvertEnum<CompositionVerticalState>());
                    }
                    break;
                case ConnectionStatusDataItem.TypeId: return ConnectionStatusDescriptions.Get(value.ConvertEnum<ConnectionStatus>());
                case ControllerModeDataItem.TypeId: return ControllerModeDescriptions.Get(value.ConvertEnum<ControllerMode>());
                case ControllerModeOverrideDataItem.TypeId: return ControllerModeOverrideValueDescriptions.Get(value.ConvertEnum<ControllerModeOverrideValue>());
                case DirectionDataItem.TypeId:
                    switch (subType.ToUnderscoreUpper().ConvertEnum<DirectionDataItem.SubTypes>())
                    {
                        case DirectionDataItem.SubTypes.LINEAR: return LinearDirectionDescriptions.Get(value.ConvertEnum<LinearDirection>());
                        case DirectionDataItem.SubTypes.ROTARY: return RotaryDirectionDescriptions.Get(value.ConvertEnum<RotaryDirection>());
                    }
                    break;
                case DoorStateDataItem.TypeId: return DoorStateDescriptions.Get(value.ConvertEnum<DoorState>());
                case EmergencyStopDataItem.TypeId: return EmergencyStopDescriptions.Get(value.ConvertEnum<EmergencyStop>());
                case EndOfBarDataItem.TypeId: return EndOfBarDescriptions.Get(value.ConvertEnum<EndOfBar>());
                case EquipmentModeDataItem.TypeId: return EquipmentModeDescriptions.Get(value.ConvertEnum<EquipmentMode>());
                case ExecutionDataItem.TypeId: return ExecutionDescriptions.Get(value.ConvertEnum<Execution>());
                case FunctionalModeDataItem.TypeId: return FunctionalModeDescriptions.Get(value.ConvertEnum<FunctionalMode>());
                case InterfaceStateDataItem.TypeId: return InterfaceStateDescriptions.Get(value.ConvertEnum<InterfaceState>());
                case LockStateDataItem.TypeId: return LockStateDescriptions.Get(value.ConvertEnum<LockState>());
                case PartDetectDataItem.TypeId: return PartDetectDescriptions.Get(value.ConvertEnum<PartDetect>());
                case PartProcessingStateDataItem.TypeId: return PartProcessingStateDescriptions.Get(value.ConvertEnum<PartProcessingState>());
                case PartStatusDataItem.TypeId: return PartStatusDescriptions.Get(value.ConvertEnum<PartStatus>());
                case PathModeDataItem.TypeId: return PathModeDescriptions.Get(value.ConvertEnum<PathMode>());
                case PowerStateDataItem.TypeId: return PowerStateDescriptions.Get(value.ConvertEnum<PowerState>());
                case ProcessStateDataItem.TypeId: return ProcessStateDescriptions.Get(value.ConvertEnum<ProcessState>());
                case ProgramLocationTypeDataItem.TypeId: return ProgramLocationTypeDescriptions.Get(value.ConvertEnum<ProgramLocationType>());
                case RotaryModeDataItem.TypeId: return RotaryModeDescriptions.Get(value.ConvertEnum<RotaryMode>());
                case SpindleInterlockDataItem.TypeId: return SpindleInterlockDescriptions.Get(value.ConvertEnum<SpindleInterlock>());
                case ValveStateDataItem.TypeId: return ValveStateDescriptions.Get(value.ConvertEnum<ValveState>());
                case WaitStateDataItem.TypeId: return WaitStateDescriptions.Get(value.ConvertEnum<WaitState>());
            }

            return null;
        }
    }
}
