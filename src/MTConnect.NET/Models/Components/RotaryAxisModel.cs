// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.Events;
using MTConnect.Devices.Samples;
using MTConnect.Models.Compositions;
using MTConnect.Models.DataItems;
using MTConnect.Streams.Events;
using MTConnect.Streams.Samples;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// A Rotary axis represents rotation about a fixed axis.
    /// </summary>
    public class RotaryAxisModel : AxisModel, IRotaryAxisModel
    {
        /// <summary>
        /// The measurement of angular position.
        /// </summary>
        public AngleModel Angle
        {
            get => GetAngle();
            set => SetAngle(value);
        }

        /// <summary>
        /// The measurement of the rate of change of angular position.
        /// </summary>
        public AngularVelocityValue AngularVelocity
        {
            get => GetSampleValue<AngularVelocityValue>(Devices.Samples.AngularVelocityDataItem.NameId);
            set => AddDataItem(new AngularVelocityDataItem(Id), value);
        }
        public IDataItemModel AngularVelocityDataItem => GetDataItem(Devices.Samples.AngularVelocityDataItem.NameId);

        /// <summary>
        /// The positive rate of change of angular velocity
        /// </summary>
        public AngularAccelerationModel AngularAcceleration
        {
            get => GetAngularAcceleration();
            set => SetAngularAcceleration(value);
        }

        /// <summary>
        /// Negative rate of change of angular velocity
        /// </summary>
        public AngularDecelerationModel AngularDeceleration
        {
            get => GetAngularDeceleration();
            set => SetAngularDeceleration(value);
        }

        /// <summary>
        /// The direction of motion.
        /// </summary>
        public RotaryDirection RotaryDirection
        {
            get => GetDataItemValue<RotaryDirection>(DataItem.CreateId(Id, DirectionDataItem.NameId, DirectionDataItem.GetSubTypeId(DirectionDataItem.SubTypes.ROTARY)));
            set => AddDataItem(new DirectionDataItem(Id, DirectionDataItem.SubTypes.ROTARY), value);
        }
        public IDataItemModel RotaryDirectionDataItem => GetDataItem(DirectionDataItem.NameId, DirectionDataItem.GetSubTypeId(DirectionDataItem.SubTypes.ROTARY));

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
            get => GetEventValue<RotaryVelocityOverrideValue>(RotaryVelocityOverrideDataItem.NameId);
            set => AddDataItem(new RotaryVelocityOverrideDataItem(Id), value);
        }

        /// <summary>
        /// The current operating mode for a Rotary type axis.
        /// </summary>
        public RotaryMode RotaryMode
        {
            get => GetDataItemValue<RotaryMode>(DataItem.CreateId(Id, RotaryModeDataItem.NameId));
            set => AddDataItem(new RotaryModeDataItem(Id), value);
        }

        /// <summary>
        /// A mechanism that holds a part, stock material, or any other item in place.
        /// </summary>
        public ChuckModel Chuck
        {
            get => GetCompositionModel<ChuckModel>(typeof(ChuckComposition));
            set => AddCompositionModel(value);
        }

        /// <summary>
        /// A Linear axis represents prismatic motion along a fixed axis.
        /// </summary>
        public IEnumerable<ILinearAxisModel> LinearAxes
        {
            get
            {
                var x = new List<ILinearAxisModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.Where(o => o.Type == LinearComponent.TypeId);
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add((LinearAxisModel)model);
                    }
                }

                return x;
            }
        }

        /// <summary>
        /// A Rotary axis represents rotation about a fixed axis.
        /// </summary>
        public IEnumerable<IRotaryAxisModel> RotaryAxes
        {
            get
            {
                var x = new List<IRotaryAxisModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.Where(o => o.Type == RotaryComponent.TypeId);
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add((RotaryAxisModel)model);
                    }
                }

                return x;
            }
        }


        public RotaryAxisModel() 
        {
            Type = RotaryComponent.TypeId;
        }

        public RotaryAxisModel(string componentId)
        {
            Id = componentId;
            Type = RotaryComponent.TypeId;
        }


        protected AngleModel GetAngle()
        {
            var x = new AngleModel();

            x.Actual = GetSampleValue<AngleValue>(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.ACTUAL));

            x.Commanded = GetSampleValue<AngleValue>(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.COMMANDED));

            return x;

        }

        protected void SetAngle(AngleModel angle)
        {
            if (angle != null)
            {
                AddDataItem(new AngleDataItem(Id, AngleDataItem.SubTypes.ACTUAL), angle?.Actual);
                AddDataItem(new AngleDataItem(Id, AngleDataItem.SubTypes.COMMANDED), angle?.Commanded);
            }
        }


        protected AngularAccelerationModel GetAngularAcceleration()
        {
            var x = new AngularAccelerationModel();

            x.Actual = (AngularAccelerationValue)GetSampleValue(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.ACTUAL));

            x.Commanded = (AngularAccelerationValue)GetSampleValue(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.COMMANDED));

            return x;

        }

        protected void SetAngularAcceleration(AngularAccelerationModel angularAcceleration)
        {
            if (angularAcceleration != null)
            {
                AddDataItem(new AngularAccelerationDataItem(Id, AngularAccelerationDataItem.SubTypes.ACTUAL), angularAcceleration.Actual);
                AddDataItem(new AngularAccelerationDataItem(Id, AngularAccelerationDataItem.SubTypes.COMMANDED), angularAcceleration.Commanded);
            }
        }


        protected AngularDecelerationModel GetAngularDeceleration()
        {
            var x = new AngularDecelerationModel();

            x.Actual = (AngularDecelerationValue)GetSampleValue(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.ACTUAL));

            x.Commanded = (AngularDecelerationValue)GetSampleValue(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.COMMANDED));

            return x;

        }

        protected void SetAngularDeceleration(AngularDecelerationModel angularDeceleration)
        {
            if (angularDeceleration != null)
            {
                AddDataItem(new AngularDecelerationDataItem(Id, AngularDecelerationDataItem.SubTypes.ACTUAL), angularDeceleration.Actual);
                AddDataItem(new AngularDecelerationDataItem(Id, AngularDecelerationDataItem.SubTypes.COMMANDED), angularDeceleration.Commanded);
            }
        }


        //public virtual LinearAxisModel AddLinearAxis(string name)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var model = new LinearAxisModel
        //        {
        //            Id = CreateId(Id, name),
        //            Name = name
        //        };

        //        AddComponentModel(model);
        //        return model;
        //    }

        //    return null;
        //}

        //public virtual RotaryAxisModel AddRotaryAxis(string name)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var model = new RotaryAxisModel
        //        {
        //            Id = CreateId(Id, name),
        //            Name = name
        //        };

        //        AddComponentModel(model);
        //        return model;
        //    }

        //    return null;
        //}

        //public virtual ChuckModel AddChuck(string name)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var model = new ChuckModel
        //        {
        //            Id = CreateId(Id, name),
        //            Name = name
        //        };

        //        AddCompositionModel(model);
        //        return model;
        //    }

        //    return null;
        //}


        private RotaryVelocityModel GetRotaryVelocity()
        {
            var x = new RotaryVelocityModel();

            // Actual
            x.Actual = GetSampleValue<RotaryVelocityValue>(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.ACTUAL));

            // Commanded
            x.Commanded = GetSampleValue<RotaryVelocityValue>(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.COMMANDED));

            // Programmed
            x.Programmed = GetSampleValue<RotaryVelocityValue>(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.PROGRAMMED));

            return x;

        }

        private void SetRotaryVelocity(RotaryVelocityModel velocity)
        {
            if (velocity != null)
            {
                // Actual
                AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.ACTUAL), velocity.Actual);

                // Commanded
                AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.COMMANDED), velocity.Commanded);

                // Programmed
                AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.PROGRAMMED), velocity.Programmed);
            }
        }
    }
}
