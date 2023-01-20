// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.Compositions;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    public interface IAxisModel : IComponentModel
    {
        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        AxisState AxisState { get; set; }
        IDataItemModel AxisStateDataItem { get; }

        /// <summary>
        /// An indicator of the state of the axis lockout function when power has been removed and the axis is allowed to move freely.
        /// </summary>
        AxisInterlock AxisInterlock { get; set; }
        IDataItemModel AxisInterlockDataItem { get; }

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        IMotorModel Motor { get; set; }

        /// <summary>
        /// An electronic component or circuit for amplifying power, electric current, or voltage.
        /// </summary>
        IAmplifierModel Amplifier { get; set; }

        /// <summary>
        /// A mechanism to measure position.
        /// </summary>
        IEncoderModel Encoder { get; set; }

        /// <summary>
        /// A mechanical structure for transforming rotary motion into linear motion.
        /// </summary>
        IBallscrewModel Ballscrew { get; set; }

        /// <summary>
        /// An endless flexible band used to transmit motion for a piece of equipment or to convey materials and objects.
        /// </summary>
        IBeltModel Belt { get; set; }

        /// <summary>
        /// A mechanism for slowing or stopping a moving object by the absorption or
        /// transfer of the energy of momentum, usually by means of friction, electrical force, or magnetic force.
        /// </summary>
        IBrakeModel Brake { get; set; }

        /// <summary>
        /// A mechanism or wheel that turns in a frame or block and serves to change the direction of or to transmit force.
        /// </summary>
        IPulleyModel Pulley { get; set; }


        //IMotorModel AddMotor(string name);

        //IAmplifierModel AddAmplifier(string name);

        //IEncoderModel AddEncoder(string name);

        //IBallscrewModel AddBallscrew(string name);

        //IBeltModel AddBelt(string name);

        //IBrakeModel AddBrake(string name);

        //IPulleyModel AddPulley(string name);
    }
}