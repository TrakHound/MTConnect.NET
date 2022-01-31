// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Devices.Events;
using MTConnect.Models.Compositions;
using MTConnect.Streams.Events;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// WasteDisposal is an Auxiliary that represents the information for a unit comprised of all the parts involved in removing manufacturing byproducts from a piece of equipment.
    /// </summary>
    public class WasteDisposalModel : AuxiliaryModel
    {
        /// <summary>
        /// The execution status of a component.
        /// </summary>
        public Execution Execution
        {
            get => GetDataItemValue<Execution>(DataItem.CreateId(Id, Devices.Events.ExecutionDataItem.NameId));
            set => AddDataItem(new ExecutionDataItem(Id), value);
        }
        public IDataItemModel ExecutionDataItem => GetDataItem(Devices.Events.ExecutionDataItem.NameId);

        /// <summary>
        /// A mechanism that converts electrical, pneumatic, or hydraulic energy into mechanical energy.
        /// </summary>
        public IMotorModel Motor
        {
            get => (MotorModel)CompositionModels?.FirstOrDefault(o => o.Type == MotorComposition.TypeId);
            set => AddCompositionModel((MotorModel)value);
        }

        /// <summary>
        /// Any substance or structure through which liquids or gases are passed to remove suspended impurities or to recover solids.
        /// </summary>
        public IFilterModel Filter
        {
            get => (FilterModel)CompositionModels?.FirstOrDefault(o => o.Type == FilterComposition.TypeId);
            set => AddCompositionModel((FilterModel)value);
        }


        public WasteDisposalModel()
        {
            Type = WasteDisposalComponent.TypeId;
        }

        public WasteDisposalModel(string componentId)
        {
            Id = componentId;
            Type = WasteDisposalComponent.TypeId;
        }


        public IMotorModel AddMotor(string name) => AddCompositionModel<MotorModel>(name);

        public IFilterModel AddFilter(string name) => AddCompositionModel<FilterModel>(name);
    }
}
