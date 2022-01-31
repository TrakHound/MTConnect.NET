// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Models.Compositions;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// ToolMagazine is a ToolingDelivery that represents a tool storage mechanism that holds any number of tools.Tools are located in POTs.
    /// POTs are moved into position to transfer tools into or out of the ToolMagazine by an AutomaticToolChanger.
    /// </summary>
    public class ToolMagazineModel : ToolingDeliveryModel, IToolMagazineModel
    {
        /// <summary>
        /// A POT for a tool awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
        /// </summary>
        public IStagingPotModel StagingPot
        {
            get => GetCompositionModel<StagingPotModel>(typeof(StagingPotComposition));
            set => AddCompositionModel((StagingPotModel)value);
        }

        /// <summary>
        /// Tool storage locations.
        /// </summary>
        public IEnumerable<IPotModel> Pots
        {
            get
            {
                var x = new List<PotModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.Where(o => o.Type == PotComposition.TypeId);
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add((PotModel)model);
                    }
                }

                return x;
            }
        }

        /// <summary>
        /// Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. 
        /// The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.
        /// </summary>
        public IEnclosureModel Enclosure => GetComponentModel<EnclosureModel>(typeof(EnclosureComponent));


        public ToolMagazineModel()
        {
            Type = ToolMagazineComponent.TypeId;
        }

        public ToolMagazineModel(string componentId)
        {
            Id = componentId;
            Type = ToolMagazineComponent.TypeId;
        }


        /// <summary>
        /// Gets the Pot with the specified Pot Number
        /// (if doesn't exist then it will be created)
        /// </summary>
        public IPotModel GetPot(int potNumber) => GetCompositionModel<PotModel>(typeof(PotComposition), PotModel.CreateName(potNumber));
    }
}
