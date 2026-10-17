// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using MTConnect.Observations;
using MTConnect.Shdr;
using NUnit.Framework;

namespace MTConnect.Tests.Shdr
{
    /// <summary>
    /// Pins SHDR line parsing and formatting: quoted values containing '|', DataSet / Table entry quoting and
    /// round-trips, anchored reset triggers, durations and timestamps on every type, culture-independent numbers,
    /// strict time series samples, and the message / condition / asset line forms.
    /// </summary>
    [TestFixture]
    [Category("ShdrParsing")]
    public class ShdrParsingTests
    {
        private const string Timestamp = "2024-01-01T00:00:00.0000000Z";


        /// <summary>
        /// Pins the behaviour expressed by the test name: data item value with pipe round trips.
        /// </summary>
        [Test]
        public void DataItem_ValueWithPipe_RoundTrips()
        {
            var line = new ShdrDataItem("programHeader", "Program Author | Patrick").ToString();

            var parsed = ShdrDataItem.FromString(line).Single();

            Assert.That(parsed.DataItemKey, Is.EqualTo("programHeader"));
            Assert.That(parsed.CDATA, Is.EqualTo("Program Author | Patrick"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data item quoted value with pipe in multi item line is one value.
        /// </summary>
        [Test]
        public void DataItem_QuotedValueWithPipe_InMultiItemLine_IsOneValue()
        {
            var items = ShdrDataItem.FromString($"{Timestamp}|a|1|header|\"x \\| y\"|b|2").ToList();

            Assert.That(items.Select(o => o.DataItemKey), Is.EqualTo(new[] { "a", "header", "b" }));
            Assert.That(items[1].CDATA, Is.EqualTo("x | y"));
            Assert.That(items[2].CDATA, Is.EqualTo("2"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data item value that only starts with quote is plain text.
        /// </summary>
        [Test]
        public void DataItem_ValueThatOnlyStartsWithQuote_IsPlainText()
        {
            var item = ShdrDataItem.FromString("|comment|\"Tool broken\" detected").Single();

            Assert.That(item.CDATA, Is.EqualTo("\"Tool broken\" detected"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data item reset triggered suffix is parsed.
        /// </summary>
        [Test]
        public void DataItem_ResetTriggeredSuffix_IsParsed()
        {
            var item = ShdrDataItem.FromString("|pcount|100:DAY").Single();

            Assert.That(item.CDATA, Is.EqualTo("100"));
            Assert.That(item.ResetTriggered, Is.EqualTo(ResetTriggered.DAY));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data item text containing colon is not treated as reset trigger.
        /// </summary>
        [TestCase("(MSG:CHECK TOOL)")]
        [TestCase("10:30")]
        [TestCase("JOB:NOT_A_RESET")]
        public void DataItem_TextContainingColon_IsNotTreatedAsResetTrigger(string value)
        {
            var item = ShdrDataItem.FromString($"|program|{value}").Single();

            Assert.That(item.CDATA, Is.EqualTo(value));
            Assert.That(item.ResetTriggered, Is.EqualTo(ResetTriggered.NOT_SPECIFIED));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data item duration parses in any culture.
        /// </summary>
        [Test]
        public void DataItem_DurationParses_InAnyCulture()
        {
            WithCulture("de-DE", () =>
            {
                var item = ShdrDataItem.FromString($"{Timestamp}@1.5|load|12.5").Single();
                Assert.That(item.Duration, Is.EqualTo(1.5));

                var line = new ShdrDataItem("load", "12.5") { Duration = 1.5 }.ToString();
                Assert.That(line, Does.StartWith("@1.5|"));
            });
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: data set quoted and braced entries are parsed.
        /// </summary>
        [Test]
        public void DataSet_QuotedAndBracedEntries_AreParsed()
        {
            var dataSet = ShdrDataSet.FromString($"{Timestamp}|vars|k='a b' z=1 q=\"x y\" r={{c=\"a}}b\"}} e= f");

            var entries = dataSet.Entries.ToDictionary(o => o.Key);
            Assert.That(entries["k"].Value, Is.EqualTo("a b"));
            Assert.That(entries["z"].Value, Is.EqualTo("1"));
            Assert.That(entries["q"].Value, Is.EqualTo("x y"));
            Assert.That(entries["r"].Value, Is.EqualTo("{c=\"a}b\"}"));
            Assert.That(entries["e"].Removed, Is.True);
            Assert.That(entries["f"].Removed, Is.True);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data set values with spaces and empty values round trip.
        /// </summary>
        [Test]
        public void DataSet_ValuesWithSpacesAndEmptyValues_RoundTrip()
        {
            var original = new ShdrDataSet("vars", new IDataSetEntry[]
            {
                new DataSetEntry("greeting", "hello world"),
                new DataSetEntry("blank", ""),
                new DataSetEntry("quote", "say \"hi\""),
                new ShdrDataSetEntry("gone", null, true)
            });

            var parsed = ShdrDataSet.FromString(original.ToString());

            var entries = parsed.Entries.ToDictionary(o => o.Key);
            Assert.That(entries["greeting"].Value, Is.EqualTo("hello world"));
            Assert.That(entries["blank"].Removed, Is.False, "an empty value must not read back as a removal");
            Assert.That(entries["blank"].Value, Is.EqualTo(""));
            Assert.That(entries["quote"].Value, Is.EqualTo("say \"hi\""));
            Assert.That(entries["gone"].Removed, Is.True);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data set reset triggered prefix is parsed.
        /// </summary>
        [Test]
        public void DataSet_ResetTriggeredPrefix_IsParsed()
        {
            var dataSet = ShdrDataSet.FromString("|vars|:MANUAL a=1 b=2");

            Assert.That(dataSet.ResetTriggered, Is.EqualTo(ResetTriggered.MANUAL));
            Assert.That(dataSet.Entries.Select(o => o.Key), Is.EqualTo(new[] { "a", "b" }));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: data set duration is kept.
        /// </summary>
        [Test]
        public void DataSet_Duration_IsKept()
        {
            var dataSet = ShdrDataSet.FromString($"{Timestamp}@2.5|vars|a=1");

            Assert.That(dataSet.Duration, Is.EqualTo(2.5));
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: table quoted cells are unquoted.
        /// </summary>
        [Test]
        public void Table_QuotedCells_AreUnquoted()
        {
            var table = ShdrTable.FromString($"{Timestamp}|tbl|r1={{c1=\"a b\" c2=2}} r2=");

            var r1 = table.Entries.Single(o => o.Key == "r1");
            var cells = r1.Cells.ToDictionary(o => o.Key, o => o.Value);
            Assert.That(cells["c1"], Is.EqualTo("a b"));
            Assert.That(cells["c2"], Is.EqualTo("2"));
            Assert.That(table.Entries.Single(o => o.Key == "r2").Removed, Is.True);
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: table cells with spaces round trip.
        /// </summary>
        [Test]
        public void Table_CellsWithSpaces_RoundTrip()
        {
            var original = new ShdrTable("tbl", new ITableEntry[]
            {
                new TableEntry("r1", new ITableCell[] { new TableCell("name", "Tool 1"), new TableCell("len", "10.5") })
            });

            var parsed = ShdrTable.FromString(original.ToString());

            var cells = parsed.Entries.Single().Cells.ToDictionary(o => o.Key, o => o.Value);
            Assert.That(cells["name"], Is.EqualTo("Tool 1"));
            Assert.That(cells["len"], Is.EqualTo("10.5"));
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: time series parses in any culture and ignores extra spaces.
        /// </summary>
        [Test]
        public void TimeSeries_ParsesInAnyCulture_AndIgnoresExtraSpaces()
        {
            WithCulture("de-DE", () =>
            {
                var timeSeries = ShdrTimeSeries.FromString($"{Timestamp}|ts|3|100.5|1.5  2.5 3.5 ");

                Assert.That(timeSeries.SampleRate, Is.EqualTo(100.5));
                Assert.That(timeSeries.Samples, Is.EqualTo(new[] { 1.5, 2.5, 3.5 }));
                Assert.That(timeSeries.ToString(), Does.Contain("|100.5|1.5 2.5 3.5"));
            });
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: time series non numeric sample is rejected.
        /// </summary>
        [Test]
        public void TimeSeries_NonNumericSample_IsRejected()
        {
            Assert.That(ShdrTimeSeries.FromString($"{Timestamp}|ts|2|100|1 x"), Is.Null);
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: condition with duration is not dropped.
        /// </summary>
        [Test]
        public void Condition_WithDuration_IsNotDropped()
        {
            var condition = ShdrFaultState.FromString($"{Timestamp}@1.5|cond|FAULT|E1|2|HIGH|Overheat");

            Assert.That(condition, Is.Not.Null);
            Assert.That(condition.Level, Is.EqualTo(ConditionLevel.FAULT));
            Assert.That(condition.Duration, Is.EqualTo(1.5));
            Assert.That(condition.Message, Is.EqualTo("Overheat"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: condition lowercase level is accepted.
        /// </summary>
        [Test]
        public void Condition_LowercaseLevel_IsAccepted()
        {
            var condition = ShdrFaultState.FromString("|cond|normal||||");

            Assert.That(condition, Is.Not.Null);
            Assert.That(condition.Level, Is.EqualTo(ConditionLevel.NORMAL));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: condition message with pipe round trips.
        /// </summary>
        [Test]
        public void Condition_MessageWithPipe_RoundTrips()
        {
            var line = new ShdrFaultState(ConditionLevel.WARNING, "Low | coolant", "W1") { DataItemKey = "cond" }.ToString();

            var parsed = ShdrFaultState.FromString(line);

            Assert.That(parsed.Message, Is.EqualTo("Low | coolant"));
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: message without timestamp keeps native code field.
        /// </summary>
        [Test]
        public void Message_WithoutTimestamp_KeepsNativeCodeField()
        {
            var line = new ShdrMessage("msg", "hello", "CODE1").ToString();

            var parsed = ShdrMessage.FromString(line);

            Assert.That(parsed.NativeCode, Is.EqualTo("CODE1"));
            Assert.That(parsed.CDATA, Is.EqualTo("hello"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: message quoted text with pipe is one value.
        /// </summary>
        [Test]
        public void Message_QuotedTextWithPipe_IsOneValue()
        {
            var parsed = ShdrMessage.FromString($"{Timestamp}|msg|CODE|\"a\\|b\"");

            Assert.That(parsed.CDATA, Is.EqualTo("a|b"));
        }


        /// <summary>
        /// Pins the behaviour expressed by the test name: asset with empty timestamp is recognised.
        /// </summary>
        [Test]
        public void Asset_WithEmptyTimestamp_IsRecognised()
        {
            const string line = "|@ASSET@|A1|File|<File assetId=\"A1\" />";

            Assert.That(ShdrAsset.IsAssetLine(line), Is.True);

            var asset = ShdrAsset.FromString(line);
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset.AssetId, Is.EqualTo("A1"));
            Assert.That(asset.AssetType, Is.EqualTo("File"));
        }

        /// <summary>
        /// Pins the behaviour expressed by the test name: multiline asset header with and without timestamp is recognised.
        /// </summary>
        [Test]
        public void MultilineAssetHeader_WithAndWithoutTimestamp_IsRecognised()
        {
            foreach (var line in new[] { $"{Timestamp}|@ASSET@|A1|File|--multiline--ABCD", "@ASSET@|A1|File|--multiline--ABCD", "|@ASSET@|A1|File|--multiline--ABCD" })
            {
                Assert.That(ShdrAsset.IsAssetMultilineBegin(line), Is.True, line);
                Assert.That(ShdrAsset.ReadAssetId(line), Is.EqualTo("A1"), line);
                Assert.That(ShdrAsset.ReadAssetMultilineId(line), Is.EqualTo("ABCD"), line);
            }
        }


        private static void WithCulture(string name, Action action)
        {
            var previous = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(name);
            try
            {
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previous;
            }
        }
    }
}
