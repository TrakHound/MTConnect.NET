// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// An entry of the <c>modules</c> or <c>processors</c> section: a list item holding a single
    /// <c>type-id: { settings }</c> pair.
    /// </summary>
    internal class ConfigurationEntry
    {
        /// <summary>The section the entry belongs to (<c>modules</c> or <c>processors</c>).</summary>
        public string Section { get; set; }

        /// <summary>Position of the entry within its section.</summary>
        public int Index { get; set; }

        /// <summary>The module / processor type key (for example <c>http-server</c>).</summary>
        public string TypeId { get; set; }

        /// <summary>The list item node.</summary>
        public YamlNode Item { get; set; }

        /// <summary>The type key node (null when the item is not a single-key mapping).</summary>
        public YamlScalarNode KeyNode { get; set; }

        /// <summary>The settings node: a block mapping, or an empty scalar when no settings are given.</summary>
        public YamlNode ValueNode { get; set; }

        /// <summary>
        /// True when the entry's settings can be edited key by key: the item is a single-key mapping and its
        /// settings are an empty value or a block (not flow) mapping.
        /// </summary>
        public bool IsEditable =>
            KeyNode != null &&
            (ValueNode is YamlMappingNode mapping ? mapping.Style != YamlDotNet.Core.Events.MappingStyle.Flow : IsEmptyScalar(ValueNode));

        internal static bool IsEmptyScalar(YamlNode node) => node is YamlScalarNode scalar && string.IsNullOrEmpty(scalar.Value) && scalar.Style == YamlDotNet.Core.ScalarStyle.Plain;
    }

    /// <summary>
    /// Thrown when a requested change cannot be applied (invalid value, unsupported layout, ...).
    /// </summary>
    internal class ConfigurationEditException : Exception
    {
        public ConfigurationEditException(string message) : base(message) { }
    }

    /// <summary>
    /// A parsed <c>agent.config.yaml</c> that can be changed key by key while keeping the rest of the
    /// text - including comments, blank lines and unrecognised keys - exactly as it was.
    /// </summary>
    /// <remarks>
    /// Changes are collected and applied as text replacements located from the YAML parser's positions,
    /// so only the changed values (or the lines of added / removed keys) are rewritten.
    /// </remarks>
    internal class ConfigurationDocument
    {
        private readonly List<TextEdit> _edits = new List<TextEdit>();
        private readonly Dictionary<int, List<string>> _insertions = new Dictionary<int, List<string>>();
        private readonly Dictionary<int, string> _insertionPrefixes = new Dictionary<int, string>();


        /// <summary>The original text.</summary>
        public string Text { get; }

        /// <summary>The line ending used by the file (used for inserted lines).</summary>
        public string Newline { get; }

        /// <summary>The top-level mapping, or null for an empty document.</summary>
        public YamlMappingNode Root { get; }

        /// <summary>A hash of <see cref="Text"/> used to detect concurrent changes to the file.</summary>
        public string Version => ComputeVersion(Text);


        private ConfigurationDocument(string text, YamlMappingNode root)
        {
            Text = text ?? "";
            Root = root;
            Newline = Text.Contains("\r\n") ? "\r\n" : "\n";
        }

        /// <summary>
        /// Parses configuration text. Throws <see cref="YamlException"/> for invalid YAML and
        /// <see cref="ConfigurationEditException"/> when the top level is not a mapping.
        /// </summary>
        public static ConfigurationDocument Parse(string text)
        {
            var stream = new YamlStream();
            using (var reader = new StringReader(text ?? "")) stream.Load(reader);

            var rootNode = stream.Documents.FirstOrDefault()?.RootNode;
            if (rootNode != null && !(rootNode is YamlMappingNode) && !ConfigurationEntry.IsEmptyScalar(rootNode))
            {
                throw new ConfigurationEditException("The top level of the configuration must be a set of key: value settings");
            }

            var root = rootNode as YamlMappingNode;
            if (root != null && root.Style == YamlDotNet.Core.Events.MappingStyle.Flow)
            {
                throw new ConfigurationEditException("Flow-style ({ ... }) top-level configuration cannot be edited on the form");
            }

            return new ConfigurationDocument(text, root);
        }

        /// <summary>
        /// A short hash identifying a version of the configuration text.
        /// </summary>
        public static string ComputeVersion(string text)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text ?? ""));
                return BitConverter.ToString(hash, 0, 12).Replace("-", "").ToLowerInvariant();
            }
        }


        #region "Reading"

        /// <summary>
        /// The entries of a list section such as <c>modules</c>, in file order.
        /// </summary>
        public List<ConfigurationEntry> GetEntries(string section)
        {
            var result = new List<ConfigurationEntry>();
            if (Root == null || !Root.Children.TryGetValue(new YamlScalarNode(section), out var sectionNode)) return result;
            if (!(sectionNode is YamlSequenceNode sequence)) return result;

            var index = 0;
            foreach (var item in sequence.Children)
            {
                var entry = new ConfigurationEntry { Section = section, Index = index++, Item = item };

                if (item is YamlMappingNode mapping && mapping.Children.Count == 1 && mapping.Style != YamlDotNet.Core.Events.MappingStyle.Flow)
                {
                    var pair = mapping.Children.First();
                    entry.KeyNode = pair.Key as YamlScalarNode;
                    entry.TypeId = entry.KeyNode?.Value;
                    entry.ValueNode = pair.Value;
                }
                else if (item is YamlScalarNode scalar)
                {
                    entry.TypeId = scalar.Value;
                }
                else if (item is YamlMappingNode multi && multi.Children.Count > 0)
                {
                    entry.TypeId = (multi.Children.First().Key as YamlScalarNode)?.Value;
                }

                result.Add(entry);
            }

            return result;
        }

        /// <summary>
        /// The settings of a mapping: scalar values as text, and other values (lists, nested objects)
        /// as dedented YAML text. Returns empty dictionaries for an empty value.
        /// </summary>
        public void GetValues(YamlNode node, out Dictionary<string, string> scalars, out Dictionary<string, string> complex, IEnumerable<string> excludeKeys = null)
        {
            scalars = new Dictionary<string, string>();
            complex = new Dictionary<string, string>();
            if (!(node is YamlMappingNode mapping)) return;

            var exclude = new HashSet<string>(excludeKeys ?? Enumerable.Empty<string>());
            foreach (var child in mapping.Children)
            {
                var key = (child.Key as YamlScalarNode)?.Value;
                if (key == null || exclude.Contains(key)) continue;

                if (child.Value is YamlScalarNode scalar) scalars[key] = scalar.Value ?? "";
                else complex[key] = GetNodeText(child.Value);
            }
        }

        /// <summary>
        /// The text of an entry from its type key to the end of its last line, dedented so the type key
        /// starts at column 0 (used for the YAML editor of modules without a form).
        /// </summary>
        public string GetEntryText(ConfigurationEntry entry)
        {
            var start = (int)entry.Item.Start.Index;
            var end = LineEnd(LastValueEnd(entry.Item));
            var indent = (int)entry.Item.Start.Column - 1;
            return Dedent(Text.Substring(start, end - start), indent);
        }

        private string GetNodeText(YamlNode node)
        {
            var start = LineStart((int)node.Start.Index);
            var end = LineEnd(LastValueEnd(node));
            var indent = (int)node.Start.Column - 1;

            // Flow / inline values (for example "key: [a, b]") start mid-line
            if (start < (int)node.Start.Index && Text.Substring(start, (int)node.Start.Index - start).Trim().Length > 0)
            {
                return Text.Substring((int)node.Start.Index, end - (int)node.Start.Index);
            }

            // The text starts at the beginning of the node's first line, so that line is indented too
            return Dedent(Text.Substring(start, end - start), indent, includeFirstLine: true);
        }

        #endregion

        #region "Editing"

        /// <summary>
        /// Sets (or, with a null value, removes) a top-level setting.
        /// </summary>
        public void SetRootValue(string key, string formattedValue)
        {
            if (Root != null && TryFindChild(Root, key, out var keyNode, out var valueNode))
            {
                ApplyToExisting(key, keyNode, valueNode, formattedValue);
            }
            else if (formattedValue != null)
            {
                // New top-level keys go at the end of the file
                var position = Text.Length;
                var prefix = Text.Length > 0 && !Text.EndsWith("\n") ? Newline : "";
                AddInsertion(position, key + ": " + formattedValue + Newline, prefix);
            }
        }

        /// <summary>
        /// Sets (or, with a null value, removes) a setting of a module / processor entry.
        /// </summary>
        public void SetEntryValue(ConfigurationEntry entry, string key, string formattedValue)
        {
            if (!entry.IsEditable) throw new ConfigurationEditException($"{entry.Section}[{entry.Index}] ({entry.TypeId}) can only be edited as YAML");

            if (entry.ValueNode is YamlMappingNode mapping && TryFindChild(mapping, key, out var keyNode, out var valueNode))
            {
                ApplyToExisting(key, keyNode, valueNode, formattedValue);
                return;
            }

            if (formattedValue == null) return;

            string indent;
            int position;
            if (entry.ValueNode is YamlMappingNode existing && existing.Children.Count > 0)
            {
                // After the last setting, at the same indentation as the existing settings
                indent = new string(' ', (int)existing.Children.First().Key.Start.Column - 1);
                position = NextLineStart(LastValueEnd(existing));
            }
            else
            {
                // No settings yet: add a block under the type key ("- http-server:")
                indent = new string(' ', (int)entry.KeyNode.Start.Column - 1 + 2);
                position = NextLineStart((int)entry.KeyNode.End.Index);
            }

            var prefix = position == Text.Length && Text.Length > 0 && !Text.EndsWith("\n") ? Newline : "";
            AddInsertion(position, indent + key + ": " + formattedValue + Newline, prefix);
        }

        /// <summary>
        /// Sets (or, with a null builder, removes) a top-level setting whose value is a list, map or object.
        /// The builder returns the complete "key: ..." lines for the given indentation.
        /// </summary>
        public void SetRootBlock(string key, Func<string, string> build)
        {
            if (Root != null && TryFindChild(Root, key, out var keyNode, out var valueNode))
            {
                ReplaceBlock(keyNode, valueNode, build);
            }
            else if (build != null)
            {
                var prefix = Text.Length > 0 && !Text.EndsWith("\n") ? Newline : "";
                AddInsertion(Text.Length, build(""), prefix);
            }
        }

        /// <summary>
        /// Sets (or, with a null builder, removes) a setting of a module / processor entry whose value is a
        /// list, map or object. The builder returns the complete "key: ..." lines for the given indentation.
        /// </summary>
        public void SetEntryBlock(ConfigurationEntry entry, string key, Func<string, string> build)
        {
            if (!entry.IsEditable) throw new ConfigurationEditException($"{entry.Section}[{entry.Index}] ({entry.TypeId}) can only be edited as YAML");

            if (entry.ValueNode is YamlMappingNode mapping && TryFindChild(mapping, key, out var keyNode, out var valueNode))
            {
                ReplaceBlock(keyNode, valueNode, build);
                return;
            }
            if (build == null) return;

            string indent;
            int position;
            if (entry.ValueNode is YamlMappingNode existing && existing.Children.Count > 0)
            {
                indent = new string(' ', (int)existing.Children.First().Key.Start.Column - 1);
                position = NextLineStart(LastValueEnd(existing));
            }
            else
            {
                indent = new string(' ', (int)entry.KeyNode.Start.Column - 1 + 2);
                position = NextLineStart((int)entry.KeyNode.End.Index);
            }

            var prefix = position == Text.Length && Text.Length > 0 && !Text.EndsWith("\n") ? Newline : "";
            AddInsertion(position, build(indent), prefix);
        }

        /// <summary>Replaces (or removes) the "key: value" lines of an existing setting.</summary>
        private void ReplaceBlock(YamlNode keyNode, YamlNode valueNode, Func<string, string> build)
        {
            var start = LineStart((int)keyNode.Start.Index);
            var end = NextLineStart(LastValueEnd(valueNode));
            var indent = new string(' ', (int)keyNode.Start.Column - 1);
            var text = build?.Invoke(indent) ?? "";
            // Keep the line ending when the block was the last thing in the file without one
            if (end == Text.Length && !Text.EndsWith("\n") && text.EndsWith(Newline)) text = text.Substring(0, text.Length - Newline.Length);
            AddEdit(start, end, text);
        }

        /// <summary>
        /// Removes an entry (its "- type:" line through its last setting). When it was separated from the
        /// others by blank lines, the blank line before it goes with it (the one before rather than after, so
        /// a new entry can still be added right after it in the same change).
        /// </summary>
        public void RemoveEntry(ConfigurationEntry entry)
        {
            var start = LineStart((int)entry.Item.Start.Index);
            var end = NextLineStart(LastValueEnd(entry.Item));

            var previousStart = start > 0 ? LineStart(start - 1) : 0;
            var blankBefore = start > 0 && Text.Substring(previousStart, start - previousStart).Trim().Length == 0;
            var blankAfter = end < Text.Length && Text.Substring(end, NextLineStart(end) - end).Trim().Length == 0;
            if (blankBefore && blankAfter) start = previousStart;

            AddEdit(start, end, "");
        }

        /// <summary>The non-scalar settings of a mapping (lists, maps, objects) as plain data.</summary>
        public Dictionary<string, object> GetStructuredValues(YamlNode node)
        {
            var result = new Dictionary<string, object>();
            if (!(node is YamlMappingNode mapping)) return result;
            foreach (var child in mapping.Children)
            {
                var key = (child.Key as YamlScalarNode)?.Value;
                if (key != null && !(child.Value is YamlScalarNode)) result[key] = ToPlainValue(child.Value);
            }
            return result;
        }

        /// <summary>
        /// A YAML value as plain data for the form: mappings become dictionaries, sequences become lists
        /// and scalars become strings.
        /// </summary>
        public static object ToPlainValue(YamlNode node)
        {
            switch (node)
            {
                case YamlMappingNode mapping:
                    var result = new Dictionary<string, object>();
                    foreach (var child in mapping.Children)
                    {
                        var key = (child.Key as YamlScalarNode)?.Value;
                        if (key != null) result[key] = ToPlainValue(child.Value);
                    }
                    return result;
                case YamlSequenceNode sequence:
                    return sequence.Children.Select(ToPlainValue).ToList();
                case YamlScalarNode scalar:
                    return scalar.Value;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Adds a new <c>- type-id:</c> entry with the given (already formatted) settings at the end of a list
        /// section such as <c>modules</c>, creating the section when the file does not have one.
        /// </summary>
        public void AddEntry(string section, string typeId, IList<KeyValuePair<string, string>> formattedValues, IList<Func<string, string>> blocks = null)
        {
            if (string.IsNullOrEmpty(typeId)) throw new ConfigurationEditException("A type is required");

            string BuildItem(int dashIndent)
            {
                var pad = new string(' ', dashIndent);
                var sb = new StringBuilder();
                sb.Append(pad).Append("- ").Append(ConfigurationDocument.QuoteString(typeId)).Append(':').Append(Newline);
                foreach (var value in formattedValues ?? new List<KeyValuePair<string, string>>())
                {
                    // Settings are indented two further than the type key, matching the sample configuration
                    sb.Append(pad).Append("    ").Append(value.Key).Append(": ").Append(value.Value).Append(Newline);
                }
                foreach (var block in blocks ?? new List<Func<string, string>>())
                {
                    sb.Append(block(pad + "    "));
                }
                return sb.ToString();
            }

            YamlNode keyNode = null, sectionNode = null;
            if (Root != null) TryFindChild(Root, section, out keyNode, out sectionNode);

            if (sectionNode is YamlSequenceNode sequence && sequence.Children.Count > 0)
            {
                if (sequence.Style == YamlDotNet.Core.Events.SequenceStyle.Flow) throw new ConfigurationEditException($"The {section} list is written in a compact style; add the entry in the YAML view");

                // After the last entry, separated by a blank line like the existing entries
                var position = NextLineStart(LastValueEnd(sequence));
                var prefix = position == Text.Length && Text.Length > 0 && !Text.EndsWith("\n") ? Newline : "";
                AddInsertion(position, Newline + BuildItem((int)sequence.Start.Column - 1), prefix);
            }
            else if (keyNode != null)
            {
                if (!ConfigurationEntry.IsEmptyScalar(sectionNode) && !(sectionNode is YamlSequenceNode))
                {
                    throw new ConfigurationEditException($"'{section}' is not a list; add the entry in the YAML view");
                }

                // "modules:" with no entries yet: start the list on the following lines
                var position = NextLineStart((int)keyNode.End.Index);
                var prefix = position == Text.Length && Text.Length > 0 && !Text.EndsWith("\n") ? Newline : "";
                AddInsertion(position, BuildItem((int)keyNode.Start.Column - 1), prefix);
            }
            else
            {
                // No section at all: add it at the end of the file
                var prefix = Text.Length > 0 && !Text.EndsWith("\n") ? Newline : "";
                AddInsertion(Text.Length, (Text.Length > 0 ? Newline : "") + section + ":" + Newline + BuildItem(0), prefix);
            }
        }

        /// <summary>
        /// Replaces an entry with new YAML text (as returned by <see cref="GetEntryText"/>, starting with the type key).
        /// </summary>
        public void ReplaceEntryText(ConfigurationEntry entry, string yaml)
        {
            if (string.IsNullOrWhiteSpace(yaml)) throw new ConfigurationEditException($"{entry.Section}[{entry.Index}] cannot be empty");

            // The replacement must be a single "type: settings" pair on its own
            ConfigurationDocument snippet;
            try { snippet = Parse(yaml); }
            catch (YamlException ex) { throw new ConfigurationEditException($"{entry.Section}[{entry.Index}]: {DescribeYamlError(ex)}"); }
            if (snippet.Root == null || snippet.Root.Children.Count != 1)
            {
                throw new ConfigurationEditException($"{entry.Section}[{entry.Index}] must contain exactly one module/processor type key");
            }

            var start = (int)entry.Item.Start.Index;
            var end = LineEnd(LastValueEnd(entry.Item));
            var indent = (int)entry.Item.Start.Column - 1;
            AddEdit(start, end, Indent(yaml.TrimEnd('\r', '\n').Replace("\r\n", "\n").Replace("\n", Newline), indent));
        }

        /// <summary>
        /// Returns the text with all requested changes applied.
        /// </summary>
        public string Apply()
        {
            var edits = new List<TextEdit>(_edits);
            foreach (var insertion in _insertions)
            {
                var prefix = _insertionPrefixes.TryGetValue(insertion.Key, out var p) ? p : "";
                edits.Add(new TextEdit(insertion.Key, insertion.Key, prefix + string.Concat(insertion.Value)));
            }

            // Apply from the end of the text so earlier positions stay valid
            edits = edits.OrderByDescending(o => o.Start).ThenByDescending(o => o.End).ToList();
            for (var i = 1; i < edits.Count; i++)
            {
                if (edits[i].End > edits[i - 1].Start) throw new ConfigurationEditException("Overlapping changes cannot be applied together");
            }

            var sb = new StringBuilder(Text);
            foreach (var edit in edits)
            {
                sb.Remove(edit.Start, edit.End - edit.Start);
                sb.Insert(edit.Start, edit.Replacement);
            }
            return sb.ToString();
        }

        private void ApplyToExisting(string key, YamlNode keyNode, YamlNode valueNode, string formattedValue)
        {
            if (formattedValue == null)
            {
                // Remove the whole "key: value" line(s)
                var start = LineStart((int)keyNode.Start.Index);
                var end = NextLineStart(LastValueEnd(valueNode));
                AddEdit(start, end, "");
                return;
            }

            if (!(valueNode is YamlScalarNode scalar) || scalar.Style == YamlDotNet.Core.ScalarStyle.Literal || scalar.Style == YamlDotNet.Core.ScalarStyle.Folded)
            {
                throw new ConfigurationEditException($"'{key}' has a list or multi-line value and can only be edited as YAML");
            }

            if (ConfigurationEntry.IsEmptyScalar(scalar))
            {
                // "key:" with no value - insert after the colon
                var colon = Text.IndexOf(':', (int)keyNode.End.Index);
                if (colon < 0) throw new ConfigurationEditException($"Could not locate the value of '{key}'");
                AddEdit(colon + 1, colon + 1, " " + formattedValue);
                return;
            }

            AddEdit((int)scalar.Start.Index, (int)scalar.End.Index, formattedValue);
        }

        private void AddEdit(int start, int end, string replacement) => _edits.Add(new TextEdit(start, end, replacement));

        private void AddInsertion(int position, string text, string prefix)
        {
            if (!_insertions.TryGetValue(position, out var list)) _insertions[position] = list = new List<string>();
            list.Add(text);
            if (!string.IsNullOrEmpty(prefix)) _insertionPrefixes[position] = prefix;
        }

        private static bool TryFindChild(YamlMappingNode mapping, string key, out YamlNode keyNode, out YamlNode valueNode)
        {
            foreach (var child in mapping.Children)
            {
                if (child.Key is YamlScalarNode scalar && scalar.Value == key)
                {
                    keyNode = child.Key;
                    valueNode = child.Value;
                    return true;
                }
            }
            keyNode = null;
            valueNode = null;
            return false;
        }

        #endregion

        #region "Positions"

        /// <summary>
        /// The end position of the last scalar inside a node. Block collections report their end at the
        /// next token (possibly after following comments), so the last scalar is used instead.
        /// </summary>
        private static int LastValueEnd(YamlNode node)
        {
            switch (node)
            {
                case YamlMappingNode mapping when mapping.Children.Count > 0:
                    if (mapping.Style == YamlDotNet.Core.Events.MappingStyle.Flow) return (int)mapping.End.Index;
                    var last = mapping.Children.Last();
                    return ConfigurationEntry.IsEmptyScalar(last.Value) ? (int)last.Key.End.Index : LastValueEnd(last.Value);
                case YamlSequenceNode sequence when sequence.Children.Count > 0:
                    if (sequence.Style == YamlDotNet.Core.Events.SequenceStyle.Flow) return (int)sequence.End.Index;
                    return LastValueEnd(sequence.Children.Last());
                default:
                    return (int)node.End.Index;
            }
        }

        private int LineStart(int position)
        {
            var newline = position > 0 ? Text.LastIndexOf('\n', position - 1) : -1;
            return newline + 1;
        }

        /// <summary>Position of the line break ending the line that contains <paramref name="position"/> (excluded).</summary>
        private int LineEnd(int position)
        {
            var newline = Text.IndexOf('\n', Math.Min(position, Text.Length));
            if (newline < 0) return Text.Length;
            return newline > 0 && Text[newline - 1] == '\r' ? newline - 1 : newline;
        }

        /// <summary>Start of the line after the one containing <paramref name="position"/> (or the end of the text).</summary>
        private int NextLineStart(int position)
        {
            var newline = Text.IndexOf('\n', Math.Min(position, Text.Length));
            return newline < 0 ? Text.Length : newline + 1;
        }

        private static string Dedent(string text, int indent, bool includeFirstLine = false)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n');
            if (indent <= 0) return string.Join("\n", lines);
            for (var i = includeFirstLine ? 0 : 1; i < lines.Length; i++)
            {
                var spaces = 0;
                while (spaces < indent && spaces < lines[i].Length && lines[i][spaces] == ' ') spaces++;
                lines[i] = lines[i].Substring(spaces);
            }
            return string.Join("\n", lines);
        }

        private static string Indent(string text, int indent)
        {
            if (indent <= 0) return text;
            var pad = new string(' ', indent);
            var lines = Regex.Split(text, "(?<=\n)");
            for (var i = 1; i < lines.Length; i++)
            {
                if (lines[i].Trim().Length > 0) lines[i] = pad + lines[i];
            }
            return string.Concat(lines);
        }

        #endregion

        #region "Values"

        private static readonly Regex _plainSafe = new Regex(@"^[A-Za-z0-9_./\\@$(][A-Za-z0-9_./\\@$():+\-]*$");
        private static readonly HashSet<string> _reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "true", "false", "yes", "no", "on", "off", "y", "n", "null", "~"
        };

        /// <summary>
        /// Validates a value typed on the form and formats it as YAML scalar text for the field's type.
        /// Returns null for an empty value of a non-string field (meaning "not set").
        /// </summary>
        public static string FormatValue(ConfigurationField field, string value)
        {
            if (value == null) return null;

            switch (field.Type)
            {
                case ConfigurationField.TypeBoolean:
                    if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)) return "true";
                    if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)) return "false";
                    throw new ConfigurationEditException($"'{field.Key}' must be true or false");

                case ConfigurationField.TypeInteger:
                    value = value.Trim();
                    if (value.Length == 0) return null;
                    if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer)) throw new ConfigurationEditException($"'{field.Key}' must be a whole number");
                    return integer.ToString(CultureInfo.InvariantCulture);

                case ConfigurationField.TypeNumber:
                    value = value.Trim();
                    if (value.Length == 0) return null;
                    if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _)) throw new ConfigurationEditException($"'{field.Key}' must be a number");
                    return value;

                case ConfigurationField.TypeEnum:
                    value = value.Trim();
                    if (value.Length == 0) return null;
                    var option = field.Options?.FirstOrDefault(o => string.Equals(o, value, StringComparison.OrdinalIgnoreCase));
                    if (option == null) throw new ConfigurationEditException($"'{field.Key}' must be one of: {string.Join(", ", field.Options ?? new string[0])}");
                    return option;

                case ConfigurationField.TypeString:
                    return QuoteString(value);

                default:
                    throw new ConfigurationEditException($"'{field.Key}' can only be edited as YAML");
            }
        }

        /// <summary>
        /// Formats a string as a YAML scalar, quoting it when a plain scalar would be read differently.
        /// </summary>
        public static string QuoteString(string value)
        {
            if (value.Length > 0 && _plainSafe.IsMatch(value) && !_reserved.Contains(value) && !LooksNumeric(value)) return value;
            return "'" + value.Replace("'", "''") + "'";
        }

        private static bool LooksNumeric(string value) =>
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _) || value.StartsWith("0x", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// A readable description of a YAML parse error.
        /// </summary>
        public static string DescribeYamlError(YamlException ex)
        {
            return $"line {ex.Start.Line}, column {ex.Start.Column}: {ex.InnerException?.Message ?? ex.Message}";
        }

        #endregion

        private class TextEdit
        {
            public int Start { get; }
            public int End { get; }
            public string Replacement { get; }
            public TextEdit(int start, int end, string replacement) { Start = start; End = end; Replacement = replacement; }
        }
    }
}
