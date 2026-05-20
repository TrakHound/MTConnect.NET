// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.DataItems;
using MTConnect.Interfaces;

namespace MTConnect.Observations.Events
{
    /// <summary>
    /// A simple EVENT category Observation that carries a single reported value.
    /// </summary>
    public class EventValue : EventValueObservation
    {
        /// <summary>
        /// The value reported by the Event.
        /// </summary>
        public object Value { get; set; }


        /// <summary>
        /// Initializes a new Event value with no reported value.
        /// </summary>
        protected EventValue() { }

        /// <summary>
        /// Initializes a new Event value with the specified reported value.
        /// </summary>
        /// <param name="value">The value reported by the Event.</param>
        public EventValue(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns the string representation of the reported value.
        /// </summary>
        /// <returns>The value's string form, or <c>null</c> when no value is set.</returns>
        public override string ToString()
        {
            return Value?.ToString();
        }


        /// <summary>
        /// Resolves the human-readable description for a controlled-vocabulary Event value based on its DataItem type and subtype.
        /// </summary>
        /// <param name="type">The DataItem type identifier (for example, <c>EXECUTION</c>).</param>
        /// <param name="subType">The DataItem subtype, used to disambiguate types such as DIRECTION and COMPOSITION_STATE.</param>
        /// <param name="value">The reported enumeration value to describe.</param>
        /// <returns>The matching description text, or <c>null</c> when the type or value has no controlled-vocabulary description.</returns>
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
                        case CompositionStateDataItem.SubTypes.ACTION: return CompositionStateActionDescriptions.Get(value.ConvertEnum<CompositionStateAction>());
                        case CompositionStateDataItem.SubTypes.LATERAL: return CompositionStateLateralDescriptions.Get(value.ConvertEnum<CompositionStateLateral>());
                        case CompositionStateDataItem.SubTypes.MOTION: return CompositionStateMotionDescriptions.Get(value.ConvertEnum<CompositionStateMotion>());
                        case CompositionStateDataItem.SubTypes.SWITCHED: return CompositionStateSwitchedDescriptions.Get(value.ConvertEnum<CompositionStateSwitched>());
                        case CompositionStateDataItem.SubTypes.VERTICAL: return CompositionStateVerticalDescriptions.Get(value.ConvertEnum<CompositionStateVertical>());
                    }
                    break;
                case ConnectionStatusDataItem.TypeId: return ConnectionStatusDescriptions.Get(value.ConvertEnum<ConnectionStatus>());
                case ControllerModeDataItem.TypeId: return ControllerModeDescriptions.Get(value.ConvertEnum<ControllerMode>());
                case ControllerModeOverrideDataItem.TypeId: return ControllerModeOverrideDescriptions.Get(value.ConvertEnum<ControllerModeOverride>());
                case DirectionDataItem.TypeId:
                    switch (subType.ToUnderscoreUpper().ConvertEnum<DirectionDataItem.SubTypes>())
                    {
                        case DirectionDataItem.SubTypes.LINEAR: return DirectionLinearDescriptions.Get(value.ConvertEnum<DirectionLinear>());
                        case DirectionDataItem.SubTypes.ROTARY: return DirectionRotaryDescriptions.Get(value.ConvertEnum<DirectionRotary>());
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