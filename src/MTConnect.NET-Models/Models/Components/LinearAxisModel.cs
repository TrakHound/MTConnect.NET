// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Devices.DataItems.Samples;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;
using MTConnect.Observations.Samples.Values;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// A Linear axis represents prismatic motion along a fixed axis.
    /// </summary>
    public class LinearAxisModel : AxisModel, ILinearAxisModel
    {
        /// <summary>
        /// A measured or calculated position of a Component element as reported by a piece of equipment.
        /// </summary>
        public PositionModel MachinePosition
        {
            get => GetPosition(DataItemCoordinateSystem.MACHINE);
            set => SetPosition(value, DataItemCoordinateSystem.MACHINE);
        }

        /// <summary>
        /// A measured or calculated position of a Component element as reported by a piece of equipment.
        /// </summary>
        public PositionModel WorkPosition
        {
            get => GetPosition(DataItemCoordinateSystem.WORK);
            set => SetPosition(value, DataItemCoordinateSystem.WORK);
        }

        /// <summary>
        /// The measurement of the feedrate of a linear axis.
        /// </summary>
        public AxisFeedrateModel AxisFeedrate
        {
            get => GetFeedrate();
            set => SetFeedrate(value);
        }

        /// <summary>
        /// The value of a signal or calculation issued to adjust the feedrate of an individual linear type axis.
        /// </summary>
        public AxisFeedrateOverrideModel AxisFeedrateOverride
        {
            get => GetFeedrateOverride();
            set => SetFeedrateOverride(value);
        }

        /// <summary>
        /// The direction of motion.
        /// </summary>
        public LinearDirection Direction
        {
            get => DataItemManager.GetDataItemValue<LinearDirection>(DataItem.CreateId(Id, DirectionDataItem.NameId, DirectionDataItem.GetSubTypeId(DirectionDataItem.SubTypes.LINEAR)));
            set => DataItemManager.AddDataItem(new DirectionDataItem(Id, DirectionDataItem.SubTypes.LINEAR), value);
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


        public LinearAxisModel() 
        {
            Type = LinearComponent.TypeId;
        }

        public LinearAxisModel(string componentId)
        {
            Id = componentId;
            Type = LinearComponent.TypeId;
        }


        protected PositionModel GetPosition(DataItemCoordinateSystem coordinateSystem)
        {
            var x = new PositionModel();

            // Actual Position
            x.Actual = DataItemManager.GetSampleValue<PositionValue>(PositionDataItem.NameId, PositionDataItem.GetSubTypeId(PositionDataItem.SubTypes.ACTUAL, coordinateSystem));
            x.ActualDataItem = DataItemManager.GetDataItem(PositionDataItem.NameId, PositionDataItem.GetSubTypeId(PositionDataItem.SubTypes.ACTUAL, coordinateSystem));

            // Commanded Position
            x.Commanded = DataItemManager.GetSampleValue<PositionValue>(PositionDataItem.NameId, PositionDataItem.GetSubTypeId(PositionDataItem.SubTypes.COMMANDED, coordinateSystem));
            x.CommandedDataItem = DataItemManager.GetDataItem(PositionDataItem.NameId, PositionDataItem.GetSubTypeId(PositionDataItem.SubTypes.COMMANDED, coordinateSystem));

            // Programmed Position
            x.Programmed = DataItemManager.GetSampleValue<PositionValue>(PositionDataItem.NameId, PositionDataItem.GetSubTypeId(PositionDataItem.SubTypes.PROGRAMMED, coordinateSystem));
            x.ProgrammedDataItem = DataItemManager.GetDataItem(PositionDataItem.NameId, PositionDataItem.GetSubTypeId(PositionDataItem.SubTypes.PROGRAMMED, coordinateSystem));

            return x;

        }

        protected void SetPosition(PositionModel position, DataItemCoordinateSystem coordinateSystem)
        {
            if (position != null)
            {
                // Actual Position
                DataItemManager.AddDataItem(new PositionDataItem(Id, PositionDataItem.SubTypes.ACTUAL, coordinateSystem), position.Actual);

                // Commanded Position
                DataItemManager.AddDataItem(new PositionDataItem(Id, PositionDataItem.SubTypes.COMMANDED, coordinateSystem), position.Commanded);

                // Programmed Position
                DataItemManager.AddDataItem(new PositionDataItem(Id, PositionDataItem.SubTypes.PROGRAMMED, coordinateSystem), position.Programmed);
            }
        }


        protected AxisFeedrateModel GetFeedrate()
        {
            var x = new AxisFeedrateModel();

            // Actual Feedrate
            x.Actual = (AxisFeedrateValue)DataItemManager.GetSampleValue(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.ACTUAL));
            x.ActualDataItem = DataItemManager.GetDataItem(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.ACTUAL));

            // Commanded Feedrate
            x.Commanded = (AxisFeedrateValue)DataItemManager.GetSampleValue(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.COMMANDED));
            x.CommandedDataItem = DataItemManager.GetDataItem(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.COMMANDED));

            // Jog Feedrate
            x.Jog = (AxisFeedrateValue)DataItemManager.GetSampleValue(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.JOG));
            x.JogDataItem = DataItemManager.GetDataItem(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.JOG));

            // Programmed Feedrate
            x.Programmed = (AxisFeedrateValue)DataItemManager.GetSampleValue(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.PROGRAMMED));
            x.ProgrammedDataItem = DataItemManager.GetDataItem(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.PROGRAMMED));

            // Rapid Feedrate
            x.Rapid = (AxisFeedrateValue)DataItemManager.GetSampleValue(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.RAPID));
            x.RapidDataItem = DataItemManager.GetDataItem(AxisFeedrateDataItem.NameId, AxisFeedrateDataItem.GetSubTypeId(AxisFeedrateDataItem.SubTypes.RAPID));

            return x;

        }

        protected void SetFeedrate(AxisFeedrateModel feedrate)
        {
            if (feedrate != null)
            {
                // Actual Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateDataItem(Id, AxisFeedrateDataItem.SubTypes.ACTUAL), feedrate.Actual);

                // Commanded Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateDataItem(Id, AxisFeedrateDataItem.SubTypes.COMMANDED), feedrate.Commanded);

                // Jog Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateDataItem(Id, AxisFeedrateDataItem.SubTypes.JOG), feedrate.Jog);

                // Programmed Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateDataItem(Id, AxisFeedrateDataItem.SubTypes.PROGRAMMED), feedrate.Programmed);

                // Rapid Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateDataItem(Id, AxisFeedrateDataItem.SubTypes.RAPID), feedrate.Rapid);
            }
        }


        protected AxisFeedrateOverrideModel GetFeedrateOverride()
        {
            var x = new AxisFeedrateOverrideModel();

            // Programmed Feedrate
            x.Programmed = DataItemManager.GetDataItemValue(AxisFeedrateOverrideDataItem.NameId, AxisFeedrateOverrideDataItem.GetSubTypeId(AxisFeedrateOverrideDataItem.SubTypes.PROGRAMMED)).ToDouble();
            x.ProgrammedDataItem = DataItemManager.GetDataItem(AxisFeedrateOverrideDataItem.NameId, AxisFeedrateOverrideDataItem.GetSubTypeId(AxisFeedrateOverrideDataItem.SubTypes.PROGRAMMED));

            // Rapid Feedrate
            x.Rapid = DataItemManager.GetDataItemValue(AxisFeedrateOverrideDataItem.NameId, AxisFeedrateOverrideDataItem.GetSubTypeId(AxisFeedrateOverrideDataItem.SubTypes.RAPID)).ToDouble();
            x.RapidDataItem = DataItemManager.GetDataItem(AxisFeedrateOverrideDataItem.NameId, AxisFeedrateOverrideDataItem.GetSubTypeId(AxisFeedrateOverrideDataItem.SubTypes.RAPID));

            // Jog Feedrate
            x.Jog = DataItemManager.GetDataItemValue(AxisFeedrateOverrideDataItem.NameId, AxisFeedrateOverrideDataItem.GetSubTypeId(AxisFeedrateOverrideDataItem.SubTypes.JOG)).ToDouble();
            x.JogDataItem = DataItemManager.GetDataItem(AxisFeedrateOverrideDataItem.NameId, AxisFeedrateOverrideDataItem.GetSubTypeId(AxisFeedrateOverrideDataItem.SubTypes.JOG));

            return x;

        }

        protected void SetFeedrateOverride(AxisFeedrateOverrideModel feedrateOverride)
        {
            if (feedrateOverride != null)
            {
                // Programmed Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateOverrideDataItem(Id, AxisFeedrateOverrideDataItem.SubTypes.PROGRAMMED), feedrateOverride.Programmed);

                // Rapid Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateOverrideDataItem(Id, AxisFeedrateOverrideDataItem.SubTypes.RAPID), feedrateOverride.Rapid);

                // Jog Feedrate
                DataItemManager.AddDataItem(new AxisFeedrateOverrideDataItem(Id, AxisFeedrateOverrideDataItem.SubTypes.JOG), feedrateOverride.Jog);
            }
        }


        public virtual LinearAxisModel AddLinearAxis(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new LinearAxisModel
                {
                    Id = CreateId(Id, name),
                    Name = name
                };

                AddComponentModel(model);
                return model;
            }

            return null;
        }

        public virtual RotaryAxisModel AddRotaryAxis(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new RotaryAxisModel
                {
                    Id = CreateId(Id, name),
                    Name = name
                };

                AddComponentModel(model);
                return model;
            }

            return null;
        }
    }
}
