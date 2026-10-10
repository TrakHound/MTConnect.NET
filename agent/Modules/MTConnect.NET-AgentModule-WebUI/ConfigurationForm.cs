// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var change in request.Entries ?? new List<ConfigurationEntryChange>())
            {
                if (change.Section == null || !entriesBySection.TryGetValue(change.Section, out var sectionEntries) ||
                    change.Index < 0 || change.Index >= sectionEntries.Count)
                {
                    throw new ConfigurationEditException($"{change.Section}[{change.Index}] was not found - reload the configuration and try again");
                }

                var entry = sectionEntries[change.Index];

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
            }

            // New entries (only types the Agent has loaded can be added)
            foreach (var add in request.Add ?? new List<ConfigurationEntryAdd>())
            {
                if (add.Section != "modules") throw new ConfigurationEditException("Only modules can be added on the form");
                if (add.TypeId == null || !_entryTypes[add.Section].TryGetValue(add.TypeId, out var entryType))
                {
                    throw new ConfigurationEditException($"'{add.TypeId}' is not a module type the Agent has loaded");
                }

                var values = new List<KeyValuePair<string, string>>();
                foreach (var value in add.Values ?? new Dictionary<string, string>())
                {
                    var field = entryType.Fields?.FirstOrDefault(o => o.Key == value.Key);
                    if (field == null || !field.IsSimple) throw new ConfigurationEditException($"'{value.Key}' of {add.TypeId} cannot be set on the form");

                    var formatted = Format(field, value.Value);
                    if (formatted != null) values.Add(new KeyValuePair<string, string>(value.Key, formatted));
                }

                document.AddEntry(add.Section, add.TypeId, values);
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
            secret = field.Secret
        };
    }
}
