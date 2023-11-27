// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. 
    /// The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.
    /// </summary>
    public class EnclosureModel : SystemModel, IEnclosureModel
    {
        /// <summary>
        /// Door is a Component that represents the information for a mechanical mechanism or closure that can cover, for example, a physical access portal into a piece of equipment.
        /// The closure can be opened or closed to allow or restrict access to other parts of the equipment.
        /// </summary>
        public IEnumerable<DoorModel> Doors
        {
            get
            {
                var x = new List<DoorModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.Where(o => o.Type == DoorComponent.TypeId);
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add((DoorModel)model);
                    }
                }

                return x;
            }
        }


        public EnclosureModel()
        {
            Type = EnclosureComponent.TypeId;
        }

        public EnclosureModel(string componentId)
        {
            Id = componentId;
            Type = EnclosureComponent.TypeId;
        }


        /// <summary>
        /// Get the Door Component Model with the specified Name
        /// </summary>
        public IDoorModel GetDoor(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return ComponentManager.GetComponentModel<DoorModel>(typeof(DoorComponent), name);
            }

            return null;
        }
    }
}