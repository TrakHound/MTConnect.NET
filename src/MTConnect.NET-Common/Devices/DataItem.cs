// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MTConnect.Devices
{
    /// <summary>
    /// An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements.
    /// There can be mulitple types of DataItem XML Elements in the document.
    /// </summary>
    public class DataItem : IDataItem
    {
        public const string DescriptionText = "An abstract XML Element. Replaced in the XML document by Elements representing various types of DataItem XML Elements. There can be mulitple types of DataItem XML Elements in the document.";

        private static readonly Version DefaultMaximumVersion = MTConnectVersions.Max;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;

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

        /// <summary>
        /// A MD5 Hash of the DataItem that can be used to compare DataItem objects
        /// </summary>
        [JsonIgnore]
        public string ChangeId => CreateChangeId();

        [JsonIgnore]
        public virtual string TypeDescription => DescriptionText;

        [JsonIgnore]
        public virtual string SubTypeDescription => null;

        /// <summary>
        /// The path of the DataItem by Type
        /// </summary>
        [JsonIgnore]
        public string TypePath { get; set; }


        [JsonIgnore]
        public virtual Version MaximumVersion => DefaultMaximumVersion;

        [JsonIgnore]
        public virtual Version MinimumVersion => DefaultMinimumVersion;


        public DataItem()
        {
            Filters = new List<Filter>();
            Relationships = new List<Relationship>();
        }


        public string CreateChangeId()
        {
            return CreateChangeId(this);
        }

        public static string CreateChangeId(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                var ids = new List<string>();
                ids.Add(ObjectExtensions.GetChangeIdPropertyString(dataItem).ToMD5Hash());

                // Add Relationship Change ID's
                if (!dataItem.Relationships.IsNullOrEmpty())
                {
                    foreach (var relationship in dataItem.Relationships)
                    {
                        ids.Add(relationship.ChangeId);
                    }
                }

                return StringFunctions.ToMD5Hash(ids.ToArray());
            }

            return null;
        }


        public virtual IDataItem Process(Version mtconnectVersion)
        {
            return Process(this, mtconnectVersion);
        }

        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        public DataItemValidationResult IsValid(Version mtconnectVersion, IObservationInput observation)
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

        private DataItemValidationResult ValidateSample(Version mtconnectVersion, IObservationInput observation)
        {
            // Get the CDATA Value for the Observation
            var cdata = observation.GetValue(ValueKeys.CDATA);
            if (cdata != null)
            {
                // Check if Unavailable
                if (cdata == Observation.Unavailable) return new DataItemValidationResult(true);
            }

            return new DataItemValidationResult(true);
        }

        private DataItemValidationResult ValidateEvent(Version mtconnectVersion, IObservationInput observation)
        {
            // Get the CDATA Value for the Observation
            var cdata = observation.GetValue(ValueKeys.CDATA);
            if (cdata != null)
            {
                // Check if Unavailable
                if (cdata == Observation.Unavailable) return new DataItemValidationResult(true);
            }

            return new DataItemValidationResult(true);
        }

        private DataItemValidationResult ValidateCondition(Version mtconnectVersion, IObservationInput observation)
        {
            // Get the Level Value for the Observation
            var level = observation.GetValue(ValueKeys.Level).ConvertEnum<ConditionLevel>();

            // Check if Unavailable
            if (level == ConditionLevel.UNAVAILABLE) return new DataItemValidationResult(true);

            return new DataItemValidationResult(true);
        }

        protected virtual DataItemValidationResult OnValidation(Version mtconnectVerion, IObservationInput observation)
        {
            return new DataItemValidationResult(true);
        }


        public static string CreateId(string parentId, string name)
        {
            return $"{parentId}_{name}";
        }

        public static string CreateDataItemId(string parentId, string type, string subType = null)
        {
            if (!string.IsNullOrEmpty(subType))
            {
                return $"{parentId}_{type}_{subType}";
            }
            else
            {
                return $"{parentId}_{type}";
            }
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

        /// <summary>
        /// Function used to return the DataItem Type in Pascal case. Handles types with acronyms.
        /// </summary>
        public static string GetPascalCaseType(string type)
        {
            switch (type)
            {
                case DataItems.Events.AdapterUriDataItem.TypeId: return "AdapterURI";
                case DataItems.Events.MTConnectVersionDataItem.TypeId: return "MTConnectVersion";
            }

            return type.ToPascalCase();
        }


        public static IDataItem Create(IDataItem dataItem)
        {
            var di = Create(dataItem.Type);
            di.Category = dataItem.Category;
            di.Id = dataItem.Id;
            di.Name = dataItem.Name;
            di.Type = dataItem.Type;
            di.SubType = dataItem.SubType;
            di.NativeUnits = dataItem.NativeUnits;
            di.NativeScale = dataItem.NativeScale;
            di.SampleRate = dataItem.SampleRate;
            di.Source = dataItem.Source;
            di.Relationships = dataItem.Relationships;
            di.Representation = dataItem.Representation;
            di.ResetTrigger = dataItem.ResetTrigger;
            di.CoordinateSystem = dataItem.CoordinateSystem;
            di.CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;
            di.CompositionId = dataItem.CompositionId;
            di.Constraints = dataItem.Constraints;
            di.Definition = dataItem.Definition;
            di.Units = dataItem.Units;
            di.Statistic = dataItem.Statistic;
            di.SignificantDigits = dataItem.SignificantDigits;
            di.Filters = dataItem.Filters;
            di.InitialValue = dataItem.InitialValue;
            di.Discrete = dataItem.Discrete;
            return di;
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

            return new DataItem();
        }

        private static Dictionary<string, Type> GetAllTypes()
        {
            var assemblies = Assemblies.Get();
            if (!assemblies.IsNullOrEmpty())
            {
                var types = assemblies
                    .SelectMany(
                        x => x.GetMatchingTypesInAssembly(
                            t => typeof(DataItem).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));

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


        public static IDataItem Process(IDataItem dataItem, Version mtconnectVersion)
        {
            if (dataItem != null)
            {
                // Don't return if Condition and Version < 1.1
                if (dataItem.Category == DataItemCategory.CONDITION && mtconnectVersion < MTConnectVersions.Version11) return null;

                // Don't return if TimeSeries and Version < 1.2
                if (dataItem.Representation == DataItemRepresentation.TIME_SERIES && mtconnectVersion < MTConnectVersions.Version12) return null;

                // Don't return if Discrete and Version < 1.3 OR Version >= 1.5
                if (dataItem.Representation == DataItemRepresentation.DISCRETE && (mtconnectVersion < MTConnectVersions.Version13 || mtconnectVersion >= MTConnectVersions.Version15)) return null;

                // Don't return if DataSet and Version < 1.3
                if (dataItem.Representation == DataItemRepresentation.DATA_SET && mtconnectVersion < MTConnectVersions.Version13) return null;

                // Don't return if Table and Version < 1.6
                if (dataItem.Representation == DataItemRepresentation.TABLE && mtconnectVersion < MTConnectVersions.Version16) return null;

                // Create a new Instance of the DataItem that will instantiate a new Derived class (if found)
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
                    obj.Units = dataItem.Units;
                    obj.SignificantDigits = dataItem.SignificantDigits;

                    // Check SampleRate
                    if (mtconnectVersion >= MTConnectVersions.Version12) obj.SampleRate = dataItem.SampleRate;

                    // Check Source
                    if (dataItem.Source != null && mtconnectVersion >= MTConnectVersions.Version12)
                    {
                        var source = new Source();
                        source.ComponentId = dataItem.Source.ComponentId;
                        if (mtconnectVersion >= MTConnectVersions.Version14) source.CompositionId = dataItem.Source.CompositionId;
                        source.DataItemId = dataItem.Source.DataItemId;
                        obj.Source = source;
                    }

                    // Check Relationships
                    obj.Relationships = dataItem.Relationships;
                    if (dataItem.Relationships != null && mtconnectVersion >= MTConnectVersions.Version15)
                    {
                        var relationships = new List<Relationship>();
                        foreach (var relationship in dataItem.Relationships)
                        {
                            // Component Relationship
                            if (relationship.GetType() == typeof(ComponentRelationship))
                            {
                                relationships.Add(relationship);
                            }

                            // DataItem Relationship
                            if (relationship.GetType() == typeof(DataItemRelationship))
                            {
                                if (mtconnectVersion >= MTConnectVersions.Version17) relationships.Add(relationship);
                            }

                            // Device Relationship
                            if (relationship.GetType() == typeof(DeviceRelationship))
                            {
                                relationships.Add(relationship);
                            }

                            // Specification Relationship
                            if (relationship.GetType() == typeof(SpecificationRelationship))
                            {
                                if (mtconnectVersion >= MTConnectVersions.Version17) relationships.Add(relationship);
                            }
                        }

                        obj.Relationships = relationships;
                    }

                    // Check Representation
                    if (mtconnectVersion >= MTConnectVersions.Version12) obj.Representation = dataItem.Representation;

                    // Check ResetTrigger
                    if (mtconnectVersion >= MTConnectVersions.Version14) obj.ResetTrigger = dataItem.ResetTrigger;

                    // Check CoordinateSystem
                    if (mtconnectVersion < MTConnectVersions.Version20) obj.CoordinateSystem = dataItem.CoordinateSystem;

                    // Check CoordinateSystemIdRef
                    if (mtconnectVersion >= MTConnectVersions.Version15) obj.CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;

                    // Check CompositionId
                    if (mtconnectVersion >= MTConnectVersions.Version14)
                    {                       
                        obj.CompositionId = dataItem.CompositionId;
                    }
                    else if (!string.IsNullOrEmpty(dataItem.CompositionId))
                    {
                        // Don't return if Composition not compatible with Version as this could cause duplicate Types within the same Component
                        return null;
                    }

                    // Check Constraints
                    if (mtconnectVersion >= MTConnectVersions.Version11) obj.Constraints = dataItem.Constraints;

                    // Check Definition
                    if (mtconnectVersion >= MTConnectVersions.Version16) obj.Definition = dataItem.Definition;

                    // Check Statistic
                    if (mtconnectVersion >= MTConnectVersions.Version12) obj.Statistic = dataItem.Statistic;

                    // Check Filters
                    if (mtconnectVersion >= MTConnectVersions.Version13) obj.Filters = dataItem.Filters;

                    // Check InitialValue
                    if (mtconnectVersion >= MTConnectVersions.Version14) obj.InitialValue = dataItem.InitialValue;

                    // Check Discrete
                    if (mtconnectVersion >= MTConnectVersions.Version15) obj.Discrete = dataItem.Discrete;
                }

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
