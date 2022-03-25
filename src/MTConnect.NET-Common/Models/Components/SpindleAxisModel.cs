// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Events;
using MTConnect.Devices.Samples;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;
using MTConnect.Models.Compositions;
using MTConnect.Models.DataItems;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// The axis is functioning as a spindle. Generally, it is configured to rotate at a defined speed.
    /// </summary>
    public class SpindleAxisModel : AxisModel, ISpindleAxisModel
    {
        public const string TypeId = "Rotary";


        /// <summary>
        /// The measurement of the rotational speed of a rotary axis.
        /// </summary>
        public RotaryVelocityModel RotaryVelocity
        {
            get => GetRotaryVelocity();
            set => SetRotaryVelocity(value);
        }

        /// <summary>
        /// The value of a command issued to adjust the programmed velocity for a Rotary type axis.
        /// </summary>
        public RotaryVelocityOverrideValue RotaryVelocityOverride
        {
            get => (RotaryVelocityOverrideValue)GetEventValue(RotaryVelocityOverrideDataItem.NameId);
            set => AddDataItem(new RotaryVelocityOverrideDataItem(Id), value);
        }

        /// <summary>
        /// The direction of motion.
        /// </summary>
        public RotaryDirection Direction
        {
            get => GetDataItemValue<RotaryDirection>(DataItem.CreateId(Id, DirectionDataItem.NameId, DirectionDataItem.GetSubTypeId(DirectionDataItem.SubTypes.ROTARY)));
            set => AddDataItem(new DirectionDataItem(Id, DirectionDataItem.SubTypes.ROTARY), value);
        }

        /// <summary>
        /// The measurement of angular position.
        /// </summary>
        public AngleModel Angle
        {
            get => GetAngle();
            set => SetAngle(value);
        }

        /// <summary>
        /// The current operating mode for a Rotary type axis.
        /// </summary>
        public RotaryMode Mode
        {
            get => GetDataItemValue<RotaryMode>(DataItem.CreateId(Id, RotaryModeDataItem.NameId));
            set => AddDataItem(new RotaryModeDataItem(Id), value);
        }

        /// <summary>
        /// A mechanism that holds a part, stock material, or any other item in place.
        /// </summary>
        public ChuckModel Chuck
        {
            get => (ChuckModel)CompositionModels?.FirstOrDefault(o => o.Type == ChuckModel.TypeId);
            set => AddCompositionModel(value);
        }


        public SpindleAxisModel() 
        {
            Type = TypeId;
        }

        public SpindleAxisModel(string deviceId)
        {
            Id = deviceId;
            Type = TypeId;
            Name = deviceId;
        }


        public virtual ChuckModel AddChuck(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new ChuckModel
                {
                    Id = CreateId(Id, name),
                    Name = name
                };

                AddCompositionModel(model);
                return model;
            }

            return null;
        }


        private RotaryVelocityModel GetRotaryVelocity()
        {
            var x = new RotaryVelocityModel();

            // Actual Position
            x.Actual = (RotaryVelocityValue)GetSampleValue(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.ACTUAL));

            // Commanded Position
            x.Commanded = (RotaryVelocityValue)GetSampleValue(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.COMMANDED));

            // Programmed Position
            x.Programmed = (RotaryVelocityValue)GetSampleValue(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.PROGRAMMED));

            return x;

        }

        private void SetRotaryVelocity(RotaryVelocityModel angle)
        {
            if (angle != null)
            {
                // Actual Position
                AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.ACTUAL), angle?.Actual);

                // Commanded Position
                AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.COMMANDED), angle?.Commanded);

                // Programmed Position
                AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.PROGRAMMED), angle?.Programmed);
            }
        }


        private AngleModel GetAngle()
        {
            var x = new AngleModel();

            // Actual Position
            x.Actual = (AngleValue)GetSampleValue(AngleDataItem.TypeId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.ACTUAL));

            // Commanded Position
            x.Commanded = (AngleValue)GetSampleValue(AngleDataItem.TypeId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.COMMANDED));
            return x;

        }

        private void SetAngle(AngleModel angle)
        {
            if (angle != null)
            {
                // Actual Position
                AddDataItem(new AngleDataItem(Id, AngleDataItem.SubTypes.ACTUAL), angle?.Actual);

                // Commanded Position
                AddDataItem(new AngleDataItem(Id, AngleDataItem.SubTypes.COMMANDED), angle?.Commanded);
            }
        }


        //private ChuckModel GetChuck()
        //{
        //    var x = new ChuckModel();

        //    // Chuck State
        //    x.ChuckState = GetDataItemValue<ChuckState>(DataItem.CreateId(Id, ChuckStateDataItem.NameId));

        //    // Chuck Interlock
        //    x.ManualUnclampInterlock = GetDataItemValue<ChuckInterlock>(DataItem.CreateId(Id, ChuckInterlockDataItem.NameId));
        //    return x;

        //}

        //private void SetChuck(ChuckModel chuck)
        //{
        //    if (chuck != null)
        //    {
        //        // Chuck State
        //        AddDataItem(new ChuckStateDataItem(Id), chuck?.ChuckState);

        //        // Chuck Interlock
        //        AddDataItem(new ChuckInterlockDataItem(Id), chuck?.ManualUnclampInterlock);
        //    }
        //}
    }
}
