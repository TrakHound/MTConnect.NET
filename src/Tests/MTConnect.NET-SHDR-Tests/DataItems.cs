// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using NUnit.Framework;
using MTConnect.Adapters.Shdr;
using MTConnect.Observations.Events.Values;
using System.Collections.Generic;

namespace MTConnect.Tests.Shdr
{
    public class DataItemFormat
    {
        private const long BeforeY2KUnix = 9466847990000000;
        private const string BeforeY2K = "1999-12-31T23:59:59.0000000Z";
        private const long AfterY2KUnix = 9466848000000000;
        private const string AfterY2K = "2000-01-01T00:00:00.0000000Z";

        private static string CheckSingleFormatOutput = "avail|AVAILABLE";
        private static string CheckSingleTimestampFormatOutput = $"{BeforeY2K}|avail|AVAILABLE";
        private static string CheckSingleQuotesFormatOutput = "programComment|The subprogram \'404\' is not found";
        private static string CheckSingleDoubleQuotesFormatOutput = "programComment|The subprogram \"404\" is not found";
        private static string CheckSinglePipeFormatOutput = "programHeader|Program Author \\| Patrick";
        private static string CheckSingleEmptyFormatOutput = "program|";
        private static string CheckSingleTrimFormatOutput = "program|TEST.NC";
        private static string CheckSingleDurationFormatOutput = "@100|pcount|0.12345";
        private static string CheckSingleDurationTimestampFormatOutput = $"{BeforeY2K}@100|pcount|0.12345";
        private static string CheckSingleResetTriggeredFormatOutput = "pcount|0.12345:DAY";
        private static string CheckSingleResetTriggeredTimestampFormatOutput = $"{BeforeY2K}|pcount|0.12345:DAY";
        private static string CheckSingleResetTriggeredDurationFormatOutput = "@100|pcount|0.12345:DAY";
        private static string CheckSingleResetTriggeredDurationTimestampFormatOutput = $"{BeforeY2K}@100|pcount|0.12345:DAY";

        private static string CheckMultipleFormatOutput = "avail|AVAILABLE|load|15.1234567|execution|READY|position|0.1267498";
        private static string CheckMultipleTimestampFormatOutput = $"{BeforeY2K}|avail|AVAILABLE|load|15.1234567|execution|READY|position|0.1267498";
        private static string CheckMultipleTimestampDifferentFormatOutput = $"{BeforeY2K}|load|15.1234567\r\n{AfterY2K}|avail|AVAILABLE|execution|READY|position|0.1267498";
        private static string CheckMultipleQuotesFormatOutput = "avail|AVAILABLE|load|15.1234567|programComment|The subprogram \'404\' is not found|position|0.1267498";
        private static string CheckMultipleDoubleQuotesFormatOutput = "avail|AVAILABLE|load|15.1234567|programComment|The subprogram \"404\" is not found|position|0.1267498";
        private static string CheckMultiplePipeFormatOutput = "avail|AVAILABLE|load|15.1234567|programHeader|Program Author \\| Patrick|position|0.1267498";
        private static string CheckMultipleEmptyFormatOutput = "avail|AVAILABLE|program||execution|READY|position|0.1267498";
        private static string CheckMultipleAllEmptyFormatOutput = "avail||program||execution||position|";
        private static string CheckMultipleResetTriggeredFormatOutput = "avail|AVAILABLE|load|15.1234567|pcount|0.12345:DAY|position|0.1267498";
        private static string CheckMultipleResetTriggeredTimestampFormatOutput = $"{BeforeY2K}|avail|AVAILABLE|load|15.1234567|pcount|0.12345:DAY|position|0.1267498";


        #region "Single"

        [Test]
        public void FormatCheckSingle()
        {
            var dataItem = new ShdrDataItem("avail", Availability.AVAILABLE);

            var input = dataItem.ToString();
            var output = CheckSingleFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleTimestamp()
        {
            var dataItem = new ShdrDataItem("avail", Availability.AVAILABLE, BeforeY2KUnix);

            var input = dataItem.ToString();
            var output = CheckSingleTimestampFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckSingleQuotes()
        {
            var dataItem = new ShdrDataItem("programComment", "The subprogram \'404\' is not found");

            var input = dataItem.ToString();
            var output = CheckSingleQuotesFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleDoubleQuotes()
        {
            var dataItem = new ShdrDataItem("programComment", "The subprogram \"404\" is not found");

            var input = dataItem.ToString();
            var output = CheckSingleDoubleQuotesFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSinglePipe()
        {
            var dataItem = new ShdrDataItem("programHeader", "Program Author | Patrick");

            var input = dataItem.ToString();
            var output = CheckSinglePipeFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleNull()
        {
            var dataItem = new ShdrDataItem("program", null);

            var input = dataItem.ToString();
            var output = CheckSingleEmptyFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleEmpty()
        {
            var dataItem = new ShdrDataItem("program", "");

            var input = dataItem.ToString();
            var output = CheckSingleEmptyFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleWhitespace()
        {
            var dataItem = new ShdrDataItem("program", "      ");

            var input = dataItem.ToString();
            var output = CheckSingleEmptyFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleLeadingWhitespace()
        {
            var dataItem = new ShdrDataItem("program", "   TEST.NC");

            var input = dataItem.ToString();
            var output = CheckSingleTrimFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleTrailingWhitespace()
        {
            var dataItem = new ShdrDataItem("program", "TEST.NC     ");

            var input = dataItem.ToString();
            var output = CheckSingleTrimFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckSingleDuration()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345);
            dataItem.Duration = 100;

            var input = dataItem.ToString();
            var output = CheckSingleDurationFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleDurationTimestamp()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345, BeforeY2KUnix);
            dataItem.Duration = 100;

            var input = dataItem.ToString();
            var output = CheckSingleDurationTimestampFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckSingleResetTriggered()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345);
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var input = dataItem.ToString();
            var output = CheckSingleResetTriggeredFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleResetTriggeredTimestamp()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345, BeforeY2KUnix);
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var input = dataItem.ToString();
            var output = CheckSingleResetTriggeredTimestampFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckSingleResetTriggeredDuration()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345);
            dataItem.Duration = 100;
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var input = dataItem.ToString();
            var output = CheckSingleResetTriggeredDurationFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckSingleResetTriggeredDurationTimestamp()
        {
            var dataItem = new ShdrDataItem("pcount", 0.12345, BeforeY2KUnix);
            dataItem.Duration = 100;
            dataItem.ResetTriggered = Observations.ResetTriggered.DAY;

            var input = dataItem.ToString();
            var output = CheckSingleResetTriggeredDurationTimestampFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        #endregion

        #region "Multiple"

        [Test]
        public void FormatCheckMultiple()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckMultipleTimestamps()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("load", 15.1234567, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("position", 0.1267498, BeforeY2KUnix));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleTimestampFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckMultipleDifferentTimestamps()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE, AfterY2KUnix));
            dataItems.Add(new ShdrDataItem("load", 15.1234567, BeforeY2KUnix));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY, AfterY2KUnix));
            dataItems.Add(new ShdrDataItem("position", 0.1267498, AfterY2KUnix));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleTimestampDifferentFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckMultipleQuotes()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("programComment", "The subprogram \'404\' is not found"));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleQuotesFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckMultipleDoubleQuotes()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("programComment", "The subprogram \"404\" is not found"));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleDoubleQuotesFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckMultiplePipe()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("programHeader", "Program Author | Patrick"));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultiplePipeFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckMultipleNull()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("program", null));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleEmptyFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckMultipleEmpty()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("program", ""));
            dataItems.Add(new ShdrDataItem("execution", Execution.READY));
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleEmptyFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckMultipleEmptyNull()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", null));
            dataItems.Add(new ShdrDataItem("program", null));
            dataItems.Add(new ShdrDataItem("execution", null));
            dataItems.Add(new ShdrDataItem("position", null));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleAllEmptyFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        [Test]
        public void FormatCheckMultipleEmptyAll()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", ""));
            dataItems.Add(new ShdrDataItem("program", ""));
            dataItems.Add(new ShdrDataItem("execution", ""));
            dataItems.Add(new ShdrDataItem("position", ""));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleAllEmptyFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }


        [Test]
        public void FormatCheckMultipleResetTriggered()
        {
            var dataItems = new List<ShdrDataItem>();
            dataItems.Add(new ShdrDataItem("avail", Availability.AVAILABLE));
            dataItems.Add(new ShdrDataItem("load", 15.1234567));
            dataItems.Add(new ShdrDataItem("pcount", 0.12345) { ResetTriggered = Observations.ResetTriggered.DAY });
            dataItems.Add(new ShdrDataItem("position", 0.1267498));

            var input = ShdrDataItem.ToString(dataItems);
            var output = CheckMultipleResetTriggeredFormatOutput;

            if (input == output)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail($"\"{input}\" is NOT EQUAL to \"{output}\"");
            }
        }

        #endregion
    }
}