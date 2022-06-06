// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

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
    public class AxesModel : ComponentModel, IAxesModel
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


        public AxesModel() 
        {
            Type = AxesComponent.TypeId;
        }

        public AxesModel(string componentId)
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

        /// <summary>
        /// Gets the Linear Component with the "X" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. X1, X2)</param>
        public ILinearAxisModel GetXAxis(string suffix = null) => ComponentManager.GetComponentModel<LinearAxisModel>(typeof(LinearComponent), CreateAxisName("X", suffix));

        /// <summary>
        /// Gets the Linear Component with the "Y" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. Y1, Y2)</param>
        public ILinearAxisModel GetYAxis(string suffix = null) => ComponentManager.GetComponentModel<LinearAxisModel>(typeof(LinearComponent), CreateAxisName("Y", suffix));

        /// <summary>
        /// Gets the Linear Component with the "Z" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. Z1, Z2)</param>
        public ILinearAxisModel GetZAxis(string suffix = null) => ComponentManager.GetComponentModel<LinearAxisModel>(typeof(LinearComponent), CreateAxisName("Z", suffix));

        /// <summary>
        /// Gets the Roatry Component with the "A" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. A1, A2)</param>
        public IRotaryAxisModel GetAAxis(string suffix = null) => ComponentManager.GetComponentModel<RotaryAxisModel>(typeof(RotaryComponent), CreateAxisName("A", suffix));

        /// <summary>
        /// Gets the Roatry Component with the "B" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. B1, B2)</param>
        public IRotaryAxisModel GetBAxis(string suffix = null) => ComponentManager.GetComponentModel<RotaryAxisModel>(typeof(RotaryComponent), CreateAxisName("B", suffix));

        /// <summary>
        /// Gets the Roatry Component with the "C" Name
        /// (If the axis doesn't exist then it will be created)
        /// </summary>
        /// <param name="suffix">An optional suffix used when there are multiple axes with the same name (ex. C1, C2)</param>
        public IRotaryAxisModel GetCAxis(string suffix = null) => ComponentManager.GetComponentModel<RotaryAxisModel>(typeof(RotaryComponent), CreateAxisName("C", suffix));
    }
}
