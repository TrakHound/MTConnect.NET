// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// The MTConnect Component, Composition and DataItem types the library knows about (generated from the
    /// MTConnect Standard), with their descriptions, subtypes, default units, allowed values and versions.
    /// Used for the type pickers on the Devices form.
    /// </summary>
    internal static class DeviceTypeCatalog
    {
        private static readonly Lazy<object> _catalog = new Lazy<object>(Build);

        /// <summary>The catalog as a JSON-ready object (built once).</summary>
        public static object Get() => _catalog.Value;


        private static object Build()
        {
            var assembly = typeof(Component).Assembly;
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }

            // Sort key (type id) and item
            var components = new List<KeyValuePair<string, object>>();
            var compositions = new List<KeyValuePair<string, object>>();
            var dataItems = new List<KeyValuePair<string, object>>();

            foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract && t.IsPublic))
            {
                try
                {
                    if (typeof(Composition).IsAssignableFrom(type) && type != typeof(Composition))
                    {
                        var typeId = ConstString(type, "TypeId");
                        if (typeId == null) continue;
                        var instance = Activator.CreateInstance(type) as Composition;
                        compositions.Add(Item(typeId, new
                        {
                            type = typeId,
                            description = ConstString(type, "DescriptionText"),
                            minimumVersion = instance?.MinimumVersion?.ToString(),
                            maximumVersion = instance?.MaximumVersion?.ToString()
                        }));
                    }
                    else if (typeof(Component).IsAssignableFrom(type) && type != typeof(Component) && !typeof(Device).IsAssignableFrom(type))
                    {
                        var typeId = ConstString(type, "TypeId");
                        if (typeId == null) continue;
                        var instance = Activator.CreateInstance(type) as Component;
                        components.Add(Item(typeId, new
                        {
                            type = typeId,
                            name = ConstString(type, "NameId"),
                            description = ConstString(type, "DescriptionText"),
                            organizer = instance?.IsOrganizer ?? false,
                            minimumVersion = instance?.MinimumVersion?.ToString(),
                            maximumVersion = instance?.MaximumVersion?.ToString()
                        }));
                    }
                    else if (typeof(DataItem).IsAssignableFrom(type) && type != typeof(DataItem))
                    {
                        var typeId = ConstString(type, "TypeId");
                        var category = type.GetField("CategoryId", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                        if (typeId == null || category == null) continue;

                        var instance = Activator.CreateInstance(type) as DataItem;
                        dataItems.Add(Item(typeId, new
                        {
                            type = typeId,
                            category = category.ToString(),
                            name = ConstString(type, "NameId"),
                            description = ConstString(type, "DescriptionText"),
                            units = ConstString(type, "DefaultUnits"),
                            subTypes = GetSubTypes(type),
                            values = GetAllowedValues(type),
                            minimumVersion = instance?.MinimumVersion?.ToString(),
                            maximumVersion = instance?.MaximumVersion?.ToString()
                        }));
                    }
                }
                catch
                {
                    // A type that cannot be described is simply not offered in the pickers
                }
            }

            return new
            {
                components = Sorted(components),
                compositions = Sorted(compositions),
                dataItems = Sorted(dataItems),
                units = ConstStrings(typeof(Units)),
                nativeUnits = ConstStrings(typeof(NativeUnits)),
                representations = Enum.GetNames(typeof(DataItemRepresentation)),
                statistics = Enum.GetNames(typeof(DataItemStatistic)),
                categories = Enum.GetNames(typeof(DataItemCategory))
            };
        }

        /// <summary>
        /// The subtypes of a DataItem type (from its nested <c>SubTypes</c> enum) with their descriptions.
        /// </summary>
        private static List<object> GetSubTypes(Type dataItemType)
        {
            var subTypes = dataItemType.GetNestedType("SubTypes");
            if (subTypes == null || !subTypes.IsEnum) return null;

            var getDescription = dataItemType.GetMethod("GetSubTypeDescription", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            return Enum.GetNames(subTypes).Select(name => (object)new
            {
                subType = name,
                description = getDescription?.Invoke(null, new object[] { name }) as string
            }).ToList();
        }

        /// <summary>
        /// The controlled vocabulary of an EVENT type, when the library has one: the enum named after the
        /// DataItem class (for example <c>ExecutionDataItem</c> → <c>MTConnect.Observations.Events.Execution</c>).
        /// </summary>
        private static List<object> GetAllowedValues(Type dataItemType)
        {
            if (!dataItemType.Name.EndsWith("DataItem")) return null;
            var baseName = dataItemType.Name.Substring(0, dataItemType.Name.Length - "DataItem".Length);

            var assembly = dataItemType.Assembly;
            var valuesType = assembly.GetType("MTConnect.Observations.Events." + baseName);
            if (valuesType == null || !valuesType.IsEnum) return null;

            var descriptions = assembly.GetType("MTConnect.Observations.Events." + baseName + "Descriptions");
            var getDescription = descriptions?.GetMethod("Get", BindingFlags.Public | BindingFlags.Static, null, new[] { valuesType }, null);

            return Enum.GetValues(valuesType).Cast<object>().Select(value => (object)new
            {
                value = value.ToString(),
                description = getDescription?.Invoke(null, new[] { value }) as string
            }).ToList();
        }

        private static KeyValuePair<string, object> Item(string key, object value) => new KeyValuePair<string, object>(key, value);

        private static List<object> Sorted(List<KeyValuePair<string, object>> items) =>
            items.OrderBy(o => o.Key, StringComparer.Ordinal).Select(o => o.Value).ToList();

        private static string ConstString(Type type, string name)
        {
            return type.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null) as string;
        }

        private static List<string> ConstStrings(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsLiteral && f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null))
                .Distinct()
                .OrderBy(o => o)
                .ToList();
        }
    }
}
