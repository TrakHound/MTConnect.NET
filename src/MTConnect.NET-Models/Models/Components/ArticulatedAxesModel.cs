// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    public class ArticulatedAxesModel : ComponentModel
    {
        public string TypeId => "dummy";


        public IEnumerable<LinearAxisModel> LinearAxes
        {
            get
            {
                var x = new List<LinearAxisModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.OfType<LinearAxisModel>();
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add(model);
                    }
                }

                return x;
            }
        }

        public IEnumerable<RotaryAxisModel> RotaryAxes
        {
            get
            {
                var x = new List<RotaryAxisModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.OfType<RotaryAxisModel>();
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add(model);
                    }
                }

                return x;
            }
        }

        //public IEnumerable<SpindleAxisModel> SpindleAxes
        //{
        //    get
        //    {
        //        var x = new List<SpindleAxisModel>();

        //        if (!ComponentModels.IsNullOrEmpty())
        //        {
        //            var models = ComponentModels.OfType<SpindleAxisModel>();
        //            if (!models.IsNullOrEmpty())
        //            {
        //                foreach (var model in models) x.Add(model);
        //            }
        //        }

        //        return x;
        //    }
        //}


        public ArticulatedAxesModel() 
        {
            Type = TypeId;
        }

        public ArticulatedAxesModel(string componentId)
        {
            Id = componentId;
            Type = TypeId;
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

        public LinearAxisModel GetXAxis(string suffix = null) => LinearAxes?.FirstOrDefault(o => o.Name == CreateAxisName("X", suffix));

        public LinearAxisModel GetYAxis(string suffix = null) => LinearAxes?.FirstOrDefault(o => o.Name == CreateAxisName("Y", suffix));

        public LinearAxisModel GetZAxis(string suffix = null) => LinearAxes?.FirstOrDefault(o => o.Name == CreateAxisName("Z", suffix));


        public RotaryAxisModel GetAAxis(string suffix = null) => RotaryAxes?.FirstOrDefault(o => o.Name == CreateAxisName("A", suffix));

        public RotaryAxisModel GetBAxis(string suffix = null) => RotaryAxes?.FirstOrDefault(o => o.Name == CreateAxisName("B", suffix));

        public RotaryAxisModel GetCAxis(string suffix = null) => RotaryAxes?.FirstOrDefault(o => o.Name == CreateAxisName("C", suffix));


        //public SpindleAxisModel GetSpindle(string suffix = null) => SpindleAxes?.FirstOrDefault(o => o.Name == CreateAxisName("S", suffix));


        public LinearAxisModel AddXAxis(string suffix = null) => AddLinearAxis(CreateAxisName("X", suffix));

        public LinearAxisModel AddYAxis(string suffix = null) => AddLinearAxis(CreateAxisName("Y", suffix));

        public LinearAxisModel AddZAxis(string suffix = null) => AddLinearAxis(CreateAxisName("Z", suffix));


        public RotaryAxisModel AddAAxis(string suffix = null) => AddRotaryAxis(CreateAxisName("A", suffix));

        public RotaryAxisModel AddBAxis(string suffix = null) => AddRotaryAxis(CreateAxisName("B", suffix));

        public RotaryAxisModel AddCAxis(string suffix = null) => AddRotaryAxis(CreateAxisName("C", suffix));

        //public SpindleAxisModel AddSpindle(string suffix = null) => AddSpindleAxis(CreateAxisName("S", suffix));


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

        //public virtual SpindleAxisModel AddSpindleAxis(string name)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        var model = new SpindleAxisModel
        //        {
        //            Id = CreateId(Id, name),
        //            Name = name
        //        };

        //        AddComponentModel(model);
        //        return model;
        //    }

        //    return null;
        //}
    }
}