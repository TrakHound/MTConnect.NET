// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Models.Compositions;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;
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
            get => DataItemManager.GetSampleValue<AngularVelocityValue>(Devices.DataItems.Samples.AngularVelocityDataItem.NameId);
            set => DataItemManager.AddDataItem(new AngularVelocityDataItem(Id), value);
        }
        public IDataItemModel AngularVelocityDataItem => DataItemManager.GetDataItem(Devices.DataItems.Samples.AngularVelocityDataItem.NameId);

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
            get => DataItemManager.GetDataItemValue<RotaryDirection>(DataItem.CreateId(Id, DirectionDataItem.NameId, DirectionDataItem.GetSubTypeId(DirectionDataItem.SubTypes.ROTARY)));
            set => DataItemManager.AddDataItem(new DirectionDataItem(Id, DirectionDataItem.SubTypes.ROTARY), value);
        }
        public IDataItemModel RotaryDirectionDataItem => DataItemManager.GetDataItem(DirectionDataItem.NameId, DirectionDataItem.GetSubTypeId(DirectionDataItem.SubTypes.ROTARY));

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
            get => DataItemManager.GetEventValue<RotaryVelocityOverrideValue>(RotaryVelocityOverrideDataItem.NameId);
            set => DataItemManager.AddDataItem(new RotaryVelocityOverrideDataItem(Id), value);
        }

        /// <summary>
        /// The current operating mode for a Rotary type axis.
        /// </summary>
        public RotaryMode RotaryMode
        {
            get => DataItemManager.GetDataItemValue<RotaryMode>(DataItem.CreateId(Id, RotaryModeDataItem.NameId));
            set => DataItemManager.AddDataItem(new RotaryModeDataItem(Id), value);
        }

        /// <summary>
        /// A mechanism that holds a part, stock material, or any other item in place.
        /// </summary>
        public ChuckModel Chuck
        {
            get => ComponentManager.GetCompositionModel<ChuckModel>(typeof(ChuckComposition));
            set => ComponentManager.AddCompositionModel(value);
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

            x.Actual = DataItemManager.GetSampleValue<AngleValue>(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = DataItemManager.GetDataItem(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.ACTUAL));

            x.Commanded = DataItemManager.GetSampleValue<AngleValue>(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = DataItemManager.GetDataItem(AngleDataItem.NameId, AngleDataItem.GetSubTypeId(AngleDataItem.SubTypes.COMMANDED));

            return x;

        }

        protected void SetAngle(AngleModel angle)
        {
            if (angle != null)
            {
                DataItemManager.AddDataItem(new AngleDataItem(Id, AngleDataItem.SubTypes.ACTUAL), angle?.Actual);
                DataItemManager.AddDataItem(new AngleDataItem(Id, AngleDataItem.SubTypes.COMMANDED), angle?.Commanded);
            }
        }


        protected AngularAccelerationModel GetAngularAcceleration()
        {
            var x = new AngularAccelerationModel();

            x.Actual = (AngularAccelerationValue)DataItemManager.GetSampleValue(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = DataItemManager.GetDataItem(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.ACTUAL));

            x.Commanded = (AngularAccelerationValue)DataItemManager.GetSampleValue(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = DataItemManager.GetDataItem(AngularAccelerationDataItem.NameId, AngularAccelerationDataItem.GetSubTypeId(AngularAccelerationDataItem.SubTypes.COMMANDED));

            return x;

        }

        protected void SetAngularAcceleration(AngularAccelerationModel angularAcceleration)
        {
            if (angularAcceleration != null)
            {
                DataItemManager.AddDataItem(new AngularAccelerationDataItem(Id, AngularAccelerationDataItem.SubTypes.ACTUAL), angularAcceleration.Actual);
                DataItemManager.AddDataItem(new AngularAccelerationDataItem(Id, AngularAccelerationDataItem.SubTypes.COMMANDED), angularAcceleration.Commanded);
            }
        }


        protected AngularDecelerationModel GetAngularDeceleration()
        {
            var x = new AngularDecelerationModel();

            x.Actual = (AngularDecelerationValue)DataItemManager.GetSampleValue(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = DataItemManager.GetDataItem(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.ACTUAL));

            x.Commanded = (AngularDecelerationValue)DataItemManager.GetSampleValue(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = DataItemManager.GetDataItem(AngularDecelerationDataItem.NameId, AngularDecelerationDataItem.GetSubTypeId(AngularDecelerationDataItem.SubTypes.COMMANDED));

            return x;

        }

        protected void SetAngularDeceleration(AngularDecelerationModel angularDeceleration)
        {
            if (angularDeceleration != null)
            {
                DataItemManager.AddDataItem(new AngularDecelerationDataItem(Id, AngularDecelerationDataItem.SubTypes.ACTUAL), angularDeceleration.Actual);
                DataItemManager.AddDataItem(new AngularDecelerationDataItem(Id, AngularDecelerationDataItem.SubTypes.COMMANDED), angularDeceleration.Commanded);
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
            x.Actual = DataItemManager.GetSampleValue<RotaryVelocityValue>(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = DataItemManager.GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.ACTUAL));

            // Commanded
            x.Commanded = DataItemManager.GetSampleValue<RotaryVelocityValue>(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = DataItemManager.GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.COMMANDED));

            // Programmed
            x.Programmed = DataItemManager.GetSampleValue<RotaryVelocityValue>(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = DataItemManager.GetDataItem(RotaryVelocityDataItem.NameId, RotaryVelocityDataItem.GetSubTypeId(RotaryVelocityDataItem.SubTypes.PROGRAMMED));

            return x;

        }

        private void SetRotaryVelocity(RotaryVelocityModel velocity)
        {
            if (velocity != null)
            {
                // Actual
                DataItemManager.AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.ACTUAL), velocity.Actual);

                // Commanded
                DataItemManager.AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.COMMANDED), velocity.Commanded);

                // Programmed
                DataItemManager.AddDataItem(new RotaryVelocityDataItem(Id, RotaryVelocityDataItem.SubTypes.PROGRAMMED), velocity.Programmed);
            }
        }
    }
}