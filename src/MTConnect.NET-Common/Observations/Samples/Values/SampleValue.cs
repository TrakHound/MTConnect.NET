// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Observations.Samples.Values
{
    public class SampleValue<T> : SampleValueObservation
    {
        internal string _units;
        internal string _nativeUnits;


        /// <summary>
        /// Gets or Sets the Units for the DataItem
        /// </summary>
        public string Units => _units;

        /// <summary>
        /// Get or Sets the Native Units for the DataItem
        /// </summary>
        public string NativeUnits => _nativeUnits;

        /// <summary>
        /// Gets or Sets the Value of the Sample in the Units that are specified for the DataItem
        /// </summary>
        public virtual T Value { get; set; }

        /// <summary>
        /// Get or Sets the Value of the Sample in the Native Units that are specified for the DataItem
        /// </summary>
        public virtual T NativeValue { get; set; }
    }


    public class SampleValue : SampleValue<double>
    {
        private static Dictionary<string, Type> _types;


        public override double Value 
        { 
            get => ConvertUnits(CDATA.ToDouble(), Units, NativeUnits); 
            set => CDATA = value.ToString();
        }

        public override double NativeValue
        {
            get => ConvertUnits(CDATA.ToDouble(), NativeUnits, Units);
            set => CDATA = value.ToString();
        }


        protected SampleValue() { }

        public SampleValue(double value)
        {
            Value = value;
        }

 
        public override string ToString()
        {
            return Value.ToString();
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
