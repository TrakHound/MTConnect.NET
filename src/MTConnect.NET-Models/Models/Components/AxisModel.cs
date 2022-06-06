// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Models.Compositions;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    public class AxisModel : ComponentModel, IAxisModel
    {
        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        public AxisState AxisState
        {
            get => DataItemManager.GetDataItemValue<AxisState>(DataItem.CreateId(Id, Devices.DataItems.Events.AxisStateDataItem.NameId));
            set => DataItemManager.AddDataItem(new AxisStateDataItem(Id), value);
        }
        public IDataItemModel AxisStateDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.AxisStateDataItem.NameId);

        /// <summary>
        /// An indicator of the state of the axis lockout function when power has been removed and the axis is allowed to move freely.
        /// </summary>
        public AxisInterlock AxisInterlock
        {
            get => DataItemManager.GetDataItemValue<AxisInterlock>(DataItem.CreateId(Id, Devices.DataItems.Events.AxisInterlockDataItem.NameId));
            set => DataItemManager.AddDataItem(new AxisInterlockDataItem(Id), value);
        }
        public IDataItemModel AxisInterlockDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.AxisInterlockDataItem.NameId);

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        public IMotorModel Motor
        {
            get => ComponentManager.GetCompositionModel<MotorModel>(typeof(MotorComposition));
            set => ComponentManager.AddCompositionModel((MotorModel)value);
        }

        /// <summary>
        /// An electronic component or circuit for amplifying power, electric current, or voltage.
        /// </summary>
        public IAmplifierModel Amplifier
        {
            get => ComponentManager.GetCompositionModel<AmplifierModel>(typeof(AmplifierComposition));
            set => ComponentManager.AddCompositionModel((AmplifierModel)value);
        }

        /// <summary>
        /// A mechanism to measure position.
        /// </summary>
        public IEncoderModel Encoder
        {
            get => ComponentManager.GetCompositionModel<EncoderModel>(typeof(EncoderComposition));
            set => ComponentManager.AddCompositionModel((EncoderModel)value);
        }

        /// <summary>
        /// A mechanical structure for transforming rotary motion into linear motion.
        /// </summary>
        public IBallscrewModel Ballscrew
        {
            get => ComponentManager.GetCompositionModel<BallscrewModel>(typeof(BallscrewComposition));
            set => ComponentManager.AddCompositionModel((BallscrewModel)value);
        }

        /// <summary>
        /// An endless flexible band used to transmit motion for a piece of equipment or to convey materials and objects.
        /// </summary>
        public IBeltModel Belt
        {
            get => ComponentManager.GetCompositionModel<BeltModel>(typeof(BeltComposition));
            set => ComponentManager.AddCompositionModel((BeltModel)value);
        }

        /// <summary>
        /// A mechanism for slowing or stopping a moving object by the absorption or
        /// transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.
        /// </summary>
        public IBrakeModel Brake
        {
            get => ComponentManager.GetCompositionModel<BrakeModel>(typeof(BrakeComposition));
            set => ComponentManager.AddCompositionModel((BrakeModel)value);
        }

        /// <summary>
        /// A mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
        /// </summary>
        public IPulleyModel Pulley
        {
            get => ComponentManager.GetCompositionModel<PulleyModel>(typeof(PulleyComposition));
            set => ComponentManager.AddCompositionModel((PulleyModel)value);
        }
    }
}
