// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// One configurable option of the Agent, a module or a processor, as shown on the Configuration form.
    /// </summary>
    internal class ConfigurationField
    {
        /// <summary>Simple field types the form edits directly.</summary>
        public const string TypeString = "string";
        public const string TypeInteger = "integer";
        public const string TypeNumber = "number";
        public const string TypeBoolean = "boolean";
        public const string TypeEnum = "enum";

        /// <summary>Lists, dictionaries and nested objects; shown read-only and edited as YAML.</summary>
        public const string TypeComplex = "complex";

        /// <summary>The key used in <c>agent.config.yaml</c>.</summary>
        public string Key { get; set; }

        /// <summary>The C# property the key binds to.</summary>
        public string Property { get; set; }

        /// <summary>One of the Type* constants.</summary>
        public string Type { get; set; }

        /// <summary>Allowed values for enum fields.</summary>
        public string[] Options { get; set; }

        /// <summary>The value used when the key is not set, formatted as YAML scalar text (null when unknown).</summary>
        public string Default { get; set; }

        /// <summary>The <c>///</c> summary of the property, when the XML documentation file is available.</summary>
        public string Description { get; set; }

        /// <summary>True for passwords and similar values that should be masked.</summary>
        public bool Secret { get; set; }

        /// <summary>Whether <see cref="Type"/> is one the form edits directly.</summary>
        public bool IsSimple => Type != TypeComplex;
    }

    /// <summary>
    /// A module or processor type that can appear in the <c>modules</c> / <c>processors</c> section.
    /// </summary>
    internal class ConfigurationEntryType
    {
        /// <summary>The key used in the configuration file (for example <c>http-server</c>).</summary>
        public string TypeId { get; set; }

        /// <summary>The linked settings class, or null when the type has none.</summary>
        public Type SettingsType { get; set; }

        /// <summary>Fields of the linked settings class, or null when the type has none (edited as YAML).</summary>
        public List<ConfigurationField> Fields { get; set; }
    }

    /// <summary>
    /// Builds form descriptions for the Agent's settings and for every loaded module and processor
    /// type. Keys mirror how the Agent reads the file: the camelCase property name, or the
    /// <c>[YamlMember(Alias)]</c> value, skipping <c>[YamlIgnore]</c> properties.
    /// </summary>
    internal static class ConfigurationSchema
    {
        private static readonly ConcurrentDictionary<Type, List<ConfigurationField>> _fieldCache = new ConcurrentDictionary<Type, List<ConfigurationField>>();
        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> _docCache = new ConcurrentDictionary<string, Dictionary<string, string>>();

        /// <summary>Agent settings that are not edited on the form (sections or internal values).</summary>
        private static readonly HashSet<string> _agentExcludedKeys = new HashSet<string> { "modules", "processors", "changeToken" };

        private static readonly Type[] _integerTypes = { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort), typeof(byte), typeof(sbyte) };
        private static readonly Type[] _numberTypes = { typeof(double), typeof(float), typeof(decimal) };


        /// <summary>
        /// Fields for the Agent's top-level settings.
        /// </summary>
        public static List<ConfigurationField> GetAgentFields(Type agentConfigurationType)
        {
            return GetFields(agentConfigurationType).Where(o => !_agentExcludedKeys.Contains(o.Key)).ToList();
        }

        /// <summary>
        /// The module types loaded by the Agent, with the fields of their linked settings class
        /// (<c>public static readonly Type ConfigurationType</c> on the module class).
        /// </summary>
        public static List<ConfigurationEntryType> GetModuleTypes() => GetEntryTypes(typeof(IMTConnectAgentModule));

        /// <summary>
        /// The processor types loaded by the Agent, with the fields of their linked settings class.
        /// </summary>
        public static List<ConfigurationEntryType> GetProcessorTypes() => GetEntryTypes(typeof(IMTConnectAgentProcessor));

        /// <summary>
        /// Fields for a settings class (most basic options first), cached per type.
        /// </summary>
        public static List<ConfigurationField> GetFields(Type configurationType)
        {
            return _fieldCache.GetOrAdd(configurationType, BuildFields);
        }


        private static List<ConfigurationEntryType> GetEntryTypes(Type interfaceType)
        {
            var result = new Dictionary<string, ConfigurationEntryType>();

            foreach (var assembly in Assemblies.Get() ?? Enumerable.Empty<Assembly>())
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }
                catch { continue; }

                foreach (var type in types)
                {
                    if (type.IsAbstract || !interfaceType.IsAssignableFrom(type)) continue;

                    var typeId = type.GetField("ConfigurationTypeId", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;
                    if (string.IsNullOrEmpty(typeId) || result.ContainsKey(typeId)) continue;

                    var configurationType = type.GetField("ConfigurationType", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as Type;

                    List<ConfigurationField> fields = null;
                    if (configurationType != null)
                    {
                        try { fields = GetFields(configurationType); } catch { }
                    }

                    result[typeId] = new ConfigurationEntryType { TypeId = typeId, SettingsType = fields != null ? configurationType : null, Fields = fields };
                }
            }

            return result.Values.OrderBy(o => o.TypeId).ToList();
        }

        private static List<ConfigurationField> BuildFields(Type configurationType)
        {
            object defaults = null;
            try { defaults = Activator.CreateInstance(configurationType); } catch { }

            var fields = new List<ConfigurationField>();
            var seenKeys = new HashSet<string>();

            var properties = configurationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                .Where(p => p.GetCustomAttribute<YamlIgnoreAttribute>(true) == null)
                // Base class options (port, server, ...) before the options a subclass adds
                .OrderBy(p => InheritanceDepth(p.DeclaringType))
                .ThenBy(p => p.MetadataToken);

            foreach (var property in properties)
            {
                var alias = property.GetCustomAttribute<YamlMemberAttribute>(true)?.Alias;
                var key = !string.IsNullOrEmpty(alias) ? alias : CamelCaseNamingConvention.Instance.Apply(property.Name);
                if (!seenKeys.Add(key)) continue;

                var valueType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var field = new ConfigurationField
                {
                    Key = key,
                    Property = property.Name,
                    Type = GetFieldType(valueType),
                    Description = GetDescription(property),
                    Secret = IsSecret(property.Name)
                };

                if (field.Type == ConfigurationField.TypeEnum) field.Options = Enum.GetNames(valueType);

                if (defaults != null && field.IsSimple)
                {
                    try { field.Default = FormatScalar(property.GetValue(defaults)); } catch { }
                }

                fields.Add(field);
            }

            return fields;
        }

        private static string GetFieldType(Type type)
        {
            if (type == typeof(string) || type == typeof(Version)) return ConfigurationField.TypeString;
            if (type == typeof(bool)) return ConfigurationField.TypeBoolean;
            if (type.IsEnum) return ConfigurationField.TypeEnum;
            if (_integerTypes.Contains(type)) return ConfigurationField.TypeInteger;
            if (_numberTypes.Contains(type)) return ConfigurationField.TypeNumber;
            return ConfigurationField.TypeComplex;
        }

        private static bool IsSecret(string propertyName)
        {
            return propertyName.IndexOf("Password", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("Secret", StringComparison.OrdinalIgnoreCase) >= 0
                || propertyName.IndexOf("ApiKey", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static int InheritanceDepth(Type type)
        {
            var depth = 0;
            while (type != null) { depth++; type = type.BaseType; }
            return depth;
        }

        /// <summary>
        /// Formats a value as YAML scalar text (no quoting).
        /// </summary>
        public static string FormatScalar(object value)
        {
            switch (value)
            {
                case null: return null;
                case bool b: return b ? "true" : "false";
                case IFormattable f when !(value is Enum): return f.ToString(null, CultureInfo.InvariantCulture);
                default: return value.ToString();
            }
        }


        #region "XML documentation"

        private static string GetDescription(PropertyInfo property)
        {
            var declaringType = property.DeclaringType;
            if (declaringType == null) return null;

            var docs = GetDocs(declaringType.Assembly);
            if (docs == null) return null;

            var memberName = "P:" + declaringType.FullName?.Replace('+', '.') + "." + property.Name;
            return docs.TryGetValue(memberName, out var summary) ? summary : null;
        }

        private static Dictionary<string, string> GetDocs(Assembly assembly)
        {
            string location;
            try { location = assembly.Location; } catch { return null; }
            if (string.IsNullOrEmpty(location)) return null;

            return _docCache.GetOrAdd(location, path =>
            {
                var result = new Dictionary<string, string>();
                var xmlPath = Path.ChangeExtension(path, ".xml");
                if (!File.Exists(xmlPath)) return result;

                try
                {
                    var document = XDocument.Load(xmlPath);
                    foreach (var member in document.Descendants("member"))
                    {
                        var name = (string)member.Attribute("name");
                        var summary = member.Element("summary");
                        if (name != null && name.StartsWith("P:") && summary != null) result[name] = CleanSummary(summary);
                    }
                }
                catch { }

                return result;
            });
        }

        /// <summary>
        /// Converts an XML doc summary to plain text (<c>&lt;see cref&gt;</c> becomes the short type name).
        /// </summary>
        private static string CleanSummary(XElement summary)
        {
            var sb = new StringBuilder();
            foreach (var node in summary.Nodes())
            {
                if (node is XText text) sb.Append(text.Value);
                else if (node is XElement element)
                {
                    var cref = (string)element.Attribute("cref");
                    var langword = (string)element.Attribute("langword");
                    var name = (string)element.Attribute("name");

                    if (cref != null) sb.Append(ShortName(cref));
                    else if (langword != null) sb.Append(langword);
                    else if (name != null && element.Name.LocalName.EndsWith("paramref")) sb.Append(name);
                    else sb.Append(element.Value);
                }
            }

            return Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
        }

        private static string ShortName(string cref)
        {
            var value = cref.Length > 2 && cref[1] == ':' ? cref.Substring(2) : cref;
            var paren = value.IndexOf('(');
            if (paren >= 0) value = value.Substring(0, paren);
            var dot = value.LastIndexOf('.');
            return dot >= 0 ? value.Substring(dot + 1) : value;
        }

        #endregion
    }
}
