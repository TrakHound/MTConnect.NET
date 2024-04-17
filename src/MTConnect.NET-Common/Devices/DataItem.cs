// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations.Relationships;
using MTConnect.Devices.DataItems;
using MTConnect.Extensions;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly Version DefaultMaximumVersion = null;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;
        private static readonly Dictionary<string, string> _typeIds = new Dictionary<string, string>();
        private static readonly object _lock = new object();

        private static Dictionary<string, Type> _types;


        /// <summary>
        /// Specifies the kind of information provided by a data item.
        /// Each category of information will provide similar characteristics in its representation.
        /// The available options are SAMPLE, EVENT, or CONDITION.
        /// </summary>
        public DataItemCategory Category { get; set; }

        /// <summary>
        /// The unique identifier for this DataItem.
        /// The id attribute MUST be unique across the entire document including the ids for components.
        /// An XML ID-type.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The type of data being measured.
        /// Examples of types are POSITION, VELOCITY, ANGLE, BLOCK, ROTARY_VELOCITY, etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The coordinate system being used.
        /// The available values for coordinateSystem are WORK and MACHINE.
        /// </summary>
        public DataItemCoordinateSystem CoordinateSystem { get; set; }

        /// <summary>
        /// The associated CoordinateSystem context for the DataItem.
        /// </summary>
        public string CoordinateSystemIdRef { get; set; }

        /// <summary>
        /// The name of the DataItem. A name is provided as an additional human readable identifier for this DataItem in addtion to the id.
        /// It is not required and will be implementation dependent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The multiplier for the native units.
        /// The received data MAY be divided by this value before conversion.
        /// If provided, the value MUST be numeric.
        /// </summary>
        public double NativeScale { get; set; }

        /// <summary>
        /// The native units used by the Component.
        /// These units will be converted before they are delivered to the application.
        /// </summary>
        public string NativeUnits { get; set; }

        /// <summary>
        /// A sub-categorization of the data item type.
        /// For example, the Sub-types of POSITION can be ACTUAL or COMMANDED.
        /// Not all types have subTypes and they can be optional.
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// Data calculated specific to a DataItem.
        /// Examples of statistic are AVERAGE, MINIMUM, MAXIMUM, ROOT_MEAN_SQUARE, RANGE, MEDIAN, MODE and STANDARD_DEVIATION.
        /// </summary>
        public virtual DataItemStatistic Statistic { get; set; }

        /// <summary>
        /// Units MUST be present for all DataItem elements in the SAMPLE category.
        /// If the data represented by a DataItem is a numeric value, except for line number and count, the units MUST be specified.
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// The reate at which successive samples of a DataItem are recorded.
        /// SampleRate is expressed in terms of samples per second.
        /// If the SampleRate is smaller than one, the number can be represented as a floating point number.
        /// For example, a rate 1 per 10 seconds would be 0.1.
        /// </summary>
        public double SampleRate { get; set; }

        /// <summary>
        /// An indication signifying whether each value reported for the Data Entity is significant and whether duplicate values are to be suppressed.
        /// </summary>
        public bool Discrete { get; set; }

        /// <summary>
        /// Data consisting of multiple data points or samples or a file presented as a single DataItem.
        /// Each representation will have a unique format defined for each representation. 
        /// Examples or representation are VALUE, TIME_SERIES, DISCRETE, MP3, WAV, etc.
        /// Initially, the represenation for TIME_SERIES, DISCRETE, and VALUE are defined.
        /// If a representation is not specified, it MUST be determined to be a VALUE.
        /// </summary>
        public DataItemRepresentation Representation { get; set; }

        /// <summary>
        /// The number of significant digits in the reported value.
        /// This is used by applications to dtermine accuracy of values.
        /// This SHOULD be specified for all numeric values.
        /// </summary>
        public int SignificantDigits { get; set; }

        /// <summary>
        /// The identifier attribute of the Composition element that the reported data is most closely associated.
        /// </summary>
        public string CompositionId { get; set; }

        /// <summary>
        /// Source is an XML element that indentifies the Component, Subcomponent, or DataItem representing the part of the device from which a measured value originates.
        /// </summary>
        public ISource Source { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        public IConstraints Constraints { get; set; }

        /// <summary>
        /// The set of possible values that can be assigned to this DataItem.
        /// </summary>
        public IEnumerable<IFilter> Filters { get; set; }

        /// <summary>
        /// InitialValue is an optional XML element that defines the starting value for a data item as well as the value to be set for the data item after a reset event.
        /// </summary>
        public string InitialValue { get; set; }

        /// <summary>
        /// ResetTrigger is an XML element that describes the reset action that causes a reset to occur.
        /// </summary>
        public DataItemResetTrigger ResetTrigger { get; set; }

        /// <summary>
        /// The Definition provides additional descriptive information for any DataItem representations.
        /// When the representation is either DATA_SET or TABLE, it gives the specific meaning of a key and MAY provide a Description, type, and units for semantic interpretation of data.
        /// </summary>
        public IDataItemDefinition Definition { get; set; }

        /// <summary>
        /// Relationships organizes DataItemRelationship and SpecificationRelationship.
        /// </summary>
        public IEnumerable<IRelationship> Relationships { get; set; }


        /// <summary>
        /// The Device that this DataItem is associated with
        /// </summary>
        public IDevice Device { get; set; }

        /// <summary>
        /// The Container (Component or Device) that this DataItem is directly associated with
        /// </summary>
        public IContainer Container { get; set; }


		private string _hash;
		/// <summary>
		/// Condensed message digest from a secure one-way hash function. FIPS PUB 180-4
		/// </summary>
		public string Hash
		{
			get
			{
				if (_hash == null) _hash = GenerateHash();
				return _hash;
			}
		}

		///// <summary>
		///// A MD5 Hash of the DataItem that can be used to compare DataItem objects
		///// </summary>
		//public string ChangeId => CreateChangeId();

		/// <summary>
		/// The text description that describes what the DataItem Type represents
		/// </summary>
		public virtual string TypeDescription => DescriptionText;

        /// <summary>
        /// The text description that describes what the DataItem SubType represents
        /// </summary>
        public virtual string SubTypeDescription => null;


        /// <summary>
        /// The full path of IDs that describes the location of the DataItem in the Device
        /// </summary>
        public string IdPath => GenerateIdPath(this);

        /// <summary>
        /// The list of IDs (in order) that describes the location of the DataItem in the Device
        /// </summary>
        public string[] IdPaths => GenerateIdPaths(this);

        /// <summary>
        /// The full path of Types that describes the location of the DataItem in the Device
        /// </summary>
        public string TypePath => GenerateTypePath(this);

        /// <summary>
        /// The list of Types (in order) that describes the location of the DataItem in the Device
        /// </summary>
        public string[] TypePaths => GenerateTypePaths(this);


        /// <summary>
        /// The maximum MTConnect Version that this DataItem Type is valid 
        /// (if set, this indicates that the Type has been Deprecated in the MTConnect Standard version specified)
        /// </summary>
        public virtual Version MaximumVersion => DefaultMaximumVersion;

        /// <summary>
        /// The minimum MTConnect Version that this DataItem Type is valid 
        /// </summary>
        public virtual Version MinimumVersion => DefaultMinimumVersion;


        public DataItem()
        {
            Init();
        }

        public DataItem(DataItemCategory category, string type, string subType = null, string dataItemId = null)
        {
            Init();

            Id = dataItemId;
            Category = category;
            Type = type;
            SubType = subType;
        }

        public DataItem(IDataItem dataItem)
        {
            Init();

            if (dataItem != null)
            {
                Category = dataItem.Category;
                Id = dataItem.Id;
                Name = dataItem.Name;
                Type = dataItem.Type;
                SubType = dataItem.SubType;
                NativeUnits = dataItem.NativeUnits;
                NativeScale = dataItem.NativeScale;
                SampleRate = dataItem.SampleRate;
                Source = dataItem.Source;
                Relationships = dataItem.Relationships;
                Representation = dataItem.Representation;
                ResetTrigger = dataItem.ResetTrigger;
                CoordinateSystem = dataItem.CoordinateSystem;
                CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;
                CompositionId = dataItem.CompositionId;
                Constraints = dataItem.Constraints;
                Definition = dataItem.Definition;
                Units = dataItem.Units;
                Statistic = dataItem.Statistic;
                SignificantDigits = dataItem.SignificantDigits;
                Filters = dataItem.Filters;
                InitialValue = dataItem.InitialValue;
                Discrete = dataItem.Discrete;
            }
        }

        private void Init()
        {
            Filters = new List<Filter>();
            Relationships = new List<Relationship>();
        }


        public string GenerateHash()
        {
            return GenerateHash(this);
        }

        public static string GenerateHash(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                var ids = new List<string>();
                ids.Add(ObjectExtensions.GetHashPropertyString(dataItem).ToSHA1Hash());

                // Add Relationship Change ID's
                if (!dataItem.Relationships.IsNullOrEmpty())
                {
                    foreach (var relationship in dataItem.Relationships)
                    {
                        ids.Add(relationship.Hash);
                    }
                }

                return StringFunctions.ToSHA1Hash(ids.ToArray());
            }

            return null;
        }


        public static string[] GenerateIdPaths(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                var types = new List<string>();

                if (dataItem.Container != null && !dataItem.Container.IdPaths.IsNullOrEmpty())
                {
                    types.AddRange(dataItem.Container.IdPaths);
                }

                types.Add(dataItem.Id);

                return types.ToArray();
            }

            return null;
        }

        public static string GenerateIdPath(IDataItem dataItem)
        {
            if (dataItem != null && !dataItem.IdPaths.IsNullOrEmpty())
            {
                return string.Join("/", dataItem.IdPaths);
            }

            return null;
        }

        public static string[] GenerateTypePaths(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                var types = new List<string>();

                if (dataItem.Container != null && !dataItem.Container.TypePaths.IsNullOrEmpty())
                {
                    types.AddRange(dataItem.Container.TypePaths);
                }

                types.Add(dataItem.Type);

                return types.ToArray();
            }

            return null;
        }

        public static string GenerateTypePath(IDataItem dataItem)
        {
            if (dataItem != null && !dataItem.TypePaths.IsNullOrEmpty())
            {
                return string.Join("/", dataItem.TypePaths);
            }

            return null;
        }


        public IDataItem Process(Version mtconnectVersion)
        {
            return Process(this, mtconnectVersion);
        }

        protected virtual IDataItem OnProcess(IDataItem dataItem, Version mtconnectVersion)
        {
            return dataItem;
        }


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        public ValidationResult IsValid(Version mtconnectVersion, IObservationInput observation)
        {
            var result = new ValidationResult(true);

            switch (Category)
            {
                // Validate Sample
                case DataItemCategory.SAMPLE:
                    var sampleValidation = ValidateSample(mtconnectVersion, observation);
                    if (!sampleValidation.IsValid) result = sampleValidation;
                    break;

                // Validate Event
                case DataItemCategory.EVENT:
                    var eventValidation = ValidateEvent(mtconnectVersion, observation);
                    if (!eventValidation.IsValid) result = eventValidation;
                    break;

                // Validate Condition
                case DataItemCategory.CONDITION:
                    var conditionValidation = ValidateCondition(mtconnectVersion, observation);
                    if (!conditionValidation.IsValid) result = conditionValidation;
                    break;
            }       
            
            return result;
        }

        private ValidationResult ValidateSample(Version mtconnectVersion, IObservationInput observation)
        {
            // Get the Result Value for the Observation
            var result = observation.GetValue(ValueKeys.Result);
            if (result != null)
            {
                // Check if Unavailable
                if (result == Observation.Unavailable) return new ValidationResult(true);
            }

            return OnValidation(mtconnectVersion, observation);
        }

        private ValidationResult ValidateEvent(Version mtconnectVersion, IObservationInput observation)
        {
            // Get the Result Value for the Observation
            var result = observation.GetValue(ValueKeys.Result);
            if (result != null)
            {
                // Check if Unavailable
                if (result == Observation.Unavailable) return new ValidationResult(true);
            }

            return OnValidation(mtconnectVersion, observation);
        }

        private ValidationResult ValidateCondition(Version mtconnectVersion, IObservationInput observation)
        {
            // Get the Level Value for the Observation
            var level = observation.GetValue(ValueKeys.Level).ConvertEnum<ConditionLevel>();

            // Check if Unavailable
            if (level == ConditionLevel.UNAVAILABLE) return new ValidationResult(true);

            return OnValidation(mtconnectVersion, observation);
        }

        protected virtual ValidationResult OnValidation(Version mtconnectVerion, IObservationInput observation)
        {
            return new ValidationResult(true);
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
            if (!string.IsNullOrEmpty(type))
            {
                string typeId;

                switch (type)
                {
                    case DataItems.Events.AdapterUriDataItem.TypeId: return "AdapterURI";
                    case DataItems.Events.MTConnectVersionDataItem.TypeId: return "MTConnectVersion";
                }

                lock (_lock)
                {
                    _typeIds.TryGetValue(type, out typeId);
                    if (typeId == null)
                    {
                        typeId = type.ToPascalCase();
                        _typeIds.Add(type, typeId);
                    }
                }

                return typeId;
            }

            return null;
        }


        public static DataItem Create(IDataItem dataItem)
        {
            var type = GetDataItemType(dataItem.Type);
            if (type != dataItem.GetType())
            {
                var di = Create(type);
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

            return (DataItem)dataItem;
        }

        public static DataItem Create(string type)
        {
            var t = GetDataItemType(type);
            return Create(t);
        }

        public static DataItem Create(Type type)
        {
            if (type != null)
            {
                var constructor = type.GetConstructor(System.Type.EmptyTypes);
                if (constructor != null)
                {
                    try
                    {
                        return (DataItem)Activator.CreateInstance(type);
                    }
                    catch { }
                }
            }          

            return new DataItem();
        }

        private static Type GetDataItemType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                lock (_lock)
                {
                    // Initialize Type List
                    if (_types == null) _types = GetAllTypes();

                    _typeIds.TryGetValue(type, out string typeId);
                    if (typeId == null)
                    {
                        typeId = type.ToPascalCase();
                        _typeIds.Add(type, typeId);
                    }

                    if (_types.TryGetValue(typeId, out Type t))
                    {
                        return t;
                    }
                }
            }

            return typeof(DataItem);
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


        public static bool IsCompatible(IDataItem dataItem, Version mtconnectVersion)
        {
            if (dataItem != null)
            {
                return dataItem.MinimumVersion != null && mtconnectVersion >= dataItem.MinimumVersion;
            }

            return false;
        }

        public static IDataItem Process(IDataItem dataItem, Version mtconnectVersion)
        {
            if (dataItem != null)
            {
                // Check Version Compatibilty
                if (dataItem.MinimumVersion != null && mtconnectVersion < dataItem.MinimumVersion) return null;

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
                    obj.Container = dataItem.Container;
                    obj.Device = dataItem.Device;

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
                        var relationships = new List<IRelationship>();
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

                // Call overridable method (used to process based on Type)
                return obj.OnProcess(obj, mtconnectVersion);
            }

            return null;
        }
    }
}