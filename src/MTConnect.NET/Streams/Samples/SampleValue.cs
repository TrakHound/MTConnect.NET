// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Streams.Samples
{
    public class SampleValue : Sample
    {
        private static Dictionary<string, Type> _types;


        protected virtual double MetricConversion => 1;
        protected virtual double InchConversion => 1;
        protected virtual string MetricUnits { get; }
        protected virtual string InchUnits { get; }
        protected virtual string MetricUnitAbbreviation { get; }
        protected virtual string InchUnitAbbreviation { get; }

        public object Value { get; set; }

        public string Units
        {
            get
            {
                if (UnitSystem == UnitSystem.METRIC) return MetricUnits;
                else return InchUnits;
            }
        }

        public string UnitAbbreviation
        {
            get
            {
                if (UnitSystem == UnitSystem.METRIC) return MetricUnitAbbreviation;
                else return InchUnitAbbreviation;
            }
        }

        public UnitSystem UnitSystem { get; set; }


        protected SampleValue() { }

        public SampleValue(double value, UnitSystem unitSystem = UnitSystem.METRIC)
        {
            Value = value;
            UnitSystem = unitSystem;
        }


        public override string ToString()
        {
            return ToMetric().ToString();
        }


        public virtual double ToMetric()
        {
            if (UnitSystem == UnitSystem.INCH)
            {
                return Value.ToDouble() * MetricConversion;
            }

            return Value.ToDouble();
        }

        public virtual double ToInch()
        {
            if (UnitSystem == UnitSystem.METRIC)
            {
                return Value.ToDouble() * InchConversion;
            }

            return Value.ToDouble();
        }


        public static SampleValue Create(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    var titleType = type.ToPascalCase();

                    if (_types.TryGetValue(titleType, out Type t))
                    {
                        var constructor = t.GetConstructor(System.Type.EmptyTypes);
                        if (constructor != null)
                        {
                            try
                            {
                                return (SampleValue)Activator.CreateInstance(t);
                            }
                            catch { }
                        }
                    }
                }
            }

            return null;
        }

        private static Dictionary<string, Type> GetAllTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (!assemblies.IsNullOrEmpty())
            {
                var allTypes = assemblies.SelectMany(x => x.GetTypes());

                var types = allTypes.Where(x => typeof(SampleValue).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
                if (!types.IsNullOrEmpty())
                {
                    var objs = new Dictionary<string, Type>();
                    var regex = new Regex("(.*)Value");

                    foreach (var type in types)
                    {
                        var match = regex.Match(type.Name);
                        if (match.Success && match.Groups.Count > 1)
                        {
                            string key = null;

                            if (match.Groups[1].Success) key = match.Groups[1].Value;
                            else if (match.Groups[2].Success) key = match.Groups[2].Value;

                            if (!string.IsNullOrEmpty(key))
                            {
                                if (!objs.ContainsKey(key)) objs.Add(key, type);
                            }
                        }
                    }

                    return objs;
                }
            }

            return new Dictionary<string, Type>();
        }
    }
}
