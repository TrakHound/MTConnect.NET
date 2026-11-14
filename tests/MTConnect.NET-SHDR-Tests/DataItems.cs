// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using NUnit.Framework;
using MTConnect.Shdr;
using MTConnect.Observations.Events;
using System.Collections.Generic;

namespace MTConnect.Tests.Shdr
{
    /// <summary>Represents the data item format.</summary>
    public class DataItemFormat
    {
        private const long BeforeY2KUnix = 9466847990000000;
        private const string BeforeY2K = "1999-12-31T23:59:59.0000000Z";
        private const long AfterY2KUnix = 9466848000000000;
        private const string AfterY2K = "2000-01-01T00:00:00.0000000Z";

        private static string CheckSingleFormatOutput = "|avail|AVAILABLE";
        private static string CheckSingleTimestampFormatOutput = $"{BeforeY2K}|avail|AVAILABLE";
        private static string CheckSingleQuotesFormatOutput = "|programComment|The subprogram \'404\' is not found";
        private static string CheckSingleDoubleQuotesFormatOutput = "|programComment|The subprogram \"404\" is not found";
        // A value containing '|' is quoted with the pipe escaped, the form the MTConnect reference agent (cppagent) reads back
        private static string CheckSinglePipeFormatOutput = "|programHeader|\"Program Author \\| Patrick\"";
        private static string CheckSingleEmptyFormatOutput = "|program|";
        private static string CheckSingleTrimFormatOutput = "|program|TEST.NC";
        private static string CheckSingleDurationFormatOutput = "@100|pcount|0.12345";
        private static string CheckSingleDurationTimestampFormatOutput = $"{BeforeY2K}@100|pcount|0.12345";
        private static string CheckSingleResetTriggeredFormatOutput = "|pcount|0.12345:DAY";
        private static string CheckSingleResetTriggeredTimestampFormatOutput = $"{BeforeY2K}|pcount|0.12345:DAY";
        private static string CheckSingleResetTriggeredDurationFormatOutput = "@100|pcount|0.12345:DAY";
        private static string CheckSingleResetTriggeredDurationTimestampFormatOutput = $"{BeforeY2K}@100|pcount|0.12345:DAY";

        private static string CheckMultipleFormatOutput = "|avail|AVAILABLE|load|15.1234567|execution|READY|position|0.1267498";
        private static string CheckMultipleTimestampFormatOutput = $"{BeforeY2K}|avail|AVAILABLE|load|15.1234567|execution|READY|position|0.1267498";
        private static string CheckMultipleTimestampDifferentFormatOutput = $"{BeforeY2K}|load|15.1234567\r\n{AfterY2K}|avail|AVAILABLE|execution|READY|position|0.1267498";
        private static string CheckMultipleQuotesFormatOutput = "|avail|AVAILABLE|load|15.1234567|programComment|The subprogram \'404\' is not found|position|0.1267498";
        private static string CheckMultipleDoubleQuotesFormatOutput = "|avail|AVAILABLE|load|15.1234567|programComment|The subprogram \"404\" is not found|position|0.1267498";
        private static string CheckMultiplePipeFormatOutput = "|avail|AVAILABLE|load|15.1234567|programHeader|\"Program Author \\| Patrick\"|position|0.1267498";
        private static string CheckMultipleEmptyFormatOutput = "|avail|AVAILABLE|program||execution|READY|position|0.1267498";
        private static string CheckMultipleAllEmptyFormatOutput = "|avail||program||execution||position|";
        private static string CheckMultipleResetTriggeredFormatOutput = "|avail|AVAILABLE|load|15.1234567|pcount|0.12345:DAY|position|0.1267498";
        private static string CheckMultipleResetTriggeredTimestampFormatOutput = $"{BeforeY2K}|avail|AVAILABLE|load|15.1234567|pcount|0.12345:DAY|position|0.1267498";


        #region "Single"

        /// <summary>Pins the behaviour expressed by the test name: format check single.</summary>
        [Test]
        public void FormatCheckSingle()
        {
            var dataItem = new ShdrDataItem("avail", Availability.AVAILABLE);

            var actual = dataItem.ToString();
            var expected = CheckSingleFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single timestamp.</summary>
        [Test]
        public void FormatCheckSingleTimestamp()
        {
            var dataItem = new ShdrDataItem("avail", Availability.AVAILABLE, BeforeY2KUnix);

            var actual = dataItem.ToString();
            var expected = CheckSingleTimestampFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check single quotes.</summary>
        [Test]
        public void FormatCheckSingleQuotes()
        {
            var dataItem = new ShdrDataItem("programComment", "The subprogram \'404\' is not found");

            var actual = dataItem.ToString();
            var expected = CheckSingleQuotesFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single double quotes.</summary>
        [Test]
        public void FormatCheckSingleDoubleQuotes()
        {
            var dataItem = new ShdrDataItem("programComment", "The subprogram \"404\" is not found");

            var actual = dataItem.ToString();
            var expected = CheckSingleDoubleQuotesFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single pipe.</summary>
        [Test]
        public void FormatCheckSinglePipe()
        {
            var dataItem = new ShdrDataItem("programHeader", "Program Author | Patrick");

            var actual = dataItem.ToString();
            var expected = CheckSinglePipeFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single null.</summary>
        [Test]
        public void FormatCheckSingleNull()
        {
            var dataItem = new ShdrDataItem("program", null);

            var actual = dataItem.ToString();
            var expected = CheckSingleEmptyFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single empty.</summary>
        [Test]
        public void FormatCheckSingleEmpty()
        {
            var dataItem = new ShdrDataItem("program", "");

            var actual = dataItem.ToString();
            var expected = CheckSingleEmptyFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single whitespace.</summary>
        [Test]
        public void FormatCheckSingleWhitespace()
        {
            var dataItem = new ShdrDataItem("program", "      ");

            var actual = dataItem.ToString();
            var expected = CheckSingleEmptyFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single leading whitespace.</summary>
        [Test]
        public void FormatCheckSingleLeadingWhitespace()
        {
            var dataItem = new ShdrDataItem("program", "   TEST.NC");

            var actual = dataItem.ToString();
            var expected = CheckSingleTrimFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single trailing whitespace.</summary>
        [Test]
        public void FormatCheckSingleTrailingWhitespace()
        {
            var dataItem = new ShdrDataItem("program", "TEST.NC     ");

            var actual = dataItem.ToString();
            var expected = CheckSingleTrimFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check single duration.</summary>
        [Test]
        public void FormatCheckSingleDuration()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345);
            dataItem.Duration = 100;

            var actual = dataItem.ToString();
            var expected = CheckSingleDurationFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single duration timestamp.</summary>
        [Test]
        public void FormatCheckSingleDurationTimestamp()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345, BeforeY2KUnix);
            dataItem.Duration = 100;

            var actual = dataItem.ToString();
            var expected = CheckSingleDurationTimestampFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check single reset triggered.</summary>
        [Test]
        public void FormatCheckSingleResetTriggered()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345);
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var actual = dataItem.ToString();
            var expected = CheckSingleResetTriggeredFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single reset triggered timestamp.</summary>
        [Test]
        public void FormatCheckSingleResetTriggeredTimestamp()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345, BeforeY2KUnix);
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var actual = dataItem.ToString();
            var expected = CheckSingleResetTriggeredTimestampFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check single reset triggered duration.</summary>
        [Test]
        public void FormatCheckSingleResetTriggeredDuration()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345);
            dataItem.Duration = 100;
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var actual = dataItem.ToString();
            var expected = CheckSingleResetTriggeredDurationFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check single reset triggered duration timestamp.</summary>
        [Test]
        public void FormatCheckSingleResetTriggeredDurationTimestamp()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345, BeforeY2KUnix);
            dataItem.Duration = 100;
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var actual = dataItem.ToString();
            var expected = CheckSingleResetTriggeredDurationTimestampFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion

        #region "Multiple"

        /// <summary>Pins the behaviour expressed by the test name: format check multiple.</summary>
        [Test]
        public void FormatCheckMultiple()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check multiple timestamps.</summary>
        [Test]
        public void FormatCheckMultipleTimestamps()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("load", 15.1234567, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("position", 0.1267498, BeforeY2KUnix));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleTimestampFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check multiple different timestamps.</summary>
        [Test]
        public void FormatCheckMultipleDifferentTimestamps()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE, AfterY2KUnix));
            dataItems.Add(new ShdrDataItem("load", 15.1234567, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY, AfterY2KUnix));
            dataItems.Add(new ShdrDataItem("position", 0.1267498, AfterY2KUnix));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleTimestampDifferentFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check multiple quotes.</summary>
        [Test]
        public void FormatCheckMultipleQuotes()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("programComment", "The subprogram \'404\' is not found"));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleQuotesFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check multiple double quotes.</summary>
        [Test]
        public void FormatCheckMultipleDoubleQuotes()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("programComment", "The subprogram \"404\" is not found"));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleDoubleQuotesFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check multiple pipe.</summary>
        [Test]
        public void FormatCheckMultiplePipe()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("programHeader", "Program Author | Patrick"));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultiplePipeFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check multiple null.</summary>
        [Test]
        public void FormatCheckMultipleNull()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("program", null));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleEmptyFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check multiple empty.</summary>
        [Test]
        public void FormatCheckMultipleEmpty()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("program", ""));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleEmptyFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check multiple empty null.</summary>
        [Test]
        public void FormatCheckMultipleEmptyNull()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", null));
            dataItems.Add(new ShdrDataItem("program", null));
            dataItems.Add(new ShdrDataItem("execution", null));
            dataItems.Add(new ShdrDataItem("position", null));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleAllEmptyFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>Pins the behaviour expressed by the test name: format check multiple empty all.</summary>
        [Test]
        public void FormatCheckMultipleEmptyAll()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", ""));
            dataItems.Add(new ShdrDataItem("program", ""));
            dataItems.Add(new ShdrDataItem("execution", ""));
            dataItems.Add(new ShdrDataItem("position", ""));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleAllEmptyFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }


        /// <summary>Pins the behaviour expressed by the test name: format check multiple reset triggered.</summary>
        [Test]
        public void FormatCheckMultipleResetTriggered()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("pcount", 0.12345) { ResetTriggered = Observations.ResetTriggered.DAY });
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var actual = ShdrDataItem.ToString(dataItems);
            var expected = CheckMultipleResetTriggeredFormatOutput;

            Assert.That(actual, Is.EqualTo(expected));
        }

        #endregion
    }
}