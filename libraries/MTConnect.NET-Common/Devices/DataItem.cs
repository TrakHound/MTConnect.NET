// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
using MTConnect.Devices.DataItems;
using MTConnect.Extensions;
using MTConnect.Input;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTConnect.Devices
{
    public partial class DataItem : IDataItem
    {
        private static readonly Version DefaultMaximumVersion = null;
        private static readonly Version DefaultMinimumVersion = MTConnectVersions.Version10;
        private static readonly Dictionary<string, string> _typeIds = new Dictionary<string, string>();
        private static readonly object _lock = new object();

        private static Dictionary<string, Type> _types;
        private static IEnumerable<string> _conditionTypes;
        private static IEnumerable<string> _eventTypes;
        private static IEnumerable<string> _sampleTypes;
        private static Dictionary<string, string> _typeDescriptions;
        private static Dictionary<string, IEnumerable<string>> _subtypes;
        private static Dictionary<string, string> _subtypeDescriptions;
        private static Dictionary<string, string> _defaultNames;
        private static Dictionary<string, string> _defaultUnits;
        private static Dictionary<string, int> _defaultSignificantDigits;


        internal string _uuid;
        /// <summary>
        /// The UUID of the MTConnect Entity
        /// </summary>
        public string Uuid
        {
            get
            {
                _uuid = Device != null ? $"{Device.Uuid}.{Id}" : Id;
                return _uuid;
            }
            set => _uuid = value;
        }

        /// <summary>
        /// The MTConnect entity classification for this object, which is always <see cref="MTConnectEntityType.DataItem"/>.
        /// </summary>
        public MTConnectEntityType EntityType => MTConnectEntityType.DataItem;

        /// <summary>
        /// The Device that this DataItem is associated with
        /// </summary>
        public IDevice Device { get; set; }

        /// <summary>
        /// The Container (Component or Device) that this DataItem is directly associated with
        /// </summary>
        public IContainer Container { get; set; }

        /// <summary>
        /// The Agent InstanceId that produced this Device
        /// </summary>
        public ulong InstanceId { get; set; }


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

        internal bool _isExtended;
        /// <summary>
        /// Indicates that this DataItem uses a Type or SubType that is not defined by the MTConnect Standard,
        /// meaning it was created from an unrecognized (vendor- or application-specific) extension value.
        /// </summary>
        public bool IsExtended => _isExtended;


        /// <summary>
        /// Initializes a new, empty DataItem with its default Type registrations loaded.
        /// </summary>
        public DataItem()
        {
            Init();
        }

        /// <summary>
        /// Initializes a new DataItem with the specified category, type, and optional subtype and identifier.
        /// </summary>
        /// <param name="category">The category (Sample, Event, or Condition) of the DataItem.</param>
        /// <param name="type">The Type that defines what the DataItem represents.</param>
        /// <param name="subType">An optional SubType that further refines the meaning of the Type.</param>
        /// <param name="dataItemId">An optional unique identifier for the DataItem.</param>
        public DataItem(DataItemCategory category, string type, string subType = null, string dataItemId = null)
        {
            Init();

            Id = dataItemId;
            Category = category;
            Type = type;
            SubType = subType;
        }

        /// <summary>
        /// Initializes a new DataItem by copying every property from an existing DataItem instance.
        /// </summary>
        /// <param name="dataItem">The DataItem to copy. When null, an empty DataItem is produced.</param>
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
                _isExtended = dataItem.IsExtended;
            }
        }

        private void Init()
        {
            Representation = DataItemRepresentation.VALUE;
            Filters = new List<Filter>();
            Relationships = new List<IAbstractDataItemRelationship>();
        }


        /// <summary>
        /// Computes a content hash for this DataItem that changes whenever any hashed property or relationship changes.
        /// </summary>
        /// <returns>A SHA-1 hash string identifying the current state of this DataItem.</returns>
        public string GenerateHash()
        {
            return GenerateHash(this);
        }

        /// <summary>
        /// Computes a content hash for the specified DataItem, incorporating its hashed properties and the hashes of its relationships.
        /// </summary>
        /// <param name="dataItem">The DataItem to hash.</param>
        /// <returns>A SHA-1 hash string, or null when <paramref name="dataItem"/> is null.</returns>
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


        /// <summary>
        /// Builds the ordered list of Id segments from the containing hierarchy down to the DataItem itself.
        /// </summary>
        /// <param name="dataItem">The DataItem whose Id path is built.</param>
        /// <returns>The Id segments from the outermost container to the DataItem, or null when <paramref name="dataItem"/> is null.</returns>
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

        /// <summary>
        /// Builds the slash-delimited Id path string from the containing hierarchy down to the DataItem itself.
        /// </summary>
        /// <param name="dataItem">The DataItem whose Id path is built.</param>
        /// <returns>The Id segments joined with '/', or null when no path is available.</returns>
        public static string GenerateIdPath(IDataItem dataItem)
        {
            if (dataItem != null && !dataItem.IdPaths.IsNullOrEmpty())
            {
                return string.Join("/", dataItem.IdPaths);
            }

            return null;
        }

        /// <summary>
        /// Builds the ordered list of Type segments from the containing hierarchy down to the DataItem itself.
        /// </summary>
        /// <param name="dataItem">The DataItem whose Type path is built.</param>
        /// <returns>The Type segments from the outermost container to the DataItem, or null when <paramref name="dataItem"/> is null.</returns>
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

        /// <summary>
        /// Builds the slash-delimited Type path string from the containing hierarchy down to the DataItem itself.
        /// </summary>
        /// <param name="dataItem">The DataItem whose Type path is built.</param>
        /// <returns>The Type segments joined with '/', or null when no path is available.</returns>
        public static string GenerateTypePath(IDataItem dataItem)
        {
            if (dataItem != null && !dataItem.TypePaths.IsNullOrEmpty())
            {
                return string.Join("/", dataItem.TypePaths);
            }

            return null;
        }


        /// <summary>
        /// Produces a copy of this DataItem adjusted for the specified MTConnect version, dropping members not valid in that version.
        /// </summary>
        /// <param name="mtconnectVersion">The target MTConnect Standard version.</param>
        /// <returns>The version-adjusted DataItem, or null when this DataItem is not valid in that version.</returns>
        public DataItem Process(Version mtconnectVersion)
        {
            return Process(this, mtconnectVersion);
        }

        /// <summary>
        /// Hook for derived DataItem types to apply type-specific adjustments during version processing.
        /// The base implementation returns the DataItem unchanged.
        /// </summary>
        /// <param name="dataItem">The DataItem being processed.</param>
        /// <param name="mtconnectVersion">The target MTConnect Standard version.</param>
        /// <returns>The adjusted DataItem.</returns>
        protected virtual DataItem OnProcess(DataItem dataItem, Version mtconnectVersion)
        {
            return dataItem;
        }


        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        public ValidationResult Validate(Version mtconnectVersion, IObservationInput observation)
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

        /// <summary>
        /// Determine if the DataItem with the specified Observation is valid in the specified MTConnectVersion
        /// </summary>
        /// <param name="mtconnectVersion">The Version of the MTConnect Standard</param>
        /// <param name="observation">The Observation to validate</param>
        /// <returns>A DataItemValidationResult indicating if Validation was successful and a Message</returns>
        public ValidationResult Validate(Version mtconnectVersion, IObservation observation)
        {
            var result = new ValidationResult(false);

            if (observation != null)
            {
                result = new ValidationResult(true);

                // Check for valid Sequence number
                if (observation.Sequence < 1)
                {
                    result = new ValidationResult(false, "Invalid Sequence Number : Sequence MUST be greater than or equal to \"1\"");
                    return result;
                }

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

        private ValidationResult ValidateSample(Version mtconnectVersion, IObservation observation)
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

        private ValidationResult ValidateEvent(Version mtconnectVersion, IObservation observation)
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

        private ValidationResult ValidateCondition(Version mtconnectVersion, IObservation observation)
        {
            // Get the Level Value for the Observation
            var level = observation.GetValue(ValueKeys.Level).ConvertEnum<ConditionLevel>();

            // Check if Unavailable
            if (level == ConditionLevel.UNAVAILABLE) return new ValidationResult(true);

            return OnValidation(mtconnectVersion, observation);
        }

        /// <summary>
        /// Hook for derived DataItem types to apply type-specific validation of an input value.
        /// The base implementation treats every value as valid.
        /// </summary>
        /// <param name="mtconnectVerion">The MTConnect Standard version to validate against.</param>
        /// <param name="observation">The observation input being validated.</param>
        /// <returns>The validation result for the type-specific check.</returns>
        protected virtual ValidationResult OnValidation(Version mtconnectVerion, IObservationInput observation)
        {
            return new ValidationResult(true);
        }

        /// <summary>
        /// Hook for derived DataItem types to apply type-specific validation of an existing observation.
        /// The base implementation treats every observation as valid.
        /// </summary>
        /// <param name="mtconnectVerion">The MTConnect Standard version to validate against.</param>
        /// <param name="observation">The observation being validated.</param>
        /// <returns>The validation result for the type-specific check.</returns>
        protected virtual ValidationResult OnValidation(Version mtconnectVerion, IObservation observation)
        {
            return new ValidationResult(true);
        }


        /// <summary>
        /// Builds a DataItem Id by combining a parent Id with a name segment.
        /// </summary>
        /// <param name="parentId">The Id of the containing Component or Device.</param>
        /// <param name="name">The name segment to append.</param>
        /// <returns>The composed Id in the form "parentId_name".</returns>
        public static string CreateId(string parentId, string name)
        {
            return $"{parentId}_{name}";
        }

        /// <summary>
        /// Builds a DataItem Id from a parent Id and the DataItem's type and optional subtype.
        /// </summary>
        /// <param name="parentId">The Id of the containing Component or Device.</param>
        /// <param name="type">The DataItem Type.</param>
        /// <param name="subType">An optional SubType included in the Id when present.</param>
        /// <returns>The composed Id, or null when <paramref name="parentId"/> or <paramref name="type"/> is empty.</returns>
        public static string CreateDataItemId(string parentId, string type, string subType = null)
        {
            if (!string.IsNullOrEmpty(parentId) && !string.IsNullOrEmpty(type))
            {
                if (!string.IsNullOrEmpty(subType))
                {
                    return $"{parentId}_{type.ToLower()}_{subType.ToLower()}";
                }
                else
                {
                    return $"{parentId}_{type.ToLower()}";
                }
            }

            return null;
        }

        /// <summary>
        /// Builds a DataItem Id from a parent Id, a name segment, and an optional suffix.
        /// </summary>
        /// <param name="parentId">The Id of the containing Component or Device.</param>
        /// <param name="name">The name segment to append.</param>
        /// <param name="suffix">An optional suffix appended after the name when present.</param>
        /// <returns>The composed Id.</returns>
        public static string CreateId(string parentId, string name, string suffix)
        {
            if (!string.IsNullOrEmpty(name))
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

            return null;
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
                    case AdapterUriDataItem.TypeId: return "AdapterURI";
                    case MTConnectVersionDataItem.TypeId: return "MTConnectVersion";
                    case AmperageACDataItem.TypeId: return "AmperageAC";
                    case AmperageDCDataItem.TypeId: return "AmperageDC";
                    case VoltageACDataItem.TypeId: return "VoltageAC";
                    case VoltageDCDataItem.TypeId: return "VoltageDC";
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


        /// <summary>
        /// Creates the most-derived DataItem subclass matching the given DataItem's Type and copies its properties.
        /// </summary>
        /// <param name="dataItem">The source DataItem to recreate as its concrete type.</param>
        /// <returns>A typed DataItem instance, or a base DataItem when the Type is not recognized.</returns>
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
                di._isExtended = type == typeof(DataItem);
                return di;
            }

            return (DataItem)dataItem;
        }

        /// <summary>
        /// Creates an instance of the DataItem subclass registered for the specified Type identifier.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <returns>A new typed DataItem instance, or null when the Type is not recognized.</returns>
        public static DataItem Create(string type)
        {
            var t = GetDataItemType(type);
            return Create(t);
        }

        /// <summary>
        /// Creates an instance of the specified DataItem CLR type using its parameterless constructor.
        /// </summary>
        /// <param name="type">The concrete DataItem CLR type to instantiate.</param>
        /// <returns>A new DataItem instance, or null when instantiation fails.</returns>
        public static DataItem Create(Type type)
        {
            if (type != null)
            {
                try
                {
                    var constructor = type.GetConstructor(System.Type.EmptyTypes);
                    if (constructor != null)
                    {

                        return (DataItem)Activator.CreateInstance(type);
                    }
                }
                catch { }
            }

            return new DataItem();
        }

        /// <summary>
        /// Returns every DataItem Type identifier known to the library across all categories.
        /// </summary>
        /// <returns>The registered DataItem Type identifiers.</returns>
        public static IEnumerable<string> GetTypes()
        {
            lock (_lock)
            {
                if (_types == null) _types = GetAllTypes();

                return _types.Keys;
            }
        }

        /// <summary>
        /// Returns every DataItem Type identifier that belongs to the Condition category.
        /// </summary>
        /// <returns>The registered Condition-category Type identifiers.</returns>
        public static IEnumerable<string> GetConditionTypes()
        {
            if (_conditionTypes == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    var x = new List<string>();

                    foreach (var type in _types)
                    {
                        var instance = Create(type.Value);
                        if (instance != null)
                        {
                            x.Add(instance.Type);
                        }
                    }

                    _conditionTypes = x.OrderBy(o => o);
                }
            }

            return _conditionTypes;
        }

        /// <summary>
        /// Returns every DataItem Type identifier that belongs to the Event category.
        /// </summary>
        /// <returns>The registered Event-category Type identifiers.</returns>
        public static IEnumerable<string> GetEventTypes()
        {
            if (_eventTypes == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    var x = new List<string>();

                    foreach (var type in _types)
                    {
                        var instance = Create(type.Value);
                        if (instance != null && instance.Category == DataItemCategory.EVENT)
                        {
                            x.Add(instance.Type);
                        }
                    }

                    _eventTypes = x.OrderBy(o => o);
                }
            }

            return _eventTypes;
        }

        /// <summary>
        /// Returns every DataItem Type identifier that belongs to the Sample category.
        /// </summary>
        /// <returns>The registered Sample-category Type identifiers.</returns>
        public static IEnumerable<string> GetSampleTypes()
        {
            if (_sampleTypes == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    var x = new List<string>();

                    foreach (var type in _types)
                    {
                        var instance = Create(type.Value);
                        if (instance != null && instance.Category == DataItemCategory.SAMPLE)
                        {
                            x.Add(instance.Type);
                        }
                    }

                    _sampleTypes = x.OrderBy(o => o);
                }
            }

            return _sampleTypes;
        }

        /// <summary>
        /// Returns the human-readable description for every known DataItem Type, keyed by Type identifier.
        /// </summary>
        /// <returns>Pairs of Type identifier and its description text.</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetTypeDescriptions()
        {
            if (_typeDescriptions == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    _typeDescriptions = new Dictionary<string, string>();

                    foreach (var type in _types)
                    {
                        var instance = Create(type.Value);
                        if (instance != null)
                        {
                            _typeDescriptions.Remove(instance.Type);
                            _typeDescriptions.Add(instance.Type, instance.TypeDescription);
                        }
                    }
                }
            }

            return _typeDescriptions;
        }

        /// <summary>
        /// Returns the set of valid SubType identifiers for every known DataItem Type, keyed by Type identifier.
        /// </summary>
        /// <returns>Pairs of Type identifier and its collection of valid SubType identifiers.</returns>
        public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetSubTypes()
        {
            if (_subtypes == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    _subtypes = new Dictionary<string, IEnumerable<string>>();

                    foreach (var type in _types)
                    {
                        var enumType = type.Value.GetNestedType("SubTypes");
                        if (enumType != null)
                        {
                            var enumValues = Enum.GetValues(enumType);
                            if (enumValues != null)
                            {
                                var values = new List<string>();

                                foreach (var enumValue in enumValues)
                                {
                                    values.Add(enumValue.ToString());
                                }

                                _subtypes.Remove(type.Key);
                                _subtypes.Add(type.Key, values);
                            }
                        }
                    }
                }
            }

            return _subtypes;
        }

        /// <summary>
        /// Returns the human-readable description for every known DataItem SubType, keyed by "Type.SubType".
        /// </summary>
        /// <returns>Pairs of qualified SubType key and its description text.</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetSubTypeDescriptions()
        {
            if (_subtypeDescriptions == null)
            {
                bool typesFound;
                lock (_lock)
                {
                    if (_types == null) _types = GetAllTypes();
                    typesFound = !_types.IsNullOrEmpty();
                }

                if (typesFound)
                {
                    _subtypeDescriptions = new Dictionary<string, string>();

                    foreach (var type in _types)
                    {
                        var method = type.Value.GetMethod("GetSubTypeDescription");
                        if (method != null)
                        {
                            var enumType = type.Value.GetNestedType("SubTypes");
                            if (enumType != null)
                            {
                                var enumValues = Enum.GetValues(enumType);
                                if (enumValues != null)
                                {
                                    var values = new List<string>();

                                    foreach (var enumValue in enumValues)
                                    {
                                        var str = enumValue.ToString();

                                        var value = (string)method.Invoke(null, new object[] { str });
                                        if (value != null)
                                        {
                                            var key = $"{type.Key}.{str}";
                                            _subtypeDescriptions.Remove(key);
                                            _subtypeDescriptions.Add(key, value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return _subtypeDescriptions;
        }

        /// <summary>
        /// Returns the conventional default name for every known DataItem Type, keyed by Type identifier.
        /// </summary>
        /// <returns>Pairs of Type identifier and its default name.</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetDefaultNames()
        {
            if (_defaultNames == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    _defaultNames = new Dictionary<string, string>();

                    foreach (var type in _types)
                    {
                        var field = type.Value.GetField("NameId");
                        if (field != null)
                        {
                            var value = (string)field.GetRawConstantValue();
                            if (value != null)
                            {
                                _defaultNames.Remove(type.Key);
                                _defaultNames.Add(type.Key, value);
                            }
                        }
                    }
                }
            }

            return _defaultNames;
        }

        /// <summary>
        /// Returns the default measurement Units for every known DataItem Type, keyed by Type identifier.
        /// </summary>
        /// <returns>Pairs of Type identifier and its default Units.</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetDefaultUnits()
        {
            if (_defaultUnits == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    _defaultUnits = new Dictionary<string, string>();

                    foreach (var type in _types)
                    {
                        var field = type.Value.GetField("DefaultUnits");
                        if (field != null)
                        {
                            var value = (string)field.GetRawConstantValue();
                            if (value != null)
                            {
                                _defaultUnits.Remove(type.Key);
                                _defaultUnits.Add(type.Key, value);
                            }
                        }
                    }
                }
            }

            return _defaultUnits;
        }

        /// <summary>
        /// Returns the default significant-digit precision for every known DataItem Type, keyed by Type identifier.
        /// </summary>
        /// <returns>Pairs of Type identifier and its default significant-digit count.</returns>
        public static IEnumerable<KeyValuePair<string, int>> GetDefaultSignificantDigits()
        {
            if (_defaultSignificantDigits == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
                {
                    _defaultSignificantDigits = new Dictionary<string, int>();

                    foreach (var type in _types)
                    {
                        var field = type.Value.GetField("DefaultSignificantDigits");
                        if (field != null)
                        {
                            var value = (int)field.GetRawConstantValue();
                            if (value >= 0)
                            {
                                _defaultSignificantDigits.Remove(type.Key);
                                _defaultSignificantDigits.Add(type.Key, value);
                            }
                        }
                    }
                }
            }

            return _defaultSignificantDigits;
        }

        /// <summary>
        /// Resolves the concrete DataItem CLR type registered for the specified Type identifier.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <returns>The matching CLR type, or null when the Type is not recognized.</returns>
        public static Type GetDataItemType(string type)
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

        /// <summary>
        /// Returns the human-readable description for the specified DataItem Type.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <returns>The description text, or null when the Type is not recognized.</returns>
        public static string GetDataItemDescription(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var x = GetTypeDescriptions();
                if (!x.IsNullOrEmpty())
                {
                    return x.FirstOrDefault(o => o.Key == type).Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the valid SubType identifiers for the specified DataItem Type.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <returns>The valid SubType identifiers, or null when the Type has none or is not recognized.</returns>
        public static IEnumerable<string> GetDataItemSubTypes(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var x = GetSubTypes();
                if (!x.IsNullOrEmpty())
                {
                    return x.FirstOrDefault(o => o.Key == type).Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the human-readable description for a specific SubType of the specified DataItem Type.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <param name="subtype">The SubType identifier.</param>
        /// <returns>The description text, or null when the Type/SubType pair is not recognized.</returns>
        public static string GetDataItemSubTypeDescription(string type, string subtype)
        {
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(subtype))
            {
                var x = GetSubTypeDescriptions();
                if (!x.IsNullOrEmpty())
                {
                    return x.FirstOrDefault(o => o.Key == $"{type}.{subtype}").Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the conventional default name for the specified DataItem Type.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <returns>The default name, or null when the Type is not recognized.</returns>
        public static string GetDataItemDefaultName(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var x = GetDefaultNames();
                if (!x.IsNullOrEmpty())
                {
                    return x.FirstOrDefault(o => o.Key == type).Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the default measurement Units for the specified DataItem Type.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <returns>The default Units, or null when the Type has no defined Units or is not recognized.</returns>
        public static string GetDataItemDefaultUnits(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var x = GetDefaultUnits();
                if (!x.IsNullOrEmpty())
                {
                    return x.FirstOrDefault(o => o.Key == type).Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the default significant-digit precision for the specified DataItem Type.
        /// </summary>
        /// <param name="type">The MTConnect DataItem Type identifier.</param>
        /// <returns>The default significant-digit count, or zero when none is defined for the Type.</returns>
        public static int GetDataItemDefaultSignificantDigits(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                var x = GetDefaultSignificantDigits();
                if (!x.IsNullOrEmpty())
                {
                    return x.FirstOrDefault(o => o.Key == type).Value;
                }
            }

            return -1;
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


        /// <summary>
        /// Determines whether the specified DataItem is valid for use under the given MTConnect Standard version.
        /// </summary>
        /// <param name="dataItem">The DataItem to check.</param>
        /// <param name="mtconnectVersion">The target MTConnect Standard version.</param>
        /// <returns>True when the DataItem's version range includes <paramref name="mtconnectVersion"/>; otherwise false.</returns>
        public static bool IsCompatible(IDataItem dataItem, Version mtconnectVersion)
        {
            if (dataItem != null)
            {
                return dataItem.MinimumVersion != null && mtconnectVersion >= dataItem.MinimumVersion;
            }

            return false;
        }

        /// <summary>
        /// Produces a typed copy of the specified DataItem adjusted for the target MTConnect version,
        /// returning null when the DataItem is not valid in that version.
        /// </summary>
        /// <param name="dataItem">The source DataItem to process.</param>
        /// <param name="mtconnectVersion">The target MTConnect Standard version; when null, the latest version is assumed.</param>
        /// <returns>The version-adjusted DataItem, or null when it is not valid in that version.</returns>
        public static DataItem Process(IDataItem dataItem, Version mtconnectVersion = null)
        {
            if (dataItem != null)
            {
                var version = mtconnectVersion != null ? mtconnectVersion : MTConnectVersions.Max;

                // Check Version Compatibilty
                if (dataItem.MinimumVersion != null && version < dataItem.MinimumVersion) return null;

                // Don't return if Condition and Version < 1.1
                if (dataItem.Category == DataItemCategory.CONDITION && version < MTConnectVersions.Version11) return null;

                // Don't return if TimeSeries and Version < 1.2
                if (dataItem.Representation == DataItemRepresentation.TIME_SERIES && version < MTConnectVersions.Version12) return null;

                // Don't return if Discrete and Version < 1.3 OR Version >= 1.5
                if (dataItem.Representation == DataItemRepresentation.DISCRETE && (version < MTConnectVersions.Version13 || version >= MTConnectVersions.Version15)) return null;

                // Don't return if DataSet and Version < 1.3
                if (dataItem.Representation == DataItemRepresentation.DATA_SET && version < MTConnectVersions.Version13) return null;

                // Don't return if Table and Version < 1.6
                if (dataItem.Representation == DataItemRepresentation.TABLE && version < MTConnectVersions.Version16) return null;

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

                    obj._isExtended = obj.GetType() == typeof(DataItem);

                    // Check SampleRate
                    if (version >= MTConnectVersions.Version12) obj.SampleRate = dataItem.SampleRate;

                    // Check Source
                    if (dataItem.Source != null && version >= MTConnectVersions.Version12)
                    {
                        var source = new Source();
                        source.ComponentId = dataItem.Source.ComponentId;
                        if (version >= MTConnectVersions.Version14) source.CompositionId = dataItem.Source.CompositionId;
                        source.DataItemId = dataItem.Source.DataItemId;

                        if (!string.IsNullOrEmpty(source.ComponentId) || !string.IsNullOrEmpty(source.CompositionId) || !string.IsNullOrEmpty(source.DataItemId))
                        {
                            obj.Source = source;
                        }
                    }

                    // Check Relationships
                    if (dataItem.Relationships != null && version >= MTConnectVersions.Version15)
                    {
                        var relationships = new List<IAbstractDataItemRelationship>();
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
                                if (version >= MTConnectVersions.Version17) relationships.Add(relationship);
                            }

                            // Device Relationship
                            if (relationship.GetType() == typeof(DeviceRelationship))
                            {
                                relationships.Add(relationship);
                            }

                            // Specification Relationship
                            if (relationship.GetType() == typeof(SpecificationRelationship))
                            {
                                if (version >= MTConnectVersions.Version17) relationships.Add(relationship);
                            }
                        }

                        obj.Relationships = relationships;
                    }

                    // Check Representation
                    if (version >= MTConnectVersions.Version12) obj.Representation = dataItem.Representation;

                    // Check ResetTrigger
                    if (version >= MTConnectVersions.Version14) obj.ResetTrigger = dataItem.ResetTrigger;

                    // Check CoordinateSystem
                    if (version < MTConnectVersions.Version20) obj.CoordinateSystem = dataItem.CoordinateSystem;

                    // Check CoordinateSystemIdRef
                    if (version >= MTConnectVersions.Version15) obj.CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;

                    // Check CompositionId
                    if (version >= MTConnectVersions.Version14)
                    {
                        obj.CompositionId = dataItem.CompositionId;
                    }
                    else if (!string.IsNullOrEmpty(dataItem.CompositionId))
                    {
                        // Don't return if Composition not compatible with Version as this could cause duplicate Types within the same Component
                        return null;
                    }

                    // Check Constraints
                    if (version >= MTConnectVersions.Version11) obj.Constraints = dataItem.Constraints;

                    // Check Definition
                    if (version >= MTConnectVersions.Version16) obj.Definition = dataItem.Definition;

                    // Check Statistic
                    if (version >= MTConnectVersions.Version12) obj.Statistic = dataItem.Statistic;

                    // Check Filters
                    if (version >= MTConnectVersions.Version13) obj.Filters = dataItem.Filters;

                    // Check InitialValue
                    if (version >= MTConnectVersions.Version14) obj.InitialValue = dataItem.InitialValue;

                    // Check Discrete
                    if (version >= MTConnectVersions.Version15) obj.Discrete = dataItem.Discrete;
                }

                // Call overridable method (used to process based on Type)
                return obj.OnProcess(obj, version);
            }

            return null;
        }
    }
}