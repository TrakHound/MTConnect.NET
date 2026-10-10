// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// Changes submitted from the Configuration form.
    /// </summary>
    internal class ConfigurationFormRequest
    {
        /// <summary>The <see cref="ConfigurationDocument.Version"/> the form was loaded from.</summary>
        public string Version { get; set; }

        /// <summary>Top-level settings to change; a null or empty value removes the key (back to its default).</summary>
        public Dictionary<string, string> Agent { get; set; }

        /// <summary>Changes to module / processor entries.</summary>
        public List<ConfigurationEntryChange> Entries { get; set; }

        /// <summary>New entries to add (added after the existing entries, in this order).</summary>
        public List<ConfigurationEntryAdd> Add { get; set; }

        /// <summary>Existing entries to remove.</summary>
        public List<ConfigurationEntryRef> Remove { get; set; }
    }

    /// <summary>
    /// An existing module / processor entry.
    /// </summary>
    internal class ConfigurationEntryRef
    {
        /// <summary><c>modules</c> or <c>processors</c>.</summary>
        public string Section { get; set; }

        /// <summary>Position of the entry within its section.</summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// A new module entry to add.
    /// </summary>
    internal class ConfigurationEntryAdd
    {
        /// <summary>The section to add to (<c>modules</c>).</summary>
        public string Section { get; set; }

        /// <summary>The module type key (for example <c>shdr-adapter</c>).</summary>
        public string TypeId { get; set; }

        /// <summary>Initial settings; empty values are left out so the defaults apply.</summary>
        public Dictionary<string, string> Values { get; set; }

        /// <summary>Initial lists, maps and nested objects, as JSON.</summary>
        public Dictionary<string, JsonElement> Structured { get; set; }
    }

    /// <summary>
    /// Changes to one module / processor entry: either individual settings or replacement YAML.
    /// </summary>
    internal class ConfigurationEntryChange
    {
        /// <summary><c>modules</c> or <c>processors</c>.</summary>
        public string Section { get; set; }

        /// <summary>Position of the entry within its section.</summary>
        public int Index { get; set; }

        /// <summary>Settings to change; a null or empty value removes the key.</summary>
        public Dictionary<string, string> Values { get; set; }

        /// <summary>Replacement YAML for the whole entry (for entries edited as YAML).</summary>
        public string Yaml { get; set; }

        /// <summary>Lists, maps and nested objects to set, as JSON (null or empty removes the key).</summary>
        public Dictionary<string, JsonElement> Structured { get; set; }
    }

    /// <summary>
    /// Writes list, map and nested object settings (sent from the form as JSON) as YAML lines, validating
    /// each value against its field. Lists use the style of the sample configuration (items at the same
    /// indentation as their key); maps and objects are indented two spaces.
    /// </summary>
    internal static class ConfigurationYaml
    {
        /// <summary>
        /// The lines (without leading indentation) for "key: value", or null when the value is empty
        /// (null, an empty list / map / object), meaning the key should be removed.
        /// </summary>
        public static List<string> BlockLines(ConfigurationField field, JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined) return null;
            var content = new List<string>();

            switch (field.Type)
            {
                case ConfigurationField.TypeList:
                    foreach (var item in Array(field, value))
                    {
                        var text = ScalarText(item);
                        if (string.IsNullOrEmpty(text)) continue;
                        content.Add("- " + ConfigurationDocument.FormatValue(field.ItemField(), text));
                    }
                    break;

                case ConfigurationField.TypeMap:
                    foreach (var property in Object(field, value))
                    {
                        var text = ScalarText(property.Value);
                        if (string.IsNullOrEmpty(property.Name.Trim()) || string.IsNullOrEmpty(text)) continue;
                        content.Add("  " + ConfigurationDocument.QuoteString(property.Name.Trim()) + ": " + ConfigurationDocument.FormatValue(field.ItemField(), text));
                    }
                    break;

                case ConfigurationField.TypeObject:
                    content.AddRange(ObjectLines(field.Fields, value, field.Key).Select(l => "  " + l));
                    break;

                case ConfigurationField.TypeObjectList:
                    foreach (var item in Array(field, value))
                    {
                        var lines = ObjectLines(field.ItemFields, item, field.Key);
                        if (lines.Count == 0) continue;
                        content.Add("- " + lines[0]);
                        content.AddRange(lines.Skip(1).Select(l => "  " + l));
                    }
                    break;

                default:
                    throw new ConfigurationEditException($"'{field.Key}' can only be edited as YAML");
            }

            if (content.Count == 0) return null;
            content.Insert(0, field.Key + ":");
            return content;
        }

        /// <summary>The lines of an object's settings (empty values left out), without leading indentation.</summary>
        private static List<string> ObjectLines(List<ConfigurationField> fields, JsonElement value, string path)
        {
            var lines = new List<string>();
            if (value.ValueKind == JsonValueKind.Null) return lines;
            if (value.ValueKind != JsonValueKind.Object) throw new ConfigurationEditException($"'{path}' must be a set of settings");

            foreach (var property in value.EnumerateObject())
            {
                var field = fields?.FirstOrDefault(f => f.Key == property.Name);
                if (field == null) throw new ConfigurationEditException($"'{property.Name}' is not a setting of '{path}'");

                if (field.IsSimple)
                {
                    var text = ScalarText(property.Value);
                    if (!string.IsNullOrEmpty(text)) lines.Add(field.Key + ": " + ConfigurationDocument.FormatValue(field, text));
                }
                else if (field.IsStructured)
                {
                    var block = BlockLines(field, property.Value);
                    if (block != null) lines.AddRange(block);
                }
                else
                {
                    throw new ConfigurationEditException($"'{path}.{field.Key}' can only be edited as YAML");
                }
            }
            return lines;
        }

        /// <summary>Indents lines and joins them with the file's line ending (each line ends with one).</summary>
        public static string Join(List<string> lines, string indent, string newline) =>
            string.Concat(lines.Select(l => indent + l + newline));

        private static IEnumerable<JsonElement> Array(ConfigurationField field, JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Array) throw new ConfigurationEditException($"'{field.Key}' must be a list");
            return value.EnumerateArray();
        }

        private static IEnumerable<JsonProperty> Object(ConfigurationField field, JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Object) throw new ConfigurationEditException($"'{field.Key}' must be a set of names and values");
            return value.EnumerateObject();
        }

        private static string ScalarText(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.String: return value.GetString();
                case JsonValueKind.Number: return value.GetRawText();
                case JsonValueKind.True: return "true";
                case JsonValueKind.False: return "false";
                case JsonValueKind.Null: return null;
                default: throw new ConfigurationEditException("Expected a single value");
            }
        }
    }

    /// <summary>
    /// Builds the Configuration form model from <c>agent.config.yaml</c> and applies form changes to it.
    /// </summary>
    internal class ConfigurationForm
    {
        /// <summary>The list sections shown as entries.</summary>
        public static readonly string[] Sections = { "modules", "processors" };

        private readonly Type _agentConfigurationType;
        private readonly List<ConfigurationField> _agentFields;
        private readonly Dictionary<string, Dictionary<string, ConfigurationEntryType>> _entryTypes;


        /// <summary>
        /// Creates a form for the given Agent settings class and the module / processor types that are loaded.
        /// </summary>
        /// <param name="agentConfigurationType">The class the Agent reads its top-level settings into.</param>
        /// <param name="moduleTypes">Loaded module types.</param>
        /// <param name="processorTypes">Loaded processor types.</param>
        public ConfigurationForm(Type agentConfigurationType, List<ConfigurationEntryType> moduleTypes, List<ConfigurationEntryType> processorTypes)
        {
            _agentConfigurationType = agentConfigurationType;
            _agentFields = ConfigurationSchema.GetAgentFields(agentConfigurationType);
            _entryTypes = new Dictionary<string, Dictionary<string, ConfigurationEntryType>>
            {
                ["modules"] = (moduleTypes ?? new List<ConfigurationEntryType>()).ToDictionary(o => o.TypeId),
                ["processors"] = (processorTypes ?? new List<ConfigurationEntryType>()).ToDictionary(o => o.TypeId)
            };
        }


        /// <summary>
        /// The form model sent to the browser.
        /// </summary>
        public object BuildModel(string text, string path, bool editable)
        {
            var document = ConfigurationDocument.Parse(text);

            document.GetValues(document.Root, out var agentValues, out var agentComplex, Sections);
            var agentKeys = new HashSet<string>(_agentFields.Select(o => o.Key));

            var entries = new List<object>();
            foreach (var section in Sections)
            {
                foreach (var entry in document.GetEntries(section))
                {
                    _entryTypes[section].TryGetValue(entry.TypeId ?? "", out var entryType);
                    var hasForm = entryType?.Fields != null && entry.IsEditable;

                    document.GetValues(entry.ValueNode, out var values, out var complex);
                    var fieldKeys = new HashSet<string>(entryType?.Fields?.Select(o => o.Key) ?? Enumerable.Empty<string>());

                    entries.Add(new
                    {
                        section,
                        index = entry.Index,
                        typeId = entry.TypeId,
                        known = entryType != null,
                        hasForm,
                        values,
                        complex,
                        // Lists, maps and objects as plain data, for the structured editors
                        data = document.GetStructuredValues(entry.ValueNode),
                        otherKeys = values.Keys.Concat(complex.Keys).Where(k => !fieldKeys.Contains(k)).ToList(),
                        yaml = document.GetEntryText(entry)
                    });
                }
            }

            return new
            {
                version = document.Version,
                path,
                editable,
                agent = new
                {
                    fields = _agentFields.Select(ToFieldModel).ToList(),
                    values = agentValues,
                    complex = agentComplex,
                    otherKeys = agentValues.Keys.Concat(agentComplex.Keys).Where(k => !agentKeys.Contains(k)).ToList()
                },
                types = Sections.ToDictionary(s => s, s => _entryTypes[s].Values.Select(t => new
                {
                    typeId = t.TypeId,
                    fields = t.Fields?.Select(ToFieldModel).ToList()
                }).ToList()),
                entries
            };
        }

        /// <summary>
        /// Applies form changes to the configuration text and validates the result.
        /// Throws <see cref="ConfigurationEditException"/> with a readable message when a change is invalid.
        /// </summary>
        public string Apply(string text, ConfigurationFormRequest request)
        {
            var document = ConfigurationDocument.Parse(text);

            // Top-level settings
            foreach (var change in request.Agent ?? new Dictionary<string, string>())
            {
                var field = _agentFields.FirstOrDefault(o => o.Key == change.Key);
                if (field == null || !field.IsSimple) throw new ConfigurationEditException($"'{change.Key}' cannot be edited on the form");
                document.SetRootValue(change.Key, Format(field, change.Value));
            }

            // Module / processor entries
            var entriesBySection = Sections.ToDictionary(s => s, s => document.GetEntries(s));
            ConfigurationEntry FindEntry(string section, int index)
            {
                if (section == null || !entriesBySection.TryGetValue(section, out var sectionEntries) || index < 0 || index >= sectionEntries.Count)
                {
                    throw new ConfigurationEditException($"{section}[{index}] was not found - reload the configuration and try again");
                }
                return sectionEntries[index];
            }

            var removed = new HashSet<ConfigurationEntry>();
            foreach (var remove in request.Remove ?? new List<ConfigurationEntryRef>())
            {
                var entry = FindEntry(remove.Section, remove.Index);
                if (removed.Add(entry)) document.RemoveEntry(entry);
            }

            foreach (var change in request.Entries ?? new List<ConfigurationEntryChange>())
            {
                var entry = FindEntry(change.Section, change.Index);
                if (removed.Contains(entry)) throw new ConfigurationEditException($"{entry.TypeId} is being removed and cannot also be changed");

                if (change.Yaml != null)
                {
                    document.ReplaceEntryText(entry, change.Yaml);
                    continue;
                }

                _entryTypes[change.Section].TryGetValue(entry.TypeId ?? "", out var entryType);
                if (entryType?.Fields == null) throw new ConfigurationEditException($"{entry.TypeId} has no form and can only be edited as YAML");

                foreach (var value in change.Values ?? new Dictionary<string, string>())
                {
                    var field = entryType.Fields.FirstOrDefault(o => o.Key == value.Key);
                    if (field == null || !field.IsSimple) throw new ConfigurationEditException($"'{value.Key}' of {entry.TypeId} cannot be edited on the form");
                    document.SetEntryValue(entry, value.Key, Format(field, value.Value));
                }

                foreach (var value in change.Structured ?? new Dictionary<string, JsonElement>())
                {
                    var field = entryType.Fields.FirstOrDefault(o => o.Key == value.Key);
                    if (field == null || !field.IsStructured) throw new ConfigurationEditException($"'{value.Key}' of {entry.TypeId} cannot be edited on the form");
                    var lines = ConfigurationYaml.BlockLines(field, value.Value);
                    document.SetEntryBlock(entry, value.Key, lines == null ? null : (Func<string, string>)(indent => ConfigurationYaml.Join(lines, indent, document.Newline)));
                }
            }

            // New entries (only types the Agent has loaded can be added)
            foreach (var add in request.Add ?? new List<ConfigurationEntryAdd>())
            {
                if (add.Section == null || !_entryTypes.ContainsKey(add.Section)) throw new ConfigurationEditException($"'{add.Section}' is not a section that entries can be added to");
                if (add.TypeId == null || !_entryTypes[add.Section].TryGetValue(add.TypeId, out var entryType))
                {
                    throw new ConfigurationEditException($"'{add.TypeId}' is not a {(add.Section == "processors" ? "processor" : "module")} type the Agent has loaded");
                }

                var values = new List<KeyValuePair<string, string>>();
                foreach (var value in add.Values ?? new Dictionary<string, string>())
                {
                    var field = entryType.Fields?.FirstOrDefault(o => o.Key == value.Key);
                    if (field == null || !field.IsSimple) throw new ConfigurationEditException($"'{value.Key}' of {add.TypeId} cannot be set on the form");

                    var formatted = Format(field, value.Value);
                    if (formatted != null) values.Add(new KeyValuePair<string, string>(value.Key, formatted));
                }

                var blocks = new List<Func<string, string>>();
                foreach (var value in add.Structured ?? new Dictionary<string, JsonElement>())
                {
                    var field = entryType.Fields?.FirstOrDefault(o => o.Key == value.Key);
                    if (field == null || !field.IsStructured) throw new ConfigurationEditException($"'{value.Key}' of {add.TypeId} cannot be set on the form");
                    var lines = ConfigurationYaml.BlockLines(field, value.Value);
                    if (lines != null) blocks.Add(indent => ConfigurationYaml.Join(lines, indent, document.Newline));
                }

                document.AddEntry(add.Section, add.TypeId, values, blocks);
            }

            var result = document.Apply();
            Validate(result);
            return result;
        }

        /// <summary>
        /// Checks that the text is valid YAML that the Agent and each module / processor can read.
        /// </summary>
        public void Validate(string text)
        {
            ConfigurationDocument document;
            try { document = ConfigurationDocument.Parse(text); }
            catch (YamlException ex) { throw new ConfigurationEditException("Invalid YAML (" + ConfigurationDocument.DescribeYamlError(ex) + ")"); }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            try { deserializer.Deserialize(text, _agentConfigurationType); }
            catch (YamlException ex) { throw new ConfigurationEditException("Invalid Agent setting (" + ConfigurationDocument.DescribeYamlError(ex) + ")"); }

            foreach (var section in Sections)
            {
                foreach (var entry in document.GetEntries(section))
                {
                    if (entry.TypeId == null || entry.KeyNode == null) continue;
                    if (!_entryTypes[section].TryGetValue(entry.TypeId, out var entryType) || entryType.SettingsType == null) continue;
                    var settingsType = entryType.SettingsType;

                    try
                    {
                        var entryText = document.GetEntryText(entry);
                        deserializer.Deserialize(entryText, typeof(Dictionary<,>).MakeGenericType(typeof(string), settingsType));
                    }
                    catch (YamlException ex)
                    {
                        throw new ConfigurationEditException($"Invalid {entry.TypeId} setting in {section}[{entry.Index}] ({ex.InnerException?.Message ?? ex.Message})");
                    }
                }
            }
        }

        private static string Format(ConfigurationField field, string value)
        {
            // An empty value means "not set": the key is removed and the default applies
            if (string.IsNullOrEmpty(value)) return null;
            return ConfigurationDocument.FormatValue(field, value);
        }

        private static object ToFieldModel(ConfigurationField field) => new
        {
            key = field.Key,
            type = field.Type,
            options = field.Options,
            @default = field.Default,
            description = field.Description,
            secret = field.Secret,
            itemType = field.ItemType,
            itemOptions = field.ItemOptions,
            itemFields = field.ItemFields?.Select(ToFieldModel).ToList(),
            valueType = field.ValueType,
            fields = field.Fields?.Select(ToFieldModel).ToList()
        };
    }
}
