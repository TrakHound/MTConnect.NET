// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// One change to a device file, addressed by element <c>id</c>. Operations are applied in order,
    /// so a later operation can refer to an element added (or an id changed) by an earlier one.
    /// </summary>
    internal class DeviceOperation
    {
        /// <summary>setAttributes, setDescription, setConstraints, add, remove, move or replaceXml.</summary>
        public string Op { get; set; }

        /// <summary>The <c>id</c> of the element the operation applies to (the parent for <c>add</c>).</summary>
        public string Target { get; set; }

        /// <summary>For <c>add</c>: component, dataItem or composition.</summary>
        public string Kind { get; set; }

        /// <summary>For <c>add</c> of a component: the element name (for example <c>Linear</c>).</summary>
        public string Tag { get; set; }

        /// <summary>Attributes to set (a null or empty value removes the attribute).</summary>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>For <c>setDescription</c>: the description text.</summary>
        public string Text { get; set; }

        /// <summary>For <c>setConstraints</c>: allowed values.</summary>
        public List<string> Values { get; set; }

        /// <summary>For <c>setConstraints</c>: minimum value.</summary>
        public string Minimum { get; set; }

        /// <summary>For <c>setConstraints</c>: maximum value.</summary>
        public string Maximum { get; set; }

        /// <summary>For <c>setConstraints</c>: nominal value.</summary>
        public string Nominal { get; set; }

        /// <summary>For <c>move</c>: up or down.</summary>
        public string Direction { get; set; }

        /// <summary>For <c>replaceXml</c>: the replacement element XML.</summary>
        public string Xml { get; set; }

        /// <summary>For <c>moveTo</c>: the <c>id</c> of the new parent device or component.</summary>
        public string Parent { get; set; }

        /// <summary>For <c>moveTo</c>: the <c>id</c> of the sibling to place the element before (null for the end).</summary>
        public string Before { get; set; }

        /// <summary>For configuration items: Specifications, CoordinateSystems or Motion.</summary>
        public string Block { get; set; }

        /// <summary>For configuration items: the position of the item within its block (0 for Motion).</summary>
        public int Index { get; set; }

        /// <summary>For configuration items: child element values by path (for example <c>Maximum</c> or <c>Transformation/Translation</c>); empty removes.</summary>
        public Dictionary<string, string> Children { get; set; }
    }

    /// <summary>
    /// A device file (a single <c>&lt;Device&gt;</c> or an <c>MTConnectDevices</c> document) that can be changed
    /// element by element while keeping the rest of the text - comments, indentation, attribute quoting and
    /// formatting - exactly as it was.
    /// </summary>
    internal class DeviceDocument
    {
        private static readonly string[] _componentChildOrder = { "Description", "Configuration", "DataItems", "Components", "Compositions", "References" };
        private static readonly string[] _dataItemChildOrder = { "Source", "Constraints", "Filters", "InitialValue", "ResetTrigger", "Definition", "Relationships" };

        // Configuration blocks and their items, in the order the MTConnect.NET XML reader declares them
        private static readonly string[] _configurationChildOrder = { "CoordinateSystems", "Motion", "Relationships", "SensorConfiguration", "SolidModel", "Specifications", "ImageFiles", "PowerSources" };
        private static readonly string[] _transformationChildOrder = { "Translation", "TranslationDataSet", "Rotation", "RotationDataSet" };
        private static readonly Dictionary<string, string[]> _configurationItemChildOrder = new Dictionary<string, string[]>
        {
            ["Specification"] = new[] { "Maximum", "UpperLimit", "UpperWarning", "Nominal", "LowerWarning", "LowerLimit", "Minimum" },
            ["CoordinateSystem"] = new[] { "Origin", "OriginDataSet", "Transformation", "TransformationDataSet", "Description" },
            ["Motion"] = new[] { "Description", "Origin", "OriginDataSet", "Transformation", "TransformationDataSet", "Axis", "AxisDataSet" }
        };

        /// <summary>The child values the forms edit for each configuration item (paths below the item).</summary>
        internal static readonly Dictionary<string, string[]> ConfigurationItemValues = new Dictionary<string, string[]>
        {
            ["Specification"] = new[] { "Maximum", "UpperLimit", "UpperWarning", "Nominal", "LowerWarning", "LowerLimit", "Minimum" },
            ["CoordinateSystem"] = new[] { "Origin", "Transformation/Translation", "Transformation/Rotation", "Description" },
            ["Motion"] = new[] { "Description", "Origin", "Transformation/Translation", "Transformation/Rotation", "Axis" }
        };

        private static readonly Dictionary<string, string> _blockItemTag = new Dictionary<string, string>
        {
            ["Specifications"] = "Specification",
            ["CoordinateSystems"] = "CoordinateSystem",
            ["Motion"] = "Motion"
        };
        private static readonly Regex _nameRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_.\-]*$");
        private static readonly Regex _tagRegex = new Regex(@"^([A-Za-z_][A-Za-z0-9_.\-]*:)?[A-Za-z_][A-Za-z0-9_.\-]*$");

        private readonly Dictionary<XElement, Span> _spans = new Dictionary<XElement, Span>();
        private readonly List<TextEdit> _edits = new List<TextEdit>();


        /// <summary>The file text.</summary>
        public string Text { get; }

        /// <summary>The parsed document (whitespace preserved).</summary>
        public XDocument Document { get; }

        /// <summary>The line ending used by the file.</summary>
        public string Newline { get; }

        /// <summary>One level of indentation as used by the file (a tab or a number of spaces).</summary>
        public string IndentUnit { get; }

        /// <summary>A hash of <see cref="Text"/> used to detect concurrent changes.</summary>
        public string Version => ConfigurationDocument.ComputeVersion(Text);


        private DeviceDocument(string text, XDocument document)
        {
            Text = text;
            Document = document;
            Newline = text.Contains("\r\n") ? "\r\n" : "\n";
            IndentUnit = DetectIndentUnit(text);
        }

        /// <summary>
        /// Parses a device file. Throws <see cref="XmlException"/> when the XML is not well formed.
        /// </summary>
        public static DeviceDocument Parse(string text)
        {
            text = text ?? "";
            var document = XDocument.Parse(text, LoadOptions.PreserveWhitespace);
            var result = new DeviceDocument(text, document);
            result.MapSpans();
            return result;
        }


        #region "Reading"

        /// <summary>
        /// The Device elements of the file: the root element, or the Device children of
        /// <c>MTConnectDevices/Devices</c>. (Agent elements are generated by the Agent and not included.)
        /// </summary>
        public List<XElement> GetDevices()
        {
            var root = Document.Root;
            if (root == null) return new List<XElement>();
            if (root.Name.LocalName == "Device") return new List<XElement> { root };

            return root.Elements().Where(e => e.Name.LocalName == "Devices")
                .SelectMany(e => e.Elements().Where(d => d.Name.LocalName == "Device"))
                .ToList();
        }

        /// <summary>
        /// What an element is: device, component, dataItem, composition or other.
        /// </summary>
        public static string GetKind(XElement element)
        {
            if (element.Name.LocalName == "Device") return "device";
            switch (element.Parent?.Name.LocalName)
            {
                case "Components": return "component";
                case "DataItems": return "dataItem";
                case "Compositions": return "composition";
                default: return "other";
            }
        }

        /// <summary>
        /// The tree of a Device for the form: attributes, description, constraints, other child elements,
        /// the element's own XML and its components, data items and compositions.
        /// </summary>
        public object BuildNode(XElement element)
        {
            var kind = GetKind(element);
            var children = new List<object>();
            foreach (var container in new[] { "DataItems", "Components", "Compositions" })
            {
                foreach (var child in ChildElements(element, container).SelectMany(c => c.Elements()))
                {
                    children.Add(BuildNode(child));
                }
            }

            var description = element.Elements().FirstOrDefault(e => e.Name.LocalName == "Description");
            var constraints = kind == "dataItem" ? element.Elements().FirstOrDefault(e => e.Name.LocalName == "Constraints") : null;
            var known = new HashSet<string> { "Description", "DataItems", "Components", "Compositions", "Constraints", "Configuration" };
            var configuration = element.Elements().FirstOrDefault(e => e.Name.LocalName == "Configuration");

            return new
            {
                id = (string)element.Attribute("id"),
                kind,
                tag = element.Name.LocalName,
                attributes = element.Attributes().Where(a => !a.IsNamespaceDeclaration && a.Name.Namespace == XNamespace.None)
                    .ToDictionary(a => a.Name.LocalName, a => a.Value),
                description = description == null ? null : new
                {
                    attributes = description.Attributes().Where(a => a.Name.Namespace == XNamespace.None).ToDictionary(a => a.Name.LocalName, a => a.Value),
                    text = NormalizeText(description.Value)
                },
                constraints = constraints == null ? null : new
                {
                    values = constraints.Elements().Where(e => e.Name.LocalName == "Value").Select(e => e.Value.Trim()).ToList(),
                    minimum = constraints.Elements().FirstOrDefault(e => e.Name.LocalName == "Minimum")?.Value.Trim(),
                    maximum = constraints.Elements().FirstOrDefault(e => e.Name.LocalName == "Maximum")?.Value.Trim(),
                    nominal = constraints.Elements().FirstOrDefault(e => e.Name.LocalName == "Nominal")?.Value.Trim()
                },
                other = element.Elements().Select(e => e.Name.LocalName).Where(n => !known.Contains(n)).Distinct().ToList(),
                configuration = kind == "device" || kind == "component" ? BuildConfiguration(configuration) : null,
                xml = GetElementText(element),
                line = ((IXmlLineInfo)element).HasLineInfo() ? ((IXmlLineInfo)element).LineNumber : 0,
                children
            };
        }

        /// <summary>
        /// The Specifications, Coordinate Systems and Motion of a Configuration element for the forms, plus the
        /// names of any other blocks (edited as XML).
        /// </summary>
        private static object BuildConfiguration(XElement configuration)
        {
            object Item(XElement item) => new
            {
                tag = item.Name.LocalName,
                attributes = item.Attributes().Where(a => !a.IsNamespaceDeclaration && a.Name.Namespace == XNamespace.None).ToDictionary(a => a.Name.LocalName, a => a.Value),
                values = (ConfigurationItemValues.TryGetValue(item.Name.LocalName, out var paths) ? paths : new string[0])
                    .Select(path => new { path, element = FindPath(item, path) })
                    .Where(o => o.element != null)
                    .ToDictionary(o => o.path, o => NormalizeText(o.element.Value))
            };

            List<object> Block(string name) => configuration?.Elements().Where(e => e.Name.LocalName == name).SelectMany(e => e.Elements()).Select(Item).ToList() ?? new List<object>();

            var motion = configuration?.Elements().FirstOrDefault(e => e.Name.LocalName == "Motion");
            return new
            {
                specifications = Block("Specifications"),
                coordinateSystems = Block("CoordinateSystems"),
                motion = motion != null ? Item(motion) : null,
                other = configuration?.Elements().Select(e => e.Name.LocalName).Where(n => !_blockItemTag.ContainsKey(n)).Distinct().ToList() ?? new List<string>()
            };
        }

        /// <summary>A child element by a path such as <c>Maximum</c> or <c>Transformation/Translation</c>.</summary>
        private static XElement FindPath(XElement element, string path)
        {
            foreach (var part in path.Split('/'))
            {
                element = element?.Elements().FirstOrDefault(e => e.Name.LocalName == part);
            }
            return element;
        }

        /// <summary>
        /// The element's text as written in the file, with the indentation of its first line removed from
        /// the following lines.
        /// </summary>
        public string GetElementText(XElement element)
        {
            var span = _spans[element];
            var indent = IndentOf(element);
            var lines = Text.Substring(span.Start, span.End - span.Start).Replace("\r\n", "\n").Split('\n');
            for (var i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(indent)) lines[i] = lines[i].Substring(indent.Length);
            }
            return string.Join("\n", lines);
        }

        /// <summary>The element with the given <c>id</c> attribute, or null.</summary>
        public XElement FindById(string id)
        {
            if (string.IsNullOrEmpty(id) || Document.Root == null) return null;
            return Document.Root.DescendantsAndSelf().FirstOrDefault(e => (string)e.Attribute("id") == id);
        }

        private static IEnumerable<XElement> ChildElements(XElement element, string localName) =>
            element.Elements().Where(e => e.Name.LocalName == localName);

        private static string NormalizeText(string value) => Regex.Replace(value ?? "", @"\s+", " ").Trim();

        #endregion

        #region "Operations"

        /// <summary>
        /// Applies operations in order to the text and returns the result. Each operation is applied to the
        /// result of the previous one. Throws <see cref="ConfigurationEditException"/> for invalid operations.
        /// </summary>
        public static string ApplyOperations(string text, IEnumerable<DeviceOperation> operations)
        {
            foreach (var operation in operations ?? Enumerable.Empty<DeviceOperation>())
            {
                switch (operation.Op)
                {
                    // Operations made of several steps; each step works on the result of the previous one
                    case "moveTo": text = MoveTo(text, operation); break;
                    case "addConfigItem":
                    case "setConfigItem":
                    case "removeConfigItem": text = ApplyConfigurationItem(text, operation); break;
                    default: text = Step(text, d => d.Apply(operation)); break;
                }
            }
            return text;
        }

        private static string Step(string text, Action<DeviceDocument> action)
        {
            var document = Parse(text);
            action(document);
            return document.Result();
        }

        private void Apply(DeviceOperation op)
        {
            var target = FindById(op.Target);
            if (target == null) throw new ConfigurationEditException($"No element with id '{op.Target}' was found");

            switch (op.Op)
            {
                case "setAttributes": SetAttributes(target, op.Attributes); break;
                case "setDescription": SetDescription(target, op.Attributes, op.Text); break;
                case "setConstraints": SetConstraints(target, op); break;
                case "add": Add(target, op); break;
                case "remove": Remove(target); break;
                case "move": Move(target, op.Direction); break;
                case "replaceXml": ReplaceXml(target, op.Xml); break;
                default: throw new ConfigurationEditException($"Unknown operation '{op.Op}'");
            }
        }

        private void SetAttributes(XElement element, Dictionary<string, string> attributes, bool requireId = true)
        {
            var span = _spans[element];
            var tag = Text.Substring(span.Start, span.StartTagEnd - span.Start);

            foreach (var attribute in attributes ?? new Dictionary<string, string>())
            {
                if (!_nameRegex.IsMatch(attribute.Key)) throw new ConfigurationEditException($"'{attribute.Key}' is not a valid attribute name");

                if (attribute.Key == "id" && !string.IsNullOrEmpty(attribute.Value) && attribute.Value != (string)element.Attribute("id") && FindById(attribute.Value) != null)
                {
                    throw new ConfigurationEditException($"The id '{attribute.Value}' is already used in this file");
                }
                if (requireId && attribute.Key == "id" && string.IsNullOrEmpty(attribute.Value)) throw new ConfigurationEditException("An id is required");

                var pattern = new Regex(@"(\s+)" + Regex.Escape(attribute.Key) + @"\s*=\s*(""[^""]*""|'[^']*')");
                var match = pattern.Match(tag);
                if (string.IsNullOrEmpty(attribute.Value))
                {
                    if (match.Success) tag = tag.Remove(match.Index, match.Length);
                }
                else if (match.Success)
                {
                    tag = tag.Substring(0, match.Index) + match.Groups[1].Value + attribute.Key + "=\"" + EscapeAttribute(attribute.Value) + "\"" + tag.Substring(match.Index + match.Length);
                }
                else
                {
                    var close = Regex.Match(tag, @"\s*/?>$");
                    tag = tag.Substring(0, close.Index) + " " + attribute.Key + "=\"" + EscapeAttribute(attribute.Value) + "\"" + tag.Substring(close.Index);
                }
            }

            AddEdit(span.Start, span.StartTagEnd, tag);
        }

        private void SetDescription(XElement element, Dictionary<string, string> attributes, string text)
        {
            var kind = GetKind(element);
            if (kind != "device" && kind != "component") throw new ConfigurationEditException("Only devices and components have a description");

            var sb = new StringBuilder("<Description");
            foreach (var name in new[] { "manufacturer", "model", "serialNumber", "station" })
            {
                if (attributes != null && attributes.TryGetValue(name, out var value) && !string.IsNullOrEmpty(value))
                {
                    sb.Append(' ').Append(name).Append("=\"").Append(EscapeAttribute(value)).Append('"');
                }
            }
            var hasAttributes = sb.Length > "<Description".Length;
            if (!string.IsNullOrWhiteSpace(text)) sb.Append('>').Append(EscapeText(text.Trim())).Append("</Description>");
            else sb.Append("/>");

            var existing = element.Elements().FirstOrDefault(e => e.Name.LocalName == "Description");
            if (!hasAttributes && string.IsNullOrWhiteSpace(text))
            {
                if (existing != null) RemoveElementText(existing);
                return;
            }

            var xml = sb.ToString();
            if (existing != null) AddEdit(_spans[existing].Start, _spans[existing].End, xml);
            else InsertChild(element, "Description", _componentChildOrder, indent => xml);
        }

        private void SetConstraints(XElement element, DeviceOperation op)
        {
            if (GetKind(element) != "dataItem") throw new ConfigurationEditException("Only data items have constraints");

            var values = (op.Values ?? new List<string>()).Select(v => v?.Trim()).Where(v => !string.IsNullOrEmpty(v)).ToList();
            var children = values.Select(v => "<Value>" + EscapeText(v) + "</Value>").ToList();
            if (!string.IsNullOrWhiteSpace(op.Minimum)) children.Add("<Minimum>" + EscapeText(op.Minimum.Trim()) + "</Minimum>");
            if (!string.IsNullOrWhiteSpace(op.Maximum)) children.Add("<Maximum>" + EscapeText(op.Maximum.Trim()) + "</Maximum>");
            if (!string.IsNullOrWhiteSpace(op.Nominal)) children.Add("<Nominal>" + EscapeText(op.Nominal.Trim()) + "</Nominal>");

            var existing = element.Elements().FirstOrDefault(e => e.Name.LocalName == "Constraints");
            if (children.Count == 0)
            {
                if (existing != null) RemoveElementText(existing);
                return;
            }

            string Build(string indent) =>
                "<Constraints>" + string.Concat(children.Select(c => Newline + indent + IndentUnit + c)) + Newline + indent + "</Constraints>";

            if (existing != null) AddEdit(_spans[existing].Start, _spans[existing].End, Build(IndentOf(existing)));
            else InsertChild(element, "Constraints", _dataItemChildOrder, Build);
        }

        private void Add(XElement parent, DeviceOperation op)
        {
            string tag;
            switch (op.Kind)
            {
                case "component": tag = op.Tag; break;
                case "dataItem": tag = "DataItem"; break;
                case "composition": tag = "Composition"; break;
                default: throw new ConfigurationEditException($"Unknown kind '{op.Kind}'");
            }
            if (string.IsNullOrEmpty(tag) || !_tagRegex.IsMatch(tag)) throw new ConfigurationEditException($"'{tag}' is not a valid component type");

            var attributes = op.Attributes ?? new Dictionary<string, string>();
            if (!attributes.TryGetValue("id", out var id) || string.IsNullOrEmpty(id)) throw new ConfigurationEditException("An id is required");
            if (FindById(id) != null) throw new ConfigurationEditException($"The id '{id}' is already used in this file");

            InsertItem(parent, op.Kind, BuildElement(tag, attributes), null);
        }

        /// <summary>Builds a self-closing element with the given (non-empty) attributes.</summary>
        private static string BuildElement(string tag, Dictionary<string, string> attributes)
        {
            var sb = new StringBuilder("<").Append(tag);
            foreach (var attribute in (attributes ?? new Dictionary<string, string>()).Where(a => !string.IsNullOrEmpty(a.Value)))
            {
                if (!_nameRegex.IsMatch(attribute.Key)) throw new ConfigurationEditException($"'{attribute.Key}' is not a valid attribute name");
                sb.Append(' ').Append(attribute.Key).Append("=\"").Append(EscapeAttribute(attribute.Value)).Append('"');
            }
            return sb.Append("/>").ToString();
        }

        private static string ContainerFor(string kind)
        {
            switch (kind)
            {
                case "component": return "Components";
                case "dataItem": return "DataItems";
                case "composition": return "Compositions";
                default: throw new ConfigurationEditException($"Unknown kind '{kind}'");
            }
        }

        /// <summary>
        /// Inserts an item (component, data item or composition XML, possibly several lines, without leading
        /// indentation) into the parent's container: before the given sibling, or after the last item. The
        /// container is created when the parent does not have one.
        /// </summary>
        private void InsertItem(XElement parent, string kind, string xml, string beforeId)
        {
            var parentKind = GetKind(parent);
            if (parentKind != "device" && parentKind != "component") throw new ConfigurationEditException("Items can only be added to a device or component");
            if (kind == "composition" && parentKind != "component") throw new ConfigurationEditException("Compositions can only be added to a component");

            var container = ContainerFor(kind);
            var existing = ChildElements(parent, container).FirstOrDefault();

            if (!string.IsNullOrEmpty(beforeId))
            {
                var before = FindById(beforeId);
                if (before == null || before.Parent != existing) throw new ConfigurationEditException($"'{beforeId}' is not in the {container} of '{(string)parent.Attribute("id")}'");
                var indent = IndentOf(before);
                AddEdit(_spans[before].Start, _spans[before].Start, Reindent(xml, indent) + Newline + indent);
            }
            else if (existing != null && existing.Elements().Any())
            {
                // After the last item of the existing container
                var last = existing.Elements().Last();
                var indent = IndentOf(last);
                AddEdit(_spans[last].End, _spans[last].End, Newline + indent + Reindent(xml, indent));
            }
            else if (existing != null)
            {
                // Empty container: rewrite it with the new item
                var indent = IndentOf(existing);
                AddEdit(_spans[existing].Start, _spans[existing].End,
                    "<" + container + ">" + Newline + indent + IndentUnit + Reindent(xml, indent + IndentUnit) + Newline + indent + "</" + container + ">");
            }
            else
            {
                InsertChild(parent, container, _componentChildOrder, indent =>
                    "<" + container + ">" + Newline + indent + IndentUnit + Reindent(xml, indent + IndentUnit) + Newline + indent + "</" + container + ">");
            }
        }

        /// <summary>Indents every line after the first, using the file's line endings.</summary>
        private string Reindent(string xml, string indent)
        {
            var lines = xml.Replace("\r\n", "\n").Split('\n');
            for (var i = 1; i < lines.Length; i++)
            {
                if (lines[i].Trim().Length > 0) lines[i] = indent + lines[i];
            }
            return string.Join(Newline, lines);
        }

        private void Remove(XElement element)
        {
            if (element == Document.Root) throw new ConfigurationEditException("The device of a single-device file cannot be removed");
            if (GetKind(element) == "device" && GetDevices().Count == 1) throw new ConfigurationEditException("The only device of a file cannot be removed");
            RemoveItem(element);
        }

        /// <summary>Removes an item, and its container too when it is the only item (no empty &lt;DataItems/&gt;).</summary>
        private void RemoveItem(XElement element)
        {
            var parent = element.Parent;
            var isContainer = parent != null && (parent.Name.LocalName == "DataItems" || parent.Name.LocalName == "Components" || parent.Name.LocalName == "Compositions");
            if (isContainer && parent.Elements().Count() == 1) RemoveElementText(parent);
            else RemoveElementText(element);
        }

        /// <summary>
        /// Moves a component, data item or composition to another parent (or another position under the same
        /// parent), keeping its text - including comments and children - as written.
        /// </summary>
        private static string MoveTo(string text, DeviceOperation op)
        {
            var document = Parse(text);
            var element = document.FindById(op.Target);
            if (element == null) throw new ConfigurationEditException($"No element with id '{op.Target}' was found");
            var kind = GetKind(element);
            if (kind != "component" && kind != "dataItem" && kind != "composition") throw new ConfigurationEditException("Only components, data items and compositions can be moved");

            var parent = document.FindById(op.Parent);
            if (parent == null) throw new ConfigurationEditException($"No element with id '{op.Parent}' was found");
            if (parent.AncestorsAndSelf().Contains(element)) throw new ConfigurationEditException("An item cannot be moved into itself");
            if (op.Before == op.Target) return text;

            var xml = document.GetElementText(element);
            text = Step(text, d => d.RemoveItem(d.FindById(op.Target)));
            return Step(text, d => d.InsertItem(d.FindById(op.Parent), kind, xml, op.Before));
        }

        /// <summary>
        /// Adds, changes or removes a Specification, Coordinate System or Motion in the Configuration of a device
        /// or component. The Configuration and block elements are created and removed as needed.
        /// </summary>
        private static string ApplyConfigurationItem(string text, DeviceOperation op)
        {
            if (op.Block == null || !_blockItemTag.TryGetValue(op.Block, out var itemTag)) throw new ConfigurationEditException($"Unknown configuration block '{op.Block}'");

            var owner = Parse(text).FindById(op.Target);
            var ownerKind = owner != null ? GetKind(owner) : null;
            if (ownerKind != "device" && ownerKind != "component") throw new ConfigurationEditException($"No device or component with id '{op.Target}' was found");

            var index = op.Index;
            if (op.Op == "addConfigItem")
            {
                var configuration = owner.Elements().FirstOrDefault(e => e.Name.LocalName == "Configuration");
                if (op.Block == "Motion" && configuration?.Elements().Any(e => e.Name.LocalName == "Motion") == true)
                {
                    throw new ConfigurationEditException("This item already has a Motion");
                }

                // Make sure Configuration (and for lists, the block) exist, then add the item at the end
                text = Step(text, d =>
                {
                    var o = d.FindById(op.Target);
                    if (!o.Elements().Any(e => e.Name.LocalName == "Configuration")) d.InsertChild(o, "Configuration", _componentChildOrder, indent => "<Configuration></Configuration>");
                });
                if (op.Block != "Motion")
                {
                    text = Step(text, d =>
                    {
                        var c = d.FindById(op.Target).Elements().First(e => e.Name.LocalName == "Configuration");
                        if (!c.Elements().Any(e => e.Name.LocalName == op.Block)) d.InsertChild(c, op.Block, _configurationChildOrder, indent => "<" + op.Block + "></" + op.Block + ">");
                    });
                }

                var xml = BuildElement(itemTag, op.Attributes);
                text = Step(text, d =>
                {
                    var c = d.FindById(op.Target).Elements().First(e => e.Name.LocalName == "Configuration");
                    if (op.Block == "Motion")
                    {
                        d.InsertChild(c, "Motion", _configurationChildOrder, indent => xml);
                        return;
                    }

                    var block = c.Elements().First(e => e.Name.LocalName == op.Block);
                    var last = block.Elements().LastOrDefault();
                    if (last != null) d.AddEdit(d._spans[last].End, d._spans[last].End, d.Newline + d.IndentOf(last) + xml);
                    else d.InsertChild(block, itemTag, new[] { itemTag }, indent => xml);
                });

                index = op.Block == "Motion" ? 0 : CountItems(Parse(text).FindById(op.Target), op.Block) - 1;
            }
            else if (op.Op == "removeConfigItem")
            {
                return Step(text, d => d.RemoveConfigurationItem(d.LocateConfigurationItem(op.Target, op.Block, index)));
            }
            else
            {
                text = Step(text, d =>
                {
                    var item = d.LocateConfigurationItem(op.Target, op.Block, index);
                    if (op.Attributes != null && op.Attributes.Count > 0) d.SetAttributes(item, op.Attributes, requireId: false);
                });
            }

            // Child values one at a time (each may create or remove elements around the others)
            foreach (var child in op.Children ?? new Dictionary<string, string>())
            {
                if (!ConfigurationItemValues[itemTag].Contains(child.Key)) throw new ConfigurationEditException($"'{child.Key}' cannot be set on a {itemTag}");
                text = Step(text, d => d.SetChildText(d.LocateConfigurationItem(op.Target, op.Block, index), child.Key, child.Value));
            }

            return text;
        }

        private static int CountItems(XElement owner, string block) =>
            owner.Elements().Where(e => e.Name.LocalName == "Configuration").SelectMany(c => c.Elements().Where(e => e.Name.LocalName == block)).SelectMany(b => b.Elements()).Count();

        private XElement LocateConfigurationItem(string ownerId, string block, int index)
        {
            var configuration = FindById(ownerId)?.Elements().FirstOrDefault(e => e.Name.LocalName == "Configuration");
            var item = block == "Motion"
                ? configuration?.Elements().FirstOrDefault(e => e.Name.LocalName == "Motion")
                : configuration?.Elements().Where(e => e.Name.LocalName == block).SelectMany(b => b.Elements()).ElementAtOrDefault(index);
            if (item == null) throw new ConfigurationEditException($"{block} item {index + 1} of '{ownerId}' was not found - reload the device and try again");
            return item;
        }

        /// <summary>Removes a configuration item, and its block and the Configuration element when they become empty.</summary>
        private void RemoveConfigurationItem(XElement item)
        {
            var parent = item.Parent;
            var isMotion = item.Name.LocalName == "Motion";
            var configuration = isMotion ? parent : parent.Parent;

            var blockEmpty = isMotion || parent.Elements().Count() == 1;
            var configurationEmpty = blockEmpty && configuration.Elements().Count() == 1;

            if (configurationEmpty) RemoveElementText(configuration);
            else if (blockEmpty && !isMotion) RemoveElementText(parent);
            else RemoveElementText(item);
        }

        /// <summary>
        /// Sets (or with an empty value removes) a text child of a configuration item, such as <c>Maximum</c> or
        /// <c>Transformation/Translation</c>, in the order the MTConnect.NET reader declares them.
        /// </summary>
        private void SetChildText(XElement item, string path, string value)
        {
            var parts = path.Split('/');
            var order = _configurationItemChildOrder[item.Name.LocalName];
            value = value?.Trim();

            if (parts.Length == 1)
            {
                var existing = item.Elements().FirstOrDefault(e => e.Name.LocalName == parts[0]);
                var xml = "<" + parts[0] + ">" + EscapeText(value) + "</" + parts[0] + ">";
                if (string.IsNullOrEmpty(value)) { if (existing != null) RemoveElementText(existing); }
                else if (existing != null) AddEdit(_spans[existing].Start, _spans[existing].End, xml);
                else InsertChild(item, parts[0], order, indent => xml);
                return;
            }

            // Two levels, for example Transformation/Translation
            var group = item.Elements().FirstOrDefault(e => e.Name.LocalName == parts[0]);
            var child = group?.Elements().FirstOrDefault(e => e.Name.LocalName == parts[1]);
            var childXml = "<" + parts[1] + ">" + EscapeText(value) + "</" + parts[1] + ">";

            if (string.IsNullOrEmpty(value))
            {
                if (child == null) return;
                if (group.Elements().Count() == 1) RemoveElementText(group);
                else RemoveElementText(child);
            }
            else if (group == null)
            {
                InsertChild(item, parts[0], order, indent => "<" + parts[0] + ">" + Newline + indent + IndentUnit + childXml + Newline + indent + "</" + parts[0] + ">");
            }
            else if (child != null)
            {
                AddEdit(_spans[child].Start, _spans[child].End, childXml);
            }
            else
            {
                InsertChild(group, parts[1], _transformationChildOrder, indent => childXml);
            }
        }

        private void Move(XElement element, string direction)
        {
            var sibling = direction == "up" ? element.ElementsBeforeSelf().LastOrDefault() : element.ElementsAfterSelf().FirstOrDefault();
            if (sibling == null) return;

            var a = _spans[element];
            var b = _spans[sibling];
            var aText = Text.Substring(a.Start, a.End - a.Start);
            var bText = Text.Substring(b.Start, b.End - b.Start);
            AddEdit(a.Start, a.End, bText);
            AddEdit(b.Start, b.End, aText);
        }

        private void ReplaceXml(XElement element, string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) throw new ConfigurationEditException("The XML cannot be empty");

            // Check the snippet is a single well-formed element in the context of the document's namespace
            try
            {
                var ns = element.Name.NamespaceName;
                var wrapper = XElement.Parse("<wrapper" + (string.IsNullOrEmpty(ns) ? "" : " xmlns=\"" + EscapeAttribute(ns) + "\"") + ">" + xml + "</wrapper>");
                if (wrapper.Elements().Count() != 1 || wrapper.Nodes().OfType<XText>().Any(t => t.Value.Trim().Length > 0))
                {
                    throw new ConfigurationEditException("The XML must contain exactly one element");
                }
            }
            catch (XmlException ex)
            {
                // The snippet is checked inside a wrapper element; don't mention it in the message
                var message = ex.Message.Contains("'wrapper'") ? "check that every start tag has a matching end tag" : ex.Message;
                throw new ConfigurationEditException("The XML is not well formed: " + message);
            }

            var indent = IndentOf(element);
            var lines = xml.Trim().Replace("\r\n", "\n").Split('\n');
            for (var i = 1; i < lines.Length; i++)
            {
                if (lines[i].Trim().Length > 0) lines[i] = indent + lines[i];
            }
            AddEdit(_spans[element].Start, _spans[element].End, string.Join(Newline, lines));
        }

        /// <summary>
        /// Inserts a child element (built for the given indentation) at the position the MTConnect schema
        /// expects relative to the existing children.
        /// </summary>
        private void InsertChild(XElement parent, string name, string[] order, Func<string, string> build)
        {
            var span = _spans[parent];
            var parentIndent = IndentOf(parent);
            var children = parent.Elements().ToList();

            if (children.Count > 0)
            {
                var indent = IndentOf(children[0]);
                var rank = Array.IndexOf(order, name);
                var predecessor = children.LastOrDefault(c => { var r = Array.IndexOf(order, c.Name.LocalName); return r >= 0 && r < rank; });

                if (predecessor != null)
                {
                    var end = _spans[predecessor].End;
                    AddEdit(end, end, Newline + indent + build(indent));
                }
                else
                {
                    var first = _spans[children[0]].Start;
                    AddEdit(first, first, build(indent) + Newline + indent);
                }
                return;
            }

            var childIndent = parentIndent + IndentUnit;
            if (span.IsEmpty)
            {
                // <Rotary id="c1"/> becomes <Rotary id="c1">...</Rotary>
                var tag = Text.Substring(span.Start, span.StartTagEnd - span.Start);
                var close = Regex.Match(tag, @"\s*/>$");
                var qualifiedName = Regex.Match(tag, @"^<([^\s/>]+)").Groups[1].Value;
                AddEdit(span.Start, span.End,
                    tag.Substring(0, close.Index) + ">" + Newline + childIndent + build(childIndent) + Newline + parentIndent + "</" + qualifiedName + ">");
            }
            else
            {
                // Start and end tags with no child elements (whitespace or comments only)
                AddEdit(span.StartTagEnd, span.EndTagStart, Newline + childIndent + build(childIndent) + Newline + parentIndent);
            }
        }

        /// <summary>Removes an element, including its whole line(s) when nothing else is on them.</summary>
        private void RemoveElementText(XElement element)
        {
            var span = _spans[element];
            var start = span.Start;
            var end = span.End;

            var lineStart = Text.LastIndexOf('\n', Math.Max(0, start - 1)) + 1;
            if (start == 0) lineStart = 0;
            var lineEnd = Text.IndexOf('\n', end);
            var after = lineEnd < 0 ? Text.Substring(end) : Text.Substring(end, lineEnd - end);

            if (Text.Substring(lineStart, start - lineStart).Trim().Length == 0 && after.Trim().Length == 0)
            {
                start = lineStart;
                end = lineEnd < 0 ? Text.Length : lineEnd + 1;
            }
            AddEdit(start, end, "");
        }

        #endregion

        #region "Text"

        /// <summary>The text with this document's edits applied.</summary>
        public string Result()
        {
            var edits = _edits.OrderByDescending(o => o.Start).ThenByDescending(o => o.End).ToList();
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

        private void AddEdit(int start, int end, string replacement) => _edits.Add(new TextEdit(start, end, replacement));

        /// <summary>The leading whitespace of the line the element starts on.</summary>
        private string IndentOf(XElement element)
        {
            var start = _spans[element].Start;
            var lineStart = start == 0 ? 0 : Text.LastIndexOf('\n', start - 1) + 1;
            var prefix = Text.Substring(lineStart, start - lineStart);
            var length = 0;
            while (length < prefix.Length && (prefix[length] == ' ' || prefix[length] == '\t')) length++;
            return prefix.Substring(0, length);
        }

        private static string DetectIndentUnit(string text)
        {
            var spaces = int.MaxValue;
            foreach (Match match in Regex.Matches(text, @"\n([ \t]+)<"))
            {
                var indent = match.Groups[1].Value;
                if (indent[0] == '\t') return "\t";
                spaces = Math.Min(spaces, indent.Length);
            }
            return spaces == int.MaxValue ? "\t" : new string(' ', Math.Min(spaces, 8));
        }

        /// <summary>Escapes a value for use inside a double-quoted attribute.</summary>
        public static string EscapeAttribute(string value) =>
            (value ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

        private static string EscapeText(string value) =>
            (value ?? "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

        /// <summary>
        /// Records where every element's start tag, contents and end tag are in the text, by reading it with an
        /// <see cref="XmlReader"/> in step with the document's elements.
        /// </summary>
        private void MapSpans()
        {
            var lineStarts = new List<int> { 0 };
            for (var i = 0; i < Text.Length; i++) if (Text[i] == '\n') lineStarts.Add(i + 1);
            int IndexOf(int line, int position) => lineStarts[line - 1] + position - 1;

            var elements = Document.Root?.DescendantsAndSelf().ToList() ?? new List<XElement>();
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, IgnoreWhitespace = false, IgnoreComments = false };
            var stack = new Stack<Span>();
            var index = 0;

            using (var reader = XmlReader.Create(new StringReader(Text), settings))
            {
                var info = (IXmlLineInfo)reader;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        var nameStart = IndexOf(info.LineNumber, info.LinePosition);
                        var span = new Span { Start = nameStart - 1 };
                        span.StartTagEnd = FindTagEnd(nameStart);
                        span.IsEmpty = reader.IsEmptyElement;

                        if (index >= elements.Count) throw new ConfigurationEditException("Could not map the device file");
                        _spans[elements[index++]] = span;

                        if (span.IsEmpty)
                        {
                            span.End = span.StartTagEnd;
                            span.EndTagStart = span.StartTagEnd;
                        }
                        else stack.Push(span);
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        var nameStart = IndexOf(info.LineNumber, info.LinePosition);
                        var span = stack.Pop();
                        span.EndTagStart = nameStart - 2;
                        span.End = Text.IndexOf('>', nameStart) + 1;
                    }
                }
            }
        }

        /// <summary>Index after the '&gt;' that ends a start tag, skipping quoted attribute values.</summary>
        private int FindTagEnd(int position)
        {
            char quote = '\0';
            for (var i = position; i < Text.Length; i++)
            {
                var c = Text[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; }
                else if (c == '"' || c == '\'') quote = c;
                else if (c == '>') return i + 1;
            }
            throw new ConfigurationEditException("Unterminated tag in the device file");
        }

        private class Span
        {
            public int Start;
            public int StartTagEnd;
            public int EndTagStart;
            public int End;
            public bool IsEmpty;
        }

        private class TextEdit
        {
            public int Start { get; }
            public int End { get; }
            public string Replacement { get; }
            public TextEdit(int start, int end, string replacement) { Start = start; End = end; Replacement = replacement; }
        }

        #endregion
    }
}
