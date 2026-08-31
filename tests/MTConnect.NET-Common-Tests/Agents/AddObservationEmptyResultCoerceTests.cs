// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Linq;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Observations;
using NUnit.Framework;

namespace MTConnect.Tests.Common.Agents
{
    /// <summary>
    /// Pins the value-class-aware empty-Result contract on
    /// <see cref="MTConnectAgent.AddObservation(string, MTConnect.Input.IObservationInput, bool?, bool?, bool?, bool)"/>.
    ///
    /// The MTConnect Standard, Part 2 - Devices Information Model classifies each DataItem
    /// by the shape of value its Result carries; the SDK coerces null / empty / whitespace
    /// Results only when the DataItem's value class forbids the empty string:
    /// <list type="bullet">
    ///   <item>Numeric DataItems (all Samples per the "Sample MUST always be reported in
    ///     float" requirement, and the numeric-typed Events enumerated in the SysML
    ///     model - PART_COUNT, LINE_NUMBER, BLOCK_COUNT, HARDNESS, TOOL_OFFSET, and
    ///     kindred integer/float Result types) are always coerced.</item>
    ///   <item>Enumeration Events (EXECUTION, CONTROLLER_MODE, AVAILABILITY, and every
    ///     other Type with a controlled vocabulary) are coerced by default; the
    ///     <see cref="IAgentConfiguration.AllowEmptyResultForEnumEvents"/> escape hatch
    ///     preserves the empty Result when set to <c>true</c>.</item>
    ///   <item>Free-form String Events (PROGRAM, MESSAGE, TOOL_NUMBER, ASSET_CHANGED, and
    ///     every other non-vocabulary Type) preserve the empty Result verbatim: the
    ///     standard's default value type for <c>Observation::result</c> is <c>string</c>,
    ///     and the reference C++ agent accepts empty strings for these Events
    ///     (confirmed by the MTConnect.NET maintainer on PR #217, 2026-08-18).</item>
    /// </list>
    ///
    /// The convenience overload
    /// <c>AddObservation(string deviceKey, string dataItemKey, object value, DateTime timestamp)</c>
    /// exercises the canonical
    /// <c>AddObservation(string, IObservationInput, ...)</c> path that every other
    /// AddObservation overload routes through; the tests cast values to <c>object</c> so
    /// the compiler resolves that overload unambiguously against the
    /// <c>(deviceKey, dataItemKey, valueKey, value)</c> sibling.
    /// </summary>
    [TestFixture]
    [Category("AddObservationEmptyResultCoerce")]
    public class AddObservationEmptyResultCoerceTests
    {
        private const string DeviceKey = "U-COERCE";
        private const string DeviceId = "d-coerce";

        private static readonly InputValidationLevel[] _nonStrictLevels =
        {
            InputValidationLevel.Ignore,
            InputValidationLevel.Warning,
            InputValidationLevel.Remove,
        };

        private static readonly object?[] _nullEmptyWhitespaceValues =
        {
            new object?[] { null },
            new object?[] { string.Empty },
            new object?[] { "   " },
            new object?[] { "\t" },
            new object?[] { "\n" },
        };


        // -------------------------------------------------------------------- //
        // Numeric class: SAMPLE                                                //
        // Spec: MTConnect Part 2, Value Properties of Sample -                 //
        // "Sample MUST always be reported in float."                           //
        // -------------------------------------------------------------------- //

        /// <summary>Empty Result on a SAMPLE DataItem is coerced to UNAVAILABLE under every non-Strict input-validation level.</summary>
        [Test]
        [TestCaseSource(nameof(_nonStrictLevels))]
        public void Sample_EmptyResult_Coerced_To_Unavailable_Under_NonStrict_Levels(InputValidationLevel level)
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            using var agent = NewAgent(level, dataItem: new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True, "empty-Result Sample observation must reach the buffer post-coerce");
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo(Observation.Unavailable),
                "Part 2 mandates that Sample values be numeric; empty MUST become UNAVAILABLE");
        }

        /// <summary>The null / empty / whitespace family is coerced for every SAMPLE DataItem.</summary>
        [Test]
        [TestCaseSource(nameof(_nullEmptyWhitespaceValues))]
        public void Sample_NullEmptyOrWhitespaceResult_Coerced_To_Unavailable(object? badValue)
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object?)badValue!, DateTime.UtcNow);

            Assert.That(added, Is.True);
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo(Observation.Unavailable));
        }

        /// <summary>Strict input-validation still admits an empty Sample Result via the coerce path rather than silently dropping it.</summary>
        [Test]
        public void Sample_EmptyResult_Under_Strict_Coerced_And_Lands()
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            using var agent = NewAgent(InputValidationLevel.Strict, dataItem: new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True, "Strict must coerce to UNAVAILABLE - never silently drop an empty Result");
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo(Observation.Unavailable));
        }


        // -------------------------------------------------------------------- //
        // Enumeration class: EVENT with controlled vocabulary                  //
        // Spec: MTConnect Part 2, Observation::result - the Result MUST be a   //
        // member of the controlled vocabulary defined for the DataItem's Type. //
        // AVAILABILITY (Availability enum: AVAILABLE, UNAVAILABLE) exercises   //
        // an EVENT Type with a small, unambiguous vocabulary.                  //
        // -------------------------------------------------------------------- //

        /// <summary>Empty Result on an Enumeration EVENT DataItem is coerced to UNAVAILABLE when the flag defaults to false.</summary>
        [Test]
        [TestCaseSource(nameof(_nullEmptyWhitespaceValues))]
        public void EnumEvent_EmptyResult_Coerced_To_Unavailable_When_Flag_False(object? badValue)
        {
            const string dataItemKey = AvailabilityDataItem.NameId;
            using var agent = NewAgent(
                InputValidationLevel.Warning,
                allowEmptyResultForEnumEvents: false,
                dataItem: new AvailabilityDataItem(DeviceId));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object?)badValue!, DateTime.UtcNow);

            Assert.That(added, Is.True);
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo(Observation.Unavailable));
        }

        /// <summary>Empty Result on an Enumeration EVENT DataItem is preserved verbatim when the escape-hatch flag is enabled.</summary>
        [Test]
        public void EnumEvent_EmptyResult_Preserved_When_Flag_True()
        {
            const string dataItemKey = AvailabilityDataItem.NameId;
            using var agent = NewAgent(
                InputValidationLevel.Warning,
                allowEmptyResultForEnumEvents: true,
                dataItem: new AvailabilityDataItem(DeviceId));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True);
            var currentResult = CurrentResult(agent, dataItemKey) ?? string.Empty;
            Assert.That(currentResult, Is.EqualTo(string.Empty),
                "AllowEmptyResultForEnumEvents=true preserves the empty Result for controlled-vocabulary Events");
        }

        /// <summary>A concrete vocabulary member is preserved verbatim on an Enumeration EVENT DataItem regardless of the flag.</summary>
        [Test]
        public void EnumEvent_ConcreteResult_Is_Preserved_Verbatim()
        {
            const string dataItemKey = AvailabilityDataItem.NameId;
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: new AvailabilityDataItem(DeviceId));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)"AVAILABLE", DateTime.UtcNow);

            Assert.That(added, Is.True);
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo("AVAILABLE"),
                "the coerce must not substitute a sentinel for a valid controlled-vocabulary member");
        }


        // -------------------------------------------------------------------- //
        // String class: free-form EVENT (PROGRAM, MESSAGE, TOOL_ID)            //
        // Spec: MTConnect Part 2, Observation::result - the default value type //
        // for Observation::result is `string`; the standard does not forbid    //
        // the empty string for non-vocabulary Event Types. The reference C++   //
        // agent accepts empty strings for these Events.                        //
        // -------------------------------------------------------------------- //

        /// <summary>Empty Result on a PROGRAM Event DataItem is preserved verbatim: PROGRAM is a free-form String Event Type.</summary>
        [Test]
        [TestCaseSource(nameof(_nullEmptyWhitespaceValues))]
        public void StringEvent_Program_EmptyResult_Preserved(object? badValue)
        {
            const string dataItemKey = ProgramDataItem.NameId;
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: new ProgramDataItem(DeviceId, ProgramDataItem.SubTypes.ACTIVE));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object?)badValue!, DateTime.UtcNow);

            Assert.That(added, Is.True);
            // Null / whitespace values pass through unchanged; the observation's Result equals the input's string form.
            var expected = badValue?.ToString() ?? string.Empty;
            var currentResult = (CurrentResult(agent, dataItemKey) as string) ?? string.Empty;
            Assert.That(currentResult, Is.EqualTo(expected),
                "PROGRAM is a free-form String Event Type; empty and whitespace Results MUST be preserved");
        }

        /// <summary>Empty Result on a MESSAGE Event DataItem is preserved verbatim.</summary>
        [Test]
        public void StringEvent_Message_EmptyResult_Preserved()
        {
            const string dataItemKey = MessageDataItem.NameId;
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: new MessageDataItem(DeviceId));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True);
            var currentResult = (CurrentResult(agent, dataItemKey) as string) ?? string.Empty;
            Assert.That(currentResult, Is.EqualTo(string.Empty),
                "MESSAGE is a free-form String Event Type; empty Results MUST be preserved");
        }

        /// <summary>Empty Result on a TOOL_NUMBER Event DataItem is preserved verbatim.</summary>
        [Test]
        public void StringEvent_ToolNumber_EmptyResult_Preserved()
        {
            const string dataItemKey = ToolNumberDataItem.NameId;
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: new ToolNumberDataItem(DeviceId));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True);
            var currentResult = (CurrentResult(agent, dataItemKey) as string) ?? string.Empty;
            Assert.That(currentResult, Is.EqualTo(string.Empty),
                "TOOL_NUMBER is a free-form String Event Type; empty Results MUST be preserved");
        }

        /// <summary>A concrete String Result is passed through verbatim on a free-form Event DataItem.</summary>
        [Test]
        public void StringEvent_Program_ConcreteResult_Is_Preserved_Verbatim()
        {
            const string dataItemKey = ProgramDataItem.NameId;
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: new ProgramDataItem(DeviceId, ProgramDataItem.SubTypes.ACTIVE));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)"PART_A.NC", DateTime.UtcNow);

            Assert.That(added, Is.True);
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo("PART_A.NC"),
                "PROGRAM MUST pass free-form text through verbatim");
        }


        // -------------------------------------------------------------------- //
        // Numeric-typed EVENT (SysML integer/float Result)                     //
        // -------------------------------------------------------------------- //

        /// <summary>Empty Result on a numeric-typed Event (PART_COUNT: SysML `result: integer`) is coerced.</summary>
        [Test]
        public void NumericEvent_PartCount_EmptyResult_Coerced_To_Unavailable()
        {
            const string dataItemKey = PartCountDataItem.NameId;
            using var agent = NewAgent(
                InputValidationLevel.Warning,
                dataItem: new PartCountDataItem(DeviceId, PartCountDataItem.SubTypes.ALL));

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True);
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo(Observation.Unavailable),
                "PART_COUNT's Result type is integer per the SysML model; empty MUST become UNAVAILABLE");
        }


        // -------------------------------------------------------------------- //
        // Numeric-typed EVENT allow-list exhaustiveness (SysML integer/float)  //
        // -------------------------------------------------------------------- //

        /// <summary>
        /// The SysML numeric-typed Event allow-list mirrored in <see cref="DataItem.GetValueClass"/>.
        /// Every entry MUST classify as <see cref="DataItemValueClass.Numeric"/>; adding a new numeric
        /// Event type without extending this list, or dropping an entry, is caught here.
        /// </summary>
        private static readonly string[] _numericEventTypeAllowList =
        {
            "ACTIVATION_COUNT",
            "AXIS_FEEDRATE_OVERRIDE",
            "BLOCK_COUNT",
            "CYCLE_COUNT",
            "DEACTIVATION_COUNT",
            "HARDNESS",
            "LINE_NUMBER",
            "LOAD_COUNT",
            "MATERIAL_LAYER",
            "MEASUREMENT_VALUE",
            "NETWORK_PORT",
            "PART_COUNT",
            "PART_INDEX",
            "PATH_FEEDRATE_OVERRIDE",
            "PROGRAM_NEST_LEVEL",
            "ROTARY_VELOCITY_OVERRIDE",
            "THICKNESS",
            "TOOL_OFFSET",
            "TRANSFER_COUNT",
            "UNCERTAINTY",
            "UNLOAD_COUNT",
        };

        /// <summary>
        /// Every entry in the SysML numeric-typed Event allow-list classifies as
        /// <see cref="DataItemValueClass.Numeric"/> so its empty Result is coerced,
        /// not preserved verbatim.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(_numericEventTypeAllowList))]
        public void GetValueClass_Numeric_Event_AllowList_Entry_Is_Numeric(string typeId)
        {
            var dataItem = new DataItem
            {
                Id = $"{DeviceId}_{typeId}",
                Category = DataItemCategory.EVENT,
                Type = typeId,
                Representation = DataItemRepresentation.VALUE,
            };

            Assert.That(DataItem.GetValueClass(dataItem), Is.EqualTo(DataItemValueClass.Numeric),
                $"SysML numeric-typed Event '{typeId}' MUST classify as Numeric so its empty Result is coerced");
        }

        /// <summary>
        /// Every entry in the SysML numeric-typed Event allow-list has its empty Result coerced to
        /// <c>UNAVAILABLE</c> through <c>AddObservation</c>, exercising the coerce path end-to-end
        /// for each type.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(_numericEventTypeAllowList))]
        public void NumericEvent_AllowList_Entry_EmptyResult_Coerced_To_Unavailable(string typeId)
        {
            var dataItemKey = $"{typeId}_key";
            var dataItem = new DataItem
            {
                Id = $"{DeviceId}_{typeId}",
                Name = dataItemKey,
                Category = DataItemCategory.EVENT,
                Type = typeId,
                Representation = DataItemRepresentation.VALUE,
            };

            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var added = agent.AddObservation(DeviceKey, dataItemKey, (object)string.Empty, DateTime.UtcNow);

            Assert.That(added, Is.True, $"{typeId} empty-Result observation must reach the buffer post-coerce");
            Assert.That(CurrentResult(agent, dataItemKey), Is.EqualTo(Observation.Unavailable),
                $"{typeId} is numeric per the SysML model; empty MUST become UNAVAILABLE");
        }


        // -------------------------------------------------------------------- //
        // DataItemValueClass switch arm guard                                  //
        // -------------------------------------------------------------------- //

        /// <summary>
        /// Guard: every <see cref="DataItemValueClass"/> enum value has been observed in the
        /// classifier's output for a representative DataItem. Adding a new arm without a matching
        /// coerce-path branch is caught here as a compile-time-adjacent tripwire.
        /// </summary>
        [Test]
        public void DataItemValueClass_All_Enum_Arms_Are_Reachable_From_GetValueClass()
        {
            var observedArms = new System.Collections.Generic.HashSet<DataItemValueClass>
            {
                DataItem.GetValueClass(new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)),
                DataItem.GetValueClass(new AvailabilityDataItem(DeviceId)),
                DataItem.GetValueClass(new ProgramDataItem(DeviceId, ProgramDataItem.SubTypes.ACTIVE)),
            };

            foreach (DataItemValueClass arm in Enum.GetValues(typeof(DataItemValueClass)))
            {
                Assert.That(observedArms, Does.Contain(arm),
                    $"Enum arm {arm} has no representative DataItem covered by GetValueClass; add one or extend the classifier");
            }
        }


        // -------------------------------------------------------------------- //
        // Classifier direct tests                                              //
        // -------------------------------------------------------------------- //

        /// <summary>Direct assertion: <see cref="DataItem.GetValueClass"/> classifies representative DataItems into the three value classes.</summary>
        [Test]
        public void GetValueClass_Classifies_Representative_DataItems()
        {
            Assert.That(DataItem.GetValueClass(new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)), Is.EqualTo(DataItemValueClass.Numeric),
                "SAMPLE observations are Numeric per Part 2 - Sample MUST be float");
            Assert.That(DataItem.GetValueClass(new AvailabilityDataItem(DeviceId)), Is.EqualTo(DataItemValueClass.Enumeration),
                "AVAILABILITY has a controlled vocabulary (Availability enum)");
            Assert.That(DataItem.GetValueClass(new ExecutionDataItem(DeviceId)), Is.EqualTo(DataItemValueClass.Enumeration),
                "EXECUTION has a controlled vocabulary (Execution enum)");
            Assert.That(DataItem.GetValueClass(new ProgramDataItem(DeviceId, ProgramDataItem.SubTypes.ACTIVE)), Is.EqualTo(DataItemValueClass.String),
                "PROGRAM carries free-form text");
            Assert.That(DataItem.GetValueClass(new MessageDataItem(DeviceId)), Is.EqualTo(DataItemValueClass.String),
                "MESSAGE carries free-form text");
            Assert.That(DataItem.GetValueClass(new ToolNumberDataItem(DeviceId)), Is.EqualTo(DataItemValueClass.String),
                "TOOL_ID carries free-form text");
            Assert.That(DataItem.GetValueClass(new AssetChangedDataItem(DeviceId)), Is.EqualTo(DataItemValueClass.String),
                "ASSET_CHANGED carries the asset id as free-form text");
            Assert.That(DataItem.GetValueClass(new PartCountDataItem(DeviceId, PartCountDataItem.SubTypes.ALL)), Is.EqualTo(DataItemValueClass.Numeric),
                "PART_COUNT Result is integer per the SysML model");
        }


        // -------------------------------------------------------------------- //
        // Structured-representation SAMPLE tests                               //
        // Spec: MTConnect Part 2, Value Properties of Sample - the             //
        // DATA_SET / TABLE / TIME_SERIES representations carry structured      //
        // payloads (Entries, Cells, Samples) rather than a single Result.      //
        // The empty-Result coerce MUST NOT fire for these representations:     //
        // their inputs legitimately omit the Result key by design, and         //
        // corrupting them with the UNAVAILABLE sentinel would break spec       //
        // compliance for every legitimate multi-value observation.             //
        // -------------------------------------------------------------------- //

        /// <summary>A TIME_SERIES SAMPLE observation with a real Samples payload MUST NOT be coerced to UNAVAILABLE despite the absence of a Result key.</summary>
        [Test]
        public void Sample_TimeSeries_Payload_Preserved_Not_Coerced()
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            var dataItem = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.TIME_SERIES,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedSamples = new[] { 1.0, 2.0, 3.0 };
            var input = new MTConnect.Input.TimeSeriesObservationInput(dataItemKey, expectedSamples, sampleRate: 10.0)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };
            input.SampleCount = expectedSamples.Length;

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "TIME_SERIES SAMPLE observation must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "TIME_SERIES SAMPLE observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "TIME_SERIES SAMPLE payload MUST NOT be corrupted to UNAVAILABLE - the Result key is absent by design");
            Assert.That(current.GetValue(ValueKeys.SampleCount).ToInt(), Is.EqualTo(expectedSamples.Length),
                "TIME_SERIES SAMPLE SampleCount MUST survive - the coerce path must not overwrite it with 0");
            Assert.That(TimeSeriesObservation.GetSamples(current.Values).ToArray(), Is.EqualTo(expectedSamples),
                "TIME_SERIES SAMPLE Samples payload MUST survive verbatim - no structural loss to the coerce path");
        }

        /// <summary>A DATA_SET SAMPLE observation with real Entries MUST NOT be coerced to UNAVAILABLE.</summary>
        [Test]
        public void Sample_DataSet_Payload_Preserved_Not_Coerced()
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            var dataItem = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.DATA_SET,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedEntries = new IDataSetEntry[]
            {
                new DataSetEntry("a", "1.0"),
                new DataSetEntry("b", "2.0"),
            };
            var input = new MTConnect.Input.DataSetObservationInput(dataItemKey, expectedEntries)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "DATA_SET SAMPLE observation must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "DATA_SET SAMPLE observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "DATA_SET SAMPLE payload MUST NOT be corrupted to UNAVAILABLE - the Result key is absent by design");
            Assert.That(current.GetValue(ValueKeys.Count).ToInt(), Is.EqualTo(expectedEntries.Length),
                "DATA_SET SAMPLE Count MUST survive - the coerce path must not overwrite it with 0");
        }

        /// <summary>A TABLE SAMPLE observation with real Cells MUST NOT be coerced to UNAVAILABLE.</summary>
        [Test]
        public void Sample_Table_Payload_Preserved_Not_Coerced()
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            var dataItem = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.TABLE,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedEntries = new ITableEntry[]
            {
                new TableEntry("row1", new ITableCell[]
                {
                    new TableCell("col1", "1.0"),
                    new TableCell("col2", "2.0"),
                }),
            };
            var input = new MTConnect.Input.TableObservationInput(dataItemKey, expectedEntries)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "TABLE SAMPLE observation must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "TABLE SAMPLE observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "TABLE SAMPLE payload MUST NOT be corrupted to UNAVAILABLE - the Result key is absent by design");
            Assert.That(current.GetValue(ValueKeys.Count).ToInt(), Is.EqualTo(expectedEntries.Length),
                "TABLE SAMPLE Count MUST survive - the coerce path must not overwrite it with 0");
        }

        /// <summary>Direct classifier assertion: SAMPLE DataItems with non-VALUE representations are classified as String (their coercion is not this classifier's concern).</summary>
        [Test]
        public void GetValueClass_Sample_NonValueRepresentation_Is_String()
        {
            var timeSeries = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.TIME_SERIES,
            };
            Assert.That(DataItem.GetValueClass(timeSeries), Is.EqualTo(DataItemValueClass.String),
                "SAMPLE + TIME_SERIES carries a Samples payload rather than a single Result - the classifier must not report Numeric");

            var dataSet = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.DATA_SET,
            };
            Assert.That(DataItem.GetValueClass(dataSet), Is.EqualTo(DataItemValueClass.String),
                "SAMPLE + DATA_SET carries an Entries payload rather than a single Result - the classifier must not report Numeric");

            var table = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.TABLE,
            };
            Assert.That(DataItem.GetValueClass(table), Is.EqualTo(DataItemValueClass.String),
                "SAMPLE + TABLE carries a Cells payload rather than a single Result - the classifier must not report Numeric");
        }

        /// <summary>Direct classifier assertion: EVENT DataItems with non-VALUE representations are classified as String, symmetric with the SAMPLE side.</summary>
        [Test]
        public void GetValueClass_Event_NonValueRepresentation_Is_String()
        {
            // A representative Enumeration Event (AVAILABILITY) classifies as Enumeration on the
            // VALUE path; the same DataItem instance with a non-VALUE representation must
            // short-circuit to String at the top of GetValueClass, mirroring the SAMPLE side.
            var eventDataSet = new AvailabilityDataItem(DeviceId)
            {
                Representation = DataItemRepresentation.DATA_SET,
            };
            Assert.That(DataItem.GetValueClass(eventDataSet), Is.EqualTo(DataItemValueClass.String),
                "EVENT + DATA_SET carries structured Entries rather than a single Result - the classifier must not report Enumeration");

            var eventTable = new AvailabilityDataItem(DeviceId)
            {
                Representation = DataItemRepresentation.TABLE,
            };
            Assert.That(DataItem.GetValueClass(eventTable), Is.EqualTo(DataItemValueClass.String),
                "EVENT + TABLE carries structured Cells rather than a single Result - the classifier must not report Enumeration");

            var eventTimeSeries = new AvailabilityDataItem(DeviceId)
            {
                Representation = DataItemRepresentation.TIME_SERIES,
            };
            Assert.That(DataItem.GetValueClass(eventTimeSeries), Is.EqualTo(DataItemValueClass.String),
                "EVENT + TIME_SERIES (though not spec-permitted for EVENTs) must still short-circuit to String rather than fall through the Enumeration path");

            // Also cover a numeric-typed Event allow-list entry with a non-VALUE representation:
            // the non-VALUE short-circuit must override the numeric-Event allow-list too.
            var numericEventDataSet = new DataItem
            {
                Id = $"{DeviceId}_PART_COUNT_DS",
                Category = DataItemCategory.EVENT,
                Type = "PART_COUNT",
                Representation = DataItemRepresentation.DATA_SET,
            };
            Assert.That(DataItem.GetValueClass(numericEventDataSet), Is.EqualTo(DataItemValueClass.String),
                "EVENT + DATA_SET on a numeric-typed Event Type must still be String - representation trumps the SysML numeric allow-list");
        }

        /// <summary>Direct classifier assertion: CONDITION DataItems classify as String; the top-of-function short-circuit added alongside the non-VALUE gate applies regardless of Type or Representation.</summary>
        [Test]
        public void GetValueClass_Condition_Is_String()
        {
            // A CONDITION DataItem carries a condition state (Normal / Warning / Fault / Unavailable)
            // rather than a Result value; the coerce path is gated by ConditionLevel and MUST NOT
            // be driven by the empty-Result classifier.
            var conditionValue = new DataItem
            {
                Id = $"{DeviceId}_SYSTEM",
                Category = DataItemCategory.CONDITION,
                Type = "SYSTEM",
                Representation = DataItemRepresentation.VALUE,
            };
            Assert.That(DataItem.GetValueClass(conditionValue), Is.EqualTo(DataItemValueClass.String),
                "CONDITION observations report a condition state; the classifier must short-circuit to String even when Representation == VALUE");

            // Belt-and-braces: a CONDITION with a non-VALUE representation still classifies as String.
            var conditionDataSet = new DataItem
            {
                Id = $"{DeviceId}_SYSTEM_DS",
                Category = DataItemCategory.CONDITION,
                Type = "SYSTEM",
                Representation = DataItemRepresentation.DATA_SET,
            };
            Assert.That(DataItem.GetValueClass(conditionDataSet), Is.EqualTo(DataItemValueClass.String),
                "CONDITION + DATA_SET classifies as String; the CONDITION short-circuit fires ahead of the representation gate");
        }

        /// <summary>Null-guard: <see cref="DataItem.GetValueClass"/> returns String for a null DataItem argument rather than throwing.</summary>
        [Test]
        public void GetValueClass_Null_DataItem_Is_String()
        {
            Assert.That(DataItem.GetValueClass(null), Is.EqualTo(DataItemValueClass.String),
                "GetValueClass(null) must return String rather than throw - defensively fails safe for callers with an unresolved DataItem");
        }


        // -------------------------------------------------------------------- //
        // Symmetric EVENT + non-VALUE representation tests                     //
        // Spec: MTConnect Part 2, Value Properties of Event - DATA_SET and     //
        // TABLE representations carry structured payloads (Entries / Cells)    //
        // rather than a single Result. The classifier's non-VALUE short-       //
        // circuit fires for EVENT the same way it fires for SAMPLE; these      //
        // tests exercise the branch end-to-end via AddObservation so any       //
        // regression that re-narrows the short-circuit to SAMPLE-only is       //
        // caught here.                                                         //
        // -------------------------------------------------------------------- //

        /// <summary>An EVENT + DATA_SET observation with real Entries MUST NOT be coerced to UNAVAILABLE despite the absence of a Result key.</summary>
        [Test]
        public void EnumEvent_DataSet_Payload_Preserved_Not_Coerced()
        {
            // Use AVAILABILITY (a controlled-vocabulary Enum Event) with DATA_SET representation.
            // On the VALUE path it classifies as Enumeration and would trigger coerce on an empty
            // Result; on the DATA_SET path it MUST classify as String and preserve the payload.
            const string dataItemKey = "avail_ds";
            var dataItem = new DataItem
            {
                Id = $"{DeviceId}_AVAIL_DS",
                Name = dataItemKey,
                Category = DataItemCategory.EVENT,
                Type = "AVAILABILITY",
                Representation = DataItemRepresentation.DATA_SET,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedEntries = new IDataSetEntry[]
            {
                new DataSetEntry("channel_1", "AVAILABLE"),
                new DataSetEntry("channel_2", "UNAVAILABLE"),
            };
            var input = new MTConnect.Input.DataSetObservationInput(dataItemKey, expectedEntries)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "EVENT + DATA_SET observation must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "EVENT + DATA_SET observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "EVENT + DATA_SET payload MUST NOT be corrupted to UNAVAILABLE - the Result key is absent by design");
            Assert.That(current.GetValue(ValueKeys.Count).ToInt(), Is.EqualTo(expectedEntries.Length),
                "EVENT + DATA_SET Count MUST survive - the coerce path must not overwrite it with 0");
        }

        /// <summary>An EVENT + TABLE observation with real Cells MUST NOT be coerced to UNAVAILABLE despite the absence of a Result key.</summary>
        [Test]
        public void EnumEvent_Table_Payload_Preserved_Not_Coerced()
        {
            const string dataItemKey = "avail_tab";
            var dataItem = new DataItem
            {
                Id = $"{DeviceId}_AVAIL_TAB",
                Name = dataItemKey,
                Category = DataItemCategory.EVENT,
                Type = "AVAILABILITY",
                Representation = DataItemRepresentation.TABLE,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedEntries = new ITableEntry[]
            {
                new TableEntry("row_1", new ITableCell[]
                {
                    new TableCell("state", "AVAILABLE"),
                    new TableCell("nested", "UNAVAILABLE"),
                }),
            };
            var input = new MTConnect.Input.TableObservationInput(dataItemKey, expectedEntries)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "EVENT + TABLE observation must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "EVENT + TABLE observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "EVENT + TABLE payload MUST NOT be corrupted to UNAVAILABLE - the Result key is absent by design");
            Assert.That(current.GetValue(ValueKeys.Count).ToInt(), Is.EqualTo(expectedEntries.Length),
                "EVENT + TABLE Count MUST survive - the coerce path must not overwrite it with 0");
        }


        // -------------------------------------------------------------------- //
        // Explicit empty / whitespace Result on non-VALUE representations      //
        // A caller who explicitly writes Result="" or Result="   " onto a      //
        // TIME_SERIES / DATA_SET / TABLE observation MUST see the value        //
        // preserved verbatim - the coerce path is out of scope for these      //
        // representations regardless of what the caller wrote. Exercises the   //
        // switch default: return false; branch of                              //
        // ShouldCoerceEmptyResultToUnavailable via IsEmptyResult == true.      //
        // -------------------------------------------------------------------- //

        /// <summary>A TIME_SERIES SAMPLE with an explicit empty-string Result key MUST preserve the payload: IsEmptyResult fires but the classifier short-circuits to String so the coerce does not.</summary>
        [Test]
        public void Sample_TimeSeries_Explicit_EmptyResult_Preserved()
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            var dataItem = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.TIME_SERIES,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedSamples = new[] { 4.0, 5.0, 6.0 };
            var input = new MTConnect.Input.TimeSeriesObservationInput(dataItemKey, expectedSamples, sampleRate: 10.0)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };
            input.SampleCount = expectedSamples.Length;

            // Explicitly write an empty-string Result key on top of the structured payload -
            // simulates a caller (or upstream layer) that stamps ValueKeys.Result unconditionally.
            input.AddValue(ValueKeys.Result, string.Empty);

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "TIME_SERIES SAMPLE with explicit empty Result must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "empty Result on a TIME_SERIES SAMPLE must NOT be coerced to UNAVAILABLE - the non-VALUE short-circuit governs");
            Assert.That(current.GetValue(ValueKeys.SampleCount).ToInt(), Is.EqualTo(expectedSamples.Length),
                "SampleCount MUST survive - IsUnavailable must remain false so the representation switch does not stamp SampleCount=0");
            Assert.That(TimeSeriesObservation.GetSamples(current.Values).ToArray(), Is.EqualTo(expectedSamples),
                "Samples payload MUST survive verbatim");
        }

        /// <summary>A DATA_SET SAMPLE with an explicit whitespace-only Result key MUST preserve the payload.</summary>
        [Test]
        public void Sample_DataSet_Explicit_WhitespaceResult_Preserved()
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            var dataItem = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.DATA_SET,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedEntries = new IDataSetEntry[]
            {
                new DataSetEntry("a", "1.0"),
                new DataSetEntry("b", "2.0"),
                new DataSetEntry("c", "3.0"),
            };
            var input = new MTConnect.Input.DataSetObservationInput(dataItemKey, expectedEntries)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };
            input.AddValue(ValueKeys.Result, "   ");

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "DATA_SET SAMPLE with explicit whitespace Result must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "whitespace Result on a DATA_SET SAMPLE must NOT be coerced to UNAVAILABLE - the non-VALUE short-circuit governs");
            Assert.That(current.GetValue(ValueKeys.Count).ToInt(), Is.EqualTo(expectedEntries.Length),
                "Count MUST survive - IsUnavailable must remain false so the representation switch does not stamp Count=0");
        }

        /// <summary>A TABLE SAMPLE with an explicit empty-string Result key MUST preserve the payload.</summary>
        [Test]
        public void Sample_Table_Explicit_EmptyResult_Preserved()
        {
            const string dataItemKey = RotaryVelocityDataItem.NameId;
            var dataItem = new RotaryVelocityDataItem(DeviceId, RotaryVelocityDataItem.SubTypes.ACTUAL)
            {
                Representation = DataItemRepresentation.TABLE,
            };
            using var agent = NewAgent(InputValidationLevel.Warning, dataItem: dataItem);

            var expectedEntries = new ITableEntry[]
            {
                new TableEntry("row1", new ITableCell[]
                {
                    new TableCell("col1", "1.0"),
                }),
            };
            var input = new MTConnect.Input.TableObservationInput(dataItemKey, expectedEntries)
            {
                DeviceKey = DeviceKey,
                Timestamp = UnixDateTime.Now,
            };
            input.AddValue(ValueKeys.Result, string.Empty);

            var added = agent.AddObservation(input);

            Assert.That(added, Is.True, "TABLE SAMPLE with explicit empty Result must reach the buffer");
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            Assert.That(current, Is.Not.Null, "observation must be retrievable from the current-observations buffer");
            Assert.That(current!.GetValue(ValueKeys.Result), Is.Not.EqualTo(Observation.Unavailable),
                "empty Result on a TABLE SAMPLE must NOT be coerced to UNAVAILABLE - the non-VALUE short-circuit governs");
            Assert.That(current.GetValue(ValueKeys.Count).ToInt(), Is.EqualTo(expectedEntries.Length),
                "Count MUST survive - IsUnavailable must remain false so the representation switch does not stamp Count=0");
        }


        // -------------------------------------------------------------------- //
        // Helpers                                                              //
        // -------------------------------------------------------------------- //

        private static MTConnectAgentBroker NewAgent(
            InputValidationLevel level,
            IDataItem dataItem,
            bool allowEmptyResultForEnumEvents = false)
        {
            var config = new AgentConfiguration
            {
                InputValidationLevel = level,
                AllowEmptyResultForEnumEvents = allowEmptyResultForEnumEvents,
            };
            var agent = new MTConnectAgentBroker(config);
            agent.Start();

            var device = new Device
            {
                Id = DeviceId,
                Name = DeviceId,
                Uuid = DeviceKey,
            };
            device.AddDataItem(dataItem);

            var added = agent.AddDevice(device);
            Assert.That(added, Is.Not.Null, "AddDevice must succeed for test pre-condition");
            return agent;
        }

        private static object? CurrentResult(IMTConnectAgentBroker agent, string dataItemKey)
        {
            var current = agent.GetCurrentObservations(DeviceKey, dataItemKey).SingleOrDefault();
            return current?.GetValue(ValueKeys.Result);
        }
    }
}
