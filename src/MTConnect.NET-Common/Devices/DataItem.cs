// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Configurations;
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

        public MTConnectEntityType EntityType => MTConnectEntityType.DataItem;

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
            Representation = DataItemRepresentation.VALUE;
            Filters = new List<Filter>();
            Relationships = new List<IAbstractDataItemRelationship>();
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
                    case Devices.DataItems.AdapterUriDataItem.TypeId: return "AdapterURI";
                    case Devices.DataItems.MTConnectVersionDataItem.TypeId: return "MTConnectVersion";
                    case Devices.DataItems.AmperageACDataItem.TypeId: return "AmperageAC";
                    case Devices.DataItems.AmperageDCDataItem.TypeId: return "AmperageDC";
                    case Devices.DataItems.VoltageACDataItem.TypeId: return "VoltageAC";
                    case Devices.DataItems.VoltageDCDataItem.TypeId: return "VoltageDC";
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

        public static IEnumerable<string> GetTypes()
        {
            if (_types == null) _types = GetAllTypes();

            return _types.Keys;
        }

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

        public static IEnumerable<KeyValuePair<string, string>> GetSubTypeDescriptions()
        {
            if (_subtypeDescriptions == null)
            {
                if (_types == null) _types = GetAllTypes();

                if (!_types.IsNullOrEmpty())
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

        public static Type GetDataItemType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                if (_types == null) _types = GetAllTypes();

                if (_types != null)
                {
                    string typeId;
                    lock (_lock)
                    {
                        _typeIds.TryGetValue(type, out typeId);
                        if (typeId == null)
                        {
                            typeId = type.ToPascalCase();
                            _typeIds.Add(type, typeId);
                        }
                    }

                    if (_types.TryGetValue(typeId, out Type t))
                    {
                        return t;
                    }
                }
            }

            return typeof(DataItem);
        }

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