// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements.
    /// There can be mulitple types of DataItem XML Elements in the document.
    /// </summary>
    public class DataItem
    {
        public const string DescriptionText = "An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements. There can be mulitple types of DataItem XML Elements in the document.";

        private static readonly Version DefaultMaximumVersion = new Version(1, 8);
        private static readonly Version DefaultMinimumVersion = new Version(1, 0);

        private static Dictionary<string, Type> _types;


        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// Each category of information will provide similar characteristics in its representation.
        /// The available options are SAMPLE, EVENT, or CONDITION.
        /// </summary>
        [JsonIgnore]
        public DataItemCategory Category { get; set; }

        /// <summary>
        /// The unique identifier for this DataItem.
        /// The id attribute MUST be unique across the entire document including the ids for components.
        /// An XML ID-type.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The coordinate system being used.
        /// The available values for coordinateSystem are WORK and MACHINE.
        /// </summary>
        [JsonIgnore]
        public DataItemCoordinateSystem CoordinateSystem { get; set; }

        /// <summary>
        /// The associated CoordinateSystem context for the DataItem.
        /// </summary>
        [JsonPropertyName("coordinateSystemIdRef")]
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.
        /// It is not required and will be implementation dependent.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The multiplier for the native units.
        /// The received data MAY be divided by this value before conversion.
        /// If provided, the value MUST be numeric.
        /// </summary>
        [JsonPropertyName("nativeScale")]
        public double NativeScale { get; set; }

        /// <summary>
        /// The native units used by the Component.
        /// These units will be converted before they are delivered to the application.
        /// </summary>
        [JsonPropertyName("nativeUnits")]
        public string NativeUnits { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        [JsonPropertyName("subType")]
        public string SubType { get; set; }

        /// <summary>
        /// Data calculated specific to a DataItem.
        /// Examples of statistic are AVERAGE, MINIMUM, MAXIMUM, ROOT_MEAN_SQUARE, RANGE, MEDIAN, MODE and STANDARD_DEVIATION.
        /// </summary>
        [JsonIgnore]
        public virtual DataItemStatistic Statistic { get; set; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        [JsonPropertyName("units")]
        public string Units { get; set; }

        /// <summary>
        /// The reate at which successive samples of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a floating point number.
        /// For example, a rate 1 per 10 seconds would be 0.1.
        /// </summary>
        [JsonPropertyName("sampleRate")]
        public double SampleRate { get; set; }

        /// <summary>
        /// An indication signifying whether each value reported for the Data Entity is significant and whether duplicate values are to be suppressed.
        /// </summary>
        [JsonPropertyName("discrete")]
        public bool Discrete { get; set; }

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        [JsonIgnore]
        public DataItemRepresentation Representation { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value.
        /// This is used by applications to dtermine accuracy of values.
        /// This SHOULD be specified for all numeric values.
        /// </summary>
        [JsonPropertyName("significantDigits")]
        public int SignificantDigits { get; set; }

        /// <summary>
        /// The identifier attribute of the Composition element that the reported data is most closely associated.
        /// </summary>
        [JsonPropertyName("compositionId")]
        public string CompositionId { get; set; }

        /// <summary>
        /// Source is an XML element that indentifies the Component, Subcomponent, or DataItem representing the part of the device from which a measured value originates.
        /// </summary>
        [JsonPropertyName("source")]
        public Source Source { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        [JsonPropertyName("constraints")]
        public Constraints Constraints { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        [JsonPropertyName("filters")]
        public List<Filter> Filters { get; set; }

        /// <summary>
        /// InitialValue is an optional XML element that defines the starting value for a data item as well as the value to be set for the data item after a reset event.
        /// </summary>
        [JsonPropertyName("initialValue")]
        public string InitialValue { get; set; }

        /// <summary>
        /// ResetTrigger is an XML element that describes the reset action that causes a reset to occur.
        /// </summary>
        [JsonPropertyName("resetTrigger")]
        public DataItemResetTrigger ResetTrigger { get; set; }

        /// <summary>
        /// The Definition provides additional descriptive information for any DataItem representations.
        /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
        /// </summary>
        [JsonPropertyName("definition")]
        public DataItemDefinition Definition { get; set; }

        /// <summary>
        /// Relationships organizes DataItemRelationship and SpecificationRelationship.
        /// </summary>
        [JsonPropertyName("relationships")]
        public List<Relationship> Relationships { get; set; }

        [JsonIgnore]
        public virtual string TypeDescription => DescriptionText;

        /// <summary>
        /// The path of the DataItem by Type
        /// </summary>
        [JsonIgnore]
        public string TypePath { get; set; }


        [JsonIgnore]
        public Version MaximumVersion { get; set; }

        [JsonIgnore]
        public Version MinimumVersion { get; set; }


        public DataItem()
        {
            Filters = new List<Filter>();
            Relationships = new List<Relationship>();
            MaximumVersion = DefaultMaximumVersion;
            MinimumVersion = DefaultMinimumVersion;
        }


        public virtual string GetSubTypeDescription() => null;

        public virtual DataItem Process(Version mtconnectVersion)
        {
            return Process(this, mtconnectVersion);
        }

        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        public DataItemValidationResult IsValid(Version mtconnectVersion, IObservation observation)
        {
            switch (Category)
            {
                // Validate Sample
                case DataItemCategory.SAMPLE:
                    var sampleValidation = ValidateSample(mtconnectVersion, observation);
                    if (!sampleValidation.IsValid) return sampleValidation;
                    break;

                // Validate Event
                case DataItemCategory.EVENT:
                    var eventValidation = ValidateEvent(mtconnectVersion, observation);
                    if (!eventValidation.IsValid) return eventValidation;
                    break;

                // Validate Condition
                case DataItemCategory.CONDITION:
                    var conditionValidation = ValidateCondition(mtconnectVersion, observation);
                    if (!conditionValidation.IsValid) return conditionValidation;
                    break;
            }            

            return new DataItemValidationResult(true);
        }

        private DataItemValidationResult ValidateSample(Version mtconnectVersion, IObservation observation)
        {
            // Get the CDATA Value for the Observation
            var cdata = observation.GetValue(ValueTypes.CDATA);
            if (cdata != null)
            {
                // Check if Unavailable
                if (cdata == Streams.DataItem.Unavailable) return new DataItemValidationResult(true);
            }

            return new DataItemValidationResult(true);
        }

        private DataItemValidationResult ValidateEvent(Version mtconnectVersion, IObservation observation)
        {
            // Get the CDATA Value for the Observation
            var cdata = observation.GetValue(ValueTypes.CDATA);
            if (cdata != null)
            {
                // Check if Unavailable
                if (cdata == Streams.DataItem.Unavailable) return new DataItemValidationResult(true);
            }

            return new DataItemValidationResult(true);
        }

        private DataItemValidationResult ValidateCondition(Version mtconnectVersion, IObservation observation)
        {
            // Get the Level Value for the Observation
            var level = observation.GetValue(ValueTypes.Level).ConvertEnum<Streams.ConditionLevel>();

            // Check if Unavailable
            if (level == Streams.ConditionLevel.UNAVAILABLE) return new DataItemValidationResult(true);

            return new DataItemValidationResult(true);
        }

        protected virtual DataItemValidationResult OnValidation(Version mtconnectVerion, IObservation observation)
        {
            return new DataItemValidationResult(true);
        }


        public static string CreateId(string parentId, string name)
        {
            return $"{parentId}_{name}";
        }

        public static string CreateId(string parentId, string name, string suffix)
        {
            if (!string.IsNullOrEmpty(suffix))
            {
                return $"{parentId}_{name}_{suffix}";
            }
            else
            {
                return $"{parentId}_{name}";
            }
        }


        public static DataItem Create(string type)
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
                                return (DataItem)Activator.CreateInstance(t);
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

                var types = allTypes.Where(x => typeof(DataItem).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
                if (!types.IsNullOrEmpty())
                {
                    var objs = new Dictionary<string, Type>();
                    var regex = new Regex("(.*)DataItem|(.*)Condition");

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


        public static DataItem Process(DataItem dataItem, Version mtconnectVersion)
        {
            if (dataItem != null)
            {
                var obj = Create(dataItem.Type);
                if (obj != null)
                {
                    obj.Category = dataItem.Category;
                    obj.Id = dataItem.Id;
                    obj.Name = dataItem.Name;
                    obj.Type = dataItem.Type;
                    obj.SubType = dataItem.SubType;
                    obj.NativeUnits = dataItem.NativeUnits;
                    obj.NativeScale = dataItem.NativeScale;
                    obj.SampleRate = dataItem.SampleRate;
                    obj.Source = dataItem.Source;
                    obj.Relationships = dataItem.Relationships;
                    obj.Representation = dataItem.Representation;
                    obj.ResetTrigger = dataItem.ResetTrigger;
                    obj.CoordinateSystem = dataItem.CoordinateSystem;
                    obj.CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;
                    obj.CompositionId = dataItem.CompositionId;
                    obj.Constraints = dataItem.Constraints;
                    obj.Definition = dataItem.Definition;
                    obj.Units = dataItem.Units;
                    obj.Statistic = dataItem.Statistic;
                    obj.SignificantDigits = dataItem.SignificantDigits;
                    obj.Filters = dataItem.Filters;
                    obj.InitialValue = dataItem.InitialValue;
                    obj.Discrete = dataItem.Discrete;
                }
                else obj = dataItem;

                // Check Version Compatibilty
                if (mtconnectVersion >= obj.MinimumVersion && mtconnectVersion <= obj.MaximumVersion)
                {
                    return obj;
                }
            }

            return null;
        }
    }
}
