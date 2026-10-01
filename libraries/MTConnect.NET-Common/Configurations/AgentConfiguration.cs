// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MTConnect.Configurations
{
    /// <summary>
    /// Configuration for an MTConnect Agent
    /// </summary>
    public class AgentConfiguration : IAgentConfiguration
    {
        private const string BackupDirectoryName = "backup";


        /// <summary>
        /// The conventional file name for a user-supplied JSON agent configuration.
        /// </summary>
        public const string JsonFilename = "agent.config.json";

        /// <summary>
        /// The file name of the shipped JSON configuration used as a fallback when no user JSON configuration is present.
        /// </summary>
        public const string DefaultJsonFilename = "agent.config.default.json";


        /// <summary>
        /// The conventional file name for a user-supplied YAML agent configuration.
        /// </summary>
        public const string YamlFilename = "agent.config.yaml";

        /// <summary>
        /// The file name of the shipped YAML configuration used as a fallback when no user YAML configuration is present.
        /// </summary>
        public const string DefaultYamlFilename = "agent.config.default.yaml";


        /// <summary>
        /// An opaque token regenerated each time the configuration is saved, allowing consumers to detect that the configuration has changed.
        /// </summary>
        [JsonPropertyName("changeToken")]
        public string ChangeToken { get; set; }

        /// <summary>
        /// The file system path the configuration was loaded from; not serialized, and used as the default target when the configuration is saved.
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public string Path { get; set; }

        /// <summary>
        /// The value emitted as the <c>Header/@sender</c> attribute on MTConnect response documents (see MTConnect Part 1 §7). When null or empty, <see cref="MTConnect.Agents.MTConnectAgent"/> falls back to <see cref="System.Net.Dns.GetHostName"/>.
        /// </summary>
        [JsonPropertyName("sender")]
        public string Sender { get; set; }


        /// <summary>
        /// The maximum number of Observations the agent can hold in its buffer
        /// </summary>
        [JsonPropertyName("observationBufferSize")]
        public uint ObservationBufferSize { get; set; }

        /// <summary>
        /// The maximum number of assets the agent can hold in its buffer
        /// </summary>
        [JsonPropertyName("assetBufferSize")]
        public uint AssetBufferSize { get; set; }


        /// <summary>
        /// Sets the TimeZone to use when timestamps are output from the Agent
        /// </summary>
        [JsonPropertyName("timezoneOutput")]
        public string TimeZoneOutput { get; set; }

        /// <summary>
        /// Overwrite timestamps with the agent time. 
        /// This will correct clock drift but will not give as accurate relative time since it will not take into consideration network latencies. 
        /// This can be overridden on a per adapter basis.
        /// </summary>
        [JsonPropertyName("ignoreTimestamps")]
        public bool IgnoreTimestamps { get; set; }

        /// <summary>
        /// Gets or Sets the default MTConnect version to output response documents for.
        /// </summary>
        [JsonIgnore]
        [YamlIgnore]
        public Version DefaultVersion { get; set; }

        /// <summary>
        /// The string form of <see cref="DefaultVersion"/> used for serialization; assigning a parseable version string updates <see cref="DefaultVersion"/>.
        /// </summary>
        [JsonPropertyName("defaultVersion")]
        [YamlMember(Alias = "defaultVersion")]
        public string DefaultVersionValue
        {
            get => DefaultVersion?.ToString();
            set
            {
                if (value != null)
                {
                    if (Version.TryParse(value, out var version))
                    {
                        DefaultVersion = version;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the default for Converting Units when adding Observations
        /// </summary>
        [JsonPropertyName("convertUnits")]
        public bool ConvertUnits { get; set; }

        /// <summary>
        /// Gets or Sets the default for Ignoring the case of Observation values
        /// </summary>
        [JsonPropertyName("ignoreObservationCase")]
        public bool IgnoreObservationCase { get; set; }

        /// <summary>
        /// Gets or Sets whether validation information is output
        /// </summary>
        [JsonPropertyName("enableValidation")]
        public bool EnableValidation { get; set; }

        // Nullable backing field collapses the pre-fix pair (a non-nullable enum
        // field plus a parallel `_isDeviceValidationLevelExplicit` boolean) into a
        // single is-explicit-or-not signal: null means "no explicit assignment —
        // the getter self-mirrors from _inputValidationLevel and Normalize will
        // latch the same mirror into the backing field", non-null means
        // "explicitly assigned, do not mirror". Dime cycle-1 finding M1
        // (simplification); cycle-2 M3-C2 hardened the null branch to mirror at
        // read time so programmatic-only callers observe the same value the
        // load-path Normalize() would set.
        private DeviceValidationLevel? _deviceValidationLevel;
        private InputValidationLevel _inputValidationLevel;

        /// <summary>
        /// Gets or Sets the default Device (MTConnectDevices) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When a configuration file omits this key the loader mirrors <see cref="InputValidationLevel"/>
        /// onto Device validation, preserving pre-v7 behavior for consumers that only knew the single
        /// <see cref="InputValidationLevel"/> knob. Setting this property — either programmatically or via a
        /// key present in the source document — latches the value as explicit (the nullable backing field
        /// becomes non-null) and disables the mirror on the next <see cref="Normalize"/>. An assignment
        /// whose ordinal is not a defined enum arm raises <see cref="ArgumentOutOfRangeException"/>.
        /// </para>
        /// <para>
        /// <b>Save-latches-mirror behavior.</b> The getter self-mirrors from
        /// <see cref="InputValidationLevel"/> when the backing field is still null (i.e. this key was
        /// never explicitly set), and JSON/YAML serializers observe the getter's return value — not the
        /// nullable backing field. That means <see cref="SaveJson"/> / <see cref="SaveYaml"/> on a
        /// configuration whose DVL was never explicitly set writes the mirrored ordinal into the
        /// document. On the next <see cref="Read{T}"/> the deserializer hits an explicit key and
        /// latches it, converting the previously implicit mirror into an EXPLICIT stored value. Runtime
        /// <see cref="InputValidationLevel"/> changes after that reload therefore do NOT re-mirror onto
        /// DVL — the operator must clear DVL back to implicit (currently only possible via a fresh
        /// construction) or set DVL explicitly. A caller that mutates IVL after a save→reload round-trip
        /// and expects DVL to follow should call <see cref="Normalize"/> explicitly on a freshly-loaded
        /// configuration BEFORE any programmatic IVL edit.
        /// </para>
        /// </remarks>
        [JsonPropertyName("deviceValidationLevel")]
        public DeviceValidationLevel DeviceValidationLevel
        {
            // Self-computed mirror in the null case — a caller that sets
            // `InputValidationLevel = Strict` on a fresh AgentConfiguration and
            // reads `DeviceValidationLevel` before calling Normalize() sees the
            // mirrored value (Strict), not the bare Warning default. Normalize()
            // still latches the mirror into the backing field so post-Normalize
            // serialisation carries the concrete value rather than null. Dime
            // cycle-2 finding M3-C2 — closes the programmatic-only footgun the
            // load-path Normalize() call papered over. Cycle-3 F-SEC-002 replaced
            // the raw `(DeviceValidationLevel)(int)_inputValidationLevel` cast
            // with an exhaustive switch — see MapInputToDeviceValidationLevel.
            get => _deviceValidationLevel ?? MapInputToDeviceValidationLevel(_inputValidationLevel);
            set
            {
                ThrowIfUndefined(
                    value,
                    "DeviceValidationLevel must be one of Ignore (0), Warning (1), Remove (2), Strict (3).");
                _deviceValidationLevel = value;
            }
        }

        /// <summary>
        /// Gets or Sets the default Input (Observation or Asset) validation level. 0 = Ignore, 1 = Warning, 2 = Remove, 3 = Strict.
        /// </summary>
        /// <remarks>
        /// An assignment whose ordinal is not a defined enum arm raises
        /// <see cref="ArgumentOutOfRangeException"/>.
        /// </remarks>
        [JsonPropertyName("inputValidationLevel")]
        public InputValidationLevel InputValidationLevel
        {
            get => _inputValidationLevel;
            set
            {
                ThrowIfUndefined(
                    value,
                    "InputValidationLevel must be one of Ignore (0), Warning (1), Remove (2), Strict (3).");
                _inputValidationLevel = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether an empty, null, or whitespace-only Result is preserved for Event DataItems
        /// whose Type has a controlled vocabulary (for example EXECUTION, CONTROLLER_MODE). Defaults to
        /// <c>false</c>, which coerces such Results to <c>UNAVAILABLE</c>. Numeric DataItems (all Samples,
        /// and the numeric-typed Events enumerated by the MTConnect Standard SysML) are always coerced
        /// regardless of this flag; free-form String Event DataItems (PROGRAM, MESSAGE, TOOL_ID,
        /// ASSET_CHANGED, and every other non-vocabulary Type) always preserve the empty Result.
        /// </summary>
        [JsonPropertyName("allowEmptyResultForEnumEvents")]
        public bool AllowEmptyResultForEnumEvents { get; set; }


        /// <summary>
        /// Gets or Sets whether the Agent Device is output
        /// </summary>
        [JsonPropertyName("enableAgentDevice")]
        public bool EnableAgentDevice { get; set; }

        /// <summary>
        /// Gets or Sets whether Metrics are captured (ex. ObserationUpdateRate, AssetUpdateRate)
        /// </summary>
        [JsonPropertyName("enableMetrics")]
        public bool EnableMetrics { get; set; }


        /// <summary>
        /// Initializes a new instance with the default agent settings (128K observation buffer, 1K asset buffer, latest MTConnect version, warning-level validation, unit conversion and metrics enabled).
        /// </summary>
        public AgentConfiguration()
        {
            ObservationBufferSize = 131072;
            AssetBufferSize = 1024;
            DefaultVersion = MTConnectVersions.Max;
            // Leave _deviceValidationLevel null (its default). Going through the public
            // setter would latch it as explicit and disable both the load-time
            // migration mirror in Normalize and the read-time self-mirror in the
            // getter — a caller that constructed with `{ InputValidationLevel = X }`
            // would then observe Warning instead of X. Dime cycle-2 findings M3-C2
            // (getter self-mirror) and L5-C2 (drop the redundant explicit-null
            // assignment — `DeviceValidationLevel?` defaults to null already).
            _inputValidationLevel = InputValidationLevel.Warning;
            AllowEmptyResultForEnumEvents = false;
            ConvertUnits = true;
            IgnoreObservationCase = false;
            EnableAgentDevice = true;
            EnableMetrics = true;
        }

        /// <summary>
        /// Applies post-deserialisation defaults that depend on cross-property state.
        /// When the source configuration omitted <see cref="DeviceValidationLevel"/>, mirror
        /// <see cref="InputValidationLevel"/> onto it. Both enums share ordinals 0-3, so the mirror is a
        /// direct cast.
        /// </summary>
        /// <remarks>
        /// Invoked by every <see cref="Read{T}(string)"/> / <see cref="ReadJson{T}(string)"/> /
        /// <see cref="ReadYaml{T}(string)"/> path so consumers who only set
        /// <see cref="InputValidationLevel"/> in their configuration observe the same Device-validation
        /// behavior they got before the split. Callers loading a configuration programmatically may invoke
        /// <see cref="Normalize"/> once construction is complete to pick up the same default.
        /// </remarks>
        public void Normalize()
        {
            // Explicit null-check + assign assigns the mirror only when the
            // backing field is still null — i.e. neither a programmatic setter
            // call nor a source-document key has latched DeviceValidationLevel
            // to an explicit value. The sticky-suppression semantics (an
            // explicit DVL assignment beats a later IVL change on the next
            // Normalize) fall out of the null-check. Written as a plain
            // `if (x == null) x = y` rather than the C# 8 `??=` operator so
            // the multi-TFM Release pack compiles under the oldest target
            // framework's language version (net461/net47 default to C# 7.3);
            // per Otto's "use the features of the oldest language version"
            // directive 2026-08-21.
            if (_deviceValidationLevel == null) _deviceValidationLevel = MapInputToDeviceValidationLevel(_inputValidationLevel);
        }

        /// <summary>
        /// Maps an <see cref="InputValidationLevel"/> onto its
        /// <see cref="DeviceValidationLevel"/> mirror. Both enums currently share
        /// ordinals 0-3, so the naive <c>(DeviceValidationLevel)(int)value</c>
        /// bit-cast is behaviorally identical today — but the cast is a static
        /// alias with no compile-time signal if either enum ever grows an
        /// asymmetric arm (or reorders one). An explicit switch expression fires
        /// CS8509 when a future <see cref="InputValidationLevel"/> arm lacks a
        /// mapping, forcing the maintainer to decide the target-side mirror
        /// intentionally. The default arm rethrows so a shipped mismatch is
        /// surfaced at runtime rather than silently coercing to an undefined
        /// <see cref="DeviceValidationLevel"/> value.
        /// </summary>
        /// <param name="value">The source <see cref="InputValidationLevel"/> to mirror.</param>
        /// <returns>The <see cref="DeviceValidationLevel"/> mirror of <paramref name="value"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="value"/> is not a mapped arm — indicates the enum has grown without a corresponding switch arm here.</exception>
        /// <remarks>
        /// Written as a classical <c>switch</c> statement rather than the C# 8 switch
        /// expression so the multi-TFM Release pack compiles under the oldest target
        /// framework's language version (net461/net47 default to C# 7.3); the
        /// exhaustive default arm preserves the runtime-throw semantics on any
        /// unmapped ordinal. Per Otto's "use the features of the oldest language
        /// version" directive 2026-08-21.
        /// </remarks>
        private static DeviceValidationLevel MapInputToDeviceValidationLevel(InputValidationLevel value)
        {
            switch (value)
            {
                case InputValidationLevel.Ignore: return DeviceValidationLevel.Ignore;
                case InputValidationLevel.Warning: return DeviceValidationLevel.Warning;
                case InputValidationLevel.Remove: return DeviceValidationLevel.Remove;
                case InputValidationLevel.Strict: return DeviceValidationLevel.Strict;
                default: throw new InvalidOperationException($"Unmapped InputValidationLevel ordinal: {(int)value}");
            }
        }

        /// <summary>
        /// Walks the <see cref="Exception.InnerException"/> chain looking for an
        /// <see cref="ArgumentOutOfRangeException"/>. Deserializers (YamlDotNet
        /// notably) wrap setter throws in one or more layers of their own
        /// container exception, so the direct <c>catch (ArgumentOutOfRangeException)</c>
        /// filter is insufficient. Depth-bounded so a pathological deeply-nested
        /// chain does not loop forever.
        /// </summary>
        private static ArgumentOutOfRangeException UnwrapArgumentOutOfRange(Exception ex)
        {
            const int MaxUnwrapDepth = 16;
            var current = ex;
            for (var i = 0; i < MaxUnwrapDepth && current != null; i++)
            {
                if (current is ArgumentOutOfRangeException aoore) return aoore;
                current = current.InnerException;
            }
            // Depth-cap hit: the walk gave up before finding an AOORE — trace so a
            // pathological wrapping chain does not silently miss a bad-enum surface.
            // Dime cycle-2 finding L2-C2.
            if (current != null)
            {
                Trace.TraceWarning($"UnwrapArgumentOutOfRange: exceeded MaxUnwrapDepth={MaxUnwrapDepth}; original AOORE (if any) suppressed");
            }
            return null;
        }

        /// <summary>
        /// Shared triage wrapper for the four <c>Read{Json,Yaml}[&lt;T&gt;]</c> loader
        /// methods. Each loader was previously duplicating the same three-clause
        /// catch triage (direct AOORE → wrapped-AOORE via
        /// <see cref="UnwrapArgumentOutOfRange"/> → generic fall-through that
        /// traces and returns null) in-line. Extracting the triage into one
        /// helper collapses ~96 lines of duplication and simultaneously closes the
        /// cycle-1-vs-cycle-2 asymmetry (dime M2-C2 subsumes M1-C2): the
        /// <see cref="ReadJson{T}(string)"/> path was missing the middle
        /// <c>when Unwrap...</c> catch, so a wrapped enum error deserialized by
        /// <c>System.Text.Json</c> was falling through to the generic
        /// trace-and-return-null branch instead of raising
        /// <see cref="ArgumentException"/> like the other three loaders. Sharing
        /// the same body by construction fixes the asymmetry forever.
        /// </summary>
        /// <typeparam name="T">The concrete return type of the deserializer call — constrained to <see cref="AgentConfiguration"/> so the helper can set <see cref="AgentConfiguration.Path"/> and call <see cref="AgentConfiguration.Normalize"/> on the loaded instance.</typeparam>
        /// <param name="configurationPath">The resolved configuration path — used both in the surfaced <see cref="ArgumentException"/> message, in the generic-fall-through <see cref="Trace.TraceError(string)"/> line, and stamped onto the loaded configuration's <see cref="AgentConfiguration.Path"/> property.</param>
        /// <param name="deserialize">A closure that runs the deserializer and returns the loaded configuration (or null when the source text was empty). The helper takes care of stamping <see cref="AgentConfiguration.Path"/> and invoking <see cref="AgentConfiguration.Normalize"/> on a non-null return, so the closure only needs to build its deserializer options and return the deserialized value.</param>
        private static T LoadWithTriage<T>(string configurationPath, Func<T> deserialize) where T : AgentConfiguration
        {
            try
            {
                var configuration = deserialize();
                if (configuration != null)
                {
                    configuration.Path = configurationPath;
                    configuration.Normalize();
                }
                return configuration;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Invalid enum value in the source document — surface the actionable
                // setter message with the offending configuration path attached so
                // the operator can trace the bad key back to its file.
                throw new ArgumentException(
                    $"Invalid enum value in {configurationPath}: {ex.Message}",
                    ex);
            }
            catch (Exception ex) when (UnwrapArgumentOutOfRange(ex) is ArgumentOutOfRangeException aoore)
            {
                // Deserializers (YamlDotNet notably, and System.Text.Json when it
                // routes through a JsonConverter) wrap setter throws inside one or
                // more layers of their own container exception; walk the
                // InnerException chain so a bad-enum config surfaces the same
                // actionable message shape as the direct AOORE catch above.
                throw new ArgumentException(
                    $"Invalid enum value in {configurationPath}: {aoore.Message}",
                    aoore);
            }
            catch (Exception ex)
            {
                // Parse / IO / unexpected failures preserve the null-return loader
                // contract, but no longer swallow silently — trace the path and
                // message so downstream operators see the diagnostic.
                Trace.TraceError($"Config load failed: {configurationPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentOutOfRangeException"/> when <paramref name="value"/>
        /// is not a defined <typeparamref name="TEnum"/> arm. Extracted from the
        /// duplicated setter throw blocks on <see cref="DeviceValidationLevel"/> and
        /// <see cref="InputValidationLevel"/> — dime cycle-1 finding L4. The message
        /// is caller-supplied so each setter can name the enum it guards.
        /// </summary>
        /// <remarks>
        /// The .NET 5+ generic <c>Enum.IsDefined&lt;TEnum&gt;</c> overload avoids the
        /// boxing that the legacy <c>Enum.IsDefined(Type, object)</c> path incurs;
        /// the netstandard2.0 branch keeps the reflection form since the generic
        /// overload was not introduced until .NET 5.
        /// </remarks>
        private static void ThrowIfUndefined<TEnum>(TEnum value, string message)
            where TEnum : struct, Enum
        {
#if NET5_0_OR_GREATER
            if (Enum.IsDefined(value)) return;
#else
            if (Enum.IsDefined(typeof(TEnum), value)) return;
#endif
            throw new ArgumentOutOfRangeException(
                "value",
                value,
                message);
        }


        /// <summary>
        /// Loads an <see cref="AgentConfiguration"/>, auto-detecting JSON or YAML; see <see cref="Read{T}(string)"/> for the resolution rules.
        /// </summary>
        /// <param name="path">An explicit configuration path, or null to probe the base directory for the conventional files.</param>
        public static AgentConfiguration Read(string path = null) => Read<AgentConfiguration>(path);

        /// <summary>
        /// Loads an <see cref="AgentConfiguration"/> from a JSON file.
        /// </summary>
        /// <param name="path">An explicit JSON path, or null to use the conventional file in the base directory.</param>
        public static AgentConfiguration ReadJson(string path = null) => ReadJson<AgentConfiguration>(path);

        /// <summary>
        /// Loads an <see cref="AgentConfiguration"/> from a YAML file.
        /// </summary>
        /// <param name="path">An explicit YAML path, or null to use the conventional file in the base directory.</param>
        public static AgentConfiguration ReadYaml(string path = null) => ReadYaml<AgentConfiguration>(path);


        /// <summary>
        /// Loads a derived configuration, treating an explicit path as YAML and otherwise preferring a JSON file in the base directory before falling back to YAML. Returns null when no file can be read.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit configuration path (resolved relative to the base directory when not rooted), or null to auto-detect.</param>
        public static T Read<T>(string path = null) where T : AgentConfiguration
        {
            if (!string.IsNullOrEmpty(path))
            {
                var configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }

                return ReadYaml<T>(configurationPath);
            }
            else
            {
                var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);

                // Test for JSON Configuration File
                if (File.Exists(jsonPath)) return ReadJson<T>(jsonPath);
                else
                {
                    var yamlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);

                    return ReadYaml<T>(yamlPath);
                }
            }
        }

        /// <summary>
        /// Loads a configuration of the given runtime type, preferring a JSON file in the base directory before falling back to YAML.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit configuration path, or null to auto-detect.</param>
        public static AgentConfiguration Read(Type type, string path = null)
        {
            var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);

            // Test for JSON Configuration File
            if (File.Exists(jsonPath)) return ReadJson(type, jsonPath);
            else
            {
                var yamlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);

                return ReadYaml(type, yamlPath);
            }
        }


        /// <summary>
        /// Deserializes a derived configuration from a JSON file, ignoring comments. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit JSON path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static T ReadJson<T>(string path = null) where T : AgentConfiguration
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                return LoadWithTriage(configurationPath, () =>
                {
                    var text = File.ReadAllText(configurationPath);
                    if (string.IsNullOrEmpty(text)) return null;

                    var options = new JsonSerializerOptions()
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip
                    };

                    return JsonSerializer.Deserialize<T>(text, options);
                });
            }

            return null;
        }

        /// <summary>
        /// Deserializes a configuration of the given runtime type from a JSON file, ignoring comments. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit JSON path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static AgentConfiguration ReadJson(Type type, string path = null)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                return LoadWithTriage(configurationPath, () =>
                {
                    var text = File.ReadAllText(configurationPath);
                    if (string.IsNullOrEmpty(text)) return null;

                    var options = new JsonSerializerOptions()
                    {
                        ReadCommentHandling = JsonCommentHandling.Skip
                    };

                    return (AgentConfiguration)JsonSerializer.Deserialize(text, type, options);
                });
            }

            return null;
        }


        /// <summary>
        /// Deserializes a derived configuration from a YAML file using camelCase naming, ignoring unmatched properties. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <typeparam name="T">The concrete configuration type to deserialize.</typeparam>
        /// <param name="path">An explicit YAML path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static T ReadYaml<T>(string path = null) where T : AgentConfiguration
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                return LoadWithTriage(configurationPath, () =>
                {
                    var text = File.ReadAllText(configurationPath);
                    if (string.IsNullOrEmpty(text)) return null;

                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();

                    return deserializer.Deserialize<T>(text);
                });
            }

            return null;
        }

        /// <summary>
        /// Deserializes a configuration of the given runtime type from a YAML file using camelCase naming, ignoring unmatched properties. Returns null when the file is missing, empty, or cannot be parsed.
        /// </summary>
        /// <param name="type">The concrete configuration type to deserialize into.</param>
        /// <param name="path">An explicit YAML path (resolved relative to the base directory when not rooted), or null to use the conventional file.</param>
        public static AgentConfiguration ReadYaml(Type type, string path = null)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);
            if (!string.IsNullOrEmpty(path))
            {
                configurationPath = path;
                if (!System.IO.Path.IsPathRooted(configurationPath))
                {
                    configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationPath);
                }
            }

            if (!string.IsNullOrEmpty(configurationPath))
            {
                return LoadWithTriage(configurationPath, () =>
                {
                    var text = File.ReadAllText(configurationPath);
                    if (string.IsNullOrEmpty(text)) return null;

                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build();

                    return (AgentConfiguration)deserializer.Deserialize(text, type);
                });
            }

            return null;
        }



        /// <summary>
        /// Serializes this configuration to JSON and writes it to disk, regenerating <see cref="ChangeToken"/> and optionally backing up any existing file. Write failures are swallowed.
        /// </summary>
        /// <param name="path">The destination path; when null the conventional JSON file in the base directory is used.</param>
        /// <param name="createBackup">When true, an existing file is copied into a timestamped backup before being overwritten.</param>
        public void SaveJson(string path = null, bool createBackup = true)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JsonFilename);
            if (path != null) configurationPath = path;

            if (createBackup)
            {
                // Create Backup of Configuration File
                var backupDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BackupDirectoryName);
                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
                var backupFilename = System.IO.Path.ChangeExtension(UnixDateTime.Now.ToString(), ".backup.json");
                var backupPath = System.IO.Path.Combine(backupDir, backupFilename);
                if (File.Exists(configurationPath))
                {
                    File.Copy(configurationPath, backupPath);
                }
            }

            // Update ChangeToken
            ChangeToken = Guid.NewGuid().ToString();

            try
            {
                var json = JsonSerializer.Serialize(this);
                File.WriteAllText(configurationPath, json);
            }
            catch { }
        }

        /// <summary>
        /// Serializes this configuration to YAML and writes it to disk, regenerating <see cref="ChangeToken"/> and optionally backing up any existing file. Write failures are swallowed.
        /// </summary>
        /// <param name="path">The destination path; when null the conventional YAML file in the base directory is used.</param>
        /// <param name="createBackup">When true, an existing file is copied into a timestamped backup before being overwritten.</param>
        public void SaveYaml(string path = null, bool createBackup = true)
        {
            var configurationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, YamlFilename);
            if (path != null) configurationPath = path;

            if (createBackup)
            {
                // Create Backup of Configuration File
                var backupDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BackupDirectoryName);
                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
                var backupFilename = System.IO.Path.ChangeExtension(UnixDateTime.Now.ToString(), ".backup.yaml");
                var backupPath = System.IO.Path.Combine(backupDir, backupFilename);
                if (File.Exists(configurationPath))
                {
                    File.Copy(configurationPath, backupPath);
                }
            }

            // Update ChangeToken
            ChangeToken = Guid.NewGuid().ToString();

            try
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(this);
                File.WriteAllText(configurationPath, yaml);
            }
            catch { }
        }
    }
}