// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Components;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// A Three-Dimensional Cartesian Coordinate control system organizes its axes orthogonally relative
    /// to a machine coordinate system where the manufacturer of the equipment specifies the origin.
    /// </summary>
    public class CartesianCoordinateAxesModel : ComponentModel, IAxesModel
    {
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


        public CartesianCoordinateAxesModel() 
        {
            Type = AxesComponent.TypeId;
        }

        public CartesianCoordinateAxesModel(string componentId)
        {
            Id = componentId;
            Type = AxesComponent.TypeId;
        }


        private string CreateAxisName(string name, string suffix = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    return $"{name}{suffix}";
                }
                else
                {
                    return name;
                }
            }

            return null;
        }

        public ILinearAxisModel GetXAxis(string suffix = null) => LinearAxes?.OfType<LinearAxisModel>().FirstOrDefault(o => o.Name == CreateAxisName("X", suffix));

        public ILinearAxisModel GetYAxis(string suffix = null) => LinearAxes?.OfType<LinearAxisModel>().FirstOrDefault(o => o.Name == CreateAxisName("Y", suffix));

        public ILinearAxisModel GetZAxis(string suffix = null) => LinearAxes?.OfType<LinearAxisModel>().FirstOrDefault(o => o.Name == CreateAxisName("Z", suffix));


        public IRotaryAxisModel GetAAxis(string suffix = null) => RotaryAxes?.OfType<RotaryAxisModel>().FirstOrDefault(o => o.Name == CreateAxisName("A", suffix));

        public IRotaryAxisModel GetBAxis(string suffix = null) => RotaryAxes?.OfType<RotaryAxisModel>().FirstOrDefault(o => o.Name == CreateAxisName("B", suffix));

        public IRotaryAxisModel GetCAxis(string suffix = null) => RotaryAxes?.OfType<RotaryAxisModel>().FirstOrDefault(o => o.Name == CreateAxisName("C", suffix));


        public ILinearAxisModel AddXAxis(string suffix = null) => AddLinearAxis(CreateAxisName("X", suffix));

        public ILinearAxisModel AddYAxis(string suffix = null) => AddLinearAxis(CreateAxisName("Y", suffix));

        public ILinearAxisModel AddZAxis(string suffix = null) => AddLinearAxis(CreateAxisName("Z", suffix));


        public IRotaryAxisModel AddAAxis(string suffix = null) => AddRotaryAxis(CreateAxisName("A", suffix));

        public IRotaryAxisModel AddBAxis(string suffix = null) => AddRotaryAxis(CreateAxisName("B", suffix));

        public IRotaryAxisModel AddCAxis(string suffix = null) => AddRotaryAxis(CreateAxisName("C", suffix));


        public virtual ILinearAxisModel AddLinearAxis(string name)
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

        public virtual IRotaryAxisModel AddRotaryAxis(string name)
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
